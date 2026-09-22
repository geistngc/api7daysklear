using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000C14 RID: 3092
public class DecoManager
{
	// Token: 0x170009A8 RID: 2472
	// (get) Token: 0x06005E5E RID: 24158 RVA: 0x0024C83E File Offset: 0x0024AA3E
	public static DecoManager Instance
	{
		get
		{
			if (DecoManager.m_Instance == null)
			{
				DecoManager.m_Instance = new DecoManager();
			}
			return DecoManager.m_Instance;
		}
	}

	// Token: 0x06005E5F RID: 24159 RVA: 0x0024C858 File Offset: 0x0024AA58
	[PublicizedFrom(EAccessModifier.Private)]
	public DecoManager()
	{
	}

	// Token: 0x06005E60 RID: 24160 RVA: 0x0024C8F0 File Offset: 0x0024AAF0
	public IEnumerator OnWorldLoaded(int _worldWidth, int _worldHeight, World _world, IChunkProvider _chunkProvider)
	{
		if (!this.IsEnabled)
		{
			yield break;
		}
		MicroStopwatch mswYields = new MicroStopwatch();
		this.world = _world;
		this.worldWidth = _worldWidth;
		this.worldHeight = _worldHeight;
		this.worldWidthHalf = this.worldWidth / 2;
		this.worldHeightHalf = this.worldHeight / 2;
		this.occupiedMap = new DecoOccupiedMap(this.worldWidth, this.worldHeight);
		this.bFixedSize = _world.ChunkCache.IsFixedSize;
		this.chunkProvider = _chunkProvider;
		this.terrainGenerator = ((this.chunkProvider != null) ? _chunkProvider.GetTerrainGenerator() : null);
		this.resourceNoise = new PerlinNoise(_world.Seed);
		yield return null;
		if (this.chunkProvider != null)
		{
			yield return this.chunkProvider.FillOccupiedMap(this.worldWidth, this.worldHeight, this.occupiedMap, this.overridePOIList);
		}
		yield return null;
		int num = DecoChunk.ToDecoChunkPos(-this.worldWidth / 2);
		int num2 = DecoChunk.ToDecoChunkPos(this.worldWidth / 2);
		int num3 = DecoChunk.ToDecoChunkPos(-this.worldHeight / 2);
		int num4 = DecoChunk.ToDecoChunkPos(this.worldHeight / 2);
		this.decoChunks = new Dictionary<int, DecoChunk>((num2 - num + 1) * (num4 - num3 + 1));
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				DecoChunk value = new DecoChunk(i, j, i, j);
				this.decoChunks.Add(DecoChunk.MakeKey16(i, j), value);
			}
		}
		yield return null;
		this.filenamePath = GameIO.GetSaveGameDir() + "/decoration.7dt";
		bool fileLoaded = this.TryLoad();
		string format = "[DECO] read {0}";
		object[] array = new object[1];
		int num5 = 0;
		HashSet<DecoObject> hashSet = this.loadedDecos;
		array[num5] = ((hashSet != null) ? new int?(hashSet.Count) : null);
		Log.Out(format, array);
		yield return null;
		int chunkStartX = -(_worldWidth / 2) / 128;
		int chunkEndX = _worldWidth / 2 / 128;
		int chunkStartZ = -(_worldHeight / 2) / 128;
		int chunkEndZ = _worldHeight / 2 / 128;
		int totalDecorated = 0;
		mswYields.ResetAndRestart();
		if (PlatformOptimizations.FileBackedArrays)
		{
			this.fileBackedOccupiedMap = new FileBackedDecoOccupiedMap(_worldWidth, _worldHeight);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !fileLoaded)
		{
			Log.Out("DecoManager chunks");
			int num6;
			for (int z = chunkStartZ; z <= chunkEndZ; z = num6 + 1)
			{
				for (int k = chunkStartX; k <= chunkEndX; k++)
				{
					GameRandom gameRandom = Utils.RandomFromSeedOnPos(k, z, this.world.Seed);
					DecoChunk decoChunk = this.decoChunks[DecoChunk.MakeKey16(k, z)];
					totalDecorated += this.decorateChunkRandom(decoChunk, gameRandom);
					GameRandomManager.Instance.FreeGameRandom(gameRandom);
				}
				if (mswYields.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					mswYields.ResetAndRestart();
				}
				if (PlatformOptimizations.FileBackedArrays && z > chunkStartZ)
				{
					this.fileBackedOccupiedMap.CopyDecoChunkRow(z - 1, this.occupiedMap.GetData());
				}
				num6 = z;
			}
			if (PlatformOptimizations.FileBackedArrays)
			{
				this.occupiedMap = null;
			}
		}
		yield return null;
		if (this.loadedDecos != null)
		{
			foreach (DecoObject decoObject in this.loadedDecos)
			{
				this.addLoadedDecoration(decoObject);
			}
			this.loadedDecos = null;
			for (int l = chunkStartZ; l <= chunkEndZ; l++)
			{
				for (int m = chunkStartX; m <= chunkEndX; m++)
				{
					this.decoChunks[DecoChunk.MakeKey16(m, l)].isDecorated = true;
				}
				if (PlatformOptimizations.FileBackedArrays && l > chunkStartZ)
				{
					this.fileBackedOccupiedMap.CopyDecoChunkRow(l - 1, this.occupiedMap.GetData());
				}
			}
			if (PlatformOptimizations.FileBackedArrays)
			{
				this.occupiedMap = null;
			}
		}
		this.bDirty = true;
		yield break;
	}

	// Token: 0x06005E61 RID: 24161 RVA: 0x0024C91C File Offset: 0x0024AB1C
	public void OriginChanged(Vector3 _offset)
	{
		foreach (KeyValuePair<int, DecoChunk> keyValuePair in this.decoChunks)
		{
			GameObject rootObj = keyValuePair.Value.rootObj;
			if (rootObj)
			{
				rootObj.transform.position += _offset;
			}
		}
	}

	// Token: 0x06005E62 RID: 24162 RVA: 0x0024C994 File Offset: 0x0024AB94
	public void OnWorldUnloaded()
	{
		if (!this.IsEnabled)
		{
			return;
		}
		if (this.updateCoroutine != null)
		{
			ThreadManager.StopCoroutine(this.updateCoroutine);
			this.updateCoroutine = null;
		}
		foreach (KeyValuePair<int, DecoChunk> keyValuePair in this.decoChunks)
		{
			keyValuePair.Value.Destroy();
		}
		this.occupiedMap = null;
		if (PlatformOptimizations.FileBackedArrays)
		{
			FileBackedDecoOccupiedMap fileBackedDecoOccupiedMap = this.fileBackedOccupiedMap;
			if (fileBackedDecoOccupiedMap != null)
			{
				fileBackedDecoOccupiedMap.Dispose();
			}
			this.fileBackedOccupiedMap = null;
		}
		this.overridePOIList = null;
		this.checkDelayTicks = 0;
		this.decoChunks.Clear();
		this.visibleDecoChunks.Clear();
		this.loadedDecos = null;
		this.addDecosFromThread.Clear();
		this.removeDecosFromThread.Clear();
		if (this.writeTask != null)
		{
			this.writeTask.WaitForEnd();
		}
	}

	// Token: 0x06005E63 RID: 24163 RVA: 0x0024CA8C File Offset: 0x0024AC8C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryLoad()
	{
		if (GameManager.Instance.IsEditMode() || !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || !SdFile.Exists(this.filenamePath))
		{
			return false;
		}
		using (Stream stream = SdFile.OpenRead(this.filenamePath))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(stream);
				byte b = pooledBinaryReader.ReadByte();
				if (b == 6)
				{
					this.Read(pooledBinaryReader, (int)b, true);
					return true;
				}
				Log.Warning(string.Format("Saved decoration data is out of date. Saved version is ({0}). Current version is ({1}). ", b, 6) + "Decorations will be regenerated for this map, but it is recommended to start a new game.");
			}
		}
		SdFile.Delete(this.filenamePath);
		return false;
	}

	// Token: 0x06005E64 RID: 24164 RVA: 0x0024CB5C File Offset: 0x0024AD5C
	public void Save()
	{
		if (!this.IsEnabled || GameManager.Instance.IsEditMode() || !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (!this.bDirty)
		{
			return;
		}
		if (this.writeTask != null)
		{
			this.writeTask.WaitForEnd();
		}
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			this.writeStream.Position = 0L;
			pooledBinaryWriter.SetBaseStream(this.writeStream);
			pooledBinaryWriter.Write(6);
			this.Write(pooledBinaryWriter, Block.nameIdMapping);
		}
		this.writeTask = ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.WriteTask), null, null, true);
		this.bDirty = false;
	}

	// Token: 0x06005E65 RID: 24165 RVA: 0x0024CC1C File Offset: 0x0024AE1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteTask(ThreadManager.TaskInfo _taskInfo)
	{
		MicroStopwatch microStopwatch = new MicroStopwatch();
		using (Stream stream = SdFile.Open(this.filenamePath, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			this.writeStream.Position = 0L;
			StreamUtils.StreamCopy(this.writeStream, stream, null, true);
			this.writeStream.SetLength(0L);
		}
		Log.Out(string.Format("[DECO] write thread {0}ms", microStopwatch.ElapsedMilliseconds));
	}

	// Token: 0x06005E66 RID: 24166 RVA: 0x0024CC9C File Offset: 0x0024AE9C
	public void Read(BinaryReader _br, int _version = 2147483647, bool _resetExisting = true)
	{
		if (_resetExisting)
		{
			this.loadedDecos = new HashSet<DecoObject>();
		}
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			DecoObject decoObject = new DecoObject();
			decoObject.Read(_br);
			this.loadedDecos.Add(decoObject);
		}
	}

	// Token: 0x06005E67 RID: 24167 RVA: 0x0024CCE4 File Offset: 0x0024AEE4
	public void Write(BinaryWriter _bw, NameIdMapping _blockMap)
	{
		MicroStopwatch microStopwatch = new MicroStopwatch();
		int num = 0;
		List<DecoObject> obj = this.decoWriteList;
		lock (obj)
		{
			this.GenerateDecoWriteList();
			num = this.decoWriteList.Count;
			_bw.Write(num);
			for (int i = 0; i < num; i++)
			{
				this.decoWriteList[i].Write(_bw, _blockMap);
			}
			this.decoWriteList.Clear();
		}
		Log.Out(string.Format("[DECO] written {0}, in {1}ms", num, microStopwatch.ElapsedMilliseconds));
	}

	// Token: 0x06005E68 RID: 24168 RVA: 0x0024CD90 File Offset: 0x0024AF90
	[PublicizedFrom(EAccessModifier.Private)]
	public void GenerateDecoWriteList()
	{
		List<DecoObject> obj = this.decoWriteList;
		lock (obj)
		{
			this.decoWriteList.Clear();
			foreach (KeyValuePair<int, DecoChunk> keyValuePair in this.decoChunks)
			{
				foreach (KeyValuePair<long, List<DecoObject>> keyValuePair2 in keyValuePair.Value.decosPerSmallChunks)
				{
					List<DecoObject> value = keyValuePair2.Value;
					for (int i = 0; i < value.Count; i++)
					{
						this.decoWriteList.Add(value[i]);
					}
				}
			}
		}
	}

	// Token: 0x06005E69 RID: 24169 RVA: 0x0024CE84 File Offset: 0x0024B084
	public void UpdateTick(World _world)
	{
		if (!this.IsEnabled)
		{
			return;
		}
		this.checkDelayTicks--;
		if (this.updateCoroutine != null)
		{
			return;
		}
		if (this.addDecosFromThread.Count > 0)
		{
			this.checkDelayTicks = 0;
			List<DecoManager.SAddDecoInfo> obj = this.addDecosFromThread;
			lock (obj)
			{
				for (int i = 0; i < this.addDecosFromThread.Count; i++)
				{
					this.AddDecorationAt(this.addDecosFromThread[i].world, this.addDecosFromThread[i].bv, this.addDecosFromThread[i].pos, this.addDecosFromThread[i].bForceBlockYPos);
				}
				this.addDecosFromThread.Clear();
			}
		}
		if (this.removeDecosFromThread.Count > 0)
		{
			this.checkDelayTicks = 0;
			List<Vector3i> obj2 = this.removeDecosFromThread;
			lock (obj2)
			{
				for (int j = 0; j < this.removeDecosFromThread.Count; j++)
				{
					this.RemoveDecorationAt(this.removeDecosFromThread[j]);
				}
				this.removeDecosFromThread.Clear();
			}
		}
		if (this.resetDecosInWorldRectFromThread.Count > 0)
		{
			this.checkDelayTicks = 0;
			List<Rect> obj3 = this.resetDecosInWorldRectFromThread;
			lock (obj3)
			{
				for (int k = 0; k < this.resetDecosInWorldRectFromThread.Count; k++)
				{
					this.ResetDecosInWorldRect(this.resetDecosInWorldRectFromThread[k]);
				}
				this.resetDecosInWorldRectFromThread.Clear();
			}
		}
		if (this.resetDecosForWorldChunkFromThread.Count > 0)
		{
			this.checkDelayTicks = 0;
			List<long> obj4 = this.resetDecosForWorldChunkFromThread;
			lock (obj4)
			{
				for (int l = 0; l < this.resetDecosForWorldChunkFromThread.Count; l++)
				{
					this.ResetDecosForWorldChunk(this.resetDecosForWorldChunkFromThread[l]);
				}
				this.resetDecosForWorldChunkFromThread.Clear();
			}
		}
		if (this.checkDelayTicks > 0)
		{
			return;
		}
		this.checkDelayTicks = 20;
		new MicroStopwatch();
		this.playersToCheck.Clear();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.playersToCheck.AddRange(_world.Players.list);
		}
		else
		{
			EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
			if (primaryPlayer == null)
			{
				return;
			}
			this.playersToCheck.Add(primaryPlayer);
		}
		if (this.IsHidden)
		{
			this.playersToCheck.Clear();
		}
		this.chunksAroundPlayers.Clear();
		for (int m = this.playersToCheck.Count - 1; m >= 0; m--)
		{
			EntityPlayer entityPlayer = this.playersToCheck[m];
			Vector3i blockPosition = entityPlayer.GetBlockPosition();
			int num = DecoChunk.ToDecoChunkPos(blockPosition.x);
			int num2 = DecoChunk.ToDecoChunkPos(blockPosition.z);
			int num3 = entityPlayer.isEntityRemote ? 1 : GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTreeDistance);
			int num4 = num - num3;
			int num5 = num + num3;
			int num6 = num2 - num3;
			int num7 = num2 + num3;
			for (int n = num4; n <= num5; n++)
			{
				for (int num8 = num6; num8 <= num7; num8++)
				{
					this.chunksAroundPlayers.Add(DecoChunk.MakeKey16(n, num8));
				}
			}
		}
		this.updateCoroutine = ThreadManager.StartCoroutine(this.UpdateDecorationsCo());
	}

	// Token: 0x06005E6A RID: 24170 RVA: 0x0024D21C File Offset: 0x0024B41C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator UpdateDecorationsCo()
	{
		int waitCount = 0;
		this.msUpdate.ResetAndRestart();
		int num;
		for (int i = this.visibleDecoChunks.Count - 1; i >= 0; i = num - 1)
		{
			DecoChunk decoChunk = this.visibleDecoChunks[i];
			bool flag = this.chunksAroundPlayers.Contains(DecoChunk.MakeKey16(decoChunk.decoChunkX, decoChunk.decoChunkZ));
			decoChunk.SetVisible(flag);
			if (!flag)
			{
				decoChunk.Destroy();
			}
			if (this.msUpdate.ElapsedMicroseconds > 900L)
			{
				num = waitCount;
				waitCount = num + 1;
				yield return null;
				this.msUpdate.ResetAndRestart();
			}
			num = i;
		}
		this.visibleDecoChunks.Clear();
		foreach (int key in this.chunksAroundPlayers)
		{
			DecoChunk decoChunk;
			this.decoChunks.TryGetValue(key, out decoChunk);
			if (decoChunk != null)
			{
				this.visibleDecoChunks.Add(decoChunk);
				DecoChunk obj = decoChunk;
				lock (obj)
				{
					if (!decoChunk.isDecorated)
					{
						GameRandom gameRandom = Utils.RandomFromSeedOnPos(decoChunk.decoChunkX, decoChunk.decoChunkZ, this.world.Seed);
						this.decorateChunkRandom(decoChunk, gameRandom);
						GameRandomManager.Instance.FreeGameRandom(gameRandom);
					}
				}
				if (this.msUpdate.ElapsedMicroseconds > 900L)
				{
					num = waitCount;
					waitCount = num + 1;
					yield return null;
					this.msUpdate.ResetAndRestart();
				}
				if (decoChunk.decosPerSmallChunks.Count > 0 && !decoChunk.isGameObjectUpdated)
				{
					decoChunk.UpdateGameObject();
				}
				if (!decoChunk.isModelsUpdated)
				{
					yield return decoChunk.UpdateModels(this.world, this.msUpdate);
					if (this.msUpdate.ElapsedMicroseconds > 900L)
					{
						num = waitCount;
						waitCount = num + 1;
						yield return null;
						this.msUpdate.ResetAndRestart();
					}
				}
			}
		}
		HashSet<int>.Enumerator enumerator = default(HashSet<int>.Enumerator);
		this.updateCoroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x06005E6B RID: 24171 RVA: 0x0024D22B File Offset: 0x0024B42B
	[PublicizedFrom(EAccessModifier.Private)]
	public DecoObject GetDecoObject()
	{
		return new DecoObject();
	}

	// Token: 0x06005E6C RID: 24172 RVA: 0x0024D234 File Offset: 0x0024B434
	[PublicizedFrom(EAccessModifier.Private)]
	public DecoChunk GetDecoChunkAt(int _x, int _z)
	{
		int key = DecoChunk.MakeKey16(_x, _z);
		DecoChunk result;
		if (!this.decoChunks.TryGetValue(key, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06005E6D RID: 24173 RVA: 0x0024D25C File Offset: 0x0024B45C
	[PublicizedFrom(EAccessModifier.Private)]
	public int decorateChunkRandom(DecoChunk _decoChunk, GameRandom rnd)
	{
		if (this.bFixedSize)
		{
			_decoChunk.isDecorated = true;
			return 0;
		}
		if (this.chunkProvider == null)
		{
			return 0;
		}
		IBiomeProvider biomeProvider = this.chunkProvider.GetBiomeProvider();
		if (biomeProvider == null)
		{
			return 0;
		}
		int num = _decoChunk.decoChunkX * 128;
		int num2 = num + 128;
		int num3 = _decoChunk.decoChunkZ * 128;
		int num4 = num3 + 128;
		int num5 = 0;
		for (int i = 0; i < 1000; i++)
		{
			int num6 = rnd.RandomRange(num, num2 - 1);
			int num7 = rnd.RandomRange(num3, num4 - 1);
			if (this.occupiedMap.Get(num6, num7) <= EnumDecoOccupied.Stop_BigDeco && !this.occupiedMap.CheckArea(num6 - 2, num7 - 2, EnumDecoOccupied.POI, 5, 5))
			{
				BiomeDefinition biomeOrSubAt = biomeProvider.GetBiomeOrSubAt(num6, num7);
				if (biomeOrSubAt != null)
				{
					float num8 = -1f;
					BlockValue blockValue = BlockValue.Air;
					int num9 = 3;
					for (int j = biomeOrSubAt.m_DistantDecoBlocks.Count - 1; j >= 0; j--)
					{
						BiomeBlockDecoration biomeBlockDecoration = biomeOrSubAt.m_DistantDecoBlocks[j];
						if (rnd.RandomFloat <= biomeBlockDecoration.prob * 0.125f * 16f)
						{
							if (biomeBlockDecoration.checkResourceOffsetY < 2147483647)
							{
								if (num8 < 0f)
								{
									num8 = this.terrainGenerator.GetTerrainHeightAt(num6, num7) + 1f;
								}
								if (!GameUtils.CheckOreNoiseAt(this.resourceNoise, num6, (int)num8 + biomeBlockDecoration.checkResourceOffsetY, num7))
								{
									goto IL_173;
								}
							}
							blockValue = biomeBlockDecoration.blockValues[0];
							num9 = biomeBlockDecoration.randomRotateMax;
							break;
						}
						IL_173:;
					}
					Block block;
					if (!blockValue.isair && (block = blockValue.Block) != null && block.IsDistantDecoration)
					{
						BlockValue bv = new BlockValue((uint)blockValue.type);
						bv.rotation = BiomeBlockDecoration.GetRandomRotation(rnd.RandomFloat, (block.isMultiBlock && num9 > 3) ? 3 : num9);
						if (this.TryAddToOccupiedMap(block, num6, num7, bv.rotation, true))
						{
							if (num8 < 0f)
							{
								num8 = this.terrainGenerator.GetTerrainHeightAt(num6, num7) + 1f;
							}
							int y = (int)(num8 + 0.5f);
							DecoObject decoObject = this.GetDecoObject();
							decoObject.Init(new Vector3i(num6, y, num7), num8, bv, DecoState.GeneratedActive);
							_decoChunk.AddDecoObject(decoObject, false);
							this.bDirty = true;
							num5++;
						}
					}
				}
			}
		}
		_decoChunk.isDecorated = true;
		return num5;
	}

	// Token: 0x06005E6E RID: 24174 RVA: 0x0024D4D8 File Offset: 0x0024B6D8
	public void GetDecorationsOnChunk(int _chunkX, int _chunkZ, List<SBlockPosValue> _multiBlockList)
	{
		int num = 0;
		try
		{
			_multiBlockList.Clear();
			num = 1;
			if (this.IsEnabled)
			{
				int x = DecoChunk.ToDecoChunkPos(_chunkX * 16);
				int z = DecoChunk.ToDecoChunkPos(_chunkZ * 16);
				DecoChunk decoChunk;
				if (this.decoChunks.TryGetValue(DecoChunk.MakeKey16(x, z), out decoChunk))
				{
					num = 2;
					DecoChunk obj = decoChunk;
					lock (obj)
					{
						if (!decoChunk.isDecorated)
						{
							Log.Error("Decorating chunk, should not happen at this point!");
							GameRandom gameRandom = Utils.RandomFromSeedOnPos(decoChunk.decoChunkX, decoChunk.decoChunkZ, this.world.Seed);
							this.decorateChunkRandom(decoChunk, gameRandom);
							GameRandomManager.Instance.FreeGameRandom(gameRandom);
						}
					}
					num = 3;
					List<DecoObject> list;
					if (decoChunk.decosPerSmallChunks.TryGetValue(WorldChunkCache.MakeChunkKey(_chunkX, _chunkZ), out list))
					{
						num = 4;
						for (int i = 0; i < list.Count; i++)
						{
							DecoObject decoObject = list[i];
							if (decoObject == null)
							{
								Log.Warning("DecoManager decosInChunk #{0} null at {1}, {2}", new object[]
								{
									i,
									_chunkX,
									_chunkZ
								});
							}
							else if (decoObject.state != DecoState.GeneratedInactive)
							{
								_multiBlockList.Add(new SBlockPosValue(new Vector3i(decoObject.pos.x, decoObject.pos.y, decoObject.pos.z), decoObject.bv));
							}
						}
					}
				}
			}
		}
		catch (NullReferenceException ex)
		{
			Log.Error("Exception position: " + num.ToString());
			throw ex;
		}
	}

	// Token: 0x06005E6F RID: 24175 RVA: 0x0024D69C File Offset: 0x0024B89C
	public bool GetParentBlockOfDecoration(Transform _t, out Vector3i _blockPos, out DecoObject _decoObject)
	{
		_blockPos = Vector3i.zero;
		_decoObject = null;
		if (!this.IsEnabled)
		{
			return false;
		}
		Transform transform = RootTransformRefParent.FindRoot(_t);
		Vector3 position = transform.position;
		int num = Utils.Fastfloor(position.x - DecoManager.cDecoMiddleOffset.x + Origin.position.x);
		int num2 = Utils.Fastfloor(position.z - DecoManager.cDecoMiddleOffset.z + Origin.position.z);
		DecoChunk decoChunk;
		if (!this.decoChunks.TryGetValue(DecoChunk.MakeKey16(DecoChunk.ToDecoChunkPos(num), DecoChunk.ToDecoChunkPos(num2)), out decoChunk))
		{
			return false;
		}
		if (!decoChunk.isModelsUpdated)
		{
			return false;
		}
		List<DecoObject> list;
		if (!decoChunk.decosPerSmallChunks.TryGetValue(WorldChunkCache.MakeChunkKey(World.toChunkXZ(num), World.toChunkXZ(num2)), out list))
		{
			return false;
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			DecoObject decoObject = list[i];
			if (decoObject.state != DecoState.GeneratedInactive && decoObject.go && decoObject.go.transform == transform)
			{
				_decoObject = decoObject;
				_blockPos = decoObject.pos;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005E70 RID: 24176 RVA: 0x0024D7C4 File Offset: 0x0024B9C4
	public Transform GetDecorationTransform(Vector3i _blockPos, bool _bDetachTransform = false)
	{
		if (!this.IsEnabled)
		{
			return null;
		}
		DecoChunk decoChunk;
		if (!this.decoChunks.TryGetValue(DecoChunk.MakeKey16(DecoChunk.ToDecoChunkPos(_blockPos.x), DecoChunk.ToDecoChunkPos(_blockPos.z)), out decoChunk))
		{
			return null;
		}
		List<DecoObject> list;
		if (!decoChunk.decosPerSmallChunks.TryGetValue(WorldChunkCache.MakeChunkKey(World.toChunkXZ(_blockPos.x), World.toChunkXZ(_blockPos.z)), out list))
		{
			return null;
		}
		int i = list.Count - 1;
		while (i >= 0)
		{
			DecoObject decoObject = list[i];
			if (decoObject.state != DecoState.GeneratedInactive && !(decoObject.pos != _blockPos))
			{
				if (decoObject.go == null)
				{
					return null;
				}
				Transform transform = decoObject.go.transform;
				if (_bDetachTransform)
				{
					if (OcclusionManager.Instance.cullDecorations)
					{
						OcclusionManager.Instance.RemoveDeco(decoChunk, decoObject.go.transform);
					}
					decoObject.go = null;
				}
				return transform;
			}
			else
			{
				i--;
			}
		}
		return null;
	}

	// Token: 0x06005E71 RID: 24177 RVA: 0x0024D8B0 File Offset: 0x0024BAB0
	public void AddDecorationAt(World _world, BlockValue _blockValue, Vector3i _blockPos, bool _bForceBlockYPos = false)
	{
		if (!ThreadManager.IsMainThread())
		{
			List<DecoManager.SAddDecoInfo> obj = this.addDecosFromThread;
			lock (obj)
			{
				this.addDecosFromThread.Add(new DecoManager.SAddDecoInfo
				{
					world = _world,
					bv = _blockValue,
					pos = _blockPos,
					bForceBlockYPos = _bForceBlockYPos
				});
				return;
			}
		}
		float num = (float)_blockPos.y;
		if (!_bForceBlockYPos && _blockPos.y > 0 && this.terrainGenerator != null)
		{
			BlockValue block = this.world.GetBlock(_blockPos - new Vector3i(0, 1, 0));
			if (block.Block != null && block.Block.shape.IsTerrain())
			{
				num = this.terrainGenerator.GetTerrainHeightAt(_blockPos.x, _blockPos.z) + 1f;
			}
		}
		this.bDirty = true;
		DecoChunk decoChunkAt = this.GetDecoChunkAt(DecoChunk.ToDecoChunkPos(_blockPos.x), DecoChunk.ToDecoChunkPos(_blockPos.z));
		if (decoChunkAt != null)
		{
			DecoObject decoObjectAt = decoChunkAt.GetDecoObjectAt(_blockPos);
			if (decoObjectAt != null)
			{
				if (decoObjectAt.realYPos == num && decoObjectAt.bv.Equals(_blockValue) && decoObjectAt.bv.rotation == _blockValue.rotation)
				{
					return;
				}
				decoChunkAt.RemoveDecoObject(decoObjectAt);
			}
		}
		DecoObject decoObject = this.GetDecoObject();
		decoObject.Init(_blockPos, num, _blockValue, DecoState.Dynamic);
		if (decoChunkAt == null)
		{
			if (this.loadedDecos == null)
			{
				this.loadedDecos = new HashSet<DecoObject>();
			}
			this.loadedDecos.Add(decoObject);
			return;
		}
		decoChunkAt.AddDecoObject(decoObject, true);
	}

	// Token: 0x06005E72 RID: 24178 RVA: 0x0024DA48 File Offset: 0x0024BC48
	[PublicizedFrom(EAccessModifier.Private)]
	public void addLoadedDecoration(DecoObject _decoObject)
	{
		if (!this.IsEnabled)
		{
			return;
		}
		DecoChunk decoChunkAt = this.GetDecoChunkAt(DecoChunk.ToDecoChunkPos(_decoObject.pos.x), DecoChunk.ToDecoChunkPos(_decoObject.pos.z));
		if (decoChunkAt == null)
		{
			return;
		}
		decoChunkAt.AddDecoObject(_decoObject, false);
		if (_decoObject.state != DecoState.Dynamic)
		{
			this.TryAddToOccupiedMap(_decoObject.bv.Block, _decoObject.pos.x, _decoObject.pos.z, _decoObject.bv.rotation, false);
		}
		this.bDirty = true;
	}

	// Token: 0x06005E73 RID: 24179 RVA: 0x0024DAD8 File Offset: 0x0024BCD8
	public static int CheckPosition(int worldWidth, int worldHeight, int _x, int _z)
	{
		int num = worldWidth / 2;
		int num2 = worldHeight / 2;
		if (_x < -num || _x >= num || _z < -num2 || _z >= num2)
		{
			return -1;
		}
		return _x + num + (_z + num2) * worldWidth;
	}

	// Token: 0x06005E74 RID: 24180 RVA: 0x0024DB0C File Offset: 0x0024BD0C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryAddToOccupiedMap(Block block, int xWorld, int zWorld, byte rotationByte, bool enableStopBigDecoCheck)
	{
		int num = xWorld;
		int num2 = zWorld;
		int num3 = 1;
		int num4 = 1;
		if (!block.isMultiBlock)
		{
			this.occupiedMap.Set(xWorld, zWorld, EnumDecoOccupied.Deco);
		}
		else
		{
			switch (rotationByte)
			{
			case 1:
				num3 = block.multiBlockPos.dim.z;
				num4 = block.multiBlockPos.dim.x;
				break;
			case 2:
				num3 = block.multiBlockPos.dim.x;
				num4 = block.multiBlockPos.dim.z;
				break;
			case 3:
				num3 = block.multiBlockPos.dim.z;
				num4 = block.multiBlockPos.dim.x;
				break;
			default:
				num3 = block.multiBlockPos.dim.x;
				num4 = block.multiBlockPos.dim.z;
				break;
			}
			num = ((num3 % 2 == 0) ? (xWorld - num3 / 2 + 1) : (xWorld - num3 / 2));
			num2 = ((num4 % 2 == 0) ? (zWorld - num4 / 2 + 1) : (zWorld - num4 / 2));
			if (enableStopBigDecoCheck && this.occupiedMap.CheckArea(num, num2, EnumDecoOccupied.Stop_BigDeco, num3, num4))
			{
				return false;
			}
			this.occupiedMap.SetArea(num, num2, EnumDecoOccupied.Deco, num3, num4);
		}
		if (block.BigDecorationRadius > 0)
		{
			this.occupiedMap.SetArea(num - block.BigDecorationRadius, num2 - block.BigDecorationRadius, EnumDecoOccupied.Perimeter, block.BigDecorationRadius * 2 + num3, block.BigDecorationRadius * 2 + num4);
		}
		return true;
	}

	// Token: 0x06005E75 RID: 24181 RVA: 0x0024DC6C File Offset: 0x0024BE6C
	public bool RemoveDecorationAt(Vector3i _blockPos)
	{
		if (!this.IsEnabled)
		{
			return false;
		}
		int key = DecoChunk.MakeKey16(DecoChunk.ToDecoChunkPos(_blockPos.x), DecoChunk.ToDecoChunkPos(_blockPos.z));
		DecoChunk decoChunk;
		if (!this.decoChunks.TryGetValue(key, out decoChunk))
		{
			return false;
		}
		if (!ThreadManager.IsMainThread())
		{
			List<Vector3i> obj = this.removeDecosFromThread;
			lock (obj)
			{
				this.removeDecosFromThread.Add(_blockPos);
				return true;
			}
		}
		this.bDirty = true;
		return decoChunk.RemoveDecoObject(_blockPos);
	}

	// Token: 0x06005E76 RID: 24182 RVA: 0x0024DD08 File Offset: 0x0024BF08
	public void ClearDecoObjectsInArea(Vector3i _boundsPosition, Vector3i _boundsSize)
	{
		if (!this.IsEnabled)
		{
			return;
		}
		int num = DecoChunk.ToDecoChunkPos(_boundsPosition.x);
		int num2 = DecoChunk.ToDecoChunkPos(_boundsPosition.z);
		int num3 = DecoChunk.ToDecoChunkPos(_boundsPosition.x + _boundsSize.x);
		int num4 = DecoChunk.ToDecoChunkPos(_boundsPosition.z + _boundsSize.z);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				int key = DecoChunk.MakeKey16(i, j);
				DecoChunk decoChunk;
				if (this.decoChunks.TryGetValue(key, out decoChunk))
				{
					decoChunk.ClearDecoObjectsInArea(_boundsPosition, _boundsSize);
					this.bDirty = true;
				}
			}
		}
	}

	// Token: 0x06005E77 RID: 24183 RVA: 0x0024DDA4 File Offset: 0x0024BFA4
	public EnumDecoOccupied GetDecoOccupiedAt(int _x, int _z)
	{
		if (!this.IsEnabled)
		{
			return EnumDecoOccupied.Free;
		}
		if (this.occupiedMap == null && !PlatformOptimizations.FileBackedArrays)
		{
			return EnumDecoOccupied.NoneAllowed;
		}
		int offs;
		if ((offs = DecoManager.CheckPosition(this.worldWidth, this.worldHeight, _x, _z)) < 0)
		{
			return EnumDecoOccupied.NoneAllowed;
		}
		DecoChunk decoChunk;
		if (!this.decoChunks.TryGetValue(DecoChunk.MakeKey16(DecoChunk.ToDecoChunkPos(_x), DecoChunk.ToDecoChunkPos(_z)), out decoChunk))
		{
			return EnumDecoOccupied.NoneAllowed;
		}
		if (!decoChunk.isDecorated)
		{
			DecoChunk obj = decoChunk;
			lock (obj)
			{
				if (!decoChunk.isDecorated)
				{
					Log.Error("Should not be decorating here!");
					GameRandom gameRandom = Utils.RandomFromSeedOnPos(decoChunk.decoChunkX, decoChunk.decoChunkZ, this.world.Seed);
					this.decorateChunkRandom(decoChunk, gameRandom);
					GameRandomManager.Instance.FreeGameRandom(gameRandom);
				}
			}
		}
		if (PlatformOptimizations.FileBackedArrays)
		{
			return this.fileBackedOccupiedMap.Get(offs);
		}
		return this.occupiedMap.Get(offs);
	}

	// Token: 0x06005E78 RID: 24184 RVA: 0x0024DEA0 File Offset: 0x0024C0A0
	public void SetChunkDistance(int _distance)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTreeDistance, _distance);
	}

	// Token: 0x06005E79 RID: 24185 RVA: 0x0024DEAD File Offset: 0x0024C0AD
	public void SetBlock(World _world, Vector3i _blockPos, BlockValue _bv)
	{
		if (_bv.isair)
		{
			this.RemoveDecorationAt(_blockPos);
			return;
		}
		this.RemoveDecorationAt(_blockPos);
		this.AddDecorationAt(_world, _bv, _blockPos, true);
	}

	// Token: 0x06005E7A RID: 24186 RVA: 0x0024DED3 File Offset: 0x0024C0D3
	public void SaveDebugTexture(string path, bool _includeFlatAreas = false)
	{
		if (PlatformOptimizations.FileBackedArrays)
		{
			this.fileBackedOccupiedMap.SaveAsTexture(path, true, null);
			return;
		}
		DecoOccupiedMap decoOccupiedMap = this.occupiedMap;
		if (decoOccupiedMap == null)
		{
			return;
		}
		decoOccupiedMap.SaveAsTexture(path, _includeFlatAreas, null);
	}

	// Token: 0x06005E7B RID: 24187 RVA: 0x0024DEFE File Offset: 0x0024C0FE
	public void SaveDebugTexture(string path, List<FlatArea> flatAreas)
	{
		if (PlatformOptimizations.FileBackedArrays)
		{
			this.fileBackedOccupiedMap.SaveAsTexture(path, true, flatAreas);
			return;
		}
		DecoOccupiedMap decoOccupiedMap = this.occupiedMap;
		if (decoOccupiedMap == null)
		{
			return;
		}
		decoOccupiedMap.SaveAsTexture(path, true, flatAreas);
	}

	// Token: 0x06005E7C RID: 24188 RVA: 0x0024DF2C File Offset: 0x0024C12C
	public void SaveStateDebugTexture(string _filename)
	{
		Color32[] array = new Color32[this.worldWidth * this.worldHeight];
		int num = -this.worldWidth / 2;
		int num2 = -this.worldHeight / 2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Color.black;
			int num3 = i % this.worldWidth + num;
			int num4 = i / this.worldWidth + num2;
			int key = DecoChunk.MakeKey16(DecoChunk.ToDecoChunkPos(num3), DecoChunk.ToDecoChunkPos(num4));
			DecoChunk decoChunk;
			if (this.decoChunks.TryGetValue(key, out decoChunk))
			{
				long key2 = WorldChunkCache.MakeChunkKey(World.toChunkXZ(num3), World.toChunkXZ(num4));
				List<DecoObject> list;
				if (decoChunk.decosPerSmallChunks.TryGetValue(key2, out list))
				{
					foreach (DecoObject decoObject in list)
					{
						if (decoObject.pos.x == num3 && decoObject.pos.z == num4)
						{
							switch (decoObject.state)
							{
							case DecoState.GeneratedActive:
								array[i] = Color.green;
								goto IL_154;
							case DecoState.GeneratedInactive:
								array[i] = Color.yellow;
								goto IL_154;
							case DecoState.Dynamic:
								array[i] = Color.blue;
								goto IL_154;
							default:
								goto IL_154;
							}
						}
					}
				}
			}
			IL_154:;
		}
		Texture2D texture2D = new Texture2D(this.worldWidth, this.worldHeight);
		texture2D.SetPixels32(array);
		texture2D.Apply();
		TextureUtils.SaveTexture(texture2D, _filename);
		UnityEngine.Object.Destroy(texture2D);
	}

	// Token: 0x06005E7D RID: 24189 RVA: 0x0024E0D4 File Offset: 0x0024C2D4
	public void SendDecosToClient(ClientInfo _cInfo)
	{
		List<DecoObject> obj = this.decoWriteList;
		lock (obj)
		{
			this.GenerateDecoWriteList();
			int i = 0;
			while (i < this.decoWriteList.Count)
			{
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageDecoUpdate>().Setup(this.decoWriteList, ref i));
			}
		}
	}

	// Token: 0x06005E7E RID: 24190 RVA: 0x0024E140 File Offset: 0x0024C340
	public void ResetDecosInWorldRect(Rect worldRect)
	{
		if (!ThreadManager.IsMainThread())
		{
			List<Rect> obj = this.resetDecosInWorldRectFromThread;
			lock (obj)
			{
				this.resetDecosInWorldRectFromThread.Add(worldRect);
				return;
			}
		}
		this.bDirty = true;
		int num = DecoChunk.ToDecoChunkPos(worldRect.x);
		int num2 = DecoChunk.ToDecoChunkPos(worldRect.y);
		int num3 = DecoChunk.ToDecoChunkPos(worldRect.xMax);
		int num4 = DecoChunk.ToDecoChunkPos(worldRect.yMax);
		Predicate<DecoObject> <>9__0;
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				int key = DecoChunk.MakeKey16(i, j);
				DecoChunk decoChunk;
				if (this.decoChunks.TryGetValue(key, out decoChunk))
				{
					DecoChunk decoChunk2 = decoChunk;
					Predicate<DecoObject> decoObjectValidator;
					if ((decoObjectValidator = <>9__0) == null)
					{
						decoObjectValidator = (<>9__0 = ((DecoObject deco) => worldRect.Contains(new Vector2((float)deco.pos.x, (float)deco.pos.z))));
					}
					decoChunk2.RestoreGeneratedDecos(decoObjectValidator);
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageDecoResetWorldRect>().Setup(worldRect), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06005E7F RID: 24191 RVA: 0x0024E290 File Offset: 0x0024C490
	public void ResetDecosForWorldChunk(long worldChunkKey)
	{
		if (!ThreadManager.IsMainThread())
		{
			List<long> obj = this.resetDecosForWorldChunkFromThread;
			lock (obj)
			{
				this.resetDecosForWorldChunkFromThread.Add(worldChunkKey);
				return;
			}
		}
		this.bDirty = true;
		int x = DecoChunk.ToDecoChunkPos(WorldChunkCache.extractX(worldChunkKey) * 16);
		int z = DecoChunk.ToDecoChunkPos(WorldChunkCache.extractZ(worldChunkKey) * 16);
		int key = DecoChunk.MakeKey16(x, z);
		DecoChunk decoChunk;
		if (this.decoChunks.TryGetValue(key, out decoChunk))
		{
			decoChunk.RestoreGeneratedDecos(worldChunkKey, (DecoObject decoObject) => true);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageDecoResetWorldChunk>().Setup(worldChunkKey), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06005E80 RID: 24192 RVA: 0x0024E378 File Offset: 0x0024C578
	public EnumDecoOccupied GetDecoOccupiedFromMap(int xWorld, int zWorld)
	{
		if (!PlatformOptimizations.FileBackedArrays)
		{
			return this.occupiedMap.Get(xWorld, zWorld);
		}
		int num = DecoManager.CheckPosition(this.worldWidth, this.worldHeight, xWorld, zWorld);
		if (num < 0)
		{
			return EnumDecoOccupied.NoneAllowed;
		}
		return this.fileBackedOccupiedMap.Get(num);
	}

	// Token: 0x06005E81 RID: 24193 RVA: 0x0024E3C0 File Offset: 0x0024C5C0
	public void ResetAll()
	{
		Rect worldRect = new Rect((float)(-(float)this.worldWidth / 2), (float)(-(float)this.worldHeight / 2), (float)this.worldWidth, (float)this.worldHeight);
		this.ResetDecosInWorldRect(worldRect);
	}

	// Token: 0x04004944 RID: 18756
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FILEVERSION = 6;

	// Token: 0x04004945 RID: 18757
	public const int cChunkSize = 128;

	// Token: 0x04004946 RID: 18758
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cUpdateDelay = 1f;

	// Token: 0x04004947 RID: 18759
	public const int cUpdateCoMaxTimeUs = 900;

	// Token: 0x04004948 RID: 18760
	public static Vector3 cDecoMiddleOffset = new Vector3(0.5f, 0f, 0.5f);

	// Token: 0x04004949 RID: 18761
	[PublicizedFrom(EAccessModifier.Private)]
	public static DecoManager m_Instance;

	// Token: 0x0400494A RID: 18762
	public bool IsEnabled = true;

	// Token: 0x0400494B RID: 18763
	public bool IsHidden;

	// Token: 0x0400494C RID: 18764
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, DecoChunk> decoChunks = new Dictionary<int, DecoChunk>();

	// Token: 0x0400494D RID: 18765
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DecoChunk> visibleDecoChunks = new List<DecoChunk>();

	// Token: 0x0400494E RID: 18766
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<DecoObject> loadedDecos;

	// Token: 0x0400494F RID: 18767
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldWidth;

	// Token: 0x04004950 RID: 18768
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldHeight;

	// Token: 0x04004951 RID: 18769
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldWidthHalf;

	// Token: 0x04004952 RID: 18770
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldHeightHalf;

	// Token: 0x04004953 RID: 18771
	[PublicizedFrom(EAccessModifier.Private)]
	public DecoOccupiedMap occupiedMap;

	// Token: 0x04004954 RID: 18772
	[PublicizedFrom(EAccessModifier.Private)]
	public FileBackedDecoOccupiedMap fileBackedOccupiedMap;

	// Token: 0x04004955 RID: 18773
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunkProvider chunkProvider;

	// Token: 0x04004956 RID: 18774
	[PublicizedFrom(EAccessModifier.Private)]
	public ITerrainGenerator terrainGenerator;

	// Token: 0x04004957 RID: 18775
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04004958 RID: 18776
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFixedSize;

	// Token: 0x04004959 RID: 18777
	[PublicizedFrom(EAccessModifier.Private)]
	public string filenamePath;

	// Token: 0x0400495A RID: 18778
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> overridePOIList;

	// Token: 0x0400495B RID: 18779
	[PublicizedFrom(EAccessModifier.Private)]
	public PerlinNoise resourceNoise;

	// Token: 0x0400495C RID: 18780
	[PublicizedFrom(EAccessModifier.Private)]
	public int checkDelayTicks;

	// Token: 0x0400495D RID: 18781
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDirty;

	// Token: 0x0400495E RID: 18782
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryStream writeStream = new MemoryStream();

	// Token: 0x0400495F RID: 18783
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.TaskInfo writeTask;

	// Token: 0x04004960 RID: 18784
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DecoManager.SAddDecoInfo> addDecosFromThread = new List<DecoManager.SAddDecoInfo>();

	// Token: 0x04004961 RID: 18785
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> removeDecosFromThread = new List<Vector3i>();

	// Token: 0x04004962 RID: 18786
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Rect> resetDecosInWorldRectFromThread = new List<Rect>();

	// Token: 0x04004963 RID: 18787
	[PublicizedFrom(EAccessModifier.Private)]
	public List<long> resetDecosForWorldChunkFromThread = new List<long>();

	// Token: 0x04004964 RID: 18788
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<DecoObject> decoWriteList = new List<DecoObject>(4096);

	// Token: 0x04004965 RID: 18789
	[PublicizedFrom(EAccessModifier.Private)]
	public MicroStopwatch msUpdate = new MicroStopwatch();

	// Token: 0x04004966 RID: 18790
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine updateCoroutine;

	// Token: 0x04004967 RID: 18791
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> playersToCheck = new List<EntityPlayer>();

	// Token: 0x04004968 RID: 18792
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> chunksAroundPlayers = new HashSet<int>();

	// Token: 0x02000C15 RID: 3093
	[PublicizedFrom(EAccessModifier.Private)]
	public struct SAddDecoInfo
	{
		// Token: 0x04004969 RID: 18793
		public World world;

		// Token: 0x0400496A RID: 18794
		public BlockValue bv;

		// Token: 0x0400496B RID: 18795
		public Vector3i pos;

		// Token: 0x0400496C RID: 18796
		public bool bForceBlockYPos;
	}
}
