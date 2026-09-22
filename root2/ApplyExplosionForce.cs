using System;
using UnityEngine;

// Token: 0x02001383 RID: 4995
public class ApplyExplosionForce : MonoBehaviour
{
	// Token: 0x06009D9F RID: 40351 RVA: 0x003BB9A8 File Offset: 0x003B9BA8
	public static void Explode(Vector3 explosionPos, float power, float radius)
	{
		explosionPos -= Origin.position;
		power *= 20f;
		radius *= 1.75f;
		int num = Physics.OverlapSphereNonAlloc(explosionPos, radius, ApplyExplosionForce.colliderList);
		if (num > 1024)
		{
			num = 1024;
		}
		for (int i = 0; i < num; i++)
		{
			Rigidbody component = ApplyExplosionForce.colliderList[i].GetComponent<Rigidbody>();
			if (component)
			{
				component.AddExplosionForce(power, explosionPos, radius, 3f);
			}
		}
	}

	// Token: 0x040077F7 RID: 30711
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cUpwards = 3f;

	// Token: 0x040077F8 RID: 30712
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMaxColliders = 1024;

	// Token: 0x040077F9 RID: 30713
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Collider[] colliderList = new Collider[1024];
}
