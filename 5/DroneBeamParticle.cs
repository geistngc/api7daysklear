using System;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class DroneBeamParticle : MonoBehaviour
{
	// Token: 0x06001DFF RID: 7679 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000B65C8 File Offset: 0x000B47C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!this.drone)
		{
			this.drone = base.GetComponentInParent<EntityDrone>();
			Transform transform = base.transform.parent.FindInChilds("WristLeft", false);
			if (transform)
			{
				base.transform.SetParent(transform, false);
			}
			return;
		}
		if (this.displayTime > 0f)
		{
			this.displayTime -= Time.deltaTime;
			EntityAlive attackTargetLocal = this.drone.GetAttackTargetLocal();
			if (attackTargetLocal)
			{
				base.transform.rotation = Quaternion.LookRotation(attackTargetLocal.getChestPosition() - this.drone.GetHealArmPosition());
				if (attackTargetLocal.IsDead())
				{
					this.displayTime = 0f;
				}
			}
			if (this.displayTime <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000B669F File Offset: 0x000B489F
	public void SetDisplayTime(float time)
	{
		this.displayTime = time;
	}

	// Token: 0x04001407 RID: 5127
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone drone;

	// Token: 0x04001408 RID: 5128
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float displayTime;
}
