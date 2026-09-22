using System;

// Token: 0x02000158 RID: 344
public class BlockTextureData
{
	// Token: 0x06000980 RID: 2432 RVA: 0x00041D3E File Offset: 0x0003FF3E
	public static void InitStatic()
	{
		BlockTextureData.list = new BlockTextureData[256];
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00041D4F File Offset: 0x0003FF4F
	public void Init()
	{
		BlockTextureData.list[this.ID] = this;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00041D5E File Offset: 0x0003FF5E
	public static void Cleanup()
	{
		BlockTextureData.list = null;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00041D68 File Offset: 0x0003FF68
	public static BlockTextureData GetDataByTextureID(int textureID)
	{
		for (int i = 0; i < BlockTextureData.list.Length; i++)
		{
			if (BlockTextureData.list[i] != null && (int)BlockTextureData.list[i].TextureID == textureID)
			{
				return BlockTextureData.list[i];
			}
		}
		return null;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00041DA8 File Offset: 0x0003FFA8
	public bool GetLocked(EntityPlayerLocal player)
	{
		if (this.LockedByPerk != "")
		{
			ProgressionValue progressionValue = player.Progression.GetProgressionValue(this.LockedByPerk);
			if (progressionValue != null && progressionValue.CalculatedLevel(player) >= (int)this.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040009C9 RID: 2505
	public static BlockTextureData[] list;

	// Token: 0x040009CA RID: 2506
	public int ID;

	// Token: 0x040009CB RID: 2507
	public ushort TextureID;

	// Token: 0x040009CC RID: 2508
	public string Name;

	// Token: 0x040009CD RID: 2509
	public string LocalizedName;

	// Token: 0x040009CE RID: 2510
	public string Group;

	// Token: 0x040009CF RID: 2511
	public ushort PaintCost;

	// Token: 0x040009D0 RID: 2512
	public bool Hidden;

	// Token: 0x040009D1 RID: 2513
	public byte SortIndex = byte.MaxValue;

	// Token: 0x040009D2 RID: 2514
	public string LockedByPerk = "";

	// Token: 0x040009D3 RID: 2515
	public ushort RequiredLevel;
}
