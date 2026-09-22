using System;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018DF RID: 6367
	[PublicizedFrom(EAccessModifier.Internal)]
	public class ASPPathNavigate : PathNavigate
	{
		// Token: 0x0600C46C RID: 50284 RVA: 0x0048A2DB File Offset: 0x004884DB
		public ASPPathNavigate(EntityAlive _ea) : base(_ea)
		{
		}

		// Token: 0x0600C46D RID: 50285 RVA: 0x0048A2E4 File Offset: 0x004884E4
		public override void GetPathTo(PathInfo _pathInfo)
		{
			if (this.pathFinder != null)
			{
				this.pathFinder.Cancel();
				this.pathFinder = null;
			}
			this.pathInfo = _pathInfo;
			if (!base.canNavigate())
			{
				return;
			}
			this.CreatePath();
		}

		// Token: 0x0600C46E RID: 50286 RVA: 0x0048A318 File Offset: 0x00488518
		public override bool SetPath(PathInfo _pathInfo, float _speed)
		{
			PathEntity pathEntity = (_pathInfo != null) ? _pathInfo.path : null;
			if (pathEntity == null)
			{
				if (this.currentPath != null)
				{
					this.currentPath.Destruct();
				}
				this.currentPath = null;
				return false;
			}
			if (this.currentPath != null)
			{
				this.currentPath.Destruct();
			}
			this.currentPath = pathEntity;
			if (this.currentPath.getCurrentPathLength() == 0)
			{
				return true;
			}
			this.ImprovePath();
			this.speed = _speed;
			this.canBreakBlocks = _pathInfo.canBreakBlocks;
			return true;
		}

		// Token: 0x0600C46F RID: 50287 RVA: 0x0048A394 File Offset: 0x00488594
		[PublicizedFrom(EAccessModifier.Private)]
		public void ImprovePath()
		{
			PathPoint[] points = this.currentPath.points;
			int num = points.Length;
			for (int i = 0; i < num; i++)
			{
				points[i].ProjectToGround(this.theEntity);
			}
			if (num >= 2)
			{
				Vector3 projectedLocation = points[0].projectedLocation;
				Vector3 projectedLocation2 = points[1].projectedLocation;
				if (projectedLocation2.y - projectedLocation.y < 0.6f)
				{
					points[0].projectedLocation = VectorMath.ClosestPointOnSegment(projectedLocation, projectedLocation2, this.theEntity.position);
				}
			}
		}

		// Token: 0x0600C470 RID: 50288 RVA: 0x0048A413 File Offset: 0x00488613
		public override void UpdateNavigation()
		{
			if (base.noPath())
			{
				return;
			}
			this.pathFollow();
			if (base.noPath())
			{
				return;
			}
			this.theEntity.moveHelper.SetMoveTo(this.currentPath, this.speed, this.canBreakBlocks);
		}

		// Token: 0x0600C471 RID: 50289 RVA: 0x0048A450 File Offset: 0x00488650
		[PublicizedFrom(EAccessModifier.Private)]
		public void pathFollow()
		{
			Vector3 vector = this.currentPath.CurrentPoint.ProjectToGround(this.theEntity);
			Vector3 position = this.theEntity.position;
			Vector3 b = VectorMath.ClosestPointOnSegment(this.theEntity.prevPos, position, vector);
			Vector3 vector2 = vector - b;
			float num = Utils.FastAbs(vector2.y);
			vector2.y = 0f;
			float num2 = this.theEntity.radius * 0.6f;
			float v = 0.15f;
			float num3 = 2f;
			int currentPathIndex = this.currentPath.getCurrentPathIndex();
			int currentPathLength = this.currentPath.getCurrentPathLength();
			if (currentPathIndex + 1 < currentPathLength)
			{
				v = ((this.theEntity.moveHelper.SideStepAngle != 0f) ? 0.49f : 0.33f);
			}
			if (this.theEntity.isSwimming)
			{
				v = 0.9f;
				num3 = 0.7f;
			}
			if (this.theEntity.IsInElevator())
			{
				num3 = 0.2f;
			}
			num2 = Utils.FastMax(v, num2);
			bool flag = false;
			PathPoint nextPoint = this.currentPath.NextPoint;
			if (nextPoint != null)
			{
				Vector3 vector3 = nextPoint.ProjectToGround(this.theEntity);
				if ((VectorMath.ClosestPointOnSegment(vector, vector3, position) - position).sqrMagnitude < 0.040000003f)
				{
					flag = true;
				}
				if (vector.y - vector3.y > 2f)
				{
					Plane plane = new Plane(vector3 - vector, vector);
					if (plane.SameSide(position, vector3))
					{
						flag = true;
					}
				}
			}
			if (flag || (vector2.sqrMagnitude <= num2 * num2 && num <= num3))
			{
				if (currentPathIndex + 1 < currentPathLength)
				{
					this.currentPath.setCurrentPathIndex(currentPathIndex + 1, this.theEntity, position);
					return;
				}
				this.currentPath.setCurrentPathIndex(currentPathLength, this.theEntity, position);
			}
		}

		// Token: 0x0600C472 RID: 50290 RVA: 0x0048A61C File Offset: 0x0048881C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void CreatePath()
		{
			EntityAlive theEntity = this.theEntity;
			this.pathFinder = new ASPPathFinder(this.pathInfo, this.canDrown, theEntity.bCanClimbLadders, theEntity.bCanClimbVertical);
			if (this.pathInfo.hasStart)
			{
				this.pathFinder.Calculate(this.pathInfo.startPos);
				return;
			}
			Vector3 fromPos = theEntity.position + theEntity.motion * 2.5f;
			this.pathFinder.Calculate(fromPos);
		}

		// Token: 0x040094B3 RID: 38067
		[PublicizedFrom(EAccessModifier.Private)]
		public ASPPathFinder pathFinder;
	}
}
