using System;
using UnityEngine;

// Token: 0x02001152 RID: 4434
public class XUiV_Button : XUiV_ImageBased
{
	// Token: 0x06008CE8 RID: 36072 RVA: 0x0035699C File Offset: 0x00354B9C
	public XUiV_Button(XUi _xui, string _id) : base(_xui, _id)
	{
		base.UseSelectionBox = false;
	}

	// Token: 0x17001087 RID: 4231
	// (get) Token: 0x06008CE9 RID: 36073 RVA: 0x00356A47 File Offset: 0x00354C47
	// (set) Token: 0x06008CEA RID: 36074 RVA: 0x00356A4F File Offset: 0x00354C4F
	[XuiXmlAttribute("atlas", false)]
	public string UIAtlas
	{
		get
		{
			return this.uiAtlas;
		}
		set
		{
			this.uiAtlas = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001088 RID: 4232
	// (get) Token: 0x06008CEB RID: 36075 RVA: 0x00356A5E File Offset: 0x00354C5E
	// (set) Token: 0x06008CEC RID: 36076 RVA: 0x00356A66 File Offset: 0x00354C66
	public string DefaultSpriteName
	{
		get
		{
			return this.defaultSpriteName;
		}
		set
		{
			this.defaultSpriteName = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17001089 RID: 4233
	// (get) Token: 0x06008CED RID: 36077 RVA: 0x00356A7B File Offset: 0x00354C7B
	// (set) Token: 0x06008CEE RID: 36078 RVA: 0x00356A83 File Offset: 0x00354C83
	public Color DefaultSpriteColor
	{
		get
		{
			return this.defaultSpriteColor;
		}
		set
		{
			this.defaultSpriteColor = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108A RID: 4234
	// (get) Token: 0x06008CEF RID: 36079 RVA: 0x00356A98 File Offset: 0x00354C98
	// (set) Token: 0x06008CF0 RID: 36080 RVA: 0x00356AB9 File Offset: 0x00354CB9
	[XuiXmlAttribute("hoversprite", false)]
	public string HoverSpriteName
	{
		get
		{
			if (!(this.hoverSpriteName == ""))
			{
				return this.hoverSpriteName;
			}
			return this.defaultSpriteName;
		}
		set
		{
			this.hoverSpriteName = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108B RID: 4235
	// (get) Token: 0x06008CF1 RID: 36081 RVA: 0x00356ACE File Offset: 0x00354CCE
	// (set) Token: 0x06008CF2 RID: 36082 RVA: 0x00356AD6 File Offset: 0x00354CD6
	[XuiXmlAttribute("hovercolor", false)]
	public Color HoverSpriteColor
	{
		get
		{
			return this.hoverSpriteColor;
		}
		set
		{
			this.hoverSpriteColor = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108C RID: 4236
	// (get) Token: 0x06008CF3 RID: 36083 RVA: 0x00356AEB File Offset: 0x00354CEB
	// (set) Token: 0x06008CF4 RID: 36084 RVA: 0x00356B0C File Offset: 0x00354D0C
	[XuiXmlAttribute("selectedsprite", false)]
	public string SelectedSpriteName
	{
		get
		{
			if (!(this.selectedSpriteName == ""))
			{
				return this.selectedSpriteName;
			}
			return this.defaultSpriteName;
		}
		set
		{
			this.selectedSpriteName = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108D RID: 4237
	// (get) Token: 0x06008CF5 RID: 36085 RVA: 0x00356B21 File Offset: 0x00354D21
	// (set) Token: 0x06008CF6 RID: 36086 RVA: 0x00356B29 File Offset: 0x00354D29
	[XuiXmlAttribute("selectedcolor", false)]
	public Color SelectedSpriteColor
	{
		get
		{
			return this.selectedSpriteColor;
		}
		set
		{
			this.selectedSpriteColor = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108E RID: 4238
	// (get) Token: 0x06008CF7 RID: 36087 RVA: 0x00356B3E File Offset: 0x00354D3E
	// (set) Token: 0x06008CF8 RID: 36088 RVA: 0x00356B5F File Offset: 0x00354D5F
	[XuiXmlAttribute("disabledsprite", false)]
	public string DisabledSpriteName
	{
		get
		{
			if (!(this.disabledSpriteName == ""))
			{
				return this.disabledSpriteName;
			}
			return this.defaultSpriteName;
		}
		set
		{
			this.disabledSpriteName = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x1700108F RID: 4239
	// (get) Token: 0x06008CF9 RID: 36089 RVA: 0x00356B74 File Offset: 0x00354D74
	// (set) Token: 0x06008CFA RID: 36090 RVA: 0x00356B7C File Offset: 0x00354D7C
	[XuiXmlAttribute("disabledcolor", false)]
	public Color DisabledSpriteColor
	{
		get
		{
			return this.disabledSpriteColor;
		}
		set
		{
			this.disabledSpriteColor = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17001090 RID: 4240
	// (get) Token: 0x06008CFB RID: 36091 RVA: 0x00356B91 File Offset: 0x00354D91
	// (set) Token: 0x06008CFC RID: 36092 RVA: 0x00356B99 File Offset: 0x00354D99
	[XuiXmlAttribute("manualcolors", false)]
	public bool ManualColors
	{
		get
		{
			return this.manualColors;
		}
		set
		{
			if (value == this.manualColors)
			{
				return;
			}
			this.manualColors = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17001091 RID: 4241
	// (get) Token: 0x06008CFD RID: 36093 RVA: 0x00356BB8 File Offset: 0x00354DB8
	// (set) Token: 0x06008CFE RID: 36094 RVA: 0x00356BC0 File Offset: 0x00354DC0
	public Color CurrentColor
	{
		get
		{
			return this.currentColor;
		}
		set
		{
			this.currentColor = value;
			base.SetDirty();
			this.colorDirty = true;
		}
	}

	// Token: 0x17001092 RID: 4242
	// (get) Token: 0x06008CFF RID: 36095 RVA: 0x00356BD6 File Offset: 0x00354DD6
	// (set) Token: 0x06008D00 RID: 36096 RVA: 0x00356BDE File Offset: 0x00354DDE
	public string CurrentSpriteName
	{
		get
		{
			return this.currentSpriteName;
		}
		set
		{
			if (value == this.currentSpriteName)
			{
				return;
			}
			this.currentSpriteName = value;
			base.SetDirty();
			this.colorDirty = true;
		}
	}

	// Token: 0x17001093 RID: 4243
	// (get) Token: 0x06008D01 RID: 36097 RVA: 0x00356C03 File Offset: 0x00354E03
	// (set) Token: 0x06008D02 RID: 36098 RVA: 0x00356C0B File Offset: 0x00354E0B
	[XuiXmlAttribute("selected", false)]
	public bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			if (this.selected == value)
			{
				return;
			}
			this.selected = value;
			base.SetDirty();
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17001094 RID: 4244
	// (get) Token: 0x06008D03 RID: 36099 RVA: 0x00356C2A File Offset: 0x00354E2A
	// (set) Token: 0x06008D04 RID: 36100 RVA: 0x00356C32 File Offset: 0x00354E32
	[XuiXmlAttribute("hoverscale", false)]
	public float HoverScale
	{
		get
		{
			return this.hoverScale;
		}
		set
		{
			this.hoverScale = value;
			base.SetDirty();
		}
	}

	// Token: 0x17001095 RID: 4245
	// (set) Token: 0x06008D05 RID: 36101 RVA: 0x00356C44 File Offset: 0x00354E44
	public override bool Enabled
	{
		set
		{
			bool enabled = this.enabled;
			base.Enabled = value;
			if (value != enabled)
			{
				this.updateCurrentSprite();
				if (!value)
				{
					TweenScale tweenScale = this.tweenScale;
					if (tweenScale != null)
					{
						tweenScale.PlayReverse();
					}
				}
				if (!this.gamepadSelectableSetFromAttributes)
				{
					base.IsNavigatable = value;
				}
			}
		}
	}

	// Token: 0x06008D06 RID: 36102 RVA: 0x00356C8C File Offset: 0x00354E8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateCurrentSprite()
	{
		if (!this.Enabled)
		{
			if (!this.manualColors)
			{
				this.CurrentColor = this.disabledSpriteColor;
			}
			this.CurrentSpriteName = this.DisabledSpriteName;
			return;
		}
		if (this.Selected)
		{
			if (!this.manualColors)
			{
				this.CurrentColor = this.selectedSpriteColor;
			}
			this.CurrentSpriteName = this.SelectedSpriteName;
			return;
		}
		if (!this.manualColors)
		{
			this.CurrentColor = (this.isOver ? this.hoverSpriteColor : this.defaultSpriteColor);
		}
		this.CurrentSpriteName = (this.isOver ? this.HoverSpriteName : this.DefaultSpriteName);
	}

	// Token: 0x06008D07 RID: 36103 RVA: 0x00356D2B File Offset: 0x00354F2B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createComponents(GameObject _go)
	{
		_go.AddComponent<UISprite>();
		_go.AddComponent<TweenScale>().enabled = false;
	}

	// Token: 0x06008D08 RID: 36104 RVA: 0x00356D40 File Offset: 0x00354F40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void captureComponents()
	{
		base.captureComponents();
		this.widget = (this.sprite = this.uiTransform.GetComponent<UISprite>());
		this.tweenScale = this.uiTransform.GetComponent<TweenScale>();
	}

	// Token: 0x06008D09 RID: 36105 RVA: 0x00356D7E File Offset: 0x00354F7E
	public override void InitView()
	{
		base.EventOnPress = true;
		base.EventOnHover = true;
		base.InitView();
		this.updateData();
		this.Enabled = true;
	}

	// Token: 0x06008D0A RID: 36106 RVA: 0x00356DA1 File Offset: 0x00354FA1
	public override void Update(float _dt)
	{
		if (this.isOver && !base.UiTransformIsHovered)
		{
			this.OnHover(false);
		}
		base.Update(_dt);
	}

	// Token: 0x06008D0B RID: 36107 RVA: 0x00356DC4 File Offset: 0x00354FC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateData()
	{
		this.sprite.spriteName = this.currentSpriteName;
		this.sprite.atlas = this.xui.GetAtlasByName(this.uiAtlas, this.currentSpriteName);
		this.sprite.color = base.opacityModColor(this.currentColor);
		if (!Mathf.Approximately(this.hoverScale, 1f))
		{
			this.tweenScale.from = Vector3.one;
			this.tweenScale.to = Vector3.one * this.hoverScale;
			this.tweenScale.duration = this.hoverDuration;
		}
		this.sprite.centerType = UIBasicSprite.AdvancedType.Sliced;
		this.sprite.type = this.type;
		this.sprite.flip = this.flip;
		base.updateData();
	}

	// Token: 0x06008D0C RID: 36108 RVA: 0x00356E9D File Offset: 0x0035509D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHover(bool _isOver)
	{
		base.OnHover(_isOver);
		this.updateCurrentSprite();
		if (this.Enabled && !Mathf.Approximately(this.hoverScale, 1f))
		{
			this.tweenScale.Play(_isOver);
		}
	}

	// Token: 0x06008D0D RID: 36109 RVA: 0x00356ED2 File Offset: 0x003550D2
	public override void OnOpen()
	{
		base.OnOpen();
		this.updateCurrentSprite();
		this.uiTransform.localScale = Vector3.one;
	}

	// Token: 0x06008D0E RID: 36110 RVA: 0x00356EF0 File Offset: 0x003550F0
	[XuiXmlAttribute("sprite", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeSprite(string _value)
	{
		this.DefaultSpriteName = _value;
		this.CurrentSpriteName = _value;
	}

	// Token: 0x06008D0F RID: 36111 RVA: 0x00356F00 File Offset: 0x00355100
	[XuiXmlAttribute("defaultcolor", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeSprite(Color _value)
	{
		this.DefaultSpriteColor = _value;
		this.CurrentColor = this.defaultSpriteColor;
	}

	// Token: 0x040067D1 RID: 26577
	[PublicizedFrom(EAccessModifier.Protected)]
	public string uiAtlas = string.Empty;

	// Token: 0x040067D2 RID: 26578
	[PublicizedFrom(EAccessModifier.Protected)]
	public UISprite sprite;

	// Token: 0x040067D3 RID: 26579
	[PublicizedFrom(EAccessModifier.Protected)]
	public string defaultSpriteName = string.Empty;

	// Token: 0x040067D4 RID: 26580
	[PublicizedFrom(EAccessModifier.Protected)]
	public string hoverSpriteName = string.Empty;

	// Token: 0x040067D5 RID: 26581
	[PublicizedFrom(EAccessModifier.Protected)]
	public string selectedSpriteName = string.Empty;

	// Token: 0x040067D6 RID: 26582
	[PublicizedFrom(EAccessModifier.Protected)]
	public string disabledSpriteName = string.Empty;

	// Token: 0x040067D7 RID: 26583
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color defaultSpriteColor = Color.white;

	// Token: 0x040067D8 RID: 26584
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color hoverSpriteColor = Color.white;

	// Token: 0x040067D9 RID: 26585
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color selectedSpriteColor = Color.white;

	// Token: 0x040067DA RID: 26586
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color disabledSpriteColor = Color.white;

	// Token: 0x040067DB RID: 26587
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool manualColors;

	// Token: 0x040067DC RID: 26588
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color currentColor = Color.white;

	// Token: 0x040067DD RID: 26589
	[PublicizedFrom(EAccessModifier.Protected)]
	public string currentSpriteName = string.Empty;

	// Token: 0x040067DE RID: 26590
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool selected;

	// Token: 0x040067DF RID: 26591
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastVisible;

	// Token: 0x040067E0 RID: 26592
	[PublicizedFrom(EAccessModifier.Private)]
	public bool colorDirty;

	// Token: 0x040067E1 RID: 26593
	[PublicizedFrom(EAccessModifier.Private)]
	public float hoverScale = 1f;

	// Token: 0x040067E2 RID: 26594
	[PublicizedFrom(EAccessModifier.Private)]
	public float hoverDuration = 0.25f;

	// Token: 0x040067E3 RID: 26595
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenScale tweenScale;
}
