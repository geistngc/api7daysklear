using System;
using System.Collections.Generic;

// Token: 0x02000AD8 RID: 2776
public class BiomeSpawnEntityGroupList
{
	// Token: 0x0600531D RID: 21277 RVA: 0x001FCA78 File Offset: 0x001FAC78
	public BiomeSpawnEntityGroupData Find(int _idHash)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].idHash == _idHash)
			{
				return this.list[i];
			}
		}
		return null;
	}

	// Token: 0x040040C1 RID: 16577
	public List<BiomeSpawnEntityGroupData> list = new List<BiomeSpawnEntityGroupData>();
}
