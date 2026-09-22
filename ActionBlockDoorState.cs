using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019E2 RID: 6626
	[Preserve]
	public class ActionBlockDoorState : ActionBaseBlockAction
	{
		// Token: 0x0600CA4A RID: 51786 RVA: 0x004A5326 File Offset: 0x004A3526
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool AllowInTrader()
		{
			return this.traderOnly;
		}

		// Token: 0x0600CA4B RID: 51787 RVA: 0x004A5330 File Offset: 0x004A3530
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				if (this.traderOnly && !world.IsWithinTraderArea(currentPos))
				{
					return null;
				}
				TileEntityComposite te = world.GetTileEntity(currentPos) as TileEntityComposite;
				TEFeatureDoor tefeatureDoor;
				if (te.TryGetSelfOrFeature(out tefeatureDoor))
				{
					ActionBlockDoorState.OpenDoorStates openDoorStates = this.setOpen;
					bool open = openDoorStates != ActionBlockDoorState.OpenDoorStates.Close && (openDoorStates != ActionBlockDoorState.OpenDoorStates.Toggle || !tefeatureDoor.IsOpen());
					tefeatureDoor.SetOpen(open, this.animate);
					TEFeatureLockable tefeatureLockable;
					if (this.handleLock && te.TryGetSelfOrFeature(out tefeatureLockable))
					{
						tefeatureLockable.SetLocked(this.setLocked);
					}
					tefeatureDoor.HandleOpenCloseSound(currentPos);
					return new BlockChangeInfo(currentPos, blockValue);
				}
			}
			return null;
		}

		// Token: 0x0600CA4C RID: 51788 RVA: 0x004A53D8 File Offset: 0x004A35D8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<ActionBlockDoorState.OpenDoorStates>(ActionBlockDoorState.PropSetOpenState, ref this.setOpen);
			properties.ParseBool(ActionBlockDoorState.PropTraderOnly, ref this.traderOnly);
			properties.ParseBool(ActionBlockDoorState.PropAnimate, ref this.animate);
			if (properties.Contains(ActionBlockDoorState.PropSetLockState))
			{
				this.handleLock = true;
				properties.ParseBool(ActionBlockDoorState.PropSetLockState, ref this.setLocked);
			}
		}

		// Token: 0x0600CA4D RID: 51789 RVA: 0x004A5444 File Offset: 0x004A3644
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockDoorState
			{
				setOpen = this.setOpen,
				setLocked = this.setLocked,
				handleLock = this.handleLock,
				traderOnly = this.traderOnly
			};
		}

		// Token: 0x040099DE RID: 39390
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionBlockDoorState.OpenDoorStates setOpen;

		// Token: 0x040099DF RID: 39391
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool setLocked;

		// Token: 0x040099E0 RID: 39392
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool traderOnly;

		// Token: 0x040099E1 RID: 39393
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool animate = true;

		// Token: 0x040099E2 RID: 39394
		[PublicizedFrom(EAccessModifier.Private)]
		public bool handleLock;

		// Token: 0x040099E3 RID: 39395
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSetOpenState = "set_open";

		// Token: 0x040099E4 RID: 39396
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSetLockState = "set_lock";

		// Token: 0x040099E5 RID: 39397
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimate = "animate";

		// Token: 0x040099E6 RID: 39398
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTraderOnly = "trader_only";

		// Token: 0x020019E3 RID: 6627
		public enum OpenDoorStates
		{
			// Token: 0x040099E8 RID: 39400
			Open,
			// Token: 0x040099E9 RID: 39401
			Close,
			// Token: 0x040099EA RID: 39402
			Toggle
		}
	}
}
