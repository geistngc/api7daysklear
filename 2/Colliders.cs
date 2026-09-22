using System;
using UnityEngine;

// Token: 0x020013C1 RID: 5057
public class Colliders
{
	// Token: 0x06009F13 RID: 40723 RVA: 0x003C1EA0 File Offset: 0x003C00A0
	public static bool Trace(Rigidbody body, Vector3 dir, float distance, int layerMask, out RaycastHit hitInfo)
	{
		hitInfo = default(RaycastHit);
		if (layerMask == 0)
		{
			return false;
		}
		RaycastHit[] array = body.SweepTestAll(dir, distance);
		if (array.Length < 1)
		{
			return false;
		}
		float num = float.MaxValue;
		foreach (RaycastHit raycastHit in array)
		{
			int layer = raycastHit.collider.gameObject.layer;
			if ((1 << layer & layerMask) != 0 && raycastHit.distance < num)
			{
				hitInfo = raycastHit;
				num = hitInfo.distance;
			}
		}
		return hitInfo.collider != null;
	}

	// Token: 0x06009F14 RID: 40724 RVA: 0x003C1F2C File Offset: 0x003C012C
	public static RaycastHit[] TraceAll(Rigidbody body, Vector3 dir, float distance, int layerMask)
	{
		if (layerMask == 0)
		{
			return null;
		}
		RaycastHit[] array = body.SweepTestAll(dir, distance);
		if (array.Length < 1)
		{
			return null;
		}
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			int layer = array[i].collider.gameObject.layer;
			if ((1 << layer & layerMask) != 0)
			{
				num++;
			}
		}
		if (num < 1)
		{
			return null;
		}
		RaycastHit[] array2 = new RaycastHit[num];
		num = 0;
		foreach (RaycastHit raycastHit in array)
		{
			int layer2 = raycastHit.collider.gameObject.layer;
			if ((1 << layer2 & layerMask) != 0)
			{
				array2[num++] = raycastHit;
			}
		}
		return array2;
	}
}
