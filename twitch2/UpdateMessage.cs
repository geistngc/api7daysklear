using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x020017F1 RID: 6129
	public class UpdateMessage
	{
		// Token: 0x0600BDE5 RID: 48613 RVA: 0x00466448 File Offset: 0x00464648
		[PublicizedFrom(EAccessModifier.Private)]
		public static string DifficultyValueLocalized()
		{
			int num = GameStats.GetInt(EnumGameStats.GameDifficulty) + 1;
			return Localization.Get(string.Format("goDifficulty{0}", num) + ((num == 2) ? "_nodefault" : ""), false, null);
		}

		// Token: 0x0600BDE6 RID: 48614 RVA: 0x0046648C File Offset: 0x0046468C
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool IsGameModded()
		{
			GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
			return gameServerInfo != null && gameServerInfo.GetValue(GameInfoBool.ModdedConfig);
		}

		// Token: 0x0600BDE7 RID: 48615 RVA: 0x004664CC File Offset: 0x004646CC
		[PublicizedFrom(EAccessModifier.Private)]
		public static string ModdedValueLocalized()
		{
			GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
			if (gameServerInfo == null)
			{
				return "--";
			}
			if (!gameServerInfo.GetValue(GameInfoBool.ModdedConfig))
			{
				return Localization.Get("xuiComboYesNoOff", false, null);
			}
			return Localization.Get("xuiComboYesNoOn", false, null);
		}

		// Token: 0x0600BDE8 RID: 48616 RVA: 0x00466528 File Offset: 0x00464728
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetLocalizedPPRateValue()
		{
			if (TwitchManager.Current.ViewerData.PointRate == 1f)
			{
				return Localization.Get("xuiTwitchPointGenerationStandard", false, null);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 2f)
			{
				return Localization.Get("xuiTwitchPointGenerationDouble", false, null);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 3f)
			{
				return Localization.Get("xuiTwitchPointGenerationTriple", false, null);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 0f)
			{
				return Localization.Get("goDisabled", false, null);
			}
			return Localization.Get("xuiTwitchPointGenerationStandard", false, null);
		}

		// Token: 0x0600BDE9 RID: 48617 RVA: 0x004665D0 File Offset: 0x004647D0
		public UpdateMessage()
		{
			TwitchLeaderboardStats.StatEntry topKillerViewer = TwitchManager.LeaderboardStats.TopKillerViewer;
			this.TopKillerValue = (((topKillerViewer != null) ? topKillerViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry topGoodViewer = TwitchManager.LeaderboardStats.TopGoodViewer;
			this.TopGoodValue = (((topGoodViewer != null) ? topGoodViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry topBadViewer = TwitchManager.LeaderboardStats.TopBadViewer;
			this.TopBadValue = (((topBadViewer != null) ? topBadViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry currentGoodViewer = TwitchManager.LeaderboardStats.CurrentGoodViewer;
			this.BestHelperValue = (((currentGoodViewer != null) ? currentGoodViewer.Name : null) ?? "--");
			this.TotalGoodActionsValue = TwitchManager.LeaderboardStats.TotalGood.ToString();
			this.TotalBadActionsValue = TwitchManager.LeaderboardStats.TotalBad.ToString();
			this.LargestPimpPotValue = TwitchManager.LeaderboardStats.LargestPimpPot.ToString();
			this.DifficultyValue = UpdateMessage.DifficultyValueLocalized();
			this.DayCycleValue = string.Format(Localization.Get("goMinutes", false, null), GamePrefs.GetInt(EnumGamePrefs.DayNightLength));
			this.PPRateValue = UpdateMessage.GetLocalizedPPRateValue();
			this.ModdedValue = UpdateMessage.ModdedValueLocalized();
			base..ctor();
		}

		// Token: 0x04008E87 RID: 36487
		public string updateSignature;

		// Token: 0x04008E88 RID: 36488
		public string status;

		// Token: 0x04008E89 RID: 36489
		public int[] actionCooldowns;

		// Token: 0x04008E8A RID: 36490
		public Dictionary<string, int> bitBalances;

		// Token: 0x04008E8B RID: 36491
		public Dictionary<string, bool> hasChatted;

		// Token: 0x04008E8C RID: 36492
		public string ActionPresetKey = TwitchManager.Current.CurrentActionPreset.Name;

		// Token: 0x04008E8D RID: 36493
		public string VotePresetKey = TwitchManager.Current.CurrentVotePreset.Name;

		// Token: 0x04008E8E RID: 36494
		public string EventPresetKey = TwitchManager.Current.CurrentEventPreset.Name;

		// Token: 0x04008E8F RID: 36495
		public int Difficulty = GameStats.GetInt(EnumGameStats.GameDifficulty) + 1;

		// Token: 0x04008E90 RID: 36496
		public int DayMinutes = GamePrefs.GetInt(EnumGamePrefs.DayNightLength);

		// Token: 0x04008E91 RID: 36497
		public int PPRate = (int)TwitchManager.Current.ViewerData.PointRate;

		// Token: 0x04008E92 RID: 36498
		public bool IsModded = UpdateMessage.IsGameModded();

		// Token: 0x04008E93 RID: 36499
		public int GoodRewardTime = TwitchManager.LeaderboardStats.GoodRewardTime;

		// Token: 0x04008E94 RID: 36500
		public string ActionPresetValue = TwitchManager.Current.CurrentActionPreset.Title;

		// Token: 0x04008E95 RID: 36501
		public string VotePresetValue = TwitchManager.Current.CurrentVotePreset.Title;

		// Token: 0x04008E96 RID: 36502
		public string EventPresetValue = TwitchManager.Current.CurrentEventPreset.Title;

		// Token: 0x04008E97 RID: 36503
		public string TopKillerValue;

		// Token: 0x04008E98 RID: 36504
		public string TopGoodValue;

		// Token: 0x04008E99 RID: 36505
		public string TopBadValue;

		// Token: 0x04008E9A RID: 36506
		public string BestHelperValue;

		// Token: 0x04008E9B RID: 36507
		public string TotalGoodActionsValue;

		// Token: 0x04008E9C RID: 36508
		public string TotalBadActionsValue;

		// Token: 0x04008E9D RID: 36509
		public string LargestPimpPotValue;

		// Token: 0x04008E9E RID: 36510
		public string DifficultyValue;

		// Token: 0x04008E9F RID: 36511
		public string DayCycleValue;

		// Token: 0x04008EA0 RID: 36512
		public string PPRateValue;

		// Token: 0x04008EA1 RID: 36513
		public string ModdedValue;
	}
}
