using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000D2 RID: 210
[Preserve]
public class AvatarSDCSController : LegacyAvatarController
{
	// Token: 0x06000522 RID: 1314 RVA: 0x00023374 File Offset: 0x00021574
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void assignStates()
	{
		this.jumpState = Animator.StringToHash("Base Layer.Jump");
		this.fpvJumpState = Animator.StringToHash("Base Layer.FPVFemaleJump");
		AvatarCharacterController.GetThirdPersonDeathStates(this.deathStates = new HashSet<int>());
		AvatarCharacterController.GetThirdPersonReloadStates(this.reloadStates = new HashSet<int>());
		AvatarCharacterController.GetThirdPersonHitStates(this.hitStates = new HashSet<int>());
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x000233DC File Offset: 0x000215DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void assignParts(bool _bFPV)
	{
		if (!_bFPV)
		{
			this.pelvis = this.bipedTransform.FindInChilds("Hips", false);
			this.spine = this.pelvis.Find("Spine");
			this.spine1 = this.spine.Find("Spine1");
			this.spine2 = this.spine1.Find("Spine2");
			this.spine3 = this.spine2.Find("Spine3");
			this.head = this.spine3.Find("Neck/Head");
			this.cameraNode = this.head.Find("CameraNode");
			this.rightHand = this.bipedTransform.FindInChilds("RightWeapon", false);
		}
		else
		{
			this.bNewModel = (this.bipedTransform.FindInChilds("Origin", false) != null);
			if (!this.bNewModel)
			{
				this.pelvis = this.bipedTransform.Find("Hips");
				this.spine = this.pelvis.Find("Spine");
				this.spine1 = this.spine.Find("Spine1");
				this.spine2 = this.spine1.Find("Spine2");
				this.spine3 = this.spine2.Find("Spine3");
				this.head = this.spine3.Find("Neck/Head");
				this.cameraNode = this.head.Find("CameraNode");
				this.cameraNode = this.spine3;
				this.rightHand = this.bipedTransform.FindInChilds("RightWeapon", false);
			}
			else
			{
				this.pelvis = null;
				this.spine = null;
				this.spine1 = null;
				this.spine2 = null;
				this.spine3 = null;
				this.head = null;
				this.cameraNode = null;
				this.rightHand = this.bipedTransform.FindInChilds("RightWeapon", false);
			}
		}
		this.meshTransform = this.bipedTransform.FindInChilds("body", false);
		if (this.meshTransform == null)
		{
			this.meshTransform = this.bipedTransform.FindInChilds("TraderBob", false);
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00022C2D File Offset: 0x00020E2D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		if (this.anim == null && this.m_bVisible)
		{
			base.SetAnimator(base.GetComponentInChildren<Animator>());
		}
		base.Update();
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00023611 File Offset: 0x00021811
	public override void SetInRightHand(Transform _transform)
	{
		base.SetInRightHand(_transform);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0002361C File Offset: 0x0002181C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setLayerWeights()
	{
		if (this.anim != null)
		{
			if (this.entity.IsDead())
			{
				this.anim.SetLayerWeight(1, 0f);
				this.anim.SetLayerWeight(2, 0f);
				this.anim.SetLayerWeight(3, 0f);
				return;
			}
			this.anim.SetLayerWeight(3, 1f);
			if (this.anim.GetBool("MinibikeIdle"))
			{
				this.anim.SetLayerWeight(1, 0f);
				this.anim.SetLayerWeight(2, 0f);
				return;
			}
			if (!this.anim.IsInTransition(1) && AnimationDelayData.AnimationDelay[this.entity.inventory.holdingItem.HoldType.Value].TwoHanded)
			{
				this.anim.SetLayerWeight(1, 0f);
				this.anim.SetLayerWeight(2, 1f);
				return;
			}
			if (!this.anim.IsInTransition(2))
			{
				this.anim.SetLayerWeight(1, 1f);
				this.anim.SetLayerWeight(2, 0f);
			}
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00023750 File Offset: 0x00021950
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateLayerStateInfo()
	{
		if (this.anim != null)
		{
			this.baseStateInfo = this.anim.GetCurrentAnimatorStateInfo(0);
			this.currentWeaponHoldLayer = this.anim.GetCurrentAnimatorStateInfo((this.entity.inventory.holdingItem.HoldType != 0 && AnimationDelayData.AnimationDelay[this.entity.inventory.holdingItem.HoldType.Value].TwoHanded) ? 2 : 1);
			this.painLayer = this.anim.GetCurrentAnimatorStateInfo(4);
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x000237F0 File Offset: 0x000219F0
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		string n = _bFPV ? "baseRigFP" : _modelName;
		Transform transform = this.modelTransform.Find(n);
		if (transform == null && _bFPV)
		{
			transform = this.modelTransform.Find(_modelName);
		}
		if (this.bipedTransform != null && this.bipedTransform != transform)
		{
			this.bipedTransform.gameObject.SetActive(false);
		}
		this.bipedTransform = transform;
		this.bipedTransform.gameObject.SetActive(true);
		this.modelName = _modelName;
		this.bMale = _bMale;
		this.bFPV = _bFPV;
		this.assignParts(this.bFPV);
		if (this.anim == null)
		{
			base.SetAnimator(base.GetComponentInChildren<Animator>());
		}
		if (this.HasParameter("IsMale"))
		{
			base._setBool("IsMale", _bMale, true);
		}
		if (this.anim != null)
		{
			this.anim.logWarnings = false;
			this.anim.GetBool(AvatarController.isDeadHash);
			this.anim.GetInteger(AvatarController.weaponHoldTypeHash);
		}
		if (this.rightHandItemTransform)
		{
			this.rightHandItemTransform.SetParent(this.rightHand);
			AnimationGunjointOffsetData.AnimationGunjointOffsets animationGunjointOffsets = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value];
			this.rightHandItemTransform.SetLocalPositionAndRotation(animationGunjointOffsets.position, Quaternion.Euler(animationGunjointOffsets.rotation));
		}
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
		this._setBool(AvatarController.isFPVHash, this.bFPV, true);
		this._setBool(AvatarController.isAliveHash, this.entity.IsAlive(), true);
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x000239B8 File Offset: 0x00021BB8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasParameter(string paramName)
	{
		AnimatorControllerParameter[] parameters = this.anim.parameters;
		for (int i = 0; i < parameters.Length; i++)
		{
			if (parameters[i].name == paramName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpineRotation()
	{
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x000239F2 File Offset: 0x00021BF2
	public override Transform GetRightHandTransform()
	{
		if (this.rightHand == null)
		{
			this.rightHand = this.bipedTransform.FindInChilds("RightWeapon", false);
		}
		return this.rightHand;
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00023A1F File Offset: 0x00021C1F
	public override Transform GetMeshTransform()
	{
		if (!(this.meshTransform != null))
		{
			return this.bipedTransform;
		}
		return this.meshTransform;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00023A3C File Offset: 0x00021C3C
	public override bool IsAnimationHitRunning()
	{
		return base.IsAnimationHitRunning() || this.hitStates.Contains(this.painLayer.fullPathHash);
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00023A60 File Offset: 0x00021C60
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		if (this.entity == null || this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim == null || !this.anim.enabled)
		{
			return;
		}
		this.updateLayerStateInfo();
		this.updateSpineRotation();
		if (this.entity.inventory.holdingItem.Actions[0] != null)
		{
			this.entity.inventory.holdingItem.Actions[0].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[0]);
		}
		if (this.entity.inventory.holdingItem.Actions[1] != null)
		{
			this.entity.inventory.holdingItem.Actions[1].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[1]);
		}
		if (this.anim.GetBool(AvatarController.isDeadHash) && !this.anim.IsInTransition(0))
		{
			this._setBool(AvatarController.isDeadHash, false, true);
		}
		if (!this.anim.IsInTransition(2) && this.anim.GetBool("Reload") && this.reloadStates.Contains(this.currentWeaponHoldLayer.fullPathHash) && this.rightHandAnimator != null)
		{
			this.rightHandAnimator.SetBool("Reload", false);
		}
		bool flag = this.anim.IsInTransition(4);
		int integer = this.anim.GetInteger(AvatarController.hitBodyPartHash);
		bool flag2 = this.IsAnimationHitRunning();
		if (!flag && integer != 0 && flag2)
		{
			this._setInt(AvatarController.hitBodyPartHash, 0, true);
			base._setBool("isCritical", false, true);
		}
		if (this.anim != null && this.anim.GetBool(AvatarController.itemUseHash))
		{
			int num = this.itemUseTicks - 1;
			this.itemUseTicks = num;
			if (num <= 0)
			{
				this._setBool(AvatarController.itemUseHash, false, true);
				if (this.rightHandAnimator != null)
				{
					this.rightHandAnimator.SetBool(AvatarController.itemUseHash, false);
				}
			}
		}
		if (this.isInDeathAnim && this.deathStates.Contains(this.baseStateInfo.fullPathHash) && !this.anim.IsInTransition(0))
		{
			this.didDeathTransition = true;
		}
		if (this.isInDeathAnim && this.didDeathTransition && (this.baseStateInfo.normalizedTime >= 1f || this.anim.IsInTransition(0)))
		{
			this.isInDeathAnim = false;
			if (this.entity.HasDeathAnim)
			{
				EModelBase emodel = this.entity.emodel;
				DamageResponse damageResponse = DamageResponse.New(true);
				emodel.DoRagdoll(damageResponse, EModelBase.RagdollMode.Default, 999999f);
			}
		}
		if (this.isInDeathAnim && this.entity.HasDeathAnim && this.entity.RootMotion && this.entity.isCollidedHorizontally)
		{
			this.isInDeathAnim = false;
			EModelBase emodel2 = this.entity.emodel;
			DamageResponse damageResponse = DamageResponse.New(true);
			emodel2.DoRagdoll(damageResponse, EModelBase.RagdollMode.Default, 999999f);
		}
		this._setBool(AvatarController.isFPVHash, this.bFPV, true);
	}

	// Token: 0x04000582 RID: 1410
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform meshTransform;

	// Token: 0x04000583 RID: 1411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<int> hitStates;

	// Token: 0x04000584 RID: 1412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimatorStateInfo painLayer;

	// Token: 0x04000585 RID: 1413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bNewModel;
}
