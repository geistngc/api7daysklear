using System;

namespace Twitch
{
	// Token: 0x020017F4 RID: 6132
	public class TwitchActionHistoryEntry
	{
		// Token: 0x0600BDFB RID: 48635 RVA: 0x00466BC4 File Offset: 0x00464DC4
		public TwitchActionHistoryEntry(string username, string usercolor, TwitchAction action, TwitchVote vote, TwitchEventActionEntry eventEntry)
		{
			this.UserName = username;
			this.Action = action;
			this.Vote = vote;
			this.EventEntry = eventEntry;
			this.UserColor = usercolor;
			ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int item3 = valueTuple.Item3;
			this.ActionTime = string.Format("{0} {1}, {2:00}:{3:00}", new object[]
			{
				Localization.Get("xuiDay", false, null),
				item,
				item2,
				item3
			});
		}

		// Token: 0x1700170D RID: 5901
		// (get) Token: 0x0600BDFC RID: 48636 RVA: 0x00466C64 File Offset: 0x00464E64
		public string Command
		{
			get
			{
				if (this.Action != null)
				{
					return string.Format("{0}({1})", this.Action.Command, this.PointsSpent);
				}
				if (this.Vote != null)
				{
					return this.Vote.VoteDescription;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.Description(this.EventEntry);
				}
				return "";
			}
		}

		// Token: 0x1700170E RID: 5902
		// (get) Token: 0x0600BDFD RID: 48637 RVA: 0x00466CD4 File Offset: 0x00464ED4
		public string Title
		{
			get
			{
				if (this.Action != null)
				{
					return this.Action.Title;
				}
				if (this.Vote != null)
				{
					return this.Vote.VoteDescription;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.EventTitle;
				}
				return "";
			}
		}

		// Token: 0x1700170F RID: 5903
		// (get) Token: 0x0600BDFE RID: 48638 RVA: 0x00466D28 File Offset: 0x00464F28
		public string Description
		{
			get
			{
				if (this.Action != null)
				{
					return this.Action.Description;
				}
				if (this.Vote != null)
				{
					return this.Vote.Description;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.EventTitle;
				}
				return "";
			}
		}

		// Token: 0x17001710 RID: 5904
		// (get) Token: 0x0600BDFF RID: 48639 RVA: 0x00466D7B File Offset: 0x00464F7B
		public string HistoryType
		{
			get
			{
				if (this.Action != null)
				{
					return "action";
				}
				if (this.Vote != null)
				{
					return "vote";
				}
				if (this.EventEntry != null)
				{
					return "event";
				}
				return "";
			}
		}

		// Token: 0x17001711 RID: 5905
		// (get) Token: 0x0600BE00 RID: 48640 RVA: 0x00466DAC File Offset: 0x00464FAC
		public bool IsRefunded
		{
			get
			{
				return this.EntryState == TwitchActionHistoryEntry.EntryStates.Refunded;
			}
		}

		// Token: 0x0600BE01 RID: 48641 RVA: 0x00466DB7 File Offset: 0x00464FB7
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool IsValid()
		{
			return this.UserName != null && ((this.Action != null && this.Action.Command != null) || this.Vote != null || this.EventEntry != null);
		}

		// Token: 0x0600BE02 RID: 48642 RVA: 0x00466DEC File Offset: 0x00464FEC
		[PublicizedFrom(EAccessModifier.Internal)]
		public void Refund()
		{
			if (this.EntryState != TwitchActionHistoryEntry.EntryStates.Refunded)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				twitchManager.ViewerData.ReimburseAction(this.UserName, this.PointsSpent, this.Action);
				twitchManager.LocalPlayer.PlayOneShot("ui_vending_purchase", false, false, false, null, 1f);
				this.EntryState = TwitchActionHistoryEntry.EntryStates.Refunded;
			}
		}

