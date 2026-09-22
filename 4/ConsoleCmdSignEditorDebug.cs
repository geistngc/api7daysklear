using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000296 RID: 662
[Preserve]
public class ConsoleCmdSignEditorDebug : ConsoleCmdAbstract
{
	// Token: 0x0600134D RID: 4941 RVA: 0x000764F2 File Offset: 0x000746F2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"signeditordebug",
			"sed"
		};
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x0007650A File Offset: 0x0007470A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggles visibility of the Sign Editor debug panel. ";
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x00076511 File Offset: 0x00074711
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "No params: Toggles visibility of the Sign Editor debug panel. ";
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06001350 RID: 4944 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06001351 RID: 4945 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x00076518 File Offset: 0x00074718
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		XUiC_SignEditorWindow.ShowDebugPanel = !XUiC_SignEditorWindow.ShowDebugPanel;
		Log.Out("[SignEditor] Set Sign Editor debug panel visibility to " + XUiC_SignEditorWindow.ShowDebugPanel.ToString() + ". Changes take effect the next time a layer is selected in the editor.");
	}
}
