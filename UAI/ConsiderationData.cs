using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200178D RID: 6029
	[Preserve]
	public class ConsiderationData
	{
		// Token: 0x0600BAAA RID: 47786 RVA: 0x0045A2C7 File Offset: 0x004584C7
		public ConsiderationData()
		{
			this.EntityTargets = new List<Entity>();
			this.WaypointTargets = new List<Vector3>();
		}

		// Token: 0x04008C2E RID: 35886
		public List<Entity> EntityTargets;

		// Token: 0x04008C2F RID: 35887
		public List<Vector3> WaypointTargets;
	}
}
