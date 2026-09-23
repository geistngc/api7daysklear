using System;
using System.Collections.Generic;
using System.Globalization;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200042C RID: 1068
[Preserve]
public class EAIApproachAndAttackTarget : EAIBase
{
	// Token: 0x060020D5 RID: 8405 RVA: 0x000C5B47 File Offset: 0x000C3D47
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 0.1f;
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000C5B64 File Offset: 0x000C3D64
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<EAIApproachAndAttackTarget.TargetClass>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 2)
			{
				EAIApproachAndAttackTarget.TargetClass targetClass = default(EAIApproachAndAttackTarget.TargetClass);
				targetClass.type = EntityFactory.GetEntityType(array[i]);
				targetClass.chaseTimeMax = 0f;
				if (i + 1 < array.Length)
				{
					targetClass.chaseTimeMax = StringParsers.ParseFloat(array[i + 1], 0, -1, NumberStyles.Any);
				}
				this.targetClasses.Add(targetClass);
				if (targetClass.type == typeof(EntityEnemyAnimal))
				{
					targetClass.type = typeof(EntityAnimalSnake);
					this.targetClasses.Add(targetClass);
				}
			}
		}
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000C5C38 File Offset: 0x000C3E38
	public void SetTargetOnlyPlayers()
	{
		this.targetClasses.Clear();
		EAIApproachAndAttackTarget.TargetClass item = default(EAIApproachAndAttackTarget.TargetClass);
		item.type = typeof(EntityPlayer);
		this.targetClasses.Add(item);
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000C5C78 File Offset: 0x000C3E78
	public override bool CanExecute()
	{
		if (this.theEntity.sleepingOrWakingUp || this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None || (this.theEntity.Jumping && !this.theEntity.isSwimming))
		{
			return false;
		}
		this.entityTarget = this.theEntity.GetAttackTarget();
		if (this.entityTarget == null)
		{
			return false;
		}
		Type type = this.entityTarget.GetType();
		for (int i = 0; i < this.targetClasses.Count; i++)
		{
			EAIApproachAndAttackTarget.TargetClass targetClass = this.targetClasses[i];
			if (targetClass.type != null && targetClass.type.IsAssignableFrom(type))
			{
				this.chaseTimeMax = targetClass.chaseTimeMax;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000C5D3C File Offset: 0x000C3F3C
	public override void Start()
	{
		this.entityTargetPos = this.entityTarget.position;
		this.entityTargetVel = Vector3.zero;
		this.isTargetToEat = this.entityTarget.IsDead();
		this.isEating = false;
		this.theEntity.IsEating = false;
		this.homeTimeout = (this.theEntity.IsSleeper ? 90f : this.chaseTimeMax);
		this.hasHome = (this.homeTimeout > 0f);
		this.isGoingHome = false;
		if (this.theEntity.ChaseReturnLocation == Vector3.zero)
		{
			this.theEntity.ChaseReturnLocation = (this.theEntity.IsSleeper ? this.theEntity.SleeperSpawnPosition : this.theEntity.position);
		}
		this.pathCounter = 0;
		this.relocateTicks = 0;
		this.attackTimeout = 5;
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x000C5E20 File Offset: 0x000C4020
	public override bool Continue()
	{
		if (this.theEntity.sleepingOrWakingUp || this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (this.isGoingHome)
		{
			return !attackTarget && this.theEntity.ChaseReturnLocation != Vector3.zero;
		}
		return attackTarget && !(attackTarget != this.entityTarget) && attackTarget.IsDead() == this.isTargetToEat;
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000C5EAD File Offset: 0x000C40AD
	public override void Reset()
	{
		this.theEntity.IsEating = false;
		this.theEntity.moveHelper.Stop();
		if (this.blockTargetTask != null)
		{
			this.blockTargetTask.canExecute = false;
		}
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000C5EE0 File Offset: 0x000C40E0
	public override void Update()
	{
		if (this.hasHome && !this.isTargetToEat)
		{
			if (this.isGoingHome)
			{
				Vector3 vector = this.theEntity.ChaseReturnLocation - this.theEntity.position;
				float y = vector.y;
				vector.y = 0f;
				if (vector.sqrMagnitude <= 0.16000001f && Utils.FastAbs(y) < 2f)
				{
					Vector3 chaseReturnLocation = this.theEntity.ChaseReturnLocation;
					chaseReturnLocation.y = this.theEntity.position.y;
					this.theEntity.SetPosition(chaseReturnLocation, true);
					this.theEntity.ChaseReturnLocation = Vector3.zero;
					if (this.theEntity.IsSleeper)
					{
						this.theEntity.ResumeSleeperPose();
					}
					return;
				}
				int num = this.pathCounter - 1;
				this.pathCounter = num;
				if (num <= 0 && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
				{
					this.pathCounter = 60;
					float moveSpeed = this.theEntity.GetMoveSpeedAggro() * 0.8f;
					this.theEntity.FindPath(this.theEntity.ChaseReturnLocation, moveSpeed, false, this);
				}
				return;
			}
			else
			{
				this.homeTimeout -= 0.05f;
				if (this.homeTimeout <= 0f)
				{
					if (this.blockTargetTask == null)
					{
						List<EAIBlockingTargetTask> targetTasks = this.manager.GetTargetTasks<EAIBlockingTargetTask>();
						if (targetTasks != null)
						{
							this.blockTargetTask = targetTasks[0];
						}
					}
					if (this.blockTargetTask != null)
					{
						this.blockTargetTask.canExecute = true;
					}
					this.theEntity.SetAttackTarget(null, 0);
					this.theEntity.SetLookPosition(Vector3.zero);
					this.theEntity.PlayGiveUpSound();
					this.pathCounter = 0;
					this.isGoingHome = true;
					return;
				}
			}
		}
		if (this.entityTarget == null)
		{
			return;
		}
		if (this.relocateTicks > 0)
		{
			if (!this.theEntity.navigator.noPathAndNotPlanningOne())
			{
				this.relocateTicks--;
				this.theEntity.moveHelper.SetFocusPos(this.entityTarget.position);
				return;
			}
			this.relocateTicks = 0;
		}
		Vector3 vector2 = this.entityTarget.position;
		if (this.isTargetToEat)
		{
			vector2 = this.entityTarget.getBellyPosition();
		}
		Vector3 a = vector2 - this.entityTargetPos;
		if (a.sqrMagnitude < 1f)
		{
			this.entityTargetVel = this.entityTargetVel * 0.7f + a * 0.3f;
		}
		this.entityTargetPos = vector2;
		this.attackTimeout--;
		if (this.isEating)
		{
			if (this.theEntity.bodyDamage.HasLimbs)
			{
				this.theEntity.RotateTo(vector2.x, vector2.y, vector2.z, 8f, 5f);
			}
			if (this.attackTimeout <= 0)
			{
				this.attackTimeout = 25 + base.GetRandom(10);
				if ((this.eatCount & 1) == 0)
				{
					this.theEntity.PlayOneShot("eat_player", false, false, false, null, 1f);
					this.entityTarget.DamageEntity(DamageSource.eat, 35, false, 1f);
				}
				Vector3 pos = new Vector3(0f, 0.04f, 0.08f);
				ParticleEffect pe = new ParticleEffect("blood_eat", pos, 1f, Color.white, null, this.theEntity.entityId, ParticleEffect.Attachment.Head);
				GameManager.Instance.SpawnParticleEffectServer(pe, this.theEntity.entityId, false, false);
				this.eatCount++;
			}
			return;
		}
		this.theEntity.moveHelper.CalcIfUnreachablePos();
		float num3;
		float num4;
		if (!this.isTargetToEat)
		{
			ItemValue holdingItemItemValue = this.theEntity.inventory.holdingItemItemValue;
			int num2 = 0;
			ItemAction itemAction = holdingItemItemValue.ItemClass.Actions[num2];
			num3 = 1.095f;
			if (itemAction != null)
			{
				num3 = itemAction.Range;
				if (num3 == 0f)
				{
					num3 = EffectManager.GetItemValue(PassiveEffects.MaxRange, holdingItemItemValue, 0f);
				}
			}
			num4 = num3;
			if (num3 >= 2.5f)
			{
				num4 -= 0.3f;
			}
			num4 = Utils.FastMax(0.7f, num4 - 0.35f);
		}
		else
		{
			num3 = this.theEntity.GetHeight() * 0.9f;
			num4 = num3 - 0.05f;
		}
		float num5 = num4 * num4;
		float num6 = 4f;
		if (this.theEntity.IsFeral)
		{
			num6 = 8f;
		}
		num6 = base.RandomFloat * num6;
		float targetXZDistanceSq = this.GetTargetXZDistanceSq(num6);
		float num7 = vector2.y - this.theEntity.position.y;
		float num8 = Utils.FastAbs(num7);
		bool flag = targetXZDistanceSq <= num5 && num8 < 1f;
		if (!flag)
		{
			if (num8 < 3f && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
			{
				PathEntity path = this.theEntity.navigator.getPath();
				if (path != null && path.NodeCountRemaining() <= 2)
				{
					this.pathCounter = 0;
				}
			}
			int num = this.pathCounter - 1;
			this.pathCounter = num;
			if (num <= 0 && this.theEntity.CanNavigatePath() && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
			{
				this.pathCounter = 6 + base.GetRandom(10);
				Vector3 moveToLocation = this.GetMoveToLocation(num4);
				if (moveToLocation.y - this.theEntity.position.y < -8f)
				{
					this.pathCounter += 40;
					if (base.RandomFloat < 0.2f)
					{
						this.seekPosOffset.x = this.seekPosOffset.x + (base.RandomFloat * 0.6f - 0.3f);
						this.seekPosOffset.y = this.seekPosOffset.y + (base.RandomFloat * 0.6f - 0.3f);
					}
					moveToLocation.x += this.seekPosOffset.x;
					moveToLocation.z += this.seekPosOffset.y;
				}
				else
				{
					float num9 = (moveToLocation - this.theEntity.position).magnitude - 5f;
					if (num9 > 0f)
					{
						if (num9 > 60f)
						{
							num9 = 60f;
						}
						this.pathCounter += (int)(num9 / 20f * 20f);
					}
				}
				this.theEntity.FindPath(moveToLocation, this.theEntity.GetMoveSpeedAggro(), true, this);
			}
		}
		if (this.theEntity.Climbing)
		{
			return;
		}
		bool flag2 = this.theEntity.CanSee(this.entityTarget);
		this.theEntity.SetLookPosition((flag2 && !this.theEntity.IsBreakingBlocks) ? this.entityTarget.getHeadPosition() : Vector3.zero);
		if (!flag)
		{
			if (this.theEntity.navigator.noPathAndNotPlanningOne() && this.pathCounter > 0 && num7 < 2.1f)
			{
				Vector3 moveToLocation2 = this.GetMoveToLocation(num4);
				this.theEntity.moveHelper.SetMoveTo(moveToLocation2, true);
			}
		}
		else
		{
			this.theEntity.moveHelper.Stop();
			this.pathCounter = 0;
		}
		float num10 = this.isTargetToEat ? num3 : (num3 - 0.1f);
		float num11 = num10 * num10;
		if (targetXZDistanceSq > num11)
		{
			return;
		}
		if (num7 < -1.25f || num7 - this.theEntity.GetHeight() > 0.95f)
		{
			return;
		}
		this.theEntity.IsBreakingBlocks = false;
		this.theEntity.IsBreakingDoors = false;
		if (this.theEntity.bodyDamage.HasLimbs && !this.theEntity.Electrocuted)
		{
			this.theEntity.RotateTo(vector2.x, vector2.y, vector2.z, 30f, 30f);
		}
		if (this.isTargetToEat)
		{
			this.isEating = true;
			this.theEntity.IsEating = true;
			this.attackTimeout = 20;
			this.eatCount = 0;
			return;
		}
		if (this.theEntity.GetDamagedTarget() == this.entityTarget || (this.entityTarget != null && this.entityTarget.GetDamagedTarget() == this.theEntity))
		{
			this.homeTimeout = (this.theEntity.IsSleeper ? 90f : this.chaseTimeMax);
			if (this.blockTargetTask != null)
			{
				this.blockTargetTask.canExecute = false;
			}
			this.theEntity.ClearDamagedTarget();
			if (this.entityTarget)
			{
				this.entityTarget.ClearDamagedTarget();
			}
		}
		if (this.attackTimeout > 0)
		{
			return;
		}
		if (this.manager.groupCircle > 0f)
		{
			Entity targetIfAttackedNow = this.theEntity.GetTargetIfAttackedNow();
			if (targetIfAttackedNow != this.entityTarget && (!this.entityTarget.AttachedToEntity || this.entityTarget.AttachedToEntity != targetIfAttackedNow))
			{
				if (targetIfAttackedNow != null)
				{
					this.relocateTicks = 46;
					Vector3 vector3 = (this.theEntity.position - vector2).normalized * (num10 + 1.1f);
					float num12 = base.RandomFloat * 28f + 18f;
					if (base.RandomFloat < 0.5f)
					{
						num12 = -num12;
					}
					vector3 = Quaternion.Euler(0f, num12, 0f) * vector3;
					Vector3 targetPos = vector2 + vector3;
					this.theEntity.FindPath(targetPos, this.theEntity.GetMoveSpeedAggro(), true, this);
				}
				return;
			}
		}
		this.theEntity.SleeperSupressLivingSounds = false;
		if (this.theEntity.Attack(false))
		{
			this.attackTimeout = this.theEntity.GetAttackTimeoutTicks();
			this.theEntity.Attack(true);
		}
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000C68B0 File Offset: 0x000C4AB0
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetTargetXZDistanceSq(float estimatedTicks)
	{
		Vector3 vector = this.entityTarget.position;
		vector += this.entityTargetVel * estimatedTicks;
		if (this.isTargetToEat)
		{
			vector = this.entityTarget.getBellyPosition();
		}
		Vector3 vector2 = this.theEntity.position + this.theEntity.motion * estimatedTicks - vector;
		vector2.y = 0f;
		return vector2.sqrMagnitude;
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000C692C File Offset: 0x000C4B2C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetMoveToLocation(float maxDist)
	{
		Vector3 vector = this.entityTarget.position;
		vector += this.entityTargetVel * 6f;
		if (this.isTargetToEat)
		{
			vector = this.entityTarget.getBellyPosition();
		}
		if (!this.entityTarget.onGround && !this.entityTarget.IsInElevator())
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(vector - Origin.position, Vector3.down, out raycastHit, 3.9f, 1082195968))
			{
				vector.y -= raycastHit.distance;
			}
			else
			{
				vector.y -= 3.9f;
			}
			maxDist *= 0.8f;
		}
		vector = this.entityTarget.world.FindSupportingBlockPos(vector);
		if (maxDist > 0f)
		{
			Vector3 vector2 = new Vector3(this.theEntity.position.x, vector.y, this.theEntity.position.z);
			Vector3 vector3 = vector - vector2;
			float magnitude = vector3.magnitude;
			if (magnitude < 3f)
			{
				if (magnitude <= maxDist)
				{
					float num = vector.y - this.theEntity.position.y;
					if (num < -3f || num > 1.5f)
					{
						return vector;
					}
					return vector2;
				}
				else
				{
					vector3 *= maxDist / magnitude;
					Vector3 vector4 = vector - vector3;
					vector4.y += 0.51f;
					Vector3i pos = World.worldToBlockPos(vector4);
					BlockValue block = this.entityTarget.world.GetBlock(pos);
					Block block2 = block.Block;
					if (block2.PathType <= 0)
					{
						RaycastHit raycastHit2;
						if (Physics.Raycast(vector4 - Origin.position, Vector3.down, out raycastHit2, 1.02f, 1082195968))
						{
							vector4.y = raycastHit2.point.y + Origin.position.y;
							return vector4;
						}
						if (block2.IsElevator((int)block.rotation))
						{
							vector4.y = vector.y;
							return vector4;
						}
					}
				}
			}
		}
		return vector;
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000C6B30 File Offset: 0x000C4D30
	public override string ToString()
	{
		ItemValue holdingItemItemValue = this.theEntity.inventory.holdingItemItemValue;
		ItemAction itemAction = holdingItemItemValue.ItemClass.Actions[0];
		float num = 1.095f;
		if (!this.isTargetToEat && itemAction != null)
		{
			num = itemAction.Range;
			if (num == 0f)
			{
				num = EffectManager.GetItemValue(PassiveEffects.MaxRange, holdingItemItemValue, 0f);
			}
		}
		float value = this.isTargetToEat ? num : (num - 0.1f);
		float targetXZDistanceSq = this.GetTargetXZDistanceSq(0f);
		return string.Format("{0}, {1}{2}{3}{4}{5} dist {6} rng {7} timeout {8}", new object[]
		{
			base.ToString(),
			this.entityTarget ? this.entityTarget.EntityName : "",
			this.theEntity.CanSee(this.entityTarget) ? "(see)" : "",
			this.theEntity.navigator.noPathAndNotPlanningOne() ? "(-path)" : (this.theEntity.navigator.noPath() ? "(!path)" : ""),
			this.isTargetToEat ? "(eat)" : "",
			this.isGoingHome ? "(home)" : "",
			Mathf.Sqrt(targetXZDistanceSq).ToCultureInvariantString("0.000"),
			value.ToCultureInvariantString("0.000"),
			this.homeTimeout.ToCultureInvariantString("0.00")
		});
	}

	// Token: 0x0400167A RID: 5754
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSleeperChaseTime = 90f;

	// Token: 0x0400167B RID: 5755
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAIApproachAndAttackTarget.TargetClass> targetClasses;

	// Token: 0x0400167C RID: 5756
	[PublicizedFrom(EAccessModifier.Private)]
	public float chaseTimeMax;

	// Token: 0x0400167D RID: 5757
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasHome;

	// Token: 0x0400167E RID: 5758
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isGoingHome;

	// Token: 0x0400167F RID: 5759
	[PublicizedFrom(EAccessModifier.Private)]
	public float homeTimeout;

	// Token: 0x04001680 RID: 5760
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x04001681 RID: 5761
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 entityTargetPos;

	// Token: 0x04001682 RID: 5762
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 entityTargetVel;

	// Token: 0x04001683 RID: 5763
	[PublicizedFrom(EAccessModifier.Private)]
	public int attackTimeout;

	// Token: 0x04001684 RID: 5764
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathCounter;

	// Token: 0x04001685 RID: 5765
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 seekPosOffset;

	// Token: 0x04001686 RID: 5766
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTargetToEat;

	// Token: 0x04001687 RID: 5767
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isEating;

	// Token: 0x04001688 RID: 5768
	[PublicizedFrom(EAccessModifier.Private)]
	public int eatCount;

	// Token: 0x04001689 RID: 5769
	[PublicizedFrom(EAccessModifier.Private)]
	public EAIBlockingTargetTask blockTargetTask;

	// Token: 0x0400168A RID: 5770
	[PublicizedFrom(EAccessModifier.Private)]
	public int relocateTicks;

	// Token: 0x0200042D RID: 1069
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TargetClass
	{
		// Token: 0x0400168B RID: 5771
		public Type type;

		// Token: 0x0400168C RID: 5772
		public float chaseTimeMax;
	}
}
