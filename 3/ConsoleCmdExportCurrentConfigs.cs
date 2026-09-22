using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x02000213 RID: 531
[Preserve]
public class ConsoleCmdExportCurrentConfigs : ConsoleCmdAbstract
{
	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06001023 RID: 4131 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06001024 RID: 4132 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06001025 RID: 4133 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x00066F56 File Offset: 0x00065156
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Exports the current game config XMLs";
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00066F5D File Offset: 0x0006515D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Exports all game config XMLs as they are currently used (including applied\npatches from mods) to the folder \"Configs\" in the save folder of the game.\nIf run from the main menu it exports the XUi configs for the menu, if run\nfrom a game session will export all others.";
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x00066F64 File Offset: 0x00065164
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"exportcurrentconfigs"
		};
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00066F74 File Offset: 0x00065174
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.World == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No game started, exporting XUi menu and rwgmixer configs");
			string text = Path.Combine(GameIO.GetApplicationTempPath(), "ExportedConfigs");
			if (SdDirectory.Exists(text))
			{
				SdDirectory.Delete(text, true);
			}
			Thread.Sleep(50);
			SdDirectory.CreateDirectory(text);
			string[] array = new string[]
			{
				"rwgmixer",
				"loadingscreen",
				"XUi_Menu/styles",
				"XUi_Menu/templates",
				"XUi_Menu/windows",
				"XUi_Menu/xui"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				XmlFile xml = null;
				ThreadManager.RunCoroutineSync(XmlPatcher.LoadAndPatchConfig(text2, delegate(XmlFile _file)
				{
					xml = _file;
				}));
				string path = text + "/" + text2 + ".xml";
				if (text2.IndexOf('/') >= 0)
				{
					string directoryName = Path.GetDirectoryName(path);
					if (!SdDirectory.Exists(directoryName))
					{
						SdDirectory.CreateDirectory(directoryName);
					}
				}
				xml.SerializeToFile(path, false, null);
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Configs exports to " + text);
			GameIO.OpenExplorer(text);
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			string @string = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			ConsoleCmdExportCurrentConfigs.DumpLoadedXmls(Path.Combine(GameIO.GetApplicationTempPath(), "SavesLocal", @string, "ConfigsDump"));
			return;
		}
		string text3 = Path.Combine(GameIO.GetSaveGameDir(), "ConfigsDump");
		if (!GameIO.IsRoamingUserDataPath(text3))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Patched XMLs are automatically dumped on game start to a ConfigsDump subdirectory of the save game.");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("In this case you can find the folder at: " + text3);
			GameIO.OpenExplorer(text3);
			return;
		}
		string string2 = GamePrefs.GetString(EnumGamePrefs.GameWorld);
		string string3 = GamePrefs.GetString(EnumGamePrefs.GameName);
		ConsoleCmdExportCurrentConfigs.DumpLoadedXmls(Path.Combine(GameIO.GetApplicationTempPath(), string2, string3, "ConfigsDump"));
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00067145 File Offset: 0x00065345
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DumpLoadedXmls(string tempPath)
	{
		if (SdDirectory.Exists(tempPath))
		{
			SdDirectory.Delete(tempPath, true);
		}
		SdDirectory.CreateDirectory(tempPath);
		WorldStaticData.SaveXmlsToFolder(tempPath);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Configs exports to " + tempPath);
		GameIO.OpenExplorer(tempPath);
	}
}
