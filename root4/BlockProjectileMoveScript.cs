using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class BlockProjectileMoveScript : ProjectileMoveScript
{
	// Token: 0x060007CD RID: 1997 RVA: 0x00037460 File Offset: 0x00035660
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void checkCollision()
	{
		GameManager instance = GameManager.Instance;
		if (!instance)
		{
			return;
		}
		if (instance.World == null)
		{
			return;
		}
		Vector3 vector;
		if (this.isOnIdealPos)
		{
			vector = base.transform.position;
		}
		else
		{
			vector = this.idealPosition;
		}
		Vector3 direction = vector - this.previousPosition;
		float magnitude = direction.magnitude;
		if (magnitude < 0.04f)
		{
			return;
		}
		Ray ray = new Ray(this.previousPosition, direction);
		this.previousPosition = vector;
		int layerId = 0;
		EntityAlive entityAlive = (EntityAlive)this.firingEntity;
		if (entityAlive != null && entityAlive.emodel != null)
		{
			layerId = entityAlive.GetModelLayer();
			entityAlive.SetModelLayer(2, false, null);
		}
		this.hitMask = 32;
		bool flag = Voxel.Raycast(instance.World, ray, magnitude, -538750989, this.hitMask, 0f);
		if (entityAlive != null && entityAlive.emodel != null)
		{
			entityAlive.SetModelLayer(layerId, false, null);
		}
		if (flag && (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag) || Voxel.voxelRayHitInfo.tag.StartsWith("E_")))
		{
			base.enabled = false;
			UnityEngine.Object.Destroy(base.transform.gameObject);
			Transform transform = Voxel.voxelRayHitInfo.transform;
			string text = null;
			if (Voxel.voxelRayHitInfo.tag.StartsWith("E_BP_"))
			{
				text = Voxel.voxelRayHitInfo.tag.Substring("E_BP_".Length).ToLower();
				transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
			}
			if (Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
			{
				Entity component = transform.GetComponent<Entity>();
				if (component == null)
				{
					return;
				}
				DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSource.External, EnumDamageTypes.Piercing, this.ProjectileOwnerID)
				{
					AttackingItem = this.itemValueProjectile,
					bIgnorePartyShare = true,
					bTrapKillXP = true
				};
				int strength = (int)this.GetProjectileDamageEntity();
				component.DamageEntity(damageSourceEntity, strength, false, 1f);
				if (this.itemActionProjectile.BuffActions != null)
				{
					EntityAlive entityAlive2 = component as EntityAlive;
					if (entityAlive2 != null)
					{
						string context = (text != null) ? GameUtils.GetChildTransformPath(entityAlive2.transform, Voxel.voxelRayHitInfo.transform) : null;
						ItemAction.ExecuteBuffActions(this.itemActionProjectile.BuffActions, -1, entityAlive2, false, damageSourceEntity.GetEntityDamageBodyPart(entityAlive2), context);
					}
				}
			}
			if (this.itemActionProjectile.Explosion.ParticleIndex > 0)
			{
				Vector3 vector2 = Voxel.voxelRayHitInfo.hit.pos - direction.normalized * 0.1f;
				Vector3i vector3i = World.worldToBlockPos(vector2);
				if (!instance.World.GetBlock(vector3i).isair)
				{
					BlockFace blockFace;
					vector3i = Voxel.OneVoxelStep(vector3i, vector2, -direction.normalized, out vector2, out blockFace);
				}
				instance.ExplosionServer(vector2, vector3i, Quaternion.identity, this.itemActionProjectile.Explosion, this.ProjectileOwnerID, 0f, false, this.itemValueProjectile);
			}
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0003776F File Offset: 0x0003596F
	public float GetProjectileDamageEntity()
	{
		return this.itemActionProjectile.GetDamageEntity(this.itemValueProjectile, null, 0);
	}
}
