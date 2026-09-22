using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000292 RID: 658
[Preserve]
public class ConsoleCmdShowSpecular : ConsoleCmdAbstract
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x0600133A RID: 4922 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00076355 File Offset: 0x00074555
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showspecular",
			"spec"
		};
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x0007636D File Offset: 0x0007456D
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Polarizer.SetDebugView((Polarizer.GetDebugView() != Polarizer.ViewEnums.Specular) ? Polarizer.ViewEnums.Specular : Polarizer.ViewEnums.None);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("showspecular " + ((Polarizer.GetDebugView() == Polarizer.ViewEnums.Specular) ? "on" : "off"));
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x000763A8 File Offset: 0x000745A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables display of specular values in gBuffer";
	}
}
