using System;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200178E RID: 6030
	[Preserve]
	public struct ActionData
	{
		// Token: 0x170016A1 RID: 5793
		// (get) Token: 0x0600BAAB RID: 47787 RVA: 0x0045A2E8 File Offset: 0x004584E8
		public UAITaskBase CurrentTask
		{
			get
			{
				if (this.Action == null || this.Action.GetTasks() == null || this.TaskIndex < 0 || this.TaskIndex >= this.Action.GetTasks().Count)
				{
					return null;
				}
				return this.Action.GetTasks()[this.TaskIndex];
			}
		}

		// Token: 0x0600BAAC RID: 47788 RVA: 0x0045A343 File Offset: 0x00458543
		public void ClearData()
		{
			this.Data = null;
			this.TaskStartTimeStamp = 0UL;
			this.Initialized = false;
			this.Started = false;
			this.Executing = false;
			this.Failed = false;
			this.Finished = false;
		}

		// Token: 0x04008C30 RID: 35888
		public UAIAction Action;

		// Token: 0x04008C31 RID: 35889
		public object Target;

		// Token: 0x04008C32 RID: 35890
		public object Data;

		// Token: 0x04008C33 RID: 35891
		public int TaskIndex;

		// Token: 0x04008C34 RID: 35892
		public ulong TaskStartTimeStamp;

		// Token: 0x04008C35 RID: 35893
		public bool Initialized;

		// Token: 0x04008C36 RID: 35894
		public bool Started;

		// Token: 0x04008C37 RID: 35895
		public bool Executing;

		// Token: 0x04008C38 RID: 35896
		public bool Failed;

		// Token: 0x04008C39 RID: 35897
		public bool Finished;
	}
}
