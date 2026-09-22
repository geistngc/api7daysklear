using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018FD RID: 6397
	[Preserve]
	public class ChallengeObjectiveBlockPlace : BaseChallengeObjective
	{
		// Token: 0x17001857 RID: 6231
		// (get) Token: 0x0600C548 RID: 50504 RVA: 0x0002003D File Offset: 0x0001E23D
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.BlockPlace;
			}
		}

		// Token: 0x17001858 RID: 6232
		// (get) Token: 0x0600C549 RID: 50505 RVA: 0x0048D3B1 File Offset: 0x0048B5B1
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("xuiWorldPrefabsPlace", false, null) + " " + Localization.Get(this.expectedBlock, false, null) + ":";
			}
		}

		// Token: 0x0600C54A RID: 50506 RVA: 0x0048D3DB File Offset: 0x0048B5DB
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C54B RID: 50507 RVA: 0x0048D3E9 File Offset: 0x0048B5E9
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupPlace((this.alternateItem != "") ? this.alternateItem : this.expectedBlock));
		}

		// Token: 0x0600C54C RID: 50508 RVA: 0x0048D424 File Offset: 0x0048B624
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BlockPlace += this.Current_BlockPlace;
		}

		// Token: 0x0600C54D RID: 50509 RVA: 0x0048D43C File Offset: 0x0048B63C
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BlockPlace -= this.Current_BlockPlace;
		}

		// Token: 0x0600C54E RID: 50510 RVA: 0x0048D454 File Offset: 0x0048B654
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BlockPlace(string blockName, Vector3i blockPos)
		{
			bool flag = false;
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.expectedBlock == null || this.expectedBlock == "" || this.expectedBlock.EqualsCaseInsensitive(blockName))
			{
				flag = true;
			}
			if (!flag && this.expectedBlock != null && this.expectedBlock != "")
			{
				Block blockByName = Block.GetBlockByName(this.expectedBlock, true);
				if (blockByName != null && blockByName.SelectAlternates && blockByName.ContainsAlternateBlock(blockName))
				{
					flag = true;
				}
			}
			if (flag)
			{
				int num = base.Current;
				base.Current = num + 1;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C54F RID: 50511 RVA: 0x0048D4F4 File Offset: 0x0048B6F4
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("block"))
			{
				this.expectedBlock = e.GetAttribute("block");
			}
			if (e.HasAttribute("alternate_item"))
			{
				this.alternateItem = e.GetAttribute("alternate_item");
			}
		}

		// Token: 0x0600C550 RID: 50512 RVA: 0x0048D558 File Offset: 0x0048B758
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveBlockPlace
			{
				expectedBlock = this.expectedBlock,
				alternateItem = this.alternateItem
			};
		}

		// Token: 0x0400957F RID: 38271
		public string expectedBlock = "";

		// Token: 0x04009580 RID: 38272
		public string alternateItem = "";
	}
}