		// Token: 0x0600BE03 RID: 48643 RVA: 0x00466E44 File Offset: 0x00465044
		public void Retry()
		{
			if (!this.HasRetried)
			{
				if (this.Action != null)
				{
					TwitchManager.Current.HandleExtensionMessage(this.UserID, string.Format("{0} {1}", this.Action.Command, this.Target), true, 0, 0);
				}
				else if (this.EventEntry != null)
				{
					this.EventEntry.IsSent = false;
					this.EventEntry.IsRetry = true;
					TwitchManager.Current.EventQueue.Add(this.EventEntry);
				}
				this.HasRetried = true;
			}
		}

		// Token: 0x0600BE04 RID: 48644 RVA: 0x00466ED0 File Offset: 0x004650D0
		public bool CanRetry()
		{
			if (this.HasRetried)
			{
				return false;
			}
			TwitchManager.CooldownTypes cooldownType = TwitchManager.Current.CooldownType;
			if (this.Action != null)
			{
				if (TwitchManager.Current.VotingManager.VotingIsActive)
				{
					return false;
				}
				if (!this.Action.IgnoreCooldown)
				{
					return cooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && cooldownType != TwitchManager.CooldownTypes.Time && cooldownType != TwitchManager.CooldownTypes.QuestDisabled && ((cooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && cooldownType != TwitchManager.CooldownTypes.SafeCooldown) || !this.Action.WaitingBlocked) && !this.HasRetried;
				}
				return !this.HasRetried;
			}
			else
			{
				if (this.EventEntry != null)
				{
					if (!this.EventEntry.Event.CooldownAllowed)
					{
						if (cooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || cooldownType == TwitchManager.CooldownTypes.Time || cooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return false;
						}
						if (cooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting)
						{
							return false;
						}
					}
					return (this.EventEntry.Event.StartingCooldownAllowed || cooldownType != TwitchManager.CooldownTypes.Startup) && (this.EventEntry.Event.VoteEventAllowed || !TwitchManager.Current.VotingManager.VotingIsActive) && !this.HasRetried;
				}
				return false;
			}
		}

		// Token: 0x0600BE05 RID: 48645 RVA: 0x00466FCA File Offset: 0x004651CA
		public bool CanRefund()
		{
			return this.PointsSpent > 0 && this.EntryState != TwitchActionHistoryEntry.EntryStates.Refunded && this.EntryState != TwitchActionHistoryEntry.EntryStates.Reimbursed && this.Action != null;
		}

		// Token: 0x04008EAE RID: 36526
		public string UserName;

		// Token: 0x04008EAF RID: 36527
		public string UserColor;

		// Token: 0x04008EB0 RID: 36528
		public string Target;

		// Token: 0x04008EB1 RID: 36529
		public int UserID;

		// Token: 0x04008EB2 RID: 36530
		public TwitchAction Action;

		// Token: 0x04008EB3 RID: 36531
		public TwitchVote Vote;

		// Token: 0x04008EB4 RID: 36532
		public TwitchActionEntry ActionEntry;

		// Token: 0x04008EB5 RID: 36533
		public TwitchEventActionEntry EventEntry;

		// Token: 0x04008EB6 RID: 36534
		public int PointsSpent;

		// Token: 0x04008EB7 RID: 36535
		public bool HasRetried;

		// Token: 0x04008EB8 RID: 36536
		public string ActionTime;

		// Token: 0x04008EB9 RID: 36537
		public TwitchActionHistoryEntry.EntryStates EntryState;

		// Token: 0x020017F5 RID: 6133
		public enum EntryStates
		{
			// Token: 0x04008EBB RID: 36539
			Waiting,
			// Token: 0x04008EBC RID: 36540
			Completed,
			// Token: 0x04008EBD RID: 36541
			Reimbursed,
			// Token: 0x04008EBE RID: 36542
			Despawned,
			// Token: 0x04008EBF RID: 36543
			Refunded
		}
	}
}
