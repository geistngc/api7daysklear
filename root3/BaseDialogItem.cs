using System;

// Token: 0x020002EA RID: 746
public class BaseDialogItem
{
	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001571 RID: 5489 RVA: 0x00080DC1 File Offset: 0x0007EFC1
	// (set) Token: 0x06001572 RID: 5490 RVA: 0x00080DC9 File Offset: 0x0007EFC9
	public virtual string HeaderName { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001573 RID: 5491 RVA: 0x00080DD2 File Offset: 0x0007EFD2
	// (set) Token: 0x06001574 RID: 5492 RVA: 0x00080DDA File Offset: 0x0007EFDA
	public Dialog OwnerDialog { get; set; }

	// Token: 0x04000E6B RID: 3691
	public string ID;
}
