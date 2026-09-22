using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000219 RID: 537
[Preserve]
public class ConsoleCmdFov : ConsoleCmdAbstract
{
	// Token: 0x0600104D RID: 4173 RVA: 0x00067708 File Offset: 0x00065908
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"fov"
		};
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x0600104E RID: 4174 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x00067718 File Offset: 0x00065918
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Camera field of view";
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x00067720 File Offset: 0x00065920
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int value;
		if (_params.Count == 1 && int.TryParse(_params[0], out value))
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, value);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Set FOV to " + value.ToString());
		}
	}
}
