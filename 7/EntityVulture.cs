using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004F3 RID: 1267
[Preserve]
public class EntityVulture : EntityFlying
{
	// Token: 0x06002978 RID: 10616 RVA: 0x00104A54 File Offset: 0x00102C54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.35f, 0f);
			component.size = new Vector3(0.4f, 0.4f, 0.4f);
		}
		base.Awake();
		this.state = EntityVulture.State.WanderStart;
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x00104AB6 File Offset: 0x00102CB6
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		this.Init();
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x00104AC7 File Offset: 0x00102CC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Init()
	{
		if (this.navigator != null)
		{
			this.navigator.setCanDrown(true);
		}
		this.battleFatigueSeconds = this.rand.RandomRange(30f, 60f);
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x00104AF8 File Offset: 0x00102CF8
	public override void SetSleeper()
	{
		base.SetSleeper();
		this.sorter = new EAISetNearestEntityAsTargetSorter(this);
		base.setHomeArea(new Vector3i(this.position), (int)this.sleeperSightRange + 1);
		this.battleFatigueSeconds = float.MaxValue;
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x00104B34 File Offset: 0x00102D34
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving))
		{
			this.aiManager.UpdateDebugName();
			return;
		}
		if (GameStats.GetInt(EnumGameStats.GameState) == 2)
		{
			return;
		}
		base.CheckDespawn();
		base.GetEntitySenses().ClearIfExpired();
		if (this.IsSleeperPassive)
		{
			return;
		}
		if (this.IsSleeping)
		{
			float seeDistance = this.GetSeeDistance();
			this.world.GetEntitiesInBounds(typeof(EntityPlayer), BoundsUtils.ExpandBounds(this.boundingBox, seeDistance, seeDistance, seeDistance), EntityVulture.list);
			EntityVulture.list.Sort(this.sorter);
			EntityPlayer entityPlayer = null;
			float num = float.MaxValue;
			if (this.noisePlayer != null && this.noisePlayerVolume >= this.sleeperNoiseToWake)
			{
				entityPlayer = this.noisePlayer;
				num = this.noisePlayerDistance;
			}
			for (int i = 0; i < EntityVulture.list.Count; i++)
			{
				EntityPlayer entityPlayer2 = (EntityPlayer)EntityVulture.list[i];
				if (base.CanSee(entityPlayer2))
				{
					float distance = base.GetDistance(entityPlayer2);
					if (base.GetSleeperDisturbedLevel(distance, entityPlayer2.Stealth.lightLevel) >= 2 && distance < num)
					{
						entityPlayer = entityPlayer2;
						num = distance;
					}
				}
			}
			EntityVulture.list.Clear();
			if (entityPlayer == null)
			{
				return;
			}
			base.ConditionalTriggerSleeperWakeUp();
			base.SetAttackTarget(entityPlayer, 1200);
		}
		bool flag = this.Buffs.HasBuff("buffShocked");
		if (flag)
		{
			this.SetState(EntityVulture.State.Stun);
		}
		else
		{
			EntityAlive revengeTarget = base.GetRevengeTarget();
			if (revengeTarget)
			{
				this.battleDuration = 0f;
				this.isBattleFatigued = false;
				base.SetRevengeTarget(null);
				if (revengeTarget != this.attackTarget && (!this.attackTarget || this.rand.RandomFloat < 0.5f))
				{
					base.SetAttackTarget(revengeTarget, 1200);
				}
			}
			if (this.attackTarget != this.currentTarget)
			{
				this.currentTarget = this.attackTarget;
				if (this.currentTarget)
				{
					this.SetState(EntityVulture.State.Attack);
					this.waypoint = this.position;
					this.moveUpdateDelay = 0;
					this.homeCheckDelay = 400;
				}
				else
				{
					this.SetState(EntityVulture.State.AttackStop);
				}
			}
		}
		float sqrMagnitude = (this.waypoint - this.position).sqrMagnitude;
		this.stateTime += 0.05f;
		int num2;
		switch (this.state)
		{
		case EntityVulture.State.Attack:
			this.battleDuration += 0.05f;
			break;
		case EntityVulture.State.AttackReposition:
			if (sqrMagnitude < 2.25f || this.stateTime >= this.stateMaxTime)
			{
				this.SetState(EntityVulture.State.Attack);
				this.motion *= -0.2f;
				this.motion.y = 0f;
			}
			break;
		case EntityVulture.State.AttackStop:
			this.ClearTarget();
			this.SetState(EntityVulture.State.WanderStart);
			break;
		case EntityVulture.State.Home:
			if (sqrMagnitude < 4f || this.stateTime > 30f)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			else
			{
				num2 = this.homeSeekDelay - 1;
				this.homeSeekDelay = num2;
				if (num2 <= 0)
				{
					this.homeSeekDelay = 40;
					int minXZ = 10;
					if (this.stateTime > 20f)
					{
						minXZ = -20;
					}
					int maximumHomeDistance = base.getMaximumHomeDistance();
					Vector3 vector = RandomPositionGenerator.CalcTowards(this, minXZ, 30, maximumHomeDistance / 2, base.getHomePosition().position.ToVector3());
					if (!vector.Equals(Vector3.zero))
					{
						this.waypoint = vector;
						this.AdjustWaypoint();
					}
				}
			}
			break;
		case EntityVulture.State.Stun:
		{
			Animator componentInChildren = this.ModelTransform.GetComponentInChildren<Animator>();
			if (flag)
			{
				this.motion = this.rand.RandomOnUnitSphere * -0.075f;
				this.motion.y = this.motion.y + -0.060000002f;
				if (componentInChildren)
				{
					componentInChildren.enabled = false;
				}
				return;
			}
			if (componentInChildren)
			{
				componentInChildren.enabled = true;
			}
			this.SetState(EntityVulture.State.WanderStart);
			break;
		}
		case EntityVulture.State.WanderStart:
			this.homeCheckDelay = 60;
			if (!base.isWithinHomeDistanceCurrentPosition())
			{
				this.StartHome(base.getHomePosition().position.ToVector3());
			}
			else
			{
				this.SetState(EntityVulture.State.Wander);
				this.isCircling = (!this.IsSleeper && this.rand.RandomFloat < 0.4f);
				float num3 = this.position.y;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.position - Origin.position, Vector3.down, out raycastHit, 999f, 65536))
				{
					float num4 = this.rand.RandomRange(this.wanderHeightRange.x, this.wanderHeightRange.y);
					if (this.IsSleeper)
					{
						num4 *= 0.4f;
					}
					num3 += -raycastHit.distance + num4;
				}
				else
				{
					this.isCircling = false;
				}
				bool flag2 = false;
				EntityPlayer entityPlayer3 = null;
				if (!this.isBattleFatigued)
				{
					entityPlayer3 = this.world.GetClosestPlayerSeen(this, 80f, 1f);
					if (entityPlayer3 && base.GetDistanceSq(entityPlayer3) > 400f)
					{
						flag2 = true;
					}
				}
				if (this.isCircling)
				{
					this.wanderChangeDelay = 120;
					Vector3 right = base.transform.right;
					right.y = 0f;
					this.circleReverseScale = 1f;
					if (this.rand.RandomFloat < 0.5f)
					{
						this.circleReverseScale = -1f;
						right.x = -right.x;
						right.z = -right.z;
					}
					this.circleCenter = this.position + right * (3f + this.rand.RandomFloat * 7f);
					this.circleCenter.y = num3;
					if (flag2)
					{
						this.circleCenter.x = this.circleCenter.x * 0.6f + entityPlayer3.position.x * 0.4f;
						this.circleCenter.z = this.circleCenter.z * 0.6f + entityPlayer3.position.z * 0.4f;
					}
				}
				else
				{
					this.wanderChangeDelay = 400;
					this.waypoint = this.position;
					this.waypoint.x = this.waypoint.x + (this.rand.RandomFloat * 16f - 8f);
					this.waypoint.y = num3;
					this.waypoint.z = this.waypoint.z + (this.rand.RandomFloat * 16f - 8f);
					if (flag2)
					{
						this.waypoint.x = this.waypoint.x * 0.6f + entityPlayer3.position.x * 0.4f;
						this.waypoint.z = this.waypoint.z * 0.6f + entityPlayer3.position.z * 0.4f;
					}
					this.AdjustWaypoint();
				}
			}
			break;
		case EntityVulture.State.Wander:
			if (this.isBattleFatigued)
			{
				this.battleDuration -= 0.05f;
				if (this.battleDuration <= 0f)
				{
					this.isBattleFatigued = false;
				}
			}
			num2 = this.wanderChangeDelay - 1;
			this.wanderChangeDelay = num2;
			if (num2 <= 0)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			if (this.isCircling)
			{
				Vector3 vector2 = this.circleCenter - this.position;
				float x = vector2.x;
				vector2.x = -vector2.z * this.circleReverseScale;
				vector2.z = x * this.circleReverseScale;
				vector2.y = 0f;
				this.waypoint = this.position + vector2;
			}
			else if (sqrMagnitude < 1f)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			num2 = this.targetSwitchDelay - 1;
			this.targetSwitchDelay = num2;
			if (num2 <= 0)
			{
				this.targetSwitchDelay = 40;
				if (this.IsSleeper || this.rand.RandomFloat >= 0.5f)
				{
					EntityPlayer entityPlayer4 = this.FindTarget();
					if (entityPlayer4)
					{
						base.SetAttackTarget(entityPlayer4, 1200);
					}
				}
			}
			break;
		}
		if (this.state != EntityVulture.State.Home)
		{
			num2 = this.homeCheckDelay - 1;
			this.homeCheckDelay = num2;
			if (num2 <= 0)
			{
				this.homeCheckDelay = 60;
				if (!base.isWithinHomeDistanceCurrentPosition())
				{
					this.SetState(EntityVulture.State.AttackStop);
				}
			}
		}
		num2 = this.moveUpdateDelay - 1;
		this.moveUpdateDelay = num2;
		if (num2 <= 0)
		{
			this.moveUpdateDelay = 4 + this.rand.RandomRange(5);
			if (this.currentTarget && this.state == EntityVulture.State.Attack)
			{
				this.waypoint = this.currentTarget.getHeadPosition();
				this.waypoint.y = this.waypoint.y + -0.1f;
				if (this.currentTarget.AttachedToEntity)
				{
					this.waypoint += this.currentTarget.GetVelocityPerSecond() * 0.3f;
				}
				else
				{
					this.waypoint += this.currentTarget.GetVelocityPerSecond() * 0.1f;
				}
				Vector3 a = this.waypoint - this.position;
				a.y = 0f;
				a.Normalize();
				this.waypoint += a * -0.6f;
			}
			float num5;
			if (!this.IsCourseTraversable(this.waypoint, out num5))
			{
				this.waypoint.y = this.waypoint.y + 2f;
				if (this.state == EntityVulture.State.Attack)
				{
					if (this.rand.RandomFloat < 0.1f)
					{
						this.StartAttackReposition();
					}
				}
				else if (this.state != EntityVulture.State.Home && this.state != EntityVulture.State.AttackReposition)
				{
					this.SetState(EntityVulture.State.WanderStart);
				}
			}
		}
		Vector3 a2 = this.waypoint - this.position;
		float magnitude = a2.magnitude;
		Vector3 vector3 = a2 * (1f / magnitude);
		this.glidingPercent = 0f;
		if (vector3.y > 0.57f)
		{
			this.accel = 0.35f;
		}
		else if (vector3.y < -0.34f)
		{
			this.accel = 0.95f;
			this.glidingPercent = 1f;
		}
		else
		{
			this.accel = 0.55f;
			if (this.state == EntityVulture.State.Home || this.state == EntityVulture.State.Wander)
			{
				this.accel = 0.8f;
				if (this.isCircling)
				{
					this.glidingPercent = 1f;
				}
			}
		}
		if (this.attackDelay > 0)
		{
			this.glidingPercent = 0f;
		}
		if (this.currentTarget && this.currentTarget.AttachedToEntity && !this.ignoreTargetAttached)
		{
			if (this.IsBloodMoon && this.accel > 0.5f)
			{
				this.accel = 2.5f;
			}
			this.accel *= this.moveSpeedAggro;
		}
		else
		{
			this.accel *= this.moveSpeed;
		}
		this.motion = this.motion * 0.9f + vector3 * (this.accel * 0.1f);
		if (this.emodel.avatarController)
		{
			this.glidingCurrentPercent = Mathf.MoveTowards(this.glidingCurrentPercent, this.glidingPercent, 0.060000002f);
			this.emodel.avatarController.UpdateFloat("Gliding", this.glidingCurrentPercent, true);
		}
		if (this.attackDelay > 0)
		{
			this.attackDelay--;
		}
		if (this.attack2Delay > 0)
		{
			this.attack2Delay--;
		}
		float num6 = Mathf.Atan2(this.motion.x * this.motionReverseScale, this.motion.z * this.motionReverseScale) * 57.29578f;
		if (this.currentTarget)
		{
			num2 = this.targetSwitchDelay - 1;
			this.targetSwitchDelay = num2;
			if (num2 <= 0)
			{
				this.targetSwitchDelay = 60;
				if (this.state != EntityVulture.State.AttackStop)
				{
					EntityPlayer entityPlayer5 = this.FindTarget();
					if (entityPlayer5 && entityPlayer5 != this.attackTarget)
					{
						base.SetAttackTarget(entityPlayer5, 400);
					}
				}
				float num7 = this.currentTarget.AttachedToEntity ? 0.1f : 0.25f;
				if (this.state != EntityVulture.State.AttackReposition && this.rand.RandomFloat < num7)
				{
					this.StartAttackReposition();
				}
			}
		}
		if (this.currentTarget)
		{
			Vector3 vector4 = this.currentTarget.getHeadPosition();
			vector4 += this.currentTarget.GetVelocityPerSecond() * 0.1f;
			Vector3 vector5 = vector4 - this.position;
			float sqrMagnitude2 = vector5.sqrMagnitude;
			if ((sqrMagnitude2 > 6400f && !this.IsBloodMoon) || this.currentTarget.IsDead())
			{
				this.SetState(EntityVulture.State.AttackStop);
			}
			else if (this.state != EntityVulture.State.AttackReposition)
			{
				if (sqrMagnitude2 < 4f)
				{
					num6 = Mathf.Atan2(vector5.x, vector5.z) * 57.29578f;
				}
				if (this.attackDelay <= 0 && !this.isAttack2On)
				{
					if (sqrMagnitude2 < 0.80999994f && this.position.y >= this.currentTarget.position.y && this.position.y < vector4.y + 0.1f)
					{
						this.AttackAndAdjust(false);
					}
					else if (this.checkBlockedDelay > 0)
					{
						this.checkBlockedDelay--;
					}
					else
					{
						this.checkBlockedDelay = 6;
						Vector3 normalized = vector5.normalized;
						Ray ray = new Ray(this.position + new Vector3(0f, 0.22f, 0f) - normalized * 0.13f, normalized);
						if (Voxel.Raycast(this.world, ray, 0.83f, 1082195968, 128, 0.13f))
						{
							this.AttackAndAdjust(true);
						}
					}
				}
				bool flag3 = false;
				ItemActionVomit.ItemActionDataVomit itemActionDataVomit = this.inventory.holdingItemData.actionData[1] as ItemActionVomit.ItemActionDataVomit;
				if (itemActionDataVomit != null && this.attack2Delay <= 0 && sqrMagnitude2 >= 9f)
				{
					float range = ((ItemActionRanged)this.inventory.holdingItem.Actions[1]).GetRange(itemActionDataVomit);
					if (sqrMagnitude2 < range * range && Utils.FastAbs(Utils.DeltaAngle(num6, this.rotation.y)) < 20f && Utils.FastAbs(Vector3.SignedAngle(vector5, base.transform.forward, Vector3.right)) < 25f)
					{
						flag3 = true;
					}
				}
				if (!this.isAttack2On && flag3)
				{
					this.isAttack2On = true;
					itemActionDataVomit.muzzle = this.emodel.GetHeadTransform();
					itemActionDataVomit.numWarningsPlayed = 999;
				}
				if (this.isAttack2On)
				{
					if (!flag3)
					{
						this.isAttack2On = false;
					}
					else
					{
						this.motion *= 0.7f;
						base.SetLookPosition(vector4);
						this.UseHoldingItem(1, false);
						if (!itemActionDataVomit.isActive)
						{
							this.isAttack2On = false;
						}
					}
					if (!this.isAttack2On)
					{
						if (itemActionDataVomit.numVomits > 0)
						{
							this.StartAttackReposition();
						}
						this.UseHoldingItem(1, true);
						this.attack2Delay = 60;
						base.SetLookPosition(Vector3.zero);
					}
				}
			}
		}
		float magnitude2 = this.motion.magnitude;
		if (magnitude2 < 0.02f)
		{
			this.motion *= 1f / magnitude2 * 0.02f;
		}
		base.SeekYaw(num6, 0f, 20f);
		this.aiManager.UpdateDebugName();
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x00105B18 File Offset: 0x00103D18
	public override string MakeDebugNameInfo()
	{
		return string.Format("\n{0} {1}\nWaypoint {2}\nTarget {3}, AtkDelay {4}, BtlTime {5}\nSpeed {6}, Motion {7}, Accel {8}", new object[]
		{
			this.state.ToStringCached<EntityVulture.State>(),
			this.stateTime.ToCultureInvariantString("0.00"),
			this.waypoint.ToCultureInvariantString(),
			this.currentTarget ? this.currentTarget.name : "",
			this.attackDelay,
			this.battleDuration.ToCultureInvariantString("0.00"),
			this.motion.magnitude.ToCultureInvariantString("0.000"),
			this.motion.ToCultureInvariantString("0.000"),
			this.accel.ToCultureInvariantString("0.000")
		});
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x00105BE6 File Offset: 0x00103DE6
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetState(EntityVulture.State newState)
	{
		this.state = newState;
		this.stateTime = 0f;
		this.motionReverseScale = 1f;
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x00105C08 File Offset: 0x00103E08
	[PublicizedFrom(EAccessModifier.Private)]
	public void AdjustWaypoint()
	{
		int num = 255;
		Vector3i pos = new Vector3i(this.waypoint);
		while (!this.world.GetBlock(pos).isair && --num >= 0)
		{
			this.waypoint.y = this.waypoint.y + 1f;
			pos.y++;
		}
		this.waypoint.y = Mathf.Min(this.waypoint.y, 250f);
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x00105C88 File Offset: 0x00103E88
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartAttackReposition()
	{
		if (!this.IsBloodMoon && this.battleDuration >= this.battleFatigueSeconds)
		{
			this.ClearTarget();
			this.battleDuration = this.rand.RandomRange(80f, 180f);
			this.isBattleFatigued = true;
			this.SetState(EntityVulture.State.Wander);
			return;
		}
		this.SetState(EntityVulture.State.AttackReposition);
		this.stateMaxTime = this.rand.RandomRange(0.8f, 5f);
		this.attackCount = 0;
		this.waypoint = this.position;
		this.waypoint.x = this.waypoint.x + (this.rand.RandomFloat * 8f - 4f);
		this.waypoint.y = this.waypoint.y + (this.rand.RandomFloat * 4f + 3f);
		this.waypoint.z = this.waypoint.z + (this.rand.RandomFloat * 8f - 4f);
		this.moveUpdateDelay = 0;
		this.motion = -this.motion;
		if (this.rand.RandomFloat < 0.5f)
		{
			this.motionReverseScale = -1f;
			this.motion.y = 0.2f;
		}
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x00105DC7 File Offset: 0x00103FC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void StartHome(Vector3 _homePos)
	{
		this.SetState(EntityVulture.State.Home);
		this.homeSeekDelay = 0;
		this.waypoint = _homePos;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x00105DDE File Offset: 0x00103FDE
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearTarget()
	{
		base.SetAttackTarget(null, 0);
		base.SetRevengeTarget(null);
		this.currentTarget = null;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x00105DF8 File Offset: 0x00103FF8
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer FindTarget()
	{
		EntityPlayer entityPlayer;
		if (this.IsBloodMoon)
		{
			entityPlayer = this.world.GetClosestPlayerSeen(this, -1f, 0f);
			if (!entityPlayer)
			{
				entityPlayer = this.world.GetClosestPlayer(this, -1f, false);
			}
			return entityPlayer;
		}
		float lightMin = 26f;
		entityPlayer = this.world.GetClosestPlayerSeen(this, 80f, lightMin);
		if (!entityPlayer || entityPlayer.inWaterPercent >= 0.6f)
		{
			entityPlayer = this.noisePlayer;
		}
		if (entityPlayer)
		{
			if (this.isBattleFatigued)
			{
				return null;
			}
			float num = (float)entityPlayer.Health / entityPlayer.Stats.Health.ModifiedMax;
			if (this.IsSleeper || num <= this.targetAttackHealthPercent)
			{
				return entityPlayer;
			}
		}
		return null;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fallHitGround(float _v, Vector3 _fallMotion)
	{
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x00105EB5 File Offset: 0x001040B5
	public override float GetEyeHeight()
	{
		return 0.3f;
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000E9F3F File Offset: 0x000E813F
	public override Vector3 GetLookVector()
	{
		if (this.lookAtPosition.Equals(Vector3.zero))
		{
			return base.GetLookVector();
		}
		return this.lookAtPosition - this.getHeadPosition();
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x00105EBC File Offset: 0x001040BC
	public override bool CanDamageEntity(int _sourceEntityId)
	{
		Entity entity = this.world.GetEntity(_sourceEntityId);
		return !entity || entity.entityClass != this.entityClass;
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x00105EF0 File Offset: 0x001040F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void AttackAndAdjust(bool isBlock)
	{
		if (this.UseHoldingItem(0, false))
		{
			this.UseHoldingItem(0, true);
			this.attackDelay = 18;
			this.isCircling = false;
			if (this.currentTarget.AttachedToEntity)
			{
				this.motion *= 0.7f;
			}
			else
			{
				this.motion *= 0.6f;
			}
			int num = this.attackCount + 1;
			this.attackCount = num;
			if (num >= 5 || this.rand.RandomFloat < 0.25f)
			{
				this.StartAttackReposition();
			}
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x00105F90 File Offset: 0x00104190
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsCourseTraversable(Vector3 _pos, out float _distance)
	{
		float num = _pos.x - this.position.x;
		float num2 = _pos.y - this.position.y;
		float num3 = _pos.z - this.position.z;
		_distance = Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
		if (_distance < 1.5f)
		{
			return true;
		}
		num /= _distance;
		num2 /= _distance;
		num3 /= _distance;
		Bounds boundingBox = this.boundingBox;
		this.collBB.Clear();
		int num4 = 1;
		while ((float)num4 < _distance - 1f)
		{
			boundingBox.center += new Vector3(num, num2, num3);
			this.world.GetCollidingBounds(this, boundingBox, this.collBB);
			if (this.collBB.Count > 0)
			{
				return false;
			}
			num4++;
		}
		return true;
	}

	// Token: 0x04001F5A RID: 8026
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cFlyingMinimumSpeed = 0.02f;

	// Token: 0x04001F5B RID: 8027
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetDistanceClose = 0.9f;

	// Token: 0x04001F5C RID: 8028
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetDistanceMax = 80f;

	// Token: 0x04001F5D RID: 8029
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetAttackOffsetY = -0.1f;

	// Token: 0x04001F5E RID: 8030
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cVomitMinRange = 3f;

	// Token: 0x04001F5F RID: 8031
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cAttackDelay = 18;

	// Token: 0x04001F60 RID: 8032
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cCollisionMask = 1082195968;

	// Token: 0x04001F61 RID: 8033
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueMin = 30f;

	// Token: 0x04001F62 RID: 8034
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueMax = 60f;

	// Token: 0x04001F63 RID: 8035
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueCooldownMin = 80f;

	// Token: 0x04001F64 RID: 8036
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueCooldownMax = 180f;

	// Token: 0x04001F65 RID: 8037
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int moveUpdateDelay;

	// Token: 0x04001F66 RID: 8038
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float motionReverseScale = 1f;

	// Token: 0x04001F67 RID: 8039
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 waypoint;

	// Token: 0x04001F68 RID: 8040
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCircling;

	// Token: 0x04001F69 RID: 8041
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 circleCenter;

	// Token: 0x04001F6A RID: 8042
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float circleReverseScale;

	// Token: 0x04001F6B RID: 8043
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float glidingCurrentPercent;

	// Token: 0x04001F6C RID: 8044
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float glidingPercent;

	// Token: 0x04001F6D RID: 8045
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float accel;

	// Token: 0x04001F6E RID: 8046
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector2 wanderHeightRange = new Vector2(10f, 30f);

	// Token: 0x04001F6F RID: 8047
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive currentTarget;

	// Token: 0x04001F70 RID: 8048
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetAttackHealthPercent = 0.8f;

	// Token: 0x04001F71 RID: 8049
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool ignoreTargetAttached;

	// Token: 0x04001F72 RID: 8050
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int targetSwitchDelay;

	// Token: 0x04001F73 RID: 8051
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int homeCheckDelay;

	// Token: 0x04001F74 RID: 8052
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int homeSeekDelay;

	// Token: 0x04001F75 RID: 8053
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int wanderChangeDelay;

	// Token: 0x04001F76 RID: 8054
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int checkBlockedDelay;

	// Token: 0x04001F77 RID: 8055
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float battleDuration;

	// Token: 0x04001F78 RID: 8056
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float battleFatigueSeconds;

	// Token: 0x04001F79 RID: 8057
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isBattleFatigued;

	// Token: 0x04001F7A RID: 8058
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackDelay;

	// Token: 0x04001F7B RID: 8059
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackCount;

	// Token: 0x04001F7C RID: 8060
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attack2Delay;

	// Token: 0x04001F7D RID: 8061
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAttack2On;

	// Token: 0x04001F7E RID: 8062
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x04001F7F RID: 8063
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Entity> list = new List<Entity>();

	// Token: 0x04001F80 RID: 8064
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVulture.State state;

	// Token: 0x04001F81 RID: 8065
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateTime;

	// Token: 0x04001F82 RID: 8066
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateMaxTime;

	// Token: 0x04001F83 RID: 8067
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Bounds> collBB = new List<Bounds>();

	// Token: 0x020004F4 RID: 1268
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum State
	{
		// Token: 0x04001F85 RID: 8069
		Attack,
		// Token: 0x04001F86 RID: 8070
		AttackReposition,
		// Token: 0x04001F87 RID: 8071
		AttackStop,
		// Token: 0x04001F88 RID: 8072
		Home,
		// Token: 0x04001F89 RID: 8073
		Stun,
		// Token: 0x04001F8A RID: 8074
		WanderStart,
		// Token: 0x04001F8B RID: 8075
		Wander
	}
}
