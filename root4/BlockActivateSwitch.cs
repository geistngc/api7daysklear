using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000110 RID: 272
[Preserve]
public class BlockActivateSwitch : Block
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000728 RID: 1832 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00033BE5 File Offset: 0x00031DE5
	public override void Init()
	{
		base.Init();
		base.Properties.ParseBool(BlockActivateSwitch.PropSingleUse, ref this.singleUse);
		base.Properties.ParseString(BlockActivateSwitch.PropActivateSound, ref this.activateSound);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00033707 File Offset: 0x00031907
	public override void LateInit()
	{
		base.LateInit();
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0003370F File Offset: 0x0003190F
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
		this.Refresh(_world, _chunk, _blockPos, _newBlockValue);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00033729 File Offset: 0x00031929
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		this.Refresh(_world, null, _blockPos, _blockValue);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00033740 File Offset: 0x00031940
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00033753 File Offset: 0x00031953
	public override void OnBlockUnloaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockUnloaded(_world, _blockPos, _blockValue);
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x00033C1C File Offset: 0x00031E1C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			if (this.singleUse && (_blockValue.meta & 2) != 0)
			{
				return "";
			}
			if ((_blockValue.meta & 1) == 0)
			{
				return "";
			}
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false, null), arg, localizedBlockName);
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00033CB0 File Offset: 0x00031EB0
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		BlockTrigger blockTrigger = _chunk.GetBlockTrigger(World.toBlock(_blockPos));
		if (blockTrigger != null)
		{
			bool flag = blockTrigger.HasAnyTriggeredBy();
			_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 0 : 1));
			_world.SetBlockRPC(_blockPos, _blockValue);
		}
		this.Refresh(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x00033D14 File Offset: 0x00031F14
	public override void OnTriggerAddedFromPrefab(BlockTrigger _trigger, Vector3i _blockPos, BlockValue _blockValue, FastTags<TagGroup.Global> _questTags)
	{
		if (GameManager.Instance.World.IsEditor())
		{
			return;
		}
		World world = GameManager.Instance.World;
		base.OnTriggerAddedFromPrefab(_trigger, _blockPos, _blockValue, _questTags);
		bool flag = _trigger.HasAnyTriggeredBy();
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 0 : 1));
		world.SetBlock(_trigger.ToWorldPos(), _blockValue, true, false);
		this.Refresh(GameManager.Instance.World, _trigger.Chunk, _trigger.LocalChunkPos, _blockValue);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x00033D98 File Offset: 0x00031F98
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!_world.IsEditor())
		{
			if (this.singleUse && (_blockValue.meta & 2) != 0)
			{
				return false;
			}
			if ((_blockValue.meta & 1) == 0)
			{
				return false;
			}
			if (_player.prefab == null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return false;
			}
		}
		if (!(_commandName == "activate"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _blockPos, true, true);
			}
		}
		else if (!_world.IsEditor())
		{
			bool flag = (_blockValue.meta & 2) > 0;
			if ((_blockValue.meta & 1) > 0 && (!flag || !this.singleUse))
			{
				base.HandleTrigger(_player, (World)_world, _blockPos, _blockValue);
				Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.activateSound, 0f);
				flag = !flag;
				_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
				_world.SetBlockRPC(_blockPos, _blockValue);
				this.Refresh(_world, null, _blockPos, _blockValue);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00033EC4 File Offset: 0x000320C4
	public override void OnTriggerRefresh(BlockTrigger _trigger, BlockValue _bv, FastTags<TagGroup.Global> questTag)
	{
		GameManager gm = GameManager.Instance;
		gm.StartCoroutine(gm.World.triggerManager.CheckPowerState(_trigger, questTag, delegate(bool powered)
		{
			_bv.meta = (byte)(((int)_bv.meta & -2) | (powered ? 1 : 0));
			gm.World.SetBlockRPC(_trigger.ToWorldPos(), _bv);
		}));
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00033F24 File Offset: 0x00032124
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = !_world.IsEditor();
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00033FB0 File Offset: 0x000321B0
	public override void Refresh(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.Refresh(_world, _chunk, _blockPos, _blockValue);
		bool flag = (_blockValue.meta & 2) > 0;
		bool flag2 = (_blockValue.meta & 1) > 0;
		BlockUtilityNavIcon.UpdateNavIcon(flag2 && !flag, _blockPos);
		IChunk chunk = _chunk;
		if (chunk == null)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			chunk = chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunk == null)
			{
				return;
			}
		}
		if (chunk == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunk.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		BlockSwitchController component = blockEntity.transform.GetComponent<BlockSwitchController>();
		if (component)
		{
			component.SetState(flag2, flag);
		}
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00034068 File Offset: 0x00032268
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | 1);
		_blockChanges.Add(new BlockChangeInfo(_blockPos, _blockValue));
	}

	// Token: 0x04000870 RID: 2160
	public const int cMetaPowered = 1;

	// Token: 0x04000871 RID: 2161
	public const int cMetaOn = 2;

	// Token: 0x04000872 RID: 2162
	[PublicizedFrom(EAccessModifier.Private)]
	public bool singleUse;

	// Token: 0x04000873 RID: 2163
	[PublicizedFrom(EAccessModifier.Private)]
	public string activateSound;

	// Token: 0x04000874 RID: 2164
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x04000875 RID: 2165
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSingleUse = "SingleUse";

	// Token: 0x04000876 RID: 2166
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivateSound = "ActivateSound";
}
