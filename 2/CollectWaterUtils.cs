using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CB2 RID: 3250
public static class CollectWaterUtils
{
	// Token: 0x060063DD RID: 25565 RVA: 0x00273214 File Offset: 0x00271414
	public static int CollectWater(ChunkCluster cc, int requiredMass, Vector3i origin, int maxRadius, List<CollectWaterUtils.WaterPoint> points)
	{
		int num = requiredMass;
		for (int i = 0; i <= maxRadius; i++)
		{
			int num2 = 0;
			int num3 = 0;
			foreach (Vector3i pos in CollectWaterUtils.GenerateCollectionPositions(origin, i))
			{
				WaterValue water = cc.GetWater(pos);
				if (water.HasMass())
				{
					int mass = water.GetMass();
					points.Add(new CollectWaterUtils.WaterPoint(pos, mass));
					num2 += mass;
					num3++;
				}
			}
			if (num2 > num)
			{
				int j = num2 - num;
				int a = (num2 - num) / num3;
				a = Mathf.Max(a, 1);
				int num4 = points.Count - num3;
				while (j > 0)
				{
					CollectWaterUtils.WaterPoint waterPoint = points[num4];
					if (waterPoint.massToTake > 0)
					{
						int num5 = Mathf.Min(a, waterPoint.massToTake);
						waterPoint.massToTake -= num5;
						j -= num5;
						num2 -= num5;
						points[num4] = waterPoint;
					}
					num4++;
					if (num4 == points.Count)
					{
						num4 = points.Count - num3;
					}
				}
			}
			num -= num2;
			if (num <= 0)
			{
				break;
			}
		}
		return requiredMass - num;
	}

	// Token: 0x060063DE RID: 25566 RVA: 0x00273350 File Offset: 0x00271550
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCollectionPositions(Vector3i center, int radius)
	{
		if (radius <= 0)
		{
			yield return center;
			yield break;
		}
		int num;
		for (int x = -radius; x <= radius; x = num + 1)
		{
			int remainingYZ = radius - Mathf.Abs(x);
			for (int y = -remainingYZ; y <= remainingYZ; y = num + 1)
			{
				int zAbs = remainingYZ - Mathf.Abs(y);
				if (zAbs == 0)
				{
					yield return new Vector3i(center.x + x, center.y + y, center.z);
				}
				else
				{
					yield return new Vector3i(center.x + x, center.y + y, center.z + zAbs);
					yield return new Vector3i(center.x + x, center.y + y, center.z - zAbs);
				}
				num = y;
			}
			num = x;
		}
		yield break;
	}

	// Token: 0x02000CB3 RID: 3251
	public struct WaterPoint
	{
		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x060063DF RID: 25567 RVA: 0x00273367 File Offset: 0x00271567
		public int finalMass
		{
			get
			{
				return this.mass - this.massToTake;
			}
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x00273376 File Offset: 0x00271576
		public WaterPoint(Vector3i _pos, int _mass)
		{
			this.worldPos = _pos;
			this.mass = _mass;
			this.massToTake = _mass;
		}

		// Token: 0x04004DA4 RID: 19876
		public Vector3i worldPos;

		// Token: 0x04004DA5 RID: 19877
		public int mass;

		// Token: 0x04004DA6 RID: 19878
		public int massToTake;
	}
}
