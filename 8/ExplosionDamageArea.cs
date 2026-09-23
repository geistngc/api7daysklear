using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000545 RID: 1349
public class ExplosionDamageArea : MonoBehaviour
{
	// Token: 0x06002C4F RID: 11343 RVA: 0x00118BD9 File Offset: 0x00116DD9
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x00118BF0 File Offset: 0x00116DF0
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity getEntityFromCollider(Collider col)
	{
		Transform transform = col.transform;
		if (!transform.tag.StartsWith("E_") && !transform.CompareTag("Item"))
		{
			return null;
		}
		if (transform.CompareTag("Item"))
		{
			return null;
		}
		Transform transform2 = null;
		if (transform.tag.StartsWith("E_BP_"))
		{
			transform2 = GameUtils.GetHitRootTransform(transform.tag, transform);
		}
		EntityAlive entityAlive = (transform2 != null) ? transform2.GetComponent<EntityAlive>() : null;
		if (entityAlive == null || entityAlive.IsDead())
		{
			return null;
		}
		return entityAlive;
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x00118C7C File Offset: 0x00116E7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		EntityAlive entityAlive = this.getEntityFromCollider(other) as EntityAlive;
		if (entityAlive == null)
		{
			return;
		}
		if (this.BuffActions != null)
		{
			for (int i = 0; i < this.BuffActions.Count; i++)
			{
				entityAlive.Buffs.AddBuff(this.BuffActions[i], this.InitiatorEntityId, entityAlive.isEntityRemote, false, -1f);
			}
		}
	}

	// Token: 0x040021FC RID: 8700
	public List<string> BuffActions;

	// Token: 0x040021FD RID: 8701
	public int InitiatorEntityId = -1;
}
