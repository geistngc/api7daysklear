using System;

// Token: 0x02000BFD RID: 3069
public class BiomeBlockDecoration
{
	// Token: 0x06005DAC RID: 23980 RVA: 0x00246384 File Offset: 0x00244584
	public BiomeBlockDecoration(string _name, float _prob, float _clusprob, bool _instantiateReferences, int _randomRotateMax, int _checkResource = 2147483647)
	{
		string[] array = _name.Split(',', StringSplitOptions.None);
		if (_instantiateReferences)
		{
			this.blockValues = new BlockValue[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				BlockValue blockValueForName = WorldBiomes.GetBlockValueForName(array[i]);
				if (_randomRotateMax > 3 && !blockValueForName.isair)
				{
					Block block = blockValueForName.Block;
					if (block.isMultiBlock && (block.multiBlockPos.dim.x > 1 || block.multiBlockPos.dim.z > 1))
					{
						Log.Error("Parsing biomes. Block with name '" + array[i] + "' supports only rotations 0-3, setting it to 3");
						_randomRotateMax = 3;
					}
				}
				this.blockValues[i] = blockValueForName;
			}
		}
		this.prob = _prob;
		this.clusterProb = _clusprob;
		this.randomRotateMax = _randomRotateMax;
		this.checkResourceOffsetY = _checkResource;
	}

	// Token: 0x06005DAD RID: 23981 RVA: 0x00246458 File Offset: 0x00244658
	public static byte GetRandomRotation(float _rnd, int _randomRotateMax)
	{
		byte b = (byte)(_rnd * (float)_randomRotateMax + 0.5f);
		if (b >= 4 && b <= 7)
		{
			b = b - 4 + 24;
		}
		return b;
	}

	// Token: 0x0400487B RID: 18555
	public BlockValue[] blockValues;

	// Token: 0x0400487C RID: 18556
	public float prob;

	// Token: 0x0400487D RID: 18557
	public float clusterProb;

	// Token: 0x0400487E RID: 18558
	public int randomRotateMax;

	// Token: 0x0400487F RID: 18559
	public int checkResourceOffsetY;
}
