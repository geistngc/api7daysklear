using System;
using UnityEngine;

// Token: 0x02001149 RID: 4425
public class XUiTweenHeight : XUiTweenAbs
{
	// Token: 0x1700102D RID: 4141
	// (get) Token: 0x06008BFC RID: 35836 RVA: 0x00353F6F File Offset: 0x0035216F
	// (set) Token: 0x06008BFD RID: 35837 RVA: 0x00353F77 File Offset: 0x00352177
	[XuiXmlAttribute("start", false)]
	public int Start
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

	// Token: 0x1700102E RID: 4142
	// (get) Token: 0x06008BFE RID: 35838 RVA: 0x00353F9F File Offset: 0x0035219F
	// (set) Token: 0x06008BFF RID: 35839 RVA: 0x00353FA7 File Offset: 0x003521A7
	[XuiXmlAttribute("end", false)]
	public int End
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

	// Token: 0x06008C00 RID: 35840 RVA: 0x00353FD0 File Offset: 0x003521D0
	[XuiXmlAttribute("play_to", false)]
	public void AttributePlayToEnd(int _value)
	{
		if (this.tween == null)
		{
			return;
		}
		if (Mathf.Abs(this.tween.to - _value) < 1)
		{
			return;
		}
		base.Enabled = true;
		this.tween.tweenFactor = 0f;
		this.tween.SetStartToCurrentValue();
		this.End = _value;
		this.tween.PlayForward();
	}

	// Token: 0x06008C01 RID: 35841 RVA: 0x00354036 File Offset: 0x00352236
	public XUiTweenHeight(XUiView _targetView) : base(_targetView)
	{
	}

	// Token: 0x06008C02 RID: 35842 RVA: 0x0035404F File Offset: 0x0035224F
	public override void CreateTween(GameObject _uiGameObject)
	{
		this.tween = _uiGameObject.AddMissingComponent<TweenHeight>();
		this.tween.from = this.start;
		this.tween.to = this.end;
		base.setCommonTweenValues(this.tween);
	}

	// Token: 0x0400676A RID: 26474
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenHeight tween;

	// Token: 0x0400676B RID: 26475
	[PublicizedFrom(EAccessModifier.Private)]
	public int start = 20;

	// Token: 0x0400676C RID: 26476
	[PublicizedFrom(EAccessModifier.Private)]
	public int end = 40;
}
