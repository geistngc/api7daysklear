using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Platform.Shared;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001BCC RID: 7116
	public interface IPlatformApplication
	{
		// Token: 0x17001A30 RID: 6704
		// (get) Token: 0x0600D3CF RID: 54223
		Resolution[] SupportedResolutions { get; }

		// Token: 0x17001A31 RID: 6705
		// (get) Token: 0x0600D3D0 RID: 54224
		[TupleElementNames(new string[]
		{
			"width",
			"height",
			"fullScreenMode"
		})]
		ValueTuple<int, int, FullScreenMode> ScreenOptions { [return: TupleElementNames(new string[]
		{
			"width",
			"height",
			"fullScreenMode"
		})] get; }

		// Token: 0x0600D3D1 RID: 54225
		void SetResolution(int width, int height, FullScreenMode fullscreen);

		// Token: 0x17001A32 RID: 6706
		// (get) Token: 0x0600D3D2 RID: 54226
		string temporaryCachePath { get; }

		// Token: 0x0600D3D3 RID: 54227
		void RestartProcess(params string[] argv);

		// Token: 0x0600D3D4 RID: 54228 RVA: 0x004CB558 File Offset: 0x004C9758
		public static IPlatformApplication Create()
		{
			return new PlatformApplicationStandalone();
		}

		// Token: 0x0600D3D5 RID: 54229 RVA: 0x004CB55F File Offset: 0x004C975F
		public static string JoinAndEscapeArgv(params string[] args)
		{
			if (args != null)
			{
				return string.Join<string>(' ', args.Select(new Func<string, string>(IPlatformApplication.EscapeArg)));
			}
			return null;
		}

		// Token: 0x0600D3D6 RID: 54230 RVA: 0x004CB580 File Offset: 0x004C9780
		public static string EscapeArg(string arg)
		{
			if (arg.Length > 0 && arg.AsSpan().IndexOfAny(" \t\n\v\"") < 0)
			{
				return arg;
			}
			if (arg.Length <= 0)
			{
				return "\"\"";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('"');
			int num = 0;
			int num2;
			for (;;)
			{
				num2 = 0;
				while (num < arg.Length && arg[num] == '\\')
				{
					num++;
					num2++;
				}
				if (num >= arg.Length)
				{
					break;
				}
				if (arg[num] == '"')
				{
					stringBuilder.Append('\\', num2 * 2 + 1);
					stringBuilder.Append(arg[num]);
				}
				else
				{
					stringBuilder.Append('\\', num2);
					stringBuilder.Append(arg[num]);
				}
				num++;
			}
			stringBuilder.Append('\\', num2 * 2);
			stringBuilder.Append('"');
			return stringBuilder.ToString();
		}

		// Token: 0x0600D3D7 RID: 54231 RVA: 0x004CB660 File Offset: 0x004C9860
		RefreshRate GetCurrentRefreshRate()
		{
			return Screen.currentResolution.refreshRateRatio;
		}

		// Token: 0x0600D3D8 RID: 54232 RVA: 0x004CB67A File Offset: 0x004C987A
		Resolution GetCurrentResolution()
		{
			return Screen.currentResolution;
		}

		// Token: 0x17001A33 RID: 6707
		// (get) Token: 0x0600D3D9 RID: 54233 RVA: 0x004CB681 File Offset: 0x004C9881
		EnumGamePrefs VSyncCountPref
		{
			get
			{
				return EnumGamePrefs.OptionsGfxVsync;
			}
		}
	}
}
