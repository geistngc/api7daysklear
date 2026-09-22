using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x020017C5 RID: 6085
	public class TwitchEntitlementManager : IEntitlementValidator
	{
		// Token: 0x170016F2 RID: 5874
		// (get) Token: 0x0600BD08 RID: 48392 RVA: 0x00462998 File Offset: 0x00460B98
		public string PlatformID
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.prefix + this.owner.User.PlatformUserId.ReadablePlatformUserIdentifier;
			}
		}

		// Token: 0x0600BD09 RID: 48393 RVA: 0x004629BA File Offset: 0x00460BBA
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			if (!TwitchEntitlementManager.platformIdentifierToPrefix.TryGetValue(_owner.PlatformIdentifier, out this.prefix))
			{
				Log.Warning("could not get platform prefix in Twitch Entitlements Manager");
				return;
			}
		}

		// Token: 0x0600BD0A RID: 48394 RVA: 0x004629E8 File Offset: 0x00460BE8
		public void Init()
		{
			TwitchDropAvailabilityManager.Instance.Updated -= this.OnDropsUpdated;
			TwitchDropAvailabilityManager.Instance.Updated += this.OnDropsUpdated;
			TwitchDropAvailabilityManager.Instance.RegisterSource("rfs://drops.xml");
			TwitchDropAvailabilityManager.Instance.UpdateAll(true);
		}

		// Token: 0x0600BD0B RID: 48395 RVA: 0x00462A3C File Offset: 0x00460C3C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDropsUpdated(TwitchDropAvailabilityManager mgr)
		{
			List<TwitchDropAvailabilityManager.TwitchDropEntry> list = new List<TwitchDropAvailabilityManager.TwitchDropEntry>();
			mgr.GetEntries(new List<string>
			{
				"rfs://drops.xml"
			}, list);
			TwitchEntitlementManager.EntitlementSetToTwitchMap.Clear();
			foreach (TwitchDropAvailabilityManager.TwitchDropEntry twitchDropEntry in list)
			{
				if (twitchDropEntry.IsAvailable(DateTime.Now))
				{
					TwitchEntitlementManager.EntitlementSetToTwitchMap.Add(twitchDropEntry.EntitlementSet, twitchDropEntry.BenefitId);
				}
			}
			this.FetchEntitlements();
		}

		// Token: 0x0600BD0C RID: 48396 RVA: 0x00462AD4 File Offset: 0x00460CD4
		public void FetchEntitlements()
		{
			ThreadManager.StartCoroutine(this.FetchEntitlementsCo());
		}

		// Token: 0x0600BD0D RID: 48397 RVA: 0x00462AE2 File Offset: 0x00460CE2
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator FetchEntitlementsCo()
		{
			UnityWebRequest request = UnityWebRequest.Get("https://xjjvn6hovg33dqetux65clszte0vhysi.lambda-url.us-east-2.on.aws/?platform_id=" + this.PlatformID);
			Log.Out("fetching Twitch entitlements for " + this.PlatformID);
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Log.Warning("Failed to fetch Twitch entitlements: " + request.error);
				yield break;
			}
			JArray jarray = (JArray)JObject.Parse(request.downloadHandler.text)["data"];
			TwitchEntitlementManager.entitlements.Clear();
			foreach (JToken jtoken in jarray)
			{
				string id = jtoken.Value<string>("id");
				string benefit_id = jtoken.Value<string>("benefit_id");
				string fulfillment_status = jtoken.Value<string>("fulfillment_status");
				TwitchEntitlementManager.entitlements.Add(new Entitlement
				{
					id = id,
					benefit_id = benefit_id,
					fulfillment_status = fulfillment_status
				});
			}
			yield return this.FulfillEntitlementsCo(null);
			Log.Out("Successfully fetched Twitch entitlements");
			yield break;
		}

		// Token: 0x0600BD0E RID: 48398 RVA: 0x00462AF1 File Offset: 0x00460CF1
		public void FulfillEntitlements(Action onSuccess = null)
		{
			GameManager.Instance.StartCoroutine(this.FulfillEntitlementsCo(onSuccess));
		}

		// Token: 0x0600BD0F RID: 48399 RVA: 0x00462B05 File Offset: 0x00460D05
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator FulfillEntitlementsCo(Action onSuccess)
		{
			List<string> list = new List<string>();
			foreach (Entitlement entitlement in TwitchEntitlementManager.entitlements)
			{
				if (entitlement.fulfillment_status == "CLAIMED")
				{
					list.Add(entitlement.id);
				}
			}
			if (list.Count == 0)
			{
				yield break;
			}
			string s = JsonConvert.SerializeObject(new FulfillmentPayload
			{
				platform_id = this.PlatformID,
				entitlement_ids = list
			});
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			UnityWebRequest request = new UnityWebRequest("https://ev2dltb7u2pdtwuaphayq5icdy0nmtsx.lambda-url.us-east-2.on.aws/?platform_id=" + this.PlatformID, "POST")
			{
				uploadHandler = new UploadHandlerRaw(bytes),
				downloadHandler = new DownloadHandlerBuffer()
			};
			request.SetRequestHeader("Content-Type", "application/json");
			Log.Out("fulfilling Twitch entitlements for " + this.PlatformID);
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Log.Warning("Failed to fulfill twitch entitlements: " + request.downloadHandler.text);
			}
			else
			{
				foreach (JToken jtoken in ((JArray)JObject.Parse(request.downloadHandler.text)["data"]))
				{
					string id = jtoken.Value<string>("id");
					string benefit_id = jtoken.Value<string>("benefit_id");
					string fulfillment_status = jtoken.Value<string>("fulfillment_status");
					Entitlement entitlement2 = TwitchEntitlementManager.entitlements.Find((Entitlement e) => e.id == id);
					if (entitlement2 != null)
					{
						entitlement2.fulfillment_status = fulfillment_status;
						entitlement2.benefit_id = benefit_id;
					}
					else
					{
						TwitchEntitlementManager.entitlements.Add(new Entitlement
						{
							id = id,
							benefit_id = benefit_id,
							fulfillment_status = fulfillment_status
						});
					}
				}
				if (onSuccess != null)
				{
					onSuccess();
				}
			}
			yield break;
		}

		// Token: 0x0600BD10 RID: 48400 RVA: 0x00462B1B File Offset: 0x00460D1B
		public void SerializeEntitlements()
		{
			JsonConvert.SerializeObject(new EntitlementListWrapper
			{
				entitlements = TwitchEntitlementManager.entitlements
			}, Formatting.Indented);
		}

		// Token: 0x0600BD11 RID: 48401 RVA: 0x00462B34 File Offset: 0x00460D34
		public bool IsAvailableOnPlatform(EntitlementSetEnum _set)
		{
			return TwitchEntitlementManager.EntitlementSetToTwitchMap.ContainsKey(_set);
		}

		// Token: 0x0600BD12 RID: 48402 RVA: 0x00462B44 File Offset: 0x00460D44
		public bool HasEntitlement(EntitlementSetEnum _set)
		{
			string id;
			return TwitchEntitlementManager.EntitlementSetToTwitchMap.TryGetValue(_set, out id) && TwitchEntitlementManager.entitlements.Exists((Entitlement e) => e.benefit_id == id);
		}

		// Token: 0x0600BD13 RID: 48403 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsEntitlementPurchasable(EntitlementSetEnum _set)
		{
			return false;
		}

		// Token: 0x0600BD14 RID: 48404 RVA: 0x00462B82 File Offset: 0x00460D82
		public string GetEntitlementSetId(EntitlementSetEnum _entitlementSet)
		{
			return TwitchEntitlementManager.EntitlementSetToTwitchMap.GetValueOrDefault(_entitlementSet);
		}

		// Token: 0x0600BD15 RID: 48405 RVA: 0x00462B90 File Offset: 0x00460D90
		public DateTime? GetAcquiredDate(EntitlementSetEnum _entitlementSet)
		{
			return null;
		}

		// Token: 0x0600BD16 RID: 48406 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool OpenStore(EntitlementSetEnum _set, Action<EntitlementSetEnum> _onDlcPurchased)
		{
			return false;
		}

		// Token: 0x04008DBB RID: 36283
		[PublicizedFrom(EAccessModifier.Private)]
		public const string FETCH_URL = "https://xjjvn6hovg33dqetux65clszte0vhysi.lambda-url.us-east-2.on.aws/?platform_id=";

		// Token: 0x04008DBC RID: 36284
		[PublicizedFrom(EAccessModifier.Private)]
		public const string FULFILLMENT_URL = "https://ev2dltb7u2pdtwuaphayq5icdy0nmtsx.lambda-url.us-east-2.on.aws/?platform_id=";

		// Token: 0x04008DBD RID: 36285
		[PublicizedFrom(EAccessModifier.Private)]
		public static EnumDictionary<EntitlementSetEnum, string> EntitlementSetToTwitchMap = new EnumDictionary<EntitlementSetEnum, string>();

		// Token: 0x04008DBE RID: 36286
		[PublicizedFrom(EAccessModifier.Private)]
		public static EnumDictionary<EPlatformIdentifier, string> platformIdentifierToPrefix = new EnumDictionary<EPlatformIdentifier, string>
		{
			{
				EPlatformIdentifier.PSN,
				"p"
			},
			{
				EPlatformIdentifier.XBL,
				"x"
			},
			{
				EPlatformIdentifier.Steam,
				"s"
			}
		};

		// Token: 0x04008DBF RID: 36287
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly List<Entitlement> entitlements = new List<Entitlement>();

		// Token: 0x04008DC0 RID: 36288
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04008DC1 RID: 36289
		[PublicizedFrom(EAccessModifier.Private)]
		public string prefix;
	}
}
