using System;
using System.Collections.Generic;
using Platform;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x020002B6 RID: 694
[Preserve]
public class ConsoleCmdTwitchAdminCommand : ConsoleCmdAbstract
{
	// Token: 0x06001400 RID: 5120 RVA: 0x00079E9A File Offset: 0x0007809A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"twitchadmin"
		};
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06001401 RID: 5121 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06001402 RID: 5122 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06001403 RID: 5123 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x00079EAC File Offset: 0x000780AC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.getHelp());
			return;
		}
		string a = _params[0];
		if (!(a == "cleanup"))
		{
			if (!(a == "export"))
			{
				if (!(a == "import"))
				{
					if (a == "pointstotal")
					{
						goto IL_190;
					}
					if (!(a == "reset"))
					{
						return;
					}
					if (_params.Count != 3 || !(_params[1] == "all"))
					{
						return;
					}
					string a2 = _params[2];
					if (a2 == "both")
					{
						TwitchManager.Current.ViewerData.ResetAllPoints();
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch PP and SP");
						return;
					}
					if (a2 == "sp")
					{
						TwitchManager.Current.ViewerData.ResetAllSpecialPoints();
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch SP");
						return;
					}
					if (!(a2 == "pp"))
					{
						return;
					}
					TwitchManager.Current.ViewerData.ResetAllStandardPoints();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch PP");
					return;
				}
			}
			else
			{
				if (!DeviceCapabilities.CanUserAccessFilesystem())
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Viewer data import/export not supported on this device");
					return;
				}
				if (_params.Count != 2)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Export failed: file path required");
					return;
				}
				try
				{
					string filePath = _params[1];
					TwitchManager.Current.SaveExportViewerData(filePath);
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Viewer data exported");
					return;
				}
				catch (Exception ex)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Export failed. " + ex.GetType().Name + ": " + ex.Message);
					return;
				}
			}
			if (!DeviceCapabilities.CanUserAccessFilesystem())
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Viewer data import/export not supported on this device");
				return;
			}
			if (_params.Count != 2)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Import failed: file path required");
				return;
			}
			try
			{
				string filePath2 = _params[1];
				TwitchManager.Current.LoadExportViewerData(filePath2);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Viewer data imported");
				return;
			}
			catch (Exception ex2)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Import failed. " + ex2.GetType().Name + ": " + ex2.Message);
				return;
			}
			IL_190:
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Twitch Viewer Data Point Totals:\n" + TwitchManager.Current.ViewerData.GetPointTotals());
			return;
		}
		TwitchManager.Current.ViewerData.Cleanup();
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0007A13C File Offset: 0x0007833C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Twitch Admin Commands";
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0007A143 File Offset: 0x00078343
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. twitchadmin cleanup\n  2. twitchadmin export <filepath>\n  3. twitchadmin import <filepath>\n  4. twitchadmin pointtotals\n  5. twitchadmin reset all both\n  6. twitchadmin reset all pp\n  7. twitchadmin reset all sp\n1. Cleans up duplicate Viewer Data.\n2. Export users, points, and credits to a file\n3. Import users, points, and credits from a file, replacing existing data\n4. Prints out the totals for PP, SP, and BitCredit\n5. Clears all PP and SP for all users\n6. Clears all PP for all users\n7. Clears all SP for all users";
	}
}
