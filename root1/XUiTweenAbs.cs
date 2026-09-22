using System;
using UnityEngine;

// Token: 0x02001144 RID: 4420
public abstract class XUiTweenAbs : IXUiElement
{
	// Token: 0x17001021 RID: 4129
	// (get) Token: 0x06008BD2 RID: 35794 RVA: 0x003537FD File Offset: 0x003519FD
	// (set) Token: 0x06008BD3 RID: 35795 RVA: 0x00353805 File Offset: 0x00351A05
	[XuiXmlAttribute("repeat", false)]
	public UITweener.Style RepeatStyle
	{
		get
		{
			return this.repeatStyle;
		}
		set
		{
			if (this.repeatStyle == value)
			{
				return;
			}
			this.repeatStyle = value;
			if (this.tweenGeneric != null)
			{
				this.tweenGeneric.style = value;
			}
		}
	}

	// Token: 0x17001022 RID: 4130
	// (get) Token: 0x06008BD4 RID: 35796 RVA: 0x00353832 File Offset: 0x00351A32
	// (set) Token: 0x06008BD5 RID: 35797 RVA: 0x0035383A File Offset: 0x00351A3A
	[XuiXmlAttribute("duration", false)]
	public float Duration
	{
		get
		{
			return this.duration;
		}
		set
		{
			if (Mathf.Approximately(this.duration, value))
			{
				return;
			}
			this.duration = value;
			if (this.tweenGeneric != null)
			{
				this.tweenGeneric.duration = value;
			}
		}
	}

	// Token: 0x17001023 RID: 4131
	// (get) Token: 0x06008BD6 RID: 35798 RVA: 0x0035386C File Offset: 0x00351A6C
	// (set) Token: 0x06008BD7 RID: 35799 RVA: 0x00353874 File Offset: 0x00351A74
	[XuiXmlAttribute("method", false)]
	public UITweener.Method Method
	{
		get
		{
			return this.method;
		}
		set
		{
			if (this.method == value)
			{
				return;
			}
			this.method = value;
			if (this.tweenGeneric != null)
			{
				this.tweenGeneric.method = value;
			}
		}
	}

