using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

// Token: 0x020013F8 RID: 5112
public static class EnumUtils
{
	// Token: 0x0600A078 RID: 41080 RVA: 0x003C741F File Offset: 0x003C561F
	public static string ToStringCached<TEnum>(this TEnum _enumValue) where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.GetName(_enumValue);
	}

	// Token: 0x0600A079 RID: 41081 RVA: 0x003C742C File Offset: 0x003C562C
	public static int Ordinal<TEnum>(this TEnum _enumValue) where TEnum : struct, IConvertible
	{
		return EnumInt32ToInt.Convert<TEnum>(_enumValue);
	}

	// Token: 0x0600A07A RID: 41082 RVA: 0x003C7434 File Offset: 0x003C5634
	public static bool TryFromOrdinal<TEnum>(int _ordinal, out TEnum _result) where TEnum : struct, IConvertible
	{
		IList<TEnum> list = EnumUtils.Values<TEnum>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Ordinal<TEnum>() == _ordinal)
			{
				_result = list[i];
				return true;
			}
		}
		_result = default(TEnum);
		return false;
	}

	// Token: 0x0600A07B RID: 41083 RVA: 0x003C7480 File Offset: 0x003C5680
	public static TEnum Parse<TEnum>(string _name, TEnum _default, bool _ignoreCase = false) where TEnum : struct, IConvertible
	{
		TEnum result;
		if (!EnumUtils.TryParse<TEnum>(_name, out result, _ignoreCase))
		{
			result = _default;
		}
		return result;
	}

	// Token: 0x0600A07C RID: 41084 RVA: 0x003C749B File Offset: 0x003C569B
	public static TEnum Parse<TEnum>(string _name, bool _ignoreCase = false) where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.Parse(_name, _ignoreCase);
	}

	// Token: 0x0600A07D RID: 41085 RVA: 0x003C74A9 File Offset: 0x003C56A9
	public static bool TryParse<TEnum>(string _name, out TEnum _result, bool _ignoreCase = false) where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.TryParse(_name, out _result, _ignoreCase);
	}

	// Token: 0x0600A07E RID: 41086 RVA: 0x003C74B8 File Offset: 0x003C56B8
	public static bool TryParseIgnoreCase<TEnum>(string _name, out TEnum _result) where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.TryParse(_name, out _result, true);
	}

	// Token: 0x0600A07F RID: 41087 RVA: 0x003C74C7 File Offset: 0x003C56C7
	public static bool HasName<TEnum>(string _name, bool _ignoreCase = false) where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.HasName(_name, _ignoreCase);
	}

	// Token: 0x0600A080 RID: 41088 RVA: 0x003C74D5 File Offset: 0x003C56D5
	public static IList<TEnum> Values<TEnum>() where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues;
	}

	// Token: 0x0600A081 RID: 41089 RVA: 0x003C74E1 File Offset: 0x003C56E1
	public static IList<string> Names<TEnum>() where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.EnumNames;
	}

	// Token: 0x0600A082 RID: 41090 RVA: 0x003C74F0 File Offset: 0x003C56F0
	public static TEnum CycleEnum<TEnum>(this TEnum _enumVal, bool _reverse = false, bool _wrap = true) where TEnum : struct, IConvertible
	{
		if (!typeof(TEnum).IsEnum)
		{
			throw new ArgumentException("Argument " + typeof(TEnum).FullName + " is not an Enum");
		}
		IList<TEnum> enumValues = EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues;
		int num = enumValues.IndexOf(_enumVal) + (_reverse ? -1 : 1);
		if (num >= enumValues.Count)
		{
			num = (_wrap ? 0 : (enumValues.Count - 1));
		}
		else if (num < 0)
		{
			num = (_wrap ? (enumValues.Count - 1) : 0);
		}
		return enumValues[num];
	}

	// Token: 0x0600A083 RID: 41091 RVA: 0x003C7584 File Offset: 0x003C5784
	public static TEnum CycleEnum<TEnum>(this TEnum _enumVal, TEnum _minVal, TEnum _maxVal, bool _reverse = false, bool _wrap = true) where TEnum : struct, IConvertible
	{
		if (!typeof(TEnum).IsEnum)
		{
			throw new ArgumentException("Argument " + typeof(TEnum).FullName + " is not an Enum");
		}
		IList<TEnum> enumValues = EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues;
		int num = enumValues.IndexOf(_minVal);
		if (num < 0)
		{
			throw new ArgumentException(string.Format("Could not find index of {0}", _minVal), "_minVal");
		}
		int num2 = enumValues.IndexOf(_maxVal);
		if (num2 < 0)
		{
			throw new ArgumentException(string.Format("Could not find index of {0}", _maxVal), "_maxVal");
		}
		if (num2 < num)
		{
			throw new ArgumentException(string.Format("Max of {0} with index {1} is less than min of {2} with index {3}", new object[]
			{
				_maxVal,
				num2,
				_minVal,
				num
			}));
		}
		int num3 = enumValues.IndexOf(_enumVal);
		if (num3 < 0)
		{
			Log.Warning(string.Format("Could not find index of {0}: {1} (using min)", "_enumVal", _enumVal));
			return enumValues[num];
		}
		int num4 = num2 - num + 1;
		if (num4 <= 1)
		{
			return enumValues[num];
		}
		int num5 = num3 - num + (_reverse ? -1 : 1);
		if (_wrap)
		{
			num5 %= num4;
			if (num5 < 0)
			{
				num5 += num4;
			}
		}
		else if (num5 < 0)
		{
			num5 = 0;
		}
		else if (num5 >= num4)
		{
			num5 = num4 - 1;
		}
		int index = num + num5;
		return enumValues[index];
	}

	// Token: 0x0600A084 RID: 41092 RVA: 0x003C76E9 File Offset: 0x003C58E9
	public static TEnum MaxValue<TEnum>() where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues[EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues.Count - 1];
	}

	// Token: 0x0600A085 RID: 41093 RVA: 0x003C770B File Offset: 0x003C590B
	public static TEnum MinValue<TEnum>() where TEnum : struct, IConvertible
	{
		return EnumUtils.EnumInfoCache<TEnum>.Instance.EnumValues[0];
	}

	// Token: 0x020013F9 RID: 5113
	[PublicizedFrom(EAccessModifier.Private)]
	public class EnumInfoCache<TEnum> where TEnum : struct, IConvertible
	{
		// Token: 0x170012E3 RID: 4835
		// (get) Token: 0x0600A086 RID: 41094 RVA: 0x003C771D File Offset: 0x003C591D
		public static EnumUtils.EnumInfoCache<TEnum> Instance
		{
			get
			{
				if (EnumUtils.EnumInfoCache<TEnum>.instance == null)
				{
					EnumUtils.EnumInfoCache<TEnum>.instance = new EnumUtils.EnumInfoCache<TEnum>();
				}
				return EnumUtils.EnumInfoCache<TEnum>.instance;
			}
		}

		// Token: 0x0600A087 RID: 41095 RVA: 0x003C7738 File Offset: 0x003C5938
		[PublicizedFrom(EAccessModifier.Private)]
		public EnumInfoCache()
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw new NotSupportedException(typeof(TEnum).FullName + " is not an enum type.");
			}
			object[] customAttributes = typeof(TEnum).GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (((Attribute)customAttributes[i]).GetType().Name == "FlagsAttribute")
				{
					this.isFlags = true;
					break;
				}
			}
			Array values = Enum.GetValues(typeof(TEnum));
			this.enumValues = new List<TEnum>(values.Length);
			this.enumToName = new EnumDictionary<TEnum, string>(values.Length);
			foreach (object obj in values)
			{
				TEnum tenum = (TEnum)((object)obj);
				string value = tenum.ToString(CultureInfo.InvariantCulture);
				if (!this.enumValues.Contains(tenum))
				{
					this.enumValues.Add(tenum);
				}
				if (!this.enumToName.ContainsKey(tenum))
				{
					this.enumToName.Add(tenum, value);
				}
			}
			this.enumValues.Sort((TEnum _enumA, TEnum _enumB) => _enumA.Ordinal<TEnum>().CompareTo(_enumB.Ordinal<TEnum>()));
			string[] names = Enum.GetNames(typeof(TEnum));
			this.enumNames = new List<string>(names.Length);
			this.nameToEnumCaseSensitive = new Dictionary<string, TEnum>(names.Length, StringComparer.Ordinal);
			this.nameToEnumCaseInsensitive = new CaseInsensitiveStringDictionary<TEnum>(names.Length);
			foreach (string text in names)
			{
				TEnum value2 = (TEnum)((object)Enum.Parse(typeof(TEnum), text));
				if (!this.enumNames.Contains(text))
				{
					this.enumNames.Add(text);
				}
				if (!this.nameToEnumCaseSensitive.ContainsKey(text))
				{
					this.nameToEnumCaseSensitive.Add(text, value2);
				}
				if (!this.nameToEnumCaseInsensitive.ContainsKey(text))
				{
					this.nameToEnumCaseInsensitive.Add(text, value2);
				}
			}
			this.EnumValues = new ReadOnlyCollection<TEnum>(this.enumValues);
			this.EnumNames = new ReadOnlyCollection<string>(this.enumNames);
		}

		// Token: 0x0600A088 RID: 41096 RVA: 0x003C79A0 File Offset: 0x003C5BA0
		public string GetName(TEnum _enumValue)
		{
			if (this.isFlags)
			{
				if (!this.enumToName.ContainsKey(_enumValue))
				{
					this.enumToName.Add(_enumValue, _enumValue.ToString(CultureInfo.InvariantCulture));
				}
				return this.enumToName[_enumValue];
			}
			if (this.enumToName.ContainsKey(_enumValue))
			{
				return this.enumToName[_enumValue];
			}
			return _enumValue.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x0600A089 RID: 41097 RVA: 0x003C7A1C File Offset: 0x003C5C1C
		public TEnum Parse(string _name, bool _ignoreCase)
		{
			if (string.IsNullOrEmpty(_name))
			{
				throw new ArgumentException("Value null or empty", "_name");
			}
			TEnum result;
			if ((_ignoreCase ? this.nameToEnumCaseInsensitive : this.nameToEnumCaseSensitive).TryGetValue(_name, out result))
			{
				return result;
			}
			TEnum tenum = (TEnum)((object)Enum.Parse(typeof(TEnum), _name, _ignoreCase));
			this.nameToEnumCaseSensitive.Add(_name, tenum);
			this.nameToEnumCaseInsensitive.Add(_name, tenum);
			return tenum;
		}

		// Token: 0x0600A08A RID: 41098 RVA: 0x003C7A90 File Offset: 0x003C5C90
		public bool TryParse(string _name, out TEnum _result, bool _ignoreCase)
		{
			_result = default(TEnum);
			if (string.IsNullOrEmpty(_name))
			{
				return false;
			}
			if ((_ignoreCase ? this.nameToEnumCaseInsensitive : this.nameToEnumCaseSensitive).TryGetValue(_name, out _result))
			{
				return true;
			}
			bool result;
			try
			{
				_result = (TEnum)((object)Enum.Parse(typeof(TEnum), _name, _ignoreCase));
				this.nameToEnumCaseSensitive.Add(_name, _result);
				this.nameToEnumCaseInsensitive.Add(_name, _result);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600A08B RID: 41099 RVA: 0x003C7B28 File Offset: 0x003C5D28
		public bool HasName(string _name, bool _ignoreCase)
		{
			if (string.IsNullOrEmpty(_name))
			{
				throw new ArgumentException("Value null or empty", "_name");
			}
			return (_ignoreCase ? this.nameToEnumCaseInsensitive : this.nameToEnumCaseSensitive).ContainsKey(_name);
		}

		// Token: 0x0400796F RID: 31087
		[PublicizedFrom(EAccessModifier.Private)]
		public static EnumUtils.EnumInfoCache<TEnum> instance;

		// Token: 0x04007970 RID: 31088
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<TEnum> enumValues;

		// Token: 0x04007971 RID: 31089
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<string> enumNames;

		// Token: 0x04007972 RID: 31090
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<TEnum, string> enumToName;

		// Token: 0x04007973 RID: 31091
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, TEnum> nameToEnumCaseSensitive;

		// Token: 0x04007974 RID: 31092
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, TEnum> nameToEnumCaseInsensitive;

		// Token: 0x04007975 RID: 31093
		public readonly ReadOnlyCollection<TEnum> EnumValues;

		// Token: 0x04007976 RID: 31094
		public readonly ReadOnlyCollection<string> EnumNames;

		// Token: 0x04007977 RID: 31095
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool isFlags;
	}
}
