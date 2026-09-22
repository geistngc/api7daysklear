using System;
using System.IO;

// Token: 0x02000C41 RID: 3137
public struct BiomeIntensity : IEquatable<BiomeIntensity>
{
	// Token: 0x170009B9 RID: 2489
	// (get) Token: 0x06005F77 RID: 24439 RVA: 0x0025C8EB File Offset: 0x0025AAEB
	// (set) Token: 0x06005F78 RID: 24440 RVA: 0x0025C8FD File Offset: 0x0025AAFD
	public float intensity0
	{
		get
		{
			return (float)(this.intensity0and1 & 15) / 15f;
		}
		set
		{
			this.intensity0and1 = (byte)((int)(this.intensity0and1 & 240) | ((int)(value * 15f) & 15));
		}
	}

	// Token: 0x170009BA RID: 2490
	// (get) Token: 0x06005F79 RID: 24441 RVA: 0x0025C91E File Offset: 0x0025AB1E
	// (set) Token: 0x06005F7A RID: 24442 RVA: 0x0025C932 File Offset: 0x0025AB32
	public float intensity1
	{
		get
		{
			return (float)(this.intensity0and1 >> 4 & 15) / 15f;
		}
		set
		{
			this.intensity0and1 = (byte)((int)(this.intensity0and1 & 15) | ((int)(value * 15f) << 4 & 240));
		}
	}

	// Token: 0x170009BB RID: 2491
	// (get) Token: 0x06005F7B RID: 24443 RVA: 0x0025C955 File Offset: 0x0025AB55
	// (set) Token: 0x06005F7C RID: 24444 RVA: 0x0025C967 File Offset: 0x0025AB67
	public float intensity2
	{
		get
		{
			return (float)(this.intensity2and3 & 15) / 15f;
		}
		set
		{
			this.intensity2and3 = (byte)((int)(this.intensity2and3 & 240) | ((int)(value * 15f) & 15));
		}
	}

	// Token: 0x170009BC RID: 2492
	// (get) Token: 0x06005F7D RID: 24445 RVA: 0x0025C988 File Offset: 0x0025AB88
	// (set) Token: 0x06005F7E RID: 24446 RVA: 0x0025C99C File Offset: 0x0025AB9C
	public float intensity3
	{
		get
		{
			return (float)(this.intensity2and3 >> 4 & 15) / 15f;
		}
		set
		{
			this.intensity2and3 = (byte)((int)(this.intensity2and3 & 15) | ((int)(value * 15f) << 4 & 240));
		}
	}

	// Token: 0x06005F7F RID: 24447 RVA: 0x0025C9BF File Offset: 0x0025ABBF
	public BiomeIntensity(byte _singleBiomeId)
	{
		this.biomeId0 = _singleBiomeId;
		this.biomeId1 = 0;
		this.biomeId2 = 0;
		this.biomeId3 = 0;
		this.intensity0and1 = 15;
		this.intensity2and3 = 0;
	}

	// Token: 0x06005F80 RID: 24448 RVA: 0x0025C9EC File Offset: 0x0025ABEC
	public BiomeIntensity(byte[] _chunkBiomeIntensityArray, int _offs)
	{
		this.biomeId0 = _chunkBiomeIntensityArray[_offs];
		this.biomeId1 = _chunkBiomeIntensityArray[_offs + 1];
		this.biomeId2 = _chunkBiomeIntensityArray[_offs + 2];
		this.biomeId3 = _chunkBiomeIntensityArray[_offs + 3];
		this.intensity0and1 = _chunkBiomeIntensityArray[_offs + 4];
		this.intensity2and3 = _chunkBiomeIntensityArray[_offs + 5];
	}

