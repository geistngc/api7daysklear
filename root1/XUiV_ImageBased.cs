using System;
using UnityEngine;

// Token: 0x02001158 RID: 4440
public abstract class XUiV_ImageBased : XUiView_WidgetBased
{
	// Token: 0x170010A5 RID: 4261
	// (get) Token: 0x06008D45 RID: 36165 RVA: 0x00357ADD File Offset: 0x00355CDD
	// (set) Token: 0x06008D46 RID: 36166 RVA: 0x00357AE5 File Offset: 0x00355CE5
	[XuiXmlAttribute("keepsourceaspectratio", false)]
	public bool KeepSourceAspectRatio
	{
		get
		{
			return this.keepSourceAspectRatio;
		}
		set
		{
			if (value == this.keepSourceAspectRatio)
			{
				return;
			}
			this.keepSourceAspectRatio = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010A6 RID: 4262
	// (get) Token: 0x06008D47 RID: 36167 RVA: 0x00356F29 File Offset: 0x00355129
	// (set) Token: 0x06008D48 RID: 36168 RVA: 0x00357AFE File Offset: 0x00355CFE
	[XuiXmlAttribute("type", false)]
	public virtual UIBasicSprite.Type Type
	{
		get
		{
			return this.type;
		}
		set
		{
			if (this.type == value)
			{
				return;
			}
			this.type = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010A7 RID: 4263
	// (get) Token: 0x06008D49 RID: 36169 RVA: 0x00357B17 File Offset: 0x00355D17
	// (set) Token: 0x06008D4A RID: 36170 RVA: 0x00357B1F File Offset: 0x00355D1F
	[XuiXmlAttribute("fillcenter", false)]
	public bool FillCenter
	{
		get
		{
			return this.fillCenter;
		}
		set
		{
			if (this.fillCenter == value)
			{
				return;
			}
			this.fillCenter = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010A8 RID: 4264
	// (get) Token: 0x06008D4B RID: 36171 RVA: 0x00357B38 File Offset: 0x00355D38
	// (set) Token: 0x06008D4C RID: 36172 RVA: 0x00357B40 File Offset: 0x00355D40
	[XuiXmlAttribute("flip", false)]
	public UIBasicSprite.Flip Flip
	{
		get
		{
			return this.flip;
		}
		set
		{
			if (this.flip == value)
			{
				return;
			}
			this.flip = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010A9 RID: 4265
	// (get) Token: 0x06008D4D RID: 36173 RVA: 0x00357B59 File Offset: 0x00355D59
	// (set) Token: 0x06008D4E RID: 36174 RVA: 0x00357B61 File Offset: 0x00355D61
	[XuiXmlAttribute("globalopacitymod", false)]
	public float GlobalOpacityModifier
	{
		get
		{
			return this.globalOpacityModifier;
		}
		set
		{
			if (Mathf.Approximately(this.globalOpacityModifier, value))
			{
				return;
			}
			this.globalOpacityModifier = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010AA RID: 4266
	// (get) Token: 0x06008D4F RID: 36175 RVA: 0x00357B7F File Offset: 0x00355D7F
	// (set) Token: 0x06008D50 RID: 36176 RVA: 0x00357B87 File Offset: 0x00355D87
	[XuiXmlAttribute("foregroundlayer", false)]
	public bool ForegroundLayer
	{
		get
		{
			return this.foregroundLayer;
		}
		set
		{
			if (this.foregroundLayer == value)
			{
				return;
			}
			this.foregroundLayer = value;
			base.SetDirty();
		}
	}

	// Token: 0x170010AB RID: 4267
	// (get) Token: 0x06008D51 RID: 36177 RVA: 0x00357BA0 File Offset: 0x00355DA0
	public float GlobalOpacitySetting
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!this.foregroundLayer)
			{
				return this.xui.BackgroundGlobalOpacity;
			}
			return this.xui.ForegroundGlobalOpacity;
		}
	}

	// Token: 0x06008D52 RID: 36178 RVA: 0x00357BC4 File Offset: 0x00355DC4
	public override void Update(float _dt)
	{
		float globalOpacitySetting = this.GlobalOpacitySetting;
		if (!Mathf.Approximately(this.previousGlobalOpacity, globalOpacitySetting))
		{
			this.previousGlobalOpacity = globalOpacitySetting;
			base.SetDirty();
		}
		base.Update(_dt);
	}

	// Token: 0x06008D53 RID: 36179 RVA: 0x00357BFC File Offset: 0x00355DFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color opacityModColor(Color _origColor)
	{
		if (this.globalOpacityModifier == 0f)
		{
			return _origColor;
		}
		float globalOpacitySetting = this.GlobalOpacitySetting;
		if (globalOpacitySetting >= 0.999f)
		{
			return _origColor;
		}
		float a = Mathf.Clamp01(_origColor.a * this.globalOpacityModifier * globalOpacitySetting);
		return new Color(_origColor.r, _origColor.g, _origColor.b, a);
	}

	// Token: 0x06008D54 RID: 36180 RVA: 0x00357C56 File Offset: 0x00355E56
	public XUiV_ImageBased(XUi _xui, string _id) : base(_xui, _id)
	{
	}

	// Token: 0x06008D55 RID: 36181 RVA: 0x00357C6B File Offset: 0x00355E6B
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.FillCenter = true;
		this.Type = UIBasicSprite.Type.Simple;
	}

	// Token: 0x040067FB RID: 26619
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool keepSourceAspectRatio;

	// Token: 0x040067FC RID: 26620
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Type type;

	// Token: 0x040067FD RID: 26621
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Flip flip;

	// Token: 0x040067FE RID: 26622
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool fillCenter;

	// Token: 0x040067FF RID: 26623
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool foregroundLayer;

	// Token: 0x04006800 RID: 26624
	[PublicizedFrom(EAccessModifier.Private)]
	public float globalOpacityModifier = 1f;

	// Token: 0x04006801 RID: 26625
	[PublicizedFrom(EAccessModifier.Private)]
	public float previousGlobalOpacity;
}
