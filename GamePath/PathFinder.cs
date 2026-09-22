using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E1 RID: 6369
	public class PathFinder
	{
		// Token: 0x0600C485 RID: 50309 RVA: 0x0048A89C File Offset: 0x00488A9C
		public PathFinder(PathInfo _pathInfo, bool _bDrn, bool _canClimbLadders, bool _bCanClimbWalls)
		{
			this.pathInfo = _pathInfo;
			this.canEntityDrown = _bDrn;
			this.canClimbWalls = _bCanClimbWalls;
			this.canClimbLadders = _canClimbLadders;
		}

		// Token: 0x0600C486 RID: 50310 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Calculate(Vector3 _fromPos)
		{
		}

		// Token: 0x0600C487 RID: 50311 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Destruct()
		{
		}

		// Token: 0x040094B9 RID: 38073
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathInfo pathInfo;

		// Token: 0x040094BA RID: 38074
		public bool canClimbWalls;

		// Token: 0x040094BB RID: 38075
		public bool canClimbLadders;

		// Token: 0x040094BC RID: 38076
		public bool canEntityDrown;
	}
}
