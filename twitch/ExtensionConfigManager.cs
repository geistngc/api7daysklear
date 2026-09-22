using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniLinq;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x020017D4 RID: 6100
	public class ExtensionConfigManager
	{
		// Token: 0x0600BD55 RID: 48469 RVA: 0x00463A94 File Offset: 0x00461C94
		public void Init()
		{
			TwitchManager.Current.CommandsChanged += this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Combine(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Combine(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
			this.pushConfig();
		}

		// Token: 0x0600BD56 RID: 48470 RVA: 0x00463B13 File Offset: 0x00461D13
		public bool UpdatedConfig()
		{
			if (this.hasUpdated)
			{
				this.hasUpdated = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600BD57 RID: 48471 RVA: 0x00463B28 File Offset: 0x00461D28
		public void Cleanup()
		{
			TwitchManager.Current.CommandsChanged -= this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Remove(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Remove(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
		}

		// Token: 0x0600BD58 RID: 48472 RVA: 0x00463BA1 File Offset: 0x00461DA1
		public void OnPartyChanged()
		{
			this.pushConfig();
		}

		// Token: 0x0600BD59 RID: 48473 RVA: 0x00463BA9 File Offset: 0x00461DA9
		[PublicizedFrom(EAccessModifier.Private)]
		public void pushConfig()
		{
			this.lastPushTime = Time.time;
			if (!this.waitingToPush)
			{
				this.waitingToPush = true;
				GameManager.Instance.StartCoroutine(this.pushConfigAfterTimeout());
			}
		}

		// Token: 0x0600BD5A RID: 48474 RVA: 0x00463BD6 File Offset: 0x00461DD6
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator pushConfigAfterTimeout()
		{
			while (Time.time - this.lastPushTime < 1f)
			{
				yield return null;
			}
			this.waitingToPush = false;
			yield return this.UpdateConfig();
			yield break;
		}

		// Token: 0x0600BD5B RID: 48475 RVA: 0x00463BE5 File Offset: 0x00461DE5
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator UpdateConfig()
		{
			if (this.displayName == string.Empty)
			{
				yield return this.GetDisplayName();
			}
			if (TwitchManager.Current == null || TwitchManager.Current.Authentication == null)
			{
				Log.Warning("attempted to updated config with no Auth object");
				yield break;
			}
			Dictionary<string, List<ExtensionConfigManager.CommandModel>> commands = this.GetCommands();
			List<string> activeCategories = this.GetActiveCategories(commands.Keys.ToList<string>());
			string bodyData = JsonConvert.SerializeObject(new ExtensionConfigManager.ConfigModel
			{
				displayName = TwitchManager.Current.Authentication.userName,
				party = this.GetPlayers(),
				categories = activeCategories,
				commands = commands
			});
			using (UnityWebRequest req = UnityWebRequest.Put("https://2v3d0ewjcg.execute-api.us-east-1.amazonaws.com/prod/broadcaster/config", bodyData))
			{
				req.SetRequestHeader("Authorization", TwitchManager.Current.Authentication.userID + " " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Content-Type", "application/json");
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning(string.Format("Could not update config on backend: {0}", req.result));
				}
				else
				{
					Log.Out("Successfully updated broadcaster config");
					this.hasUpdated = true;
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BD5C RID: 48476 RVA: 0x00463BF4 File Offset: 0x00461DF4
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetDisplayName()
		{
			using (UnityWebRequest req = UnityWebRequest.Get("https://api.twitch.tv/helix/users"))
			{
				req.SetRequestHeader("Content-Type", "application/json");
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning(string.Format("Could not get user data from Twitch: {0}", req.result));
				}
				else
				{
					Log.Out("Successfully retrieved user data from Twitch");
					JObject jobject = JObject.Parse(req.downloadHandler.text);
					this.displayName = jobject["data"][0]["display_name"].ToString();
					this.broadcasterType = jobject["data"][0]["broadcaster_type"].ToString();
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BD5D RID: 48477 RVA: 0x00463C04 File Offset: 0x00461E04
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> GetActiveCategories(List<string> categories)
		{
			return (from c in TwitchActionManager.Current.CategoryList
			select c.Name into name
			where categories.Contains(name)
			select name).ToList<string>();
		}

		// Token: 0x0600BD5E RID: 48478 RVA: 0x00463C64 File Offset: 0x00461E64
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> GetPlayers()
		{
			if (TwitchManager.Current.LocalPlayer != null && TwitchManager.Current.LocalPlayer.Party != null && TwitchManager.Current.LocalPlayer.Party.MemberList != null)
			{
				return (from m in TwitchManager.Current.LocalPlayer.Party.MemberList
				where !(m is EntityPlayerLocal) && !m.TwitchEnabled
				select m into e
				select e.EntityName).ToList<string>();
			}
			return ExtensionConfigManager.emptyParty;
		}

		// Token: 0x0600BD5F RID: 48479 RVA: 0x00463D14 File Offset: 0x00461F14
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, List<ExtensionConfigManager.CommandModel>> GetCommands()
		{
			Dictionary<string, List<ExtensionConfigManager.CommandModel>> dictionary = new Dictionary<string, List<ExtensionConfigManager.CommandModel>>();
			TwitchAction[] array = (from a in TwitchManager.Current.AvailableCommands.Values
			where a.HasExtraConditions() && (this.CanUseBitCommands() || a.PointType != TwitchAction.PointTypes.Bits)
			select a).ToArray<TwitchAction>();
			int num = 0;
			foreach (TwitchAction twitchAction in array)
			{
				ExtensionConfigManager.CommandModel item = new ExtensionConfigManager.CommandModel
				{
					name = twitchAction.Command.Replace("#", string.Empty).Replace("_", " ").ToUpper(),
					baseCommand = twitchAction.BaseCommand,
					command = twitchAction.Command,
					isPositive = twitchAction.IsPositive,
					spends = twitchAction.PointType.ToString(),
					cost = twitchAction.CurrentCost,
					cooldownType = (twitchAction.WaitingBlocked ? "wait" : (twitchAction.CooldownBlocked ? "regular" : "full")),
					cooldownIndex = num / 32,
					bitPosition = (byte)(num % 32),
					streamerOnly = twitchAction.StreamerOnly
				};
				List<ExtensionConfigManager.CommandModel> list;
				if (!dictionary.TryGetValue(twitchAction.MainCategory.Name, out list))
				{
					dictionary.Add(twitchAction.MainCategory.Name, list = new List<ExtensionConfigManager.CommandModel>());
				}
				list.Add(item);
				num++;
			}
			return dictionary;
		}

		// Token: 0x0600BD60 RID: 48480 RVA: 0x00463E76 File Offset: 0x00462076
		public bool CanUseBitCommands()
		{
			return this.broadcasterType == "affiliate" || this.broadcasterType == "partner";
		}

		// Token: 0x04008DED RID: 36333
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<string> emptyParty = new List<string>(0);

		// Token: 0x04008DEE RID: 36334
		[PublicizedFrom(EAccessModifier.Private)]
		public string displayName = string.Empty;

		// Token: 0x04008DEF RID: 36335
		[PublicizedFrom(EAccessModifier.Private)]
		public string broadcasterType = string.Empty;

		// Token: 0x04008DF0 RID: 36336
		[PublicizedFrom(EAccessModifier.Private)]
		public string jwt = string.Empty;

		// Token: 0x04008DF1 RID: 36337
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasUpdated;

		// Token: 0x04008DF2 RID: 36338
		[PublicizedFrom(EAccessModifier.Private)]
		public const float pushConfigTimeout = 1f;

		// Token: 0x04008DF3 RID: 36339
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastPushTime = float.NegativeInfinity;

		// Token: 0x04008DF4 RID: 36340
		[PublicizedFrom(EAccessModifier.Private)]
		public bool waitingToPush;

		// Token: 0x020017D5 RID: 6101
		public class ConfigModel
		{
			// Token: 0x04008DF5 RID: 36341
			public string displayName;

			// Token: 0x04008DF6 RID: 36342
			public List<string> party;

			// Token: 0x04008DF7 RID: 36343
			public List<string> categories;

			// Token: 0x04008DF8 RID: 36344
			public string IdentityGrantHeader = Localization.Get("TwitchInfo_IdentityGrantHeader", false, null);

			// Token: 0x04008DF9 RID: 36345
			public string IdentityGrantSubtext = Localization.Get("TwitchInfo_IdentityGrantSubtext", false, null);

			// Token: 0x04008DFA RID: 36346
			public string LoadingText = Localization.Get("loadActionLoading", false, null);

			// Token: 0x04008DFB RID: 36347
			public string OfflineHeader = Localization.Get("TwitchInfo_OfflineHeader", false, null);

			// Token: 0x04008DFC RID: 36348
			public string OfflineSubtext1 = Localization.Get("TwitchInfo_OfflineSubtext1", false, null);

			// Token: 0x04008DFD RID: 36349
			public string OfflineSubtext2 = Localization.Get("TwitchInfo_OfflineSubtext2", false, null);

			// Token: 0x04008DFE RID: 36350
			public string CommandDescriptionsText = Localization.Get("TwitchInfo_CommandDescriptionsText", false, null);

			// Token: 0x04008DFF RID: 36351
			public string ChatPromptHeader = Localization.Get("TwitchInfo_ChatPromptHeader", false, null);

			// Token: 0x04008E00 RID: 36352
			public string ChatPromptSubtext = Localization.Get("TwitchInfo_ChatPromptSubtext", false, null);

			// Token: 0x04008E01 RID: 36353
			public string ActionsOffText = Localization.Get("TwitchInfo_ActionsOffText", false, null);

			// Token: 0x04008E02 RID: 36354
			public string CooldownText = Localization.Get("TwitchInfo_CooldownText", false, null);

			// Token: 0x04008E03 RID: 36355
			public string PausedText = Localization.Get("TwitchCooldownStatus_Paused", false, null);

			// Token: 0x04008E04 RID: 36356
			public string ActionPresetLabel = Localization.Get("xuiOptionsTwitchActionPreset", false, null);

			// Token: 0x04008E05 RID: 36357
			public string VotePresetLabel = Localization.Get("xuiOptionsTwitchVotePreset", false, null);

			// Token: 0x04008E06 RID: 36358
			public string EventPresetLabel = Localization.Get("xuiOptionsTwitchCustomEvents", false, null);

			// Token: 0x04008E07 RID: 36359
			public string TopKillerLabel = Localization.Get("TwitchInfo_TopKiller", false, null);

			// Token: 0x04008E08 RID: 36360
			public string TopGoodLabel = Localization.Get("TwitchInfo_TopGood", false, null);

			// Token: 0x04008E09 RID: 36361
			public string TopBadLabel = Localization.Get("TwitchInfo_TopEvil", false, null);

			// Token: 0x04008E0A RID: 36362
			public string BestHelperLabel = string.Format(Localization.Get("TwitchInfo_CurrentGood", false, null), TwitchManager.LeaderboardStats.GoodRewardTime);

			// Token: 0x04008E0B RID: 36363
			public string TotalGoodActionsLabel = Localization.Get("TwitchInfo_TotalGood", false, null);

			// Token: 0x04008E0C RID: 36364
			public string TotalBadActionsLabel = Localization.Get("TwitchInfo_TotalBad", false, null);

			// Token: 0x04008E0D RID: 36365
			public string LargestPimpPotLabel = Localization.Get("TwitchInfo_LargestPimpPot", false, null);

			// Token: 0x04008E0E RID: 36366
			public string DifficultyLabel = Localization.Get("goDifficultyShort", false, null);

			// Token: 0x04008E0F RID: 36367
			public string DayCycleLabel = Localization.Get("goDayLength", false, null);

			// Token: 0x04008E10 RID: 36368
			public string ModdedLabel = Localization.Get("goModded", false, null);

			// Token: 0x04008E11 RID: 36369
			public string PPRateLabel = Localization.Get("TwitchInfo_PPRate", false, null);

			// Token: 0x04008E12 RID: 36370
			public Dictionary<string, List<ExtensionConfigManager.CommandModel>> commands;
		}

		// Token: 0x020017D6 RID: 6102
		public class CommandModel
		{
			// Token: 0x04008E13 RID: 36371
			public string name;

			// Token: 0x04008E14 RID: 36372
			public string baseCommand;

			// Token: 0x04008E15 RID: 36373
			public string command;

			// Token: 0x04008E16 RID: 36374
			public bool isPositive;

			// Token: 0x04008E17 RID: 36375
			public string spends;

			// Token: 0x04008E18 RID: 36376
			public int cost;

			// Token: 0x04008E19 RID: 36377
			public string cooldownType;

			// Token: 0x04008E1A RID: 36378
			public int cooldownIndex;

			// Token: 0x04008E1B RID: 36379
			public byte bitPosition;

			// Token: 0x04008E1C RID: 36380
			public bool streamerOnly;
		}
	}
}
