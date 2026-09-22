using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000B32 RID: 2866
public class ChunkProviderGenerateWorldFromImage : ChunkProviderGenerateWorld
{
	// Token: 0x060056E6 RID: 22246 RVA: 0x002155A4 File Offset: 0x002137A4
	public ChunkProviderGenerateWorldFromImage(string _levelName, PathAbstractions.AbstractedLocation _worldLocation, bool _bLoadDTM = true) : base(_levelName, _worldLocation, false)
	{
		this.bLoadDTM = _bLoadDTM;
	}

	// Token: 0x060056E7 RID: 22247 RVA: 0x002155B6 File Offset: 0x002137B6
	public override void Cleanup()
	{
		HeightMap heightMap = this.heightMap;
		if (heightMap != null)
		{
			heightMap.Dispose();
		}
		this.heightMap = null;
		IBackedArray<ushort> backedArray = this.rawdata;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.rawdata = null;
		base.Cleanup();
	}

	// Token: 0x060056E8 RID: 22248 RVA: 0x002155EE File Offset: 0x002137EE
	public override IEnumerator Init(World _world)
	{
		if (this.bLoadDTM)
		{
			yield return this.<>n__0(_world);
		}
		else
		{
			this.world = _world;
		}
		Stopwatch sw = new Stopwatch();
		sw.Start();
		if (this.bLoadDTM)
		{
			this.loadDTM();
		}
		Log.Out("Loading and parsing of dtm.png took " + sw.ElapsedMilliseconds.ToString() + "ms");
		sw.Reset();
		sw.Start();
		HeightMap heightMap = this.heightMap;
		if (heightMap != null)
		{
			heightMap.Dispose();
		}
		this.heightMap = new HeightMap(this.m_Dtm.DimX, this.m_Dtm.DimY, 255f, this.rawdata, 0);
		this.m_BiomeProvider = new WorldBiomeProviderFromImage(this.levelName, this.worldLocation.FullPath, this.world.Biomes, 4096);
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = new WorldDecoratorPOIFromImage(this.levelName, this.worldLocation.FullPath, this.GetDynamicPrefabDecorator(), this.m_Dtm.DimX, this.m_Dtm.DimY, null, false, 1, this.heightMap, null);
		this.m_Decorators.Add(worldDecoratorPOIFromImage);
		yield return worldDecoratorPOIFromImage.InitData();
		this.m_Decorators.Add(new WorldDecoratorBlocksFromBiome(this.m_BiomeProvider, this.GetDynamicPrefabDecorator()));
		Log.Out("Loading and parsing of generator took " + sw.ElapsedMilliseconds.ToString() + "ms");
		sw.Reset();
		sw.Start();
		if (this.bLoadDTM)
		{
			this.m_TerrainGenerator = new TerrainFromDTM();
			((TerrainFromDTM)this.m_TerrainGenerator).Init(this.m_Dtm, this.m_BiomeProvider, this.levelName, this.worldLocation.FullPath, this.world.Seed);
			string text = _world.IsEditor() ? null : GameIO.GetSaveGameRegionDir();
			this.m_RegionFileManager = new RegionFileManager(text, text, 0, !_world.IsEditor());
			MultiBlockManager.Instance.Initialize(this.m_RegionFileManager);
		}
		yield return null;
		yield break;
	}

