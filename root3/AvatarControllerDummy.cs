using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class AvatarControllerDummy : LegacyAvatarController
{
	// Token: 0x060004B6 RID: 1206 RVA: 0x00021060 File Offset: 0x0001F260
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.modelTransform != null)
		{
			this.bipedTransform = this.modelTransform.Find(_modelName + (_bFPV ? "_FP" : ""));
			if (this.bipedTransform != null && this.entity != null)
			{
				this.rightHand = this.bipedTransform.FindInChilds(this.entity.GetRightHandTransformName(), false);
				base.SetAnimator(this.bipedTransform);
			}
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x000210E6 File Offset: 0x0001F2E6
	public override Transform GetRightHandTransform()
	{
		return this.rightHand;
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void assignStates()
	{
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpineRotation()
	{
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x000210EE File Offset: 0x0001F2EE
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.bSpecialAttackPlaying;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x000210F6 File Offset: 0x0001F2F6
	public override void StartAnimationSpecialAttack(bool _b, int _animType)
	{
		this.idleTime = 0f;
		this.bSpecialAttackPlaying = _b;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0002110A File Offset: 0x0001F30A
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0f;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00021119 File Offset: 0x0001F319
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00021131 File Offset: 0x0001F331
	public override bool IsAnimationRagingPlaying()
	{
		return this.timeRagePlaying > 0f;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00021140 File Offset: 0x0001F340
	public override void StartAnimationRaging()
	{
		this.idleTime = 0f;
		this.ragingTicks = 3;
		this.timeRagePlaying = 0.3f;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsAnimationWithMotionRunning()
	{
		return true;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x00021160 File Offset: 0x0001F360
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.timeAttackAnimationPlaying > 0f)
		{
			this.timeAttackAnimationPlaying -= Time.deltaTime;
		}
		if (this.timeUseAnimationPlaying > 0f)
		{
			this.timeUseAnimationPlaying -= Time.deltaTime;
		}
		if (this.timeRagePlaying > 0f)
		{
			this.timeRagePlaying -= Time.deltaTime;
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= Time.deltaTime;
		}
	}

	// Token: 0x04000552 RID: 1362
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public new bool bSpecialAttackPlaying;

	// Token: 0x04000553 RID: 1363
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public new float timeSpecialAttack2Playing;

	// Token: 0x04000554 RID: 1364
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeRagePlaying;

	// Token: 0x04000555 RID: 1365
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int ragingTicks;
}
