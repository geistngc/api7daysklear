using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019EC RID: 6636
	[Preserve]
	public class ActionBlockTriggerFall : ActionBaseBlockAction
	{
		// Token: 0x0600CA71 RID: 51825 RVA: 0x004A4F9A File Offset: 0x004A319A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				return new BlockChangeInfo(currentPos, blockValue, true);
			}
			return null;
		}

		// Token: 0x0600CA72 RID: 51826 RVA: 0x004A5CC4 File Offset: 0x004A3EC4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			for (int i = 0; i < blockChanges.Count; i++)
			{
				world.AddFallingBlock(blockChanges[i].blockValueRef, false);
			}
		}

		// Token: 0x0600CA73 RID: 51827 RVA: 0x004A5CFA File Offset: 0x004A3EFA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockTriggerFall();
		}
	}
}
