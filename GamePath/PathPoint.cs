using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018EC RID: 6380
	public class PathPoint
	{
		// Token: 0x0600C4AC RID: 50348 RVA: 0x0048AADC File Offset: 0x00488CDC
		public static PathPoint Allocate(Vector3 _pos)
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			PathPoint result;
			lock (s_pool)
			{
				PathPoint pathPoint = MemoryPools.s_pool.Allocate();
				pathPoint.x = (int)_pos.x;
				pathPoint.y = (int)_pos.y;
				pathPoint.z = (int)_pos.z;
				pathPoint.projectedLocation = _pos;
				pathPoint.hash = PathPoint.makeHash(pathPoint.x, pathPoint.y, pathPoint.z);
				result = pathPoint;
			}
			return result;
		}

		// Token: 0x0600C4AD RID: 50349 RVA: 0x0048AB70 File Offset: 0x00488D70
		public static void CompactPool()
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			lock (s_pool)
			{
				MemoryPools.s_pool.Compact();
			}
		}

		// Token: 0x0600C4AE RID: 50350 RVA: 0x0048ABB4 File Offset: 0x00488DB4
		public void Release()
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			lock (s_pool)
			{
				MemoryPools.s_pool.Free(this);
			}
		}

		// Token: 0x0600C4AF RID: 50351 RVA: 0x0048ABF8 File Offset: 0x00488DF8
		public static int makeHash(int _x, int _y, int _z)
		{
			return (_y & 255) | (_x & 32767) << 8 | (_z & 32767) << 24 | ((_x >= 0) ? 0 : int.MinValue) | ((_z >= 0) ? 0 : 32768);
		}

		// Token: 0x0600C4B0 RID: 50352 RVA: 0x0048AC30 File Offset: 0x00488E30
		public override bool Equals(object _obj)
		{
			PathPoint pathPoint = _obj as PathPoint;
			return pathPoint != null && this.hash == pathPoint.hash && this.IsSamePos(pathPoint);
		}

		// Token: 0x0600C4B1 RID: 50353 RVA: 0x0048AC60 File Offset: 0x00488E60
		public bool IsSamePos(PathPoint _p)
		{
			return _p.x == this.x && _p.y == this.y && _p.z == this.z;
		}

		// Token: 0x0600C4B2 RID: 50354 RVA: 0x0048AC8E File Offset: 0x00488E8E
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x0600C4B3 RID: 50355 RVA: 0x0048AC98 File Offset: 0x00488E98
		public float GetDistanceSq(int _x, int _y, int _z)
		{
			int num = this.x - _x;
			int num2 = this.y - _y;
			int num3 = this.z - _z;
			return (float)(num * num + num2 * num2 + num3 * num3);
		}

		// Token: 0x0600C4B4 RID: 50356 RVA: 0x0048ACCA File Offset: 0x00488ECA
		public Vector3 AdjustedPositionForEntity(Entity entity)
		{
			return this.projectedLocation;
		}

		// Token: 0x0600C4B5 RID: 50357 RVA: 0x0048ACCA File Offset: 0x00488ECA
		public Vector3 ProjectToGround(Entity entity)
		{
			return this.projectedLocation;
		}

		// Token: 0x0600C4B6 RID: 50358 RVA: 0x0048ACD2 File Offset: 0x00488ED2
		public Vector3i GetBlockPos()
		{
			return World.worldToBlockPos(this.projectedLocation);
		}

		// Token: 0x0600C4B7 RID: 50359 RVA: 0x0048ACE0 File Offset: 0x00488EE0
		public string toString()
		{
			return string.Concat(new string[]
			{
				this.x.ToString(),
				", ",
				this.y.ToString(),
				", ",
				this.z.ToString()
			});
		}

		// Token: 0x040094E8 RID: 38120
		public Vector3 projectedLocation;

		// Token: 0x040094E9 RID: 38121
		[PublicizedFrom(EAccessModifier.Private)]
		public int x;

		// Token: 0x040094EA RID: 38122
		[PublicizedFrom(EAccessModifier.Private)]
		public int y;

		// Token: 0x040094EB RID: 38123
		[PublicizedFrom(EAccessModifier.Private)]
		public int z;

		// Token: 0x040094EC RID: 38124
		[PublicizedFrom(EAccessModifier.Private)]
		public int hash;
	}
}
