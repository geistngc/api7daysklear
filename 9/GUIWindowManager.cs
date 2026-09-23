using System;
using System.Collections.Generic;
using InControl;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02001222 RID: 4642
public class GUIWindowManager : MonoBehaviour
{
	// Token: 0x06009436 RID: 37942 RVA: 0x00380D20 File Offset: 0x0037EF20
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.nguiWindowManager = base.GetComponent<NGUIWindowManager>();
		this.playerUI = base.GetComponent<LocalPlayerUI>();
	}

	// Token: 0x06009437 RID: 37943 RVA: 0x00380D3C File Offset: 0x0037EF3C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		for (int i = 0; i < this.windowsToOpen.Count; i++)
		{
			GUIWindow guiwindow = this.windowsToOpen[i];
			guiwindow.isShowing = true;
			this.openWindows.Add(guiwindow);
			this.topmostWindow = guiwindow;
		}
		this.windowsToOpen.Clear();
		this.modalWindow = null;
		for (int j = 0; j < this.openWindows.Count; j++)
		{
			GUIWindow guiwindow2 = this.openWindows[j];
			if (guiwindow2.isModal)
			{
				this.modalWindow = guiwindow2;
				break;
			}
		}
		this.cursorWindowOpen = false;
		for (int k = 0; k < this.openWindows.Count; k++)
		{
			GUIWindow guiwindow3 = this.openWindows[k];
			if (guiwindow3.isShowing)
			{
				this.cursorWindowOpen |= guiwindow3.alwaysUsesMouseCursor;
				GUI.matrix = Matrix4x4.identity;
				guiwindow3.OnGUI();
			}
		}
		GUI.enabled = true;
		List<GUIWindow> list = this.windowsToRemove;
		list.Clear();
		for (int l = 0; l < this.openWindows.Count; l++)
		{
			GUIWindow guiwindow4 = this.openWindows[l];
			if (!guiwindow4.isShowing)
			{
				list.Add(guiwindow4);
				this.topmostWindow = ((this.openWindows.Count > 0) ? this.openWindows[this.openWindows.Count - 1] : null);
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			GUIWindow item = list[m];
			this.openWindows.Remove(item);
		}
	}

	// Token: 0x06009438 RID: 37944 RVA: 0x00380ED8 File Offset: 0x0037F0D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		for (int i = 0; i < this.openWindows.Count; i++)
		{
			this.openWindows[i].Update();
		}
		if (this.lastActionClicked.Count != 0)
		{
			for (int j = 0; j < this.lastActionClicked.Count; j++)
			{
				NGuiAction nguiAction = this.lastActionClicked[j];
				PlayerAction hotkey = nguiAction.GetHotkey();
				if (hotkey != null && hotkey.WasReleased)
				{
					nguiAction.OnRelease();
					this.actionsToClear.Add(nguiAction);
				}
			}
			for (int k = 0; k < this.actionsToClear.Count; k++)
			{
				NGuiAction item = this.actionsToClear[k];
				this.lastActionClicked.Remove(item);
			}
			this.actionsToClear.Clear();
		}
		if (this.IsInputActive())
		{
			if (this.playerUI.playerInput != null && this.playerUI.playerInput.PermanentActions.Cancel.WasPressed && UIInput.selection != null)
			{
				UIInput.selection.RemoveFocus();
			}
			return;
		}
		if (!this.IsInputLocked)
		{
			List<NGuiAction> list = this.actionsForGlobalHotkeys;
			list.Clear();
			for (int l = 0; l < this.globalActions.Count; l++)
			{
				PlayerAction hotkey2 = this.globalActions[l].GetHotkey();
				if (hotkey2 != null && (((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnRelease) == NGuiAction.EnumKeyMode.FireOnRelease && hotkey2.WasReleased) || ((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnPress) == NGuiAction.EnumKeyMode.FireOnPress && hotkey2.WasPressed) || ((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnRepeat) == NGuiAction.EnumKeyMode.FireOnRepeat && hotkey2.WasRepeated)))
				{
					list.Add(this.globalActions[l]);
				}
			}
			if (list.Count > 0)
			{
				for (int m = 0; m < list.Count; m++)
				{
					NGuiAction nguiAction2 = list[m];
					nguiAction2.OnClick();
					this.lastActionClicked.Add(nguiAction2);
				}
				list.Clear();
			}
			if (this.playerUI.playerInput != null && this.playerUI.playerInput.PermanentActions.Cancel.WasPressed && !this.IsWindowOpen("popupGroup") && this.modalWindow != null && this.modalWindow.isEscClosable)
			{
				this.CloseAllOpenModalWindows(null, true);
			}
		}
	}

	// Token: 0x06009439 RID: 37945 RVA: 0x00381150 File Offset: 0x0037F350
	public bool IsInputActive()
	{
		return (UIInput.selection != null && UIInput.selection.gameObject.activeInHierarchy) || (this.topmostWindow != null && this.topmostWindow.isInputActive) || this.IsUGUIInputActive() || GUIWindowConsole.IsOpen();
	}

	// Token: 0x0600943A RID: 37946 RVA: 0x003811A0 File Offset: 0x0037F3A0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsUGUIInputActive()
	{
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return false;
		}
		GameObject currentSelectedGameObject = current.currentSelectedGameObject;
		if (currentSelectedGameObject == null)
		{
			return false;
		}
		InputField inputField;
		if (currentSelectedGameObject.TryGetComponent<InputField>(out inputField))
		{
			return inputField.isFocused;
		}
		TMP_InputField tmp_InputField;
		return currentSelectedGameObject.TryGetComponent<TMP_InputField>(out tmp_InputField) && tmp_InputField.isFocused;
	}

	// Token: 0x0600943B RID: 37947 RVA: 0x003811F3 File Offset: 0x0037F3F3
	public bool IsKeyShortcutsAllowed()
	{
		return !this.IsInputLocked && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || !this.IsInputActive());
	}

	// Token: 0x0600943C RID: 37948 RVA: 0x0038121C File Offset: 0x0037F41C
	public void Add(GUIWindow _window)
	{
		this.nameToWindowMap.Add(_window.Id, _window);
		_window.windowManager = this;
		if (this.nguiWindowManager == null)
		{
			this.nguiWindowManager = base.GetComponent<NGUIWindowManager>();
		}
		if (this.playerUI == null)
		{
			this.playerUI = base.GetComponent<LocalPlayerUI>();
		}
		_window.playerUI = this.playerUI;
	}

	// Token: 0x0600943D RID: 37949 RVA: 0x00381282 File Offset: 0x0037F482
	public void Remove(GUIWindow _w)
	{
		if (_w.isShowing)
		{
			this.Close(_w, false);
		}
		_w.Cleanup();
		this.nameToWindowMap.Remove(_w.Id);
	}

	// Token: 0x0600943E RID: 37950 RVA: 0x003812AC File Offset: 0x0037F4AC
	public GUIWindow GetWindow(string _windowName)
	{
		GUIWindow result;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out result))
		{
			Log.Warning("GUIWindowManager.GetWindow: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			return null;
		}
		return result;
	}

	// Token: 0x0600943F RID: 37951 RVA: 0x003812E0 File Offset: 0x0037F4E0
	public bool TryGetWindow(string _windowName, out GUIWindow _window)
	{
		return this.nameToWindowMap.TryGetValue(_windowName, out _window);
	}

	// Token: 0x06009440 RID: 37952 RVA: 0x003812EF File Offset: 0x0037F4EF
	public T GetWindow<T>(string _windowName) where T : GUIWindow
	{
		return (T)((object)this.nameToWindowMap[_windowName]);
	}

	// Token: 0x06009441 RID: 37953 RVA: 0x00381304 File Offset: 0x0037F504
	public void SwitchVisible(string _windowName, bool _modal = true)
	{
		GUIWindow guiWindow;
		if (this.nameToWindowMap.TryGetValue(_windowName, out guiWindow))
		{
			this.SwitchVisible(guiWindow, _modal);
		}
	}

	// Token: 0x06009442 RID: 37954 RVA: 0x00381329 File Offset: 0x0037F529
	public void SwitchVisible(GUIWindow _guiWindow, bool _modal = true)
	{
		if ((!_modal || _guiWindow.isModal) && _guiWindow.isShowing)
		{
			this.Close(_guiWindow, false);
			return;
		}
		this.Open(_guiWindow, _modal);
	}

	// Token: 0x06009443 RID: 37955 RVA: 0x00381350 File Offset: 0x0037F550
	public bool CloseAllOpenModalWindows(string _exceptWindowName)
	{
		GUIWindow exceptWindow;
		this.nameToWindowMap.TryGetValue(_exceptWindowName, out exceptWindow);
		return this.CloseAllOpenModalWindows(exceptWindow, false);
	}

	// Token: 0x06009444 RID: 37956 RVA: 0x00381374 File Offset: 0x0037F574
	public bool CloseAllOpenModalWindows(GUIWindow _exceptWindow = null, bool _fromEsc = false)
	{
		bool result = false;
		for (int i = 0; i < this.openWindows.Count; i++)
		{
			GUIWindow guiwindow = this.openWindows[i];
			if (guiwindow.isModal && (_exceptWindow == null || _exceptWindow != guiwindow))
			{
				this.Close(guiwindow, _fromEsc);
				result = true;
			}
		}
		if (this.playerUI.CursorController != null)
		{
			if (this.playerUI.CursorController.navigationTarget != null)
			{
				this.playerUI.CursorController.navigationTarget.Controller.Hovered(false);
			}
			this.playerUI.CursorController.SetNavigationTarget(null);
			this.playerUI.CursorController.SetNavigationLockView(null, null);
		}
		return result;
	}

	// Token: 0x06009445 RID: 37957 RVA: 0x00381424 File Offset: 0x0037F624
	public void Open(string _windowName, bool _bModal)
	{
		GUIWindow w;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out w))
		{
			Log.Warning("GUIWindowManager.Open: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			Log.Out("Trace: " + StackTraceUtility.ExtractStackTrace());
			return;
		}
		this.openInternal(w, _bModal, false);
	}

	// Token: 0x06009446 RID: 37958 RVA: 0x00381474 File Offset: 0x0037F674
	public void Open(string _windowName, bool _bModal, bool _bIsNotEscClosable)
	{
		GUIWindow w;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out w))
		{
			Log.Warning("GUIWindowManager.Open: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			Log.Out("Trace: " + StackTraceUtility.ExtractStackTrace());
			return;
		}
		this.openInternal(w, _bModal, _bIsNotEscClosable);
	}

	// Token: 0x06009447 RID: 37959 RVA: 0x003814C3 File Offset: 0x0037F6C3
	public void Open(GUIWindow _w, bool _bModal)
	{
		this.openInternal(_w, _bModal, false);
	}

	// Token: 0x06009448 RID: 37960 RVA: 0x003814CE File Offset: 0x0037F6CE
	public void Open(GUIWindow _w, bool _bModal, bool _bIsNotEscClosable)
	{
		this.openInternal(_w, _bModal, _bIsNotEscClosable);
	}

	// Token: 0x06009449 RID: 37961 RVA: 0x003814DC File Offset: 0x0037F6DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void openInternal(GUIWindow _w, bool _bModal, bool _bIsNotEscClosable)
	{
		if (_w == null)
		{
			return;
		}
		if (this.IsFullHUDDisabled())
		{
			return;
		}
		if (this.IsWindowOpen(_w))
		{
			return;
		}
		QuestEventManager.Current.ChangedWindow(_w.Id);
		if (_bModal)
		{
			this.CloseAllOpenModalWindows(null, false);
			int i = 0;
			while (i < this.windowsToOpen.Count)
			{
				GUIWindow guiwindow = this.windowsToOpen[i];
				if (guiwindow.isModal && guiwindow != _w)
				{
					this.windowsToOpen.Remove(guiwindow);
					guiwindow.OnClose();
					if (guiwindow.HasActionSet())
					{
						this.DisableWindowActionSet(guiwindow);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		_w.isModal = _bModal;
		if (_w.isShowing)
		{
			return;
		}
		_w.windowManager = this;
		_w.isEscClosable = !_bIsNotEscClosable;
		bool flag = _w.isShowing || this.windowsToOpen.Contains(_w);
		if (!this.openWindows.Contains(_w))
		{
			this.windowsToOpen.Add(_w);
		}
		else
		{
			_w.isShowing = true;
		}
		if (!flag)
		{
			_w.OnOpen();
		}
		if (_w.HasActionSet() && !_w.bActionSetEnabled && (_w.isShowing || this.windowsToOpen.Contains(_w)))
		{
			this.EnableWindowActionSet(_w);
		}
	}

	// Token: 0x0600944A RID: 37962 RVA: 0x00381600 File Offset: 0x0037F800
	public bool IsWindowOpen(string _wdwID)
	{
		GUIWindow window;
		return this.nameToWindowMap.TryGetValue(_wdwID, out window) && this.IsWindowOpen(window);
	}

	// Token: 0x0600944B RID: 37963 RVA: 0x00381626 File Offset: 0x0037F826
	public bool IsWindowOpen(GUIWindow _window)
	{
		return _window != null && (_window.isShowing || this.windowsToOpen.Contains(_window));
	}

	// Token: 0x0600944C RID: 37964 RVA: 0x00381643 File Offset: 0x0037F843
	public bool IsModalWindowOpen()
	{
		return this.modalWindow != null;
	}

	// Token: 0x0600944D RID: 37965 RVA: 0x0038164E File Offset: 0x0037F84E
	public GUIWindow GetModalWindow()
	{
		return this.modalWindow;
	}

	// Token: 0x0600944E RID: 37966 RVA: 0x00381656 File Offset: 0x0037F856
	public bool IsCursorWindowOpen()
	{
		return this.cursorWindowOpen;
	}

	// Token: 0x0600944F RID: 37967 RVA: 0x00381660 File Offset: 0x0037F860
	public void Close(string _windowName)
	{
		GUIWindow w;
		if (this.nameToWindowMap.TryGetValue(_windowName, out w))
		{
			this.Close(w, false);
		}
	}

	// Token: 0x06009450 RID: 37968 RVA: 0x00381688 File Offset: 0x0037F888
	public void Close(GUIWindow _w, bool _fromEsc = false)
	{
		if (_w == null)
		{
			return;
		}
		if (_w.isShowing)
		{
			_w.isShowing = false;
			_w.OnClose();
			if (_fromEsc && !string.IsNullOrEmpty(_w.openWindowOnEsc))
			{
				this.Open(_w.openWindowOnEsc, _w.isModal);
			}
		}
		else if (this.windowsToOpen.Contains(_w))
		{
			this.windowsToOpen.Remove(_w);
			_w.OnClose();
		}
		if (_w.bActionSetEnabled)
		{
			this.DisableWindowActionSet(_w);
		}
	}

	// Token: 0x06009451 RID: 37969 RVA: 0x00381702 File Offset: 0x0037F902
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisableWindowActionSet(GUIWindow _w)
	{
		if (_w.playerUI != null && _w.playerUI.ActionSetManager != null)
		{
			_w.playerUI.ActionSetManager.Pop(_w);
			_w.bActionSetEnabled = false;
		}
	}

	// Token: 0x06009452 RID: 37970 RVA: 0x00381737 File Offset: 0x0037F937
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnableWindowActionSet(GUIWindow _w)
	{
		if (_w.playerUI != null && _w.playerUI.ActionSetManager != null)
		{
			_w.playerUI.ActionSetManager.Push(_w);
			_w.bActionSetEnabled = true;
		}
	}

	// Token: 0x06009453 RID: 37971 RVA: 0x0038176C File Offset: 0x0037F96C
	public void ResetActionSets()
	{
		ActionSetManager actionSetManager = this.playerUI.ActionSetManager;
		if (actionSetManager == null)
		{
			return;
		}
		actionSetManager.Reset();
		actionSetManager.Push(this.playerUI.playerInput);
		foreach (GUIWindow guiwindow in this.openWindows)
		{
			if (guiwindow.isShowing && guiwindow.HasActionSet())
			{
				this.EnableWindowActionSet(guiwindow);
			}
		}
		foreach (GUIWindow guiwindow2 in this.windowsToOpen)
		{
			if (guiwindow2.bActionSetEnabled)
			{
				this.EnableWindowActionSet(guiwindow2);
			}
		}
	}

	// Token: 0x06009454 RID: 37972 RVA: 0x00381840 File Offset: 0x0037FA40
	public void RemoveGlobalAction(NGuiAction _action)
	{
		this.globalActions.Remove(_action);
	}

	// Token: 0x06009455 RID: 37973 RVA: 0x0038184F File Offset: 0x0037FA4F
	public void AddGlobalAction(NGuiAction _action)
	{
		this.globalActions.Add(_action);
	}

	// Token: 0x06009456 RID: 37974 RVA: 0x0038185D File Offset: 0x0037FA5D
	public bool IsHUDEnabled()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.Enabled;
	}

	// Token: 0x06009457 RID: 37975 RVA: 0x00381868 File Offset: 0x0037FA68
	public bool IsHUDPartialHidden()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.PartialHide;
	}

	// Token: 0x06009458 RID: 37976 RVA: 0x00381873 File Offset: 0x0037FA73
	public bool IsFullHUDDisabled()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.FullHide;
	}

	// Token: 0x06009459 RID: 37977 RVA: 0x0038187E File Offset: 0x0037FA7E
	public void ToggleHUDEnabled()
	{
		this.bHUDEnabled = this.bHUDEnabled.CycleEnum(false, true);
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x0600945A RID: 37978 RVA: 0x0038189F File Offset: 0x0037FA9F
	public void TempHUDDisable()
	{
		this.bTempEnabled = this.bHUDEnabled;
		this.bHUDEnabled = GUIWindowManager.HudEnabledStates.FullHide;
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x0600945B RID: 37979 RVA: 0x003818C0 File Offset: 0x0037FAC0
	public void ReEnableHUD()
	{
		this.bHUDEnabled = this.bTempEnabled;
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x0600945C RID: 37980 RVA: 0x003818DC File Offset: 0x0037FADC
	public void SetHUDEnabled(GUIWindowManager.HudEnabledStates _hudState)
	{
		this.bHUDEnabled = _hudState;
		if (_hudState <= GUIWindowManager.HudEnabledStates.PartialHide)
		{
			this.nguiWindowManager.ShowAll(true);
			this.playerUI.xui.transform.gameObject.SetActive(true);
			return;
		}
		if (_hudState != GUIWindowManager.HudEnabledStates.FullHide)
		{
			return;
		}
		this.nguiWindowManager.ShowAll(false);
		this.playerUI.xui.transform.gameObject.SetActive(false);
	}

	// Token: 0x04006F13 RID: 28435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> windowsToOpen = new List<GUIWindow>();

	// Token: 0x04006F14 RID: 28436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> windowsToRemove = new List<GUIWindow>();

	// Token: 0x04006F15 RID: 28437
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> openWindows = new List<GUIWindow>();

	// Token: 0x04006F16 RID: 28438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<string, GUIWindow> nameToWindowMap = new CaseInsensitiveStringDictionary<GUIWindow>();

	// Token: 0x04006F17 RID: 28439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindow topmostWindow;

	// Token: 0x04006F18 RID: 28440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindow modalWindow;

	// Token: 0x04006F19 RID: 28441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool cursorWindowOpen;

	// Token: 0x04006F1A RID: 28442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> globalActions = new List<NGuiAction>();

	// Token: 0x04006F1B RID: 28443
	public bool IsInputLocked;

	// Token: 0x04006F1C RID: 28444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04006F1D RID: 28445
	public LocalPlayerUI playerUI;

	// Token: 0x04006F1E RID: 28446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> lastActionClicked = new List<NGuiAction>();

	// Token: 0x04006F1F RID: 28447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> actionsToClear = new List<NGuiAction>();

	// Token: 0x04006F20 RID: 28448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> actionsForGlobalHotkeys = new List<NGuiAction>();

	// Token: 0x04006F21 RID: 28449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager.HudEnabledStates bHUDEnabled;

	// Token: 0x04006F22 RID: 28450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager.HudEnabledStates bTempEnabled;

	// Token: 0x02001223 RID: 4643
	public enum HudEnabledStates
	{
		// Token: 0x04006F24 RID: 28452
		Enabled,
		// Token: 0x04006F25 RID: 28453
		PartialHide,
		// Token: 0x04006F26 RID: 28454
		FullHide
	}
}
