using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public abstract class AvatarController : MonoBehaviour
{
	// Token: 0x0600042A RID: 1066 RVA: 0x0001F8AF File Offset: 0x0001DAAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		AvatarController.StaticInit();
		this.entity = base.GetComponent<EntityAlive>();
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x0001F8C4 File Offset: 0x0001DAC4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void StaticInit()
	{
		if (AvatarController.initialized)
		{
			return;
		}
		AvatarController.initialized = true;
		AvatarController.hashNames = new Dictionary<int, string>();
		AvatarController.AssignAnimatorHash(ref AvatarController.attackHash, "Attack");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackBlendHash, "AttackBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackStartHash, "AttackStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackReadyHash, "AttackReady");
		AvatarController.AssignAnimatorHash(ref AvatarController.meleeAttackSpeedHash, "MeleeAttackSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.deathHash, "Death");
		AvatarController.AssignAnimatorHash(ref AvatarController.digHash, "Dig");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitStartHash, "HitStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitHash, "Hit");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpHash, "Jump");
		AvatarController.AssignAnimatorHash(ref AvatarController.moveHash, "Move");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunHash, "Stun");
		AvatarController.AssignAnimatorHash(ref AvatarController.readyToFireHash, "readyToFire");
		AvatarController.AssignAnimatorHash(ref AvatarController.beginCorpseEatHash, "BeginCorpseEat");
		AvatarController.AssignAnimatorHash(ref AvatarController.endCorpseEatHash, "EndCorpseEat");
		AvatarController.AssignAnimatorHash(ref AvatarController.forwardHash, "Forward");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitBodyPartHash, "HitBodyPart");
		AvatarController.AssignAnimatorHash(ref AvatarController.idleTimeHash, "IdleTime");
		AvatarController.AssignAnimatorHash(ref AvatarController.aimPitchHash, "AimPitch");
		AvatarController.AssignAnimatorHash(ref AvatarController.aimYawHash, "AimYaw");
		AvatarController.AssignAnimatorHash(ref AvatarController.isAimingHash, "IsAiming");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemUseHash, "ItemUse");
		AvatarController.AssignAnimatorHash(ref AvatarController.movementStateHash, "MovementState");
		AvatarController.AssignAnimatorHash(ref AvatarController.rotationPitchHash, "RotationPitch");
		AvatarController.AssignAnimatorHash(ref AvatarController.strafeHash, "Strafe");
		AvatarController.AssignAnimatorHash(ref AvatarController.swimSelectHash, "SwimSelect");
		AvatarController.AssignAnimatorHash(ref AvatarController.turnRateHash, "TurnRate");
		AvatarController.AssignAnimatorHash(ref AvatarController.walkTypeHash, "WalkType");
		AvatarController.AssignAnimatorHash(ref AvatarController.walkTypeBlendHash, "WalkTypeBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponCarryHash, "WeaponCarry");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponHoldTypeHash, "WeaponHoldType");
		AvatarController.AssignAnimatorHash(ref AvatarController.isAliveHash, "IsAlive");
		AvatarController.AssignAnimatorHash(ref AvatarController.isDeadHash, "IsDead");
		AvatarController.AssignAnimatorHash(ref AvatarController.isFPVHash, "IsFPV");
		AvatarController.AssignAnimatorHash(ref AvatarController.isMovingHash, "IsMoving");
		AvatarController.AssignAnimatorHash(ref AvatarController.isSwimHash, "IsSwim");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackTriggerHash, "AttackTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.deathTriggerHash, "DeathTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitTriggerHash, "HitTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.movementTriggerHash, "MovementTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.electrocuteTriggerHash, "ElectrocuteTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.painTriggerHash, "PainTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemHasChangedTriggerHash, "ItemHasChangedTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemThrownAwayTriggerHash, "ItemThrownAwayTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.dodgeBlendHash, "DodgeBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.dodgeTriggerHash, "DodgeTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.reactionTriggerHash, "ReactionTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.reactionTypeHash, "ReactionType");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperPoseHash, "SleeperPose");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperTriggerHash, "SleeperTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandResponseHash, "JumpLandResponse");
		AvatarController.AssignAnimatorHash(ref AvatarController.forcedRootMotionHash, "ForcedRootMotion");
		AvatarController.AssignAnimatorHash(ref AvatarController.preventAttackHash, "PreventAttack");
		AvatarController.AssignAnimatorHash(ref AvatarController.canFallHash, "CanFall");
		AvatarController.AssignAnimatorHash(ref AvatarController.isOnGroundHash, "IsOnGround");
		AvatarController.AssignAnimatorHash(ref AvatarController.triggerAliveHash, "TriggerAlive");
		AvatarController.AssignAnimatorHash(ref AvatarController.bodyPartHitHash, "BodyPartHit");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitDirectionHash, "HitDirection");
		AvatarController.AssignAnimatorHash(ref AvatarController.criticalHitHash, "CriticalHit");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitDamageHash, "HitDamage");
		AvatarController.AssignAnimatorHash(ref AvatarController.randomHash, "Random");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpStartHash, "JumpStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandHash, "JumpLand");
		AvatarController.AssignAnimatorHash(ref AvatarController.isMaleHash, "IsMale");
		AvatarController.AssignAnimatorHash(ref AvatarController.specialAttack2Hash, "SpecialAttack2");
		AvatarController.AssignAnimatorHash(ref AvatarController.rageHash, "Rage");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunTypeHash, "StunType");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunBodyPartHash, "StunBodyPart");
		AvatarController.AssignAnimatorHash(ref AvatarController.isCriticalHash, "isCritical");
		AvatarController.AssignAnimatorHash(ref AvatarController.HitRandomValueHash, "HitRandomValue");
		AvatarController.AssignAnimatorHash(ref AvatarController.beginStunTriggerHash, "BeginStunTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.endStunTriggerHash, "EndStunTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.toCrawlerTriggerHash, "ToCrawlerTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.isElectrocutedHash, "IsElectrocuted");
		AvatarController.AssignAnimatorHash(ref AvatarController.isClimbingHash, "IsClimbing");
		AvatarController.AssignAnimatorHash(ref AvatarController.verticalSpeedHash, "VerticalSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.reviveHash, "Revive");
		AvatarController.AssignAnimatorHash(ref AvatarController.harvestingHash, "Harvesting");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponFireHash, "WeaponFire");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponPreFireCancelHash, "WeaponPreFireCancel");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponPreFireHash, "WeaponPreFire");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponAmmoRemaining, "WeaponAmmoRemaining");
		AvatarController.AssignAnimatorHash(ref AvatarController.useItemHash, "UseItem");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemActionIndexHash, "ItemActionIndex");
		AvatarController.AssignAnimatorHash(ref AvatarController.isCrouchingHash, "IsCrouching");
		AvatarController.AssignAnimatorHash(ref AvatarController.reloadHash, "Reload");
		AvatarController.AssignAnimatorHash(ref AvatarController.reloadSpeedHash, "ReloadSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpTriggerHash, "JumpTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.inAirHash, "InAir");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandHash, "JumpLand");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitRandomValueHash, "HitRandomValue");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitTriggerHash, "HitTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.CoverModeHash, "CoverMode");
		AvatarController.AssignAnimatorHash(ref AvatarController.CoverTypeHash, "CoverType");
		AvatarController.AssignAnimatorHash(ref AvatarController.CoverEngageHash, "CoverEngage");
		AvatarController.AssignAnimatorHash(ref AvatarController.CoverHeightHash, "CoverHeight");
		AvatarController.AssignAnimatorHash(ref AvatarController.archetypeStanceHash, "ArchetypeStance");
		AvatarController.AssignAnimatorHash(ref AvatarController.yLookHash, "YLook");
		AvatarController.AssignAnimatorHash(ref AvatarController.vehiclePoseHash, "VehiclePose");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleBackHash, "SleeperIdleBack");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSideLeftHash, "SleeperIdleSideLeft");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSideRightHash, "SleeperIdleSideRight");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSitHash, "SleeperIdleSit");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleStandHash, "SleeperIdleStand");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleStomachHash, "SleeperIdleStomach");
		AvatarController.AssignAnimatorHash(ref AvatarController.holsteredHash, "Holstered");
		AvatarController.AssignAnimatorHash(ref AvatarController.stressLevel, "ChickenStress");
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x0001FF2E File Offset: 0x0001E12E
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void AssignAnimatorHash(ref int hash, string parameterName)
	{
		hash = Animator.StringToHash(parameterName);
		if (!AvatarController.hashNames.ContainsKey(hash))
		{
			AvatarController.hashNames.Add(hash, parameterName);
		}
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void assignStates()
	{
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001FF53 File Offset: 0x0001E153
	public virtual Animator GetAnimator()
	{
		return this.anim;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x0001FF5B File Offset: 0x0001E15B
	public void SetAnimator(Transform _animT)
	{
		this.SetAnimator(_animT.GetComponent<Animator>());
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0001FF6C File Offset: 0x0001E16C
	public void SetAnimator(Animator _anim)
	{
		if (this.anim == _anim)
		{
			return;
		}
		this.anim = _anim;
		if (this.anim)
		{
			this.anim.logWarnings = false;
			AnimatorControllerParameter[] parameters = this.anim.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].nameHash == AvatarController.turnRateHash)
				{
					this.hasTurnRate = true;
				}
			}
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000431 RID: 1073 RVA: 0x0001FFD8 File Offset: 0x0001E1D8
	public EntityAlive Entity
	{
		get
		{
			return this.entity;
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001FFE0 File Offset: 0x0001E1E0
	public bool IsMoving(float forwardSpeed, float strafeSpeed)
	{
		return forwardSpeed * forwardSpeed + strafeSpeed * strafeSpeed > 0.0001f;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x0001FFF0 File Offset: 0x0001E1F0
	public virtual void NotifyAnimatorMove(Animator instigator)
	{
		this.entity.NotifyRootMotion(instigator);
	}

	// Token: 0x06000434 RID: 1076
	public abstract Transform GetActiveModelRoot();

	// Token: 0x06000435 RID: 1077 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual Transform GetRightHandTransform()
	{
		return null;
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public Texture2D GetTexture()
	{
		return null;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00020004 File Offset: 0x0001E204
	public virtual void ResetAnimations()
	{
		Animator animator = this.GetAnimator();
		if (animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.enabled = true;
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x0002002E File Offset: 0x0001E22E
	public virtual void SetMeleeAttackSpeed(float _speed)
	{
		this.UpdateFloat(AvatarController.meleeAttackSpeedHash, _speed, true);
	}

	// Token: 0x06000439 RID: 1081
	public abstract bool IsAnimationAttackPlaying();

	// Token: 0x0600043A RID: 1082
	public abstract void StartAnimationAttack();

	// Token: 0x0600043B RID: 1083 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetInAir(bool inAir)
	{
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetAttackImpact()
	{
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsAttackImpact()
	{
		return true;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsAnimationWithMotionRunning()
	{
		return true;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual AvatarController.ActionState GetActionState()
	{
		return AvatarController.ActionState.None;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsActionActive()
	{
		return false;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAction(int _animType)
	{
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationSpecialAttackPlaying()
	{
		return false;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationSpecialAttack(bool _b, int _animType)
	{
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationSpecialAttack2Playing()
	{
		return false;
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationSpecialAttack2()
	{
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationRagingPlaying()
	{
		return false;
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationRaging()
	{
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationFiring()
	{
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationHitRunning()
	{
		return false;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00020040 File Offset: 0x0001E240
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool CheckHit(float duration)
	{
		return this.hitWeight < 0.15f || duration > this.hitDuration || !this.IsAnimationHitRunning();
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00020064 File Offset: 0x0001E264
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitHitDuration(float duration)
	{
		if (this.hitWeight > 0.15f)
		{
			float num = 0.2f;
			if (duration == 0f)
			{
				num = 0.1f;
			}
			if (this.hitWeightTarget > this.hitWeight)
			{
				this.hitWeightTarget += num;
				if (this.hitWeightTarget > this.hitWeightMax)
				{
					this.hitWeightTarget = this.hitWeightMax;
				}
			}
			this.hitWeight += num;
			if (this.hitWeight > this.hitWeightMax)
			{
				this.hitWeight = this.hitWeightMax;
			}
			return;
		}
		duration = Utils.FastMax(duration, 0.120000005f);
		this.hitDuration = duration;
		float num2 = Utils.FastMin(duration * 0.25f, 0.1f);
		this.hitDurationOut = duration - num2;
		this.hitWeightTarget = num2 / 0.1f;
		this.hitWeightTarget = Utils.FastClamp(this.hitWeightTarget, 0.2f, 0.8f);
		this.hitWeightDuration = num2 / Utils.FastMax(0.01f, this.hitWeightTarget - this.hitWeight);
		if (this.hitWeight == 0f)
		{
			this.anim.SetLayerWeight(this.hitLayerIndex, 0.01f);
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationHarvestingPlaying()
	{
		return false;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationHarvesting(float _length, bool _weaponFireTrigger)
	{
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationDigRunning()
	{
		return false;
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAttackProhibited()
	{
		return false;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationDodge(float _blend)
	{
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationToDodge()
	{
		return false;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationJumping()
	{
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationJump(AnimJumpMode jumpMode)
	{
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationJumpRunning()
	{
		return false;
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetSwim(bool _enable)
	{
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0002018A File Offset: 0x0001E38A
	public virtual float GetAnimationElectrocuteRemaining()
	{
		return this.electrocuteTime;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00020192 File Offset: 0x0001E392
	public virtual void StartAnimationElectrocute(float _duration)
	{
		this.electrocuteTime = _duration;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Electrocute(bool enabled)
	{
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationReloading()
	{
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0002019B File Offset: 0x0001E39B
	public void SetReloadBool(bool value)
	{
		this._setBool(AvatarController.reloadHash, value, true);
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000201AA File Offset: 0x0001E3AA
	public void SetStress(float val)
	{
		this._setFloat(AvatarController.stressLevel, val, true);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationUsePlaying()
	{
		return false;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartAnimationUse()
	{
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetAiming(bool _bEnable)
	{
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x000201B9 File Offset: 0x0001E3B9
	public virtual void SetAlive()
	{
		if (this.anim != null)
		{
			this._setBool(AvatarController.isAliveHash, true, true);
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetCrouching(bool _bEnable)
	{
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetDrunk(float _numBeers)
	{
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetInRightHand(Transform _transform)
	{
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetLookPosition(Vector3 _pos)
	{
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetVehicleAnimation(int _animHash, int _pose)
	{
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x000201D8 File Offset: 0x0001E3D8
	public virtual int GetVehicleAnimation()
	{
		int result;
		if (this.TryGetInt(AvatarController.vehiclePoseHash, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual AvatarController.CoverMode CurrentCoverMode()
	{
		return AvatarController.CoverMode.None;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual AvatarController.CoverType CurrentCoverType()
	{
		return AvatarController.CoverType.Center;
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual AvatarController.CoverEngageType CurrentCoverEngageType()
	{
		return AvatarController.CoverEngageType.None;
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetCoverMode(AvatarController.CoverMode _coverMode)
	{
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetCoverType(AvatarController.CoverType _coverType)
	{
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetCoverEngageType(AvatarController.CoverEngageType _engage)
	{
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetCoverHeight(float coverHeight)
	{
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetRagdollEnabled(bool _b)
	{
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetWalkingSpeed(float _f)
	{
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x000201F8 File Offset: 0x0001E3F8
	public virtual void SetWalkType(int _walkType, bool _trigger = false)
	{
		this._setInt(AvatarController.walkTypeHash, _walkType, true);
		if (_walkType >= 20)
		{
			this._setFloat(AvatarController.walkTypeBlendHash, 1f, true);
		}
		else if (_walkType > 0)
		{
			this._setFloat(AvatarController.walkTypeBlendHash, 0f, true);
		}
		if (_trigger)
		{
			this._setTrigger(AvatarController.movementTriggerHash, true);
		}
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetHeadAngles(float _nick, float _yaw)
	{
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetArmsAngles(float _rightArmAngle, float _leftArmAngle)
	{
	}

	// Token: 0x06000475 RID: 1141
	public abstract void SetVisible(bool _b);

	// Token: 0x06000476 RID: 1142 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetArchetypeStance(NPCInfo.StanceTypes stance)
	{
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0002024E File Offset: 0x0001E44E
	public virtual void TriggerReaction(int reaction)
	{
		if (this.anim != null)
		{
			this._setInt(AvatarController.reactionTypeHash, reaction, true);
			this._setTrigger(AvatarController.reactionTriggerHash, true);
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00020277 File Offset: 0x0001E477
	public virtual void TriggerSleeperPose(int pose, bool returningToSleep = false)
	{
		if (this.anim != null)
		{
			this._setInt(AvatarController.sleeperPoseHash, pose, true);
			this._setTrigger(AvatarController.sleeperTriggerHash, true);
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void RemoveLimb(BodyDamage _bodyDamage, bool restoreState)
	{
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x000202A0 File Offset: 0x0001E4A0
	public virtual void DismemberLimb(BodyDamage _bodyDamage, bool restoreState)
	{
		if (_bodyDamage.bodyPartHit != EnumBodyPartHit.None)
		{
			this.RemoveLimb(_bodyDamage, restoreState);
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void TurnIntoCrawler()
	{
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void BeginStun(EnumEntityStunType stun, EnumBodyPartHit _bodyPart, Utils.EnumHitDirection _hitDirection, bool _criticalHit, float random)
	{
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void EndStun()
	{
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationStunRunning()
	{
		return false;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StartEating()
	{
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void StopEating()
	{
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void PlayPlayerFPRevive()
	{
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsAnimationPlayerFPRevivePlaying()
	{
		return false;
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000202B2 File Offset: 0x0001E4B2
	public bool IsRootMotionForced()
	{
		return this.anim != null && this.anim.GetFloat(AvatarController.forcedRootMotionHash) > 0f;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000202DB File Offset: 0x0001E4DB
	public bool IsAttackPrevented()
	{
		return this.anim != null && this.anim.GetFloat(AvatarController.preventAttackHash) > 0f;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00020304 File Offset: 0x0001E504
	public virtual void SetFallAndGround(bool _canFall, bool _onGnd)
	{
		this._setBool(AvatarController.canFallHash, _canFall, false);
		this._setBool(AvatarController.isOnGroundHash, _onGnd, false);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00020320 File Offset: 0x0001E520
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
		if (this.hasTurnRate)
		{
			float y = this.entity.transform.eulerAngles.y;
			float num = Mathf.DeltaAngle(y, this.turnRateFacing) * 50f;
			if ((num > 5f && this.turnRate >= 0f) || (num < -5f && this.turnRate <= 0f))
			{
				float num2 = Utils.FastAbs(num) - Utils.FastAbs(this.turnRate);
				if (num2 > 0f)
				{
					this.turnRate = Utils.FastLerpUnclamped(this.turnRate, num, 0.2f);
				}
				else if (num2 < -50f)
				{
					this.turnRate = Utils.FastLerpUnclamped(this.turnRate, num, 0.05f);
				}
			}
			else
			{
				this.turnRate *= 0.92f;
				this.turnRate = Utils.FastMoveTowards(this.turnRate, 0f, 2f);
			}
			this.turnRateFacing = y;
			this._setFloat(AvatarController.turnRateHash, this.turnRate, false);
		}
		this.updateNetworkAnimData();
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0002042C File Offset: 0x0001E62C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.electrocuteTime > 0f)
		{
			this.electrocuteTime -= deltaTime;
		}
		else if (this.electrocuteTime <= 0f)
		{
			this.Electrocute(false);
			this.electrocuteTime = 0f;
		}
		if (this.hitLayerIndex >= 0)
		{
			if (this.hitWeightTarget > 0f && this.hitWeight == this.hitWeightTarget)
			{
				if (this.hitDuration > 999f)
				{
					if (!this.IsAnimationHitRunning() || this.entity.IsDead() || this.entity.emodel.IsRagdollActive)
					{
						this.hitWeightDuration = 0.4f;
						this.hitWeightTarget = 0f;
					}
				}
				else if (this.hitWeightTarget > 0.15f)
				{
					this.hitWeightDuration = (this.hitDurationOut + 0.2f) / (this.hitWeight - 0.15f);
					this.hitWeightTarget = 0.15f;
				}
				else
				{
					this.hitWeightDuration = 4f;
					this.hitWeightTarget = 0f;
				}
			}
			if (this.hitWeight != this.hitWeightTarget)
			{
				this.hitWeight = Mathf.MoveTowards(this.hitWeight, this.hitWeightTarget, deltaTime / this.hitWeightDuration);
				this.anim.SetLayerWeight(this.hitLayerIndex, this.hitWeight);
			}
		}
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00020588 File Offset: 0x0001E788
	[PublicizedFrom(EAccessModifier.Private)]
	public void processAnimParamData(List<AnimParamData> animationParameterData)
	{
		for (int i = 0; i < animationParameterData.Count; i++)
		{
			int nameHash = animationParameterData[i].NameHash;
			switch (animationParameterData[i].ValueType)
			{
			case AnimParamData.ValueTypes.Bool:
				this.UpdateBool(nameHash, animationParameterData[i].IntValue != 0, true);
				break;
			case AnimParamData.ValueTypes.Trigger:
				if (animationParameterData[i].IntValue != 0)
				{
					this.TriggerEvent(nameHash);
				}
				else
				{
					this.CancelEvent(nameHash);
				}
				break;
			case AnimParamData.ValueTypes.Float:
				this.UpdateFloat(nameHash, animationParameterData[i].FloatValue, true);
				break;
			case AnimParamData.ValueTypes.Int:
				this.UpdateInt(nameHash, animationParameterData[i].IntValue, true);
				break;
			case AnimParamData.ValueTypes.DataFloat:
				this.SetDataFloat((AvatarController.DataTypes)nameHash, animationParameterData[i].FloatValue, true);
				break;
			}
		}
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0002065B File Offset: 0x0001E85B
	public void TriggerEvent(string _property)
	{
		this._setTrigger(_property, true);
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00020665 File Offset: 0x0001E865
	public void TriggerEvent(int _pid)
	{
		this._setTrigger(_pid, true);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0002066F File Offset: 0x0001E86F
	public void CancelEvent(string _property)
	{
		this._resetTrigger(_property, true);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00020679 File Offset: 0x0001E879
	public void CancelEvent(int _pid)
	{
		this._resetTrigger(_pid, true);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00020683 File Offset: 0x0001E883
	public void UpdateFloat(string _property, float _value, bool _netsync = true)
	{
		this._setFloat(_property, _value, _netsync);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0002068E File Offset: 0x0001E88E
	public void UpdateFloat(int _pid, float _value, bool _netsync = true)
	{
		this._setFloat(_pid, _value, _netsync);
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00020699 File Offset: 0x0001E899
	public void UpdateBool(string _property, bool _value, bool _netsync = true)
	{
		this._setBool(_property, _value, _netsync);
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x000206A4 File Offset: 0x0001E8A4
	public void UpdateBool(int _pid, bool _value, bool _netsync = true)
	{
		this._setBool(_pid, _value, _netsync);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x000206AF File Offset: 0x0001E8AF
	public void UpdateInt(string _property, int _value, bool _netsync = true)
	{
		this._setInt(_property, _value, _netsync);
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x000206BA File Offset: 0x0001E8BA
	public void UpdateInt(int _pid, int _value, bool _netsync = true)
	{
		this._setInt(_pid, _value, _netsync);
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x000206C5 File Offset: 0x0001E8C5
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setTrigger(string _property, bool _netsync = true)
	{
		this._setTrigger(Animator.StringToHash(_property), _netsync);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x000206D4 File Offset: 0x0001E8D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setTrigger(int _pid, bool _netsync = true)
	{
		if (this.anim != null)
		{
			this.anim.SetTrigger(_pid);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_pid, AnimParamData.ValueTypes.Trigger, true));
			}
			this.OnTrigger(_pid);
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnTrigger(int _id)
	{
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00020727 File Offset: 0x0001E927
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _resetTrigger(string _property, bool _netsync = true)
	{
		this._resetTrigger(Animator.StringToHash(_property), _netsync);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00020738 File Offset: 0x0001E938
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _resetTrigger(int _propertyHash, bool _netsync = true)
	{
		if (this.anim != null && this.anim.gameObject.activeSelf && this.anim.GetBool(_propertyHash))
		{
			this.anim.ResetTrigger(_propertyHash);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, false));
			}
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x000207A4 File Offset: 0x0001E9A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setFloat(string _property, float _value, bool _netsync = true)
	{
		this._setFloat(Animator.StringToHash(_property), _value, _netsync);
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x000207B4 File Offset: 0x0001E9B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setBool(string _property, bool _value, bool _netsync = true)
	{
		int propertyHash = Animator.StringToHash(_property);
		this._setBool(propertyHash, _value, _netsync);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x000207D1 File Offset: 0x0001E9D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setInt(string _property, int _value, bool _netsync = true)
	{
		this._setInt(Animator.StringToHash(_property), _value, _netsync);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x000207E4 File Offset: 0x0001E9E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setFloat(int _propertyHash, float _value, bool _netSync = true)
	{
		if (this.anim)
		{
			if (!_netSync)
			{
				this.anim.SetFloat(_propertyHash, _value);
				return;
			}
			float num = this.anim.GetFloat(_propertyHash) - _value;
			if (num * num > 0.0001f)
			{
				this.anim.SetFloat(_propertyHash, _value);
				if (!this.entity.isEntityRemote && _netSync)
				{
					this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Float, _value));
				}
			}
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00020858 File Offset: 0x0001EA58
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setBool(int _propertyHash, bool _value, bool _netsync = true)
	{
		if (this.anim != null && this.anim.GetBool(_propertyHash) != _value)
		{
			this.anim.SetBool(_propertyHash, _value);
			if (_propertyHash == AvatarController.isFPVHash)
			{
				return;
			}
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Bool, _value));
			}
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x000208C0 File Offset: 0x0001EAC0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		if (this.anim != null && this.anim.GetInteger(_propertyHash) != _value)
		{
			this.anim.SetInteger(_propertyHash, _value);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Int, _value));
			}
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0002091C File Offset: 0x0001EB1C
	public virtual void SetDataFloat(AvatarController.DataTypes _type, float _value, bool _netsync = true)
	{
		if (_type == AvatarController.DataTypes.HitDuration)
		{
			this.InitHitDuration(_value);
		}
		if (!this.entity.isEntityRemote && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData((int)_type, AnimParamData.ValueTypes.DataFloat, _value));
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0002094D File Offset: 0x0001EB4D
	public virtual bool TryGetTrigger(string _property, out bool _value)
	{
		return this.TryGetTrigger(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0002095C File Offset: 0x0001EB5C
	public virtual bool TryGetFloat(string _property, out float _value)
	{
		return this.TryGetFloat(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0002096B File Offset: 0x0001EB6B
	public virtual bool TryGetBool(string _property, out bool _value)
	{
		return this.TryGetBool(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0002097A File Offset: 0x0001EB7A
	public virtual bool TryGetInt(string _property, out int _value)
	{
		return this.TryGetInt(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0002098C File Offset: 0x0001EB8C
	public virtual bool TryGetTrigger(int _propertyHash, out bool _value)
	{
		if (this.anim == null)
		{
			return _value = false;
		}
		_value = this.anim.GetBool(_propertyHash);
		return true;
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000209BD File Offset: 0x0001EBBD
	public virtual bool TryGetFloat(int _propertyHash, out float _value)
	{
		if (this.anim == null)
		{
			_value = 0f;
			return false;
		}
		_value = this.anim.GetFloat(_propertyHash);
		return true;
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000209E8 File Offset: 0x0001EBE8
	public virtual bool TryGetBool(int _propertyHash, out bool _value)
	{
		if (this.anim == null)
		{
			return _value = false;
		}
		_value = this.anim.GetBool(_propertyHash);
		return true;
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00020A19 File Offset: 0x0001EC19
	public virtual bool TryGetInt(int _propertyHash, out int _value)
	{
		if (this.anim == null)
		{
			_value = 0;
			return false;
		}
		_value = this.anim.GetInteger(_propertyHash);
		return true;
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00020A40 File Offset: 0x0001EC40
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateNetworkAnimData()
	{
		if (this.entity == null)
		{
			return;
		}
		if (this.entity.isEntityRemote)
		{
			if (this.queuedAnimParams.Count > 0)
			{
				do
				{
					this.processAnimParamData(this.queuedAnimParams[0]);
					this.queuedAnimParams.RemoveAt(0);
				}
				while (this.queuedAnimParams.Count > 10);
				return;
			}
		}
		else
		{
			foreach (List<AnimParamData> animationParameterData in this.changedAnimationParameters.GetParameterLists())
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, animationParameterData), false, -1, this.entity.entityId, this.entity.entityId, null, 192, false);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, animationParameterData), false);
				}
			}
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00020B68 File Offset: 0x0001ED68
	public void SyncAnimParameters(int _toEntityId)
	{
		if (!this.anim)
		{
			return;
		}
		Dictionary<int, AnimParamData> dictionary = new Dictionary<int, AnimParamData>();
		foreach (AnimatorControllerParameter animatorControllerParameter in this.anim.parameters)
		{
			switch (animatorControllerParameter.type)
			{
			case AnimatorControllerParameterType.Float:
			{
				float @float = this.anim.GetFloat(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Float, @float);
				break;
			}
			case AnimatorControllerParameterType.Int:
			{
				int integer = this.anim.GetInteger(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Int, integer);
				break;
			}
			case AnimatorControllerParameterType.Bool:
			{
				bool @bool = this.anim.GetBool(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Bool, @bool);
				break;
			}
			}
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, dictionary), false, _toEntityId, -1, -1, null, 192, false);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00020C94 File Offset: 0x0001EE94
	public virtual string GetParameterName(int _nameHash)
	{
		foreach (AnimatorControllerParameter animatorControllerParameter in this.anim.parameters)
		{
			if (animatorControllerParameter.nameHash == _nameHash)
			{
				return animatorControllerParameter.name;
			}
		}
		return "?";
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00020CD4 File Offset: 0x0001EED4
	public void SetAnimParameters(List<AnimParamData> animationParameterData)
	{
		this.queuedAnimParams.Add(animationParameterData);
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00020CE4 File Offset: 0x0001EEE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarController()
	{
	}

	// Token: 0x0400049C RID: 1180
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMinFloatChangeSquared = 0.0001f;

	// Token: 0x0400049D RID: 1181
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool initialized;

	// Token: 0x0400049E RID: 1182
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int attackTag;

	// Token: 0x0400049F RID: 1183
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int deathHash;

	// Token: 0x040004A0 RID: 1184
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int digHash;

	// Token: 0x040004A1 RID: 1185
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitStartHash;

	// Token: 0x040004A2 RID: 1186
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitHash;

	// Token: 0x040004A3 RID: 1187
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpHash;

	// Token: 0x040004A4 RID: 1188
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int moveHash;

	// Token: 0x040004A5 RID: 1189
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunHash;

	// Token: 0x040004A6 RID: 1190
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int readyToFireHash;

	// Token: 0x040004A7 RID: 1191
	public static int attackHash;

	// Token: 0x040004A8 RID: 1192
	public static int attackBlendHash;

	// Token: 0x040004A9 RID: 1193
	public static int attackStartHash;

	// Token: 0x040004AA RID: 1194
	public static int attackReadyHash;

	// Token: 0x040004AB RID: 1195
	public static int meleeAttackSpeedHash;

	// Token: 0x040004AC RID: 1196
	public static int beginCorpseEatHash;

	// Token: 0x040004AD RID: 1197
	public static int endCorpseEatHash;

	// Token: 0x040004AE RID: 1198
	public static int forwardHash;

	// Token: 0x040004AF RID: 1199
	public static int hitBodyPartHash;

	// Token: 0x040004B0 RID: 1200
	public static int idleTimeHash;

	// Token: 0x040004B1 RID: 1201
	public static int aimPitchHash;

	// Token: 0x040004B2 RID: 1202
	public static int aimYawHash;

	// Token: 0x040004B3 RID: 1203
	public static int isAimingHash;

	// Token: 0x040004B4 RID: 1204
	public static int itemUseHash;

	// Token: 0x040004B5 RID: 1205
	public static int movementStateHash;

	// Token: 0x040004B6 RID: 1206
	public static int rotationPitchHash;

	// Token: 0x040004B7 RID: 1207
	public static int strafeHash;

	// Token: 0x040004B8 RID: 1208
	public static int swimSelectHash;

	// Token: 0x040004B9 RID: 1209
	public static int turnRateHash;

	// Token: 0x040004BA RID: 1210
	public static int weaponCarryHash;

	// Token: 0x040004BB RID: 1211
	public static int weaponHoldTypeHash;

	// Token: 0x040004BC RID: 1212
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int walkTypeHash;

	// Token: 0x040004BD RID: 1213
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int walkTypeBlendHash;

	// Token: 0x040004BE RID: 1214
	public static int isAliveHash;

	// Token: 0x040004BF RID: 1215
	public static int isDeadHash;

	// Token: 0x040004C0 RID: 1216
	public static int isFPVHash;

	// Token: 0x040004C1 RID: 1217
	public static int isMovingHash;

	// Token: 0x040004C2 RID: 1218
	public static int isSwimHash;

	// Token: 0x040004C3 RID: 1219
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int attackTriggerHash;

	// Token: 0x040004C4 RID: 1220
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int deathTriggerHash;

	// Token: 0x040004C5 RID: 1221
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitTriggerHash;

	// Token: 0x040004C6 RID: 1222
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int movementTriggerHash;

	// Token: 0x040004C7 RID: 1223
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int electrocuteTriggerHash;

	// Token: 0x040004C8 RID: 1224
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int painTriggerHash;

	// Token: 0x040004C9 RID: 1225
	public static int itemHasChangedTriggerHash;

	// Token: 0x040004CA RID: 1226
	public static int itemThrownAwayTriggerHash;

	// Token: 0x040004CB RID: 1227
	public static int reloadHash;

	// Token: 0x040004CC RID: 1228
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int dodgeBlendHash;

	// Token: 0x040004CD RID: 1229
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int dodgeTriggerHash;

	// Token: 0x040004CE RID: 1230
	public static int reactionTypeHash;

	// Token: 0x040004CF RID: 1231
	public static int reactionTriggerHash;

	// Token: 0x040004D0 RID: 1232
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperPoseHash;

	// Token: 0x040004D1 RID: 1233
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperTriggerHash;

	// Token: 0x040004D2 RID: 1234
	public static int jumpLandResponseHash;

	// Token: 0x040004D3 RID: 1235
	public static int forcedRootMotionHash;

	// Token: 0x040004D4 RID: 1236
	public static int preventAttackHash;

	// Token: 0x040004D5 RID: 1237
	public static int canFallHash;

	// Token: 0x040004D6 RID: 1238
	public static int isOnGroundHash;

	// Token: 0x040004D7 RID: 1239
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int triggerAliveHash;

	// Token: 0x040004D8 RID: 1240
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int bodyPartHitHash;

	// Token: 0x040004D9 RID: 1241
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitDirectionHash;

	// Token: 0x040004DA RID: 1242
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitDamageHash;

	// Token: 0x040004DB RID: 1243
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int criticalHitHash;

	// Token: 0x040004DC RID: 1244
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int randomHash;

	// Token: 0x040004DD RID: 1245
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpStartHash;

	// Token: 0x040004DE RID: 1246
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpLandHash;

	// Token: 0x040004DF RID: 1247
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isMaleHash;

	// Token: 0x040004E0 RID: 1248
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int specialAttack2Hash;

	// Token: 0x040004E1 RID: 1249
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int rageHash;

	// Token: 0x040004E2 RID: 1250
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunTypeHash;

	// Token: 0x040004E3 RID: 1251
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunBodyPartHash;

	// Token: 0x040004E4 RID: 1252
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isCriticalHash;

	// Token: 0x040004E5 RID: 1253
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int HitRandomValueHash;

	// Token: 0x040004E6 RID: 1254
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int beginStunTriggerHash;

	// Token: 0x040004E7 RID: 1255
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int endStunTriggerHash;

	// Token: 0x040004E8 RID: 1256
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int toCrawlerTriggerHash;

	// Token: 0x040004E9 RID: 1257
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isElectrocutedHash;

	// Token: 0x040004EA RID: 1258
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isClimbingHash;

	// Token: 0x040004EB RID: 1259
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int verticalSpeedHash;

	// Token: 0x040004EC RID: 1260
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int reviveHash;

	// Token: 0x040004ED RID: 1261
	public static int harvestingHash;

	// Token: 0x040004EE RID: 1262
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponFireHash;

	// Token: 0x040004EF RID: 1263
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponPreFireCancelHash;

	// Token: 0x040004F0 RID: 1264
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponPreFireHash;

	// Token: 0x040004F1 RID: 1265
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponAmmoRemaining;

	// Token: 0x040004F2 RID: 1266
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int useItemHash;

	// Token: 0x040004F3 RID: 1267
	public static int itemActionIndexHash;

	// Token: 0x040004F4 RID: 1268
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isCrouchingHash;

	// Token: 0x040004F5 RID: 1269
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int reloadSpeedHash;

	// Token: 0x040004F6 RID: 1270
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpTriggerHash;

	// Token: 0x040004F7 RID: 1271
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int inAirHash;

	// Token: 0x040004F8 RID: 1272
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpLandTriggerHash;

	// Token: 0x040004F9 RID: 1273
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitRandomValueHash;

	// Token: 0x040004FA RID: 1274
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int CoverModeHash;

	// Token: 0x040004FB RID: 1275
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int CoverTypeHash;

	// Token: 0x040004FC RID: 1276
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int CoverEngageHash;

	// Token: 0x040004FD RID: 1277
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int CoverHeightHash;

	// Token: 0x040004FE RID: 1278
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSitHash;

	// Token: 0x040004FF RID: 1279
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSideRightHash;

	// Token: 0x04000500 RID: 1280
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSideLeftHash;

	// Token: 0x04000501 RID: 1281
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleBackHash;

	// Token: 0x04000502 RID: 1282
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleStomachHash;

	// Token: 0x04000503 RID: 1283
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleStandHash;

	// Token: 0x04000504 RID: 1284
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int archetypeStanceHash;

	// Token: 0x04000505 RID: 1285
	public static int yLookHash;

	// Token: 0x04000506 RID: 1286
	public static int vehiclePoseHash;

	// Token: 0x04000507 RID: 1287
	public static int holsteredHash;

	// Token: 0x04000508 RID: 1288
	public static int stressLevel;

	// Token: 0x04000509 RID: 1289
	public const int cSleeperPoseMove = -2;

	// Token: 0x0400050A RID: 1290
	public const int cSleeperPoseAwake = -1;

	// Token: 0x0400050B RID: 1291
	public const int cSleeperPoseSit = 0;

	// Token: 0x0400050C RID: 1292
	public const int cSleeperPoseSideRight = 1;

	// Token: 0x0400050D RID: 1293
	public const int cSleeperPoseSideLeft = 2;

	// Token: 0x0400050E RID: 1294
	public const int cSleeperPoseBack = 3;

	// Token: 0x0400050F RID: 1295
	public const int cSleeperPoseStomach = 4;

	// Token: 0x04000510 RID: 1296
	public const int cSleeperPoseStand = 5;

	// Token: 0x04000511 RID: 1297
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive entity;

	// Token: 0x04000512 RID: 1298
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator anim;

	// Token: 0x04000513 RID: 1299
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<List<AnimParamData>> queuedAnimParams = new List<List<AnimParamData>>();

	// Token: 0x04000514 RID: 1300
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AvatarController.ChangedAnimationParameters changedAnimationParameters = new AvatarController.ChangedAnimationParameters();

	// Token: 0x04000515 RID: 1301
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float animSyncWaitTime = 0.5f;

	// Token: 0x04000516 RID: 1302
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float electrocuteTime;

	// Token: 0x04000517 RID: 1303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitBlendInTimeMax = 0.1f;

	// Token: 0x04000518 RID: 1304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitBlendOutExtraTime = 0.2f;

	// Token: 0x04000519 RID: 1305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitWeightFastTarget = 0.15f;

	// Token: 0x0400051A RID: 1306
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitAgainWeightAdd = 0.2f;

	// Token: 0x0400051B RID: 1307
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitAgainWeightAddWeak = 0.1f;

	// Token: 0x0400051C RID: 1308
	public float hitWeightMax = 1f;

	// Token: 0x0400051D RID: 1309
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitDuration;

	// Token: 0x0400051E RID: 1310
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitDurationOut;

	// Token: 0x0400051F RID: 1311
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int hitLayerIndex = -1;

	// Token: 0x04000520 RID: 1312
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeight = 0.001f;

	// Token: 0x04000521 RID: 1313
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeightTarget;

	// Token: 0x04000522 RID: 1314
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeightDuration;

	// Token: 0x04000523 RID: 1315
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float forwardSpeedLerpMultiplier = 10f;

	// Token: 0x04000524 RID: 1316
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float strafeSpeedLerpMultiplier = 10f;

	// Token: 0x04000525 RID: 1317
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetSpeedForward;

	// Token: 0x04000526 RID: 1318
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetSpeedStrafe;

	// Token: 0x04000527 RID: 1319
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPhysicsTicks = 50f;

	// Token: 0x04000528 RID: 1320
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasTurnRate;

	// Token: 0x04000529 RID: 1321
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float turnRateFacing;

	// Token: 0x0400052A RID: 1322
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float turnRate;

	// Token: 0x0400052B RID: 1323
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Dictionary<int, string> hashNames;

	// Token: 0x0400052C RID: 1324
	public const int cActionSpecial = 3000;

	// Token: 0x0400052D RID: 1325
	public const int cActionEnd = 9999;

	// Token: 0x0400052E RID: 1326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMaxQueuedAnimData = 10;

	// Token: 0x020000C4 RID: 196
	public enum DataTypes
	{
		// Token: 0x04000530 RID: 1328
		HitDuration
	}

	// Token: 0x020000C5 RID: 197
	[PublicizedFrom(EAccessModifier.Protected)]
	public class ChangedAnimationParameters
	{
		// Token: 0x060004AC RID: 1196 RVA: 0x00020D4C File Offset: 0x0001EF4C
		public void Add(AnimParamData apd)
		{
			int count = this.m_animationParameters.Count;
			List<AnimParamData> list;
			if (count < 1)
			{
				list = this.newPacket();
			}
			else
			{
				list = this.m_animationParameters[count - 1];
			}
			int index;
			if (this.m_animationParameterLookup.TryGetValue(apd.NameHash, out index))
			{
				list.RemoveAt(index);
			}
			this.m_animationParameterLookup[apd.NameHash] = list.Count;
			list.Add(apd);
			if (apd.ValueType == AnimParamData.ValueTypes.Trigger)
			{
				this.newPacket();
				this.m_hasAnyTriggers = true;
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00020DD4 File Offset: 0x0001EFD4
		[PublicizedFrom(EAccessModifier.Private)]
		public List<AnimParamData> newPacket()
		{
			List<AnimParamData> list = new List<AnimParamData>();
			this.m_animationParameters.Add(list);
			this.m_animationParameterLookup.Clear();
			return list;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00020E00 File Offset: 0x0001F000
		public List<List<AnimParamData>> GetParameterLists()
		{
			List<List<AnimParamData>> list = new List<List<AnimParamData>>();
			this.m_sendDelay -= Time.deltaTime;
			if (!this.m_hasAnyTriggers)
			{
				if (this.m_sendDelay > 0f)
				{
					return list;
				}
			}
			while (this.m_animationParameters.Count > 0)
			{
				List<AnimParamData> list2 = this.m_animationParameters[0];
				this.m_animationParameters.RemoveAt(0);
				if (list2.Count != 0)
				{
					list.Add(list2);
				}
			}
			this.m_hasAnyTriggers = false;
			this.m_sendDelay = 0.05f;
			return list;
		}

		// Token: 0x04000531 RID: 1329
		[PublicizedFrom(EAccessModifier.Private)]
		public const float sendPeriodInSeconds = 0.05f;

		// Token: 0x04000532 RID: 1330
		[PublicizedFrom(EAccessModifier.Private)]
		public float m_sendDelay;

		// Token: 0x04000533 RID: 1331
		[PublicizedFrom(EAccessModifier.Private)]
		public List<List<AnimParamData>> m_animationParameters = new List<List<AnimParamData>>();

		// Token: 0x04000534 RID: 1332
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, int> m_animationParameterLookup = new Dictionary<int, int>();

		// Token: 0x04000535 RID: 1333
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_hasAnyTriggers;
	}

	// Token: 0x020000C6 RID: 198
	public enum ActionState
	{
		// Token: 0x04000537 RID: 1335
		None,
		// Token: 0x04000538 RID: 1336
		Start,
		// Token: 0x04000539 RID: 1337
		Ready,
		// Token: 0x0400053A RID: 1338
		Active
	}

	// Token: 0x020000C7 RID: 199
	public enum CoverMode
	{
		// Token: 0x0400053C RID: 1340
		None,
		// Token: 0x0400053D RID: 1341
		Stand,
		// Token: 0x0400053E RID: 1342
		Crouch,
		// Token: 0x0400053F RID: 1343
		Prone
	}

	// Token: 0x020000C8 RID: 200
	public enum CoverType
	{
		// Token: 0x04000541 RID: 1345
		Left = -1,
		// Token: 0x04000542 RID: 1346
		Center,
		// Token: 0x04000543 RID: 1347
		Right
	}

	// Token: 0x020000C9 RID: 201
	public enum CoverEngageType
	{
		// Token: 0x04000545 RID: 1349
		None,
		// Token: 0x04000546 RID: 1350
		Peek,
		// Token: 0x04000547 RID: 1351
		Lean
	}
}
