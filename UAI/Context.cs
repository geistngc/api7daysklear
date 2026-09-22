using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200178C RID: 6028
	[Preserve]
	public class Context
	{
		// Token: 0x170016A0 RID: 5792
		// (get) Token: 0x0600BAA8 RID: 47784 RVA: 0x0045A284 File Offset: 0x00458484
		public List<string> AIPackages
		{
			get
			{
				return this.Self.AIPackages;
			}
		}

		// Token: 0x0600BAA9 RID: 47785 RVA: 0x0045A291 File Offset: 0x00458491
		public Context(EntityAlive _self)
		{
			this.Self = _self;
			this.World = GameManager.Instance.World;
			this.ConsiderationData = new ConsiderationData();
			this.ActionData = default(ActionData);
		}

		// Token: 0x04008C29 RID: 35881
		public EntityAlive Self;

		// Token: 0x04008C2A RID: 35882
		public World World;

		// Token: 0x04008C2B RID: 35883
		public ConsiderationData ConsiderationData;

		// Token: 0x04008C2C RID: 35884
		public ActionData ActionData;

		// Token: 0x04008C2D RID: 35885
		public float updateTimer;
	}
}
