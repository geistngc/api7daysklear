using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E9 RID: 6377
	public class PathInfoFleeRandom : PathInfo
	{
		// Token: 0x0600C499 RID: 50329 RVA: 0x0048A9AA File Offset: 0x00488BAA
		public PathInfoFleeRandom(EntityAlive _entity, int _searchLength, Vector3 _aimBias, float _aimStrength, float _speed, EAIBase _aiTask) : base(_entity, false, _speed, _aiTask)
		{
			this.searchLength = _searchLength;
			this.aimBias = _aimBias;
			this.aimStrength = _aimStrength;
		}

		// Token: 0x040094DB RID: 38107
		public int searchLength;

		// Token: 0x040094DC RID: 38108
		public Vector3 aimBias = Vector3.zero;

		// Token: 0x040094DD RID: 38109
		public float aimStrength;
	}
}
