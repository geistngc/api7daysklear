using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000215 RID: 533
[Preserve]
public class ConsoleCmdExportPrefab : ConsoleCmdAbstract
{
	// Token: 0x0600102E RID: 4142 RVA: 0x00067187 File Offset: 0x00065387
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			ConsoleCmdExportPrefab.CommandName
		};
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x0600102F RID: 4143 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00067197 File Offset: 0x00065397
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Exports a prefab from a world area";
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0006719E File Offset: 0x0006539E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  exportprefab <name> <x1> <y1> <z1> <x2> <y2> <z2> [part]\nExports a prefab with the given name from a box defined by the coordinate pair of two corners\nIf the optional parameter 'part' is 'true' it will export into the 'Parts' subfolder of the prefabs folder";
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x000671A8 File Offset: 0x000653A8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 7 || _params.Count > 8)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Illegal number of parameters");
			return;
		}
		bool flag = false;
		if (_params.Count == 8)
		{
			flag = ConsoleHelper.ParseParamBool(_params[7], true);
		}
		string text = _params[0];
		int x;
		if (!StringParsers.TryParseSInt32(_params[1], out x))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x1 coordinate is not a valid integer");
			return;
		}
		int y;
		if (!StringParsers.TryParseSInt32(_params[2], out y))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("y1 coordinate is not a valid integer");
			return;
		}
		int z;
		if (!StringParsers.TryParseSInt32(_params[3], out z))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z1 coordinate is not a valid integer");
			return;
		}
		Vector3i posStart = new Vector3i(x, y, z);
		if (!StringParsers.TryParseSInt32(_params[4], out x))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x2 coordinate is not a valid integer");
			return;
		}
		if (!StringParsers.TryParseSInt32(_params[5], out y))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("y2 coordinate is not a valid integer");
			return;
		}
		if (!StringParsers.TryParseSInt32(_params[6], out z))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z2 coordinate is not a valid integer");
			return;
		}
		Vector3i posEnd = new Vector3i(x, y, z);
		if (Prefab.PrefabExists(text))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("A prefab with the name \"" + text + "\" already exists.");
			return;
		}
		Prefab prefab = new Prefab();
		prefab.location = Prefab.LocationForNewPrefab(text, flag ? "Parts" : null);
		prefab.copyFromWorld(GameManager.Instance.World, posStart, posEnd);
		if (prefab.Save(prefab.location, true))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Prefab saved to " + prefab.location.ToString());
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Prefab could not be saved");
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00067370 File Offset: 0x00065570
	public static string BuildCommandString(string _prefabName, Vector3i _startPos, Vector3i _endPos, bool _asPart)
	{
		return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", new object[]
		{
			ConsoleCmdExportPrefab.CommandName,
			_prefabName,
			_startPos.x,
			_startPos.y,
			_startPos.z,
			_endPos.x,
			_endPos.y,
			_endPos.z,
			_asPart
		});
	}

	// Token: 0x04000CB0 RID: 3248
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string CommandName = "exportprefab";
}
