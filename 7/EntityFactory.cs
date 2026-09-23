using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004AC RID: 1196
[Preserve]
public class EntityFactory
{
	// Token: 0x060025AD RID: 9645 RVA: 0x000E5D50 File Offset: 0x000E3F50
	public static void Init(Transform _entitiesTransform)
	{
		EntityFactory.ParentNameToTransform.Clear();
		EntityFactory.FindOrCreateTransform(null, "Players", 0);
		EntityFactory.FindOrCreateTransform(_entitiesTransform, "Items", 0);
		EntityFactory.FindOrCreateTransform(_entitiesTransform, "FallingBlocks", 0);
		EntityFactory.FindOrCreateTransform(_entitiesTransform, "FallingTrees", 0);
		EntityFactory.FindOrCreateTransform(_entitiesTransform, "Enemies", 1);
		EntityFactory.FindOrCreateTransform(_entitiesTransform, "Animals", 1);
		foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
		{
			EntityFactory.FindOrCreateTransform(_entitiesTransform, keyValuePair.Value.parentGameObjectName, 0);
		}
	}

	// Token: 0x060025AE RID: 9646 RVA: 0x000E5E08 File Offset: 0x000E4008
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FindOrCreateTransform(Transform _parent, string _name, int _originLevel)
	{
		if (_name == null)
		{
			return;
		}
		if (!EntityFactory.ParentNameToTransform.ContainsKey(_name))
		{
			Transform transform;
			if (_parent != null)
			{
				transform = _parent.Find(_name);
			}
			else
			{
				GameObject gameObject = GameObject.Find("/" + _name);
				transform = ((gameObject != null) ? gameObject.transform : null);
			}
			if (transform == null)
			{
				transform = new GameObject(_name).transform;
				transform.name = _name;
				transform.parent = _parent;
				Origin.Add(transform, _originLevel);
			}
			EntityFactory.ParentNameToTransform[_name] = transform;
		}
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000027FC File Offset: 0x000009FC
	public static void Cleanup()
	{
	}

	// Token: 0x060025B0 RID: 9648 RVA: 0x000DCEEE File Offset: 0x000DB0EE
	public static void CleanupStatic()
	{
		EntityClass.list.Clear();
	}

	// Token: 0x060025B1 RID: 9649 RVA: 0x000E5E94 File Offset: 0x000E4094
	public static Type GetEntityType(string _className)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_className);
		if (num <= 2057157508U)
		{
			if (num <= 680637634U)
			{
				if (num != 431260344U)
				{
					if (num == 680637634U)
					{
						if (_className == "EntityAnimalRabbit")
						{
							return typeof(EntityAnimalRabbit);
						}
					}
				}
				else if (_className == "EntityZombie")
				{
					return typeof(EntityZombie);
				}
			}
			else if (num != 1041995262U)
			{
				if (num != 1891769587U)
				{
					if (num == 2057157508U)
					{
						if (_className == "EntityBandit")
						{
							return typeof(EntityBandit);
						}
					}
				}
				else if (_className == "EntityPlayer")
				{
					return typeof(EntityPlayer);
				}
			}
			else if (_className == "EntityAnimal")
			{
				return typeof(EntityAnimal);
			}
		}
		else if (num <= 3350477675U)
		{
			if (num != 2926901533U)
			{
				if (num == 3350477675U)
				{
					if (_className == "EntityHuman")
					{
						return typeof(EntityHuman);
					}
				}
			}
			else if (_className == "EntityAnimalStag")
			{
				return typeof(EntityAnimalStag);
			}
		}
		else if (num != 3522953666U)
		{
			if (num != 3771259116U)
			{
				if (num == 4192015845U)
				{
					if (_className == "EntityNPC")
					{
						return typeof(EntityNPC);
					}
				}
			}
			else if (_className == "EntityDrone")
			{
				return typeof(EntityDrone);
			}
		}
		else if (_className == "EntityEnemyAnimal")
		{
			return typeof(EntityEnemyAnimal);
		}
		Log.Warning("GetEntityType slow lookup for {0}", new object[]
		{
			_className
		});
		return Type.GetType(_className);
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000E6080 File Offset: 0x000E4280
	public static EntityCreationData SetupEntityCreationData(int _et, int _id, ItemValue _itemValue, int _count, Vector3 _transformPos, Vector3 _transformRot, float _lifetime, int _playerId, int _spawnById = -1, string _spawnByName = "")
	{
		return new EntityCreationData
		{
			entityClass = _et,
			id = _id,
			itemStack = new ItemStack(_itemValue, _count),
			pos = _transformPos,
			rot = _transformRot,
			lifetime = _lifetime,
			belongsPlayerId = _playerId,
			spawnById = _spawnById,
			spawnByName = _spawnByName
		};
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000E60E0 File Offset: 0x000E42E0
	public static EntityCreationData SetupEntityCreationData(int _et, int _id, BlockValue[] _blockValues, TextureFullArray[] _textureFullArrays, int _count, Vector3 _transformPos, Vector3 _transformRot, float _lifetime, int _playerId, int _spawnById = -1, string _spawnByName = "")
	{
		return new EntityCreationData
		{
			entityClass = _et,
			id = _id,
			blockValues = _blockValues,
			textureFullArrays = _textureFullArrays,
			itemStack = 
			{
				count = _count
			},
			pos = _transformPos,
			rot = _transformRot,
			lifetime = _lifetime,
			belongsPlayerId = _playerId,
			spawnById = _spawnById,
			spawnByName = _spawnByName
		};
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000E614B File Offset: 0x000E434B
	public static EntityCreationData SetupEntityCreationData(int _et, Vector3 _transformPos)
	{
		return EntityFactory.SetupEntityCreationData(_et, EntityFactory.nextEntityID++, _transformPos, Vector3.zero);
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000E6166 File Offset: 0x000E4366
	public static EntityCreationData SetupEntityCreationData(int _et, Vector3 _transformPos, Vector3 _rotation)
	{
		return EntityFactory.SetupEntityCreationData(_et, EntityFactory.nextEntityID++, _transformPos, _rotation);
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000E6180 File Offset: 0x000E4380
	public static EntityCreationData SetupEntityCreationData(int _et, int _id, Vector3 _transformPos, Vector3 _rotation)
	{
		return EntityFactory.SetupEntityCreationData(_et, _id, ItemValue.None, 1, _transformPos, _rotation, float.MaxValue, -1, -1, "");
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000E61A8 File Offset: 0x000E43A8
	public static Entity CreateEntity(int _et, Vector3 _transformPos)
	{
		return EntityFactory.CreateEntity(EntityFactory.SetupEntityCreationData(_et, _transformPos));
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000E61B6 File Offset: 0x000E43B6
	public static Entity CreateEntity(int _et, Vector3 _transformPos, Vector3 _rotation)
	{
		return EntityFactory.CreateEntity(EntityFactory.SetupEntityCreationData(_et, _transformPos, _rotation));
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000E61C5 File Offset: 0x000E43C5
	public static Entity CreateEntity(int _et, int _id, Vector3 _transformPos, Vector3 _rotation)
	{
		return EntityFactory.CreateEntity(EntityFactory.SetupEntityCreationData(_et, _id, _transformPos, _rotation));
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x000E61D8 File Offset: 0x000E43D8
	public static Entity CreateEntity(int _et, Vector3 _transformPos, Vector3 _rotation, int _spawnById, string _spawnByName)
	{
		return EntityFactory.CreateEntity(EntityFactory.SetupEntityCreationData(_et, EntityFactory.nextEntityID++, ItemValue.None, 1, _transformPos, _rotation, float.MaxValue, -1, _spawnById, _spawnByName));
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x000E6210 File Offset: 0x000E4410
	public static Entity CreateEntity(int _et, int _id, BlockValue[] _blockValues, TextureFullArray[] _textureFullArrays, int _count, Vector3 _transformPos, Vector3 _transformRot, float _lifetime, int _playerId, int _spawnById = -1, string _spawnByName = "")
	{
		return EntityFactory.CreateEntity(EntityFactory.SetupEntityCreationData(_et, _id, _blockValues, _textureFullArrays, _count, _transformPos, _transformRot, _lifetime, _playerId, _spawnById, _spawnByName));
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x000E6239 File Offset: 0x000E4439
	public static Entity CreateEntity(EntityCreationData _ecd)
	{
		EntityFactory.CreateEntityOperation createEntityOperation = EntityFactory.CreateEntityOperation.Start(_ecd, true);
		createEntityOperation.CompleteEntity();
		return createEntityOperation.entity;
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000E624D File Offset: 0x000E444D
	public static EntityFactory.CreateEntityOperation CreateEntityAsync(EntityCreationData _ecd)
	{
		return EntityFactory.CreateEntityOperation.Start(_ecd, false);
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x000E6256 File Offset: 0x000E4456
	[PublicizedFrom(EAccessModifier.Private)]
	public static Entity addEntityComponent(GameObject _gameObject, string _className)
	{
		return EntityFactory.addEntityComponent(_gameObject, Type.GetType(_className));
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000E6264 File Offset: 0x000E4464
	[PublicizedFrom(EAccessModifier.Private)]
	public static Entity addEntityComponent(GameObject _gameObject, Type _classType)
	{
		if (_classType != null)
		{
			return (Entity)_gameObject.AddComponent(_classType);
		}
		return null;
	}

	// Token: 0x04001BFB RID: 7163
	public static int nextEntityID;

	// Token: 0x04001BFC RID: 7164
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cNumberOfCachedFallingBlocks = 150;

	// Token: 0x04001BFD RID: 7165
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cNumberOfCachedItems = 20;

	// Token: 0x04001BFE RID: 7166
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFirstEntityID = 1;

	// Token: 0x04001BFF RID: 7167
	public const int StartEntityID = 171;

	// Token: 0x04001C00 RID: 7168
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly int playerNewMaleClass = EntityClass.FromString("playerNewMale");

	// Token: 0x04001C01 RID: 7169
	public static Dictionary<string, Transform> ParentNameToTransform = new Dictionary<string, Transform>();

	// Token: 0x04001C02 RID: 7170
	public static EntityClass.EntityTierTypes MaxEntityTier = EntityClass.EntityTierTypes.Elite;

	// Token: 0x04001C03 RID: 7171
	public static bool EnemySpawnMode = true;

	// Token: 0x020004AD RID: 1197
	public class CreateEntityOperation
	{
		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060025C2 RID: 9666 RVA: 0x000E62A4 File Offset: 0x000E44A4
		public int EntityId
		{
			get
			{
				return this.ecd.id;
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060025C3 RID: 9667 RVA: 0x000E62B1 File Offset: 0x000E44B1
		public string DebugEntityInfo
		{
			get
			{
				return this.ecd.ToString();
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060025C4 RID: 9668 RVA: 0x000E62BE File Offset: 0x000E44BE
		public bool IsLoadingComplete
		{
			get
			{
				if (this.ec == null)
				{
					return true;
				}
				EntityInstanceAssets entityInstanceAssets = this.entityInstanceAssets;
				if (entityInstanceAssets != null && entityInstanceAssets.IsLoadComplete)
				{
					EModelInstanceAssets emodelInstanceAssets = this.eModelInstanceAssets;
					return emodelInstanceAssets != null && emodelInstanceAssets.IsLoadComplete;
				}
				return false;
			}
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x000E62F4 File Offset: 0x000E44F4
		public static EntityFactory.CreateEntityOperation Start(EntityCreationData _ecd, bool _isSync)
		{
			if (_ecd.id == -1)
			{
				_ecd.id = EntityFactory.nextEntityID++;
			}
			else
			{
				EntityFactory.nextEntityID = Math.Max(_ecd.id + 1, EntityFactory.nextEntityID);
			}
			EntityFactory.CreateEntityOperation createEntityOperation = new EntityFactory.CreateEntityOperation(_ecd);
			createEntityOperation.LoadAssets(_isSync);
			return createEntityOperation;
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x000E6343 File Offset: 0x000E4543
		[PublicizedFrom(EAccessModifier.Private)]
		public CreateEntityOperation(EntityCreationData _ecd)
		{
			this.ecd = _ecd;
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x000E6354 File Offset: 0x000E4554
		[PublicizedFrom(EAccessModifier.Private)]
		public void LoadAssets(bool _isSync)
		{
			this.ec = EntityClass.GetEntityClass(this.ecd.entityClass);
			if (this.ec == null)
			{
				Log.Error(string.Format("EntityFactory CreateEntity: unknown type ({0}) {1}", this.ecd.entityClass, this.ecd.entityName));
				return;
			}
			this.ec = EntityClass.GetEntityClassWithinMaxTier(this.ec, EntityFactory.MaxEntityTier);
			if (this.ec == null)
			{
				Log.Error(string.Format("EntityFactory CreateEntity: no entity within Max Tier {0} could be found for type ({1}) {2}", EntityFactory.MaxEntityTier, this.ecd.entityClass, this.ecd.entityName));
				return;
			}
			this.ecd.entityClass = EntityClass.GetId(this.ec.entityClassName);
			this.isPlayer = (this.ecd.entityClass == EntityClass.playerMaleClass || this.ecd.entityClass == EntityClass.playerFemaleClass);
			this.isLocalPlayer = (this.isPlayer && this.ecd.id == this.ecd.belongsPlayerId);
			this.entityInstanceAssets = new EntityInstanceAssets();
			this.entityInstanceAssets.Load(_isSync, this.ec, this.isLocalPlayer);
			this.eModelInstanceAssets = new EModelInstanceAssets();
			this.eModelInstanceAssets.Load(_isSync, this.ecd, this.ec);
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x000E64B4 File Offset: 0x000E46B4
		public void WaitForLoadingComplete()
		{
			EntityInstanceAssets entityInstanceAssets = this.entityInstanceAssets;
			if (entityInstanceAssets != null)
			{
				entityInstanceAssets.WaitForComplete();
			}
			EModelInstanceAssets emodelInstanceAssets = this.eModelInstanceAssets;
			if (emodelInstanceAssets == null)
			{
				return;
			}
			emodelInstanceAssets.WaitForComplete();
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x000E64D8 File Offset: 0x000E46D8
		public void CompleteEntity()
		{
			if (!this.entityInstanceAssets.IsLoadComplete || !this.eModelInstanceAssets.IsLoadComplete)
			{
				Log.Error(string.Format("CreateEntityOperation cannot complete {0}, assets not loaded yet", this.ecd));
				return;
			}
			if (!this.entityInstanceAssets.IsLoadSuccessful)
			{
				Log.Error(string.Format("CreateEntityOperation cannot complete {0}, entity assets did not load successfully", this.ecd));
				return;
			}
			if (!this.eModelInstanceAssets.IsLoadSuccessful)
			{
				Log.Error(string.Format("CreateEntityOperation cannot complete {0}, emodel assets did not load successfully", this.ecd));
				return;
			}
			if (this.entity != null)
			{
				Log.Error(string.Format("Entity {0} has already been created. CompleteEntity is expected to only be called once", this.entity));
				return;
			}
			Transform transform = this.entityInstanceAssets.PrefabT;
			transform = UnityEngine.Object.Instantiate<Transform>(transform, Vector3.zero, Quaternion.identity);
			Transform transform2 = transform;
			Transform transform3 = transform.Find("GameObject");
			if (transform3)
			{
				transform2 = transform3;
			}
			transform2.position = this.ecd.pos - Origin.position;
			GameObject gameObject = transform2.gameObject;
			Entity entity;
			if (this.isPlayer)
			{
				EntityPlayer entityPlayer;
				if (this.isLocalPlayer)
				{
					entity = EntityFactory.addEntityComponent(gameObject, this.ec.classname.FullName + "Local");
					entityPlayer = (EntityPlayer)entity;
					entity.RootTransform = transform;
					entity.ModelTransform = transform.Find("Graphics");
					entity.PhysicsTransform = transform;
					entityPlayer.playerProfile = this.ecd.playerProfile;
					entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
					gameObject.AddComponent<LocalPlayer>();
				}
				else
				{
					entity = EntityFactory.addEntityComponent(gameObject, this.ec.classname);
					entityPlayer = (EntityPlayer)entity;
					entity.RootTransform = transform;
					entity.ModelTransform = transform2;
					entity.PhysicsTransform = transform.Find("Physics");
					entityPlayer.playerProfile = this.ecd.playerProfile;
					entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
					gameObject.AddComponent<GUIHUDEntityName>();
				}
				if (!this.ecd.holdingItem.IsEmpty())
				{
					entityPlayer.inventory.AddItem(new ItemStack(this.ecd.holdingItem, 1));
					entityPlayer.inventory.SetHoldingItemIdx(0);
				}
				entityPlayer.TeamNumber = this.ecd.teamNumber;
				entityPlayer.emodel.SetSkinTexture(this.ecd.skinTexture);
				transform.SetParent(EntityFactory.ParentNameToTransform[this.ec.parentGameObjectName], false);
				transform.name = "Player_" + this.ecd.id.ToString();
				Log.Out("Created player with id=" + this.ecd.id.ToString());
			}
			else if (this.ecd.entityClass == EntityClass.itemClass)
			{
				Entity entity2 = entity = gameObject.AddComponent<EntityItem>();
				entity.RootTransform = transform;
				entity.ModelTransform = transform2;
				entity.clientEntityId = this.ecd.clientEntityId;
				entity2.OwnerId = this.ecd.belongsPlayerId;
				entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
				transform.SetParent(EntityFactory.ParentNameToTransform["Items"], false);
				transform.name = "Item_" + this.ecd.id.ToString();
				entity2.SetItemStack(this.ecd.itemStack);
			}
			else if (this.ecd.entityClass == EntityClass.fallingBlockClass)
			{
				Entity entity3 = entity = gameObject.AddComponent<EntityFallingBlock>();
				entity.RootTransform = transform;
				entity.ModelTransform = transform2;
				entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
				transform.SetParent(EntityFactory.ParentNameToTransform["FallingBlocks"], false);
				transform.name = "FallingBlock_" + this.ecd.id.ToString();
				entity3.SetBlockValue(this.ecd.blockValues[0]);
				entity3.SetTextureFull(this.ecd.textureFullArrays[0]);
			}
			else if (this.ecd.entityClass == EntityClass.fallingBlocksClass)
			{
				Entity entity4 = entity = gameObject.AddComponent<EntityFallingBlocks>();
				entity.RootTransform = transform;
				entity.ModelTransform = transform2;
				entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
				transform.SetParent(EntityFactory.ParentNameToTransform["FallingBlocks"], false);
				transform.name = "FallingBlock_" + this.ecd.id.ToString();
				entity4.SetBlockGroupData(this.ecd.blockPositions, this.ecd.blockValues);
				entity4.SetTextureFullArrays(this.ecd.textureFullArrays);
			}
			else if (this.ecd.entityClass == EntityClass.fallingTreeClass)
			{
				EntityFallingTree entityFallingTree = entity = gameObject.AddComponent<EntityFallingTree>();
				entity.RootTransform = transform;
				entity.ModelTransform = transform2;
				entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
				transform.SetParent(EntityFactory.ParentNameToTransform["FallingTrees"], false);
				transform.name = "FallingTree_" + this.ecd.id.ToString();
				entityFallingTree.SetBlockPos(this.ecd.blockPos, this.ecd.fallTreeDir);
			}
			else
			{
				if (this.ec.classname == null)
				{
					Log.Error("Unknown entity " + this.ecd.entityClass.ToString());
					return;
				}
				entity = EntityFactory.addEntityComponent(gameObject, this.ec.classname);
				if (!entity)
				{
					return;
				}
				transform2.eulerAngles = this.ecd.rot;
				entity.entityId = this.ecd.id;
				entity.RootTransform = transform;
				entity.ModelTransform = transform2;
				entity.Init(this.ecd.entityClass, this.entityInstanceAssets, this.eModelInstanceAssets);
				if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuShowTasks) && entity is EntityAlive)
				{
					gameObject.AddComponent<GUIHUDEntityName>();
				}
				if (this.ec.parentGameObjectName != null)
				{
					transform.SetParent(EntityFactory.ParentNameToTransform[this.ec.parentGameObjectName], false);
				}
				transform.name = this.ec.entityClassName + "_" + this.ecd.id.ToString();
				entity.SetEntityName(this.ec.entityClassName);
				entity.emodel.SetSkinTexture(this.ec.skinTexture);
				CapsuleCollider[] componentsInChildren = transform.GetComponentsInChildren<CapsuleCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					GameObject gameObject2 = componentsInChildren[i].gameObject;
					if (!gameObject2.CompareTag("LargeEntityBlocker") && !gameObject2.CompareTag("Physics"))
					{
						gameObject2.layer = 14;
					}
				}
				BoxCollider[] componentsInChildren2 = transform.GetComponentsInChildren<BoxCollider>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].gameObject.layer = 14;
				}
			}
			this.ecd.ApplyToEntity(entity);
			if (entity.GetSpawnerSource() == EnumSpawnerSource.Delete)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
				return;
			}
			entity.lifetime = this.ecd.lifetime;
			entity.entityId = this.ecd.id;
			entity.belongsPlayerId = this.ecd.belongsPlayerId;
			entity.InitLocation(this.ecd.pos, this.ecd.rot);
			entity.onGround = this.ecd.onGround;
			if (this.ec.SizeScale != 1f)
			{
				entity.SetScale(this.ec.SizeScale);
			}
			if (this.ecd.overrideSize != 1f)
			{
				entity.SetScale(this.ecd.overrideSize);
			}
			if (this.ecd.overrideHeadSize != 1f)
			{
				EntityAlive entityAlive = entity as EntityAlive;
				if (entityAlive != null)
				{
					entityAlive.SetHeadSize(this.ecd.overrideHeadSize);
				}
			}
			entity.PostInit();
			this.entity = entity;
		}

		// Token: 0x04001C04 RID: 7172
		public Entity entity;

		// Token: 0x04001C05 RID: 7173
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityCreationData ecd;

		// Token: 0x04001C06 RID: 7174
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityClass ec;

		// Token: 0x04001C07 RID: 7175
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPlayer;

		// Token: 0x04001C08 RID: 7176
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isLocalPlayer;

		// Token: 0x04001C09 RID: 7177
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityInstanceAssets entityInstanceAssets;

		// Token: 0x04001C0A RID: 7178
		[PublicizedFrom(EAccessModifier.Private)]
		public EModelInstanceAssets eModelInstanceAssets;
	}
}
