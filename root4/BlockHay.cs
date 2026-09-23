using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000120 RID: 288
[Preserve]
public class BlockHay : Block
{
	// Token: 0x06000799 RID: 1945 RVA: 0x00036123 File Offset: 0x00034323
	public BlockHay()
	{
		this.IsCheckCollideWithEntity = true;
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00036134 File Offset: 0x00034334
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		float num = 0.0625f;
		_result.Add(BoundsUtils.BoundsForMinMax((float)_x + num, (float)_y, (float)_z + num, (float)(_x + 1) - num, (float)(_y + 1) - num, (float)(_z + 1) - num));
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0003550F File Offset: 0x0003370F
	public override IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, (int)_blockPos.x, (int)_blockPos.y, (int)_blockPos.z, 0f, Block.staticList_IntersectRayWithBlockList);
		return Block.staticList_IntersectRayWithBlockList;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00036171 File Offset: 0x00034371
	public override bool OnEntityCollidedWithBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, Entity _e)
	{
		_e.fallDistance = Mathf.Max(_e.fallDistance - 5f, 0f);
		return true;
	}
}
