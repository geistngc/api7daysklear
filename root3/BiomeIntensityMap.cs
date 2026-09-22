using System;
using System.IO;

// Token: 0x02000C42 RID: 3138
public class BiomeIntensityMap
{
	// Token: 0x06005F8A RID: 24458 RVA: 0x0000640C File Offset: 0x0000460C
	public BiomeIntensityMap()
	{
	}

	// Token: 0x06005F8B RID: 24459 RVA: 0x0025CE44 File Offset: 0x0025B044
	public BiomeIntensityMap(int _w, int _h)
	{
		this.intensities = new ArrayWithOffset<BiomeIntensity>(_w, _h);
	}

	// Token: 0x06005F8C RID: 24460 RVA: 0x0025CE5C File Offset: 0x0025B05C
	public void Load(string _worldName)
	{
		try
		{
			string path = PathAbstractions.WorldsSearchPaths.GetLocation(_worldName, null, null).FullPath + "/biomeintensity.dat";
			if (!SdFile.Exists(path))
			{
				this.intensities = null;
			}
			else
			{
				using (Stream stream = SdFile.Open(path, FileMode.Open))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						pooledBinaryReader.ReadByte();
						pooledBinaryReader.ReadByte();
						pooledBinaryReader.ReadByte();
						pooledBinaryReader.ReadByte();
						pooledBinaryReader.ReadByte();
						int num = (int)pooledBinaryReader.ReadUInt16();
						int num2 = (int)pooledBinaryReader.ReadUInt16();
						this.intensities = new ArrayWithOffset<BiomeIntensity>(num, num2);
						num /= 2;
						num2 /= 2;
						for (int i = -num; i < num; i++)
						{
							for (int j = -num2; j < num2; j++)
							{
								BiomeIntensity value = default(BiomeIntensity);
								value.Read(pooledBinaryReader);
								this.intensities[i, j] = value;
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("Reading biome intensity map: " + ex.Message);
		}
	}

	// Token: 0x06005F8D RID: 24461 RVA: 0x0025CFEC File Offset: 0x0025B1EC
	public void Save(string _worldPath)
	{
		try
		{
			using (Stream stream = SdFile.Open(_worldPath + "/biomeintensity.dat", FileMode.Create))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(stream);
					pooledBinaryWriter.Write(66);
					pooledBinaryWriter.Write(73);
					pooledBinaryWriter.Write(73);
					pooledBinaryWriter.Write(0);
					pooledBinaryWriter.Write(1);
					int num = this.intensities.DimX;
					int num2 = this.intensities.DimY;
					pooledBinaryWriter.Write((ushort)num);
					pooledBinaryWriter.Write((ushort)num2);
					num /= 2;
					num2 /= 2;
					for (int i = -num; i < num; i++)
					{
						for (int j = -num2; j < num2; j++)
						{
							this.intensities[i, j].Write(pooledBinaryWriter);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("Writing biome intensity map: " + ex.Message);
		}
	}

	// Token: 0x06005F8E RID: 24462 RVA: 0x0025D10C File Offset: 0x0025B30C
	public void SetBiomeIntensity(int _x, int _y, BiomeIntensity _bi)
	{
		if (this.intensities != null && this.intensities.Contains(_x, _y))
		{
			this.intensities[_x, _y] = _bi;
		}
	}

	// Token: 0x06005F8F RID: 24463 RVA: 0x0025D133 File Offset: 0x0025B333
	public BiomeIntensity GetBiomeIntensity(int _x, int _y)
	{
		if (this.intensities != null && this.intensities.Contains(_x, _y))
		{
			return this.intensities[_x, _y];
		}
		return BiomeIntensity.Default;
	}

	// Token: 0x04004AF0 RID: 19184
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayWithOffset<BiomeIntensity> intensities;
}
