using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000263 RID: 611
[Preserve]
public class ConsoleCmdPois : ConsoleCmdAbstract
{
	// Token: 0x06001225 RID: 4645 RVA: 0x00071A4C File Offset: 0x0006FC4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"pois"
		};
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00071A5C File Offset: 0x0006FC5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Switches distant POIs on/off";
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x00071A63 File Offset: 0x0006FC63
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Use on or off or only the command to toggle";
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x00071A6C File Offset: 0x0006FC6C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameObject x = GameObject.Find("/PrefabsLOD");
		if (x != null)
		{
			ConsoleCmdPois.parentGO = x;
		}
		if (ConsoleCmdPois.parentGO == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Distant POIs not active!");
			return;
		}
		if (_params.Count == 0)
		{
			ConsoleCmdPois.parentGO.SetActive(!ConsoleCmdPois.parentGO.activeSelf);
		}
		else if (_params[0] == "on")
		{
			ConsoleCmdPois.parentGO.SetActive(true);
		}
		else if (_params[0] == "off")
		{
			ConsoleCmdPois.parentGO.SetActive(true);
		}
		else
		{
			int num;
			if (int.TryParse(_params[0], out num))
			{
				GameManager.Instance.prefabLODManager.SetPOIDistance(128 * num);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					"Setting to POI chunk distance ",
					num.ToString(),
					" =",
					(128 * num).ToString(),
					"m"
				}));
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown parameter");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("POIs set to " + (ConsoleCmdPois.parentGO.activeSelf ? "on" : "off"));
	}

	// Token: 0x04000D22 RID: 3362
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject parentGO;
}
