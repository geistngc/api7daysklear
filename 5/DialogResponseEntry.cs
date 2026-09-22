using System;
using UnityEngine.Scripting;

// Token: 0x02000300 RID: 768
[Preserve]
public class DialogResponseEntry : BaseResponseEntry
{
	// Token: 0x060015CB RID: 5579 RVA: 0x00081BB9 File Offset: 0x0007FDB9
	public DialogResponseEntry(string _ID)
	{
		base.ID = _ID;
		base.ResponseType = BaseResponseEntry.ResponseTypes.Response;
	}
}
