using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x02001783 RID: 6019
	[Preserve]
	public class UAITaskBase
	{
		// Token: 0x1700169D RID: 5789
		// (get) Token: 0x0600BA7A RID: 47738 RVA: 0x00459884 File Offset: 0x00457A84
		// (set) Token: 0x0600BA7B RID: 47739 RVA: 0x0045988C File Offset: 0x00457A8C
		public string Name { get; set; }

		// Token: 0x0600BA7C RID: 47740 RVA: 0x00459895 File Offset: 0x00457A95
		public virtual void Init(Context _context)
		{
			_context.ActionData.Initialized = true;
			_context.ActionData.Started = false;
			_context.ActionData.Executing = false;
			if (!this.parmsInitialized)
			{
				this.initializeParameters();
				this.parmsInitialized = true;
			}
		}

		// Token: 0x0600BA7D RID: 47741 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void initializeParameters()
		{
		}

		// Token: 0x0600BA7E RID: 47742 RVA: 0x004598D0 File Offset: 0x00457AD0
		public virtual void Start(Context _context)
		{
			_context.ActionData.Started = true;
			_context.ActionData.Executing = true;
		}

		// Token: 0x0600BA7F RID: 47743 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Update(Context _context)
		{
		}

		// Token: 0x0600BA80 RID: 47744 RVA: 0x004598EA File Offset: 0x00457AEA
		public virtual void Stop(Context _context)
		{
			_context.ActionData.Executing = false;
		}

		// Token: 0x0600BA81 RID: 47745 RVA: 0x004598F8 File Offset: 0x00457AF8
		public virtual void Reset(Context _context)
		{
			_context.ActionData.ClearData();
		}

		// Token: 0x04008C18 RID: 35864
		public Dictionary<string, string> Parameters = new Dictionary<string, string>();

		// Token: 0x04008C19 RID: 35865
		[PublicizedFrom(EAccessModifier.Private)]
		public bool parmsInitialized;
	}
}
