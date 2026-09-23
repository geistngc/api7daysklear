using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CEE RID: 3310
public class FlatArea
{
	// Token: 0x17000A93 RID: 2707
	// (get) Token: 0x06006550 RID: 25936 RVA: 0x00279A7E File Offset: 0x00277C7E
	public int Height
	{
		get
		{
			return this.position.y;
		}
	}

	// Token: 0x17000A94 RID: 2708
	// (get) Token: 0x06006551 RID: 25937 RVA: 0x00279A8B File Offset: 0x00277C8B
	public int MinX
	{
		get
		{
			return this.position.x;
		}
	}

	// Token: 0x17000A95 RID: 2709
	// (get) Token: 0x06006552 RID: 25938 RVA: 0x00279A98 File Offset: 0x00277C98
	public int MaxX
	{
		get
		{
			return this.position.x + this.size - 1;
		}
	}

	// Token: 0x17000A96 RID: 2710
	// (get) Token: 0x06006553 RID: 25939 RVA: 0x00279AAE File Offset: 0x00277CAE
	public int MinZ
	{
		get
		{
			return this.position.z;
		}
	}

	// Token: 0x17000A97 RID: 2711
	// (get) Token: 0x06006554 RID: 25940 RVA: 0x00279ABB File Offset: 0x00277CBB
	public int MaxZ
	{
		get
		{
			return this.position.z + this.size - 1;
		}
	}

	// Token: 0x17000A98 RID: 2712
	// (get) Token: 0x06006555 RID: 25941 RVA: 0x00279AD1 File Offset: 0x00277CD1
	public Vector3 Center
	{
		get
		{
			return this.position + new Vector3((float)(this.size / 2), 0f, (float)(this.size / 2));
		}
	}

	// Token: 0x06006556 RID: 25942 RVA: 0x00279AFF File Offset: 0x00277CFF
	public FlatArea(Vector3i _position, int _size)
	{
		this.position = _position;
		this.size = _size;
	}

	// Token: 0x06006557 RID: 25943 RVA: 0x00279B18 File Offset: 0x00277D18
	public bool IsValid(World world, BiomeFilterTypes biomeFilter = BiomeFilterTypes.AnyBiome, string[] biomeNames = null, ChunkProtectionLevel maxAllowedChunkProtectionLevel = ChunkProtectionLevel.NearLandClaim)
	{
		this.maxChunkProtectionLevel = ChunkProtectionLevel.None;
		int num = this.MinX >> 4 << 4;
		int num2 = this.MaxX >> 4 << 4;
		int num3 = this.MinZ >> 4 << 4;
		int num4 = this.MaxZ >> 4 << 4;
		for (int i = num; i <= num2; i += 16)
		{
			for (int j = num3; j <= num4; j += 16)
			{
				ChunkProtectionLevel chunkProtectionLevel = world.ChunkCache.ChunkProvider.GetChunkProtectionLevel(new Vector3i(i, this.position.y, j));
				if (chunkProtectionLevel > this.maxChunkProtectionLevel)
				{
					this.maxChunkProtectionLevel = chunkProtectionLevel;
				}
			}
		}
		if (this.maxChunkProtectionLevel > maxAllowedChunkProtectionLevel)
		{
			return false;
		}
		if (biomeFilter != BiomeFilterTypes.AnyBiome)
		{
			BiomeDefinition biomeInWorld = GameManager.Instance.World.GetBiomeInWorld((int)this.Center.x, (int)this.Center.z);
			if (biomeInWorld == null)
			{
				return false;
			}
			if (biomeFilter == BiomeFilterTypes.OnlyBiome)
			{
				if (biomeInWorld.m_sBiomeName != biomeNames[0])
				{
					return false;
				}
			}
			else if (biomeFilter == BiomeFilterTypes.ExcludeBiome)
			{
				bool flag = false;
				for (int k = 0; k < biomeNames.Length; k++)
				{
					if (biomeInWorld.m_sBiomeName == biomeNames[k])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return false;
				}
			}
			else if (biomeFilter == BiomeFilterTypes.SameBiome && biomeInWorld.m_sBiomeName != biomeNames[0])
			{
				return false;
			}
		}
		return !world.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator().HasPrefabsAtXZ(this.MinX, this.MaxX, this.MinZ, this.MaxZ);
	}

	// Token: 0x06006558 RID: 25944 RVA: 0x00279C82 File Offset: 0x00277E82
	public bool IsInArea(int _x, int _z)
	{
		return _x >= this.MinX && _x <= this.MaxX && _z >= this.MinZ && _z <= this.MaxZ;
	}

	// Token: 0x06006559 RID: 25945 RVA: 0x00279CB0 File Offset: 0x00277EB0
	public List<Vector2i> GetPositions()
	{
		List<Vector2i> list = new List<Vector2i>();
		for (int i = this.MinX; i <= this.MaxX; i++)
		{
			for (int j = this.MinZ; j <= this.MaxZ; j++)
			{
				list.Add(new Vector2i(i, j));
			}
		}
		return list;
	}

	// Token: 0x0600655A RID: 25946 RVA: 0x00279D00 File Offset: 0x00277F00
	public Vector3 GetRandomPosition(float margin = 0f)
	{
		return new Vector3(UnityEngine.Random.Range((float)this.MinX + margin, (float)this.MaxX - margin + 1f), (float)this.Height, UnityEngine.Random.Range((float)this.MinZ + margin, (float)this.MaxZ - margin + 1f));
	}

	// Token: 0x0600655B RID: 25947 RVA: 0x00279D54 File Offset: 0x00277F54
	public override bool Equals(object obj)
	{
		FlatArea flatArea = obj as FlatArea;
		return flatArea != null && this.position == flatArea.position && this.size == flatArea.size;
	}

	// Token: 0x0600655C RID: 25948 RVA: 0x000111D7 File Offset: 0x0000F3D7
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x04004ED0 RID: 20176
	public Vector3i position;

	// Token: 0x04004ED1 RID: 20177
	public int size;

	// Token: 0x04004ED2 RID: 20178
	public ChunkProtectionLevel maxChunkProtectionLevel;
}
