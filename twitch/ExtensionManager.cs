using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x020017EB RID: 6123
	public class ExtensionManager
	{
		// Token: 0x0600BDBA RID: 48570 RVA: 0x004659D0 File Offset: 0x00463BD0
		public void Init()
		{
			this.extensionStateManager = new ExtensionStateManager();
			this.extensionCommandPoller = new ExtensionCommandPoller();
			this.extensionStateManager.Init();
			this.extensionCommandPoller.Init();
		}

		// Token: 0x0600BDBB RID: 48571 RVA: 0x004659FE File Offset: 0x00463BFE
		public void OnPartyChanged()
		{
			ExtensionStateManager extensionStateManager = this.extensionStateManager;
			if (extensionStateManager == null)
			{
				return;
			}
			extensionStateManager.OnPartyChanged();
		}

		// Token: 0x0600BDBC RID: 48572 RVA: 0x00465A10 File Offset: 0x00463C10
		public void TwitchEnabledChanged(EntityPlayer _ep)
		{
			EntityPlayerLocal localPlayer = TwitchManager.Current.LocalPlayer;
			if (_ep != localPlayer && localPlayer.Party != null && localPlayer.Party.ContainsMember(_ep))
			{
				this.extensionStateManager.OnPartyChanged();
			}
		}

		// Token: 0x0600BDBD RID: 48573 RVA: 0x00465A52 File Offset: 0x00463C52
		public void PushUserBalance(ValueTuple<string, int> userBalance)
		{
			ExtensionStateManager extensionStateManager = this.extensionStateManager;
			if (extensionStateManager == null)
			{
				return;
			}
			extensionStateManager.PushUserBalance(userBalance);
		}

		// Token: 0x0600BDBE RID: 48574 RVA: 0x00465A65 File Offset: 0x00463C65
		public void PushViewerChatState(string id, bool hasChatted)
		{
			ExtensionStateManager extensionStateManager = this.extensionStateManager;
			if (extensionStateManager == null)
			{
				return;
			}
			extensionStateManager.PushViewerChatState(id, hasChatted);
		}

		// Token: 0x0600BDBF RID: 48575 RVA: 0x00465A79 File Offset: 0x00463C79
		public bool CanUseBitCommands()
		{
			return this.extensionStateManager.CanUseBitCommands();
		}

		// Token: 0x0600BDC0 RID: 48576 RVA: 0x00465A86 File Offset: 0x00463C86
		public void Update()
		{
			this.extensionStateManager.Update();
			this.extensionCommandPoller.Update();
		}

		// Token: 0x0600BDC1 RID: 48577 RVA: 0x00465A9E File Offset: 0x00463C9E
		public bool HasCommand()
		{
			return this.extensionCommandPoller.HasCommand();
		}

		// Token: 0x0600BDC2 RID: 48578 RVA: 0x00465AAB File Offset: 0x00463CAB
		public ExtensionAction GetCommand()
		{
			return this.extensionCommandPoller.GetCommand();
		}

		// Token: 0x0600BDC3 RID: 48579 RVA: 0x00465AB8 File Offset: 0x00463CB8
		public void RetrieveJWT()
		{
			this.extensionStateManager.RetrieveJWT();
		}

		// Token: 0x0600BDC4 RID: 48580 RVA: 0x00465AC5 File Offset: 0x00463CC5
		public void Cleanup()
		{
			this.extensionCommandPoller.Cleanup();
			this.extensionStateManager.Cleanup();
			this.extensionCommandPoller = null;
			this.extensionStateManager = null;
		}

		// Token: 0x0600BDC5 RID: 48581 RVA: 0x00465AEB File Offset: 0x00463CEB
		public static void CheckExtensionInstalled(Action<bool> _cb)
		{
			GameManager.Instance.StartCoroutine(ExtensionManager.CheckExtensionInstall(_cb));
		}

		// Token: 0x0600BDC6 RID: 48582 RVA: 0x00465AFE File Offset: 0x00463CFE
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator CheckExtensionInstall(Action<bool> _cb)
		{
			using (UnityWebRequest req = UnityWebRequest.Get("https://api.twitch.tv/helix/users/extensions?user_id=" + TwitchManager.Current.Authentication.userID))
			{
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning("InBeta Check Failed: " + req.downloadHandler.text);
				}
				else
				{
					try
					{
						JObject jobject = JObject.Parse(req.downloadHandler.text);
						foreach (JToken jtoken in jobject["data"]["panel"].ToObject<JObject>().Values())
						{
							JObject jobject2 = jtoken.ToObject<JObject>();
							JToken jtoken2;
							if (jobject2.TryGetValue("id", out jtoken2) && jtoken2.ToString() == "k6ji189bf7i4ge8il4iczzw7kpgmjt" && jobject2["active"].ToString() == bool.TrueString)
							{
								_cb(true);
								yield break;
							}
						}
						foreach (JToken jtoken3 in jobject["data"]["overlay"].ToObject<JObject>().Values())
						{
							JObject jobject3 = jtoken3.ToObject<JObject>();
							JToken jtoken4;
							if (jobject3.TryGetValue("version", out jtoken4))
							{
								ExtensionManager.Version = jtoken4.ToString();
							}
							JToken jtoken5;
							if (jobject3.TryGetValue("id", out jtoken5) && jtoken5.ToString() == "k6ji189bf7i4ge8il4iczzw7kpgmjt" && jobject3["active"].ToString() == bool.TrueString)
							{
								_cb(true);
								yield break;
							}
						}
					}
					catch (Exception)
					{
						Log.Warning("could not read extension check data");
					}
				}
			}
			UnityWebRequest req = null;
			_cb(false);
			yield break;
			yield break;
		}

		// Token: 0x04008E6E RID: 36462
		public const string API_STAGE = "prod";

		// Token: 0x04008E6F RID: 36463
		public const string EXTENSION_ID = "k6ji189bf7i4ge8il4iczzw7kpgmjt";

		// Token: 0x04008E70 RID: 36464
		public static string Version = "2.0.2";

		// Token: 0x04008E71 RID: 36465
		[PublicizedFrom(EAccessModifier.Private)]
		public ExtensionStateManager extensionStateManager;

		// Token: 0x04008E72 RID: 36466
		[PublicizedFrom(EAccessModifier.Private)]
		public ExtensionCommandPoller extensionCommandPoller;
	}
}
