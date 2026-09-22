using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E7 RID: 6375
	public class PathInfoMultiTarget : PathInfo
	{
		// Token: 0x0600C497 RID: 50327 RVA: 0x0048A967 File Offset: 0x00488B67
		public PathInfoMultiTarget(EntityAlive _entity, List<Vector3> _targetPositions, bool _canBreakBlocks, float _speed, EAIBase _aiTask) : base(_entity, _canBreakBlocks, _speed, _aiTask)
		{
			this.targetPositions = (_targetPositions ?? new List<Vector3>());
		}

		// Token: 0x040094D3 RID: 38099
		public readonly List<Vector3> targetPositions;

		// Token: 0x040094D4 RID: 38100
		public bool pathsForAll;

		// Token: 0x040094D5 RID: 38101
		public OnPathDelegate[] OnTargetPathFinished;

		// Token: 0x040094D6 RID: 38102
		public MultiTargetPathSelection pathSelection = MultiTargetPathSelection.Closest;

		// Token: 0x040094D7 RID: 38103
		public List<Vector3>[] vectorPaths;
	}
}
