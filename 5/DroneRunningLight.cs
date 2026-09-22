using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class DroneRunningLight : MonoBehaviour
{
	// Token: 0x06001E34 RID: 7732 RVA: 0x000B7CB1 File Offset: 0x000B5EB1
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.runningLight = base.GetComponent<Light>();
		this.particles = base.transform.GetComponentInChildren<ParticleSystem>();
		this._initLights();
		this.setLightsActive(true);
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x000B7CDD File Offset: 0x000B5EDD
	[PublicizedFrom(EAccessModifier.Private)]
	public void _initLights()
	{
		this.lightBlinkTimer = this.LightBlinkInterval;
		this.startIntensity = this.MinLightIntensity;
		this.runningLight.intensity = this.startIntensity;
		this.setLightColor(this.LightColor);
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000B7D14 File Offset: 0x000B5F14
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLightsActive(bool value)
	{
		this.runningLight.enabled = value;
		if (this.connectedLights != null)
		{
			for (int i = 0; i < this.connectedLights.Count; i++)
			{
				this.connectedLights[i].enabled = value;
			}
		}
		if (!this.dayTimeVisibility)
		{
			this.particles.gameObject.SetActive(value);
		}
		this.lightsActive = value;
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x000B7D80 File Offset: 0x000B5F80
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLightColor(Color color)
	{
		this.runningLight.color = color;
		this.particles.main.startColor = color;
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x000B7DB4 File Offset: 0x000B5FB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		float num = (float)GameUtils.WorldTimeToHours(world.worldTime);
		if (num > 4f && num < 22f)
		{
			if (this.runningLight.intensity > this.startIntensity)
			{
				this._initLights();
			}
			if (this.lightsActive)
			{
				this.setLightsActive(!this.lightsActive);
			}
			return;
		}
		if (!this.lightsActive)
		{
			this.setLightsActive(!this.lightsActive);
		}
		if (this.runningLight.color != this.LightColor || this.particles.main.startColor.color != this.LightColor)
		{
			this.setLightColor(this.LightColor);
		}
		if (this.startIntensity != this.MinLightIntensity)
		{
			this.startIntensity = this.MinLightIntensity;
		}
		if (this.lightBlinkTimer > 0f)
		{
			this.lightBlinkTimer -= Time.deltaTime;
			if (this.lightBlinkTimer < 0.2f && this.lightBlinkTimer > 0.15f && this.particles.gameObject.activeSelf)
			{
				this.particles.gameObject.SetActive(false);
			}
			if (this.lightBlinkTimer < 0.15f && !this.particles.gameObject.activeSelf)
			{
				this.particles.gameObject.SetActive(true);
			}
			if (this.lightBlinkTimer <= 0f)
			{
				base.StartCoroutine(this.blink());
			}
		}
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x000B7F46 File Offset: 0x000B6146
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator blink()
	{
		this.transitionTimer = this.transitionTime;
		while (this.runningLight.intensity < this.MaxLightIntensity)
		{
			this.transitionTimer += Time.deltaTime;
			this.runningLight.intensity = Mathf.Lerp(this.startIntensity, this.MaxLightIntensity, this.transitionTimer / this.transitionTime);
			yield return null;
		}
		this.transitionTimer = this.transitionTime;
		while (this.runningLight.intensity > this.startIntensity)
		{
			this.transitionTimer += Time.deltaTime;
			this.runningLight.intensity = Mathf.Lerp(this.MaxLightIntensity, this.startIntensity, this.transitionTimer / this.transitionTime);
			yield return null;
		}
		this.runningLight.intensity = this.startIntensity;
		this.lightBlinkTimer = this.LightBlinkInterval;
		yield return null;
		yield break;
	}

	// Token: 0x04001423 RID: 5155
	public float MinLightIntensity;

	// Token: 0x04001424 RID: 5156
	public float MaxLightIntensity;

	// Token: 0x04001425 RID: 5157
	public float LightBlinkInterval;

	// Token: 0x04001426 RID: 5158
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light runningLight;

	// Token: 0x04001427 RID: 5159
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startIntensity;

	// Token: 0x04001428 RID: 5160
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem particles;

	// Token: 0x04001429 RID: 5161
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightBlinkTimer;

	// Token: 0x0400142A RID: 5162
	public Color LightColor;

	// Token: 0x0400142B RID: 5163
	public List<Light> connectedLights;

	// Token: 0x0400142C RID: 5164
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lightsActive;

	// Token: 0x0400142D RID: 5165
	public bool dayTimeVisibility;

	// Token: 0x0400142E RID: 5166
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float transitionTime = 0.2f;

	// Token: 0x0400142F RID: 5167
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float transitionTimer;
}
