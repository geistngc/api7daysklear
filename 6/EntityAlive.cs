using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using GamePath;
using Platform;
using PrefabVolumes;
using UAI;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000473 RID: 1139
[Preserve]
public abstract class EntityAlive : Entity
{
	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x0600223C RID: 8764 RVA: 0x000CE9E8 File Offset: 0x000CCBE8
	// (set) Token: 0x0600223D RID: 8765 RVA: 0x000CE9F3 File Offset: 0x000CCBF3
	public bool IsEquipping
	{
		get
		{
			return this.equippingCount > 0;
		}
		set
		{
			if (value)
			{
				this.equippingCount++;
				return;
			}
			if (this.equippingCount > 0)
			{
				this.equippingCount--;
			}
		}
	}

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x0600223E RID: 8766 RVA: 0x000CEA1E File Offset: 0x000CCC1E
	public bool HasAI
	{
		get
		{
			return this.hasAI;
		}
	}

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x0600223F RID: 8767 RVA: 0x000CEA26 File Offset: 0x000CCC26
	public static bool IsHeadshotOnly
	{
		get
		{
			return EntityAlive.HeadshotMode == EntityAlive.HeadShotOnlyModes.HeadshotOnly;
		}
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06002240 RID: 8768 RVA: 0x000CEA30 File Offset: 0x000CCC30
	public static bool IsHeadshotFinisher
	{
		get
		{
			return EntityAlive.HeadshotMode == EntityAlive.HeadShotOnlyModes.HeadshotFinisher;
		}
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06002241 RID: 8769 RVA: 0x000CEA3A File Offset: 0x000CCC3A
	public static bool IsCelebrate
	{
		get
		{
			return EntityAlive.CelebrateMode == EntityAlive.CelebrateModes.Enabled;
		}
	}

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x06002242 RID: 8770 RVA: 0x000CEA44 File Offset: 0x000CCC44
	public static bool IsCelebrateHeadshot
	{
		get
		{
			return EntityAlive.CelebrateMode == EntityAlive.CelebrateModes.HeadshotOnly;
		}
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x06002243 RID: 8771 RVA: 0x000CEA4E File Offset: 0x000CCC4E
	// (set) Token: 0x06002244 RID: 8772 RVA: 0x000CEA58 File Offset: 0x000CCC58
	public bool IsDancing
	{
		get
		{
			return this.isDancing;
		}
		set
		{
			this.isDancing = value;
			if (value)
			{
				if (this.emodel != null && this.emodel.avatarController != null)
				{
					this.emodel.avatarController.UpdateInt("IsDancing", base.EntityClass.DanceTypeID, true);
					return;
				}
			}
			else if (this.emodel != null && this.emodel.avatarController != null)
			{
				this.emodel.avatarController.UpdateInt("IsDancing", 0, true);
			}
		}
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000CEAEA File Offset: 0x000CCCEA
	public void BeginDynamicRagdoll(DynamicRagdollFlags flags, FloatRange stunTime)
	{
		this._dynamicRagdoll = flags;
		this._dynamicRagdollRootMotion = Vector3.zero;
		this._dynamicRagdollStunTime = stunTime.Random(this.rand);
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000CEB14 File Offset: 0x000CCD14
	public void ActivateDynamicRagdoll()
	{
		if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.Active))
		{
			DynamicRagdollFlags dynamicRagdoll = this._dynamicRagdoll;
			this._dynamicRagdoll = DynamicRagdollFlags.None;
			Vector3 forceVec = this._dynamicRagdollRootMotion * 20f;
			this.bodyDamage.StunDuration = this._dynamicRagdollStunTime;
			this.emodel.DoRagdoll(EModelBase.RagdollMode.Default, this._dynamicRagdollStunTime, EnumBodyPartHit.None, forceVec, Vector3.zero, true);
			if (dynamicRagdoll.HasFlag(DynamicRagdollFlags.UseBoneVelocities) && this._ragdollPositionsPrev.Count == this._ragdollPositionsCur.Count)
			{
				List<Vector3> list = new List<Vector3>();
				for (int i = 0; i < this._ragdollPositionsPrev.Count; i++)
				{
					Vector3 a = this._ragdollPositionsCur[i] - this._ragdollPositionsPrev[i];
					list.Add(a * 20f);
				}
				this.emodel.ApplyRagdollVelocities(list);
			}
		}
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000CEC0C File Offset: 0x000CCE0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.entityName = base.GetType().Name;
		this.MinEventContext.Self = this;
		this.seeCache = new EntitySeeCache(this);
		this.maximumHomeDistance = -1;
		this.homePosition = new ChunkCoordinates(0, 0, 0);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !(this is EntityPlayer))
		{
			this.hasAI = true;
			this.navigator = AstarManager.CreateNavigator(this);
			this.aiManager = new EAIManager(this);
			this.lookHelper = new EntityLookHelper(this);
			this.moveHelper = new EntityMoveHelper(this);
		}
		this.equipment = new Equipment(this);
		this.InitInventory();
		this.stepHeight = 0.52f;
		this.soundDelayTicks = this.GetSoundRandomTicks() / 3 - 5;
		this.spawnPoints = new EntityBedrollPositionList(this);
		this.CreationTimeSinceLevelLoad = Time.timeSinceLevelLoad;
		this.Buffs = new EntityBuffs(this);
		this.droppedBackpackPositions = new List<Vector3i>();
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000CED03 File Offset: 0x000CCF03
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		this.InitStats();
		this.switchModelView(EnumEntityModelView.ThirdPerson);
		this.InitPostCommon();
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000CED24 File Offset: 0x000CCF24
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitPostCommon()
	{
		if (GameManager.IsDedicatedServer)
		{
			Transform modelTransform = this.emodel.GetModelTransform();
			if (modelTransform)
			{
				ServerHelper.SetupForServer(modelTransform.gameObject);
			}
		}
		this.AddCharacterController();
		this.wasSeenByPlayer = false;
		this.ticksToCheckSeenByPlayer = 20;
		if (EntityClass.list[this.entityClass].UseAIPackages)
		{
			this.hasAI = true;
			this.AIPackages = new List<string>();
			this.AIPackages.AddRange(EntityClass.list[this.entityClass].AIPackages);
			this.utilityAIContext = new Context(this);
		}
		List<string> buffs = EntityClass.list[this.entityClass].Buffs;
		if (buffs != null)
		{
			for (int i = 0; i < buffs.Count; i++)
			{
				string name = buffs[i];
				if (!this.Buffs.HasBuff(name))
				{
					this.Buffs.AddBuff(name, -1, true, false, -1f);
				}
			}
		}
		if ((this.entityFlags & EntityFlags.AIHearing) > EntityFlags.None)
		{
			this.emodel.SetVisible(false, false);
			this.emodel.SetFade(0f);
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000CEE40 File Offset: 0x000CD040
	public override void PostInit()
	{
		base.PostInit();
		this.ApplySpawnState();
		LODGroup componentInChildren = this.emodel.GetModelTransform().GetComponentInChildren<LODGroup>();
		if (componentInChildren)
		{
			LOD[] lods = componentInChildren.GetLODs();
			lods[lods.Length - 1].screenRelativeTransitionHeight = 0.003f;
			componentInChildren.SetLODs(lods);
		}
		this.disableFallBehaviorUntilOnGround = true;
		GameEventManager.Current.HandleSpawnModifier(this);
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000CEEA7 File Offset: 0x000CD0A7
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplySpawnState()
	{
		if (this.Health <= 0 && this.isEntityRemote)
		{
			this.ClientKill(DamageResponse.New(true));
		}
		this.ExecuteDismember(true);
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000CEECD File Offset: 0x000CD0CD
	public virtual void InitInventory()
	{
		if (this.inventory == null)
		{
			this.inventory = new Inventory(GameManager.Instance, this);
		}
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000CEEE8 File Offset: 0x000CD0E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void switchModelView(EnumEntityModelView modelView)
	{
		this.emodel.SwitchModelAndView(modelView == EnumEntityModelView.FirstPerson, this.IsMale);
		this.ReassignEquipmentTransforms();
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000CEF08 File Offset: 0x000CD108
	public virtual void ReassignEquipmentTransforms()
	{
		if (this.isFirstTimeEquipmentReassigned)
		{
			this.Buffs.SetCustomVar("_equipReload", 0f, true, CVarOperation.set, false);
			this.isFirstTimeEquipmentReassigned = false;
		}
		else
		{
			this.Buffs.SetCustomVar("_equipReload", 1f, true, CVarOperation.set, false);
		}
		this.equipment.InitializeEquipmentTransforms();
		this.Buffs.SetCustomVar("_equipReload", 0f, true, CVarOperation.set, false);
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000CEF7C File Offset: 0x000CD17C
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		string text = entityClass.Properties.GetString(EntityClass.PropHandItem);
		if (text.Length > 0)
		{
			int num = text.IndexOf(",");
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			this.handItem = ItemClass.GetItem(text, false);
			if (this.handItem.IsEmpty())
			{
				throw new Exception("HandItem missing " + text);
			}
		}
		else
		{
			this.handItem = ItemClass.GetItem("meleeHandPlayer", false).Clone();
		}
		if (this.inventory != null)
		{
			this.inventory.SetBareHandItem(this.handItem);
		}
		this.rightHandTransformName = "Gunjoint";
		if (this.emodel is EModelSDCS)
		{
			this.rightHandTransformName = "RightWeapon";
		}
		entityClass.Properties.ParseString(EntityClass.PropRightHandJointName, ref this.rightHandTransformName);
		if (!(this is EntityPlayer))
		{
			this.factionId = 0;
			this.factionRank = 0;
			string @string = entityClass.Properties.GetString("Faction");
			if (@string.Length > 0)
			{
				Faction factionByName = FactionManager.Instance.GetFactionByName(@string);
				if (factionByName != null)
				{
					this.factionId = factionByName.ID;
					string string2 = entityClass.Properties.GetString("FactionRank");
					if (string2.Length > 0)
					{
						this.factionRank = StringParsers.ParseUInt8(string2, 0, -1, NumberStyles.Integer);
					}
				}
			}
		}
		else if (FactionManager.Instance.GetFaction(this.factionId).ID == 0)
		{
			this.factionId = FactionManager.Instance.CreateFaction(this.entityName, true, "").ID;
			this.factionRank = byte.MaxValue;
		}
		this.maxViewAngle = 180f;
		entityClass.Properties.ParseFloat(EntityClass.PropMaxViewAngle, ref this.maxViewAngle);
		this.sightRangeBase = entityClass.SightRange;
		this.sightLightThreshold = entityClass.sightLightThreshold;
		this.SetSleeperSight(-1f, -1f);
		this.sightWakeThresholdAtRange.x = this.rand.RandomRange(entityClass.SleeperSightToWakeMin.x, entityClass.SleeperSightToWakeMin.y);
		this.sightWakeThresholdAtRange.y = this.rand.RandomRange(entityClass.SleeperSightToWakeMax.y, entityClass.SleeperSightToWakeMax.y);
		this.sightGroanThresholdAtRange.x = this.rand.RandomRange(entityClass.SleeperSightToSenseMin.x, entityClass.SleeperSightToSenseMin.y);
		this.sightGroanThresholdAtRange.y = this.rand.RandomRange(entityClass.SleeperSightToSenseMax.y, entityClass.SleeperSightToSenseMax.y);
		this.sleeperNoiseToSense = this.rand.RandomRange(entityClass.SleeperNoiseToSense.x, entityClass.SleeperNoiseToSense.y);
		this.sleeperNoiseToSenseSoundChance = entityClass.SleeperNoiseToSenseSoundChance;
		this.sleeperNoiseToWake = this.rand.RandomRange(entityClass.SleeperNoiseToWake.x, entityClass.SleeperNoiseToWake.y);
		float num2 = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropAttackTimeoutDay, ref num2);
		this.attackTimeoutDay = (int)(num2 * 20f);
		entityClass.Properties.ParseFloat(EntityClass.PropAttackTimeoutNight, ref num2);
		this.attackTimeoutNight = (int)(num2 * 20f);
		entityClass.Properties.ParseBool(EntityClass.PropStompsSpikes, ref this.stompsSpikes);
		this.weight = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropWeight, ref this.weight);
		this.weight = Utils.FastMax(this.weight, 0.5f);
		this.pushFactor = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropPushFactor, ref this.pushFactor);
		float num3 = 5f;
		entityClass.Properties.ParseFloat(EntityClass.PropTimeStayAfterDeath, ref num3);
		this.timeStayAfterDeath = (int)(num3 * 20f);
		this.IsMale = true;
		entityClass.Properties.ParseBool(EntityClass.PropIsMale, ref this.IsMale);
		this.IsFeral = entityClass.Tags.Test_Bit(EntityAlive.FeralTagBit);
		this.proneRefillRate = this.rand.RandomRange(entityClass.KnockdownProneRefillRate.x, entityClass.KnockdownProneRefillRate.y);
		this.kneelRefillRate = this.rand.RandomRange(entityClass.KnockdownKneelRefillRate.x, entityClass.KnockdownKneelRefillRate.y);
		this.moveSpeed = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeed, ref this.moveSpeed);
		this.moveSpeedNight = this.moveSpeed;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeedNight, ref this.moveSpeedNight);
		this.moveSpeedAggro = this.moveSpeed;
		this.moveSpeedAggroMax = this.moveSpeed;
		entityClass.Properties.ParseVec(EntityClass.PropMoveSpeedAggro, ref this.moveSpeedAggro, ref this.moveSpeedAggroMax);
		this.moveSpeedPanic = 1f;
		this.moveSpeedPanicMax = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeedPanic, ref this.moveSpeedPanic);
		if (this.moveSpeedPanic != 1f)
		{
			this.moveSpeedPanicMax = this.moveSpeedPanic;
		}
		entityClass.Properties.ParseFloat(EntityClass.PropSwimSpeed, ref this.swimSpeed);
		entityClass.Properties.ParseVec(EntityClass.PropSwimStrokeRate, ref this.swimStrokeRate);
		Vector2 negativeInfinity = Vector2.negativeInfinity;
		entityClass.Properties.ParseVec(EntityClass.PropMoveSpeedRand, ref negativeInfinity);
		if (negativeInfinity.x > -1f)
		{
			float num4 = this.rand.RandomRange(negativeInfinity.x, negativeInfinity.y);
			if (this.moveSpeedAggro < 1f)
			{
				this.moveSpeedAggro += num4;
				if (this.moveSpeedAggro < 0.1f)
				{
					this.moveSpeedAggro = 0.1f;
				}
				if (this.moveSpeedAggro > this.moveSpeedAggroMax)
				{
					this.moveSpeedAggro = this.moveSpeedAggroMax;
				}
			}
		}
		entityClass.Properties.ParseInt(EntityClass.PropCrouchType, ref this.crouchType);
		this.walkType = EntityAlive.GetSpawnWalkType(entityClass);
		entityClass.Properties.ParseBool(EntityClass.PropCanClimbLadders, ref this.bCanClimbLadders);
		entityClass.Properties.ParseBool(EntityClass.PropCanClimbVertical, ref this.bCanClimbVertical);
		Vector2 vector = new Vector2(1.9f, 2.1f);
		entityClass.Properties.ParseVec(EntityClass.PropJumpMaxDistance, ref vector);
		this.jumpMaxDistance = this.rand.RandomRange(vector.x, vector.y);
		this.jumpDelay = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropJumpDelay, ref this.jumpDelay);
		this.jumpDelay *= 20f;
		this.ExperienceValue = 20;
		entityClass.Properties.ParseInt(EntityClass.PropExperienceGain, ref this.ExperienceValue);
		if (this.aiManager != null)
		{
			this.aiManager.CopyPropertiesFromEntityClass(entityClass);
		}
		entityClass.Properties.ParseString(EntityClass.PropSoundSpawn, ref this.soundSpawn);
		entityClass.Properties.ParseString(EntityClass.PropSoundSleeperSense, ref this.soundSleeperGroan);
		entityClass.Properties.ParseString(EntityClass.PropSoundSleeperSnore, ref this.soundSleeperSnore);
		entityClass.Properties.ParseString(EntityClass.PropSoundDeath, ref this.soundDeath);
		entityClass.Properties.ParseString(EntityClass.PropSoundAlert, ref this.soundAlert);
		entityClass.Properties.ParseString(EntityClass.PropSoundAttack, ref this.soundAttack);
		entityClass.Properties.ParseString(EntityClass.PropSoundLiving, ref this.soundLiving);
		entityClass.Properties.ParseString(EntityClass.PropSoundRandom, ref this.soundRandom);
		entityClass.Properties.ParseString(EntityClass.PropSoundSense, ref this.soundSense);
		entityClass.Properties.ParseString(EntityClass.PropSoundGiveUp, ref this.soundGiveUp);
		this.soundStepType = "step";
		entityClass.Properties.ParseString(EntityClass.PropSoundStepType, ref this.soundStepType);
		entityClass.Properties.ParseString(EntityClass.PropSoundStamina, ref this.soundStamina);
		entityClass.Properties.ParseString(EntityClass.PropSoundJump, ref this.soundJump);
		entityClass.Properties.ParseString(EntityClass.PropSoundLand, ref this.soundLand);
		entityClass.Properties.ParseString(EntityClass.PropSoundPlayerLandThump, ref this.soundLandThump);
		entityClass.Properties.ParseString(EntityClass.PropSoundHurt, ref this.soundHurt);
		entityClass.Properties.ParseString(EntityClass.PropSoundDistressed, ref this.soundDistressed);
		entityClass.Properties.ParseString(EntityClass.PropSoundHurtSmall, ref this.soundHurtSmall);
		entityClass.Properties.ParseString(EntityClass.PropSoundDrownPain, ref this.soundDrownPain);
		entityClass.Properties.ParseString(EntityClass.PropSoundDrownDeath, ref this.soundDrownDeath);
		entityClass.Properties.ParseString(EntityClass.PropSoundWaterSurface, ref this.soundWaterSurface);
		this.soundAlertTicks = 25;
		entityClass.Properties.ParseInt(EntityClass.PropSoundAlertTime, ref this.soundAlertTicks);
		this.soundAlertTicks *= 20;
		this.soundRandomTicks = 25;
		entityClass.Properties.ParseInt(EntityClass.PropSoundRandomTime, ref this.soundRandomTicks);
		this.soundRandomTicks *= 20;
		entityClass.Properties.ParseString(EntityClass.PropParticleOnDeath, ref this.particleOnDeath);
		entityClass.Properties.ParseString(EntityClass.PropParticleOnDestroy, ref this.particleOnDestroy);
		GameMode gameModeForId = GameMode.GetGameModeForId(GameStats.GetInt(EnumGameStats.GameModeId));
		if (gameModeForId != null)
		{
			string text2 = string.Empty;
			if (entityClass.Properties.Classes.ContainsKey(EntityClass.PropItemsOnEnterGame))
			{
				text2 = entityClass.Properties.Classes[EntityClass.PropItemsOnEnterGame].GetString(gameModeForId.GetTypeName());
			}
			if (text2.Length > 0)
			{
				foreach (string text3 in text2.Split(',', StringSplitOptions.None))
				{
					ItemStack itemStack = ItemStack.FromString(text3.Trim());
					if (itemStack.itemValue.IsEmpty())
					{
						throw new Exception("Item with name '" + text3 + "' not found in class " + EntityClass.list[this.entityClass].entityClassName);
					}
					if (itemStack.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Console || (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
					{
						this.itemsOnEnterGame.Add(itemStack);
					}
				}
			}
		}
		DynamicProperties @class = entityClass.Properties.GetClass(EntityClass.PropFallLandBehavior);
		if (@class != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair in @class.Data)
			{
				string key = keyValuePair.Key;
				Dictionary<string, string> dictionary = @class.ParseKeyData(key);
				if (dictionary != null)
				{
					FloatRange height = default(FloatRange);
					FloatRange ragePer = default(FloatRange);
					FloatRange rageTime = default(FloatRange);
					IntRange difficulty = new IntRange(0, 10);
					string text4;
					EntityAlive.FallBehavior.Op type;
					if (!dictionary.TryGetValue("anim", out text4) || !Enum.TryParse<EntityAlive.FallBehavior.Op>(text4, out type))
					{
						Log.Error("Expected 'anim' parameter as float for FallBehavior " + key + ", skipping");
					}
					else
					{
						float num5 = 0f;
						if (!dictionary.TryGetValue("weight", out text4) || !StringParsers.TryParseFloat(text4, out num5))
						{
							Log.Error("Expected 'weight' parameter as float for FallBehavior " + key + ", skipping");
						}
						else if (dictionary.TryGetValue("height", out text4))
						{
							FloatRange floatRange;
							if (StringParsers.TryParseRange(text4, out floatRange, new float?(3.4028235E+38f), ','))
							{
								height = floatRange;
								if (dictionary.TryGetValue("ragePer", out text4))
								{
									FloatRange floatRange2;
									if (!StringParsers.TryParseRange(text4, out floatRange2, null, ','))
									{
										Log.Error("Expected 'ragePer' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									ragePer = floatRange2;
								}
								if (dictionary.TryGetValue("rageTime", out text4))
								{
									FloatRange floatRange3;
									if (!StringParsers.TryParseRange(text4, out floatRange3, null, ','))
									{
										Log.Error("Expected 'rageTime' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									rageTime = floatRange3;
								}
								if (dictionary.TryGetValue("difficulty", out text4))
								{
									IntRange intRange;
									if (!StringParsers.TryParseRange(text4, out intRange, null, ','))
									{
										Log.Error("Expected 'difficulty' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									difficulty = intRange;
								}
								this.fallBehaviors.Add(new EntityAlive.FallBehavior(key, type, height, num5, ragePer, rageTime, difficulty));
							}
							else
							{
								Log.Error("Expected 'height' parameter as range(min,min-max) " + key + ", skipping");
							}
						}
						else
						{
							Log.Error("Expected 'height' parameter for FallBehavior " + key + ", skipping");
						}
					}
				}
			}
		}
		DynamicProperties class2 = entityClass.Properties.GetClass(EntityClass.PropDestroyBlockBehavior);
		if (class2 != null)
		{
			EntityAlive.DestroyBlockBehavior.Op[] array2 = Enum.GetValues(typeof(EntityAlive.DestroyBlockBehavior.Op)) as EntityAlive.DestroyBlockBehavior.Op[];
			for (int j = 0; j < array2.Length; j++)
			{
				string text5 = array2[j].ToStringCached<EntityAlive.DestroyBlockBehavior.Op>();
				Dictionary<string, string> dictionary2 = class2.ParseKeyData(array2[j].ToStringCached<EntityAlive.DestroyBlockBehavior.Op>());
				if (dictionary2 != null)
				{
					FloatRange ragePer2 = default(FloatRange);
					FloatRange rageTime2 = default(FloatRange);
					IntRange difficulty2 = new IntRange(0, 10);
					string input;
					float num6;
					if (!dictionary2.TryGetValue("weight", out input) || !StringParsers.TryParseFloat(input, out num6))
					{
						Log.Error(string.Format("Expected 'weight' parameter as float for FallBehavior {0}, skipping", array2[j]));
					}
					else
					{
						if (dictionary2.TryGetValue("ragePer", out input))
						{
							FloatRange floatRange4;
							if (!StringParsers.TryParseRange(input, out floatRange4, null, ','))
							{
								Log.Error(string.Format("Expected 'ragePer' parameter as range(min,min-max) {0}, skipping", array2[j]));
								goto IL_E17;
							}
							ragePer2 = floatRange4;
						}
						if (dictionary2.TryGetValue("rageTime", out input))
						{
							FloatRange floatRange5;
							if (!StringParsers.TryParseRange(input, out floatRange5, null, ','))
							{
								Log.Error(string.Format("Expected 'rageTime' parameter as range(min,min-max) {0}, skipping", array2[j]));
								goto IL_E17;
							}
							rageTime2 = floatRange5;
						}
						if (dictionary2.TryGetValue("difficulty", out input))
						{
							IntRange intRange2;
							if (!StringParsers.TryParseRange(input, out intRange2, null, ','))
							{
								Log.Error("Expected 'difficulty' parameter as range(min,min-max) " + text5 + ", skipping");
								goto IL_E17;
							}
							difficulty2 = intRange2;
						}
						this._destroyBlockBehaviors.Add(new EntityAlive.DestroyBlockBehavior(text5, array2[j], num6, ragePer2, rageTime2, difficulty2));
					}
				}
				IL_E17:;
			}
		}
		this.distractionResistance = EffectManager.GetValue(PassiveEffects.DistractionResistance, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.distractionResistanceWithTarget = EffectManager.GetValue(PassiveEffects.DistractionResistance, null, 0f, this, null, EntityAlive.DistractionResistanceWithTargetTags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000CFE18 File Offset: 0x000CE018
	public void SetInventorySlots(string _handItemName)
	{
		if (_handItemName.Contains(" "))
		{
			_handItemName = _handItemName.Replace(" ", "");
		}
		string[] array = _handItemName.Split(",", StringSplitOptions.None);
		if (this.inventory != null)
		{
			ItemStack[] array2 = new ItemStack[array.Length - 1];
			for (int i = 1; i < array.Length; i++)
			{
				string text = array[i];
				ItemStack itemStack;
				if (text.Length == 0)
				{
					itemStack = ItemStack.Empty;
				}
				else
				{
					itemStack = ItemStack.FromString(text);
					if (itemStack.itemValue.IsEmpty())
					{
						Log.Error("HandItem missing " + text);
					}
				}
				array2[i - 1] = itemStack;
			}
			this.inventory.SetSlots(array2, true);
		}
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000CFEC4 File Offset: 0x000CE0C4
	public static int GetSpawnWalkType(EntityClass _entityClass)
	{
		int result = 0;
		_entityClass.Properties.ParseInt(EntityClass.PropWalkType, ref result);
		return result;
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000CFEE8 File Offset: 0x000CE0E8
	public override void VisiblityCheck(float _distanceSqr, bool _isZoom)
	{
		if ((this.entityFlags & EntityFlags.AIHearing) > EntityFlags.None)
		{
			if (GameManager.IsDedicatedServer)
			{
				this.emodel.SetVisible(true, false);
				return;
			}
			if (_distanceSqr < (float)(_isZoom ? 14400 : 8100))
			{
				this.renderFadeTarget = this.renderFadeMax;
				return;
			}
			this.renderFadeTarget = 0f;
		}
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000CFF43 File Offset: 0x000CE143
	public virtual void SetSleeper()
	{
		this.IsSleeper = true;
		this.aiManager.pathCostScale += 0.2f;
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000CFF63 File Offset: 0x000CE163
	public void SetSleeperSight(float angle, float range)
	{
		if (angle < 0f)
		{
			angle = this.maxViewAngle;
		}
		this.sleeperViewAngle = angle;
		if (range < 0f)
		{
			range = Utils.FastMax(3f, this.sightRangeBase * 0.2f);
		}
		this.sleeperSightRange = range;
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000CFFA3 File Offset: 0x000CE1A3
	public void SetSleeperHearing(float percent)
	{
		if (percent < 0.001f)
		{
			percent = 0.001f;
		}
		percent = 1f / percent;
		this.sleeperNoiseToSense *= percent;
		this.sleeperNoiseToWake *= percent;
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000CFFDC File Offset: 0x000CE1DC
	public int GetSleeperDisturbedLevel(float dist, float lightLevel)
	{
		float num = dist / this.sightRangeBase;
		if (num <= 1f)
		{
			float num2 = Mathf.Lerp(this.sightWakeThresholdAtRange.x, this.sightWakeThresholdAtRange.y, num);
			if (lightLevel > num2)
			{
				return 2;
			}
			float num3 = Mathf.Lerp(this.sightGroanThresholdAtRange.x, this.sightGroanThresholdAtRange.y, num);
			if (lightLevel > num3)
			{
				return 1;
			}
		}
		return 0;
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000D0044 File Offset: 0x000CE244
	public void GetSleeperDebugScale(float dist, out float wake, out float groan)
	{
		float t = dist / this.sightRangeBase;
		wake = Mathf.Lerp(this.sightWakeThresholdAtRange.x, this.sightWakeThresholdAtRange.y, t);
		groan = Mathf.Lerp(this.sightGroanThresholdAtRange.x, this.sightGroanThresholdAtRange.y, t);
	}

	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x06002258 RID: 8792 RVA: 0x000D0096 File Offset: 0x000CE296
	public bool sleepingOrWakingUp
	{
		get
		{
			return this.IsSleeping;
		}
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000D00A0 File Offset: 0x000CE2A0
	public void TriggerSleeperPose(int _pose, bool _returningToSleep = false)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.TriggerSleeperPose(_pose, _returningToSleep);
			this.pendingSleepTrigger = -1;
			if (_pose != 5)
			{
				this.physicsHeight = 0.85f;
			}
		}
		else
		{
			this.pendingSleepTrigger = _pose;
		}
		this.lastSleeperPose = _pose;
		this.IsSleeping = true;
		this.SleeperSupressLivingSounds = true;
		this.sleeperLookDir = Quaternion.AngleAxis(this.rotation.y, Vector3.up) * this.SleeperSpawnLookDir;
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000D0141 File Offset: 0x000CE341
	public void ResumeSleeperPose()
	{
		this.TriggerSleeperPose(this.lastSleeperPose, true);
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000D0150 File Offset: 0x000CE350
	public void ConditionalTriggerSleeperWakeUp()
	{
		if (this.IsSleeping && !this.IsDead())
		{
			this.IsSleeping = false;
			this.IsSleeperPassive = false;
			int pose = (this.physicsHeight < 1f && !this.IsWalkTypeACrawl()) ? -2 : -1;
			this.emodel.avatarController.TriggerSleeperPose(pose, false);
			if (this.aiManager != null)
			{
				this.aiManager.SleeperWokeUp();
			}
			if (!this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSleeperWakeup>().Setup(this.entityId), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000D01FC File Offset: 0x000CE3FC
	public void SetSleeperActive()
	{
		if (this.IsSleeperPassive)
		{
			this.IsSleeperPassive = false;
			if (!this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSleeperPassiveChange>().Setup(this.entityId), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x000D0252 File Offset: 0x000CE452
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitStats()
	{
		this.entityStats = new EntityStats(this);
		this.startOfFrameStats = new EntityStats(this);
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000D026C File Offset: 0x000CE46C
	public void SetStats(EntityStats _stats)
	{
		this.entityStats.CopyFrom(_stats);
	}

	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x0600225F RID: 8799 RVA: 0x000D027A File Offset: 0x000CE47A
	public EntityStats Stats
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.entityStats;
		}
	}

	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002260 RID: 8800 RVA: 0x000D0282 File Offset: 0x000CE482
	public EntityStats StartOfFrameStats
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.startOfFrameStats;
		}
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000D028A File Offset: 0x000CE48A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual ItemValue GetHandItem()
	{
		return this.handItem;
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000D0292 File Offset: 0x000CE492
	public bool IsHoldingLight()
	{
		return this.inventory.IsFlashlightOn;
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000027FC File Offset: 0x000009FC
	public void CycleActivatableItems()
	{
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000D02A0 File Offset: 0x000CE4A0
	public List<ItemValue> GetActivatableItemPool()
	{
		List<ItemValue> list = new List<ItemValue>();
		this.CollectActivatableItems(list);
		return list;
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000D02BC File Offset: 0x000CE4BC
	public void CollectActivatableItems(List<ItemValue> _pool)
	{
		if (this.inventory != null)
		{
			EntityAlive.GetActivatableItems(this.inventory.holdingItemItemValue, _pool);
		}
		if (this.equipment != null)
		{
			int slotCount = this.equipment.GetSlotCount();
			for (int i = 0; i < slotCount; i++)
			{
				EntityAlive.GetActivatableItems(this.equipment.GetSlotItemOrNone(i), _pool);
			}
		}
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000D0314 File Offset: 0x000CE514
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetActivatableItems(ItemValue _item, List<ItemValue> _itemPool)
	{
		ItemClass itemClass = _item.ItemClass;
		if (itemClass == null)
		{
			return;
		}
		if (itemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
		{
			_itemPool.Add(_item);
		}
		for (int i = 0; i < _item.Modifications.Length; i++)
		{
			ItemValue itemValue = _item.Modifications[i];
			if (itemValue != null)
			{
				ItemClass itemClass2 = itemValue.ItemClass;
				if (itemClass2 != null && itemClass2.HasTrigger(MinEventTypes.onSelfItemActivate))
				{
					_itemPool.Add(itemValue);
				}
			}
		}
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000D0378 File Offset: 0x000CE578
	public override void OnUpdatePosition(float _partialTicks)
	{
		float rotYDelta = Utils.DeltaAngle(this.rotation.y, this.prevRotation.y);
		base.OnUpdatePosition(_partialTicks);
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.lastTickPos.Length - 1; i++)
		{
			vector.x += this.lastTickPos[i].x - this.lastTickPos[i + 1].x;
			vector.z += this.lastTickPos[i].z - this.lastTickPos[i + 1].z;
		}
		vector += this.position - this.lastTickPos[0];
		vector /= (float)this.lastTickPos.Length;
		if (this.AttachedToEntity == null)
		{
			this.updateStepSound(vector.x, vector.z, rotYDelta);
		}
		if (!this.RootMotion && !this.isEntityRemote)
		{
			this.updateSpeedForwardAndStrafe(vector, _partialTicks);
		}
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000D048C File Offset: 0x000CE68C
	public void Snore()
	{
		if (!this.isSnore && this.isGroan && this.snoreGroanCD <= 0)
		{
			this.isSnore = true;
			this.isGroan = false;
			this.snoreGroanCD = this.rand.RandomRange(20, 21);
			if (this.soundSleeperSnore != null && !this.isGroanSilent)
			{
				Manager.BroadcastPlay(this, this.soundSleeperSnore, false, 1f);
			}
		}
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000D04F8 File Offset: 0x000CE6F8
	public void Groan()
	{
		if (!this.isGroan && this.snoreGroanCD <= 0)
		{
			this.isGroan = true;
			this.isSnore = false;
			this.snoreGroanCD = this.rand.RandomRange(20, 21);
			if (this.sleeperNoiseToSenseSoundChance >= 1f || this.rand.RandomFloat <= this.sleeperNoiseToSenseSoundChance)
			{
				this.isGroanSilent = false;
				if (this.soundSleeperGroan != null)
				{
					Manager.BroadcastPlay(this, this.soundSleeperGroan, false, 1f);
					return;
				}
			}
			else
			{
				this.isGroanSilent = true;
			}
		}
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000D0584 File Offset: 0x000CE784
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		this.Buffs.SetCustomVar("_underwater", this.inWaterPercent, true, CVarOperation.set, false);
		if (this.Buffs != null)
		{
			this.Buffs.Tick();
		}
		if (this.StressAmount > 0f)
		{
			if (base.EntityClass.PickupStressCvar != "")
			{
				this.SetCVar(base.EntityClass.PickupStressCvar, this.StressAmount);
			}
			if (base.EntityClass.PickupStressBuff != "")
			{
				this.Buffs.AddBuff(base.EntityClass.PickupStressBuff, -1, true, false, -1f);
			}
			this.StressAmount = 0f;
		}
		this.OnUpdateLive();
		if (!this.IsSleeping && (!this.isEntityRemote || !(this is EntityPlayer)) && this.inventory != null)
		{
			this.inventory.OnUpdate();
		}
		if (this.Health <= 0 && !this.IsDead() && !this.isEntityRemote && !this.IsGodMode.Value)
		{
			if (this.Buffs.HasBuff("drowning"))
			{
				this.DamageEntity(DamageSource.suffocating, 1, false, 1f);
			}
			else
			{
				this.DamageEntity(DamageSource.disease, 1, false, 1f);
			}
		}
		if (base.IsAlive() && this.bPlayHurtSound)
		{
			string text = this.GetSoundHurt(this.woundedDamageSource, this.woundedStrength);
			if (text != null)
			{
				this.PlayOneShot(text, false, false, false, null, 1f);
			}
		}
		this.bPlayHurtSound = false;
		this.bBeenWounded = false;
		this.woundedStrength = 0;
		this.woundedDamageSource = null;
		if (this.snoreGroanCD > 0)
		{
			this.snoreGroanCD--;
		}
		if (!this.IsDead() && !this.isEntityRemote)
		{
			if (this.isRadiationSensitive() && this.biomeStandingOn != null && this.biomeStandingOn.m_RadiationLevel > 0 && !this.IsGodMode.Value && this.world.worldTime % 20UL == 0UL)
			{
				this.DamageEntity(DamageSource.radiation, this.biomeStandingOn.m_RadiationLevel, false, 1f);
			}
			if (this.hasAI)
			{
				if (this.IsSleeping && this.pendingSleepTrigger > -1)
				{
					this.TriggerSleeperPose(this.pendingSleepTrigger, false);
				}
				this.soundDelayTicks--;
				if (this.attackingTime <= 0 && this.soundDelayTicks <= 0 && this.aiClosestPlayerDistSq <= 400f && this.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.SleeperSupressLivingSounds)
				{
					if (this.targetAlertChanged)
					{
						this.targetAlertChanged = false;
						this.soundDelayTicks = this.GetSoundAlertTicks();
						if (this.GetSoundAlert() != null && !this.IsScoutZombie)
						{
							this.PlayOneShot(this.GetSoundAlert(), false, false, false, null, 1f);
						}
						this.OnEntityTargeted(this.attackTarget);
					}
					else
					{
						this.soundDelayTicks = this.GetSoundRandomTicks();
						this.attackTargetLast = null;
						if (this.GetSoundRandom() != null)
						{
							this.PlayOneShot(this.GetSoundRandom(), false, false, false, null, 1f);
						}
					}
				}
			}
		}
		if (this.hasBeenAttackedTime > 0)
		{
			this.hasBeenAttackedTime--;
		}
		if (this.painResistPercent > 0f)
		{
			this.painResistPercent -= 0.010000001f;
			if (this.painResistPercent <= 0f)
			{
				this.painHitsFelt = 0f;
			}
		}
		if (this.attackingTime > 0)
		{
			this.attackingTime--;
		}
		if (this.investigatePositionTicks > 0)
		{
			int num = this.investigatePositionTicks - 1;
			this.investigatePositionTicks = num;
			if (num == 0)
			{
				this.ClearInvestigatePosition();
			}
		}
		bool flag = this.IsDead();
		if (this.alertEnabled)
		{
			this.isAlert = this.bReplicatedAlertFlag;
			if (!this.isEntityRemote)
			{
				if (this.alertTicks > 0)
				{
					this.alertTicks--;
				}
				this.isAlert = (!flag && (this.alertTicks > 0 || this.attackTarget || (this.HasInvestigatePosition && this.isInvestigateAlert)));
				if (this.bReplicatedAlertFlag != this.isAlert)
				{
					this.bReplicatedAlertFlag = this.isAlert;
					this.bEntityAliveFlagsChanged = true;
				}
			}
			if (!this.isAlert && !flag)
			{
				this.Buffs.SetCustomVar(EntityAlive.notAlertedId, 1f, true, CVarOperation.set, false);
				this.notAlertDelayTicks = 4;
			}
			else
			{
				if (this.notAlertDelayTicks > 0)
				{
					this.notAlertDelayTicks--;
				}
				if (this.notAlertDelayTicks == 0)
				{
					this.Buffs.SetCustomVar(EntityAlive.notAlertedId, 0f, true, CVarOperation.set, false);
				}
			}
		}
		if (flag)
		{
			this.OnDeathUpdate();
		}
		if (this.revengeEntity != null)
		{
			if (!this.revengeEntity.IsAlive())
			{
				this.SetRevengeTarget(null);
				return;
			}
			if (this.revengeTimer > 0)
			{
				this.revengeTimer--;
				return;
			}
			this.SetRevengeTarget(null);
		}
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000D0A68 File Offset: 0x000CEC68
	public override void KillLootContainer()
	{
		if (!this.isEntityRemote && this.IsDead() && !this.corpseBlockValue.isair && this.deathUpdateTime < this.timeStayAfterDeath)
		{
			this.deathUpdateTime = this.timeStayAfterDeath - 1;
		}
		base.KillLootContainer();
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000D0AB4 File Offset: 0x000CECB4
	public override void Kill(DamageResponse _dmResponse)
	{
		this.NotifySleeperDeath();
		if (this.AttachedToEntity != null)
		{
			this.Detach();
		}
		if (this.deathUpdateTime == 0)
		{
			string text = this.GetSoundDeath(_dmResponse.Source);
			if (text != null)
			{
				this.PlayOneShot(text, false, false, false, null, 1f);
			}
		}
		if (this.IsDead())
		{
			this.SetDead();
			return;
		}
		this.ClientKill(_dmResponse);
		base.Kill(_dmResponse);
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000D0B20 File Offset: 0x000CED20
	public override void SetDead()
	{
		base.SetDead();
		this.Stats.Health.Value = 0f;
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000D0B3D File Offset: 0x000CED3D
	public void NotifySleeperDeath()
	{
		if (!this.isEntityRemote && this.IsSleeper)
		{
			this.world.NotifySleeperVolumesEntityDied(this);
		}
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000D0B5B File Offset: 0x000CED5B
	public void ClearEntityThatKilledMe()
	{
		this.entityThatKilledMe = null;
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000D0B64 File Offset: 0x000CED64
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ClientKill(DamageResponse _dmResponse)
	{
		this.lastHitDirection = Utils.EnumHitDirection.Back;
		if (this.entityThatKilledMe == null && _dmResponse.Source != null)
		{
			Entity entity = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
			if (this.Spawned && entity is EntityAlive)
			{
				this.entityThatKilledMe = (EntityAlive)entity;
			}
		}
		if (!this.IsDead())
		{
			this.SetDead();
			if (this.Buffs != null)
			{
				this.Buffs.OnDeath(this.entityThatKilledMe, (_dmResponse.Source == null) ? null : _dmResponse.Source.AttackingItem, _dmResponse.Source != null && _dmResponse.Source.damageType == EnumDamageTypes.Crushing, (_dmResponse.Source == null) ? FastTags<TagGroup.Global>.Parse("crushing") : _dmResponse.Source.DamageTypeTag);
			}
			if (this.Progression != null)
			{
				this.Progression.OnDeath();
			}
			UnityEngine.Object x = this as EntityPlayer;
			this.AnalyticsSendDeath(_dmResponse);
			EntityAlive entityAlive = this.entityThatKilledMe;
			this.HandleClientDeath((_dmResponse.Source != null) ? _dmResponse.Source.BlockPosition : base.GetBlockPosition());
			this.OnEntityDeath();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.AwardKillXPServer(_dmResponse.Source, entityAlive);
				this.PartyShareKillServer(_dmResponse.Source, entityAlive);
			}
			if (x == null && base.EntityClass.bIsEnemyEntity && entityAlive is EntityPlayer && (EffectManager.GetValue(PassiveEffects.CelebrationKill, null, 0f, this.entityThatKilledMe, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f || EntityAlive.IsCelebrate || (EntityAlive.IsCelebrateHeadshot && _dmResponse.Source != null && _dmResponse.HitBodyPart == EnumBodyPartHit.Head)))
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
					this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("confetti", this.position, lightBrightness, Color.white, null, null, false, 1f, ""), this.entityId, false, true);
					Manager.BroadcastPlay(entityAlive, this.position, "twitch_celebrate");
					GameManager.Instance.World.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
					return;
				}
			}
			else
			{
				this.emodel.OnDeath(_dmResponse, this.world.ChunkCache);
			}
		}
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleClientDeath(Vector3i attackPos)
	{
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000D0DD8 File Offset: 0x000CEFD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void AwardKillXPServer(DamageSource _source, EntityAlive _killingEntity)
	{
		if (_source == null || _source.BuffClass != null)
		{
			return;
		}
		EntityPlayer entityPlayer = _killingEntity as EntityPlayer;
		if (!entityPlayer || entityPlayer == this)
		{
			return;
		}
		if (!EntityClass.list.ContainsKey(this.entityClass))
		{
			return;
		}
		float num = _source.KillXPScale;
		if (_source.bTrapKillXP)
		{
			num *= EffectManager.GetValue(PassiveEffects.ElectricalTrapXP, entityPlayer.inventory.holdingItemItemValue, 0f, entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		if (num <= 0f)
		{
			return;
		}
		entityPlayer.AddKillXP(this, _source.AttackingItem, num);
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000D0E74 File Offset: 0x000CF074
	[PublicizedFrom(EAccessModifier.Private)]
	public void PartyShareKillServer(DamageSource _source, EntityAlive _killingEntity)
	{
		if (!(_killingEntity is EntityPlayer) || _killingEntity == this)
		{
			return;
		}
		if (_source != null)
		{
			if (_source.bIgnorePartyShare || _source.bTrapKillXP || (int)_source.KillXPScale != 1)
			{
				return;
			}
			if (_source.BuffClass != null)
			{
				EntityBuffs buffs = this.Buffs;
				if (buffs != null && buffs.GetCustomVar("ETrapHit") == (float)1)
				{
					return;
				}
			}
		}
		GameManager.Instance.SharedKillServer(this.entityId, _killingEntity.entityId, 1f);
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntityTargeted(EntityAlive target)
	{
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000D0EF4 File Offset: 0x000CF0F4
	public void ForceHoldingWeaponUpdate()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(this), false, -1, this.entityId, -1, null, 192, false);
			return;
		}
		if (this.entityId > 0 && this as EntityPlayerLocal != null)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(this), false);
		}
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000D0F75 File Offset: 0x000CF175
	public virtual void SetHoldingItemTransform(Transform _transform)
	{
		this.emodel.SetInRightHand(_transform);
		this.ForceHoldingWeaponUpdate();
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnHoldingItemChanged()
	{
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateCameraFOV(bool _bLerpPosition)
	{
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000D0F89 File Offset: 0x000CF189
	public virtual int GetCameraFOV()
	{
		return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000D0F92 File Offset: 0x000CF192
	public virtual float GetWetnessRate()
	{
		return this.inWaterPercent;
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000D0F9C File Offset: 0x000CF19C
	public float GetAmountEnclosed()
	{
		Vector3 position = this.position;
		position.y += 0.5f;
		Vector3i vector3i = World.worldToBlockPos(position);
		if (vector3i.y < 255)
		{
			IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos(vector3i);
			if (chunkFromWorldPos != null)
			{
				float v = (float)chunkFromWorldPos.GetLight(vector3i.x, vector3i.y, vector3i.z, Chunk.LIGHT_TYPE.SUN);
				float v2 = (float)chunkFromWorldPos.GetLight(vector3i.x, vector3i.y + 1, vector3i.z, Chunk.LIGHT_TYPE.SUN);
				float num = Utils.FastMax(v, v2) / 15f;
				return 1f - num;
			}
		}
		return 1f;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000D1037 File Offset: 0x000CF237
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		base.OnHeadUnderwaterStateChanged(_bUnderwater);
		if (_bUnderwater)
		{
			this.FireEvent(MinEventTypes.onSelfWaterSubmerge, true);
			return;
		}
		this.FireEvent(MinEventTypes.onSelfWaterSurface, true);
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x0600227D RID: 8829 RVA: 0x000D1056 File Offset: 0x000CF256
	// (set) Token: 0x0600227E RID: 8830 RVA: 0x000D105E File Offset: 0x000CF25E
	public virtual bool JetpackActive
	{
		get
		{
			return this.bJetpackActive;
		}
		set
		{
			if (value != this.bJetpackActive)
			{
				this.bJetpackActive = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x0600227F RID: 8831 RVA: 0x000D1086 File Offset: 0x000CF286
	// (set) Token: 0x06002280 RID: 8832 RVA: 0x000D108E File Offset: 0x000CF28E
	public virtual bool JetpackWearing
	{
		get
		{
			return this.bJetpackWearing;
		}
		set
		{
			if (value != this.bJetpackWearing)
			{
				this.bJetpackWearing = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002281 RID: 8833 RVA: 0x000D10B6 File Offset: 0x000CF2B6
	// (set) Token: 0x06002282 RID: 8834 RVA: 0x000D10BE File Offset: 0x000CF2BE
	public virtual bool ParachuteWearing
	{
		get
		{
			return this.bParachuteWearing;
		}
		set
		{
			if (value != this.bParachuteWearing)
			{
				this.bParachuteWearing = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x06002283 RID: 8835 RVA: 0x000D10E6 File Offset: 0x000CF2E6
	// (set) Token: 0x06002284 RID: 8836 RVA: 0x000D1120 File Offset: 0x000CF320
	public virtual bool AimingGun
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.TryGetBool(AvatarController.isAimingHash, out this.bAimingGun) && this.bAimingGun;
		}
		set
		{
			bool aimingGun = this.AimingGun;
			if (value != aimingGun)
			{
				if (this.emodel.avatarController != null)
				{
					this.emodel.avatarController.UpdateBool(AvatarController.isAimingHash, value, true);
				}
				this.UpdateCameraFOV(true);
			}
			if (this is EntityPlayerLocal && this.inventory != null)
			{
				ItemAction itemAction = this.inventory.holdingItem.Actions[1];
				if (itemAction != null)
				{
					itemAction.AimingSet(this.inventory.holdingItemData.actionData[1], value, aimingGun);
				}
			}
			EntityPlayerLocal entityPlayerLocal = this as EntityPlayerLocal;
			if (entityPlayerLocal != null && value)
			{
				entityPlayerLocal.StartTPCameraLockTimer();
			}
		}
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000D11C4 File Offset: 0x000CF3C4
	public virtual Vector3 GetChestTransformPosition()
	{
		if (this.IsCrouching || this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
		{
			return base.transform.position + new Vector3(0f, this.GetEyeHeight() * 0.25f, 0f);
		}
		return base.transform.position + new Vector3(0f, this.GetEyeHeight() * 0.95f, 0f);
	}

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x06002286 RID: 8838 RVA: 0x000D124C File Offset: 0x000CF44C
	// (set) Token: 0x06002287 RID: 8839 RVA: 0x000D1254 File Offset: 0x000CF454
	public virtual bool MovementRunning
	{
		get
		{
			return this.bMovementRunning;
		}
		set
		{
			if (value != this.bMovementRunning)
			{
				this.bMovementRunning = value;
			}
		}
	}

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x06002288 RID: 8840 RVA: 0x000D1266 File Offset: 0x000CF466
	// (set) Token: 0x06002289 RID: 8841 RVA: 0x000D1270 File Offset: 0x000CF470
	public virtual bool Crouching
	{
		get
		{
			return this.bCrouching;
		}
		set
		{
			if (value != this.bCrouching)
			{
				this.bCrouching = value;
				if (this.emodel.avatarController != null)
				{
					this.emodel.avatarController.SetCrouching(value);
				}
				this.CurrentStanceTag = (this.bCrouching ? EntityAlive.StanceTagCrouching : EntityAlive.StanceTagStanding);
				this.Buffs.SetCustomVar("_crouching", (float)(this.bCrouching ? 1 : 0), true, CVarOperation.set, false);
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x0600228A RID: 8842 RVA: 0x000D1301 File Offset: 0x000CF501
	public bool IsCrouching
	{
		get
		{
			return this.Crouching || this.CrouchingLocked;
		}
	}

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x0600228B RID: 8843 RVA: 0x000D1314 File Offset: 0x000CF514
	// (set) Token: 0x0600228C RID: 8844 RVA: 0x000D1358 File Offset: 0x000CF558
	public virtual bool Jumping
	{
		get
		{
			return this.bJumping && EffectManager.GetValue(PassiveEffects.JumpStrength, null, 1f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) != 0f;
		}
		set
		{
			if (value != this.bJumping)
			{
				this.bJumping = value;
				if (this.Jumping)
				{
					this.StartJump();
					this.CurrentMovementTag &= EntityAlive.MovementTagIdle;
					this.CurrentMovementTag |= EntityAlive.MovementTagJumping;
				}
				else
				{
					this.EndJump();
					this.CurrentMovementTag &= EntityAlive.MovementTagJumping;
					this.bJumping = false;
				}
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x0600228D RID: 8845 RVA: 0x000D13EA File Offset: 0x000CF5EA
	// (set) Token: 0x0600228E RID: 8846 RVA: 0x000D13F4 File Offset: 0x000CF5F4
	public bool Climbing
	{
		get
		{
			return this.bClimbing;
		}
		set
		{
			if (value != this.bClimbing)
			{
				this.bClimbing = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (this.bClimbing)
				{
					this.CurrentMovementTag &= EntityAlive.MovementTagIdle;
					this.CurrentMovementTag |= EntityAlive.MovementTagClimbing;
					return;
				}
				this.CurrentMovementTag &= EntityAlive.MovementTagClimbing;
			}
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000D1472 File Offset: 0x000CF672
	public virtual bool CanNavigatePath()
	{
		return this.onGround || this.isSwimming || this.bInElevator || this.Climbing;
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000D1494 File Offset: 0x000CF694
	public AvatarController.ActionState GetAnimActionState()
	{
		if (this.emodel.avatarController)
		{
			return this.emodel.avatarController.GetActionState();
		}
		return AvatarController.ActionState.None;
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000D14BC File Offset: 0x000CF6BC
	public virtual void StartAnimAction(int _animType)
	{
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.GetActionState();
			if (_animType != 9999)
			{
				if (this.emodel.avatarController.IsActionActive())
				{
					return;
				}
			}
			else if (!this.emodel.avatarController.IsActionActive())
			{
				return;
			}
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.emodel.avatarController.StartAction(_animType);
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000D1541 File Offset: 0x000CF741
	public virtual void ContinueAnimAction(int _animType)
	{
		if (this.emodel.avatarController)
		{
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.emodel.avatarController.StartAction(_animType);
		}
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06002293 RID: 8851 RVA: 0x000D157C File Offset: 0x000CF77C
	// (set) Token: 0x06002294 RID: 8852 RVA: 0x000D15A3 File Offset: 0x000CF7A3
	public virtual bool RightArmAnimationAttack
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationAttackPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value && !this.emodel.avatarController.IsAnimationAttackPlaying())
			{
				this.emodel.avatarController.StartAnimationAttack();
			}
		}
	}

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06002295 RID: 8853 RVA: 0x000D15DD File Offset: 0x000CF7DD
	// (set) Token: 0x06002296 RID: 8854 RVA: 0x000D1604 File Offset: 0x000CF804
	public virtual bool RightArmAnimationUse
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationUsePlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value != this.emodel.avatarController.IsAnimationUsePlaying())
			{
				this.emodel.avatarController.StartAnimationUse();
			}
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06002297 RID: 8855 RVA: 0x000D163C File Offset: 0x000CF83C
	// (set) Token: 0x06002298 RID: 8856 RVA: 0x000D1664 File Offset: 0x000CF864
	public virtual bool SpecialAttack
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationSpecialAttackPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value != this.emodel.avatarController.IsAnimationSpecialAttackPlaying())
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				this.emodel.avatarController.StartAnimationSpecialAttack(value, 0);
			}
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06002299 RID: 8857 RVA: 0x000D16BF File Offset: 0x000CF8BF
	// (set) Token: 0x0600229A RID: 8858 RVA: 0x000D16E6 File Offset: 0x000CF8E6
	public virtual bool SpecialAttack2
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationSpecialAttack2Playing();
		}
		set
		{
			if (this.emodel.avatarController != null && value)
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				this.emodel.avatarController.StartAnimationSpecialAttack2();
			}
		}
	}

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x0600229B RID: 8859 RVA: 0x000D1723 File Offset: 0x000CF923
	// (set) Token: 0x0600229C RID: 8860 RVA: 0x000D174A File Offset: 0x000CF94A
	public virtual bool Raging
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationRagingPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value && !this.emodel.avatarController.IsAnimationRagingPlaying())
			{
				this.emodel.avatarController.StartAnimationRaging();
			}
		}
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x0600229D RID: 8861 RVA: 0x000D1784 File Offset: 0x000CF984
	// (set) Token: 0x0600229E RID: 8862 RVA: 0x000D17C0 File Offset: 0x000CF9C0
	public virtual bool Electrocuted
	{
		get
		{
			return this.emodel != null && this.emodel.avatarController != null && this.emodel.avatarController.GetAnimationElectrocuteRemaining() > 0f;
		}
		set
		{
			if (this.emodel != null && this.emodel.avatarController != null && value != this.emodel.avatarController.GetAnimationElectrocuteRemaining() > 0.4f)
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (value)
				{
					this.emodel.avatarController.StartAnimationElectrocute(0.6f);
					this.emodel.avatarController.Electrocute(true);
				}
			}
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x0600229F RID: 8863 RVA: 0x000D1847 File Offset: 0x000CFA47
	// (set) Token: 0x060022A0 RID: 8864 RVA: 0x000D186E File Offset: 0x000CFA6E
	public virtual bool HarvestingAnimation
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationHarvestingPlaying();
		}
		set
		{
			this.emodel.avatarController.UpdateBool("Harvesting", value, true);
		}
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000D1887 File Offset: 0x000CFA87
	public virtual void StartHarvestingAnim(float _length, bool _weaponFireTrigger)
	{
		if (this.emodel != null && this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationHarvesting(_length, _weaponFireTrigger);
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x060022A2 RID: 8866 RVA: 0x000D18BC File Offset: 0x000CFABC
	// (set) Token: 0x060022A3 RID: 8867 RVA: 0x000D18C4 File Offset: 0x000CFAC4
	public bool IsEating
	{
		get
		{
			return this.m_isEating;
		}
		set
		{
			if (value != this.m_isEating)
			{
				this.m_isEating = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (this.emodel != null && this.emodel.avatarController != null)
				{
					if (this.m_isEating)
					{
						this.emodel.avatarController.StartEating();
						return;
					}
					this.emodel.avatarController.StopEating();
				}
			}
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000D1944 File Offset: 0x000CFB44
	public virtual void SetVehicleAnimation(int _animHash, int _pose)
	{
		if (this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.SetVehicleAnimation(_animHash, _pose);
			this.bPlayerStatsChanged = !this.isEntityRemote;
			if (_pose == -1)
			{
				AvatarLocalPlayerController avatarLocalPlayerController = this.emodel.avatarController as AvatarLocalPlayerController;
				if (avatarLocalPlayerController != null)
				{
					avatarLocalPlayerController.TPVResetAnimPose();
				}
			}
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000D19AF File Offset: 0x000CFBAF
	public virtual int GetVehicleAnimation()
	{
		if (this.emodel && this.emodel.avatarController)
		{
			return this.emodel.avatarController.GetVehicleAnimation();
		}
		return -1;
	}

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060022A6 RID: 8870 RVA: 0x000D19E2 File Offset: 0x000CFBE2
	// (set) Token: 0x060022A7 RID: 8871 RVA: 0x000D19EA File Offset: 0x000CFBEA
	public virtual int Died
	{
		get
		{
			return this.died;
		}
		set
		{
			if (value != this.died)
			{
				this.died = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060022A8 RID: 8872 RVA: 0x000D1A12 File Offset: 0x000CFC12
	// (set) Token: 0x060022A9 RID: 8873 RVA: 0x000D1A1A File Offset: 0x000CFC1A
	public virtual int Score
	{
		get
		{
			return this.score;
		}
		set
		{
			if (value != this.score)
			{
				this.score = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060022AA RID: 8874 RVA: 0x000D1A42 File Offset: 0x000CFC42
	// (set) Token: 0x060022AB RID: 8875 RVA: 0x000D1A4A File Offset: 0x000CFC4A
	public virtual int KilledZombies
	{
		get
		{
			return this.killedZombies;
		}
		set
		{
			if (value != this.killedZombies)
			{
				this.killedZombies = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060022AC RID: 8876 RVA: 0x000D1A72 File Offset: 0x000CFC72
	// (set) Token: 0x060022AD RID: 8877 RVA: 0x000D1A7A File Offset: 0x000CFC7A
	public virtual int KilledPlayers
	{
		get
		{
			return this.killedPlayers;
		}
		set
		{
			if (value != this.killedPlayers)
			{
				this.killedPlayers = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060022AE RID: 8878 RVA: 0x000D1AA2 File Offset: 0x000CFCA2
	// (set) Token: 0x060022AF RID: 8879 RVA: 0x000D1AAA File Offset: 0x000CFCAA
	public virtual int TeamNumber
	{
		get
		{
			return this.teamNumber;
		}
		set
		{
			if (value != this.teamNumber)
			{
				this.teamNumber = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (!this.isEntityRemote)
				{
					GameManager.Instance.GameMessage(EnumGameMessages.ChangedTeam, this, null);
				}
			}
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x060022B0 RID: 8880 RVA: 0x000D1AE7 File Offset: 0x000CFCE7
	public virtual string EntityName
	{
		get
		{
			return this.entityName;
		}
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000D1AEF File Offset: 0x000CFCEF
	public override void SetEntityName(string _name)
	{
		if (!_name.Equals(this.entityName))
		{
			this.entityName = _name;
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.HandleSetNavName();
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x060022B2 RID: 8882 RVA: 0x000D1B22 File Offset: 0x000CFD22
	// (set) Token: 0x060022B3 RID: 8883 RVA: 0x000D1B2A File Offset: 0x000CFD2A
	public virtual int DeathHealth
	{
		get
		{
			return this.deathHealth;
		}
		set
		{
			if (value != this.deathHealth)
			{
				this.deathHealth = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x060022B4 RID: 8884 RVA: 0x000D1B52 File Offset: 0x000CFD52
	// (set) Token: 0x060022B5 RID: 8885 RVA: 0x000D1B5A File Offset: 0x000CFD5A
	public virtual bool Spawned
	{
		get
		{
			return this.bSpawned;
		}
		set
		{
			if (value != this.bSpawned)
			{
				this.bSpawned = value;
				this.onSpawnStateChanged();
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x060022B6 RID: 8886 RVA: 0x000D1B88 File Offset: 0x000CFD88
	// (set) Token: 0x060022B7 RID: 8887 RVA: 0x000D1B90 File Offset: 0x000CFD90
	public bool IsBreakingBlocks
	{
		get
		{
			return this.m_isBreakingBlocks;
		}
		set
		{
			if (value != this.m_isBreakingBlocks)
			{
				this.m_isBreakingBlocks = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000D1B52 File Offset: 0x000CFD52
	public override bool IsSpawned()
	{
		return this.bSpawned;
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x060022B9 RID: 8889 RVA: 0x000D1BB8 File Offset: 0x000CFDB8
	public virtual EntityBedrollPositionList SpawnPoints
	{
		get
		{
			return this.spawnPoints;
		}
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000D1BC0 File Offset: 0x000CFDC0
	public virtual void RemoveIKTargets()
	{
		this.emodel.RemoveIKController();
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000D1BD0 File Offset: 0x000CFDD0
	public virtual void SetIKTargets(List<IKController.Target> targets)
	{
		IKController ikcontroller = this.emodel.AddIKController();
		if (ikcontroller)
		{
			ikcontroller.SetTargets(targets);
		}
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000D1BF8 File Offset: 0x000CFDF8
	public virtual List<Vector3i> GetDroppedBackpackPositions()
	{
		return this.droppedBackpackPositions;
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000D1C00 File Offset: 0x000CFE00
	public virtual Vector3i GetLastDroppedBackpackPosition()
	{
		if (this.droppedBackpackPositions == null)
		{
			return Vector3i.zero;
		}
		if (this.droppedBackpackPositions.Count == 0)
		{
			return Vector3i.zero;
		}
		List<Vector3i> list = this.droppedBackpackPositions;
		return list[list.Count - 1];
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000D1C38 File Offset: 0x000CFE38
	public virtual bool EqualsDroppedBackpackPositions(Vector3i position)
	{
		if (this.droppedBackpackPositions != null)
		{
			foreach (Vector3i other in this.droppedBackpackPositions)
			{
				if (position.Equals(other))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000D1CA0 File Offset: 0x000CFEA0
	public virtual void SetDroppedBackpackPositions(List<Vector3i> positions)
	{
		this.droppedBackpackPositions.Clear();
		if (positions != null)
		{
			this.droppedBackpackPositions.AddRange(positions);
		}
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000D1CBC File Offset: 0x000CFEBC
	public virtual void ClearDroppedBackpackPositions()
	{
		this.droppedBackpackPositions.Clear();
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x060022C1 RID: 8897 RVA: 0x000D1CC9 File Offset: 0x000CFEC9
	// (set) Token: 0x060022C2 RID: 8898 RVA: 0x000D1CDC File Offset: 0x000CFEDC
	public virtual int Health
	{
		get
		{
			return (int)this.Stats.Health.Value;
		}
		set
		{
			this.Stats.Health.Value = (float)value;
		}
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000D1CF0 File Offset: 0x000CFEF0
	// (set) Token: 0x060022C4 RID: 8900 RVA: 0x000D1D02 File Offset: 0x000CFF02
	public virtual float Stamina
	{
		get
		{
			return this.Stats.Stamina.Value;
		}
		set
		{
			this.Stats.Stamina.Value = value;
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000D1D15 File Offset: 0x000CFF15
	// (set) Token: 0x060022C6 RID: 8902 RVA: 0x000D1D27 File Offset: 0x000CFF27
	public virtual float Water
	{
		get
		{
			return this.Stats.Water.Value;
		}
		set
		{
			this.Stats.Water.Value = value;
		}
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000D1D3A File Offset: 0x000CFF3A
	public virtual int GetMaxHealth()
	{
		return (int)this.Stats.Health.Max;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000D1D4D File Offset: 0x000CFF4D
	public virtual int GetMaxStamina()
	{
		return (int)this.Stats.Stamina.Max;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000D1D60 File Offset: 0x000CFF60
	public virtual int GetMaxWater()
	{
		return (int)this.Stats.Water.Max;
	}

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x060022CA RID: 8906 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x060022CB RID: 8907 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsValidAimAssistSnapTarget
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x060022CC RID: 8908 RVA: 0x000D1D73 File Offset: 0x000CFF73
	// (set) Token: 0x060022CD RID: 8909 RVA: 0x000D1D7B File Offset: 0x000CFF7B
	public virtual EModelBase.HeadStates CurrentHeadState
	{
		get
		{
			return this.currentHeadState;
		}
		set
		{
			if (value != this.currentHeadState)
			{
				this.currentHeadState = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
			this.emodel.ForceHeadState(value);
		}
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public virtual float GetStaminaMultiplier()
	{
		return 1f;
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000D1DB0 File Offset: 0x000CFFB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetMovementState()
	{
		float num = this.speedStrafe;
		if (num >= 1234f)
		{
			num = 0f;
		}
		float num2 = this.speedForward * this.speedForward + num * num;
		this.MovementState = ((num2 > this.moveSpeedAggro * this.moveSpeedAggro) ? 3 : ((num2 > this.moveSpeed * this.moveSpeed) ? 2 : ((num2 > 0.001f) ? 1 : 0)));
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000D1E1C File Offset: 0x000D001C
	public virtual void OnUpdateLive()
	{
		this.Stats.Health.RegenerationAmount = 0f;
		if (!this.isEntityRemote && !this.IsDead())
		{
			this.Stats.Tick(this.world.worldTime);
			if (this.attractPlayer)
			{
				int num = this.attractPlayerTimeoutTicks - 1;
				this.attractPlayerTimeoutTicks = num;
				if (num <= 0)
				{
					this.attractPlayer = null;
				}
			}
		}
		if (this.jumpTicks > 0)
		{
			this.jumpTicks--;
		}
		if (this.attackTargetTime > 0)
		{
			this.attackTargetTime--;
			if (this.attackTarget != null && this.attackTargetTime == 0)
			{
				this.attackTarget = null;
				if (!this.isEntityRemote)
				{
					this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, -1, NetPackageManager.GetPackage<NetPackageSetAttackTarget>().Setup(this.entityId, -1), false);
				}
			}
		}
		this.updateCurrentBlockPosAndValue();
		if (this.AttachedToEntity == null)
		{
			if (this.isEntityRemote)
			{
				if (this.RootMotion)
				{
					this.MoveEntityHeaded(Vector3.zero, false);
				}
			}
			else
			{
				if (this.Health <= 0)
				{
					this.bJumping = false;
					this.bClimbing = false;
					this.moveDirection = Vector3.zero;
					this.renderFadeMax = 1f;
				}
				else if (!this.world.IsRemote() && !this.IsDead() && !this.IsClientControlled() && this.hasAI)
				{
					this.updateTasks();
				}
				this.noisePlayer = null;
				this.noisePlayerDistance = 0f;
				this.noisePlayerVolume = 0f;
				if (this.bJumping)
				{
					this.UpdateJump();
				}
				else
				{
					this.jumpTicks = 0;
				}
				float num2 = this.landMovementFactor;
				this.landMovementFactor *= this.GetSpeedModifier();
				this.MoveEntityHeaded(this.moveDirection, this.isMoveDirAbsolute);
				this.landMovementFactor = num2;
			}
			if (this.moveDirection.x > 0f || this.moveDirection.z > 0f)
			{
				if (this.bMovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagRunning;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagWalking;
				}
			}
			else
			{
				this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			}
		}
		if (this.bodyDamage.CurrentStun != EnumEntityStunType.None && !this.emodel.IsRagdollActive && !this.IsDead())
		{
			if (this.bodyDamage.CurrentStun == EnumEntityStunType.Getup)
			{
				if (!this.emodel.avatarController || !this.emodel.avatarController.IsAnimationStunRunning())
				{
					this.ClearStun();
				}
			}
			else
			{
				this.bodyDamage.StunDuration = this.bodyDamage.StunDuration - 0.05f;
				if (this.bodyDamage.StunDuration <= 0f)
				{
					this.SetStun(EnumEntityStunType.Getup);
					if (this.emodel.avatarController)
					{
						this.emodel.avatarController.EndStun();
					}
				}
			}
		}
		this.proneRefillCounter += 0.05f * this.proneRefillRate;
		while (this.proneRefillCounter >= 1f)
		{
			this.bodyDamage.StunProne = Mathf.Max(0, this.bodyDamage.StunProne - 1);
			this.proneRefillCounter -= 1f;
		}
		this.kneelRefillCounter += 0.05f * this.kneelRefillRate;
		while (this.kneelRefillCounter >= 1f)
		{
			this.bodyDamage.StunKnee = Mathf.Max(0, this.bodyDamage.StunKnee - 1);
			this.kneelRefillCounter -= 1f;
		}
		EntityPlayer primaryPlayer = this.world.GetPrimaryPlayer();
		if (primaryPlayer != null && primaryPlayer != this)
		{
			int num = this.ticksToCheckSeenByPlayer - 1;
			this.ticksToCheckSeenByPlayer = num;
			if (num <= 0)
			{
				this.wasSeenByPlayer = primaryPlayer.CanSee(this);
				if (this.wasSeenByPlayer)
				{
					this.ticksToCheckSeenByPlayer = 200;
				}
				else
				{
					this.ticksToCheckSeenByPlayer = 20;
				}
			}
			else if (this.wasSeenByPlayer)
			{
				primaryPlayer.SetCanSee(this);
			}
		}
		if (this.onGround)
		{
			this.disableFallBehaviorUntilOnGround = false;
		}
		this.UpdateDynamicRagdoll();
		this.checkForTeleportOutOfTraderArea();
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000D2244 File Offset: 0x000D0444
	[PublicizedFrom(EAccessModifier.Protected)]
	public void checkForTeleportOutOfTraderArea()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !GameManager.Instance.IsEditMode() && !this.IsGodMode.Value)
		{
			EntityPlayer entityPlayer = this as EntityPlayer;
			if (entityPlayer != null && Time.time - this.lastTimeTraderStationChecked > 0.1f)
			{
				this.lastTimeTraderStationChecked = Time.time;
				Vector3 position = this.position;
				position.y += 0.5f;
				Vector3i vector3i = World.worldToBlockPos(position);
				TraderArea traderAreaAt = this.world.GetTraderAreaAt(vector3i);
				if (traderAreaAt != null && traderAreaAt.IsInitialized)
				{
					Vector3 targetPos = default(Vector3);
					int num = 0;
					PrefabTeleportVolume prefabTeleportVolume;
					if (TraderInfo.TraderHoursPreset != TraderInfo.TraderHourPresets.AlwaysOpen && this.world.IsWorldEvent(World.WorldEvent.BloodMoon) && traderAreaAt.IsWithinProtectArea(position))
					{
						targetPos = traderAreaAt.ProtectPosition + traderAreaAt.ProtectSize * 0.5f;
						num = Math.Max(traderAreaAt.ProtectSize.x, traderAreaAt.ProtectSize.z) / 2;
					}
					else if (traderAreaAt.IsWithinTeleportArea(position, out prefabTeleportVolume) && (traderAreaAt.IsClosed || EffectManager.GetValue(PassiveEffects.NoTrader, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f))
					{
						PrefabInstance poiatPosition = this.world.GetPOIAtPosition(vector3i, null, null);
						if (poiatPosition == null)
						{
							return;
						}
						targetPos = poiatPosition.boundingBoxPosition + traderAreaAt.PrefabSize * 0.5f;
						num = Math.Max(traderAreaAt.PrefabSize.x, traderAreaAt.PrefabSize.z) / 2;
					}
					if (num > 0)
					{
						num += this.traderTeleportStreak;
						this.traderTeleportStreak++;
						Vector3 vector;
						if (!this.world.GetRandomSpawnPositionMinMaxToPosition(targetPos, num, num + 1, 1, false, out vector, this.entityId, true, 20, true, EnumLandClaimOwner.Ally, true))
						{
							Log.Warning("Trader teleport: Could not find a valid teleport position, returning original position");
							return;
						}
						if (this.isEntityRemote)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(this.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(vector, null, false));
						}
						else if (entityPlayer)
						{
							entityPlayer.Teleport(vector, float.MinValue);
						}
						else if (this.AttachedToEntity != null)
						{
							this.AttachedToEntity.SetPosition(vector, true);
						}
						else
						{
							this.SetPosition(vector, true);
						}
						if (entityPlayer)
						{
							GameEventManager.Current.HandleAction("game_on_trader_teleport", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
							return;
						}
					}
				}
				else
				{
					this.traderTeleportStreak = 1;
				}
			}
		}
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000D2508 File Offset: 0x000D0708
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJump()
	{
		this.jumpState = EntityAlive.JumpState.Leap;
		this.jumpStateTicks = 0;
		this.jumpDistance = 1f;
		this.jumpHeightDiff = 0f;
		this.disableFallBehaviorUntilOnGround = true;
		if (this.isSwimming)
		{
			this.jumpState = EntityAlive.JumpState.SwimStart;
			if (this.emodel.avatarController != null)
			{
				this.emodel.avatarController.SetSwim(true);
				return;
			}
		}
		else if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Start);
		}
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000D2598 File Offset: 0x000D0798
	public virtual void SetJumpDistance(float _distance, float _heightDiff)
	{
		this.jumpDistance = _distance;
		this.jumpHeightDiff = _heightDiff;
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000D25A8 File Offset: 0x000D07A8
	public virtual void SetSwimValues(float _durationTicks, Vector3 _motion)
	{
		this.jumpSwimDurationTicks = Mathf.Clamp(_durationTicks / this.swimSpeed - 6f, 3f, 20f);
		this.jumpSwimMotion = _motion;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000D25D4 File Offset: 0x000D07D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateJump()
	{
		if (this.IsFlyMode.Value)
		{
			this.Jumping = false;
			return;
		}
		this.jumpStateTicks++;
		switch (this.jumpState)
		{
		case EntityAlive.JumpState.Leap:
			if (this.accumulatedRootMotion.y > 0.005f || (float)this.jumpStateTicks >= this.jumpDelay)
			{
				this.StartJumpMotion();
				this.jumpTicks = 200;
				this.jumpState = EntityAlive.JumpState.Air;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = true;
				return;
			}
			break;
		case EntityAlive.JumpState.Air:
			if (this.onGround || (this.motionMultiplier < 0.45f && this.jumpStateTicks > 40))
			{
				this.jumpState = EntityAlive.JumpState.Land;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = false;
				return;
			}
			break;
		case EntityAlive.JumpState.Land:
			if (this.jumpStateTicks > 5)
			{
				this.Jumping = false;
				return;
			}
			break;
		case EntityAlive.JumpState.SwimStart:
			if ((float)this.jumpStateTicks > 6f)
			{
				this.jumpTicks = 100;
				this.jumpState = EntityAlive.JumpState.Swim;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = true;
				this.StartJumpSwimMotion();
				return;
			}
			break;
		case EntityAlive.JumpState.Swim:
			if (!this.isSwimming || (float)this.jumpStateTicks >= this.jumpSwimDurationTicks)
			{
				this.Jumping = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000D270C File Offset: 0x000D090C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJumpSwimMotion()
	{
		if (this.inWaterPercent > 0.65f)
		{
			float num = Mathf.Sqrt(this.jumpSwimMotion.x * this.jumpSwimMotion.x + this.jumpSwimMotion.z * this.jumpSwimMotion.z) + 0.001f;
			float min = Mathf.Lerp(-0.6f, -0.05f, num * 0.8f);
			this.jumpSwimMotion.y = Utils.FastClamp(this.jumpSwimMotion.y, min, 1f);
			float num2 = this.jumpSwimDurationTicks;
			float num3 = (num2 - 1f) * this.world.Gravity * 0.025f * 0.4999f;
			num3 /= Mathf.Pow(0.91f, (num2 - 3f) * 0.91f * 0.115f);
			float t = (num2 - 1f) / 15f;
			float num4 = Mathf.LerpUnclamped(0.46f, 0.41860002f, t);
			float num5 = Mathf.Pow(0.91f, (num2 - 1f) * num4);
			float num6 = 1f / num2 / num5;
			num3 += this.jumpSwimMotion.y * num6;
			num6 /= Utils.FastMax(1f, num);
			this.motion.x = this.jumpSwimMotion.x * num6;
			this.motion.z = this.jumpSwimMotion.z * num6;
			this.motion.y = num3;
			return;
		}
		this.motion.y = 0f;
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000D2898 File Offset: 0x000D0A98
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FaceJumpTo()
	{
		Vector3 vector = this.moveHelper.JumpToPos - this.position;
		float yaw = Mathf.Round(Mathf.Atan2(vector.x, vector.z) * 57.29578f / 90f) * 90f;
		base.SeekYaw(yaw, 0f, 0f);
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000D28F8 File Offset: 0x000D0AF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJumpMotion()
	{
		base.SetAirBorne(true);
		float num = (float)((int)(5f + Mathf.Pow(this.jumpDistance * 8f, 0.5f)));
		this.motion = this.GetForwardVector() * (this.jumpDistance / num);
		float num2 = num * this.world.Gravity * 0.5f;
		this.motion.y = Utils.FastMax(num2 * 0.5f, num2 + this.jumpHeightDiff / num);
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000D297C File Offset: 0x000D0B7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void JumpMove()
	{
		this.accumulatedRootMotion = Vector3.zero;
		Vector3 motion = this.motion;
		this.entityCollision(this.motion);
		this.motion.x = motion.x;
		this.motion.z = motion.z;
		if (this.motion.y != 0f)
		{
			this.motion.y = motion.y;
		}
		if (this.jumpState == EntityAlive.JumpState.Air)
		{
			this.motion.y = this.motion.y - this.world.Gravity;
			return;
		}
		this.motion.x = this.motion.x * 0.91f;
		this.motion.z = this.motion.z * 0.91f;
		this.motion.y = this.motion.y - this.world.Gravity * 0.025f;
		this.motion.y = this.motion.y * 0.91f;
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000D2A6C File Offset: 0x000D0C6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void EndJump()
	{
		this.jumpState = EntityAlive.JumpState.Off;
		this.jumpIsMoving = false;
		if (!this.isEntityRemote && this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Land);
		}
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000D2AA8 File Offset: 0x000D0CA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CalcIfSwimming()
	{
		float num = (this.onGround || this.Jumping) ? 0.7f : 0.5f;
		return this.inWaterPercent >= num;
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000D2ADE File Offset: 0x000D0CDE
	public override void SwimChanged()
	{
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.SetSwim(this.isSwimming);
		}
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000D2B08 File Offset: 0x000D0D08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		this.updateNetworkStats();
		if (!this.isEntityRemote && this.RootMotion && this.lerpForwardSpeed)
		{
			float num = Utils.FastAbs(this.speedForward);
			float num2 = 0.06935714f;
			if (num > 0.01942f)
			{
				num2 = this.speedForwardTargetStep;
			}
			float num3 = Utils.FastMoveTowards(this.speedForward, this.speedForwardTarget, num2 * Time.deltaTime);
			if (num > 0.01942f && Utils.FastAbs(num3) <= 0.01942f)
			{
				num3 = 0.01942f;
			}
			this.speedForward = num3;
		}
		if (this.isHeadUnderwater != (this.Buffs.GetCustomVar("_underwater") == 1f))
		{
			this.Buffs.SetCustomVar("_underwater", (float)(this.isHeadUnderwater ? 1 : 0), true, CVarOperation.set, false);
		}
		this.MinEventContext.Area = this.boundingBox;
		this.MinEventContext.Biome = this.biomeStandingOn;
		this.MinEventContext.ItemValue = this.inventory.holdingItemItemValue;
		this.MinEventContext.BlockValue = this.blockValueStandingOn;
		this.MinEventContext.ItemInventoryData = this.inventory.holdingItemData;
		this.MinEventContext.Position = this.position;
		this.MinEventContext.Seed = this.entityId + Mathf.Abs(GameManager.Instance.World.Seed);
		this.MinEventContext.Transform = base.transform;
		FastTags<TagGroup.Global>.CombineTags(EntityClass.list[this.entityClass].Tags, this.inventory.holdingItem.ItemTags, this.CurrentStanceTag, this.CurrentMovementTag, ref this.MinEventContext.Tags);
		if (this.Progression != null)
		{
			this.Progression.Update();
		}
		if (this.renderFade != this.renderFadeTarget)
		{
			this.renderFade = Mathf.MoveTowards(this.renderFade, this.renderFadeTarget, Time.deltaTime);
			this.emodel.SetFade(this.renderFade);
			bool flag = this.renderFade > 0.01f;
			if (this.emodel.visible != flag)
			{
				this.emodel.SetVisible(flag, false);
			}
		}
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000D2D31 File Offset: 0x000D0F31
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		this.startOfFrameStats.CopyFrom(this.entityStats);
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000D2D44 File Offset: 0x000D0F44
	public virtual void OnDeathUpdate()
	{
		if (this.deathUpdateTime < this.timeStayAfterDeath)
		{
			this.deathUpdateTime++;
		}
		int deadBodyHitPoints = EntityClass.list[this.entityClass].DeadBodyHitPoints;
		if (deadBodyHitPoints > 0 && this.DeathHealth <= -deadBodyHitPoints)
		{
			this.deathUpdateTime = this.timeStayAfterDeath;
		}
		if (this.deathUpdateTime < this.timeStayAfterDeath)
		{
			return;
		}
		if (!this.isEntityRemote && !this.markedForUnload && this.particleOnDestroy != null && this.particleOnDestroy.Length > 0)
		{
			float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
			this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.particleOnDestroy, this.getHeadPosition(), lightBrightness, Color.white, null, null, false, 1f, ""), this.entityId, false, false);
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000D2E20 File Offset: 0x000D1020
	public void NotifyRootMotion(Animator animator)
	{
		this.accumulatedRootMotion += animator.deltaPosition;
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x060022E1 RID: 8929 RVA: 0x000D2E39 File Offset: 0x000D1039
	public virtual float MaxVelocity
	{
		get
		{
			return 5f;
		}
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000D2E40 File Offset: 0x000D1040
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DefaultMoveEntity(Vector3 _direction, bool _isDirAbsolute)
	{
		float num = 0.91f;
		if (AIDirector.debugFreezePos && this.aiManager != null)
		{
			this.motion = Vector3.zero;
		}
		if (this.onGround)
		{
			num = 0.546f;
			if (!this.IsDead() && this is EntityPlayer)
			{
				BlockValue block = this.world.GetBlock(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.boundingBox.min.y), Utils.Fastfloor(this.position.z));
				if (block.isair || block.Block.blockMaterial.IsGroundCover)
				{
					block = this.world.GetBlock(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.boundingBox.min.y - 1f), Utils.Fastfloor(this.position.z));
				}
				if (!block.isair)
				{
					num = Mathf.Clamp(1f - block.Block.blockMaterial.Friction, 0.01f, 1f);
				}
			}
		}
		if (!this.RootMotion || (!this.onGround && this.jumpTicks > 0))
		{
			float num2;
			if (this.onGround)
			{
				num2 = this.landMovementFactor;
				float num3 = 0.163f / (num * num * num);
				num2 *= num3;
			}
			else
			{
				num2 = this.jumpMovementFactor;
			}
			this.Move(_direction, _isDirAbsolute, num2, this.MaxVelocity);
		}
		if (this.Climbing)
		{
			this.fallDistance = 0f;
			this.entityCollision(this.motion);
			this.distanceClimbed += this.motion.magnitude;
			if (this.distanceClimbed > 0.5f)
			{
				this.internalPlayStepSound(1f);
				this.distanceClimbed = 0f;
			}
		}
		else
		{
			if (base.IsInElevator())
			{
				if (!this.RootMotion)
				{
					float num4 = 0.15f;
					if (this.motion.x < -num4)
					{
						this.motion.x = -num4;
					}
					if (this.motion.x > num4)
					{
						this.motion.x = num4;
					}
					if (this.motion.z < -num4)
					{
						this.motion.z = -num4;
					}
					if (this.motion.z > num4)
					{
						this.motion.z = num4;
					}
				}
				this.fallDistance = 0f;
			}
			if (this.IsSleeping)
			{
				this.motion.x = 0f;
				this.motion.z = 0f;
			}
			this.entityCollision(this.motion);
		}
		if (this.isSwimming)
		{
			this.motion.x = this.motion.x * 0.91f;
			this.motion.z = this.motion.z * 0.91f;
			this.motion.y = this.motion.y - this.world.Gravity * 0.025f;
			this.motion.y = this.motion.y * 0.91f;
			return;
		}
		this.motion.x = this.motion.x * num;
		this.motion.z = this.motion.z * num;
		if (!this.bInElevator)
		{
			this.motion.y = this.motion.y - this.world.Gravity;
		}
		this.motion.y = this.motion.y * 0.98f;
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000D31A4 File Offset: 0x000D13A4
	public virtual void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (this.jumpIsMoving)
		{
			this.JumpMove();
			return;
		}
		if (this.RootMotion)
		{
			if (this.isEntityRemote && this.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.IsDead() && (!(this.emodel != null) || !(this.emodel.avatarController != null) || !this.emodel.avatarController.IsAnimationHitRunning()))
			{
				this.accumulatedRootMotion = Vector3.zero;
				return;
			}
			bool flag = this.emodel && this.emodel.IsRagdollActive;
			if (this.isSwimming && !flag)
			{
				this.motion += this.accumulatedRootMotion * 0.001f;
			}
			else if (this.onGround || this.jumpTicks > 0)
			{
				if (flag)
				{
					this.motion.x = 0f;
					this.motion.z = 0f;
				}
				else
				{
					float y = this.motion.y;
					this.motion = this.accumulatedRootMotion;
					this.motion.y = this.motion.y + y;
				}
			}
			this.accumulatedRootMotion = Vector3.zero;
		}
		if (this.IsFlyMode.Value)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			float num = (primaryPlayer != null) ? primaryPlayer.GodModeSpeedModifier : 1f;
			float num2 = 2f * (this.MovementRunning ? 0.35f : 0.12f) * num;
			if (!this.RootMotion)
			{
				this.Move(_direction, _isDirAbsolute, this.GetPassiveEffectSpeedModifier() * num2, this.GetPassiveEffectSpeedModifier() * num2);
			}
			if (!this.IsNoCollisionMode.Value)
			{
				this.entityCollision(this.motion);
				this.motion *= base.ConditionalScalePhysicsMulConstant(0.546f);
			}
			else
			{
				this.SetPosition(this.position + this.motion, true);
				this.motion = Vector3.zero;
			}
		}
		else
		{
			this.DefaultMoveEntity(_direction, _isDirAbsolute);
		}
		if (!this.isEntityRemote && this.RootMotion)
		{
			float num3 = this.landMovementFactor;
			num3 *= 2.5f;
			if (this.inWaterPercent > 0.3f)
			{
				if (num3 > 0.01f)
				{
					float t = (this.inWaterPercent - 0.3f) * 1.4285715f;
					num3 = Mathf.Lerp(num3, 0.01f + (num3 - 0.01f) * 0.1f, t);
				}
				if (this.isSwimming)
				{
					num3 = this.landMovementFactor * 5f;
				}
			}
			float magnitude = _direction.magnitude;
			if (magnitude > 1f)
			{
				num3 /= magnitude;
			}
			Vector3 vector = _direction;
			if (_isDirAbsolute)
			{
				vector = base.transform.InverseTransformDirection(this.moveDirection);
			}
			float num4 = vector.z * num3;
			if (this.lerpForwardSpeed)
			{
				if (Utils.FastAbs(this.speedForwardTarget - num4) > 0.05f)
				{
					this.speedForwardTargetStep = Utils.FastAbs(num4 - this.speedForward) / 0.18f;
				}
				this.speedForwardTarget = num4;
			}
			else
			{
				this.speedForward = num4;
			}
			this.speedStrafe = vector.x * num3;
			this.SetMovementState();
			base.ReplicateSpeeds();
		}
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000D34FC File Offset: 0x000D16FC
	public float GetPassiveEffectSpeedModifier()
	{
		if (this.IsCrouching)
		{
			if (this.MovementRunning)
			{
				return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, Constants.cPlayerSpeedModifierWalking, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			return EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, Constants.cPlayerSpeedModifierCrouching, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		else
		{
			if (this.MovementRunning)
			{
				return EffectManager.GetValue(PassiveEffects.RunSpeed, null, Constants.cPlayerSpeedModifierRunning, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, Constants.cPlayerSpeedModifierWalking, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool CalcStrafeYawOffset(float _moveX, float _moveZ, ref float _desiredyaw, ref float _yawOffset)
	{
		return false;
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x000D35B0 File Offset: 0x000D17B0
	public void SetMoveForward(float _moveForward)
	{
		this.moveDirection.x = 0f;
		this.moveDirection.z = _moveForward;
		this.isMoveDirAbsolute = false;
		this.Climbing = false;
		this.lerpForwardSpeed = true;
		this.motion.x = 0f;
		this.motion.z = 0f;
		this.accumulatedRootMotion.x = 0f;
		this.accumulatedRootMotion.z = 0f;
		if (this.bInElevator)
		{
			this.motion.y = 0f;
		}
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x000D3648 File Offset: 0x000D1848
	public void SetMoveForwardWithModifiers(float _speedModifier, float _speedScale, float _strafeAngle, bool _climb)
	{
		Vector3 vector = new Vector3(0f, 0f, 1f);
		vector = Quaternion.Euler(0f, _strafeAngle, 0f) * vector;
		this.moveDirection.x = vector.x;
		this.moveDirection.z = vector.z;
		this.isMoveDirAbsolute = false;
		this.Climbing = _climb;
		this.lerpForwardSpeed = true;
		float num = this.speedModifier;
		this.speedModifier = _speedModifier * _speedScale;
		if (num > 0.2f)
		{
			num = this.speedModifier / num;
			this.accumulatedRootMotion.x = this.accumulatedRootMotion.x * num;
			this.accumulatedRootMotion.z = this.accumulatedRootMotion.z * num;
		}
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000D36FC File Offset: 0x000D18FC
	public void AddMotion(float dir, float speed)
	{
		float f = dir * 0.017453292f;
		this.accumulatedRootMotion.x = this.accumulatedRootMotion.x + Mathf.Sin(f) * speed;
		this.accumulatedRootMotion.z = this.accumulatedRootMotion.z + Mathf.Cos(f) * speed;
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000D3740 File Offset: 0x000D1940
	public void MakeMotionMoveToward(float x, float z, float minMotion, float maxMotion)
	{
		if (this.RootMotion)
		{
			float num = Mathf.Sqrt(x * x + z * z);
			if (num > 0f)
			{
				num = Utils.FastClamp(Mathf.Sqrt(this.accumulatedRootMotion.x * this.accumulatedRootMotion.x + this.accumulatedRootMotion.z * this.accumulatedRootMotion.z), minMotion, maxMotion) / num;
				if (num < 1f)
				{
					x *= num;
					z *= num;
				}
			}
			this.accumulatedRootMotion.x = x;
			this.accumulatedRootMotion.z = z;
			return;
		}
		this.moveDirection.x = x;
		this.moveDirection.z = z;
		this.isMoveDirAbsolute = true;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000D37F4 File Offset: 0x000D19F4
	public bool IsInFrontOfMe(Vector3 _position)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 dir = _position - headPosition;
		Vector3 forwardVector = this.GetForwardVector();
		float angleBetween = Utils.GetAngleBetween(dir, forwardVector);
		float num = this.GetMaxViewAngle() * 0.5f;
		return angleBetween >= -num && angleBetween <= num;
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000D3838 File Offset: 0x000D1A38
	public bool IsInViewCone(Vector3 _position)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 dir = _position - headPosition;
		Vector3 lookVector;
		float num;
		if (this.IsSleeping)
		{
			lookVector = this.sleeperLookDir;
			num = this.sleeperViewAngle;
		}
		else
		{
			lookVector = this.GetLookVector();
			num = this.GetMaxViewAngle();
		}
		num *= 0.5f;
		float angleBetween = Utils.GetAngleBetween(dir, lookVector);
		return angleBetween >= -num && angleBetween <= num;
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000D3894 File Offset: 0x000D1A94
	public void DrawViewCone()
	{
		Vector3 vector;
		float num;
		if (this.IsSleeping)
		{
			vector = this.sleeperLookDir;
			num = this.sleeperViewAngle;
		}
		else
		{
			vector = this.GetLookVector();
			num = this.GetMaxViewAngle();
		}
		vector *= this.GetSeeDistance();
		num *= 0.5f;
		Vector3 start = this.getHeadPosition() - Origin.position;
		Debug.DrawRay(start, vector, new Color(0.9f, 0.9f, 0.5f), 0.1f);
		Vector3 dir = Quaternion.Euler(0f, -num, 0f) * vector;
		Debug.DrawRay(start, dir, new Color(0.6f, 0.6f, 0.3f), 0.1f);
		Vector3 dir2 = Quaternion.Euler(0f, num, 0f) * vector;
		Debug.DrawRay(start, dir2, new Color(0.6f, 0.6f, 0.3f), 0.1f);
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000D397C File Offset: 0x000D1B7C
	public bool CanSee(Vector3 _pos)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 direction = _pos - headPosition;
		float seeDistance = this.GetSeeDistance();
		if (direction.magnitude > seeDistance)
		{
			return false;
		}
		if (!this.IsInViewCone(_pos))
		{
			return false;
		}
		Ray ray = new Ray(headPosition, direction);
		ray.origin += direction.normalized * 0.2f;
		int modelLayer = this.GetModelLayer();
		this.SetModelLayer(2, false, null);
		bool result = true;
		if (Voxel.Raycast(this.world, ray, seeDistance, false, false))
		{
			result = false;
		}
		this.SetModelLayer(modelLayer, false, null);
		return result;
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000D3A18 File Offset: 0x000D1C18
	public bool CanEntityBeSeen(Entity _other, bool checkViewCone = true)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 headPosition2 = _other.getHeadPosition();
		Vector3 direction = headPosition2 - headPosition;
		float magnitude = direction.magnitude;
		float num = this.GetSeeDistance();
		EntityPlayer entityPlayer = _other as EntityPlayer;
		if (entityPlayer != null)
		{
			num *= entityPlayer.DetectUsScale(this);
		}
		if (magnitude > num)
		{
			return false;
		}
		if (checkViewCone && !this.IsInViewCone(headPosition2))
		{
			return false;
		}
		bool result = false;
		Ray ray = new Ray(headPosition, direction);
		ray.origin += direction.normalized * -0.1f;
		int modelLayer = this.GetModelLayer();
		this.SetModelLayer(2, false, null);
		if (Voxel.Raycast(this.world, ray, num, -1612492829, 64, 0f))
		{
			if (Voxel.voxelRayHitInfo.tag == "E_Vehicle")
			{
				EntityVehicle entityVehicle = EntityVehicle.FindCollisionEntity(Voxel.voxelRayHitInfo.transform);
				if (entityVehicle && entityVehicle.IsAttached(_other))
				{
					result = true;
				}
			}
			else
			{
				if (Voxel.voxelRayHitInfo.tag == "E_Enemy")
				{
					EntityDrone entityDrone = EntityDrone.FindCollisionEntity(Voxel.voxelRayHitInfo.transform);
					if (entityDrone)
					{
						entityDrone.IgnoreCollisionEntity(ray, num);
					}
				}
				if (Voxel.voxelRayHitInfo.tag.StartsWith("E_BP_"))
				{
					Voxel.voxelRayHitInfo.transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
				}
				if (_other.transform == Voxel.voxelRayHitInfo.transform)
				{
					result = true;
				}
			}
		}
		this.SetModelLayer(modelLayer, false, null);
		return result;
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x000D3BB4 File Offset: 0x000D1DB4
	public virtual float GetSeeDistance()
	{
		this.senseScale = 1f;
		if (this.IsSleeping)
		{
			this.sightRange = this.sleeperSightRange;
			return this.sleeperSightRange;
		}
		this.sightRange = this.sightRangeBase;
		if (this.aiManager != null)
		{
			float num = EAIManager.CalcSenseScale();
			this.senseScale = 1f + num * this.aiManager.feralSense;
			this.sightRange = this.sightRangeBase * this.senseScale;
		}
		return this.sightRange;
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000D3C34 File Offset: 0x000D1E34
	public bool CanSeeStealth(float dist, float lightLevel)
	{
		float t = dist / this.sightRange;
		float num = Utils.FastLerp(this.sightLightThreshold.x, this.sightLightThreshold.y, t);
		return lightLevel > num;
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000D3C70 File Offset: 0x000D1E70
	public float GetSeeStealthDebugScale(float dist)
	{
		float t = dist / this.sightRange;
		return Utils.FastLerp(this.sightLightThreshold.x, this.sightLightThreshold.y, t);
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000D3CA4 File Offset: 0x000D1EA4
	public override void SetAlive()
	{
		if (this.IsDead())
		{
			this.lastAliveTime = Time.time;
		}
		base.SetAlive();
		if (!this.isEntityRemote)
		{
			this.Stats.ResetStats();
		}
		this.Stats.Health.MaxModifier = 0f;
		this.Health = (int)this.Stats.Health.ModifiedMax;
		this.Stamina = this.Stats.Stamina.ModifiedMax;
		this.deathUpdateTime = 0;
		this.bDead = false;
		this.RecordedDamage.Fatal = false;
		this.emodel.SetAlive();
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000D3D44 File Offset: 0x000D1F44
	public float YawForTarget(Entity _otherEntity)
	{
		return this.YawForTarget(_otherEntity.GetPosition());
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000D3D54 File Offset: 0x000D1F54
	public float YawForTarget(Vector3 target)
	{
		float num = target.x - this.position.x;
		return -(float)(Math.Atan2((double)(target.z - this.position.z), (double)num) * 180.0 / 3.141592653589793) + 90f;
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000D3DAC File Offset: 0x000D1FAC
	public void RotateTo(Entity _otherEntity, float _dYaw, float _dPitch)
	{
		float num = _otherEntity.position.x - this.position.x;
		float num2 = _otherEntity.position.z - this.position.z;
		float num3;
		if (_otherEntity is EntityAlive)
		{
			EntityAlive entityAlive = (EntityAlive)_otherEntity;
			num3 = this.position.y + this.GetEyeHeight() - (entityAlive.position.y + entityAlive.GetEyeHeight());
		}
		else
		{
			num3 = (_otherEntity.boundingBox.min.y + _otherEntity.boundingBox.max.y) / 2f - (this.position.y + this.GetEyeHeight());
		}
		float num4 = Mathf.Sqrt(num * num + num2 * num2);
		float intendedRotation = -(float)(Math.Atan2((double)num2, (double)num) * 180.0 / 3.141592653589793) + 90f;
		float intendedRotation2 = (float)(-(float)(Math.Atan2((double)num3, (double)num4) * 180.0 / 3.141592653589793));
		this.rotation.x = EntityAlive.UpdateRotation(this.rotation.x, intendedRotation2, _dPitch);
		this.rotation.y = EntityAlive.UpdateRotation(this.rotation.y, intendedRotation, _dYaw);
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000D3EF4 File Offset: 0x000D20F4
	public void RotateTo(float _x, float _y, float _z, float _dYaw, float _dPitch)
	{
		float num = _x - this.position.x;
		float num2 = _z - this.position.z;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		float intendedRotation = -(float)(Math.Atan2((double)num2, (double)num) * 180.0 / 3.141592653589793) + 90f;
		this.rotation.y = EntityAlive.UpdateRotation(this.rotation.y, intendedRotation, _dYaw);
		if (_dPitch > 0f)
		{
			float intendedRotation2 = (float)(-(float)(Math.Atan2((double)(_y - this.position.y), (double)num3) * 180.0 / 3.141592653589793));
			this.rotation.x = -EntityAlive.UpdateRotation(this.rotation.x, intendedRotation2, _dPitch);
		}
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000D3FC4 File Offset: 0x000D21C4
	public static float UpdateRotation(float _curRotation, float _intendedRotation, float _maxIncr)
	{
		float num;
		for (num = _intendedRotation - _curRotation; num < -180f; num += 360f)
		{
		}
		while (num >= 180f)
		{
			num -= 360f;
		}
		if (num > _maxIncr)
		{
			num = _maxIncr;
		}
		if (num < -_maxIncr)
		{
			num = -_maxIncr;
		}
		return _curRotation + num;
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000D400C File Offset: 0x000D220C
	public override float GetEyeHeight()
	{
		if (this.walkType == 21)
		{
			return 0.15f;
		}
		if (this.walkType == 22)
		{
			return 0.6f;
		}
		if (!this.IsCrouching)
		{
			return base.height * 0.8f;
		}
		return base.height * 0.5f;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000D405A File Offset: 0x000D225A
	public virtual float GetSpeedModifier()
	{
		return this.speedModifier;
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000D4064 File Offset: 0x000D2264
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fallHitGround(float _distance, Vector3 _fallMotion)
	{
		base.fallHitGround(_distance, _fallMotion);
		if (_distance > 2f)
		{
			int num = (int)((-_fallMotion.y - 0.85f) * 160f);
			if (num > 0)
			{
				this.DamageEntity(DamageSource.fall, num, false, 1f);
			}
			this.PlayHitGroundSound(1f);
		}
		if (!this.IsDead() && !this.emodel.IsRagdollActive && (this.disableFallBehaviorUntilOnGround || !this.ChooseFallBehavior(_distance, _fallMotion)) && this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Land);
		}
		if (this.aiManager != null)
		{
			this.aiManager.FallHitGround(_distance);
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000D4124 File Offset: 0x000D2324
	public bool NotifyDestroyedBlock(ItemActionAttack.AttackHitInfo attackHitInfo)
	{
		if (attackHitInfo == null || this.moveHelper == null || this.moveHelper.BlockedFlags <= 0)
		{
			return false;
		}
		if (this.moveHelper.HitInfo.hit.blockValueRef == attackHitInfo.hitRef)
		{
			this.moveHelper.ClearBlocked();
		}
		if (this._destroyBlockBehaviors.Count == 0)
		{
			return false;
		}
		float num = 0f;
		EntityAlive.weightBehaviorTemp.Clear();
		int num2 = 1;
		for (int i = 0; i < this._destroyBlockBehaviors.Count; i++)
		{
			EntityAlive.DestroyBlockBehavior destroyBlockBehavior = this._destroyBlockBehaviors[i];
			if (num2 >= destroyBlockBehavior.Difficulty.min && num2 <= destroyBlockBehavior.Difficulty.max)
			{
				EntityAlive.WeightBehavior item;
				item.weight = destroyBlockBehavior.Weight + num;
				item.index = i;
				EntityAlive.weightBehaviorTemp.Add(item);
				num += destroyBlockBehavior.Weight;
			}
		}
		bool result = false;
		if (num > 0f)
		{
			EntityAlive.DestroyBlockBehavior destroyBlockBehavior2 = null;
			float num3 = this.rand.RandomFloat * num;
			for (int j = 0; j < EntityAlive.weightBehaviorTemp.Count; j++)
			{
				if (num3 <= EntityAlive.weightBehaviorTemp[j].weight)
				{
					destroyBlockBehavior2 = this._destroyBlockBehaviors[EntityAlive.weightBehaviorTemp[j].index];
					break;
				}
			}
			if (destroyBlockBehavior2 != null)
			{
				result = this.ExecuteDestroyBlockBehavior(destroyBlockBehavior2, attackHitInfo);
			}
		}
		return result;
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ExecuteDestroyBlockBehavior(EntityAlive.DestroyBlockBehavior behavior, ItemActionAttack.AttackHitInfo attackHitInfo)
	{
		return false;
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000D4294 File Offset: 0x000D2494
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ChooseFallBehavior(float _distance, Vector3 _fallMotion)
	{
		if (this.fallBehaviors.Count == 0)
		{
			return false;
		}
		float num = 0f;
		EntityAlive.weightBehaviorTemp.Clear();
		for (int i = 0; i < this.fallBehaviors.Count; i++)
		{
			EntityAlive.FallBehavior fallBehavior = this.fallBehaviors[i];
			if (_distance >= fallBehavior.Height.min && _distance <= fallBehavior.Height.max && 1 >= fallBehavior.Difficulty.min && 1 <= fallBehavior.Difficulty.max)
			{
				EntityAlive.WeightBehavior item;
				item.weight = fallBehavior.Weight + num;
				item.index = i;
				EntityAlive.weightBehaviorTemp.Add(item);
				num += fallBehavior.Weight;
			}
		}
		bool result = false;
		if (num > 0f)
		{
			EntityAlive.FallBehavior fallBehavior2 = null;
			float num2 = this.rand.RandomFloat * num;
			for (int j = 0; j < EntityAlive.weightBehaviorTemp.Count; j++)
			{
				if (num2 <= EntityAlive.weightBehaviorTemp[j].weight)
				{
					fallBehavior2 = this.fallBehaviors[EntityAlive.weightBehaviorTemp[j].index];
					break;
				}
			}
			if (fallBehavior2 != null)
			{
				result = this.ExecuteFallBehavior(fallBehavior2, _distance, _fallMotion);
			}
		}
		return result;
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ExecuteFallBehavior(EntityAlive.FallBehavior behavior, float _distance, Vector3 _fallMotion)
	{
		return false;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000D43CC File Offset: 0x000D25CC
	public virtual void PlayHitGroundSound(float impactSpeed = 1f)
	{
		float volumeScale = Mathf.Lerp(0.3f, 1f, impactSpeed);
		if (!string.IsNullOrEmpty(this.soundLand))
		{
			this.PlayOneShot(this.soundLand, false, false, false, null, volumeScale);
			return;
		}
		if (!string.IsNullOrEmpty(this.soundLandThump))
		{
			this.PlayOneShot(this.soundLandThump, false, false, false, null, volumeScale);
			return;
		}
		this.PlayOneShot("entityhitsground", false, false, false, null, volumeScale);
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool FriendlyFireCheck(EntityAlive other)
	{
		return true;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool HasImmunity(BuffClass _buffClass)
	{
		return false;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000D4438 File Offset: 0x000D2638
	public int CalculateBlockDamage(BlockDamage block, int defaultBlockDamage, out bool bypassMaxDamage)
	{
		if (this.stompsSpikes && block.HasTag(BlockTags.Spike))
		{
			bypassMaxDamage = true;
			return 999;
		}
		bypassMaxDamage = false;
		return defaultBlockDamage;
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000D4458 File Offset: 0x000D2658
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale = 1f)
	{
		if (_damageSource.damageType == EnumDamageTypes.Suicide && this.emodel && this.emodel.avatarController is AvatarZombieController)
		{
			(this.emodel.avatarController as AvatarZombieController).CleanupDismemberedLimbs();
		}
		EnumDamageSource source = _damageSource.GetSource();
		if (_damageSource.IsIgnoreConsecutiveDamages() && source != EnumDamageSource.Internal)
		{
			if (this.damageSourceTimeouts.ContainsKey(source) && GameTimer.Instance.ticks - this.damageSourceTimeouts[source] < 30UL)
			{
				return -1;
			}
			this.damageSourceTimeouts[source] = GameTimer.Instance.ticks;
		}
		EntityAlive entityAlive = this.world.GetEntity(_damageSource.getEntityId()) as EntityAlive;
		if (!this.FriendlyFireCheck(entityAlive))
		{
			return -1;
		}
		bool flag = _damageSource.GetDamageType() == EnumDamageTypes.Heat;
		if (!flag && entityAlive && (this.entityFlags & entityAlive.entityFlags & EntityFlags.Zombie) > EntityFlags.None)
		{
			return -1;
		}
		if (this.IsGodMode.Value)
		{
			return -1;
		}
		if (!this.IsDead() && entityAlive)
		{
			PassiveEffects passiveEffect = PassiveEffects.DamageBonus;
			EntityAlive entity = entityAlive;
			float value = EffectManager.GetValue(passiveEffect, _damageSource.AttackingItem, 0f, entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			if (value > 0f)
			{
				_damageSource.DamageMultiplier = value;
				_damageSource.BonusDamageType = EnumDamageBonusType.Sneak;
			}
		}
		this.MinEventContext.Other = entityAlive;
		float num = Utils.FastMin(1f, EffectManager.GetValue(PassiveEffects.GeneralDamageResist, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		float num2 = (float)_strength * num + this.accumulatedDamageResisted;
		int num3 = Utils.FastMin(_strength, (int)num2);
		this.accumulatedDamageResisted = num2 - (float)num3;
		_strength -= num3;
		DamageResponse damageResponse = this.damageEntityLocal(_damageSource, _strength, _criticalHit, _impulseScale);
		NetPackage package = NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, damageResponse);
		if (this.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		}
		else
		{
			int excludePlayer = -1;
			if (!flag && _damageSource.CreatorEntityId != -2)
			{
				excludePlayer = _damageSource.getEntityId();
				if (_damageSource.CreatorEntityId != -1)
				{
					Entity entity2 = this.world.GetEntity(_damageSource.CreatorEntityId);
					if (entity2 && !entity2.isEntityRemote)
					{
						excludePlayer = -1;
					}
				}
			}
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, excludePlayer, package, false);
		}
		return damageResponse.ModStrength;
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000D46B3 File Offset: 0x000D28B3
	public virtual void SetDamagedTarget(EntityAlive _attackTarget)
	{
		this.damagedTarget = _attackTarget;
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000D46BC File Offset: 0x000D28BC
	public virtual void ClearDamagedTarget()
	{
		this.damagedTarget = null;
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000D46C5 File Offset: 0x000D28C5
	public EntityAlive GetDamagedTarget()
	{
		return this.damagedTarget;
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000D46CD File Offset: 0x000D28CD
	public override bool IsDead()
	{
		return base.IsDead() || this.RecordedDamage.Fatal;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000D46E4 File Offset: 0x000D28E4
	public override bool CanLockOnServer(int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		return !this.IsDead() && base.CanLockOnServer(_lockingPlayerID, _context, _channel);
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000D46F9 File Offset: 0x000D28F9
	public override bool CanLockLocally(ILockContext _context, ushort _channel)
	{
		return !this.IsDead() && base.CanLockLocally(_context, _channel);
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000D4710 File Offset: 0x000D2910
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse damageResponse = default(DamageResponse);
		damageResponse.Source = _damageSource;
		damageResponse.Strength = _strength;
		damageResponse.Critical = _criticalHit;
		damageResponse.HitDirection = Utils.EnumHitDirection.None;
		damageResponse.MovementState = this.MovementState;
		damageResponse.Random = this.rand.RandomFloat;
		damageResponse.ImpulseScale = impulseScale;
		damageResponse.HitBodyPart = _damageSource.GetEntityDamageBodyPart(this);
		damageResponse.ArmorSlot = _damageSource.GetEntityDamageEquipmentSlot(this);
		damageResponse.ArmorSlotGroup = _damageSource.GetEntityDamageEquipmentSlotGroup(this);
		if (_strength > 0)
		{
			damageResponse.HitDirection = (_damageSource.Equals(DamageSource.fall) ? Utils.EnumHitDirection.Back : ((Utils.EnumHitDirection)Utils.Get4HitDirectionAsInt(_damageSource.getDirection(), this.GetLookVector())));
		}
		if (!GameManager.IsDedicatedServer && _damageSource.damageSource != EnumDamageSource.Internal && GameManager.Instance != null)
		{
			World world = GameManager.Instance.World;
			if (world != null && _damageSource.getEntityId() == world.GetPrimaryPlayerId())
			{
				Transform hitTransform = this.emodel.GetHitTransform(_damageSource);
				Vector3 position;
				if (hitTransform)
				{
					position = hitTransform.position;
				}
				else
				{
					position = this.emodel.transform.position;
				}
				bool flag = world.GetPrimaryPlayer().inventory.holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("ranged"));
				float magnitude = (world.GetPrimaryPlayer().GetPosition() - position).magnitude;
				if (flag && magnitude > EntityAlive.HitSoundDistance)
				{
					Manager.PlayInsidePlayerHead("HitEntitySound", -1, 0f, false, false);
				}
				if (EntityAlive.ShowDebugDisplayHit)
				{
					Transform transform = hitTransform ? hitTransform : this.emodel.transform;
					Vector3 position2 = Camera.main.transform.position;
					DebugLines.CreateAttached("EntityDamage" + this.entityId.ToString(), transform, position2 + Origin.position, _damageSource.getHitTransformPosition(), new Color(0.3f, 0f, 0.3f), new Color(1f, 0f, 1f), EntityAlive.DebugDisplayHitSize * 2f, EntityAlive.DebugDisplayHitSize, EntityAlive.DebugDisplayHitTime);
					DebugLines.CreateAttached("EntityDamage2" + this.entityId.ToString(), transform, _damageSource.getHitTransformPosition(), transform.position + Origin.position, new Color(0f, 0f, 0.5f), new Color(0.3f, 0.3f, 1f), EntityAlive.DebugDisplayHitSize * 2f, EntityAlive.DebugDisplayHitSize, EntityAlive.DebugDisplayHitTime);
				}
			}
		}
		if (_damageSource.AffectedByArmor())
		{
			this.equipment.CalcDamage(ref damageResponse.Strength, ref damageResponse.ArmorDamage, damageResponse.Source.DamageTypeTag, this.MinEventContext.Other, damageResponse.Source.AttackingItem);
		}
		float num = this.GetDamageFraction((float)damageResponse.Strength);
		if (damageResponse.Fatal || damageResponse.Strength >= this.Health)
		{
			if ((damageResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
			{
				if (num >= 0.2f)
				{
					damageResponse.Source.DismemberChance = Utils.FastMax(damageResponse.Source.DismemberChance * 0.5f, 0.3f);
				}
			}
			else if (num >= 0.12f)
			{
				damageResponse.Source.DismemberChance = Utils.FastMax(damageResponse.Source.DismemberChance * 0.5f, 0.5f);
			}
			num = 1f;
			if (this.canDisintegrate)
			{
				this.Disintegrate();
			}
		}
		this.CheckDismember(ref damageResponse, num);
		int num2 = this.bodyDamage.StunKnee;
		int num3 = this.bodyDamage.StunProne;
		if ((damageResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None && damageResponse.Dismember)
		{
			if (this.Health > 0)
			{
				damageResponse.Strength = this.Health;
			}
		}
		else if (_damageSource.CanStun && this.GetWalkType() != 21 && this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
		{
			if ((damageResponse.HitBodyPart & (EnumBodyPartHit.Torso | EnumBodyPartHit.Head | EnumBodyPartHit.LeftUpperArm | EnumBodyPartHit.RightUpperArm | EnumBodyPartHit.LeftLowerArm | EnumBodyPartHit.RightLowerArm)) > EnumBodyPartHit.None)
			{
				num3 += _strength;
			}
			else if (damageResponse.HitBodyPart.IsLeg())
			{
				num2 += _strength * (_criticalHit ? 2 : 1);
			}
		}
		if ((!damageResponse.HitBodyPart.IsLeg() || !damageResponse.Dismember) && this.GetWalkType() != 21 && !this.sleepingOrWakingUp)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (this.GetDamageFraction((float)num3) >= entityClass.KnockdownProneDamageThreshold && entityClass.KnockdownProneDamageThreshold > 0f)
			{
				if (this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
				{
					damageResponse.Stun = EnumEntityStunType.Prone;
					damageResponse.StunDuration = this.rand.RandomRange(entityClass.KnockdownProneStunDuration.x, entityClass.KnockdownProneStunDuration.y);
				}
			}
			else if (this.GetDamageFraction((float)num2) >= entityClass.KnockdownKneelDamageThreshold && entityClass.KnockdownKneelDamageThreshold > 0f && this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
			{
				damageResponse.Stun = EnumEntityStunType.Kneel;
				damageResponse.StunDuration = this.rand.RandomRange(entityClass.KnockdownKneelStunDuration.x, entityClass.KnockdownKneelStunDuration.y);
			}
		}
		bool flag2 = false;
		int num4 = damageResponse.Strength + damageResponse.ArmorDamage / 2;
		if (num4 > 0 && !this.IsGodMode.Value && damageResponse.Stun == EnumEntityStunType.None && !this.sleepingOrWakingUp)
		{
			flag2 = (damageResponse.Strength < this.Health);
			if (flag2)
			{
				flag2 = (this.GetWalkType() == 21 || !damageResponse.Dismember || !damageResponse.HitBodyPart.IsLeg());
			}
			if (flag2 && damageResponse.Source.GetDamageType() != EnumDamageTypes.Bashing)
			{
				flag2 = (num4 >= 6);
			}
			if (damageResponse.Source.GetDamageType() == EnumDamageTypes.BarbedWire)
			{
				flag2 = true;
			}
		}
		damageResponse.PainHit = flag2;
		if (damageResponse.Strength >= this.Health)
		{
			damageResponse.Fatal = true;
		}
		if (damageResponse.Fatal)
		{
			damageResponse.Stun = EnumEntityStunType.None;
		}
		if (this.isEntityRemote)
		{
			damageResponse.ModStrength = 0;
		}
		else
		{
			if (this.Health <= damageResponse.Strength)
			{
				_strength -= this.Health;
			}
			damageResponse.ModStrength = _strength;
		}
		if (damageResponse.Dismember)
		{
			EntityAlive entityAlive = this.world.GetEntity(damageResponse.Source.getEntityId()) as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.FireEvent(MinEventTypes.onDismember, true);
			}
		}
		if (this.MinEventContext.Other != null)
		{
			this.MinEventContext.Other.MinEventContext.DamageResponse = damageResponse;
			float value = EffectManager.GetValue(PassiveEffects.HealthSteal, null, 0f, this.MinEventContext.Other, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			if (value != 0f)
			{
				int num5 = (int)((float)num4 * value);
				if (num5 + this.MinEventContext.Other.Health <= 0)
				{
					num5 = (this.MinEventContext.Other.Health - 1) * -1;
				}
				this.MinEventContext.Other.AddHealth(num5);
				if (num5 < 0 && this.MinEventContext.Other is EntityPlayerLocal)
				{
					((EntityPlayerLocal)this.MinEventContext.Other).ForceBloodSplatter();
				}
			}
		}
		this.FireAttackedEvents(damageResponse);
		this.ProcessDamageResponseLocal(damageResponse);
		return damageResponse;
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000D4E54 File Offset: 0x000D3054
	public override void FireAttackedEvents(DamageResponse _dmResponse)
	{
		base.FireAttackedEvents(_dmResponse);
		if (_dmResponse.Source.BuffClass == null || this.Progression != null)
		{
			this.MinEventContext.DamageResponse = _dmResponse;
			EntityAlive entityAlive = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
			if (entityAlive && !entityAlive.isEntityRemote)
			{
				this.MinEventContext.IsLocal = (this is EntityPlayer && this.isEntityRemote);
			}
			if (_dmResponse.Source.BuffClass == null)
			{
				this.FireEvent(MinEventTypes.onOtherAttackedSelf, true);
			}
			else if (this.Progression != null)
			{
				this.Progression.FireEvent(MinEventTypes.onOtherAttackedSelf, this.MinEventContext);
			}
			this.MinEventContext.IsLocal = false;
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x0600230C RID: 8972 RVA: 0x000D4F14 File Offset: 0x000D3114
	public virtual bool IsImmuneToLegDamage
	{
		get
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			return this.GetWalkType() == 21 || !this.bodyDamage.HasLeftLeg || !this.bodyDamage.HasRightLeg || (entityClass.LowerLegDismemberThreshold <= 0f && entityClass.UpperLegDismemberThreshold <= 0f);
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000D4F78 File Offset: 0x000D3178
	public override void ProcessDamageResponse(DamageResponse _dmResponse)
	{
		if (Time.time - this.lastAliveTime < 1f)
		{
			return;
		}
		base.ProcessDamageResponse(_dmResponse);
		this.ProcessDamageResponseLocal(_dmResponse);
		if (!this.world.IsRemote())
		{
			Entity entity = this.world.GetEntity(_dmResponse.Source.getEntityId());
			if (entity && !entity.isEntityRemote && this.isEntityRemote && this is EntityPlayer)
			{
				this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
				return;
			}
			if (_dmResponse.Source.BuffClass != null)
			{
				this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
				return;
			}
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, _dmResponse.Source.getEntityId(), NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000D508C File Offset: 0x000D328C
	public virtual void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		if (this.emodel == null)
		{
			return;
		}
		if (_dmResponse.Source.BonusDamageType != EnumDamageBonusType.None)
		{
			EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
			if (primaryPlayer && primaryPlayer.entityId == _dmResponse.Source.getEntityId())
			{
				EnumDamageBonusType bonusDamageType = _dmResponse.Source.BonusDamageType;
				if (bonusDamageType != EnumDamageBonusType.Sneak)
				{
					if (bonusDamageType == EnumDamageBonusType.Stun)
					{
						primaryPlayer.NotifyDamageMultiplier(_dmResponse.Source.DamageMultiplier);
					}
				}
				else
				{
					primaryPlayer.NotifySneakDamage(_dmResponse.Source.DamageMultiplier);
				}
			}
		}
		EntityAlive entityAlive = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.SetDamagedTarget(this);
		}
		if (this.IsSleeperPassive)
		{
			this.world.CheckSleeperVolumeNoise(this.position);
		}
		this.ConditionalTriggerSleeperWakeUp();
		this.SleeperSupressLivingSounds = false;
		this.bPlayHurtSound = false;
		if (this.equipment != null && _dmResponse.ArmorDamage > 0)
		{
			List<ItemValue> armor = this.equipment.GetArmor();
			if (armor.Count > 0)
			{
				float num = (float)_dmResponse.ArmorDamage / (float)armor.Count;
				if (num < 1f && num != 0f)
				{
					num = 1f;
				}
				for (int i = 0; i < armor.Count; i++)
				{
					armor[i].UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, armor[i], num, this, null, armor[i].ItemClass.ItemTags, true, true, true, true, true, 1, true, false) * ItemAction.ItemDegradationModifier;
				}
			}
		}
		this.ApplyLocalBodyDamage(_dmResponse);
		this.lastHitRanged = false;
		this.lastDamageResponse = _dmResponse;
		bool flag = EffectManager.GetValue(PassiveEffects.NegateDamageSelf, null, 0f, this, null, FastTags<TagGroup.Global>.Parse(_dmResponse.HitBodyPart.ToString()), true, true, true, true, true, 1, true, false) > 0f || EffectManager.GetValue(PassiveEffects.NegateDamageOther, (entityAlive != null) ? entityAlive.inventory.holdingItemItemValue : null, 0f, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f;
		if (_dmResponse.Dismember && !flag)
		{
			this.lastHitImpactDir = _dmResponse.Source.getDirection();
			if (entityAlive != null)
			{
				this.lastHitEntityFwd = entityAlive.GetForwardVector();
			}
			if (_dmResponse.Source.ItemClass != null && _dmResponse.Source.ItemClass.HasAnyTags(DismembermentManager.rangedTags))
			{
				this.lastHitRanged = true;
			}
			if (_dmResponse.Source.ItemClass != null)
			{
				float strength = (float)_dmResponse.ModStrength / (float)this.GetMaxHealth();
				this.lastHitForce = DismembermentManager.GetImpactForce(_dmResponse.Source.ItemClass, strength);
			}
			this.ExecuteDismember(false);
		}
		bool flag2 = _dmResponse.Stun > EnumEntityStunType.None;
		bool flag3 = this.bodyDamage.CurrentStun > EnumEntityStunType.None;
		bool flag4 = this.Health <= 0;
		int num2 = _dmResponse.Strength;
		if (!(this is EntityPlayer) && base.EntityClass.bIsEnemyEntity)
		{
			if (!flag4 && ((EntityAlive.IsHeadshotOnly && entityAlive is EntityPlayer) || EffectManager.GetValue(PassiveEffects.HeadShotOnly, null, 0f, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f) && !base.HasAnyTags(EntityAlive.noheadTag) && (_dmResponse.HitBodyPart & EnumBodyPartHit.Head) == EnumBodyPartHit.None)
			{
				num2 = 0;
				_dmResponse.Fatal = false;
			}
			if (!flag4 && EntityAlive.IsHeadshotFinisher && entityAlive is EntityPlayer && _dmResponse.Fatal && !base.HasAnyTags(EntityAlive.noheadTag) && (_dmResponse.HitBodyPart & EnumBodyPartHit.Head) == EnumBodyPartHit.None)
			{
				num2 = this.Health - 1;
				_dmResponse.Fatal = false;
			}
		}
		if (!flag && _dmResponse.Fatal && this.isEntityRemote)
		{
			this.ClientKill(_dmResponse);
		}
		else if (flag2 && this.emodel.avatarController)
		{
			if (_dmResponse.Stun == EnumEntityStunType.Prone)
			{
				if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
				{
					if ((_dmResponse.Critical && _dmResponse.Source.damageType == EnumDamageTypes.Bashing) || this.rand.RandomFloat < 0.6f)
					{
						this.DoRagdoll(_dmResponse);
					}
					else
					{
						this.emodel.avatarController.BeginStun(EnumEntityStunType.Prone, _dmResponse.HitBodyPart, _dmResponse.HitDirection, _dmResponse.Critical, _dmResponse.Random);
					}
					this.SetStun(EnumEntityStunType.Prone);
					this.bodyDamage.StunDuration = _dmResponse.StunDuration;
				}
				else if (this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
				{
					this.DoRagdoll(_dmResponse);
					this.SetStun(EnumEntityStunType.Prone);
					this.bodyDamage.StunDuration = _dmResponse.StunDuration * 0.5f;
				}
			}
			else if (_dmResponse.Stun == EnumEntityStunType.Kneel)
			{
				bool flag5 = false;
				if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
				{
					if (_dmResponse.Critical || this.rand.RandomFloat < 0.25f)
					{
						flag5 = true;
					}
					else
					{
						this.SetStun(EnumEntityStunType.Kneel);
						this.emodel.avatarController.BeginStun(EnumEntityStunType.Kneel, _dmResponse.HitBodyPart, _dmResponse.HitDirection, _dmResponse.Critical, _dmResponse.Random);
					}
				}
				else if (this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel)
				{
					flag5 = true;
				}
				if (flag5)
				{
					this.DoRagdoll(_dmResponse);
					this.SetStun(EnumEntityStunType.Prone);
				}
				this.bodyDamage.StunDuration = _dmResponse.StunDuration;
			}
		}
		else if (_dmResponse.PainHit && !flag3 && this.emodel.avatarController)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			float num3 = entityClass.PainResistPerHit;
			if (num3 >= 0f)
			{
				float num4 = (float)this.GetMaxHealth();
				if ((float)this.Health / num4 < entityClass.PainResistPerHitLowHealthPercent)
				{
					num3 = entityClass.PainResistPerHitLowHealth;
				}
				this.painResistPercent = Utils.FastMin(this.painResistPercent + num3, 3f);
				float duration = float.MaxValue;
				if (this.painResistPercent >= 3f && num3 >= 1f)
				{
					duration = 0f;
					this.painHitsFelt += 0.15f;
				}
				else if (this.painResistPercent >= 1f)
				{
					duration = Utils.FastLerp(0.5f, 0.15f, (this.painResistPercent - 1f) * 0.75f);
					this.painHitsFelt += 0.3f;
				}
				else
				{
					this.painHitsFelt += Utils.FastLerp(1f, 0.3f, this.painResistPercent);
				}
				this.emodel.avatarController.StartAnimationHit(_dmResponse.HitBodyPart, (int)_dmResponse.HitDirection, (int)((float)_dmResponse.Strength * 100f / num4), _dmResponse.Critical, _dmResponse.MovementState, _dmResponse.Random, duration);
			}
		}
		if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
		{
			if (_dmResponse.Source.CanStun)
			{
				if ((_dmResponse.HitBodyPart & (EnumBodyPartHit.Torso | EnumBodyPartHit.Head | EnumBodyPartHit.LeftUpperArm | EnumBodyPartHit.RightUpperArm | EnumBodyPartHit.LeftLowerArm | EnumBodyPartHit.RightLowerArm)) > EnumBodyPartHit.None)
				{
					this.bodyDamage.StunProne = this.bodyDamage.StunProne + _dmResponse.Strength;
				}
				else if (_dmResponse.HitBodyPart.IsLeg())
				{
					this.bodyDamage.StunKnee = this.bodyDamage.StunKnee + _dmResponse.Strength;
				}
			}
		}
		else
		{
			this.bodyDamage.StunProne = 0;
			this.bodyDamage.StunKnee = 0;
		}
		if (this.Health <= 0 && this.deathUpdateTime > 0)
		{
			this.DeathHealth -= _dmResponse.Strength;
		}
		if (flag)
		{
			num2 = 0;
			_dmResponse.Fatal = false;
		}
		if (this.isEntityRemote)
		{
			this.Health -= num2;
			this.RecordedDamage = _dmResponse;
		}
		else
		{
			if (!this.IsGodMode.Value)
			{
				this.Health -= num2;
				if (_dmResponse.Fatal && this.Health > 0)
				{
					this.Health = 0;
				}
				this.hasBeenAttackedTime = 0;
				if (_dmResponse.PainHit)
				{
					this.hasBeenAttackedTime = this.GetMaxAttackTime();
				}
			}
			this.bPlayHurtSound = (this.bBeenWounded = (num2 > 0));
			if (this.bBeenWounded)
			{
				base.setBeenAttacked();
				this.MinEventContext.Other = (GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive);
				this.FireEvent(MinEventTypes.onOtherDamagedSelf, true);
			}
			if (num2 > this.woundedStrength)
			{
				this.woundedStrength = _dmResponse.Strength;
				this.woundedDamageSource = _dmResponse.Source;
			}
			this.lastHitDirection = _dmResponse.HitDirection;
			if (this.Health <= 0)
			{
				_dmResponse.Source.getDirection();
				_dmResponse.Strength += this.Health;
				Entity entity = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
				if (this.Spawned && !flag4)
				{
					if (entity is EntityAlive)
					{
						this.entityThatKilledMe = (EntityAlive)entity;
					}
					else
					{
						this.entityThatKilledMe = null;
					}
				}
				this.Kill(_dmResponse);
				if (!_dmResponse.Fatal && this.world.IsRemote())
				{
					this.DamageEntity(DamageSource.disease, 1, false, 1f);
				}
			}
		}
		Entity entity2 = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
		if (entity2 != null && entity2 != this)
		{
			if (entity2 is EntityAlive && !this.isEntityRemote && !entity2.IsIgnoredByAI())
			{
				this.SetRevengeTarget((EntityAlive)entity2);
				if (this.aiManager != null)
				{
					this.aiManager.DamagedByEntity();
				}
			}
			if (entity2 is EntityPlayer)
			{
				((EntityPlayer)entity2).FireEvent(MinEventTypes.onCombatEntered, true);
			}
			this.FireEvent(MinEventTypes.onCombatEntered, true);
		}
		if (_dmResponse.Strength > 0 && _dmResponse.Source.GetDamageType() == EnumDamageTypes.Electrical)
		{
			this.Electrocuted = true;
		}
		if (!GameManager.IsDedicatedServer && DamageText.Enabled && (this.world.GetPrimaryPlayer().cameraTransform.position + Origin.position - this.position).sqrMagnitude < 225f)
		{
			string text = string.Format("{0}", _dmResponse.Strength);
			Color color = ((_dmResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None) ? Color.red : Color.yellow;
			if (_dmResponse.Critical)
			{
				color.b = 0.8f;
			}
			DamageText.Create(text, color, this.getHeadPosition() + new Vector3(0f, 0.1f, 0f), new Vector3(this.rand.RandomRange(-0.7f, 0.7f), 0.8f, this.rand.RandomRange(-0.7f, 0.7f)), 0.22f);
		}
		this.RecordedDamage = _dmResponse;
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000D5B70 File Offset: 0x000D3D70
	public string GetArmorImpactSound(EquipmentSlots _slot, bool _grazingHit, bool _includeCosmetic = true)
	{
		ItemClass itemClass = null;
		if (_includeCosmetic)
		{
			itemClass = this.equipment.GetCosmeticSlot((int)_slot, false);
		}
		if (itemClass == null)
		{
			ItemValue slotItem = this.equipment.GetSlotItem((int)_slot);
			itemClass = ((slotItem != null) ? slotItem.ItemClass : null);
		}
		if (itemClass == null)
		{
			return string.Empty;
		}
		if (_grazingHit)
		{
			return itemClass.SoundImpactGraze;
		}
		return itemClass.SoundImpactHit;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000D5BC8 File Offset: 0x000D3DC8
	public string GetArmorMaterial(EquipmentSlots _slot, bool _includeCosmetic = true)
	{
		ItemClass itemClass = null;
		if (_includeCosmetic)
		{
			itemClass = this.equipment.GetCosmeticSlot((int)_slot, false);
		}
		if (itemClass == null)
		{
			ItemValue slotItem = this.equipment.GetSlotItem((int)_slot);
			itemClass = ((slotItem != null) ? slotItem.ItemClass : null);
		}
		if (itemClass == null)
		{
			return string.Empty;
		}
		if (itemClass != null)
		{
			return itemClass.MadeOfMaterial.SurfaceCategory;
		}
		return string.Empty;
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000D5C21 File Offset: 0x000D3E21
	public EntityAlive GetRevengeTarget()
	{
		return this.revengeEntity;
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000D5C29 File Offset: 0x000D3E29
	public void SetRevengeTarget(EntityAlive _other)
	{
		this.revengeEntity = _other;
		this.revengeTimer = ((this.revengeEntity == null) ? 0 : 500);
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000D5C4E File Offset: 0x000D3E4E
	public void SetRevengeTimer(int ticks)
	{
		this.revengeTimer = ticks;
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000BC594 File Offset: 0x000BA794
	public override bool CanBePushed()
	{
		return !this.IsDead();
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000D5C57 File Offset: 0x000D3E57
	public override bool CanCollideWith(Entity _other)
	{
		return !this.IsDead() && !(_other is EntityItem) && !(_other is EntitySupplyCrate);
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000D5C77 File Offset: 0x000D3E77
	public override bool CanCollideWithBlocks()
	{
		return !this.IsSleeping;
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000D5C84 File Offset: 0x000D3E84
	public void DoRagdoll(in DamageResponse _dmResponse)
	{
		this.emodel.DoRagdoll(_dmResponse, EModelBase.RagdollMode.Default, _dmResponse.StunDuration);
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000D5C99 File Offset: 0x000D3E99
	public void DoRagdoll(in DamageResponse _dmResponse, EModelBase.RagdollMode _mode)
	{
		this.emodel.DoRagdoll(_dmResponse, _mode, _dmResponse.StunDuration);
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000D5CB0 File Offset: 0x000D3EB0
	public void AddScore(int _diedMySelfTimes, int _zombieKills, int _playerKills, int _otherTeamnumber, int _conditions)
	{
		this.KilledZombies += _zombieKills;
		this.KilledPlayers += _playerKills;
		this.Died += _diedMySelfTimes;
		this.Score += _zombieKills * GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) + _playerKills * GameStats.GetInt(EnumGameStats.ScorePlayerKillMultiplier) + _diedMySelfTimes * GameStats.GetInt(EnumGameStats.ScoreDiedMultiplier);
		if (this.Score < 0)
		{
			this.Score = 0;
		}
		if (this is EntityPlayerLocal)
		{
			if (_diedMySelfTimes > 0)
			{
				IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager != null)
				{
					achievementManager.SetAchievementStat(EnumAchievementDataStat.Deaths, _diedMySelfTimes);
				}
			}
			if (_zombieKills > 0)
			{
				IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager2 != null)
				{
					achievementManager2.SetAchievementStat(EnumAchievementDataStat.ZombiesKilled, _zombieKills);
				}
			}
			if (_playerKills > 0)
			{
				IAchievementManager achievementManager3 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager3 != null)
				{
					achievementManager3.SetAchievementStat(EnumAchievementDataStat.PlayersKilled, _playerKills);
				}
			}
			if ((_conditions & 2) != 0)
			{
				IAchievementManager achievementManager4 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager4 == null)
				{
					return;
				}
				achievementManager4.SetAchievementStat(EnumAchievementDataStat.KilledWith44Magnum, 1);
			}
		}
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000D5D98 File Offset: 0x000D3F98
	public virtual void AwardKill(EntityAlive killer)
	{
		if (killer != null && killer != this)
		{
			int num = 0;
			int num2 = 0;
			int conditions = 0;
			EntityType entityType = this.entityType;
			if (entityType != EntityType.Player)
			{
				if (entityType == EntityType.Zombie)
				{
					num++;
				}
			}
			else
			{
				num2++;
			}
			EntityPlayer entityPlayer = killer as EntityPlayer;
			if (entityPlayer)
			{
				GameManager.Instance.AwardKill(killer, this);
				if (entityPlayer.inventory.IsHoldingGun() && entityPlayer.inventory.holdingItem.Name.Equals("gunHandgunT2Magnum44"))
				{
					conditions = 2;
				}
				GameManager.Instance.AddScoreServer(killer.entityId, num, num2, this.TeamNumber, conditions);
			}
		}
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000D5E40 File Offset: 0x000D4040
	public virtual void OnEntityDeath()
	{
		if (this.deathUpdateTime != 0)
		{
			return;
		}
		this.AddScore(1, 0, 0, -1, 0);
		if (this.soundLiving != null && this.soundLivingID >= 0)
		{
			Manager.Stop(this.entityId, this.soundLiving);
			this.soundLivingID = -1;
		}
		if (this.AttachedToEntity)
		{
			this.Detach();
		}
		if (this.isEntityRemote)
		{
			return;
		}
		this.AwardKill(this.entityThatKilledMe);
		if (this.particleOnDeath != null && this.particleOnDeath.Length > 0)
		{
			float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
			this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.particleOnDeath, this.getHeadPosition(), lightBrightness, Color.white, null, null, false, 1f, ""), this.entityId, false, false);
		}
		if (this.isGameMessageOnDeath())
		{
			GameManager.Instance.GameMessage(EnumGameMessages.EntityWasKilled, this, this.entityThatKilledMe);
		}
		if (this.entityThatKilledMe != null)
		{
			Log.Out("Entity {0} {1} killed by {2} {3}", new object[]
			{
				base.GetDebugName(),
				this.entityId,
				this.entityThatKilledMe.GetDebugName(),
				this.entityThatKilledMe.entityId
			});
		}
		else
		{
			Log.Out("Entity {0} {1} killed", new object[]
			{
				base.GetDebugName(),
				this.entityId
			});
		}
		ModEvents.SEntityKilledData sentityKilledData = new ModEvents.SEntityKilledData(this, this.entityThatKilledMe);
		ModEvents.EntityKilled.Invoke(ref sentityKilledData);
		this.dropItemOnDeath();
		this.entityThatKilledMe = null;
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000D5FD6 File Offset: 0x000D41D6
	public void Disintegrate()
	{
		this.timeStayAfterDeath = 0;
		this.isDisintegrated = true;
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000D5FE6 File Offset: 0x000D41E6
	public virtual void PlayGiveUpSound()
	{
		if (this.soundGiveUp != null)
		{
			this.PlayOneShot(this.soundGiveUp, false, false, false, null, 1f);
		}
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000D6005 File Offset: 0x000D4205
	public virtual Vector3 GetCameraLook(float _t)
	{
		return this.GetLookVector();
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000D6010 File Offset: 0x000D4210
	public Vector3 GetForwardVector()
	{
		float num = Mathf.Cos(this.rotation.y * 0.0175f - 3.1415927f);
		float num2 = Mathf.Sin(this.rotation.y * 0.0175f - 3.1415927f);
		float num3 = -Mathf.Cos(0f);
		float y = Mathf.Sin(0f);
		return new Vector3(num2 * num3, y, num * num3);
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000D6078 File Offset: 0x000D4278
	public Vector2 GetForwardVector2()
	{
		float f = this.rotation.y * 0.017453292f;
		float y = Mathf.Cos(f);
		return new Vector2(Mathf.Sin(f), y);
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000D60A8 File Offset: 0x000D42A8
	public virtual Vector3 GetLookVector()
	{
		float num = Mathf.Cos(this.rotation.y * 0.0175f - 3.1415927f);
		float num2 = Mathf.Sin(this.rotation.y * 0.0175f - 3.1415927f);
		float num3 = -Mathf.Cos(this.rotation.x * 0.0175f);
		float y = Mathf.Sin(this.rotation.x * 0.0175f);
		return new Vector3(num2 * num3, y, num * num3);
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000D6005 File Offset: 0x000D4205
	public virtual Vector3 GetLookVector(Vector3 _altLookVector)
	{
		return this.GetLookVector();
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000D6128 File Offset: 0x000D4328
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetSoundRandomTicks()
	{
		return this.rand.RandomRange(this.soundRandomTicks / 2, this.soundRandomTicks);
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000D6143 File Offset: 0x000D4343
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetSoundAlertTicks()
	{
		return this.rand.RandomRange(this.soundAlertTicks / 2, this.soundAlertTicks);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000D615E File Offset: 0x000D435E
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundRandom()
	{
		return this.soundRandom;
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000D6166 File Offset: 0x000D4366
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundJump()
	{
		return this.soundJump;
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000D616E File Offset: 0x000D436E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundHurt(DamageSource _damageSource, int _damageStrength)
	{
		return this.soundHurt;
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000D6176 File Offset: 0x000D4376
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundHurtSmall()
	{
		return this.soundHurtSmall;
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000D616E File Offset: 0x000D436E
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundHurt()
	{
		return this.soundHurt;
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000D617E File Offset: 0x000D437E
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundDistressed()
	{
		return this.soundDistressed;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000D6186 File Offset: 0x000D4386
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundDrownPain()
	{
		return this.soundDrownPain;
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000D618E File Offset: 0x000D438E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundDeath(DamageSource _damageSource)
	{
		return this.soundDeath;
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000D6196 File Offset: 0x000D4396
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundAttack()
	{
		return this.soundAttack;
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000D619E File Offset: 0x000D439E
	public virtual string GetSoundAlert()
	{
		return this.soundAlert;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000D61A6 File Offset: 0x000D43A6
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundStamina()
	{
		return this.soundStamina;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000D61AE File Offset: 0x000D43AE
	public virtual string GetSoundSense()
	{
		return this.soundSense;
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000D61B6 File Offset: 0x000D43B6
	public virtual Ray GetLookRay()
	{
		return new Ray(this.position + new Vector3(0f, this.GetEyeHeight(), 0f), this.GetLookVector());
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000D61E3 File Offset: 0x000D43E3
	public virtual Ray GetMeleeRay()
	{
		return this.GetLookRay();
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000D61EC File Offset: 0x000D43EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void dropItemOnDeath()
	{
		if (!this.hasAI)
		{
			for (int i = 0; i < this.inventory.GetItemCount(); i++)
			{
				ItemStack item = this.inventory.GetItem(i);
				ItemClass forId = ItemClass.GetForId(item.itemValue.type);
				if (forId != null && forId.CanDrop(null))
				{
					this.world.GetGameManager().ItemDropServer(item, this.position, new Vector3(0.5f, 0f, 0.5f), -1, Constants.cItemDroppedOnDeathLifetime, false);
					this.inventory.SetItem(i, ItemValue.None, 0, true);
				}
			}
			this.inventory.SetFlashlight(false);
			this.equipment.DropItems();
		}
		if (this.entityThatKilledMe)
		{
			this.lootDropProb = EffectManager.GetValue(PassiveEffects.LootDropProb, this.entityThatKilledMe.inventory.holdingItemItemValue, this.lootDropProb, this.entityThatKilledMe, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		this.lootDropProb *= LootContainer.LootBagChance;
		if (!LootContainer.NoLoot && this.lootDropProb > this.rand.RandomFloat)
		{
			base.DropBagServer();
		}
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000D6320 File Offset: 0x000D4520
	public virtual Vector3 GetDropPosition()
	{
		if (this.ParachuteWearing || this.JetpackWearing)
		{
			return base.transform.position + base.transform.forward - Vector3.up * 0.3f + Origin.position;
		}
		return base.transform.position + base.transform.forward + Vector3.up + Origin.position;
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000D63A6 File Offset: 0x000D45A6
	public virtual void OnFired()
	{
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationFiring();
		}
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000D63CB File Offset: 0x000D45CB
	public virtual void OnReloadStart()
	{
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationReloading();
		}
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnReloadEnd()
	{
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool WillForceToFollow(EntityAlive _other)
	{
		return false;
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000D63F0 File Offset: 0x000D45F0
	public void AddHealth(int _v)
	{
		if (this.Health <= 0)
		{
			return;
		}
		this.Health += _v;
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000D640A File Offset: 0x000D460A
	public void AddStamina(float _v)
	{
		if (this.entityStats.Stamina != null && this.Health > 0)
		{
			this.entityStats.Stamina.Value += _v;
		}
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000D643A File Offset: 0x000D463A
	public void AddWater(float _v)
	{
		this.Stats.Water.Value += _v;
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000D6454 File Offset: 0x000D4654
	public int GetTicksNoPlayerAdjacent()
	{
		return this.ticksNoPlayerAdjacent;
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000D645C File Offset: 0x000D465C
	public bool CanSee(EntityAlive _other)
	{
		return this.seeCache.CanSee(_other);
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000D646A File Offset: 0x000D466A
	public void SetCanSee(EntityAlive _other)
	{
		this.seeCache.SetCanSee(_other);
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000D6478 File Offset: 0x000D4678
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateTasks()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving) && !(this is EntityDrone))
		{
			this.SetMoveForwardWithModifiers(0f, 0f, 0f, false);
			if (this.aiManager != null)
			{
				this.aiManager.UpdateDebugName();
			}
			return;
		}
		this.CheckDespawn();
		this.seeCache.ClearIfExpired();
		bool useAIPackages = EntityClass.list[this.entityClass].UseAIPackages;
		this.aiActiveDelay -= this.aiActiveScale;
		if (this.aiActiveDelay <= 0f)
		{
			this.aiActiveDelay = 1f;
			if (!useAIPackages)
			{
				this.aiManager.Update();
			}
			else
			{
				UAIBase.Update(this.utilityAIContext);
			}
		}
		PathInfo path = PathFinderThread.Instance.GetPath(this.entityId);
		if (((path != null) ? path.path : null) != null)
		{
			bool flag = true;
			if (!useAIPackages)
			{
				flag = this.aiManager.CheckPath(path);
			}
			if (flag)
			{
				this.navigator.SetPath(path, path.speed);
			}
		}
		this.navigator.UpdateNavigation();
		this.moveHelper.UpdateMoveHelper();
		this.lookHelper.onUpdateLook();
		if (this.distraction != null && (this.distraction.IsDead() || this.distraction.IsMarkedForUnload()))
		{
			this.distraction = null;
		}
		if (this.pendingDistraction != null && (this.pendingDistraction.IsDead() || this.pendingDistraction.IsMarkedForUnload()))
		{
			this.pendingDistraction = null;
		}
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000D65F3 File Offset: 0x000D47F3
	public PathNavigate getNavigator()
	{
		return this.navigator;
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000D65FC File Offset: 0x000D47FC
	public void FindPath(Vector3 targetPos, float moveSpeed, bool canBreak, EAIBase behavior)
	{
		Vector3 vector = targetPos - this.position;
		if (vector.x * vector.x + vector.z * vector.z > 1225f)
		{
			if (vector.y > 45f)
			{
				targetPos.y = this.position.y + 45f;
			}
			else if (vector.y < -45f)
			{
				targetPos.y = this.position.y - 45f;
			}
		}
		PathFinderThread.Instance.FindPath(this, targetPos, moveSpeed, canBreak, behavior);
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000D6694 File Offset: 0x000D4894
	public bool isWithinHomeDistanceCurrentPosition()
	{
		return this.isWithinHomeDistance(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.position.y), Utils.Fastfloor(this.position.z));
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000D66CC File Offset: 0x000D48CC
	public bool isWithinHomeDistance(int _x, int _y, int _z)
	{
		return this.maximumHomeDistance < 0 || this.homePosition.getDistanceSquared(_x, _y, _z) < (float)(this.maximumHomeDistance * this.maximumHomeDistance);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000D66F7 File Offset: 0x000D48F7
	public void setHomeArea(Vector3i _pos, int _maxDistance)
	{
		this.homePosition.position = _pos;
		this.maximumHomeDistance = _maxDistance;
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000D670C File Offset: 0x000D490C
	public ChunkCoordinates getHomePosition()
	{
		return this.homePosition;
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000D6714 File Offset: 0x000D4914
	public int getMaximumHomeDistance()
	{
		return this.maximumHomeDistance;
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000D671C File Offset: 0x000D491C
	public void detachHome()
	{
		this.maximumHomeDistance = -1;
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000D6725 File Offset: 0x000D4925
	public bool hasHome()
	{
		return this.maximumHomeDistance >= 0;
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000D6733 File Offset: 0x000D4933
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool canDespawn()
	{
		return !this.IsClientControlled() && base.GetSpawnerSource() != EnumSpawnerSource.StaticSpawner && !this.IsSleeping;
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000D6751 File Offset: 0x000D4951
	public void ResetDespawnTime()
	{
		this.ticksNoPlayerAdjacent = 0;
		this.seeCache.SetLastTimePlayerSeen();
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000D6768 File Offset: 0x000D4968
	public void CheckDespawn()
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.CanUpdateEntity() && this.bIsChunkObserver && this.world.GetClosestPlayer(this, -1f, false) == null)
		{
			this.MarkToUnload();
			return;
		}
		if (!this.canDespawn())
		{
			return;
		}
		int num = this.despawnDelayCounter + 1;
		this.despawnDelayCounter = num;
		if (num < 20)
		{
			return;
		}
		this.despawnDelayCounter = 0;
		this.ticksNoPlayerAdjacent += 20;
		EnumSpawnerSource spawnerSource = base.GetSpawnerSource();
		EntityPlayer closestPlayer = this.world.GetClosestPlayer(this, -1f, false);
		if (spawnerSource == EnumSpawnerSource.Dynamic)
		{
			if (!closestPlayer)
			{
				if (!this.world.GetClosestPlayer(this, -1f, true))
				{
					this.Despawn();
				}
				return;
			}
		}
		else if (spawnerSource == EnumSpawnerSource.Biome && !this.world.GetClosestPlayer(this, 130f, false))
		{
			if (this.world.GetClosestPlayer(this, 20f, true))
			{
				this.isDespawnWhenPlayerFar = true;
			}
			else if (this.isDespawnWhenPlayerFar)
			{
				this.Despawn();
			}
		}
		if (!closestPlayer)
		{
			return;
		}
		float sqrMagnitude = (closestPlayer.position - this.position).sqrMagnitude;
		if (sqrMagnitude < 6400f)
		{
			this.ticksNoPlayerAdjacent = 0;
		}
		int num2 = int.MaxValue;
		float lastTimePlayerSeen = this.seeCache.GetLastTimePlayerSeen();
		if (lastTimePlayerSeen > 0f)
		{
			num2 = (int)(Time.time - lastTimePlayerSeen);
		}
		switch (spawnerSource)
		{
		case EnumSpawnerSource.Biome:
			if (this.ticksNoPlayerAdjacent > 100 && sqrMagnitude > 16384f)
			{
				this.Despawn();
				return;
			}
			if (this.ticksNoPlayerAdjacent > 1800)
			{
				this.Despawn();
				return;
			}
			break;
		case EnumSpawnerSource.StaticSpawner:
			break;
		case EnumSpawnerSource.Dynamic:
			if (this.attackTarget)
			{
				num2 = 0;
			}
			if (this.IsSleeper && !this.IsSleeping)
			{
				if (sqrMagnitude > 9216f && num2 > 80)
				{
					this.Despawn();
					return;
				}
			}
			else
			{
				if (sqrMagnitude > 2304f && num2 > 60 && !this.HasInvestigatePosition)
				{
					this.Despawn();
					return;
				}
				if (this.ticksNoPlayerAdjacent > 1800)
				{
					this.Despawn();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000D6978 File Offset: 0x000D4B78
	[PublicizedFrom(EAccessModifier.Private)]
	public void Despawn()
	{
		this.IsDespawned = true;
		this.MarkToUnload();
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000D6987 File Offset: 0x000D4B87
	public void ForceDespawn()
	{
		this.Despawn();
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000D698F File Offset: 0x000D4B8F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityAlive GetAttackTarget()
	{
		return this.attackTarget;
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000D6997 File Offset: 0x000D4B97
	public virtual Vector3 GetAttackTargetHitPosition()
	{
		return this.attackTarget.getChestPosition();
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000D69A4 File Offset: 0x000D4BA4
	public EntityAlive GetAttackTargetLocal()
	{
		if (this.isEntityRemote)
		{
			return this.attackTargetClient;
		}
		return this.attackTarget;
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x000D69BC File Offset: 0x000D4BBC
	public void SetAttackTarget(EntityAlive _attackTarget, int _attackTargetTime)
	{
		if (_attackTarget == this.attackTarget)
		{
			this.attackTargetTime = _attackTargetTime;
			return;
		}
		if (this.attackTarget)
		{
			this.attackTargetLast = this.attackTarget;
		}
		this.targetAlertChanged = false;
		if (_attackTarget)
		{
			if (_attackTarget != this.attackTargetLast)
			{
				this.targetAlertChanged = true;
				this.soundDelayTicks = this.rand.RandomRange(5, 20);
			}
			this.investigatePositionTicks = 0;
		}
		if (!this.isEntityRemote)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, -1, NetPackageManager.GetPackage<NetPackageSetAttackTarget>().Setup(this.entityId, _attackTarget ? _attackTarget.entityId : -1), false);
		}
		this.attackTarget = _attackTarget;
		this.attackTargetTime = _attackTargetTime;
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000D6A86 File Offset: 0x000D4C86
	public void SetAttackTargetClient(EntityAlive _attackTarget)
	{
		this.attackTargetClient = _attackTarget;
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002353 RID: 9043 RVA: 0x000D6A8F File Offset: 0x000D4C8F
	public bool HasInvestigatePosition
	{
		get
		{
			return this.investigatePositionTicks > 0;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002354 RID: 9044 RVA: 0x000D6A9A File Offset: 0x000D4C9A
	public Vector3 InvestigatePosition
	{
		get
		{
			return this.investigatePos;
		}
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000D6AA2 File Offset: 0x000D4CA2
	public int GetInvestigatePositionTicks()
	{
		return this.investigatePositionTicks;
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x000D6AAC File Offset: 0x000D4CAC
	public void ClearInvestigatePosition()
	{
		this.investigatePos = Vector3.zero;
		this.investigatePositionTicks = 0;
		this.ResetDespawnTime();
		int num = this.rand.RandomRange(20, 35) * 20;
		if (this.entityType == EntityType.Zombie)
		{
			num /= 2;
		}
		this.SetAlertTicks(num);
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000D6AF8 File Offset: 0x000D4CF8
	public int CalcInvestigateTicks(int _ticks, EntityAlive _investigateEntity)
	{
		float value = EffectManager.GetValue(PassiveEffects.EnemySearchDuration, null, 1f, _investigateEntity, null, EntityClass.list[this.entityClass].Tags, true, true, true, true, true, 1, true, false);
		return (int)((float)_ticks / value);
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000D6B3A File Offset: 0x000D4D3A
	public void SetInvestigatePosition(Vector3 pos, int ticks, bool isAlert = true)
	{
		this.investigatePos = pos;
		this.investigatePositionTicks = ticks;
		this.isInvestigateAlert = isAlert;
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000D6B51 File Offset: 0x000D4D51
	public int GetAlertTicks()
	{
		return this.alertTicks;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000D6B59 File Offset: 0x000D4D59
	public void SetAlertTicks(int ticks)
	{
		this.alertTicks = ticks;
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x0600235B RID: 9051 RVA: 0x000D6B62 File Offset: 0x000D4D62
	public virtual bool IsAlert
	{
		get
		{
			if (this.isEntityRemote)
			{
				return this.bReplicatedAlertFlag;
			}
			return this.isAlert;
		}
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x000D6B79 File Offset: 0x000D4D79
	public EntitySeeCache GetEntitySenses()
	{
		return this.seeCache;
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x0600235D RID: 9053 RVA: 0x000D6B81 File Offset: 0x000D4D81
	public virtual bool IsRunning
	{
		get
		{
			return this.IsBloodMoon || this.world.IsDark();
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000D6B98 File Offset: 0x000D4D98
	public virtual float GetMoveSpeed()
	{
		if (this.IsBloodMoon || this.world.IsDark())
		{
			return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, this.moveSpeedNight, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		return EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, this.moveSpeed, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000D6C04 File Offset: 0x000D4E04
	public virtual float GetMoveSpeedAggro()
	{
		if (this.IsBloodMoon || this.world.IsDark())
		{
			return EffectManager.GetValue(PassiveEffects.RunSpeed, null, this.moveSpeedAggroMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, this.moveSpeedAggro, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000D6C70 File Offset: 0x000D4E70
	public float GetMoveSpeedPanic()
	{
		return EffectManager.GetValue(PassiveEffects.RunSpeed, null, this.moveSpeedPanic, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000D6CA1 File Offset: 0x000D4EA1
	public override float GetWeight()
	{
		return this.weight;
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000D6CA9 File Offset: 0x000D4EA9
	public override float GetPushFactor()
	{
		return this.pushFactor;
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CanEntityJump()
	{
		return true;
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000D6CB1 File Offset: 0x000D4EB1
	public void SetMaxViewAngle(float _angle)
	{
		this.maxViewAngle = _angle;
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000D6CBA File Offset: 0x000D4EBA
	public virtual float GetMaxViewAngle()
	{
		return this.maxViewAngle;
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000D6CC2 File Offset: 0x000D4EC2
	public void SetSightLightThreshold(Vector2 _threshold)
	{
		this.sightLightThreshold = _threshold;
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000D6CCB File Offset: 0x000D4ECB
	public int GetModelLayer()
	{
		return this.emodel.GetModelTransform().gameObject.layer;
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000D6CE2 File Offset: 0x000D4EE2
	public virtual void SetModelLayer(int _layerId, bool force = false, string[] excludeTags = null)
	{
		Utils.SetLayerRecursively(this.emodel.GetModelTransform().gameObject, _layerId);
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000D6CFA File Offset: 0x000D4EFA
	public virtual void SetColliderLayer(int _layerId, bool _force = false)
	{
		Utils.SetColliderLayerRecursively(this.emodel.GetModelTransform().gameObject, _layerId);
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x00081502 File Offset: 0x0007F702
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual int GetMaxAttackTime()
	{
		return 10;
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000D6D12 File Offset: 0x000D4F12
	public int GetAttackTimeoutTicks()
	{
		if (!this.world.IsDark())
		{
			return this.attackTimeoutDay;
		}
		return this.attackTimeoutNight;
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000D6D2E File Offset: 0x000D4F2E
	public override void MarkToUnload()
	{
		base.MarkToUnload();
		this.deathUpdateTime = this.timeStayAfterDeath;
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x000D6D42 File Offset: 0x000D4F42
	public override bool IsMarkedForUnload()
	{
		return base.IsMarkedForUnload() && this.deathUpdateTime >= this.timeStayAfterDeath;
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x000D6D60 File Offset: 0x000D4F60
	public virtual bool IsAttackValid()
	{
		if (!(this is EntityPlayer))
		{
			if (this.Electrocuted)
			{
				return false;
			}
			if (this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
			{
				return false;
			}
		}
		return (!(this.emodel != null) || !(this.emodel.avatarController != null) || !this.emodel.avatarController.IsAttackPrevented()) && !this.IsDead() && (this.painResistPercent >= 1f || (this.hasBeenAttackedTime <= 0 && (this.emodel.avatarController == null || !this.emodel.avatarController.IsAnimationHitRunning())));
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x000D6E1E File Offset: 0x000D501E
	public virtual bool IsAttackImpact()
	{
		return this.emodel && this.emodel.avatarController && this.emodel.avatarController.IsAttackImpact();
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x000D6E51 File Offset: 0x000D5051
	public virtual void ShowHoldingItem(bool _show)
	{
		this.inventory.ShowRightHand(_show);
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x000D6E5F File Offset: 0x000D505F
	public virtual bool IsHoldingItemInUse(int _actionIndex)
	{
		ItemAction itemAction = this.inventory.holdingItem.Actions[_actionIndex];
		return itemAction != null && itemAction.IsActionRunning(this.inventory.holdingItemData.actionData[_actionIndex]);
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x000D6E94 File Offset: 0x000D5094
	public virtual bool UseHoldingItem(int _actionIndex, bool _isReleased)
	{
		if (!_isReleased)
		{
			if (_actionIndex == 0 && this.emodel && this.emodel.avatarController && this.emodel.avatarController.IsAnimationAttackPlaying())
			{
				return false;
			}
			if (!this.IsAttackValid())
			{
				return false;
			}
		}
		if (_actionIndex == 0 && _isReleased && this.GetSoundAttack() != null)
		{
			this.PlayOneShot(this.GetSoundAttack(), false, false, false, null, 1f);
		}
		this.attackingTime = 60;
		ItemAction itemAction = this.inventory.holdingItem.Actions[_actionIndex];
		if (itemAction != null)
		{
			itemAction.ExecuteAction(this.inventory.holdingItemData.actionData[_actionIndex], _isReleased);
		}
		return true;
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000D6F44 File Offset: 0x000D5144
	public bool Attack(bool _isReleased)
	{
		return this.UseHoldingItem(0, _isReleased);
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000D6F50 File Offset: 0x000D5150
	public Entity GetTargetIfAttackedNow()
	{
		if (!this.IsAttackValid())
		{
			return null;
		}
		ItemClass holdingItem = this.inventory.holdingItem;
		if (holdingItem != null)
		{
			ItemAction itemAction = holdingItem.Actions[0];
			if (itemAction != null)
			{
				WorldRayHitInfo executeActionTarget = itemAction.GetExecuteActionTarget(this.inventory.holdingItemData.actionData[0]);
				if (executeActionTarget != null && executeActionTarget.bHitValid && executeActionTarget.transform)
				{
					float num = itemAction.Range;
					if (num == 0f)
					{
						ItemValue holdingItemItemValue = this.inventory.holdingItemItemValue;
						num = EffectManager.GetItemValue(PassiveEffects.MaxRange, holdingItemItemValue, 0f);
					}
					num += 0.3f;
					if (executeActionTarget.hit.distanceSq <= num * num)
					{
						Transform transform = executeActionTarget.transform;
						if (executeActionTarget.tag.StartsWith("E_BP_"))
						{
							transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, executeActionTarget.transform);
						}
						if (transform != null)
						{
							Entity component = transform.GetComponent<Entity>();
							if (component)
							{
								return component;
							}
						}
						if (executeActionTarget.tag == "E_Vehicle")
						{
							return EntityVehicle.FindCollisionEntity(transform);
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000D7074 File Offset: 0x000D5274
	public virtual float GetBlockDamageScale(bool isTerrain)
	{
		if (this.IsBloodMoon)
		{
			return ItemActionAttack.BMBlockDamagePercent;
		}
		return ItemActionAttack.EntityBlockDamagePercent;
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000D7089 File Offset: 0x000D5289
	public virtual void PlayStepSound(float _volume)
	{
		this.internalPlayStepSound(_volume);
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000D7094 File Offset: 0x000D5294
	[PublicizedFrom(EAccessModifier.Protected)]
	public void internalPlayStepSound(float _volume)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		if ((!this.onGround && !base.IsInElevator()) || this.isHeadUnderwater)
		{
			if (!(this is EntityPlayerLocal) && (this.isHeadUnderwater || this.world.IsWater(this.blockPosStandingOn)))
			{
				Manager.Play(this, "player_swim", 1f, false);
			}
			return;
		}
		BlockValue blockValue = this.blockValueStandingOn;
		Vector3i vector3i = this.blockPosStandingOn;
		vector3i.y++;
		BlockValue blockValue2 = this.world.GetBlock(vector3i);
		if (blockValue2.Block.blockMaterial.stepSound != null)
		{
			blockValue = blockValue2;
		}
		else if (blockValue.Block.blockMaterial.stepSound == null)
		{
			BlockValue block;
			blockValue2 = (block = this.world.GetBlock(vector3i + Vector3i.right));
			if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
			{
				blockValue = blockValue2;
			}
			else
			{
				blockValue2 = (block = this.world.GetBlock(vector3i - Vector3i.right));
				if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
				{
					blockValue = blockValue2;
				}
				else
				{
					blockValue2 = (block = this.world.GetBlock(vector3i + Vector3i.forward));
					if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
					{
						blockValue = blockValue2;
					}
					else
					{
						blockValue2 = (block = this.world.GetBlock(vector3i - Vector3i.forward));
						if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
						{
							blockValue = blockValue2;
						}
					}
				}
			}
		}
		if (blockValue.isair)
		{
			return;
		}
		Block block2 = blockValue.Block;
		if (EffectManager.GetValue(PassiveEffects.SilenceBlockSteps, null, 0f, this, null, block2.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			return;
		}
		MaterialBlock materialForSide = block2.GetMaterialForSide(blockValue, BlockFace.Top);
		if (materialForSide != null && materialForSide.stepSound != null)
		{
			string name = materialForSide.stepSound.name;
			if (name.Length > 0)
			{
				string stepSound = this.soundStepType + name;
				this.PlayStepSound(stepSound, _volume);
			}
		}
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000D72C0 File Offset: 0x000D54C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateStepSound(float _distX, float _distZ, float _rotYDelta)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		float num = Mathf.Sqrt(_distX * _distX + _distZ * _distZ);
		if (!this.onGround || this.isHeadUnderwater)
		{
			this.distanceSwam += num;
			if (this.distanceSwam > this.nextSwimDistance)
			{
				this.nextSwimDistance += 1f;
				if (this.nextSwimDistance < this.distanceSwam || this.nextSwimDistance > this.distanceSwam + 1f)
				{
					this.nextSwimDistance = this.distanceSwam + 1f;
				}
				this.internalPlayStepSound(1f);
			}
			return;
		}
		this.distanceWalked += num;
		if (num == 0f)
		{
			this.stepSoundDistanceRemaining = 0.25f;
		}
		else
		{
			this.stepSoundDistanceRemaining -= num;
			if (this.stepSoundDistanceRemaining <= 0f)
			{
				this.stepSoundDistanceRemaining = this.getNextStepSoundDistance();
				this.internalPlayStepSound(1f);
			}
		}
		this.stepSoundRotYRemaining -= Utils.FastAbs(_rotYDelta);
		if (this.stepSoundRotYRemaining <= 0f)
		{
			this.stepSoundRotYRemaining = 90f;
			this.internalPlayStepSound(1f);
		}
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000D73F4 File Offset: 0x000D55F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updatePlayerLandSound(float _distXZ, float _diffY)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		if (_distXZ >= 0.025f || Utils.FastAbs(_diffY) >= 0.015f)
		{
			float num = this.inWaterPercent * 2f;
			float x = num - this.landWaterLevel;
			this.landWaterLevel = num;
			float num2 = Utils.FastAbs(x);
			if (num > 0f)
			{
				num2 = Utils.FastMax(num2, _distXZ);
			}
			if (num2 >= 0.02f)
			{
				float volumeScale = Utils.FastMin(num2 * 2.2f + 0.01f, 1f);
				Manager.Play(this, "player_swim", volumeScale, false);
			}
		}
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000D7484 File Offset: 0x000D5684
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateCurrentBlockPosAndValue()
	{
		Vector3i vector3i = base.GetBlockPosition();
		BlockValue block = this.world.GetBlock(vector3i);
		if (block.isair)
		{
			vector3i.y--;
			block = this.world.GetBlock(vector3i);
		}
		if (block.ischild)
		{
			vector3i += block.parent;
			block = this.world.GetBlock(vector3i);
		}
		if (this.blockPosStandingOn != vector3i || !this.blockValueStandingOn.Equals(block) || (this.onGround && !this.wasOnGround))
		{
			this.blockPosStandingOn = vector3i;
			this.blockValueStandingOn = block;
			this.blockStandingOnChanged = !this.world.IsRemote();
			BiomeDefinition biome = this.world.GetBiome(this.blockPosStandingOn.x, this.blockPosStandingOn.z);
			if (biome != null && this.biomeStandingOn != biome && (this.biomeStandingOn == null || this.biomeStandingOn.m_Id != biome.m_Id))
			{
				this.onNewBiomeEntered(biome);
			}
		}
		this.CalcIfInElevator();
		Block block2 = this.blockValueStandingOn.Block;
		if (block2.BuffsWhenWalkedOn != null && block2.UseBuffsWhenWalkedOn(this.world, this.blockPosStandingOn, this.blockValueStandingOn))
		{
			bool flag = true;
			TileEntityWorkstation tileEntityWorkstation = this.world.GetTileEntity(this.blockPosStandingOn) as TileEntityWorkstation;
			if (tileEntityWorkstation != null)
			{
				flag = tileEntityWorkstation.IsBurning;
			}
			if (flag)
			{
				for (int i = 0; i < block2.BuffsWhenWalkedOn.Length; i++)
				{
					BuffValue buff = this.Buffs.GetBuff(block2.BuffsWhenWalkedOn[i]);
					if (buff == null || buff.DurationInSeconds >= 1f)
					{
						this.Buffs.AddBuff(block2.BuffsWhenWalkedOn[i], vector3i, -1, true, false, -1f);
					}
				}
			}
		}
		if (this.onGround && !this.IsFlyMode.Value)
		{
			if (block2.MovementFactor != 1f && block2.HasCollidingAABB(this.blockValueStandingOn, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z, 0f, this.boundingBox))
			{
				this.SetMotionMultiplier(EffectManager.GetValue(PassiveEffects.MovementFactorMultiplier, null, block2.MovementFactor, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			}
			if (this.blockStandingOnChanged)
			{
				this.blockStandingOnChanged = false;
				if (!this.blockValueStandingOn.isair)
				{
					block2.OnEntityWalking(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z, this.blockValueStandingOn, this);
					if (GameManager.bPhysicsActive && !this.blockValueStandingOn.ischild && !this.blockValueStandingOn.Block.isOversized)
					{
						Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(vector3i.x, vector3i.z);
						if (chunk != null && !chunk.StopStabilityCalculation && chunk.GetStability(World.toBlockXZ(this.blockPosStandingOn.x), World.toBlockY(this.blockPosStandingOn.y), World.toBlockXZ(this.blockPosStandingOn.z)) == 0 && Block.CanFallBelow(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z))
						{
							Log.Warning("EntityAlive {0} AddFallingBlock stab 0 happens?", new object[]
							{
								this.EntityName
							});
							this.world.AddFallingBlock(this.blockPosStandingOn, false);
						}
					}
				}
				BlockValue block3 = this.world.GetBlock(this.blockPosStandingOn + Vector3i.up);
				if (!block3.isair)
				{
					block3.Block.OnEntityWalking(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y + 1, this.blockPosStandingOn.z, block3, this);
				}
			}
		}
		this.HandleLootStageMaxCheck();
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleLootStageMaxCheck()
	{
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x000D7888 File Offset: 0x000D5A88
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcIfInElevator()
	{
		if (!this.bCanClimbLadders)
		{
			this.bInElevator = false;
			return;
		}
		ChunkCluster chunkCache = this.world.ChunkCache;
		Vector3i pos = new Vector3i(this.blockPosStandingOn.x, Utils.Fastfloor(this.boundingBox.min.y), this.blockPosStandingOn.z);
		BlockValue block = chunkCache.GetBlock(pos);
		Block block2 = block.Block;
		this.bInElevator = block2.IsElevator((int)block.rotation);
		pos.y++;
		block = chunkCache.GetBlock(pos);
		block2 = block.Block;
		this.bInElevator |= block2.IsElevator((int)block.rotation);
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000D793A File Offset: 0x000D5B3A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float getNextStepSoundDistance()
	{
		return 1.5f;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000D7941 File Offset: 0x000D5B41
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onNewBiomeEntered(BiomeDefinition _biome)
	{
		this.biomeStandingOn = _biome;
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000D794C File Offset: 0x000D5B4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateSpeedForwardAndStrafe(Vector3 _dist, float _partialTicks)
	{
		if (this.isEntityRemote && _partialTicks > 1f)
		{
			_dist /= _partialTicks;
		}
		this.speedForward *= 0.5f;
		this.speedStrafe *= 0.5f;
		this.speedVertical *= 0.5f;
		if (Mathf.Abs(_dist.x) > 0.001f || Mathf.Abs(_dist.z) > 0.001f)
		{
			float num = Mathf.Sin(-this.rotation.y * 3.1415927f / 180f);
			float num2 = Mathf.Cos(-this.rotation.y * 3.1415927f / 180f);
			this.speedForward += num2 * _dist.z - num * _dist.x;
			this.speedStrafe += num2 * _dist.x + num * _dist.z;
		}
		if (Mathf.Abs(_dist.y) > 0.001f)
		{
			this.speedVertical += _dist.y;
		}
		this.SetMovementState();
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000D7A6F File Offset: 0x000D5C6F
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PlayStepSound(string stepSound, float _volume)
	{
		if (this is EntityPlayerLocal)
		{
			Manager.BroadcastPlay(this, stepSound, false, 1f);
			return;
		}
		Manager.Play(this, stepSound, 1f, false);
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x000D7A98 File Offset: 0x000D5C98
	public void SetLookPosition(Vector3 _lookPos)
	{
		if ((this.lookAtPosition - _lookPos).sqrMagnitude < 0.0016f)
		{
			return;
		}
		this.lookAtPosition = _lookPos;
		if (this.world.entityDistributer != null)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.world.GetPrimaryPlayerId(), NetPackageManager.GetPackage<NetPackageEntityLookAt>().Setup(this.entityId, _lookPos), false);
		}
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.SetLookPosition(_lookPos);
		}
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isRadiationSensitive()
	{
		return true;
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsAimingGunPossible()
	{
		return true;
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000D7B2B File Offset: 0x000D5D2B
	public int GetDeathTime()
	{
		return this.deathUpdateTime;
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000D7B33 File Offset: 0x000D5D33
	public void SetDeathTime(int _deathTime)
	{
		this.deathUpdateTime = _deathTime;
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000D7B3C File Offset: 0x000D5D3C
	public int GetTimeStayAfterDeath()
	{
		return this.timeStayAfterDeath;
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000D7B44 File Offset: 0x000D5D44
	public bool IsCorpse()
	{
		return this.emodel && this.emodel.IsRagdollDead && (float)this.deathUpdateTime > 70f;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000D7B74 File Offset: 0x000D5D74
	public override void OnAddedToWorld()
	{
		if (!(this is EntityPlayerLocal))
		{
			OcclusionManager.AddEntity(this, 7f);
		}
		this.m_addedToWorld = true;
		if (!this.isEntityRemote)
		{
			this.bSpawned = true;
		}
		if (this as EntityPlayer == null)
		{
			this.FireEvent(MinEventTypes.onSelfFirstSpawn, true);
		}
		this.StartStopLivingSound();
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000D7BC8 File Offset: 0x000D5DC8
	public override void OnEntityUnload()
	{
		if (!(this is EntityPlayerLocal))
		{
			OcclusionManager.RemoveEntity(this);
		}
		if (this.navigator != null)
		{
			this.navigator.SetPath(null, 0f);
			this.navigator = null;
		}
		base.OnEntityUnload();
		this.lookHelper = null;
		this.moveHelper = null;
		this.seeCache = null;
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000D7C1F File Offset: 0x000D5E1F
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetDamageFraction(float _damage)
	{
		return _damage / (float)this.GetMaxHealth();
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x000D7C2C File Offset: 0x000D5E2C
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetDismemberChance(ref DamageResponse _dmResponse, float damagePer)
	{
		EnumBodyPartHit hitBodyPart = _dmResponse.HitBodyPart;
		EntityClass entityClass = EntityClass.list[this.entityClass];
		float num = 0f;
		switch (hitBodyPart.ToPrimary())
		{
		case BodyPrimaryHit.Head:
			num = entityClass.DismemberMultiplierHead;
			break;
		case BodyPrimaryHit.LeftUpperArm:
		case BodyPrimaryHit.RightUpperArm:
		case BodyPrimaryHit.LeftLowerArm:
		case BodyPrimaryHit.RightLowerArm:
			num = entityClass.DismemberMultiplierArms;
			break;
		case BodyPrimaryHit.LeftUpperLeg:
		case BodyPrimaryHit.RightUpperLeg:
		case BodyPrimaryHit.LeftLowerLeg:
		case BodyPrimaryHit.RightLowerLeg:
			num = entityClass.DismemberMultiplierLegs;
			break;
		}
		num = EffectManager.GetValue(PassiveEffects.DismemberSelfChance, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		float dismemberChance = _dmResponse.Source.DismemberChance;
		float num2 = (dismemberChance < 100f) ? (dismemberChance * damagePer * num) : 100f;
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityPlayerLocal;
		if (entityPlayerLocal && entityPlayerLocal.DebugDismembermentChance)
		{
			num2 = 1f;
		}
		if (DismembermentManager.DebugLogEnabled && num2 > 0f)
		{
			Log.Out("[EntityAlive.GetDismemberChance] - {0}, primary {1}, damage {2}, chance {3} * damage% {4} * multiplier {5} = {6}", new object[]
			{
				hitBodyPart,
				hitBodyPart.ToPrimary(),
				_dmResponse.Strength,
				dismemberChance.ToCultureInvariantString(),
				damagePer.ToCultureInvariantString(),
				num.ToCultureInvariantString(),
				num2.ToCultureInvariantString()
			});
		}
		return num2;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000D7D90 File Offset: 0x000D5F90
	public virtual void CheckDismember(ref DamageResponse _dmResponse, float damagePer)
	{
		bool flag = _dmResponse.HitBodyPart.IsLeg();
		if (flag && base.IsAlive() && (this.bodyDamage.CurrentStun != EnumEntityStunType.None || this.sleepingOrWakingUp))
		{
			return;
		}
		float dismemberChance = this.GetDismemberChance(ref _dmResponse, damagePer);
		if (dismemberChance > 0f && this.rand.RandomFloat <= dismemberChance)
		{
			_dmResponse.Dismember = true;
			if (flag)
			{
				_dmResponse.TurnIntoCrawler = true;
			}
			return;
		}
		if (flag)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (entityClass.LegCrawlerThreshold > 0f && this.GetDamageFraction((float)_dmResponse.Strength) >= entityClass.LegCrawlerThreshold)
			{
				_dmResponse.TurnIntoCrawler = true;
			}
			if (!this.bodyDamage.ShouldBeCrawler && !_dmResponse.TurnIntoCrawler && entityClass.LegCrippleScale > 0f)
			{
				float num = this.GetDamageFraction((float)_dmResponse.Strength) * entityClass.LegCrippleScale;
				if (num >= 0.05f)
				{
					if ((this.bodyDamage.Flags & 4096U) == 0U && _dmResponse.HitBodyPart.IsLeftLeg() && this.rand.RandomFloat < num)
					{
						_dmResponse.CrippleLegs = true;
					}
					if ((this.bodyDamage.Flags & 8192U) == 0U && _dmResponse.HitBodyPart.IsRightLeg() && this.rand.RandomFloat < num)
					{
						_dmResponse.CrippleLegs = true;
					}
				}
			}
		}
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000D7EF0 File Offset: 0x000D60F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyLocalBodyDamage(DamageResponse _dmResponse)
	{
		EnumBodyPartHit enumBodyPartHit = _dmResponse.HitBodyPart;
		this.bodyDamage.bodyPartHit = enumBodyPartHit;
		this.bodyDamage.damageType = _dmResponse.Source.damageType;
		if (_dmResponse.Dismember)
		{
			if (DismembermentManager.DebugBodyPartHit != EnumBodyPartHit.None)
			{
				enumBodyPartHit = DismembermentManager.DebugBodyPartHit;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 1U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftUpperArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 2U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftLowerArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 4U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightUpperArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 8U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightLowerArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 16U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftUpperLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 32U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftLowerLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 64U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightUpperLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 128U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightLowerLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 256U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
		}
		if (_dmResponse.TurnIntoCrawler)
		{
			this.bodyDamage.ShouldBeCrawler = true;
		}
		if (_dmResponse.CrippleLegs)
		{
			if (_dmResponse.HitBodyPart.IsLeftLeg())
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 4096U);
			}
			if (_dmResponse.HitBodyPart.IsRightLeg())
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 8192U);
			}
		}
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000D80AC File Offset: 0x000D62AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ExecuteDismember(bool restoreState)
	{
		if (this.emodel == null || this.emodel.avatarController == null)
		{
			return;
		}
		if (this.bodyDamage.IsCrippled && this.bodyDamage.bodyPartHit.IsLeg() && base.IsAlive() && this.walkType != 5 && this.walkType < 20)
		{
			this.SetWalkType(5);
		}
		this.emodel.avatarController.DismemberLimb(this.bodyDamage, restoreState);
		if (this.bodyDamage.ShouldBeCrawler)
		{
			this.SetupCrawler();
		}
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000D8148 File Offset: 0x000D6348
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupCrawler()
	{
		if (base.IsAlive())
		{
			this.SetWalkType(21);
			base.SetMaxHeight(0.5f);
			ItemValue itemValue = null;
			if (EntityClass.list[this.entityClass].Properties.Values.ContainsKey(EntityClass.PropHandItemCrawler))
			{
				itemValue = ItemClass.GetItem(EntityClass.list[this.entityClass].Properties.Values[EntityClass.PropHandItemCrawler], false);
				if (itemValue.IsEmpty())
				{
					itemValue = null;
				}
			}
			if (itemValue == null)
			{
				itemValue = ItemClass.GetItem("meleeHandZombie02", false);
			}
			this.inventory.SetBareHandItem(itemValue);
			this.TurnIntoCrawler();
		}
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void TurnIntoCrawler()
	{
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000D81F1 File Offset: 0x000D63F1
	public void ClearStun()
	{
		this.bodyDamage.CurrentStun = EnumEntityStunType.None;
		this.bodyDamage.StunDuration = 0f;
		this.SetCVar("_stunned", 0f);
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000D821F File Offset: 0x000D641F
	public void SetStun(EnumEntityStunType stun)
	{
		this.bodyDamage.CurrentStun = stun;
		this.SetCVar("_stunned", 1f);
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000D823D File Offset: 0x000D643D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onSpawnStateChanged()
	{
		if (!this.m_addedToWorld)
		{
			return;
		}
		this.StartStopLivingSound();
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000D8250 File Offset: 0x000D6450
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartStopLivingSound()
	{
		if (this.soundLiving != null)
		{
			if (this.Spawned)
			{
				if (!this.IsDead() && this.Health > 0)
				{
					Manager.Play(this, this.soundLiving, 1f, false);
					this.soundLivingID = 0;
				}
			}
			else if (this.soundLivingID >= 0)
			{
				Manager.Stop(this.entityId, this.soundLiving);
				this.soundLivingID = -1;
			}
		}
		if (this.Spawned && this.soundSpawn != null && !this.SleeperSupressLivingSounds)
		{
			this.PlayOneShot(this.soundSpawn, false, false, false, null, 1f);
		}
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000D82E8 File Offset: 0x000D64E8
	public void CrouchHeightFixedUpdate()
	{
		if (this.crouchType == 0)
		{
			return;
		}
		if (this.physicsBaseHeight <= 1.3f)
		{
			return;
		}
		float num = this.physicsBaseHeight;
		if (base.IsInElevator())
		{
			num *= 1.06f;
		}
		if (this.emodel.IsRagdollMovement || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
		{
			num = this.physicsBaseHeight * 0.08f;
		}
		float num2 = this.m_characterController.GetRadius() * 0.9f;
		float num3 = num2 + 0.3f;
		float maxDistance = num + 0.01f - num3 - num2;
		Vector3 vector = this.PhysicsTransform.position;
		vector.y += num3;
		if (this.moveHelper != null && (this.moveHelper.BlockedFlags & 3) == 2)
		{
			vector += this.ModelTransform.forward * 0.15f;
		}
		RaycastHit raycastHit;
		if (Physics.SphereCast(vector, num2, Vector3.up, out raycastHit, maxDistance, 1083277320))
		{
			Transform transform = raycastHit.transform;
			if (transform && transform.CompareTag("Physics"))
			{
				Entity component = transform.GetComponent<Entity>();
				if (component)
				{
					component.PhysicsPush(transform.forward * (0.1f * Time.fixedDeltaTime), raycastHit.point, true);
				}
				return;
			}
			if (this.world.GetBlock(new Vector3i(raycastHit.point + Origin.position)).Block.Damage <= 0f)
			{
				num = raycastHit.point.y - (vector.y - num3) - 0.21f;
			}
		}
		if (num < this.physicsHeight)
		{
			if (base.IsInElevator())
			{
				return;
			}
			num = Utils.FastMoveTowards(this.physicsHeight, num, 0.099999994f);
		}
		else
		{
			num = Mathf.MoveTowards(this.physicsHeight, num, 0.016666666f);
		}
		base.SetHeight(num);
		float num4 = this.physicsBaseHeight * 0.7f;
		if (num <= num4)
		{
			this.crouchBendPerTarget = 0f;
			if (this.walkType != 8 && this.walkType != 21)
			{
				int num5 = this.walkType;
				this.SetWalkType(8);
				this.walkTypeBeforeCrouch = num5;
			}
		}
		else
		{
			this.crouchBendPerTarget = 1f - (num - num4) / (this.physicsBaseHeight - num4);
			if (this.walkTypeBeforeCrouch != 0)
			{
				int num6 = this.walkTypeBeforeCrouch;
				this.walkTypeBeforeCrouch = 0;
				this.SetWalkType(num6);
			}
		}
		this.crouchBendPer = Mathf.MoveTowards(this.crouchBendPer, this.crouchBendPerTarget, 0.099999994f);
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000D8564 File Offset: 0x000D6764
	public void SetWalkType(int _walkType)
	{
		if (this.walkType == 21)
		{
			return;
		}
		if (_walkType == 21)
		{
			this.walkType = _walkType;
			this.emodel.avatarController.TurnIntoCrawler();
			this.walkTypeBeforeCrouch = 0;
			return;
		}
		if (this.walkTypeBeforeCrouch != 0)
		{
			this.walkTypeBeforeCrouch = _walkType;
			return;
		}
		this.walkType = _walkType;
		this.emodel.avatarController.SetWalkType(_walkType, true);
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000D85C9 File Offset: 0x000D67C9
	public int GetWalkType()
	{
		return this.walkType;
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000D85D1 File Offset: 0x000D67D1
	public bool IsWalkTypeACrawl()
	{
		return this.walkType >= 20;
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000D85E0 File Offset: 0x000D67E0
	public string GetRightHandTransformName()
	{
		return this.rightHandTransformName;
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isGameMessageOnDeath()
	{
		return true;
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000D85E8 File Offset: 0x000D67E8
	public override float GetLightBrightness()
	{
		Vector3i blockPosition = base.GetBlockPosition();
		Vector3i blockPos = blockPosition;
		blockPos.y += Mathf.RoundToInt(base.height + 0.5f);
		return Utils.FastMax(this.world.GetLightBrightness(blockPosition), this.world.GetLightBrightness(blockPos));
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000D8638 File Offset: 0x000D6838
	public virtual float GetLightLevel()
	{
		EntityAlive entityAlive = this.AttachedToEntity as EntityAlive;
		if (entityAlive)
		{
			return entityAlive.GetLightLevel();
		}
		return this.inventory.GetLightLevel();
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000D866C File Offset: 0x000D686C
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			this.Crouching = false;
			if (!this.isEntityRemote)
			{
				this.saveInventory = null;
				if (_other is EntityAlive && _other.GetAttachedToInfo(slot).bReplaceLocalInventory)
				{
					this.saveInventory = this.inventory;
					this.saveHoldingItemIdxBeforeAttach = this.inventory.holdingItemIdx;
					this.inventory.SetHoldingItemIdxNoHolsterTime(this.inventory.DUMMY_SLOT_IDX);
					this.inventory = ((EntityAlive)_other).inventory;
				}
				this.bPlayerStatsChanged |= true;
			}
			else
			{
				this.ShowHoldingItem(false);
			}
		}
		return slot;
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000D8720 File Offset: 0x000D6920
	public override void Detach()
	{
		if (this.saveInventory != null)
		{
			this.inventory = this.saveInventory;
			this.inventory.SetHoldingItemIdxNoHolsterTime(this.saveHoldingItemIdxBeforeAttach);
			this.saveInventory = null;
		}
		base.Detach();
		this.bPlayerStatsChanged |= !this.isEntityRemote;
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000D8775 File Offset: 0x000D6975
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(this.deathHealth);
		_bw.Write(this.StressAmount);
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000D8797 File Offset: 0x000D6997
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version > 24)
		{
			this.deathHealth = _br.ReadInt32();
		}
		if (_version >= 36)
		{
			this.StressAmount = _br.ReadSingle();
		}
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000D87C3 File Offset: 0x000D69C3
	public override string ToString()
	{
		return string.Format("[type={0}, name={1}, id={2}]", base.GetType().Name, GameUtils.SafeStringFormat(this.EntityName), this.entityId);
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000D87F0 File Offset: 0x000D69F0
	public virtual void FireEvent(MinEventTypes _eventType, bool useInventory = true)
	{
		MinEffectController effects = EntityClass.list[this.entityClass].Effects;
		if (effects != null)
		{
			effects.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.Progression != null)
		{
			this.Progression.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.inventory != null && useInventory)
		{
			this.inventory.FireEvent(_eventType, this.MinEventContext);
		}
		this.equipment.FireEvent(_eventType, this.MinEventContext);
		this.Buffs.FireEvent(_eventType, this.MinEventContext);
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000D889A File Offset: 0x000D6A9A
	public float GetCVar(string _varName)
	{
		if (this.Buffs == null)
		{
			return 0f;
		}
		return this.Buffs.GetCustomVar(_varName);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000D88B6 File Offset: 0x000D6AB6
	public void SetCVar(string _varName, float _value)
	{
		if (this.Buffs == null)
		{
			return;
		}
		this.Buffs.SetCustomVar(_varName, _value, true, CVarOperation.set, false);
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void BuffAdded(BuffValue _buff)
	{
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000D88D4 File Offset: 0x000D6AD4
	public override void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
		if (!this.emodel.IsRagdollActive)
		{
			return;
		}
		if (collision.relativeVelocity.sqrMagnitude < 0.0625f)
		{
			return;
		}
		float sqrMagnitude = collision.impulse.sqrMagnitude;
		if (sqrMagnitude < 400f)
		{
			return;
		}
		if (this.IsDead())
		{
			EntityAlive.ImpactData impactData;
			this.impacts.TryGetValue(t, out impactData);
			impactData.count++;
			this.impacts[t] = impactData;
			if (impactData.count >= 10)
			{
				if (impactData.count == 10)
				{
					Rigidbody component = t.GetComponent<Rigidbody>();
					if (component)
					{
						component.velocity = Vector3.zero;
						component.angularVelocity = Vector3.zero;
						component.drag = 0.5f;
						component.angularDrag = 0.5f;
					}
					CharacterJoint component2 = t.GetComponent<CharacterJoint>();
					if (component2)
					{
						component2.enableProjection = false;
					}
				}
				if (impactData.count == 25 && !t.gameObject.CompareTag("E_BP_Body"))
				{
					t.GetComponent<Collider>().enabled = false;
				}
				return;
			}
		}
		if (Time.time - this.impactSoundTime < 0.25f)
		{
			return;
		}
		this.impactSoundTime = Time.time;
		if (t.lossyScale.x == 0f)
		{
			return;
		}
		string soundGroupName = "impactbodylight";
		if (sqrMagnitude >= 3600f)
		{
			soundGroupName = "impactbodyheavy";
		}
		Vector3 a = Vector3.zero;
		int contactCount = collision.contactCount;
		for (int i = 0; i < contactCount; i++)
		{
			a += collision.GetContact(i).point;
		}
		a *= 1f / (float)contactCount;
		Manager.BroadcastPlay(a + Origin.position, soundGroupName, 0f);
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000D8A90 File Offset: 0x000D6C90
	public void AddParticle(string _name, Transform _t)
	{
		if (this.particles.ContainsKey(_name))
		{
			this.particles[_name] = _t;
			return;
		}
		this.particles.Add(_name, _t);
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000D8ABC File Offset: 0x000D6CBC
	public bool RemoveParticle(string _name)
	{
		Transform transform;
		if (this.particles.Remove(_name, out transform))
		{
			if (transform)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000D8AF0 File Offset: 0x000D6CF0
	public bool HasParticle(string _name)
	{
		Transform transform;
		return this.particles.TryGetValue(_name, out transform);
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000D8B10 File Offset: 0x000D6D10
	public void AddPart(string _name, Transform _t)
	{
		if (this.parts.ContainsKey(_name))
		{
			this.parts[_name] = _t;
			return;
		}
		this.parts.Add(_name, _t);
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000D8B3C File Offset: 0x000D6D3C
	public void RemovePart(string _name)
	{
		Transform transform;
		if (this.parts.TryGetValue(_name, out transform))
		{
			this.parts.Remove(_name);
			if (transform)
			{
				transform.gameObject.name = ".";
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000D8B8C File Offset: 0x000D6D8C
	public void SetPartActive(string _name, bool isActive)
	{
		Transform transform;
		if (this.parts.TryGetValue(_name, out transform) && transform)
		{
			bool flag = true;
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				Transform child = transform.GetChild(i);
				if (child.CompareTag("ModOn"))
				{
					child.gameObject.SetActive(isActive);
					flag = false;
				}
				else if (child.CompareTag("ModMesh"))
				{
					if (transform.parent.name == "CameraNode")
					{
						child.gameObject.SetActive(false);
					}
					flag = false;
				}
			}
			if (flag)
			{
				transform.gameObject.SetActive(isActive);
			}
		}
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000D8C30 File Offset: 0x000D6E30
	public OwnedEntityData GetOwnedEntity(int _entityId)
	{
		return this.ownedEntities.Find((OwnedEntityData e) => e.Id == _entityId);
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000D8C61 File Offset: 0x000D6E61
	public bool HasOwnedEntity(int _entityId)
	{
		return this.GetOwnedEntity(_entityId) != null;
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000D8C70 File Offset: 0x000D6E70
	public void AddOwnedEntity(OwnedEntityData _data)
	{
		if (!this.HasOwnedEntity(_data.Id) && _data != null)
		{
			this.ownedEntities.Add(_data);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageOwnedEntitySync>().Setup(this.entityId, _data.Id, _data.ClassId, NetPackageOwnedEntitySync.SyncType.Add), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000D8CE0 File Offset: 0x000D6EE0
	public void AddOwnedEntity(Entity _entity)
	{
		this.AddOwnedEntity(new OwnedEntityData(_entity));
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000D8CF0 File Offset: 0x000D6EF0
	public void RemoveOwnedEntity(OwnedEntityData _data)
	{
		if (_data != null)
		{
			this.ownedEntities.Remove(_data);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageOwnedEntitySync>().Setup(this.entityId, _data.Id, _data.ClassId, NetPackageOwnedEntitySync.SyncType.Remove), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000D8D53 File Offset: 0x000D6F53
	public void RemoveOwnedEntity(int _entityId)
	{
		this.RemoveOwnedEntity(this.GetOwnedEntity(_entityId));
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000D8D62 File Offset: 0x000D6F62
	public void RemoveOwnedEntity(Entity _entity)
	{
		this.RemoveOwnedEntity(_entity.entityId);
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000D8D70 File Offset: 0x000D6F70
	public List<OwnedEntityData> GetOwnedEntities(int _classId)
	{
		List<OwnedEntityData> list = new List<OwnedEntityData>();
		for (int i = 0; i < this.ownedEntities.Count; i++)
		{
			OwnedEntityData ownedEntityData = this.ownedEntities[i];
			if (ownedEntityData.ClassId == _classId)
			{
				list.Add(ownedEntityData);
			}
		}
		return list;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000D8DB7 File Offset: 0x000D6FB7
	public void HandleSetNavName()
	{
		if (this.NavObject != null)
		{
			this.NavObject.name = this.entityName;
		}
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000D8DD4 File Offset: 0x000D6FD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDynamicRagdoll()
	{
		if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.Active))
		{
			if (this.accumulatedRootMotion != Vector3.zero)
			{
				this._dynamicRagdollRootMotion = this.accumulatedRootMotion;
			}
			if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.UseBoneVelocities))
			{
				this._ragdollPositionsPrev.Clear();
				this._ragdollPositionsCur.CopyTo(this._ragdollPositionsPrev);
				this.emodel.CaptureRagdollPositions(this._ragdollPositionsCur);
			}
			if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.RagdollOnFall) && !this.onGround)
			{
				this.ActivateDynamicRagdoll();
				return;
			}
		}
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AnalyticsSendDeath(DamageResponse _dmResponse)
	{
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x0004E558 File Offset: 0x0004C758
	public virtual string MakeDebugNameInfo()
	{
		return string.Empty;
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000D8E88 File Offset: 0x000D7088
	public static void SetupAllDebugNameHUDs(bool _isAdd)
	{
		List<Entity> list = GameManager.Instance.World.Entities.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityAlive entityAlive = list[i] as EntityAlive;
			if (entityAlive)
			{
				entityAlive.SetupDebugNameHUD(_isAdd);
			}
		}
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000D8ED8 File Offset: 0x000D70D8
	public void SetupDebugNameHUD(bool _isAdd)
	{
		if (this is EntityPlayer)
		{
			return;
		}
		GUIHUDEntityName component = this.ModelTransform.GetComponent<GUIHUDEntityName>();
		if (_isAdd)
		{
			if (!component)
			{
				this.ModelTransform.gameObject.AddComponent<GUIHUDEntityName>();
				return;
			}
		}
		else if (component)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000D8F25 File Offset: 0x000D7125
	public EModelBase.HeadStates GetHeadState()
	{
		if (base.EntityClass.CanBigHead)
		{
			return this.emodel.HeadState;
		}
		return EModelBase.HeadStates.Standard;
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000D8F44 File Offset: 0x000D7144
	public void SetBigHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && this.emodel.HeadState == EModelBase.HeadStates.Standard)
		{
			this.emodel.HeadState = EModelBase.HeadStates.Growing;
			Manager.BroadcastPlayByLocalPlayer(this.position, "twitch_bighead_inflate");
		}
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000D8F9F File Offset: 0x000D719F
	public void ForceBigHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && this.emodel.HeadState == EModelBase.HeadStates.Standard)
		{
			this.emodel.HeadState = EModelBase.HeadStates.BigHead;
		}
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000D8FE0 File Offset: 0x000D71E0
	public void ResetHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && (this.emodel.HeadState == EModelBase.HeadStates.BigHead || this.emodel.HeadState == EModelBase.HeadStates.Growing))
		{
			base.StartCoroutine(this.resetHeadLater(this.emodel));
		}
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000D9044 File Offset: 0x000D7244
	public void ForceResetHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && (this.emodel.HeadState == EModelBase.HeadStates.BigHead || this.emodel.HeadState == EModelBase.HeadStates.Growing))
		{
			this.emodel.HeadState = EModelBase.HeadStates.Standard;
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000D909E File Offset: 0x000D729E
	public void SetDancing(bool enabled)
	{
		if (base.EntityClass.DanceTypeID != 0)
		{
			this.IsDancing = enabled;
			return;
		}
		this.IsDancing = false;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000D90BC File Offset: 0x000D72BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator resetHeadLater(EModelBase model)
	{
		yield return new WaitForSeconds(0.25f);
		if (this.emodel != null && this.emodel.GetHeadTransform() != null && this.emodel.GetHeadTransform().localScale.x > 1f)
		{
			this.emodel.HeadState = EModelBase.HeadStates.Shrinking;
			Manager.BroadcastPlayByLocalPlayer(this.position, "twitch_bighead_deflate");
		}
		yield break;
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000D90CB File Offset: 0x000D72CB
	public void SetSpawnByData(int newSpawnByID, string newSpawnByName)
	{
		this.spawnById = newSpawnByID;
		this.spawnByName = newSpawnByName;
		this.bPlayerStatsChanged |= !this.isEntityRemote;
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000D90F1 File Offset: 0x000D72F1
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetHeadSize(float overrideHeadSize)
	{
		this.OverrideHeadSize = overrideHeadSize;
		this.emodel.SetHeadScale(overrideHeadSize);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000D9106 File Offset: 0x000D7306
	public void SetVehiclePoseMode(int _pose)
	{
		this.vehiclePoseMode = _pose;
		if (_pose != this.GetVehicleAnimation())
		{
			this.Crouching = false;
			this.SetVehicleAnimation(AvatarController.vehiclePoseHash, _pose);
		}
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000D912C File Offset: 0x000D732C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateNetworkStats()
	{
		if (this.networkStatsUpdateQueue.Count > 0)
		{
			EntityAlive.NetworkStatChange networkStatChange = this.networkStatsUpdateQueue[0];
			this.networkStatsUpdateQueue.RemoveAt(0);
			if (networkStatChange.m_NetworkStats != null)
			{
				networkStatChange.m_NetworkStats.ToEntity(this);
				return;
			}
			EntityAlive.EntityNetworkHoldingData holdingData = networkStatChange.m_HoldingData;
			if (holdingData != null)
			{
				ItemStack holdingItemStack = holdingData.m_HoldingItemStack;
				byte holdingItemIndex = holdingData.m_HoldingItemIndex;
				if (!this.inventory.GetItem((int)holdingItemIndex).Equals(holdingItemStack))
				{
					this.inventory.SetItem((int)holdingItemIndex, holdingItemStack);
				}
				if (this.inventory.holdingItemIdx != (int)holdingItemIndex)
				{
					this.inventory.SetHoldingItemIdxNoHolsterTime((int)holdingItemIndex);
				}
			}
		}
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000D91CC File Offset: 0x000D73CC
	public void EnqueueNetworkStats(EntityAlive.EntityNetworkStats netStats)
	{
		EntityAlive.NetworkStatChange networkStatChange = new EntityAlive.NetworkStatChange();
		networkStatChange.m_NetworkStats = netStats;
		this.networkStatsUpdateQueue.Add(networkStatChange);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000D91F4 File Offset: 0x000D73F4
	public void EnqueueNetworkHoldingData(ItemStack holdingItemStack, byte holdingItemIndex)
	{
		EntityAlive.NetworkStatChange networkStatChange = new EntityAlive.NetworkStatChange();
		networkStatChange.m_HoldingData = new EntityAlive.EntityNetworkHoldingData
		{
			m_HoldingItemStack = holdingItemStack,
			m_HoldingItemIndex = holdingItemIndex
		};
		this.networkStatsUpdateQueue.Add(networkStatChange);
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x000D9230 File Offset: 0x000D7430
	public virtual bool GetAimTarget(out Vector3 aimTarget)
	{
		EntityAlive attackTargetLocal = this.GetAttackTargetLocal();
		if (attackTargetLocal)
		{
			aimTarget = attackTargetLocal.getChestPosition();
			return true;
		}
		aimTarget = Vector3.zero;
		return false;
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000D9268 File Offset: 0x000D7468
	public virtual bool GetHeadLookTarget(out Vector3 lookTarget)
	{
		EntityAlive attackTargetLocal = this.GetAttackTargetLocal();
		if (attackTargetLocal && this.CanSee(attackTargetLocal))
		{
			lookTarget = attackTargetLocal.getHeadPosition();
			return true;
		}
		lookTarget = Vector3.zero;
		return false;
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000D92A7 File Offset: 0x000D74A7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		if (base.EntityClass.PickupItem != "")
		{
			_addCallback(new EntityActivationCommand("grab", "hand", null, null));
		}
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000D92D8 File Offset: 0x000D74D8
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_commandName, "grab"))
		{
			return !this.IsDead() && _playerFocusing.inventory.UsingBareHand() && base.EntityClass.PickupItem != "";
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool grabDisabled()
	{
		return false;
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000D9328 File Offset: 0x000D7528
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_command.commandId, "grab") && !this.grabDisabled())
		{
			ItemStack itemStack = new ItemStack(ItemClass.GetItem(base.EntityClass.PickupItem, false), 1);
			_playerFocusing.inventory.SetItem(_playerFocusing.inventory.holdingItemIdx, itemStack);
			_playerFocusing.MinEventContext.Other = this;
			_playerFocusing.FireEvent(MinEventTypes.onSelfPickupOther, true);
			this.MinEventContext.Other = _playerFocusing;
			this.FireEvent(MinEventTypes.onSelfPickedUpByOther, true);
			if (base.EntityClass.PickupStressCvar != "")
			{
				_playerFocusing.Buffs.SetCustomVar(base.EntityClass.PickupStressCvar, this.GetCVar(base.EntityClass.PickupStressCvar), true, CVarOperation.set, true);
			}
			base.Collect(_playerFocusing.entityId);
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000D9401 File Offset: 0x000D7601
	public override string GetActivationText()
	{
		if (base.EntityClass.PickupItem != "")
		{
			return string.Format(Localization.Get("overlayChickenGrab", false, null), Array.Empty<object>());
		}
		return string.Empty;
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ClearDistressed()
	{
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000D9438 File Offset: 0x000D7638
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive()
	{
	}

	// Token: 0x04001800 RID: 6144
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTraderTeleportCheckTime = 0.1f;

	// Token: 0x04001801 RID: 6145
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageImmunityOnRespawnSeconds = 1f;

	// Token: 0x04001802 RID: 6146
	public static readonly FastTags<TagGroup.Global> DistractionResistanceWithTargetTags = FastTags<TagGroup.Global>.GetTag("with_target");

	// Token: 0x04001803 RID: 6147
	public static readonly FastTags<TagGroup.Global> CoverWorthTags = FastTags<TagGroup.Global>.GetTag("coverWorthy");

	// Token: 0x04001804 RID: 6148
	public static readonly int FeralTagBit = FastTags<TagGroup.Global>.GetBit("feral");

	// Token: 0x04001805 RID: 6149
	public static readonly int FallingBuffTagBit = FastTags<TagGroup.Global>.GetBit("buffPlayerFallingDamage");

	// Token: 0x04001806 RID: 6150
	public static readonly FastTags<TagGroup.Global> StanceTagCrouching = FastTags<TagGroup.Global>.GetTag("crouching");

	// Token: 0x04001807 RID: 6151
	public static readonly FastTags<TagGroup.Global> StanceTagStanding = FastTags<TagGroup.Global>.GetTag("standing");

	// Token: 0x04001808 RID: 6152
	public static readonly FastTags<TagGroup.Global> MovementTagIdle = FastTags<TagGroup.Global>.GetTag("idle");

	// Token: 0x04001809 RID: 6153
	public static readonly FastTags<TagGroup.Global> MovementTagWalking = FastTags<TagGroup.Global>.GetTag("walking");

	// Token: 0x0400180A RID: 6154
	public static readonly FastTags<TagGroup.Global> MovementTagRunning = FastTags<TagGroup.Global>.GetTag("running");

	// Token: 0x0400180B RID: 6155
	public static readonly FastTags<TagGroup.Global> MovementTagFloating = FastTags<TagGroup.Global>.GetTag("floating");

	// Token: 0x0400180C RID: 6156
	public static readonly FastTags<TagGroup.Global> MovementTagSwimming = FastTags<TagGroup.Global>.GetTag("swimming");

	// Token: 0x0400180D RID: 6157
	public static readonly FastTags<TagGroup.Global> MovementTagSwimmingRun = FastTags<TagGroup.Global>.GetTag("swimmingRun");

	// Token: 0x0400180E RID: 6158
	public static readonly FastTags<TagGroup.Global> MovementTagJumping = FastTags<TagGroup.Global>.GetTag("jumping");

	// Token: 0x0400180F RID: 6159
	public static readonly FastTags<TagGroup.Global> MovementTagFalling = FastTags<TagGroup.Global>.GetTag("falling");

	// Token: 0x04001810 RID: 6160
	public static readonly FastTags<TagGroup.Global> MovementTagClimbing = FastTags<TagGroup.Global>.GetTag("climbing");

	// Token: 0x04001811 RID: 6161
	public static readonly FastTags<TagGroup.Global> MovementTagDriving = FastTags<TagGroup.Global>.GetTag("driving");

	// Token: 0x04001812 RID: 6162
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly float[] moveSpeedRandomness = new float[]
	{
		0.2f,
		1f,
		1.1f,
		1.2f,
		1.35f,
		1.5f
	};

	// Token: 0x04001813 RID: 6163
	public const float CLIMB_LADDER_SPEED = 1234f;

	// Token: 0x04001814 RID: 6164
	public static ulong HitDelay = 11000UL;

	// Token: 0x04001815 RID: 6165
	public static float HitSoundDistance = 10f;

	// Token: 0x04001816 RID: 6166
	public MinEventParams MinEventContext = new MinEventParams();

	// Token: 0x04001817 RID: 6167
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int equippingCount;

	// Token: 0x04001818 RID: 6168
	public bool IsSleeper;

	// Token: 0x04001819 RID: 6169
	public bool IsSleeping;

	// Token: 0x0400181A RID: 6170
	public bool IsSleeperPassive;

	// Token: 0x0400181B RID: 6171
	public bool SleeperSupressLivingSounds;

	// Token: 0x0400181C RID: 6172
	public Vector3 SleeperSpawnPosition;

	// Token: 0x0400181D RID: 6173
	public Vector3 SleeperSpawnLookDir;

	// Token: 0x0400181E RID: 6174
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float accumulatedDamageResisted;

	// Token: 0x0400181F RID: 6175
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int pendingSleepTrigger = -1;

	// Token: 0x04001820 RID: 6176
	public int lastSleeperPose;

	// Token: 0x04001821 RID: 6177
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 sleeperLookDir;

	// Token: 0x04001822 RID: 6178
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float sleeperSightRange;

	// Token: 0x04001823 RID: 6179
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float sleeperViewAngle;

	// Token: 0x04001824 RID: 6180
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightLightThreshold;

	// Token: 0x04001825 RID: 6181
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightWakeThresholdAtRange;

	// Token: 0x04001826 RID: 6182
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightGroanThresholdAtRange;

	// Token: 0x04001827 RID: 6183
	public float sleeperNoiseToSense;

	// Token: 0x04001828 RID: 6184
	public float sleeperNoiseToWake;

	// Token: 0x04001829 RID: 6185
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isSnore;

	// Token: 0x0400182A RID: 6186
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGroan;

	// Token: 0x0400182B RID: 6187
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGroanSilent;

	// Token: 0x0400182C RID: 6188
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float sleeperNoiseToSenseSoundChance;

	// Token: 0x0400182D RID: 6189
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int snoreGroanCD;

	// Token: 0x0400182E RID: 6190
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int kSnoreGroanMinCD = 20;

	// Token: 0x0400182F RID: 6191
	public EntityPlayer noisePlayer;

	// Token: 0x04001830 RID: 6192
	public float noisePlayerDistance;

	// Token: 0x04001831 RID: 6193
	public float noisePlayerVolume;

	// Token: 0x04001832 RID: 6194
	public EntityPlayer attractPlayer;

	// Token: 0x04001833 RID: 6195
	public float attractPlayerDistance;

	// Token: 0x04001834 RID: 6196
	public int attractPlayerTimeoutTicks;

	// Token: 0x04001835 RID: 6197
	public EntityPlayer smellPlayer;

	// Token: 0x04001836 RID: 6198
	public float smellPlayerDistance;

	// Token: 0x04001837 RID: 6199
	public int smellPlayerTimeoutTicks;

	// Token: 0x04001838 RID: 6200
	public EntityItem pendingDistraction;

	// Token: 0x04001839 RID: 6201
	public float pendingDistractionDistanceSq;

	// Token: 0x0400183A RID: 6202
	public EntityItem distraction;

	// Token: 0x0400183B RID: 6203
	public float distractionResistance;

	// Token: 0x0400183C RID: 6204
	public float distractionResistanceWithTarget;

	// Token: 0x0400183D RID: 6205
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimGravityPer = 0.025f;

	// Token: 0x0400183E RID: 6206
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragY = 0.91f;

	// Token: 0x0400183F RID: 6207
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDrag = 0.91f;

	// Token: 0x04001840 RID: 6208
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAnimDelay = 6f;

	// Token: 0x04001841 RID: 6209
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpTicks;

	// Token: 0x04001842 RID: 6210
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive.JumpState jumpState;

	// Token: 0x04001843 RID: 6211
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpStateTicks;

	// Token: 0x04001844 RID: 6212
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpDistance;

	// Token: 0x04001845 RID: 6213
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpHeightDiff;

	// Token: 0x04001846 RID: 6214
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpSwimDurationTicks;

	// Token: 0x04001847 RID: 6215
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 jumpSwimMotion;

	// Token: 0x04001848 RID: 6216
	public float jumpDelay;

	// Token: 0x04001849 RID: 6217
	public float jumpMaxDistance;

	// Token: 0x0400184A RID: 6218
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool jumpIsMoving;

	// Token: 0x0400184B RID: 6219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float landWaterLevel;

	// Token: 0x0400184C RID: 6220
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksNoPlayerAdjacent;

	// Token: 0x0400184D RID: 6221
	public int hasBeenAttackedTime;

	// Token: 0x0400184E RID: 6222
	public float painHitsFelt;

	// Token: 0x0400184F RID: 6223
	public float painResistPercent;

	// Token: 0x04001850 RID: 6224
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int attackingTime;

	// Token: 0x04001851 RID: 6225
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive revengeEntity;

	// Token: 0x04001852 RID: 6226
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int revengeTimer;

	// Token: 0x04001853 RID: 6227
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool targetAlertChanged;

	// Token: 0x04001854 RID: 6228
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAliveTime;

	// Token: 0x04001855 RID: 6229
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool alertEnabled = true;

	// Token: 0x04001856 RID: 6230
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int alertTicks;

	// Token: 0x04001857 RID: 6231
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string notAlertedId = "_notAlerted";

	// Token: 0x04001858 RID: 6232
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int notAlertDelayTicks;

	// Token: 0x04001859 RID: 6233
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAlert;

	// Token: 0x0400185A RID: 6234
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 investigatePos;

	// Token: 0x0400185B RID: 6235
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int investigatePositionTicks;

	// Token: 0x0400185C RID: 6236
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInvestigateAlert;

	// Token: 0x0400185D RID: 6237
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool hasAI;

	// Token: 0x0400185E RID: 6238
	public EAIManager aiManager;

	// Token: 0x0400185F RID: 6239
	public List<string> AIPackages;

	// Token: 0x04001860 RID: 6240
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Context utilityAIContext;

	// Token: 0x04001861 RID: 6241
	public EntityPlayer aiClosestPlayer;

	// Token: 0x04001862 RID: 6242
	public float aiClosestPlayerDistSq;

	// Token: 0x04001863 RID: 6243
	public float aiActiveScale;

	// Token: 0x04001864 RID: 6244
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float aiActiveDelay;

	// Token: 0x04001865 RID: 6245
	public bool IsBloodMoon;

	// Token: 0x04001866 RID: 6246
	public bool IsFeral;

	// Token: 0x04001867 RID: 6247
	public bool IsBreakingDoors;

	// Token: 0x04001868 RID: 6248
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_isBreakingBlocks;

	// Token: 0x04001869 RID: 6249
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_isEating;

	// Token: 0x0400186A RID: 6250
	public Vector3 ChaseReturnLocation;

	// Token: 0x0400186B RID: 6251
	public bool IsScoutZombie;

	// Token: 0x0400186C RID: 6252
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityLookHelper lookHelper;

	// Token: 0x0400186D RID: 6253
	public EntityMoveHelper moveHelper;

	// Token: 0x0400186E RID: 6254
	public PathNavigate navigator;

	// Token: 0x0400186F RID: 6255
	public bool bCanClimbLadders;

	// Token: 0x04001870 RID: 6256
	public bool bCanClimbVertical;

	// Token: 0x04001871 RID: 6257
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive damagedTarget;

	// Token: 0x04001872 RID: 6258
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive attackTarget;

	// Token: 0x04001873 RID: 6259
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTargetTime;

	// Token: 0x04001874 RID: 6260
	public EntityAlive attackTargetClient;

	// Token: 0x04001875 RID: 6261
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive attackTargetLast;

	// Token: 0x04001876 RID: 6262
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntitySeeCache seeCache;

	// Token: 0x04001877 RID: 6263
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ChunkCoordinates homePosition;

	// Token: 0x04001878 RID: 6264
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int maximumHomeDistance;

	// Token: 0x04001879 RID: 6265
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float jumpMovementFactor = 0.02f;

	// Token: 0x0400187A RID: 6266
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float landMovementFactor = 0.1f;

	// Token: 0x0400187B RID: 6267
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpMotionYValue = 0.419f;

	// Token: 0x0400187C RID: 6268
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundDistanceRemaining;

	// Token: 0x0400187D RID: 6269
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundRotYRemaining;

	// Token: 0x0400187E RID: 6270
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextSwimDistance;

	// Token: 0x0400187F RID: 6271
	public Inventory inventory;

	// Token: 0x04001880 RID: 6272
	public Inventory saveInventory;

	// Token: 0x04001881 RID: 6273
	public Equipment equipment;

	// Token: 0x04001882 RID: 6274
	public ChallengeJournal challengeJournal;

	// Token: 0x04001883 RID: 6275
	public int ExperienceValue;

	// Token: 0x04001884 RID: 6276
	public int deathUpdateTime;

	// Token: 0x04001885 RID: 6277
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive entityThatKilledMe;

	// Token: 0x04001886 RID: 6278
	public bool bPlayerStatsChanged;

	// Token: 0x04001887 RID: 6279
	public bool bEntityAliveFlagsChanged;

	// Token: 0x04001888 RID: 6280
	public bool bPlayerTwitchChanged;

	// Token: 0x04001889 RID: 6281
	public bool bPlayerEquipmentChanged;

	// Token: 0x0400188A RID: 6282
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<EnumDamageSource, ulong> damageSourceTimeouts = new EnumDictionary<EnumDamageSource, ulong>();

	// Token: 0x0400188B RID: 6283
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int traderTeleportStreak = 1;

	// Token: 0x0400188C RID: 6284
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJetpackWearing;

	// Token: 0x0400188D RID: 6285
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJetpackActive;

	// Token: 0x0400188E RID: 6286
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bParachuteWearing;

	// Token: 0x0400188F RID: 6287
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAimingGun;

	// Token: 0x04001890 RID: 6288
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bMovementRunning;

	// Token: 0x04001891 RID: 6289
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bCrouching;

	// Token: 0x04001892 RID: 6290
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJumping;

	// Token: 0x04001893 RID: 6291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bClimbing;

	// Token: 0x04001894 RID: 6292
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int died;

	// Token: 0x04001895 RID: 6293
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int score;

	// Token: 0x04001896 RID: 6294
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int killedZombies;

	// Token: 0x04001897 RID: 6295
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int killedPlayers;

	// Token: 0x04001898 RID: 6296
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int teamNumber;

	// Token: 0x04001899 RID: 6297
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string entityName = string.Empty;

	// Token: 0x0400189A RID: 6298
	public string DebugNameInfo = string.Empty;

	// Token: 0x0400189B RID: 6299
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int damageLocationBits;

	// Token: 0x0400189C RID: 6300
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bSpawned;

	// Token: 0x0400189D RID: 6301
	public bool bReplicatedAlertFlag;

	// Token: 0x0400189E RID: 6302
	public int vehiclePoseMode = -1;

	// Token: 0x0400189F RID: 6303
	public byte factionId;

	// Token: 0x040018A0 RID: 6304
	public byte factionRank;

	// Token: 0x040018A1 RID: 6305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToCheckSeenByPlayer;

	// Token: 0x040018A2 RID: 6306
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasSeenByPlayer;

	// Token: 0x040018A3 RID: 6307
	public DamageResponse RecordedDamage;

	// Token: 0x040018A4 RID: 6308
	public float moveSpeed;

	// Token: 0x040018A5 RID: 6309
	public float moveSpeedNight;

	// Token: 0x040018A6 RID: 6310
	public float moveSpeedAggro;

	// Token: 0x040018A7 RID: 6311
	public float moveSpeedAggroMax;

	// Token: 0x040018A8 RID: 6312
	public float moveSpeedPanic;

	// Token: 0x040018A9 RID: 6313
	public float moveSpeedPanicMax;

	// Token: 0x040018AA RID: 6314
	public float swimSpeed;

	// Token: 0x040018AB RID: 6315
	public Vector2 swimStrokeRate;

	// Token: 0x040018AC RID: 6316
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemValue handItem;

	// Token: 0x040018AD RID: 6317
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSpawn;

	// Token: 0x040018AE RID: 6318
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSleeperGroan;

	// Token: 0x040018AF RID: 6319
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSleeperSnore;

	// Token: 0x040018B0 RID: 6320
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundDeath;

	// Token: 0x040018B1 RID: 6321
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundAlert;

	// Token: 0x040018B2 RID: 6322
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundAttack;

	// Token: 0x040018B3 RID: 6323
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundLiving;

	// Token: 0x040018B4 RID: 6324
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundRandom;

	// Token: 0x040018B5 RID: 6325
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSense;

	// Token: 0x040018B6 RID: 6326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundGiveUp;

	// Token: 0x040018B7 RID: 6327
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundStepType;

	// Token: 0x040018B8 RID: 6328
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundStamina;

	// Token: 0x040018B9 RID: 6329
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundJump;

	// Token: 0x040018BA RID: 6330
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundLand;

	// Token: 0x040018BB RID: 6331
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundLandThump;

	// Token: 0x040018BC RID: 6332
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundHurt;

	// Token: 0x040018BD RID: 6333
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundDistressed;

	// Token: 0x040018BE RID: 6334
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundHurtSmall;

	// Token: 0x040018BF RID: 6335
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundDrownPain;

	// Token: 0x040018C0 RID: 6336
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string soundDrownDeath;

	// Token: 0x040018C1 RID: 6337
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string soundWaterSurface;

	// Token: 0x040018C2 RID: 6338
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundDelayTicks;

	// Token: 0x040018C3 RID: 6339
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundLivingID = -1;

	// Token: 0x040018C4 RID: 6340
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSoundRandomMaxDist = 20f;

	// Token: 0x040018C5 RID: 6341
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundAlertTicks;

	// Token: 0x040018C6 RID: 6342
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundRandomTicks;

	// Token: 0x040018C7 RID: 6343
	public int classMaxHealth;

	// Token: 0x040018C8 RID: 6344
	public int classMaxStamina;

	// Token: 0x040018C9 RID: 6345
	public int classMaxFood;

	// Token: 0x040018CA RID: 6346
	public int classMaxWater;

	// Token: 0x040018CB RID: 6347
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float weight;

	// Token: 0x040018CC RID: 6348
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float pushFactor;

	// Token: 0x040018CD RID: 6349
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxViewAngle;

	// Token: 0x040018CE RID: 6350
	public float sightRangeBase;

	// Token: 0x040018CF RID: 6351
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float sightRange;

	// Token: 0x040018D0 RID: 6352
	public float senseScale;

	// Token: 0x040018D1 RID: 6353
	public int timeStayAfterDeath;

	// Token: 0x040018D2 RID: 6354
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue corpseBlockValue;

	// Token: 0x040018D3 RID: 6355
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float corpseBlockChance;

	// Token: 0x040018D4 RID: 6356
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTimeoutDay;

	// Token: 0x040018D5 RID: 6357
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTimeoutNight;

	// Token: 0x040018D6 RID: 6358
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string particleOnDeath;

	// Token: 0x040018D7 RID: 6359
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string particleOnDestroy;

	// Token: 0x040018D8 RID: 6360
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityBedrollPositionList spawnPoints;

	// Token: 0x040018D9 RID: 6361
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<Vector3i> droppedBackpackPositions;

	// Token: 0x040018DA RID: 6362
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float speedModifier = 1f;

	// Token: 0x040018DB RID: 6363
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 accumulatedRootMotion;

	// Token: 0x040018DC RID: 6364
	public Vector3 moveDirection;

	// Token: 0x040018DD RID: 6365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMoveDirAbsolute;

	// Token: 0x040018DE RID: 6366
	public Vector3 lookAtPosition;

	// Token: 0x040018DF RID: 6367
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i blockPosStandingOn;

	// Token: 0x040018E0 RID: 6368
	public BlockValue blockValueStandingOn;

	// Token: 0x040018E1 RID: 6369
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool blockStandingOnChanged;

	// Token: 0x040018E2 RID: 6370
	public BiomeDefinition biomeStandingOn;

	// Token: 0x040018E3 RID: 6371
	public bool IsMale;

	// Token: 0x040018E4 RID: 6372
	public int crouchType;

	// Token: 0x040018E5 RID: 6373
	public float crouchBendPer;

	// Token: 0x040018E6 RID: 6374
	public float crouchBendPerTarget;

	// Token: 0x040018E7 RID: 6375
	public const int cWalkTypeSwim = -1;

	// Token: 0x040018E8 RID: 6376
	public const int cWalkTypeFat = 1;

	// Token: 0x040018E9 RID: 6377
	public const int cWalkTypeCripple = 5;

	// Token: 0x040018EA RID: 6378
	public const int cWalkTypeCrouch = 8;

	// Token: 0x040018EB RID: 6379
	public const int cWalkTypeBandit = 15;

	// Token: 0x040018EC RID: 6380
	public const int cWalkTypeCrawlFirst = 20;

	// Token: 0x040018ED RID: 6381
	public const int cWalkTypeCrawler = 21;

	// Token: 0x040018EE RID: 6382
	public const int cWalkTypeSpider = 22;

	// Token: 0x040018EF RID: 6383
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int walkType;

	// Token: 0x040018F0 RID: 6384
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int walkTypeBeforeCrouch;

	// Token: 0x040018F1 RID: 6385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string rightHandTransformName;

	// Token: 0x040018F2 RID: 6386
	public int pingToServer;

	// Token: 0x040018F3 RID: 6387
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<ItemStack> itemsOnEnterGame = new List<ItemStack>();

	// Token: 0x040018F4 RID: 6388
	public Utils.EnumHitDirection lastHitDirection = Utils.EnumHitDirection.None;

	// Token: 0x040018F5 RID: 6389
	public Vector3 lastHitImpactDir = Vector3.zero;

	// Token: 0x040018F6 RID: 6390
	public Vector3 lastHitEntityFwd = Vector3.zero;

	// Token: 0x040018F7 RID: 6391
	public bool lastHitRanged;

	// Token: 0x040018F8 RID: 6392
	public float lastHitForce;

	// Token: 0x040018F9 RID: 6393
	public DamageResponse lastDamageResponse;

	// Token: 0x040018FA RID: 6394
	public bool canDisintegrate;

	// Token: 0x040018FB RID: 6395
	public bool isDisintegrated;

	// Token: 0x040018FC RID: 6396
	public float CreationTimeSinceLevelLoad;

	// Token: 0x040018FD RID: 6397
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityStats entityStats;

	// Token: 0x040018FE RID: 6398
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityStats startOfFrameStats;

	// Token: 0x040018FF RID: 6399
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float proneRefillRate;

	// Token: 0x04001900 RID: 6400
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float kneelRefillRate;

	// Token: 0x04001901 RID: 6401
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float proneRefillCounter;

	// Token: 0x04001902 RID: 6402
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float kneelRefillCounter;

	// Token: 0x04001903 RID: 6403
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int deathHealth;

	// Token: 0x04001904 RID: 6404
	public BodyDamage bodyDamage;

	// Token: 0x04001905 RID: 6405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool stompsSpikes;

	// Token: 0x04001906 RID: 6406
	public float OverrideSize = 1f;

	// Token: 0x04001907 RID: 6407
	public float OverrideHeadSize = 1f;

	// Token: 0x04001908 RID: 6408
	public float OverrideHeadDismemberScaleTime = 1.5f;

	// Token: 0x04001909 RID: 6409
	public float OverridePitch;

	// Token: 0x0400190A RID: 6410
	public static EntityAlive.HeadShotOnlyModes HeadshotMode = EntityAlive.HeadShotOnlyModes.None;

	// Token: 0x0400190B RID: 6411
	public static EntityAlive.CelebrateModes CelebrateMode = EntityAlive.CelebrateModes.Disabled;

	// Token: 0x0400190C RID: 6412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDancing;

	// Token: 0x0400190D RID: 6413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeTraderStationChecked;

	// Token: 0x0400190E RID: 6414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lerpForwardSpeed;

	// Token: 0x0400190F RID: 6415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardTarget;

	// Token: 0x04001910 RID: 6416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardTargetStep = 1f;

	// Token: 0x04001911 RID: 6417
	public EntityBuffs Buffs;

	// Token: 0x04001912 RID: 6418
	public Progression Progression;

	// Token: 0x04001913 RID: 6419
	public FastTags<TagGroup.Global> CurrentStanceTag = EntityAlive.StanceTagStanding;

	// Token: 0x04001914 RID: 6420
	public FastTags<TagGroup.Global> CurrentMovementTag = FastTags<TagGroup.Global>.none;

	// Token: 0x04001915 RID: 6421
	public float renderFadeMax = 1f;

	// Token: 0x04001916 RID: 6422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float renderFade;

	// Token: 0x04001917 RID: 6423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float renderFadeTarget;

	// Token: 0x04001918 RID: 6424
	public float StressAmount;

	// Token: 0x04001919 RID: 6425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EntityAlive.FallBehavior> fallBehaviors = new List<EntityAlive.FallBehavior>();

	// Token: 0x0400191A RID: 6426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool disableFallBehaviorUntilOnGround;

	// Token: 0x0400191B RID: 6427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EntityAlive.DestroyBlockBehavior> _destroyBlockBehaviors = new List<EntityAlive.DestroyBlockBehavior>();

	// Token: 0x0400191C RID: 6428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicRagdollFlags _dynamicRagdoll;

	// Token: 0x0400191D RID: 6429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _dynamicRagdollStunTime;

	// Token: 0x0400191E RID: 6430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 _dynamicRagdollRootMotion;

	// Token: 0x0400191F RID: 6431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<Vector3> _ragdollPositionsPrev = new List<Vector3>();

	// Token: 0x04001920 RID: 6432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<Vector3> _ragdollPositionsCur = new List<Vector3>();

	// Token: 0x04001921 RID: 6433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFirstTimeEquipmentReassigned = true;

	// Token: 0x04001922 RID: 6434
	public bool CrouchingLocked;

	// Token: 0x04001923 RID: 6435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EModelBase.HeadStates currentHeadState;

	// Token: 0x04001924 RID: 6436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<EntityAlive.WeightBehavior> weightBehaviorTemp = new List<EntityAlive.WeightBehavior>();

	// Token: 0x04001925 RID: 6437
	public static bool ShowDebugDisplayHit = false;

	// Token: 0x04001926 RID: 6438
	public static float DebugDisplayHitSize = 0.005f;

	// Token: 0x04001927 RID: 6439
	public static float DebugDisplayHitTime = 10f;

	// Token: 0x04001928 RID: 6440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static FastTags<TagGroup.Global> noheadTag = FastTags<TagGroup.Global>.Parse("noHead");

	// Token: 0x04001929 RID: 6441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bPlayHurtSound;

	// Token: 0x0400192A RID: 6442
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bBeenWounded;

	// Token: 0x0400192B RID: 6443
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int woundedStrength;

	// Token: 0x0400192C RID: 6444
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public DamageSource woundedDamageSource;

	// Token: 0x0400192D RID: 6445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int despawnDelayCounter;

	// Token: 0x0400192E RID: 6446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDespawnWhenPlayerFar;

	// Token: 0x0400192F RID: 6447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_addedToWorld;

	// Token: 0x04001930 RID: 6448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int saveHoldingItemIdxBeforeAttach;

	// Token: 0x04001931 RID: 6449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float impactSoundTime;

	// Token: 0x04001932 RID: 6450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Transform, EntityAlive.ImpactData> impacts = new Dictionary<Transform, EntityAlive.ImpactData>();

	// Token: 0x04001933 RID: 6451
	public const string cParticlePrefix = "Ptl_";

	// Token: 0x04001934 RID: 6452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, Transform> particles = new Dictionary<string, Transform>();

	// Token: 0x04001935 RID: 6453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, Transform> parts = new Dictionary<string, Transform>();

	// Token: 0x04001936 RID: 6454
	public List<OwnedEntityData> ownedEntities = new List<OwnedEntityData>();

	// Token: 0x04001937 RID: 6455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<EntityAlive.NetworkStatChange> networkStatsUpdateQueue = new List<EntityAlive.NetworkStatChange>();

	// Token: 0x02000474 RID: 1140
	public enum JumpState
	{
		// Token: 0x04001939 RID: 6457
		Off,
		// Token: 0x0400193A RID: 6458
		Climb,
		// Token: 0x0400193B RID: 6459
		Leap,
		// Token: 0x0400193C RID: 6460
		Air,
		// Token: 0x0400193D RID: 6461
		Land,
		// Token: 0x0400193E RID: 6462
		SwimStart,
		// Token: 0x0400193F RID: 6463
		Swim
	}

	// Token: 0x02000475 RID: 1141
	public enum HeadShotOnlyModes
	{
		// Token: 0x04001941 RID: 6465
		None,
		// Token: 0x04001942 RID: 6466
		HeadshotOnly,
		// Token: 0x04001943 RID: 6467
		HeadshotFinisher
	}

	// Token: 0x02000476 RID: 1142
	public enum CelebrateModes
	{
		// Token: 0x04001945 RID: 6469
		Disabled,
		// Token: 0x04001946 RID: 6470
		Enabled,
		// Token: 0x04001947 RID: 6471
		HeadshotOnly
	}

	// Token: 0x02000477 RID: 1143
	[PublicizedFrom(EAccessModifier.Protected)]
	public class FallBehavior
	{
		// Token: 0x060023D2 RID: 9170 RVA: 0x000D9719 File Offset: 0x000D7919
		public FallBehavior(string name, EntityAlive.FallBehavior.Op type, FloatRange height, float weight, FloatRange ragePer, FloatRange rageTime, IntRange difficulty)
		{
			this.Name = name;
			this.ResponseOp = type;
			this.Height = height;
			this.Weight = weight;
			this.RagePer = ragePer;
			this.RageTime = rageTime;
			this.Difficulty = difficulty;
		}

		// Token: 0x04001948 RID: 6472
		public string Name;

		// Token: 0x04001949 RID: 6473
		public readonly EntityAlive.FallBehavior.Op ResponseOp;

		// Token: 0x0400194A RID: 6474
		public readonly FloatRange Height;

		// Token: 0x0400194B RID: 6475
		public readonly float Weight;

		// Token: 0x0400194C RID: 6476
		public readonly FloatRange RagePer;

		// Token: 0x0400194D RID: 6477
		public readonly FloatRange RageTime;

		// Token: 0x0400194E RID: 6478
		public readonly IntRange Difficulty;

		// Token: 0x02000478 RID: 1144
		public enum Op
		{
			// Token: 0x04001950 RID: 6480
			None,
			// Token: 0x04001951 RID: 6481
			Land,
			// Token: 0x04001952 RID: 6482
			LandLow,
			// Token: 0x04001953 RID: 6483
			LandHard,
			// Token: 0x04001954 RID: 6484
			Stumble,
			// Token: 0x04001955 RID: 6485
			Ragdoll
		}
	}

	// Token: 0x02000479 RID: 1145
	[PublicizedFrom(EAccessModifier.Protected)]
	public class DestroyBlockBehavior
	{
		// Token: 0x060023D3 RID: 9171 RVA: 0x000D9758 File Offset: 0x000D7958
		public DestroyBlockBehavior(string name, EntityAlive.DestroyBlockBehavior.Op type, float weight, FloatRange ragePer, FloatRange rageTime, IntRange difficulty)
		{
			this.Name = name;
			this.ResponseOp = type;
			this.Weight = weight;
			this.RagePer = ragePer;
			this.RageTime = rageTime;
			this.Difficulty = difficulty;
		}

		// Token: 0x04001956 RID: 6486
		public string Name;

		// Token: 0x04001957 RID: 6487
		public readonly EntityAlive.DestroyBlockBehavior.Op ResponseOp;

		// Token: 0x04001958 RID: 6488
		public readonly float Weight;

		// Token: 0x04001959 RID: 6489
		public readonly FloatRange RagePer;

		// Token: 0x0400195A RID: 6490
		public readonly FloatRange RageTime;

		// Token: 0x0400195B RID: 6491
		public readonly IntRange Difficulty = new IntRange(int.MinValue, int.MaxValue);

		// Token: 0x0200047A RID: 1146
		public enum Op
		{
			// Token: 0x0400195D RID: 6493
			None,
			// Token: 0x0400195E RID: 6494
			Ragdoll,
			// Token: 0x0400195F RID: 6495
			Stumble
		}
	}

	// Token: 0x0200047B RID: 1147
	public enum EnumApproachState
	{
		// Token: 0x04001961 RID: 6497
		Ok,
		// Token: 0x04001962 RID: 6498
		TooFarAway,
		// Token: 0x04001963 RID: 6499
		BlockedByWorldMesh,
		// Token: 0x04001964 RID: 6500
		BlockedByEntity,
		// Token: 0x04001965 RID: 6501
		Unknown
	}

	// Token: 0x0200047C RID: 1148
	[PublicizedFrom(EAccessModifier.Private)]
	public struct WeightBehavior
	{
		// Token: 0x04001966 RID: 6502
		public float weight;

		// Token: 0x04001967 RID: 6503
		public int index;
	}

	// Token: 0x0200047D RID: 1149
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ImpactData
	{
		// Token: 0x04001968 RID: 6504
		public int count;
	}

	// Token: 0x0200047E RID: 1150
	[PublicizedFrom(EAccessModifier.Private)]
	public class NetworkStatChange
	{
		// Token: 0x04001969 RID: 6505
		public EntityAlive.EntityNetworkStats m_NetworkStats;

		// Token: 0x0400196A RID: 6506
		public EntityAlive.EntityNetworkHoldingData m_HoldingData;
	}

	// Token: 0x0200047F RID: 1151
	public class EntityNetworkHoldingData
	{
		// Token: 0x0400196B RID: 6507
		public ItemStack m_HoldingItemStack;

		// Token: 0x0400196C RID: 6508
		public byte m_HoldingItemIndex;
	}

	// Token: 0x02000480 RID: 1152
	public class EntityNetworkStats
	{
		// Token: 0x060023D7 RID: 9175 RVA: 0x000D97B0 File Offset: 0x000D79B0
		public void FillFromEntity(EntityAlive _entity)
		{
			this.killed = _entity.Died;
			this.holdingItemStack = _entity.inventory.holdingItemStack;
			this.holdingItemIndex = (byte)_entity.inventory.holdingItemIdx;
			this.deathHealth = _entity.DeathHealth;
			this.teamNumber = _entity.TeamNumber;
			if (GameManager.Instance.World.GetPrimaryPlayer() == _entity)
			{
				_entity.inventory.TurnOffLightFlares();
			}
			if (_entity.Progression != null && _entity.Progression.bProgressionStatsChanged)
			{
				_entity.Progression.bProgressionStatsChanged = false;
				this.hasProgression = true;
				this.progressionsData = _entity.Progression.ToBytes(false);
			}
			this.attachedToEntityId = ((_entity.AttachedToEntity != null) ? _entity.AttachedToEntity.entityId : -1);
			this.entityName = _entity.EntityName;
			EntityPlayer entityPlayer = _entity as EntityPlayer;
			if (entityPlayer != null)
			{
				this.isPlayer = true;
				this.killedPlayers = _entity.KilledPlayers;
				this.killedZombies = _entity.KilledZombies;
				this.experience = entityPlayer.Progression.ExpToNextLevel;
				this.level = entityPlayer.Progression.Level;
				this.totalItemsCrafted = entityPlayer.totalItemsCrafted;
				this.distanceWalked = entityPlayer.distanceWalked;
				this.longestLife = entityPlayer.longestLife;
				this.currentLife = entityPlayer.currentLife;
				this.totalTimePlayed = entityPlayer.totalTimePlayed;
				this.vehiclePose = entityPlayer.GetVehicleAnimation();
				this.isSpectator = entityPlayer.IsSpectator;
				return;
			}
			this.isPlayer = false;
			this.experience = 0;
			this.level = 1;
			this.distanceWalked = 0f;
			this.totalItemsCrafted = 0U;
			this.longestLife = 0f;
			this.currentLife = 0f;
			this.totalTimePlayed = 0f;
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x000D9980 File Offset: 0x000D7B80
		public void ToEntity(EntityAlive _entity)
		{
			_entity.Died = this.killed;
			_entity.DeathHealth = this.deathHealth;
			_entity.TeamNumber = this.teamNumber;
			_entity.inventory.bResetLightLevelWhenChanged = true;
			if (!_entity.inventory.GetItem((int)this.holdingItemIndex).Equals(this.holdingItemStack))
			{
				_entity.inventory.SetItem((int)this.holdingItemIndex, this.holdingItemStack);
				_entity.inventory.ForceHoldingItemUpdate();
			}
			if (_entity.inventory.holdingItemIdx != (int)this.holdingItemIndex)
			{
				_entity.inventory.SetHoldingItemIdxNoHolsterTime((int)this.holdingItemIndex);
			}
			if (this.hasProgression)
			{
				_entity.Progression = Progression.FromBytes(this.progressionsData, _entity);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _entity.Progression != null)
				{
					_entity.Progression.bProgressionStatsChanged = true;
				}
			}
			_entity.SetEntityName(this.entityName);
			EntityPlayer entityPlayer = _entity as EntityPlayer;
			if (entityPlayer != null && this.isPlayer)
			{
				if (_entity.NavObject != null)
				{
					_entity.NavObject.name = this.entityName;
				}
				_entity.KilledZombies = this.killedZombies;
				_entity.KilledPlayers = this.killedPlayers;
				entityPlayer.Progression.ExpToNextLevel = this.experience;
				entityPlayer.Progression.Level = this.level;
				entityPlayer.totalItemsCrafted = this.totalItemsCrafted;
				entityPlayer.distanceWalked = this.distanceWalked;
				entityPlayer.longestLife = this.longestLife;
				entityPlayer.currentLife = this.currentLife;
				entityPlayer.totalTimePlayed = this.totalTimePlayed;
				entityPlayer.SetVehiclePoseMode(this.vehiclePose);
				entityPlayer.IsSpectator = this.isSpectator;
			}
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x000D9B2C File Offset: 0x000D7D2C
		public void read(PooledBinaryReader _reader)
		{
			this.killed = _reader.ReadInt32();
			this.holdingItemStack = new ItemStack();
			this.holdingItemStack.Read(_reader);
			this.holdingItemIndex = _reader.ReadByte();
			this.deathHealth = _reader.ReadInt32();
			this.teamNumber = (int)_reader.ReadByte();
			this.attachedToEntityId = _reader.ReadInt32();
			this.entityName = _reader.ReadString();
			this.isPlayer = _reader.ReadBoolean();
			if (this.isPlayer)
			{
				this.killedZombies = _reader.ReadInt32();
				this.killedPlayers = _reader.ReadInt32();
				this.experience = _reader.ReadInt32();
				this.level = _reader.ReadInt32();
				this.totalItemsCrafted = _reader.ReadUInt32();
				this.distanceWalked = _reader.ReadSingle();
				this.longestLife = _reader.ReadSingle();
				this.currentLife = _reader.ReadSingle();
				this.totalTimePlayed = _reader.ReadSingle();
				this.vehiclePose = _reader.ReadInt32();
				this.isSpectator = _reader.ReadBoolean();
			}
			this.hasProgression = _reader.ReadBoolean();
			if (this.hasProgression)
			{
				int num = (int)_reader.ReadInt16();
				this.progressionsData = new byte[num];
				_reader.Read(this.progressionsData, 0, num);
			}
		}

		// Token: 0x060023DA RID: 9178 RVA: 0x000D9C6C File Offset: 0x000D7E6C
		public void write(PooledBinaryWriter _writer)
		{
			_writer.Write(this.killed);
			this.holdingItemStack.Write(_writer);
			_writer.Write(this.holdingItemIndex);
			_writer.Write(this.deathHealth);
			_writer.Write((byte)this.teamNumber);
			_writer.Write(this.attachedToEntityId);
			_writer.Write(this.entityName);
			_writer.Write(this.isPlayer);
			if (this.isPlayer)
			{
				_writer.Write(this.killedZombies);
				_writer.Write(this.killedPlayers);
				_writer.Write(this.experience);
				_writer.Write(this.level);
				_writer.Write(this.totalItemsCrafted);
				_writer.Write(this.distanceWalked);
				_writer.Write(this.longestLife);
				_writer.Write(this.currentLife);
				_writer.Write(this.totalTimePlayed);
				_writer.Write(this.vehiclePose);
				_writer.Write(this.isSpectator);
			}
			_writer.Write(this.hasProgression);
			if (this.hasProgression)
			{
				_writer.Write((short)this.progressionsData.Length);
				_writer.Write(this.progressionsData, 0, this.progressionsData.Length);
			}
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x000D9DA1 File Offset: 0x000D7FA1
		public void SetName(string name)
		{
			this.entityName = name;
		}

		// Token: 0x0400196D RID: 6509
		[PublicizedFrom(EAccessModifier.Private)]
		public int experience;

		// Token: 0x0400196E RID: 6510
		[PublicizedFrom(EAccessModifier.Private)]
		public int level;

		// Token: 0x0400196F RID: 6511
		[PublicizedFrom(EAccessModifier.Private)]
		public int killed;

		// Token: 0x04001970 RID: 6512
		[PublicizedFrom(EAccessModifier.Private)]
		public int killedZombies;

		// Token: 0x04001971 RID: 6513
		[PublicizedFrom(EAccessModifier.Private)]
		public int killedPlayers;

		// Token: 0x04001972 RID: 6514
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemStack holdingItemStack;

		// Token: 0x04001973 RID: 6515
		[PublicizedFrom(EAccessModifier.Private)]
		public byte holdingItemIndex;

		// Token: 0x04001974 RID: 6516
		[PublicizedFrom(EAccessModifier.Private)]
		public int deathHealth;

		// Token: 0x04001975 RID: 6517
		[PublicizedFrom(EAccessModifier.Private)]
		public int teamNumber;

		// Token: 0x04001976 RID: 6518
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasProgression;

		// Token: 0x04001977 RID: 6519
		[PublicizedFrom(EAccessModifier.Private)]
		public byte[] progressionsData;

		// Token: 0x04001978 RID: 6520
		[PublicizedFrom(EAccessModifier.Private)]
		public int attachedToEntityId;

		// Token: 0x04001979 RID: 6521
		[PublicizedFrom(EAccessModifier.Private)]
		public string entityName;

		// Token: 0x0400197A RID: 6522
		[PublicizedFrom(EAccessModifier.Private)]
		public float distanceWalked;

		// Token: 0x0400197B RID: 6523
		[PublicizedFrom(EAccessModifier.Private)]
		public uint totalItemsCrafted;

		// Token: 0x0400197C RID: 6524
		[PublicizedFrom(EAccessModifier.Private)]
		public float longestLife;

		// Token: 0x0400197D RID: 6525
		[PublicizedFrom(EAccessModifier.Private)]
		public float currentLife;

		// Token: 0x0400197E RID: 6526
		[PublicizedFrom(EAccessModifier.Private)]
		public float totalTimePlayed;

		// Token: 0x0400197F RID: 6527
		[PublicizedFrom(EAccessModifier.Private)]
		public int vehiclePose;

		// Token: 0x04001980 RID: 6528
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isSpectator;

		// Token: 0x04001981 RID: 6529
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPlayer;
	}
}
