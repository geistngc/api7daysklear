using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000169 RID: 361
[Preserve]
public class BlockWorkstation : BlockParticle
{
	// Token: 0x06000A3E RID: 2622 RVA: 0x00044738 File Offset: 0x00042938
	public BlockWorkstation()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x000447AC File Offset: 0x000429AC
	public override void Init()
	{
		base.Init();
		this.TakeDelay = 2f;
		base.Properties.ParseFloat("TakeDelay", ref this.TakeDelay);
		string text = "1,2,3";
		base.Properties.ParseString("Workstation", "ToolNames", ref text);
		this.toolTransformNames = text.Split(',', StringSplitOptions.None);
		this.WorkstationData = new WorkstationData(base.GetBlockName(), base.Properties);
		CraftingManager.AddWorkstationData(this.WorkstationData);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x00044830 File Offset: 0x00042A30
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		_chunk.AddTileEntity(new TileEntityWorkstation(_chunk)
		{
			localChunkPos = World.toBlock(_blockPos)
		});
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0004486E File Offset: 0x00042A6E
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		_chunk.RemoveTileEntityAt<TileEntityWorkstation>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00044890 File Offset: 0x00042A90
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_result.blockPos);
		if (tileEntityWorkstation != null)
		{
			tileEntityWorkstation.IsPlayerPlaced = true;
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x000448C4 File Offset: 0x00042AC4
	public override bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_blockPos);
		if (tileEntityWorkstation == null)
		{
			return false;
		}
		_player.AimingGun = false;
		LockManager.Instance.LockRequestLocal(tileEntityWorkstation, null, 0);
		return true;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000448F9 File Offset: 0x00042AF9
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		this.checkParticles(_world, _blockPos, _newBlockValue);
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00010E62 File Offset: 0x0000F062
	public override byte GetLightValue(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00044914 File Offset: 0x00042B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void checkParticles(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		bool flag = GameManager.Instance.HasBlockParticleEffect(_blockPos);
		if (_blockValue.meta != 0 && !flag)
		{
			this.addParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
			if (this.CraftingParticleLightIntensity > 0f)
			{
				this.UpdateVisible(_world, _blockPos);
				return;
			}
		}
		else if (_blockValue.meta == 0 && flag)
		{
			this.removeParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0004499B File Offset: 0x00042B9B
	public static bool IsLit(BlockValue _blockValue)
	{
		return _blockValue.meta > 0;
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x000449A7 File Offset: 0x00042BA7
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _blockValue.Block.GetLocalizedBlockName() + "\n" + Localization.Get("useWorkstation", false, null);
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x000449CC File Offset: 0x00042BCC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "open")
		{
			return this.OnBlockActivated(_world, _blockPos, _blockValue, _player);
		}
		if (_commandName == "take")
		{
			base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
			return true;
		}
		if (!(_commandName == "extract"))
		{
			return false;
		}
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_blockPos);
		for (int i = 0; i < tileEntityWorkstation.Input.Length; i++)
		{
			if (!tileEntityWorkstation.Input[i].IsEmpty())
			{
				ItemStack itemStack = tileEntityWorkstation.Input[i];
				ItemClass itemClass = itemStack.itemValue.ItemClass;
				ItemStack itemStack2;
				if (itemClass.ReplaceResourceUnit != "")
				{
					itemStack2 = new ItemStack(ItemClass.GetItem(itemClass.ReplaceResourceUnit, false).Clone(), itemStack.count);
				}
				else
				{
					itemStack2 = itemStack.Clone();
				}
				itemStack.count = 0;
				if (!itemStack2.IsEmpty())
				{
					_player.PlayerUI.xui.PlayerInventory.AddItem(itemStack2);
				}
				if (!itemStack2.IsEmpty())
				{
					_player.PlayerUI.xui.PlayerInventory.DropItem(itemStack2);
				}
			}
		}
		return true;
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x00044AFC File Offset: 0x00042CFC
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_blockPos);
		bool flag2 = false;
		if (tileEntityWorkstation != null)
		{
			flag2 = tileEntityWorkstation.IsPlayerPlaced;
		}
		this.cmds[1].enabled = (flag && flag2 && this.TakeDelay > 0f);
		this.cmds[2].enabled = (tileEntityWorkstation.InputSlotCount > 0 && !tileEntityWorkstation.InputIsEmpty() && XUiM_Recipes.DisableSmelter);
		return this.cmds;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00044B8C File Offset: 0x00042D8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool takeItemWithTimerCanTake(Vector3i _blockPos, EntityAlive _player)
	{
		if ((GameManager.Instance.World.GetTileEntity(_blockPos) as TileEntityWorkstation).IsEmpty)
		{
			return true;
		}
		GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttWorkstationNotEmpty", false, null), string.Empty, "ui_denied", null, false, false, 0f);
		return false;
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00044BE1 File Offset: 0x00042DE1
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
		this.UpdateVisible(_world, _blockPos);
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00044BF8 File Offset: 0x00042DF8
	public void UpdateVisible(WorldBase _world, Vector3i _blockPos)
	{
		TileEntityWorkstation tileEntityWorkstation = _world.GetTileEntity(_blockPos) as TileEntityWorkstation;
		if (tileEntityWorkstation != null)
		{
			this.UpdateVisible(tileEntityWorkstation);
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00044C1C File Offset: 0x00042E1C
	public void UpdateVisible(TileEntityWorkstation _te)
	{
		BlockEntityData blockEntity = _te.GetChunk().GetBlockEntity(_te.ToWorldPos());
		if (blockEntity == null)
		{
			return;
		}
		Transform transform = blockEntity.transform;
		if (transform)
		{
			ItemStack[] tools = _te.Tools;
			int num = Utils.FastMin(tools.Length, this.toolTransformNames.Length);
			for (int i = 0; i < num; i++)
			{
				Transform transform2 = transform.Find(this.toolTransformNames[i]);
				if (transform2)
				{
					transform2.gameObject.SetActive(!tools[i].IsEmpty());
				}
			}
			Transform transform3 = transform.Find("craft");
			if (transform3)
			{
				bool isCrafting = _te.IsCrafting;
				transform3.gameObject.SetActive(isCrafting);
				if (this.CraftingParticleLightIntensity > 0f)
				{
					Transform blockParticleEffect = GameManager.Instance.GetBlockParticleEffect(_te.ToWorldPos());
					if (blockParticleEffect)
					{
						Light componentInChildren = blockParticleEffect.GetComponentInChildren<Light>();
						if (componentInChildren)
						{
							componentInChildren.intensity = (isCrafting ? this.CraftingParticleLightIntensity : 1f);
							return;
						}
					}
					else if (isCrafting)
					{
						_te.SetVisibleChanged();
					}
				}
			}
		}
	}

	// Token: 0x04000A1C RID: 2588
	[PublicizedFrom(EAccessModifier.Protected)]
	public float TakeDelay;

	// Token: 0x04000A1D RID: 2589
	public WorkstationData WorkstationData;

	// Token: 0x04000A1E RID: 2590
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] toolTransformNames;

	// Token: 0x04000A1F RID: 2591
	[PublicizedFrom(EAccessModifier.Protected)]
	public float CraftingParticleLightIntensity;

	// Token: 0x04000A20 RID: 2592
	[PublicizedFrom(EAccessModifier.Protected)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("open", "campfire", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null),
		new BlockActivationCommand("extract", "store_all_up", false, false, null)
	};
}
