using System;
using UnityEngine;

// Token: 0x02001161 RID: 4449
public class XUiV_Table : XUiView
{
	// Token: 0x170010E2 RID: 4322
	// (get) Token: 0x06008E08 RID: 36360 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override UIRect UiRect
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return null;
		}
	}

	// Token: 0x170010E3 RID: 4323
	// (get) Token: 0x06008E09 RID: 36361 RVA: 0x003597E7 File Offset: 0x003579E7
	// (set) Token: 0x06008E0A RID: 36362 RVA: 0x003597EF File Offset: 0x003579EF
	[XuiXmlAttribute("sorting", false)]
	public UITable.Sorting Sorting { get; set; }

	// Token: 0x170010E4 RID: 4324
	// (get) Token: 0x06008E0B RID: 36363 RVA: 0x003597F8 File Offset: 0x003579F8
	// (set) Token: 0x06008E0C RID: 36364 RVA: 0x00359800 File Offset: 0x00357A00
	[XuiXmlAttribute("cols", false)]
	public int Columns { get; set; } = 1;

	// Token: 0x170010E5 RID: 4325
	// (get) Token: 0x06008E0D RID: 36365 RVA: 0x00359809 File Offset: 0x00357A09
	// (set) Token: 0x06008E0E RID: 36366 RVA: 0x00359811 File Offset: 0x00357A11
	[XuiXmlAttribute("padding", true)]
	public Vector2 Padding { get; set; }

	// Token: 0x170010E6 RID: 4326
	// (get) Token: 0x06008E0F RID: 36367 RVA: 0x0035981A File Offset: 0x00357A1A
	// (set) Token: 0x06008E10 RID: 36368 RVA: 0x00359822 File Offset: 0x00357A22
	[XuiXmlAttribute("hide_inactive", false)]
	public bool HideInactive { get; set; } = true;

	// Token: 0x170010E7 RID: 4327
	// (get) Token: 0x06008E11 RID: 36369 RVA: 0x0035982B File Offset: 0x00357A2B
	// (set) Token: 0x06008E12 RID: 36370 RVA: 0x00359833 File Offset: 0x00357A33
	[XuiXmlAttribute("always_reposition", false)]
	public bool AlwaysReposition { get; set; }

	// Token: 0x170010E8 RID: 4328
	// (get) Token: 0x06008E13 RID: 36371 RVA: 0x0035983C File Offset: 0x00357A3C
	// (set) Token: 0x06008E14 RID: 36372 RVA: 0x00359844 File Offset: 0x00357A44
	[XuiXmlAttribute("reposition_twice", false)]
	public bool RepositionTwice { get; set; }

	// Token: 0x170010E9 RID: 4329
	// (get) Token: 0x06008E15 RID: 36373 RVA: 0x0035984D File Offset: 0x00357A4D
	// (set) Token: 0x06008E16 RID: 36374 RVA: 0x00359855 File Offset: 0x00357A55
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

	// Token: 0x170010EA RID: 4330
	// (get) Token: 0x06008E17 RID: 36375 RVA: 0x0035986E File Offset: 0x00357A6E
	// (set) Token: 0x06008E18 RID: 36376 RVA: 0x00359876 File Offset: 0x00357A76
	[XuiXmlAttribute("cell_alignment", false)]
	public UIWidget.Pivot CellAlignment
	{
		get
		{
			return this.cellAlignment;
		}
		set
		{
			if (this.cellAlignment == value)
			{
				return;
			}
			this.cellAlignment = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010EB RID: 4331
	// (get) Token: 0x06008E19 RID: 36377 RVA: 0x00356F15 File Offset: 0x00355115
	public override Vector3[] WorldCorners
	{
		get
		{
			return XUiV_Empty.WorldCornersEmpty;
		}
	}

	// Token: 0x06008E1A RID: 36378 RVA: 0x0035988F File Offset: 0x00357A8F
	public XUiV_Table(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008E1B RID: 36379 RVA: 0x003598AE File Offset: 0x00357AAE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		_go.AddComponent<UITable>();
	}

	// Token: 0x06008E1C RID: 36380 RVA: 0x003598B7 File Offset: 0x00357AB7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.table = this.uiTransform.gameObject.GetComponent<UITable>();
	}

	// Token: 0x06008E1D RID: 36381 RVA: 0x003598D8 File Offset: 0x00357AD8
	public override void InitView()
	{
		base.InitView();
		this.table.hideInactive = this.HideInactive;
		this.table.sorting = this.Sorting;
		this.table.direction = UITable.Direction.Down;
		this.table.columns = this.Columns;
		this.table.padding = this.Padding;
		this.table.pivot = this.pivot;
		this.table.cellAlignment = this.cellAlignment;
		this.table.Reposition();
	}

	// Token: 0x06008E1E RID: 36382 RVA: 0x00359968 File Offset: 0x00357B68
	public override void OnOpen()
	{
		base.OnOpen();
		this.table.repositionNow = true;
		if (this.RepositionTwice)
		{
			this.repositionNextFrame = true;
		}
	}

	// Token: 0x06008E1F RID: 36383 RVA: 0x0035998C File Offset: 0x00357B8C
	public override void Update(float _dt)
	{
		if (this.firstUpdate || this.AlwaysReposition)
		{
			this.firstUpdate = false;
			this.table.Reposition();
		}
		if (this.repositionNextFrame && !this.table.enabled)
		{
			this.table.repositionNow = true;
			this.repositionNextFrame = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06008E20 RID: 36384 RVA: 0x003599EA File Offset: 0x00357BEA
	public void Reposition()
	{
		this.repositionNextFrame = true;
	}

	// Token: 0x04006842 RID: 26690
	[PublicizedFrom(EAccessModifier.Private)]
	public UITable table;

	// Token: 0x04006849 RID: 26697
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget.Pivot pivot;

	// Token: 0x0400684A RID: 26698
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget.Pivot cellAlignment;

	// Token: 0x0400684B RID: 26699
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstUpdate = true;

	// Token: 0x0400684C RID: 26700
	[PublicizedFrom(EAccessModifier.Private)]
	public bool repositionNextFrame;
}
