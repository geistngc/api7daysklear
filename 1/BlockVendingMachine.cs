using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000168 RID: 360
[Preserve]
public class BlockVendingMachine : Block
{
	// Token: 0x06000A31 RID: 2609 RVA: 0x00043EE4 File Offset: 0x000420E4
	public BlockVendingMachine()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00043F70 File Offset: 0x00042170
	public override void Init()
	{
		base.Init();
		if (!base.Properties.Values.ContainsKey(BlockVendingMachine.PropTraderID))
		{
			throw new Exception("Block with name " + base.GetBlockName() + " doesnt have a trader ID.");
		}
		int.TryParse(base.Properties.Values[BlockVendingMachine.PropTraderID], out this.traderID);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00043FD8 File Offset: 0x000421D8
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		if (_world.GetTileEntity(_result.blockPos) is TileEntityVendingMachine && _ea != null && _ea.entityType == EntityType.Player && TraderInfo.traderInfoList[this.traderID].PlayerOwned)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				GameManager.Instance.persistentPlayers.Players[PlatformManager.InternalLocalUserIdentifier].AddVendingMachinePosition(_result.blockPos);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerVendingMachine>().Setup(PlatformManager.InternalLocalUserIdentifier, _result.blockPos, false), false);
		}
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x00044078 File Offset: 0x00042278
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_blockPos) as TileEntityVendingMachine;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (tileEntityVendingMachine == null)
		{
			return "";
		}
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string arg2 = _blockValue.Block.GetLocalizedBlockName();
		if ((tileEntityVendingMachine.IsRentable || tileEntityVendingMachine.TraderData.TraderInfo.PlayerOwned) && tileEntityVendingMachine.GetOwner() != null)
		{
			PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(tileEntityVendingMachine.GetOwner());
			if (playerData != null)
			{
				GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
				if ((gameServerInfo != null && gameServerInfo.AllowsCrossplay) || playerData.PlayGroup != DeviceFlag.StandaloneWindows.ToPlayGroup())
				{
					string str = "[sp=" + PlatformManager.NativePlatform.Utils.GetCrossplayPlayerIcon(playerData.PlayGroup, true, playerData.PlatformData.NativeId.PlatformIdentifier) + "]";
					arg2 = string.Format(Localization.Get("xuiVendingWithOwner", false, null), GameUtils.SafeStringFormat(str + " " + playerData.PlayerName.DisplayName));
				}
				else
				{
					arg2 = string.Format(Localization.Get("xuiVendingWithOwner", false, null), GameUtils.SafeStringFormat(playerData.PlayerName.DisplayName));
				}
			}
			else
			{
				arg2 = string.Format(Localization.Get("xuiVendingWithOwner", false, null), Localization.Get("sleepingBagPlayerUnknown", false, null));
			}
		}
		if (!TraderManager.VendingEnabled)
		{
			return "";
		}
		return string.Format(Localization.Get("vendingMachineActivate", false, null), arg, arg2);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00044231 File Offset: 0x00042431
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _world.GetTileEntity(_blockPos) is TileEntityVendingMachine;
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00044244 File Offset: 0x00042444
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_blockPos) as TileEntityVendingMachine;
		if (tileEntityVendingMachine == null)
		{
			return BlockActivationCommand.Empty;
		}
		PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
		PersistentPlayerData playerData = _world.GetGameManager().GetPersistentPlayerList().GetPlayerData(tileEntityVendingMachine.GetOwner());
		bool flag = tileEntityVendingMachine.LocalPlayerIsOwner();
		if (!flag)
		{
			if (playerData != null)
			{
				playerData.IsAlly(internalLocalUserIdentifier);
			}
		}
		bool playerOwned = TraderInfo.traderInfoList[this.traderID].PlayerOwned;
		this.cmds[0].enabled = (TraderManager.VendingEnabled || playerOwned);
		this.cmds[1].enabled = (playerOwned && flag && tileEntityVendingMachine.TraderData.PrimaryInventory.Count == 0);
		this.cmds[2].enabled = (playerOwned && ((!tileEntityVendingMachine.IsUserAllowed(internalLocalUserIdentifier) && tileEntityVendingMachine.HasPassword()) || flag));
		this.cmds[3].enabled = (!playerOwned && GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		return this.cmds;
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0004434C File Offset: 0x0004254C
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (!(world.GetTileEntity(_blockPos) is TileEntityVendingMachine))
		{
			TileEntityVendingMachine tileEntityVendingMachine = new TileEntityVendingMachine(_chunk);
			tileEntityVendingMachine.SetDisableModifiedCheck(true);
			tileEntityVendingMachine.localChunkPos = World.toBlock(_blockPos);
			tileEntityVendingMachine.TraderData.TraderID = this.traderID;
			if (_addedByPlayer != null)
			{
				tileEntityVendingMachine.SetOwner(_addedByPlayer);
			}
			_chunk.AddTileEntity(tileEntityVendingMachine);
			tileEntityVendingMachine.SetDisableModifiedCheck(false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				tileEntityVendingMachine.SetModified();
			}
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x000443D8 File Offset: 0x000425D8
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			TileEntityVendingMachine tileEntityVendingMachine = world.GetTileEntity(_blockPos) as TileEntityVendingMachine;
			if (tileEntityVendingMachine != null)
			{
				PlatformUserIdentifierAbs owner = tileEntityVendingMachine.GetOwner();
				PersistentPlayerData persistentPlayerData;
				if (owner != null && GameManager.Instance.persistentPlayers.Players.TryGetValue(owner, out persistentPlayerData))
				{
					persistentPlayerData.TryRemoveVendingMachinePosition(_blockPos);
				}
			}
		}
		_chunk.RemoveTileEntityAt<TileEntityVendingMachine>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00044449 File Offset: 0x00042649
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_bvRef) as TileEntityVendingMachine;
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0004445C File Offset: 0x0004265C
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, parentPos, block, _player);
		}
		TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_blockPos) as TileEntityVendingMachine;
		if (tileEntityVendingMachine == null)
		{
			return false;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
		if (null != uiforPlayer)
		{
			if (_commandName == "trade")
			{
				_player.PlayOneShot("interact_vending", false, false, false, null, 1f);
				return this.OnBlockActivated(_world, _blockPos, _blockValue, _player);
			}
			if (_commandName == "take")
			{
				ItemStack itemStack = new ItemStack(_blockValue.ToItemValue(), 1);
				if (uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
				{
					_world.SetBlockRPC(_blockPos, BlockValue.Air);
				}
				return true;
			}
			if (_commandName == "keypad")
			{
				XUiC_KeypadWindow.Open(uiforPlayer, tileEntityVendingMachine, null, null);
				return true;
			}
			if (_commandName == "restock")
			{
				_player.PlayOneShot("ui_trader_inv_reset", false, false, false, null, 1f);
				tileEntityVendingMachine.TraderData.lastInventoryUpdate = 0UL;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00044584 File Offset: 0x00042784
	public override bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_world.GetBlock(_blockPos.x, _blockPos.y - 1, _blockPos.z).Block.HasTag(BlockTags.Door))
		{
			_blockPos = new Vector3i(_blockPos.x, _blockPos.y - 1, _blockPos.z);
			return this.OnBlockActivated(_world, _blockPos, _blockValue, _player);
		}
		TileEntityVendingMachine tileEntityVendingMachine = _world.GetTileEntity(_blockPos) as TileEntityVendingMachine;
		if (tileEntityVendingMachine == null)
		{
			return false;
		}
		_player.AimingGun = false;
		LockManager.Instance.LockRequestLocal(tileEntityVendingMachine, new TileEntityVendingMachine.VendingMachineLockContext(tileEntityVendingMachine.TraderData), 0);
		return true;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00044614 File Offset: 0x00042814
	public override int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (_damagePoints > 0 && base.Properties.Values.ContainsKey("Buff"))
		{
			EntityAlive entityAlive = _world.GetEntity(_entityIdThatDamaged) as EntityAlive;
			if (entityAlive != null && entityAlive as EntityTurret == null)
			{
				bool flag = true;
				if (_attackHitInfo != null && _attackHitInfo.WeaponTypeTag.Equals(ItemActionAttack.ThrownTag))
				{
					flag = true;
				}
				else
				{
					ItemActionRanged itemActionRanged = entityAlive.inventory.holdingItemData.item.Actions[0] as ItemActionRanged;
					if (itemActionRanged == null || (itemActionRanged.Hitmask & 128) != 0)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					string[] array = base.Properties.Values["Buff"].Split(',', StringSplitOptions.None);
					for (int i = 0; i < array.Length; i++)
					{
						entityAlive.Buffs.AddBuff(array[i].Trim(), _bvRef, entityAlive.entityId, true, false, -1f);
					}
				}
			}
		}
		return base.OnBlockDamaged(_world, _bvRef, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x04000A18 RID: 2584
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTraderID = "TraderID";

	// Token: 0x04000A19 RID: 2585
	[PublicizedFrom(EAccessModifier.Protected)]
	public int traderID;

	// Token: 0x04000A1A RID: 2586
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> buffActions;

	// Token: 0x04000A1B RID: 2587
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("trade", "vending", false, false, null),
		new BlockActivationCommand("take", "hand", false, false, null),
		new BlockActivationCommand("keypad", "keypad", false, false, null),
		new BlockActivationCommand("restock", "coin", false, false, null)
	};
}
