using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class HazardDamageController : MonoBehaviour
{
	// Token: 0x0600061B RID: 1563 RVA: 0x0002B8F0 File Offset: 0x00029AF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!this.IsActive)
		{
			if (this.CollidersThisFrame != null && this.CollidersThisFrame.Count > 0)
			{
				this.CollidersThisFrame.Clear();
			}
			return;
		}
		if (this.CollidersThisFrame == null || this.CollidersThisFrame.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.CollidersThisFrame.Count; i++)
		{
			this.touched(this.CollidersThisFrame[i]);
		}
		this.CollidersThisFrame.Clear();
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0002B970 File Offset: 0x00029B70
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0002B970 File Offset: 0x00029B70
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerStay(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0002B970 File Offset: 0x00029B70
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerExit(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0002B9A8 File Offset: 0x00029BA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void touched(Collider collider)
	{
		if (!this.IsActive || collider == null)
		{
			return;
		}
		Transform transform = collider.transform;
		if (transform != null)
		{
			EntityAlive entityAlive = transform.GetComponent<EntityAlive>();
			if (entityAlive == null)
			{
				entityAlive = transform.GetComponentInParent<EntityAlive>();
			}
			if (entityAlive == null && transform.parent != null)
			{
				entityAlive = transform.parent.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive == null)
			{
				entityAlive = transform.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive != null && entityAlive.IsAlive() && this.buffActions != null)
			{
				for (int i = 0; i < this.buffActions.Count; i++)
				{
					if (entityAlive.emodel != null && entityAlive.emodel.transform != null && !entityAlive.Buffs.HasBuff(this.buffActions[i]))
					{
						entityAlive.Buffs.AddBuff(this.buffActions[i], -1, true, false, -1f);
					}
				}
			}
		}
	}

	// Token: 0x040006C8 RID: 1736
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Collider> CollidersThisFrame;

	// Token: 0x040006C9 RID: 1737
	public bool IsActive;

	// Token: 0x040006CA RID: 1738
	public List<string> buffActions;
}
