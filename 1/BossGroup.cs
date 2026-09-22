using System;
using System.Collections.Generic;
using Audio;
using GameEvent.SequenceActions;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public class BossGroup
{
	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x06002BE4 RID: 11236 RVA: 0x00115008 File Offset: 0x00113208
	public string GetBossNavClass
	{
		get
		{
			BossGroup.BossGroupTypes currentGroupType = this.CurrentGroupType;
			if (currentGroupType == BossGroup.BossGroupTypes.ImmortalBoss)
			{
				return "twitch_vote_boss_shield";
			}
			if (currentGroupType != BossGroup.BossGroupTypes.Specialized)
			{
				return "twitch_vote_boss";
			}
			return "";
		}
	}

	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x06002BE5 RID: 11237 RVA: 0x00115038 File Offset: 0x00113238
	public string GetMinionNavClass
	{
		get
		{
			BossGroup.BossGroupTypes currentGroupType = this.CurrentGroupType;
			if (currentGroupType == BossGroup.BossGroupTypes.ImmortalMinions)
			{
				return "twitch_vote_minion_shield";
			}
			if (currentGroupType != BossGroup.BossGroupTypes.Specialized)
			{
				return "twitch_vote_minion";
			}
			return "";
		}
	}

	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x06002BE6 RID: 11238 RVA: 0x00115067 File Offset: 0x00113267
	public int MinionCount
	{
		get
		{
			if (this.MinionEntityIDs != null)
			{
				return this.MinionEntityIDs.Count;
			}
			return 0;
		}
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x00115080 File Offset: 0x00113280
	public BossGroup(EntityPlayer target, EntityAlive boss, List<EntityAlive> minions, BossGroup.BossGroupTypes bossGroupType, string bossIcon)
	{
		this.CurrentGroupType = bossGroupType;
		this.TargetPlayer = target;
		this.BossEntity = boss;
		this.MinionEntities = minions;
		this.BossName = Localization.Get(EntityClass.list[this.BossEntity.entityClass].entityClassName, false, null);
		this.BossEntityID = boss.entityId;
		this.MinionEntityIDs = new List<int>();
		for (int i = 0; i < minions.Count; i++)
		{
			this.MinionEntityIDs.Add(minions[i].entityId);
		}
		this.BossIcon = bossIcon;
		this.BossGroupID = ++BossGroup.nextID;
		this.serverBounds.size = this.LeavingSize;
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x001151BC File Offset: 0x001133BC
	public BossGroup(int bossGroupID, BossGroup.BossGroupTypes bossGroupType, int bossEntityID, List<int> minionIDs, string bossIcon)
	{
		this.CurrentGroupType = bossGroupType;
		this.BossEntityID = bossEntityID;
		this.MinionEntityIDs = minionIDs;
		this.BossEntity = null;
		this.MinionEntities = null;
		this.BossIcon = bossIcon;
		this.BossGroupID = bossGroupID;
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x0011527C File Offset: 0x0011347C
	public void Update(EntityPlayerLocal player)
	{
		float num = -1f;
		EntityAlive entityAlive = null;
		if (this.BossEntity == null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				this.BossEntity = (GameManager.Instance.World.GetEntity(this.BossEntityID) as EntityAlive);
				if (this.BossEntity != null)
				{
					if (this.BossName == "")
					{
						this.BossName = Localization.Get(EntityClass.list[this.BossEntity.entityClass].entityClassName, false, null);
					}
					if (this.BossEntity.IsAlive())
					{
						entityAlive = this.BossEntity;
					}
				}
			}
		}
		else if (this.BossEntity.IsAlive())
		{
			entityAlive = this.BossEntity;
		}
		if (entityAlive != null)
		{
			num = entityAlive.GetDistance(player);
		}
		if (this.MinionEntities != null)
		{
			for (int i = 0; i < this.MinionEntities.Count; i++)
			{
				if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
				{
					float distance = this.MinionEntities[i].GetDistance(player);
					if (num == -1f || distance < num)
					{
						entityAlive = this.MinionEntities[i];
						num = distance;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < this.MinionEntityIDs.Count; j++)
			{
				EntityAlive entityAlive2 = GameManager.Instance.World.GetEntity(this.MinionEntityIDs[j]) as EntityAlive;
				if (entityAlive2 != null && entityAlive2.IsAlive())
				{
					float distance2 = entityAlive2.GetDistance(player);
					if (num == -1f || distance2 < num)
					{
						entityAlive = entityAlive2;
						num = distance2;
					}
				}
			}
		}
		if (entityAlive == null)
		{
			this.ReadyForRemove = true;
			return;
		}
		this.ReadyForRemove = false;
		this.bounds.center = entityAlive.position;
		this.bounds.size = (this.IsCurrent ? this.LeavingSize : this.EnteringSize);
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x0011547F File Offset: 0x0011367F
	public bool IsPlayerWithinRange(EntityPlayer player)
	{
		return this.bounds.Contains(player.position);
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x00115492 File Offset: 0x00113692
	public bool IsPlayerWithinServerRange(EntityPlayer player)
	{
		return this.serverBounds.Contains(player.position);
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x001154A8 File Offset: 0x001136A8
	public void RemoveMinion(int entityID)
	{
		if (this.MinionEntityIDs != null)
		{
			this.MinionEntityIDs.Remove(entityID);
		}
		if (this.MinionEntities != null)
		{
			for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
			{
				if (this.MinionEntities[i] != null && this.MinionEntities[i].entityId == entityID)
				{
					this.MinionEntities.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x00115520 File Offset: 0x00113720
	public void AddNavObjects()
	{
		if (this.MinionEntities == null)
		{
			this.MinionEntities = new List<EntityAlive>();
			for (int i = 0; i < this.MinionEntityIDs.Count; i++)
			{
				this.MinionEntities.Add(GameManager.Instance.World.GetEntity(this.MinionEntityIDs[i]) as EntityAlive);
			}
		}
		if (this.BossEntity != null)
		{
			this.BossEntity.Buffs.AddBuff("twitch_give_navobject", -1, true, false, -1f);
		}
		if (this.MinionEntities != null)
		{
			for (int j = 0; j < this.MinionEntities.Count; j++)
			{
				if (this.MinionEntities[j] != null)
				{
					this.MinionEntities[j].Buffs.AddBuff("twitch_give_navobject", -1, true, false, -1f);
				}
			}
		}
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x00115603 File Offset: 0x00113803
	public void RequestStatRefresh()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RequestStats, this.BossGroupID), false);
		}
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x00115630 File Offset: 0x00113830
	public void RefreshStats(int playerID)
	{
		if (this.MinionEntities == null)
		{
			this.MinionEntities = new List<EntityAlive>();
			for (int i = 0; i < this.MinionEntityIDs.Count; i++)
			{
				this.MinionEntities.Add(GameManager.Instance.World.GetEntity(this.MinionEntityIDs[i]) as EntityAlive);
			}
		}
		if (this.BossEntity != null)
		{
			this.BossEntity.bPlayerStatsChanged = true;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.BossEntity, playerID, NetPackageEntityStatChanged.EnumStat.Health), false, -1, -1, -1, null, 192, false);
		}
		if (this.MinionEntities != null)
		{
			for (int j = 0; j < this.MinionEntities.Count; j++)
			{
				if (this.MinionEntities[j] != null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.MinionEntities[j], playerID, NetPackageEntityStatChanged.EnumStat.Health), false, -1, -1, -1, null, 192, false);
				}
			}
		}
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x00115744 File Offset: 0x00113944
	public void RemoveNavObjects()
	{
		if (this.BossEntity != null)
		{
			this.BossEntity.RemoveNavObject("twitch_vote_boss");
			this.BossEntity.RemoveNavObject("twitch_vote_boss_shield");
		}
		if (this.MinionEntities != null)
		{
			for (int i = 0; i < this.MinionEntities.Count; i++)
			{
				if (this.MinionEntities[i] != null)
				{
					this.MinionEntities[i].RemoveNavObject("twitch_vote_minion");
					this.MinionEntities[i].RemoveNavObject("twitch_vote_minion_shield");
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				this.MinionEntities = null;
			}
		}
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x001157F0 File Offset: 0x001139F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupTeleportList()
	{
		this.ClosestEnemy = this.GetClosestEntity(this.TargetPlayer);
		if (this.ClosestEnemy == null)
		{
			return;
		}
		this.serverBounds.center = this.ClosestEnemy.position;
		for (int i = 0; i < this.MinionEntities.Count; i++)
		{
			if (!(this.MinionEntities[i] == null) && this.MinionEntities[i].IsAlive() && !(this.MinionEntities[i] == this.ClosestEnemy))
			{
				EntityAlive entityAlive = this.MinionEntities[i];
				if (Vector3.Distance(this.ClosestEnemy.position, this.MinionEntities[i].position) > BossGroup.autoPullDistance)
				{
					if (!this.TeleportList.Contains(this.MinionEntities[i]))
					{
						this.TeleportList.Add(this.MinionEntities[i]);
					}
				}
				else if (this.TeleportList.Contains(this.MinionEntities[i]))
				{
					this.TeleportList.Remove(this.MinionEntities[i]);
				}
			}
		}
		if (this.ClosestEnemy == this.BossEntity || this.BossEntity == null || !this.BossEntity.IsAlive())
		{
			return;
		}
		if (Vector3.Distance(this.ClosestEnemy.position, this.BossEntity.position) > BossGroup.autoPullDistance)
		{
			if (!this.TeleportList.Contains(this.BossEntity))
			{
				this.TeleportList.Add(this.BossEntity);
				return;
			}
		}
		else if (this.TeleportList.Contains(this.BossEntity))
		{
			this.TeleportList.Remove(this.BossEntity);
		}
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x001159CC File Offset: 0x00113BCC
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive GetClosestEntity(EntityPlayer player)
	{
		EntityAlive result = null;
		float num = -1f;
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
			{
				float num2 = Vector3.Distance(this.TargetPlayer.position, this.MinionEntities[i].position);
				if (num > num2 || num == -1f)
				{
					num = num2;
					result = this.MinionEntities[i];
				}
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			float num2 = Vector3.Distance(this.TargetPlayer.position, this.BossEntity.position);
			if (num > num2 || num == -1f)
			{
				result = this.BossEntity;
			}
		}
		return result;
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x00115AB0 File Offset: 0x00113CB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleTeleportList()
	{
		for (int i = this.TeleportList.Count - 1; i >= 0; i--)
		{
			EntityAlive entityAlive = this.TeleportList[i];
			Vector3 zero = Vector3.zero;
			if (ActionBaseSpawn.FindValidPosition(out zero, this.ClosestEnemy.position, 3f, 6f, true, 0f, false, 0f))
			{
				if (this.pullSound != "")
				{
					Manager.BroadcastPlayByLocalPlayer(entityAlive.position, this.pullSound);
				}
				entityAlive.SetPosition(zero, true);
				entityAlive.SetAttackTarget(this.TargetPlayer, 12000);
				this.TeleportList.RemoveAt(i);
				if (this.pullSound != "")
				{
					Manager.BroadcastPlayByLocalPlayer(zero, this.pullSound);
				}
			}
		}
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x00115B7E File Offset: 0x00113D7E
	public void HandleAutoPull()
	{
		if (this.TeleportList.Count > 0)
		{
			this.HandleTeleportList();
		}
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x00115B94 File Offset: 0x00113D94
	public void HandleLiveHandling()
	{
		this.liveTime += Time.deltaTime;
		this.attackTime -= Time.deltaTime;
		if (this.liveTime > 5f && !this.IsPlayerWithinServerRange(this.TargetPlayer))
		{
			this.RemoveNavObjects();
			this.DespawnAll();
		}
		if (this.attackTime <= 0f)
		{
			this.HandleAttackTrigger();
			this.attackTime = 5f;
		}
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x00115C0C File Offset: 0x00113E0C
	public bool ServerUpdate()
	{
		bool flag = false;
		this.SetupTeleportList();
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] == null || !this.MinionEntities[i].IsAlive())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RemoveMinion, this.BossGroupID, this.MinionEntities[i].entityId), false, -1, -1, -1, null, 192, false);
				this.MinionEntityIDs.Remove(this.MinionEntities[i].entityId);
				this.MinionEntities.RemoveAt(i);
			}
			else
			{
				flag = true;
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			flag = true;
		}
		if (!flag)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RemoveGroup, this.BossGroupID), false, -1, -1, -1, null, 192, false);
		}
		return !flag;
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x00115D24 File Offset: 0x00113F24
	public void DespawnAll()
	{
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
			{
				this.MinionEntities[i].ForceDespawn();
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			this.BossEntity.ForceDespawn();
		}
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x00115DA8 File Offset: 0x00113FA8
	public void HandleAttackTrigger()
	{
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive() && this.MinionEntities[i].GetAttackTarget() == null)
			{
				this.MinionEntities[i].SetAttackTarget(this.TargetPlayer, 60000);
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive() && this.BossEntity.GetAttackTarget() == null)
		{
			this.BossEntity.SetAttackTarget(this.TargetPlayer, 60000);
		}
	}

	// Token: 0x0400216C RID: 8556
	public int BossGroupID = -1;

	// Token: 0x0400216D RID: 8557
	public int BossEntityID = -1;

	// Token: 0x0400216E RID: 8558
	public EntityAlive BossEntity;

	// Token: 0x0400216F RID: 8559
	public List<int> MinionEntityIDs;

	// Token: 0x04002170 RID: 8560
	public List<EntityAlive> MinionEntities;

	// Token: 0x04002171 RID: 8561
	public EntityPlayer TargetPlayer;

	// Token: 0x04002172 RID: 8562
	public BossGroup.BossGroupTypes CurrentGroupType;

	// Token: 0x04002173 RID: 8563
	public string BossIcon = "";

	// Token: 0x04002174 RID: 8564
	public string BossName = "";

	// Token: 0x04002175 RID: 8565
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds serverBounds;

	// Token: 0x04002176 RID: 8566
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds bounds;

	// Token: 0x04002177 RID: 8567
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 EnteringSize = new Vector3(32f, 32f, 32f);

	// Token: 0x04002178 RID: 8568
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 LeavingSize = new Vector3(200f, 200f, 200f);

	// Token: 0x04002179 RID: 8569
	[PublicizedFrom(EAccessModifier.Private)]
	public static float autoPullDistance = 32f;

	// Token: 0x0400217A RID: 8570
	[PublicizedFrom(EAccessModifier.Private)]
	public static int nextID = -1;

	// Token: 0x0400217B RID: 8571
	public bool IsCurrent;

	// Token: 0x0400217C RID: 8572
	public bool ReadyForRemove;

	// Token: 0x0400217D RID: 8573
	public string pullSound = "twitch_pull";

	// Token: 0x0400217E RID: 8574
	[PublicizedFrom(EAccessModifier.Private)]
	public float liveTime;

	// Token: 0x0400217F RID: 8575
	[PublicizedFrom(EAccessModifier.Private)]
	public float attackTime = 5f;

	// Token: 0x04002180 RID: 8576
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive ClosestEnemy;

	// Token: 0x04002181 RID: 8577
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> TeleportList = new List<EntityAlive>();

	// Token: 0x02000535 RID: 1333
	public enum BossGroupTypes
	{
		// Token: 0x04002183 RID: 8579
		Standard,
		// Token: 0x04002184 RID: 8580
		ImmortalBoss,
		// Token: 0x04002185 RID: 8581
		ImmortalMinions,
		// Token: 0x04002186 RID: 8582
		Specialized
	}
}
