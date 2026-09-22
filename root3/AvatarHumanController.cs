using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000CD RID: 205
[Preserve]
public abstract class AvatarHumanController : AvatarController
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x000211F7 File Offset: 0x0001F3F7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.modelT = EModelBase.FindModel(base.transform);
		this.assignStates();
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00021218 File Offset: 0x0001F418
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (!this.bipedT)
		{
			this.hitLayerIndex = 3;
			this.bipedT = this.entity.emodel.GetModelTransform();
			this.rightHandT = this.FindTransform(this.entity.GetRightHandTransformName());
			base.SetAnimator(this.bipedT);
			if (this.entity.RootMotion)
			{
				this.rootMotion = this.bipedT.gameObject.AddComponent<AvatarRootMotion>();
				this.rootMotion.Init(this, this.anim);
			}
		}
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x000212D0 File Offset: 0x0001F4D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public Transform FindTransform(string _name)
	{
		return this.bipedT.FindInChildren(_name);
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x000212E0 File Offset: 0x0001F4E0
	public override void SetVisible(bool _b)
	{
		if (this.isVisible != _b || !this.isVisibleInit)
		{
			this.isVisible = _b;
			this.isVisibleInit = true;
			Transform transform = this.bipedT;
			if (transform)
			{
				Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = _b;
				}
			}
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0002133A File Offset: 0x0001F53A
	public override Transform GetActiveModelRoot()
	{
		return this.modelT;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00021342 File Offset: 0x0001F542
	public override Transform GetRightHandTransform()
	{
		return this.rightHandT;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0002134C File Offset: 0x0001F54C
	public override void TriggerSleeperPose(int pose, bool returningToSleep = false)
	{
		if (returningToSleep)
		{
			base.TriggerSleeperPose(pose, returningToSleep);
			return;
		}
		if (this.anim)
		{
			this._setInt(AvatarController.sleeperPoseHash, pose, true);
			switch (pose)
			{
			case -2:
				this.anim.CrossFadeInFixedTime("Crouch Walk 8", 0.25f);
				return;
			case -1:
				this._setTrigger(AvatarController.sleeperTriggerHash, true);
				break;
			case 0:
				this.anim.Play(AvatarController.sleeperIdleSitHash);
				return;
			case 1:
				this.anim.Play(AvatarController.sleeperIdleSideRightHash);
				return;
			case 2:
				this.anim.Play(AvatarController.sleeperIdleSideLeftHash);
				return;
			case 3:
				this.anim.Play(AvatarController.sleeperIdleBackHash);
				return;
			case 4:
				this.anim.Play(AvatarController.sleeperIdleStomachHash);
				return;
			case 5:
				this.anim.Play(AvatarController.sleeperIdleStandHash);
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x00021434 File Offset: 0x0001F634
	public override void TurnIntoCrawler()
	{
		this.isCrawler = true;
		this.crawlerTime = Time.time;
		this.isSuppressPain = true;
		this._setInt(AvatarController.hitBodyPartHash, 0, true);
		this.SetWalkType(21, false);
		this._setTrigger(AvatarController.toCrawlerTriggerHash, true);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00021471 File Offset: 0x0001F671
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		if (_propertyHash == AvatarController.walkTypeHash && _value == 5 && this.entity.GetWalkType() == 21)
		{
			return;
		}
		base._setInt(_propertyHash, _value, _netsync);
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001BEBD File Offset: 0x0001A0BD
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarHumanController()
	{
	}

	// Token: 0x04000556 RID: 1366
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cOverrideLayerIndex = 1;

	// Token: 0x04000557 RID: 1367
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cFullBodyLayerIndex = 2;

	// Token: 0x04000558 RID: 1368
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cHitLayerIndex = 3;

	// Token: 0x04000559 RID: 1369
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AvatarRootMotion rootMotion;

	// Token: 0x0400055A RID: 1370
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelT;

	// Token: 0x0400055B RID: 1371
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform bipedT;

	// Token: 0x0400055C RID: 1372
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightHandT;

	// Token: 0x0400055D RID: 1373
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo baseStateInfo;

	// Token: 0x0400055E RID: 1374
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo overrideStateInfo;

	// Token: 0x0400055F RID: 1375
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo fullBodyStateInfo;

	// Token: 0x04000560 RID: 1376
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo hitStateInfo;

	// Token: 0x04000561 RID: 1377
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isVisibleInit;

	// Token: 0x04000562 RID: 1378
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isVisible;

	// Token: 0x04000563 RID: 1379
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpState;

	// Token: 0x04000564 RID: 1380
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isJumpStarted;

	// Token: 0x04000565 RID: 1381
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isCrawler;

	// Token: 0x04000566 RID: 1382
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float crawlerTime;

	// Token: 0x04000567 RID: 1383
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isSuppressPain;
}
