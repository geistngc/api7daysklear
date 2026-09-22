using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.WpfDesign.XamlDom;
using XMLData.Exceptions;

namespace XMLData
{
	// Token: 0x02001638 RID: 5688
	public static class ParserUtils
	{
		// Token: 0x0600B28E RID: 45710 RVA: 0x0042B0F4 File Offset: 0x004292F4
		[PublicizedFrom(EAccessModifier.Private)]
		static ParserUtils()
		{
			ParserUtils.ci.NumberFormat.CurrencyDecimalSeparator = ".";
			ParserUtils.ci.NumberFormat.NumberDecimalSeparator = ".";
			ParserUtils.ci.NumberFormat.NumberGroupSeparator = ",";
		}

		// Token: 0x0600B28F RID: 45711 RVA: 0x0042B151 File Offset: 0x00429351
		public static bool TryParseFloat(string _s, out float _result)
		{
			return float.TryParse(_s, NumberStyles.Any, ParserUtils.ci, out _result);
		}

		// Token: 0x0600B290 RID: 45712 RVA: 0x0042B164 File Offset: 0x00429364
		public static bool TryParseDouble(string _s, out double _result)
		{
			return double.TryParse(_s, NumberStyles.Any, ParserUtils.ci, out _result);
		}

		// Token: 0x0600B291 RID: 45713 RVA: 0x0042B178 File Offset: 0x00429378
		public static bool ParseBoolAttribute(PositionXmlElement _elem, string _attrName, bool _mandatory, bool _defaultValue)
		{
			if (_elem.HasAttribute(_attrName))
			{
				string attribute = _elem.GetAttribute(_attrName);
				if (attribute == "true")
				{
					return true;
				}
				if (attribute == "false")
				{
					return false;
				}
				throw new InvalidValueException(string.Concat(new string[]
				{
					"Element has invalid value \"",
					attribute,
					"\" for bool attribute \"",
					_attrName,
					"\""
				}), _elem.LineNumber);
			}
			else
			{
				if (_mandatory)
				{
					throw new MissingAttributeException("Element \"\" + _elem.Name + \"\" is missing required attribute \"" + _attrName + "\"", _elem.LineNumber);
				}
				return _defaultValue;
			}
		}

		// Token: 0x0600B292 RID: 45714 RVA: 0x0042B20C File Offset: 0x0042940C
		public static string ParseStringAttribute(PositionXmlElement _elem, string _attrName, bool _mandatory, string _defaultValue = null)
		{
			if (_elem.HasAttribute(_attrName))
			{
				return _elem.GetAttribute(_attrName);
			}
			if (_mandatory)
			{
				throw new MissingAttributeException(string.Concat(new string[]
				{
					"Element \"",
					_elem.Name,
					"\" is missing required attribute \"",
					_attrName,
					"\""
				}), _elem.LineNumber);
			}
			return _defaultValue;
		}

		// Token: 0x0600B293 RID: 45715 RVA: 0x0042B26C File Offset: 0x0042946C
		public static string ParseStringAttribute(XElement _elem, string _attrName, bool _mandatory, string _defaultValue = null)
		{
			if (_elem.HasAttribute(_attrName))
			{
				return _elem.GetAttribute(_attrName);
			}
			if (_mandatory)
			{
				string[] array = new string[5];
				array[0] = "Element \"";
				int num = 1;
				XName name = _elem.Name;
				array[num] = ((name != null) ? name.ToString() : null);
				array[2] = "\" is missing required attribute \"";
				array[3] = _attrName;
				array[4] = "\"";
				throw new MissingAttributeException(string.Concat(array), ((IXmlLineInfo)_elem).LineNumber);
			}
			return _defaultValue;
		}

		// Token: 0x0600B294 RID: 45716 RVA: 0x0042B2E0 File Offset: 0x004294E0
		public static int ParseIntAttribute(PositionXmlElement _elem, string _attrName, bool _mandatory, int _defaultValue = 0)
		{
			if (_elem.HasAttribute(_attrName))
			{
				int result;
				if (int.TryParse(_elem.GetAttribute(_attrName), out result))
				{
					return result;
				}
				throw new InvalidValueException(string.Concat(new string[]
				{
					"Element has invalid value \"",
					result.ToString(),
					"\" for int attribute \"",
					_attrName,
					"\""
				}), _elem.LineNumber);
			}
			else
			{
				if (_mandatory)
				{
					throw new MissingAttributeException("Element \"\" + _elem.Name + \"\" is missing required attribute \"" + _attrName + "\"", _elem.LineNumber);
				}
				return _defaultValue;
			}
		}

		// Token: 0x0600B295 RID: 45717 RVA: 0x0042B368 File Offset: 0x00429568
		public static void ParseRangeAttribute<T>(PositionXmlElement _elem, string _attrName, bool _mandatory, ref Range<T> _defaultAndOutput, bool _allowFixedValue, ParserUtils.ConverterDelegate<T> _converter)
		{
			if (!_elem.HasAttribute(_attrName))
			{
				if (_mandatory)
				{
					throw new MissingAttributeException("Element \"\" + _elem.Name + \"\" is missing required attribute \"" + _attrName + "\"", _elem.LineNumber);
				}
				return;
			}
			else
			{
				string attribute = _elem.GetAttribute(_attrName);
				string[] array = attribute.Split('-', StringSplitOptions.None);
				Range<T> range = new Range<T>();
				if (array.Length > 2)
				{
					throw new InvalidValueException(string.Concat(new string[]
					{
						"Element has invalid value \"",
						attribute,
						"\" for range attribute \"",
						_attrName,
						"\" with more than one separator \"-\""
					}), _elem.LineNumber);
				}
				if (!_allowFixedValue && array.Length < 2)
				{
					throw new InvalidValueException(string.Concat(new string[]
					{
						"Element has invalid value \"",
						attribute,
						"\" for range attribute \"",
						_attrName,
						"\" with less than one separator \"-\""
					}), _elem.LineNumber);
				}
				if (array[0] != "*")
				{
					T min;
					if (!_converter(array[0], out min))
					{
						throw new InvalidValueException(string.Concat(new string[]
						{
							"Element has invalid min range value \"",
							array[0],
							"\" for attribute \"",
							_attrName,
							"\""
						}), _elem.LineNumber);
					}
					range.min = min;
					range.hasMin = true;
				}
				if (array.Length > 1)
				{
					if (array[1] != "*")
					{
						T max;
						if (!_converter(array[1], out max))
						{
							throw new InvalidValueException(string.Concat(new string[]
							{
								"Element has invalid max range value \"",
								array[1],
								"\" for attribute \"",
								_attrName,
								"\""
							}), _elem.LineNumber);
						}
						range.max = max;
						range.hasMax = true;
					}
				}
				else
				{
					range.hasMax = range.hasMin;
					range.max = range.min;
				}
				_defaultAndOutput = range;
				return;
			}
		}

		// Token: 0x04008677 RID: 34423
		public static readonly CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

		// Token: 0x02001639 RID: 5689
		// (Invoke) Token: 0x0600B297 RID: 45719
		public delegate bool ConverterDelegate<T>(string _in, out T _out);
	}
}
