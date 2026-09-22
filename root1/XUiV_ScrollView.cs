using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200115F RID: 4447
public class XUiV_ScrollView : XUiV_Panel
{
	// Token: 0x170010D6 RID: 4310
	// (get) Token: 0x06008DD4 RID: 36308 RVA: 0x00358CA7 File Offset: 0x00356EA7
	// (set) Token: 0x06008DD5 RID: 36309 RVA: 0x00358CAF File Offset: 0x00356EAF
	[XuiXmlAttribute("movement", false)]
	public UIScrollView.Movement Movement
	{
		get
		{
			return this.movement;
		}
		set
		{
			if (value == this.movement)
			{
				return;
			}
			this.movement = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010D7 RID: 4311
	// (get) Token: 0x06008DD6 RID: 36310 RVA: 0x00358CC8 File Offset: 0x00356EC8
	// (set) Token: 0x06008DD7 RID: 36311 RVA: 0x00358CD0 File Offset: 0x00356ED0
	[XuiXmlAttribute("drageffect", false)]
	public UIScrollView.DragEffect DragEffect
	{
		get
		{
			return this.dragEffect;
		}
		set
		{
			if (value == this.dragEffect)
			{
				return;
			}
			this.dragEffect = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010D8 RID: 4312
	// (get) Token: 0x06008DD8 RID: 36312 RVA: 0x00358CE9 File Offset: 0x00356EE9
	// (set) Token: 0x06008DD9 RID: 36313 RVA: 0x00358CF1 File Offset: 0x00356EF1
	[XuiXmlAttribute("scrollfactor", false)]
	public float ScrollFactor
	{
		get
		{
			return this.scrollFactor;
		}
		set
		{
			if (Mathf.Approximately(value, this.scrollFactor))
			{
				return;
			}
			this.scrollFactor = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010D9 RID: 4313
	// (get) Token: 0x06008DDA RID: 36314 RVA: 0x00358D0F File Offset: 0x00356F0F
	// (set) Token: 0x06008DDB RID: 36315 RVA: 0x00358D17 File Offset: 0x00356F17
	[XuiXmlAttribute("scrollwithoutfocus", false)]
	public bool ScrollWithoutFocus
	{
		get
		{
			return this.scrollWithoutFocus;
		}
		set
		{
			if (value == this.scrollWithoutFocus)
			{
				return;
			}
			this.scrollWithoutFocus = value;
			base.SetDirty();
		}
	}

	// Token: 0x06008DDC RID: 36316 RVA: 0x00358D30 File Offset: 0x00356F30
	public XUiV_ScrollView(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008DDD RID: 36317 RVA: 0x00358D4C File Offset: 0x00356F4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		base.createComponents(_go);
		_go.AddComponent<UIScrollView>();
	}

	// Token: 0x06008DDE RID: 36318 RVA: 0x00358D5C File Offset: 0x00356F5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.scrollView = this.uiTransform.gameObject.GetComponent<UIScrollView>();
	}

	// Token: 0x06008DDF RID: 36319 RVA: 0x00358D7C File Offset: 0x00356F7C
	public override void InitView()
	{
		base.InitView();
		this.controller.Parent.OnScroll += this.OnScrolled;
		this.xui.OnBuilt += this.OnXuiLoadDone;
		base.SetDirty();
	}

	// Token: 0x06008DE0 RID: 36320 RVA: 0x00358DC8 File Offset: 0x00356FC8
	public override void Cleanup()
	{
		base.Cleanup();
		this.xui.OnBuilt -= this.OnXuiLoadDone;
	}

	// Token: 0x06008DE1 RID: 36321 RVA: 0x00358DE8 File Offset: 0x00356FE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnXuiLoadDone()
	{
		this.xui.OnBuilt -= this.OnXuiLoadDone;
		this.applyScrollEventToChildren(this.controller);
		this.controller.Parent.TryGetChildView<XUiV_ScrollBar>("", out this.scrollBar);
		XUiV_ScrollBar xuiV_ScrollBar = this.scrollBar;
		if (xuiV_ScrollBar == null)
		{
			return;
		}
		xuiV_ScrollBar.Connect(new XUiEvent_OnScrollEventHandler(this.OnScrolled));
	}

	// Token: 0x06008DE2 RID: 36322 RVA: 0x00358E50 File Offset: 0x00357050
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyScrollEventToChildren(XUiController _controller)
	{
		foreach (XUiController xuiController in _controller.Children)
		{
			this.applyScrollEventToChildren(xuiController);
			xuiController.ViewComponent.EventOnScroll = true;
			xuiController.OnScroll += this.OnScrolled;
		}
	}

	// Token: 0x06008DE3 RID: 36323 RVA: 0x00358EC4 File Offset: 0x003570C4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.controllerScroll(Time.unscaledDeltaTime);
	}

	// Token: 0x06008DE4 RID: 36324 RVA: 0x00358ED8 File Offset: 0x003570D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void controllerScroll(float _dt)
	{
		if (!XUiUtils.HotkeysAllowedFor(this.controller.Parent.ViewComponent, false))
		{
			return;
		}
		XUiView currentTarget = this.xui.playerUI.CursorController.CurrentTarget;
		if (!this.ScrollWithoutFocus && (currentTarget == null || !currentTarget.Controller.IsSelfOrChildOf(this.controller.Parent)))
		{
			return;
		}
		Vector2 vector = this.xui.playerUI.playerInput.GUIActions.Camera.Vector;
		float num = Mathf.Abs(vector.x);
		float num2 = Mathf.Abs(vector.y);
		if (this.movement == UIScrollView.Movement.Vertical)
		{
			if ((double)num2 < (double)num * 1.8)
			{
				return;
			}
			if ((double)num2 > 0.1)
			{
				this.scrollView.Scroll(vector.y * _dt);
				return;
			}
		}
		else
		{
			if ((double)num < (double)num2 * 1.8)
			{
				return;
			}
			if ((double)num > 0.1)
			{
				this.scrollView.Scroll(vector.x * _dt);
			}
		}
	}

	// Token: 0x06008DE5 RID: 36325 RVA: 0x00358FDC File Offset: 0x003571DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		base.Clipping = UIDrawCall.Clipping.SoftClip;
		this.scrollView.movement = this.movement;
		this.scrollView.dragEffect = this.dragEffect;
		this.scrollView.scrollWheelFactor = this.scrollFactor;
		this.scrollView.constrainOnDrag = true;
		this.scrollView.disableDragIfFits = true;
		this.scrollView.smoothDragStart = true;
		XUiV_ScrollBar xuiV_ScrollBar = this.scrollBar;
		UIScrollBar uiscrollBar = (xuiV_ScrollBar != null) ? xuiV_ScrollBar.ScrollBar : null;
		if (uiscrollBar != null)
		{
			uiscrollBar.fillDirection = ((this.movement == UIScrollView.Movement.Horizontal) ? UIProgressBar.FillDirection.LeftToRight : UIProgressBar.FillDirection.TopToBottom);
			UIScrollView.Movement movement = this.movement;
			if (movement != UIScrollView.Movement.Horizontal)
			{
				if (movement == UIScrollView.Movement.Vertical)
				{
					this.scrollView.horizontalScrollBar = null;
					this.scrollView.verticalScrollBar = uiscrollBar;
				}
			}
			else
			{
				this.scrollView.horizontalScrollBar = uiscrollBar;
				this.scrollView.verticalScrollBar = null;
			}
			this.scrollView.CheckScrollbars();
		}
		base.updateData();
	}

	// Token: 0x06008DE6 RID: 36326 RVA: 0x003590C8 File Offset: 0x003572C8
	public override void OnOpen()
	{
		base.OnOpen();
		this.ResetPosition();
	}

	// Token: 0x06008DE7 RID: 36327 RVA: 0x003590D6 File Offset: 0x003572D6
	public override void OnVisibilityChanged(bool _visibleInScene)
	{
		base.OnVisibilityChanged(_visibleInScene);
		if (_visibleInScene)
		{
			this.UpdatePosition();
		}
	}

	// Token: 0x06008DE8 RID: 36328 RVA: 0x003590E8 File Offset: 0x003572E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnScrolled(XUiController _sender, float _delta)
	{
		if (InputUtils.ShiftKeyPressed)
		{
			return;
		}
		this.scrollView.Scroll(_delta);
	}

	// Token: 0x06008DE9 RID: 36329 RVA: 0x003590FE File Offset: 0x003572FE
	public void ResetPosition()
	{
		this.scrollView.ResetPosition();
	}

	// Token: 0x06008DEA RID: 36330 RVA: 0x0035910B File Offset: 0x0035730B
	public void UpdatePosition()
	{
		this.scrollView.UpdatePosition();
	}

	// Token: 0x06008DEB RID: 36331 RVA: 0x00359118 File Offset: 0x00357318
	public void MakeVisible(XUiView _viewComponent)
	{
		if (_viewComponent == null)
		{
			return;
		}
		Vector3[] worldCorners = _viewComponent.WorldCorners;
		this.MakeVisible(worldCorners);
	}

	// Token: 0x06008DEC RID: 36332 RVA: 0x00359138 File Offset: 0x00357338
	public void MakeVisible(IList<Vector3> _worldCorners)
	{
		if (_worldCorners == null || _worldCorners.Count < 4)
		{
			return;
		}
		Transform cachedTransform = this.panel.cachedTransform;
		Vector3[] worldCorners = this.panel.worldCorners;
		Vector3 vector = cachedTransform.InverseTransformPoint(worldCorners[1]);
		Vector3 vector2 = cachedTransform.InverseTransformPoint(worldCorners[3]);
		Vector3 vector3 = cachedTransform.InverseTransformPoint(_worldCorners[1]);
		Vector3 vector4 = cachedTransform.InverseTransformPoint(_worldCorners[3]);
		Vector3 zero = Vector3.zero;
		if (!this.scrollView.canMoveHorizontally)
		{
			zero.x = 0f;
		}
		else if (vector3.x < vector.x)
		{
			zero.x = vector.x - vector3.x + base.ClippingSoftness.x;
		}
		else if (vector4.x > vector2.x)
		{
			zero.x = vector2.x - vector4.x - base.ClippingSoftness.x;
		}
		if (!this.scrollView.canMoveVertically)
		{
			zero.y = 0f;
		}
		else if (vector3.y > vector.y)
		{
			zero.y = vector.y - vector3.y - base.ClippingSoftness.y;
		}
		else if (vector4.y < vector2.y)
		{
			zero.y = vector2.y - vector4.y + base.ClippingSoftness.y;
		}
		zero.z = 0f;
		cachedTransform.localPosition += zero;
		Vector2 clipOffset = this.panel.clipOffset;
		clipOffset.x -= zero.x;
		clipOffset.y -= zero.y;
		this.panel.clipOffset = clipOffset;
	}

	// Token: 0x06008DED RID: 36333 RVA: 0x003592FC File Offset: 0x003574FC
	public void CenterOn(XUiView _target)
	{
		if (_target == null)
		{
			return;
		}
		Vector3[] worldCorners = this.panel.worldCorners;
		Vector3 position = (worldCorners[2] + worldCorners[0]) * 0.5f;
		Transform cachedTransform = this.panel.cachedTransform;
		Vector3 a = cachedTransform.InverseTransformPoint(_target.UiTransform.position);
		Vector3 b = cachedTransform.InverseTransformPoint(position);
		Vector3 vector = a - b;
		if (!this.scrollView.canMoveHorizontally)
		{
			vector.x = 0f;
		}
		if (!this.scrollView.canMoveVertically)
		{
			vector.y = 0f;
		}
		vector.z = 0f;
		cachedTransform.localPosition -= vector;
		Vector2 clipOffset = this.panel.clipOffset;
		clipOffset.x += vector.x;
		clipOffset.y += vector.y;
		this.panel.clipOffset = clipOffset;
	}

	// Token: 0x04006831 RID: 26673
	[PublicizedFrom(EAccessModifier.Private)]
	public UIScrollView scrollView;

	// Token: 0x04006832 RID: 26674
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_ScrollBar scrollBar;

	// Token: 0x04006833 RID: 26675
	[PublicizedFrom(EAccessModifier.Private)]
	public UIScrollView.Movement movement = UIScrollView.Movement.Vertical;

	// Token: 0x04006834 RID: 26676
	[PublicizedFrom(EAccessModifier.Private)]
	public UIScrollView.DragEffect dragEffect;

	// Token: 0x04006835 RID: 26677
	[PublicizedFrom(EAccessModifier.Private)]
	public float scrollFactor = 2f;

	// Token: 0x04006836 RID: 26678
	[PublicizedFrom(EAccessModifier.Private)]
	public bool scrollWithoutFocus;
}
