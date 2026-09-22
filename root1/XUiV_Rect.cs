using System;
using UnityEngine;

// Token: 0x0200115D RID: 4445
public class XUiV_Rect : XUiView_WidgetBased
{
	// Token: 0x06008DBE RID: 36286 RVA: 0x00357824 File Offset: 0x00355A24
	public XUiV_Rect(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x170010D0 RID: 4304
	// (get) Token: 0x06008DBF RID: 36287 RVA: 0x0035892F File Offset: 0x00356B2F
	// (set) Token: 0x06008DC0 RID: 36288 RVA: 0x00358937 File Offset: 0x00356B37
	[XuiXmlAttribute("disablefallthrough", false)]
	public bool DisableFallthrough
	{
		get
		{
			return this.disableFallthrough;
		}
		set
		{
			this.disableFallthrough = value;
		}
	}

	// Token: 0x170010D1 RID: 4305
	// (get) Token: 0x06008DC1 RID: 36289 RVA: 0x00358940 File Offset: 0x00356B40
	public override bool HasAnyEvent
	{
		get
		{
			return this.disableFallthrough || base.HasAnyEvent;
		}
	}

	// Token: 0x06008DC2 RID: 36290 RVA: 0x00358952 File Offset: 0x00356B52
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		_go.AddComponent<UIWidget>();
	}

	// Token: 0x06008DC3 RID: 36291 RVA: 0x0035895B File Offset: 0x00356B5B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.widget = this.uiTransform.gameObject.GetComponent<UIWidget>();
	}

	// Token: 0x06008DC4 RID: 36292 RVA: 0x00358979 File Offset: 0x00356B79
	public override void InitView()
	{
		base.InitView();
		UIWidget widget = this.widget;
		widget.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(widget.onChange, new UIWidget.OnDimensionsChanged(base.SetDirty));
		this.updateData();
	}

	// Token: 0x06008DC5 RID: 36293 RVA: 0x003589B0 File Offset: 0x00356BB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void refreshBoxCollider()
	{
		base.refreshBoxCollider();
		if (!this.disableFallthrough)
		{
			return;
		}
		Vector3 center = this.collider.center;
		center.z = 100f;
		this.collider.center = center;
	}

	// Token: 0x0400682B RID: 26667
	[PublicizedFrom(EAccessModifier.Private)]
	public bool disableFallthrough;
}
