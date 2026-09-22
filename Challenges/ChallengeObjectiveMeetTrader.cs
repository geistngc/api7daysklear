using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200190E RID: 6414
	[Preserve]
	public class ChallengeObjectiveMeetTrader : BaseChallengeObjective
	{
		// Token: 0x1700187C RID: 6268
		// (get) Token: 0x0600C60B RID: 50699 RVA: 0x000B9709 File Offset: 0x000B7909
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.MeetTrader;
			}
		}

		// Token: 0x1700187D RID: 6269
		// (get) Token: 0x0600C60C RID: 50700 RVA: 0x004907AC File Offset: 0x0048E9AC
		public override string DescriptionText
		{
			get
			{
				if (string.IsNullOrEmpty(this.TraderName))
				{
					return Localization.Get("challengeObjectiveMeetAnyTrader", false, null);
				}
				return Localization.Get("challengeObjectiveMeet", false, null) + " " + Localization.Get(this.TraderName, false, null) + ":";
			}
		}

		// Token: 0x0600C60D RID: 50701 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C60E RID: 50702 RVA: 0x004907FB File Offset: 0x0048E9FB
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.NPCMeet += this.Current_NPCMeet;
		}

		// Token: 0x0600C60F RID: 50703 RVA: 0x00490813 File Offset: 0x0048EA13
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.NPCMeet -= this.Current_NPCMeet;
		}

		// Token: 0x0600C610 RID: 50704 RVA: 0x0049082C File Offset: 0x0048EA2C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_NPCMeet(EntityNPC npc)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || npc.EntityName == this.TraderName)
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C611 RID: 50705 RVA: 0x00490899 File Offset: 0x0048EA99
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("trader_name"))
			{
				this.TraderName = e.GetAttribute("trader_name");
			}
		}

		// Token: 0x0600C612 RID: 50706 RVA: 0x004908CA File Offset: 0x0048EACA
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveMeetTrader
			{
				TraderName = this.TraderName
			};
		}

		// Token: 0x040095C0 RID: 38336
		[PublicizedFrom(EAccessModifier.Private)]
		public string TraderName = "";
	}
}
