using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

// Token: 0x020013F1 RID: 5105
[MemoryPackable(GenerateType.Object)]
public class DynamicProperties : IMemoryPackable<DynamicProperties>, IMemoryPackFormatterRegister
{
	// Token: 0x0600A013 RID: 40979 RVA: 0x003C55DC File Offset: 0x003C37DC
	public Dictionary<string, string> ParseKeyData(string key)
	{
		try
		{
			string data;
			if (this.Data.TryGetValue(key, out data))
			{
				return DynamicProperties.ParseData(data);
			}
		}
		catch (Exception ex)
		{
			Log.Error("ParseKeyData error parsing key {0}, {1}", new object[]
			{
				key,
				ex
			});
		}
		return null;
	}

	// Token: 0x0600A014 RID: 40980 RVA: 0x003C5634 File Offset: 0x003C3834
	public static Dictionary<string, string> ParseData(string data)
	{
		Dictionary<string, string> dictionary = null;
		try
		{
			dictionary = new Dictionary<string, string>();
			if (data.IndexOf(';') < 0)
			{
				string[] array = data.Split(DynamicProperties.equalSeparator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2)
				{
					dictionary[array[0]] = array[1];
				}
			}
			else
			{
				string[] array2 = data.Split(DynamicProperties.semicolonSeparator, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Split(DynamicProperties.equalSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length >= 2)
					{
						dictionary[array3[0]] = array3[1];
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("ParseData error parsing {0}, {1}", new object[]
			{
				data,
				ex
			});
		}
		return dictionary;
	}

	// Token: 0x0600A016 RID: 40982 RVA: 0x003C573D File Offset: 0x003C393D
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateKey(string _key)
	{
		if (string.IsNullOrEmpty(_key) || _key.IndexOf('.') >= 0)
		{
			throw new Exception(string.Format("Property name '{0}' contains illegal character '{1}'", _key, '.'));
		}
	}

	// Token: 0x0600A017 RID: 40983 RVA: 0x003C5774 File Offset: 0x003C3974
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicProperties GetOrCreateClass(string _className)
	{
		DynamicProperties.ValidateKey(_className);
		DynamicProperties dynamicProperties;
		if (!this.Classes.TryGetValue(_className, out dynamicProperties))
		{
			dynamicProperties = new DynamicProperties();
			this.Classes[_className] = dynamicProperties;
		}
		return dynamicProperties;
	}

	// Token: 0x0600A018 RID: 40984 RVA: 0x003C57AC File Offset: 0x003C39AC
	public DynamicProperties GetClass(string _className)
	{
		DynamicProperties result;
		if (this.Classes.TryGetValue(_className, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600A019 RID: 40985 RVA: 0x003C57CC File Offset: 0x003C39CC
	public void SetValue(string _propName, string _value)
	{
		DynamicProperties.ValidateKey(_propName);
		this.Values[_propName] = _value;
	}

	// Token: 0x0600A01A RID: 40986 RVA: 0x003C57E1 File Offset: 0x003C39E1
	public void SetValue(string _className, string _propName, string _value)
	{
		this.GetOrCreateClass(_className).SetValue(_propName, _value);
	}

	// Token: 0x0600A01B RID: 40987 RVA: 0x003C57F4 File Offset: 0x003C39F4
	public string GetValue(string _propName)
	{
		string result;
		if (this.Values.TryGetValue(_propName, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600A01C RID: 40988 RVA: 0x003C5814 File Offset: 0x003C3A14
	public bool TryGetValue(string _propName, out string _value)
	{
		DynamicProperties.ValidateKey(_propName);
		return this.Values.TryGetValue(_propName, out _value);
	}

	// Token: 0x0600A01D RID: 40989 RVA: 0x003C582C File Offset: 0x003C3A2C
	public bool TryGetValue(string _className, string _propName, out string _value)
	{
		DynamicProperties.ValidateKey(_className);
		DynamicProperties dynamicProperties;
		if (!this.Classes.TryGetValue(_className, out dynamicProperties))
		{
			_value = null;
			return false;
		}
		return dynamicProperties.TryGetValue(_propName, out _value);
	}

	// Token: 0x0600A01E RID: 40990 RVA: 0x003C585C File Offset: 0x003C3A5C
	public bool Contains(string _propName)
	{
		DynamicProperties.ValidateKey(_propName);
		return this.Values.ContainsKey(_propName);
	}

	// Token: 0x0600A01F RID: 40991 RVA: 0x003C5870 File Offset: 0x003C3A70
	public bool Contains(string _className, string _propName)
	{
		string text;
		return this.TryGetValue(_className, _propName, out text);
	}

	// Token: 0x0600A020 RID: 40992 RVA: 0x003C5888 File Offset: 0x003C3A88
	public void SetParam1(string _propName, string _param1)
	{
		DynamicProperties.ValidateKey(_propName);
		if (!this.Values.ContainsKey(_propName))
		{
			this.Values.Add(_propName, null);
		}
		if (this.Params1.ContainsKey(_propName))
		{
			this.Params1[_propName] = _param1;
			return;
		}
		this.Params1.Add(_propName, _param1);
	}

	// Token: 0x0600A021 RID: 40993 RVA: 0x003C58DF File Offset: 0x003C3ADF
	public void SetParam1(string _className, string _propName, string _param1)
	{
		this.GetOrCreateClass(_className).SetParam1(_propName, _param1);
	}

	// Token: 0x0600A022 RID: 40994 RVA: 0x003C58EF File Offset: 0x003C3AEF
	public bool TryGetParam1(string _propName, out string _param1)
	{
		DynamicProperties.ValidateKey(_propName);
		return this.Params1.TryGetValue(_propName, out _param1);
	}

	// Token: 0x0600A023 RID: 40995 RVA: 0x003C5904 File Offset: 0x003C3B04
	public bool TryGetParam1(string _className, string _propName, out string _param1)
	{
		DynamicProperties.ValidateKey(_className);
		DynamicProperties dynamicProperties;
		if (!this.Classes.TryGetValue(_className, out dynamicProperties))
		{
			_param1 = null;
			return false;
		}
		return dynamicProperties.TryGetParam1(_propName, out _param1);
	}

	// Token: 0x0600A024 RID: 40996 RVA: 0x003C5934 File Offset: 0x003C3B34
	public string GetParam1(string _propName)
	{
		string result;
		if (this.Params1.TryGetValue(_propName, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600A025 RID: 40997 RVA: 0x003C5954 File Offset: 0x003C3B54
	public bool GetBool(string _propName)
	{
		string input;
		if (!this.TryGetValue(_propName, out input))
		{
			return false;
		}
		bool result;
		StringParsers.TryParseBool(input, out result);
		return result;
	}

	// Token: 0x0600A026 RID: 40998 RVA: 0x003C5978 File Offset: 0x003C3B78
	public bool GetBool(string _className, string _propName)
	{
		string input;
		if (!this.TryGetValue(_className, _propName, out input))
		{
			return false;
		}
		bool result;
		StringParsers.TryParseBool(input, out result);
		return result;
	}

	// Token: 0x0600A027 RID: 40999 RVA: 0x003C59A0 File Offset: 0x003C3BA0
	public float GetFloat(string _propName)
	{
		string input;
		if (!this.TryGetValue(_propName, out input))
		{
			return 0f;
		}
		float result;
		StringParsers.TryParseFloat(input, out result);
		return result;
	}

	// Token: 0x0600A028 RID: 41000 RVA: 0x003C59C8 File Offset: 0x003C3BC8
	public float GetFloat(string _className, string _propName)
	{
		string input;
		if (!this.TryGetValue(_className, _propName, out input))
		{
			return 0f;
		}
		float result;
		StringParsers.TryParseFloat(input, out result);
		return result;
	}

	// Token: 0x0600A029 RID: 41001 RVA: 0x003C59F4 File Offset: 0x003C3BF4
	public int GetInt(string _propName)
	{
		string s;
		if (!this.TryGetValue(_propName, out s))
		{
			return 0;
		}
		int result;
		int.TryParse(s, out result);
		return result;
	}

	// Token: 0x0600A02A RID: 41002 RVA: 0x003C5A18 File Offset: 0x003C3C18
	public int GetInt(string _className, string _propName)
	{
		string s;
		if (!this.TryGetValue(_className, _propName, out s))
		{
			return 0;
		}
		int result;
		int.TryParse(s, out result);
		return result;
	}

	// Token: 0x0600A02B RID: 41003 RVA: 0x003C5A40 File Offset: 0x003C3C40
	public string GetString(string _propName)
	{
		string result;
		if (!this.TryGetValue(_propName, out result))
		{
			return string.Empty;
		}
		return result;
	}

	// Token: 0x0600A02C RID: 41004 RVA: 0x003C5A60 File Offset: 0x003C3C60
	public string GetString(string _className, string _propName)
	{
		string result;
		if (!this.TryGetValue(_className, _propName, out result))
		{
			return string.Empty;
		}
		return result;
	}

	// Token: 0x0600A02D RID: 41005 RVA: 0x003C5A80 File Offset: 0x003C3C80
	public bool Load(string _directory, string _name)
	{
		try
		{
			foreach (XElement propertyNode in new XmlFile(_directory, _name, false, false).XmlDoc.Root.Elements(XNames.property))
			{
				this.Add(propertyNode, false);
			}
			return true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x0600A02E RID: 41006 RVA: 0x003C5B00 File Offset: 0x003C3D00
	public bool Save(string _rootNodeName, Stream stream)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.InsertBefore(newChild, xmlDocument.DocumentElement);
			XmlNode parent = xmlDocument.AppendChild(xmlDocument.CreateElement(_rootNodeName));
			this.toXml(xmlDocument, parent);
			xmlDocument.Save(stream);
			return true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x0600A02F RID: 41007 RVA: 0x003C5B70 File Offset: 0x003C3D70
	public bool Save(string _rootNodeName, string _path, string _name)
	{
		try
		{
			using (Stream stream = SdFile.Create(Path.Join(_path, _name + ".xml")))
			{
				return this.Save(_rootNodeName, stream);
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x0600A030 RID: 41008 RVA: 0x003C5BDC File Offset: 0x003C3DDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void toXml(XmlDocument _doc, XmlNode _parent)
	{
		foreach (KeyValuePair<string, string> keyValuePair in this.Values)
		{
			XmlElement xmlElement = _doc.CreateElement("property");
			XmlAttribute xmlAttribute = _doc.CreateAttribute("name");
			xmlAttribute.Value = keyValuePair.Key;
			xmlElement.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = _doc.CreateAttribute("value");
			xmlAttribute2.Value = this.Values[keyValuePair.Key];
			xmlElement.Attributes.Append(xmlAttribute2);
			if (this.Params1.ContainsKey(keyValuePair.Key))
			{
				XmlAttribute xmlAttribute3 = _doc.CreateAttribute("param1");
				xmlAttribute3.Value = this.Params1[keyValuePair.Key];
				xmlElement.Attributes.Append(xmlAttribute3);
			}
			if (this.Data.ContainsKey(keyValuePair.Key))
			{
				XmlAttribute xmlAttribute4 = _doc.CreateAttribute("fields");
				xmlAttribute4.Value = this.Data[keyValuePair.Key];
				xmlElement.Attributes.Append(xmlAttribute4);
			}
			_parent.AppendChild(xmlElement);
		}
		if (this.Classes.Count > 0)
		{
			foreach (KeyValuePair<string, DynamicProperties> keyValuePair2 in this.Classes)
			{
				XmlElement xmlElement2 = _doc.CreateElement("property");
				XmlAttribute xmlAttribute5 = _doc.CreateAttribute("class");
				xmlAttribute5.Value = keyValuePair2.Key;
				xmlElement2.Attributes.Append(xmlAttribute5);
				keyValuePair2.Value.toXml(_doc, xmlElement2);
				_parent.AppendChild(xmlElement2);
			}
		}
	}

	// Token: 0x0600A031 RID: 41009 RVA: 0x003C5DE4 File Offset: 0x003C3FE4
	public void Add(XElement _propertyNode, bool _doValueReplace = false)
	{
		this.Parse(_propertyNode, _doValueReplace);
	}

	// Token: 0x0600A032 RID: 41010 RVA: 0x003C5DF0 File Offset: 0x003C3FF0
	public void AddArray(XElement _arrayNode)
	{
		string attribute = _arrayNode.GetAttribute(XNames.name);
		if (!string.IsNullOrEmpty(attribute))
		{
			List<Dictionary<string, string>> list;
			if (!this.Array.TryGetValue(attribute, out list) || list == null)
			{
				list = new List<Dictionary<string, string>>();
				this.Array[attribute] = list;
			}
			list.Clear();
			foreach (XElement xelement in _arrayNode.Elements(XNames.item))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				list.Add(dictionary);
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					dictionary[xattribute.Name.ToString()] = xattribute.Value;
				}
			}
		}
	}

	// Token: 0x0600A033 RID: 41011 RVA: 0x003C5EE0 File Offset: 0x003C40E0
	public void Parse(XElement elementProperty, bool _doValueReplace = false)
	{
		if (elementProperty.HasAttribute(XNames.class_))
		{
			string attribute = elementProperty.GetAttribute(XNames.class_);
			DynamicProperties orCreateClass = this.GetOrCreateClass(attribute);
			using (IEnumerator<XElement> enumerator = elementProperty.Elements(XNames.property).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement elementProperty2 = enumerator.Current;
					orCreateClass.Parse(elementProperty2, _doValueReplace);
				}
				return;
			}
		}
		string attribute2 = elementProperty.GetAttribute(XNames.name);
		if (attribute2.Length == 0)
		{
			throw new Exception("Attribute 'name' missing on property");
		}
		DynamicProperties.ValidateKey(attribute2);
		string text = attribute2;
		string text2 = elementProperty.GetAttribute(XNames.value);
		if (_doValueReplace)
		{
			text2 = EntityClassesFromXml.ReplaceProperty(text2);
		}
		string text3 = null;
		if (elementProperty.HasAttribute(XNames.param1))
		{
			text3 = elementProperty.GetAttribute(XNames.param1);
		}
		string text4 = null;
		if (elementProperty.HasAttribute(XNames.param2))
		{
			text4 = elementProperty.GetAttribute(XNames.param2);
		}
		if (text3 != null)
		{
			this.Params1[text] = text3;
		}
		if (text4 != null)
		{
			this.Params2[text] = text4;
		}
		if (text2 != null)
		{
			string attribute3 = elementProperty.GetAttribute(XNames.data);
			if (attribute3.Length > 0)
			{
				this.Data[text] = attribute3;
			}
		}
		if (this.Classes.ContainsKey(text))
		{
			throw new Exception("Cannot create property '" + text + "': a class with the same name already exists. Property and class names must be unique.");
		}
		this.Values[text] = text2;
	}

	// Token: 0x0600A034 RID: 41012 RVA: 0x003C6060 File Offset: 0x003C4260
	public void Clear()
	{
		this.Values.Clear();
		this.Params1.Clear();
		this.Params2.Clear();
		this.Data.Clear();
		this.Classes.Clear();
	}

	// Token: 0x0600A035 RID: 41013 RVA: 0x003C609C File Offset: 0x003C429C
	public void ParseString(string _propName, ref string optionalValue)
	{
		string text;
		if (this.TryGetValue(_propName, out text))
		{
			optionalValue = text;
		}
	}

	// Token: 0x0600A036 RID: 41014 RVA: 0x003C60B8 File Offset: 0x003C42B8
	public void ParseString(string _className, string _propName, ref string optionalValue)
	{
		string text;
		if (this.TryGetValue(_className, _propName, out text))
		{
			optionalValue = text;
		}
	}

	// Token: 0x0600A037 RID: 41015 RVA: 0x003C60D4 File Offset: 0x003C42D4
	public void ParseStringFloatDictWithSubStringKey(string _propName, char _propSeparator, Dictionary<string, string> _dict, ref Dictionary<string, float> optionalValue)
	{
		foreach (KeyValuePair<string, string> keyValuePair in _dict)
		{
			if (keyValuePair.Key.StartsWith(_propName))
			{
				string text = keyValuePair.Key.Substring(keyValuePair.Key.IndexOf(_propSeparator) + 1);
				if (text.Length != keyValuePair.Key.Length)
				{
					optionalValue.Add(text, StringParsers.ParseFloat(keyValuePair.Value, 0, -1, NumberStyles.Any));
				}
			}
		}
	}

	// Token: 0x0600A038 RID: 41016 RVA: 0x003C6178 File Offset: 0x003C4378
	public IntRange TryParseRange(string _propName, IntRange defaultValue = default(IntRange))
	{
		IntRange result = defaultValue;
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			try
			{
				if (!StringParsers.TryParseRange(input, out result, null, '-'))
				{
					result = defaultValue;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
		return result;
	}

	// Token: 0x0600A039 RID: 41017 RVA: 0x003C61C4 File Offset: 0x003C43C4
	public FloatRange TryParseRange(string _propName, FloatRange defaultValue = default(FloatRange))
	{
		FloatRange result = defaultValue;
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			try
			{
				if (!StringParsers.TryParseRange(input, out result, null, '-'))
				{
					result = defaultValue;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
		return result;
	}

	// Token: 0x0600A03A RID: 41018 RVA: 0x003C6210 File Offset: 0x003C4410
	public string GetLocalizedString(string _propName)
	{
		string result;
		if (this.TryGetValue(_propName, out result))
		{
			return result;
		}
		return string.Empty;
	}

	// Token: 0x0600A03B RID: 41019 RVA: 0x003C6230 File Offset: 0x003C4430
	public void ParseLocalizedString(string _propName, ref string optionalValue)
	{
		string key;
		if (this.TryGetValue(_propName, out key))
		{
			optionalValue = Localization.Get(key, false, null);
		}
	}

	// Token: 0x0600A03C RID: 41020 RVA: 0x003C6254 File Offset: 0x003C4454
	public void ParseBool(string _propName, ref bool optionalValue)
	{
		string text;
		if (this.TryGetValue(_propName, out text))
		{
			bool flag;
			if (StringParsers.TryParseBool(text, out flag))
			{
				optionalValue = flag;
				return;
			}
			Log.Warning("Can't parse bool {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x0600A03D RID: 41021 RVA: 0x003C6294 File Offset: 0x003C4494
	public void ParseByte(string _propName, ref byte optionalValue)
	{
		string text;
		if (this.TryGetValue(_propName, out text))
		{
			byte b;
			if (StringParsers.TryParseUInt8(text, out b))
			{
				optionalValue = b;
				return;
			}
			Log.Warning("Can't parse byte {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x0600A03E RID: 41022 RVA: 0x003C62D4 File Offset: 0x003C44D4
	public void ParseColor(string _propName, ref Color optionalValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseColor32(input);
		}
	}

	// Token: 0x0600A03F RID: 41023 RVA: 0x003C62F8 File Offset: 0x003C44F8
	public void ParseColorHex(string _propName, ref Color optionalValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseHexColor(input);
		}
	}

	// Token: 0x0600A040 RID: 41024 RVA: 0x003C631C File Offset: 0x003C451C
	public void ParseEnum<T>(string _propName, ref T optionalValue) where T : struct, IConvertible
	{
		string name;
		T t;
		if (this.TryGetValue(_propName, out name) && EnumUtils.TryParse<T>(name, out t, true))
		{
			optionalValue = t;
		}
	}

	// Token: 0x0600A041 RID: 41025 RVA: 0x003C6348 File Offset: 0x003C4548
	public void ParseFloat(string _propName, ref float optionalValue)
	{
		string text;
		if (this.TryGetValue(_propName, out text))
		{
			float num;
			if (StringParsers.TryParseFloat(text, out num))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse float {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x0600A042 RID: 41026 RVA: 0x003C6388 File Offset: 0x003C4588
	public void ParseFloat(string _className, string _propName, ref float optionalValue)
	{
		string text;
		if (this.TryGetValue(_className, _propName, out text))
		{
			float num;
			if (StringParsers.TryParseFloat(text, out num))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse float '{0}'", new object[]
			{
				text
			});
		}
	}

	// Token: 0x0600A043 RID: 41027 RVA: 0x003C63C4 File Offset: 0x003C45C4
	public static void ParseFloat(XElement _e, string _propName, ref float optionalValue)
	{
		XAttribute xattribute = _e.Attribute(_propName);
		if (xattribute != null)
		{
			string value = xattribute.Value;
			float num;
			if (StringParsers.TryParseFloat(value, out num))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse float {0} '{1}'", new object[]
			{
				_propName,
				value
			});
		}
	}

	// Token: 0x0600A044 RID: 41028 RVA: 0x003C6410 File Offset: 0x003C4610
	public void ParseInt(string _propName, ref int optionalValue)
	{
		string text;
		if (this.TryGetValue(_propName, out text))
		{
			int num;
			if (StringParsers.TryParseSInt32(text, out num))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse int {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x0600A045 RID: 41029 RVA: 0x003C6450 File Offset: 0x003C4650
	public void ParseInt(string _className, string _propName, ref int optionalValue)
	{
		string text;
		if (this.TryGetValue(_className, _propName, out text))
		{
			int num;
			if (StringParsers.TryParseSInt32(text, out num))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse int {0}.{1} '{2}'", new object[]
			{
				_className,
				_propName,
				text
			});
		}
	}

	// Token: 0x0600A046 RID: 41030 RVA: 0x003C6494 File Offset: 0x003C4694
	public void ParseVec(string _propName, ref Vector2 optionalValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input);
		}
	}

	// Token: 0x0600A047 RID: 41031 RVA: 0x003C64B8 File Offset: 0x003C46B8
	public void ParseVec(string _className, string _propName, ref Vector2 optionalValue)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input);
		}
	}

	// Token: 0x0600A048 RID: 41032 RVA: 0x003C64E0 File Offset: 0x003C46E0
	public void ParseVec(string _propName, ref Vector2 optionalValue, float _defaultValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input, _defaultValue);
		}
	}

	// Token: 0x0600A049 RID: 41033 RVA: 0x003C6508 File Offset: 0x003C4708
	public void ParseVec(string _className, string _propName, ref Vector2 optionalValue, float _defaultValue)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input, _defaultValue);
		}
	}

	// Token: 0x0600A04A RID: 41034 RVA: 0x003C6530 File Offset: 0x003C4730
	public void ParseVec(string _propName, ref Vector3 optionalValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, 0, -1);
		}
	}

	// Token: 0x0600A04B RID: 41035 RVA: 0x003C6558 File Offset: 0x003C4758
	public void ParseVec(string _className, string _propName, ref Vector3 optionalValue)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, 0, -1);
		}
	}

	// Token: 0x0600A04C RID: 41036 RVA: 0x003C6580 File Offset: 0x003C4780
	public void ParseVec(string _propName, ref Vector3 optionalValue, float _defaultValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, _defaultValue);
		}
	}

	// Token: 0x0600A04D RID: 41037 RVA: 0x003C65A8 File Offset: 0x003C47A8
	public void ParseVec(string _className, string _propName, ref Vector3 optionalValue, float _defaultValue)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, _defaultValue);
		}
	}

	// Token: 0x0600A04E RID: 41038 RVA: 0x003C65D0 File Offset: 0x003C47D0
	public void ParseVec(string _propName, ref Vector3i optionalValue)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3i(input, 0, -1, false);
		}
	}

	// Token: 0x0600A04F RID: 41039 RVA: 0x003C65F8 File Offset: 0x003C47F8
	public void ParseVec(string _className, string _propName, ref Vector3i optionalValue)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			optionalValue = StringParsers.ParseVector3i(input, 0, -1, false);
		}
	}

	// Token: 0x0600A050 RID: 41040 RVA: 0x003C6620 File Offset: 0x003C4820
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			Vector2 vector = StringParsers.ParseVector2(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
		}
	}

