using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200015F RID: 351
[Preserve]
public class BlockTriggerDowngrade : Block
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060009AA RID: 2474 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00042834 File Offset: 0x00040A34
	public override void LateInit()
	{
		base.LateInit();
		if (!this.DowngradeBlock.isair)
		{
			BlockHazard blockHazard = this.DowngradeBlock.Block as BlockHazard;
			if (blockHazard != null)
			{
				this.DowngradeBlock = blockHazard.SetHazardState(this.DowngradeBlock, true);
			}
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00033482 File Offset: 0x00031682
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0004287C File Offset: 0x00040A7C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			return "";
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false, null), arg, localizedBlockName);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x000428EB File Offset: 0x00040AEB
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "trigger")
		{
			XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _blockPos, false, true);
		}
		return false;
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00042910 File Offset: 0x00040B10
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0004297F File Offset: 0x00040B7F
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		this.HandleDowngrade(_world, _blockPos, _blockValue, _blockChanges);
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x0004299C File Offset: 0x00040B9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleDowngrade(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		if (!this.DowngradeBlock.isair)
		{
			base.SpawnDowngradeFX(_world, _blockValue, _blockPos, _blockValue.Block.tintColor, -1);
			BlockValue blockValue = this.DowngradeBlock;
			blockValue = BlockPlaceholderMap.Instance.Replace(blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
			blockValue.rotation = _blockValue.rotation;
			if (!blockValue.Block.shape.IsTerrain())
			{
				_blockChanges.Add(new BlockChangeInfo(_blockPos, blockValue));
				if (chunkCache.GetTextureFull(_blockPos) != 0L)
				{
					if (this.RemovePaintOnDowngrade == null)
					{
						GameManager.Instance.SetBlockTextureServer(_blockPos, BlockFace.None, 0, -1, byte.MaxValue);
						return;
					}
					for (int i = 0; i < this.RemovePaintOnDowngrade.Count; i++)
					{
						GameManager.Instance.SetBlockTextureServer(_blockPos, this.RemovePaintOnDowngrade[i], 0, -1, byte.MaxValue);
					}
					return;
				}
			}
			else
			{
				_blockChanges.Add(new BlockChangeInfo(_blockPos, blockValue, blockValue.Block.Density));
			}
		}
	}

	// Token: 0x040009E4 RID: 2532
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
