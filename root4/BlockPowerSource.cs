using System;
using System.Collections;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000140 RID: 320
[Preserve]
public class BlockPowerSource : Block
{
	// Token: 0x060008C9 RID: 2249 RVA: 0x0003DDE0 File Offset: 0x0003BFE0
	public BlockPowerSource()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0003DE5C File Offset: 0x0003C05C
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("SlotItem"))
		{
			this.SlotItemName = base.Properties.Values["SlotItem"];
		}
		else
		{
			this.SlotItemName = "carBattery";
		}
		if (base.Properties.Values.ContainsKey("OutputPerStack"))
		{
			this.OutputPerStack = Convert.ToInt32(base.Properties.Values["OutputPerStack"]);
		}
		else
		{
			this.OutputPerStack = 25;
		}
		if (base.Properties.Values.ContainsKey("TakeDelay"))
		{
			this.TakeDelay = StringParsers.ParseFloat(base.Properties.Values["TakeDelay"], 0, -1, NumberStyles.Any);
			return;
		}
		this.TakeDelay = 2f;
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0003DF3C File Offset: 0x0003C13C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
		{
			return "";
		}
		bool flag = _world.GetTileEntity(_blockPos) is TileEntityPowerSource;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (flag)
		{
			string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
			return string.Format(Localization.Get("vendingMachineActivate", false, null), arg, localizedBlockName);
		}
		return "";
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0003DFCC File Offset: 0x0003C1CC
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _world.GetTileEntity(_blockPos) is TileEntityPowerSource;
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0003DFE0 File Offset: 0x0003C1E0
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource == null)
		{
			return BlockActivationCommand.Empty;
		}
		bool enabled = _world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.cmds[0].icon = this.GetPowerSourceIcon();
		this.cmds[0].enabled = enabled;
		this.cmds[1].enabled = enabled;
		bool flag2 = false;
		if (tileEntityPowerSource != null)
		{
			flag2 = tileEntityPowerSource.IsPlayerPlaced;
		}
		this.cmds[2].enabled = (flag && flag2 && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x0003E0A0 File Offset: 0x0003C2A0
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (!(world.GetTileEntity(_blockPos) is TileEntityPowerSource))
		{
			TileEntityPowerSource tileEntityPowerSource = this.CreateTileEntity(_chunk);
			tileEntityPowerSource.SetDisableModifiedCheck(true);
			tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
			if (_addedByPlayer != null)
			{
				tileEntityPowerSource.SetOwner(_addedByPlayer);
				tileEntityPowerSource.IsPlayerPlaced = true;
			}
			tileEntityPowerSource.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPowerSource);
			tileEntityPowerSource.SetDisableModifiedCheck(false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				tileEntityPowerSource.SetModified();
			}
		}
		_chunk.AddEntityBlockStub(new BlockEntityData(_blockValue, _blockPos)
		{
			bNeedsTemperature = true
		});
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x0003E140 File Offset: 0x0003C340
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		TileEntityPowered tileEntityPowered = _chunk.GetTileEntity(World.toBlock(_blockPos)) as TileEntityPowered;
		if (tileEntityPowered != null)
		{
			if (!GameManager.IsDedicatedServer)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer.inventory.holdingItem.Actions[1] is ItemActionConnectPower)
				{
					(primaryPlayer.inventory.holdingItem.Actions[1] as ItemActionConnectPower).CheckForWireRemoveNeeded(primaryPlayer, _blockPos);
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				PowerManager.Instance.RemovePowerNode(tileEntityPowered.GetPowerItem());
			}
			if (tileEntityPowered.GetParent().y != -9999)
			{
				IPowered powered = world.GetTileEntity(tileEntityPowered.GetParent()) as IPowered;
				if (powered != null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					powered.SendWireData();
				}
			}
			tileEntityPowered.RemoveWires();
		}
		_chunk.RemoveTileEntityAt<TileEntityPowerSource>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0003E22C File Offset: 0x0003C42C
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_bvRef) as TileEntityPowerSource;
		if (tileEntityPowerSource != null)
		{
			tileEntityPowerSource.OnDestroy();
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0003E250 File Offset: 0x0003C450
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, parentPos, block, _player);
		}
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource == null)
		{
			return false;
		}
		if (_commandName == "open")
		{
			_player.AimingGun = false;
			LockManager.Instance.LockRequestLocal(tileEntityPowerSource, null, 0);
			return true;
		}
		if (!(_commandName == "light"))
		{
			if (!(_commandName == "take"))
			{
				return false;
			}
			base.takeItemWithTimer(_blockPos, _blockValue, _player, 4f);
			return true;
		}
		else
		{
			if (tileEntityPowerSource.MaxOutput == 0)
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttRequiresOneComponent", false, null), false, false, 0f);
				return false;
			}
			if (tileEntityPowerSource.PowerItemType == PowerItem.PowerItemTypes.Generator && tileEntityPowerSource.CurrentFuel == 0)
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttGeneratorRequiresFuel", false, null), false, false, 0f);
				return false;
			}
			bool flag = (_blockValue.meta & 2) > 0;
			if (!flag && (false | _world.IsWater(_blockPos.x, _blockPos.y + 1, _blockPos.z) | _world.IsWater(_blockPos.x + 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x - 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z + 1) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z - 1)))
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttPowerSourceUnderwater", false, null), false, false, 0f);
				return false;
			}
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!flag) ? 2 : 0));
			_world.SetBlockRPC(_blockPos, _blockValue);
			return true;
		}
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0003E468 File Offset: 0x0003C668
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)_world.GetTileEntity(_blockPos);
		if (tileEntityPowerSource == null)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			Chunk chunk = (Chunk)chunkCache.GetChunkFromWorldPos(_blockPos);
			if (chunk == null)
			{
				return;
			}
			tileEntityPowerSource = this.CreateTileEntity(chunk);
			tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowerSource.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowerSource);
		}
		tileEntityPowerSource.BlockTransform = _ebcd.transform;
		GameManager.Instance.StartCoroutine(this.drawWiresLater(tileEntityPowerSource));
		if (tileEntityPowerSource.GetParent().y != -9999)
		{
			IPowered powered = _world.GetTileEntity(tileEntityPowerSource.GetParent()) as IPowered;
			if (powered != null)
			{
				GameManager.Instance.StartCoroutine(this.drawWiresLater(powered));
			}
		}
		this.updateState(_world, _blockPos, _blockValue, false);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0003E530 File Offset: 0x0003C730
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator drawWiresLater(IPowered powered)
	{
		yield return new WaitForSeconds(0.5f);
		powered.DrawWires();
		yield break;
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0003E540 File Offset: 0x0003C740
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_blockPos) as TileEntityPowerSource;
			if (tileEntityPowerSource == null)
			{
				ChunkCluster chunkCache = _world.ChunkCache;
				if (chunkCache == null)
				{
					return;
				}
				Chunk chunk = (Chunk)chunkCache.GetChunkFromWorldPos(_blockPos);
				if (chunk == null)
				{
					return;
				}
				tileEntityPowerSource = this.CreateTileEntity(chunk);
				tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
				chunk.AddTileEntity(tileEntityPowerSource);
				string str = "TileEntityPowerSource not found (";
				Vector3i vector3i = _blockPos;
				Log.Out(str + vector3i.ToString() + ")");
			}
			PowerSource powerSource = tileEntityPowerSource.GetPowerItem() as PowerSource;
			if (powerSource == null)
			{
				powerSource = (PowerManager.Instance.GetPowerItemByWorldPos(tileEntityPowerSource.ToWorldPos()) as PowerSource);
				if (powerSource == null)
				{
					powerSource = (tileEntityPowerSource.CreatePowerItemForTileEntity((ushort)_newBlockValue.type) as PowerSource);
					tileEntityPowerSource.SetModified();
					powerSource.AddTileEntity(tileEntityPowerSource);
					string str2 = "PowerSource not found (";
					Vector3i vector3i = _blockPos;
					Log.Out(str2 + vector3i.ToString() + ")");
				}
			}
			bool isOn = (_newBlockValue.meta & 2) > 0;
			powerSource.IsOn = isOn;
		}
		this.updateState(_world, _blockPos, _newBlockValue, false);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0003E668 File Offset: 0x0003C868
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateState(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bSwitchLight = false)
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
		if (_bSwitchLight)
		{
			flag = !flag;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			_world.SetBlockRPC(_blockPos, _blockValue);
		}
		Transform transform = blockEntity.transform.Find("Activated");
		if (transform != null)
		{
			transform.gameObject.SetActive(flag);
		}
		return true;
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0003E72C File Offset: 0x0003C92C
	public override bool ActivateBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		if ((_blockValue.meta & 2) > 0 != isOn)
		{
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
			_world.SetBlockRPC(_blockPos, _blockValue);
		}
		this.updateState(_world, _blockPos, _blockValue, false);
		return true;
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		return null;
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetPowerSourceIcon()
	{
		return "";
	}

	// Token: 0x0400098B RID: 2443
	public string SlotItemName;

	// Token: 0x0400098C RID: 2444
	public int OutputPerStack;

	// Token: 0x0400098D RID: 2445
	[PublicizedFrom(EAccessModifier.Private)]
	public float TakeDelay = 2f;

	// Token: 0x0400098E RID: 2446
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemClass slotItem;

	// Token: 0x0400098F RID: 2447
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("open", "hand", false, false, null),
		new BlockActivationCommand("light", "electric_switch", false, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
