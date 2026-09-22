using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020003CC RID: 972
public class AutoTurretPitchLerp : MonoBehaviour
{
	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06001D55 RID: 7509 RVA: 0x000B13EA File Offset: 0x000AF5EA
	// (set) Token: 0x06001D56 RID: 7510 RVA: 0x000B13F2 File Offset: 0x000AF5F2
	public float Pitch { get; set; }

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06001D57 RID: 7511 RVA: 0x000B13FC File Offset: 0x000AF5FC
	public float CurrentPitch
	{
		get
		{
			return this.myTransform.localRotation.eulerAngles.x - this.BaseRotation.x;
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06001D58 RID: 7512 RVA: 0x000B142D File Offset: 0x000AF62D
	// (set) Token: 0x06001D59 RID: 7513 RVA: 0x000B1435 File Offset: 0x000AF635
	public bool IsTurning { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06001D5A RID: 7514 RVA: 0x000B143E File Offset: 0x000AF63E
	public void Init(DynamicProperties _properties)
	{
		if (_properties.Values.ContainsKey("TurnSpeed"))
		{
			this.degreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeed"], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x000B1474 File Offset: 0x000AF674
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.myTransform = base.transform;
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x000B1482 File Offset: 0x000AF682
	public void SetPitch()
	{
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x + this.Pitch, this.BaseRotation.y, this.BaseRotation.z);
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x000B14BC File Offset: 0x000AF6BC
	public void UpdatePitch()
	{
		int num = (int)(this.myTransform.localRotation.eulerAngles.x * 1000f);
		this.myTransform.localRotation = Quaternion.Euler(Mathf.LerpAngle(this.myTransform.localRotation.eulerAngles.x, this.BaseRotation.x + this.Pitch, Time.deltaTime * ((this.IdleScan ? 0.25f : 1f) * this.degreesPerSecond)), this.BaseRotation.y, this.BaseRotation.z);
		this.IsTurning = ((int)(this.myTransform.localRotation.eulerAngles.x * 1000f) != num);
	}

	// Token: 0x0400134A RID: 4938
	public Vector3 BaseRotation = Vector3.zero;

	// Token: 0x0400134C RID: 4940
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float degreesPerSecond = 11.25f;

	// Token: 0x0400134D RID: 4941
	public bool IdleScan;

	// Token: 0x0400134E RID: 4942
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform myTransform;
}
