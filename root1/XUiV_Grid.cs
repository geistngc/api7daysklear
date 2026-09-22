using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001157 RID: 4439
public class XUiV_Grid : XUiView_WidgetBased
{
	// Token: 0x1700109D RID: 4253
	// (get) Token: 0x06008D29 RID: 36137 RVA: 0x0035770D File Offset: 0x0035590D
	// (set) Token: 0x06008D2A RID: 36138 RVA: 0x00357715 File Offset: 0x00355915
	[XuiXmlAttribute("arrangement", false)]
	public UIGrid.Arrangement Arrangement { get; set; }

	// Token: 0x140000EC RID: 236
	// (add) Token: 0x06008D2B RID: 36139 RVA: 0x00357720 File Offset: 0x00355920
	// (remove) Token: 0x06008D2C RID: 36140 RVA: 0x00357758 File Offset: 0x00355958
	public event UIGrid.OnSizeChanged OnSizeChanged;

	// Token: 0x1700109E RID: 4254
	// (get) Token: 0x06008D2D RID: 36141 RVA: 0x0035778D File Offset: 0x0035598D
	// (set) Token: 0x06008D2E RID: 36142 RVA: 0x00357795 File Offset: 0x00355995
	[XuiXmlAttribute("cols", false)]
	public int Columns
	{
		get
		{
			return this.columns;
		}
		set
		{
			if (this.columns == value)
			{
				return;
			}
			this.columns = value;
			base.SetDirty();
		}
	}

	// Token: 0x1700109F RID: 4255
	// (get) Token: 0x06008D2F RID: 36143 RVA: 0x003577AE File Offset: 0x003559AE
	// (set) Token: 0x06008D30 RID: 36144 RVA: 0x003577B6 File Offset: 0x003559B6
	[XuiXmlAttribute("rows", false)]
	public int Rows
	{
		get
		{
			return this.rows;
		}
		set
		{
			if (this.rows == value)
			{
				return;
			}
			this.rows = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010A0 RID: 4256
	// (get) Token: 0x06008D31 RID: 36145 RVA: 0x003577CF File Offset: 0x003559CF
	// (set) Token: 0x06008D32 RID: 36146 RVA: 0x000027FC File Offset: 0x000009FC
	public override int RepeatCount
	{
		get
		{
			return this.Columns * this.Rows;
		}
		set
		{
		}
	}

	// Token: 0x170010A1 RID: 4257
	// (get) Token: 0x06008D33 RID: 36147 RVA: 0x003577DE File Offset: 0x003559DE
	// (set) Token: 0x06008D34 RID: 36148 RVA: 0x003577E6 File Offset: 0x003559E6
	[XuiXmlAttribute("cell_width", false)]
	public int CellWidth { get; set; }

	// Token: 0x170010A2 RID: 4258
	// (get) Token: 0x06008D35 RID: 36149 RVA: 0x003577EF File Offset: 0x003559EF
	// (set) Token: 0x06008D36 RID: 36150 RVA: 0x003577F7 File Offset: 0x003559F7
	[XuiXmlAttribute("cell_height", false)]
	public int CellHeight { get; set; }

	// Token: 0x170010A3 RID: 4259
	// (get) Token: 0x06008D37 RID: 36151 RVA: 0x00357800 File Offset: 0x00355A00
	// (set) Token: 0x06008D38 RID: 36152 RVA: 0x00357808 File Offset: 0x00355A08
	[XuiXmlAttribute("hide_inactive", false)]
	public bool HideInactive { get; set; }

	// Token: 0x170010A4 RID: 4260
	// (get) Token: 0x06008D39 RID: 36153 RVA: 0x00357811 File Offset: 0x00355A11
	public override Vector2i InnerSize
	{
		get
		{
			return new Vector2i(this.CellWidth, this.CellHeight);
		}
	}

	// Token: 0x06008D3A RID: 36154 RVA: 0x00357824 File Offset: 0x00355A24
	public XUiV_Grid(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008D3B RID: 36155 RVA: 0x0035782E File Offset: 0x00355A2E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		_go.AddComponent<UIWidget>();
		_go.AddComponent<UIGrid>();
	}

	// Token: 0x06008D3C RID: 36156 RVA: 0x0035783E File Offset: 0x00355A3E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.widget = this.uiTransform.gameObject.GetComponent<UIWidget>();
		this.grid = this.uiTransform.gameObject.GetComponent<UIGrid>();
	}

	// Token: 0x06008D3D RID: 36157 RVA: 0x00357874 File Offset: 0x00355A74
	public override void InitView()
	{
		base.InitView();
		this.widget.autoResizeBoxCollider = true;
		this.grid.hideInactive = this.HideInactive;
		this.grid.arrangement = this.Arrangement;
		this.grid.pivot = this.pivot;
		this.grid.onSizeChanged = new UIGrid.OnSizeChanged(this.OnGridSizeChanged);
		this.grid.maxPerLine = ((this.Arrangement == UIGrid.Arrangement.Horizontal) ? this.Columns : this.Rows);
		this.grid.cellWidth = (float)this.CellWidth;
		this.grid.cellHeight = (float)this.CellHeight;
	}

	// Token: 0x06008D3E RID: 36158 RVA: 0x00357924 File Offset: 0x00355B24
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGridSizeChanged(Vector2Int _cells, Vector2 _size)
	{
		this.widget.width = Mathf.RoundToInt(_size.x);
		this.widget.height = Mathf.RoundToInt(_size.y);
		this.size = new Vector2i(this.widget.width, this.widget.height);
		UIGrid.OnSizeChanged onSizeChanged = this.OnSizeChanged;
		if (onSizeChanged == null)
		{
			return;
		}
		onSizeChanged(_cells, _size);
	}

	// Token: 0x06008D3F RID: 36159 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void refreshBoxCollider()
	{
	}

	// Token: 0x06008D40 RID: 36160 RVA: 0x00357990 File Offset: 0x00355B90
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.grid.repositionNow = true;
	}

