using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019EA RID: 6634
	[Preserve]
	public class ActionBlockReplace : ActionBaseBlockAction
	{
		// Token: 0x0600CA65 RID: 51813 RVA: 0x004A58C0 File Offset: 0x004A3AC0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckValid(World world, Vector3i currentPos)
		{
			BlockValue block = world.GetBlock(currentPos + Vector3i.down);
			return !block.isair && !block.Block.IsTerrainDecoration;
		}

		// Token: 0x0600CA66 RID: 51814 RVA: 0x004A58FC File Offset: 0x004A3AFC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (this.blockTo == null)
			{
				return null;
			}
			if (this.emptyOnly && !blockValue.isair)
			{
				return null;
			}
			if (!blockValue.Block.blockMaterial.CanDestroy)
			{
				return null;
			}
			BlockValue blockValue2 = Block.GetBlockValue(this.blockTo[this.random.RandomRange(0, this.blockTo.Length)], false);
			if (blockValue.type != blockValue2.type)
			{
				return new BlockChangeInfo(currentPos, blockValue2, true);
			}
			return null;
		}

		// Token: 0x0600CA67 RID: 51815 RVA: 0x004A597C File Offset: 0x004A3B7C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string text = "";
			this.Properties.ParseString(ActionBlockReplace.PropBlockTo, ref text);
			if (text != "")
			{
				this.blockTo = text.Split(',', StringSplitOptions.None);
			}
			properties.ParseBool(ActionBlockReplace.PropEmptyOnly, ref this.emptyOnly);
		}

		// Token: 0x0600CA68 RID: 51816 RVA: 0x004A59D5 File Offset: 0x004A3BD5
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockReplace
			{
				blockTo = this.blockTo,
				emptyOnly = this.emptyOnly
			};
		}

		// Token: 0x040099F7 RID: 39415
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] blockTo;

		// Token: 0x040099F8 RID: 39416
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool emptyOnly;

		// Token: 0x040099F9 RID: 39417
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBlockTo = "block_to";

		// Token: 0x040099FA RID: 39418
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEmptyOnly = "empty_only";
	}
}
