using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001641 RID: 5697
	[Preserve]
	public class DamageBonusData : IXMLData
	{
		// Token: 0x1700150F RID: 5391
		// (get) Token: 0x0600B2C2 RID: 45762 RVA: 0x0042C6F7 File Offset: 0x0042A8F7
		// (set) Token: 0x0600B2C3 RID: 45763 RVA: 0x0042C6FF File Offset: 0x0042A8FF
		public DataItem<float> Head
		{
			get
			{
				return this.pHead;
			}
			set
			{
				this.pHead = value;
			}
		}

		// Token: 0x17001510 RID: 5392
		// (get) Token: 0x0600B2C4 RID: 45764 RVA: 0x0042C708 File Offset: 0x0042A908
		// (set) Token: 0x0600B2C5 RID: 45765 RVA: 0x0042C710 File Offset: 0x0042A910
		public DataItem<float> Glass
		{
			get
			{
				return this.pGlass;
			}
			set
			{
				this.pGlass = value;
			}
		}

		// Token: 0x17001511 RID: 5393
		// (get) Token: 0x0600B2C6 RID: 45766 RVA: 0x0042C719 File Offset: 0x0042A919
		// (set) Token: 0x0600B2C7 RID: 45767 RVA: 0x0042C721 File Offset: 0x0042A921
		public DataItem<float> Stone
		{
			get
			{
				return this.pStone;
			}
			set
			{
				this.pStone = value;
			}
		}

		// Token: 0x17001512 RID: 5394
		// (get) Token: 0x0600B2C8 RID: 45768 RVA: 0x0042C72A File Offset: 0x0042A92A
		// (set) Token: 0x0600B2C9 RID: 45769 RVA: 0x0042C732 File Offset: 0x0042A932
		public DataItem<float> Cloth
		{
			get
			{
				return this.pCloth;
			}
			set
			{
				this.pCloth = value;
			}
		}

		// Token: 0x17001513 RID: 5395
		// (get) Token: 0x0600B2CA RID: 45770 RVA: 0x0042C73B File Offset: 0x0042A93B
		// (set) Token: 0x0600B2CB RID: 45771 RVA: 0x0042C743 File Offset: 0x0042A943
		public DataItem<float> Concrete
		{
			get
			{
				return this.pConcrete;
			}
			set
			{
				this.pConcrete = value;
			}
		}

		// Token: 0x17001514 RID: 5396
		// (get) Token: 0x0600B2CC RID: 45772 RVA: 0x0042C74C File Offset: 0x0042A94C
		// (set) Token: 0x0600B2CD RID: 45773 RVA: 0x0042C754 File Offset: 0x0042A954
		public DataItem<float> Boulder
		{
			get
			{
				return this.pBoulder;
			}
			set
			{
				this.pBoulder = value;
			}
		}

		// Token: 0x17001515 RID: 5397
		// (get) Token: 0x0600B2CE RID: 45774 RVA: 0x0042C75D File Offset: 0x0042A95D
		// (set) Token: 0x0600B2CF RID: 45775 RVA: 0x0042C765 File Offset: 0x0042A965
		public DataItem<float> Metal
		{
			get
			{
				return this.pMetal;
			}
			set
			{
				this.pMetal = value;
			}
		}

		// Token: 0x17001516 RID: 5398
		// (get) Token: 0x0600B2D0 RID: 45776 RVA: 0x0042C76E File Offset: 0x0042A96E
		// (set) Token: 0x0600B2D1 RID: 45777 RVA: 0x0042C776 File Offset: 0x0042A976
		public DataItem<float> Wood
		{
			get
			{
				return this.pWood;
			}
			set
			{
				this.pWood = value;
			}
		}

		// Token: 0x17001517 RID: 5399
		// (get) Token: 0x0600B2D2 RID: 45778 RVA: 0x0042C77F File Offset: 0x0042A97F
		// (set) Token: 0x0600B2D3 RID: 45779 RVA: 0x0042C787 File Offset: 0x0042A987
		public DataItem<float> Earth
		{
			get
			{
				return this.pEarth;
			}
			set
			{
				this.pEarth = value;
			}
		}

		// Token: 0x17001518 RID: 5400
		// (get) Token: 0x0600B2D4 RID: 45780 RVA: 0x0042C790 File Offset: 0x0042A990
		// (set) Token: 0x0600B2D5 RID: 45781 RVA: 0x0042C798 File Offset: 0x0042A998
		public DataItem<float> Snow
		{
			get
			{
				return this.pSnow;
			}
			set
			{
				this.pSnow = value;
			}
		}

		// Token: 0x17001519 RID: 5401
		// (get) Token: 0x0600B2D6 RID: 45782 RVA: 0x0042C7A1 File Offset: 0x0042A9A1
		// (set) Token: 0x0600B2D7 RID: 45783 RVA: 0x0042C7A9 File Offset: 0x0042A9A9
		public DataItem<float> Plants
		{
			get
			{
				return this.pPlants;
			}
			set
			{
				this.pPlants = value;
			}
		}

		// Token: 0x1700151A RID: 5402
		// (get) Token: 0x0600B2D8 RID: 45784 RVA: 0x0042C7B2 File Offset: 0x0042A9B2
		// (set) Token: 0x0600B2D9 RID: 45785 RVA: 0x0042C7BA File Offset: 0x0042A9BA
		public DataItem<float> Leaves
		{
			get
			{
				return this.pLeaves;
			}
			set
			{
				this.pLeaves = value;
			}
		}

		// Token: 0x0600B2DA RID: 45786 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x0400868F RID: 34447
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHead;

		// Token: 0x04008690 RID: 34448
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGlass;

		// Token: 0x04008691 RID: 34449
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pStone;

		// Token: 0x04008692 RID: 34450
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pCloth;

		// Token: 0x04008693 RID: 34451
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pConcrete;

		// Token: 0x04008694 RID: 34452
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBoulder;

		// Token: 0x04008695 RID: 34453
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMetal;

		// Token: 0x04008696 RID: 34454
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pWood;

		// Token: 0x04008697 RID: 34455
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pEarth;

		// Token: 0x04008698 RID: 34456
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pSnow;

		// Token: 0x04008699 RID: 34457
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pPlants;

		// Token: 0x0400869A RID: 34458
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pLeaves;

		// Token: 0x02001642 RID: 5698
		public static class Parser
		{
			// Token: 0x0600B2DC RID: 45788 RVA: 0x0042C7C4 File Offset: 0x0042A9C4
			public static DamageBonusData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "DamageBonusData";
				Type type = Type.GetType(typeof(DamageBonusData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				DamageBonusData damageBonusData = (DamageBonusData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing DamageBonus", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!DamageBonusData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing DamageBonus", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2545987019U)
						{
							if (num <= 1307099730U)
							{
								if (num != 78706450U)
								{
									if (num != 81868168U)
									{
										if (num == 1307099730U)
										{
											if (name == "Boulder")
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
												DataItem<float> pBoulder = new DataItem<float>("Boulder", startValue);
												damageBonusData.pBoulder = pBoulder;
											}
										}
									}
									else if (name == "Wood")
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
										DataItem<float> pWood = new DataItem<float>("Wood", startValue2);
										damageBonusData.pWood = pWood;
									}
								}
								else if (name == "Snow")
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
									DataItem<float> pSnow = new DataItem<float>("Snow", startValue3);
									damageBonusData.pSnow = pSnow;
								}
							}
							else if (num != 1842662042U)
							{
								if (num != 1858281043U)
								{
									if (num == 2545987019U)
									{
										if (name == "Glass")
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
											DataItem<float> pGlass = new DataItem<float>("Glass", startValue4);
											damageBonusData.pGlass = pGlass;
										}
									}
								}
								else if (name == "Plants")
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
									DataItem<float> pPlants = new DataItem<float>("Plants", startValue5);
									damageBonusData.pPlants = pPlants;
								}
							}
							else if (name == "Stone")
							{
								float startValue6;
								try
								{
									startValue6 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
								DataItem<float> pStone = new DataItem<float>("Stone", startValue6);
								damageBonusData.pStone = pStone;
							}
						}
						else if (num <= 2995012523U)
						{
							if (num != 2553495518U)
							{
								if (num != 2840670588U)
								{
									if (num == 2995012523U)
									{
										if (name == "Cloth")
										{
											float startValue7;
											try
											{
												startValue7 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException7)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException7);
											}
											DataItem<float> pCloth = new DataItem<float>("Cloth", startValue7);
											damageBonusData.pCloth = pCloth;
										}
									}
								}
								else if (name == "Metal")
								{
									float startValue8;
									try
									{
										startValue8 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException8)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException8);
									}
									DataItem<float> pMetal = new DataItem<float>("Metal", startValue8);
									damageBonusData.pMetal = pMetal;
								}
							}
							else if (name == "Concrete")
							{
								float startValue9;
								try
								{
									startValue9 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException9)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException9);
								}
								DataItem<float> pConcrete = new DataItem<float>("Concrete", startValue9);
								damageBonusData.pConcrete = pConcrete;
							}
						}
						else if (num != 2996251363U)
						{
							if (num != 3947615209U)
							{
								if (num == 4159608695U)
								{
									if (name == "Earth")
									{
										float startValue10;
										try
										{
											startValue10 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException10)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException10);
										}
										DataItem<float> pEarth = new DataItem<float>("Earth", startValue10);
										damageBonusData.pEarth = pEarth;
									}
								}
							}
							else if (name == "Leaves")
							{
								float startValue11;
								try
								{
									startValue11 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException11)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException11);
								}
								DataItem<float> pLeaves = new DataItem<float>("Leaves", startValue11);
								damageBonusData.pLeaves = pLeaves;
							}
						}
						else if (name == "Head")
						{
							float startValue12;
							try
							{
								startValue12 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException12)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException12);
							}
							DataItem<float> pHead = new DataItem<float>("Head", startValue12);
							damageBonusData.pHead = pHead;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num2 = dictionary2[name];
						dictionary2[name] = num2 + 1;
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in DamageBonusData.Parser.knownAttributesMultiplicity)
				{
					int num3 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num3 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num3 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num3.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return damageBonusData;
			}

			// Token: 0x0400869B RID: 34459
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Head",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Glass",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Stone",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Cloth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Concrete",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Boulder",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Metal",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Wood",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Earth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Snow",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Plants",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Leaves",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
