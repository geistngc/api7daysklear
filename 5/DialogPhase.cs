using System;
using System.Collections.Generic;

// Token: 0x020002EC RID: 748
public class DialogPhase : BaseDialogItem
{
	// Token: 0x17000264 RID: 612
	// (get) Token: 0x0600157B RID: 5499 RVA: 0x00080E1D File Offset: 0x0007F01D
	// (set) Token: 0x0600157C RID: 5500 RVA: 0x00080E25 File Offset: 0x0007F025
	public string StartStatementID { get; set; }

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600157D RID: 5501 RVA: 0x00080E2E File Offset: 0x0007F02E
	// (set) Token: 0x0600157E RID: 5502 RVA: 0x00080E36 File Offset: 0x0007F036
	public string StartResponseID { get; set; }

	// Token: 0x0600157F RID: 5503 RVA: 0x00080E3F File Offset: 0x0007F03F
	public DialogPhase(string newID)
	{
		this.ID = newID;
		this.HeaderName = string.Format("Phase : {0}", newID);
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00080E6A File Offset: 0x0007F06A
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddRequirement(BaseDialogRequirement requirement)
	{
		this.RequirementList.Add(requirement);
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x00080E78 File Offset: 0x0007F078
	public override string ToString()
	{
		return this.HeaderName;
	}

	// Token: 0x04000E70 RID: 3696
	public List<BaseDialogRequirement> RequirementList = new List<BaseDialogRequirement>();
}
