using System;
using UnityEngine;

// Token: 0x02001146 RID: 4422
public class XUiTweenAlpha : XUiTweenAbs
{
	// Token: 0x17001027 RID: 4135
	// (get) Token: 0x06008BE7 RID: 35815 RVA: 0x00353BA9 File Offset: 0x00351DA9
	// (set) Token: 0x06008BE8 RID: 35816 RVA: 0x00353BB1 File Offset: 0x00351DB1
	[XuiXmlAttribute("start", false)]
	public float Start
	{
		get
		{
			return this.start;
		}
		set
		{
			this.start = Mathf.Clamp01(value);
			if (this.tween != null)
			{
				this.tween.from = this.start;
			}
		}
	}

	// Token: 0x17001028 RID: 4136
	// (get) Token: 0x06008BE9 RID: 35817 RVA: 0x00353BDE File Offset: 0x00351DDE
	// (set) Token: 0x06008BEA RID: 35818 RVA: 0x00353BE6 File Offset: 0x00351DE6
	[XuiXmlAttribute("end", false)]
	public float End
	{
		get
		{
			return this.end;
		}
		set
		{
			this.end = Mathf.Clamp01(value);
			if (this.tween != null)
			{
				this.tween.to = this.end;
			}
		}
	}

	// Token: 0x06008BEB RID: 35819 RVA: 0x00353C14 File Offset: 0x00351E14
	[XuiXmlAttribute("play_to", false)]
	public void AttributePlayToEnd(float _value)
	{
		if (this.tween == null)
		{
			return;
		}
		if ((double)Mathf.Abs(this.tween.to - _value) < 0.0001)
		{
			return;
		}
		base.Enabled = true;
		this.tween.tweenFactor = 0f;
		this.tween.SetStartToCurrentValue();
		this.End = _value;
		this.tween.PlayForward();
	}

	// Token: 0x06008BEC RID: 35820 RVA: 0x00353C83 File Offset: 0x00351E83
	public XUiTweenAlpha(XUiView _targetView) : base(_targetView)
	{
	}

	// Token: 0x06008BED RID: 35821 RVA: 0x00353C97 File Offset: 0x00351E97
	public override void CreateTween(GameObject _uiGameObject)
	{
		this.tween = _uiGameObject.AddMissingComponent<TweenAlpha>();
		this.tween.from = this.start;
		this.tween.to = this.end;
		base.setCommonTweenValues(this.tween);
	}

	// Token: 0x04006761 RID: 26465
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenAlpha tween;

	// Token: 0x04006762 RID: 26466
	[PublicizedFrom(EAccessModifier.Private)]
	public float start;

	// Token: 0x04006763 RID: 26467
	[PublicizedFrom(EAccessModifier.Private)]
	public float end = 1f;
}
