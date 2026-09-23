using System;
using Platform;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020011D8 RID: 4568
public static class GameOptionsPlatforms
{
	// Token: 0x06009250 RID: 37456 RVA: 0x0037310D File Offset: 0x0037130D
	public static float GetStreamingMipmapBudget()
	{
		return (float)SystemInfo.graphicsMemorySize * 0.9f;
	}

	// Token: 0x06009251 RID: 37457 RVA: 0x0037311C File Offset: 0x0037131C
	public static string GetItemIconFilterString()
	{
		string result = "mip0";
		if ((float)SystemInfo.graphicsMemorySize <= 3200f || SystemInfo.systemMemorySize < 6800)
		{
			result = "mip1";
		}
		return result;
	}

	// Token: 0x06009252 RID: 37458 RVA: 0x00373150 File Offset: 0x00371350
	public static int CalcTextureQualityMin()
	{
		int systemMemorySize = SystemInfo.systemMemorySize;
		if (SystemInfo.graphicsMemorySize < 2400 || systemMemorySize < 4900)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06009253 RID: 37459 RVA: 0x0037317C File Offset: 0x0037137C
	public static GameOptionsPlatforms.GfxPreset CalcDefaultGfxPreset()
	{
		if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
		{
			GameOptionsPlatforms.GfxPreset result = GameOptionsPlatforms.GfxPreset.Medium;
			float num = (float)SystemInfo.systemMemorySize;
			float num2 = (float)SystemInfo.graphicsMemorySize;
			if (!SystemInfo.operatingSystem.Contains(" Steam ") && (num2 < 2400f || num < 4800f))
			{
				result = GameOptionsPlatforms.GfxPreset.Low;
			}
			if (num2 > 7500f && num > 5200f)
			{
				string text = SystemInfo.graphicsDeviceVendor.ToLower();
				if (text.Contains("nvidia"))
				{
					if (!GameOptionsPlatforms.FindGfxName(" 1070, 305"))
					{
						result = GameOptionsPlatforms.GfxPreset.High;
						if (GameOptionsPlatforms.FindGfxName(" 208, 307, 308, 309, 407, 408, 409, 507, 508, 509"))
						{
							result = GameOptionsPlatforms.GfxPreset.Ultra;
						}
					}
				}
				else if ((text == "amd" || text == "ati") && !GameOptionsPlatforms.FindGfxName("RX 570,RX 580,RX 590,RX 5500,RX 65,RX 66"))
				{
					result = GameOptionsPlatforms.GfxPreset.High;
					if (GameOptionsPlatforms.FindGfxName(" 680, 690, 695, 770, 780, 790, 907, 908, 909"))
					{
						result = GameOptionsPlatforms.GfxPreset.Ultra;
					}
				}
			}
			return result;
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			return GameOptionsPlatforms.GfxPreset.ConsolePerformance;
		}
		return GameOptionsPlatforms.GfxPreset.Medium;
	}

	// Token: 0x17001173 RID: 4467
	// (get) Token: 0x06009254 RID: 37460 RVA: 0x00373251 File Offset: 0x00371451
	public static int DefaultUpscalerMode
	{
		get
		{
			if (!FSR3.FSR3Supported())
			{
				return 4;
			}
			return 2;
		}
	}

	// Token: 0x06009255 RID: 37461 RVA: 0x00373260 File Offset: 0x00371460
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool FindGfxName(string names)
	{
		string text = SystemInfo.graphicsDeviceName.ToLower();
		if (text.Contains("laptop"))
		{
			return false;
		}
		string[] array = names.Split(',', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (text.Contains(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06009256 RID: 37462 RVA: 0x003732AD File Offset: 0x003714AD
	public static int ApplyTextureFilterLimit(int filter)
	{
		if ((float)SystemInfo.graphicsMemorySize < 3200f)
		{
			filter = 0;
		}
		return filter;
	}

	// Token: 0x020011D9 RID: 4569
	public enum GfxPreset
	{
		// Token: 0x04006C0B RID: 27659
		Lowest,
		// Token: 0x04006C0C RID: 27660
		Low,
		// Token: 0x04006C0D RID: 27661
		Medium,
		// Token: 0x04006C0E RID: 27662
		High,
		// Token: 0x04006C0F RID: 27663
		Ultra,
		// Token: 0x04006C10 RID: 27664
		Custom,
		// Token: 0x04006C11 RID: 27665
		ConsolePerformance,
		// Token: 0x04006C12 RID: 27666
		LEGACY_ConsolePerformanceFSR,
		// Token: 0x04006C13 RID: 27667
		ConsoleQuality,
		// Token: 0x04006C14 RID: 27668
		LEGACY_ConsoleQualityFSR,
		// Token: 0x04006C15 RID: 27669
		Simplified = 100
	}

	// Token: 0x020011DA RID: 4570
	public static class UpscalerMode
	{
		// Token: 0x06009257 RID: 37463 RVA: 0x003732C0 File Offset: 0x003714C0
		public static string ToString(int upscalerSettingValue)
		{
			switch (upscalerSettingValue)
			{
			case 0:
				return "Off";
			case 1:
				return "FSR2";
			case 2:
				return "FSR3";
			case 3:
				return "Dynamic Resolution";
			case 4:
				return "Scale";
			case 5:
				return "DLSS";
			default:
				return "Unknown";
			}
		}

		// Token: 0x04006C16 RID: 27670
		public const int Off = 0;

		// Token: 0x04006C17 RID: 27671
		public const int FSR2 = 1;

		// Token: 0x04006C18 RID: 27672
		public const int FSR3 = 2;

		// Token: 0x04006C19 RID: 27673
		public const int DynamicResolution = 3;

		// Token: 0x04006C1A RID: 27674
		public const int Scale = 4;

		// Token: 0x04006C1B RID: 27675
		public const int DLSS = 5;
	}
}
