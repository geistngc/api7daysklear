using System;
using System.Globalization;

// Token: 0x0200117F RID: 4479
public readonly struct XUiSideSizes
{
	// Token: 0x1700113C RID: 4412
	// (get) Token: 0x06008FA0 RID: 36768 RVA: 0x003601EA File Offset: 0x0035E3EA
	public int SumLeftRight
	{
		get
		{
			return this.Left + this.Right;
		}
	}

	// Token: 0x1700113D RID: 4413
	// (get) Token: 0x06008FA1 RID: 36769 RVA: 0x003601F9 File Offset: 0x0035E3F9
	public int SumTopBottom
	{
		get
		{
			return this.Top + this.Bottom;
		}
	}

	// Token: 0x06008FA2 RID: 36770 RVA: 0x00360208 File Offset: 0x0035E408
	public XUiSideSizes(int _left, int _right, int _top, int _bottom)
	{
		this.Left = _left;
		this.Right = _right;
		this.Top = _top;
		this.Bottom = _bottom;
	}

	// Token: 0x06008FA3 RID: 36771 RVA: 0x00360227 File Offset: 0x0035E427
	public XUiSideSizes SetLeft(int _value)
	{
		return new XUiSideSizes(_value, this.Right, this.Top, this.Bottom);
	}

	// Token: 0x06008FA4 RID: 36772 RVA: 0x00360241 File Offset: 0x0035E441
	public XUiSideSizes SetRight(int _value)
	{
		return new XUiSideSizes(this.Left, _value, this.Top, this.Bottom);
	}

	// Token: 0x06008FA5 RID: 36773 RVA: 0x0036025B File Offset: 0x0035E45B
	public XUiSideSizes SetTop(int _value)
	{
		return new XUiSideSizes(this.Left, this.Right, _value, this.Bottom);
	}

	// Token: 0x06008FA6 RID: 36774 RVA: 0x00360275 File Offset: 0x0035E475
	public XUiSideSizes SetBottom(int _value)
	{
		return new XUiSideSizes(this.Left, this.Right, this.Top, _value);
	}

	// Token: 0x06008FA7 RID: 36775 RVA: 0x00360290 File Offset: 0x0035E490
	public static bool TryParse(string _value, out XUiSideSizes _result, string _valueName)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_value, ',', 3, 0, -1);
		if (separatorPositions.TotalFound > 3)
		{
			Log.Warning(string.Format("[XUi] Invalid number of values for {0}: {1}, max of 4 expected (input string: '{2}')", _valueName, separatorPositions.TotalFound, _value));
			_result = default(XUiSideSizes);
			return false;
		}
		int num;
		if (!StringParsers.TryParseSInt32(_value, out num, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 1st value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 0)
		{
			_result = new XUiSideSizes(num, num, num, num);
			return true;
		}
		int num2;
		if (!StringParsers.TryParseSInt32(_value, out num2, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 2nd value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 1)
		{
			_result = new XUiSideSizes(num2, num2, num, num);
			return true;
		}
		int bottom;
		if (!StringParsers.TryParseSInt32(_value, out bottom, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 3rd value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 2)
		{
			_result = new XUiSideSizes(num2, num2, num, bottom);
			return true;
		}
		int left;
		if (!StringParsers.TryParseSInt32(_value, out left, separatorPositions.Sep3 + 1, separatorPositions.Sep4 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 4th value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		_result = new XUiSideSizes(left, num2, num, bottom);
		return true;
	}

	// Token: 0x0400691F RID: 26911
	public readonly int Left;

	// Token: 0x04006920 RID: 26912
	public readonly int Right;

	// Token: 0x04006921 RID: 26913
	public readonly int Top;

	// Token: 0x04006922 RID: 26914
	public readonly int Bottom;
}
