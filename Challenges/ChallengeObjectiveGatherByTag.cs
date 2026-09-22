using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001906 RID: 6406
	[Preserve]
	public class ChallengeObjectiveGatherByTag : ChallengeBaseTrackedItemObjective
	{
		// Token: 0x1700186B RID: 6251
		// (get) Token: 0x0600C5B1 RID: 50609 RVA: 0x0048EB4D File Offset: 0x0048CD4D
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.GatherByTag;
			}
		}

		// Token: 0x1700186C RID: 6252
		// (get) Token: 0x0600C5B2 RID: 50610 RVA: 0x0048EB51 File Offset: 0x0048CD51
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveGather", false, null) + " " + Localization.Get(this.targetName, false, null) + ":";
			}
		}

		// Token: 0x0600C5B3 RID: 50611 RVA: 0x0048EB7B File Offset: 0x0048CD7B
		public override void Init()
		{
			this.gatherTags = FastTags<TagGroup.Global>.Parse(this.gatherTag);
		}

		// Token: 0x0600C5B4 RID: 50612 RVA: 0x0048EB90 File Offset: 0x0048CD90
		public override void HandleAddHooks()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player).xui.PlayerInventory;
			playerInventory.Backpack.OnBackpackItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Backpack.OnBackpackItemsChangedInternal += this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal += this.ItemsChangedInternal;
			player.DragAndDropItemChanged -= this.ItemsChangedInternal;
			player.DragAndDropItemChanged += this.ItemsChangedInternal;
			base.HandleAddHooks();
			this.ItemsChangedInternal();
			if (this.IsRequirement && this.trackingEntry != null)
			{
				this.Owner.AddTrackingEntry(this.trackingEntry);
				this.trackingEntry.TrackingHelper = this.Owner.TrackingHandler;
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5B5 RID: 50613 RVA: 0x0048EC98 File Offset: 0x0048CE98
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

		// Token: 0x0600C5B6 RID: 50614 RVA: 0x0048E875 File Offset: 0x0048CA75
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
			if (this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C5B7 RID: 50615 RVA: 0x0048ECE5 File Offset: 0x0048CEE5
		public override bool CheckObjectiveComplete(bool handleComplete = true)
		{
			if (this.CheckForNeededItem())
			{
				base.Complete = true;
				base.Current = this.MaxCount;
				if (handleComplete)
				{
					this.Owner.HandleComplete(true, false);
				}
				return true;
			}
			base.Complete = false;
			return base.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0600C5B8 RID: 50616 RVA: 0x0048ED24 File Offset: 0x0048CF24
		[PublicizedFrom(EAccessModifier.Private)]
		public void ItemsChangedInternal()
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.CheckObjectiveComplete(true))
			{
				if (this.IsTracking && this.trackingEntry != null)
				{
					this.trackingEntry.RemoveHooks();
				}
				if (this.IsRequirement)
				{
					this.Parent.CheckPrerequisites();
					return;
				}
			}
			else if (this.IsTracking && this.trackingEntry != null)
			{
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5B9 RID: 50617 RVA: 0x0048E949 File Offset: 0x0048CB49
		public override void UpdateStatus()
		{
			base.UpdateStatus();
			if (base.Complete)
			{
				if (this.trackingEntry != null)
				{
					this.trackingEntry.RemoveHooks();
					return;
				}
			}
			else if (this.trackingEntry != null)
			{
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5BA RID: 50618 RVA: 0x0048ED90 File Offset: 0x0048CF90
		public override void HandleRemoveHooks()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			if (player == null)
			{
				return;
			}
			LocalPlayerUI.GetUIForPlayer(player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			playerInventory.Backpack.OnBackpackItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal -= this.ItemsChangedInternal;
			player.DragAndDropItemChanged -= this.ItemsChangedInternal;
			if (this.IsRequirement && this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C5BB RID: 50619 RVA: 0x0048EE3C File Offset: 0x0048D03C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleUpdatingCurrent()
		{
			base.HandleUpdatingCurrent();
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player).xui.PlayerInventory;
			int num = playerInventory.Backpack.GetItemCount(this.gatherTags, -1, -1, true);
			num += playerInventory.Toolbelt.GetItemCount(this.gatherTags, -1, -1, true);
			if (num > this.MaxCount)
			{
				num = this.MaxCount;
			}
			if (this.current != num)
			{
				base.Current = num;
			}
		}

		// Token: 0x0600C5BC RID: 50620 RVA: 0x0048EEBC File Offset: 0x0048D0BC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckForNeededItem()
		{
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player).xui.PlayerInventory;
			return playerInventory.Backpack.GetItemCount(this.gatherTags, -1, -1, true) + playerInventory.Toolbelt.GetItemCount(this.gatherTags, -1, -1, true) >= this.MaxCount;
		}

		// Token: 0x0600C5BD RID: 50621 RVA: 0x0048EF20 File Offset: 0x0048D120
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("gather_tags"))
			{
				this.gatherTag = e.GetAttribute("gather_tags");
			}
			if (e.HasAttribute("target_name_key"))
			{
				this.targetName = Localization.Get(e.GetAttribute("target_name_key"), false, null);
				return;
			}
			if (e.HasAttribute("target_name"))
			{
				this.targetName = e.GetAttribute("target_name");
			}
		}

		// Token: 0x0600C5BE RID: 50622 RVA: 0x0048EFB4 File Offset: 0x0048D1B4
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveGatherByTag
			{
				gatherTag = this.gatherTag,
				gatherTags = this.gatherTags,
				trackingEntry = this.trackingEntry,
				targetName = this.targetName
			};
		}

		// Token: 0x04009599 RID: 38297
		[PublicizedFrom(EAccessModifier.Private)]
		public string gatherTag = "";

		// Token: 0x0400959A RID: 38298
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> gatherTags;

		// Token: 0x0400959B RID: 38299
		[PublicizedFrom(EAccessModifier.Private)]
		public string targetName = "";

		// Token: 0x0400959C RID: 38300
		public BaseRequirementObjectiveGroup Parent;
	}
}
