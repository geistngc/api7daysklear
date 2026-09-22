using System;
using UnityEngine.Scripting;

// Token: 0x020002FE RID: 766
[Preserve]
public class BaseResponseEntry
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x060015C6 RID: 5574 RVA: 0x00081B84 File Offset: 0x0007FD84
	// (set) Token: 0x060015C7 RID: 5575 RVA: 0x00081B8C File Offset: 0x0007FD8C
	public string ID { get; set; }

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x060015C8 RID: 5576 RVA: 0x00081B95 File Offset: 0x0007FD95
	// (set) Token: 0x060015C9 RID: 5577 RVA: 0x00081B9D File Offset: 0x0007FD9D
	public BaseResponseEntry.ResponseTypes ResponseType { get; set; }

	// Token: 0x04000E9E RID: 3742
	public string UniqueID = "";

	// Token: 0x04000EA1 RID: 3745
	public DialogResponse Response;

	// Token: 0x020002FF RID: 767
	public enum ResponseTypes
	{
		// Token: 0x04000EA3 RID: 3747
		Response,
		// Token: 0x04000EA4 RID: 3748
		QuestAdd
	}
}