	// Token: 0x06005F81 RID: 24449 RVA: 0x0025CA3C File Offset: 0x0025AC3C
	public static BiomeIntensity FromArray(int[] _unsortedBiomeIdArray)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		BiomeIntensity biomeIntensity = default(BiomeIntensity);
		for (int i = 0; i < _unsortedBiomeIdArray.Length; i++)
		{
			if (num < _unsortedBiomeIdArray[i])
			{
				biomeIntensity.biomeId0 = (byte)i;
				num = _unsortedBiomeIdArray[i];
				if (num5 < num)
				{
					num5 = num;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId0] = 0;
		for (int j = 0; j < _unsortedBiomeIdArray.Length; j++)
		{
			if (num2 < _unsortedBiomeIdArray[j])
			{
				biomeIntensity.biomeId1 = (byte)j;
				num2 = _unsortedBiomeIdArray[j];
				if (num5 < num2)
				{
					num5 = num2;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId1] = 0;
		for (int k = 0; k < _unsortedBiomeIdArray.Length; k++)
		{
			if (num3 < _unsortedBiomeIdArray[k])
			{
				biomeIntensity.biomeId2 = (byte)k;
				num3 = _unsortedBiomeIdArray[k];
				if (num5 < num3)
				{
					num5 = num3;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId2] = 0;
		for (int l = 0; l < _unsortedBiomeIdArray.Length; l++)
		{
			if (num4 < _unsortedBiomeIdArray[l])
			{
				biomeIntensity.biomeId3 = (byte)l;
				num4 = _unsortedBiomeIdArray[l];
				if (num5 < num4)
				{
					num5 = num4;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId3] = 0;
		biomeIntensity.intensity0 = (float)num / (float)num5;
		biomeIntensity.intensity1 = (float)num2 / (float)num5;
		biomeIntensity.intensity2 = (float)num3 / (float)num5;
		biomeIntensity.intensity3 = (float)num4 / (float)num5;
		return biomeIntensity;
	}

	// Token: 0x06005F82 RID: 24450 RVA: 0x0025CB7C File Offset: 0x0025AD7C
	public void ToArray(byte[] _array, int offs)
	{
		_array[offs] = this.biomeId0;
		_array[1 + offs] = this.biomeId1;
		_array[2 + offs] = this.biomeId2;
		_array[3 + offs] = this.biomeId3;
		_array[4 + offs] = this.intensity0and1;
		_array[5 + offs] = this.intensity2and3;
	}

	// Token: 0x06005F83 RID: 24451 RVA: 0x0025CBCC File Offset: 0x0025ADCC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.biomeId0);
		_bw.Write((byte)(this.intensity0 * 255f));
		_bw.Write(this.biomeId1);
		_bw.Write((byte)(this.intensity1 * 255f));
		_bw.Write(this.biomeId2);
		_bw.Write((byte)(this.intensity2 * 255f));
		_bw.Write(this.biomeId3);
		_bw.Write((byte)(this.intensity3 * 255f));
	}

	// Token: 0x06005F84 RID: 24452 RVA: 0x0025CC58 File Offset: 0x0025AE58
	public void Read(BinaryReader _br)
	{
		this.biomeId0 = _br.ReadByte();
		this.intensity0 = (float)_br.ReadByte() / 255f;
		this.biomeId1 = _br.ReadByte();
		this.intensity1 = (float)_br.ReadByte() / 255f;
		this.biomeId2 = _br.ReadByte();
		this.intensity2 = (float)_br.ReadByte() / 255f;
		this.biomeId3 = _br.ReadByte();
		this.intensity3 = (float)_br.ReadByte() / 255f;
	}

	// Token: 0x06005F85 RID: 24453 RVA: 0x0025CCE4 File Offset: 0x0025AEE4
	public bool Equals(BiomeIntensity other)
	{
		return this.biomeId0 == other.biomeId0 && this.biomeId1 == other.biomeId1 && this.biomeId2 == other.biomeId2 && this.biomeId3 == other.biomeId3 && this.intensity0and1 == other.intensity0and1 && this.intensity2and3 == other.intensity2and3;
	}

	// Token: 0x06005F86 RID: 24454 RVA: 0x0025CD47 File Offset: 0x0025AF47
	public override bool Equals(object obj)
	{
		return obj != null && obj is BiomeIntensity && this.Equals((BiomeIntensity)obj);
	}

	// Token: 0x06005F87 RID: 24455 RVA: 0x0025CD64 File Offset: 0x0025AF64
	public override int GetHashCode()
	{
		return ((((this.biomeId0.GetHashCode() * 397 ^ this.biomeId1.GetHashCode()) * 397 ^ this.biomeId2.GetHashCode()) * 397 ^ this.biomeId3.GetHashCode()) * 397 ^ this.intensity0and1.GetHashCode()) * 397 ^ this.intensity2and3.GetHashCode();
	}

	// Token: 0x06005F88 RID: 24456 RVA: 0x0025CDD8 File Offset: 0x0025AFD8
	public override string ToString()
	{
		return string.Format("[b0={0} b1={1} i0={2} i1={3}]", new object[]
		{
			this.biomeId0,
			this.biomeId1,
			this.intensity0.ToCultureInvariantString("0.0"),
			this.intensity1.ToCultureInvariantString("0.0")
		});
	}

	// Token: 0x04004AE8 RID: 19176
	public const int cDataSize = 6;

	// Token: 0x04004AE9 RID: 19177
	public static BiomeIntensity Default = new BiomeIntensity(0);

	// Token: 0x04004AEA RID: 19178
	public byte biomeId0;

	// Token: 0x04004AEB RID: 19179
	public byte biomeId1;

	// Token: 0x04004AEC RID: 19180
	public byte biomeId2;

	// Token: 0x04004AED RID: 19181
	public byte biomeId3;

	// Token: 0x04004AEE RID: 19182
	public byte intensity0and1;

	// Token: 0x04004AEF RID: 19183
	public byte intensity2and3;
}
