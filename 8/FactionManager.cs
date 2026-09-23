using System;
using System.IO;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class FactionManager
{
	// Token: 0x06002B37 RID: 11063 RVA: 0x00111E94 File Offset: 0x00110094
	public void PrintData()
	{
		for (int i = 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i] != null)
			{
				Log.Out(this.Factions[i].ToString());
			}
		}
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x00111ED0 File Offset: 0x001100D0
	public static void Init()
	{
		FactionManager.Instance = new FactionManager();
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x00111EDC File Offset: 0x001100DC
	public void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || GameManager.Instance.World == null || GameManager.Instance.World.Players == null || GameManager.Instance.World.Players.Count == 0 || GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		this.saveTime -= Time.deltaTime;
		if (this.saveTime <= 0f && (this.dataSaveThreadInfo == null || this.dataSaveThreadInfo.HasTerminated()))
		{
			this.saveTime = 60f;
			this.Save();
		}
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x00111F80 File Offset: 0x00110180
	public FactionManager.Relationship GetRelationshipTier(EntityAlive checkingEntity, EntityAlive targetEntity)
	{
		if (checkingEntity == null || targetEntity == null)
		{
			return FactionManager.Relationship.Neutral;
		}
		this.rel = this.GetRelationshipValue(checkingEntity, targetEntity);
		if (this.rel < 200f)
		{
			return FactionManager.Relationship.Hate;
		}
		if (this.rel < 400f)
		{
			return FactionManager.Relationship.Dislike;
		}
		if (this.rel < 600f)
		{
			return FactionManager.Relationship.Neutral;
		}
		if (this.rel < 800f)
		{
			return FactionManager.Relationship.Like;
		}
		if (this.rel < 1001f)
		{
			return FactionManager.Relationship.Love;
		}
		return FactionManager.Relationship.Leader;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x00112014 File Offset: 0x00110214
	public Faction CreateFaction(string _name = "", bool _playerFaction = true, string _icon = "")
	{
		Faction faction = new Faction(_name, _playerFaction, _icon);
		this.AddFaction(faction);
		return faction;
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x00112034 File Offset: 0x00110234
	public void AddFaction(Faction _faction)
	{
		for (int i = _faction.IsPlayerFaction ? 8 : 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i] == null)
			{
				this.Factions[i] = _faction;
				_faction.ID = (byte)i;
				return;
			}
		}
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x0011207B File Offset: 0x0011027B
	public void RemoveFaction(byte _id)
	{
		this.Factions[(int)_id] = null;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x00112086 File Offset: 0x00110286
	public Faction GetFaction(byte _id)
	{
		return this.Factions[(int)_id];
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x00112090 File Offset: 0x00110290
	public Faction GetFactionByName(string _name)
	{
		for (int i = 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i].Name == _name)
			{
				return this.Factions[i];
			}
		}
		return null;
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x001120D0 File Offset: 0x001102D0
	public float GetRelationshipValue(EntityAlive checkingEntity, EntityAlive targetEntity)
	{
		if (checkingEntity == null || targetEntity == null)
		{
			return 400f;
		}
		if (checkingEntity.factionId == targetEntity.factionId)
		{
			return 800f;
		}
		if (this.Factions[(int)checkingEntity.factionId] != null && this.Factions[(int)targetEntity.factionId] != null)
		{
			return this.Factions[(int)checkingEntity.factionId].GetRelationship(targetEntity.factionId);
		}
		return 400f;
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x00112145 File Offset: 0x00110345
	public void SetRelationship(byte _myFaction, byte _targetFaction, sbyte _modification)
	{
		if (this.Factions[(int)_myFaction] != null)
		{
			this.Factions[(int)_myFaction].ModifyRelationship(_targetFaction, (float)_modification);
		}
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x00112145 File Offset: 0x00110345
	public void ModifyRelationship(byte _myFaction, byte _targetFaction, sbyte _modification)
	{
		if (this.Factions[(int)_myFaction] != null)
		{
			this.Factions[(int)_myFaction].ModifyRelationship(_targetFaction, (float)_modification);
		}
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x00112164 File Offset: 0x00110364
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(FactionManager.Version);
		for (int i = 0; i < this.Factions.Length; i++)
		{
			_bw.Write(this.Factions[i] != null);
			if (this.Factions[i] != null)
			{
				this.Factions[i].Write(_bw);
			}
		}
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x001121B8 File Offset: 0x001103B8
	public void Read(BinaryReader _br)
	{
		_br.ReadByte();
		for (int i = 0; i < 255; i++)
		{
			if (_br.ReadBoolean())
			{
				this.Factions[i] = new Faction();
				this.Factions[i].Read(_br);
			}
		}
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x00112200 File Offset: 0x00110400
	[PublicizedFrom(EAccessModifier.Private)]
	public int saveFactionDataThreaded(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat");
		if (!SdDirectory.Exists(GameIO.GetSaveGameDir()))
		{
			return -1;
		}
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x0011227C File Offset: 0x0011047C
	public void Save()
	{
		if (this.dataSaveThreadInfo == null || !ThreadManager.ActiveThreads.ContainsKey("factionDataSave"))
		{
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.Write(pooledBinaryWriter);
			}
			this.dataSaveThreadInfo = ThreadManager.StartThread("factionDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.saveFactionDataThreaded), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x00112308 File Offset: 0x00110508
	public void Load()
	{
		string path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat");
		if (SdDirectory.Exists(GameIO.GetSaveGameDir()) && SdFile.Exists(path))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(path))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.Read(pooledBinaryReader);
					}
				}
			}
			catch (Exception)
			{
				path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat.bak");
				if (SdFile.Exists(path))
				{
					using (Stream stream2 = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.Read(pooledBinaryReader2);
						}
					}
				}
			}
		}
	}

	// Token: 0x0400210C RID: 8460
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SAVE_TIME_SEC = 60f;

	// Token: 0x0400210D RID: 8461
	public static FactionManager Instance;

	// Token: 0x0400210E RID: 8462
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte Version = 1;

	// Token: 0x0400210F RID: 8463
	[PublicizedFrom(EAccessModifier.Private)]
	public Faction[] Factions = new Faction[255];

	// Token: 0x04002110 RID: 8464
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 60f;

	// Token: 0x04002111 RID: 8465
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo dataSaveThreadInfo;

	// Token: 0x04002112 RID: 8466
	[PublicizedFrom(EAccessModifier.Private)]
	public float rel;

	// Token: 0x02000521 RID: 1313
	public enum Relationship
	{
		// Token: 0x04002114 RID: 8468
		Hate,
		// Token: 0x04002115 RID: 8469
		Dislike = 200,
		// Token: 0x04002116 RID: 8470
		Neutral = 400,
		// Token: 0x04002117 RID: 8471
		Like = 600,
		// Token: 0x04002118 RID: 8472
		Love = 800,
		// Token: 0x04002119 RID: 8473
		Leader = 1001
	}
}
