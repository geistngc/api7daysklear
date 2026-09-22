using System;
using UnityEngine;

// Token: 0x020013DE RID: 5086
public class Detonator : MonoBehaviour
{
	// Token: 0x06009FAF RID: 40879 RVA: 0x003C46E3 File Offset: 0x003C28E3
	public void StartCountdown()
	{
		if (base.isActiveAndEnabled)
		{
			return;
		}
		base.enabled = true;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06009FB0 RID: 40880 RVA: 0x003C4701 File Offset: 0x003C2901
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this._animTime = 0f;
		this._animTimeDetonator = 0f;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06009FB1 RID: 40881 RVA: 0x003C4725 File Offset: 0x003C2925
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06009FB2 RID: 40882 RVA: 0x003C4734 File Offset: 0x003C2934
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		float num = Time.deltaTime;
		num *= this.PulseRateScale;
		this._animTime += num;
		this._animTimeDetonator += num * ((this._timeRate != null) ? this._timeRate.Evaluate(this._animTime) : 1f);
		if (this._light != null && this._lightIntensity != null)
		{
			this._light.intensity = this._lightIntensity.Evaluate(this._animTimeDetonator);
		}
	}

	// Token: 0x04007936 RID: 31030
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Light _light;

	// Token: 0x04007937 RID: 31031
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve _timeRate;

	// Token: 0x04007938 RID: 31032
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve _lightIntensity;

	// Token: 0x04007939 RID: 31033
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _animTime;

	// Token: 0x0400793A RID: 31034
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _animTimeDetonator;

	// Token: 0x0400793B RID: 31035
	public float PulseRateScale = 1f;
}
