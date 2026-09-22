using System;

namespace GamePath
{
	// Token: 0x020018EB RID: 6379
	public class PathNavigate
	{
		// Token: 0x0600C49B RID: 50331 RVA: 0x0048A9F2 File Offset: 0x00488BF2
		public PathNavigate(EntityAlive _ea)
		{
			this.theEntity = _ea;
			this.inWater = false;
			this.canDrown = false;
		}

		// Token: 0x0600C49C RID: 50332 RVA: 0x0048AA0F File Offset: 0x00488C0F
		public void setMoveSpeed(float _b)
		{
			this.speed = _b;
		}

		// Token: 0x0600C49D RID: 50333 RVA: 0x0048AA18 File Offset: 0x00488C18
		public void setCanDrown(bool _b)
		{
			this.canDrown = _b;
		}

		// Token: 0x0600C49E RID: 50334 RVA: 0x0048AA21 File Offset: 0x00488C21
		public bool getCanDrown()
		{
			return this.canDrown;
		}

		// Token: 0x0600C49F RID: 50335 RVA: 0x0048AA29 File Offset: 0x00488C29
		public bool noPath()
		{
			return this.currentPath == null || this.currentPath.isFinished();
		}

		// Token: 0x0600C4A0 RID: 50336 RVA: 0x0048AA40 File Offset: 0x00488C40
		public bool isPlanningPath()
		{
			return PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId);
		}

		// Token: 0x0600C4A1 RID: 50337 RVA: 0x0048AA57 File Offset: 0x00488C57
		public bool noPathAndNotPlanningOne()
		{
			return this.noPath() && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId);
		}

		// Token: 0x0600C4A2 RID: 50338 RVA: 0x0048AA7B File Offset: 0x00488C7B
		public bool HasPath()
		{
			return this.currentPath != null && !this.currentPath.isFinished();
		}

		// Token: 0x0600C4A3 RID: 50339 RVA: 0x0048AA95 File Offset: 0x00488C95
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canNavigate()
		{
			return this.theEntity.CanNavigatePath();
		}

		// Token: 0x0600C4A4 RID: 50340 RVA: 0x0048AAA2 File Offset: 0x00488CA2
		public void clearPath()
		{
			if (this.currentPath != null)
			{
				this.currentPath.Destruct();
			}
			this.currentPath = null;
		}

		// Token: 0x0600C4A5 RID: 50341 RVA: 0x0048AABE File Offset: 0x00488CBE
		public PathEntity getPath()
		{
			return this.currentPath;
		}

		// Token: 0x0600C4A6 RID: 50342 RVA: 0x0048AAC6 File Offset: 0x00488CC6
		public void ShortenEnd(float _distance)
		{
			if (this.currentPath != null)
			{
				this.currentPath.ShortenEnd(_distance);
			}
		}

		// Token: 0x0600C4A7 RID: 50343 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void GetPathTo(PathInfo _pathInfo)
		{
		}

		// Token: 0x0600C4A8 RID: 50344 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void GetPathToEntity(PathInfo _pathInfo, EntityAlive _entity)
		{
		}

		// Token: 0x0600C4A9 RID: 50345 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool SetPath(PathInfo _pathInfo, float _speed)
		{
			return false;
		}

		// Token: 0x0600C4AA RID: 50346 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void UpdateNavigation()
		{
		}

		// Token: 0x0600C4AB RID: 50347 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void CreatePath()
		{
		}

		// Token: 0x040094DF RID: 38111
		public PathInfo pathInfo;

		// Token: 0x040094E0 RID: 38112
		[PublicizedFrom(EAccessModifier.Protected)]
		public EntityAlive theEntity;

		// Token: 0x040094E1 RID: 38113
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathEntity currentPath;

		// Token: 0x040094E2 RID: 38114
		[PublicizedFrom(EAccessModifier.Protected)]
		public float speed;

		// Token: 0x040094E3 RID: 38115
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canBreakBlocks;

		// Token: 0x040094E4 RID: 38116
		[PublicizedFrom(EAccessModifier.Protected)]
		public int curNavTicks;

		// Token: 0x040094E5 RID: 38117
		[PublicizedFrom(EAccessModifier.Protected)]
		public int prevNavTicks;

		// Token: 0x040094E6 RID: 38118
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool inWater;

		// Token: 0x040094E7 RID: 38119
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canDrown;
	}
}
