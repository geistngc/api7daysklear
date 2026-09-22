using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x02001D61 RID: 7521
	public static class Extensions
	{
		// Token: 0x0600DE5A RID: 56922 RVA: 0x004FC9C0 File Offset: 0x004FABC0
		public static Quaternion Append(this Quaternion source, Quaternion quaternion)
		{
			return quaternion * source;
		}

		// Token: 0x0600DE5B RID: 56923 RVA: 0x004FC9C9 File Offset: 0x004FABC9
		public static Quaternion FromToRotation(this Quaternion source, Quaternion target)
		{
			return Quaternion.Inverse(source) * target;
		}

		// Token: 0x0600DE5C RID: 56924 RVA: 0x004FC9D7 File Offset: 0x004FABD7
		public static Quaternion Scale(this Quaternion source, float scale)
		{
			return Quaternion.SlerpUnclamped(Quaternion.identity, source, scale);
		}

		// Token: 0x0600DE5D RID: 56925 RVA: 0x004FC9E5 File Offset: 0x004FABE5
		public static Quaternion Inverse(this Quaternion source)
		{
			return Quaternion.Inverse(source);
		}

		// Token: 0x0600DE5E RID: 56926 RVA: 0x004FC9F0 File Offset: 0x004FABF0
		public static List<Vector3> GetOrthogonalVectors(this Vector3 source, int numVectors)
		{
			Vector3 normalized = source.normalized;
			Vector3 point = (Mathf.Abs(source.normalized.y) != 1f) ? Vector3.Cross(source, Vector3.up) : Vector3.Cross(source, Vector3.right);
			float num = 360f / (float)numVectors;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < numVectors; i++)
			{
				list.Add(Quaternion.AngleAxis(num * (float)i, source) * point);
			}
			return list;
		}

		// Token: 0x0600DE5F RID: 56927 RVA: 0x004FCA68 File Offset: 0x004FAC68
		public static bool HasLength(this Vector3 source)
		{
			return source.x != 0f || source.y != 0f || source.z != 0f;
		}

		// Token: 0x0600DE60 RID: 56928 RVA: 0x004FCA96 File Offset: 0x004FAC96
		public static float Clamp01(this float source)
		{
			return Mathf.Max(Mathf.Min(source, 1f), 0f);
		}
	}
}
