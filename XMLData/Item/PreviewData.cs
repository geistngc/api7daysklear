using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x0200164F RID: 5711
	public class PreviewData : IXMLData
	{
		// Token: 0x17001587 RID: 5511
		// (get) Token: 0x0600B3CC RID: 46028 RVA: 0x00435F67 File Offset: 0x00434167
		// (set) Token: 0x0600B3CD RID: 46029 RVA: 0x00435F6F File Offset: 0x0043416F
		public DataItem<int> Zoom
		{
			get
			{
				return this.pZoom;
			}
			set
			{
				this.pZoom = value;
			}
		}

		// Token: 0x17001588 RID: 5512
		// (get) Token: 0x0600B3CE RID: 46030 RVA: 0x00435F78 File Offset: 0x00434178
		// (set) Token: 0x0600B3CF RID: 46031 RVA: 0x00435F80 File Offset: 0x00434180
		public DataItem<Vector2> Pos
		{
			get
			{
				return this.pPos;
			}
			set
			{
				this.pPos = value;
			}
		}

		// Token: 0x17001589 RID: 5513
		// (get) Token: 0x0600B3D0 RID: 46032 RVA: 0x00435F89 File Offset: 0x00434189
		// (set) Token: 0x0600B3D1 RID: 46033 RVA: 0x00435F91 File Offset: 0x00434191
		public DataItem<Vector3> Rot
		{
			get
			{
				return this.pRot;
			}
			set
			{
				this.pRot = value;
			}
		}

		// Token: 0x0600B3D2 RID: 46034 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x0400873B RID: 34619
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pZoom;

		// Token: 0x0400873C RID: 34620
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<Vector2> pPos;

		// Token: 0x0400873D RID: 34621
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<Vector3> pRot;

		// Token: 0x02001650 RID: 5712
		public static class Parser
		{
			// Token: 0x0600B3D4 RID: 46036 RVA: 0x00435F9C File Offset: 0x0043419C
			public static PreviewData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "PreviewData";
				Type type = Type.GetType(typeof(PreviewData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				PreviewData previewData = (PreviewData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Preview", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!PreviewData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Preview", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Zoom"))
						{
							if (!(name == "Pos"))
							{
								if (name == "Rot")
								{
									Vector3 startValue;
									try
									{
										startValue = Vector3Parser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException);
									}
									DataItem<Vector3> pRot = new DataItem<Vector3>("Rot", startValue);
									previewData.pRot = pRot;
								}
							}
							else
							{
								Vector2 startValue2;
								try
								{
									startValue2 = Vector2Parser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException2)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException2);
								}
								DataItem<Vector2> pPos = new DataItem<Vector2>("Pos", startValue2);
								previewData.pPos = pPos;
							}
						}
						else
						{
							int startValue3;
							try
							{
								startValue3 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException3)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException3);
							}
							DataItem<int> pZoom = new DataItem<int>("Zoom", startValue3);
							previewData.pZoom = pZoom;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num = dictionary2[name];
						dictionary2[name] = num + 1;
					}
				}
				if (!dictionary.ContainsKey("Zoom"))
				{
					int startValue4;
					try
					{
						startValue4 = intParser.Parse("0");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"0\" for attribute \"Zoom\" could not be parsed", -1);
					}
					DataItem<int> pZoom2 = new DataItem<int>("Zoom", startValue4);
					previewData.pZoom = pZoom2;
					dictionary["Zoom"] = 1;
				}
				if (!dictionary.ContainsKey("Pos"))
				{
					Vector2 startValue5;
					try
					{
						startValue5 = Vector2Parser.Parse("0,0");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"0,0\" for attribute \"Pos\" could not be parsed", -1);
					}
					DataItem<Vector2> pPos2 = new DataItem<Vector2>("Pos", startValue5);
					previewData.pPos = pPos2;
					dictionary["Pos"] = 1;
				}
				if (!dictionary.ContainsKey("Rot"))
				{
					Vector3 startValue6;
					try
					{
						startValue6 = Vector3Parser.Parse("0,0,0");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"0,0,0\" for attribute \"Rot\" could not be parsed", -1);
					}
					DataItem<Vector3> pRot2 = new DataItem<Vector3>("Rot", startValue6);
					previewData.pRot = pRot2;
					dictionary["Rot"] = 1;
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in PreviewData.Parser.knownAttributesMultiplicity)
				{
					int num2 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num2 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num2 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num2.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return previewData;
			}

			// Token: 0x0400873E RID: 34622
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Zoom",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Pos",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Rot",
					new Range<int>(true, 1, true, 1)
				}
			};
		}
	}
}
