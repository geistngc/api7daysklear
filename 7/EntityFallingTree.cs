using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x020004B0 RID: 1200
[Preserve]
public class EntityFallingTree : Entity
{
	// Token: 0x060025F6 RID: 9718 RVA: 0x000E8957 File Offset: 0x000E6B57
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.treeRB = base.GetComponent<Rigidbody>();
		this.treeRB.useGravity = !this.isEntityRemote;
		this.treeRB.isKinematic = this.isEntityRemote;
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000E8990 File Offset: 0x000E6B90
	public Vector3i GetBlockPos()
	{
		return this.treeBlockPos;
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000E8998 File Offset: 0x000E6B98
	public Vector3 GetFallTreeDir()
	{
		return this.fallTreeDir;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000E89A0 File Offset: 0x000E6BA0
	public void SetBlockPos(Vector3i _blockPos, Vector3 _fallTreeDir)
	{
		this.treeBlockPos = _blockPos;
		this.fallTreeDir = _fallTreeDir;
		if (!this.isEntityRemote)
		{
			base.SetAirBorne(true);
		}
		Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(this.treeBlockPos);
		if (chunk == null)
		{
			return;
		}
		this.treeBV = chunk.GetBlock(World.toBlock(_blockPos));
		if (DecoManager.Instance.IsEnabled && this.treeBV.Block.IsDistantDecoration)
		{
			this.treeTransform = DecoManager.Instance.GetDecorationTransform(this.treeBlockPos, true);
		}
		else
		{
			BlockEntityData blockEntity = chunk.GetBlockEntity(this.treeBlockPos);
			if (blockEntity != null && blockEntity.bHasTransform)
			{
				this.treeTransform = blockEntity.transform;
				blockEntity.transform = null;
				blockEntity.bHasTransform = false;
			}
		}
		this.collHeight = 3f;
		if (this.treeTransform)
		{
			foreach (Collider collider in this.treeTransform.GetComponentsInChildren<Collider>())
			{
				collider.enabled = false;
				CapsuleCollider capsuleCollider = collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					this.collHeight = Utils.FastMax(this.collHeight, capsuleCollider.height);
				}
			}
		}
		this.collHeight *= 0.9f;
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000E8AD1 File Offset: 0x000E6CD1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnCollisionEnter(Collision collision)
	{
		this.Collide(collision);
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000E8AD1 File Offset: 0x000E6CD1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnCollisionStay(Collision collision)
	{
		this.Collide(collision);
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000E8ADC File Offset: 0x000E6CDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Collide(Collision collision)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (collision.contactCount == 0)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		if (magnitude > 1f)
		{
			this.collidedWith(collision.gameObject.transform);
		}
		if (magnitude > 0.2f && collision.impulse.magnitude / this.treeRB.mass > 1.5f)
		{
			Vector3 a = base.transform.position;
			float num = -1f;
			for (int i = 0; i < collision.contactCount; i++)
			{
				ContactPoint contact = collision.GetContact(i);
				float magnitude2 = contact.impulse.magnitude;
				if (magnitude2 > num)
				{
					num = magnitude2;
					a = contact.point;
				}
			}
			Manager.BroadcastPlay(this, "treefallimpact", false, 1f);
			ParticleEffect pe = new ParticleEffect("treefall", a + Origin.position, base.transform.rotation * Quaternion.AngleAxis(90f, Vector3.forward), 1f, Color.white, null, null, 1f, "");
			GameManager.Instance.SpawnParticleEffectServer(pe, this.entityId, false, false);
		}
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000E8C14 File Offset: 0x000E6E14
	[PublicizedFrom(EAccessModifier.Private)]
	public void collidedWith(Transform _other)
	{
		if (this.timeToEnableDamage > 0f)
		{
			return;
		}
		Transform transform = _other;
		string tag = _other.tag;
		if (tag.StartsWith("E_BP_"))
		{
			transform = GameUtils.GetHitRootTransform(tag, transform);
			tag = transform.tag;
		}
		if (tag.StartsWith("E_"))
		{
			Entity component = transform.GetComponent<Entity>();
			if (component && !component.IsDead() && this.treeCanDamageEntity(component))
			{
				this.hitEntities.Add(component.entityId);
				int damage = (int)(this.treeRB.mass * 0.35999998f);
				base.StartCoroutine(this.onEntityDamageLater(component, damage));
			}
		}
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000E8CB4 File Offset: 0x000E6EB4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool treeCanDamageEntity(Entity _entity)
	{
		return !this.hitEntities.Contains(_entity.entityId) && !(_entity is EntityPlayer) && !(_entity is EntitySupplyCrate);
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000E8CE1 File Offset: 0x000E6EE1
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator onEntityDamageLater(Entity _entity, int _damage)
	{
		yield return new WaitForSeconds(0.05f);
		if (!_entity.IsDead() && _damage > 10)
		{
			_entity.DamageEntity(new DamageSource(EnumDamageSource.External, EnumDamageTypes.Crushing), _damage, false, 1f);
		}
		yield break;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000E8CF8 File Offset: 0x000E6EF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateMesh()
	{
		if (!this.isEntityRemote && this.world.GetBlock(this.treeBlockPos).type == this.treeBV.type)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
		if (this.treeBV.isair)
		{
			if (this.treeTransform)
			{
				UnityEngine.Object.Destroy(this.treeTransform.gameObject);
				this.treeTransform = null;
			}
			return;
		}
		Transform transform = base.transform;
		Vector3 position = this.treeTransform.position;
		this.SetPosition(position + Origin.position, true);
		this.SetRotation(this.treeTransform.eulerAngles);
		transform.SetPositionAndRotation(position, this.treeTransform.rotation);
		this.treeTransform.SetParent(transform, false);
		this.treeTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Transform transform2 = this.treeTransform.Find("rootBall");
		if (transform2)
		{
			transform2.gameObject.SetActive(true);
			transform2.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
		}
		this.treeRB.useGravity = !this.isEntityRemote;
		this.treeRB.isKinematic = this.isEntityRemote;
		this.treeRB.position = position;
		this.treeRB.rotation = this.treeTransform.rotation;
		Block block = this.treeBV.Block;
		BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
		float num = (blockShapeModelEntity != null) ? blockShapeModelEntity.modelOffset.y : 0f;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(transform.position + 3f * Vector3.up, Vector3.down), 0.25f, out raycastHit, 5f, -538750989))
		{
			num = transform.position.y - raycastHit.point.y;
		}
		transform.gameObject.layer = 23;
		CapsuleCollider component = transform.GetComponent<CapsuleCollider>();
		component.height = this.collHeight;
		component.center = new Vector3(0f, this.collHeight * 0.5f - num, 0f);
		component.enabled = true;
		this.treeRB.mass = (15f + 7f * this.collHeight) * 5f;
		this.treeRB.centerOfMass = new Vector3(0f, this.collHeight * 0.3f - num, 0f);
		if (!this.isEntityRemote)
		{
			this.treeRB.velocity = Vector3.zero;
			this.treeRB.angularVelocity = Vector3.zero;
			this.treeRB.solverIterations = 10;
			this.treeRB.solverVelocityIterations = 3;
			this.treeRB.AddForceAtPosition(this.fallTreeDir * ((80f + this.collHeight * 8f) * 5f), transform.position + Vector3.up * (this.collHeight * 0.65f - num), ForceMode.Impulse);
			block.SpawnDestroyParticleEffect(this.world, this.treeBV, this.treeBlockPos, this.world.GetLightBrightness(this.treeBlockPos), block.GetColorForSide(this.treeBV, BlockFace.Top), -1);
			this.lifetime = 3f;
			this.timeToEnableDamage = 1.5f;
		}
		this.rendererList.Clear();
		foreach (MeshRenderer item in transform.GetComponentsInChildren<MeshRenderer>())
		{
			this.rendererList.Add(item);
		}
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000E90A4 File Offset: 0x000E72A4
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.isEntityRemote)
		{
			return;
		}
		this.timeToEnableDamage -= 0.05f;
		if (this.lifetime > 0f)
		{
			this.lifetime -= 0.05f;
			return;
		}
		if (this.timeToRemoveTree < 0f)
		{
			if (this.treeRB.angularVelocity.sqrMagnitude < 0.1f && this.treeRB.velocity.sqrMagnitude < 0.1f)
			{
				this.timeToRemoveTree = 1f;
				this.targetFade = 0f;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<EntityFallingTree.NetPackageTreeFade>().Setup(this), false, -1, -1, -1, null, 192, false);
			}
		}
		else
		{
			this.timeToRemoveTree -= 0.05f;
			if (this.timeToRemoveTree < 0f)
			{
				this.DestroyTree();
			}
		}
		if (this.isMeshCreated && base.transform.position.y + Origin.position.y < 1f)
		{
			this.DestroyTree();
		}
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000E91C8 File Offset: 0x000E73C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyTree()
	{
		this.SetDead();
		if (!this.isEntityRemote && this.world.GetBlock(this.treeBlockPos).type == this.treeBV.type)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
		if (this.treeTransform != null)
		{
			UnityEngine.Object.Destroy(this.treeTransform.gameObject);
			this.treeTransform = null;
		}
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanCollideWith(Entity _other)
	{
		return false;
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000E924C File Offset: 0x000E744C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (!this.isMeshCreated && this.treeTransform)
		{
			this.isMeshCreated = true;
			this.CreateMesh();
		}
		float deltaTime = Time.deltaTime;
		this.fade = Mathf.MoveTowards(this.fade, this.targetFade, deltaTime);
		if (this.fade < 1f)
		{
			float num = this.fade;
			for (int i = 0; i < this.rendererList.Count; i++)
			{
				Renderer renderer = this.rendererList[i];
				if (renderer)
				{
					if (this.fade < 0.5f && renderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
					{
						renderer.gameObject.SetActive(false);
					}
					renderer.GetMaterials(this.mats);
					for (int j = 0; j < this.mats.Count; j++)
					{
						if (num < 1f)
						{
							this.mats[j].EnableKeyword("ENABLE_FADEOUT");
						}
						else
						{
							this.mats[j].DisableKeyword("ENABLE_FADEOUT");
						}
						this.mats[j].SetFloat("_FadeOut", num);
					}
				}
			}
			this.mats.Clear();
		}
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		if (this.isEntityRemote)
		{
			float t = deltaTime * 20f;
			vector = Vector3.Lerp(vector, this.targetPos - Origin.position, t);
			Quaternion rotation = Quaternion.Slerp(transform.rotation, this.targetQRot, t);
			transform.SetPositionAndRotation(vector, rotation);
			return;
		}
		this.SetPosition(vector + Origin.position, true);
		this.SetRotation(transform.eulerAngles);
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000E9406 File Offset: 0x000E7606
	public override void OnEntityUnload()
	{
		if (this.treeTransform != null)
		{
			UnityEngine.Object.Destroy(this.treeTransform.gameObject);
			this.treeTransform = null;
		}
		base.OnEntityUnload();
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000E9433 File Offset: 0x000E7633
	public override void MarkToUnload()
	{
		base.MarkToUnload();
		if (!this.isEntityRemote)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
	}

	// Token: 0x04001C42 RID: 7234
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMassScale = 5f;

	// Token: 0x04001C43 RID: 7235
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i treeBlockPos;

	// Token: 0x04001C44 RID: 7236
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue treeBV;

	// Token: 0x04001C45 RID: 7237
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallTreeDir;

	// Token: 0x04001C46 RID: 7238
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform treeTransform;

	// Token: 0x04001C47 RID: 7239
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody treeRB;

	// Token: 0x04001C48 RID: 7240
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<MeshRenderer> rendererList = new List<MeshRenderer>();

	// Token: 0x04001C49 RID: 7241
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float collHeight;

	// Token: 0x04001C4A RID: 7242
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMeshCreated;

	// Token: 0x04001C4B RID: 7243
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToRemoveTree = -1f;

	// Token: 0x04001C4C RID: 7244
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToEnableDamage;

	// Token: 0x04001C4D RID: 7245
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<int> hitEntities = new List<int>();

	// Token: 0x04001C4E RID: 7246
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fade = 1f;

	// Token: 0x04001C4F RID: 7247
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetFade = 1f;

	// Token: 0x04001C50 RID: 7248
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Material> mats = new List<Material>();

	// Token: 0x020004B1 RID: 1201
	[Preserve]
	public class NetPackageTreeFade : NetPackage
	{
		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x0600260A RID: 9738 RVA: 0x0002F184 File Offset: 0x0002D384
		public override NetPackageDirection PackageDirection
		{
			get
			{
				return NetPackageDirection.ToClient;
			}
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x000E94B5 File Offset: 0x000E76B5
		public EntityFallingTree.NetPackageTreeFade Setup(Entity entity)
		{
			this.entityId = entity.entityId;
			return this;
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000E94C4 File Offset: 0x000E76C4
		public override void read(PooledBinaryReader _reader)
		{
			this.entityId = _reader.ReadInt32();
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x000E94D2 File Offset: 0x000E76D2
		public override void write(PooledBinaryWriter _writer)
		{
			base.write(_writer);
			_writer.Write(this.entityId);
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x000E94E8 File Offset: 0x000E76E8
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			EntityFallingTree entityFallingTree = _world.GetEntity(this.entityId) as EntityFallingTree;
			if (entityFallingTree != null)
			{
				entityFallingTree.targetFade = 0f;
			}
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x00080864 File Offset: 0x0007EA64
		public override int GetLength()
		{
			return 4;
		}

		// Token: 0x04001C51 RID: 7249
		[PublicizedFrom(EAccessModifier.Private)]
		public int entityId;
	}
}
