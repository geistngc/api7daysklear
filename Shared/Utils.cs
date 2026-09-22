using System;
using System.IO;
using System.Text;
using System.Threading;
using InControl;
using UnityEngine;

namespace Platform.Shared
{
	// Token: 0x02001CBB RID: 7355
	public class Utils : IUtils
	{
		// Token: 0x0600DA34 RID: 55860 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600DA35 RID: 55861 RVA: 0x004E6044 File Offset: 0x004E4244
		public virtual bool OpenBrowser(string _url)
		{
			return Utils.OpenSystemBrowser(_url);
		}

		// Token: 0x0600DA36 RID: 55862 RVA: 0x000027FC File Offset: 0x000009FC
		public void ControllerDisconnected(InputDevice inputDevice)
		{
		}

		// Token: 0x0600DA37 RID: 55863 RVA: 0x004E604C File Offset: 0x004E424C
		public virtual string GetPlatformLanguage()
		{
			if (this.platformLanguageCache == null)
			{
				string text = Application.systemLanguage.ToStringCached<SystemLanguage>().ToLower();
				string text2;
				if (!(text == "chinesesimplified"))
				{
					if (!(text == "chinesetraditional"))
					{
						if (!(text == "korean"))
						{
							text2 = text;
						}
						else
						{
							text2 = "koreana";
						}
					}
					else
					{
						text2 = "tchinese";
					}
				}
				else
				{
					text2 = "schinese";
				}
				text = text2;
				this.platformLanguageCache = text;
			}
			return this.platformLanguageCache;
		}

		// Token: 0x0600DA38 RID: 55864 RVA: 0x004E60C3 File Offset: 0x004E42C3
		public virtual string GetAppLanguage()
		{
			return this.GetPlatformLanguage();
		}

		// Token: 0x0600DA39 RID: 55865 RVA: 0x004E60CB File Offset: 0x004E42CB
		public virtual string GetCountry()
		{
			return "??";
		}

		// Token: 0x0600DA3A RID: 55866 RVA: 0x004E60D2 File Offset: 0x004E42D2
		public virtual string GetStoreBranchName()
		{
			return "none";
		}

		// Token: 0x0600DA3B RID: 55867 RVA: 0x004E4A63 File Offset: 0x004E2C63
		public virtual void ClearTempFiles()
		{
			Utils.TryDeleteTempCacheContents();
		}

		// Token: 0x0600DA3C RID: 55868 RVA: 0x004E4A6A File Offset: 0x004E2C6A
		public virtual string GetTempFileName(string prefix = "", string suffix = "")
		{
			return Utils.GetRandomTempCacheFileName(prefix, suffix);
		}

		// Token: 0x0600DA3D RID: 55869 RVA: 0x004E60DC File Offset: 0x004E42DC
		public bool? IsFamilyShare()
		{
			return null;
		}

		// Token: 0x0600DA3E RID: 55870 RVA: 0x004E60F2 File Offset: 0x004E42F2
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void TryDeleteTempCacheContents()
		{
			Utils.TryDeleteTempDirectoryContentsExceptCrashes(PlatformApplicationManager.Application.temporaryCachePath);
		}

		// Token: 0x0600DA3F RID: 55871 RVA: 0x004E6104 File Offset: 0x004E4304
		[PublicizedFrom(EAccessModifier.Private)]
		public static void TryDeleteTempDirectoryContentsExceptCrashes(string path)
		{
			try
			{
				if (Directory.Exists(path))
				{
					foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(path).EnumerateFileSystemInfos())
					{
						try
						{
							if (!fileSystemInfo.Name.EqualsCaseInsensitive("Crashes"))
							{
								DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
								if (directoryInfo != null)
								{
									directoryInfo.Delete(true);
								}
								else
								{
									fileSystemInfo.Delete();
								}
							}
						}
						catch (Exception ex)
						{
							Log.Warning(string.Concat(new string[]
							{
								"[Platform.Shared.Utils] Could not delete '",
								fileSystemInfo.Name,
								"' from temp cache. ",
								ex.GetType().FullName,
								": ",
								ex.Message
							}));
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Warning("[Platform.Shared.Utils] Could not delete contents of temp cache. " + ex2.GetType().FullName + ": " + ex2.Message);
			}
		}

		// Token: 0x0600DA40 RID: 55872 RVA: 0x004E6224 File Offset: 0x004E4424
		[PublicizedFrom(EAccessModifier.Internal)]
		public static string GetRandomTempCacheFileName(string prefix, string suffix)
		{
			return Utils.GetRandomFileName(PlatformApplicationManager.Application.temporaryCachePath, prefix, suffix);
		}

		// Token: 0x0600DA41 RID: 55873 RVA: 0x004E6238 File Offset: 0x004E4438
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetRandomFileName(string parentDir, string prefix, string suffix)
		{
			for (int i = 0; i < 100; i++)
			{
				string randomName = Utils.GetRandomName(prefix, suffix);
				string text = Path.Join(parentDir, randomName);
				if (!File.Exists(text))
				{
					using (File.Open(text, FileMode.OpenOrCreate))
					{
						return text;
					}
				}
			}
			throw new IOException(string.Format("Failed to create a temporary file after {0} attempts.", 100));
		}

		// Token: 0x0600DA42 RID: 55874 RVA: 0x004E62B4 File Offset: 0x004E44B4
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetRandomName(string prefix, string suffix)
		{
			System.Random value = Utils.RandLocal.Value;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix);
			for (int i = 0; i < 16; i++)
			{
				stringBuilder.Append("0123456789ABCDEFGHIJKLMNOPabcdefghijklmnop"[value.Next("0123456789ABCDEFGHIJKLMNOPabcdefghijklmnop".Length)]);
			}
			stringBuilder.Append(suffix);
			return stringBuilder.ToString();
		}

		// Token: 0x0600DA43 RID: 55875 RVA: 0x0004E558 File Offset: 0x0004C758
		public virtual string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform)
		{
			return string.Empty;
		}

		// Token: 0x0400A5AE RID: 42414
		[PublicizedFrom(EAccessModifier.Private)]
		public static int Seed = Environment.TickCount;

		// Token: 0x0400A5AF RID: 42415
		public static readonly ThreadLocal<System.Random> RandLocal = new ThreadLocal<System.Random>(() => new System.Random(Interlocked.Increment(ref Utils.Seed)));

		// Token: 0x0400A5B0 RID: 42416
		[PublicizedFrom(EAccessModifier.Private)]
		public string platformLanguageCache;
	}
}
