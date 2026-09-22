using System;
using UnityEngine;

// Token: 0x020004FE RID: 1278
public class AudioSourceLifetimeSwitch : MonoBehaviour
{
	// Token: 0x060029CF RID: 10703 RVA: 0x00107D6E File Offset: 0x00105F6E
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		this.entityRoot = RootTransformRefEntity.FindEntityUpwards(base.transform);
		this.audio = base.transform.GetComponent<AudioSource>();
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x00107D94 File Offset: 0x00105F94
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.bFirstUpdate)
		{
			this.bFirstUpdate = false;
			this.entity = ((this.entityRoot != null) ? this.entityRoot.GetComponent<Entity>() : null);
			if (this.entity && this.entity.IsDead())
			{
				if (this.audio)
				{
					this.audio.enabled = false;
					this.audio = null;
				}
				this.entity = null;
			}
		}
		if (this.audio && !this.entity)
		{
			this.delay -= Time.deltaTime;
			if (this.delay <= 0f)
			{
				this.audio.enabled = false;
				this.audio = null;
			}
		}
		if (this.entity && this.entity.IsDead())
		{
			this.delay = this.TurnOffDelayAfterEntityDies;
			this.entity = null;
		}
	}

	// Token: 0x04001FDF RID: 8159
	public float TurnOffDelayAfterEntityDies = 5f;

	// Token: 0x04001FE0 RID: 8160
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform entityRoot;

	// Token: 0x04001FE1 RID: 8161
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity entity;

	// Token: 0x04001FE2 RID: 8162
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource audio;

	// Token: 0x04001FE3 RID: 8163
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float delay;

	// Token: 0x04001FE4 RID: 8164
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bFirstUpdate = true;
}
