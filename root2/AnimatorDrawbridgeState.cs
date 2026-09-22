using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
public class AnimatorDrawbridgeState : AnimatorDoorState
{
	// Token: 0x0600030A RID: 778 RVA: 0x00017407 File Offset: 0x00015607
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.mask = LayerMask.GetMask(new string[]
		{
			"Physics",
			"CC Physics",
			"CC Local Physics"
		});
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00017438 File Offset: 0x00015638
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CheckForObstacles()
	{
		if (this.colliders == null)
		{
			return false;
		}
		for (int i = 0; i < this.colliders.Length; i++)
		{
			EntityCollisionRules entityCollisionRules = this.rules[i];
			if (!entityCollisionRules || !entityCollisionRules.IsStatic)
			{
				Vector3 halfExtents = this.colliders[i].bounds.extents;
				if (this.colliders[i] is MeshCollider)
				{
					halfExtents = Vector3.Scale(((MeshCollider)this.colliders[i]).sharedMesh.bounds.extents, this.colliders[i].transform.localScale);
				}
				if (Physics.OverlapBoxNonAlloc(this.colliders[i].bounds.center, halfExtents, AnimatorDrawbridgeState.overlapBoxHits, this.colliders[i].transform.rotation, this.mask) > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PushPlayers(float _normalizedTime)
	{
	}

	// Token: 0x040003A4 RID: 932
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Collider[] overlapBoxHits = new Collider[20];

	// Token: 0x040003A5 RID: 933
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LayerMask mask;
}
