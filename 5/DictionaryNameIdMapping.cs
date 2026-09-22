using System;

// Token: 0x020013E7 RID: 5095
public class DictionaryNameIdMapping
{
	// Token: 0x06009FE0 RID: 40928 RVA: 0x003C5020 File Offset: 0x003C3220
	public int Add(string _name)
	{
		int num;
		this.namesToIds.TryGetValue(_name, out num);
		if (num == 0)
		{
			int num2 = this.nextId + 1;
			this.nextId = num2;
			num = num2;
			this.namesToIds[_name] = num;
		}
		return num;
	}

	// Token: 0x06009FE1 RID: 40929 RVA: 0x003C505F File Offset: 0x003C325F
	public void Clear()
	{
		this.nextId = 0;
		this.namesToIds.Clear();
	}

	// Token: 0x06009FE2 RID: 40930 RVA: 0x003C5074 File Offset: 0x003C3274
	public int FindId(string _name)
	{
		int result;
		this.namesToIds.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x0400794B RID: 31051
	public const int cIDNone = 0;

	// Token: 0x0400794C RID: 31052
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextId;

	// Token: 0x0400794D RID: 31053
	[PublicizedFrom(EAccessModifier.Private)]
	public CaseInsensitiveStringDictionary<int> namesToIds = new CaseInsensitiveStringDictionary<int>();
}
