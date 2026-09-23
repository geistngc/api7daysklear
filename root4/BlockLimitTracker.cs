using System;
using System.Collections.Generic;
using System.IO;
using Platform;

// Token: 0x0200012A RID: 298
public class BlockLimitTracker
{
	// Token: 0x060007EA RID: 2026 RVA: 0x0003807C File Offset: 0x0003627C
	public static void Init()
	{
		BlockLimitTracker.instance = new BlockLimitTracker();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			BlockLimitTracker.instance.Load();
		}
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0003809E File Offset: 0x0003629E
	public BlockLimitTracker()
	{
		this.poweredBlockTracker = new BlockTracker(10000);
		this.playerStorageTracker = new BlockTracker(10000);
		this.clientAmounts = new List<int>();
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x000380D4 File Offset: 0x000362D4
	public bool CanAddBlock(BlockValue _blockValue, Vector3i _blockPosition, out eSetBlockResponse _response)
	{
		_response = eSetBlockResponse.Success;
		if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent() || GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (_blockValue.Block.isMultiBlock && _blockValue.ischild)
		{
			return true;
		}
		if (_blockValue.Block is BlockPowered || _blockValue.Block is BlockPowerSource)
		{
			if (!this.poweredBlockTracker.CanAdd(_blockPosition))
			{
				_response = eSetBlockResponse.PowerBlockLimitExceeded;
				return false;
			}
		}
		else
		{
			BlockCompositeTileEntity blockCompositeTileEntity = _blockValue.Block as BlockCompositeTileEntity;
			if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>() && !this.playerStorageTracker.CanAdd(_blockPosition))
			{
				_response = eSetBlockResponse.StorageBlockLimitExceeded;
				return false;
			}
		}
		return true;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x00038174 File Offset: 0x00036374
	public void TryAddTrackedBlock(BlockValue _blockValue, Vector3i _blockPosition, int _entityId)
	{
		if (_entityId == -1)
		{
			return;
		}
		if (_blockValue.isair)
		{
			return;
		}
		if (_blockValue.Block.isMultiBlock && _blockValue.ischild)
		{
			return;
		}
		Entity entity = GameManager.Instance.World.GetEntity(_entityId);
		if (entity == null || !(entity is EntityPlayer))
		{
			return;
		}
		Log.Out("TryAddTrackedBlock {0} from entity {1}", new object[]
		{
			_blockValue.Block.GetBlockName(),
			_entityId
		});
		if (_blockValue.Block is BlockPowered || _blockValue.Block is BlockPowerSource)
		{
			if (this.poweredBlockTracker.TryAddBlock(_blockPosition))
			{
				this.TriggerSave();
				Log.Out("{0}/{1} Powered Blocks", new object[]
				{
					this.poweredBlockTracker.blockLocations.Count,
					this.poweredBlockTracker.limit
				});
				return;
			}
		}
		else
		{
			BlockCompositeTileEntity blockCompositeTileEntity = _blockValue.Block as BlockCompositeTileEntity;
			if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>() && this.playerStorageTracker.TryAddBlock(_blockPosition))
			{
				this.TriggerSave();
				Log.Out("{0}/{1} Storage Blocks", new object[]
				{
					this.playerStorageTracker.blockLocations.Count,
					this.playerStorageTracker.limit
				});
			}
		}
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x000382D0 File Offset: 0x000364D0
	public void TryRemoveOrReplaceBlock(BlockValue _oldBlockValue, BlockValue _newBlockValue, Vector3i _blockPosition)
	{
		if (_oldBlockValue.Block.isMultiBlock && _oldBlockValue.ischild)
		{
			return;
		}
		if (_oldBlockValue.Block is BlockPowered || _oldBlockValue.Block is BlockPowerSource)
		{
			if (_newBlockValue.Block is BlockPowered || _newBlockValue.Block is BlockPowerSource)
			{
				return;
			}
			if (this.poweredBlockTracker.RemoveBlock(_blockPosition))
			{
				this.TriggerSave();
				Log.Out("{0}/{1} powered Blocks", new object[]
				{
					this.poweredBlockTracker.blockLocations.Count,
					this.poweredBlockTracker.limit
				});
				return;
			}
		}
		else
		{
			BlockCompositeTileEntity blockCompositeTileEntity = _oldBlockValue.Block as BlockCompositeTileEntity;
			if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
			{
				BlockCompositeTileEntity blockCompositeTileEntity2 = _newBlockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity2 != null && blockCompositeTileEntity2.CompositeData.HasFeature<ITileEntityLootable>())
				{
					return;
				}
				if (this.playerStorageTracker.RemoveBlock(_blockPosition))
				{
					this.TriggerSave();
					Log.Out("{0}/{1} Storage Blocks", new object[]
					{
						this.playerStorageTracker.blockLocations.Count,
						this.playerStorageTracker.limit
					});
				}
			}
		}
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0003840C File Offset: 0x0003660C
	public void ServerUpdateClients()
	{
		if (this.clientAmounts.Count == 0 || this.clientAmounts[0] != this.poweredBlockTracker.blockLocations.Count || this.clientAmounts[1] != this.playerStorageTracker.blockLocations.Count)
		{
			this.clientAmounts.Clear();
			this.clientAmounts.Add(this.poweredBlockTracker.blockLocations.Count);
			this.clientAmounts.Add(this.playerStorageTracker.blockLocations.Count);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBlockLimitTracking>().Setup(this.clientAmounts), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x000384D0 File Offset: 0x000366D0
	public void UpdateClientAmounts(List<int> _amounts)
	{
		if (_amounts.Count != 2)
		{
			Log.Error("Client block limit count not exepcted amount");
			return;
		}
		this.poweredBlockTracker.clientAmount = _amounts[0];
		this.playerStorageTracker.clientAmount = _amounts[1];
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0003850A File Offset: 0x0003670A
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearAll()
	{
		this.poweredBlockTracker.Clear();
		this.playerStorageTracker.Clear();
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00038522 File Offset: 0x00036722
	public static void Cleanup()
	{
		if (BlockLimitTracker.instance != null)
		{
			BlockLimitTracker.instance.Save();
			BlockLimitTracker.instance.ClearAll();
			BlockLimitTracker.instance = null;
		}
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00038548 File Offset: 0x00036748
	public void Load()
	{
		string path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat");
		if (SdFile.Exists(path))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(path))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.read(pooledBinaryReader);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("BlockLimitTracker Load Exception: " + ex.Message);
				path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat.bak");
				if (SdFile.Exists(path))
				{
					using (Stream stream2 = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.read(pooledBinaryReader2);
						}
					}
				}
			}
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00038668 File Offset: 0x00036868
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _reader)
	{
		if (_reader.ReadByte() != 1)
		{
			Log.Error("BlockLimitTracker Read bad version");
			return;
		}
		this.ClearAll();
		this.poweredBlockTracker.Read(_reader);
		this.playerStorageTracker.Read(_reader);
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0003869C File Offset: 0x0003689C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerSave()
	{
		if (this.saveThread == null || this.saveThread.HasTerminated())
		{
			this.Save();
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x000386BC File Offset: 0x000368BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && (this.saveThread == null || !ThreadManager.ActiveThreads.ContainsKey("blockLimitSaveData")))
		{
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.saveThread = ThreadManager.StartThread("blockLimitSaveData", null, new ThreadManager.ThreadFunctionLoopDelegate(this.SaveThread), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x00038754 File Offset: 0x00036954
	[PublicizedFrom(EAccessModifier.Private)]
	public int SaveThread(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}.dat.bak", GameIO.GetSaveGameDir(), "blockLimits"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x000387C0 File Offset: 0x000369C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _writer)
	{
		_writer.Write(1);
		this.poweredBlockTracker.Write(_writer);
		this.playerStorageTracker.Write(_writer);
	}

	// Token: 0x04000921 RID: 2337
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 1;

	// Token: 0x04000922 RID: 2338
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxPowerBlocks = 10000;

	// Token: 0x04000923 RID: 2339
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxPlayerStorageBlocks = 10000;

	// Token: 0x04000924 RID: 2340
	public static BlockLimitTracker instance;

	// Token: 0x04000925 RID: 2341
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTracker poweredBlockTracker;

	// Token: 0x04000926 RID: 2342
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTracker playerStorageTracker;

	// Token: 0x04000927 RID: 2343
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo saveThread;

	// Token: 0x04000928 RID: 2344
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> clientAmounts;

	// Token: 0x04000929 RID: 2345
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cNameKey = "blockLimits";

	// Token: 0x0400092A RID: 2346
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cThreadKey = "blockLimitSaveData";
}
