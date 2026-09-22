using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;

namespace XMLData.Item
{
	// Token: 0x0200164D RID: 5709
	[Preserve]
	public class PartsData : IXMLData
	{
		// Token: 0x17001583 RID: 5507
		// (get) Token: 0x0600B3C0 RID: 46016 RVA: 0x00435961 File Offset: 0x00433B61
		// (set) Token: 0x0600B3C1 RID: 46017 RVA: 0x00435969 File Offset: 0x00433B69
		public DataItem<ItemClass> Stock
		{
			get
			{
				return this.pStock;
			}
			set
			{
				this.pStock = value;
			}
		}

		// Token: 0x17001584 RID: 5508
		// (get) Token: 0x0600B3C2 RID: 46018 RVA: 0x00435972 File Offset: 0x00433B72
		// (set) Token: 0x0600B3C3 RID: 46019 RVA: 0x0043597A File Offset: 0x00433B7A
		public DataItem<ItemClass> Receiver
		{
			get
			{
				return this.pReceiver;
			}
			set
			{
				this.pReceiver = value;
			}
		}

		// Token: 0x17001585 RID: 5509
		// (get) Token: 0x0600B3C4 RID: 46020 RVA: 0x00435983 File Offset: 0x00433B83
		// (set) Token: 0x0600B3C5 RID: 46021 RVA: 0x0043598B File Offset: 0x00433B8B
		public DataItem<ItemClass> Pump
		{
			get
			{
				return this.pPump;
			}
			set
			{
				this.pPump = value;
			}
		}

		// Token: 0x17001586 RID: 5510
		// (get) Token: 0x0600B3C6 RID: 46022 RVA: 0x00435994 File Offset: 0x00433B94
		// (set) Token: 0x0600B3C7 RID: 46023 RVA: 0x0043599C File Offset: 0x00433B9C
		public DataItem<ItemClass> Barrel
		{
			get
			{
				return this.pBarrel;
			}
			set
			{
				this.pBarrel = value;
			}
		}

		// Token: 0x0600B3C8 RID: 46024 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x04008736 RID: 34614
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ItemClass> pStock;

		// Token: 0x04008737 RID: 34615
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ItemClass> pReceiver;

		// Token: 0x04008738 RID: 34616
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ItemClass> pPump;

		// Token: 0x04008739 RID: 34617
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ItemClass> pBarrel;

		// Token: 0x0200164E RID: 5710
		public static class Parser
		{
			// Token: 0x0600B3CA RID: 46026 RVA: 0x004359A8 File Offset: 0x00433BA8
			public static PartsData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "PartsData";
				Type type = Type.GetType(typeof(PartsData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				PartsData partsData = (PartsData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Parts", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!PartsData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Parts", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Stock"))
						{
							if (!(name == "Receiver"))
							{
								if (!(name == "Pump"))
								{
									if (name == "Barrel")
									{
										ItemClass startValue;
										try
										{
											startValue = null;
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
										DataItem<ItemClass> dataItem = new DataItem<ItemClass>("Barrel", startValue);
										_updateLater.Add(positionXmlElement, dataItem);
										partsData.pBarrel = dataItem;
									}
								}
								else
								{
									ItemClass startValue2;
									try
									{
										startValue2 = null;
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
									DataItem<ItemClass> dataItem2 = new DataItem<ItemClass>("Pump", startValue2);
									_updateLater.Add(positionXmlElement, dataItem2);
									partsData.pPump = dataItem2;
								}
							}
							else
							{
								ItemClass startValue3;
								try
								{
									startValue3 = null;
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
								DataItem<ItemClass> dataItem3 = new DataItem<ItemClass>("Receiver", startValue3);
								_updateLater.Add(positionXmlElement, dataItem3);
								partsData.pReceiver = dataItem3;
							}
						}
						else
						{
							ItemClass startValue4;
							try
							{
								startValue4 = null;
							}
							catch (Exception innerException4)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException4);
							}
							DataItem<ItemClass> dataItem4 = new DataItem<ItemClass>("Stock", startValue4);
							_updateLater.Add(positionXmlElement, dataItem4);
							partsData.pStock = dataItem4;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in PartsData.Parser.knownAttributesMultiplicity)
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
				return partsData;
			}

			// Token: 0x0400873A RID: 34618
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Stock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Receiver",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Pump",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Barrel",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
