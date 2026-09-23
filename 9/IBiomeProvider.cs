using System;
using System.Collections;

// Token: 0x02000C44 RID: 3140
public interface IBiomeProvider
{
	// Token: 0x06005F9A RID: 24474
	IEnumerator InitData();

	// Token: 0x06005F9B RID: 24475
	void Init(int _seed, string _worldName, WorldBiomes _biomes, string _params1, string _params2);

	// Token: 0x06005F9C RID: 24476
	int GetSubBiomeIdxAt(BiomeDefinition bd, int _x, int _y, int _z);

	// Token: 0x06005F9D RID: 24477
	BiomeDefinition GetBiomeAt(int _x, int _z);

	// Token: 0x06005F9E RID: 24478
	BiomeDefinition GetBiomeAt(int _x, int _z, out float _intensity);

	// Token: 0x06005F9F RID: 24479 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	BiomeDefinition GetBiomeOrSubAt(int x, int z)
	{
		return null;
	}

	// Token: 0x06005FA0 RID: 24480
	float GetHumidityAt(int x, int z);

	// Token: 0x06005FA1 RID: 24481
	float GetTemperatureAt(int x, int z);

	// Token: 0x06005FA2 RID: 24482
	float GetRadiationAt(int x, int z);

	// Token: 0x06005FA3 RID: 24483
	string GetWorldName();

	// Token: 0x06005FA4 RID: 24484
	BlockValue GetTopmostBlockValue(int xWorld, int zWorld);

	// Token: 0x06005FA5 RID: 24485 RVA: 0x000027FC File Offset: 0x000009FC
	void Cleanup()
	{
	}
}
