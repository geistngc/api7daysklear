using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x02001831 RID: 6193
	public class TwitchChannelPointEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600BF30 RID: 48944 RVA: 0x0046C032 File Offset: 0x0046A232
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return this.ChannelPointTitle == name;
		}

		// Token: 0x0600BF31 RID: 48945 RVA: 0x0046C040 File Offset: 0x0046A240
		public TwitchChannelPointEventEntry.CreateCustomReward SetupRewardEntry(string channelID)
		{
			return new TwitchChannelPointEventEntry.CreateCustomReward
			{
				broadcaster_id = channelID,
				title = this.ChannelPointTitle,
				cost = this.Cost,
				is_max_per_user_per_stream_enabled = (this.MaxPerUserPerStream > 0),
				max_per_user_per_stream = this.MaxPerUserPerStream,
				is_max_per_stream_enabled = (this.MaxPerStream > 0),
				max_per_stream = this.MaxPerStream,
				is_global_cooldown_enabled = (this.GlobalCooldown > 0),
				global_cooldown_seconds = this.GlobalCooldown
			};
		}

		// Token: 0x0600BF32 RID: 48946 RVA: 0x0046C0C2 File Offset: 0x0046A2C2
		public static IEnumerator CreateCustomRewardPost(TwitchChannelPointEventEntry.CreateCustomReward _rd, Action<string> _onSucess, Action<string> _onFail)
		{
			yield return new WaitUntil(() => TwitchManager.Current.Authentication != null && TwitchManager.Current.Authentication.oauth != "" && TwitchManager.Current.Authentication.userID != "");
			Log.Out("creating Custom reward on: " + TwitchManager.Current.Authentication.userID);
			string uri = "https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id=" + TwitchManager.Current.Authentication.userID;
			string bodyData = JsonUtility.ToJson(_rd);
			using (UnityWebRequest req = UnityWebRequest.Put(uri, bodyData))
			{
				req.method = "POST";
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				req.SetRequestHeader("Content-Type", "application/json");
				yield return req.SendWebRequest();
				if (req.result == UnityWebRequest.Result.Success)
				{
					Log.Out("sucessfully created Custom Channel Points Reward");
					_onSucess(req.downloadHandler.text);
				}
				else
				{
					TwitchChannelPointEventEntry.ErrorResponse errorResponse = JsonConvert.DeserializeObject<TwitchChannelPointEventEntry.ErrorResponse>(req.downloadHandler.text);
					if (errorResponse != null)
					{
						_onFail("response code: " + errorResponse.status + "\nmessage: " + errorResponse.message);
					}
					else
					{
						_onFail("Something went wrong. Please Try again.");
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BF33 RID: 48947 RVA: 0x0046C0DF File Offset: 0x0046A2DF
		public static IEnumerator DeleteCustomRewardsDelete(string id, Action<string> _onSucess, Action<string> _onFail)
		{
			string uri = string.Format("https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={0}&id={1}", TwitchManager.Current.Authentication.userID, id);
			using (UnityWebRequest req = UnityWebRequest.Delete(uri))
			{
				req.method = "DELETE";
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				yield return req.SendWebRequest();
				if (req.result == UnityWebRequest.Result.Success)
				{
					_onSucess("Success");
				}
				else
				{
					Debug.Log(string.Format("response code: {0}", req.responseCode));
					if (req.responseCode == 404L)
					{
						_onSucess("Not Found");
					}
					else
					{
						TwitchChannelPointEventEntry.ErrorResponse errorResponse = JsonConvert.DeserializeObject<TwitchChannelPointEventEntry.ErrorResponse>(req.downloadHandler.text);
						if (errorResponse != null)
						{
							_onFail(errorResponse.message);
						}
						else
						{
							_onFail("Something went wrong. Please Try again.");
						}
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x04008FE8 RID: 36840
		public string ChannelPointTitle = "";

		// Token: 0x04008FE9 RID: 36841
		public int Cost = 1000;

		// Token: 0x04008FEA RID: 36842
		public int MaxPerUserPerStream;

		// Token: 0x04008FEB RID: 36843
		public int MaxPerStream;

		// Token: 0x04008FEC RID: 36844
		public int GlobalCooldown;

		// Token: 0x04008FED RID: 36845
		public string ChannelPointID = "";

		// Token: 0x04008FEE RID: 36846
		public bool AutoCreate = true;

		// Token: 0x02001832 RID: 6194
		[Serializable]
		public class CreateCustomRewards
		{
			// Token: 0x04008FEF RID: 36847
			public List<TwitchChannelPointEventEntry.CreateCustomReward> data = new List<TwitchChannelPointEventEntry.CreateCustomReward>();
		}

		// Token: 0x02001833 RID: 6195
		[Serializable]
		public class CreateCustomReward
		{
			// Token: 0x04008FF0 RID: 36848
			public string broadcaster_id;

			// Token: 0x04008FF1 RID: 36849
			public string title;

			// Token: 0x04008FF2 RID: 36850
			public string background_color = "#F13030";

			// Token: 0x04008FF3 RID: 36851
			public int cost;

			// Token: 0x04008FF4 RID: 36852
			public int max_per_user_per_stream;

			// Token: 0x04008FF5 RID: 36853
			public bool is_max_per_user_per_stream_enabled;

			// Token: 0x04008FF6 RID: 36854
			public int max_per_stream;

			// Token: 0x04008FF7 RID: 36855
			public bool is_max_per_stream_enabled;

			// Token: 0x04008FF8 RID: 36856
			public int global_cooldown_seconds;

			// Token: 0x04008FF9 RID: 36857
			public bool is_global_cooldown_enabled;
		}

		// Token: 0x02001834 RID: 6196
		[Serializable]
		public class CreateCustomRewardResponses
		{
			// Token: 0x04008FFA RID: 36858
			public List<TwitchChannelPointEventEntry.CreateCustomRewardResponse> data;
		}

		// Token: 0x02001835 RID: 6197
		[Serializable]
		public class CreateCustomRewardResponse
		{
			// Token: 0x04008FFB RID: 36859
			public string id;

			// Token: 0x04008FFC RID: 36860
			public string title;
		}

		// Token: 0x02001836 RID: 6198
		[Serializable]
		public class ErrorResponse
		{
			// Token: 0x04008FFD RID: 36861
			public string status;

			// Token: 0x04008FFE RID: 36862
			public string message;
		}
	}
}
