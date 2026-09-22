using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200190A RID: 6410
	[Preserve]
	public class ChallengeObjectiveHold : BaseChallengeObjective
	{
		// Token: 0x17001874 RID: 6260
		// (get) Token: 0x0600C5E7 RID: 50663 RVA: 0x00081502 File Offset: 0x0007F702
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Hold;
			}
		}

		// Token: 0x17001875 RID: 6261
		// (get) Token: 0x0600C5E8 RID: 50664 RVA: 0x0048FECC File Offset: 0x0048E0CC
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveHold", false, null) + " " + Localization.Get(this.itemClassList[0], false, null) + ":";
			}
		}

		// Token: 0x0600C5E9 RID: 50665 RVA: 0x0048FEF8 File Offset: 0x0048E0F8
		public override void Init()
		{
			this.itemClassList = this.itemClassID.Split(',', StringSplitOptions.None);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassList[0], false);
		}

		// Token: 0x0600C5EA RID: 50666 RVA: 0x0048FF22 File Offset: 0x0048E122
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.HoldItem -= this.Current_HoldItem;
			QuestEventManager.Current.HoldItem += this.Current_HoldItem;
		}

		// Token: 0x0600C5EB RID: 50667 RVA: 0x0048FF50 File Offset: 0x0048E150
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_HoldItem(ItemValue itemValue)
		{
			if (itemValue.ItemClass != null && this.itemClassList.ContainsCaseInsensitive(itemValue.ItemClass.Name))
			{
				base.Current = this.MaxCount;
			}
			else
			{
				base.Current = 0;
			}
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600C5EC RID: 50668 RVA: 0x0048FF8F File Offset: 0x0048E18F
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.HoldItem -= this.Current_HoldItem;
		}

		// Token: 0x0600C5ED RID: 50669 RVA: 0x0048FFA8 File Offset: 0x0048E1A8
		public override bool HandleCheckStatus()
		{
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			if (holdingItem != null)
			{
				base.Current = (this.itemClassList.ContainsCaseInsensitive(holdingItem.Name) ? this.MaxCount : 0);
			}
			base.Complete = this.CheckObjectiveComplete(false);
			return base.Complete;
		}

		// Token: 0x0600C5EE RID: 50670 RVA: 0x00490008 File Offset: 0x0048E208
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
		}

		// Token: 0x0600C5EF RID: 50671 RVA: 0x00490039 File Offset: 0x0048E239
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveHold
			{
				itemClassID = this.itemClassID,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x040095B1 RID: 38321
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x040095B2 RID: 38322
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x040095B3 RID: 38323
		public string itemClassID = "";

		// Token: 0x040095B4 RID: 38324
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemClassList;
	}
}
