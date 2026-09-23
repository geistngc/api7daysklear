using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;
using SandboxOptions;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004C5 RID: 1221
[Preserve]
public class EntityPlayer : EntityAlive
{
	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x060026A2 RID: 9890 RVA: 0x000ECA27 File Offset: 0x000EAC27
	public PersistentPlayerData PersistentPlayerData
	{
		get
		{
			PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
			if (persistentPlayers == null)
			{
				return null;
			}
			return persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x060026A3 RID: 9891 RVA: 0x000ECA44 File Offset: 0x000EAC44
	public string PlayerDisplayName
	{
		get
		{
			if (this.cachedPlayerName != null)
			{
				return this.cachedPlayerName.DisplayName;
			}
			if (this.PersistentPlayerData == null)
			{
				return null;
			}
			this.cachedPlayerName = this.PersistentPlayerData.PlayerName;
			return this.cachedPlayerName.DisplayName;
		}
	}

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x060026A4 RID: 9892 RVA: 0x000ECA80 File Offset: 0x000EAC80
	// (remove) Token: 0x060026A5 RID: 9893 RVA: 0x000ECAB8 File Offset: 0x000EACB8
	public event EntityPlayer.OnPlayerTeleportDelegate PlayerTeleportedDelegates;

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x060026A6 RID: 9894 RVA: 0x000ECAED File Offset: 0x000EACED
	// (set) Token: 0x060026A7 RID: 9895 RVA: 0x000ECAF5 File Offset: 0x000EACF5
	public int CarryCapacity { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x060026A8 RID: 9896 RVA: 0x000ECAFE File Offset: 0x000EACFE
	// (set) Token: 0x060026A9 RID: 9897 RVA: 0x000ECB08 File Offset: 0x000EAD08
	public bool TwitchEnabled
	{
		get
		{
			return this.twitchEnabled;
		}
		set
		{
			if (value != this.twitchEnabled)
			{
				this.twitchEnabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
				if (TwitchManager.HasInstance && TwitchManager.Current.extensionManager != null)
				{
					TwitchManager.Current.extensionManager.TwitchEnabledChanged(this);
				}
			}
		}
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x060026AA RID: 9898 RVA: 0x000ECB5E File Offset: 0x000EAD5E
	// (set) Token: 0x060026AB RID: 9899 RVA: 0x000ECB68 File Offset: 0x000EAD68
	public bool TwitchSafe
	{
		get
		{
			return this.twitchSafe;
		}
		set
		{
			if (value != this.twitchSafe)
			{
				this.twitchSafe = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
				if (this.twitchSafe)
				{
					this.Buffs.AddBuff("twitch_safe", -1, true, false, -1f);
					return;
				}
				this.Buffs.RemoveBuff("twitch_safe", -1, true);
			}
		}
	}

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x060026AC RID: 9900 RVA: 0x000ECBCF File Offset: 0x000EADCF
	// (set) Token: 0x060026AD RID: 9901 RVA: 0x000ECBD7 File Offset: 0x000EADD7
	public TwitchVoteLockTypes TwitchVoteLock
	{
		get
		{
			return this.twitchVoteLock;
		}
		set
		{
			if (value != this.twitchVoteLock)
			{
				this.twitchVoteLock = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x060026AE RID: 9902 RVA: 0x000ECBFF File Offset: 0x000EADFF
	// (set) Token: 0x060026AF RID: 9903 RVA: 0x000ECC07 File Offset: 0x000EAE07
	public bool TwitchVisionDisabled
	{
		get
		{
			return this.twitchVisionDisabled;
		}
		set
		{
			if (value != this.twitchVisionDisabled)
			{
				this.twitchVisionDisabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x060026B0 RID: 9904 RVA: 0x000ECC2F File Offset: 0x000EAE2F
	// (set) Token: 0x060026B1 RID: 9905 RVA: 0x000ECC37 File Offset: 0x000EAE37
	public EntityPlayer.TwitchActionsStates TwitchActionsEnabled
	{
		get
		{
			return this.twitchActionsEnabled;
		}
		set
		{
			if (value != this.twitchActionsEnabled)
			{
				this.twitchActionsEnabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x060026B2 RID: 9906 RVA: 0x000ECC5F File Offset: 0x000EAE5F
	// (set) Token: 0x060026B3 RID: 9907 RVA: 0x000ECC67 File Offset: 0x000EAE67
	public bool IsSpectator
	{
		get
		{
			return this.isSpectator;
		}
		set
		{
			this.isSpectator = value;
			this.isIgnoredByAI = this.isSpectator;
			this.SetVisible(this.bModelVisible);
			this.bPlayerStatsChanged |= !this.isEntityRemote;
		}
	}

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x060026B4 RID: 9908 RVA: 0x000ECCA0 File Offset: 0x000EAEA0
	public bool IsFriendOfLocalPlayer
	{
		get
		{
			PersistentPlayerData persistentLocalPlayer = GameManager.Instance.persistentLocalPlayer;
			if (persistentLocalPlayer == null)
			{
				return false;
			}
			PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
			return playerDataFromEntityID != null && persistentLocalPlayer.IsAlly(playerDataFromEntityID);
		}
	}

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x060026B5 RID: 9909 RVA: 0x000ECCDF File Offset: 0x000EAEDF
	// (set) Token: 0x060026B6 RID: 9910 RVA: 0x000ECCE8 File Offset: 0x000EAEE8
	public Vector3i markerPosition
	{
		get
		{
			return this.m_MarkerPosition;
		}
		set
		{
			if (!this.isEntityRemote)
			{
				if (value.Equals(Vector3i.zero))
				{
					if (this.navMarker != null)
					{
						NavObjectManager.Instance.UnRegisterNavObject(this.navMarker);
						this.navMarker = null;
					}
				}
				else if (this.navMarker == null)
				{
					this.navMarker = NavObjectManager.Instance.RegisterNavObject("quick_waypoint", value.ToVector3(), "", this.navMarkerHidden, -1, null);
				}
				else
				{
					this.navMarker.TrackedPosition = value.ToVector3();
					this.navMarker.hiddenOnCompass = this.navMarkerHidden;
				}
				this.m_MarkerPosition = value;
			}
		}
	}

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x060026B7 RID: 9911 RVA: 0x000ECD8C File Offset: 0x000EAF8C
	// (remove) Token: 0x060026B8 RID: 9912 RVA: 0x000ECDC4 File Offset: 0x000EAFC4
	public event QuestJournal_QuestEvent QuestAccepted;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x060026B9 RID: 9913 RVA: 0x000ECDFC File Offset: 0x000EAFFC
	// (remove) Token: 0x060026BA RID: 9914 RVA: 0x000ECE34 File Offset: 0x000EB034
	public event QuestJournal_QuestEvent QuestChanged;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x060026BB RID: 9915 RVA: 0x000ECE6C File Offset: 0x000EB06C
	// (remove) Token: 0x060026BC RID: 9916 RVA: 0x000ECEA4 File Offset: 0x000EB0A4
	public event QuestJournal_QuestEvent QuestRemoved;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x060026BD RID: 9917 RVA: 0x000ECEDC File Offset: 0x000EB0DC
	// (remove) Token: 0x060026BE RID: 9918 RVA: 0x000ECF14 File Offset: 0x000EB114
	public event QuestJournal_QuestSharedEvent SharedQuestAdded;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x060026BF RID: 9919 RVA: 0x000ECF4C File Offset: 0x000EB14C
	// (remove) Token: 0x060026C0 RID: 9920 RVA: 0x000ECF84 File Offset: 0x000EB184
	public event QuestJournal_QuestSharedEvent SharedQuestRemoved;

	// Token: 0x060026C1 RID: 9921 RVA: 0x000ECFB9 File Offset: 0x000EB1B9
	public void TriggerQuestAddedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questAccepted = this.QuestAccepted;
		if (questAccepted == null)
		{
			return;
		}
		questAccepted(_q);
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000ECFCC File Offset: 0x000EB1CC
	public void TriggerQuestChangedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questChanged = this.QuestChanged;
		if (questChanged == null)
		{
			return;
		}
		questChanged(_q);
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000ECFDF File Offset: 0x000EB1DF
	public void TriggerQuestRemovedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questRemoved = this.QuestRemoved;
		if (questRemoved == null)
		{
			return;
		}
		questRemoved(_q);
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000ECFF2 File Offset: 0x000EB1F2
	public void TriggerSharedQuestAddedEvent(SharedQuestEntry _entry)
	{
		if (this.SharedQuestAdded != null)
		{
			this.SharedQuestAdded(_entry);
			return;
		}
		Log.Warning(string.Format("No SharedQuestAdded listeners! Player: {0}", this));
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000ED019 File Offset: 0x000EB219
	public void TriggerSharedQuestRemovedEvent(SharedQuestEntry _entry)
	{
		QuestJournal_QuestSharedEvent sharedQuestRemoved = this.SharedQuestRemoved;
		if (sharedQuestRemoved == null)
		{
			return;
		}
		sharedQuestRemoved(_entry);
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x060026C6 RID: 9926 RVA: 0x000ED02C File Offset: 0x000EB22C
	// (set) Token: 0x060026C7 RID: 9927 RVA: 0x000ED034 File Offset: 0x000EB234
	public Vector3i RentedVMPosition
	{
		get
		{
			return this.m_rentedVMPosition;
		}
		set
		{
			if (!this.isEntityRemote)
			{
				if (value.Equals(Vector3i.zero))
				{
					if (this.navVending != null)
					{
						NavObjectManager.Instance.UnRegisterNavObject(this.navVending);
						this.navVending = null;
					}
				}
				else if (this.navVending == null)
				{
					this.navVending = NavObjectManager.Instance.RegisterNavObject("vending_machine", value.ToVector3(), "", false, -1, null);
				}
				else
				{
					this.navVending.TrackedPosition = value.ToVector3();
				}
				this.m_rentedVMPosition = value;
			}
		}
	}

	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x060026C8 RID: 9928 RVA: 0x000ED0C0 File Offset: 0x000EB2C0
	// (set) Token: 0x060026C9 RID: 9929 RVA: 0x000ED123 File Offset: 0x000EB323
	public bool IsAdmin
	{
		get
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.isAdmin;
			}
			if (!this.isEntityRemote)
			{
				return true;
			}
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(this.entityId);
			AdminTools adminTools = GameManager.Instance.adminTools;
			return ((adminTools != null) ? adminTools.Users.GetUserPermissionLevel(clientInfo) : 1000) == 0;
		}
		set
		{
			if (value != this.isAdmin)
			{
				this.isAdmin = value;
			}
		}
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000ED138 File Offset: 0x000EB338
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.CarryCapacity = (int)EffectManager.GetValue(PassiveEffects.BagSize, null, 45f, this, null, default(FastTags<TagGroup.Global>), false, false, true, true, true, 1, true, false);
		this.bag = new Bag(this.CarryCapacity);
		this.Progression = new Progression(this);
		this.bWillRespawn = true;
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000ED198 File Offset: 0x000EB398
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		this.gameStageBornAtWorldTime = ulong.MaxValue;
		if (this.playerProfile == null)
		{
			this.playerProfile = PlayerProfile.LoadLocalProfile();
		}
		this.Stealth.Init(this);
		this.alertEnabled = false;
		base.Init(_entityClass, _assets, _eModelAssets);
		if (ConsoleCmdCCPhysics.EnableCCPhysicsChanges && !(this is EntityPlayerLocal))
		{
			this.PhysicsTransform.gameObject.layer = 3;
		}
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000ED1FC File Offset: 0x000EB3FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitStats()
	{
		this.entityStats = new PlayerEntityStats(this);
		this.startOfFrameStats = new PlayerEntityStats(this);
	}

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x060026CD RID: 9933 RVA: 0x000ED216 File Offset: 0x000EB416
	public PlayerEntityStats PlayerStats
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (PlayerEntityStats)this.entityStats;
		}
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000ED223 File Offset: 0x000EB423
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000ED22B File Offset: 0x000EB42B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.SetVisible(this.Spawned);
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000ED240 File Offset: 0x000EB440
	public override void SetAlive()
	{
		bool flag = this.IsDead();
		base.SetAlive();
		if (flag)
		{
			long num = GameStageDefinition.DaysAliveChangeWhenKilled * 24000L;
			if (this.world.worldTime - this.gameStageBornAtWorldTime < (ulong)num)
			{
				this.gameStageBornAtWorldTime = this.world.worldTime;
				return;
			}
			this.gameStageBornAtWorldTime += (ulong)num;
		}
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000ED29D File Offset: 0x000EB49D
	public override void SetDead()
	{
		base.SetDead();
		if (this.world.aiDirector != null)
		{
			this.IsBloodMoonDead = this.world.aiDirector.BloodMoonComponent.BloodMoonActive;
		}
	}

	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x060026D2 RID: 9938 RVA: 0x000ED2D0 File Offset: 0x000EB4D0
	public int unModifiedGameStage
	{
		get
		{
			float num = Mathf.Clamp((float)((this.world.worldTime - this.gameStageBornAtWorldTime) / 24000UL), 0f, (float)this.Progression.Level);
			float difficultyBonus = GameStageDefinition.DifficultyBonus;
			return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level + num) * difficultyBonus, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		}
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x060026D3 RID: 9939 RVA: 0x000ED348 File Offset: 0x000EB548
	public int gameStage
	{
		get
		{
			float num = Mathf.Clamp((float)((this.world.worldTime - this.gameStageBornAtWorldTime) / 24000UL), 0f, (float)this.Progression.Level);
			float difficultyBonus = GameStageDefinition.DifficultyBonus;
			if (this.biomeStandingOn != null)
			{
				float num2 = 0f;
				float num3 = 0f;
				if (this.QuestJournal.ActiveQuest != null)
				{
					num2 = this.QuestJournal.ActiveQuest.QuestClass.GameStageMod;
					num3 = this.QuestJournal.ActiveQuest.QuestClass.GameStageBonus;
				}
				float num4 = this.biomeStandingOn.GameStageMod * EntityPlayer.BiomeGameStageModifier;
				float num5 = this.biomeStandingOn.GameStageBonus * EntityPlayer.BiomeGameStageModifier;
				return Utils.FastMax(Utils.Fastfloor(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level * (1f + num4 + num2) + num + num5 + num3) * difficultyBonus, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * EntityPlayer.GlobalGameStageModifier), 1);
			}
			return Utils.FastMax(Utils.Fastfloor(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level + num) * difficultyBonus, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * EntityPlayer.GlobalGameStageModifier), 1);
		}
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000ED494 File Offset: 0x000EB694
	public int GetTraderStage(int tier)
	{
		int a = Mathf.Max(0, tier - 1);
		float num = TraderManager.QuestTierMod[Mathf.Min(a, TraderManager.QuestTierMod.Length - 1)];
		return Utils.FastMax(Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.TraderStage, null, (float)this.Progression.Level * (1f + num), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * EntityPlayer.GlobalTraderStageModifier), 1);
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000ED504 File Offset: 0x000EB704
	public int GetLootStage(float containerMod, float containerBonus)
	{
		float num = 0f;
		float num2 = 0f;
		if (this.prefab != null && this.prefab.prefab.DifficultyTier > 0)
		{
			int a = Mathf.Max(0, (int)(this.prefab.prefab.DifficultyTier - 1));
			num = LootManager.POITierMod[Mathf.Min(a, LootManager.POITierMod.Length - 1)] * EntityPlayer.POITierLootStageModifier;
			num2 = LootManager.POITierBonus[Mathf.Min(a, LootManager.POITierBonus.Length - 1)] * EntityPlayer.POITierLootStageModifier;
		}
		if (this.biomeStandingOn != null)
		{
			float num3 = this.biomeStandingOn.LootStageMod * EntityPlayer.BiomeLootStageModifier;
			float num4 = this.biomeStandingOn.LootStageBonus * EntityPlayer.BiomeLootStageModifier;
			int num5 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStage, null, (float)this.Progression.Level * (1f + num + num3 + containerMod) + (num2 + num4 + containerBonus), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			if (this.biomeStandingOn.LootStageMin != -1)
			{
				num5 = Utils.FastMax(num5, this.biomeStandingOn.LootStageMin);
			}
			if (GameStats.GetBool(EnumGameStats.BiomeProgression) && this.biomeStandingOn.LootStageMax != -1)
			{
				num5 = Utils.FastMin(num5, Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStageMax, null, (float)this.biomeStandingOn.LootStageMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)));
			}
			return Utils.FastMax(Mathf.FloorToInt((float)num5 * EntityPlayer.GlobalLootStageModifier), 1);
		}
		return Utils.FastMax(Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStage, null, (float)this.Progression.Level * (1f + num + containerMod) + (num2 + containerBonus), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * EntityPlayer.GlobalLootStageModifier), 1);
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000ED6CF File Offset: 0x000EB8CF
	public int GetHighestPartyLootStage(float containerMod, float containerBonus)
	{
		if (this.Party != null)
		{
			return this.Party.GetHighestLootStage(containerMod, containerBonus);
		}
		return this.GetLootStage(containerMod, containerBonus);
	}

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x060026D7 RID: 9943 RVA: 0x000ED6EF File Offset: 0x000EB8EF
	public int HighestPartyGameStage
	{
		get
		{
			if (this.Party != null)
			{
				return this.Party.HighestGameStage;
			}
			return this.gameStage;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x060026D8 RID: 9944 RVA: 0x000ED70B File Offset: 0x000EB90B
	public int PartyGameStage
	{
		get
		{
			if (this.Party != null)
			{
				return this.Party.GameStage;
			}
			return this.gameStage;
		}
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000ED727 File Offset: 0x000EB927
	public void TurnOffLightFlares()
	{
		this.inventory.TurnOffLightFlares();
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000EC808 File Offset: 0x000EAA08
	public override float GetSeeDistance()
	{
		return 80f;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000ED734 File Offset: 0x000EB934
	public float DetectUsScale(EntityAlive _entity)
	{
		if (this.prefab != null && this.prefab.prefab.DifficultyTier >= 1 && Time.time - this.prefabTimeIn > 60f && _entity.GetSpawnerSource() == EnumSpawnerSource.Biome && _entity is EntityEnemy)
		{
			return 0.3f;
		}
		return 1f;
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000ED78C File Offset: 0x000EB98C
	public override Vector3 getHeadPosition()
	{
		if (!(this.emodel != null) || !(this.emodel.GetHeadTransform() != null))
		{
			return base.transform.position + new Vector3(0f, base.height - 0.15f, 0f) + Origin.position;
		}
		return this.emodel.GetHeadTransform().position + Origin.position;
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000ED80C File Offset: 0x000EBA0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		this.generalTags = this.MinEventContext.Tags;
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		float num = this.totalTimePlayed + Time.unscaledDeltaTime / 60f;
		if (this is EntityPlayerLocal)
		{
			int num2 = (int)this.totalTimePlayed;
			int num3 = (int)num;
			if (num2 != num3 && num3 % 60 == 0)
			{
				int num4 = num3 / 60;
				if (num4 < 301)
				{
					GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PlayerLevelAtHour, num4.ToString(), this.Progression.Level, true, GameSparksCollector.GSDataCollection.SessionUpdates);
				}
			}
		}
		this.totalTimePlayed = num;
		if (this.ChunkObserver != null)
		{
			this.ChunkObserver.SetPosition(base.GetPosition());
			if (this.ChunkObserver.mapDatabase != null && this.IsSpawned() && this.chunkPosAddedEntityTo != this.lastChunkPos)
			{
				this.lastChunkPos = this.chunkPosAddedEntityTo;
				this.ChunkObserver.mapDatabase.Add(this.chunkPosAddedEntityTo, this.world);
			}
		}
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.SetHeadAngles(this.rotation.x, 0f);
			if (this.inventory.holdingItem != null && this.inventory.holdingItem.CanHold())
			{
				this.emodel.avatarController.SetArmsAngles(this.rotation.x + 90f, 0f);
			}
			else
			{
				this.emodel.avatarController.SetArmsAngles(0f, 0f);
			}
		}
		if (!this.IsDead())
		{
			this.currentLife += Time.deltaTime / 60f;
			if (this.currentLife > this.longestLife)
			{
				this.longestLife = this.currentLife;
				if ((int)this.longestLife > this.longestLifeLived)
				{
					this.longestLifeLived = (int)this.longestLife;
					if (this is EntityPlayerLocal)
					{
						QuestEventManager.Current.TimeSurvived((float)this.longestLifeLived);
						IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
						if (achievementManager != null)
						{
							achievementManager.SetAchievementStat(EnumAchievementDataStat.LongestLifeLived, this.longestLifeLived);
						}
					}
				}
			}
		}
		this.HasUpdated = true;
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000EDA34 File Offset: 0x000EBC34
	public override float GetSpeedModifier()
	{
		float num;
		float num2;
		if (base.IsCrouching)
		{
			if (this.MovementRunning)
			{
				num = Constants.cPlayerSpeedModifierWalking;
				num2 = EffectManager.GetValue(PassiveEffects.WalkSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				num *= EntityPlayer.CrouchRunSpeedModifier;
				num2 *= EntityPlayer.CrouchRunSpeedModifier;
			}
			else
			{
				num = Constants.cPlayerSpeedModifierCrouching;
				num2 = EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				num *= EntityPlayer.CrouchSpeedModifier;
				num2 *= EntityPlayer.CrouchSpeedModifier;
			}
		}
		else if (this.MovementRunning)
		{
			num = Constants.cPlayerSpeedModifierRunning;
			num2 = EffectManager.GetValue(PassiveEffects.RunSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			num *= EntityPlayer.RunSpeedModifier;
			num2 *= EntityPlayer.RunSpeedModifier;
		}
		else
		{
			num = Constants.cPlayerSpeedModifierWalking;
			num2 = EffectManager.GetValue(PassiveEffects.WalkSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			num *= EntityPlayer.WalkSpeedModifier;
			num2 *= EntityPlayer.WalkSpeedModifier;
		}
		num *= 0.35f;
		if (num2 < num)
		{
			num2 = num;
		}
		return num2;
	}

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x060026DF RID: 9951 RVA: 0x000EDB49 File Offset: 0x000EBD49
	public override float MaxVelocity
	{
		get
		{
			if (this.MovementRunning)
			{
				return 0.35f;
			}
			return 0.17999f;
		}
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000EDB5E File Offset: 0x000EBD5E
	public override Vector3 GetVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetVelocityPerSecond();
		}
		return this.averageVel * 20f;
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000EDB89 File Offset: 0x000EBD89
	public Color GetTeamColor()
	{
		return Constants.cTeamColors[this.TeamNumber];
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000EDB9C File Offset: 0x000EBD9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpMotion()
	{
		base.StartJumpMotion();
		float @float = SandboxOptionManager.GetFloat(SandboxOptions.JumpStrength);
		this.motion.y = EffectManager.GetValue(PassiveEffects.JumpStrength, null, this.jumpStrength, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * base.Stats.Stamina.ValuePercent * @float;
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x000EDBF8 File Offset: 0x000EBDF8
	public override void OnUpdateLive()
	{
		base.Stats.Stamina.RegenerationAmount = 0f;
		base.OnUpdateLive();
		base.GetEntitySenses().Clear();
		this.CheckSleeperTriggers();
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000EDC26 File Offset: 0x000EBE26
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CheckSleeperTriggers()
	{
		if (!this.world.IsRemote() && base.IsAlive())
		{
			this.world.CheckSleeperVolumeTouching(this);
			this.world.CheckTriggerVolumeTrigger(this);
		}
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000EDC55 File Offset: 0x000EBE55
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale = 1f)
	{
		if (GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled) && ItemActionAttack.IncomingDamageModifier > 0f)
		{
			return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
		}
		return 0;
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000027FC File Offset: 0x000009FC
	public override void CheckDismember(ref DamageResponse _dmResponse, float damagePer)
	{
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000EDC79 File Offset: 0x000EBE79
	public override void PlayOneShot(string clipName, bool sound_in_head = false, bool serverSignalOnly = false, bool isUnique = false, AnimationEvent _animEvent = null, float volumeScale = 1f)
	{
		if (!this.isSpectator || sound_in_head)
		{
			base.PlayOneShot(clipName, sound_in_head, serverSignalOnly, isUnique, _animEvent, volumeScale);
		}
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000EDC98 File Offset: 0x000EBE98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetSoundHurt(DamageSource _damageSource, int _damageStrength)
	{
		string soundDrownPain;
		if (_damageSource.GetDamageType() == EnumDamageTypes.Suffocation && (soundDrownPain = base.GetSoundDrownPain()) != null)
		{
			return soundDrownPain;
		}
		if (_damageStrength > 15 || base.GetSoundHurtSmall() == null)
		{
			return base.GetSoundHurt();
		}
		return base.GetSoundHurtSmall();
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x000EDCD5 File Offset: 0x000EBED5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetSoundDeath(DamageSource _damageSource)
	{
		if (this.soundDrownDeath == null || _damageSource.GetDamageType() != EnumDamageTypes.Suffocation)
		{
			return base.GetSoundDeath(_damageSource);
		}
		return this.soundDrownDeath;
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000EDCF7 File Offset: 0x000EBEF7
	public bool CanHeal()
	{
		return this.Health > 0 && this.Health < this.GetMaxHealth();
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsSavedToNetwork()
	{
		return false;
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void EnableCamera(bool _b)
	{
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000EDD12 File Offset: 0x000EBF12
	public virtual void Respawn(RespawnType _reason)
	{
		this.lastRespawnReason = _reason;
		this.emodel.DisableRagdoll(true);
		this.InitBreadcrumbs();
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000EDD30 File Offset: 0x000EBF30
	public virtual void Teleport(Vector3 _pos, float _dir = -3.4028235E+38f)
	{
		if (this.AttachedToEntity)
		{
			this.AttachedToEntity.SetPosition(_pos, true);
		}
		else
		{
			this.SetPosition(_pos, true);
			if (_dir > -999999f)
			{
				this.SetRotation(new Vector3(0f, _dir, 0f));
			}
		}
		GameEventManager.Current.HandleForceBossDespawn(this);
		this.Respawn(RespawnType.Teleport);
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000EDD91 File Offset: 0x000EBF91
	public void InvokeTeleportDelegates()
	{
		EntityPlayer.OnPlayerTeleportDelegate playerTeleportedDelegates = this.PlayerTeleportedDelegates;
		if (playerTeleportedDelegates == null)
		{
			return;
		}
		playerTeleportedDelegates();
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void BeforePlayerRespawn(RespawnType _type)
	{
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000EDDA3 File Offset: 0x000EBFA3
	public virtual void AfterPlayerRespawn(RespawnType _type)
	{
		this.InvokeTeleportDelegates();
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000EDDAC File Offset: 0x000EBFAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onSpawnStateChanged()
	{
		base.onSpawnStateChanged();
		this.SetVisible(this.Spawned);
		if (this.Spawned)
		{
			this.SpawnedTicks = 0;
			switch (this.lastRespawnReason)
			{
			case RespawnType.NewGame:
			case RespawnType.Died:
			case RespawnType.EnterMultiplayer:
				if (!this.world.IsRemote() && !this.world.IsEditor() && this.IsSafeZoneActive())
				{
					this.world.LockAreaMasterChunksAround(World.worldToBlockPos(base.GetPosition()), this.world.worldTime + (ulong)((long)(GamePrefs.GetInt(EnumGamePrefs.PlayerSafeZoneHours) * 1000)));
				}
				break;
			}
			if (this.lastRespawnReason != RespawnType.Teleport)
			{
				this.lastRespawnReason = RespawnType.Unknown;
			}
		}
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000EDE64 File Offset: 0x000EC064
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			Transform modelTransformParent = this.emodel.GetModelTransformParent();
			this.attachedModelPos = modelTransformParent.localPosition;
			modelTransformParent.localPosition = Vector3.zero;
		}
		return slot;
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000EDEA3 File Offset: 0x000EC0A3
	public override void Detach()
	{
		base.Detach();
		this.emodel.GetModelTransformParent().localPosition = this.attachedModelPos;
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000EDEC4 File Offset: 0x000EC0C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onNewPrefabEntered(PrefabInstance _prefabInstance)
	{
		if (_prefabInstance == null)
		{
			return;
		}
		if (_prefabInstance.prefab.bTraderArea)
		{
			EntityPlayerLocal entityPlayerLocal = this as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				Waypoint waypoint = new Waypoint();
				waypoint.pos = World.worldToBlockPos(_prefabInstance.boundingBoxPosition + _prefabInstance.boundingBoxSize / 2);
				waypoint.icon = "ui_game_symbol_map_trader";
				waypoint.name.Update(_prefabInstance.prefab.PrefabName, PlatformManager.MultiPlatform.User.PlatformUserId);
				waypoint.ownerId = null;
				waypoint.lastKnownPositionEntityId = -1;
				waypoint.bIsAutoWaypoint = true;
				waypoint.bUsingLocalizationId = true;
				if (!entityPlayerLocal.Waypoints.ContainsWaypoint(waypoint))
				{
					NavObject navObject = NavObjectManager.Instance.RegisterNavObject("waypoint", waypoint.pos, waypoint.icon, true, -1, null);
					navObject.UseOverrideColor = true;
					navObject.OverrideColor = Color.white;
					navObject.IsActive = false;
					navObject.name = waypoint.name.Text;
					navObject.usingLocalizationId = true;
					waypoint.navObject = navObject;
					entityPlayerLocal.Waypoints.Collection.Add(waypoint);
				}
			}
		}
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000EDFE6 File Offset: 0x000EC1E6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpSwimMotion()
	{
		this.motion.y = this.motion.y + 0.04f;
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x060026F8 RID: 9976 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsImmuneToLegDamage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000EDFFC File Offset: 0x000EC1FC
	public override int GetLayerForMapIcon()
	{
		return 19;
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000EE000 File Offset: 0x000EC200
	public override bool CanMapIconBeSelected()
	{
		return GameStats.GetBool(EnumGameStats.IsSpawnNearOtherPlayer);
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000EE009 File Offset: 0x000EC209
	public override bool IsDrawMapIcon()
	{
		return base.IsSpawned() && ((this.IsFriendOfLocalPlayer && GameStats.GetBool(EnumGameStats.ShowFriendPlayerOnMap)) || GameStats.GetBool(EnumGameStats.ShowAllPlayersOnMap) || this.IsInPartyOfLocalPlayer);
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000BD92B File Offset: 0x000BBB2B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000EE037 File Offset: 0x000EC237
	public override Vector3 GetMapIconScale()
	{
		return new Vector3(1.5f, 1.5f, 1.5f);
	}

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x060026FF RID: 9983 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsAlert
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsClientControlled()
	{
		return true;
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000EE050 File Offset: 0x000EC250
	public bool IsFriendsWith(EntityPlayer _other)
	{
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
		PersistentPlayerData playerDataFromEntityID2 = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_other.entityId);
		return playerDataFromEntityID2 != null && playerDataFromEntityID2.IsAlly(playerDataFromEntityID);
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000EE09A File Offset: 0x000EC29A
	public bool IsSafeZoneActive()
	{
		return this.Progression.Level <= GamePrefs.GetInt(EnumGamePrefs.PlayerSafeZoneLevel) && this.spawnPoints.Count == 0;
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000EE0C0 File Offset: 0x000EC2C0
	public override void OnEntityUnload()
	{
		if (!this.world.IsEditor() && this.prefab != null)
		{
			this.world.triggerManager.RemovePlayer(this.prefab, this.entityId);
		}
		base.OnEntityUnload();
		this.ChunkObserver = null;
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000EE100 File Offset: 0x000EC300
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		this.SpawnedTicks++;
		Vector3 a = this.position - this.averagVelLastPos;
		this.averagVelLastPos = this.position;
		if (a.sqrMagnitude < 25f)
		{
			this.averageVel = this.averageVel * 0.7f + a * 0.3f;
		}
		if (this.Health <= 0)
		{
			this.lastRespawnReason = RespawnType.Died;
			List<Transform> list = new List<Transform>();
			GameUtils.FindDeepChildWithPartialName(base.transform, "temp_Projectile", ref list);
			for (int i = 0; i < list.Count; i++)
			{
				UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Vector3 position = this.position;
			if ((position - this.breadcrumbLastPos).sqrMagnitude >= 0.9025f)
			{
				this.breadcrumbLastPos = position;
				this.breadcrumbIndex = (this.breadcrumbIndex + 1 & 31);
				this.breadcrumbs[this.breadcrumbIndex] = position;
			}
			this.Stealth.TickServer();
		}
		else if (!this.isEntityRemote)
		{
			this.Stealth.TickLocalClient();
		}
		if (this.bag != null)
		{
			this.CarryCapacity = (int)EffectManager.GetValue(PassiveEffects.CarryCapacity, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			int num = this.bag.GetUsedSlotCount() - this.CarryCapacity;
			this.Buffs.SetCustomVar("_carrycapacity", (float)this.CarryCapacity, true, CVarOperation.set, false);
			this.Buffs.SetCustomVar("_encumbrance", Mathf.Max((float)num, 0f) / (float)(this.bag.GetSlots().Length - this.CarryCapacity), true, CVarOperation.set, false);
			this.Buffs.SetCustomVar("_encumberedslots", Mathf.Max((float)num, 0f), true, CVarOperation.set, false);
		}
		this.PrefabTick();
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000EE2F4 File Offset: 0x000EC4F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrefabTick()
	{
		if (Time.time - this.lastTimePrefabChecked > 1f)
		{
			this.lastTimePrefabChecked = Time.time;
			PrefabInstance poiatPosition = this.world.GetPOIAtPosition(this.position, null, null);
			if (poiatPosition != this.prefab)
			{
				if (!this.world.IsEditor())
				{
					if (this.prefab != null)
					{
						this.world.triggerManager.RemovePlayer(this.prefab, this.entityId);
					}
					if (poiatPosition != null)
					{
						this.world.triggerManager.AddPrefabData(poiatPosition, this.entityId);
					}
				}
				this.prefab = poiatPosition;
				this.prefabTimeIn = Time.time;
				this.prefabInfoEntered = false;
				this.onNewPrefabEntered(this.prefab);
			}
			if (this.prefab != null && !this.prefabInfoEntered && !this.world.IsEditor())
			{
				if (this is EntityPlayerLocal)
				{
					if (this.prefab.IsWithinInfoArea(this.position))
					{
						if (this.prefab.prefab.InfoVolumeList.Count > 0 || this.prefab.prefab.DifficultyTier >= 0)
						{
							this.enteredPrefab = this.prefab;
						}
						this.prefabInfoEntered = true;
					}
				}
				else
				{
					this.prefabInfoEntered = true;
				}
			}
			Vector3i blockPosition = base.GetBlockPosition();
			this.IsInTrader = (this.world.GetTraderAreaAt(blockPosition) != null);
			if (this.TwitchEnabled || this.HasTwitchMember())
			{
				this.TwitchSafe = (!this.world.CanPlaceBlockAt(blockPosition, null, false) || this.IsInTrader);
				return;
			}
			if (this.twitchSafe)
			{
				this.TwitchSafe = false;
			}
		}
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000EE499 File Offset: 0x000EC699
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitBreadcrumbs()
	{
		this.breadcrumbs.Fill(this.position);
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000EE4AC File Offset: 0x000EC6AC
	public Vector3 GetBreadcrumbPos(float distance)
	{
		int num = (int)(distance + 0.5f);
		int num2 = this.breadcrumbIndex;
		if (num >= 31)
		{
			num2++;
		}
		else
		{
			num2 -= num;
		}
		return this.breadcrumbs[num2 & 31];
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000EE4E7 File Offset: 0x000EC6E7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateStepSound(float _distX, float _distZ, float _rotYDelta)
	{
		if (this is EntityPlayerLocal && !this.isSpectator)
		{
			base.updateStepSound(_distX, _distZ, _rotYDelta);
		}
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000EE502 File Offset: 0x000EC702
	public override float GetBlockDamageScale(bool isTerrain)
	{
		if (!isTerrain)
		{
			return ItemActionAttack.BlockDamagePercent;
		}
		return ItemActionAttack.TerrainDamagePercent;
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000EE512 File Offset: 0x000EC712
	public override void SetDamagedTarget(EntityAlive _attackTarget)
	{
		base.SetDamagedTarget(_attackTarget);
		if (_attackTarget is EntityEnemy)
		{
			this.LastZombieAttackTime = this.world.worldTime;
		}
		this.IsBloodMoonDead = false;
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000EE53C File Offset: 0x000EC73C
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		if (!this.Spawned)
		{
			return;
		}
		int num = this.visiblityCheckTicks - 1;
		this.visiblityCheckTicks = num;
		if (num > 0)
		{
			return;
		}
		this.visiblityCheckTicks = 5;
		int num2 = Utils.FastMin(12, GameUtils.GetViewDistance()) * 16;
		num2--;
		this.bModelVisible = (_distanceSqr < (float)(num2 * num2));
		if (!this.IsDead() && base.GetDeathTime() == 0)
		{
			this.SetVisible(this.bModelVisible);
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000EE5AB File Offset: 0x000EC7AB
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVisible(bool _isVisible)
	{
		if (this.isSpectator)
		{
			this.emodel.SetVisible(false, false);
			return;
		}
		this.emodel.SetVisible(_isVisible, !this.world.IsRemote());
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000EE5DD File Offset: 0x000EC7DD
	public override void Kill(DamageResponse _dmResponse)
	{
		base.Kill(_dmResponse);
		this.currentLife = 0f;
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnHUD()
	{
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000EE5F4 File Offset: 0x000EC7F4
	public void ServerNetSendRangeCheckedDamage(Vector3 _origin, float _maxRange, DamageSourceEntity _damageSource, int _strength, bool _isCritical, List<string> _buffActions, string _buffActionsContext, ParticleEffect particleEffect)
	{
		NetPackageRangeCheckDamageEntity package = NetPackageManager.GetPackage<NetPackageRangeCheckDamageEntity>().Setup(this.entityId, _origin, _maxRange, _damageSource, _strength, _isCritical, _buffActions, _buffActionsContext, particleEffect);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000EE642 File Offset: 0x000EC842
	public override AttachedToEntitySlotExit FindValidExitPosition(List<AttachedToEntitySlotExit> candidatePositions)
	{
		this.lastVehiclePositionOnDismount = this.position;
		this.timeOfVehicleDismount = Time.time;
		this.forcedDetach = false;
		return base.FindValidExitPosition(candidatePositions);
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000EE66C File Offset: 0x000EC86C
	public override void CheckPosition()
	{
		base.CheckPosition();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.IsFlyMode.Value || !this.Spawned)
		{
			return;
		}
		if (this.position.y >= 0f)
		{
			return;
		}
		if (this.AttachedToEntity != null)
		{
			this.Detach();
			this.forcedDetach = true;
			return;
		}
		Log.Out(string.Format("[FELLTHROUGHWORLD] Player is under the world, starting teleport respawn from {0}", this.position));
		Vector3 fallingSavePosition = this.GetFallingSavePosition();
		if (this.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(fallingSavePosition, null, true), false, this.entityId, -1, -1, null, 192, false);
			return;
		}
		Log.Out(string.Format("[FELLTHROUGHWORLD] Attempting teleport to {0}", fallingSavePosition));
		this.Teleport(fallingSavePosition, float.MinValue);
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000EE754 File Offset: 0x000EC954
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetFallingSavePosition()
	{
		if (!this.forcedDetach && Time.time - this.timeOfVehicleDismount < this.vehicleTeleportThresholdSeconds)
		{
			return this.lastVehiclePositionOnDismount;
		}
		Vector3 position = this.position;
		IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos((int)position.x, (int)position.z);
		if (chunkFromWorldPos == null || chunkFromWorldPos.IsEmpty())
		{
			Log.Out(string.Format("[FELLTHROUGHWORLD] GetFallingSavePosition - CurrentChunk {0}", chunkFromWorldPos));
			IChunk chunk = null;
			Vector2 b = new Vector2(position.x, position.z);
			float num = float.PositiveInfinity;
			foreach (long chunkKey in this.ChunkObserver.chunksAround.list)
			{
				IChunk chunkSync = this.world.GetChunkSync(chunkKey);
				if (chunkSync != null && !chunkSync.IsEmpty())
				{
					Vector3i worldPos = chunkSync.GetWorldPos();
					float sqrMagnitude = (new Vector2((float)worldPos.x + 8f, (float)worldPos.z + 8f) - b).sqrMagnitude;
					if (chunk == null || sqrMagnitude < num)
					{
						chunk = chunkSync;
						num = sqrMagnitude;
					}
				}
			}
			Log.Out(string.Format("[FELLTHROUGHWORLD] GetFallingSavePosition - closestChunk {0}", chunk));
			if (chunk != null)
			{
				Vector3i worldPos2 = chunk.GetWorldPos();
				position.x = Math.Clamp(position.x, (float)worldPos2.x + 0.5f, (float)(worldPos2.x + 16) - 1f);
				position.z = Math.Clamp(position.z, (float)worldPos2.z + 0.5f, (float)(worldPos2.z + 16) - 1f);
			}
		}
		position.y = (float)GameManager.Instance.World.GetTerrainHeight((int)position.x, (int)position.z) + 0.5f;
		return position;
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000EE93C File Offset: 0x000ECB3C
	public override bool FriendlyFireCheck(EntityAlive other)
	{
		bool result = true;
		try
		{
			EntityPlayer entityPlayer = other as EntityPlayer;
			if (entityPlayer != null)
			{
				if (this.entityId == entityPlayer.entityId)
				{
					return true;
				}
				int @int = GameStats.GetInt(EnumGameStats.PlayerKillingMode);
				if (@int != 0)
				{
					if (@int - 1 <= 1)
					{
						PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
						PersistentPlayerData playerDataFromEntityID2 = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer.entityId);
						if (playerDataFromEntityID != null && playerDataFromEntityID2 != null)
						{
							bool flag = playerDataFromEntityID2.IsAlly(playerDataFromEntityID);
							bool flag2 = this.Party != null && this.Party.MemberList.Contains(entityPlayer);
							result = ((flag || flag2) ^ @int == 2);
						}
					}
				}
				else
				{
					result = false;
				}
			}
		}
		catch
		{
			result = true;
		}
		return result;
	}

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06002714 RID: 10004 RVA: 0x000EEA0C File Offset: 0x000ECC0C
	// (set) Token: 0x06002715 RID: 10005 RVA: 0x000EEA14 File Offset: 0x000ECC14
	public Party Party
	{
		get
		{
			return this.party;
		}
		set
		{
			if (this.party != null && value == null && this is EntityPlayerLocal)
			{
				this.party.ClearAllNavObjectColors();
			}
			this.party = value;
			if (this.party == null && this is EntityPlayerLocal)
			{
				this.QuestJournal.RemoveAllSharedQuests();
			}
		}
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06002716 RID: 10006 RVA: 0x000EEA61 File Offset: 0x000ECC61
	public CompanionGroup Companions
	{
		get
		{
			if (this.companions == null)
			{
				this.companions = new CompanionGroup();
			}
			return this.companions;
		}
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000EEA7C File Offset: 0x000ECC7C
	public bool IsInParty()
	{
		return this.Party != null;
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000EEA87 File Offset: 0x000ECC87
	public bool IsPartyLead()
	{
		return this.Party != null && this.Party.Leader == this;
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000EEAA4 File Offset: 0x000ECCA4
	public bool HasTwitchMember()
	{
		return this.Party != null && this.Party.HasTwitchMember;
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000EEABB File Offset: 0x000ECCBB
	public TwitchVoteLockTypes HasTwitchVoteLockMember()
	{
		if (this.Party != null)
		{
			return this.Party.HasTwitchVoteLock;
		}
		return TwitchVoteLockTypes.None;
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000EEAD2 File Offset: 0x000ECCD2
	public void CreateParty()
	{
		this.Party = new Party();
		this.Party.AddPlayer(this);
		this.Party.LeaderIndex = 0;
		this.HandleOnPartyJoined();
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000EEB00 File Offset: 0x000ECD00
	public void LeaveParty()
	{
		Party oldParty = this.Party;
		if (this.Party != null)
		{
			this.Party.MemberList.Remove(this);
			if (this is EntityPlayerLocal)
			{
				for (int i = 0; i < this.Party.MemberList.Count; i++)
				{
					if (this.Party.MemberList[i].NavObject != null)
					{
						this.Party.MemberList[i].NavObject.UseOverrideColor = false;
					}
				}
			}
		}
		this.Party = null;
		this.HandleOnPartyLeave(oldParty);
	}

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x0600271D RID: 10013 RVA: 0x000EEB94 File Offset: 0x000ECD94
	// (remove) Token: 0x0600271E RID: 10014 RVA: 0x000EEBCC File Offset: 0x000ECDCC
	public event OnPartyChanged PartyJoined;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x0600271F RID: 10015 RVA: 0x000EEC04 File Offset: 0x000ECE04
	// (remove) Token: 0x06002720 RID: 10016 RVA: 0x000EEC3C File Offset: 0x000ECE3C
	public event OnPartyChanged PartyChanged;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06002721 RID: 10017 RVA: 0x000EEC74 File Offset: 0x000ECE74
	// (remove) Token: 0x06002722 RID: 10018 RVA: 0x000EECAC File Offset: 0x000ECEAC
	public event OnPartyChanged PartyLeave;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06002723 RID: 10019 RVA: 0x000EECE4 File Offset: 0x000ECEE4
	// (remove) Token: 0x06002724 RID: 10020 RVA: 0x000EED1C File Offset: 0x000ECF1C
	public event OnPartyChanged InvitedToParty;

	// Token: 0x06002725 RID: 10021 RVA: 0x000EED54 File Offset: 0x000ECF54
	public void RemovePartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		if (this.partyInvites.Contains(item))
		{
			this.partyInvites.Remove(item);
		}
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000EED92 File Offset: 0x000ECF92
	public void RemoveAllPartyInvites()
	{
		this.partyInvites.Clear();
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000EEDA0 File Offset: 0x000ECFA0
	public void AddPartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		if (!this.partyInvites.Contains(item))
		{
			this.partyInvites.Add(item);
			if (this.InvitedToParty != null)
			{
				this.InvitedToParty(null, this);
			}
		}
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000EEDF4 File Offset: 0x000ECFF4
	public bool HasPendingPartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		return this.partyInvites.Contains(item);
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000EEE23 File Offset: 0x000ED023
	public void HandleOnPartyJoined()
	{
		OnPartyChanged partyJoined = this.PartyJoined;
		if (partyJoined == null)
		{
			return;
		}
		partyJoined(this.party, this);
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x000EEE3C File Offset: 0x000ED03C
	public void HandleOnPartyChanged()
	{
		OnPartyChanged partyChanged = this.PartyChanged;
		if (partyChanged == null)
		{
			return;
		}
		partyChanged(this.party, this);
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x000EEE55 File Offset: 0x000ED055
	public void HandleOnPartyLeave(Party _oldParty)
	{
		OnPartyChanged partyLeave = this.PartyLeave;
		if (partyLeave == null)
		{
			return;
		}
		partyLeave(_oldParty, this);
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000EEE69 File Offset: 0x000ED069
	public void PartyDisconnect()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Party.ServerHandleDisconnectParty(this);
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000EEE80 File Offset: 0x000ED080
	public void SetPrefabsAroundNear(Dictionary<int, PrefabInstance> _prefabsAround)
	{
		this.prefabsAroundNear.Clear();
		foreach (KeyValuePair<int, PrefabInstance> keyValuePair in _prefabsAround)
		{
			this.prefabsAroundNear.Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x000EEEEC File Offset: 0x000ED0EC
	public Dictionary<int, PrefabInstance> GetPrefabsAroundNear()
	{
		return this.prefabsAroundNear;
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000EEEF4 File Offset: 0x000ED0F4
	public void AddKillXP(EntityAlive killedEntity, ItemValue itemUsed, float xpModifier = 1f)
	{
		int num = EntityClass.list[killedEntity.entityClass].ExperienceValue;
		num = (int)EffectManager.GetValue(PassiveEffects.ExperienceGain, killedEntity.inventory.holdingItemItemValue, (float)num, killedEntity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (xpModifier != 1f)
		{
			num = (int)((float)num * xpModifier + 0.5f);
		}
		if (this.IsInParty())
		{
			num = this.Party.GetPartyXP(this, num);
		}
		if (!this.isEntityRemote)
		{
			this.Progression.AddLevelExp(num, "_xpFromKill", Progression.XPTypes.Kill, true, true, this.entityId, itemUsed);
			this.bPlayerStatsChanged = true;
			return;
		}
		NetPackageEntityAddExpClient package = NetPackageManager.GetPackage<NetPackageEntityAddExpClient>().Setup(this.entityId, num, Progression.XPTypes.Kill, itemUsed);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000EEFD0 File Offset: 0x000ED1D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleClientDeath(Vector3i attackPos)
	{
		base.HandleClientDeath(attackPos);
		TwitchManager.Current.CheckKiller(this, this.entityThatKilledMe, attackPos);
		switch (GameStats.GetInt(EnumGameStats.DeathPenalty))
		{
		case 0:
			GameEventManager.Current.HandleAction("game_on_death_none", this, this, false, "", "", false, true, "", null);
			return;
		case 1:
			GameEventManager.Current.HandleAction("game_on_death_default", this, this, false, "", "", false, true, "", null);
			return;
		case 2:
			GameEventManager.Current.HandleAction("game_on_death_injured", this, this, false, "", "", false, true, "", null);
			return;
		case 3:
			GameEventManager.Current.HandleAction("game_on_death_permanent", this, this, false, "", "", false, true, "", null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000EF0AC File Offset: 0x000ED2AC
	public void HandleTwitchActionsTempEnabled(EntityPlayer.TwitchActionsStates newState)
	{
		if (this.twitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled)
		{
			return;
		}
		this.TwitchActionsEnabled = newState;
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000EF0C0 File Offset: 0x000ED2C0
	public bool IsReloadCancelled()
	{
		if (this.inventory.holdingItemData.actionData != null)
		{
			foreach (ItemActionData itemActionData in this.inventory.holdingItemData.actionData)
			{
				ItemActionRanged.ItemActionDataRanged itemActionDataRanged = itemActionData as ItemActionRanged.ItemActionDataRanged;
				if (itemActionDataRanged != null && itemActionDataRanged.isReloadCancelled)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000EF140 File Offset: 0x000ED340
	public void SetLaserSightData(bool _laserSightActive, Vector3 _laserSightPosition)
	{
		if (_laserSightActive == this.laserSightActive && (this.laserSightPosition - _laserSightPosition).sqrMagnitude < 0.0016f)
		{
			return;
		}
		this.laserSightPosition = _laserSightPosition;
		this.laserSightActive = _laserSightActive;
		if (this.world.entityDistributer != null)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, (this.world.GetPrimaryPlayer() != null) ? this.world.GetPrimaryPlayer().entityId : -1, NetPackageManager.GetPackage<NetPackagePlayerLaserSight>().Setup(this.entityId, _laserSightActive, _laserSightPosition), true);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerLaserSight>().Setup(this.entityId, _laserSightActive, _laserSightPosition), false);
	}

	// Token: 0x04001CB5 RID: 7349
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly FastTags<TagGroup.Global> STAMINA_LOSS_TAGS = FastTags<TagGroup.Global>.GetTag("Athletics");

	// Token: 0x04001CB6 RID: 7350
	public float jumpStrength = 0.451f;

	// Token: 0x04001CB7 RID: 7351
	public SpawnPosition lastSpawnPosition = SpawnPosition.Undef;

	// Token: 0x04001CB8 RID: 7352
	public PlayerProfile playerProfile;

	// Token: 0x04001CB9 RID: 7353
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PersistentPlayerName cachedPlayerName;

	// Token: 0x04001CBA RID: 7354
	public static float EncumbranceModifier = 1f;

	// Token: 0x04001CBB RID: 7355
	public static float GlobalGameStageModifier = 1f;

	// Token: 0x04001CBC RID: 7356
	public static float BiomeGameStageModifier = 1f;

	// Token: 0x04001CBD RID: 7357
	public static float GlobalLootStageModifier = 1f;

	// Token: 0x04001CBE RID: 7358
	public static float BiomeLootStageModifier = 1f;

	// Token: 0x04001CBF RID: 7359
	public static float POITierLootStageModifier = 1f;

	// Token: 0x04001CC0 RID: 7360
	public static float GlobalTraderStageModifier = 1f;

	// Token: 0x04001CC1 RID: 7361
	public static float FallDamageModifier = 1f;

	// Token: 0x04001CC2 RID: 7362
	public List<EntityAlive> aiClosest = new List<EntityAlive>();

	// Token: 0x04001CC3 RID: 7363
	public AIDirectorBloodMoonParty bloodMoonParty;

	// Token: 0x04001CC4 RID: 7364
	public bool IsBloodMoonDead;

	// Token: 0x04001CC5 RID: 7365
	public PlayerStealth Stealth;

	// Token: 0x04001CC6 RID: 7366
	public const long cSpawnPointKeyInvalid = -1L;

	// Token: 0x04001CC7 RID: 7367
	public long selectedSpawnPointKey = -1L;

	// Token: 0x04001CC8 RID: 7368
	public ulong LastZombieAttackTime;

	// Token: 0x04001CC9 RID: 7369
	public bool IsInPartyOfLocalPlayer;

	// Token: 0x04001CCA RID: 7370
	public uint totalItemsCrafted;

	// Token: 0x04001CCB RID: 7371
	public float longestLife;

	// Token: 0x04001CCC RID: 7372
	public float currentLife;

	// Token: 0x04001CCD RID: 7373
	public float totalTimePlayed;

	// Token: 0x04001CCE RID: 7374
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int longestLifeLived;

	// Token: 0x04001CCF RID: 7375
	public ChunkManager.ChunkObserver ChunkObserver;

	// Token: 0x04001CD0 RID: 7376
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public RespawnType lastRespawnReason = RespawnType.Unknown;

	// Token: 0x04001CD1 RID: 7377
	public int SpawnedTicks;

	// Token: 0x04001CD2 RID: 7378
	public ulong gameStageBornAtWorldTime;

	// Token: 0x04001CD3 RID: 7379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i m_MarkerPosition = Vector3i.zero;

	// Token: 0x04001CD4 RID: 7380
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject navMarker;

	// Token: 0x04001CD5 RID: 7381
	public bool navMarkerHidden;

	// Token: 0x04001CD6 RID: 7382
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject navVending;

	// Token: 0x04001CD7 RID: 7383
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastTimePrefabChecked;

	// Token: 0x04001CD8 RID: 7384
	public PrefabInstance prefab;

	// Token: 0x04001CD9 RID: 7385
	public PrefabInstance enteredPrefab;

	// Token: 0x04001CDA RID: 7386
	public bool prefabInfoEntered;

	// Token: 0x04001CDB RID: 7387
	public float prefabTimeIn;

	// Token: 0x04001CDC RID: 7388
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bModelVisible = true;

	// Token: 0x04001CDF RID: 7391
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchEnabled;

	// Token: 0x04001CE0 RID: 7392
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchSafe;

	// Token: 0x04001CE1 RID: 7393
	public bool IsInTrader;

	// Token: 0x04001CE2 RID: 7394
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public TwitchVoteLockTypes twitchVoteLock;

	// Token: 0x04001CE3 RID: 7395
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchVisionDisabled;

	// Token: 0x04001CE4 RID: 7396
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityPlayer.TwitchActionsStates twitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;

	// Token: 0x04001CE5 RID: 7397
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isSpectator;

	// Token: 0x04001CE6 RID: 7398
	public WaypointCollection Waypoints = new WaypointCollection();

	// Token: 0x04001CE7 RID: 7399
	public List<Waypoint> WaypointInvites = new List<Waypoint>();

	// Token: 0x04001CE8 RID: 7400
	public QuestJournal QuestJournal = new QuestJournal();

	// Token: 0x04001CEE RID: 7406
	public List<ushort> favoriteCreativeStacks = new List<ushort>();

	// Token: 0x04001CEF RID: 7407
	public List<string> favoriteShapes = new List<string>();

	// Token: 0x04001CF0 RID: 7408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i m_rentedVMPosition = Vector3i.zero;

	// Token: 0x04001CF1 RID: 7409
	public ulong RentalEndTime;

	// Token: 0x04001CF2 RID: 7410
	public int RentalEndDay;

	// Token: 0x04001CF3 RID: 7411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 averageVel;

	// Token: 0x04001CF4 RID: 7412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 averagVelLastPos;

	// Token: 0x04001CF5 RID: 7413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cBreadcrumbMask = 31;

	// Token: 0x04001CF6 RID: 7414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3[] breadcrumbs = new Vector3[32];

	// Token: 0x04001CF7 RID: 7415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 breadcrumbLastPos;

	// Token: 0x04001CF8 RID: 7416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int breadcrumbIndex;

	// Token: 0x04001CF9 RID: 7417
	public static float WalkSpeedModifier = 1f;

	// Token: 0x04001CFA RID: 7418
	public static float RunSpeedModifier = 1f;

	// Token: 0x04001CFB RID: 7419
	public static float CrouchSpeedModifier = 1f;

	// Token: 0x04001CFC RID: 7420
	public static float CrouchRunSpeedModifier = 1f;

	// Token: 0x04001CFD RID: 7421
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isAdmin;

	// Token: 0x04001CFE RID: 7422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i lastChunkPos = new Vector3i(int.MinValue, int.MinValue, int.MinValue);

	// Token: 0x04001CFF RID: 7423
	public bool HasUpdated;

	// Token: 0x04001D00 RID: 7424
	public FastTags<TagGroup.Global> generalTags = FastTags<TagGroup.Global>.none;

	// Token: 0x04001D01 RID: 7425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 attachedModelPos;

	// Token: 0x04001D02 RID: 7426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int visiblityCheckTicks;

	// Token: 0x04001D03 RID: 7427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastVehiclePositionOnDismount = Vector3.zero;

	// Token: 0x04001D04 RID: 7428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeOfVehicleDismount;

	// Token: 0x04001D05 RID: 7429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float vehicleTeleportThresholdSeconds = 10f;

	// Token: 0x04001D06 RID: 7430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool forcedDetach;

	// Token: 0x04001D07 RID: 7431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Party party;

	// Token: 0x04001D08 RID: 7432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CompanionGroup companions;

	// Token: 0x04001D0D RID: 7437
	public List<EntityPlayer> partyInvites = new List<EntityPlayer>();

	// Token: 0x04001D0E RID: 7438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<int, PrefabInstance> prefabsAroundNear = new Dictionary<int, PrefabInstance>();

	// Token: 0x04001D0F RID: 7439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool laserSightActive;

	// Token: 0x04001D10 RID: 7440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 laserSightPosition;

	// Token: 0x020004C6 RID: 1222
	// (Invoke) Token: 0x06002737 RID: 10039
	public delegate void OnPlayerTeleportDelegate();

	// Token: 0x020004C7 RID: 1223
	public enum TwitchActionsStates
	{
		// Token: 0x04001D12 RID: 7442
		Disabled,
		// Token: 0x04001D13 RID: 7443
		Enabled,
		// Token: 0x04001D14 RID: 7444
		TempDisabled,
		// Token: 0x04001D15 RID: 7445
		TempDisabledEnding
	}
}
