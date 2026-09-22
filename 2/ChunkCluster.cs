using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B15 RID: 2837
public class ChunkCluster : WorldChunkCache, IBlockAccess
{
	// Token: 0x1400007F RID: 127
	// (add) Token: 0x06005596 RID: 21910 RVA: 0x0020BCEC File Offset: 0x00209EEC
	// (remove) Token: 0x06005597 RID: 21911 RVA: 0x0020BD24 File Offset: 0x00209F24
	public event ChunkCluster.OnBlockDamagedDelegate OnBlockDamagedDelegates;

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x06005598 RID: 21912 RVA: 0x0020BD5C File Offset: 0x00209F5C
	// (remove) Token: 0x06005599 RID: 21913 RVA: 0x0020BD94 File Offset: 0x00209F94
	public event ChunkCluster.OnChunksFinishedLoadingDelegate OnChunksFinishedLoadingDelegates;

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x0600559A RID: 21914 RVA: 0x0020BDCC File Offset: 0x00209FCC
	// (remove) Token: 0x0600559B RID: 21915 RVA: 0x0020BE04 File Offset: 0x0020A004
	public event ChunkCluster.OnChunksFinishedDisplayingDelegate OnChunksFinishedDisplayingDelegates;

	// Token: 0x14000082 RID: 130
	// (add) Token: 0x0600559C RID: 21916 RVA: 0x0020BE3C File Offset: 0x0020A03C
	// (remove) Token: 0x0600559D RID: 21917 RVA: 0x0020BE74 File Offset: 0x0020A074
	public event ChunkCluster.OnChunkVisibleDelegate OnChunkVisibleDelegates;

	// Token: 0x14000083 RID: 131
	// (add) Token: 0x0600559E RID: 21918 RVA: 0x0020BEAC File Offset: 0x0020A0AC
	// (remove) Token: 0x0600559F RID: 21919 RVA: 0x0020BEE4 File Offset: 0x0020A0E4
	public event ChunkCluster.OnBlockChangedDelegate OnBlockChangedDelegates;

	// Token: 0x060055A0 RID: 21920 RVA: 0x0020BF1C File Offset: 0x0020A11C
	public ChunkCluster(World _world, string _name)
	{
		this.Name = _name;
		this.world = _world;
	}

	// Token: 0x060055A1 RID: 21921 RVA: 0x0020BF74 File Offset: 0x0020A174
	public IEnumerator Init(EnumChunkProviderId _providerId, PathAbstractions.AbstractedLocation _worldLocation)
	{
		this.nChunks = new ChunkCacheNeighborChunks(this);
		this.nBlocks = new ChunkCacheNeighborBlocks(this.nChunks);
		this.meshGenerator = new MeshGeneratorMC2(this.nBlocks, this.nChunks);
		this.stabilityCalcMainThread = new StabilityCalculator();
		this.stabilityCalcLightingThread = new StabilityInitializer(this.world);
		this.m_LightProcessorMainThread = new LightProcessor(this);
		this.m_LightProcessorLightingThread = new LightProcessor(this);
		if (this.world.GetGameManager() != null)
		{
			this.stabilityCalcMainThread.Init(this.world);
		}
		this.ChunkProvider = null;
		switch (_providerId)
		{
		case EnumChunkProviderId.Disc:
			this.ChunkProvider = new ChunkProviderDisc(this, this.Name, _worldLocation);
			break;
		case EnumChunkProviderId.GenerateFromDtm:
			this.ChunkProvider = new ChunkProviderGenerateWorldFromImage(this.Name, _worldLocation, true);
			break;
		case EnumChunkProviderId.NetworkClient:
			if (!this.IsFixedSize)
			{
				this.ChunkProvider = new ChunkProviderGenerateWorldFromRaw(this.Name, _worldLocation, true, true);
			}
			else
			{
				this.ChunkProvider = new ChunkProviderDummy();
			}
			break;
		case EnumChunkProviderId.ChunkDataDriven:
			this.ChunkProvider = new ChunkProviderGenerateWorldFromRaw(this.Name, _worldLocation, false, false);
			break;
		case EnumChunkProviderId.FlatWorld:
			this.ChunkProvider = new ChunkProviderGenerateFlat(this, _worldLocation.FullPath);
			break;
		}
		yield return null;
		if (this.ChunkProvider != null)
		{
			yield return this.ChunkProvider.Init(this.world);
		}
		yield break;
	}

