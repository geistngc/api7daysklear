using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000CE RID: 206
[Preserve]
public class AvatarLocalPlayerController : AvatarCharacterController
{
	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060004CD RID: 1229 RVA: 0x00021498 File Offset: 0x0001F698
	public BodyAnimator FPSArms
	{
		get
		{
			return this.fpsArms;
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x000214A0 File Offset: 0x0001F6A0
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		base.SwitchModelAndView(_modelName, _bFPV, _bMale);
		if (this.entity is EntityPlayerLocal && (this.entity as EntityPlayerLocal).IsSpectator)
		{
			return;
		}
		if (this.fpsArms != null && this.isMale != _bMale)
		{
			if (this.fpsArms.Parts.BodyObj != null)
			{
				this.fpsArms.Parts.BodyObj.SetActive(false);
			}
			base.removeBodyAnimator(this.fpsArms);
			this.fpsArms = null;
		}
		if (this.fpsArms == null)
		{
			this.isMale = _bMale;
			EntityPlayerLocal entityPlayerLocal = this.entity as EntityPlayerLocal;
			Transform transform = (entityPlayerLocal != null) ? entityPlayerLocal.cameraTransform : null;
			if (transform != null)
			{
				Transform transform2 = transform.FindInChildren((this.entity.emodel is EModelSDCS) ? "baseRigFP" : (this.isMale ? "maleArms_fp" : "femaleArms_fp"));
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(true);
					this.fpsArms = base.addBodyAnimator(this.createFPSArms(transform2));
				}
			}
		}
		if (this.fpsArms != null)
		{
			this.initBodyAnimator(this.fpsArms, _bFPV, _bMale);
		}
		this.isFPV = _bFPV;
		if (_bFPV)
		{
			base.PrimaryBody = this.fpsArms;
			this.fpsArms.State = BodyAnimator.EnumState.Visible;
			base.CharacterBody.State = BodyAnimator.EnumState.OnlyColliders;
			return;
		}
		base.PrimaryBody = base.CharacterBody;
		if (this.fpsArms != null)
		{
			this.fpsArms.State = BodyAnimator.EnumState.Disabled;
		}
		base.CharacterBody.State = BodyAnimator.EnumState.Visible;
		if (base.HeldItemTransform != null)
		{
			Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 24, Utils.ExcludeLayerZoom);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0002164D File Offset: 0x0001F84D
	public void TPVResetAnimPose()
	{
		if (this.anim)
		{
			this.tpvDisableInFrames = 2;
			this.anim.enabled = true;
		}
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00021670 File Offset: 0x0001F870
	public override void SetInRightHand(Transform _transform)
	{
		base.SetInRightHand(_transform);
		if (base.HeldItemTransform != null)
		{
			if (this.isFPV)
			{
				Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 10, Utils.ExcludeLayerZoom);
				return;
			}
			Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 24, Utils.ExcludeLayerZoom);
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x000216C9 File Offset: 0x0001F8C9
	public override Transform GetActiveModelRoot()
	{
		if (base.PrimaryBody == null || base.PrimaryBody.Parts == null)
		{
			return null;
		}
		return base.PrimaryBody.Parts.BodyObj.transform;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x000216F8 File Offset: 0x0001F8F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void avatarVisibilityChanged(BodyAnimator _body, bool _bVisible)
	{
		if (!_bVisible)
		{
			base.avatarVisibilityChanged(_body, _bVisible);
			return;
		}
		if (_body == this.fpsArms)
		{
			_body.State = (this.isFPV ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
			return;
		}
		if (_body == base.CharacterBody)
		{
			_body.State = ((!this.isFPV) ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.OnlyColliders);
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00021748 File Offset: 0x0001F948
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.tpvDisableInFrames > 0)
		{
			int num = this.tpvDisableInFrames - 1;
			this.tpvDisableInFrames = num;
			if (num == 0 && this.anim && this.isFPV)
			{
				this.anim.enabled = false;
			}
		}
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00021798 File Offset: 0x0001F998
	[PublicizedFrom(EAccessModifier.Protected)]
	public override BodyAnimator createCharacterBody(Transform _bodyTransform)
	{
		AvatarCharacterController.AnimationStates animStates = new AvatarCharacterController.AnimationStates(this.getJumpStates(), this.getDeathStates(), this.getReloadStates(), this.getHitStates());
		return new UMACharacterBodyAnimator(base.Entity, animStates, _bodyTransform, BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x000217D1 File Offset: 0x0001F9D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void initBodyAnimator(BodyAnimator _body, bool _bFPV, bool _bMale)
	{
		base.initBodyAnimator(_body, _bFPV, _bMale);
		if (_body == this.fpsArms)
		{
			this.fpsArms.State = (_bFPV ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x000217F8 File Offset: 0x0001F9F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual BodyAnimator createFPSArms(Transform _fpsArmsTransform)
	{
		AvatarCharacterController.AnimationStates animStates = new AvatarCharacterController.AnimationStates(this.getJumpStates(), this.getDeathStates(), this.getReloadStates(), this.getHitStates());
		return new FirstPersonAnimator(base.Entity, animStates, _fpsArmsTransform, BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00021831 File Offset: 0x0001FA31
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _pid, bool _netsync = true)
	{
		base._setTrigger(_pid, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetTrigger(_pid);
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00021855 File Offset: 0x0001FA55
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _pid, bool _netsync = true)
	{
		base._resetTrigger(_pid, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.ResetTrigger(_pid);
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00021879 File Offset: 0x0001FA79
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _pid, float _value, bool _netsync = true)
	{
		base._setFloat(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetFloat(_pid, _value);
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0002189F File Offset: 0x0001FA9F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _pid, bool _value, bool _netsync = true)
	{
		base._setBool(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetBool(_pid, _value);
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x000218C5 File Offset: 0x0001FAC5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _pid, int _value, bool _netsync = true)
	{
		base._setInt(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetInteger(_pid, _value);
		}
	}

	// Token: 0x04000568 RID: 1384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator fpsArms;

	// Token: 0x04000569 RID: 1385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMale;

	// Token: 0x0400056A RID: 1386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFPV;

	// Token: 0x0400056B RID: 1387
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int tpvDisableInFrames;
}
