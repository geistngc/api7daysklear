using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000131 RID: 305
[Preserve]
public class BlockMotionSensor : BlockPowered
{
	// Token: 0x0600083F RID: 2111 RVA: 0x0003AAF0 File Offset: 0x00038CF0
	public BlockMotionSensor()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0003AB48 File Offset: 0x00038D48
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0003AB50 File Offset: 0x00038D50
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		TileEntityPoweredTrigger tileEntity = _world.GetTileEntity(_blockPos) as TileEntityPoweredTrigger;
		MotionSensorController component = _ebcd.transform.gameObject.GetComponent<MotionSensorController>();
		if (component != null)
		{
			component.Init(base.Properties);
			component.TileEntity = tileEntity;
		}
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0003ABA4 File Offset: 0x00038DA4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateState(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bChangeState = false)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return false;
		}
		IChunk chunkSync = chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return false;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return false;
		}
		bool flag = (_blockValue.meta & 1) > 0;
		bool flag2 = (_blockValue.meta & 2) > 0;
		if (_bChangeState)
		{
			flag2 = !flag2;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag2 ? 2 : 0));
			_world.SetBlockRPC(_blockPos, _blockValue);
			if (flag2)
			{
				Manager.PlayInsidePlayerHead("switch_up", -1, 0f, false, false);
			}
			else
			{
				Manager.PlayInsidePlayerHead("switch_down", -1, 0f, false, false);
			}
		}
		TileEntityPoweredTrigger tileEntity = _world.GetTileEntity(_blockPos) as TileEntityPoweredTrigger;
		MotionSensorController component = blockEntity.transform.gameObject.GetComponent<MotionSensorController>();
		if (component != null)
		{
			component.Init(base.Properties);
			component.TileEntity = tileEntity;
			component.IsOn = flag;
		}
		BlockEntityData blockEntity2 = ((World)_world).ChunkCache.GetBlockEntity(_blockPos);
		if (blockEntity2 != null && blockEntity2.transform != null && blockEntity2.transform.gameObject != null)
		{
			Renderer[] componentsInChildren = blockEntity2.transform.gameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].material != componentsInChildren[i].sharedMaterial)
					{
						componentsInChildren[i].material = new Material(componentsInChildren[i].sharedMaterial);
					}
					if (flag)
					{
						componentsInChildren[i].material.SetColor("_EmissionColor", flag2 ? Color.green : Color.red);
					}
					else
					{
						componentsInChildren[i].material.SetColor("_EmissionColor", Color.black);
					}
					componentsInChildren[i].sharedMaterial = componentsInChildren[i].material;
				}
			}
		}
		return true;
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue blockDef, BlockFace face)
	{
		return false;
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0003ADC0 File Offset: 0x00038FC0
	public override void OnBlockLoaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _blockPos, _blockValue);
		this.updateState(_world, _blockPos, _blockValue, false);
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0003ADD6 File Offset: 0x00038FD6
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		this.updateState(_world, _blockPos, _newBlockValue, false);
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0003ADF1 File Offset: 0x00038FF1
	public override bool ActivateBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		this.updateState(_world, _blockPos, _blockValue, false);
		return true;
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0003ADFF File Offset: 0x00038FFF
	public static bool IsSwitchOn(byte _metadata)
	{
		return (_metadata & 2) > 0;
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0003AE07 File Offset: 0x00039007
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredTrigger(chunk)
		{
			TriggerType = PowerTrigger.TriggerTypes.Motion
		};
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0003AE18 File Offset: 0x00039018
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!(_world.GetTileEntity(_blockPos) is TileEntityPoweredTrigger))
		{
			TileEntityPoweredTrigger tileEntityPoweredTrigger = this.CreateTileEntity(_chunk) as TileEntityPoweredTrigger;
			tileEntityPoweredTrigger.SetDisableModifiedCheck(true);
			tileEntityPoweredTrigger.localChunkPos = World.toBlock(_blockPos);
			if (_addedByPlayer != null && tileEntityPoweredTrigger != null)
			{
				TileEntityPoweredTrigger tileEntityPoweredTrigger2 = tileEntityPoweredTrigger;
				tileEntityPoweredTrigger2.SetOwner(_addedByPlayer);
			}
			tileEntityPoweredTrigger.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPoweredTrigger);
			tileEntityPoweredTrigger.SetDisableModifiedCheck(false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				tileEntityPoweredTrigger.SetModified();
			}
		}
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0003AE9C File Offset: 0x0003909C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
		{
			return "";
		}
		bool flag = _world.GetTileEntity(_blockPos) is TileEntityPoweredTrigger;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (flag)
		{
			string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
			return string.Format(Localization.Get("vendingMachineActivate", false, null), arg, localizedBlockName);
		}
		return "";
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0003AF2C File Offset: 0x0003912C
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, parentPos, block, _player);
		}
		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_blockPos) as TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger == null)
		{
			return false;
		}
		if (_commandName == "options")
		{
			_player.AimingGun = false;
			LockManager.Instance.LockRequestLocal(tileEntityPoweredTrigger, null, 0);
			return true;
		}
		if (!(_commandName == "take"))
		{
			return false;
		}
		base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
		return true;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0003AFC8 File Offset: 0x000391C8
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool enabled = _world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.cmds[0].enabled = enabled;
		this.cmds[1].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x0400094D RID: 2381
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("options", "tool", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
