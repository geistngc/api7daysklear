using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02001150 RID: 4432
public abstract class XUiView_WidgetBased : XUiView
{
	// Token: 0x1700107D RID: 4221
	// (get) Token: 0x06008CCE RID: 36046 RVA: 0x00356616 File Offset: 0x00354816
	public override UIRect UiRect
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.widget;
		}
	}

	// Token: 0x1700107E RID: 4222
	// (get) Token: 0x06008CCF RID: 36047 RVA: 0x0035661E File Offset: 0x0035481E
	public override Vector3 LocalCenter
	{
		get
		{
			return this.widget.localCenter;
		}
	}

	// Token: 0x1700107F RID: 4223
	// (get) Token: 0x06008CD0 RID: 36048 RVA: 0x0035662B File Offset: 0x0035482B
	public override Vector3[] WorldCorners
	{
		get
		{
			return this.widget.worldCorners;
		}
	}

	// Token: 0x17001080 RID: 4224
	// (get) Token: 0x06008CD1 RID: 36049 RVA: 0x00356638 File Offset: 0x00354838
	// (set) Token: 0x06008CD2 RID: 36050 RVA: 0x00356640 File Offset: 0x00354840
	[XuiXmlAttribute("keep_aspect_ratio", false)]
	public UIWidget.AspectRatioSource KeepAspectRatio
	{
		get
		{
			return this.keepAspectRatio;
		}
		set
		{
			if (this.keepAspectRatio == value)
			{
				return;
			}
			this.keepAspectRatio = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001081 RID: 4225
	// (get) Token: 0x06008CD3 RID: 36051 RVA: 0x00356659 File Offset: 0x00354859
	// (set) Token: 0x06008CD4 RID: 36052 RVA: 0x00356661 File Offset: 0x00354861
	[XuiXmlAttribute("aspect_ratio", false)]
	public float AspectRatio
	{
		get
		{
			return this.aspectRatio;
		}
		set
		{
			if (Mathf.Approximately(this.aspectRatio, value))
			{
				return;
			}
			this.aspectRatio = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001082 RID: 4226
	// (get) Token: 0x06008CD5 RID: 36053 RVA: 0x0035667F File Offset: 0x0035487F
	// (set) Token: 0x06008CD6 RID: 36054 RVA: 0x00356687 File Offset: 0x00354887
	[XuiXmlAttribute("pivot", false)]
	public UIWidget.Pivot Pivot
	{
		get
		{
			return this.pivot;
		}
		set
		{
			if (this.pivot == value)
			{
				return;
			}
			this.pivot = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001083 RID: 4227
	// (get) Token: 0x06008CD7 RID: 36055 RVA: 0x003566A0 File Offset: 0x003548A0
	// (set) Token: 0x06008CD8 RID: 36056 RVA: 0x003566A8 File Offset: 0x003548A8
	[XuiXmlAttribute("auto_resize_collider", false)]
	public bool AutoResizeCollider
	{
		get
		{
			return this.autoResizeCollider;
		}
		set
		{
			if (this.autoResizeCollider == value)
			{
				return;
			}
			this.autoResizeCollider = value;
			base.SetDirty();
		}
	}

	// Token: 0x06008CD9 RID: 36057 RVA: 0x003566C1 File Offset: 0x003548C1
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiView_WidgetBased(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008CDA RID: 36058 RVA: 0x003566CC File Offset: 0x003548CC
	public override void InitView()
	{
		base.InitView();
		this.widget.pivot = this.pivot;
		this.widget.depth = this.depth;
		this.uiTransform.localScale = Vector3.one;
		this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
	}

	// Token: 0x06008CDB RID: 36059 RVA: 0x0035673E File Offset: 0x0035493E
	public override void SetDefaults(XUiController _parent)
	{
		this.Pivot = UIWidget.Pivot.TopLeft;
		base.SetDefaults(_parent);
	}

	// Token: 0x06008CDC RID: 36060 RVA: 0x00356750 File Offset: 0x00354950
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void refreshBoxCollider()
	{
		if (this.AutoResizeCollider || this.widget.isAnchored)
		{
			this.widget.autoResizeBoxCollider = true;
			return;
		}
		this.collider.center = this.widget.localCenter;
		this.collider.size = new Vector3(this.widget.localSize.x * base.ColliderScale + (float)(2 * base.ColliderPadding.x), this.widget.localSize.y * base.ColliderScale + (float)(2 * base.ColliderPadding.y), 0f);
	}

	// Token: 0x06008CDD RID: 36061 RVA: 0x003567F8 File Offset: 0x003549F8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.anchoredLeftAndRight)
		{
			this.size.x = Mathf.RoundToInt(this.widget.localSize.x);
		}
		if (this.anchoredTopAndBottom)
		{
			this.size.y = Mathf.RoundToInt(this.widget.localSize.y);
		}
	}

	// Token: 0x06008CDE RID: 36062 RVA: 0x0035685C File Offset: 0x00354A5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.widget.keepAspectRatio = this.keepAspectRatio;
		this.widget.aspectRatio = this.aspectRatio;
		base.updateData();
		this.refreshBoxCollider();
	}

	// Token: 0x17001084 RID: 4228
	// (get) Token: 0x06008CDF RID: 36063 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool anchorsKeepSize
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x06008CE0 RID: 36064 RVA: 0x0035688C File Offset: 0x00354A8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void anchorsParsed()
	{
		base.anchorsParsed();
		if (this.anchorsKeepSize)
		{
			if (!this.anchoredLeftAndRight && this.widget.width != this.size.x)
			{
				this.widget.width = this.size.x;
			}
			if (!this.anchoredTopAndBottom && this.widget.height != this.size.y)
			{
				this.widget.height = this.size.y;
			}
		}
		ThreadManager.StartCoroutine(this.<anchorsParsed>g__MarkAsChangedLater|31_0());
	}

	// Token: 0x06008CE1 RID: 36065 RVA: 0x0035691F File Offset: 0x00354B1F
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator <anchorsParsed>g__MarkAsChangedLater|31_0()
	{
		yield return new WaitForEndOfFrame();
		this.widget.MarkAsChanged();
		yield break;
	}

	// Token: 0x040067C9 RID: 26569
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget widget;

	// Token: 0x040067CA RID: 26570
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget.AspectRatioSource keepAspectRatio;

	// Token: 0x040067CB RID: 26571
	[PublicizedFrom(EAccessModifier.Protected)]
	public float aspectRatio;

	// Token: 0x040067CC RID: 26572
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool autoResizeCollider;

	// Token: 0x040067CD RID: 26573
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget.Pivot pivot;
}
