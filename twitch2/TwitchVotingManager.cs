using System;
using System.Collections.Generic;
using Audio;
using Challenges;
using UniLinq;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001871 RID: 6257
	public class TwitchVotingManager
	{
		// Token: 0x170017A3 RID: 6051
		// (get) Token: 0x0600C11E RID: 49438 RVA: 0x0047A543 File Offset: 0x00478743
		// (set) Token: 0x0600C11F RID: 49439 RVA: 0x0047A54B File Offset: 0x0047874B
		public int MaxDailyVotes
		{
			get
			{
				return this.maxDailyVotes;
			}
			set
			{
				if (this.maxDailyVotes != value)
				{
					this.maxDailyVotes = value;
					this.lastGameDay = 1;
				}
			}
		}

		// Token: 0x170017A4 RID: 6052
		// (get) Token: 0x0600C120 RID: 49440 RVA: 0x0047A564 File Offset: 0x00478764
		// (set) Token: 0x0600C121 RID: 49441 RVA: 0x0047A56C File Offset: 0x0047876C
		public int CurrentVoteDayTimeRange
		{
			get
			{
				return this.currentVoteDayTimeRange;
			}
			set
			{
				if (this.currentVoteDayTimeRange != value)
				{
					this.currentVoteDayTimeRange = value;
					this.lastGameDay = 1;
				}
			}
		}

		// Token: 0x170017A5 RID: 6053
		// (get) Token: 0x0600C122 RID: 49442 RVA: 0x0047A585 File Offset: 0x00478785
		public bool VotingEnabled
		{
			get
			{
				return !this.Owner.CurrentVotePreset.IsEmpty;
			}
		}

		// Token: 0x170017A6 RID: 6054
		// (get) Token: 0x0600C123 RID: 49443 RVA: 0x0047A59A File Offset: 0x0047879A
		public bool VotingIsActive
		{
			get
			{
				return this.VotingEnabled && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.WaitingForNextVote && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.Init && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.ReadyForVoteStart && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.RequestedVoteStart && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.VoteReady;
			}
		}

		// Token: 0x170017A7 RID: 6055
		// (get) Token: 0x0600C124 RID: 49444 RVA: 0x0047A5D5 File Offset: 0x004787D5
		public int VoteCount
		{
			get
			{
				if (this.voterlist == null)
				{
					return 0;
				}
				return this.voterlist.Count;
			}
		}

		// Token: 0x170017A8 RID: 6056
		// (get) Token: 0x0600C125 RID: 49445 RVA: 0x0047A5EC File Offset: 0x004787EC
		public string VoteTypeText
		{
			get
			{
				return this.CurrentVoteType.Title;
			}
		}

		// Token: 0x170017A9 RID: 6057
		// (get) Token: 0x0600C126 RID: 49446 RVA: 0x0047A5F9 File Offset: 0x004787F9
		public string VoteTip
		{
			get
			{
				if (this.CurrentEvent != null)
				{
					return this.CurrentEvent.VoteClass.VoteTip;
				}
				return "";
			}
		}

		// Token: 0x170017AA RID: 6058
		// (get) Token: 0x0600C127 RID: 49447 RVA: 0x0047A619 File Offset: 0x00478819
		public string VoteOffset
		{
			get
			{
				if (this.CurrentEvent != null)
				{
					return this.CurrentEvent.VoteClass.VoteHeight;
				}
				return "0";
			}
		}

		// Token: 0x170017AB RID: 6059
		// (get) Token: 0x0600C128 RID: 49448 RVA: 0x0047A639 File Offset: 0x00478839
		public bool UseMystery
		{
			get
			{
				return this.CurrentVoteType.UseMystery;
			}
		}

		// Token: 0x170017AC RID: 6060
		// (get) Token: 0x0600C129 RID: 49449 RVA: 0x0047A646 File Offset: 0x00478846
		// (set) Token: 0x0600C12A RID: 49450 RVA: 0x0047A64E File Offset: 0x0047884E
		public int NeededLines { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170017AD RID: 6061
		// (get) Token: 0x0600C12B RID: 49451 RVA: 0x0047A657 File Offset: 0x00478857
		// (set) Token: 0x0600C12C RID: 49452 RVA: 0x0047A65F File Offset: 0x0047885F
		public TwitchVoteType CurrentVoteType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600C12D RID: 49453 RVA: 0x0047A668 File Offset: 0x00478868
		public TwitchVotingManager(TwitchManager owner)
		{
			this.Owner = owner;
			this.SetupVoteDayTimeRanges();
		}

		// Token: 0x0600C12E RID: 49454 RVA: 0x0047A736 File Offset: 0x00478936
		public void CleanupData()
		{
			this.VoteTypes.Clear();
			this.VoteGroups.Clear();
		}

		// Token: 0x0600C12F RID: 49455 RVA: 0x0047A750 File Offset: 0x00478950
		public void SetupLocalization()
		{
			this.chatOutput_VoteStarted = Localization.Get("TwitchChat_VoteStarted", false, null);
			this.chatOutput_VoteFinished = Localization.Get("TwitchChat_VoteFinished", false, null);
			this.dayTimeRangeOutput = Localization.Get("xuiOptionsTwitchVoteDayTimeRangeDisplay", false, null);
			this.VoteOptionA = Localization.Get("TwitchVoteOption_A", false, null);
			this.VoteOptionB = Localization.Get("TwitchVoteOption_B", false, null);
			this.VoteOptionC = Localization.Get("TwitchVoteOption_C", false, null);
			this.VoteOptionD = Localization.Get("TwitchVoteOption_D", false, null);
			this.VoteOptionE = Localization.Get("TwitchVoteOption_E", false, null);
		}

		// Token: 0x0600C130 RID: 49456 RVA: 0x0047A7F0 File Offset: 0x004789F0
		public void AddVoteType(TwitchVoteType voteType)
		{
			this.VoteTypes.Add(voteType.Name, voteType);
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				if (this.VoteGroups[i].Name == voteType.Group)
				{
					this.VoteGroups[i].VoteTypes.Add(voteType);
					return;
				}
			}
			TwitchVoteGroup twitchVoteGroup = new TwitchVoteGroup(voteType.Group);
			twitchVoteGroup.VoteTypes.Add(voteType);
			this.VoteGroups.Add(twitchVoteGroup);
		}

		// Token: 0x0600C131 RID: 49457 RVA: 0x0047A87F File Offset: 0x00478A7F
		public TwitchVoteType GetVoteType(string voteTypeName)
		{
			if (this.VoteTypes.ContainsKey(voteTypeName))
			{
				return this.VoteTypes[voteTypeName];
			}
			return null;
		}

		// Token: 0x0600C132 RID: 49458 RVA: 0x0047A8A0 File Offset: 0x00478AA0
		public void AddVote(int index, string userName)
		{
			if (!this.voterlist.Contains(userName) && this.voteList.Count > index)
			{
				this.voteList[index].VoteCount++;
				Manager.PlayInsidePlayerHead("twitch_vote_received", -1, 0f, false, false);
				this.voterlist.Add(userName);
				this.UIDirty = true;
				for (int i = 0; i < this.voteList.Count; i++)
				{
					this.voteList[i].UIDirty = true;
				}
			}
		}

		// Token: 0x0600C133 RID: 49459 RVA: 0x0047A930 File Offset: 0x00478B30
		public void ClearVotes()
		{
			for (int i = 0; i < this.voteList.Count; i++)
			{
				this.voteList[i].VoteCount = 0;
				this.voteList[i].VoterNames.Clear();
			}
			this.voterlist.Clear();
		}

		// Token: 0x0600C134 RID: 49460 RVA: 0x0047A988 File Offset: 0x00478B88
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalculateVoteTimes()
		{
			this.DailyVoteTimes.Clear();
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[this.CurrentVoteDayTimeRange];
			float num = (float)(voteDayTimeRange.EndHour - voteDayTimeRange.StartHour) / (float)this.MaxDailyVotes;
			float num2 = (float)voteDayTimeRange.StartHour;
			for (int i = 0; i < this.MaxDailyVotes; i++)
			{
				float num3 = -1f;
				if (i == 0)
				{
					gameRandom.RandomRange(0f, num);
				}
				else
				{
					gameRandom.RandomRange(num - 1f, num);
				}
				int num4 = (int)(num2 + num3);
				int minutes = gameRandom.RandomRange(0, 59);
				TwitchVotingManager.DailyVoteEntry dailyVoteEntry = new TwitchVotingManager.DailyVoteEntry();
				dailyVoteEntry.VoteStartTime = GameUtils.DayTimeToWorldTime(1, num4, minutes);
				dailyVoteEntry.VoteEndTime = GameUtils.DayTimeToWorldTime(1, num4 + 1, minutes);
				dailyVoteEntry.Index = i + 1;
				num2 += num;
				this.DailyVoteTimes.Add(dailyVoteEntry);
			}
		}

		// Token: 0x0600C135 RID: 49461 RVA: 0x0047AA80 File Offset: 0x00478C80
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetVoteTypeDay()
		{
			foreach (TwitchVoteType twitchVoteType in this.VoteTypes.Values)
			{
				twitchVoteType.CurrentDayCount = 0;
			}
			foreach (TwitchVote twitchVote in TwitchActionManager.TwitchVotes.Values)
			{
				twitchVote.CurrentDayCount = 0;
			}
		}

		// Token: 0x0600C136 RID: 49462 RVA: 0x0047AB1C File Offset: 0x00478D1C
		public bool SetupVoteList(List<TwitchVoteEntry> voteList)
		{
			World world = GameManager.Instance.World;
			TwitchVoteType currentVoteType = this.CurrentVoteType;
			string name = currentVoteType.Name;
			TwitchVote twitchVote = null;
			int highestGameStage = this.Owner.HighestGameStage;
			this.ClearVotes();
			voteList.Clear();
			this.tempSortList.Clear();
			this.tempVoteGroupList.Clear();
			EntityPlayer localPlayer = this.Owner.LocalPlayer;
			if (currentVoteType.GuaranteedGroup != "")
			{
				foreach (TwitchVote twitchVote2 in TwitchActionManager.TwitchVotes.Values)
				{
					if (twitchVote2.Enabled && twitchVote2.IsInPreset(this.Owner.CurrentVotePreset) && twitchVote2.VoteTypes.Contains(name) && twitchVote2.Group == currentVoteType.GuaranteedGroup && twitchVote2.CanUse(this.hour, highestGameStage, localPlayer))
					{
						this.tempSortList.Add(twitchVote2);
					}
				}
				for (int i = 0; i < this.tempSortList.Count * 3; i++)
				{
					int num = world.GetGameRandom().RandomRange(this.tempSortList.Count);
					int num2 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
					if (num != num2)
					{
						TwitchVote value = this.tempSortList[num];
						this.tempSortList[num] = this.tempSortList[num2];
						this.tempSortList[num2] = value;
					}
				}
				twitchVote = this.tempSortList[0];
				this.tempSortList.Clear();
			}
			foreach (TwitchVote twitchVote3 in TwitchActionManager.TwitchVotes.Values)
			{
				if (twitchVote3.Enabled && twitchVote3.IsInPreset(this.Owner.CurrentVotePreset) && twitchVote3.VoteTypes.Contains(name) && twitchVote3.CanUse(this.hour, highestGameStage, localPlayer))
				{
					this.tempSortList.Add(twitchVote3);
				}
			}
			for (int j = 0; j < this.tempSortList.Count * 3; j++)
			{
				int num3 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
				int num4 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
				if (num3 != num4)
				{
					TwitchVote value2 = this.tempSortList[num3];
					this.tempSortList[num3] = this.tempSortList[num4];
					this.tempSortList[num4] = value2;
				}
			}
			this.NeededLines = 1;
			int num5 = 0;
			if (twitchVote != null)
			{
				this.tempSortList.Insert(UnityEngine.Random.Range(0, 3), twitchVote);
			}
			for (int k = 0; k < this.tempSortList.Count; k++)
			{
				TwitchVote twitchVote4 = this.tempSortList[k];
				if (!(twitchVote4.Group != "") || !this.tempVoteGroupList.Contains(twitchVote4.Group))
				{
					if (twitchVote4.Group != "")
					{
						this.tempVoteGroupList.Add(twitchVote4.Group);
					}
					string voteCommand = this.VoteOptionA;
					switch (num5)
					{
					case 1:
						voteCommand = this.VoteOptionB;
						break;
					case 2:
						voteCommand = this.VoteOptionC;
						break;
					case 3:
						voteCommand = this.VoteOptionD;
						break;
					case 4:
						voteCommand = this.VoteOptionE;
						break;
					}
					if (twitchVote4.VoteLine1 != "" && this.NeededLines < 2)
					{
						this.NeededLines = 2;
					}
					if (twitchVote4.VoteLine2 != "" && this.NeededLines < 3)
					{
						this.NeededLines = 3;
					}
					voteList.Add(new TwitchVoteEntry(voteCommand, twitchVote4)
					{
						Owner = this,
						Index = num5
					});
					num5++;
					if (num5 == currentVoteType.VoteChoiceCount)
					{
						break;
					}
				}
			}
			return voteList.Count != 0;
		}

		// Token: 0x0600C137 RID: 49463 RVA: 0x0047AF60 File Offset: 0x00479160
		public TwitchVoteEntry GetVoteWinner()
		{
			this.tempVoteList.Clear();
			int num = -1;
			for (int i = 0; i < this.voteList.Count; i++)
			{
				if (this.voteList[i].VoteCount > num)
				{
					num = this.voteList[i].VoteCount;
					this.tempVoteList.Clear();
					this.tempVoteList.Add(this.voteList[i]);
				}
				else if (this.voteList[i].VoteCount == num)
				{
					this.tempVoteList.Add(this.voteList[i]);
				}
			}
			return this.tempVoteList[GameManager.Instance.World.GetGameRandom().RandomRange(0, this.tempVoteList.Count)];
		}

		// Token: 0x0600C138 RID: 49464 RVA: 0x0047B034 File Offset: 0x00479234
		public void ResetVoteOnDeath()
		{
			TwitchVotingManager.VoteStateTypes currentVoteState = this.CurrentVoteState;
			if (currentVoteState - TwitchVotingManager.VoteStateTypes.VoteStarted <= 1)
			{
				this.readyForVote = false;
				this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				this.ResetVoteGroupsForVote();
				if (this.VoteEventEnded != null)
				{
					this.VoteEventEnded();
				}
			}
		}

		// Token: 0x0600C139 RID: 49465 RVA: 0x0047B088 File Offset: 0x00479288
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetVoteGroupsForVote()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				this.VoteGroups[i].SkippedThisVote = false;
			}
		}

		// Token: 0x0600C13A RID: 49466 RVA: 0x0047B0C0 File Offset: 0x004792C0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckAllVoteGroupsSkipped()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				if (!this.VoteGroups[i].SkippedThisVote)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600C13B RID: 49467 RVA: 0x0047B0FC File Offset: 0x004792FC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool AllowVoting()
		{
			return this.Owner.CooldownType == TwitchManager.CooldownTypes.None || ((this.Owner.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.Owner.CooldownType == TwitchManager.CooldownTypes.QuestDisabled) && this.AllowVotesDuringQuests) || ((this.Owner.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown || this.Owner.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled) && this.AllowVotesDuringBloodmoon);
		}

		// Token: 0x0600C13C RID: 49468 RVA: 0x0047B160 File Offset: 0x00479360
		public void Update(float deltaTime)
		{
			switch (this.CurrentVoteState)
			{
			case TwitchVotingManager.VoteStateTypes.Init:
				GameEventManager.Current.GameEventApproved += this.Current_GameEventApproved;
				GameEventManager.Current.GameEventCompleted += this.Current_GameEventCompleted;
				GameEventManager.Current.GameEntitySpawned += this.Current_GameEntitySpawned;
				GameEventManager.Current.GameEntityKilled += this.Current_GameEntityKilled;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				this.ShuffleVoteGroups();
				this.ShuffleVoteGroupVoteTypes();
				return;
			case TwitchVotingManager.VoteStateTypes.WaitingForNextVote:
			{
				if (this.QueuedVoteType != null)
				{
					this.CurrentVoteType = this.QueuedVoteType;
					if (this.SetupVoteList(this.voteList))
					{
						this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.ReadyForVoteStart;
						this.Owner.RefreshVoteLockedLevel();
					}
					this.QueuedVoteType = null;
				}
				if (this.VoteStartDelayTimeRemaining > 0f)
				{
					this.VoteStartDelayTimeRemaining -= deltaTime;
					return;
				}
				World world = GameManager.Instance.World;
				ulong num = world.worldTime;
				this.day = GameUtils.WorldTimeToDays(num);
				num %= 24000UL;
				this.hour = GameUtils.WorldTimeToHours(num);
				if (this.day == 1)
				{
					return;
				}
				TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[this.CurrentVoteDayTimeRange];
				bool flag = !this.AllowVotesDuringBloodmoon && world.IsWorldEvent(World.WorldEvent.BloodMoon);
				if (this.VoteInProgress)
				{
					if (flag)
					{
						this.CancelVote();
					}
					if (this.hour < voteDayTimeRange.StartHour || this.hour > voteDayTimeRange.EndHour)
					{
						this.CancelVote();
					}
					return;
				}
				if (flag || this.hour < voteDayTimeRange.StartHour || this.hour > voteDayTimeRange.EndHour)
				{
					return;
				}
				if (this.day != this.lastGameDay)
				{
					this.CalculateVoteTimes();
					this.ResetVoteTypeDay();
					this.lastGameDay = this.day;
				}
				for (int i = 0; i < this.DailyVoteTimes.Count; i++)
				{
					if (this.DailyVoteTimes[i].LastVoteDay != this.day)
					{
						if (num > this.DailyVoteTimes[i].VoteStartTime && num < this.DailyVoteTimes[i].VoteEndTime)
						{
							if (!this.SetReadyForVote(this.DailyVoteTimes[i].Index))
							{
								if (this.CheckAllVoteGroupsSkipped())
								{
									this.DailyVoteTimes[i].LastVoteDay = this.day;
									this.ResetVoteGroupsForVote();
								}
								return;
							}
							this.ResetVoteGroupsForVote();
							if (this.SetupVoteList(this.voteList))
							{
								this.DailyVoteTimes[i].LastVoteDay = this.day;
								this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.ReadyForVoteStart;
								this.Owner.RefreshVoteLockedLevel();
							}
						}
						else if (num > this.DailyVoteTimes[i].VoteEndTime)
						{
							this.DailyVoteTimes[i].LastVoteDay = this.day;
						}
					}
				}
				return;
			}
			case TwitchVotingManager.VoteStateTypes.ReadyForVoteStart:
				if (this.VoteStartDelayTimeRemaining > 0f)
				{
					this.VoteStartDelayTimeRemaining -= deltaTime;
					return;
				}
				if (this.VotingEnabled && this.Owner.VoteLockedLevel == TwitchVoteLockTypes.None && this.AllowVoting() && (!this.CurrentVoteType.SpawnBlocked || this.Owner.ReadyForVote))
				{
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageTwitchVoteScheduling>().Setup(), false);
					}
					else
					{
						TwitchVoteScheduler.Current.AddParticipant(this.Owner.LocalPlayer.entityId);
					}
					this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.RequestedVoteStart;
					return;
				}
				break;
			case TwitchVotingManager.VoteStateTypes.RequestedVoteStart:
				break;
			case TwitchVotingManager.VoteStateTypes.VoteReady:
				if (this.VotingEnabled && this.Owner.VoteLockedLevel == TwitchVoteLockTypes.None && !this.CheckVoteLock() && this.AllowVoting() && (!this.CurrentVoteType.SpawnBlocked || this.Owner.ReadyForVote))
				{
					this.StartVote();
					return;
				}
				break;
			case TwitchVotingManager.VoteStateTypes.VoteStarted:
				if (this.VotingEnabled && this.AllowVoting())
				{
					this.VoteTimeRemaining -= Time.deltaTime;
					if (this.VoteTimeRemaining <= 0f)
					{
						this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteFinished;
						return;
					}
				}
				break;
			case TwitchVotingManager.VoteStateTypes.VoteFinished:
				this.CurrentEvent = this.GetVoteWinner();
				this.CurrentEvent.ActiveSpawns.Clear();
				this.CurrentEvent.VoteClass.CurrentDayCount++;
				this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_VoteFinished, this.CurrentEvent.VoteClass.VoteDescription), true);
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForActive;
				this.VoteTimeRemaining = 2f;
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.VoteComplete, this.CurrentEvent.VoteClass.Group);
				return;
			case TwitchVotingManager.VoteStateTypes.WaitingForActive:
				if (this.VoteTimeRemaining > 0f)
				{
					this.VoteTimeRemaining -= deltaTime;
					return;
				}
				if (GameEventManager.Current.HandleAction(this.CurrentEvent.VoteClass.GameEvent, this.Owner.LocalPlayer, this.Owner.LocalPlayer, true, " ", "vote", this.Owner.AllowCrateSharing, true, "", null))
				{
					GameEventManager.Current.HandleGameEventApproved(this.CurrentEvent.VoteClass.GameEvent, this.Owner.LocalPlayer.entityId, " ", "vote");
					return;
				}
				this.VoteTimeRemaining = 10f;
				return;
			case TwitchVotingManager.VoteStateTypes.EventActive:
				if (this.VoteEventTimeRemaining < 0f)
				{
					if (this.VoteEventComplete)
					{
						this.HandleGameEventEnded(true);
						return;
					}
				}
				else
				{
					this.VoteEventTimeRemaining -= deltaTime;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600C13D RID: 49469 RVA: 0x0047B6FA File Offset: 0x004798FA
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckVoteLock()
		{
			return (!this.AllowVotesDuringQuests && QuestEventManager.Current.QuestBounds.width != 0f) || (!this.AllowVotesInSafeZone && this.Owner.IsSafe);
		}

		// Token: 0x0600C13E RID: 49470 RVA: 0x0047B734 File Offset: 0x00479934
		public bool IsHighest(TwitchVoteEntry vote)
		{
			for (int i = 0; i < this.voteList.Count; i++)
			{
				if (i != vote.Index && this.voteList[i].VoteCount > vote.VoteCount)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600C13F RID: 49471 RVA: 0x0047B77C File Offset: 0x0047997C
		public bool SetReadyForVote(int index)
		{
			return this.GetNextVoteType();
		}

		// Token: 0x0600C140 RID: 49472 RVA: 0x0047B784 File Offset: 0x00479984
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetCurrentVote()
		{
			this.VoteTimeRemaining = this.VoteTime;
		}

		// Token: 0x0600C141 RID: 49473 RVA: 0x0047B794 File Offset: 0x00479994
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupVoteDayTimeRanges()
		{
			this.VoteDayTimeRanges.Clear();
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Short", false, null),
				StartHour = 8,
				EndHour = 16
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Average", false, null),
				StartHour = 6,
				EndHour = 18
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Extended", false, null),
				StartHour = 4,
				EndHour = 20
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_All", false, null),
				StartHour = 0,
				EndHour = 23
			});
		}

		// Token: 0x0600C142 RID: 49474 RVA: 0x0047B870 File Offset: 0x00479A70
		[PublicizedFrom(EAccessModifier.Private)]
		public void ShuffleVoteGroups()
		{
			for (int i = 0; i <= this.VoteGroups.Count * this.VoteGroups.Count; i++)
			{
				int num = UnityEngine.Random.Range(0, this.VoteGroups.Count);
				int num2 = UnityEngine.Random.Range(0, this.VoteGroups.Count);
				if (num != num2)
				{
					TwitchVoteGroup value = this.VoteGroups[num];
					this.VoteGroups[num] = this.VoteGroups[num2];
					this.VoteGroups[num2] = value;
				}
			}
		}

		// Token: 0x0600C143 RID: 49475 RVA: 0x0047B8FC File Offset: 0x00479AFC
		[PublicizedFrom(EAccessModifier.Private)]
		public void ShuffleVoteGroupVoteTypes()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				this.VoteGroups[i].ShuffleVoteTypes();
			}
		}

		// Token: 0x0600C144 RID: 49476 RVA: 0x0047B930 File Offset: 0x00479B30
		public void CancelVote()
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForNextVote && this.readyForVote)
			{
				this.readyForVote = false;
			}
		}

		// Token: 0x0600C145 RID: 49477 RVA: 0x0047B94A File Offset: 0x00479B4A
		public void RequestApprovedToStart()
		{
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteReady;
		}

		// Token: 0x0600C146 RID: 49478 RVA: 0x0047B954 File Offset: 0x00479B54
		public void StartVote()
		{
			this.VoteTimeRemaining = this.VoteTime;
			this.voterlist.Clear();
			if (this.VoteStarted != null)
			{
				this.VoteStarted();
			}
			this.Owner.UIDirty = true;
			this.Owner.LocalPlayer.TwitchVoteLock = (this.CurrentVoteType.ActionLockout ? TwitchVoteLockTypes.ActionsLocked : TwitchVoteLockTypes.VoteLocked);
			this.Owner.ircClient.SendChannelMessage(this.chatOutput_VoteStarted, true);
			Manager.BroadcastPlay(this.Owner.LocalPlayer.position, "twitch_vote_started", 0f);
			this.readyForVote = false;
			this.CurrentVoteType.CurrentDayCount++;
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteStarted;
		}

		// Token: 0x0600C147 RID: 49479 RVA: 0x0047BA10 File Offset: 0x00479C10
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntitySpawned(string gameEventID, int entityID, string tag)
		{
			if (this.CurrentEvent == null || tag != "vote")
			{
				return;
			}
			if (gameEventID == this.CurrentEvent.VoteClass.GameEvent)
			{
				this.CurrentEvent.ActiveSpawns.Add(entityID);
			}
		}

		// Token: 0x0600C148 RID: 49480 RVA: 0x0047BA5C File Offset: 0x00479C5C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityKilled(int entityID)
		{
			if (this.CurrentEvent == null)
			{
				return;
			}
			if (this.CurrentEvent.ActiveSpawns.Contains(entityID))
			{
				this.CurrentEvent.ActiveSpawns.Remove(entityID);
			}
		}

		// Token: 0x0600C149 RID: 49481 RVA: 0x0047BA8C File Offset: 0x00479C8C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventCompleted(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			if (this.CurrentEvent != null && this.CurrentEvent.VoteClass.GameEvent == gameEventID && tag == "vote")
			{
				this.VoteEventComplete = true;
			}
		}

		// Token: 0x0600C14A RID: 49482 RVA: 0x0047BAC4 File Offset: 0x00479CC4
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleGameEventEnded(bool playSound)
		{
			if (this.CurrentVoteType.CooldownOnEnd && this.Owner.AllowActions && this.Owner.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
			{
				this.Owner.SetCooldown((float)this.Owner.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReached, false, true);
			}
			this.CurrentEvent.VoteClass.HandleVoteComplete();
			this.CurrentEvent = null;
			if (playSound)
			{
				Manager.BroadcastPlay(this.Owner.LocalPlayer.position, "twitch_vote_ended", 0f);
			}
			this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
			this.ResetVoteGroupsForVote();
			if (this.VoteEventEnded != null)
			{
				this.VoteEventEnded();
			}
			this.VoteStartDelayTimeRemaining = 10f;
		}

		// Token: 0x0600C14B RID: 49483 RVA: 0x0047BB94 File Offset: 0x00479D94
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GetNextVoteType()
		{
			if (this.voteGroupIndex == -1)
			{
				this.voteGroupIndex = GameManager.Instance.World.GetGameRandom().RandomRange(this.VoteGroups.Count);
			}
			bool flag = GameManager.Instance.World.IsWorldEvent(World.WorldEvent.BloodMoon);
			TwitchVoteGroup twitchVoteGroup = this.VoteGroups[this.voteGroupIndex];
			if (!twitchVoteGroup.SkippedThisVote)
			{
				for (int i = 0; i < twitchVoteGroup.VoteTypes.Count; i++)
				{
					TwitchVoteType nextVoteType = twitchVoteGroup.GetNextVoteType();
					if (nextVoteType.IsInPreset(this.Owner.CurrentVotePreset.Name) && !nextVoteType.ManualStart && nextVoteType.CanUse() && this.hour >= nextVoteType.AllowedStartHour && this.hour <= nextVoteType.AllowedEndHour && (!nextVoteType.IsBoss || this.Owner.CurrentVotePreset.BossVoteSetting != TwitchVotingManager.BossVoteSettings.Disabled) && (nextVoteType.AllowedWithActions || !this.Owner.AllowActions) && ((nextVoteType.IsBoss && this.Owner.CurrentVotePreset.BossVoteSetting == TwitchVotingManager.BossVoteSettings.Daily) || ((nextVoteType.BloodMoonDay || this.Owner.nextBMDay != this.day) && (nextVoteType.BloodMoonAllowed || !flag))))
					{
						this.CurrentVoteType = nextVoteType;
						this.voteGroupIndex++;
						if (this.voteGroupIndex >= this.VoteGroups.Count)
						{
							this.voteGroupIndex = 0;
						}
						return true;
					}
				}
				twitchVoteGroup.SkippedThisVote = true;
			}
			this.voteGroupIndex++;
			if (this.voteGroupIndex >= this.VoteGroups.Count)
			{
				this.voteGroupIndex = 0;
			}
			return false;
		}

		// Token: 0x0600C14C RID: 49484 RVA: 0x0047BD48 File Offset: 0x00479F48
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForActive && this.CurrentEvent.VoteClass.GameEvent == gameEventID)
			{
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.EventActive;
				this.VoteEventComplete = false;
				this.VoteEventTimeRemaining = 10f;
				this.CurrentEvent.VoterNames.AddRange(this.voterlist);
				this.Owner.AddVoteHistory(this.CurrentEvent.VoteClass);
				if (this.VoteEventStarted != null)
				{
					this.VoteEventStarted();
				}
			}
		}

		// Token: 0x0600C14D RID: 49485 RVA: 0x0047BDD0 File Offset: 0x00479FD0
		public void HandleMessage(TwitchIRCClient.TwitchChatMessage message)
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.VoteStarted)
			{
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionA))
				{
					this.AddVote(0, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionB))
				{
					this.AddVote(1, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionC))
				{
					this.AddVote(2, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionD))
				{
					this.AddVote(3, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionE))
				{
					this.AddVote(4, message.UserName);
				}
			}
		}

		// Token: 0x0600C14E RID: 49486 RVA: 0x0047BE90 File Offset: 0x0047A090
		public List<string> HandleKiller(TwitchVoteEntry voteEntry)
		{
			if (this.CurrentEvent == null && voteEntry == null)
			{
				return null;
			}
			if (this.CurrentEvent != null)
			{
				List<string> voterNames = this.CurrentEvent.VoterNames;
				this.CurrentEvent.Complete = true;
				this.HandleGameEventEnded(false);
				return this.voterlist;
			}
			if (voteEntry != null)
			{
				return voteEntry.VoterNames;
			}
			return null;
		}

		// Token: 0x0600C14F RID: 49487 RVA: 0x0047BEE4 File Offset: 0x0047A0E4
		public string GetDayTimeRange(int tempVoteDayTimeRange)
		{
			TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[tempVoteDayTimeRange];
			if (voteDayTimeRange.StartHour == 0 && voteDayTimeRange.EndHour == 23)
			{
				return voteDayTimeRange.Name;
			}
			return string.Format(this.dayTimeRangeOutput, voteDayTimeRange.StartHour, voteDayTimeRange.EndHour);
		}

		// Token: 0x0600C150 RID: 49488 RVA: 0x0047BF38 File Offset: 0x0047A138
		public void QueueVote(string voteType)
		{
			if (this.VoteTypes.ContainsKey(voteType))
			{
				this.QueuedVoteType = this.VoteTypes[voteType];
			}
		}

		// Token: 0x0600C151 RID: 49489 RVA: 0x0047BF5A File Offset: 0x0047A15A
		public void ForceEndVote()
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.VoteStarted)
			{
				this.CurrentEvent = null;
				this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				if (this.VoteEventEnded != null)
				{
					this.VoteEventEnded();
				}
			}
		}

		// Token: 0x0400925A RID: 37466
		public TwitchManager Owner;

		// Token: 0x0400925B RID: 37467
		public TwitchVotingManager.VoteStateTypes CurrentVoteState;

		// Token: 0x0400925C RID: 37468
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_VoteStarted;

		// Token: 0x0400925D RID: 37469
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_VoteFinished;

		// Token: 0x0400925E RID: 37470
		[PublicizedFrom(EAccessModifier.Private)]
		public string dayTimeRangeOutput;

		// Token: 0x0400925F RID: 37471
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionA;

		// Token: 0x04009260 RID: 37472
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionB;

		// Token: 0x04009261 RID: 37473
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionC;

		// Token: 0x04009262 RID: 37474
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionD;

		// Token: 0x04009263 RID: 37475
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionE;

		// Token: 0x04009264 RID: 37476
		[PublicizedFrom(EAccessModifier.Private)]
		public int maxDailyVotes = 4;

		// Token: 0x04009265 RID: 37477
		public int lastGameDay = 1;

		// Token: 0x04009266 RID: 37478
		public bool WinnerShowing;

		// Token: 0x04009267 RID: 37479
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentVoteDayTimeRange = 2;

		// Token: 0x04009268 RID: 37480
		public List<TwitchVotingManager.VoteDayTimeRange> VoteDayTimeRanges = new List<TwitchVotingManager.VoteDayTimeRange>();

		// Token: 0x04009269 RID: 37481
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVotingManager.DailyVoteEntry> DailyVoteTimes = new List<TwitchVotingManager.DailyVoteEntry>();

		// Token: 0x0400926A RID: 37482
		public bool AllowVotesDuringBloodmoon;

		// Token: 0x0400926B RID: 37483
		public bool AllowVotesDuringQuests;

		// Token: 0x0400926C RID: 37484
		public bool AllowVotesInSafeZone;

		// Token: 0x0400926D RID: 37485
		public List<TwitchVoteType> NextVotes = new List<TwitchVoteType>();

		// Token: 0x0400926E RID: 37486
		public bool VoteInProgress;

		// Token: 0x0400926F RID: 37487
		public float VoteTime = 60f;

		// Token: 0x04009270 RID: 37488
		public int ViewerDefeatReward = 250;

		// Token: 0x04009272 RID: 37490
		public float VoteStartDelayTimeRemaining;

		// Token: 0x04009273 RID: 37491
		public float VoteEventTimeRemaining;

		// Token: 0x04009274 RID: 37492
		public float VoteTimeRemaining;

		// Token: 0x04009275 RID: 37493
		public bool UIDirty;

		// Token: 0x04009276 RID: 37494
		public bool VoteEventComplete;

		// Token: 0x04009277 RID: 37495
		public List<TwitchVoteEntry> voteList = new List<TwitchVoteEntry>();

		// Token: 0x04009278 RID: 37496
		public List<string> voterlist = new List<string>();

		// Token: 0x04009279 RID: 37497
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVoteEntry> tempVoteList = new List<TwitchVoteEntry>();

		// Token: 0x0400927A RID: 37498
		public OnGameEventVoteAction VoteStarted;

		// Token: 0x0400927B RID: 37499
		public OnGameEventVoteAction VoteEventStarted;

		// Token: 0x0400927C RID: 37500
		public OnGameEventVoteAction VoteEventEnded;

		// Token: 0x0400927D RID: 37501
		public Dictionary<string, TwitchVoteType> VoteTypes = new Dictionary<string, TwitchVoteType>();

		// Token: 0x0400927E RID: 37502
		[PublicizedFrom(EAccessModifier.Private)]
		public int voteGroupIndex = -1;

		// Token: 0x0400927F RID: 37503
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVoteGroup> VoteGroups = new List<TwitchVoteGroup>();

		// Token: 0x04009280 RID: 37504
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchVoteType QueuedVoteType;

		// Token: 0x04009282 RID: 37506
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVote> tempSortList = new List<TwitchVote>();

		// Token: 0x04009283 RID: 37507
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> tempVoteGroupList = new List<string>();

		// Token: 0x04009284 RID: 37508
		[PublicizedFrom(EAccessModifier.Private)]
		public int day = -1;

		// Token: 0x04009285 RID: 37509
		[PublicizedFrom(EAccessModifier.Private)]
		public int hour = -1;

		// Token: 0x04009286 RID: 37510
		[PublicizedFrom(EAccessModifier.Private)]
		public bool readyForVote;

		// Token: 0x04009287 RID: 37511
		public TwitchVoteEntry CurrentEvent;

		// Token: 0x02001872 RID: 6258
		public enum VoteStateTypes
		{
			// Token: 0x04009289 RID: 37513
			Init,
			// Token: 0x0400928A RID: 37514
			WaitingForNextVote,
			// Token: 0x0400928B RID: 37515
			ReadyForVoteStart,
			// Token: 0x0400928C RID: 37516
			RequestedVoteStart,
			// Token: 0x0400928D RID: 37517
			VoteReady,
			// Token: 0x0400928E RID: 37518
			VoteStarted,
			// Token: 0x0400928F RID: 37519
			VoteFinished,
			// Token: 0x04009290 RID: 37520
			WaitingForActive,
			// Token: 0x04009291 RID: 37521
			EventActive
		}

		// Token: 0x02001873 RID: 6259
		public enum BossVoteSettings
		{
			// Token: 0x04009293 RID: 37523
			Disabled,
			// Token: 0x04009294 RID: 37524
			Standard,
			// Token: 0x04009295 RID: 37525
			Daily
		}

		// Token: 0x02001874 RID: 6260
		public class DailyVoteEntry
		{
			// Token: 0x04009296 RID: 37526
			public ulong VoteStartTime;

			// Token: 0x04009297 RID: 37527
			public ulong VoteEndTime;

			// Token: 0x04009298 RID: 37528
			public int LastVoteDay;

			// Token: 0x04009299 RID: 37529
			public int Index = -1;
		}

		// Token: 0x02001875 RID: 6261
		public class VoteDayTimeRange
		{
			// Token: 0x0400929A RID: 37530
			public string Name;

			// Token: 0x0400929B RID: 37531
			public int StartHour;

			// Token: 0x0400929C RID: 37532
			public int EndHour;
		}
	}
}
