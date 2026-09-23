using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200060D RID: 1549
[Preserve]
public class Fluctuating : LightState
{
	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x06003274 RID: 12916 RVA: 0x00149457 File Offset: 0x00147657
	public override float LODThreshold
	{
		get
		{
			return 0.2f;
		}
	}

	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x06003275 RID: 12917 RVA: 0x0014945E File Offset: 0x0014765E
	public override float Intensity
	{
		get
		{
			return this.currentIntensity;
		}
	}

	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x06003276 RID: 12918 RVA: 0x00149466 File Offset: 0x00147666
	public override float Emissive
	{
		get
		{
			return this.currentEmissive;
		}
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x0014946E File Offset: 0x0014766E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.startIntensity = 1f;
		this.fixedFrameRate = 1f / Time.fixedDeltaTime;
		this.currentIntensity = this.startIntensity;
		this.currentEmissive = this.startIntensity;
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x001494AC File Offset: 0x001476AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (GameManager.Instance.IsPaused())
		{
			return;
		}
		if (this.canSwitchProcess)
		{
			this.ChangeProcess();
			this.currentFrame = 0;
			int num = (int)(this.lightLOD.FluxDelay * this.fixedFrameRate);
			if (this.process == 0)
			{
				this.numOfFrames = UnityEngine.Random.Range(num / 2, num);
				this.t = 0f;
				this.preSlideIntenisty = this.currentIntensity;
				this.up = (UnityEngine.Random.Range(0f, 1f) > this.slideProbability(this.preSlideIntenisty));
				if (this.up)
				{
					this.slideTo = UnityEngine.Random.Range(this.preSlideIntenisty, 1f);
				}
				else
				{
					this.slideTo = UnityEngine.Random.Range(0.2f, this.preSlideIntenisty);
				}
				this.increment = (this.slideTo - this.preSlideIntenisty) / (float)this.numOfFrames;
			}
			else if (this.process == 1)
			{
				this.numOfFrames = UnityEngine.Random.Range(90, 181);
			}
			else
			{
				this.numOfFrames = UnityEngine.Random.Range(num / 2, num);
			}
		}
		if (this.process == 0)
		{
			this.Slide();
		}
		else if (this.process == 1)
		{
			this.Flutter();
		}
		else if (this.canSwitchProcess)
		{
			this.currentIntensity = this.startIntensity;
			this.currentEmissive = this.startIntensity / 1f;
		}
		int num2 = this.currentFrame + 1;
		this.currentFrame = num2;
		this.canSwitchProcess = (num2 >= this.numOfFrames);
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00149630 File Offset: 0x00147830
	[PublicizedFrom(EAccessModifier.Private)]
	public void Slide()
	{
		if (this.up)
		{
			this.currentIntensity = Mathf.Lerp(this.preSlideIntenisty, this.slideTo, this.t);
			this.currentEmissive = this.currentIntensity / 1f;
			this.t += this.increment;
			return;
		}
		this.currentIntensity = Mathf.Lerp(this.slideTo, this.preSlideIntenisty, this.t);
		this.currentEmissive = this.currentIntensity / 1f;
		this.t -= this.increment;
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x001496CC File Offset: 0x001478CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Flutter()
	{
		int num = UnityEngine.Random.Range(0, 3);
		if (num == 0)
		{
			this.currentIntensity = Mathf.Clamp(this.currentIntensity + 0.0625f, 0.2f, 1f);
			this.currentEmissive = this.currentIntensity / 1f;
			return;
		}
		if (num != 1)
		{
			return;
		}
		this.currentIntensity = Mathf.Clamp(this.currentIntensity - 0.0625f, 0.2f, 1f);
		this.currentEmissive = this.currentIntensity / 1f;
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x00149750 File Offset: 0x00147950
	[PublicizedFrom(EAccessModifier.Private)]
	public float slideProbability(float intensity)
	{
		return (intensity - 0.2f) / 0.8f;
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x00149760 File Offset: 0x00147960
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeProcess()
	{
		int num = UnityEngine.Random.Range(1, 3);
		if (num != this.process)
		{
			this.process = num;
			return;
		}
		if (this.process > 0)
		{
			this.process = (this.process + 1) % 3;
			return;
		}
		this.process++;
	}

	// Token: 0x0400280B RID: 10251
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float unityLightIntensityMax = 8f;

	// Token: 0x0400280C RID: 10252
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float hiRange = 1f;

	// Token: 0x0400280D RID: 10253
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float loRange = 0.2f;

	// Token: 0x0400280E RID: 10254
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float flutterVariance = 0.0625f;

	// Token: 0x0400280F RID: 10255
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float increment;

	// Token: 0x04002810 RID: 10256
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startIntensity;

	// Token: 0x04002811 RID: 10257
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fixedFrameRate;

	// Token: 0x04002812 RID: 10258
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentIntensity;

	// Token: 0x04002813 RID: 10259
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentEmissive;

	// Token: 0x04002814 RID: 10260
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canSwitchProcess = true;

	// Token: 0x04002815 RID: 10261
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int process;

	// Token: 0x04002816 RID: 10262
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numOfFrames;

	// Token: 0x04002817 RID: 10263
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentFrame;

	// Token: 0x04002818 RID: 10264
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float t;

	// Token: 0x04002819 RID: 10265
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float preSlideIntenisty;

	// Token: 0x0400281A RID: 10266
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float slideTo;

	// Token: 0x0400281B RID: 10267
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool up;
}
