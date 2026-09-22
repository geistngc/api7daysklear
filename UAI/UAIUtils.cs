using System;
using System.Collections.Generic;
using UnityEngine;

namespace UAI
{
	// Token: 0x02001789 RID: 6025
	public static class UAIUtils
	{
		// Token: 0x0600BAA1 RID: 47777 RVA: 0x0045A160 File Offset: 0x00458360
		public static float DistanceSqr(Vector3 pointA, Vector3 pointB)
		{
			Vector3 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		// Token: 0x0600BAA2 RID: 47778 RVA: 0x0045A1A0 File Offset: 0x004583A0
		public static float DistanceSqr(Vector2 pointA, Vector2 pointB)
		{
			Vector2 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y;
		}

		// Token: 0x0600BAA3 RID: 47779 RVA: 0x0045A1D0 File Offset: 0x004583D0
		public static EntityAlive ConvertToEntityAlive(object obj)
		{
			EntityAlive result = null;
			try
			{
				result = (EntityAlive)obj;
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x0200178A RID: 6026
		public class NearestWaypointSorter : IComparer<Vector3>
		{
			// Token: 0x0600BAA4 RID: 47780 RVA: 0x0045A1FC File Offset: 0x004583FC
			public NearestWaypointSorter(Entity _self)
			{
				this.self = _self;
			}

			// Token: 0x0600BAA5 RID: 47781 RVA: 0x0045A20C File Offset: 0x0045840C
			public int Compare(Vector3 _obj1, Vector3 _obj2)
			{
				float distanceSq = this.self.GetDistanceSq(_obj1);
				float distanceSq2 = this.self.GetDistanceSq(_obj2);
				if (distanceSq < distanceSq2)
				{
					return -1;
				}
				if (distanceSq > distanceSq2)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04008C27 RID: 35879
			[PublicizedFrom(EAccessModifier.Private)]
			public Entity self;
		}

		// Token: 0x0200178B RID: 6027
		public class NearestEntitySorter : IComparer<Entity>
		{
			// Token: 0x0600BAA6 RID: 47782 RVA: 0x0045A240 File Offset: 0x00458440
			public NearestEntitySorter(Entity _self)
			{
				this.self = _self;
			}

			// Token: 0x0600BAA7 RID: 47783 RVA: 0x0045A250 File Offset: 0x00458450
			public int Compare(Entity _obj1, Entity _obj2)
			{
				float distanceSq = this.self.GetDistanceSq(_obj1);
				float distanceSq2 = this.self.GetDistanceSq(_obj2);
				if (distanceSq < distanceSq2)
				{
					return -1;
				}
				if (distanceSq > distanceSq2)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04008C28 RID: 35880
			[PublicizedFrom(EAccessModifier.Private)]
			public Entity self;
		}
	}
}
