using System;
using UnityEngine;

// Token: 0x0200116C RID: 4460
public class XUiV_Window : XUiView
{
	// Token: 0x1700110A RID: 4362
	// (get) Token: 0x06008E9F RID: 36511 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override UIRect UiRect
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return null;
		}
	}

	// Token: 0x1700110B RID: 4363
	// (get) Token: 0x06008EA0 RID: 36512 RVA: 0x0035AC03 File Offset: 0x00358E03
	// (set) Token: 0x06008EA1 RID: 36513 RVA: 0x0035AC0B File Offset: 0x00358E0B
	[XuiXmlAttribute("anchor", false)]
	public string Anchor
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.anchor;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.anchor = value;
			base.SetDirty();
		}
	}

	// Token: 0x1700110C RID: 4364
	// (get) Token: 0x06008EA2 RID: 36514 RVA: 0x0035AC1A File Offset: 0x00358E1A
	// (set) Token: 0x06008EA3 RID: 36515 RVA: 0x0035AC22 File Offset: 0x00358E22
	[XuiXmlAttribute("cursor_area", false)]
	public bool IsCursorArea { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700110D RID: 4365
	// (get) Token: 0x06008EA4 RID: 36516 RVA: 0x0035AC2B File Offset: 0x00358E2B
	// (set) Token: 0x06008EA5 RID: 36517 RVA: 0x0035AC33 File Offset: 0x00358E33
	[XuiXmlAttribute("lock_navigation", false)]
	public bool LockNavigation { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700110E RID: 4366
	// (get) Token: 0x06008EA6 RID: 36518 RVA: 0x0035AC3C File Offset: 0x00358E3C
	// (set) Token: 0x06008EA7 RID: 36519 RVA: 0x0035AC44 File Offset: 0x00358E44
	public bool IsOpen { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700110F RID: 4367
	// (get) Token: 0x06008EA8 RID: 36520 RVA: 0x0035AC4D File Offset: 0x00358E4D
	public bool IsInStackPanel
	{
		get
		{
			return this.stackPanel != null;
		}
	}

	// Token: 0x17001110 RID: 4368
	// (get) Token: 0x06008EA9 RID: 36521 RVA: 0x0035AC58 File Offset: 0x00358E58
	public override Vector3[] WorldCorners
	{
		get
		{
			return this.panel.worldCorners;
		}
	}

	// Token: 0x06008EAA RID: 36522 RVA: 0x0035AC65 File Offset: 0x00358E65
	public XUiV_Window(XUi _xui, string _id) : base(_xui, _id)
	{
		this.xui.AddWindow(this);
	}

	// Token: 0x06008EAB RID: 36523 RVA: 0x00358784 File Offset: 0x00356984
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		_go.AddComponent<UIPanel>();
	}

	// Token: 0x06008EAC RID: 36524 RVA: 0x0035AC98 File Offset: 0x00358E98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.panel = this.uiTransform.gameObject.GetComponent<UIPanel>();
	}

	// Token: 0x06008EAD RID: 36525 RVA: 0x0035ACB8 File Offset: 0x00358EB8
	public override void InitView()
	{
		base.InitView();
		this.setRootNode();
		base.Controller.OnVisiblity += this.updateVisibility;
		this.panel.depth = base.Depth + 1;
		this.panel.alpha = 0f;
	}

	// Token: 0x06008EAE RID: 36526 RVA: 0x0035AD0B File Offset: 0x00358F0B
	public override void Cleanup()
	{
		base.Cleanup();
		if (this.uiTransform != null)
		{
			UnityEngine.Object.DestroyImmediate(this.uiTransform.gameObject);
		}
		this.xui.RemoveWindow(this);
	}

	// Token: 0x06008EAF RID: 36527 RVA: 0x0035AD40 File Offset: 0x00358F40
	[PublicizedFrom(EAccessModifier.Protected)]
	public void setRootNode()
	{
		Transform transform;
		if (this.stackPanel != null)
		{
			transform = this.stackPanel.Transform;
		}
		else if (!string.IsNullOrEmpty(this.Anchor))
		{
			transform = this.xui.GetAnchor(this.Anchor);
			if (transform == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Specified window anchor \"",
					this.Anchor,
					"\" not found for window \"",
					base.ID,
					"\""
				}));
				throw new Exception();
			}
		}
		else
		{
			transform = this.xui.transform;
		}
		this.uiTransform.parent = transform;
		this.uiTransform.gameObject.layer = 12;
		this.uiTransform.localScale = Vector3.one;
		this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
		this.uiTransform.localEulerAngles = new Vector3(0f, 0f, this.rotation);
	}

	// Token: 0x17001111 RID: 4369
	// (get) Token: 0x06008EB0 RID: 36528 RVA: 0x0035AE52 File Offset: 0x00359052
	public float PanelAlpha
	{
		get
		{
			return this.panel.alpha;
		}
	}

	// Token: 0x17001112 RID: 4370
	// (get) Token: 0x06008EB1 RID: 36529 RVA: 0x0035AE5F File Offset: 0x0035905F
	// (set) Token: 0x06008EB2 RID: 36530 RVA: 0x0035AE67 File Offset: 0x00359067
	[XuiXmlAttribute("fade_window", false)]
	public bool Fade
	{
		get
		{
			return this.fade;
		}
		set
		{
			this.fade = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001113 RID: 4371
	// (get) Token: 0x06008EB3 RID: 36531 RVA: 0x0035AE76 File Offset: 0x00359076
	// (set) Token: 0x06008EB4 RID: 36532 RVA: 0x0035AE7E File Offset: 0x0035907E
	[XuiXmlAttribute("fade_time", false)]
	public float FadeInTime
	{
		get
		{
			return this.fadeInTime;
		}
		set
		{
			this.fadeInTime = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001114 RID: 4372
	// (get) Token: 0x06008EB5 RID: 36533 RVA: 0x0035AE8D File Offset: 0x0035908D
	// (set) Token: 0x06008EB6 RID: 36534 RVA: 0x0035AE95 File Offset: 0x00359095
	[XuiXmlAttribute("fade_delay", false)]
	public float DelayToFadeTime
	{
		get
		{
			return this.delayToFadeTime;
		}
		set
		{
			this.delayToFadeTime = value;
			base.SetDirty();
		}
	}

	// Token: 0x06008EB7 RID: 36535 RVA: 0x0035AEA4 File Offset: 0x003590A4
	public override void Update(float _dt)
	{
		if ((double)Time.timeScale < 0.01 || !this.fade)
		{
			this.delayTimer = this.delayToFadeTime + 1f;
			this.fadeTimer = this.fadeInTime;
		}
		if (this.delayTimer < this.delayToFadeTime)
		{
			this.delayTimer += _dt;
		}
		if (this.delayTimer < this.delayToFadeTime)
		{
			base.Update(_dt);
			return;
		}
		this.panel.alpha = Mathf.Lerp(this.panel.alpha, this.targetAlpha, this.fadeTimer / this.fadeInTime);
		this.fadeTimer += _dt;
		if (this.IsCursorArea && this.oldTransformPosition != this.uiTransform.position && base.IsVisible)
		{
			this.xui.UpdateWindowSoftCursorBounds(this);
			this.oldTransformPosition = this.uiTransform.position;
		}
		base.Update(_dt);
	}

	// Token: 0x06008EB8 RID: 36536 RVA: 0x0035AFA2 File Offset: 0x003591A2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.setRootNode();
		base.updateData();
		this.panel.SetDirty();
	}

	// Token: 0x06008EB9 RID: 36537 RVA: 0x0035AFBC File Offset: 0x003591BC
	public override void OnOpen()
	{
		base.OnOpen();
		base.IsVisible = true;
		if (this.frameClosed == Time.frameCount)
		{
			this.panel.alpha = 1f;
			this.targetAlpha = 1f;
			this.fadeTimer = this.fadeInTime + 1f;
			this.delayTimer = this.delayToFadeTime + 1f;
		}
		else
		{
			this.panel.alpha = 0f;
			this.targetAlpha = 1f;
			this.fadeTimer = 0f;
			this.delayTimer = 0f;
		}
		if (this.IsCursorArea)
		{
			this.oldTransformPosition = Vector3.zero;
		}
		this.applyNavigationChanges();
		this.IsOpen = true;
	}

	// Token: 0x06008EBA RID: 36538 RVA: 0x0035B078 File Offset: 0x00359278
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyNavigationChanges()
	{
		CursorControllerAbs cursorController = this.xui.playerUI.CursorController;
		this.previousNavigationTarget = (cursorController.navigationTargetLater ?? cursorController.CurrentTarget);
		if (this.LockNavigation)
		{
			this.previousNavigationLockView = cursorController.lockNavigationToView;
			cursorController.SetNavigationLockView(this, null);
		}
	}

	// Token: 0x06008EBB RID: 36539 RVA: 0x0035B0C8 File Offset: 0x003592C8
	public override void OnClose()
	{
		base.OnClose();
		base.IsVisible = false;
		this.panel.alpha = 0f;
		this.targetAlpha = 0f;
		this.fadeTimer = this.fadeInTime + 1f;
		this.delayTimer = this.delayToFadeTime + 1f;
		this.frameClosed = Time.frameCount;
		this.IsOpen = false;
		if (this.LockNavigation)
		{
			this.xui.playerUI.CursorController.SetNavigationLockView(this.previousNavigationLockView, this.previousNavigationTarget);
		}
		this.previousNavigationTarget = null;
		this.previousNavigationLockView = null;
	}

	// Token: 0x06008EBC RID: 36540 RVA: 0x0035B16C File Offset: 0x0035936C
	public void ForceVisible(float _alpha = -1f)
	{
		this.delayTimer = this.delayToFadeTime + 1f;
		this.fadeTimer = this.fadeInTime + 1f;
		if (_alpha >= 0f)
		{
			this.targetAlpha = _alpha;
		}
		this.panel.alpha = this.targetAlpha;
	}

	// Token: 0x06008EBD RID: 36541 RVA: 0x0035B1BD File Offset: 0x003593BD
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateVisibility(XUiController _sender, bool _visibleSelf, bool _visibleInScene)
	{
		if (!this.IsCursorArea)
		{
			return;
		}
		if (_visibleInScene)
		{
			this.xui.UpdateWindowSoftCursorBounds(this);
			return;
		}
		this.xui.RemoveWindowFromSoftCursorBounds(this);
	}

	// Token: 0x06008EBE RID: 36542 RVA: 0x0035B1E4 File Offset: 0x003593E4
	[XuiXmlAttribute("panel", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePanel(string _value)
	{
		if (string.IsNullOrEmpty(_value))
		{
			return;
		}
		if (!this.xui.StackPanels.dict.TryGetValue(_value, out this.stackPanel))
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Could not find StackPanel '",
				_value,
				"' for view '",
				this.id,
				"' in window group '",
				this.controller.WindowGroup.Id,
				"'"
			}));
		}
	}

	// Token: 0x04006878 RID: 26744
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchor;

	// Token: 0x04006879 RID: 26745
	[PublicizedFrom(EAccessModifier.Private)]
	public UIPanel panel;

	// Token: 0x0400687B RID: 26747
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView previousNavigationTarget;

	// Token: 0x0400687C RID: 26748
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView previousNavigationLockView;

	// Token: 0x0400687E RID: 26750
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 oldTransformPosition;

	// Token: 0x04006880 RID: 26752
	[PublicizedFrom(EAccessModifier.Private)]
	public XUi.StackPanel stackPanel;

	// Token: 0x04006881 RID: 26753
	[PublicizedFrom(EAccessModifier.Private)]
	public float targetAlpha;

	// Token: 0x04006882 RID: 26754
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fade = true;

	// Token: 0x04006883 RID: 26755
	[PublicizedFrom(EAccessModifier.Private)]
	public int frameClosed;

	// Token: 0x04006884 RID: 26756
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeInTime = 0.05f;

	// Token: 0x04006885 RID: 26757
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeTimer;

	// Token: 0x04006886 RID: 26758
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayToFadeTime = 0.1f;

	// Token: 0x04006887 RID: 26759
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayTimer;
}
