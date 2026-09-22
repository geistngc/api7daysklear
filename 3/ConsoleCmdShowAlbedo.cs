using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200028D RID: 653
[Preserve]
public class ConsoleCmdShowAlbedo : ConsoleCmdAbstract
{
	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600131C RID: 4892 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600131D RID: 4893 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00075E8D File Offset: 0x0007408D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showalbedo",
			"albedo"
		};
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00075EA5 File Offset: 0x000740A5
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Polarizer.SetDebugView((Polarizer.GetDebugView() != Polarizer.ViewEnums.Albedo) ? Polarizer.ViewEnums.Albedo : Polarizer.ViewEnums.None);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("showalbedo " + ((Polarizer.GetDebugView() == Polarizer.ViewEnums.Albedo) ? "on" : "off"));
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00075EE0 File Offset: 0x000740E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables display of albedo in gBuffer";
	}
}
