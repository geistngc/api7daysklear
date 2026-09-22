using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C1C RID: 3100
public class DecoOccupiedMap
{
	// Token: 0x06005EA0 RID: 24224 RVA: 0x0024F138 File Offset: 0x0024D338
	public DecoOccupiedMap(int _worldWidth, int _worldHeight)
	{
		this.width = _worldWidth;
		this.height = _worldHeight;
		this.widthHalf = this.width / 2;
		this.heightHalf = this.height / 2;
		this.occupiedMap = new EnumDecoOccupied[this.width * this.height];
	}

	// Token: 0x06005EA1 RID: 24225 RVA: 0x0024F18D File Offset: 0x0024D38D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EnumDecoOccupied Get(int _offs)
	{
		return this.occupiedMap[_offs];
	}

	// Token: 0x06005EA2 RID: 24226 RVA: 0x0024F198 File Offset: 0x0024D398
	public EnumDecoOccupied Get(int _x, int _z)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num >= 0)
		{
			return this.occupiedMap[num];
		}
		return EnumDecoOccupied.NoneAllowed;
	}

	// Token: 0x06005EA3 RID: 24227 RVA: 0x0024F1C7 File Offset: 0x0024D3C7
	public void Set(int _offs, EnumDecoOccupied _v)
	{
		this.occupiedMap[_offs] = _v;
	}

	// Token: 0x06005EA4 RID: 24228 RVA: 0x0024F1D4 File Offset: 0x0024D3D4
	public void Set(int _x, int _z, EnumDecoOccupied _v)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num >= 0)
		{
			this.occupiedMap[num] = _v;
		}
	}

	// Token: 0x06005EA5 RID: 24229 RVA: 0x0024F204 File Offset: 0x0024D404
	public bool CheckArea(int _x, int _z, EnumDecoOccupied _v, int _rectSizeX, int _rectSizeZ)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num < 0)
		{
			return true;
		}
		for (int i = 0; i < _rectSizeZ; i++)
		{
			for (int j = 0; j < _rectSizeX; j++)
			{
				if (num >= this.occupiedMap.Length)
				{
					return true;
				}
				if (this.occupiedMap[num] >= _v)
				{
					return true;
				}
				num++;
			}
			num += this.width - _rectSizeX;
		}
		return false;
	}

	// Token: 0x06005EA6 RID: 24230 RVA: 0x0024F270 File Offset: 0x0024D470
	public void SetArea(int _x, int _z, EnumDecoOccupied _v, int _rectSizeX, int _rectSizeZ)
	{
		int num = _x + this.widthHalf + (_z + this.heightHalf) * this.width;
		for (int i = 0; i < _rectSizeZ; i++)
		{
			for (int j = 0; j < _rectSizeX; j++)
			{
				if (num < 0 || num >= this.occupiedMap.Length)
				{
					num++;
				}
				else
				{
					if (this.occupiedMap[num] < _v)
					{
						this.occupiedMap[num] = _v;
					}
					num++;
				}
			}
			num += this.width - _rectSizeX;
		}
	}

	// Token: 0x06005EA7 RID: 24231 RVA: 0x0024F2E9 File Offset: 0x0024D4E9
	public EnumDecoOccupied[] GetData()
	{
		return this.occupiedMap;
	}

	// Token: 0x06005EA8 RID: 24232 RVA: 0x0024F2F4 File Offset: 0x0024D4F4
	public void SaveAsTexture(string path, bool includeFlatAreas = false, List<FlatArea> flatAreas = null)
	{
		Color32[] array = new Color32[this.occupiedMap.Length];
		for (int i = 0; i < this.occupiedMap.Length; i++)
		{
			Color c = Color.black;
			switch (this.occupiedMap[i])
			{
			case EnumDecoOccupied.SmallSlope:
				c = Color.blue;
				break;
			case EnumDecoOccupied.Stop_BigDeco:
				c = Color.gray;
				break;
			case EnumDecoOccupied.Perimeter:
				if (!includeFlatAreas)
				{
					c = Color.red;
				}
				break;
			case EnumDecoOccupied.Stop_AnyDeco:
				c = Color.cyan;
				break;
			case EnumDecoOccupied.Deco:
				if (!includeFlatAreas)
				{
					c = Color.green;
				}
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

	// Token: 0x04004991 RID: 18833
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDecoOccupied[] occupiedMap;

	// Token: 0x04004992 RID: 18834
	[PublicizedFrom(EAccessModifier.Private)]
	public int width;

	// Token: 0x04004993 RID: 18835
	[PublicizedFrom(EAccessModifier.Private)]
	public int height;

	// Token: 0x04004994 RID: 18836
	[PublicizedFrom(EAccessModifier.Private)]
	public int widthHalf;

	// Token: 0x04004995 RID: 18837
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightHalf;
}
