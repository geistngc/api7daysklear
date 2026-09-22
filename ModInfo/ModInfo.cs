using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.WpfDesign.XamlDom;
using XMLData;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace ModInfo
{
	// Token: 0x02001625 RID: 5669
	public class ModInfo : IXMLData
	{
		// Token: 0x170014E4 RID: 5348
		// (get) Token: 0x0600B234 RID: 45620 RVA: 0x0042A4C4 File Offset: 0x004286C4
		// (set) Token: 0x0600B235 RID: 45621 RVA: 0x0042A4CC File Offset: 0x004286CC
		public DataItem<string> Name
		{
			get
			{
				return this.pName;
			}
			set
			{
				this.pName = value;
			}
		}

		// Token: 0x170014E5 RID: 5349
		// (get) Token: 0x0600B236 RID: 45622 RVA: 0x0042A4D5 File Offset: 0x004286D5
		// (set) Token: 0x0600B237 RID: 45623 RVA: 0x0042A4DD File Offset: 0x004286DD
		public DataItem<string> Description
		{
			get
			{
				return this.pDescription;
			}
			set
			{
				this.pDescription = value;
			}
		}

		// Token: 0x170014E6 RID: 5350
		// (get) Token: 0x0600B238 RID: 45624 RVA: 0x0042A4E6 File Offset: 0x004286E6
		// (set) Token: 0x0600B239 RID: 45625 RVA: 0x0042A4EE File Offset: 0x004286EE
		public DataItem<string> Author
		{
			get
			{
				return this.pAuthor;
			}
			set
			{
				this.pAuthor = value;
			}
		}

		// Token: 0x170014E7 RID: 5351
		// (get) Token: 0x0600B23A RID: 45626 RVA: 0x0042A4F7 File Offset: 0x004286F7
		// (set) Token: 0x0600B23B RID: 45627 RVA: 0x0042A4FF File Offset: 0x004286FF
		public DataItem<string> Version
		{
			get
			{
				return this.pVersion;
			}
			set
			{
				this.pVersion = value;
			}
		}

		// Token: 0x170014E8 RID: 5352
		// (get) Token: 0x0600B23C RID: 45628 RVA: 0x0042A508 File Offset: 0x00428708
		// (set) Token: 0x0600B23D RID: 45629 RVA: 0x0042A510 File Offset: 0x00428710
		public DataItem<string> Website
		{
			get
			{
				return this.pWebsite;
			}
			set
			{
				this.pWebsite = value;
			}
		}

		// Token: 0x0400864B RID: 34379
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pName;

		// Token: 0x0400864C RID: 34380
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDescription;

		// Token: 0x0400864D RID: 34381
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAuthor;

		// Token: 0x0400864E RID: 34382
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pVersion;

		// Token: 0x0400864F RID: 34383
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pWebsite;

		// Token: 0x02001626 RID: 5670
		public static class Parser
		{
			// Token: 0x0600B23F RID: 45631 RVA: 0x0042A51C File Offset: 0x0042871C
			public static ModInfo Parse(XElement _elem, Dictionary<PositionXmlElement, DataItem<ModInfo>> _updateLater)
			{
				ModInfo modInfo = new ModInfo();
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (XElement xelement in _elem.Elements())
				{
					if (!ModInfo.Parser.knownAttributesMultiplicity.ContainsKey(xelement.Name.LocalName))
					{
						string str = "Unknown element \"";
						XName name = xelement.Name;
						throw new UnexpectedElementException(str + ((name != null) ? name.ToString() : null) + "\" found while parsing ModInfo", ((IXmlLineInfo)xelement).LineNumber);
					}
					string localName = xelement.Name.LocalName;
					if (!(localName == "Name"))
					{
						if (!(localName == "Description"))
						{
							if (!(localName == "Author"))
							{
								if (!(localName == "Version"))
								{
									if (localName == "Website")
									{
										ModInfo.Parser.ParseFieldAttributeWebsite(modInfo, dictionary, xelement);
									}
								}
								else
								{
									ModInfo.Parser.ParseFieldAttributeVersion(modInfo, dictionary, xelement);
								}
							}
							else
							{
								ModInfo.Parser.ParseFieldAttributeAuthor(modInfo, dictionary, xelement);
							}
						}
						else
						{
							ModInfo.Parser.ParseFieldAttributeDescription(modInfo, dictionary, xelement);
						}
					}
					else
					{
						ModInfo.Parser.ParseFieldAttributeName(modInfo, dictionary, xelement);
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ModInfo.Parser.knownAttributesMultiplicity)
				{
					int num = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num > keyValuePair.Value.max))
					{
						string[] array = new string[6];
						array[0] = "Element has incorrect number of \"";
						array[1] = keyValuePair.Key;
						array[2] = "\" attribute instances, found ";
						array[3] = num.ToString();
						array[4] = ", expected ";
						int num2 = 5;
						Range<int> value = keyValuePair.Value;
						array[num2] = ((value != null) ? value.ToString() : null);
						throw new IncorrectAttributeOccurrenceException(string.Concat(array), ((IXmlLineInfo)_elem).LineNumber);
					}
				}
				return modInfo;
			}

			// Token: 0x0600B240 RID: 45632 RVA: 0x0042A73C File Offset: 0x0042893C
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeName(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Name\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pName = new DataItem<string>("Name", startValue);
				_entry.pName = pName;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Name"))
					{
						_foundAttributes["Name"] = 0;
					}
					int num = _foundAttributes["Name"];
					_foundAttributes["Name"] = num + 1;
				}
			}

			// Token: 0x0600B241 RID: 45633 RVA: 0x0042A7E8 File Offset: 0x004289E8
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeDescription(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Description\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pDescription = new DataItem<string>("Description", startValue);
				_entry.pDescription = pDescription;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Description"))
					{
						_foundAttributes["Description"] = 0;
					}
					int num = _foundAttributes["Description"];
					_foundAttributes["Description"] = num + 1;
				}
			}

			// Token: 0x0600B242 RID: 45634 RVA: 0x0042A894 File Offset: 0x00428A94
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeAuthor(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Author\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pAuthor = new DataItem<string>("Author", startValue);
				_entry.pAuthor = pAuthor;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Author"))
					{
						_foundAttributes["Author"] = 0;
					}
					int num = _foundAttributes["Author"];
					_foundAttributes["Author"] = num + 1;
				}
			}

			// Token: 0x0600B243 RID: 45635 RVA: 0x0042A940 File Offset: 0x00428B40
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeVersion(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Version\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pVersion = new DataItem<string>("Version", startValue);
				_entry.pVersion = pVersion;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Version"))
					{
						_foundAttributes["Version"] = 0;
					}
					int num = _foundAttributes["Version"];
					_foundAttributes["Version"] = num + 1;
				}
			}

			// Token: 0x0600B244 RID: 45636 RVA: 0x0042A9EC File Offset: 0x00428BEC
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeWebsite(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Website\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pWebsite = new DataItem<string>("Website", startValue);
				_entry.pWebsite = pWebsite;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Website"))
					{
						_foundAttributes["Website"] = 0;
					}
					int num = _foundAttributes["Website"];
					_foundAttributes["Website"] = num + 1;
				}
			}

			// Token: 0x04008650 RID: 34384
			[PublicizedFrom(EAccessModifier.Private)]
			public static readonly Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Name",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Description",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Author",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Version",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Website",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
