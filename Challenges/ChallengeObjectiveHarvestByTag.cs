using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001909 RID: 6409
	[Preserve]
	public class ChallengeObjectiveHarvestByTag : ChallengeBaseTrackedItemObjective
	{
		// Token: 0x17001872 RID: 6258
		// (get) Token: 0x0600C5DA RID: 50650 RVA: 0x00132045 File Offset: 0x00130245
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Harvest;
			}
		}

		// Token: 0x17001873 RID: 6259
		// (get) Token: 0x0600C5DB RID: 50651 RVA: 0x0048F99B File Offset: 0x0048DB9B
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveHarvest", false, null) + " " + Localization.Get(this.targetName, false, null) + ":";
			}
		}

		// Token: 0x0600C5DC RID: 50652 RVA: 0x0048F9C5 File Offset: 0x0048DBC5
		public override void Init()
		{
			this.harvestTags = FastTags<TagGroup.Global>.Parse(this.harvestTag);
			this.expectedHeldClass = ItemClass.GetItemClass(this.heldItemClassID, false);
		}

		// Token: 0x0600C5DD RID: 50653 RVA: 0x0048F9EA File Offset: 0x0048DBEA
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C5DE RID: 50654 RVA: 0x0048F9F8 File Offset: 0x0048DBF8
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			if (!this.requireHeld)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupHold(this.heldItemClassID));
		}

		// Token: 0x0600C5DF RID: 50655 RVA: 0x0048FA24 File Offset: 0x0048DC24
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.HarvestItem -= this.Current_HarvestItem;
			QuestEventManager.Current.HarvestItem += this.Current_HarvestItem;
			base.HandleAddHooks();
			if (this.overrideTrackerIndexName != null && this.overrideTrackerIndexName != null && this.trackingEntry == null && !this.disableTracking)
			{
				this.trackingEntry = new TrackingEntry
				{
					TrackedItem = this.expectedItemClass,
					Owner = this,
					blockIndexName = this.overrideTrackerIndexName,
					navObjectName = ((this.overrideNavObject != "") ? this.overrideNavObject : "quest_resource"),
					trackDistance = this.trackDistance
				};
				this.trackingEntry.TrackingHelper = this.Owner.GetTrackingHelper();
			}
		}

		// Token: 0x0600C5E0 RID: 50656 RVA: 0x0048FAFA File Offset: 0x0048DCFA
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.HarvestItem -= this.Current_HarvestItem;
		}

		// Token: 0x0600C5E1 RID: 50657 RVA: 0x0048FB14 File Offset: 0x0048DD14
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_HarvestItem(ItemValue held, ItemStack stack, BlockValue bv)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (held.ItemClass == this.expectedHeldClass || !this.requireHeld)
			{
				if (bv.isair && this.isBlock)
				{
					return;
				}
				if ((!this.isBlock || this.blockTag.IsEmpty || bv.Block.HasAnyFastTags(this.blockTag)) && stack.itemValue.ItemClass.ItemTags.Test_AnySet(this.harvestTags))
				{
					if (base.Current + stack.count > this.MaxCount)
					{
						base.Current = this.MaxCount;
					}
					else
					{
						base.Current += stack.count;
					}
					this.CheckObjectiveComplete(true);
					if (base.Complete && this.IsTracking && this.trackingEntry != null)
					{
						this.trackingEntry.RemoveHooks();
					}
				}
			}
		}

		// Token: 0x0600C5E2 RID: 50658 RVA: 0x0048FBFC File Offset: 0x0048DDFC
		public override void HandleTrackingStarted()
		{
			base.HandleTrackingStarted();
			if (this.trackingEntry != null)
			{
				this.Owner.AddTrackingEntry(this.trackingEntry);
				this.trackingEntry.TrackingHelper = this.Owner.TrackingHandler;
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5E3 RID: 50659 RVA: 0x0048E875 File Offset: 0x0048CA75
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
			if (this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C5E4 RID: 50660 RVA: 0x0048FC4C File Offset: 0x0048DE4C
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("block_tag"))
			{
				this.blockTag = FastTags<TagGroup.Global>.Parse(e.GetAttribute("block_tag"));
			}
			if (e.HasAttribute("harvest_tags"))
			{
				this.harvestTag = e.GetAttribute("harvest_tags");
			}
			if (e.HasAttribute("target_name_key"))
			{
				this.targetName = Localization.Get(e.GetAttribute("target_name_key"), false, null);
			}
			else if (e.HasAttribute("target_name"))
			{
				this.targetName = e.GetAttribute("target_name");
			}
			if (e.HasAttribute("held"))
			{
				this.heldItemClassID = e.GetAttribute("held");
			}
			if (e.HasAttribute("is_block"))
			{
				this.isBlock = StringParsers.ParseBool(e.GetAttribute("is_block"), 0, -1, true);
			}
			if (e.HasAttribute("required_held"))
			{
				this.requireHeld = StringParsers.ParseBool(e.GetAttribute("required_held"), 0, -1, true);
			}
			if (e.HasAttribute("override_nav_object"))
			{
				this.overrideNavObject = e.GetAttribute("override_nav_object");
			}
		}

		// Token: 0x0600C5E5 RID: 50661 RVA: 0x0048FDC0 File Offset: 0x0048DFC0
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveHarvestByTag
			{
				itemClassID = this.itemClassID,
				heldItemClassID = this.heldItemClassID,
				overrideTrackerIndexName = this.overrideTrackerIndexName,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass,
				expectedHeldClass = this.expectedHeldClass,
				requireHeld = this.requireHeld,
				blockTag = this.blockTag,
				isBlock = this.isBlock,
				harvestTag = this.harvestTag,
				harvestTags = this.harvestTags,
				targetName = this.targetName,
				overrideNavObject = this.overrideNavObject
			};
		}

		// Token: 0x040095A8 RID: 38312
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedHeldClass;

		// Token: 0x040095A9 RID: 38313
		[PublicizedFrom(EAccessModifier.Private)]
		public string heldItemClassID = "";

		// Token: 0x040095AA RID: 38314
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isBlock = true;

		// Token: 0x040095AB RID: 38315
		[PublicizedFrom(EAccessModifier.Private)]
		public bool requireHeld;

		// Token: 0x040095AC RID: 38316
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> blockTag = FastTags<TagGroup.Global>.none;

		// Token: 0x040095AD RID: 38317
		[PublicizedFrom(EAccessModifier.Private)]
		public string harvestTag = "";

		// Token: 0x040095AE RID: 38318
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> harvestTags = FastTags<TagGroup.Global>.none;

		// Token: 0x040095AF RID: 38319
		[PublicizedFrom(EAccessModifier.Private)]
		public string targetName = "";

		// Token: 0x040095B0 RID: 38320
		[PublicizedFrom(EAccessModifier.Private)]
		public string overrideNavObject = "";
	}
}
