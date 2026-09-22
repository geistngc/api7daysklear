using System;
using System.Text;

// Token: 0x02001529 RID: 5417
public class ConstantValueMetric : IMetric
{
	// Token: 0x170013C3 RID: 5059
	// (get) Token: 0x0600AA20 RID: 43552 RVA: 0x003FA1B8 File Offset: 0x003F83B8
	// (set) Token: 0x0600AA21 RID: 43553 RVA: 0x003FA1C0 File Offset: 0x003F83C0
	public string Header { get; set; }

	// Token: 0x0600AA22 RID: 43554 RVA: 0x003FA1C9 File Offset: 0x003F83C9
	public void AppendLastValue(StringBuilder builder)
	{
		builder.Append(this.value);
	}

	// Token: 0x0600AA23 RID: 43555 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x04007EA4 RID: 32420
	public int value;
}
