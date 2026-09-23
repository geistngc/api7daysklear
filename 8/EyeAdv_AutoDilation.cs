using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class EyeAdv_AutoDilation : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x00002C4F File Offset: 0x00000E4F
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.eyeRenderer = base.gameObject.GetComponent<Renderer>();
		if (this.sceneLightObject != null)
		{
			this.sceneLight = this.sceneLightObject.GetComponent<Light>();
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002C84 File Offset: 0x00000E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.sceneLight != null)
		{
			this.lightIntensity = this.sceneLight.intensity;
			if (this.enableAutoDilation)
			{
				if (this.currTargetDilation != this.targetDilation || this.currLightSensitivity != this.lightSensitivity)
				{
					this.dilateTime = 0f;
					this.currTargetDilation = this.targetDilation;
					this.currLightSensitivity = this.lightSensitivity;
				}
				this.lightAngle = Vector3.Angle(this.sceneLightObject.transform.forward, base.transform.forward) / 180f;
				this.targetDilation = Mathf.Lerp(1f, 0f, this.lightAngle * this.lightIntensity * this.lightSensitivity);
				this.dilateTime += Time.deltaTime * this.dilationSpeed;
				this.pupilDilation = Mathf.Clamp(this.pupilDilation, 0f, this.maxDilation);
				this.pupilDilation = Mathf.Lerp(this.pupilDilation, this.targetDilation, this.dilateTime);
				this.eyeRenderer.sharedMaterial.SetFloat("_pupilSize", this.pupilDilation);
			}
		}
	}

	// Token: 0x04000034 RID: 52
	public bool enableAutoDilation = true;

	// Token: 0x04000035 RID: 53
	public Transform sceneLightObject;

	// Token: 0x04000036 RID: 54
	public float lightSensitivity = 1f;

	// Token: 0x04000037 RID: 55
	public float dilationSpeed = 0.1f;

	// Token: 0x04000038 RID: 56
	public float maxDilation = 1f;

	// Token: 0x04000039 RID: 57
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light sceneLight;

	// Token: 0x0400003A RID: 58
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightIntensity;

	// Token: 0x0400003B RID: 59
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightAngle;

	// Token: 0x0400003C RID: 60
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dilateTime;

	// Token: 0x0400003D RID: 61
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float pupilDilation = 0.5f;

	// Token: 0x0400003E RID: 62
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currTargetDilation = -1f;

	// Token: 0x0400003F RID: 63
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetDilation;

	// Token: 0x04000040 RID: 64
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currLightSensitivity = -1f;

	// Token: 0x04000041 RID: 65
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer eyeRenderer;
}
