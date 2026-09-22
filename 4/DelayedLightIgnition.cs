using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class DelayedLightIgnition : MonoBehaviour
{
	// Token: 0x0600023D RID: 573 RVA: 0x00012C12 File Offset: 0x00010E12
	public void Awake()
	{
		this.myLight = base.GetComponent<Light>();
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00012C20 File Offset: 0x00010E20
	public void Start()
	{
		this.myLight.enabled = false;
		this.timer = this.delay;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00012C3C File Offset: 0x00010E3C
	public void Update()
	{
		if (this.myLight != null && !this.myLight.enabled)
		{
			if (this.timer <= 0f)
			{
				this.myLight.enabled = true;
			}
			this.timer -= Time.deltaTime;
		}
	}

	// Token: 0x040002F6 RID: 758
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light myLight;

	// Token: 0x040002F7 RID: 759
	public float delay = 0.5f;

	// Token: 0x040002F8 RID: 760
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timer;
}
