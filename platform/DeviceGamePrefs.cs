using System;
using System.IO;

namespace Platform
{
	// Token: 0x02001B49 RID: 6985
	public static class DeviceGamePrefs
	{
		// Token: 0x0600D145 RID: 53573 RVA: 0x004C8B28 File Offset: 0x004C6D28
		public static void Apply()
		{
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
			{
				GameOptionsPlatforms.GfxPreset @int = (GameOptionsPlatforms.GfxPreset)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset);
				if (@int != GameOptionsPlatforms.GfxPreset.ConsolePerformance && @int != GameOptionsPlatforms.GfxPreset.ConsoleQuality)
				{
					Log.Out(string.Format("[DeviceGamePrefs] Quality preset \"{0}\" is unsupported on this platform; defaulting to ConsolePerformance.", @int));
					GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, 6);
				}
				int int2 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode);
				if (int2 != 2 && int2 != 4)
				{
					Log.Out(string.Format("[DeviceGamePrefs] Upscaler mode \"{0}\" is unsupported on this platform; defaulting to \"{1}\".", int2, GameOptionsPlatforms.DefaultUpscalerMode));
					GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, GameOptionsPlatforms.DefaultUpscalerMode);
				}
				GameOptionsManager.SetGraphicsQuality();
			}
			DeviceGamePrefs.ApplyConfigFilePrefs();
		}

		// Token: 0x0600D146 RID: 53574 RVA: 0x004C8BBC File Offset: 0x004C6DBC
		public static string ConfigFilename(string _deviceName)
		{
			return "gameprefs_" + _deviceName;
		}

		// Token: 0x0600D147 RID: 53575 RVA: 0x004C8BCC File Offset: 0x004C6DCC
		[PublicizedFrom(EAccessModifier.Private)]
		public static void ApplyConfigFilePrefs()
		{
			string text = DeviceGamePrefs.ConfigFilename(DeviceFlag.StandaloneWindows.GetDeviceName());
			string text2 = Path.Combine(GameIO.GetApplicationPath(), text + ".xml");
			if (File.Exists(text2))
			{
				Log.Out("[DeviceGamePrefs] Applying game prefs from {0}", new object[]
				{
					text2
				});
				DynamicProperties dynamicProperties = new DynamicProperties();
				if (dynamicProperties.Load(GameIO.GetApplicationPath(), text))
				{
					DeviceGamePrefs.ApplyGamePrefs(dynamicProperties);
				}
			}
		}

		// Token: 0x0600D148 RID: 53576 RVA: 0x004C8C34 File Offset: 0x004C6E34
		[PublicizedFrom(EAccessModifier.Private)]
		public static void ApplyGamePrefs(DynamicProperties properties)
		{
			foreach (string text in properties.Values.Keys)
			{
				EnumGamePrefs enumGamePrefs;
				if (EnumUtils.TryParse<EnumGamePrefs>(text, out enumGamePrefs, true))
				{
					object obj = GamePrefs.Parse(enumGamePrefs, properties.Values[text]);
					if (obj != null)
					{
						GamePrefs.SetObject(enumGamePrefs, obj);
						Log.Out("[DeviceGamePrefs] {0}={1}", new object[]
						{
							text,
							GamePrefs.GetObject(enumGamePrefs)
						});
					}
					else
					{
						Log.Error("[DeviceGamePrefs] Invalid value for GamePref: {0}", new object[]
						{
							text
						});
					}
				}
				else
				{
					Log.Error("[DeviceGamePrefs] Unknown GamePref: {0}", new object[]
					{
						text
					});
				}
			}
		}
	}
}
