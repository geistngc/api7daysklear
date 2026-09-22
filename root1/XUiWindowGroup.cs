using System;
using System.Collections.Generic;
using Platform;

// Token: 0x02001187 RID: 4487
public class XUiWindowGroup : GUIWindow
{
	// Token: 0x1700113E RID: 4414
	// (get) Token: 0x06008FBC RID: 36796 RVA: 0x00360C92 File Offset: 0x0035EE92
	// (set) Token: 0x06008FBD RID: 36797 RVA: 0x00360C9A File Offset: 0x0035EE9A
	public bool Initialized { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06008FBE RID: 36798 RVA: 0x00360CA4 File Offset: 0x0035EEA4
	public XUiWindowGroup(XUi _xui, string _id, XUiWindowGroup.EHasActionSetFor _hasActionSetFor, string _defaultSelectedName, int _stackPanelYOffset, int _stackPanelPadding, bool _openBackpackOnOpen, bool _closeCompassOnOpen) : base(_id)
	{
		this.Windows = this.windows.AsReadOnly();
		this.xui = _xui;
		this.playerUI = this.xui.playerUI;
		this.windowManager = this.playerUI.windowManager;
		this.hasActionSetFor = _hasActionSetFor;
		this.defaultSelectedView = _defaultSelectedName;
		if (_stackPanelYOffset != -2147483648)
		{
			this.StackPanelYOffset = _stackPanelYOffset;
		}
		if (_stackPanelPadding != -2147483648)
		{
			this.StackPanelPadding = _stackPanelPadding;
		}
		this.openBackpackOnOpen = _openBackpackOnOpen;
		this.closeCompassOnOpen = _closeCompassOnOpen;
	}

	// Token: 0x06008FBF RID: 36799 RVA: 0x00360D68 File Offset: 0x0035EF68
	public void Init()
	{
		if (this.Initialized)
		{
			return;
		}
		this.Controller.AutoBindComponents();
		this.Controller.Init();
		this.Controller.AutoBindEvents();
		foreach (XUiController xuiController in this.Controller.Children)
		{
			if (xuiController.ViewComponent != null)
			{
				this.windows.Add((XUiV_Window)xuiController.ViewComponent);
				xuiController.ViewComponent.IsVisible = false;
			}
		}
		this.windowManager.Add(this);
		this.Initialized = true;
	}

	// Token: 0x06008FC0 RID: 36800 RVA: 0x00360E20 File Offset: 0x0035F020
	public override void Cleanup()
	{
		if (this.cleanedUp)
		{
			return;
		}
		this.cleanedUp = true;
		CursorControllerAbs cursorController = this.xui.playerUI.CursorController;
		if (cursorController.navigationTarget != null && cursorController.navigationTarget.Controller.IsChildOf(this.Controller))
		{
			cursorController.SetNavigationTarget(null);
		}
		this.Controller.Cleanup();
	}

	// Token: 0x06008FC1 RID: 36801 RVA: 0x00360E80 File Offset: 0x0035F080
	public override void OnOpen()
	{
		base.OnOpen();
		this.Controller.OnOpen();
		this.applyDefaultSelectedView();
		if (this.closeCompassOnOpen)
		{
			this.windowManager.Close("compass");
		}
		if (this.openBackpackOnOpen && GameManager.Instance != null)
		{
			this.windowManager.Open("backpack", false);
		}
		this.xui.RecenterWindowGroup(this, false);
		bool flag;
		switch (this.hasActionSetFor)
		{
		case XUiWindowGroup.EHasActionSetFor.Both:
			flag = true;
			break;
		case XUiWindowGroup.EHasActionSetFor.OnlyController:
			flag = (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard);
			break;
		case XUiWindowGroup.EHasActionSetFor.OnlyKeyboard:
			flag = (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard);
			break;
		case XUiWindowGroup.EHasActionSetFor.None:
			flag = false;
			break;
		default:
			flag = this.hasActionSetThisOpen;
			break;
		}
		this.hasActionSetThisOpen = flag;
	}

	// Token: 0x06008FC2 RID: 36802 RVA: 0x00360F54 File Offset: 0x0035F154
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyDefaultSelectedView()
	{
		if (string.IsNullOrEmpty(this.defaultSelectedView))
		{
			return;
		}
		if (this.defaultSelectedView.StartsWith("bp."))
		{
			XUiC_BackpackWindow.defaultSelectedElement = this.defaultSelectedView.Remove(0, 3);
			return;
		}
		XUiController childById = this.Controller.GetChildById(this.defaultSelectedView);
		if (childById != null)
		{
			childById.SelectCursorElement(true, false);
			return;
		}
		Log.Warning("Could not find selectable element " + this.defaultSelectedView + " in WindowGroup " + this.Id);
	}

	// Token: 0x06008FC3 RID: 36803 RVA: 0x00360FD4 File Offset: 0x0035F1D4
	public override void OnClose()
	{
		base.OnClose();
		XUiC_DragAndDropWindow dragAndDropWindow = this.xui.DragAndDropWindow;
		if (dragAndDropWindow != null)
		{
			dragAndDropWindow.PlaceItemBackInInventory();
		}
		this.Controller.OnClose();
		if (this.openBackpackOnOpen && GameManager.Instance != null)
		{
			this.windowManager.Close("backpack");
		}
		if (this.xui.ToolTipWindow != null)
		{
			this.xui.ToolTipWindow.ToolTip = "";
		}
		XUiC_PopupMenu popupMenuWindow = this.xui.PopupMenuWindow;
		if (popupMenuWindow == null)
		{
			return;
		}
		popupMenuWindow.Close();
	}

	// Token: 0x06008FC4 RID: 36804 RVA: 0x00361064 File Offset: 0x0035F264
	public override bool HasActionSet()
	{
		return this.hasActionSetThisOpen;
	}

	// Token: 0x06008FC5 RID: 36805 RVA: 0x0036106C File Offset: 0x0035F26C
	public bool HasStackPanelWindows()
	{
		if (this.Controller == null)
		{
			return false;
		}
		using (List<XUiV_Window>.Enumerator enumerator = this.windows.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsInStackPanel)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04006934 RID: 26932
	public readonly XUi xui;

	// Token: 0x04006935 RID: 26933
	public XUiController Controller;

	// Token: 0x04006937 RID: 26935
	public readonly bool LeftPanelVAlignTop = true;

	// Token: 0x04006938 RID: 26936
	public readonly bool RightPanelVAlignTop = true;

	// Token: 0x04006939 RID: 26937
	public readonly int StackPanelYOffset = 457;

	// Token: 0x0400693A RID: 26938
	public readonly int StackPanelPadding = 9;

	// Token: 0x0400693B RID: 26939
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool openBackpackOnOpen;

	// Token: 0x0400693C RID: 26940
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool closeCompassOnOpen;

	// Token: 0x0400693D RID: 26941
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string defaultSelectedView;

	// Token: 0x0400693E RID: 26942
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiWindowGroup.EHasActionSetFor hasActionSetFor;

	// Token: 0x0400693F RID: 26943
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasActionSetThisOpen = true;

	// Token: 0x04006940 RID: 26944
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiV_Window> windows = new List<XUiV_Window>();

	// Token: 0x04006941 RID: 26945
	public readonly IReadOnlyList<XUiV_Window> Windows;

	// Token: 0x04006942 RID: 26946
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cleanedUp;

	// Token: 0x02001188 RID: 4488
	public enum EHasActionSetFor
	{
		// Token: 0x04006944 RID: 26948
		Both,
		// Token: 0x04006945 RID: 26949
		OnlyController,
		// Token: 0x04006946 RID: 26950
		OnlyKeyboard,
		// Token: 0x04006947 RID: 26951
		None
	}
}
