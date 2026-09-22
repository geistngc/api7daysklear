using System;

// Token: 0x02000653 RID: 1619
public class BuffManager
{
	// Token: 0x06003465 RID: 13413 RVA: 0x0015CB78 File Offset: 0x0015AD78
	public static void Cleanup()
	{
		if (BuffManager.Buffs != null)
		{
			BuffManager.Buffs.Clear();
			BuffManager.Buffs = null;
		}
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x0015CB91 File Offset: 0x0015AD91
	public static void AddBuff(BuffClass _buffClass)
	{
		BuffManager.Buffs[_buffClass.Name] = _buffClass;
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x0015CBA4 File Offset: 0x0015ADA4
	public static BuffClass GetBuff(string _name)
	{
		BuffClass result;
		BuffManager.Buffs.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x040029C9 RID: 10697
	public static CaseInsensitiveStringDictionary<BuffClass> Buffs;
}
