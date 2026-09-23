using System;
using UnityEngine;

// Token: 0x02001416 RID: 5142
public class GeometryUtils
{
	// Token: 0x0600A185 RID: 41349 RVA: 0x003CB794 File Offset: 0x003C9994
	public static bool IntersectRayTriangle(Ray ray, GeometryUtils.Triangle tri, out Vector3 outNormal, out float hitDistance)
	{
		outNormal = Vector3.zero;
		hitDistance = 0f;
		Vector3 lhs = tri.v1 - tri.v0;
		Vector3 lhs2 = tri.v2 - tri.v0;
		Vector3 normal = tri.normal;
		float num = Vector3.Dot(normal, ray.direction);
		if (Mathf.Abs(num) < Mathf.Epsilon)
		{
			return false;
		}
		float num2 = (Vector3.Dot(normal, tri.v0) - Vector3.Dot(normal, ray.origin)) / num;
		if (num2 < 0f)
		{
			return false;
		}
		Vector3 vector = ray.origin + ray.direction * num2;
		if (Vector3.Dot(normal, Vector3.Cross(lhs, vector - tri.v0)) > 0f || Vector3.Dot(normal, Vector3.Cross(lhs2, tri.v2 - vector)) > 0f || Vector3.Dot(normal, Vector3.Cross(tri.v2 - tri.v1, vector - tri.v1)) > 0f)
		{
			return false;
		}
		hitDistance = num2;
		outNormal = normal.normalized;
		return true;
	}

	// Token: 0x0600A186 RID: 41350 RVA: 0x003CB8C4 File Offset: 0x003C9AC4
	public static Vector3 NearestPointOnLine(Vector3 fromPoint, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 lhs = fromPoint - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float d = Mathf.Clamp01(Vector3.Dot(lhs, vector) / Vector3.Dot(vector, vector));
		return lineStart + d * vector;
	}

	// Token: 0x0600A187 RID: 41351 RVA: 0x003CB904 File Offset: 0x003C9B04
	public static Vector3 NearestPointOnEdgeLoop(Vector3 fromPoint, Vector3[] loopPoints, int loopPointCount)
	{
		ValueTuple<float, Vector3> valueTuple = new ValueTuple<float, Vector3>(float.MaxValue, fromPoint);
		for (int i = 0; i < loopPointCount; i++)
		{
			Vector3 lineStart = loopPoints[i];
			Vector3 lineEnd = loopPoints[(i + 1) % loopPointCount];
			Vector3 vector = GeometryUtils.NearestPointOnLine(fromPoint, lineStart, lineEnd);
			float num = Vector3.Distance(fromPoint, vector);
			if (num < valueTuple.Item1)
			{
				valueTuple = new ValueTuple<float, Vector3>(num, vector);
			}
		}
		return valueTuple.Item2;
	}

	// Token: 0x0600A188 RID: 41352 RVA: 0x003CB96C File Offset: 0x003C9B6C
	public static void RotatePlaneAroundPoint(ref Plane plane, Vector3 pivot, Quaternion rotation)
	{
		if (rotation == Quaternion.identity)
		{
			return;
		}
		Vector3 vector = plane.normal;
		Vector3 vector2 = plane.normal * -plane.distance;
		vector2 = rotation * (vector2 - pivot) + pivot;
		vector = rotation * vector;
		plane.SetNormalAndPosition(vector, vector2);
	}

	// Token: 0x0600A189 RID: 41353 RVA: 0x003CB9C8 File Offset: 0x003C9BC8
	public static Rect RotateRectAboutY(Rect rect, float yRot)
	{
		Quaternion rotation = Quaternion.AngleAxis(yRot, Vector3.up);
		Vector3 vector = Vector3.zero;
		vector.x = rect.min.x - rect.center.x;
		vector.z = rect.min.y - rect.center.y;
		Vector3 vector2 = Vector3.zero;
		vector2.x = rect.max.x - rect.center.x;
		vector2.z = rect.max.y - rect.center.y;
		vector = rotation * vector;
		vector2 = rotation * vector2;
		vector.x += rect.center.x;
		vector.z += rect.center.y;
		vector2.x += rect.center.x;
		vector2.z += rect.center.y;
		if (vector2.x < vector.x)
		{
			float x = vector.x;
			vector.x = vector2.x;
			vector2.x = x;
		}
		if (vector2.z < vector.z)
		{
			float z = vector.z;
			vector.z = vector2.z;
			vector2.z = z;
		}
		return new Rect(new Vector2(vector.x, vector.z), new Vector2(vector2.x - vector.x, vector2.z - vector.z));
	}

	// Token: 0x02001417 RID: 5143
	public struct Triangle
	{
		// Token: 0x0600A18B RID: 41355 RVA: 0x003CBB60 File Offset: 0x003C9D60
		public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.v2 = v2;
			this.normal = Vector3.Normalize(Vector3.Cross(v0 - v2, v0 - v1));
		}

		// Token: 0x040079DB RID: 31195
		public Vector3 v0;

		// Token: 0x040079DC RID: 31196
		public Vector3 v1;

		// Token: 0x040079DD RID: 31197
		public Vector3 v2;

		// Token: 0x040079DE RID: 31198
		public Vector3 normal;
	}
}
