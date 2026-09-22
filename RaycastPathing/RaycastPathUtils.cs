using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018D5 RID: 6357
	[Preserve]
	public class RaycastPathUtils
	{
		// Token: 0x0600C415 RID: 50197 RVA: 0x00487A60 File Offset: 0x00485C60
		public static bool IsPositionBlocked(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask = 0, bool debugDraw = false)
		{
			Vector3 direction = end - start;
			return RaycastPathUtils.IsPositionBlocked(new Ray(start - Origin.position, direction), out hit, layerMask, debugDraw, direction.magnitude + 1f);
		}

		// Token: 0x0600C416 RID: 50198 RVA: 0x00487A9C File Offset: 0x00485C9C
		public static bool IsPositionBlocked(Vector3 start, Vector3 end, int layerMask = 0, bool debugDraw = false)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.IsPositionBlocked(start, end, out raycastHit, layerMask, debugDraw);
		}

		// Token: 0x0600C417 RID: 50199 RVA: 0x00487AB4 File Offset: 0x00485CB4
		public static bool IsPointBlocked(Vector3 start, Vector3 end, int layerMask = 0, bool debugDraw = false, float duration = 0f)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.CheckPositionBlocked(start, end, out raycastHit, layerMask, debugDraw, duration);
		}

		// Token: 0x0600C418 RID: 50200 RVA: 0x00487AD0 File Offset: 0x00485CD0
		public static bool CheckPositionBlocked(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float duration = 0f)
		{
			Vector3 direction = end - start;
			return RaycastPathUtils.CheckPositionBlocked(new Ray(start - Origin.position, direction), out hit, layerMask, debugDraw, direction.magnitude, duration);
		}

		// Token: 0x0600C419 RID: 50201 RVA: 0x00487B08 File Offset: 0x00485D08
		public static bool CheckPositionBlocked(Ray ray, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float maxDist = 100f, float duration = 0f)
		{
			bool flag = Physics.Raycast(ray, out hit, maxDist, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.magenta, Color.red, 1, duration);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * maxDist, Color.cyan, Color.blue, 1, duration);
				}
			}
			return flag;
		}

		// Token: 0x0600C41A RID: 50202 RVA: 0x00487B94 File Offset: 0x00485D94
		public static bool IsPositionBlocked(Ray ray, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float maxDist = 100f)
		{
			bool flag = Physics.Raycast(ray, out hit, maxDist, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.magenta, Color.red, 1, 5f);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * maxDist, Color.cyan, Color.blue, 1, 5f);
				}
			}
			return flag;
		}

		// Token: 0x0600C41B RID: 50203 RVA: 0x00487C28 File Offset: 0x00485E28
		public static bool IsPositionBlocked(Ray ray, int layerMask = 0, bool debugDraw = false, float maxDist = 100f)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.IsPositionBlocked(ray, out raycastHit, layerMask, debugDraw, 100f);
		}

		// Token: 0x0600C41C RID: 50204 RVA: 0x00487C44 File Offset: 0x00485E44
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1f)
		{
			Utils.DrawLine(start - Origin.position, end - Origin.position, color, color, 10, duration);
		}

		// Token: 0x0600C41D RID: 50205 RVA: 0x00487C68 File Offset: 0x00485E68
		public static void DrawBounds(Vector3i pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3((float)pos.x, (float)pos.y, (float)pos.z) - Origin.position, new Vector3((float)pos.x + scale, (float)pos.y + scale, (float)pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600C41E RID: 50206 RVA: 0x00487CCC File Offset: 0x00485ECC
		public static void DrawBounds(Vector3 pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3i(pos.x, pos.y, pos.z) - Origin.position, new Vector3i(pos.x + scale, pos.y + scale, pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600C41F RID: 50207 RVA: 0x00487D32 File Offset: 0x00485F32
		public static void DrawNode(RaycastNode node, Color color, float duration)
		{
			RaycastPathUtils.DrawNode(node.Min, node.Max, color, duration);
		}

		// Token: 0x0600C420 RID: 50208 RVA: 0x00487D47 File Offset: 0x00485F47
		public static void DrawNode(Vector3 min, Vector3 max, Color color, float duration)
		{
			RaycastPathUtils.DrawVolume(min - Origin.position, max - Origin.position, color, duration);
		}

		// Token: 0x0600C421 RID: 50209 RVA: 0x00487D68 File Offset: 0x00485F68
		[PublicizedFrom(EAccessModifier.Private)]
		public static void DrawVolume(Vector3 min, Vector3 max, Color color, float duration)
		{
			Vector3 vector = new Vector3(max.x, min.y, min.z);
			Vector3 vector2 = new Vector3(min.x, max.y, min.z);
			Vector3 end = new Vector3(min.x, min.y, max.z);
			Vector3 vector3 = new Vector3(min.x, max.y, max.z);
			Vector3 vector4 = new Vector3(max.x, min.y, max.z);
			Vector3 end2 = new Vector3(max.x, max.y, min.z);
			Debug.DrawLine(min, vector, color, duration);
			Debug.DrawLine(min, vector2, color, duration);
			Debug.DrawLine(min, end, color, duration);
			Debug.DrawLine(max, vector3, color, duration);
			Debug.DrawLine(max, vector4, color, duration);
			Debug.DrawLine(max, end2, color, duration);
			Debug.DrawLine(vector3, vector2, color, duration);
			Debug.DrawLine(vector, vector4, color, duration);
			Debug.DrawLine(vector4, end, color, duration);
			Debug.DrawLine(vector2, end2, color, duration);
			Debug.DrawLine(vector3, end, color, duration);
			Debug.DrawLine(vector, end2, color, duration);
		}

		// Token: 0x0400949C RID: 38044
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cMaxRayDist = 100;
	}
}
