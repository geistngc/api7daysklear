using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200013C RID: 316
[Preserve]
public class BlockPoweredDoor : BlockPowered
{
	// Token: 0x06000896 RID: 2198 RVA: 0x0003CC50 File Offset: 0x0003AE50
	public BlockPoweredDoor()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0003CC90 File Offset: 0x0003AE90
	public override void Init()
	{
		if (base.Properties.GetValue(Block.PropMultiBlockDim) == null)
		{
			base.Properties.Values[Block.PropMultiBlockDim] = "1,2,1";
		}
		base.Init();
		if (base.Properties.Values.ContainsKey("OpenSound"))
		{
			this.openSound = base.Properties.Values["OpenSound"];
		}
		if (base.Properties.Values.ContainsKey("CloseSound"))
		{
			this.closeSound = base.Properties.Values["CloseSound"];
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0003CD33 File Offset: 0x0003AF33
	public static bool IsDoorOpen(byte _metadata)
	{
		return (_metadata & 1) > 0;
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0003CD3C File Offset: 0x0003AF3C
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return !BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			string[] array = new string[5];
			array[0] = "Door on position ";
			int num = 1;
			Vector3i vector3i = parentPos;
			array[num] = vector3i.ToString();
			array[2] = " with value ";
			int num2 = 3;
			BlockValue blockValue = block;
			array[num2] = blockValue.ToString();
			array[4] = " should be a parent but is not! (2)";
			Log.Error(string.Concat(array));
			return true;
		}
		return this.IsMovementBlocked(_world, parentPos, block, _face);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0003CDE8 File Offset: 0x0003AFE8
	public override bool IsSeeThrough(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			string[] array = new string[5];
			array[0] = "Door on position ";
			int num = 1;
			Vector3i vector3i = parentPos;
			array[num] = vector3i.ToString();
			array[2] = " with value ";
			int num2 = 3;
			BlockValue blockValue = block;
			array[num2] = blockValue.ToString();
			array[4] = " should be a parent but is not! (1)";
			Log.Error(string.Concat(array));
			return true;
		}
		return this.IsSeeThrough(_world, parentPos, block);
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0003CE8C File Offset: 0x0003B08C
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetBlockActivationCommands(_world, block, parentPos, _entityFocusing);
		}
		TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)_world.GetTileEntity(_blockPos);
		if (tileEntityPoweredBlock != null)
		{
			bool isPowered = tileEntityPoweredBlock.IsPowered;
		}
		this.cmds[0].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0003CF20 File Offset: 0x0003B120
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, parentPos, block, _player);
		}
		if (_commandName == "take")
		{
			base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
			return true;
		}
		return false;
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0003CF84 File Offset: 0x0003B184
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		if (this.shape is BlockShapeModelEntity && (_oldBlockValue.type != _newBlockValue.type || _oldBlockValue.meta != _newBlockValue.meta) && !_newBlockValue.ischild)
		{
			BlockEntityData blockEntity = ((World)_world).ChunkCache.GetBlockEntity(_blockPos);
			bool flag = BlockPoweredDoor.IsDoorOpen(_newBlockValue.meta);
			bool flag2 = BlockPoweredDoor.IsDoorOpen(_oldBlockValue.meta);
			if (flag != flag2)
			{
				this.updateAnimState(blockEntity, flag);
			}
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x0003D00C File Offset: 0x0003B20C
	public override bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		bool flag = !BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
		this.updateOpenCloseState(flag, _world, _blockPos, _blockValue, false);
		if (_player != null)
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, flag ? this.openSound : this.closeSound);
		}
		return true;
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0003D070 File Offset: 0x0003B270
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAnimState(WorldBase _world, Vector3i _blockPos, bool _bOpen)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		IChunk chunkSync = chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		this.updateAnimState(blockEntity, _bOpen);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0003D0D0 File Offset: 0x0003B2D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAnimState(BlockEntityData _ebcd, bool _bOpen)
	{
		if (_ebcd != null && _ebcd.bHasTransform)
		{
			Animator[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Animator animator = componentsInChildren[i];
					animator.enabled = true;
					animator.SetBool(AnimatorDoorState.IsOpenHash, _bOpen);
					animator.SetTrigger(AnimatorDoorState.OpenTriggerHash);
				}
			}
		}
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0003D128 File Offset: 0x0003B328
	public override bool ActivateBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		byte meta = _blockValue.meta;
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (isOn ? 1 : 0));
		if (meta != _blockValue.meta)
		{
			_world.SetBlockRPC(_blockPos, _blockValue);
			this.updateAnimState(_world, _blockPos, isOn);
			if (isOn)
			{
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.openSound);
			}
			else
			{
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.closeSound);
			}
		}
		return true;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0003D1CC File Offset: 0x0003B3CC
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		PowerItem.PowerItemTypes powerItemType = PowerItem.PowerItemTypes.Consumer;
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = powerItemType
		};
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0003D1E8 File Offset: 0x0003B3E8
	public override void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (_ebcd != null && _ebcd.bHasTransform)
		{
			Animator[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				bool flag = BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Animator animator = componentsInChildren[i];
					animator.enabled = true;
					animator.keepAnimatorStateOnDisable = true;
					animator.SetBool(AnimatorDoorState.IsOpenHash, flag);
					animator.Play(flag ? AnimatorDoorState.OpenHash : AnimatorDoorState.CloseHash, 0, 1f);
				}
			}
		}
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0003D264 File Offset: 0x0003B464
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateOpenCloseState(bool _bOpen, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOnlyLocal)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		_blockValue.meta = (byte)((_bOpen ? 1 : 0) | ((int)_blockValue.meta & -2));
		if (!_bOnlyLocal)
		{
			_world.SetBlockRPC(_blockPos, _blockValue);
			return;
		}
		chunkCache.SetBlockRaw(_blockPos, _blockValue);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0003D2B2 File Offset: 0x0003B4B2
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		this.updateOpenCloseState(BlockPoweredDoor.IsDoorOpen(_blockValue.meta), _world, _blockPos, _blockValue, true);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	public override float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue blockDef, BlockFace stepFace)
	{
		return 0f;
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0003D2EC File Offset: 0x0003B4EC
	public bool IsOpen(IBlockAccess _blockAccess, int _x, int _y, int _z)
	{
		return BlockPoweredDoor.IsDoorOpen(_blockAccess.GetBlock(_x, _y, _z).meta);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0003D310 File Offset: 0x0003B510
	public override void RenderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		if (!(this.shape is BlockShapeModelEntity) || (_blockValue.meta & 2) == 0)
		{
			this.shape.renderDecorations(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, _nBlocks);
		}
	}

	// Token: 0x0400097E RID: 2430
	[PublicizedFrom(EAccessModifier.Private)]
	public string openSound;

	// Token: 0x0400097F RID: 2431
	[PublicizedFrom(EAccessModifier.Private)]
	public string closeSound;

	// Token: 0x04000980 RID: 2432
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockEntityData ebcd;

	// Token: 0x04000981 RID: 2433
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
