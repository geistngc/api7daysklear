using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200060B RID: 1547
[Preserve]
public class Blinking : LightState
{
	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x06003267 RID: 12903 RVA: 0x00149380 File Offset: 0x00147580
	public override float LODThreshold
	{
		get
		{
			return 0.75f;
		}
	}

	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x06003268 RID: 12904 RVA: 0x00149387 File Offset: 0x00147587
	public override bool CanBeOn
	{
		get
		{
			return this.switchedOn;
		}
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x06003269 RID: 12905 RVA: 0x0014938F File Offset: 0x0014758F
	public override float Emissive
	{
		get
		{
			if (!this.switchedOn)
			{
				return 0f;
			}
			return 1f;
		}
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x001493A4 File Offset: 0x001475A4
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator blink()
	{
		for (;;)
		{
			this.switchedOn = !this.switchedOn;
			yield return new WaitForSeconds(1f / this.lightLOD.StateRate);
		}
		yield break;
	}

	// Token: 0x0600326B RID: 12907 RVA: 0x001493B3 File Offset: 0x001475B3
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		base.StartCoroutine(this.blink());
	}

	// Token: 0x0600326C RID: 12908 RVA: 0x001493C2 File Offset: 0x001475C2
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x04002807 RID: 10247
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool switchedOn = true;
}
