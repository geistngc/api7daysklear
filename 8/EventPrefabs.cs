using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000927 RID: 2343
public class EventPrefabs
{
	// Token: 0x06004418 RID: 17432 RVA: 0x001A8E4C File Offset: 0x001A704C
	public EventPrefabs(World world, DynamicPrefabDecorator prefabDecorator, RegionFileManager regionFileManager)
	{
		this.world = world;
		this.prefabCache = world.m_PrefabCache;
		this.dpd = prefabDecorator;
		this.rfm = regionFileManager;
		this.saveWriter = new ThreadedFileWriterQueue("EventPrefabs.Save", Path.Combine(GameIO.GetSaveGameDir(), "eventprefabs.dat"));
	}

	// Token: 0x06004419 RID: 17433 RVA: 0x001A8EAA File Offset: 0x001A70AA
	public void GetPrefabs(List<PrefabInstance> prefabs)
	{
		prefabs.AddRange(this.prefabs);
	}

	// Token: 0x0600441A RID: 17434 RVA: 0x001A8EB8 File Offset: 0x001A70B8
	public List<PrefabInstance.Serializable> GetPrefabsSerialized()
	{
		List<PrefabInstance.Serializable> list = new List<PrefabInstance.Serializable>();
		foreach (PrefabInstance prefabInstance in this.prefabs)
		{
			list.Add(prefabInstance.GetSerializable());
		}
		return list;
	}

	// Token: 0x0600441B RID: 17435 RVA: 0x001A8F18 File Offset: 0x001A7118
	public PrefabInstance TryPlaceAt(string prefabName, byte rotation, Vector3i position, bool yIsGroundLevel = true)
	{
		Prefab prefabRotated = this.prefabCache.GetPrefabRotated(prefabName, (int)rotation, true, true, false, false);
		if (prefabRotated == null)
		{
			Log.Error("[EventPrefabs] cannot place " + prefabName + ", prefab not found");
			return null;
		}
		if (yIsGroundLevel)
		{
			position.y += prefabRotated.yOffset;
		}
		PrefabInstance prefabInstance = new PrefabInstance(this.dpd.GetNextId(), prefabRotated.location, position, rotation, prefabRotated, 0);
		HashSetLong occupiedChunks = prefabInstance.GetOccupiedChunks();
		ChunkProtectionLevel chunkProtectionLevel;
		if (!this.rfm.TryResetChunks(occupiedChunks, ChunkProtectionLevel.All, out chunkProtectionLevel))
		{
			Log.Error(string.Format("[EventPrefabs] cannot place {0} at ({1}), {2}. Chunks could not be reset, protection level: {3}", new object[]
			{
				prefabName,
				position,
				rotation,
				chunkProtectionLevel
			}));
			return null;
		}
		DecoManager.Instance.ClearDecoObjectsInArea(prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxSize);
		Log.Out(string.Format("[EventPrefabs] placing {0} at ({1}), {2}", prefabName, position, rotation));
		this.dpd.AddEventPrefab(prefabInstance);
		this.prefabs.Add(prefabInstance);
		this.rfm.AddGroupedChunks(occupiedChunks);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEventPrefab>().Setup(NetPackageEventPrefab.Operation.Add, prefabInstance), false, -1, -1, -1, null, 192, false);
		this.needsSaving = true;
		return prefabInstance;
	}

	// Token: 0x0600441C RID: 17436 RVA: 0x001A9058 File Offset: 0x001A7258
	public bool Remove(PrefabInstance pi)
	{
		HashSetLong occupiedChunks = pi.GetOccupiedChunks();
		ChunkProtectionLevel chunkProtectionLevel;
		if (!this.rfm.TryResetChunks(occupiedChunks, ChunkProtectionLevel.All, out chunkProtectionLevel))
		{
			Log.Error(string.Format("[EventPrefabs] cannot remove {0}. Chunks could not be reset, protection level: {1}", pi, chunkProtectionLevel));
			return false;
		}
		if (!this.prefabs.Remove(pi))
		{
			Log.Error(string.Format("[EventPrefabs] cannot remove {0}. Not an event prefab", pi));
			return false;
		}
		this.world.RemoveTriggerVolumesFor(pi);
		this.world.RemoveSleeperVolumesFor(pi);
		this.world.RemoveWallVolumesFor(pi);
		this.dpd.RemoveEventPrefab(pi);
		this.rfm.RemoveGroupedChunks(occupiedChunks);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEventPrefab>().Setup(NetPackageEventPrefab.Operation.Remove, pi), false, -1, -1, -1, null, 192, false);
		this.needsSaving = true;
		return true;
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x001A9124 File Offset: 0x001A7324
	public void Save(bool waitForComplete = false)
	{
		if (!this.needsSaving)
		{
			return;
		}
		this.needsSaving = false;
		PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
			pooledBinaryWriter.Write(1);
			pooledBinaryWriter.Write(this.prefabs.Count);
			foreach (PrefabInstance prefabInstance in this.prefabs)
			{
				pooledBinaryWriter.Write(prefabInstance.prefab.PrefabName);
				StreamUtils.Write(pooledBinaryWriter, prefabInstance.boundingBoxPosition);
				pooledBinaryWriter.Write(prefabInstance.rotation);
			}
		}
		pooledExpandableMemoryStream.Position = 0L;
		this.saveWriter.Write(pooledExpandableMemoryStream, waitForComplete);
	}

	// Token: 0x0600441E RID: 17438 RVA: 0x001A920C File Offset: 0x001A740C
	public void Load()
	{
		string path = Path.Combine(GameIO.GetSaveGameDir(), "eventprefabs.dat");
		if (!SdFile.Exists(path))
		{
			return;
		}
		try
		{
			Log.Out("[EventPrefabs] loading prefab list");
			using (Stream stream = SdFile.OpenRead(path))
			{
				using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader.SetBaseStream(stream);
					pooledBinaryReader.ReadInt32();
					int num = pooledBinaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						string name = pooledBinaryReader.ReadString();
						Vector3i position = StreamUtils.ReadVector3i(pooledBinaryReader);
						byte rotation = pooledBinaryReader.ReadByte();
						Prefab prefabRotated = this.prefabCache.GetPrefabRotated(name, (int)rotation, true, true, false, false);
						PrefabInstance item = new PrefabInstance(this.dpd.GetNextId(), prefabRotated.location, position, rotation, prefabRotated, 0);
						this.prefabs.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("[EventPrefabs] error while reading save data " + ex.Message);
			Log.Exception(ex);
		}
		foreach (PrefabInstance prefabInstance in this.prefabs)
		{
			DecoManager.Instance.ClearDecoObjectsInArea(prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxSize);
			this.dpd.AddEventPrefab(prefabInstance);
			this.rfm.AddGroupedChunks(prefabInstance.GetOccupiedChunks());
		}
	}

	// Token: 0x040036E1 RID: 14049
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x040036E2 RID: 14050
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabCache prefabCache;

	// Token: 0x040036E3 RID: 14051
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicPrefabDecorator dpd;

	// Token: 0x040036E4 RID: 14052
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileManager rfm;

	// Token: 0x040036E5 RID: 14053
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> prefabs = new List<PrefabInstance>();

	// Token: 0x040036E6 RID: 14054
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadedFileWriterQueue saveWriter;

	// Token: 0x040036E7 RID: 14055
	[PublicizedFrom(EAccessModifier.Private)]
	public bool needsSaving;

	// Token: 0x040036E8 RID: 14056
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSaveVersion = 1;

	// Token: 0x040036E9 RID: 14057
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cSaveFilename = "eventprefabs.dat";
}
