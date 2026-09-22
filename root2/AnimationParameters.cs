using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class AnimationParameters : MonoBehaviour
{
	// Token: 0x060002F1 RID: 753 RVA: 0x00016934 File Offset: 0x00014B34
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00016944 File Offset: 0x00014B44
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		this.currentRotation = base.transform.rotation;
		Quaternion quaternion = this.currentRotation * Quaternion.Inverse(this.previousRotation);
		this.previousRotation = this.currentRotation;
		float num;
		Vector3 a;
		quaternion.ToAngleAxis(out num, out a);
		num *= 0.017453292f;
		this.angularVelocity = 1f / Time.deltaTime * num * a;
		this.deltaYaw = Mathf.SmoothDamp(this.deltaYaw, this.angularVelocity.y, ref this.turnVelocity, this.deltaYawSmoothTime);
		if (this.debugMode)
		{
			if (Mathf.Abs(this.deltaYaw) > 0.001f)
			{
				Debug.Log("DeltaYaw: " + this.deltaYaw.ToString());
			}
			if (this.deltaYaw < this.deltaYawMin)
			{
				this.deltaYawMin = this.deltaYaw;
			}
			if (this.deltaYaw > this.deltaYawMax)
			{
				this.deltaYawMax = this.deltaYaw;
			}
		}
		this.anim.SetFloat("deltaYaw", this.deltaYaw);
		this.anim.SetFloat("TurnPlayRate", this.deltaYaw);
		if (Mathf.Abs(this.angularVelocity.y) > 0.1f)
		{
			this.anim.SetBool("Turning", true);
			return;
		}
		this.anim.SetBool("Turning", false);
	}

	// Token: 0x0400037E RID: 894
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator anim;

	// Token: 0x0400037F RID: 895
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 currentEulerAngles;

	// Token: 0x04000380 RID: 896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentRotation;

	// Token: 0x04000381 RID: 897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion previousRotation;

	// Token: 0x04000382 RID: 898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentYaw;

	// Token: 0x04000383 RID: 899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastYaw;

	// Token: 0x04000384 RID: 900
	public float turnPlayRateMultiplier;

	// Token: 0x04000385 RID: 901
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float turnPlayRate;

	// Token: 0x04000386 RID: 902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deltaYawTarget;

	// Token: 0x04000387 RID: 903
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deltaYaw;

	// Token: 0x04000388 RID: 904
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float angle;

	// Token: 0x04000389 RID: 905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 angularVelocity;

	// Token: 0x0400038A RID: 906
	public bool debugMode;

	// Token: 0x0400038B RID: 907
	public float deltaYawMin;

	// Token: 0x0400038C RID: 908
	public float deltaYawMax;

	// Token: 0x0400038D RID: 909
	public float deltaYawSmoothTime = 0.3f;

	// Token: 0x0400038E RID: 910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float turnVelocity;
}
