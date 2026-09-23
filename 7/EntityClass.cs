using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000490 RID: 1168
[Preserve]
public class EntityClass
{
	// Token: 0x06002435 RID: 9269 RVA: 0x000DB79C File Offset: 0x000D999C
	public static void Add(string _entityClassname, EntityClass _entityClass)
	{
		_entityClass.entityClassName = _entityClassname;
		EntityClass.list[_entityClassname.GetHashCode()] = _entityClass;
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000DB7B8 File Offset: 0x000D99B8
	public static EntityClass GetEntityClass(int entityClass)
	{
		EntityClass result;
		EntityClass.list.TryGetValue(entityClass, out result);
		return result;
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000DB7D4 File Offset: 0x000D99D4
	public static string GetEntityClassName(int entityClass)
	{
		EntityClass entityClass2;
		if (EntityClass.list.TryGetValue(entityClass, out entityClass2))
		{
			return entityClass2.entityClassName;
		}
		return "null";
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000DB7FC File Offset: 0x000D99FC
	public static int GetId(string _name)
	{
		foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
		{
			if (keyValuePair.Value.entityClassName == _name)
			{
				return keyValuePair.Key;
			}
		}
		return -1;
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000DB870 File Offset: 0x000D9A70
	public static int FromString(string _s)
	{
		return _s.GetHashCode();
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000DB8FC File Offset: 0x000D9AFC
	public EntityClass Init()
	{
		this.censorType = 1;
		string text = "";
		if (this.Properties.Contains(EntityClass.PropCensor))
		{
			text = this.Properties.GetString(EntityClass.PropCensor);
		}
		if (!string.IsNullOrEmpty(text) && text.Contains(","))
		{
			string[] array = text.Split(",", StringSplitOptions.None);
			if (array.Length > 1)
			{
				StringParsers.TryParseSInt32(array[0], out this.censorMode);
				StringParsers.TryParseSInt32(array[1], out this.censorType);
			}
		}
		else
		{
			this.Properties.ParseInt(EntityClass.PropCensor, ref this.censorMode);
		}
		if (!this.Properties.Values.TryGetValue(EntityClass.PropPrefab, out this.prefabPath) || this.prefabPath.Length == 0)
		{
			throw new Exception("Mandatory property 'prefab' missing in entity_class '" + this.entityClassName + "'");
		}
		string value;
		bool flag;
		if (this.Properties.Values.TryGetValue(EntityClass.PropPrefabCombined, out value) && bool.TryParse(value, out flag) && flag)
		{
			this.IsPrefabCombined = true;
		}
		else if (this.prefabPath[0] == '/')
		{
			this.prefabPath = this.prefabPath.Substring(1);
			this.IsPrefabCombined = true;
		}
		else if (DataLoader.IsInResources(this.prefabPath))
		{
			this.prefabPath = "Prefabs/prefabEntity" + this.prefabPath;
		}
		string text2;
		if (this.Properties.Values.TryGetValue(EntityClass.PropMesh, out text2) && text2.Length > 0)
		{
			if (this.censorMode != 0 && (this.censorType == 1 || this.censorType == 3) && GameManager.Instance && GameManager.Instance.IsGoreCensored())
			{
				text2 = text2.Replace(".", "_CGore.");
			}
			if (DataLoader.IsInResources(text2))
			{
				text2 = "Entities/" + text2;
			}
			this.meshPath = text2;
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropMeshFP))
		{
			string text3 = this.Properties.Values[EntityClass.PropMeshFP];
			if (DataLoader.IsInResources(text3))
			{
				text3 = "Entities/" + text3;
			}
			this.meshFP = DataLoader.LoadAsset<Transform>(text3, false);
			if (this.meshFP == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Could not load file '",
					text3,
					"' for entity_class '",
					this.entityClassName,
					"'"
				}));
			}
		}
		this.entityFlags = EntityFlags.None;
		EntityClass.ParseEntityFlags(this.Properties.GetString(EntityClass.PropEntityFlags), ref this.entityFlags);
		if (this.Properties.Values.ContainsKey(EntityClass.PropClass))
		{
			this.classname = Type.GetType(this.Properties.Values[EntityClass.PropClass]);
			if (this.classname == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Could not instantiate class",
					this.Properties.Values[EntityClass.PropClass],
					"' for entity_class '",
					this.entityClassName,
					"'"
				}));
			}
		}
		this.modelType = typeof(EModelCustom);
		string @string = this.Properties.GetString(EntityClass.PropModelType);
		if (@string.Length > 0)
		{
			this.modelType = ReflectionHelpers.GetTypeWithPrefix("EModel", @string);
			if (this.modelType == null)
			{
				throw new Exception("Model class '" + @string + "' not found!");
			}
		}
		string string2 = this.Properties.GetString(EntityClass.PropAltMats);
		if (string2.Length > 0)
		{
			this.AltMatNames = string2.Split(',', StringSplitOptions.None);
		}
		string string3 = this.Properties.GetString(EntityClass.PropSwapMats);
		if (string3.Length > 0)
		{
			this.MatSwap = string3.Split(",", StringSplitOptions.None);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropParticleOnSpawn))
		{
			this.particleOnSpawn.fileName = this.Properties.Values[EntityClass.PropParticleOnSpawn];
			this.particleOnSpawn.shapeMesh = this.Properties.Params1[EntityClass.PropParticleOnSpawn];
			DataLoader.PreloadBundle(this.particleOnSpawn.fileName);
		}
		this.RagdollOnDeathChance = 0.5f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropRagdollOnDeathChance))
		{
			this.RagdollOnDeathChance = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropRagdollOnDeathChance], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropHasRagdoll))
		{
			this.HasRagdoll = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropHasRagdoll], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropColliders))
		{
			this.CollidersRagdollAsset = this.Properties.Values[EntityClass.PropColliders];
			DataLoader.PreloadBundle(this.CollidersRagdollAsset);
		}
		this.Properties.ParseFloat(EntityClass.PropLookAtAngle, ref this.LookAtAngle);
		if (this.Properties.Values.ContainsKey(EntityClass.PropCrouchYOffsetFP))
		{
			this.crouchYOffsetFP = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropCrouchYOffsetFP], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropParent))
		{
			this.parentGameObjectName = this.Properties.Values[EntityClass.PropParent];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropSkinTexture))
		{
			this.skinTexture = this.Properties.Values[EntityClass.PropSkinTexture];
			DataLoader.PreloadBundle(this.skinTexture);
		}
		this.bIsEnemyEntity = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsEnemyEntity))
		{
			this.bIsEnemyEntity = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsEnemyEntity], 0, -1, true);
		}
		this.bIsAnimalEntity = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsAnimalEntity))
		{
			this.bIsAnimalEntity = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsAnimalEntity], 0, -1, true);
		}
		this.RootMotion = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropRootMotion))
		{
			this.RootMotion = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropRootMotion], 0, -1, true);
		}
		this.HasDeathAnim = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropHasDeathAnim))
		{
			this.HasDeathAnim = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropHasDeathAnim], 0, -1, true);
		}
		this.ExperienceValue = 100;
		if (this.Properties.Values.ContainsKey(EntityClass.PropExperienceGain))
		{
			this.ExperienceValue = (int)StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropExperienceGain], 0, -1, NumberStyles.Any);
		}
		string string4 = this.Properties.GetString(EntityClass.PropLootDropEntityClass);
		if (string4.Length > 0)
		{
			this.lootDrops = new List<EntityClass.LootDrop>();
			EntityClass.LootDrop lootDrop = default(EntityClass.LootDrop);
			if (!string4.Contains(","))
			{
				lootDrop.entityClass = EntityClass.FromString(string4);
				lootDrop.weight = 1f;
				this.lootDrops.Add(lootDrop);
			}
			else
			{
				string[] array2 = string4.Split(EntityClass.commaSeparator);
				int num = array2.Length / 2;
				for (int i = 0; i < num; i++)
				{
					int num2 = i * 2;
					lootDrop.entityClass = EntityClass.FromString(array2[num2].Trim());
					lootDrop.weight = float.Parse(array2[num2 + 1]);
					this.lootDrops.Add(lootDrop);
				}
				float num3 = 0f;
				for (int j = 0; j < num; j++)
				{
					num3 += this.lootDrops[j].weight;
				}
				for (int k = 0; k < num; k++)
				{
					lootDrop = this.lootDrops[k];
					lootDrop.weight /= num3;
					this.lootDrops[k] = lootDrop;
				}
			}
		}
		this.bIsMale = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsMale))
		{
			this.bIsMale = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsMale], 0, -1, true);
		}
		this.bIsChunkObserver = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsChunkObserver))
		{
			this.bIsChunkObserver = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsChunkObserver], 0, -1, true);
		}
		this.SightRange = Constants.cDefaultMonsterSeeDistance;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSightRange))
		{
			this.SightRange = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSightRange], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropSightLightThreshold))
		{
			this.sightLightThreshold = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropSightLightThreshold]);
		}
		else
		{
			this.sightLightThreshold = new Vector2(30f, 100f);
		}
		this.SleeperNoiseToSense = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperNoiseToSense, ref this.SleeperNoiseToSense);
		this.SleeperNoiseToSenseSoundChance = 1f;
		this.Properties.ParseFloat(EntityClass.PropSleeperNoiseToSenseSoundChance, ref this.SleeperNoiseToSenseSoundChance);
		this.SleeperNoiseToWake = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperNoiseToWake, ref this.SleeperNoiseToWake);
		this.SleeperSightToSenseMin = new Vector2(25f, 25f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToSenseMin, ref this.SleeperSightToSenseMin);
		this.SleeperSightToSenseMax = new Vector2(200f, 200f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToSenseMax, ref this.SleeperSightToSenseMax);
		this.SleeperSightToWakeMin = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToWakeMin, ref this.SleeperSightToWakeMin);
		this.SleeperSightToWakeMax = new Vector2(200f, 200f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToWakeMax, ref this.SleeperSightToWakeMax);
		this.MassKg = 10f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropMass))
		{
			this.MassKg = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropMass], 0, -1, NumberStyles.Any);
		}
		this.MassKg *= 0.454f;
		this.SizeScale = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSizeScale))
		{
			this.SizeScale = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSizeScale], 0, -1, NumberStyles.Any);
		}
		string string5 = this.Properties.GetString(EntityClass.PropPhysicsBody);
		if (string5.Length > 0)
		{
			this.PhysicsBody = PhysicsBodyLayout.Find(string5);
		}
		if (this.Properties.Values.ContainsKey("DeadBodyHitPoints"))
		{
			this.DeadBodyHitPoints = int.Parse(this.Properties.Values["DeadBodyHitPoints"]);
		}
		this.Properties.ParseFloat(EntityClass.PropLegCrippleScale, ref this.LegCrippleScale);
		this.Properties.ParseFloat(EntityClass.PropLegCrawlerThreshold, ref this.LegCrawlerThreshold);
		this.DismemberMultiplierHead = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierHead, ref this.DismemberMultiplierHead);
		this.DismemberMultiplierArms = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierArms, ref this.DismemberMultiplierArms);
		this.DismemberMultiplierLegs = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierLegs, ref this.DismemberMultiplierLegs);
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelDamageThreshold))
		{
			this.KnockdownKneelDamageThreshold = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropKnockdownKneelDamageThreshold], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelStunDuration))
		{
			this.KnockdownKneelStunDuration = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownKneelStunDuration]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneDamageThreshold))
		{
			this.KnockdownProneDamageThreshold = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropKnockdownProneDamageThreshold], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneStunDuration))
		{
			this.KnockdownProneStunDuration = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownProneStunDuration]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelRefillRate))
		{
			this.KnockdownKneelRefillRate = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownKneelRefillRate]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneRefillRate))
		{
			this.KnockdownProneRefillRate = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownProneRefillRate]);
		}
		this.LegsExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropLegsExplosionDamageMultiplier))
		{
			this.LegsExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropLegsExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.ArmsExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropArmsExplosionDamageMultiplier))
		{
			this.ArmsExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropArmsExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.HeadExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropHeadExplosionDamageMultiplier))
		{
			this.HeadExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropHeadExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.ChestExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropChestExplosionDamageMultiplier))
		{
			this.ChestExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropChestExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		Vector3 zero = Vector3.zero;
		this.Properties.ParseVec(EntityClass.PropPainResistPerHit, ref zero, 0f);
		this.PainResistPerHit = zero.x;
		this.PainResistPerHitLowHealth = zero.y;
		this.PainResistPerHitLowHealthPercent = zero.z;
		if (this.Properties.Values.ContainsKey(EntityClass.PropArchetype))
		{
			this.ArchetypeName = this.Properties.Values[EntityClass.PropArchetype];
		}
		this.SwimOffset = 0.9f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSwimOffset))
		{
			this.SwimOffset = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSwimOffset], 0, -1, NumberStyles.Any);
		}
		this.SearchRadius = 6f;
		this.Properties.ParseFloat(EntityClass.PropSearchRadius, ref this.SearchRadius);
		if (this.Properties.Values.ContainsKey(EntityClass.PropUMARace))
		{
			this.UMARace = this.Properties.Values[EntityClass.PropUMARace];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropUMAGeneratedModelName))
		{
			this.UMAGeneratedModelName = this.Properties.Values[EntityClass.PropUMAGeneratedModelName];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropModelTransformAdjust))
		{
			this.ModelTransformAdjust = StringParsers.ParseVector3(this.Properties.Values[EntityClass.PropModelTransformAdjust], 0, -1);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropAIPackages))
		{
			this.AIPackages = this.Properties.Values[EntityClass.PropAIPackages].Split(',', StringSplitOptions.None);
			for (int l = 0; l < this.AIPackages.Length; l++)
			{
				this.AIPackages[l] = this.AIPackages[l].Trim();
			}
			this.UseAIPackages = true;
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropBuffs))
		{
			string[] array3 = this.Properties.Values[EntityClass.PropBuffs].Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array3.Length != 0)
			{
				this.Buffs = new List<string>(array3);
			}
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropMaxTurnSpeed))
		{
			this.MaxTurnSpeed = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropMaxTurnSpeed], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropTags))
		{
			this.Tags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[EntityClass.PropTags]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropNavObject))
		{
			this.NavObject = this.Properties.Values[EntityClass.PropNavObject];
		}
		this.Properties.ParseVec(EntityClass.PropNavObjectHeadOffset, ref this.NavObjectHeadOffset);
		this.explosionData = new ExplosionData(this.Properties, this.Effects);
		bool flag2 = false;
		this.Properties.ParseBool(EntityClass.PropHideInSpawnMenu, ref flag2);
		if (flag2)
		{
			this.userSpawnType = EntityClass.UserSpawnType.Console;
		}
		this.Properties.ParseEnum<EntityClass.UserSpawnType>(EntityClass.PropUserSpawnType, ref this.userSpawnType);
		this.Properties.ParseBool(EntityClass.PropCanBigHead, ref this.CanBigHead);
		this.Properties.ParseInt(EntityClass.PropDanceType, ref this.DanceTypeID);
		this.Properties.ParseString(EntityClass.PropOnActivateEvent, ref this.onActivateEvent);
		this.Properties.ParseString(EntityClass.PropPreviousTierZombie, ref this.PreviousTierZombieName);
		this.Properties.ParseString(EntityClass.PropPickupItem, ref this.PickupItem);
		this.Properties.ParseString(EntityClass.PropPickupStressCvar, ref this.PickupStressCvar);
		this.Properties.ParseString(EntityClass.PropPickupStressBuff, ref this.PickupStressBuff);
		List<Dictionary<string, string>> list;
		if (this.Properties.Array.TryGetValue(EntityClass.PropTokenManager, out list))
		{
			this.TokenManagerConfig = new Dictionary<AITokenType, AITokenConfig>();
			for (int m = 0; m < list.Count; m++)
			{
				string value2;
				AITokenType key;
				if (list[m].TryGetValue("type", out value2) && Enum.TryParse<AITokenType>(value2, true, out key))
				{
					AITokenConfig value3 = default(AITokenConfig);
					string s;
					if (list[m].TryGetValue("max", out s))
					{
						int.TryParse(s, out value3.MaxClaims);
					}
					this.TokenManagerConfig[key] = value3;
				}
			}
		}
		this.CalculateEntityTier();
		return this;
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000DCC28 File Offset: 0x000DAE28
	public void CopyFrom(EntityClass _other, HashSet<string> _exclude = null)
	{
		foreach (KeyValuePair<string, string> keyValuePair in _other.Properties.Values)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair.Key))
			{
				this.Properties.Values[keyValuePair.Key] = _other.Properties.Values[keyValuePair.Key];
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair2 in _other.Properties.Params1)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair2.Key))
			{
				this.Properties.Params1[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair3 in _other.Properties.Params2)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair3.Key))
			{
				this.Properties.Params2[keyValuePair3.Key] = keyValuePair3.Value;
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair4 in _other.Properties.Data)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair4.Key))
			{
				this.Properties.Data[keyValuePair4.Key] = keyValuePair4.Value;
			}
		}
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair5 in _other.Properties.Classes)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair5.Key))
			{
				DynamicProperties dynamicProperties = new DynamicProperties();
				dynamicProperties.CopyFrom(keyValuePair5.Value, null);
				this.Properties.Classes[keyValuePair5.Key] = dynamicProperties;
			}
		}
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000DCE90 File Offset: 0x000DB090
	public static void ParseEntityFlags(string _names, ref EntityFlags optionalValue)
	{
		if (_names.Length > 0)
		{
			if (_names.IndexOf(',') >= 0)
			{
				string[] array = _names.Split(EntityClass.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					EntityFlags entityFlags;
					if (EnumUtils.TryParse<EntityFlags>(array[i], out entityFlags, true))
					{
						optionalValue |= entityFlags;
					}
				}
				return;
			}
			EntityFlags entityFlags2;
			if (EnumUtils.TryParse<EntityFlags>(_names, out entityFlags2, true))
			{
				optionalValue = entityFlags2;
			}
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000DCEEE File Offset: 0x000DB0EE
	public static void Cleanup()
	{
		EntityClass.list.Clear();
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x000DCEFC File Offset: 0x000DB0FC
	public void AddDroppedId(EnumDropEvent _eEvent, string _name, int _minCount, int _maxCount, float _prob, float _stickChance, string _toolCategory, string _tag)
	{
		List<Block.SItemDropProb> list = this.itemsToDrop.ContainsKey(_eEvent) ? this.itemsToDrop[_eEvent] : null;
		if (list == null)
		{
			list = new List<Block.SItemDropProb>();
			this.itemsToDrop[_eEvent] = list;
		}
		list.Add(new Block.SItemDropProb(_name, _minCount, _maxCount, _prob, 1f, _stickChance, _toolCategory, _tag));
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000DCF5C File Offset: 0x000DB15C
	public int LootDropPick(GameRandom _rand)
	{
		int index = 0;
		if (this.lootDrops.Count >= 2)
		{
			float num = 0f;
			float randomFloat = _rand.RandomFloat;
			for (int i = 0; i < this.lootDrops.Count; i++)
			{
				num += this.lootDrops[i].weight;
				if (randomFloat <= num)
				{
					index = i;
					break;
				}
			}
		}
		return this.lootDrops[index].entityClass;
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000DCFCC File Offset: 0x000DB1CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalculateEntityTier()
	{
		if (this.Tags.Test_AnySet(EntityClass.eliteTag))
		{
			this.EntityTier = EntityClass.EntityTierTypes.Elite;
			return;
		}
		if (this.Tags.Test_AnySet(EntityClass.radiatedTag))
		{
			this.EntityTier = EntityClass.EntityTierTypes.Radiated;
			return;
		}
		if (this.Tags.Test_AnySet(EntityClass.feralTag))
		{
			this.EntityTier = EntityClass.EntityTierTypes.Feral;
			return;
		}
		if (this.Tags.Test_AnySet(EntityClass.specialTag))
		{
			this.EntityTier = EntityClass.EntityTierTypes.Special;
			return;
		}
		if (this.Tags.Test_AnySet(EntityClass.strongTag))
		{
			this.EntityTier = EntityClass.EntityTierTypes.Strong;
			return;
		}
		this.EntityTier = EntityClass.EntityTierTypes.Normal;
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000DD064 File Offset: 0x000DB264
	public EntityClass GetPreviousTierEntity()
	{
		if (this.previousTierEntities == null && this.PreviousTierZombieName != "")
		{
			string[] array = this.PreviousTierZombieName.Split(',', StringSplitOptions.None);
			this.previousTierEntities = new EntityClass[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				this.previousTierEntities[i] = EntityClass.GetEntityClass(EntityClass.GetId(array[i]));
			}
		}
		if (this.previousTierEntities == null || this.previousTierEntities.Length == 0)
		{
			return null;
		}
		if (this.previousTierEntities.Length > 1)
		{
			return this.previousTierEntities[GameManager.Instance.World.GetGameRandom().RandomRange(this.previousTierEntities.Length)];
		}
		return this.previousTierEntities[0];
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x000DD118 File Offset: 0x000DB318
	public static EntityClass GetEntityClassWithinMaxTier(EntityClass ec, EntityClass.EntityTierTypes maxTier)
	{
		if (ec.EntityTier <= maxTier)
		{
			return ec;
		}
		for (EntityClass previousTierEntity = ec.GetPreviousTierEntity(); previousTierEntity != null; previousTierEntity = previousTierEntity.GetPreviousTierEntity())
		{
			if (previousTierEntity.EntityTier <= maxTier)
			{
				return previousTierEntity;
			}
		}
		Log.Warning(string.Format("EntityFactory CreateEntity: No entity within max tier ({0}) found for {1}", maxTier, ec.entityClassName));
		return null;
	}

	// Token: 0x040019B6 RID: 6582
	public static string PropEntityFlags = "EntityFlags";

	// Token: 0x040019B7 RID: 6583
	public static string PropEntityType = "EntityType";

	// Token: 0x040019B8 RID: 6584
	public static string PropClass = "Class";

	// Token: 0x040019B9 RID: 6585
	public static string PropCensor = "Censor";

	// Token: 0x040019BA RID: 6586
	public static string PropMesh = "Mesh";

	// Token: 0x040019BB RID: 6587
	public static string PropMeshFP = "MeshFP";

	// Token: 0x040019BC RID: 6588
	public static string PropPrefab = "Prefab";

	// Token: 0x040019BD RID: 6589
	public static string PropPrefabCombined = "PrefabCombined";

	// Token: 0x040019BE RID: 6590
	public static string PropParent = "Parent";

	// Token: 0x040019BF RID: 6591
	public static string PropAvatarController = "AvatarController";

	// Token: 0x040019C0 RID: 6592
	public static string PropLocalAvatarController = "LocalAvatarController";

	// Token: 0x040019C1 RID: 6593
	public static string PropSkinTexture = "SkinTexture";

	// Token: 0x040019C2 RID: 6594
	public static string PropAltMats = "AltMats";

	// Token: 0x040019C3 RID: 6595
	public static string PropSwapMats = "SwapMats";

	// Token: 0x040019C4 RID: 6596
	public static string PropMatColor = "MatColor";

	// Token: 0x040019C5 RID: 6597
	public static string PropRightHandJointName = "RightHandJointName";

	// Token: 0x040019C6 RID: 6598
	public static string PropHandItem = "HandItem";

	// Token: 0x040019C7 RID: 6599
	public static string PropHandItemCrawler = "HandItemCrawler";

	// Token: 0x040019C8 RID: 6600
	public static string PropMaxHealth = "MaxHealth";

	// Token: 0x040019C9 RID: 6601
	public static string PropMaxStamina = "MaxStamina";

	// Token: 0x040019CA RID: 6602
	public static string PropSickness = "Sickness";

	// Token: 0x040019CB RID: 6603
	public static string PropGassiness = "Gassiness";

	// Token: 0x040019CC RID: 6604
	public static string PropWellness = "Wellness";

	// Token: 0x040019CD RID: 6605
	public static string PropFood = "Food";

	// Token: 0x040019CE RID: 6606
	public static string PropWater = "Water";

	// Token: 0x040019CF RID: 6607
	public static string PropMaxViewAngle = "MaxViewAngle";

	// Token: 0x040019D0 RID: 6608
	public static string PropWeight = "Weight";

	// Token: 0x040019D1 RID: 6609
	public static string PropPushFactor = "PushFactor";

	// Token: 0x040019D2 RID: 6610
	public static string PropTimeStayAfterDeath = "TimeStayAfterDeath";

	// Token: 0x040019D3 RID: 6611
	public static string PropImmunity = "Immunity";

	// Token: 0x040019D4 RID: 6612
	public static string PropIsMale = "IsMale";

	// Token: 0x040019D5 RID: 6613
	public static string PropIsChunkObserver = "IsChunkObserver";

	// Token: 0x040019D6 RID: 6614
	public static string PropAIFeralSense = "AIFeralSense";

	// Token: 0x040019D7 RID: 6615
	public static string PropAIGroupCircle = "AIGroupCircle";

	// Token: 0x040019D8 RID: 6616
	public static string PropAINoiseSeekDist = "AINoiseSeekDist";

	// Token: 0x040019D9 RID: 6617
	public static string PropAISeeOffset = "AISeeOffset";

	// Token: 0x040019DA RID: 6618
	public static string PropAIPathCostScale = "AIPathCostScale";

	// Token: 0x040019DB RID: 6619
	public static string PropAITask = "AITask-";

	// Token: 0x040019DC RID: 6620
	public static string PropAITargetTask = "AITarget-";

	// Token: 0x040019DD RID: 6621
	public static string PropMoveSpeed = "MoveSpeed";

	// Token: 0x040019DE RID: 6622
	public static string PropMoveSpeedNight = "MoveSpeedNight";

	// Token: 0x040019DF RID: 6623
	public static string PropMoveSpeedAggro = "MoveSpeedAggro";

	// Token: 0x040019E0 RID: 6624
	public static string PropMoveSpeedRand = "MoveSpeedRand";

	// Token: 0x040019E1 RID: 6625
	public static string PropMoveSpeedPanic = "MoveSpeedPanic";

	// Token: 0x040019E2 RID: 6626
	public static string PropMoveSpeedPattern = "MoveSpeedPattern";

	// Token: 0x040019E3 RID: 6627
	public static string PropSwimSpeed = "SwimSpeed";

	// Token: 0x040019E4 RID: 6628
	public static string PropSwimStrokeRate = "SwimStrokeRate";

	// Token: 0x040019E5 RID: 6629
	public static string PropCrouchType = "CrouchType";

	// Token: 0x040019E6 RID: 6630
	public static string PropDanceType = "DanceType";

	// Token: 0x040019E7 RID: 6631
	public static string PropWalkType = "WalkType";

	// Token: 0x040019E8 RID: 6632
	public static string PropCanClimbVertical = "CanClimbVertical";

	// Token: 0x040019E9 RID: 6633
	public static string PropCanClimbLadders = "CanClimbLadders";

	// Token: 0x040019EA RID: 6634
	public static string PropJumpDelay = "JumpDelay";

	// Token: 0x040019EB RID: 6635
	public static string PropJumpMaxDistance = "JumpMaxDistance";

	// Token: 0x040019EC RID: 6636
	public static string PropIsEnemyEntity = "IsEnemyEntity";

	// Token: 0x040019ED RID: 6637
	public static string PropIsAnimalEntity = "IsAnimalEntity";

	// Token: 0x040019EE RID: 6638
	public static string PropSoundRandomTime = "SoundRandomTime";

	// Token: 0x040019EF RID: 6639
	public static string PropSoundAlertTime = "SoundAlertTime";

	// Token: 0x040019F0 RID: 6640
	public static string PropSoundRandom = "SoundRandom";

	// Token: 0x040019F1 RID: 6641
	public static string PropSoundHurt = "SoundHurt";

	// Token: 0x040019F2 RID: 6642
	public static string PropSoundDistressed = "SoundDistressed";

	// Token: 0x040019F3 RID: 6643
	public static string PropSoundJump = "SoundJump";

	// Token: 0x040019F4 RID: 6644
	public static string PropSoundPlayerLandThump = "SoundPlayerLandThump";

	// Token: 0x040019F5 RID: 6645
	public static string PropSoundHurtSmall = "SoundHurtSmall";

	// Token: 0x040019F6 RID: 6646
	public static string PropSoundDrownPain = "SoundDrownPain";

	// Token: 0x040019F7 RID: 6647
	public static string PropSoundDrownDeath = "SoundDrownDeath";

	// Token: 0x040019F8 RID: 6648
	public static string PropSoundWaterSurface = "SoundWaterSurface";

	// Token: 0x040019F9 RID: 6649
	public static string PropSoundDeath = "SoundDeath";

	// Token: 0x040019FA RID: 6650
	public static string PropSoundAttack = "SoundAttack";

	// Token: 0x040019FB RID: 6651
	public static string PropSoundAlert = "SoundAlert";

	// Token: 0x040019FC RID: 6652
	public static string PropSoundSense = "SoundSense";

	// Token: 0x040019FD RID: 6653
	public static string PropSoundStamina = "SoundStamina";

	// Token: 0x040019FE RID: 6654
	public static string PropSoundLiving = "SoundLiving";

	// Token: 0x040019FF RID: 6655
	public static string PropSoundSpawn = "SoundSpawn";

	// Token: 0x04001A00 RID: 6656
	public static string PropSoundLand = "SoundLanding";

	// Token: 0x04001A01 RID: 6657
	public static string PropSoundStepType = "SoundStepType";

	// Token: 0x04001A02 RID: 6658
	public static string PropSoundGiveUp = "SoundGiveUp";

	// Token: 0x04001A03 RID: 6659
	public static string PropSoundExplodeWarn = "SoundExplodeWarn";

	// Token: 0x04001A04 RID: 6660
	public static string PropSoundTick = "SoundTick";

	// Token: 0x04001A05 RID: 6661
	public static string PropExplodeDelay = "ExplodeDelay";

	// Token: 0x04001A06 RID: 6662
	public static string PropExplodeHealthThreshold = "ExplodeHealthThreshold";

	// Token: 0x04001A07 RID: 6663
	public static string PropLootList = "LootList";

	// Token: 0x04001A08 RID: 6664
	public static string PropLootDropProb = "LootDropProb";

	// Token: 0x04001A09 RID: 6665
	public static string PropLootDropEntityClass = "LootDropEntityClass";

	// Token: 0x04001A0A RID: 6666
	public static string PropAttackTimeoutDay = "AttackTimeoutDay";

	// Token: 0x04001A0B RID: 6667
	public static string PropAttackTimeoutNight = "AttackTimeoutNight";

	// Token: 0x04001A0C RID: 6668
	public static string PropMapIcon = "MapIcon";

	// Token: 0x04001A0D RID: 6669
	public static string PropCompassIcon = "CompassIcon";

	// Token: 0x04001A0E RID: 6670
	public static string PropTrackerIcon = "TrackerIcon";

	// Token: 0x04001A0F RID: 6671
	public static string PropCompassUpIcon = "CompassUpIcon";

	// Token: 0x04001A10 RID: 6672
	public static string PropCompassDownIcon = "CompassDownIcon";

	// Token: 0x04001A11 RID: 6673
	public static string PropParticleOnSpawn = "ParticleOnSpawn";

	// Token: 0x04001A12 RID: 6674
	public static string PropParticleOnDeath = "ParticleOnDeath";

	// Token: 0x04001A13 RID: 6675
	public static string PropParticleOnDestroy = "ParticleOnDestroy";

	// Token: 0x04001A14 RID: 6676
	public static string PropItemsOnEnterGame = "ItemsOnEnterGame";

	// Token: 0x04001A15 RID: 6677
	public static string PropFallLandBehavior = "FallLandBehavior";

	// Token: 0x04001A16 RID: 6678
	public static string PropDestroyBlockBehavior = "DestroyBlockBehavior";

	// Token: 0x04001A17 RID: 6679
	public static string PropDropInventoryBlock = "DropInventoryBlock";

	// Token: 0x04001A18 RID: 6680
	public static string PropModelType = "ModelType";

	// Token: 0x04001A19 RID: 6681
	public static string PropRagdollOnDeathChance = "RagdollOnDeathChance";

	// Token: 0x04001A1A RID: 6682
	public static string PropHasRagdoll = "HasRagdoll";

	// Token: 0x04001A1B RID: 6683
	public static string PropMass = "Mass";

	// Token: 0x04001A1C RID: 6684
	public static string PropSizeScale = "SizeScale";

	// Token: 0x04001A1D RID: 6685
	public static string PropPhysicsBody = "PhysicsBody";

	// Token: 0x04001A1E RID: 6686
	public static string PropColliders = "Colliders";

	// Token: 0x04001A1F RID: 6687
	public static string PropLookAtAngle = "LookAtAngle";

	// Token: 0x04001A20 RID: 6688
	public static string PropCrouchYOffsetFP = "CrouchYOffsetFP";

	// Token: 0x04001A21 RID: 6689
	public static string PropRotateToGround = "RotateToGround";

	// Token: 0x04001A22 RID: 6690
	public static string PropRootMotion = "RootMotion";

	// Token: 0x04001A23 RID: 6691
	public static string PropExperienceGain = "ExperienceGain";

	// Token: 0x04001A24 RID: 6692
	public static string PropHasDeathAnim = "HasDeathAnim";

	// Token: 0x04001A25 RID: 6693
	public static string PropLegCrippleScale = "LegCrippleScale";

	// Token: 0x04001A26 RID: 6694
	public static string PropLegCrawlerThreshold = "LegCrawlerThreshold";

	// Token: 0x04001A27 RID: 6695
	public static string PropDismemberMultiplierHead = "DismemberMultiplierHead";

	// Token: 0x04001A28 RID: 6696
	public static string PropDismemberMultiplierArms = "DismemberMultiplierArms";

	// Token: 0x04001A29 RID: 6697
	public static string PropDismemberMultiplierLegs = "DismemberMultiplierLegs";

	// Token: 0x04001A2A RID: 6698
	public static string PropKnockdownKneelDamageThreshold = "KnockdownKneelDamageThreshold";

	// Token: 0x04001A2B RID: 6699
	public static string PropKnockdownKneelStunDuration = "KnockdownKneelStunDuration";

	// Token: 0x04001A2C RID: 6700
	public static string PropKnockdownProneDamageThreshold = "KnockdownProneDamageThreshold";

	// Token: 0x04001A2D RID: 6701
	public static string PropKnockdownProneStunDuration = "KnockdownProneStunDuration";

	// Token: 0x04001A2E RID: 6702
	public static string PropKnockdownProneRefillRate = "KnockdownProneRefillRate";

	// Token: 0x04001A2F RID: 6703
	public static string PropKnockdownKneelRefillRate = "KnockdownKneelRefillRate";

	// Token: 0x04001A30 RID: 6704
	public static string PropArmsExplosionDamageMultiplier = "ArmsExplosionDamageMultiplier";

	// Token: 0x04001A31 RID: 6705
	public static string PropLegsExplosionDamageMultiplier = "LegsExplosionDamageMultiplier";

	// Token: 0x04001A32 RID: 6706
	public static string PropChestExplosionDamageMultiplier = "ChestExplosionDamageMultiplier";

	// Token: 0x04001A33 RID: 6707
	public static string PropHeadExplosionDamageMultiplier = "HeadExplosionDamageMultiplier";

	// Token: 0x04001A34 RID: 6708
	public static string PropPainResistPerHit = "PainResistPerHit";

	// Token: 0x04001A35 RID: 6709
	public static string PropArchetype = "Archetype";

	// Token: 0x04001A36 RID: 6710
	public static string PropSwimOffset = "SwimOffset";

	// Token: 0x04001A37 RID: 6711
	public static string PropUMARace = "UMARace";

	// Token: 0x04001A38 RID: 6712
	public static string PropUMAGeneratedModelName = "UMAGeneratedModelName";

	// Token: 0x04001A39 RID: 6713
	public static string PropNPCID = "NPCID";

	// Token: 0x04001A3A RID: 6714
	public static string PropModelTransformAdjust = "ModelTransformAdjust";

	// Token: 0x04001A3B RID: 6715
	public static string PropAIPackages = "AIPackages";

	// Token: 0x04001A3C RID: 6716
	public static string PropBuffs = "Buffs";

	// Token: 0x04001A3D RID: 6717
	public static string PropStealthSoundDecayRate = "StealthSoundDecayRate";

	// Token: 0x04001A3E RID: 6718
	public static string PropSightRange = "SightRange";

	// Token: 0x04001A3F RID: 6719
	public static string PropSightLightThreshold = "SightLightThreshold";

	// Token: 0x04001A40 RID: 6720
	public static string PropSleeperSightToSenseMin = "SleeperSightToSenseMin";

	// Token: 0x04001A41 RID: 6721
	public static string PropSleeperSightToSenseMax = "SleeperSightToSenseMax";

	// Token: 0x04001A42 RID: 6722
	public static string PropSleeperSightToWakeMin = "SleeperSightToWakeMin";

	// Token: 0x04001A43 RID: 6723
	public static string PropSleeperSightToWakeMax = "SleeperSightToWakeMax";

	// Token: 0x04001A44 RID: 6724
	public static string PropSleeperNoiseToSense = "SleeperNoiseToSense";

	// Token: 0x04001A45 RID: 6725
	public static string PropSleeperNoiseToSenseSoundChance = "SleeperNoiseToSenseSoundChance";

	// Token: 0x04001A46 RID: 6726
	public static string PropSleeperNoiseToWake = "SleeperNoiseToWake";

	// Token: 0x04001A47 RID: 6727
	public static string PropSoundSleeperSense = "SoundSleeperSense";

	// Token: 0x04001A48 RID: 6728
	public static string PropSoundSleeperSnore = "SoundSleeperBackToSleep";

	// Token: 0x04001A49 RID: 6729
	public static string PropMaxTurnSpeed = "MaxTurnSpeed";

	// Token: 0x04001A4A RID: 6730
	public static string PropSearchRadius = "SearchRadius";

	// Token: 0x04001A4B RID: 6731
	public static string PropTags = "Tags";

	// Token: 0x04001A4C RID: 6732
	public static string PropNavObject = "NavObject";

	// Token: 0x04001A4D RID: 6733
	public static string PropNavObjectHeadOffset = "NavObjectHeadOffset";

	// Token: 0x04001A4E RID: 6734
	public static string PropStompsSpikes = "StompsSpikes";

	// Token: 0x04001A4F RID: 6735
	public static string PropUserSpawnType = "UserSpawnType";

	// Token: 0x04001A50 RID: 6736
	public static string PropHideInSpawnMenu = "HideInSpawnMenu";

	// Token: 0x04001A51 RID: 6737
	public static string PropCanBigHead = "CanBigHead";

	// Token: 0x04001A52 RID: 6738
	public static string PropOnActivateEvent = "ActivateEvent";

	// Token: 0x04001A53 RID: 6739
	public static string PropCustomCommandName = "CustomCommandName";

	// Token: 0x04001A54 RID: 6740
	public static string PropCustomCommandIcon = "CustomCommandIcon";

	// Token: 0x04001A55 RID: 6741
	public static string PropCustomCommandIconColor = "CustomCommandIconColor";

	// Token: 0x04001A56 RID: 6742
	public static string PropCustomCommandEvent = "CustomCommandEvent";

	// Token: 0x04001A57 RID: 6743
	public static string PropCustomCommandActivateTime = "CustomCommandActivateTime";

	// Token: 0x04001A58 RID: 6744
	public static string PropPreviousTierZombie = "PreviousTier";

	// Token: 0x04001A59 RID: 6745
	public static string PropTokenManager = "TokenManager";

	// Token: 0x04001A5A RID: 6746
	public static string PropPickupItem = "PickupItem";

	// Token: 0x04001A5B RID: 6747
	public static string PropPickupStressCvar = "PickupStressCVar";

	// Token: 0x04001A5C RID: 6748
	public static string PropPickupStressBuff = "PickupStressBuff";

	// Token: 0x04001A5D RID: 6749
	public static readonly int itemClass = EntityClass.FromString("item");

	// Token: 0x04001A5E RID: 6750
	public static readonly int fallingBlockClass = EntityClass.FromString("fallingBlock");

	// Token: 0x04001A5F RID: 6751
	public static readonly int fallingBlocksClass = EntityClass.FromString("fallingBlocks");

	// Token: 0x04001A60 RID: 6752
	public static readonly int fallingTreeClass = EntityClass.FromString("fallingTree");

	// Token: 0x04001A61 RID: 6753
	public static readonly int playerMaleClass = EntityClass.FromString("playerMale");

	// Token: 0x04001A62 RID: 6754
	public static readonly int playerFemaleClass = EntityClass.FromString("playerFemale");

	// Token: 0x04001A63 RID: 6755
	public static readonly int playerNewMaleClass = EntityClass.FromString("playerNewMale");

	// Token: 0x04001A64 RID: 6756
	public static readonly int junkDroneClass = EntityClass.FromString("entityJunkDrone");

	// Token: 0x04001A65 RID: 6757
	public static FastTags<TagGroup.Global> strongTag = FastTags<TagGroup.Global>.Parse("strong");

	// Token: 0x04001A66 RID: 6758
	public static FastTags<TagGroup.Global> specialTag = FastTags<TagGroup.Global>.Parse("special");

	// Token: 0x04001A67 RID: 6759
	public static FastTags<TagGroup.Global> feralTag = FastTags<TagGroup.Global>.Parse("feral,animalFeral");

	// Token: 0x04001A68 RID: 6760
	public static FastTags<TagGroup.Global> radiatedTag = FastTags<TagGroup.Global>.Parse("radiated");

	// Token: 0x04001A69 RID: 6761
	public static FastTags<TagGroup.Global> eliteTag = FastTags<TagGroup.Global>.Parse("charged,infernal");

	// Token: 0x04001A6A RID: 6762
	public static FastTags<TagGroup.Global> droneTag = FastTags<TagGroup.Global>.Parse("drone");

	// Token: 0x04001A6B RID: 6763
	public static FastTags<TagGroup.Global> turretRangedTag = FastTags<TagGroup.Global>.Parse("turretRanged");

	// Token: 0x04001A6C RID: 6764
	public static FastTags<TagGroup.Global> turretMeleeTag = FastTags<TagGroup.Global>.Parse("turretMelee");

	// Token: 0x04001A6D RID: 6765
	public static Dictionary<string, Color> sColors = new Dictionary<string, Color>();

	// Token: 0x04001A6E RID: 6766
	public static DictionarySave<int, EntityClass> list = new DictionarySave<int, EntityClass>();

	// Token: 0x04001A6F RID: 6767
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] commaSeparator = new char[]
	{
		','
	};

	// Token: 0x04001A70 RID: 6768
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x04001A71 RID: 6769
	public Type classname;

	// Token: 0x04001A72 RID: 6770
	public int censorMode;

	// Token: 0x04001A73 RID: 6771
	public EntityFlags entityFlags;

	// Token: 0x04001A74 RID: 6772
	public int censorType;

	// Token: 0x04001A75 RID: 6773
	public string prefabPath;

	// Token: 0x04001A76 RID: 6774
	public bool IsPrefabCombined;

	// Token: 0x04001A77 RID: 6775
	public string meshPath;

	// Token: 0x04001A78 RID: 6776
	public Transform meshFP;

	// Token: 0x04001A79 RID: 6777
	public string skinTexture;

	// Token: 0x04001A7A RID: 6778
	public string parentGameObjectName;

	// Token: 0x04001A7B RID: 6779
	public string entityClassName;

	// Token: 0x04001A7C RID: 6780
	public EntityClass.UserSpawnType userSpawnType = EntityClass.UserSpawnType.Menu;

	// Token: 0x04001A7D RID: 6781
	public bool bIsEnemyEntity;

	// Token: 0x04001A7E RID: 6782
	public bool bIsAnimalEntity;

	// Token: 0x04001A7F RID: 6783
	public ExplosionData explosionData;

	// Token: 0x04001A80 RID: 6784
	public Type modelType;

	// Token: 0x04001A81 RID: 6785
	public float MassKg;

	// Token: 0x04001A82 RID: 6786
	public float SizeScale;

	// Token: 0x04001A83 RID: 6787
	public float RagdollOnDeathChance;

	// Token: 0x04001A84 RID: 6788
	public bool HasRagdoll;

	// Token: 0x04001A85 RID: 6789
	public string CollidersRagdollAsset;

	// Token: 0x04001A86 RID: 6790
	public float LookAtAngle;

	// Token: 0x04001A87 RID: 6791
	public float crouchYOffsetFP;

	// Token: 0x04001A88 RID: 6792
	public float MaxTurnSpeed;

	// Token: 0x04001A89 RID: 6793
	public bool RootMotion;

	// Token: 0x04001A8A RID: 6794
	public bool HasDeathAnim;

	// Token: 0x04001A8B RID: 6795
	public bool bIsMale;

	// Token: 0x04001A8C RID: 6796
	public bool bIsChunkObserver;

	// Token: 0x04001A8D RID: 6797
	public int ExperienceValue;

	// Token: 0x04001A8E RID: 6798
	public PhysicsBodyLayout PhysicsBody;

	// Token: 0x04001A8F RID: 6799
	public int DeadBodyHitPoints;

	// Token: 0x04001A90 RID: 6800
	public List<EntityClass.LootDrop> lootDrops;

	// Token: 0x04001A91 RID: 6801
	public float LegCrippleScale;

	// Token: 0x04001A92 RID: 6802
	public float LegCrawlerThreshold;

	// Token: 0x04001A93 RID: 6803
	public float DismemberMultiplierHead;

	// Token: 0x04001A94 RID: 6804
	public float DismemberMultiplierArms;

	// Token: 0x04001A95 RID: 6805
	public float DismemberMultiplierLegs;

	// Token: 0x04001A96 RID: 6806
	public float LowerLegDismemberThreshold;

	// Token: 0x04001A97 RID: 6807
	public float LowerLegDismemberBonusChance;

	// Token: 0x04001A98 RID: 6808
	public float LowerLegDismemberBaseChance;

	// Token: 0x04001A99 RID: 6809
	public float UpperLegDismemberThreshold;

	// Token: 0x04001A9A RID: 6810
	public float UpperLegDismemberBonusChance;

	// Token: 0x04001A9B RID: 6811
	public float UpperLegDismemberBaseChance;

	// Token: 0x04001A9C RID: 6812
	public float LowerArmDismemberThreshold;

	// Token: 0x04001A9D RID: 6813
	public float LowerArmDismemberBonusChance;

	// Token: 0x04001A9E RID: 6814
	public float LowerArmDismemberBaseChance;

	// Token: 0x04001A9F RID: 6815
	public float UpperArmDismemberThreshold;

	// Token: 0x04001AA0 RID: 6816
	public float UpperArmDismemberBonusChance;

	// Token: 0x04001AA1 RID: 6817
	public float UpperArmDismemberBaseChance;

	// Token: 0x04001AA2 RID: 6818
	public float KnockdownKneelDamageThreshold;

	// Token: 0x04001AA3 RID: 6819
	public float LegsExplosionDamageMultiplier;

	// Token: 0x04001AA4 RID: 6820
	public float ArmsExplosionDamageMultiplier;

	// Token: 0x04001AA5 RID: 6821
	public float ChestExplosionDamageMultiplier;

	// Token: 0x04001AA6 RID: 6822
	public float HeadExplosionDamageMultiplier;

	// Token: 0x04001AA7 RID: 6823
	public float PainResistPerHit;

	// Token: 0x04001AA8 RID: 6824
	public float PainResistPerHitLowHealth;

	// Token: 0x04001AA9 RID: 6825
	public float PainResistPerHitLowHealthPercent;

	// Token: 0x04001AAA RID: 6826
	public float SearchRadius;

	// Token: 0x04001AAB RID: 6827
	public float SwimOffset;

	// Token: 0x04001AAC RID: 6828
	public float SightRange;

	// Token: 0x04001AAD RID: 6829
	public Vector2 SleeperSightToSenseMin;

	// Token: 0x04001AAE RID: 6830
	public Vector2 SleeperSightToSenseMax;

	// Token: 0x04001AAF RID: 6831
	public Vector2 SleeperSightToWakeMin;

	// Token: 0x04001AB0 RID: 6832
	public Vector2 SleeperSightToWakeMax;

	// Token: 0x04001AB1 RID: 6833
	public Vector2 sightLightThreshold;

	// Token: 0x04001AB2 RID: 6834
	public Vector2 NoiseAlert;

	// Token: 0x04001AB3 RID: 6835
	public Vector2 SleeperNoiseToSense;

	// Token: 0x04001AB4 RID: 6836
	public float SleeperNoiseToSenseSoundChance;

	// Token: 0x04001AB5 RID: 6837
	public Vector2 SleeperNoiseToWake;

	// Token: 0x04001AB6 RID: 6838
	public string UMARace;

	// Token: 0x04001AB7 RID: 6839
	public string UMAGeneratedModelName;

	// Token: 0x04001AB8 RID: 6840
	public string[] AltMatNames;

	// Token: 0x04001AB9 RID: 6841
	public string[] MatSwap;

	// Token: 0x04001ABA RID: 6842
	public EntityClass.ParticleData particleOnSpawn;

	// Token: 0x04001ABB RID: 6843
	public Vector2 KnockdownKneelStunDuration;

	// Token: 0x04001ABC RID: 6844
	public float KnockdownProneDamageThreshold;

	// Token: 0x04001ABD RID: 6845
	public Vector2 KnockdownProneStunDuration;

	// Token: 0x04001ABE RID: 6846
	public Vector2 KnockdownProneRefillRate;

	// Token: 0x04001ABF RID: 6847
	public Vector2 KnockdownKneelRefillRate;

	// Token: 0x04001AC0 RID: 6848
	public Vector3 ModelTransformAdjust;

	// Token: 0x04001AC1 RID: 6849
	public string ArchetypeName;

	// Token: 0x04001AC2 RID: 6850
	public string[] AIPackages;

	// Token: 0x04001AC3 RID: 6851
	public bool UseAIPackages;

	// Token: 0x04001AC4 RID: 6852
	public Dictionary<EnumDropEvent, List<Block.SItemDropProb>> itemsToDrop = new EnumDictionary<EnumDropEvent, List<Block.SItemDropProb>>();

	// Token: 0x04001AC5 RID: 6853
	public List<string> Buffs;

	// Token: 0x04001AC6 RID: 6854
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x04001AC7 RID: 6855
	public string NavObject = "";

	// Token: 0x04001AC8 RID: 6856
	public Vector3 NavObjectHeadOffset = Vector3.zero;

	// Token: 0x04001AC9 RID: 6857
	public bool CanBigHead = true;

	// Token: 0x04001ACA RID: 6858
	public int DanceTypeID;

	// Token: 0x04001ACB RID: 6859
	public MinEffectController Effects;

	// Token: 0x04001ACC RID: 6860
	public string PreviousTierZombieName = "";

	// Token: 0x04001ACD RID: 6861
	public string PickupItem = "";

	// Token: 0x04001ACE RID: 6862
	public string PickupStressCvar = "";

	// Token: 0x04001ACF RID: 6863
	public string PickupStressBuff = "";

	// Token: 0x04001AD0 RID: 6864
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityClass[] previousTierEntities;

	// Token: 0x04001AD1 RID: 6865
	public EntityClass.EntityTierTypes EntityTier;

	// Token: 0x04001AD2 RID: 6866
	public string onActivateEvent = "";

	// Token: 0x04001AD3 RID: 6867
	public Dictionary<AITokenType, AITokenConfig> TokenManagerConfig;

	// Token: 0x02000491 RID: 1169
	public enum CensorModeType
	{
		// Token: 0x04001AD5 RID: 6869
		None,
		// Token: 0x04001AD6 RID: 6870
		ZPrefab,
		// Token: 0x04001AD7 RID: 6871
		Dismemberment,
		// Token: 0x04001AD8 RID: 6872
		ZPrefabAndDismemberment
	}

	// Token: 0x02000492 RID: 1170
	public enum UserSpawnType
	{
		// Token: 0x04001ADA RID: 6874
		None,
		// Token: 0x04001ADB RID: 6875
		Console,
		// Token: 0x04001ADC RID: 6876
		Menu
	}

	// Token: 0x02000493 RID: 1171
	public struct LootDrop
	{
		// Token: 0x04001ADD RID: 6877
		public int entityClass;

		// Token: 0x04001ADE RID: 6878
		public float weight;
	}

	// Token: 0x02000494 RID: 1172
	public struct ParticleData
	{
		// Token: 0x04001ADF RID: 6879
		public string fileName;

		// Token: 0x04001AE0 RID: 6880
		public string shapeMesh;
	}

	// Token: 0x02000495 RID: 1173
	public enum EntityTierTypes
	{
		// Token: 0x04001AE2 RID: 6882
		Normal,
		// Token: 0x04001AE3 RID: 6883
		Strong,
		// Token: 0x04001AE4 RID: 6884
		Special,
		// Token: 0x04001AE5 RID: 6885
		Feral,
		// Token: 0x04001AE6 RID: 6886
		Radiated,
		// Token: 0x04001AE7 RID: 6887
		Elite
	}
}
