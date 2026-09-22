using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CF RID: 207
public abstract class AvatarMultiBodyController : AvatarController
{
	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060004DD RID: 1245 RVA: 0x000218F3 File Offset: 0x0001FAF3
	// (set) Token: 0x060004DE RID: 1246 RVA: 0x000218FB File Offset: 0x0001FAFB
	public BodyAnimator PrimaryBody
	{
		get
		{
			return this.primaryBody;
		}
		set
		{
			this.primaryBody = value;
			this.SetInRightHand(this.heldItemTransform);
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060004DF RID: 1247 RVA: 0x00021910 File Offset: 0x0001FB10
	public List<BodyAnimator> BodyAnimators
	{
		get
		{
			return this.bodyAnimators;
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00021918 File Offset: 0x0001FB18
	public Animator HeldItemAnimator
	{
		get
		{
			return this.heldItemAnimator;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00021920 File Offset: 0x0001FB20
	public Transform HeldItemTransform
	{
		get
		{
			return this.heldItemTransform;
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00021928 File Offset: 0x0001FB28
	[PublicizedFrom(EAccessModifier.Protected)]
	public BodyAnimator addBodyAnimator(BodyAnimator _body)
	{
		Animator animator = _body.Animator;
		if (animator)
		{
			animator.logWarnings = false;
		}
		this.bodyAnimators.Add(_body);
		base.SetAnimator(this.bodyAnimators[0].Animator);
		return _body;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0002196F File Offset: 0x0001FB6F
	[PublicizedFrom(EAccessModifier.Protected)]
	public void removeBodyAnimator(BodyAnimator _body)
	{
		this.bodyAnimators.Remove(_body);
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00021980 File Offset: 0x0001FB80
	public override void PlayPlayerFPRevive()
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				animator.SetTrigger(AvatarController.reviveHash);
			}
		}
		this.reviveTime = Time.time;
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x000219D5 File Offset: 0x0001FBD5
	public override bool IsAnimationPlayerFPRevivePlaying()
	{
		return Time.time - this.reviveTime < 4.5f;
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x000219EC File Offset: 0x0001FBEC
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.heldItemTransform == null || this.entity == null || this.entity.inventory == null || this.entity.inventory.holdingItem == null)
		{
			return;
		}
		if (_bFPV)
		{
			this.heldItemTransform.localPosition = Vector3.zero;
			this.heldItemTransform.localEulerAngles = Vector3.zero;
			return;
		}
		this.heldItemTransform.localPosition = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].position;
		this.heldItemTransform.localEulerAngles = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].rotation;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00021AC1 File Offset: 0x0001FCC1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnTrigger(int _id)
	{
		if (_id == AvatarController.weaponFireHash)
		{
			this.animationToDodgeTime = 1f;
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00021AD6 File Offset: 0x0001FCD6
	public override bool IsAnimationToDodge()
	{
		return this.animationToDodgeTime > 0f;
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsAnimationAttackPlaying()
	{
		return false;
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00021AE5 File Offset: 0x0001FCE5
	public override void SetInAir(bool inAir)
	{
		this._setBool(AvatarController.inAirHash, inAir, true);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00021AF4 File Offset: 0x0001FCF4
	public override void StartAnimationAttack()
	{
		this._setBool(AvatarController.harvestingHash, false, true);
		int meta = this.entity.inventory.holdingItemItemValue.Meta;
		this._setInt(AvatarController.weaponAmmoRemaining, meta, true);
		this._setTrigger(AvatarController.weaponFireHash, true);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00021B3D File Offset: 0x0001FD3D
	public override bool IsAnimationUsePlaying()
	{
		return this.timeUseAnimationPlaying > 0f;
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00021B4C File Offset: 0x0001FD4C
	public override void StartAnimationUse()
	{
		this._setTrigger(AvatarController.useItemHash, true);
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00021B5A File Offset: 0x0001FD5A
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.bSpecialAttackPlaying;
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00021B62 File Offset: 0x0001FD62
	public override void StartAnimationSpecialAttack(bool _b, int _animType)
	{
		this.idleTime = 0f;
		this.bSpecialAttackPlaying = _b;
		if (_b)
		{
			this._resetTrigger(AvatarController.weaponFireHash, true);
			this._resetTrigger(AvatarController.weaponPreFireCancelHash, true);
			this._setTrigger(AvatarController.weaponPreFireHash, true);
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00021B9D File Offset: 0x0001FD9D
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0f;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00021BAC File Offset: 0x0001FDAC
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
		this._resetTrigger(AvatarController.weaponFireHash, true);
		this._resetTrigger(AvatarController.weaponPreFireHash, true);
		this._setTrigger(AvatarController.weaponPreFireCancelHash, true);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00021BE8 File Offset: 0x0001FDE8
	public override bool IsAnimationHarvestingPlaying()
	{
		return this.timeHarestingAnimationPlaying > 0f;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00021BF7 File Offset: 0x0001FDF7
	public override void StartAnimationHarvesting(float _length, bool _weaponFireTrigger)
	{
		this.timeHarestingAnimationPlaying = _length;
		this._setBool(AvatarController.harvestingHash, true, true);
		if (_weaponFireTrigger)
		{
			this._setTrigger(AvatarController.weaponFireHash, true);
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00021C1C File Offset: 0x0001FE1C
	public override void SetDrunk(float _numBeers)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			BodyAnimator bodyAnimator = this.bodyAnimators[i];
			if (bodyAnimator.Animator)
			{
				bodyAnimator.SetDrunk(_numBeers);
			}
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00021C64 File Offset: 0x0001FE64
	public override void SetVehicleAnimation(int _animHash, int _pose)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				animator.SetInteger(_animHash, _pose);
			}
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00021CAB File Offset: 0x0001FEAB
	public override void SetAiming(bool _bEnable)
	{
		this.idleTime = 0f;
		this._setBool(AvatarController.isAimingHash, _bEnable, true);
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00021CC5 File Offset: 0x0001FEC5
	public override void SetCrouching(bool _bEnable)
	{
		this.idleTime = 0f;
		this._setBool(AvatarController.isCrouchingHash, _bEnable, true);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00021CDF File Offset: 0x0001FEDF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void avatarVisibilityChanged(BodyAnimator _body, bool _bVisible)
	{
		_body.State = (_bVisible ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00021CF0 File Offset: 0x0001FEF0
	public override void SetVisible(bool _b)
	{
		if (this.visible != _b)
		{
			int count = this.bodyAnimators.Count;
			for (int i = 0; i < count; i++)
			{
				BodyAnimator body = this.bodyAnimators[i];
				this.avatarVisibilityChanged(body, _b);
			}
			Transform holdingItemTransform = this.entity.inventory.GetHoldingItemTransform();
			if (holdingItemTransform != null)
			{
				MeshRenderer[] componentsInChildren = holdingItemTransform.GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].enabled = true;
				}
			}
			this.visible = _b;
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00021D7C File Offset: 0x0001FF7C
	public override void SetRagdollEnabled(bool _b)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			this.bodyAnimators[i].RagdollActive = _b;
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x000201AA File Offset: 0x0001E3AA
	public void SetStressLevel(float stress)
	{
		this._setFloat(AvatarController.stressLevel, stress, true);
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00021DB4 File Offset: 0x0001FFB4
	public override void StartAnimationReloading()
	{
		this.idleTime = 0f;
		float value = EffectManager.GetValue(PassiveEffects.ReloadSpeedMultiplier, this.entity.inventory.holdingItemItemValue, 1f, this.entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		int count = this.bodyAnimators.Count;
		bool value2 = this.entity as EntityPlayerLocal != null && (this.entity as EntityPlayerLocal).emodel.IsFPV;
		this._setBool(AvatarController.isFPVHash, value2, true);
		this._setBool(AvatarController.reloadHash, true, true);
		this._setFloat(AvatarController.reloadSpeedHash, value, true);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00021E60 File Offset: 0x00020060
	public override void StartAnimationJump(AnimJumpMode jumpMode)
	{
		this.idleTime = 0f;
		if (jumpMode == AnimJumpMode.Start)
		{
			this._setTrigger(AvatarController.jumpTriggerHash, true);
			this._setBool(AvatarController.inAirHash, true, true);
			return;
		}
		if (jumpMode != AnimJumpMode.Land)
		{
			return;
		}
		this._setTrigger(AvatarController.jumpLandHash, true);
		this._setInt(AvatarController.jumpLandResponseHash, 0, true);
		this._setBool(AvatarController.inAirHash, false, true);
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00021EC0 File Offset: 0x000200C0
	public override void SetSwim(bool _enable)
	{
		int walkType = -1;
		if (!_enable)
		{
			walkType = this.entity.GetWalkType();
		}
		this.SetWalkType(walkType, true);
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00021EE6 File Offset: 0x000200E6
	public override void StartAnimationFiring()
	{
		this.StartAnimationAttack();
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00021EF0 File Offset: 0x000200F0
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		this.idleTime = 0f;
		this._setInt(AvatarController.movementStateHash, _movementState, true);
		this._setInt(AvatarController.hitDirectionHash, _dir, true);
		this._setInt(AvatarController.hitDamageHash, _hitDamage, true);
		this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart, true);
		this._setFloat(AvatarController.hitRandomValueHash, _random, true);
		this._setBool(AvatarController.isCriticalHash, _criticalHit, true);
		this._setTrigger(AvatarController.hitTriggerHash, true);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00021F68 File Offset: 0x00020168
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		this.idleTime = 0f;
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			this.bodyAnimators[i].StartDeathAnimation(_bodyPart, _movementState, random);
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00021FAC File Offset: 0x000201AC
	public override void SetInRightHand(Transform _transform)
	{
		this.idleTime = 0f;
		if (_transform != null)
		{
			Quaternion localRotation = (this.heldItemTransform != null) ? this.heldItemTransform.localRotation : Quaternion.identity;
			_transform.SetParent(this.GetRightHandTransform(), false);
			if ((!this.entity.emodel.IsFPV || this.entity.isEntityRemote) && this.entity.inventory != null && this.entity.inventory.holdingItem != null)
			{
				AnimationGunjointOffsetData.AnimationGunjointOffsets animationGunjointOffsets = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value];
				_transform.localPosition = animationGunjointOffsets.position;
				_transform.localRotation = Quaternion.Euler(animationGunjointOffsets.rotation);
			}
			else
			{
				_transform.localPosition = Vector3.zero;
				_transform.localRotation = localRotation;
			}
		}
		this.heldItemTransform = _transform;
		this.heldItemAnimator = ((_transform != null) ? _transform.GetComponent<Animator>() : null);
		if (this.heldItemAnimator != null)
		{
			this.heldItemAnimator.logWarnings = false;
			this.heldItemAnimator.runtimeAnimatorController = (this.entity.emodel.IsFPV ? GameManager.Instance.FirstPersonWeaponAnimatorController : GameManager.Instance.ThirdPersonWeaponAnimatorController);
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x000220FE File Offset: 0x000202FE
	public override Transform GetRightHandTransform()
	{
		if (this.primaryBody == null)
		{
			return null;
		}
		return this.primaryBody.Parts.RightHandT;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0002211C File Offset: 0x0002031C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (this.animationToDodgeTime > 0f)
		{
			this.animationToDodgeTime -= deltaTime;
		}
		if (this.timeUseAnimationPlaying > 0f)
		{
			this.timeUseAnimationPlaying -= deltaTime;
		}
		if (this.timeHarestingAnimationPlaying > 0f)
		{
			this.timeHarestingAnimationPlaying -= deltaTime;
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= deltaTime;
		}
		if (!this.IsAnimationUsePlaying())
		{
			int value = this.entity.inventory.holdingItem.HoldType.Value;
			this._setInt(AvatarController.weaponHoldTypeHash, value, true);
			int carry = AnimationDelayData.AnimationDelay[value].Carry;
			this._setInt(AvatarController.weaponCarryHash, carry, true);
		}
		float speedForward = this.entity.speedForward;
		float speedStrafe = this.entity.speedStrafe;
		float x = this.entity.rotation.x;
		bool flag = this.entity.IsDead();
		bool flag2 = base.IsMoving(speedForward, speedStrafe);
		if (flag2)
		{
			this.idleTime = 0f;
		}
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			this.bodyAnimators[i].Update();
		}
		float num = speedStrafe;
		if (num >= 1234f)
		{
			num = 0f;
		}
		this._setFloat(AvatarController.forwardHash, speedForward, false);
		this._setFloat(AvatarController.strafeHash, num, false);
		this._setBool(AvatarController.isMovingHash, flag2, false);
		this._setFloat(AvatarController.rotationPitchHash, x, false);
		if (!flag)
		{
			if (speedStrafe >= 1234f)
			{
				this._setInt(AvatarController.movementStateHash, 4, true);
			}
			else
			{
				float num2 = speedForward * speedForward + speedStrafe * speedStrafe;
				this._setInt(AvatarController.movementStateHash, (num2 > base.Entity.moveSpeedAggro * base.Entity.moveSpeedAggro) ? 3 : ((num2 > base.Entity.moveSpeed * base.Entity.moveSpeed) ? 2 : ((num2 > 0.001f) ? 1 : 0)), true);
			}
		}
		float num3 = this.idleTime - this.idleTimeSent;
		if (num3 * num3 > 0.25f)
		{
			this.idleTimeSent = this.idleTime;
			this._setFloat(AvatarController.idleTimeHash, this.idleTime, false);
		}
		this.idleTime += deltaTime;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00022374 File Offset: 0x00020574
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (base.Entity.inventory.holdingItem.Actions[0] != null)
		{
			base.Entity.inventory.holdingItem.Actions[0].UpdateNozzleParticlesPosAndRot(base.Entity.inventory.holdingItemData.actionData[0]);
		}
		if (base.Entity.inventory.holdingItem.Actions[1] != null)
		{
			base.Entity.inventory.holdingItem.Actions[1].UpdateNozzleParticlesPosAndRot(base.Entity.inventory.holdingItemData.actionData[1]);
		}
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x000027FC File Offset: 0x000009FC
	public override void NotifyAnimatorMove(Animator anim)
	{
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00022421 File Offset: 0x00020621
	public override Animator GetAnimator()
	{
		return this.primaryBody.Animator;
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0002242E File Offset: 0x0002062E
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool animatorIsValid(Animator animator)
	{
		return animator && animator.enabled && animator.gameObject.activeInHierarchy;
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00022450 File Offset: 0x00020650
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _propertyHash, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (AvatarMultiBodyController.animatorIsValid(animator) && !animator.GetBool(_propertyHash))
			{
				animator.SetTrigger(_propertyHash);
				this.changed = true;
			}
		}
		if (AvatarMultiBodyController.animatorIsValid(this.heldItemAnimator) && !this.heldItemAnimator.GetBool(_propertyHash))
		{
			this.heldItemAnimator.SetTrigger(_propertyHash);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, true));
		}
		if (this.changed)
		{
			this.OnTrigger(_propertyHash);
		}
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00022518 File Offset: 0x00020718
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _propertyHash, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetBool(_propertyHash))
			{
				animator.ResetTrigger(_propertyHash);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetBool(_propertyHash))
		{
			this.heldItemAnimator.ResetTrigger(_propertyHash);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, false));
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x000225E0 File Offset: 0x000207E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _propertyHash, float _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				float num = animator.GetFloat(_propertyHash) - _value;
				if (num * num > 1.0000001E-06f)
				{
					animator.SetFloat(_propertyHash, _value);
					this.changed = true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetFloat(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetFloat(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Float, _value));
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x000226B4 File Offset: 0x000208B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _propertyHash, bool _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetBool(_propertyHash) != _value)
			{
				animator.SetBool(_propertyHash, _value);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetBool(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetBool(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _propertyHash != AvatarController.isFPVHash && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Bool, _value));
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00022790 File Offset: 0x00020990
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetInteger(_propertyHash) != _value)
			{
				animator.SetInteger(_propertyHash, _value);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetInteger(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetInteger(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Int, _value));
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0002285C File Offset: 0x00020A5C
	public override bool TryGetTrigger(int _propertyHash, out bool _value)
	{
		_value = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value |= animator.GetBool(_propertyHash);
				if (_value)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value |= this.heldItemAnimator.GetBool(_propertyHash);
		}
		return true;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x000228E0 File Offset: 0x00020AE0
	public override bool TryGetFloat(int _propertyHash, out float _value)
	{
		_value = float.NaN;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value = animator.GetFloat(_propertyHash);
				if (_value != float.NaN)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value = this.heldItemAnimator.GetFloat(_propertyHash);
		}
		return _value != float.NaN;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00022974 File Offset: 0x00020B74
	public override bool TryGetBool(int _propertyHash, out bool _value)
	{
		_value = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value |= animator.GetBool(_propertyHash);
				if (_value)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value |= this.heldItemAnimator.GetBool(_propertyHash);
		}
		return true;
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x000229F8 File Offset: 0x00020BF8
	public override bool TryGetInt(int _propertyHash, out int _value)
	{
		_value = int.MinValue;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value = animator.GetInteger(_propertyHash);
				if (_value != -2147483648)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value = this.heldItemAnimator.GetInteger(_propertyHash);
		}
		return _value != int.MinValue;
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00022A89 File Offset: 0x00020C89
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarMultiBodyController()
	{
	}

	// Token: 0x0400056C RID: 1388
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<BodyAnimator> bodyAnimators = new List<BodyAnimator>();

	// Token: 0x0400056D RID: 1389
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator primaryBody;

	// Token: 0x0400056E RID: 1390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform heldItemTransform;

	// Token: 0x0400056F RID: 1391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator heldItemAnimator;

	// Token: 0x04000570 RID: 1392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool visible = true;

	// Token: 0x04000571 RID: 1393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float animationToDodgeTime;

	// Token: 0x04000572 RID: 1394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeUseAnimationPlaying;

	// Token: 0x04000573 RID: 1395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeHarestingAnimationPlaying;

	// Token: 0x04000574 RID: 1396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSpecialAttackPlaying;

	// Token: 0x04000575 RID: 1397
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeSpecialAttack2Playing;

	// Token: 0x04000576 RID: 1398
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleTime;

	// Token: 0x04000577 RID: 1399
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleTimeSent;

	// Token: 0x04000578 RID: 1400
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float reviveTime;

	// Token: 0x04000579 RID: 1401
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cReviveAnimLength = 4.5f;

	// Token: 0x0400057A RID: 1402
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<int, AnimParamData> FullSyncAnimationParameters = new Dictionary<int, AnimParamData>();

	// Token: 0x0400057B RID: 1403
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool changed;
}
