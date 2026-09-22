using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001917 RID: 6423
	[Preserve]
	public class ChallengeObjectiveUseItem : BaseChallengeObjective
	{
		// Token: 0x17001892 RID: 6290
		// (get) Token: 0x0600C65D RID: 50781 RVA: 0x00268A1D File Offset: 0x00266C1D
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Use;
			}
		}

		// Token: 0x17001893 RID: 6291
		// (get) Token: 0x0600C65E RID: 50782 RVA: 0x004915E4 File Offset: 0x0048F7E4
		public override string DescriptionText
		{
			get
			{
				string str = (this.overrideText != "") ? this.overrideText : Localization.Get(this.itemName, false, null);
				return Localization.Get("challengeObjectiveUse", false, null) + " " + str + ":";
			}
		}

		// Token: 0x0600C65F RID: 50783 RVA: 0x00491635 File Offset: 0x0048F835
		public override void Init()
		{
			if (this.itemName != null)
			{
				this.itemNames = this.itemName.Split(',', StringSplitOptions.None);
				if (this.itemNames.Length > 1)
				{
					this.itemName = this.itemNames[0];
				}
			}
		}

		// Token: 0x0600C660 RID: 50784 RVA: 0x0049166C File Offset: 0x0048F86C
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.UseItem += this.Current_UseItem;
		}

		// Token: 0x0600C661 RID: 50785 RVA: 0x00491684 File Offset: 0x0048F884
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.UseItem -= this.Current_UseItem;
		}

		// Token: 0x0600C662 RID: 50786 RVA: 0x0049169C File Offset: 0x0048F89C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_UseItem(ItemValue itemValue)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.itemNames.ContainsCaseInsensitive(itemValue.ItemClass.Name) || (!this.itemTags.IsEmpty && itemValue.ItemClass.ItemTags.Test_AnySet(this.itemTags)))
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

		// Token: 0x0600C663 RID: 50787 RVA: 0x00491724 File Offset: 0x0048F924
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemName = e.GetAttribute("item");
			}
			if (e.HasAttribute("item_tags"))
			{
				this.itemTags = FastTags<TagGroup.Global>.Parse(e.GetAttribute("item_tags"));
			}
			if (e.HasAttribute("override_text_key"))
			{
				this.overrideText = Localization.Get(e.GetAttribute("override_text_key"), false, null);
				return;
			}
			if (e.HasAttribute("override_text"))
			{
				this.overrideText = e.GetAttribute("override_text");
			}
		}

		// Token: 0x0600C664 RID: 50788 RVA: 0x004917E5 File Offset: 0x0048F9E5
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveUseItem
			{
				itemName = this.itemName,
				itemNames = this.itemNames,
				itemTags = this.itemTags,
				overrideText = this.overrideText
			};
		}

		// Token: 0x040095DD RID: 38365
		[PublicizedFrom(EAccessModifier.Private)]
		public string itemName = "";

		// Token: 0x040095DE RID: 38366
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemNames;

		// Token: 0x040095DF RID: 38367
		[PublicizedFrom(EAccessModifier.Private)]
		public string overrideText = "";

		// Token: 0x040095E0 RID: 38368
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> itemTags = FastTags<TagGroup.Global>.none;
	}
}
