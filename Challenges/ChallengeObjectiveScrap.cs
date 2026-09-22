using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001910 RID: 6416
	[Preserve]
	public class ChallengeObjectiveScrap : BaseChallengeObjective
	{
		// Token: 0x17001880 RID: 6272
		// (get) Token: 0x0600C61D RID: 50717 RVA: 0x000FA766 File Offset: 0x000F8966
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Scrap;
			}
		}

		// Token: 0x17001881 RID: 6273
		// (get) Token: 0x0600C61E RID: 50718 RVA: 0x00490ADC File Offset: 0x0048ECDC
		public override string DescriptionText
		{
			get
			{
				string str = (this.expectedItemClass != null) ? this.expectedItemClass.GetLocalizedItemName() : Localization.Get("xuiItems", false, null);
				return Localization.Get("challengeObjectiveScrap", false, null) + " " + str + ":";
			}
		}

		// Token: 0x0600C61F RID: 50719 RVA: 0x00490B27 File Offset: 0x0048ED27
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
		}

		// Token: 0x0600C620 RID: 50720 RVA: 0x00490B4D File Offset: 0x0048ED4D
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ScrapItem += this.Current_ScrapItem;
		}

		// Token: 0x0600C621 RID: 50721 RVA: 0x00490B68 File Offset: 0x0048ED68
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ScrapItem(ItemStack stack)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.expectedItemClass == null || stack.itemValue.type == this.expectedItem.type)
			{
				base.Current += stack.count;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C622 RID: 50722 RVA: 0x00490BB9 File Offset: 0x0048EDB9
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ScrapItem -= this.Current_ScrapItem;
		}

		// Token: 0x0600C623 RID: 50723 RVA: 0x00490BD1 File Offset: 0x0048EDD1
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
		}

		// Token: 0x0600C624 RID: 50724 RVA: 0x00490C02 File Offset: 0x0048EE02
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveScrap
			{
				itemClassID = this.itemClassID,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x040095C5 RID: 38341
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x040095C6 RID: 38342
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x040095C7 RID: 38343
		public string itemClassID = "";
	}
}
