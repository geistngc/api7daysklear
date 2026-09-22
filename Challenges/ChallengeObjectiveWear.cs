using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001918 RID: 6424
	[Preserve]
	public class ChallengeObjectiveWear : BaseChallengeObjective
	{
		// Token: 0x17001894 RID: 6292
		// (get) Token: 0x0600C666 RID: 50790 RVA: 0x0017EB72 File Offset: 0x0017CD72
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Wear;
			}
		}

		// Token: 0x17001895 RID: 6293
		// (get) Token: 0x0600C667 RID: 50791 RVA: 0x00491848 File Offset: 0x0048FA48
		public override string DescriptionText
		{
			get
			{
				if (this.expectedItemClass == null)
				{
					return Localization.Get("challengeObjectiveWear", false, null) + " " + this.wearName + ":";
				}
				return Localization.Get("challengeObjectiveWear", false, null) + " " + this.expectedItemClass.GetLocalizedItemName() + ":";
			}
		}

		// Token: 0x0600C668 RID: 50792 RVA: 0x004918A5 File Offset: 0x0048FAA5
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
		}

		// Token: 0x0600C669 RID: 50793 RVA: 0x004918CC File Offset: 0x0048FACC
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.WearItem -= this.Current_WearItem;
			XUi xui = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player).xui;
			XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
			if (xui.PlayerEquipment.IsWearing(this.expectedItem))
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
				return;
			}
			QuestEventManager.Current.WearItem += this.Current_WearItem;
		}

		// Token: 0x0600C66A RID: 50794 RVA: 0x0049194D File Offset: 0x0048FB4D
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.WearItem -= this.Current_WearItem;
		}

		// Token: 0x0600C66B RID: 50795 RVA: 0x00491968 File Offset: 0x0048FB68
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_WearItem(ItemValue itemValue)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (!this.armorTags.IsEmpty && !this.armorTags.Test_AnySet(itemValue.ItemClass.ItemTags))
			{
				return;
			}
			if (this.itemClassID != "" && this.expectedItem.type != itemValue.type)
			{
				return;
			}
			base.Current = this.MaxCount;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600C66C RID: 50796 RVA: 0x004919E0 File Offset: 0x0048FBE0
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
			if (e.HasAttribute("tags"))
			{
				this.armorTags = FastTags<TagGroup.Global>.Parse(e.GetAttribute("tags"));
			}
			if (e.HasAttribute("wear_name_key"))
			{
				this.wearName = Localization.Get(e.GetAttribute("wear_name_key"), false, null);
				return;
			}
			if (e.HasAttribute("wear_name"))
			{
				this.wearName = e.GetAttribute("wear_name");
			}
		}

		// Token: 0x0600C66D RID: 50797 RVA: 0x00491AA4 File Offset: 0x0048FCA4
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveWear
			{
				itemClassID = this.itemClassID,
				armorTags = this.armorTags,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass,
				wearName = this.wearName
			};
		}

		// Token: 0x040095E1 RID: 38369
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x040095E2 RID: 38370
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x040095E3 RID: 38371
		public string itemClassID = "";

		// Token: 0x040095E4 RID: 38372
		[PublicizedFrom(EAccessModifier.Private)]
		public string wearName = "";

		// Token: 0x040095E5 RID: 38373
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> armorTags;
	}
}
