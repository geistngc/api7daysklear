using System;

// Token: 0x020013FB RID: 5115
[AttributeUsage(AttributeTargets.Field)]
public class Documentation : Attribute
{
	// Token: 0x170012E4 RID: 4836
	// (get) Token: 0x0600A08F RID: 41103 RVA: 0x003C7B89 File Offset: 0x003C5D89
	public string Text { get; }

	// Token: 0x0600A090 RID: 41104 RVA: 0x003C7B91 File Offset: 0x003C5D91
	public Documentation(string value)
	{
		this.Text = value;
	}
}
