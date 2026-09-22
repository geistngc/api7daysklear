using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001903 RID: 6403
	[Preserve]
	public class ChallengeObjectiveCureDebuff : BaseChallengeObjective
	{
		// Token: 0x17001865 RID: 6245
		// (get) Token: 0x0600C590 RID: 50576 RVA: 0x00080A2D File Offset: 0x0007EC2D
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.CureDebuff;
			}
		}

		// Token: 0x17001866 RID: 6246
		// (get) Token: 0x0600C591 RID: 50577 RVA: 0x0048E3B8 File Offset: 0x0048C5B8
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveCure", false, null) + " " + BuffManager.GetBuff(this.buffName).LocalizedName + ":";
			}
		}

		// Token: 0x0600C592 RID: 50578 RVA: 0x0048E3E8 File Offset: 0x0048C5E8
		public override void Init()
		{
			if (this.buffName != null)
			{
				this.buffNames = this.buffName.Split(',', StringSplitOptions.None);
				if (this.buffNames.Length > 1)
				{
					this.buffName = this.buffNames[0];
				}
			}
			if (this.itemName != null)
			{
				this.itemNames = this.itemName.Split(',', StringSplitOptions.None);
				if (this.itemNames.Length > 1)
				{
					this.itemName = this.itemNames[0];
				}
			}
		}

		// Token: 0x0600C593 RID: 50579 RVA: 0x0048E45F File Offset: 0x0048C65F
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.UseItem += this.Current_UseItem;
		}

		// Token: 0x0600C594 RID: 50580 RVA: 0x0048E477 File Offset: 0x0048C677
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.UseItem -= this.Current_UseItem;
		}

		// Token: 0x0600C595 RID: 50581 RVA: 0x0048E490 File Offset: 0x0048C690
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_UseItem(ItemValue itemValue)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.itemNames.ContainsCaseInsensitive(itemValue.ItemClass.Name) && this.PlayerHasBuff())
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

		// Token: 0x0600C596 RID: 50582 RVA: 0x0048E4F8 File Offset: 0x0048C6F8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool PlayerHasBuff()
		{
			EntityBuffs buffs = this.Owner.Owner.Player.Buffs;
			for (int i = 0; i < this.buffNames.Length; i++)
			{
				if (buffs.HasBuff(this.buffNames[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600C597 RID: 50583 RVA: 0x0048E544 File Offset: 0x0048C744
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("debuff"))
			{
				this.buffName = e.GetAttribute("debuff");
			}
			if (e.HasAttribute("item"))
			{
				this.itemName = e.GetAttribute("item");
			}
		}

		// Token: 0x0600C598 RID: 50584 RVA: 0x0048E5A8 File Offset: 0x0048C7A8
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveCureDebuff
			{
				buffName = this.buffName,
				buffNames = this.buffNames,
				itemName = this.itemName,
				itemNames = this.itemNames
			};
		}

		// Token: 0x04009593 RID: 38291
		[PublicizedFrom(EAccessModifier.Private)]
		public string buffName = "";

		// Token: 0x04009594 RID: 38292
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] buffNames;

		// Token: 0x04009595 RID: 38293
		[PublicizedFrom(EAccessModifier.Private)]
		public string itemName = "";

		// Token: 0x04009596 RID: 38294
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemNames;
	}
}
