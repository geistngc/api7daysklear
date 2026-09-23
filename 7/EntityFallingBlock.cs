using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x020004AE RID: 1198
[Preserve]
public class EntityFallingBlock : Entity
{
	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x060025CA RID: 9674 RVA: 0x0002F184 File Offset: 0x0002D384
	public override Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Instant;
		}
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000E6D08 File Offset: 0x000E4F08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.yOffset = 0.15f;
		Transform transform = base.transform;
		this.rigidBody = transform.GetComponent<Rigidbody>();
		if (this.isEntityRemote)
		{
			UnityEngine.Object.Destroy(this.rigidBody);
		}
		transform.GetComponent<BoxCollider>().enabled = false;
		transform.GetComponent<SphereCollider>().enabled = false;
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000E6D64 File Offset: 0x000E4F64
	public override void InitLocation(Vector3 _pos, Vector3 _rot)
	{
		base.InitLocation(_pos, _rot);
		Vector2i vector2i = World.toChunkXZ(_pos);
		this.chunkKey = WorldChunkCache.MakeChunkKey(vector2i.x, vector2i.y);
		if (!EntityFallingBlock.fallingBlocksByChunk.ContainsKey(this.chunkKey))
		{
			EntityFallingBlock.fallingBlocksByChunk[this.chunkKey] = new List<EntityFallingBlock>();
		}
		EntityFallingBlock.fallingBlocksByChunk[this.chunkKey].Add(this);
		if (!this.isEntityRemote)
		{
			this.rigidBody.position = _pos - Origin.position;
			this.rigidBody.rotation = Quaternion.Euler(_rot);
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000E6E03 File Offset: 0x000E5003
	public BlockValue GetBlockValue()
	{
		return this.blockValue;
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000E6E0C File Offset: 0x000E500C
	public void SetBlockValue(BlockValue _blockValue)
	{
		this.blockValue = _blockValue;
		this.isTerrain = this.blockValue.Block.shape.IsTerrain();
		if (this.isTerrain)
		{
			this.terrainScale = this.rand.RandomRange(0.3f, 0.98f);
			this.myCollider = base.transform.GetComponent<SphereCollider>();
			return;
		}
		this.myCollider = base.transform.GetComponent<BoxCollider>();
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000E6E81 File Offset: 0x000E5081
	public TextureFullArray GetTextureFull()
	{
		return this.textureFull;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000E6E89 File Offset: 0x000E5089
	public void SetTextureFull(TextureFullArray _textureFull)
	{
		this.textureFull = _textureFull;
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000E6E92 File Offset: 0x000E5092
	public void SetCanvasState(SignCanvas.CanvasState _state)
	{
		this.pendingCanvasState = _state;
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000E6E9B File Offset: 0x000E509B
	public void SetStartVelocity(Vector3 _vel, float _angularVel)
	{
		this.startVel = _vel;
		this.startAngularVel = _angularVel;
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000E6EAC File Offset: 0x000E50AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (!this.isMeshCreated && !GameManager.IsDedicatedServer)
		{
			this.CreateMesh();
			if (this.meshRenderer)
			{
				this.isMeshCreated = true;
				this.meshRenderer.enabled = true;
				if (this.isTerrain)
				{
					base.transform.localScale = new Vector3(this.terrainScale, this.terrainScale, this.terrainScale);
					TextureAtlasTerrain textureAtlasTerrain = (TextureAtlasTerrain)MeshDescription.meshes[5].textureAtlas;
					int sideTextureId = this.blockValue.Block.GetSideTextureId(this.blockValue, BlockFace.Top, 0);
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					materialPropertyBlock.SetTexture("_MainTex", textureAtlasTerrain.diffuse[sideTextureId]);
					materialPropertyBlock.SetTexture("_BumpMap", textureAtlasTerrain.normal[sideTextureId]);
					this.meshRenderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
		}
		if (this.myCollider && !this.myCollider.enabled)
		{
			this.myCollider.enabled = true;
			Block block = this.blockValue.Block;
			this.massKg = Utils.FastMin(block.blockMaterial.Hardness.Value * (float)block.blockMaterial.Mass.Value, 10f) * 8f;
			this.massKg *= ((!this.isTerrain) ? (block.isMultiBlock ? 2.2f : 1f) : (this.terrainScale * this.terrainScale * 1.5f));
			if (!this.isEntityRemote)
			{
				this.rigidBody.mass = Utils.FastMax(10f, this.massKg);
				this.rigidBody.velocity = this.startVel;
				this.rigidBody.angularVelocity = this.rand.RandomOnUnitSphere * this.startAngularVel;
			}
		}
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000E7090 File Offset: 0x000E5290
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.IsDead())
		{
			return;
		}
		this.fallTimeInTicks++;
		Block block = this.blockValue.Block;
		if (this.fallTimeInTicks == 1 && Time.time - EntityFallingBlock.lastTimeStartParticleSpawned > 0.2f)
		{
			EntityFallingBlock.lastTimeStartParticleSpawned = Time.time;
			if (!GameManager.IsDedicatedServer && this.prefabParticleOnFallT)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.prefabParticleOnFallT.gameObject, base.transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Emit(10);
			}
		}
		if (this.isEntityRemote)
		{
			return;
		}
		Vector3 velocity = this.rigidBody.velocity;
		if ((this.fallTimeInTicks & 1) == 0)
		{
			List<Entity> entitiesInBounds = this.world.GetEntitiesInBounds(this, BoundsUtils.ExpandBounds(BoundsUtils.ExpandDirectional(this.boundingBox, this.motion), 0f, 0.2f, 0f));
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				Entity entity = entitiesInBounds[i];
				int entityId = entity.entityId;
				int num;
				this.entityHits.TryGetValue(entityId, out num);
				if (num < 3 && entity.CanCollideWith(this) && this.position.y >= entity.getHeadPosition().y && velocity.y < -0.8f)
				{
					float num2 = (float)((int)Utils.FastMin(this.massKg * velocity.y * -0.05f, 40f));
					num2 = EffectManager.GetValue(PassiveEffects.FallingBlockDamage, null, num2, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					int num3 = (int)num2;
					entity.DamageEntity(DamageSource.fallingBlock, num3, false, 1f);
					if (num3 >= 1)
					{
						num++;
						this.entityHits[entityId] = num;
					}
					Log.Warning("{0} EntityFallingBlock {1} hit {2}, vel {3}, for {4}", new object[]
					{
						GameManager.frameCount,
						this,
						entity,
						velocity,
						num3
					});
				}
			}
		}
		bool flag = false;
		Transform transform = base.transform;
		if (this.fallTimeInTicks < 60 || velocity.sqrMagnitude > 0.0625f)
		{
			this.notMovingCount = 0;
		}
		else
		{
			int num4 = this.notMovingCount + 1;
			this.notMovingCount = num4;
			if (num4 > 3)
			{
				Vector3i pos = World.worldToBlockPos(this.position + Vector3.down);
				BlockValue block2 = this.world.GetBlock(pos);
				if (!block2.isair && this.world.GetStability(pos) > 0)
				{
					float time = Time.time;
					if (time - EntityFallingBlock.lastTimeEndParticleSpawned > 0.15f)
					{
						EntityFallingBlock.lastTimeEndParticleSpawned = time;
						Block block3 = block;
						string destroyParticle = block3.GetDestroyParticle(this.blockValue);
						if (destroyParticle != null && block3.blockMaterial.SurfaceCategory != null)
						{
							Vector3i blockPos = World.worldToBlockPos(transform.position + new Vector3(0f, 0.5f, 0f) + Origin.position);
							this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("blockdestroy_" + destroyParticle, World.blockToTransformPos(blockPos), this.world.GetLightBrightness(blockPos), block3.GetColorForSide(this.blockValue, BlockFace.Top), block3.blockMaterial.SurfaceCategory + "destroy", null, false, 1f, ""), this.entityId, false, false);
						}
					}
					if (GamePrefs.GetBool(EnumGamePrefs.OptionsStabSpawnBlocksOnGround) && (!this.isTerrain || block2.Block.shape.IsTerrain()))
					{
						if (block.HasItemsToDropForEvent(EnumDropEvent.Fall))
						{
							float overallProb = 1f;
							List<Block.SItemDropProb> list = block.itemsToDrop[EnumDropEvent.Fall];
							if (list.Count > 0)
							{
								overallProb = list[0].prob;
							}
							block.DropItemsOnEvent(this.world, this.blockValue, EnumDropEvent.Fall, overallProb, base.GetPosition(), new Vector3(1.5f, 0f, 1.5f), Constants.cItemExplosionLifetime, -1, false);
						}
						else if (this.fallTimeInTicks < 16)
						{
							block.DropItemsOnEvent(this.world, this.blockValue, EnumDropEvent.Destroy, 0.7f, base.GetPosition(), new Vector3(1.5f, 0f, 1.5f), Constants.cItemExplosionLifetime, -1, false);
						}
					}
				}
				flag = true;
			}
		}
		if (this.fallTimeInTicks > 300)
		{
			flag = true;
		}
		if (transform.position.y + Origin.position.y < 2f)
		{
			flag = true;
		}
		if (flag)
		{
			this.SetDead();
		}
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000E7543 File Offset: 0x000E5743
	public override void SetDead()
	{
		SignCanvas signCanvas = this.signCanvas;
		if (signCanvas != null)
		{
			signCanvas.Cleanup();
		}
		this.signCanvas = null;
		EntityFallingBlock.fallingBlocksByChunk[this.chunkKey].Remove(this);
		base.SetDead();
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000E757C File Offset: 0x000E577C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateMesh()
	{
		int num = this.blockValue.ToItemType();
		ItemClass forId = ItemClass.GetForId(num);
		if (num == 0 || forId == null)
		{
			Log.Error("EntityFallingBlock failed id {0}, type", new object[]
			{
				num
			});
			this.SetDead();
			return;
		}
		Transform transform = base.transform;
		Transform transform2 = null;
		if (this.isTerrain)
		{
			GameObject gameObject = DataLoader.LoadAsset<GameObject>("@:Entities/Debris/Falling/Terrain1.prefab", false);
			if (gameObject)
			{
				transform2 = UnityEngine.Object.Instantiate<GameObject>(gameObject).transform;
				transform2.SetParent(transform, false);
				transform2.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
		}
		else
		{
			transform2 = forId.CloneModel(this.world, this.blockValue.ToItemValue(), this.position, transform, BlockShape.MeshPurpose.Local, this.textureFull);
		}
		if (!transform2)
		{
			Log.Warning("EntityFallingBlock failed id {0}, mesh", new object[]
			{
				num
			});
			this.SetDead();
			return;
		}
		transform2.rotation = this.blockValue.Block.shape.GetRotation(this.blockValue);
		this.meshRenderer = transform2.GetComponentInChildren<Renderer>();
		if (this.isTerrain)
		{
			return;
		}
		if (this.meshRenderer)
		{
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		}
		Collider[] componentsInChildren = transform2.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		Animator[] componentsInChildren2 = transform2.GetComponentsInChildren<Animator>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
		Utils.SetColliderLayerRecursively(transform.gameObject, 13);
		if (this.pendingCanvasState != null)
		{
			this.signCanvas = transform2.GetComponentInChildren<SignCanvas>(true);
			if (this.signCanvas != null)
			{
				this.signCanvas.State = this.pendingCanvasState;
				this.signCanvas.Initialize(null);
			}
		}
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000E7746 File Offset: 0x000E5946
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = (_distanceSqr < (float)(_masterIsZooming ? 14400 : 10000));
		}
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000E7773 File Offset: 0x000E5973
	public override bool CanCollideWith(Entity _other)
	{
		return _other is EntityAlive;
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x000E7780 File Offset: 0x000E5980
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		Transform transform = base.transform;
		if (this.isEntityRemote)
		{
			if (this.targetPos.sqrMagnitude > 0f && (this.targetPos - this.interpEndPos).sqrMagnitude > 0.0001f)
			{
				this.interpStartPos = transform.position + Origin.position;
				this.interpEndPos = this.targetPos;
				this.interpTime = 0.1f;
			}
			Vector3 a = this.targetPos;
			float deltaTime = Time.deltaTime;
			if (this.interpTime > 0f)
			{
				this.interpTime -= deltaTime;
				float t = 1f - this.interpTime / 0.1f;
				a = Vector3.Lerp(this.interpStartPos, this.interpEndPos, t);
			}
			Quaternion rotation = Quaternion.Slerp(transform.rotation, this.qrotation, deltaTime * 20f);
			transform.SetPositionAndRotation(a - Origin.position, rotation);
			return;
		}
		this.SetPosition(transform.position + Origin.position, true);
		this.SetRotation(transform.eulerAngles);
		this.qrotation = transform.rotation;
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000E78AC File Offset: 0x000E5AAC
	public void OnContactEvent()
	{
		if (this.isGroundHit || this.isEntityRemote)
		{
			return;
		}
		Vector3i blockPosition = base.GetBlockPosition();
		blockPosition.y--;
		BlockValue block = this.world.GetBlock(blockPosition);
		if (block.isair)
		{
			return;
		}
		this.isGroundHit = true;
		float lightBrightness = this.world.GetLightBrightness(blockPosition);
		Color colorForSide = block.Block.GetColorForSide(block, BlockFace.Top);
		string name = "impact_stone_on_" + block.Block.blockMaterial.SurfaceCategory;
		this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(name, base.GetPosition(), Quaternion.identity, lightBrightness, colorForSide, this.blockValue.Block.blockMaterial.SurfaceCategory + "hit" + block.Block.blockMaterial.SurfaceCategory, null, 1f, ""), this.entityId, false, false);
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000E799B File Offset: 0x000E5B9B
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		if (!this.isTerrain && this.meshRenderer)
		{
			UnityEngine.Object.Destroy(this.meshRenderer.material);
		}
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000E79C8 File Offset: 0x000E5BC8
	public static void ClearFallingBlocksForChunks(HashSetLong chunks)
	{
		List<EntityFallingBlock> list = new List<EntityFallingBlock>();
		foreach (long key in chunks)
		{
			if (EntityFallingBlock.fallingBlocksByChunk.ContainsKey(key))
			{
				foreach (EntityFallingBlock item in EntityFallingBlock.fallingBlocksByChunk[key])
				{
					list.Add(item);
				}
			}
		}
		foreach (EntityFallingBlock entityFallingBlock in list)
		{
			entityFallingBlock.SetDead();
		}
	}

	// Token: 0x04001C0B RID: 7179
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMaxHitsPerEntity = 3;

	// Token: 0x04001C0C RID: 7180
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMinDamageToCountHit = 1;

	// Token: 0x04001C0D RID: 7181
	public Transform prefabParticleOnFallT;

	// Token: 0x04001C0E RID: 7182
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer meshRenderer;

	// Token: 0x04001C0F RID: 7183
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x04001C10 RID: 7184
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue blockValue;

	// Token: 0x04001C11 RID: 7185
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isTerrain;

	// Token: 0x04001C12 RID: 7186
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float terrainScale;

	// Token: 0x04001C13 RID: 7187
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float massKg;

	// Token: 0x04001C14 RID: 7188
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TextureFullArray textureFull;

	// Token: 0x04001C15 RID: 7189
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMeshCreated;

	// Token: 0x04001C16 RID: 7190
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SignCanvas.CanvasState pendingCanvasState;

	// Token: 0x04001C17 RID: 7191
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SignCanvas signCanvas;

	// Token: 0x04001C18 RID: 7192
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int fallTimeInTicks;

	// Token: 0x04001C19 RID: 7193
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<int, int> entityHits = new Dictionary<int, int>(8);

	// Token: 0x04001C1A RID: 7194
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float lastTimeStartParticleSpawned;

	// Token: 0x04001C1B RID: 7195
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float lastTimeEndParticleSpawned;

	// Token: 0x04001C1C RID: 7196
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int notMovingCount;

	// Token: 0x04001C1D RID: 7197
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGroundHit;

	// Token: 0x04001C1E RID: 7198
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Collider myCollider;

	// Token: 0x04001C1F RID: 7199
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public long chunkKey;

	// Token: 0x04001C20 RID: 7200
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Dictionary<long, List<EntityFallingBlock>> fallingBlocksByChunk = new Dictionary<long, List<EntityFallingBlock>>();

	// Token: 0x04001C21 RID: 7201
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 startVel;

	// Token: 0x04001C22 RID: 7202
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startAngularVel = 0.5f;

	// Token: 0x04001C23 RID: 7203
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 interpStartPos;

	// Token: 0x04001C24 RID: 7204
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 interpEndPos;

	// Token: 0x04001C25 RID: 7205
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float interpTime;
}
