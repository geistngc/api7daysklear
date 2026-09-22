using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x02000414 RID: 1044
[Preserve]
public class AIDirectorGameStagePartySpawner
{
	// Token: 0x06002042 RID: 8258 RVA: 0x000C31E4 File Offset: 0x000C13E4
	public AIDirectorGameStagePartySpawner(World _world, string _gameStageName)
	{
		this.world = _world;
		this.def = GameStageDefinition.GetGameStage(_gameStageName);
		this.partyMembers = new ReadOnlyCollection<EntityPlayer>(this.members);
		this.partyLevel = -1;
		this.gsScaling = 1f;
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x000C3243 File Offset: 0x000C1443
	public void SetScaling(float _scaling)
	{
		this.gsScaling = Utils.FastLerp(1f, 2.5f, (_scaling - 1f) / 3f);
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x000C3268 File Offset: 0x000C1468
	public void ResetPartyLevel(int mod = 0)
	{
		int num = this.CalcPartyLevel();
		if (mod != 0)
		{
			num %= mod;
		}
		this.SetPartyLevel(num);
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x000C328C File Offset: 0x000C148C
	public int CalcPartyLevel()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.members.Count; i++)
		{
			EntityPlayer entityPlayer = this.members[i];
			list.Add(entityPlayer.gameStage);
		}
		return GameStageDefinition.CalcPartyLevel(list);
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000C32D4 File Offset: 0x000C14D4
	public void SetPartyLevel(int _partyLevel)
	{
		this.partyLevel = _partyLevel;
		this.partyLevel = (int)((float)this.partyLevel * this.gsScaling);
		this.stageSpawnMax = 0;
		this.groupIndex = 0;
		this.spawnCount = 0;
		if (this.def != null)
		{
			this.stage = this.def.GetStage(_partyLevel);
			if (this.stage != null)
			{
				this.stageSpawnMax = this.CalcStageSpawnMax();
				this.SetupGroup();
			}
		}
		this.bonusLootEvery = Utils.FastMax(this.stageSpawnMax / GameStageDefinition.LootBonusMaxCount, GameStageDefinition.LootBonusEvery);
		Log.Out("Party of {0}, GS {1} ({2}), scaling {3}, enemy max {4}, bonus every {5}", new object[]
		{
			this.members.Count,
			this.partyLevel,
			_partyLevel,
			this.gsScaling,
			this.stageSpawnMax,
			this.bonusLootEvery
		});
		Log.Out("Party members: ");
		for (int i = 0; i < this.members.Count; i++)
		{
			EntityPlayer entityPlayer = this.members[i];
			Log.Out("Player id {0}, gameStage {1}", new object[]
			{
				entityPlayer.entityId,
				entityPlayer.gameStage
			});
		}
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x000C3420 File Offset: 0x000C1620
	public bool Tick(double _deltaTime)
	{
		if (this.spawnGroup != null)
		{
			bool flag = false;
			if (this.nextStageTime > 0UL && this.world.worldTime >= this.nextStageTime)
			{
				flag = true;
			}
			else if (this.spawnCount >= this.numToSpawn)
			{
				this.interval -= _deltaTime;
				flag = (this.interval <= 0.0);
			}
			if (flag)
			{
				this.groupIndex++;
				this.SetupGroup();
			}
		}
		return this.spawnGroup != null;
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000C34AC File Offset: 0x000C16AC
	public void AddMember(EntityPlayer _player)
	{
		if (!this.memberIDs.Contains(_player.entityId))
		{
			this.memberIDs.Add(_player.entityId);
		}
		if (!this.members.Contains(_player))
		{
			this.members.Add(_player);
		}
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000C34F8 File Offset: 0x000C16F8
	public bool IsMemberOfParty(int _entityID)
	{
		return this.memberIDs.Contains(_entityID);
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000C3506 File Offset: 0x000C1706
	public void RemoveMember(EntityPlayer _player, bool removeID)
	{
		this.members.Remove(_player);
		if (removeID)
		{
			this.memberIDs.Remove(_player.entityId);
		}
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000C352C File Offset: 0x000C172C
	[PublicizedFrom(EAccessModifier.Private)]
	public int CalcStageSpawnMax()
	{
		int num = 0;
		int count = this.stage.Count;
		for (int i = 0; i < count; i++)
		{
			this.spawnGroup = this.stage.GetSpawnGroup(i);
			num += (int)this.spawnGroup.spawnCount;
		}
		return num;
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x000C3574 File Offset: 0x000C1774
	public void ClearMembers()
	{
		this.members.Clear();
		this.memberIDs.Clear();
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000C358C File Offset: 0x000C178C
	public void IncSpawnCount()
	{
		this.spawnCount++;
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000C359C File Offset: 0x000C179C
	public void DecSpawnCount(int dec)
	{
		if (dec > this.spawnCount)
		{
			this.spawnCount = 0;
			return;
		}
		this.spawnCount -= dec;
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x0600204F RID: 8271 RVA: 0x000C35BD File Offset: 0x000C17BD
	public bool IsDone
	{
		get
		{
			return this.groupIndex > 0 && this.spawnGroup == null;
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x06002050 RID: 8272 RVA: 0x000C35D3 File Offset: 0x000C17D3
	public bool canSpawn
	{
		get
		{
			return this.spawnGroup != null && this.spawnCount < this.numToSpawn;
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x06002051 RID: 8273 RVA: 0x000C35ED File Offset: 0x000C17ED
	public int maxAlive
	{
		get
		{
			if (this.spawnGroup == null)
			{
				return 0;
			}
			return (int)this.spawnGroup.maxAlive;
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002052 RID: 8274 RVA: 0x000C3604 File Offset: 0x000C1804
	public string spawnGroupName
	{
		get
		{
			if (this.spawnGroup == null)
			{
				return null;
			}
			return this.spawnGroup.groupName;
		}
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000C361C File Offset: 0x000C181C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupGroup()
	{
		this.spawnGroup = this.stage.GetSpawnGroup(this.groupIndex);
		if (this.spawnGroup != null)
		{
			this.interval = (double)this.spawnGroup.interval;
			this.nextStageTime = ((this.spawnGroup.duration > 0) ? (this.world.worldTime + (ulong)(this.spawnGroup.duration * 1000)) : 0UL);
			this.numToSpawn = EntitySpawner.ModifySpawnCountByGameDifficulty((int)this.spawnGroup.spawnCount);
			this.spawnCount = 0;
			return;
		}
		Log.Out("AIDirectorGameStagePartySpawner: groups done ({0})", new object[]
		{
			this.groupIndex
		});
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000C36CC File Offset: 0x000C18CC
	public override string ToString()
	{
		return string.Format("{0} {1} (count {2}, numToSpawn {3}, maxAlive {4})", new object[]
		{
			this.groupIndex,
			this.spawnGroupName,
			this.spawnCount,
			this.numToSpawn,
			this.maxAlive
		});
	}

	// Token: 0x0400160F RID: 5647
	public ReadOnlyCollection<EntityPlayer> partyMembers;

	// Token: 0x04001610 RID: 5648
	public float gsScaling;

	// Token: 0x04001611 RID: 5649
	public int groupIndex;

	// Token: 0x04001612 RID: 5650
	public int partyLevel;

	// Token: 0x04001613 RID: 5651
	public int stageSpawnMax;

	// Token: 0x04001614 RID: 5652
	public int bonusLootEvery;

	// Token: 0x04001615 RID: 5653
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition def;

	// Token: 0x04001616 RID: 5654
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition.Stage stage;

	// Token: 0x04001617 RID: 5655
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition.SpawnGroup spawnGroup;

	// Token: 0x04001618 RID: 5656
	[PublicizedFrom(EAccessModifier.Private)]
	public int spawnCount;

	// Token: 0x04001619 RID: 5657
	[PublicizedFrom(EAccessModifier.Private)]
	public int numToSpawn;

	// Token: 0x0400161A RID: 5658
	[PublicizedFrom(EAccessModifier.Private)]
	public double interval;

	// Token: 0x0400161B RID: 5659
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong nextStageTime;

	// Token: 0x0400161C RID: 5660
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x0400161D RID: 5661
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> memberIDs = new HashSet<int>();

	// Token: 0x0400161E RID: 5662
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> members = new List<EntityPlayer>();
}
