using System;

namespace Twitch
{
	// Token: 0x0200182F RID: 6191
	public class BaseTwitchEventEntry
	{
		// Token: 0x0600BF2B RID: 48939 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return false;
		}

		// Token: 0x0600BF2C RID: 48940 RVA: 0x0046BE5E File Offset: 0x0046A05E
		public virtual string Description(TwitchEventActionEntry entry)
		{
			return string.Format("{0}({1})", this.EventTitle, entry.Count);
		}

		// Token: 0x0600BF2D RID: 48941 RVA: 0x0046BE7C File Offset: 0x0046A07C
		public virtual void HandleInstant(string username, TwitchManager tm)
		{
			if (this.PimpPotAdd > 0)
			{
				tm.AddToPot(this.PimpPotAdd);
			}
			if (this.BitPotAdd > 0)
			{
				tm.AddToBitPot(this.BitPotAdd);
			}
			if (this.CooldownAdd > 0)
			{
				tm.AddCooldownAmount(this.CooldownAdd);
			}
			if (this.PPAmount > 0 || this.SPAmount > 0)
			{
				if (username == "")
				{
					tm.ViewerData.AddPointsAll(this.PPAmount, this.SPAmount, true);
					return;
				}
				ViewerEntry viewerEntry = tm.ViewerData.GetViewerEntry(username);
				viewerEntry.SpecialPoints += (float)this.SPAmount;
				viewerEntry.StandardPoints += (float)this.PPAmount;
			}
		}

		// Token: 0x0600BF2E RID: 48942 RVA: 0x0046BF34 File Offset: 0x0046A134
		public virtual bool HandleEvent(string username, TwitchManager tm)
		{
			if (this.EventName == "")
			{
				return true;
			}
			if (!this.SafeAllowed && tm.IsSafe)
			{
				return false;
			}
			if (TwitchManager.BossHordeActive)
			{
				return false;
			}
			TwitchManager.CooldownTypes cooldownType = tm.CooldownType;
			if (cooldownType != TwitchManager.CooldownTypes.None)
			{
				if (cooldownType == TwitchManager.CooldownTypes.Startup)
				{
					if (!this.StartingCooldownAllowed)
					{
						return false;
					}
				}
				else if (!this.CooldownAllowed)
				{
					return false;
				}
			}
			if (!this.VoteEventAllowed && tm.VotingManager.VotingIsActive)
			{
				return false;
			}
			if (GameEventManager.Current.HandleAction(this.EventName, tm.LocalPlayer, tm.LocalPlayer, false, username, "event", tm.AllowCrateSharing, false, "", null))
			{
				GameEventManager.Current.HandleGameEventApproved(this.EventName, tm.LocalPlayer.entityId, username, "event");
				return true;
			}
			return false;
		}

		// Token: 0x04008FD2 RID: 36818
		public string EventName = "";

		// Token: 0x04008FD3 RID: 36819
		public string EventTitle = "";

		// Token: 0x04008FD4 RID: 36820
		public bool SafeAllowed = true;

		// Token: 0x04008FD5 RID: 36821
		public bool StartingCooldownAllowed;

		// Token: 0x04008FD6 RID: 36822
		public bool CooldownAllowed = true;

		// Token: 0x04008FD7 RID: 36823
		public bool VoteEventAllowed = true;

		// Token: 0x04008FD8 RID: 36824
		public bool RewardsBitPot;

		// Token: 0x04008FD9 RID: 36825
		public int PPAmount;

		// Token: 0x04008FDA RID: 36826
		public int SPAmount;

		// Token: 0x04008FDB RID: 36827
		public int PimpPotAdd;

		// Token: 0x04008FDC RID: 36828
		public int BitPotAdd;

		// Token: 0x04008FDD RID: 36829
		public int CooldownAdd;

		// Token: 0x04008FDE RID: 36830
		public BaseTwitchEventEntry.EventTypes EventType;

		// Token: 0x02001830 RID: 6192
		public enum EventTypes
		{
			// Token: 0x04008FE0 RID: 36832
			Bits,
			// Token: 0x04008FE1 RID: 36833
			Subs,
			// Token: 0x04008FE2 RID: 36834
			GiftSubs,
			// Token: 0x04008FE3 RID: 36835
			Raid,
			// Token: 0x04008FE4 RID: 36836
			Charity,
			// Token: 0x04008FE5 RID: 36837
			ChannelPoints,
			// Token: 0x04008FE6 RID: 36838
			HypeTrain,
			// Token: 0x04008FE7 RID: 36839
			CreatorGoal
		}
	}
}
