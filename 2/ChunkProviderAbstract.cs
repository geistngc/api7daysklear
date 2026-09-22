using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B47 RID: 2887
public abstract class ChunkProviderAbstract : IChunkProvider
{
	// Token: 0x06005791 RID: 22417 RVA: 0x00218D9F File Offset: 0x00216F9F
	public virtual IEnumerator Init(World _worldData)
	{
		yield return null;
		yield break;
	}

	// Token: 0x06005792 RID: 22418 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Update()
	{
	}

	// Token: 0x06005793 RID: 22419 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StopUpdate()
	{
	}

	// Token: 0x06005794 RID: 22420 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Cleanup()
	{
	}

	// Token: 0x06005795 RID: 22421 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RequestChunk(int _x, int _y)
	{
	}

	// Token: 0x06005796 RID: 22422 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual HashSetList<long> GetRequestedChunks()
	{
		return null;
	}

	// Token: 0x06005797 RID: 22423 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SaveAll()
	{
	}

	// Token: 0x06005798 RID: 22424 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet)
	{
	}

	// Token: 0x06005799 RID: 22425 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ReloadAllChunks()
	{
	}

	// Token: 0x0600579A RID: 22426 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ClearCaches()
	{
	}

	// Token: 0x0600579B RID: 22427 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.None;
	}

	// Token: 0x0600579C RID: 22428 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void UnloadChunk(Chunk _chunk)
	{
	}

	// Token: 0x0600579D RID: 22429 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		return null;
	}

	// Token: 0x0600579E RID: 22430 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual EventPrefabs GetEventPrefabs()
	{
		return null;
	}

	// Token: 0x0600579F RID: 22431 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual SpawnPointList GetSpawnPointList()
	{
		return null;
	}

	// Token: 0x060057A0 RID: 22432 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
	}

	// Token: 0x060057A1 RID: 22433 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		return false;
	}

	// Token: 0x060057A2 RID: 22434 RVA: 0x00218DA7 File Offset: 0x00216FA7
	public virtual void SetDecorationsEnabled(bool _bEnable)
	{
		this.bDecorationsEnabled = _bEnable;
	}

	// Token: 0x060057A3 RID: 22435 RVA: 0x00218DB0 File Offset: 0x00216FB0
	public virtual bool IsDecorationsEnabled()
	{
		return this.bDecorationsEnabled;
	}

	// Token: 0x060057A4 RID: 22436 RVA: 0x00218DB8 File Offset: 0x00216FB8
	public virtual bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = Vector3i.zero;
		_maxSize = Vector3i.zero;
		return false;
	}

	// Token: 0x060057A5 RID: 22437 RVA: 0x00218DD4 File Offset: 0x00216FD4
	public virtual BoundsInt GetWorldBounds()
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		return new BoundsInt(vector3i.x, vector3i.y, vector3i.z, vector3i2.x, vector3i2.y, vector3i2.z);
	}

	// Token: 0x060057A6 RID: 22438 RVA: 0x00218E18 File Offset: 0x00217018
	public virtual Vector2i GetWorldSize()
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		return new Vector2i(vector3i2.x - vector3i.x + 1, vector3i2.y - vector3i.y + 1);
	}

	// Token: 0x060057A7 RID: 22439 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual IBiomeProvider GetBiomeProvider()
	{
		return null;
	}

	// Token: 0x060057A8 RID: 22440 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual ITerrainGenerator GetTerrainGenerator()
	{
		return null;
	}

	// Token: 0x060057A9 RID: 22441 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual int GetPOIBlockIdOverride(int x, int z)
	{
		return 0;
	}

	// Token: 0x060057AA RID: 22442 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	public virtual float GetPOIHeightOverride(int x, int z)
	{
		return 0f;
	}

	// Token: 0x060057AB RID: 22443 RVA: 0x00218E53 File Offset: 0x00217053
	public virtual IEnumerator FillOccupiedMap(int w, int h, DecoOccupiedMap occupiedMap, List<PrefabInstance> overridePOIList = null)
	{
		yield break;
	}

	// Token: 0x060057AC RID: 22444 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _isStopStabilityCalc, bool _isRegenChunk, bool _isFillEmptyBlocks, bool _isReset)
	{
	}

	// Token: 0x060057AD RID: 22445 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return ChunkProtectionLevel.None;
	}

	// Token: 0x17000944 RID: 2372
	// (get) Token: 0x060057AE RID: 22446 RVA: 0x00218E5B File Offset: 0x0021705B
	// (set) Token: 0x060057AF RID: 22447 RVA: 0x00218E63 File Offset: 0x00217063
	public GameUtils.WorldInfo WorldInfo { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x060057B0 RID: 22448 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public ChunkProviderAbstract()
	{
	}

	// Token: 0x040043BF RID: 17343
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDecorationsEnabled;
}
