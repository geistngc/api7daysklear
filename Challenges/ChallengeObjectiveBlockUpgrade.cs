using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018FE RID: 6398
	[Preserve]
	public class ChallengeObjectiveBlockUpgrade : BaseChallengeObjective
	{
		// Token: 0x17001859 RID: 6233
		// (get) Token: 0x0600C552 RID: 50514 RVA: 0x0002F184 File Offset: 0x0002D384
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.BlockUpgrade;
			}
		}

		// Token: 0x1700185A RID: 6234
		// (get) Token: 0x0600C553 RID: 50515 RVA: 0x0048D595 File Offset: 0x0048B795
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveUpgrade", false, null) + " " + Localization.Get(this.expectedBlock, false, null) + ":";
			}
		}

		// Token: 0x0600C554 RID: 50516 RVA: 0x0048D5BF File Offset: 0x0048B7BF
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C555 RID: 50517 RVA: 0x0048D5CD File Offset: 0x0048B7CD
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupBlockUpgrade(this.heldItemID, this.neededResourceID, this.neededResourceCount));
		}

		// Token: 0x0600C556 RID: 50518 RVA: 0x0048D5FA File Offset: 0x0048B7FA
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BlockUpgrade -= this.Current_BlockUpgrade;
			QuestEventManager.Current.BlockUpgrade += this.Current_BlockUpgrade;
		}

		// Token: 0x0600C557 RID: 50519 RVA: 0x0048D628 File Offset: 0x0048B828
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BlockUpgrade -= this.Current_BlockUpgrade;
		}

		// Token: 0x0600C558 RID: 50520 RVA: 0x0048D640 File Offset: 0x0048B840
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BlockUpgrade(string blockName, Vector3i blockPos)
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
			if (!flag && blockName.Contains(":") && this.expectedBlock.EqualsCaseInsensitive(blockName.Substring(0, blockName.IndexOf(':'))))
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

		// Token: 0x0600C559 RID: 50521 RVA: 0x0048D70C File Offset: 0x0048B90C
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("block"))
			{
				this.expectedBlock = e.GetAttribute("block");
			}
			if (e.HasAttribute("held"))
			{
				this.heldItemID = e.GetAttribute("held");
			}
			if (e.HasAttribute("needed_resource"))
			{
				this.neededResourceID = e.GetAttribute("needed_resource");
			}
			if (e.HasAttribute("needed_resource_count"))
			{
				this.neededResourceCount = StringParsers.ParseSInt32(e.GetAttribute("needed_resource_count"), 0, -1, NumberStyles.Integer);
			}
		}

		// Token: 0x0600C55A RID: 50522 RVA: 0x0048D7C8 File Offset: 0x0048B9C8
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveBlockUpgrade
			{
				expectedBlock = this.expectedBlock,
				heldItemID = this.heldItemID,
				neededResourceID = this.neededResourceID,
				neededResourceCount = this.neededResourceCount
			};
		}

		// Token: 0x04009581 RID: 38273
		public string expectedBlock = "";

		// Token: 0x04009582 RID: 38274
		public string heldItemID = "";

		// Token: 0x04009583 RID: 38275
		public string neededResourceID = "";

		// Token: 0x04009584 RID: 38276
		public int neededResourceCount = 1;
	}
}
