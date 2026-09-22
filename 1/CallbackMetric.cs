using System;
using System.Text;

// Token: 0x0200152B RID: 5419
public class CallbackMetric : IMetric
{
	// Token: 0x170013C5 RID: 5061
	// (get) Token: 0x0600AA2A RID: 43562 RVA: 0x003FA204 File Offset: 0x003F8404
	// (set) Token: 0x0600AA2B RID: 43563 RVA: 0x003FA20C File Offset: 0x003F840C
	public string Header { get; set; }

	// Token: 0x0600AA2C RID: 43564 RVA: 0x003FA215 File Offset: 0x003F8415
	public void AppendLastValue(StringBuilder builder)
	{
		builder.Append(this.callback());
	}

	// Token: 0x0600AA2D RID: 43565 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x04007EA7 RID: 32423
	public CallbackMetric.GetLastValue callback;

	// Token: 0x0200152C RID: 5420
	// (Invoke) Token: 0x0600AA30 RID: 43568
	public delegate string GetLastValue();
}