	// Token: 0x06008D41 RID: 36161 RVA: 0x003579A8 File Offset: 0x00355BA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.grid.maxPerLine = ((this.Arrangement == UIGrid.Arrangement.Horizontal) ? this.Columns : this.Rows);
		this.grid.cellWidth = (float)this.CellWidth;
		this.grid.cellHeight = (float)this.CellHeight;
		base.updateData();
	}

	// Token: 0x06008D42 RID: 36162 RVA: 0x00357A00 File Offset: 0x00355C00
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.Columns = 0;
		this.Rows = 0;
		this.CellWidth = int.MinValue;
		this.CellHeight = int.MinValue;
		this.Arrangement = UIGrid.Arrangement.Horizontal;
		this.HideInactive = true;
	}

	// Token: 0x06008D43 RID: 36163 RVA: 0x00357A3B File Offset: 0x00355C3B
	public override void SetPostParsingDefaults(XUiController _parent)
	{
		base.SetPostParsingDefaults(_parent);
		if (this.CellWidth == -2147483648)
		{
			this.CellWidth = base.Width;
		}
		if (this.CellHeight == -2147483648)
		{
			this.CellHeight = base.Height;
		}
	}

	// Token: 0x06008D44 RID: 36164 RVA: 0x00357A78 File Offset: 0x00355C78
	public override void SetRepeatContentTemplateParams(Dictionary<string, object> _templateParams, int _curRepeatNum)
	{
		base.SetRepeatContentTemplateParams(_templateParams, _curRepeatNum);
		int num;
		int num2;
		if (this.Arrangement == UIGrid.Arrangement.Horizontal)
		{
			num = _curRepeatNum % this.Columns;
			num2 = _curRepeatNum / this.Columns;
		}
		else
		{
			num = _curRepeatNum / this.Rows;
			num2 = _curRepeatNum % this.Rows;
		}
		_templateParams["repeat_col"] = num;
		_templateParams["repeat_row"] = num2;
	}

	// Token: 0x040067F3 RID: 26611
	[PublicizedFrom(EAccessModifier.Private)]
	public int columns;

	// Token: 0x040067F4 RID: 26612
	[PublicizedFrom(EAccessModifier.Private)]
	public int rows;

	// Token: 0x040067F5 RID: 26613
	[PublicizedFrom(EAccessModifier.Private)]
	public UIGrid grid;
}
