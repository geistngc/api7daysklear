using System;

// Token: 0x02000AD9 RID: 2777
public class BiomeSpawnEntityGroupData
{
	// Token: 0x0600531F RID: 21279 RVA: 0x001FCAD0 File Offset: 0x001FACD0
	public BiomeSpawnEntityGroupData(int _idHash, int _maxCount, int[] _respawndelay, EDaytime _daytime, BiomeSpawnEntityGroupData.eType _type)
	{
		this.idHash = _idHash;
		this.maxCount = _maxCount;
		this.daytime = _daytime;
		this.respawnDelayInWorldTime = _respawndelay;
		this.type = _type;
	}

	// Token: 0x040040C2 RID: 16578
	public int idHash;

	// Token: 0x040040C3 RID: 16579
	public string entityGroupName;

	// Token: 0x040040C4 RID: 16580
	public int maxCount;

	// Token: 0x040040C5 RID: 16581
	public int[] respawnDelayInWorldTime;

	// Token: 0x040040C6 RID: 16582
	public EDaytime daytime;

	// Token: 0x040040C7 RID: 16583
	public FastTags<TagGroup.Poi> POITags;

	// Token: 0x040040C8 RID: 16584
	public FastTags<TagGroup.Poi> noPOITags;

	// Token: 0x040040C9 RID: 16585
	public BiomeSpawnEntityGroupData.eType type;

	// Token: 0x02000ADA RID: 2778
	public enum eType
	{
		// Token: 0x040040CB RID: 16587
		Normal,
		// Token: 0x040040CC RID: 16588
		Animal,
		// Token: 0x040040CD RID: 16589
		Rare
	}
}
