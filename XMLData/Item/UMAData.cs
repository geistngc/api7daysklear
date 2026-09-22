using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001651 RID: 5713
	public class UMAData : IXMLData
	{
		// Token: 0x1700158A RID: 5514
		// (get) Token: 0x0600B3D6 RID: 46038 RVA: 0x004365E3 File Offset: 0x004347E3
		// (set) Token: 0x0600B3D7 RID: 46039 RVA: 0x004365EB File Offset: 0x004347EB
		public DataItem<string> Mesh
		{
			get
			{
				return this.pMesh;
			}
			set
			{
				this.pMesh = value;
			}
		}

		// Token: 0x1700158B RID: 5515
		// (get) Token: 0x0600B3D8 RID: 46040 RVA: 0x004365F4 File Offset: 0x004347F4
		// (set) Token: 0x0600B3D9 RID: 46041 RVA: 0x004365FC File Offset: 0x004347FC
		public DataItem<string> OverlayTints
		{
			get
			{
				return this.pOverlayTints;
			}
			set
			{
				this.pOverlayTints = value;
			}
		}

		// Token: 0x1700158C RID: 5516
		// (get) Token: 0x0600B3DA RID: 46042 RVA: 0x00436605 File Offset: 0x00434805
		// (set) Token: 0x0600B3DB RID: 46043 RVA: 0x0043660D File Offset: 0x0043480D
		public DataItem<string> Overlay
		{
			get
			{
				return this.pOverlay;
			}
			set
			{
				this.pOverlay = value;
			}
		}

		// Token: 0x1700158D RID: 5517
		// (get) Token: 0x0600B3DC RID: 46044 RVA: 0x00436616 File Offset: 0x00434816
		// (set) Token: 0x0600B3DD RID: 46045 RVA: 0x0043661E File Offset: 0x0043481E
		public DataItem<int> Layer
		{
			get
			{
				return this.pLayer;
			}
			set
			{
				this.pLayer = value;
			}
		}

		// Token: 0x1700158E RID: 5518
		// (get) Token: 0x0600B3DE RID: 46046 RVA: 0x00436627 File Offset: 0x00434827
		// (set) Token: 0x0600B3DF RID: 46047 RVA: 0x0043662F File Offset: 0x0043482F
		public DataItem<string> UISlot
		{
			get
			{
				return this.pUISlot;
			}
			set
			{
				this.pUISlot = value;
			}
		}

		// Token: 0x1700158F RID: 5519
		// (get) Token: 0x0600B3E0 RID: 46048 RVA: 0x00436638 File Offset: 0x00434838
		// (set) Token: 0x0600B3E1 RID: 46049 RVA: 0x00436640 File Offset: 0x00434840
		public DataItem<bool> ShowHair
		{
			get
			{
				return this.pShowHair;
			}
			set
			{
				this.pShowHair = value;
			}
		}

		// Token: 0x0600B3E2 RID: 46050 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x0400873F RID: 34623
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMesh;

		// Token: 0x04008740 RID: 34624
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pOverlayTints;

		// Token: 0x04008741 RID: 34625
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pOverlay;

		// Token: 0x04008742 RID: 34626
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pLayer;

		// Token: 0x04008743 RID: 34627
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUISlot;

		// Token: 0x04008744 RID: 34628
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pShowHair;

		// Token: 0x02001652 RID: 5714
		public static class Parser
		{
			// Token: 0x0600B3E4 RID: 46052 RVA: 0x0043664C File Offset: 0x0043484C
			public static UMAData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "UMAData";
				Type type = Type.GetType(typeof(UMAData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				UMAData umadata = (UMAData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing UMA", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!UMAData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing UMA", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Mesh"))
						{
							if (!(name == "OverlayTints"))
							{
								if (!(name == "Overlay"))
								{
									if (!(name == "Layer"))
									{
										if (!(name == "UISlot"))
										{
											if (name == "ShowHair")
											{
												bool startValue;
												try
												{
													startValue = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<bool> pShowHair = new DataItem<bool>("ShowHair", startValue);
												umadata.pShowHair = pShowHair;
											}
										}
										else
										{
											string startValue2;
											try
											{
												startValue2 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<string> pUISlot = new DataItem<string>("UISlot", startValue2);
											umadata.pUISlot = pUISlot;
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
										DataItem<int> pLayer = new DataItem<int>("Layer", startValue3);
										umadata.pLayer = pLayer;
									}
								}
								else
								{
									string startValue4;
									try
									{
										startValue4 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
									DataItem<string> pOverlay = new DataItem<string>("Overlay", startValue4);
									umadata.pOverlay = pOverlay;
								}
							}
							else
							{
								string startValue5;
								try
								{
									startValue5 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException5)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException5);
								}
								DataItem<string> pOverlayTints = new DataItem<string>("OverlayTints", startValue5);
								umadata.pOverlayTints = pOverlayTints;
							}
						}
						else
						{
							string startValue6;
							try
							{
								startValue6 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException6)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException6);
							}
							DataItem<string> pMesh = new DataItem<string>("Mesh", startValue6);
							umadata.pMesh = pMesh;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in UMAData.Parser.knownAttributesMultiplicity)
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
				return umadata;
			}

			// Token: 0x04008745 RID: 34629
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Mesh",
					new Range<int>(true, 0, true, 1)
				},
				{
					"OverlayTints",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Overlay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Layer",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UISlot",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ShowHair",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
