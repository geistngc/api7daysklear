using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001A8 RID: 424
[Preserve]
public class BlockShapeDistantDecoTree : BlockShapeDistantDeco
{
	// Token: 0x06000CCF RID: 3279 RVA: 0x00050867 File Offset: 0x0004EA67
	public BlockShapeDistantDecoTree()
	{
		this.Has45DegreeRotations = true;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x00050878 File Offset: 0x0004EA78
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
			gameObject.layer = 23;
		}
		Transform transform = _ebcd.transform.Find("rootBall");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00050909 File Offset: 0x0004EB09
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
			return;
		}
		DecoManager.Instance.RemoveDecorationAt(_blockPos);
	}
}
