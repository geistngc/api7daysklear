using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class AutoTurretYawLerp : MonoBehaviour
{
	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06001D5F RID: 7519 RVA: 0x000B15A8 File Offset: 0x000AF7A8
	// (set) Token: 0x06001D60 RID: 7520 RVA: 0x000B15B0 File Offset: 0x000AF7B0
	public float Yaw { get; set; }

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06001D61 RID: 7521 RVA: 0x000B15BC File Offset: 0x000AF7BC
	public float CurrentYaw
	{
		get
		{
			return this.myTransform.localRotation.eulerAngles.y - this.BaseRotation.y;
		}
	}

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06001D62 RID: 7522 RVA: 0x000B15ED File Offset: 0x000AF7ED
	// (set) Token: 0x06001D63 RID: 7523 RVA: 0x000B15F5 File Offset: 0x000AF7F5
	public bool IsTurning { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06001D64 RID: 7524 RVA: 0x000B1600 File Offset: 0x000AF800
	public void Init(DynamicProperties _properties)
	{
		if (_properties.Values.ContainsKey("TurnSpeed"))
		{
			this.degreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeed"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("TurnSpeedIdle"))
		{
			this.idleDegreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeedIdle"], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x000B1675 File Offset: 0x000AF875
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.myTransform = base.transform;
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x000B1683 File Offset: 0x000AF883
	public void SetYaw()
	{
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x, this.BaseRotation.y + this.Yaw, this.BaseRotation.z);
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x000B16C0 File Offset: 0x000AF8C0
	public void UpdateYaw()
	{
		float num = Mathf.LerpAngle(this.myTransform.localRotation.eulerAngles.y, this.BaseRotation.y + this.Yaw, Time.deltaTime * (this.IdleScan ? this.idleDegreesPerSecond : this.degreesPerSecond));
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x, num, this.BaseRotation.z);
		this.IsTurning = ((int)num != (int)((this.BaseRotation.y + this.Yaw) * 100f));
	}

	// Token: 0x04001350 RID: 4944
	public Vector3 BaseRotation = Vector3.zero;

	// Token: 0x04001352 RID: 4946
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float degreesPerSecond = 11.25f;

	// Token: 0x04001353 RID: 4947
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleDegreesPerSecond = 0.5f;

	// Token: 0x04001354 RID: 4948
	public bool IdleScan;

	// Token: 0x04001355 RID: 4949
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform myTransform;
}
