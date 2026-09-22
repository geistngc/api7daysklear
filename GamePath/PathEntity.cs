using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E0 RID: 6368
	public class PathEntity
	{
		// Token: 0x0600C474 RID: 50292 RVA: 0x0048A6A0 File Offset: 0x004888A0
		public void Destruct()
		{
			PathPoint[] array = this.points;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Release();
			}
			this.points = null;
		}

		// Token: 0x0600C475 RID: 50293 RVA: 0x0048A6D1 File Offset: 0x004888D1
		public void SetPoints(PathPoint[] _points)
		{
			this.points = _points;
			this.pathLength = _points.Length;
		}

		// Token: 0x0600C476 RID: 50294 RVA: 0x0048A6E3 File Offset: 0x004888E3
		public bool HasPoints()
		{
			return this.points != null;
		}

		// Token: 0x0600C477 RID: 50295 RVA: 0x0048A6EE File Offset: 0x004888EE
		public bool isFinished()
		{
			return this.currentPathIndex >= this.pathLength;
		}

		// Token: 0x0600C478 RID: 50296 RVA: 0x0048A701 File Offset: 0x00488901
		public int NodeCountRemaining()
		{
			return this.pathLength - this.currentPathIndex;
		}

		// Token: 0x0600C479 RID: 50297 RVA: 0x0048A710 File Offset: 0x00488910
		public Vector3 GetEndPos()
		{
			if (this.pathLength > 0)
			{
				return this.points[this.pathLength - 1].projectedLocation;
			}
			return this.rawEndPos;
		}

		// Token: 0x0600C47A RID: 50298 RVA: 0x0048A736 File Offset: 0x00488936
		public PathPoint GetEndPoint()
		{
			if (this.pathLength > 0)
			{
				return this.points[this.pathLength - 1];
			}
			return null;
		}

		// Token: 0x0600C47B RID: 50299 RVA: 0x0048A754 File Offset: 0x00488954
		public void ShortenEnd(float _distance)
		{
			if (this.pathLength >= 2)
			{
				PathPoint pathPoint = this.points[this.pathLength - 2];
				this.points[this.pathLength - 1].projectedLocation = pathPoint.projectedLocation;
			}
		}

		// Token: 0x0600C47C RID: 50300 RVA: 0x0048A794 File Offset: 0x00488994
		public PathPoint getPathPointFromIndex(int _idx)
		{
			return this.points[_idx];
		}

		// Token: 0x0600C47D RID: 50301 RVA: 0x0048A79E File Offset: 0x0048899E
		public int getCurrentPathLength()
		{
			return this.pathLength;
		}

		// Token: 0x0600C47E RID: 50302 RVA: 0x0048A7A6 File Offset: 0x004889A6
		public void setCurrentPathLength(int _length)
		{
			this.pathLength = _length;
		}

		// Token: 0x0600C47F RID: 50303 RVA: 0x0048A7AF File Offset: 0x004889AF
		public int getCurrentPathIndex()
		{
			return this.currentPathIndex;
		}

		// Token: 0x0600C480 RID: 50304 RVA: 0x0048A7B7 File Offset: 0x004889B7
		public void setCurrentPathIndex(int _idx, Entity entity, Vector3 entityPos)
		{
			this.currentPathIndex = _idx;
		}

		// Token: 0x1700183F RID: 6207
		// (get) Token: 0x0600C481 RID: 50305 RVA: 0x0048A7C0 File Offset: 0x004889C0
		public PathPoint CurrentPoint
		{
			get
			{
				return this.points[this.currentPathIndex];
			}
		}

		// Token: 0x17001840 RID: 6208
		// (get) Token: 0x0600C482 RID: 50306 RVA: 0x0048A7D0 File Offset: 0x004889D0
		public PathPoint NextPoint
		{
			get
			{
				int num = this.currentPathIndex + 1;
				if (num >= this.points.Length)
				{
					return null;
				}
				return this.points[num];
			}
		}

		// Token: 0x0600C483 RID: 50307 RVA: 0x0048A7FC File Offset: 0x004889FC
		public override bool Equals(object _other)
		{
			if (!(_other is PathEntity) || _other == null)
			{
				return false;
			}
			PathEntity pathEntity = (PathEntity)_other;
			if (pathEntity.points.Length != this.points.Length)
			{
				return false;
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				if (!this.points[i].IsSamePos(pathEntity.points[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600C484 RID: 50308 RVA: 0x0048A860 File Offset: 0x00488A60
		public override int GetHashCode()
		{
			if (this.points == null)
			{
				return 0;
			}
			int num = 0;
			foreach (PathPoint pathPoint in this.points)
			{
				num += pathPoint.GetHashCode();
			}
			return num;
		}

		// Token: 0x040094B4 RID: 38068
		public PathPoint[] points;

		// Token: 0x040094B5 RID: 38069
		public Vector3 toPos;

		// Token: 0x040094B6 RID: 38070
		public Vector3 rawEndPos;

		// Token: 0x040094B7 RID: 38071
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentPathIndex;

		// Token: 0x040094B8 RID: 38072
		[PublicizedFrom(EAccessModifier.Private)]
		public int pathLength;
	}
}
