using System;
using System.Collections.Generic;

// Token: 0x020002EB RID: 747
public class BaseStatement : BaseDialogItem
{
	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001576 RID: 5494 RVA: 0x00080DE3 File Offset: 0x0007EFE3
	// (set) Token: 0x06001577 RID: 5495 RVA: 0x00080DEB File Offset: 0x0007EFEB
	public string NextStatementID { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x06001578 RID: 5496 RVA: 0x00080DF4 File Offset: 0x0007EFF4
	public override string ToString()
	{
		return this.Text;
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x00080DFC File Offset: 0x0007EFFC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddAction(BaseDialogAction action)
	{
		this.Actions.Add(action);
	}

	// Token: 0x04000E6D RID: 3693
	public string Text;

	// Token: 0x04000E6F RID: 3695
	public List<BaseDialogAction> Actions = new List<BaseDialogAction>();
}
