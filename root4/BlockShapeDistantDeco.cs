using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001A7 RID: 423
[Preserve]
public class BlockShapeDistantDeco : BlockShapeModelEntity
{
	// Token: 0x06000CC9 RID: 3273 RVA: 0x00050724 File Offset: 0x0004E924
	public override void Init(Block _block)
	{
		base.Init(_block);
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x0005072D File Offset: 0x0004E92D
	public override void OnBlockValueChanged(WorldBase _world, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockValueChanged(_world, _blockPos, _oldBlockValue, _newBlockValue);
			return;
		}
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00050747 File Offset: 0x0004E947
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
			return;
		}
		if (!_blockValue.Block.IsDistantDecoration)
		{
			return;
		}
		DecoManager.Instance.AddDecorationAt((World)_world, _blockValue, _blockPos, true);
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00050784 File Offset: 0x0004E984
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		}
		DecoManager.Instance.RemoveDecorationAt(_blockPos);
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x000507AC File Offset: 0x0004E9AC
	public override void OnBlockLoaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockLoaded(_world, _blockPos, _blockValue);
			return;
		}
		if (_world.IsRemote())
		{
			if (!_blockValue.Block.IsDistantDecoration)
			{
				return;
			}
			DecoManager.Instance.AddDecorationAt((World)_world, _blockValue, _blockPos, true);
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000507FC File Offset: 0x0004E9FC
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
			return;
		}
		_ebcd.transform.tag = "T_Deco";
		Collider[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = componentsInChildren[i].gameObject;
			gameObject.tag = "T_Deco";
			gameObject.layer = 16;
		}
	}
}
