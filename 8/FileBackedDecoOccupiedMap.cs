using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C24 RID: 3108
public class FileBackedDecoOccupiedMap : IDisposable
{
	// Token: 0x06005EC7 RID: 24263 RVA: 0x0024FCF4 File Offset: 0x0024DEF4
	public FileBackedDecoOccupiedMap(int _worldWidth, int _worldHeight)
	{
		this.width = _worldWidth;
		this.height = _worldHeight;
		this.heightHalf = this.height / 2;
		this.occupiedMap = new FileBackedArray<EnumDecoOccupied>(this.width * this.height);
		this.cacheLength = this.width * 128;
	}

	// Token: 0x06005EC8 RID: 24264 RVA: 0x0024FD4D File Offset: 0x0024DF4D
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetDecoChunkRowCacheStart(int offset)
	{
		return offset / this.cacheLength * this.cacheLength;
	}

	// Token: 0x06005EC9 RID: 24265 RVA: 0x0024FD60 File Offset: 0x0024DF60
	[PublicizedFrom(EAccessModifier.Private)]
	public void Cache(int offset)
	{
		if (offset >= this.cacheEnd || offset < this.cacheStart)
		{
			this.cacheStart = this.GetDecoChunkRowCacheStart(offset);
			this.cacheEnd = this.cacheStart + this.cacheLength;
			IBackedArrayHandle backedArrayHandle = this.cacheHandle;
			if (backedArrayHandle != null)
			{
				backedArrayHandle.Dispose();
			}
			this.cacheHandle = this.occupiedMap.GetReadOnlyMemory(this.cacheStart, this.cacheLength, out this.cache);
		}
	}

	// Token: 0x06005ECA RID: 24266 RVA: 0x0024FDD4 File Offset: 0x0024DFD4
	public unsafe EnumDecoOccupied Get(int _offs)
	{
		this.Cache(_offs);
		return (EnumDecoOccupied)(*this.cache.Span[_offs - this.cacheStart]);
	}

	// Token: 0x06005ECB RID: 24267 RVA: 0x0024FE04 File Offset: 0x0024E004
	public void CopyDecoChunkRow(int row, EnumDecoOccupied[] from)
	{
		int num = this.heightHalf / 128;
		int start = (row + num) * 128 * this.width;
		Span<EnumDecoOccupied> destination;
		using (this.occupiedMap.GetSpan(start, this.cacheLength, out destination))
		{
			from.AsSpan(start, this.cacheLength).CopyTo(destination);
		}
	}

	// Token: 0x06005ECC RID: 24268 RVA: 0x0024FE78 File Offset: 0x0024E078
	public void Dispose()
	{
		IBackedArrayHandle backedArrayHandle = this.cacheHandle;
		if (backedArrayHandle != null)
		{
			backedArrayHandle.Dispose();
		}
		this.cacheHandle = null;
		FileBackedArray<EnumDecoOccupied> fileBackedArray = this.occupiedMap;
		if (fileBackedArray != null)
		{
			fileBackedArray.Dispose();
		}
		this.occupiedMap = null;
	}

	// Token: 0x06005ECD RID: 24269 RVA: 0x0024FEAC File Offset: 0x0024E0AC
	public void SaveAsTexture(string path, bool includeFlatAreas = false, List<FlatArea> flatAreas = null)
	{
		Color32[] array = new Color32[this.occupiedMap.Length];
		for (int i = 0; i < this.occupiedMap.Length; i++)
		{
			Color c = Color.black;
			switch (this.Get(i))
			{
			case EnumDecoOccupied.SmallSlope:
				c = Color.blue;
				break;
			case EnumDecoOccupied.Stop_BigDeco:
				c = Color.gray;
				break;
			case EnumDecoOccupied.Perimeter:
				c = Color.red;
				break;
			case EnumDecoOccupied.Stop_AnyDeco:
				c = Color.cyan;
				break;
			case EnumDecoOccupied.Deco:
				c = Color.green;
				break;
			case EnumDecoOccupied.POI:
				c = Color.magenta;
				break;
			case EnumDecoOccupied.BigSlope:
				c = Color.yellow;
				break;
			case EnumDecoOccupied.NoneAllowed:
				c = Color.white;
				break;
			}
			array[i] = c;
		}
		if (includeFlatAreas)
		{
			if (flatAreas == null || flatAreas.Count == 0)
			{
				FlatAreaManager flatAreaManager = GameManager.Instance.World.FlatAreaManager;
				flatAreas = ((flatAreaManager != null) ? flatAreaManager.GetAllFlatAreas() : null);
			}
			if (flatAreas != null)
			{
				foreach (FlatArea flatArea in flatAreas)
				{
					foreach (Vector2i vector2i in flatArea.GetPositions())
					{
						Color red;
						if (vector2i.x == flatArea.position.x && vector2i.y == flatArea.position.z)
						{
							red = Color.red;
						}
						else if (flatArea.size == 16)
						{
							red = new Color(0.75f, 0.75f, 0.75f, 1f);
						}
						else
						{
							red = new Color(0.5f, 0.5f, 0.5f, 1f);
						}
						int num = DecoManager.CheckPosition(this.width, this.height, vector2i.x, vector2i.y);
						array[num] = red;
					}
				}
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height);
		texture2D.SetPixels32(array);
		texture2D.Apply();
		TextureUtils.SaveTexture(texture2D, path);
		Log.Out("Saved deco texture to {0}", new object[]
		{
			path
		});
		UnityEngine.Object.Destroy(texture2D);
	}

	// Token: 0x040049AC RID: 18860
	[PublicizedFrom(EAccessModifier.Private)]
	public FileBackedArray<EnumDecoOccupied> occupiedMap;

	// Token: 0x040049AD RID: 18861
	[PublicizedFrom(EAccessModifier.Private)]
	public int width;

	// Token: 0x040049AE RID: 18862
	[PublicizedFrom(EAccessModifier.Private)]
	public int height;

	// Token: 0x040049AF RID: 18863
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightHalf;

	// Token: 0x040049B0 RID: 18864
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheLength;

	// Token: 0x040049B1 RID: 18865
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayHandle cacheHandle;

	// Token: 0x040049B2 RID: 18866
	[PublicizedFrom(EAccessModifier.Private)]
	public ReadOnlyMemory<EnumDecoOccupied> cache;

	// Token: 0x040049B3 RID: 18867
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheStart;

	// Token: 0x040049B4 RID: 18868
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheEnd;
}
