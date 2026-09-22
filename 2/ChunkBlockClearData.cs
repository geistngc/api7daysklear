using System;
using System.Collections.Generic;

// Token: 0x02000B0C RID: 2828
public class ChunkBlockClearData : ChunkCustomData
{
	// Token: 0x06005536 RID: 21814 RVA: 0x00209B34 File Offset: 0x00207D34
	public ChunkBlockClearData()
	{
	}

	// Token: 0x06005537 RID: 21815 RVA: 0x00209B47 File Offset: 0x00207D47
	public ChunkBlockClearData(string _key, ulong _expiresInWorldTime, bool _isSavedToNetwork, World _world) : base(_key, _expiresInWorldTime, _isSavedToNetwork)
	{
		this.World = _world;
	}

	// Token: 0x06005538 RID: 21816 RVA: 0x00209B68 File Offset: 0x00207D68
	public override void OnRemove(Chunk chunk)
	{
		for (int i = this.BlockList.Count - 1; i >= 0; i--)
		{
			Vector3i vector3i = this.BlockList[i];
			chunk.SetBlock(this.World, vector3i.x, vector3i.y, vector3i.z, BlockValue.Air, true, true, false, false, -1);
		}
	}

	// Token: 0x0400422C RID: 16940
	public List<Vector3i> BlockList = new List<Vector3i>();

	// Token: 0x0400422D RID: 16941
	public World World;
}
