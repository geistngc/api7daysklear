using System;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E3 RID: 6371
	public abstract class PathInfo
	{
		// Token: 0x0600C493 RID: 50323 RVA: 0x0048A8C1 File Offset: 0x00488AC1
		public PathInfo(EntityAlive _entity, bool _canBreakBlocks, float _speed, EAIBase _aiTask)
		{
			this.entity = _entity;
			this.hasStart = false;
			this.canBreakBlocks = _canBreakBlocks;
			this.speed = _speed;
			this.aiTask = _aiTask;
			this.calculatePartial = false;
			this.path = null;
		}

		// Token: 0x0600C494 RID: 50324 RVA: 0x0048A8FB File Offset: 0x00488AFB
		public void SetStartPos(Vector3 _startPos)
		{
			this.startPos = _startPos;
			this.hasStart = true;
		}

		// Token: 0x0600C495 RID: 50325 RVA: 0x0048A90B File Offset: 0x00488B0B
		public virtual void ResetPathInfo()
		{
			this.path = null;
		}

		// Token: 0x040094BE RID: 38078
		public EntityAlive entity;

		// Token: 0x040094BF RID: 38079
		public PathInfo.State state;

		// Token: 0x040094C0 RID: 38080
		public Vector3 startPos;

		// Token: 0x040094C1 RID: 38081
		public bool hasStart;

		// Token: 0x040094C2 RID: 38082
		public bool canBreakBlocks;

		// Token: 0x040094C3 RID: 38083
		public float speed;

		// Token: 0x040094C4 RID: 38084
		public EAIBase aiTask;

		// Token: 0x040094C5 RID: 38085
		public bool calculatePartial;

		// Token: 0x040094C6 RID: 38086
		public ChunkCache chunkcache;

		// Token: 0x040094C7 RID: 38087
		public OnPathDelegate OnPathResult;

		// Token: 0x040094C8 RID: 38088
		public PathEntity path;

		// Token: 0x020018E4 RID: 6372
		public enum State
		{
			// Token: 0x040094CA RID: 38090
			Queued,
			// Token: 0x040094CB RID: 38091
			Pathing,
			// Token: 0x040094CC RID: 38092
			Done
		}
	}
}
