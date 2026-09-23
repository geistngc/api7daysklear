using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200010E RID: 270
[Preserve]
public class BlockActivate : Block
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x0600070D RID: 1805 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x000333F4 File Offset: 0x000315F4
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("ActivateSound"))
		{
			this.activateSound = base.Properties.Values["ActivateSound"];
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00033430 File Offset: 0x00031630
	public override void LateInit()
	{
		base.LateInit();
		if (base.Properties.Values.ContainsKey(BlockActivate.PropBlockChangeTo))
		{
			this.blockChangeTo = Block.GetBlockValue(base.Properties.Values[BlockActivate.PropBlockChangeTo], false);
			this.useChangeTo = true;
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00033482 File Offset: 0x00031682
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _blockPos, _oldBlockValue, _newBlockValue);
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00033494 File Offset: 0x00031694
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false, null), arg, localizedBlockName);
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x000334F8 File Offset: 0x000316F8
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!(_commandName == "activate"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _blockPos, true, false);
			}
		}
		else if (!_world.IsEditor())
		{
			base.HandleTrigger(_player, (World)_world, _blockPos, _blockValue);
			Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.activateSound, 0f);
			if (this.useChangeTo)
			{
				this.blockChangeTo.rotation = _blockValue.rotation;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00033598 File Offset: 0x00031798
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = true;
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x04000861 RID: 2145
	[PublicizedFrom(EAccessModifier.Private)]
	public string activateSound;

	// Token: 0x04000862 RID: 2146
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockChangeTo = BlockValue.Air;

	// Token: 0x04000863 RID: 2147
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useChangeTo;

	// Token: 0x04000864 RID: 2148
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x04000865 RID: 2149
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBlockChangeTo = "BlockChangeTo";
}
