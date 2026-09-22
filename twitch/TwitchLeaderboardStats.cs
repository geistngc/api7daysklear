using System;
using System.Collections.Generic;
using Challenges;
using UniLinq;

namespace Twitch
{
	// Token: 0x02001846 RID: 6214
	public class TwitchLeaderboardStats
	{
		// Token: 0x1400010C RID: 268
		// (add) Token: 0x0600BF8C RID: 49036 RVA: 0x0046D61C File Offset: 0x0046B81C
		// (remove) Token: 0x0600BF8D RID: 49037 RVA: 0x0046D654 File Offset: 0x0046B854
		public event OnLeaderboardStatsChanged StatsChanged;

		// Token: 0x1400010D RID: 269
		// (add) Token: 0x0600BF8E RID: 49038 RVA: 0x0046D68C File Offset: 0x0046B88C
		// (remove) Token: 0x0600BF8F RID: 49039 RVA: 0x0046D6C4 File Offset: 0x0046B8C4
		public event OnLeaderboardStatsChanged LeaderboardChanged;

		// Token: 0x1700177F RID: 6015
		// (get) Token: 0x0600BF90 RID: 49040 RVA: 0x0046D6F9 File Offset: 0x0046B8F9
		// (set) Token: 0x0600BF91 RID: 49041 RVA: 0x0046D701 File Offset: 0x0046B901
		public int GoodRewardTime
		{
			get
			{
				return this.goodRewardTime;
			}
			set
			{
				if (this.goodRewardTime != value)
				{
					this.goodRewardTime = value;
					this.nextGoodTime = (float)(this.goodRewardTime * 60);
				}
			}
		}

		// Token: 0x0600BF92 RID: 49042 RVA: 0x0046D723 File Offset: 0x0046B923
		public void SetupLocalization()
		{
			this.chatOutput_GoodReward = Localization.Get("TwitchChat_GoodReward", false, null);
			this.ingameOutput_GoodReward = Localization.Get("TwitchInGame_GoodReward", false, null);
		}

		// Token: 0x0600BF93 RID: 49043 RVA: 0x0046D749 File Offset: 0x0046B949
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleStatsChanged()
		{
			if (this.StatsChanged != null)
			{
				this.StatsChanged();
			}
		}

		// Token: 0x0600BF94 RID: 49044 RVA: 0x0046D75E File Offset: 0x0046B95E
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleLeaderboardChanged()
		{
			if (this.LeaderboardChanged != null)
			{
				this.LeaderboardChanged();
			}
		}

