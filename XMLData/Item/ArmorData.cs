using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x0200163D RID: 5693
	[Preserve]
	public class ArmorData : IXMLData
	{
		// Token: 0x17001502 RID: 5378
		// (get) Token: 0x0600B2A0 RID: 45728 RVA: 0x0042B5E6 File Offset: 0x004297E6
		// (set) Token: 0x0600B2A1 RID: 45729 RVA: 0x0042B5EE File Offset: 0x004297EE
		public DataItem<float> Melee
		{
			get
			{
				return this.pMelee;
			}
			set
			{
				this.pMelee = value;
			}
		}

		// Token: 0x17001503 RID: 5379
		// (get) Token: 0x0600B2A2 RID: 45730 RVA: 0x0042B5F7 File Offset: 0x004297F7
		// (set) Token: 0x0600B2A3 RID: 45731 RVA: 0x0042B5FF File Offset: 0x004297FF
		public DataItem<float> Bullet
		{
			get
			{
				return this.pBullet;
			}
			set
			{
				this.pBullet = value;
			}
		}

		// Token: 0x17001504 RID: 5380
		// (get) Token: 0x0600B2A4 RID: 45732 RVA: 0x0042B608 File Offset: 0x00429808
		// (set) Token: 0x0600B2A5 RID: 45733 RVA: 0x0042B610 File Offset: 0x00429810
		public DataItem<float> Puncture
		{
			get
			{
				return this.pPuncture;
			}
			set
			{
				this.pPuncture = value;
			}
		}

		// Token: 0x17001505 RID: 5381
		// (get) Token: 0x0600B2A6 RID: 45734 RVA: 0x0042B619 File Offset: 0x00429819
		// (set) Token: 0x0600B2A7 RID: 45735 RVA: 0x0042B621 File Offset: 0x00429821
		public DataItem<float> Blunt
		{
			get
			{
				return this.pBlunt;
			}
			set
			{
				this.pBlunt = value;
			}
		}

		// Token: 0x17001506 RID: 5382
		// (get) Token: 0x0600B2A8 RID: 45736 RVA: 0x0042B62A File Offset: 0x0042982A
		// (set) Token: 0x0600B2A9 RID: 45737 RVA: 0x0042B632 File Offset: 0x00429832
		public DataItem<float> Explosive
		{
			get
			{
				return this.pExplosive;
			}
			set
			{
				this.pExplosive = value;
			}
		}

		// Token: 0x0600B2AA RID: 45738 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x04008680 RID: 34432
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMelee;

		// Token: 0x04008681 RID: 34433
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBullet;

		// Token: 0x04008682 RID: 34434
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pPuncture;

		// Token: 0x04008683 RID: 34435
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBlunt;

		// Token: 0x04008684 RID: 34436
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pExplosive;

		// Token: 0x0200163E RID: 5694
		public static class Parser
		{
			// Token: 0x0600B2AC RID: 45740 RVA: 0x0042B644 File Offset: 0x00429844
			public static ArmorData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ArmorData";
				Type type = Type.GetType(typeof(ArmorData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ArmorData armorData = (ArmorData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Armor", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ArmorData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Armor", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Melee"))
						{
							if (!(name == "Bullet"))
							{
								if (!(name == "Puncture"))
								{
									if (!(name == "Blunt"))
									{
										if (name == "Explosive")
										{
											float startValue;
											try
											{
												startValue = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<float> pExplosive = new DataItem<float>("Explosive", startValue);
											armorData.pExplosive = pExplosive;
										}
									}
									else
									{
										float startValue2;
										try
										{
											startValue2 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<float> pBlunt = new DataItem<float>("Blunt", startValue2);
										armorData.pBlunt = pBlunt;
									}
								}
								else
								{
									float startValue3;
									try
									{
										startValue3 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
									DataItem<float> pPuncture = new DataItem<float>("Puncture", startValue3);
									armorData.pPuncture = pPuncture;
								}
							}
							else
							{
								float startValue4;
								try
								{
									startValue4 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
								DataItem<float> pBullet = new DataItem<float>("Bullet", startValue4);
								armorData.pBullet = pBullet;
							}
						}
						else
						{
							float startValue5;
							try
							{
								startValue5 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
							DataItem<float> pMelee = new DataItem<float>("Melee", startValue5);
							armorData.pMelee = pMelee;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ArmorData.Parser.knownAttributesMultiplicity)
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
				return armorData;
			}

			// Token: 0x04008685 RID: 34437
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Melee",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Bullet",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Puncture",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Blunt",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Explosive",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
