using System;

// Token: 0x02000BFE RID: 3070
public class BiomePrefabDecoration
{
	// Token: 0x06005DAE RID: 23982 RVA: 0x00246482 File Offset: 0x00244682
	public BiomePrefabDecoration(string _prefabName, float _prob, bool _isDecorateOnSlopes, int _checkResource = 2147483647)
	{
		this.prefabName = _prefabName;
		this.prob = _prob;
		this.checkResourceOffsetY = _checkResource;
		this.isDecorateOnSlopes = _isDecorateOnSlopes;
	}

	// Token: 0x04004880 RID: 18560
	public string prefabName;

	// Token: 0x04004881 RID: 18561
	public float prob;

	// Token: 0x04004882 RID: 18562
	public int checkResourceOffsetY;

	// Token: 0x04004883 RID: 18563
	public bool isDecorateOnSlopes;
}
