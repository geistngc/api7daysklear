using System;
using UnityEngine;

// Token: 0x0200114C RID: 4428
public class XUiTweenScale : XUiTweenAbs
{
	// Token: 0x17001033 RID: 4147
	// (get) Token: 0x06008C11 RID: 35857 RVA: 0x0035430C File Offset: 0x0035250C
	// (set) Token: 0x06008C12 RID: 35858 RVA: 0x00354314 File Offset: 0x00352514
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

	// Token: 0x17001034 RID: 4148
	// (get) Token: 0x06008C13 RID: 35859 RVA: 0x0035433C File Offset: 0x0035253C
	// (set) Token: 0x06008C14 RID: 35860 RVA: 0x00354344 File Offset: 0x00352544
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

	// Token: 0x06008C15 RID: 35861 RVA: 0x0035436C File Offset: 0x0035256C
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

	// Token: 0x06008C16 RID: 35862 RVA: 0x003543E2 File Offset: 0x003525E2
	public XUiTweenScale(XUiView _targetView) : base(_targetView)
	{
	}

	// Token: 0x06008C17 RID: 35863 RVA: 0x00354410 File Offset: 0x00352610
	public override void CreateTween(GameObject _uiGameObject)
	{
		this.tween = _uiGameObject.AddMissingComponent<TweenScale>();
		this.tween.from = this.start;
		this.tween.to = this.end;
		base.setCommonTweenValues(this.tween);
	}

	// Token: 0x04006773 RID: 26483
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenScale tween;

	// Token: 0x04006774 RID: 26484
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 start = Vector3.one;

	// Token: 0x04006775 RID: 26485
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 end = new Vector3(1.25f, 1.25f, 1f);
}