	// Token: 0x0600A051 RID: 41041 RVA: 0x003C6650 File Offset: 0x003C4850
	public void ParseVec(string _className, string _propName, ref float optionalValue1, ref float optionalValue2)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			Vector2 vector = StringParsers.ParseVector2(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
		}
	}

	// Token: 0x0600A052 RID: 41042 RVA: 0x003C6684 File Offset: 0x003C4884
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			Vector3 vector = StringParsers.ParseVector3(input, 0, -1);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
		}
	}

	// Token: 0x0600A053 RID: 41043 RVA: 0x003C66C0 File Offset: 0x003C48C0
	public void ParseVec(string _className, string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			Vector3 vector = StringParsers.ParseVector3(input, 0, -1);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
		}
	}

	// Token: 0x0600A054 RID: 41044 RVA: 0x003C66FC File Offset: 0x003C48FC
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3, ref float optionalValue4)
	{
		string input;
		if (this.TryGetValue(_propName, out input))
		{
			Vector4 vector = StringParsers.ParseVector4(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
			optionalValue4 = vector.w;
		}
	}

	// Token: 0x0600A055 RID: 41045 RVA: 0x003C6740 File Offset: 0x003C4940
	public void ParseVec(string _className, string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3, ref float optionalValue4)
	{
		string input;
		if (this.TryGetValue(_className, _propName, out input))
		{
			Vector4 vector = StringParsers.ParseVector4(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
			optionalValue4 = vector.w;
		}
	}

	// Token: 0x0600A056 RID: 41046 RVA: 0x003C6784 File Offset: 0x003C4984
	public void CopyFrom(DynamicProperties _other, HashSet<string> _exclude = null)
	{
		DynamicProperties.copyDict(_other.Values, this.Values, _exclude);
		DynamicProperties.copyDict(_other.Params1, this.Params1, _exclude);
		DynamicProperties.copyDict(_other.Params2, this.Params2, _exclude);
		DynamicProperties.copyDict(_other.Data, this.Data, _exclude);
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair in _other.Classes)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair.Key))
			{
				DynamicProperties dynamicProperties = this.Classes.ContainsKey(keyValuePair.Key) ? this.Classes[keyValuePair.Key] : new DynamicProperties();
				this.Classes[keyValuePair.Key] = dynamicProperties;
				HashSet<string> nestedExclusions = DynamicProperties.GetNestedExclusions(keyValuePair.Key, _exclude);
				dynamicProperties.CopyFrom(keyValuePair.Value, nestedExclusions);
			}
		}
	}

	// Token: 0x0600A057 RID: 41047 RVA: 0x003C6888 File Offset: 0x003C4A88
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<string> GetNestedExclusions(string _className, HashSet<string> _exclude)
	{
		if (_exclude == null)
		{
			return null;
		}
		HashSet<string> hashSet = null;
		string text = _className + ".";
		foreach (string text2 in _exclude)
		{
			if (text2.StartsWith(text))
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<string>();
				}
				hashSet.Add(text2.Substring(text.Length));
			}
		}
		return hashSet;
	}

	// Token: 0x0600A058 RID: 41048 RVA: 0x003C6908 File Offset: 0x003C4B08
	[PublicizedFrom(EAccessModifier.Private)]
	public static void copyDict(Dictionary<string, string> _source, Dictionary<string, string> _dest, HashSet<string> _exclude)
	{
		foreach (KeyValuePair<string, string> keyValuePair in _source)
		{
			if (DynamicProperties.copyKey(keyValuePair.Key, _exclude))
			{
				_dest[keyValuePair.Key] = keyValuePair.Value;
			}
		}
	}

	// Token: 0x0600A059 RID: 41049 RVA: 0x003C6974 File Offset: 0x003C4B74
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool copyKey(string _key, HashSet<string> _exclude)
	{
		if (_exclude == null)
		{
			return true;
		}
		if (_exclude.Contains(_key))
		{
			return false;
		}
		if (_key.IndexOf('.') > 0)
		{
			foreach (string str in _exclude)
			{
				if (_key.StartsWith(str + "."))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x0600A05A RID: 41050 RVA: 0x003C69F0 File Offset: 0x003C4BF0
	public string PrettyPrint()
	{
		StringBuilder stringBuilder = new StringBuilder();
		this.PrettyPrint(stringBuilder, "");
		return stringBuilder.ToString();
	}

	// Token: 0x0600A05B RID: 41051 RVA: 0x003C6A18 File Offset: 0x003C4C18
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrettyPrint(StringBuilder sb, string indent)
	{
		sb.AppendFormat("{0}Properties:\n", indent);
		foreach (KeyValuePair<string, string> keyValuePair in from kvp in this.Values
		orderby kvp.Key
		select kvp)
		{
			sb.AppendFormat("{2}    name={0}, value={1}", keyValuePair.Key, this.Values[keyValuePair.Key], indent);
			if (this.Params1.ContainsKey(keyValuePair.Key))
			{
				sb.AppendFormat(", param1={0}", this.Params1[keyValuePair.Key]);
			}
			if (this.Params2.ContainsKey(keyValuePair.Key))
			{
				sb.AppendFormat(", param2={0}", this.Params2[keyValuePair.Key]);
			}
			if (this.Data.ContainsKey(keyValuePair.Key))
			{
				sb.AppendFormat(", fields={0}", this.Data[keyValuePair.Key]);
			}
			sb.AppendLine();
		}
		if (this.Classes.Count > 0)
		{
			sb.AppendFormat("{0}Classes:\n", indent);
			foreach (KeyValuePair<string, DynamicProperties> keyValuePair2 in this.Classes)
			{
				sb.AppendFormat("{1}    class={0}\n", keyValuePair2.Key, indent);
				keyValuePair2.Value.PrettyPrint(sb, string.Format("{0}    ", indent));
			}
		}
	}

	// Token: 0x0600A05C RID: 41052 RVA: 0x003C6BDC File Offset: 0x003C4DDC
	[PublicizedFrom(EAccessModifier.Private)]
	static DynamicProperties()
	{
		DynamicProperties.RegisterFormatter();
	}

	// Token: 0x0600A05D RID: 41053 RVA: 0x003C6C04 File Offset: 0x003C4E04
	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicProperties>())
		{
			MemoryPackFormatterProvider.Register<DynamicProperties>(new DynamicProperties.DynamicPropertiesFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicProperties[]>())
		{
			MemoryPackFormatterProvider.Register<DynamicProperties[]>(new ArrayFormatter<DynamicProperties>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<string, string>>())
		{
			MemoryPackFormatterProvider.Register<Dictionary<string, string>>(new DictionaryFormatter<string, string>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<string, DynamicProperties>>())
		{
			MemoryPackFormatterProvider.Register<Dictionary<string, DynamicProperties>>(new DictionaryFormatter<string, DynamicProperties>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<string, List<Dictionary<string, string>>>>())
		{
			MemoryPackFormatterProvider.Register<Dictionary<string, List<Dictionary<string, string>>>>(new DictionaryFormatter<string, List<Dictionary<string, string>>>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<List<Dictionary<string, string>>>())
		{
			MemoryPackFormatterProvider.Register<List<Dictionary<string, string>>>(new ListFormatter<Dictionary<string, string>>());
		}
	}

	// Token: 0x0600A05E RID: 41054 RVA: 0x003C6C78 File Offset: 0x003C4E78
	[NullableContext(2)]
	[Preserve]
	public static void Serialize(ref MemoryPackWriter writer, ref DynamicProperties value)
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(6);
		writer.WriteValue<Dictionary<string, string>>(value.Values);
		writer.WriteValue<Dictionary<string, string>>(value.Params1);
		writer.WriteValue<Dictionary<string, string>>(value.Params2);
		writer.WriteValue<Dictionary<string, string>>(value.Data);
		writer.WriteValue<Dictionary<string, DynamicProperties>>(value.Classes);
		writer.WriteValue<Dictionary<string, List<Dictionary<string, string>>>>(value.Array);
	}

	// Token: 0x0600A05F RID: 41055 RVA: 0x003C6CE8 File Offset: 0x003C4EE8
	[NullableContext(2)]
	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DynamicProperties value)
	{
		byte b;
		if (!reader.TryReadObjectHeader(out b))
		{
			value = null;
			return;
		}
		Dictionary<string, string> values;
		Dictionary<string, string> @params;
		Dictionary<string, string> params2;
		Dictionary<string, string> data;
		Dictionary<string, DynamicProperties> classes;
		Dictionary<string, List<Dictionary<string, string>>> array;
		if (b == 6)
		{
			if (value == null)
			{
				values = reader.ReadValue<Dictionary<string, string>>();
				@params = reader.ReadValue<Dictionary<string, string>>();
				params2 = reader.ReadValue<Dictionary<string, string>>();
				data = reader.ReadValue<Dictionary<string, string>>();
				classes = reader.ReadValue<Dictionary<string, DynamicProperties>>();
				array = reader.ReadValue<Dictionary<string, List<Dictionary<string, string>>>>();
				goto IL_194;
			}
			values = value.Values;
			@params = value.Params1;
			params2 = value.Params2;
			data = value.Data;
			classes = value.Classes;
			array = value.Array;
			reader.ReadValue<Dictionary<string, string>>(ref values);
			reader.ReadValue<Dictionary<string, string>>(ref @params);
			reader.ReadValue<Dictionary<string, string>>(ref params2);
			reader.ReadValue<Dictionary<string, string>>(ref data);
			reader.ReadValue<Dictionary<string, DynamicProperties>>(ref classes);
			reader.ReadValue<Dictionary<string, List<Dictionary<string, string>>>>(ref array);
		}
		else
		{
			if (b > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DynamicProperties), 6, b);
				return;
			}
			if (value == null)
			{
				values = null;
				@params = null;
				params2 = null;
				data = null;
				classes = null;
				array = null;
			}
			else
			{
				values = value.Values;
				@params = value.Params1;
				params2 = value.Params2;
				data = value.Data;
				classes = value.Classes;
				array = value.Array;
			}
			if (b != 0)
			{
				reader.ReadValue<Dictionary<string, string>>(ref values);
				if (b != 1)
				{
					reader.ReadValue<Dictionary<string, string>>(ref @params);
					if (b != 2)
					{
						reader.ReadValue<Dictionary<string, string>>(ref params2);
						if (b != 3)
						{
							reader.ReadValue<Dictionary<string, string>>(ref data);
							if (b != 4)
							{
								reader.ReadValue<Dictionary<string, DynamicProperties>>(ref classes);
								if (b != 5)
								{
									reader.ReadValue<Dictionary<string, List<Dictionary<string, string>>>>(ref array);
								}
							}
						}
					}
				}
			}
			if (value == null)
			{
				goto IL_194;
			}
		}
		value.Values = values;
		value.Params1 = @params;
		value.Params2 = params2;
		value.Data = data;
		value.Classes = classes;
		value.Array = array;
		return;
		IL_194:
		value = new DynamicProperties
		{
			Values = values,
			Params1 = @params,
			Params2 = params2,
			Data = data,
			Classes = classes,
			Array = array
		};
	}

	// Token: 0x04007955 RID: 31061
	[MemoryPackInclude]
	public Dictionary<string, string> Values = new Dictionary<string, string>();

	// Token: 0x04007956 RID: 31062
	[MemoryPackInclude]
	public Dictionary<string, string> Params1 = new Dictionary<string, string>();

	// Token: 0x04007957 RID: 31063
	[MemoryPackInclude]
	public Dictionary<string, string> Params2 = new Dictionary<string, string>();

	// Token: 0x04007958 RID: 31064
	[MemoryPackInclude]
	public Dictionary<string, string> Data = new Dictionary<string, string>();

	// Token: 0x04007959 RID: 31065
	[MemoryPackInclude]
	public Dictionary<string, DynamicProperties> Classes = new Dictionary<string, DynamicProperties>();

	// Token: 0x0400795A RID: 31066
	[MemoryPackInclude]
	public Dictionary<string, List<Dictionary<string, string>>> Array = new Dictionary<string, List<Dictionary<string, string>>>();

	// Token: 0x0400795B RID: 31067
	[PublicizedFrom(EAccessModifier.Private)]
	public const char PathAccessSeparator = '.';

	// Token: 0x0400795C RID: 31068
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] semicolonSeparator = new char[]
	{
		';'
	};

	// Token: 0x0400795D RID: 31069
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] equalSeparator = new char[]
	{
		'='
	};

	// Token: 0x020013F2 RID: 5106
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DynamicPropertiesFormatter : MemoryPackFormatter<DynamicProperties>
	{
		// Token: 0x0600A060 RID: 41056 RVA: 0x003C6EBD File Offset: 0x003C50BD
		[Preserve]
		public override void Serialize(ref MemoryPackWriter writer, ref DynamicProperties value)
		{
			DynamicProperties.Serialize(ref writer, ref value);
		}

		// Token: 0x0600A061 RID: 41057 RVA: 0x003C6EC6 File Offset: 0x003C50C6
		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DynamicProperties value)
		{
			DynamicProperties.Deserialize(ref reader, ref value);
		}
	}
}
