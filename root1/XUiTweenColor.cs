using System;
using UnityEngine;

// Token: 0x02001147 RID: 4423
public class XUiTweenColor : XUiTweenAbs
{
	// Token: 0x17001029 RID: 4137
	// (get) Token: 0x06008BEE RID: 35822 RVA: 0x00353CD3 File Offset: 0x00351ED3
	// (set) Token: 0x06008BEF RID: 35823 RVA: 0x00353CDB File Offset: 0x00351EDB
	[XuiXmlAttribute("start", false)]
	public Color Start
	{
		get
		{
			return this.start;
		}
		set
		{
			this.start = value;
			if (this.tween != null)
			{
				this.tween.from = this.start;
			}
		}
	}

	// Token: 0x1700102A RID: 4138
	// (get) Token: 0x06008BF0 RID: 35824 RVA: 0x00353D03 File Offset: 0x00351F03
	// (set) Token: 0x06008BF1 RID: 35825 RVA: 0x00353D0B File Offset: 0x00351F0B
	[XuiXmlAttribute("end", false)]
	public Color End
	{
		get
		{
			return this.end;
		}
		set
		{
			this.end = value;
			if (this.tween != null)
			{
				this.tween.to = this.end;
			}
		}
	}

	// Token: 0x06008BF2 RID: 35826 RVA: 0x00353D34 File Offset: 0x00351F34
	[XuiXmlAttribute("play_to", false)]
	public void AttributePlayToEnd(Color _value)
	{
		if (this.tween == null)
		{
			return;
		}
		Color to = this.tween.to;
		if ((double)(Mathf.Abs(to.r - _value.r) + Mathf.Abs(to.g - _value.g) + Mathf.Abs(to.b - _value.b) + Mathf.Abs(to.a - _value.a)) < 0.002)
		{
			return;
		}
		base.Enabled = true;
		this.tween.tweenFactor = 0f;
		this.tween.SetStartToCurrentValue();
		this.End = _value;
		this.tween.PlayForward();
	}

	// Token: 0x06008BF3 RID: 35827 RVA: 0x00353DE8 File Offset: 0x00351FE8
	public XUiTweenColor(XUiView _targetView) : base(_targetView)
	{
	}

	// Token: 0x06008BF4 RID: 35828 RVA: 0x00353E07 File Offset: 0x00352007
	public override void CreateTween(GameObject _uiGameObject)
	{
		this.tween = _uiGameObject.AddMissingComponent<TweenColor>();
		this.tween.from = this.start;
		this.tween.to = this.end;
		base.setCommonTweenValues(this.tween);
	}

	// Token: 0x04006764 RID: 26468
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenColor tween;

	// Token: 0x04006765 RID: 26469
	[PublicizedFrom(EAccessModifier.Private)]
	public Color start = Color.black;

	// Token: 0x04006766 RID: 26470
	[PublicizedFrom(EAccessModifier.Private)]
	public Color end = Color.white;
}
