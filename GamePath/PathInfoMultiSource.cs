using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E8 RID: 6376
	public class PathInfoMultiSource : PathInfo
	{
		// Token: 0x0600C498 RID: 50328 RVA: 0x0048A98C File Offset: 0x00488B8C
		public PathInfoMultiSource(EntityAlive _entity, List<Vector3> _sourcePositions, bool _canBreakBlocks, float _speed, EAIBase _aiTask) : base(_entity, _canBreakBlocks, _speed, _aiTask)
		{
			this.sourcePositions = (_sourcePositions ?? new List<Vector3>());
		}

		// Token: 0x040094D8 RID: 38104
		public readonly List<Vector3> sourcePositions;

		// Token: 0x040094D9 RID: 38105
		public OnPathDelegate[] OnTargetPathFinished;

		// Token: 0x040094DA RID: 38106
		public List<Vector3>[] vectorPaths;
	}
}
