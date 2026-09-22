using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001F0 RID: 496
[Preserve]
public class ConsoleCmdCamera : ConsoleCmdAbstract
{
	// Token: 0x06000F4A RID: 3914 RVA: 0x000639AC File Offset: 0x00061BAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"camera",
			"cam"
		};
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x000639C4 File Offset: 0x00061BC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Lock/unlock camera movement or load/save a specific camera position";
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x000639CB File Offset: 0x00061BCB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n   1. cam save <name> [comment]\n   2. cam load <name>\n   3. cam list\n   4. cam lock\n   5. cam unlock\n1. Save the current player's position and camera view or the camera position\nand view if in detached mode under the given name. Optionally a more descriptive\ncomment can be supplied.\n2. Load the position and direction with the given name. If in detached camera\nmode the camera itself will be adjusted, otherwise the player will be teleported.\n3. List the saved camera positions.\n4/5. Lock/unlock the camera rotation. Can also be achieved with the \"Lock Camera\" key.";
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x000639D4 File Offset: 0x00061BD4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No sub command given.");
			return;
		}
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on clients");
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (_params[0].EqualsCaseInsensitive("lock"))
		{
			this.ExecuteLock(_params, primaryPlayer);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("unlock"))
		{
			this.ExecuteUnlock(_params, primaryPlayer);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("save"))
		{
			this.ExecuteSave(_params, primaryPlayer);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("load"))
		{
			this.ExecuteLoad(_params, primaryPlayer);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("list"))
		{
			this.ExecuteList(_params);
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid sub command \"" + _params[0] + "\".");
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x00063AD0 File Offset: 0x00061CD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteLock(List<string> _params, EntityPlayerLocal _epl)
	{
		_epl.movementInput.bCameraPositionLocked = true;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x00063ADE File Offset: 0x00061CDE
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteUnlock(List<string> _params, EntityPlayerLocal _epl)
	{
		_epl.movementInput.bCameraPositionLocked = false;
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x00063AEC File Offset: 0x00061CEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteSave(List<string> _params, EntityPlayerLocal _epl)
	{
		if (_params.Count < 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command requires a name for the position.");
			return;
		}
		string text = _params[1];
		string comment = (_params.Count > 2) ? _params[2] : null;
		CameraPerspectives cameraPerspectives = new CameraPerspectives(true);
		cameraPerspectives.Perspectives[text] = new CameraPerspectives.Perspective(text, _epl, comment);
		cameraPerspectives.Save();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Position saved with name \"" + text + "\"");
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x00063B68 File Offset: 0x00061D68
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteLoad(List<string> _params, EntityPlayerLocal _epl)
	{
		if (_params.Count < 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No position name given.");
			return;
		}
		CameraPerspectives.Perspective perspective;
		if (!new CameraPerspectives(true).Perspectives.TryGetValue(_params[1], out perspective))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Position name not found.");
			return;
		}
		perspective.ToPlayer(_epl);
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x00063BC0 File Offset: 0x00061DC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteList(List<string> _params)
	{
		CameraPerspectives cameraPerspectives = new CameraPerspectives(true);
		string text = (_params.Count > 1) ? _params[1] : null;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved camera positions:");
		foreach (KeyValuePair<string, CameraPerspectives.Perspective> keyValuePair in cameraPerspectives.Perspectives)
		{
			string text2;
			CameraPerspectives.Perspective perspective;
			keyValuePair.Deconstruct(out text2, out perspective);
			CameraPerspectives.Perspective perspective2 = perspective;
			if (text == null || perspective2.Name.ContainsCaseInsensitive(text) || perspective2.Comment.ContainsCaseInsensitive(text))
			{
				string str = string.IsNullOrEmpty(perspective2.Comment) ? "" : (" (" + perspective2.Comment + ")");
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  " + perspective2.Name + str);
			}
		}
	}
}
