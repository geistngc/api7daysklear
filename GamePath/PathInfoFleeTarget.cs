using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018EA RID: 6378
	public class PathInfoFleeTarget : PathInfoFleeRandom
	{
		// Token: 0x0600C49A RID: 50330 RVA: 0x0048A9D9 File Offset: 0x00488BD9
		public PathInfoFleeTarget(EntityAlive _entity, Vector3 _fleeTarget, int _searchLength, Vector3 _aimBias, float _aimStrength, float _speed, EAIBase _aiTask) : base(_entity, _searchLength, _aimBias, _aimStrength, _speed, _aiTask)
		{
			this.fleeTarget = _fleeTarget;
		}

		// Token: 0x040094DE RID: 38110
		public Vector3 fleeTarget;
	}
}
