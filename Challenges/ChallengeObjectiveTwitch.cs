using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001916 RID: 6422
	[Preserve]
	public class ChallengeObjectiveTwitch : BaseChallengeObjective
	{
		// Token: 0x1700188F RID: 6287
		// (get) Token: 0x0600C654 RID: 50772 RVA: 0x004913B2 File Offset: 0x0048F5B2
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Twitch;
			}
		}

		// Token: 0x17001890 RID: 6288
		// (get) Token: 0x0600C655 RID: 50773 RVA: 0x004913B8 File Offset: 0x0048F5B8
		public override string DescriptionText
		{
			get
			{
				switch (this.TwitchObjectiveType)
				{
				case TwitchObjectiveTypes.Enabled:
					return Localization.Get("challengeObjectiveTwitchEnabled", false, null);
				case TwitchObjectiveTypes.EnableExtras:
					return Localization.Get("challengeObjectiveTwitchEnableExtras", false, null);
				case TwitchObjectiveTypes.HelperReward:
					return Localization.Get("challengeObjectiveTwitchHelperRewards", false, null);
				case TwitchObjectiveTypes.ChannelPointRedeems:
					return Localization.Get("challengeObjectiveTwitchChannelPointRedeems", false, null);
				case TwitchObjectiveTypes.VoteComplete:
					return Localization.Get("challengeObjectiveTwitchVotesCompleted", false, null);
				case TwitchObjectiveTypes.PimpPot:
					return Localization.Get("challengeObjectiveTwitchPimpPotRewarded", false, null);
				case TwitchObjectiveTypes.BitPot:
					return Localization.Get("challengeObjectiveTwitchBitPotRewarded", false, null);
				case TwitchObjectiveTypes.DefeatBossHorde:
					return Localization.Get("challengeObjectiveTwitchBossHordesDefeated", false, null);
				case TwitchObjectiveTypes.GoodAction:
					return Localization.Get("challengeObjectiveTwitchGoodActions", false, null);
				case TwitchObjectiveTypes.BadAction:
					return Localization.Get("challengeObjectiveTwitchBadActions", false, null);
				default:
					return "";
				}
			}
		}

		// Token: 0x17001891 RID: 6289
		// (get) Token: 0x0600C656 RID: 50774 RVA: 0x00491486 File Offset: 0x0048F686
		public override ChallengeClass.UINavTypes NavType
		{
			get
			{
				if (this.TwitchObjectiveType == TwitchObjectiveTypes.EnableExtras)
				{
					return ChallengeClass.UINavTypes.TwitchActions;
				}
				return ChallengeClass.UINavTypes.None;
			}
		}

		// Token: 0x0600C657 RID: 50775 RVA: 0x00491494 File Offset: 0x0048F694
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.TwitchEventReceive += this.Current_TwitchEventReceive;
		}

		// Token: 0x0600C658 RID: 50776 RVA: 0x004914AC File Offset: 0x0048F6AC
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.TwitchEventReceive -= this.Current_TwitchEventReceive;
		}

		// Token: 0x0600C659 RID: 50777 RVA: 0x004914C4 File Offset: 0x0048F6C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TwitchEventReceive(TwitchObjectiveTypes action, string param)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (action == this.TwitchObjectiveType && (this.Param == "" || this.Param.EqualsCaseInsensitive(param)))
			{
				int num = base.Current;
				base.Current = num + 1;
			}
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C65A RID: 50778 RVA: 0x00491538 File Offset: 0x0048F738
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("objective_type"))
			{
				this.TwitchObjectiveType = (TwitchObjectiveTypes)Enum.Parse(typeof(TwitchObjectiveTypes), e.GetAttribute("objective_type"), true);
			}
			if (e.HasAttribute("objective_param"))
			{
				this.Param = e.GetAttribute("objective_param");
			}
		}

		// Token: 0x0600C65B RID: 50779 RVA: 0x004915B1 File Offset: 0x0048F7B1
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveTwitch
			{
				TwitchObjectiveType = this.TwitchObjectiveType,
				Param = this.Param
			};
		}

		// Token: 0x040095DB RID: 38363
		public TwitchObjectiveTypes TwitchObjectiveType;

		// Token: 0x040095DC RID: 38364
		public string Param = "";
	}
}
