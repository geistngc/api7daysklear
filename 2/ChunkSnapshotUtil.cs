using System;

// Token: 0x02000B93 RID: 2963
public class ChunkSnapshotUtil : IRegionFileChunkSnapshotUtil
{
	// Token: 0x060059AD RID: 22957 RVA: 0x002252C8 File Offset: 0x002234C8
	public ChunkSnapshotUtil(RegionFileAccessAbstract regionFileAccess)
	{
		this.regionFileAccess = regionFileAccess;
		this.chunkReader = new RegionFileChunkReader(regionFileAccess);
		this.chunkWriter = new RegionFileChunkWriter(regionFileAccess);
	}

	// Token: 0x060059AE RID: 22958 RVA: 0x002252FF File Offset: 0x002234FF
	public IRegionFileChunkSnapshot TakeSnapshot(Chunk chunk, bool saveIfUnchanged)
	{
		RegionFileChunkSnapshot regionFileChunkSnapshot = this.poolSnapshots.AllocSync(true);
		regionFileChunkSnapshot.Update(chunk, saveIfUnchanged);
		return regionFileChunkSnapshot;
	}

	// Token: 0x060059AF RID: 22959 RVA: 0x00225315 File Offset: 0x00223515
	public void WriteSnapshot(IRegionFileChunkSnapshot snapshot, string dir, int chunkX, int chunkZ)
	{
		snapshot.Write(this.chunkWriter, dir, chunkX, chunkZ);
	}

	// Token: 0x060059B0 RID: 22960 RVA: 0x00225328 File Offset: 0x00223528
	public Chunk LoadChunk(string dir, long key)
	{
		int chunkX = WorldChunkCache.extractX(key);
		int chunkZ = WorldChunkCache.extractZ(key);
		try
		{
			uint version;
			PooledBinaryReader pooledBinaryReader = this.chunkReader.readIntoLoadStream(dir, chunkX, chunkZ, "7rg", out version);
			if (pooledBinaryReader == null)
			{
				return null;
			}
			Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
			chunk.load(pooledBinaryReader, version);
			chunk.NeedsRegeneration = true;
			return chunk;
		}
		catch (Exception e)
		{
			Log.Error(string.Concat(new string[]
			{
				"EXCEPTION: In load chunk (chunkX=",
				chunkX.ToString(),
				" chunkZ=",
				chunkZ.ToString(),
				")"
			}));
			Log.Exception(e);
			try
			{
				this.chunkReader.WriteBackup(dir, chunkX, chunkZ);
			}
			catch (Exception e2)
			{
				Log.Error("Error backing up data:");
				Log.Exception(e2);
			}
		}
		try
		{
			this.regionFileAccess.Remove(dir, chunkX, chunkZ);
		}
		catch (Exception ex)
		{
			Log.Error(string.Concat(new string[]
			{
				"In remove chunk (chunkX=",
				chunkX.ToString(),
				" chunkZ=",
				chunkZ.ToString(),
				"):",
				ex.Message
			}));
		}
		return null;
	}

	// Token: 0x060059B1 RID: 22961 RVA: 0x00225474 File Offset: 0x00223674
	public void Free(IRegionFileChunkSnapshot iSnapshot)
	{
		if (iSnapshot == null)
		{
			return;
		}
		RegionFileChunkSnapshot regionFileChunkSnapshot = iSnapshot as RegionFileChunkSnapshot;
		if (regionFileChunkSnapshot != null)
		{
			this.poolSnapshots.FreeSync(regionFileChunkSnapshot);
			return;
		}
		Log.Error("Attempting to free snapshot of wrong type. Expected: RegionFileChunkSnapshot, Actual: " + iSnapshot.GetType().Name);
	}

	// Token: 0x060059B2 RID: 22962 RVA: 0x002254B6 File Offset: 0x002236B6
	public void Cleanup()
	{
		this.poolSnapshots.Cleanup();
	}

	// Token: 0x04004584 RID: 17796
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessAbstract regionFileAccess;

	// Token: 0x04004585 RID: 17797
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileChunkReader chunkReader;

	// Token: 0x04004586 RID: 17798
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileChunkWriter chunkWriter;

	// Token: 0x04004587 RID: 17799
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<RegionFileChunkSnapshot> poolSnapshots = new MemoryPooledObject<RegionFileChunkSnapshot>(255);
}
