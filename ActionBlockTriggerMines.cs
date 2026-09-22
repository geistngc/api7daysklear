using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019ED RID: 6637
	[Preserve]
	public class ActionBlockTriggerMines : ActionBaseBlockAction
	{
		// Token: 0x0600CA75 RID: 51829 RVA: 0x004A4F9A File Offset: 0x004A319A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				return new BlockChangeInfo(currentPos, blockValue, true);
			}
			return null;
		}

		// Token: 0x0600CA76 RID: 51830 RVA: 0x004A5D04 File Offset: 0x004A3F04
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			for (int i = 0; i < blockChanges.Count; i++)
			{
				BlockMine blockMine = blockChanges[i].blockValue.Block as BlockMine;
				if (blockMine != null)
				{
					blockMine.TriggerMine(base.Owner.Target, world, blockChanges[i].blockValueRef, this.useTrigger);
				}
			}
		}

		// Token: 0x0600CA77 RID: 51831 RVA: 0x004A5D65 File Offset: 0x004A3F65
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			this.Properties.ParseBool(ActionBlockTriggerMines.PropUseTrigger, ref this.useTrigger);
		}

		// Token: 0x0600CA78 RID: 51832 RVA: 0x004A5D84 File Offset: 0x004A3F84
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockTriggerMines
			{
				useTrigger = this.useTrigger
			};
		}

		// Token: 0x04009A02 RID: 39426
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool useTrigger;

		// Token: 0x04009A03 RID: 39427
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropUseTrigger = "use_trigger";
	}
}