	// Token: 0x060056E9 RID: 22249 RVA: 0x00215604 File Offset: 0x00213804
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe void loadDTM()
	{
		Texture2D texture2D = null;
		if (SdFile.Exists(this.worldLocation.FullPath + "/dtm.png"))
		{
			texture2D = TextureUtils.LoadTexture(this.worldLocation.FullPath + "/dtm.png", FilterMode.Point, false, false, null);
			Log.Out("Loading local DTM");
		}
		else if (SdFile.Exists(this.worldLocation.FullPath + "/dtm.tga"))
		{
			texture2D = TextureUtils.LoadTexture(this.worldLocation.FullPath + "/dtm.tga", FilterMode.Point, false, false, null);
			Log.Out("Loading local DTM");
		}
		else
		{
			texture2D = (Resources.Load("Data/Worlds/" + this.levelName + "/dtm", typeof(Texture2D)) as Texture2D);
		}
		Log.Out("DTM image size w= " + texture2D.width.ToString() + ", h = " + texture2D.height.ToString());
		Color[] pixels = texture2D.GetPixels();
		this.m_Dtm = new ArrayWithOffset<byte>(texture2D.width, texture2D.height);
		IBackedArray<ushort> backedArray = this.rawdata;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.rawdata = BackedArrays.Create<ushort>(pixels.Length);
		int width = texture2D.width;
		int height = texture2D.height;
		for (int i = 0; i < height; i++)
		{
			Span<ushort> span2;
			using (this.rawdata.GetSpan(i * width, width, out span2))
			{
				for (int j = 0; j < width; j++)
				{
					this.m_Dtm[j + this.m_Dtm.MinPos.x, i + this.m_Dtm.MinPos.y] = (byte)(pixels[width * i + j].grayscale * 255f);
					*span2[j] = (ushort)((int)((byte)(pixels[width * i + j].grayscale * 255f)) | (int)((byte)((pixels[width * i + j].grayscale * 255f - 255f) * 255f)) << 8);
				}
			}
		}
		Resources.UnloadAsset(texture2D);
	}

	// Token: 0x060056EA RID: 22250 RVA: 0x00215844 File Offset: 0x00213A44
	public override void ReloadAllChunks()
	{
		this.loadDTM();
		((TerrainFromDTM)this.m_TerrainGenerator).Init(this.m_Dtm, this.m_BiomeProvider, this.levelName, this.worldLocation.FullPath, this.world.Seed);
		base.ReloadAllChunks();
	}

	// Token: 0x060056EB RID: 22251 RVA: 0x0002F184 File Offset: 0x0002D384
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.GenerateFromDtm;
	}

	// Token: 0x060056EC RID: 22252 RVA: 0x00215898 File Offset: 0x00213A98
	public override bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = new Vector3i(this.m_Dtm.MinPos.x, 0, this.m_Dtm.MinPos.y);
		_maxSize = new Vector3i(this.m_Dtm.MaxPos.x, 255, this.m_Dtm.MaxPos.y);
		return true;
	}

	// Token: 0x060056ED RID: 22253 RVA: 0x00215904 File Offset: 0x00213B04
	public override int GetPOIBlockIdOverride(int _x, int _z)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = (WorldDecoratorPOIFromImage)this.m_Decorators[0];
		WorldGridCompressedData<byte> poi = worldDecoratorPOIFromImage.m_Poi;
		int x = _x / worldDecoratorPOIFromImage.worldScale;
		int y = _z / worldDecoratorPOIFromImage.worldScale;
		if (!poi.Contains(x, y))
		{
			return 0;
		}
		byte data = poi.GetData(x, y);
		if (data == 0 || data == 255)
		{
			return 0;
		}
		PoiMapElement poiForColor = this.world.Biomes.getPoiForColor((uint)data);
		if (poiForColor == null)
		{
			return 0;
		}
		return poiForColor.m_BlockValue.type;
	}

	// Token: 0x060056EE RID: 22254 RVA: 0x00215988 File Offset: 0x00213B88
	public override float GetPOIHeightOverride(int x, int z)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = (WorldDecoratorPOIFromImage)this.m_Decorators[0];
		WorldGridCompressedData<byte> poi = worldDecoratorPOIFromImage.m_Poi;
		byte data;
		if (!poi.Contains(x / worldDecoratorPOIFromImage.worldScale, z / worldDecoratorPOIFromImage.worldScale) || (data = poi.GetData(x / worldDecoratorPOIFromImage.worldScale, z / worldDecoratorPOIFromImage.worldScale)) == 255 || data == 0)
		{
			return 0f;
		}
		PoiMapElement poiForColor = this.world.Biomes.getPoiForColor((uint)data);
		if (poiForColor == null)
		{
			return 0f;
		}
		if (!poiForColor.m_BlockValue.Block.blockMaterial.IsLiquid)
		{
			return 0f;
		}
		return (float)poiForColor.m_YPosFill;
	}

	// Token: 0x04004344 RID: 17220
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayWithOffset<byte> m_Dtm;

	// Token: 0x04004345 RID: 17221
	public IBackedArray<ushort> rawdata;

	// Token: 0x04004346 RID: 17222
	[PublicizedFrom(EAccessModifier.Private)]
	public HeightMap heightMap;

	// Token: 0x04004347 RID: 17223
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bLoadDTM;
}
