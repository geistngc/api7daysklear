using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x02001D3E RID: 7486
	public class MicroSplatRuntimeUtil
	{
		// Token: 0x0600DDB7 RID: 56759 RVA: 0x004F7F90 File Offset: 0x004F6190
		public static Vector2 UnityUVScaleToUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			uv.x = 1f / (uv.x / x);
			uv.y = 1f / (uv.y / z);
			return uv;
		}

		// Token: 0x0600DDB8 RID: 56760 RVA: 0x004F7FEC File Offset: 0x004F61EC
		public static Vector2 UVScaleToUnityUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			if (uv.x < 0f)
			{
				uv.x = 0.001f;
			}
			if (uv.y < 0f)
			{
				uv.y = 0.001f;
			}
			uv.x = x / uv.x;
			uv.y = z / uv.y;
			return uv;
		}
	}
}
