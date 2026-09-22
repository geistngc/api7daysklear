using System;
using System.Collections;

// Token: 0x02000B2B RID: 2859
public class ChunkProviderDummy : ChunkProviderAbstract
{
	// Token: 0x06005696 RID: 22166 RVA: 0x00213C4F File Offset: 0x00211E4F
	public override IEnumerator Init(World _worldData)
	{
		MultiBlockManager.Instance.Initialize(null);
		yield return null;
		yield break;
	}

	// Token: 0x06005697 RID: 22167 RVA: 0x00213C57 File Offset: 0x00211E57
	public override void Cleanup()
	{
		base.Cleanup();
		MultiBlockManager.Instance.Cleanup();
	}

	// Token: 0x06005698 RID: 22168 RVA: 0x00046EF6 File Offset: 0x000450F6
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.NetworkClient;
	}

	// Token: 0x06005699 RID: 22169 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void setChunkPrerequisits(string sSeed)
	{
	}

	// Token: 0x0600569A RID: 22170 RVA: 0x00213C69 File Offset: 0x00211E69
	public override void UnloadChunk(Chunk _c)
	{
		MemoryPools.PoolChunks.FreeSync(_c);
	}

	// Token: 0x0600569B RID: 22171 RVA: 0x00213C76 File Offset: 0x00211E76
	public override SpawnPointList GetSpawnPointList()
	{
		return this.dummySpawnPointList;
	}

	// Token: 0x0600569C RID: 22172 RVA: 0x00213C7E File Offset: 0x00211E7E
	public override void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
		this.dummySpawnPointList = _spawnPointList;
	}

	// Token: 0x04004315 RID: 17173
	[PublicizedFrom(EAccessModifier.Private)]
	public SpawnPointList dummySpawnPointList = new SpawnPointList();
}
