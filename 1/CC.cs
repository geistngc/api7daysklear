using System;
using KinematicCharacterController;
using UnityEngine;

// Token: 0x02000501 RID: 1281
public class CC : ICharacterController
{
	// Token: 0x060029FA RID: 10746 RVA: 0x00108110 File Offset: 0x00106310
	public void Move()
	{
		this.collisionFlags = CollisionFlags.None;
		this.hadWallOverlap = false;
		this.tickCount++;
		this.motor.UpdatePhase1(0.05f);
		this.motor.UpdatePhase2(0.05f);
		this.motor.Transform.SetPositionAndRotation(this.motor.TransientPosition, this.motor.TransientRotation);
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000027FC File Offset: 0x000009FC
	public void BeforeCharacterUpdate(float deltaTime)
	{
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x00108180 File Offset: 0x00106380
	public bool OnCollisionOverlap(int nbOverlaps, Collider[] _colliders)
	{
		Vector3 position = this.motor.Transform.position;
		bool flag;
		do
		{
			flag = false;
			for (int i = 0; i < nbOverlaps - 1; i++)
			{
				Collider collider = _colliders[i];
				Collider collider2 = _colliders[i + 1];
				if (collider.gameObject.layer == 15)
				{
					if (collider2.gameObject.layer != 15)
					{
						_colliders[i] = collider2;
						_colliders[i + 1] = collider;
						flag = true;
					}
					else
					{
						float sqrMagnitude = (collider.transform.position - position).sqrMagnitude;
						if ((collider2.transform.position - position).sqrMagnitude < sqrMagnitude)
						{
							_colliders[i] = collider2;
							_colliders[i + 1] = collider;
							flag = true;
						}
					}
				}
			}
		}
		while (flag);
		if (_colliders[0].gameObject.layer != 15)
		{
			this.hadWallOverlap = true;
		}
		else if (this.hadWallOverlap)
		{
			return false;
		}
		return true;
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x00108260 File Offset: 0x00106460
	public float GetCollisionOverlapScale(Transform overlappedTransform)
	{
		if (overlappedTransform.gameObject.layer != 15)
		{
			return 1f;
		}
		if ((this.entity.entityId + this.tickCount & 15) != 0)
		{
			return 0.1f;
		}
		return 0.5f;
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000027FC File Offset: 0x000009FC
	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x0010829C File Offset: 0x0010649C
	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		if (this.vel.y <= 0.001f)
		{
			if (this.motor.GroundingStatus.IsStableOnGround)
			{
				Vector3 groundNormal = this.motor.GroundingStatus.GroundNormal;
				this.vel.y = 0f;
				float magnitude = this.vel.magnitude;
				Vector3 rhs = Vector3.Cross(this.vel, this.motor.CharacterUp);
				this.vel = Vector3.Cross(groundNormal, rhs).normalized * magnitude;
				this.vel = this.vel * 0.5f + currentVelocity * 0.5f;
			}
			else
			{
				this.vel = this.vel * 0.5f + currentVelocity * 0.5f;
			}
		}
		currentVelocity = this.vel;
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000027FC File Offset: 0x000009FC
	public void AfterCharacterUpdate(float deltaTime)
	{
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x0002003D File Offset: 0x0001E23D
	public bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000027FC File Offset: 0x000009FC
	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x000027FC File Offset: 0x000009FC
	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x00108396 File Offset: 0x00106596
	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		if (hitNormal.y >= 0.64f)
		{
			this.collisionFlags |= CollisionFlags.Below;
			return;
		}
		if (hitNormal.y > -0.5f)
		{
			this.collisionFlags |= CollisionFlags.Sides;
		}
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x000027FC File Offset: 0x000009FC
	public void PostGroundingUpdate(float deltaTime)
	{
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x000027FC File Offset: 0x000009FC
	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	// Token: 0x04001FE8 RID: 8168
	public Entity entity;

	// Token: 0x04001FE9 RID: 8169
	public KinematicCharacterMotor motor;

	// Token: 0x04001FEA RID: 8170
	public CollisionFlags collisionFlags;

	// Token: 0x04001FEB RID: 8171
	public Vector3 vel;

	// Token: 0x04001FEC RID: 8172
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickCount;

	// Token: 0x04001FED RID: 8173
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadWallOverlap;
}
