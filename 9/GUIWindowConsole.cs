using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200121D RID: 4637
public class GUIWindowConsole : GUIWindowUGUI
{
	// Token: 0x17001199 RID: 4505
	// (get) Token: 0x06009409 RID: 37897 RVA: 0x0037FFCF File Offset: 0x0037E1CF
	public override string UIPrefabPath
	{
		get
		{
			return "GUI/Prefabs/ConsoleWindow";
		}
	}

	// Token: 0x0600940A RID: 37898 RVA: 0x0037FFD8 File Offset: 0x0037E1D8
	public GUIWindowConsole() : base("GUIWindowConsole")
	{
		Log.LogCallbacks += this.LogCallback;
		this.alwaysUsesMouseCursor = true;
		this.components = this.canvas.GetComponent<GUIWindowConsoleComponents>();
		this.scrollRect = this.components.scrollRect;
		this.contentRect = this.components.contentRect;
		this.commandField = this.components.commandField;
		this.commandField.onSubmit.AddListener(new UnityAction<string>(this.EnterCommand));
		this.commandField.shouldActivateOnSelect = !TouchScreenKeyboard.isSupported;
		this.components.closeButton.onClick.AddListener(new UnityAction(this.CloseConsole));
		this.components.openLogsButton.onClick.AddListener(delegate()
		{
			GameIO.OpenExplorer(Application.consoleLogPath);
		});
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.components.openLogsButton.gameObject.SetActive(false);
		}
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.components.consoleLinePrefab);
			gameObject.SetActive(false);
			gameObject.transform.SetParent(this.contentRect, false);
			this.linePool.Push(gameObject.GetComponent<Text>());
		}
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.Input_OnLastInputStyleChanged;
	}

	// Token: 0x0600940B RID: 37899 RVA: 0x00380189 File Offset: 0x0037E389
	[PublicizedFrom(EAccessModifier.Private)]
	public void Input_OnLastInputStyleChanged(PlayerInputManager.InputStyle _inputStyle)
	{
		if (_inputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.components.controllerPrompts.SetActive(false);
			return;
		}
		this.components.controllerPrompts.SetActive(true);
		this.components.RefreshButtonPrompts();
	}

	// Token: 0x0600940C RID: 37900 RVA: 0x003801C0 File Offset: 0x0037E3C0
	[PublicizedFrom(EAccessModifier.Private)]
	public Text AllocText()
	{
		Text text;
		if (this.linePool.TryPop(out text))
		{
			text.gameObject.SetActive(true);
			return text;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.components.consoleLinePrefab);
		gameObject.transform.SetParent(this.contentRect, false);
		return gameObject.GetComponent<Text>();
	}

	// Token: 0x0600940D RID: 37901 RVA: 0x00380211 File Offset: 0x0037E411
	[PublicizedFrom(EAccessModifier.Private)]
	public void FreeText(Text _text)
	{
		_text.gameObject.SetActive(false);
		this.linePool.Push(_text);
	}

	// Token: 0x0600940E RID: 37902 RVA: 0x0038022C File Offset: 0x0037E42C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddDisplayedLine(GUIWindowConsole.ConsoleLine _line)
	{
		foreach (StringSpan stringSpan in _line.text.GetSplitAnyEnumerator(GUIWindowConsole.lineSeparators, StringSplitOptions.RemoveEmptyEntries))
		{
			Text text;
			if (this.displayedLines.Count == 300)
			{
				text = this.displayedLines.Dequeue();
			}
			else
			{
				text = this.AllocText();
			}
			if (stringSpan.Length > 500)
			{
				text.text = SpanUtils.Concat(stringSpan.Slice(0, 500), "...");
			}
			else
			{
				text.text = stringSpan.ToString();
			}
			text.color = _line.GetLogColor();
			text.transform.SetAsLastSibling();
			this.displayedLines.Enqueue(text);
		}
	}

	// Token: 0x0600940F RID: 37903 RVA: 0x0038030C File Offset: 0x0037E50C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearDisplayedLines()
	{
		Text text;
		while (this.displayedLines.TryDequeue(out text))
		{
			this.FreeText(text);
		}
	}

	// Token: 0x06009410 RID: 37904 RVA: 0x00380331 File Offset: 0x0037E531
	[PublicizedFrom(EAccessModifier.Private)]
	public void shutdown()
	{
		Log.LogCallbacks -= this.LogCallback;
	}

	// Token: 0x06009411 RID: 37905 RVA: 0x00380344 File Offset: 0x0037E544
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogCallback(string _msg, string _trace, LogType _type)
	{
		switch (_type)
		{
		case LogType.Assert:
			this.openConsole(_msg);
			break;
		case LogType.Exception:
			this.openConsole(_msg);
			break;
		}
		this.internalAddLine(new GUIWindowConsole.ConsoleLine(_msg, _trace, _type));
	}

	// Token: 0x06009412 RID: 37906 RVA: 0x00380380 File Offset: 0x0037E580
	[PublicizedFrom(EAccessModifier.Private)]
	public void openConsole(string _logString)
	{
		if (Submission.Enabled)
		{
			return;
		}
		if (_logString.StartsWith("Can't send RPC"))
		{
			return;
		}
		if (_logString.StartsWith("You are trying to load data from"))
		{
			return;
		}
		this.windowManager.Open(this, false);
	}

	// Token: 0x06009413 RID: 37907 RVA: 0x003803B4 File Offset: 0x0037E5B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void addLines(List<string> _lines)
	{
		for (int i = 0; i < _lines.Count; i++)
		{
			this.addLine(_lines[i]);
		}
	}

	// Token: 0x06009414 RID: 37908 RVA: 0x003803DF File Offset: 0x0037E5DF
	[PublicizedFrom(EAccessModifier.Private)]
	public void addLine(string _line)
	{
		this.internalAddLine(new GUIWindowConsole.ConsoleLine(_line, string.Empty, LogType.Log));
	}

	// Token: 0x06009415 RID: 37909 RVA: 0x003803F4 File Offset: 0x0037E5F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void internalAddLine(GUIWindowConsole.ConsoleLine consoleLine)
	{
		Queue<GUIWindowConsole.ConsoleLine> obj = this.linesToAdd;
		lock (obj)
		{
			this.linesToAdd.Enqueue(consoleLine);
			while (this.linesToAdd.Count > 300)
			{
				this.linesToAdd.Dequeue();
			}
		}
	}

	// Token: 0x06009416 RID: 37910 RVA: 0x0038045C File Offset: 0x0037E65C
	public override void Update()
	{
		base.Update();
		this.scrolledToBottom = (this.scrollRect.verticalNormalizedPosition < 0.1f);
		bool flag = false;
		Queue<GUIWindowConsole.ConsoleLine> obj = this.linesToAdd;
		lock (obj)
		{
			if (this.linesToAdd.Count > 0)
			{
				flag = true;
				foreach (GUIWindowConsole.ConsoleLine line in this.linesToAdd)
				{
					this.AddDisplayedLine(line);
				}
				this.linesToAdd.Clear();
			}
		}
		if (flag && this.scrolledToBottom)
		{
			Canvas.ForceUpdateCanvases();
			this.scrollRect.verticalNormalizedPosition = 0f;
		}
		if (this.bFirstTime)
		{
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
			this.scrollRect.verticalNormalizedPosition = 0f;
			this.bFirstTime = false;
		}
		if (this.bUpdateCursor)
		{
			this.commandField.MoveTextEnd(false);
			this.bUpdateCursor = false;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.PreviousCommand();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.NextCommand();
		}
		else if (Input.GetKeyDown(KeyCode.PageUp))
		{
			float num = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Min(this.scrollRect.verticalNormalizedPosition + num, 1f);
		}
		else if (Input.GetKeyDown(KeyCode.PageDown))
		{
			float num2 = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Max(this.scrollRect.verticalNormalizedPosition - num2, 0f);
		}
		PlayerActionsLocal playerInput = this.playerUI.playerInput;
		PlayerActionsGUI playerActionsGUI = (playerInput != null) ? playerInput.GUIActions : null;
		if (playerActionsGUI == null)
		{
			return;
		}
		if (playerActionsGUI.Submit.WasReleased)
		{
			this.EnterCommand(this.commandField.text);
		}
		else if (playerActionsGUI.DPad_Up.WasReleased && playerActionsGUI.DPad_Up.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			this.PreviousCommand();
		}
		else if (playerActionsGUI.DPad_Down.WasReleased && playerActionsGUI.DPad_Down.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			this.NextCommand();
		}
		else if (playerActionsGUI.DPad_Left.WasReleased && playerActionsGUI.DPad_Down.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			IVirtualKeyboard virtualKeyboard = PlatformManager.NativePlatform.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Open("Enter Command", this.commandField.text, new Action<bool, string>(this.OnTextReceived), UIInput.InputType.Standard, false, 200U);
			}
		}
		else if (this.playerUI.playerInput.PermanentActions.Cancel.WasReleased || PlayerActionsGlobal.Instance.Console.WasPressed)
		{
			ThreadManager.StartCoroutine(this.closeLater());
		}
		float y = playerActionsGUI.Camera.Vector.y;
		if (y != 0f)
		{
			float num3 = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Max(this.scrollRect.verticalNormalizedPosition + num3 * y * 0.05f, 0f);
		}
	}

	// Token: 0x06009417 RID: 37911 RVA: 0x00380790 File Offset: 0x0037E990
	[PublicizedFrom(EAccessModifier.Private)]
	public float CalculateNormalizedPageSize()
	{
		float height = this.scrollRect.viewport.rect.height;
		float num = this.scrollRect.content.rect.height - height;
		if (num > height)
		{
			return Math.Max(height / num, 0.01f);
		}
		return 1f;
	}

	// Token: 0x06009418 RID: 37912 RVA: 0x003807E8 File Offset: 0x0037E9E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTextReceived(bool _success, string _text)
	{
		if (_success)
		{
			this.commandField.text = _text;
		}
	}

	// Token: 0x06009419 RID: 37913 RVA: 0x003807F9 File Offset: 0x0037E9F9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator closeLater()
	{
		yield return null;
		this.CloseConsole();
		yield break;
	}

	// Token: 0x0600941A RID: 37914 RVA: 0x00380808 File Offset: 0x0037EA08
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseConsole()
	{
		this.windowManager.Close(this, false);
		this.commandField.text = string.Empty;
	}

	// Token: 0x0600941B RID: 37915 RVA: 0x00380828 File Offset: 0x0037EA28
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnterCommand(string _command)
	{
		if (_command.Length > 0)
		{
			if (_command == "clear")
			{
				this.Clear();
			}
			else
			{
				this.scrollRect.verticalNormalizedPosition = 0f;
				this.internalAddLine(new GUIWindowConsole.ConsoleLine("> " + _command, string.Empty, LogType.Log));
				if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
				{
					GUIWindowConsole.AddLines(SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(_command, null));
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageConsoleCmdServer>().Setup(_command), false);
				}
			}
			if (this.lastCommands.Count == 0 || !this.lastCommands[this.lastCommands.Count - 1].Equals(_command))
			{
				if (this.lastCommands.Contains(_command))
				{
					this.lastCommands.Remove(_command);
				}
				this.lastCommands.Add(_command);
			}
			this.lastCommandsIdx = this.lastCommands.Count;
			this.commandField.text = "";
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
		}
	}

	// Token: 0x0600941C RID: 37916 RVA: 0x0038094C File Offset: 0x0037EB4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviousCommand()
	{
		if (this.lastCommands.Count > 0)
		{
			this.lastCommandsIdx = Mathf.Max(0, this.lastCommandsIdx - 1);
			this.commandField.text = this.lastCommands[this.lastCommandsIdx];
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
			this.bUpdateCursor = true;
		}
	}

	// Token: 0x0600941D RID: 37917 RVA: 0x003809BC File Offset: 0x0037EBBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void NextCommand()
	{
		if (this.lastCommands.Count > 0)
		{
			this.lastCommandsIdx = Mathf.Min(this.lastCommands.Count, this.lastCommandsIdx + 1);
			if (this.lastCommandsIdx < this.lastCommands.Count)
			{
				this.commandField.text = this.lastCommands[this.lastCommandsIdx];
				this.bUpdateCursor = true;
				if (!TouchScreenKeyboard.isSupported)
				{
					this.commandField.Select();
					this.commandField.ActivateInputField();
					return;
				}
			}
			else
			{
				this.commandField.text = string.Empty;
			}
		}
	}

	// Token: 0x0600941E RID: 37918 RVA: 0x00380A5C File Offset: 0x0037EC5C
	public void Clear()
	{
		this.ClearDisplayedLines();
	}

	// Token: 0x0600941F RID: 37919 RVA: 0x00380A64 File Offset: 0x0037EC64
	public override void OnOpen()
	{
		base.OnOpen();
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && this.windowManager.IsWindowOpen(XUiC_InGameDebugMenu.ID))
		{
			this.bShouldReopenGebugMenu = true;
			this.windowManager.Close(XUiC_InGameDebugMenu.ID);
		}
		else
		{
			this.bShouldReopenGebugMenu = false;
		}
		this.commandField.text = string.Empty;
		this.bFirstTime = true;
		this.isInputActive = true;
		if (UIInput.selection != null)
		{
			UIInput.selection.isSelected = false;
		}
	}

	// Token: 0x06009420 RID: 37920 RVA: 0x00380AE8 File Offset: 0x0037ECE8
	public override void OnClose()
	{
		this.scrollRect.verticalNormalizedPosition = 0f;
		base.OnClose();
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && this.bShouldReopenGebugMenu)
		{
			this.windowManager.Open(XUiC_InGameDebugMenu.ID, false);
		}
		this.bShouldReopenGebugMenu = false;
		this.isInputActive = false;
	}

	// Token: 0x06009421 RID: 37921 RVA: 0x00380B3B File Offset: 0x0037ED3B
	public static GUIWindowConsole GetNewInstance()
	{
		GUIWindowConsole result;
		if ((result = GUIWindowConsole.instance) == null)
		{
			result = (GUIWindowConsole.instance = new GUIWindowConsole());
		}
		return result;
	}

	// Token: 0x06009422 RID: 37922 RVA: 0x00380B51 File Offset: 0x0037ED51
	public static void Open()
	{
		LocalPlayerUI.primaryUI.windowManager.Open(GUIWindowConsole.instance, false);
	}

	// Token: 0x06009423 RID: 37923 RVA: 0x00380B68 File Offset: 0x0037ED68
	public static void Close()
	{
		LocalPlayerUI.primaryUI.windowManager.Close(GUIWindowConsole.instance, false);
	}

	// Token: 0x06009424 RID: 37924 RVA: 0x00380B7F File Offset: 0x0037ED7F
	public static bool IsOpen()
	{
		return LocalPlayerUI.primaryUI.windowManager.IsWindowOpen(GUIWindowConsole.instance);
	}

	// Token: 0x06009425 RID: 37925 RVA: 0x00380B95 File Offset: 0x0037ED95
	public static void AddLines(List<string> _lines)
	{
		GUIWindowConsole guiwindowConsole = GUIWindowConsole.instance;
		if (guiwindowConsole == null)
		{
			return;
		}
		guiwindowConsole.addLines(_lines);
	}

	// Token: 0x06009426 RID: 37926 RVA: 0x00380BA7 File Offset: 0x0037EDA7
	public static void Shutdown()
	{
		GUIWindowConsole guiwindowConsole = GUIWindowConsole.instance;
		if (guiwindowConsole != null)
		{
			guiwindowConsole.shutdown();
		}
		GUIWindowConsole.instance = null;
	}

	// Token: 0x04006EF3 RID: 28403
	[PublicizedFrom(EAccessModifier.Private)]
	public static GUIWindowConsole instance;

	// Token: 0x04006EF4 RID: 28404
	[PublicizedFrom(EAccessModifier.Private)]
	public bool scrolledToBottom = true;

	// Token: 0x04006EF5 RID: 28405
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<GUIWindowConsole.ConsoleLine> linesToAdd = new Queue<GUIWindowConsole.ConsoleLine>(301);

	// Token: 0x04006EF6 RID: 28406
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<Text> displayedLines = new Queue<Text>();

	// Token: 0x04006EF7 RID: 28407
	[PublicizedFrom(EAccessModifier.Private)]
	public const int maxConsoleLines = 300;

	// Token: 0x04006EF8 RID: 28408
	[PublicizedFrom(EAccessModifier.Private)]
	public Stack<Text> linePool = new Stack<Text>();

	// Token: 0x04006EF9 RID: 28409
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> lastCommands = new List<string>();

	// Token: 0x04006EFA RID: 28410
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastCommandsIdx;

	// Token: 0x04006EFB RID: 28411
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFirstTime;

	// Token: 0x04006EFC RID: 28412
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUpdateCursor;

	// Token: 0x04006EFD RID: 28413
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShouldReopenGebugMenu;

	// Token: 0x04006EFE RID: 28414
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIWindowConsoleComponents components;

	// Token: 0x04006EFF RID: 28415
	[PublicizedFrom(EAccessModifier.Private)]
	public ScrollRect scrollRect;

	// Token: 0x04006F00 RID: 28416
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform contentRect;

	// Token: 0x04006F01 RID: 28417
	[PublicizedFrom(EAccessModifier.Private)]
	public InputField commandField;

	// Token: 0x04006F02 RID: 28418
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] lineSeparators = new string[]
	{
		"\r\n",
		"\r",
		"\n"
	};

	// Token: 0x0200121E RID: 4638
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ConsoleLine
	{
		// Token: 0x06009428 RID: 37928 RVA: 0x00380BE4 File Offset: 0x0037EDE4
		public ConsoleLine(string _text, string _stackTrace, LogType _type)
		{
			this.text = _text;
			this.stackTrace = _stackTrace;
			this.type = _type;
		}

		// Token: 0x06009429 RID: 37929 RVA: 0x00380BFC File Offset: 0x0037EDFC
		public Color GetLogColor()
		{
			switch (this.type)
			{
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				return Color.red;
			case LogType.Warning:
				return Color.yellow;
			case LogType.Log:
				return Color.white;
			default:
				return Color.white;
			}
		}

		// Token: 0x04006F03 RID: 28419
		public string text;

		// Token: 0x04006F04 RID: 28420
		public LogType type;

		// Token: 0x04006F05 RID: 28421
		public string stackTrace;
	}
}
