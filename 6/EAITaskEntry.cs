using System;
using UnityEngine.Scripting;

// Token: 0x0200045A RID: 1114
[Preserve]
public class EAITaskEntry
{
	// Token: 0x060021F1 RID: 8689 RVA: 0x000CD734 File Offset: 0x000CB934
	public EAITaskEntry(int _priority, EAIBase _action)
	{
		this.priority = _priority;
		this.action = _action;
	}

	// Token: 0x04001774 RID: 6004
	public EAIBase action;

	// Token: 0x04001775 RID: 6005
	public int priority;

	// Token: 0x04001776 RID: 6006
	public bool isExecuting;

	// Token: 0x04001777 RID: 6007
	public float executeTime;
}
