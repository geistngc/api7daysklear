using System;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E5 RID: 6373
	public class PathInfoSingleTarget : PathInfo
	{
		// Token: 0x0600C496 RID: 50326 RVA: 0x0048A914 File Offset: 0x00488B14
		public PathInfoSingleTarget(EntityAlive _entity, Vector3 _targetPos, bool _canBreakBlocks, float _speed, EAIBase _aiTask) : base(_entity, _canBreakBlocks, _speed, _aiTask)
		{
			this.entity = _entity;
			this.hasStart = false;
			this.targetPos = _targetPos;
			this.canBreakBlocks = _canBreakBlocks;
			this.speed = _speed;
			this.aiTask = _aiTask;
			this.path = null;
			this.calculatePartial = true;
		}

		// Token: 0x040094CD RID: 38093
		public Vector3 targetPos;

		// Token: 0x040094CE RID: 38094
		public PathEndingCondition endingCondition;
	}
}