	// Token: 0x17001024 RID: 4132
	// (get) Token: 0x06008BD8 RID: 35800 RVA: 0x003538A1 File Offset: 0x00351AA1
	// (set) Token: 0x06008BD9 RID: 35801 RVA: 0x003538A9 File Offset: 0x00351AA9
	[XuiXmlAttribute("enabled", false)]
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			if (this.enabled == value)
			{
				return;
			}
			this.enabled = value;
			if (this.tweenGeneric != null)
			{
				this.tweenGeneric.enabled = value;
			}
		}
	}

	// Token: 0x06008BDA RID: 35802 RVA: 0x003538D6 File Offset: 0x00351AD6
	[XuiXmlAttribute("curve", false)]
	public void AttributeCurve(string _curveString)
	{
		this.curve = XUiTweenAbs.parseCurve(_curveString);
		if (this.tweenGeneric != null)
		{
			this.tweenGeneric.animationCurve = this.curve;
		}
	}

	// Token: 0x06008BDB RID: 35803 RVA: 0x00353903 File Offset: 0x00351B03
	[XuiXmlAttribute("play_forward", false)]
	public void AttributePlayForward(bool _play)
	{
		if (!_play)
		{
			return;
		}
		this.Enabled = true;
		if (this.tweenGeneric != null)
		{
			this.tweenGeneric.PlayForward();
		}
	}

	// Token: 0x06008BDC RID: 35804 RVA: 0x00353929 File Offset: 0x00351B29
	[XuiXmlAttribute("play_reverse", false)]
	public void AttributePlayReverse(bool _play)
	{
		if (!_play)
		{
			return;
		}
		this.Enabled = true;
		if (this.tweenGeneric != null)
		{
			this.tweenGeneric.PlayReverse();
		}
	}

	// Token: 0x06008BDD RID: 35805 RVA: 0x0035394F File Offset: 0x00351B4F
	[XuiXmlAttribute("set_to_start", false)]
	public void AttributeSetToStart(bool _set)
	{
		if (!_set)
		{
			return;
		}
		this.Enabled = false;
		if (this.tweenGeneric != null)
		{
			this.tweenGeneric.tweenFactor = 0f;
			this.tweenGeneric.Sample(0f, false);
		}
	}

	// Token: 0x06008BDE RID: 35806 RVA: 0x0035398B File Offset: 0x00351B8B
	[XuiXmlAttribute("set_to_end", false)]
	public void AttributeSetToEnd(bool _set)
	{
		if (!_set)
		{
			return;
		}
		this.Enabled = false;
		if (this.tweenGeneric != null)
		{
			this.tweenGeneric.tweenFactor = 1f;
			this.tweenGeneric.Sample(1f, false);
		}
	}

	// Token: 0x06008BDF RID: 35807 RVA: 0x003539C7 File Offset: 0x00351BC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiTweenAbs(XUiView _targetView)
	{
		this.targetView = _targetView;
	}

	// Token: 0x06008BE0 RID: 35808 RVA: 0x003539F4 File Offset: 0x00351BF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void setCommonTweenValues(UITweener _tween)
	{
		this.tweenGeneric = _tween;
		_tween.style = this.repeatStyle;
		_tween.duration = this.duration;
		_tween.method = this.method;
		_tween.animationCurve = this.curve;
		_tween.enabled = this.enabled;
	}

	// Token: 0x06008BE1 RID: 35809
	public abstract void CreateTween(GameObject _uiGameObject);

	// Token: 0x17001025 RID: 4133
	// (get) Token: 0x06008BE2 RID: 35810 RVA: 0x00353A44 File Offset: 0x00351C44
	public XUiController Controller
	{
		get
		{
			return this.targetView.Controller;
		}
	}

	// Token: 0x06008BE3 RID: 35811 RVA: 0x00353A51 File Offset: 0x00351C51
	public string GetXuiHierarchy()
	{
		return XUiUtils.GetXuiHierarchy(this.targetView.Controller);
	}

	// Token: 0x06008BE4 RID: 35812 RVA: 0x00353A64 File Offset: 0x00351C64
	public void ParseInitialAttributeValue(string _attribute, string _value)
	{
		if (_value.Contains("{"))
		{
			new BindingInfoNcalc(this, _attribute, _value);
			return;
		}
		if (ParsingMethodCache.Instance.TryParseDirect(this, _attribute, _value))
		{
			return;
		}
		Log.Error("[XUi] Unknown Tween attribute '" + _attribute + "', hierarchy: " + this.GetXuiHierarchy());
	}

	// Token: 0x17001026 RID: 4134
	// (get) Token: 0x06008BE5 RID: 35813 RVA: 0x00353AB4 File Offset: 0x00351CB4
	public static AnimationCurve SecondHalfLinear
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 0f, 0f),
				new Keyframe(0.5f, 0f, 0f, 2f),
				new Keyframe(1f, 1f, 2f, 2f)
			});
		}
	}

	// Token: 0x06008BE6 RID: 35814 RVA: 0x00353B2C File Offset: 0x00351D2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public static AnimationCurve parseCurve(string _curveString)
	{
		AnimationCurve result;
		if (!(_curveString == "EaseInOut"))
		{
			if (!(_curveString == "Linear"))
			{
				if (!(_curveString == "SecondHalfLinear"))
				{
					throw new ArgumentOutOfRangeException();
				}
				result = XUiTweenAbs.SecondHalfLinear;
			}
			else
			{
				result = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}
		}
		else
		{
			result = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}
		return result;
	}

	// Token: 0x04006751 RID: 26449
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiView targetView;

	// Token: 0x04006752 RID: 26450
	[PublicizedFrom(EAccessModifier.Private)]
	public UITweener tweenGeneric;

	// Token: 0x04006753 RID: 26451
	[PublicizedFrom(EAccessModifier.Private)]
	public UITweener.Style repeatStyle;

	// Token: 0x04006754 RID: 26452
	[PublicizedFrom(EAccessModifier.Protected)]
	public float duration = 1f;

	// Token: 0x04006755 RID: 26453
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITweener.Method method;

	// Token: 0x04006756 RID: 26454
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool enabled;

	// Token: 0x04006757 RID: 26455
	[PublicizedFrom(EAccessModifier.Protected)]
	public AnimationCurve curve = XUiTweenAbs.parseCurve("Linear");

	// Token: 0x02001145 RID: 4421
	public enum ETweenType
	{
		// Token: 0x04006759 RID: 26457
		Alpha,
		// Token: 0x0400675A RID: 26458
		Color,
		// Token: 0x0400675B RID: 26459
		Fill,
		// Token: 0x0400675C RID: 26460
		Height,
		// Token: 0x0400675D RID: 26461
		Position,
		// Token: 0x0400675E RID: 26462
		Rotation,
		// Token: 0x0400675F RID: 26463
		Scale,
		// Token: 0x04006760 RID: 26464
		Width
	}
}
