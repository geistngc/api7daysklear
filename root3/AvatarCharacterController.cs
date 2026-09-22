using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public abstract class AvatarCharacterController : AvatarMultiBodyController
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000414 RID: 1044 RVA: 0x0001F084 File Offset: 0x0001D284
	public BodyAnimator CharacterBody
	{
		get
		{
			return this.characterBody;
		}
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x0001F08C File Offset: 0x0001D28C
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.characterBody != null && this.modelName != _modelName)
		{
			if (this.characterBody.Parts.BodyObj != null)
			{
				this.characterBody.Parts.BodyObj.SetActive(false);
			}
			base.removeBodyAnimator(this.characterBody);
			this.characterBody = null;
		}
		if (this.characterBody == null)
		{
			this.modelName = _modelName;
			Transform transform = EModelBase.FindModel(base.transform);
			if (transform != null)
			{
				Transform transform2 = transform.Find(_modelName);
				if (transform2)
				{
					transform2.gameObject.SetActive(true);
					this.characterBody = base.addBodyAnimator(this.createCharacterBody(transform2));
				}
			}
		}
		if (this.characterBody != null)
		{
			this.initBodyAnimator(this.characterBody, _bFPV, _bMale);
		}
		base.SwitchModelAndView(_modelName, _bFPV, _bMale);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0001F164 File Offset: 0x0001D364
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void initBodyAnimator(BodyAnimator _body, bool _bFPV, bool _bMale)
	{
		base._setBool("IsMale", _bMale, true);
		base._setFloat("IsMaleFloat", _bMale ? 1f : 0f, true);
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
		this._setBool(AvatarController.isFPVHash, _bFPV, true);
		this._setBool(AvatarController.isAliveHash, this.entity.IsAlive(), true);
		if (_body == this.characterBody)
		{
			this.characterBody.State = (_bFPV ? BodyAnimator.EnumState.OnlyColliders : BodyAnimator.EnumState.Visible);
		}
	}

	// Token: 0x06000417 RID: 1047
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract BodyAnimator createCharacterBody(Transform _bodyTransform);

	// Token: 0x06000418 RID: 1048 RVA: 0x0001F201 File Offset: 0x0001D401
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getJumpStates()
	{
		return new HashSet<int>
		{
			Animator.StringToHash("Base Layer.Jump")
		};
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x0001F219 File Offset: 0x0001D419
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getDeathStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetFirstPersonDeathStates(hashSet);
		AvatarCharacterController.GetThirdPersonDeathStates(hashSet);
		return hashSet;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0001F22C File Offset: 0x0001D42C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getReloadStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetFirstPersonReloadStates(hashSet);
		AvatarCharacterController.GetThirdPersonReloadStates(hashSet);
		return hashSet;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x0001F23F File Offset: 0x0001D43F
	public override Animator GetAnimator()
	{
		return this.characterBody.Animator;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0001F24C File Offset: 0x0001D44C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getHitStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetThirdPersonHitStates(hashSet);
		return hashSet;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x0001F25C File Offset: 0x0001D45C
	public static void GetFirstPersonReloadStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBlunderbussReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvPistolReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvMP5Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSniperRifleReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvM136Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvCrossbowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvHuntingRifleReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAugerReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvChainsawReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvMagnumReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvNailGunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAK47Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAK47Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvCompoundBowReload"));
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0001F39C File Offset: 0x0001D59C
	public static void GetThirdPersonReloadStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleMP5Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSniperRifleReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleM136Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleCrossbowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleHuntingRifleReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleAugerReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleChainsawReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReloadIntro"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReloadExit"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.Female44MagnumReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleNailGunReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBlunderbussReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemalePistolReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleAk47Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleCompoundBowReload"));
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x000027FC File Offset: 0x000009FC
	public static void GetFirstPersonHitStates(HashSet<int> hashSet)
	{
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x0001F4DC File Offset: 0x0001D6DC
	public static void GetThirdPersonHitStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchHeadLeft"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchHeadRight"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchChestLeft"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchChestRight"));
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x000027FC File Offset: 0x000009FC
	public static void GetFirstPersonDeathStates(HashSet<int> hashSet)
	{
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0001F530 File Offset: 0x0001D730
	public static void GetThirdPersonDeathStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("Base Layer.generic"));
		hashSet.Add(Animator.StringToHash("Base Layer.FemaleDeath01"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingHead"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingChest"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingLeftArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingRightArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingRightLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadFront"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeft"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftA"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftB"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftC"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadRight"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningChestA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningChestB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningHeadA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningHeadB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftArmA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftArmB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightArmA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightArmB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingChestA"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingChestB"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingHeadA"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingHeadB"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingLeftArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingRightArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingRightLeg"));
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0001F760 File Offset: 0x0001D960
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _pid, bool _netsync = true)
	{
		base._setTrigger(_pid, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (AvatarMultiBodyController.animatorIsValid(animator))
			{
				animator.SetTrigger(_pid);
			}
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001F798 File Offset: 0x0001D998
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _pid, bool _netsync = true)
	{
		base._resetTrigger(_pid, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.ResetTrigger(_pid);
			}
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x0001F7D0 File Offset: 0x0001D9D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _pid, float _value, bool _netsync = true)
	{
		base._setFloat(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetFloat(_pid, _value);
			}
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0001F80C File Offset: 0x0001DA0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _pid, bool _value, bool _netsync = true)
	{
		base._setBool(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetBool(_pid, _value);
			}
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0001F848 File Offset: 0x0001DA48
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _pid, int _value, bool _netsync = true)
	{
		base._setInt(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetInteger(_pid, _value);
			}
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x0001F882 File Offset: 0x0001DA82
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarCharacterController()
	{
	}

	// Token: 0x04000493 RID: 1171
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator characterBody;

	// Token: 0x04000494 RID: 1172
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string modelName;

	// Token: 0x020000C1 RID: 193
	public class AnimationStates
	{
		// Token: 0x06000429 RID: 1065 RVA: 0x0001F88A File Offset: 0x0001DA8A
		public AnimationStates(HashSet<int> _jumpStates, HashSet<int> _deathStates, HashSet<int> _reloadStates, HashSet<int> _hitStates)
		{
			this.JumpStates = _jumpStates;
			this.DeathStates = _deathStates;
			this.ReloadStates = _reloadStates;
			this.HitStates = _hitStates;
		}

		// Token: 0x04000495 RID: 1173
		public readonly HashSet<int> JumpStates;

		// Token: 0x04000496 RID: 1174
		public readonly HashSet<int> DeathStates;

		// Token: 0x04000497 RID: 1175
		public readonly HashSet<int> ReloadStates;

		// Token: 0x04000498 RID: 1176
		public readonly HashSet<int> HitStates;
	}
}