	// Token: 0x060055A2 RID: 21922 RVA: 0x0020BF94 File Offset: 0x0020A194
	public void Cleanup()
	{
		ChunkManager chunkManager = this.world.m_ChunkManager;
		if (this.ChunkProvider != null)
		{
			this.ChunkProvider.StopUpdate();
		}
		List<Chunk> chunkArrayCopySync = base.GetChunkArrayCopySync();
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			this.RemoveChunk(chunk);
			this.UnloadChunk(chunk);
		}
		chunkManager.ClearChunksForAllObservers(this);
		if (this.ChunkProvider != null)
		{
			this.ChunkProvider.Cleanup();
			this.ChunkProvider = null;
		}
		DictionarySave<long, ChunkGameObject> displayedChunkGameObjects = this.DisplayedChunkGameObjects;
		lock (displayedChunkGameObjects)
		{
			long[] array = new long[this.DisplayedChunkGameObjects.Count];
			this.DisplayedChunkGameObjects.Dict.CopyKeysTo(array);
			foreach (long num in array)
			{
				ChunkGameObject chunkGameObject = this.DisplayedChunkGameObjects[num];
				chunkManager.FreeChunkGameObject(this, num);
			}
			this.DisplayedChunkGameObjects.Clear();
		}
		if (this.stabilityCalcMainThread != null)
		{
			this.stabilityCalcMainThread.Cleanup();
			this.stabilityCalcMainThread = null;
		}
		this.stabilityCalcLightingThread = null;
		this.OnBlockDamagedDelegates = null;
		this.OnChunksFinishedLoadingDelegates = null;
		this.OnChunksFinishedDisplayingDelegates = null;
		this.OnChunkVisibleDelegates = null;
		this.OnBlockChangedDelegates = null;
		ChunkCluster.chunkRegenerationStartTimestamps.Clear();
		ChunkCluster.chunkRegenerationEndTimestamps.Clear();
		ChunkCluster.chunkRegenerationHistories.Clear();
	}

	// Token: 0x060055A3 RID: 21923 RVA: 0x0020C104 File Offset: 0x0020A304
	public World GetWorld()
	{
		return this.world;
	}

	// Token: 0x060055A4 RID: 21924 RVA: 0x0020C10C File Offset: 0x0020A30C
	public List<Vector3i> GetIndexedBlocks(string _name)
	{
		List<Vector3i> list = new List<Vector3i>();
		List<Chunk> chunkArrayCopySync = base.GetChunkArrayCopySync();
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk obj = chunkArrayCopySync[i];
			lock (obj)
			{
				if (!chunkArrayCopySync[i].InProgressUnloading)
				{
					List<Vector3i> list2 = chunkArrayCopySync[i].IndexedBlocks[_name];
					if (list2 != null)
					{
						for (int j = 0; j < list2.Count; j++)
						{
							Vector3i pos = list2[j];
							list.Add(chunkArrayCopySync[i].ToWorldPos(pos));
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060055A5 RID: 21925 RVA: 0x0020C1D0 File Offset: 0x0020A3D0
	public override bool AddChunkSync(Chunk _chunk, bool _bOmitCallbacks = false)
	{
		bool result = base.AddChunkSync(_chunk, _bOmitCallbacks);
		this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.Loaded);
		if (!_bOmitCallbacks && this.IsFixedSize && !this.bFinishedLoadingDelegateCalled)
		{
			if (this.chunkKeysNeedLoading == null)
			{
				this.chunkKeysNeedLoading = new HashSetLong();
				for (int i = this.ChunkMinPos.x; i <= this.ChunkMaxPos.x; i++)
				{
					for (int j = this.ChunkMinPos.y; j <= this.ChunkMaxPos.y; j++)
					{
						long item = WorldChunkCache.MakeChunkKey(i, j);
						this.chunkKeysNeedLoading.Add(item);
					}
				}
			}
			if (this.chunkKeysNeedDisplaying == null)
			{
				this.chunkKeysNeedDisplaying = new HashSetLong();
				for (int k = this.ChunkMinPos.x + 2; k <= this.ChunkMaxPos.x - 2; k++)
				{
					for (int l = this.ChunkMinPos.y + 2; l <= this.ChunkMaxPos.y - 2; l++)
					{
						long item2 = WorldChunkCache.MakeChunkKey(k, l);
						this.chunkKeysNeedDisplaying.Add(item2);
					}
				}
			}
			this.chunkKeysNeedLoading.Remove(_chunk.Key);
			if (this.chunkKeysNeedDisplaying.Count > 0 && _chunk.IsEmpty())
			{
				this.chunkKeysNeedDisplaying.Remove(_chunk.Key);
			}
			if (this.chunkKeysNeedLoading.Count == 0)
			{
				this.NotifyOnChunksFinishedLoading();
			}
		}
		return result;
	}

	// Token: 0x060055A6 RID: 21926 RVA: 0x0020C341 File Offset: 0x0020A541
	public void NotifyOnChunksFinishedLoading()
	{
		if (this.OnChunksFinishedLoadingDelegates != null)
		{
			this.OnChunksFinishedLoadingDelegates();
			this.bFinishedLoadingDelegateCalled = true;
		}
	}

	// Token: 0x060055A7 RID: 21927 RVA: 0x0020C35D File Offset: 0x0020A55D
	public void RemoveChunk(Chunk _chunk)
	{
		_chunk.OnUnload(this.world);
		this.RemoveChunkSync(_chunk.Key);
		this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.Removed);
	}

	// Token: 0x060055A8 RID: 21928 RVA: 0x0020C384 File Offset: 0x0020A584
	public void UnloadChunk(Chunk _chunk)
	{
		if (this.ChunkProvider != null)
		{
			_chunk.NeedsRegeneration = true;
			this.ChunkProvider.UnloadChunk(_chunk);
			this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.Unloaded);
		}
	}

	// Token: 0x060055A9 RID: 21929 RVA: 0x0020C3B0 File Offset: 0x0020A5B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void addDistantDecorationBlocks(Chunk _chunk)
	{
		this.multiBlockList.Clear();
		World world = GameManager.Instance.World;
		DecoManager.Instance.GetDecorationsOnChunk(_chunk.X, _chunk.Z, this.multiBlockList);
		for (int i = this.multiBlockList.Count - 1; i >= 0; i--)
		{
			SBlockPosValue sblockPosValue = this.multiBlockList[i];
			int x = World.toBlockXZ(sblockPosValue.blockPos.x);
			int y = sblockPosValue.blockPos.y;
			int z = World.toBlockXZ(sblockPosValue.blockPos.z);
			if (!_chunk.GetBlock(x, y, z).Block.isMultiBlock)
			{
				Block block = sblockPosValue.blockValue.Block;
				if (block.isMultiBlock)
				{
					for (int j = block.multiBlockPos.Length - 1; j >= 0; j--)
					{
						Vector3i vector3i = block.multiBlockPos.Get(j, sblockPosValue.blockValue.type, (int)sblockPosValue.blockValue.rotation);
						Vector3i vector3i2 = sblockPosValue.blockPos + vector3i;
						BlockValue blockValue = sblockPosValue.blockValue;
						if (vector3i.x != 0 || vector3i.y != 0 || vector3i.z != 0)
						{
							blockValue.ischild = true;
							blockValue.parentx = -vector3i.x;
							blockValue.parenty = -vector3i.y;
							blockValue.parentz = -vector3i.z;
						}
						sbyte density = this.GetDensity(vector3i2);
						this.SetBlockRaw(vector3i2, blockValue);
						this.SetDensityRaw(vector3i2, density);
						this.SetStability(vector3i2, 15);
					}
				}
				else if (world.GetBlock(sblockPosValue.blockPos).isair)
				{
					this.SetBlockRaw(sblockPosValue.blockPos, sblockPosValue.blockValue);
				}
			}
		}
	}

	// Token: 0x060055AA RID: 21930 RVA: 0x0020C587 File Offset: 0x0020A787
	public void LightChunk(Chunk chunk, Chunk[] _neighbours)
	{
		this.addDistantDecorationBlocks(chunk);
		if (this.m_LightProcessorLightingThread != null)
		{
			this.m_LightProcessorLightingThread.LightChunk(chunk);
			chunk.CheckSameLight();
		}
		this.CalcStability(chunk);
		chunk.CalcBiomeIntensity(_neighbours);
		chunk.CalcDominantBiome();
	}

	// Token: 0x060055AB RID: 21931 RVA: 0x0020C5BE File Offset: 0x0020A7BE
	public void CalcStability(Chunk chunk)
	{
		if (this.stabilityCalcLightingThread != null)
		{
			this.stabilityCalcLightingThread.DistributeStability(chunk);
			chunk.CheckSameStability();
		}
	}

	// Token: 0x060055AC RID: 21932 RVA: 0x0020C5DC File Offset: 0x0020A7DC
	public void RegenerateChunk(Chunk _chunk, Chunk[] _neighbours)
	{
		this.nChunks.Init(_chunk, _neighbours);
		while (_chunk.NeedsRegeneration)
		{
			VoxelMeshLayer voxelMeshLayer = null;
			for (int i = 0; i < 16; i++)
			{
				if ((_chunk.NeedsRegenerationAt & 1 << i) != 0)
				{
					if (!this.meshGenerator.IsLayerEmpty(i))
					{
						voxelMeshLayer = MemoryPools.poolVML.AllocSync(true);
						voxelMeshLayer.idx = i;
						voxelMeshLayer.SizeToChunkDefaults();
						break;
					}
					_chunk.ClearNeedsRegenerationAt(i);
				}
			}
			if (voxelMeshLayer == null)
			{
				this.nChunks.Clear();
				return;
			}
			ChunkCluster.currentChunkKey = _chunk.Key;
			ChunkCluster.currentChunkVMLIndex = voxelMeshLayer.idx;
			this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.Starting);
			ChunkCluster.chunkRegenerationStartTimestamps[_chunk.Key] = new ValueTuple<int, DateTime>(voxelMeshLayer.idx, DateTime.UtcNow);
			_chunk.ClearNeedsRegenerationAt(voxelMeshLayer.idx);
			this.nChunks.Init(_chunk, _neighbours);
			this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.GeneratingMeshes);
			Vector3i chunkPos = new Vector3i(_chunk.X << 4, _chunk.Y << 8 - voxelMeshLayer.idx * 16, _chunk.Z << 4);
			this.meshGenerator.GenerateMesh(chunkPos, voxelMeshLayer.idx, voxelMeshLayer.meshes);
			this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.AddingMeshLayer);
			_chunk.AddMeshLayer(voxelMeshLayer);
			this.UpdateRegenerationState(_chunk.Key, ChunkCluster.ChunkState.Regenerated);
			ChunkCluster.chunkRegenerationEndTimestamps[_chunk.Key] = new ValueTuple<int, DateTime>(voxelMeshLayer.idx, DateTime.UtcNow);
		}
		this.nBlocks.Clear();
		this.nChunks.Clear();
	}

	// Token: 0x060055AD RID: 21933 RVA: 0x0020C770 File Offset: 0x0020A970
	public bool IsOnBorder(Chunk _c)
	{
		return this.IsFixedSize && (_c.X == this.ChunkMinPos.x || _c.X == this.ChunkMaxPos.x || _c.Z == this.ChunkMinPos.y || _c.Z == this.ChunkMaxPos.y);
	}

	// Token: 0x060055AE RID: 21934 RVA: 0x0020C7D8 File Offset: 0x0020A9D8
	public sbyte GetDensity(Vector3i _worldPos)
	{
		IChunk chunkSync = base.GetChunkSync(World.toChunkXZ(_worldPos.x), World.toChunkXZ(_worldPos.z));
		if (chunkSync == null)
		{
			return MarchingCubes.DensityAir;
		}
		Vector3i vector3i = World.toBlock(_worldPos);
		return chunkSync.GetDensity(vector3i.x, vector3i.y, vector3i.z);
	}

	// Token: 0x060055AF RID: 21935 RVA: 0x0020C82C File Offset: 0x0020AA2C
	public void SetDensity(Vector3i _pos, sbyte _density, bool _isForceDensity = false)
	{
		this.SetBlock(_pos, false, BlockValue.Air, true, _density, false, false, _isForceDensity, false, -1);
	}

	// Token: 0x060055B0 RID: 21936 RVA: 0x0020C850 File Offset: 0x0020AA50
	public void SetDensityRaw(Vector3i _pos, sbyte _density)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_pos);
		if (chunk == null)
		{
			return;
		}
		int x = World.toBlockXZ(_pos.x);
		int z = World.toBlockXZ(_pos.z);
		int y = World.toBlockY(_pos.y);
		chunk.SetDensity(x, y, z, _density);
	}

	// Token: 0x060055B1 RID: 21937 RVA: 0x0020C89C File Offset: 0x0020AA9C
	public void SetStability(Vector3i _pos, byte _v)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_pos);
		if (chunk == null)
		{
			return;
		}
		int x = World.toBlockXZ(_pos.x);
		int z = World.toBlockXZ(_pos.z);
		int y = World.toBlockY(_pos.y);
		chunk.SetStability(x, y, z, _v);
	}

	// Token: 0x060055B2 RID: 21938 RVA: 0x0020C8E8 File Offset: 0x0020AAE8
	public WaterValue GetWater(Vector3i _pos)
	{
		if (_pos.y < 256)
		{
			IChunk chunkFromWorldPos = base.GetChunkFromWorldPos(_pos);
			if (chunkFromWorldPos != null)
			{
				return chunkFromWorldPos.GetWater(World.toBlockXZ(_pos.x), _pos.y, World.toBlockXZ(_pos.z));
			}
		}
		return WaterValue.Empty;
	}

	// Token: 0x060055B3 RID: 21939 RVA: 0x0020C938 File Offset: 0x0020AB38
	public void SetWater(Vector3i _pos, WaterValue _waterData)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_pos);
		if (chunk == null)
		{
			return;
		}
		int num = World.toBlockXZ(_pos.x);
		int num2 = World.toBlockXZ(_pos.z);
		int num3 = World.toBlockY(_pos.y);
		chunk.SetWater(num, num3, num2, _waterData);
		this.chunkPosNeedsRegeneration(chunk, num, num3, num2, false);
	}

	// Token: 0x060055B4 RID: 21940 RVA: 0x0020C990 File Offset: 0x0020AB90
	public BlockValue GetBlock(int x, int y, int z)
	{
		if (y < 256)
		{
			IChunk chunkFromWorldPos = base.GetChunkFromWorldPos(x, y, z);
			if (chunkFromWorldPos != null)
			{
				return chunkFromWorldPos.GetBlock(World.toBlockXZ(x), y, World.toBlockXZ(z));
			}
		}
		return BlockValue.Air;
	}

	// Token: 0x060055B5 RID: 21941 RVA: 0x001B2C80 File Offset: 0x001B0E80
	public BlockValue GetBlock(Vector3i pos)
	{
		return IBlockAccess.DefaultGetBlock(this, pos);
	}

	// Token: 0x060055B6 RID: 21942 RVA: 0x001B2C89 File Offset: 0x001B0E89
	public BlockValue GetBlock(BlockValueRef bvRef)
	{
		return IBlockAccess.DefaultGetBlock(this, bvRef);
	}

	// Token: 0x060055B7 RID: 21943 RVA: 0x0020C9CC File Offset: 0x0020ABCC
	public BlockValue SetBlock(BlockValueRef _bvRef, BlockValue _bv, bool _isNotify, bool _isUpdateLight)
	{
		return this.SetBlock(_bvRef, true, _bv, false, 0, _isNotify, _isUpdateLight, false, false, -1);
	}

	// Token: 0x060055B8 RID: 21944 RVA: 0x0020C9EC File Offset: 0x0020ABEC
	public BlockValue SetBlock(BlockValueRef _bvRef, bool _isChangeBV, BlockValue _bv, bool _isChangeDensity, sbyte _density, bool _isNotify, bool _isUpdateLight, bool _isForceDensity = false, bool _wasChild = false, int _changedByEntityId = -1)
	{
		BlockValue result;
		switch (_bvRef.Type)
		{
		case BlockValueRefType.None:
			result = BlockValue.Air;
			break;
		case BlockValueRefType.Block:
			result = this.SetBlock(_bvRef.BlockPosition, _isChangeBV, _bv, _isChangeDensity, _density, _isNotify, _isUpdateLight, _isForceDensity, _wasChild, _changedByEntityId);
			break;
		case BlockValueRefType.Prop:
		{
			PropRef propReference = _bvRef.PropReference;
			BlockValue? blockValue = new BlockValue?(_bv);
			result = this.SetProp(propReference, null, null, null, blockValue).blockValue;
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x060055B9 RID: 21945 RVA: 0x0020CA7C File Offset: 0x0020AC7C
	public unsafe BlockValue SetBlock(Vector3i _pos, bool _isChangeBV, BlockValue _bv, bool _isChangeDensity, sbyte _density, bool _isNotify, bool _isUpdateLight, bool _isForceDensity = false, bool _wasChild = false, int _changedByEntityId = -1)
	{
		if (_pos.y <= 0 || _pos.y >= 255)
		{
			return BlockValue.Air;
		}
		Block block = _bv.Block;
		if (block == null)
		{
			return BlockValue.Air;
		}
		int num = World.toChunkXZ(_pos.x);
		int num2 = World.toChunkXZ(_pos.z);
		if (this.IsFixedSize && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (num <= this.ChunkMinPos.x + 2)
			{
				int x = this.ChunkMinPos.x;
				this.ChunkMinPos.x = num - 3;
				for (int i = this.ChunkMinPos.x; i < x; i++)
				{
					for (int j = this.ChunkMinPos.y; j <= this.ChunkMaxPos.y; j++)
					{
						this.AddChunkSync(new Chunk(i, j), false);
					}
				}
			}
			if (num >= this.ChunkMaxPos.x - 2)
			{
				int x2 = this.ChunkMaxPos.x;
				this.ChunkMaxPos.x = num + 3;
				for (int k = x2 + 1; k <= this.ChunkMaxPos.x; k++)
				{
					for (int l = this.ChunkMinPos.y; l <= this.ChunkMaxPos.y; l++)
					{
						this.AddChunkSync(new Chunk(k, l), false);
					}
				}
			}
			if (num2 <= this.ChunkMinPos.y + 2)
			{
				int y = this.ChunkMinPos.y;
				this.ChunkMinPos.y = num2 - 3;
				for (int m = this.ChunkMinPos.y; m < y; m++)
				{
					for (int n = this.ChunkMinPos.x; n <= this.ChunkMaxPos.x; n++)
					{
						this.AddChunkSync(new Chunk(n, m), false);
					}
				}
			}
			if (num2 >= this.ChunkMaxPos.y - 2)
			{
				int y2 = this.ChunkMaxPos.y;
				this.ChunkMaxPos.y = num2 + 3;
				for (int num3 = y2 + 1; num3 <= this.ChunkMaxPos.y; num3++)
				{
					for (int num4 = this.ChunkMinPos.x; num4 <= this.ChunkMaxPos.x; num4++)
					{
						this.AddChunkSync(new Chunk(num4, num3), false);
					}
				}
			}
		}
		Chunk chunkSync = base.GetChunkSync(num, num2);
		if (chunkSync == null)
		{
			if (_bv.isair)
			{
				DecoManager.Instance.SetBlock(this.world, _pos, BlockValue.Air);
			}
			else if (block.IsDistantDecoration)
			{
				DecoManager.Instance.SetBlock(this.world, _pos, _bv);
			}
			return BlockValue.Air;
		}
		int num5 = World.toBlockXZ(_pos.x);
		int blockY = World.toBlockY(_pos.y);
		int num6 = World.toBlockXZ(_pos.z);
		BlockValue blockValue = BlockValue.Air;
		if (_isChangeBV)
		{
			blockValue = chunkSync.SetBlock(this.world, num5, _pos.y, num6, _bv, true, !_wasChild, false, false, _changedByEntityId);
		}
		bool flag;
		bool flag2;
		bool flag3;
		blockValue.Block.CheckUpdate(blockValue, _bv, out flag, out flag2, out flag3);
		if (_isNotify && !flag2)
		{
			_isNotify = false;
		}
		if (_isUpdateLight && !flag3)
		{
			_isUpdateLight = false;
		}
		if (!_isChangeBV)
		{
			_bv = chunkSync.GetBlock(num5, _pos.y, num6);
			block = _bv.Block;
		}
		sbyte density = chunkSync.GetDensity(num5, _pos.y, num6);
		if (!_isChangeDensity)
		{
			_density = density;
		}
		if (!_isForceDensity)
		{
			if (block.shape.IsTerrain())
			{
				if (_density > MarchingCubes.DensityAirHi)
				{
					_density = MarchingCubes.DensityAirHi;
					_isChangeDensity = true;
				}
			}
			else if (_density < MarchingCubes.DensityTerrainHi)
			{
				_density = MarchingCubes.DensityTerrainHi;
				_isChangeDensity = true;
			}
		}
		if (_isChangeDensity)
		{
			chunkSync.SetDensity(num5, _pos.y, num6, _density);
		}
		TextureFullArray textureFullArray = chunkSync.GetTextureFullArray(num5, _pos.y, num6, true);
		if (_isChangeBV && _isNotify && (!blockValue.Equals(_bv) || blockValue.damage != _bv.damage))
		{
			if (!this.isInNotify && !this.world.IsRemote())
			{
				this.isInNotify = true;
				this.notifyBlocksOfNeighborChange(_pos, _bv, blockValue);
				this.isInNotify = false;
			}
			bool flag4 = !chunkSync.StopStabilityCalculation;
			if (flag4 && blockValue.Block.isMultiBlock)
			{
				Span<long> span = new Span<long>(stackalloc byte[(UIntPtr)24], 3);
				span.Clear();
				Block.MultiBlockArray multiBlockPos = blockValue.Block.multiBlockPos;
				int num7 = multiBlockPos.Length - 1;
				while (num7 >= 0 && *span[2] == 0L)
				{
					Vector2i vector2i = World.toChunkXZ(_pos + multiBlockPos.Get(num7, blockValue.type, (int)blockValue.rotation));
					long num8 = WorldChunkCache.MakeChunkKey(vector2i.x, vector2i.y);
					if (num8 != chunkSync.Key)
					{
						bool flag5 = false;
						for (int num9 = 0; num9 < 3; num9++)
						{
							long num10 = *span[num9];
							if (num10 == 0L)
							{
								*span[num9] = num8;
								break;
							}
							if (num10 == num8)
							{
								flag5 = true;
								break;
							}
						}
						if (!flag5)
						{
							Chunk chunkSync2 = this.GetChunkSync(num8);
							if (chunkSync2 != null && chunkSync2.StopStabilityCalculation)
							{
								flag4 = false;
								break;
							}
						}
					}
					num7--;
				}
			}
			if (GameManager.bPhysicsActive && flag4 && !blockValue.Equals(_bv))
			{
				bool isair = _bv.isair;
				if (!isair && !block.blockMaterial.IsLiquid)
				{
					bool stabilityFull = _bv.Block.StabilityFull;
					this.stabilityCalcMainThread.BlockPlacedAt(_pos, stabilityFull);
					if (block.isMultiBlock)
					{
						for (int num11 = block.multiBlockPos.Length - 1; num11 >= 0; num11--)
						{
							Vector3i vector3i = _pos + block.multiBlockPos.Get(num11, _bv.type, (int)_bv.rotation);
							if (vector3i.x != 0 || vector3i.y != 0 || vector3i.z != 0)
							{
								this.stabilityCalcMainThread.BlockPlacedAt(vector3i, stabilityFull);
							}
						}
					}
				}
				else if (isair && !blockValue.Block.blockMaterial.IsLiquid)
				{
					this.stabilityCalcMainThread.BlockRemovedAt(_pos);
					Block block2 = blockValue.Block;
					if (block2.isMultiBlock)
					{
						for (int num12 = block2.multiBlockPos.Length - 1; num12 >= 0; num12--)
						{
							Vector3i vector3i2 = block2.multiBlockPos.Get(num12, blockValue.type, (int)blockValue.rotation);
							if (vector3i2.x != 0 || vector3i2.y != 0 || vector3i2.z != 0)
							{
								Vector3i pos = _pos + vector3i2;
								this.stabilityCalcMainThread.BlockRemovedAt(pos);
							}
						}
					}
				}
				if (MeshDescription.bDebugStability)
				{
					for (int num13 = num2 - 1; num13 <= num2 + 1; num13++)
					{
						for (int num14 = num - 1; num14 <= num + 1; num14++)
						{
							Chunk chunkSync3 = base.GetChunkSync(num14, num13);
							if (chunkSync3 != null)
							{
								chunkSync3.NeedsRegeneration = true;
							}
						}
					}
				}
			}
		}
		if (flag)
		{
			this.chunkPosNeedsRegeneration(chunkSync, num5, blockY, num6, _isChangeDensity || blockValue.Block.shape.IsTerrain() || block.shape.IsTerrain());
		}
		if (_isChangeBV && _isUpdateLight)
		{
			this.m_LightProcessorMainThread.RefreshSunlightAtLocalPos(chunkSync, num5, num6, true);
			byte light = chunkSync.GetLight(num5, _pos.y, num6, Chunk.LIGHT_TYPE.BLOCK);
			byte b;
			if (!blockValue.isair && _bv.isair)
			{
				chunkSync.SetLight(num5, _pos.y, num6, 0, Chunk.LIGHT_TYPE.BLOCK);
				this.m_LightProcessorMainThread.RefreshLightAtLocalPos(chunkSync, num5, _pos.y, num6, Chunk.LIGHT_TYPE.BLOCK);
				b = chunkSync.GetLight(num5, _pos.y, num6, Chunk.LIGHT_TYPE.BLOCK);
			}
			else
			{
				b = block.GetLightValue(_bv);
				chunkSync.SetLight(num5, _pos.y, num6, b, Chunk.LIGHT_TYPE.BLOCK);
			}
			if (b > light)
			{
				this.m_LightProcessorMainThread.SpreadLight(chunkSync, num5, _pos.y, num6, b, Chunk.LIGHT_TYPE.BLOCK, true);
			}
			else if (b < light)
			{
				this.m_LightProcessorMainThread.UnspreadLight(chunkSync, num5, _pos.y, num6, light, Chunk.LIGHT_TYPE.BLOCK);
			}
		}
		if (chunkSync.GetTextureFull(num5, _pos.y, num6, 0) != 0L && !blockValue.isair && _bv.isair)
		{
			chunkSync.SetTextureFull(num5, _pos.y, num6, 0L, 0);
		}
		if (this.OnBlockChangedDelegates != null)
		{
			this.OnBlockChangedDelegates(_pos, blockValue, density, textureFullArray, _bv);
		}
		return blockValue;
	}

	// Token: 0x060055BA RID: 21946 RVA: 0x0020D2C0 File Offset: 0x0020B4C0
	public void SetBlockRaw(Vector3i _worldBlockPos, BlockValue _blockValue)
	{
		Chunk chunkSync = base.GetChunkSync(World.toChunkXZ(_worldBlockPos.x), World.toChunkXZ(_worldBlockPos.z));
		if (chunkSync == null)
		{
			return;
		}
		chunkSync.SetBlockRaw(World.toBlockXZ(_worldBlockPos.x), _worldBlockPos.y, World.toBlockXZ(_worldBlockPos.z), _blockValue);
	}

	// Token: 0x060055BB RID: 21947 RVA: 0x0020D314 File Offset: 0x0020B514
	public byte GetLight(Vector3i _blockPos, Chunk.LIGHT_TYPE type)
	{
		IChunk chunkFromWorldPos = base.GetChunkFromWorldPos(_blockPos);
		if (chunkFromWorldPos != null)
		{
			return chunkFromWorldPos.GetLight(World.toBlockXZ(_blockPos.x), World.toBlockY(_blockPos.y), World.toBlockXZ(_blockPos.z), type);
		}
		return 0;
	}

	// Token: 0x060055BC RID: 21948 RVA: 0x0020D356 File Offset: 0x0020B556
	public PropValue GetProp(int chunkX, int chunkZ, int propId)
	{
		Chunk chunkSync = base.GetChunkSync(chunkX, chunkZ);
		if (chunkSync == null)
		{
			return PropValue.AIR;
		}
		return chunkSync.GetProp(chunkX, chunkZ, propId);
	}

	// Token: 0x060055BD RID: 21949 RVA: 0x0020D372 File Offset: 0x0020B572
	public PropValue GetProp(long chunkKey, int propId)
	{
		Chunk chunkSync = this.GetChunkSync(chunkKey);
		if (chunkSync == null)
		{
			return PropValue.AIR;
		}
		return chunkSync.GetProp(chunkKey, propId);
	}

	// Token: 0x060055BE RID: 21950 RVA: 0x001B2E15 File Offset: 0x001B1015
	public PropValue GetProp(Vector2i chunkPos, int propId)
	{
		return IBlockAccess.DefaultGetProp(this, chunkPos, propId);
	}

	// Token: 0x060055BF RID: 21951 RVA: 0x001B2E1F File Offset: 0x001B101F
	public PropValue GetProp(PropRef propRef)
	{
		return IBlockAccess.DefaultGetProp(this, propRef);
	}

	// Token: 0x060055C0 RID: 21952 RVA: 0x0020D38C File Offset: 0x0020B58C
	public PropValue SetProp(PropRef propRef, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, BlockValue? blockValue = null)
	{
		return this.SetProp(propRef.ChunkPos, new int?(propRef.PropId), position, rotation, scale, blockValue);
	}

	// Token: 0x060055C1 RID: 21953 RVA: 0x0020D3AC File Offset: 0x0020B5AC
	public PropValue SetProp(Vector2i chunkPos, int? propId = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, BlockValue? blockValue = null)
	{
		Chunk chunk = (Chunk)base.GetChunkSync(chunkPos);
		PropValue left = (propId != null) ? chunk.GetProp(chunkPos, propId.Value) : PropValue.AIR;
		PropValue propValue = chunk.SetProp(propId, position, rotation, scale, blockValue);
		if (left != propValue)
		{
			Vector3i vector3i = Vector3i.Floor(propValue.position);
			this.chunkPosNeedsRegeneration(chunk, vector3i.x, vector3i.y, vector3i.z, false);
		}
		if (!propValue.IsAir)
		{
			return propValue;
		}
		return PropValue.AIR;
	}

	// Token: 0x060055C2 RID: 21954 RVA: 0x0020D434 File Offset: 0x0020B634
	public void SetBlockValue(BlockValueRef blockValueRef, BlockValue blockValue)
	{
		switch (blockValueRef.Type)
		{
		case BlockValueRefType.None:
			return;
		case BlockValueRefType.Block:
			this.SetBlockRaw(blockValueRef.BlockPosition, blockValue);
			return;
		case BlockValueRefType.Prop:
			this.SetProp(blockValueRef.PropReference, null, null, null, new BlockValue?(blockValue));
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060055C3 RID: 21955 RVA: 0x0020D4A0 File Offset: 0x0020B6A0
	public ChunkGameObject RemoveDisplayedChunkGameObject(long _key)
	{
		DictionarySave<long, ChunkGameObject> displayedChunkGameObjects = this.DisplayedChunkGameObjects;
		ChunkGameObject result;
		lock (displayedChunkGameObjects)
		{
			result = this.DisplayedChunkGameObjects[_key];
			this.DisplayedChunkGameObjects.Remove(_key);
		}
		return result;
	}

	// Token: 0x060055C4 RID: 21956 RVA: 0x0020D4F4 File Offset: 0x0020B6F4
	public void SetDisplayedChunkGameObject(long _key, ChunkGameObject _cgo)
	{
		DictionarySave<long, ChunkGameObject> displayedChunkGameObjects = this.DisplayedChunkGameObjects;
		lock (displayedChunkGameObjects)
		{
			this.DisplayedChunkGameObjects[_key] = _cgo;
		}
	}

	// Token: 0x060055C5 RID: 21957 RVA: 0x0020D53C File Offset: 0x0020B73C
	public void ChunkPosNeedsRegeneration_DelayedStart()
	{
		this.delayedRegenCount++;
		if (this.delayedRegenCount == 1)
		{
			Dictionary<Chunk, int> obj = this.delayedRegenChunks;
			lock (obj)
			{
				this.delayedRegenChunks.Clear();
			}
		}
	}

	// Token: 0x060055C6 RID: 21958 RVA: 0x0020D598 File Offset: 0x0020B798
	public void ChunkPosNeedsRegeneration_DelayedStop()
	{
		this.delayedRegenCount--;
		if (this.delayedRegenCount == 0)
		{
			Dictionary<Chunk, int> obj = this.delayedRegenChunks;
			lock (obj)
			{
				foreach (KeyValuePair<Chunk, int> keyValuePair in this.delayedRegenChunks)
				{
					keyValuePair.Key.NeedsRegenerationOrBits(keyValuePair.Value);
				}
				this.delayedRegenChunks.Clear();
			}
		}
	}

	// Token: 0x060055C7 RID: 21959 RVA: 0x0020D644 File Offset: 0x0020B844
	[PublicizedFrom(EAccessModifier.Private)]
	public void chunkRegenerateAt(Chunk _c, int _yPos)
	{
		if (this.delayedRegenCount == 0)
		{
			_c.NeedsRegenerationAt = _yPos;
			return;
		}
		Dictionary<Chunk, int> obj = this.delayedRegenChunks;
		lock (obj)
		{
			int needsRegenerationAt;
			if (!this.delayedRegenChunks.TryGetValue(_c, out needsRegenerationAt))
			{
				needsRegenerationAt = _c.NeedsRegenerationAt;
				this.delayedRegenChunks.Add(_c, 0);
			}
			this.delayedRegenChunks[_c] = (needsRegenerationAt | 1 << (_yPos >> 4));
		}
	}

	// Token: 0x060055C8 RID: 21960 RVA: 0x0020D6C8 File Offset: 0x0020B8C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void chunkPosNeedsRegeneration(Chunk _chunk, int _blockX, int _blockY, int _blockZ, bool _bTerrainBlockChanged)
	{
		this.chunksNeedingRegThisCall.Clear();
		this.chunkRegenerateAt(_chunk, _blockY);
		this.chunksNeedingRegThisCall.Add(_chunk);
		if (_blockY > 0 && _blockY % 16 == 0)
		{
			this.chunkRegenerateAt(_chunk, _blockY - 1);
		}
		else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
		{
			this.chunkRegenerateAt(_chunk, _blockY + 1);
		}
		if (_blockX == 15)
		{
			Chunk chunkSync = base.GetChunkSync(_chunk.X + 1, _chunk.Z);
			if (chunkSync != null)
			{
				this.chunkRegenerateAt(chunkSync, _blockY);
				this.chunksNeedingRegThisCall.Add(chunkSync);
				if (_blockY > 0 && _blockY % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync, _blockY - 1);
				}
				else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync, _blockY + 1);
				}
			}
		}
		else if (_blockX == 0)
		{
			Chunk chunkSync2 = base.GetChunkSync(_chunk.X - 1, _chunk.Z);
			if (chunkSync2 != null)
			{
				this.chunkRegenerateAt(chunkSync2, _blockY);
				this.chunksNeedingRegThisCall.Add(chunkSync2);
				if (_blockY > 0 && _blockY % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync2, _blockY - 1);
				}
				else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync2, _blockY + 1);
				}
			}
		}
		if (_blockZ == 0)
		{
			Chunk chunkSync3 = base.GetChunkSync(_chunk.X, _chunk.Z - 1);
			if (chunkSync3 != null)
			{
				this.chunkRegenerateAt(chunkSync3, _blockY);
				this.chunksNeedingRegThisCall.Add(chunkSync3);
				if (_blockY > 0 && _blockY % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync3, _blockY - 1);
				}
				else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync3, _blockY + 1);
				}
			}
		}
		else if (_blockZ == 15)
		{
			Chunk chunkSync4 = base.GetChunkSync(_chunk.X, _chunk.Z + 1);
			if (chunkSync4 != null)
			{
				this.chunkRegenerateAt(chunkSync4, _blockY);
				this.chunksNeedingRegThisCall.Add(chunkSync4);
				if (_blockY > 0 && _blockY % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync4, _blockY - 1);
				}
				else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
				{
					this.chunkRegenerateAt(chunkSync4, _blockY + 1);
				}
			}
		}
		if (_bTerrainBlockChanged)
		{
			if (_blockX == 0 && _blockZ == 0)
			{
				Chunk chunkSync5 = base.GetChunkSync(_chunk.X - 1, _chunk.Z - 1);
				if (chunkSync5 != null)
				{
					this.chunkRegenerateAt(chunkSync5, _blockY);
					this.chunksNeedingRegThisCall.Add(chunkSync5);
					if (_blockY > 0 && _blockY % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync5, _blockY - 1);
					}
					else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync5, _blockY + 1);
					}
				}
			}
			if (_blockX == 0 && _blockZ == 15)
			{
				Chunk chunkSync6 = base.GetChunkSync(_chunk.X - 1, _chunk.Z + 1);
				if (chunkSync6 != null)
				{
					this.chunkRegenerateAt(chunkSync6, _blockY);
					this.chunksNeedingRegThisCall.Add(chunkSync6);
					if (_blockY > 0 && _blockY % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync6, _blockY - 1);
					}
					else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync6, _blockY + 1);
					}
				}
			}
			if (_blockX == 15 && _blockZ == 0)
			{
				Chunk chunkSync7 = base.GetChunkSync(_chunk.X + 1, _chunk.Z - 1);
				if (chunkSync7 != null)
				{
					this.chunkRegenerateAt(chunkSync7, _blockY);
					this.chunksNeedingRegThisCall.Add(chunkSync7);
					if (_blockY > 0 && _blockY % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync7, _blockY - 1);
					}
					else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync7, _blockY + 1);
					}
				}
			}
			if (_blockX == 15 && _blockZ == 15)
			{
				Chunk chunkSync8 = base.GetChunkSync(_chunk.X + 1, _chunk.Z + 1);
				if (chunkSync8 != null)
				{
					this.chunkRegenerateAt(chunkSync8, _blockY);
					this.chunksNeedingRegThisCall.Add(chunkSync8);
					if (_blockY > 0 && _blockY % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync8, _blockY - 1);
					}
					else if (_blockY < 255 && (_blockY + 1) % 16 == 0)
					{
						this.chunkRegenerateAt(chunkSync8, _blockY + 1);
					}
				}
			}
		}
		EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
		if (primaryPlayer != null && this.chunksNeedingRegThisCall.Count > 0)
		{
			Vector3i vector3i = World.worldToBlockPos(this.ToLocalPosition(primaryPlayer.GetPosition()));
			int num = World.toChunkXZ(vector3i.x);
			int num2 = World.toChunkXZ(vector3i.z);
			ChunkManager chunkManager = this.world.m_ChunkManager;
			chunkManager.ResetChunksToCopyInOneFrame();
			for (int i = 0; i < this.chunksNeedingRegThisCall.Count; i++)
			{
				Chunk chunk = this.chunksNeedingRegThisCall[i];
				int num3 = num - chunk.X;
				int num4 = num2 - chunk.Z;
				if (num3 < 0)
				{
					num3 = -num3;
				}
				if (num4 < 0)
				{
					num4 = -num4;
				}
				if (num3 <= 1 && num4 <= 1)
				{
					chunkManager.ChunksToCopyInOneFrame.Add(chunk);
				}
			}
		}
		this.chunksNeedingRegThisCall.Clear();
	}

	// Token: 0x060055C9 RID: 21961 RVA: 0x0020DB54 File Offset: 0x0020BD54
	[PublicizedFrom(EAccessModifier.Private)]
	public void notifyBlocksOfNeighborChange(Vector3i _worldBlockPos, BlockValue _newBlockValue, BlockValue _oldBlockValue)
	{
		for (int i = 0; i < Vector3i.AllDirections.Length; i++)
		{
			this.notifyBlockOfNeighborChange(_worldBlockPos + Vector3i.AllDirections[i], _newBlockValue, _oldBlockValue, _worldBlockPos);
		}
	}

	// Token: 0x060055CA RID: 21962 RVA: 0x0020DB90 File Offset: 0x0020BD90
	[PublicizedFrom(EAccessModifier.Private)]
	public void notifyBlockOfNeighborChange(Vector3i _myBlockPos, BlockValue _newBlockValue, BlockValue _oldBlockValue, Vector3i _blockPosThatChanged)
	{
		if (this.world.IsRemote())
		{
			return;
		}
		BlockValue block = this.world.GetBlock(_myBlockPos);
		if (!block.isair)
		{
			block.Block.OnNeighborBlockChange(this.world, _myBlockPos, block, _blockPosThatChanged, _newBlockValue, _oldBlockValue);
		}
	}

	// Token: 0x060055CB RID: 21963 RVA: 0x0020DBD9 File Offset: 0x0020BDD9
	public Vector3 ToWorldPosition(Vector3 _localPos)
	{
		return _localPos + this.Position;
	}

	// Token: 0x060055CC RID: 21964 RVA: 0x0020DBE8 File Offset: 0x0020BDE8
	public Vector3 ToLocalPosition(Vector3 _worldPos)
	{
		_worldPos.x -= this.Position.x;
		_worldPos.y -= this.Position.y;
		_worldPos.z -= this.Position.z;
		return _worldPos;
	}

	// Token: 0x060055CD RID: 21965 RVA: 0x000149AE File Offset: 0x00012BAE
	public Vector3 ToLocalVector(Vector3 _vector)
	{
		return _vector;
	}

	// Token: 0x060055CE RID: 21966 RVA: 0x0020DC38 File Offset: 0x0020BE38
	public long ToLocalKey(long _key)
	{
		int num = World.toChunkXZ(Mathf.FloorToInt(this.Position.x));
		int num2 = World.toChunkXZ(Mathf.FloorToInt(this.Position.z));
		int x = WorldChunkCache.extractX(_key) - num;
		int y = WorldChunkCache.extractZ(_key) - num2;
		return WorldChunkCache.MakeChunkKey(x, y);
	}

	// Token: 0x060055CF RID: 21967 RVA: 0x0020DC88 File Offset: 0x0020BE88
	public List<BlockEntityData> GetBlockEntities(string _indexedBlockKey)
	{
		List<BlockEntityData> list = new List<BlockEntityData>();
		List<Chunk> chunkArrayCopySync = base.GetChunkArrayCopySync();
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			List<Vector3i> list2 = chunkArrayCopySync[i].IndexedBlocks[_indexedBlockKey];
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					Vector3i pos = list2[j];
					Vector3i worldPos = chunkArrayCopySync[i].ToWorldPos(pos);
					BlockEntityData blockEntity = chunkArrayCopySync[i].GetBlockEntity(worldPos);
					if (blockEntity != null)
					{
						list.Add(blockEntity);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060055D0 RID: 21968 RVA: 0x0020DD18 File Offset: 0x0020BF18
	public BlockEntityData GetBlockEntity(Vector3i _blockLocalPos)
	{
		IChunk chunkFromWorldPos = base.GetChunkFromWorldPos(_blockLocalPos);
		if (chunkFromWorldPos == null)
		{
			return null;
		}
		return chunkFromWorldPos.GetBlockEntity(_blockLocalPos);
	}

	// Token: 0x060055D1 RID: 21969 RVA: 0x0020DD3C File Offset: 0x0020BF3C
	public void DebugOnGUI(float middleX, float middleY, float size)
	{
		List<Chunk> chunkArrayCopySync = base.GetChunkArrayCopySync();
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			vector.x = Utils.FastMin((float)chunk.X, vector.x);
			vector.y = Utils.FastMin((float)chunk.Z, vector.y);
			vector2.x = Utils.FastMax((float)chunk.X, vector2.x);
			vector2.y = Utils.FastMax((float)chunk.Z, vector2.y);
		}
		vector *= size;
		vector2 *= size;
		float num = middleX + vector.x;
		if (num < 0f)
		{
			middleX -= num;
		}
		float num2 = middleY + vector.y;
		if (num2 < 0f)
		{
			middleY += num2;
		}
		num = middleX - vector2.x;
		if (num < 0f)
		{
			middleX += num;
		}
		num2 = middleY - vector2.y;
		if (num2 < 0f)
		{
			middleY -= num2;
		}
		for (int j = 0; j < chunkArrayCopySync.Count; j++)
		{
			Chunk chunk2 = chunkArrayCopySync[j];
			bool flag = this.DisplayedChunkGameObjects.ContainsKey(chunk2.Key);
			Color colFill = new Color(0.1f, 0.5f, 0.1f);
			Color black = Color.black;
			if (flag)
			{
				black = new Color(0.6f, 0.6f, 0.6f);
			}
			if (chunk2.NeedsDecoration)
			{
				colFill = Color.red;
			}
			else if (chunk2.NeedsLightCalculation)
			{
				colFill = new Color(0.7f, 0.7f, 0f);
			}
			else if (chunk2.NeedsRegeneration)
			{
				colFill = new Color(0.1f, 0.1f, 0.7f);
			}
			else if (chunk2.NeedsCopying)
			{
				colFill = new Color(0.7f, 0.1f, 0.7f);
			}
			else if (chunk2.NeedsOnlyCollisionMesh)
			{
				colFill = Color.gray;
			}
			colFill.a = 0.7f;
			GUIUtils.DrawFilledRect(new Rect(middleX + (float)chunk2.X * size - size * 0.5f, middleY - (float)chunk2.Z * size - size * 0.5f, size, size), colFill, true, black);
		}
	}

	// Token: 0x060055D2 RID: 21970 RVA: 0x0020DFA6 File Offset: 0x0020C1A6
	public void SnapTerrainToPositionAroundLocal(Vector3i _worldPos)
	{
		this.SnapTerrainToPositionAroundRPC(null, _worldPos);
	}

	// Token: 0x060055D3 RID: 21971 RVA: 0x0020DFB0 File Offset: 0x0020C1B0
	public void SnapTerrainToPositionAroundRPC(WorldBase _world, Vector3i _worldPos)
	{
		if (!this.GetBlock(_worldPos).Block.shape.IsTerrain())
		{
			return;
		}
		this.snapTerrainToPosition(_world, _worldPos, false, false);
		this.snapTerrainToPosition(_world, _worldPos + Vector3i.right, false, true);
		this.snapTerrainToPosition(_world, _worldPos - Vector3i.right, false, true);
		this.snapTerrainToPosition(_world, _worldPos + Vector3i.forward, false, true);
		this.snapTerrainToPosition(_world, _worldPos - Vector3i.forward, false, true);
	}

	// Token: 0x060055D4 RID: 21972 RVA: 0x0020E033 File Offset: 0x0020C233
	public void SnapTerrainToPositionAtLocal(Vector3i _worldPos, bool _bLiftUpTerrainByOneIfNeeded, bool _bUseHalfTerrainDensity)
	{
		this.snapTerrainToPosition(null, _worldPos, _bLiftUpTerrainByOneIfNeeded, _bUseHalfTerrainDensity);
	}

	// Token: 0x060055D5 RID: 21973 RVA: 0x0020E040 File Offset: 0x0020C240
	[PublicizedFrom(EAccessModifier.Private)]
	public void snapTerrainToPosition(WorldBase _world, Vector3i _worldPos, bool _bLiftUpTerrainByOneIfNeeded, bool _bUseHalfTerrainDensity)
	{
		if (_worldPos.y < 1)
		{
			return;
		}
		BlockValue block = this.GetBlock(_worldPos);
		if (!block.Block.shape.IsTerrain())
		{
			if (!_bLiftUpTerrainByOneIfNeeded)
			{
				return;
			}
			if (!block.isair)
			{
				return;
			}
			BlockValue block2 = this.GetBlock(_worldPos - Vector3i.up);
			if (block2.Block.shape.IsTerrain())
			{
				sbyte density = this.GetDensity(_worldPos);
				sbyte b = _bUseHalfTerrainDensity ? (MarchingCubes.DensityTerrain / 2) : MarchingCubes.DensityTerrain;
				if (_world == null)
				{
					this.SetBlockRaw(_worldPos, block2);
					if (density > b)
					{
						this.SetDensityRaw(_worldPos, b);
						return;
					}
				}
				else
				{
					if (density > b)
					{
						_world.SetBlockRPC(_worldPos, block2, b);
						return;
					}
					_world.SetBlockRPC(_worldPos, block2);
					return;
				}
			}
		}
		else
		{
			if (this.GetBlock(_worldPos + Vector3i.up).Block.IsTerrainDecoration)
			{
				return;
			}
			sbyte density2 = this.GetDensity(_worldPos);
			sbyte b2 = _bUseHalfTerrainDensity ? (MarchingCubes.DensityTerrain / 2) : MarchingCubes.DensityTerrain;
			if (density2 > b2)
			{
				if (_world == null)
				{
					this.SetDensityRaw(_worldPos, b2);
					return;
				}
				_world.SetBlockRPC(_worldPos, b2);
			}
		}
	}

	// Token: 0x060055D6 RID: 21974 RVA: 0x0020E160 File Offset: 0x0020C360
	public void InvokeOnBlockDamagedDelegates(BlockValueRef _bvRef, BlockValue _blockValue, int _damage, int _attackerEntityId)
	{
		if (this.OnBlockDamagedDelegates != null)
		{
			this.OnBlockDamagedDelegates(_bvRef, _blockValue, _damage, _attackerEntityId);
		}
	}

	// Token: 0x060055D7 RID: 21975 RVA: 0x0002003D File Offset: 0x0001E23D
	public bool Overlaps(Bounds _boundsInWorldCoord)
	{
		return true;
	}

	// Token: 0x060055D8 RID: 21976 RVA: 0x0020E17C File Offset: 0x0020C37C
	public void OnChunkDisplayed(long _key, bool _isDisplayed)
	{
		if (this.OnChunkVisibleDelegates != null)
		{
			this.OnChunkVisibleDelegates(_key, _isDisplayed);
		}
		if (!this.bFinishedDisplayingDelegateCalled && this.chunkKeysNeedDisplaying != null)
		{
			this.chunkKeysNeedDisplaying.Remove(_key);
			if (this.chunkKeysNeedDisplaying.Count == 0)
			{
				this.bFinishedDisplayingDelegateCalled = true;
				if (this.OnChunksFinishedDisplayingDelegates != null)
				{
					this.OnChunksFinishedDisplayingDelegates();
				}
			}
		}
	}

	// Token: 0x060055D9 RID: 21977 RVA: 0x0020E1E4 File Offset: 0x0020C3E4
	public void CheckCollisionWithBlocks(Entity _entity)
	{
		Vector3 vector = this.ToLocalPosition(_entity.boundingBox.min + new Vector3(0.001f, 0.001f, 0.001f));
		Vector3 vector2 = this.ToLocalPosition(_entity.boundingBox.max - new Vector3(0.001f, 0.001f, 0.001f));
		int num = Utils.Fastfloor(vector.x - 0.5f);
		int num2 = Utils.Fastfloor(vector.y - 0.5f);
		int num3 = Utils.Fastfloor(vector.z - 0.5f);
		int num4 = Utils.Fastfloor(vector2.x + 0.5f);
		int num5 = Utils.Fastfloor(vector2.y + 0.5f);
		int num6 = Utils.Fastfloor(vector2.z + 0.5f);
		Bounds aabb = default(Bounds);
		aabb.SetMinMax(vector, vector2);
		aabb.Expand(new Vector3(0.05f, 0.05f, 0.05f));
		CharacterControllerAbstract characterController = _entity.m_characterController;
		float num7;
		if (characterController != null)
		{
			num7 = characterController.GetSkinWidth();
		}
		else
		{
			num7 = 0.08f;
		}
		aabb.min = new Vector3(aabb.min.x, aabb.min.y - num7, aabb.min.z);
		if (num2 <= 0)
		{
			num2 = 1;
		}
		if (num5 >= 256)
		{
			num5 = 255;
		}
		IChunk chunk = null;
		for (int i = num; i <= num4; i++)
		{
			int j = num3;
			while (j <= num6)
			{
				int num8 = World.toChunkXZ(i);
				int num9 = World.toChunkXZ(j);
				if (chunk != null && chunk.X == num8 && chunk.Z == num9)
				{
					goto IL_1A8;
				}
				chunk = base.GetChunkSync(num8, num9);
				if (chunk != null)
				{
					goto IL_1A8;
				}
				IL_27C:
				j++;
				continue;
				IL_1A8:
				int x = World.toBlockXZ(i);
				int z = World.toBlockXZ(j);
				for (int k = num2; k <= num5; k++)
				{
					BlockValue block = chunk.GetBlock(x, k, z);
					if (!block.isair)
					{
						Block block2 = block.Block;
						if (block2.IsCheckCollideWithEntity)
						{
							Vector3i vector3i = new Vector3i(i, k, j);
							if (block2.isMultiBlock && block.ischild)
							{
								Vector3i parentPos = block2.multiBlockPos.GetParentPos(vector3i, block);
								block = this.world.GetBlock(parentPos);
								vector3i = parentPos;
							}
							if (block2.HasCollidingAABB(block, vector3i.x, vector3i.y, vector3i.z, 0f, aabb))
							{
								block2.OnEntityCollidedWithBlock(this.world, vector3i, block, _entity);
							}
						}
					}
				}
				goto IL_27C;
			}
		}
	}

	// Token: 0x060055DA RID: 21978 RVA: 0x0020E48B File Offset: 0x0020C68B
	public void Save()
	{
		if (this.ChunkProvider != null)
		{
			this.ChunkProvider.SaveAll();
		}
	}

	// Token: 0x060055DB RID: 21979 RVA: 0x0020E4A0 File Offset: 0x0020C6A0
	public int GetBlockFaceTexture(Vector3i _blockPos, BlockFace _blockFace, int _channel)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return 0;
		}
		return chunk.GetBlockFaceTexture(World.toBlockXZ(_blockPos.x), World.toBlockY(_blockPos.y), World.toBlockXZ(_blockPos.z), _blockFace, _channel);
	}

	// Token: 0x060055DC RID: 21980 RVA: 0x0020E4E8 File Offset: 0x0020C6E8
	public void SetBlockFaceTexture(Vector3i _blockPos, BlockFace _blockFace, int _textureIdx, int _channel)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return;
		}
		int num = World.toBlockXZ(_blockPos.x);
		int num2 = World.toBlockY(_blockPos.y);
		int num3 = World.toBlockXZ(_blockPos.z);
		TextureFullArray textureFullArray = chunk.GetTextureFullArray(num, num2, num3, false);
		chunk.SetBlockFaceTexture(num, num2, num3, _blockFace, _textureIdx, _channel);
		this.chunkPosNeedsRegeneration(chunk, num, num2, num3, false);
		if (this.OnBlockChangedDelegates != null)
		{
			BlockValue block = this.GetBlock(_blockPos);
			this.OnBlockChangedDelegates(_blockPos, block, this.GetDensity(_blockPos), textureFullArray, block);
		}
	}

	// Token: 0x060055DD RID: 21981 RVA: 0x0020E57C File Offset: 0x0020C77C
	public void SetTextureFull(Vector3i _blockPos, long _textureFull, int channel)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return;
		}
		int num = World.toBlockXZ(_blockPos.x);
		int num2 = World.toBlockY(_blockPos.y);
		int num3 = World.toBlockXZ(_blockPos.z);
		TextureFullArray textureFullArray = chunk.GetTextureFullArray(num, num2, num3, false);
		chunk.SetTextureFull(num, num2, num3, _textureFull, channel);
		this.chunkPosNeedsRegeneration(chunk, num, num2, num3, false);
		if (this.OnBlockChangedDelegates != null)
		{
			BlockValue block = this.GetBlock(_blockPos);
			this.OnBlockChangedDelegates(_blockPos, block, this.GetDensity(_blockPos), textureFullArray, block);
		}
	}

	// Token: 0x060055DE RID: 21982 RVA: 0x0020E60C File Offset: 0x0020C80C
	public void SetTextureFull(BlockValueRef _bvRef, long _textureFull, int channel)
	{
		switch (_bvRef.Type)
		{
		case BlockValueRefType.None:
			return;
		case BlockValueRefType.Block:
			this.SetTextureFull(_bvRef.BlockPosition, _textureFull, channel);
			return;
		case BlockValueRefType.Prop:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060055DF RID: 21983 RVA: 0x0020E64C File Offset: 0x0020C84C
	public void SetTextureFullArray(Vector3i _blockPos, TextureFullArray _textureFull)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return;
		}
		int num = World.toBlockXZ(_blockPos.x);
		int num2 = World.toBlockY(_blockPos.y);
		int num3 = World.toBlockXZ(_blockPos.z);
		TextureFullArray setTextureFullArray = chunk.GetSetTextureFullArray(num, num2, num3, _textureFull);
		this.chunkPosNeedsRegeneration(chunk, num, num2, num3, false);
		if (this.OnBlockChangedDelegates != null)
		{
			BlockValue block = this.GetBlock(_blockPos);
			this.OnBlockChangedDelegates(_blockPos, block, this.GetDensity(_blockPos), setTextureFullArray, block);
		}
	}

	// Token: 0x060055E0 RID: 21984 RVA: 0x0020E6D0 File Offset: 0x0020C8D0
	public void SetTextureFullArray(BlockValueRef _bvRef, TextureFullArray _textureFull)
	{
		switch (_bvRef.Type)
		{
		case BlockValueRefType.None:
			return;
		case BlockValueRefType.Block:
			this.SetTextureFullArray(_bvRef.BlockPosition, _textureFull);
			return;
		case BlockValueRefType.Prop:
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060055E1 RID: 21985 RVA: 0x0020E710 File Offset: 0x0020C910
	public long GetTextureFull(Vector3i _blockPos)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return 0L;
		}
		return chunk.GetTextureFull(World.toBlockXZ(_blockPos.x), World.toBlockY(_blockPos.y), World.toBlockXZ(_blockPos.z), 0);
	}

	// Token: 0x060055E2 RID: 21986 RVA: 0x0020E758 File Offset: 0x0020C958
	public long GetTextureFull(BlockValueRef _bvRef)
	{
		switch (_bvRef.Type)
		{
		case BlockValueRefType.None:
			return 0L;
		case BlockValueRefType.Block:
			return this.GetTextureFull(_bvRef.BlockPosition);
		case BlockValueRefType.Prop:
			return 0L;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060055E3 RID: 21987 RVA: 0x0020E798 File Offset: 0x0020C998
	public TextureFullArray GetTextureFullArray(Vector3i _blockPos)
	{
		Chunk chunk = (Chunk)base.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return TextureFullArray.Default;
		}
		return chunk.GetTextureFullArray(World.toBlockXZ(_blockPos.x), World.toBlockY(_blockPos.y), World.toBlockXZ(_blockPos.z), true);
	}

	// Token: 0x060055E4 RID: 21988 RVA: 0x0020E7E4 File Offset: 0x0020C9E4
	public TextureFullArray GetTextureFullArray(BlockValueRef _bvRef)
	{
		switch (_bvRef.Type)
		{
		case BlockValueRefType.None:
			return TextureFullArray.Default;
		case BlockValueRefType.Block:
			return this.GetTextureFullArray(_bvRef.BlockPosition);
		case BlockValueRefType.Prop:
			return TextureFullArray.Default;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060055E5 RID: 21989 RVA: 0x0020E82A File Offset: 0x0020CA2A
	public void MGTest()
	{
		this.meshGenerator.Test();
	}

	// Token: 0x060055E6 RID: 21990 RVA: 0x0020E838 File Offset: 0x0020CA38
	public static ValueTuple<int, double> SecondsSinceChunkStartedRegeneration(long chunkKey)
	{
		ValueTuple<int, DateTime> valueTuple;
		if (ChunkCluster.chunkRegenerationStartTimestamps.TryGetValue(chunkKey, out valueTuple))
		{
			return new ValueTuple<int, double>(valueTuple.Item1, (DateTime.UtcNow - valueTuple.Item2).TotalSeconds);
		}
		return new ValueTuple<int, double>(-1, -1.0);
	}

	// Token: 0x060055E7 RID: 21991 RVA: 0x0020E888 File Offset: 0x0020CA88
	public static ValueTuple<int, double> SecondsSinceChunkEndedRegeneration(long chunkKey)
	{
		ValueTuple<int, DateTime> valueTuple;
		if (ChunkCluster.chunkRegenerationEndTimestamps.TryGetValue(chunkKey, out valueTuple))
		{
			return new ValueTuple<int, double>(valueTuple.Item1, (DateTime.UtcNow - valueTuple.Item2).TotalSeconds);
		}
		return new ValueTuple<int, double>(-1, -1.0);
	}

	// Token: 0x060055E8 RID: 21992 RVA: 0x0020E8D8 File Offset: 0x0020CAD8
	public static void LogCurrentChunkRegenerationState(long importantChunkKey)
	{
		StateHistory<ChunkCluster.ChunkState> stateHistory;
		Log.Out(string.Format("[FELLTHROUGHWORLD] ChunkCluster Current Mesh Regeneration State - Chunk: {0}, VML Index: {1}, Regeneration State: {2}", ChunkCluster.currentChunkKey, ChunkCluster.currentChunkVMLIndex, (ChunkCluster.chunkRegenerationHistories.TryGetValue(ChunkCluster.currentChunkKey, out stateHistory) && stateHistory.Length > 0) ? stateHistory.Current : "N/A"));
		List<ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>>> list = new List<ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>>>(ChunkCluster.chunkRegenerationHistories.Count);
		foreach (KeyValuePair<long, StateHistory<ChunkCluster.ChunkState>> keyValuePair in ChunkCluster.chunkRegenerationHistories)
		{
			long num;
			StateHistory<ChunkCluster.ChunkState> stateHistory2;
			keyValuePair.Deconstruct(out num, out stateHistory2);
			long num2 = num;
			StateHistory<ChunkCluster.ChunkState> item = stateHistory2;
			list.Add(new ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>>(ChunkManager.SecondsSinceChunkSelectedForGeneration(num2), num2, item));
		}
		list.Sort(([TupleElementNames(new string[]
		{
			"seconds",
			"chunkKey",
			"history"
		})] ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>> a, [TupleElementNames(new string[]
		{
			"seconds",
			"chunkKey",
			"history"
		})] ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>> b) => a.Item1.CompareTo(b.Item1));
		int num3 = 0;
		foreach (ValueTuple<double, long, StateHistory<ChunkCluster.ChunkState>> valueTuple in list)
		{
			double item2 = valueTuple.Item1;
			long item3 = valueTuple.Item2;
			StateHistory<ChunkCluster.ChunkState> item4 = valueTuple.Item3;
			if (num3 < 16 || item3 == importantChunkKey || item3 == ChunkCluster.currentChunkKey)
			{
				StateHistory<ChunkCluster.ChunkState> stateHistory2 = item4;
				lock (stateHistory2)
				{
					Log.Out(string.Format("[FELLTHROUGHWORLD] Chunk {0} ({1}, {2}), Last Regeneration Requested: {3} State History: {4}", new object[]
					{
						item3,
						WorldChunkCache.extractX(item3),
						WorldChunkCache.extractZ(item3),
						(item2 < 0.0) ? "Never" : string.Format("{0:F3} s", item2),
						item4
					}));
				}
				num3++;
			}
		}
	}

	// Token: 0x060055E9 RID: 21993 RVA: 0x0020EAD0 File Offset: 0x0020CCD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRegenerationState(long chunkKey, ChunkCluster.ChunkState state)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		StateHistory<ChunkCluster.ChunkState> orAdd = ChunkCluster.chunkRegenerationHistories.GetOrAdd(chunkKey, (long _) => new StateHistory<ChunkCluster.ChunkState>(16, true, false));
		StateHistory<ChunkCluster.ChunkState> obj = orAdd;
		lock (obj)
		{
			orAdd.Add(state);
		}
	}

	// Token: 0x060055EA RID: 21994 RVA: 0x0020EB40 File Offset: 0x0020CD40
	public void ClearStabilityForChunks(HashSetLong chunks)
	{
		this.stabilityCalcMainThread.ClearChunkStabilityQueues(chunks);
		this.world.ClearFallingBlocksForChunks(chunks);
	}

	// Token: 0x04004254 RID: 16980
	public static readonly IReadOnlyDictionary<string, int> LayerMappingTable = new Dictionary<string, int>
	{
		{
			"terraincollision",
			16
		},
		{
			"nocollision",
			14
		},
		{
			"grass",
			18
		},
		{
			"Glass",
			30
		},
		{
			"water",
			4
		},
		{
			"terrain",
			28
		}
	};

	// Token: 0x0400425A RID: 16986
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFinishedLoadingDelegateCalled;

	// Token: 0x0400425B RID: 16987
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSetLong chunkKeysNeedLoading;

	// Token: 0x0400425C RID: 16988
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFinishedDisplayingDelegateCalled;

	// Token: 0x0400425D RID: 16989
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSetLong chunkKeysNeedDisplaying;

	// Token: 0x0400425E RID: 16990
	public string Name;

	// Token: 0x0400425F RID: 16991
	public bool IsFixedSize;

	// Token: 0x04004260 RID: 16992
	public int LayerMappingId;

	// Token: 0x04004261 RID: 16993
	public DictionarySave<long, ChunkGameObject> DisplayedChunkGameObjects = new DictionarySave<long, ChunkGameObject>();

	// Token: 0x04004262 RID: 16994
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentDictionary<long, ValueTuple<int, DateTime>> chunkRegenerationStartTimestamps = new ConcurrentDictionary<long, ValueTuple<int, DateTime>>();

	// Token: 0x04004263 RID: 16995
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentDictionary<long, ValueTuple<int, DateTime>> chunkRegenerationEndTimestamps = new ConcurrentDictionary<long, ValueTuple<int, DateTime>>();

	// Token: 0x04004264 RID: 16996
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ConcurrentDictionary<long, StateHistory<ChunkCluster.ChunkState>> chunkRegenerationHistories = new ConcurrentDictionary<long, StateHistory<ChunkCluster.ChunkState>>();

	// Token: 0x04004265 RID: 16997
	[PublicizedFrom(EAccessModifier.Private)]
	public static long currentChunkKey;

	// Token: 0x04004266 RID: 16998
	[PublicizedFrom(EAccessModifier.Private)]
	public static int currentChunkVMLIndex;

	// Token: 0x04004267 RID: 16999
	public Vector3 Position;

	// Token: 0x04004268 RID: 17000
	[PublicizedFrom(EAccessModifier.Private)]
	public ILightProcessor m_LightProcessorMainThread;

	// Token: 0x04004269 RID: 17001
	[PublicizedFrom(EAccessModifier.Private)]
	public ILightProcessor m_LightProcessorLightingThread;

	// Token: 0x0400426A RID: 17002
	[PublicizedFrom(EAccessModifier.Private)]
	public StabilityCalculator stabilityCalcMainThread;

	// Token: 0x0400426B RID: 17003
	[PublicizedFrom(EAccessModifier.Private)]
	public StabilityInitializer stabilityCalcLightingThread;

	// Token: 0x0400426C RID: 17004
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCacheNeighborChunks nChunks;

	// Token: 0x0400426D RID: 17005
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCacheNeighborBlocks nBlocks;

	// Token: 0x0400426E RID: 17006
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshGenerator meshGenerator;

	// Token: 0x0400426F RID: 17007
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04004270 RID: 17008
	public IChunkProvider ChunkProvider;

	// Token: 0x04004271 RID: 17009
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SBlockPosValue> multiBlockList = new List<SBlockPosValue>();

	// Token: 0x04004272 RID: 17010
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInNotify;

	// Token: 0x04004273 RID: 17011
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Chunk> chunksNeedingRegThisCall = new List<Chunk>();

	// Token: 0x04004274 RID: 17012
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Chunk, int> delayedRegenChunks = new Dictionary<Chunk, int>();

	// Token: 0x04004275 RID: 17013
	[PublicizedFrom(EAccessModifier.Private)]
	public int delayedRegenCount;

	// Token: 0x04004276 RID: 17014
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Bounds> aabbList = new List<Bounds>();

	// Token: 0x02000B16 RID: 2838
	// (Invoke) Token: 0x060055ED RID: 21997
	public delegate void OnBlockDamagedDelegate(BlockValueRef _bvRef, BlockValue _blockValue, int _damage, int _attackerEntityId);

	// Token: 0x02000B17 RID: 2839
	// (Invoke) Token: 0x060055F1 RID: 22001
	public delegate void OnChunksFinishedLoadingDelegate();

	// Token: 0x02000B18 RID: 2840
	// (Invoke) Token: 0x060055F5 RID: 22005
	public delegate void OnChunksFinishedDisplayingDelegate();

	// Token: 0x02000B19 RID: 2841
	// (Invoke) Token: 0x060055F9 RID: 22009
	public delegate void OnChunkVisibleDelegate(long _key, bool _isDisplayed);

	// Token: 0x02000B1A RID: 2842
	// (Invoke) Token: 0x060055FD RID: 22013
	public delegate void OnBlockChangedDelegate(Vector3i pos, BlockValue bvOld, sbyte densOld, TextureFullArray texOld, BlockValue bvNew);

	// Token: 0x02000B1B RID: 2843
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ChunkState : byte
	{
		// Token: 0x04004278 RID: 17016
		Unloaded,
		// Token: 0x04004279 RID: 17017
		Removed,
		// Token: 0x0400427A RID: 17018
		Loaded,
		// Token: 0x0400427B RID: 17019
		Starting,
		// Token: 0x0400427C RID: 17020
		GeneratingMeshes,
		// Token: 0x0400427D RID: 17021
		AddingMeshLayer,
		// Token: 0x0400427E RID: 17022
		Regenerated
	}
}
