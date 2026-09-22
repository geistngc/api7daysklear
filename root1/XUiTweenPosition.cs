using System;
using UnityEngine;

// Token: 0x0200114A RID: 4426
public class XUiTweenPosition : XUiTweenAbs
{
	// Token: 0x1700102F RID: 4143
	// (get) Token: 0x06008C03 RID: 35843 RVA: 0x0035408B File Offset: 0x0035228B
	// (set) Token: 0x06008C04 RID: 35844 RVA: 0x00354093 File Offset: 0x00352293
	[XuiXmlAttribute("start", false)]
	public Vector3 Start
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

	// Token: 0x17001030 RID: 4144
	// (get) Token: 0x06008C05 RID: 35845 RVA: 0x003540BB File Offset: 0x003522BB
	// (set) Token: 0x06008C06 RID: 35846 RVA: 0x003540C3 File Offset: 0x003522C3
	[XuiXmlAttribute("end", false)]
	public Vector3 End
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

	// Token: 0x06008C07 RID: 35847 RVA: 0x003540EC File Offset: 0x003522EC
	[XuiXmlAttribute("play_to", false)]
	public void AttributePlayToEnd(Vector3 _value)
	{
		if (this.tween == null)
		{
			return;
		}
		if ((double)(this.tween.to - _value).sqrMagnitude < 0.1)
		{
			return;
		}
		base.Enabled = true;
		this.tween.tweenFactor = 0f;
		this.tween.SetStartToCurrentValue();
		this.End = _value;
		this.tween.PlayForward();
	}

	// Token: 0x06008C08 RID: 35848 RVA: 0x00354162 File Offset: 0x00352362
	public XUiTweenPosition(XUiView _targetView) : base(_targetView)
	{
	}

	// Token: 0x06008C09 RID: 35849 RVA: 0x00354190 File Offset: 0x00352390
	public override void CreateTween(GameObject _uiGameObject)
	{
		this.tween = _uiGameObject.AddMissingComponent<TweenPosition>();
		this.tween.from = this.start;
		this.tween.to = this.end;
		base.setCommonTweenValues(this.tween);
	}

	// Token: 0x0400676D RID: 26477
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenPosition tween;

	// Token: 0x0400676E RID: 26478
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 start = Vector3.zero;

	// Token: 0x0400676F RID: 26479
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 end = new Vector3(0f, 8f, 0f);
}
