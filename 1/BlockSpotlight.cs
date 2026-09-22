using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000153 RID: 339
[Preserve]
public class BlockSpotlight : BlockPowered
{
	// Token: 0x0600095C RID: 2396 RVA: 0x00041060 File Offset: 0x0003F260
	public BlockSpotlight()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0003AB48 File Offset: 0x00038D48
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x000410D4 File Offset: 0x0003F2D4
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		TileEntityPowered tileEntityPowered = _world.GetTileEntity(_blockPos) as TileEntityPowered;
		if (tileEntityPowered == null)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			Chunk chunk = (Chunk)chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunk == null)
			{
				return;
			}
			tileEntityPowered = this.CreateTileEntity(chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowered);
		}
		if (tileEntityPowered != null)
		{
			tileEntityPowered.WindowGroupToOpen = XUiC_PoweredSpotlightWindowGroup.ID;
		}
		SpotlightController component = _ebcd.transform.gameObject.GetComponent<SpotlightController>();
		if (component != null)
		{
			component.Init(base.Properties);
			component.TileEntity = tileEntityPowered;
		}
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00041198 File Offset: 0x0003F398
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
		bool flag = (_blockValue.meta & 2) > 0;
		TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)_world.GetTileEntity(_blockPos);
		if (tileEntityPoweredBlock != null)
		{
			flag = (flag && tileEntityPoweredBlock.IsToggled);
		}
		if (_bChangeState)
		{
			flag = !flag;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			_world.SetBlockRPC(_blockPos, _blockValue);
			if (flag)
			{
				Manager.PlayInsidePlayerHead("switch_up", -1, 0f, false, false);
			}
			else
			{
				Manager.PlayInsidePlayerHead("switch_down", -1, 0f, false, false);
			}
		}
		TileEntityPowered tileEntityPowered = _world.GetTileEntity(_blockPos) as TileEntityPowered;
		if (tileEntityPowered == null)
		{
			tileEntityPowered = this.CreateTileEntity((Chunk)chunkSync);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			(chunkSync as Chunk).AddTileEntity(tileEntityPowered);
			tileEntityPowered.WindowGroupToOpen = XUiC_PoweredSpotlightWindowGroup.ID;
		}
		SpotlightController component = blockEntity.transform.gameObject.GetComponent<SpotlightController>();
		if (component != null)
		{
			component.Init(base.Properties);
			component.TileEntity = tileEntityPowered;
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
						componentsInChildren[i].material.SetColor("_EmissionColor", Color.white);
					}
					else
					{
						componentsInChildren[i].material.SetColor("_EmissionColor", Color.black);
					}
					componentsInChildren[i].sharedMaterial = componentsInChildren[i].material;
				}
			}
		}
		Transform transform = blockEntity.transform.Find("MainLight");
		if (transform != null)
		{
			LightLOD component2 = transform.GetComponent<LightLOD>();
			if (component2 != null)
			{
				component2.SwitchOnOff(flag, false);
				component2.SetBlockEntityData(blockEntity);
				component2.otherLight.enabled = flag;
			}
		}
		return true;
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue blockDef, BlockFace face)
	{
		return false;
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00041443 File Offset: 0x0003F643
	public override void OnBlockLoaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _blockPos, _blockValue);
		this.updateState(_world, _blockPos, _blockValue, false);
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00041459 File Offset: 0x0003F659
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		this.updateState(_world, _blockPos, _newBlockValue, false);
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00041474 File Offset: 0x0003F674
	public override bool ActivateBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		_world.SetBlockRPC(_blockPos, _blockValue);
		this.updateState(_world, _blockPos, _blockValue, false);
		return true;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0003ADFF File Offset: 0x00038FFF
	public static bool IsSwitchOn(byte _metadata)
	{
		return (_metadata & 2) > 0;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x000414AA File Offset: 0x0003F6AA
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.ConsumerToggle
		};
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x000414BC File Offset: 0x0003F6BC
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!(_world.GetTileEntity(_blockPos) is TileEntityPowered))
		{
			TileEntityPowered tileEntityPowered = this.CreateTileEntity(_chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPowered);
			tileEntityPowered.WindowGroupToOpen = XUiC_PoweredSpotlightWindowGroup.ID;
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00041510 File Offset: 0x0003F710
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
		{
			return "";
		}
		bool flag = _world.GetTileEntity(_blockPos) is TileEntityPowered;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (flag)
		{
			string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
			return string.Format(Localization.Get("vendingMachineActivate", false, null), arg, localizedBlockName);
		}
		return "";
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x000415A0 File Offset: 0x0003F7A0
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, parentPos, block, _player);
		}
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return false;
		}
		Chunk chunk = (Chunk)chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunk == null)
		{
			return false;
		}
		TileEntityPowered tileEntityPowered = _world.GetTileEntity(_blockPos) as TileEntityPowered;
		if (tileEntityPowered == null)
		{
			tileEntityPowered = this.CreateTileEntity(chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowered);
			tileEntityPowered.WindowGroupToOpen = XUiC_PoweredSpotlightWindowGroup.ID;
		}
		bool flag = (_blockValue.meta & 2) > 0;
		if (_commandName == "light")
		{
			flag = !flag;
			TileEntityPoweredBlock tileEntityPoweredBlock = tileEntityPowered as TileEntityPoweredBlock;
			if (tileEntityPoweredBlock != null)
			{
				tileEntityPoweredBlock.IsToggled = !tileEntityPoweredBlock.IsToggled;
			}
			return true;
		}
		if (_commandName == "aim")
		{
			_player.AimingGun = false;
			LockManager.Instance.LockRequestLocal(tileEntityPowered, null, 0);
			return true;
		}
		if (!(_commandName == "take"))
		{
			return false;
		}
		base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
		return true;
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x000416E4 File Offset: 0x0003F8E4
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool enabled = _world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.cmds[0].enabled = enabled;
		this.cmds[1].enabled = enabled;
		this.cmds[2].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x040009BE RID: 2494
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("light", "electric_switch", true, false, null),
		new BlockActivationCommand("aim", "map_cursor", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
