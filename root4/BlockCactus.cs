using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000113 RID: 275
[Preserve]
public class BlockCactus : BlockDamage
{
	// Token: 0x0600073F RID: 1855 RVA: 0x00034193 File Offset: 0x00032393
	public override void Init()
	{
		base.Init();
		this.IsTerrainDecoration = true;
		this.CanDecorateOnSlopes = false;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x000341AC File Offset: 0x000323AC
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		base.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedY, _result);
		Vector3 b = new Vector3(0.15f, 0.05f, 0.15f);
		Block block = _blockValue.Block;
		if (block.isMultiBlock && block.multiBlockPos.dim.y == 1)
		{
			b = new Vector3(0.15f, -0.75f, 0.15f);
		}
		for (int i = 0; i < _result.Count; i++)
		{
			Bounds value = _result[i];
			value.SetMinMax(value.min - b, value.max + b);
			_result[i] = value;
		}
	}
}
