using System;

// Token: 0x02000B2F RID: 2863
public class ChunkProviderParameter
{
	// Token: 0x060056B5 RID: 22197 RVA: 0x0021479C File Offset: 0x0021299C
	public ChunkProviderParameter(int _id, string _name, float _val, float _minVal, float _maxVal)
	{
		this.id = _id;
		this.name = _name;
		this.val = _val;
		this.minVal = _minVal;
		this.maxVal = _maxVal;
	}

	// Token: 0x04004328 RID: 17192
	public int id;

	// Token: 0x04004329 RID: 17193
	public string name;

	// Token: 0x0400432A RID: 17194
	public float val;

	// Token: 0x0400432B RID: 17195
	public float minVal;

	// Token: 0x0400432C RID: 17196
	public float maxVal;
}
