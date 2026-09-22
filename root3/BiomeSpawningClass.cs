using System;

// Token: 0x02000ADB RID: 2779
public class BiomeSpawningClass
{
	// Token: 0x06005320 RID: 21280 RVA: 0x001FCAFD File Offset: 0x001FACFD
	public static void Cleanup()
	{
		BiomeSpawningClass.list.Clear();
	}

	// Token: 0x040040CE RID: 16590
	public static DictionarySave<string, BiomeSpawnEntityGroupList> list = new DictionarySave<string, BiomeSpawnEntityGroupList>();
}