		// Token: 0x0600BF95 RID: 49045 RVA: 0x0046D774 File Offset: 0x0046B974
		public void UpdateStats(float deltaTime)
		{
			if (this.CurrentGoodDirty)
			{
				this.lastTime -= deltaTime;
				if (this.lastTime <= 0f)
				{
					List<TwitchLeaderboardStats.StatEntry> list = (from entry in this.StatEntries.Values
					where entry.CurrentGoodActions > 0
					orderby entry.CurrentGoodActions descending
					select entry).ToList<TwitchLeaderboardStats.StatEntry>();
					if (list.Count > 0)
					{
						if (this.CurrentGoodViewer != list[0])
						{
							this.CurrentGoodViewer = list[0];
							this.HandleStatsChanged();
						}
					}
					else if (this.CurrentGoodViewer != null)
					{
						this.CurrentGoodViewer = null;
						this.HandleStatsChanged();
					}
					this.CurrentGoodDirty = false;
					this.lastTime = 1f;
				}
			}
			if (this.nextGoodTime == -1f)
			{
				this.nextGoodTime = (float)(this.GoodRewardTime * 60);
			}
			if (this.CurrentGoodViewer == null)
			{
				return;
			}
			this.nextGoodTime -= deltaTime;
			if (this.nextGoodTime <= 0f)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				if (twitchManager.CurrentActionPreset.UseHelperReward && this.CurrentGoodViewer != null)
				{
					ViewerEntry viewerEntry = TwitchManager.Current.ViewerData.GetViewerEntry(this.CurrentGoodViewer.Name.ToLower());
					viewerEntry.StandardPoints += (float)this.GoodRewardAmount;
					twitchManager.ircClient.SendChannelMessage(string.Format(this.chatOutput_GoodReward, new object[]
					{
						this.CurrentGoodViewer.Name,
						viewerEntry.CombinedPoints,
						this.GoodRewardAmount,
						Localization.Get("TwitchPoints_PP", false, null)
					}), true);
					twitchManager.AddToInGameChatQueue(string.Format(this.ingameOutput_GoodReward, new object[]
					{
						viewerEntry.UserColor,
						this.CurrentGoodViewer.Name,
						this.GoodRewardAmount,
						Localization.Get("TwitchPoints_PP", false, null)
					}), null);
					twitchManager.LocalPlayer.PlayOneShot("twitch_top_helper", false, false, false, null, 1f);
					this.ClearAllCurrentGood();
					QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.HelperReward, "");
				}
				this.nextGoodTime = (float)(this.GoodRewardTime * 60);
			}
		}

		// Token: 0x0600BF96 RID: 49046 RVA: 0x0046D9D1 File Offset: 0x0046BBD1
		public void CheckTopKiller(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopKillerViewer == null || this.TopKillerViewer.Kills < newData.Kills)
			{
				this.TopKillerViewer = newData;
				this.HandleStatsChanged();
				return;
			}
			if (this.TopKillerViewer == newData)
			{
				this.HandleStatsChanged();
			}
		}

		// Token: 0x0600BF97 RID: 49047 RVA: 0x0046DA0C File Offset: 0x0046BC0C
		public void CheckTopGood(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopGoodViewer == null || this.TopGoodViewer.GoodActions < newData.GoodActions)
			{
				this.TopGoodViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.TopGoodViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
			this.CurrentGoodDirty = true;
		}

		// Token: 0x0600BF98 RID: 49048 RVA: 0x0046DA60 File Offset: 0x0046BC60
		public void CheckTopBad(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopBadViewer == null || this.TopBadViewer.BadActions < newData.BadActions)
			{
				this.TopBadViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.TopBadViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
			this.CurrentGoodDirty = true;
		}

		// Token: 0x0600BF99 RID: 49049 RVA: 0x0046DAB4 File Offset: 0x0046BCB4
		public void CheckMostBitsSpent(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.MostBitsSpentViewer == null || this.MostBitsSpentViewer.BitsUsed < newData.BitsUsed)
			{
				this.MostBitsSpentViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.MostBitsSpentViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
		}

		// Token: 0x0600BF9A RID: 49050 RVA: 0x0046DB00 File Offset: 0x0046BD00
		public TwitchLeaderboardStats.StatEntry AddKill(string name, string userColor)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.Kills++;
			return statEntry;
		}

		// Token: 0x0600BF9B RID: 49051 RVA: 0x0046DB54 File Offset: 0x0046BD54
		public TwitchLeaderboardStats.StatEntry AddGoodActionUsed(string name, string userColor, bool isBits)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			int num = isBits ? 2 : 1;
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.GoodActions += num;
			statEntry.CurrentGoodActions += num;
			statEntry.CurrentActions++;
			return statEntry;
		}

		// Token: 0x0600BF9C RID: 49052 RVA: 0x0046DBCC File Offset: 0x0046BDCC
		public TwitchLeaderboardStats.StatEntry AddBadActionUsed(string name, string userColor, bool isBits)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			int num = isBits ? 2 : 1;
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.BadActions += num;
			statEntry.CurrentGoodActions -= num;
			statEntry.CurrentActions++;
			return statEntry;
		}

		// Token: 0x0600BF9D RID: 49053 RVA: 0x0046DC44 File Offset: 0x0046BE44
		public TwitchLeaderboardStats.StatEntry AddBitsUsed(string name, string userColor, int amount)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.BitsUsed += amount;
			return statEntry;
		}

		// Token: 0x0600BF9E RID: 49054 RVA: 0x0046DC98 File Offset: 0x0046BE98
		public void ClearAllCurrentGood()
		{
			foreach (TwitchLeaderboardStats.StatEntry statEntry in this.StatEntries.Values)
			{
				statEntry.CurrentGoodActions = 0;
				statEntry.CurrentActions = 0;
			}
			this.CurrentGoodViewer = null;
			this.HandleStatsChanged();
			this.HandleLeaderboardChanged();
		}

		// Token: 0x04009052 RID: 36946
		public int LargestPimpPot;

		// Token: 0x04009053 RID: 36947
		public int LargestBitPot;

		// Token: 0x04009054 RID: 36948
		public int TotalGood;

		// Token: 0x04009055 RID: 36949
		public int TotalBad;

		// Token: 0x04009056 RID: 36950
		public int TotalActions;

		// Token: 0x04009057 RID: 36951
		public int TotalBits;

		// Token: 0x04009058 RID: 36952
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GoodReward;

		// Token: 0x04009059 RID: 36953
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_GoodReward;

		// Token: 0x0400905C RID: 36956
		public TwitchLeaderboardStats.StatEntry TopKillerViewer;

		// Token: 0x0400905D RID: 36957
		public TwitchLeaderboardStats.StatEntry TopGoodViewer;

		// Token: 0x0400905E RID: 36958
		public TwitchLeaderboardStats.StatEntry TopBadViewer;

		// Token: 0x0400905F RID: 36959
		public TwitchLeaderboardStats.StatEntry MostBitsSpentViewer;

		// Token: 0x04009060 RID: 36960
		public TwitchLeaderboardStats.StatEntry CurrentGoodViewer;

		// Token: 0x04009061 RID: 36961
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CurrentGoodDirty;

		// Token: 0x04009062 RID: 36962
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastTime = 1f;

		// Token: 0x04009063 RID: 36963
		public float nextGoodTime = -1f;

		// Token: 0x04009064 RID: 36964
		public int GoodRewardAmount = 1000;

		// Token: 0x04009065 RID: 36965
		[PublicizedFrom(EAccessModifier.Private)]
		public int goodRewardTime = 15;

		// Token: 0x04009066 RID: 36966
		public Dictionary<string, TwitchLeaderboardStats.StatEntry> StatEntries = new Dictionary<string, TwitchLeaderboardStats.StatEntry>();

		// Token: 0x02001847 RID: 6215
		public class StatEntry
		{
			// Token: 0x04009067 RID: 36967
			public string Name;

			// Token: 0x04009068 RID: 36968
			public string UserColor;

			// Token: 0x04009069 RID: 36969
			public int Kills;

			// Token: 0x0400906A RID: 36970
			public int GoodActions;

			// Token: 0x0400906B RID: 36971
			public int BadActions;

			// Token: 0x0400906C RID: 36972
			public int BitsUsed;

			// Token: 0x0400906D RID: 36973
			public int CurrentGoodActions;

			// Token: 0x0400906E RID: 36974
			public int CurrentActions;
		}
	}
}
