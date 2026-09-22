using System;
using UnityEngine;

// Token: 0x02001162 RID: 4450
public class XUiV_TextList : XUiV_LabelBase
{
	// Token: 0x170010EC RID: 4332
	// (get) Token: 0x06008E21 RID: 36385 RVA: 0x003599F3 File Offset: 0x00357BF3
	// (set) Token: 0x06008E22 RID: 36386 RVA: 0x00359A00 File Offset: 0x00357C00
	public float ScrollValue
	{
		get
		{
			return this.textList.scrollValue;
		}
		set
		{
			this.textList.scrollValue = value;
		}
	}

	// Token: 0x170010ED RID: 4333
	// (get) Token: 0x06008E23 RID: 36387 RVA: 0x00359A0E File Offset: 0x00357C0E
	// (set) Token: 0x06008E24 RID: 36388 RVA: 0x00359A16 File Offset: 0x00357C16
	[XuiXmlAttribute("max_paragraphs", false)]
	public int ParagraphHistory
	{
		get
		{
			return this.paragraphHistory;
		}
		set
		{
			if (value == this.paragraphHistory)
			{
				return;
			}
			this.paragraphHistory = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010EE RID: 4334
	// (get) Token: 0x06008E25 RID: 36389 RVA: 0x00359A2F File Offset: 0x00357C2F
	// (set) Token: 0x06008E26 RID: 36390 RVA: 0x00359A37 File Offset: 0x00357C37
	[XuiXmlAttribute("list_style", false)]
	public UITextList.Style ListStyle
	{
		get
		{
			return this.listStyle;
		}
		set
		{
			if (value == this.listStyle)
			{
				return;
			}
			this.listStyle = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010EF RID: 4335
	// (get) Token: 0x06008E27 RID: 36391 RVA: 0x00359A50 File Offset: 0x00357C50
	// (set) Token: 0x06008E28 RID: 36392 RVA: 0x00359A58 File Offset: 0x00357C58
	[XuiXmlAttribute("prefix_first_line", false)]
	public string FirstLinePrefix
	{
		get
		{
			return this.firstLinePrefix;
		}
		set
		{
			if (this.firstLinePrefix == value)
			{
				return;
			}
			this.firstLinePrefix = value;
			base.SetDirty();
		}
	}

	// Token: 0x06008E29 RID: 36393 RVA: 0x00359A76 File Offset: 0x00357C76
	public XUiV_TextList(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008E2A RID: 36394 RVA: 0x00359A88 File Offset: 0x00357C88
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		base.createComponents(_go);
		_go.AddComponent<UITextList>();
	}

	// Token: 0x06008E2B RID: 36395 RVA: 0x00359A98 File Offset: 0x00357C98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.textList = this.uiTransform.GetComponent<UITextList>();
	}

	// Token: 0x06008E2C RID: 36396 RVA: 0x00359AB1 File Offset: 0x00357CB1
	public override void InitView()
	{
		base.EventOnDrag = true;
		base.EventOnScroll = true;
		base.InitView();
	}

	// Token: 0x06008E2D RID: 36397 RVA: 0x00359AC8 File Offset: 0x00357CC8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.label.width = this.size.x;
		this.label.height = this.size.y;
		this.textList.paragraphHistory = this.paragraphHistory;
		this.textList.style = this.listStyle;
		base.updateData();
	}

	// Token: 0x06008E2E RID: 36398 RVA: 0x00359B29 File Offset: 0x00357D29
	public override void Cleanup()
	{
		this.textList.Clear();
		base.Cleanup();
	}

	// Token: 0x06008E2F RID: 36399 RVA: 0x00359B3C File Offset: 0x00357D3C
	public void AddLine(string _line)
	{
		if (!string.IsNullOrEmpty(this.firstLinePrefix))
		{
			_line = this.firstLinePrefix + _line;
		}
		_line = this.getFormattedText(_line);
		this.textList.Add(_line);
	}

	// Token: 0x0400684D RID: 26701
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITextList textList;

	// Token: 0x0400684E RID: 26702
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITextList.Style listStyle;

	// Token: 0x0400684F RID: 26703
	[PublicizedFrom(EAccessModifier.Protected)]
	public string firstLinePrefix;

	// Token: 0x04006850 RID: 26704
	[PublicizedFrom(EAccessModifier.Protected)]
	public int paragraphHistory = 50;
}
