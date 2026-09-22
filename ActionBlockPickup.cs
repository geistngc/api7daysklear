using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019E9 RID: 6633
	[Preserve]
	public class ActionBlockPickup : ActionBaseBlockAction
	{
		// Token: 0x0600CA61 RID: 51809 RVA: 0x004A4F9A File Offset: 0x004A319A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				return new BlockChangeInfo(currentPos, blockValue, true);
			}
			return null;
		}

		// Token: 0x0600CA62 RID: 51810 RVA: 0x004A5860 File Offset: 0x004A3A60
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			for (int i = 0; i < blockChanges.Count; i++)
			{
				BlockChangeInfo blockChangeInfo = blockChanges[i];
				blockChangeInfo.blockValue.Block.PickupOrDrop(world, blockChangeInfo.blockValueRef, blockChangeInfo.blockValue, base.Owner.Target as EntityPlayer, true);
			}
		}

		// Token: 0x0600CA63 RID: 51811 RVA: 0x004A58B9 File Offset: 0x004A3AB9
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockPickup();
		}
	}
}
