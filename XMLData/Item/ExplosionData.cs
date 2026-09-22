using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001645 RID: 5701
	[Preserve]
	public class ExplosionData : IXMLData
	{
		// Token: 0x1700151B RID: 5403
		// (get) Token: 0x0600B2DE RID: 45790 RVA: 0x0042D493 File Offset: 0x0042B693
		// (set) Token: 0x0600B2DF RID: 45791 RVA: 0x0042D49B File Offset: 0x0042B69B
		public DataItem<int> BlockDamage
		{
			get
			{
				return this.pBlockDamage;
			}
			set
			{
				this.pBlockDamage = value;
			}
		}

		// Token: 0x1700151C RID: 5404
		// (get) Token: 0x0600B2E0 RID: 45792 RVA: 0x0042D4A4 File Offset: 0x0042B6A4
		// (set) Token: 0x0600B2E1 RID: 45793 RVA: 0x0042D4AC File Offset: 0x0042B6AC
		public DataItem<int> EntityDamage
		{
			get
			{
				return this.pEntityDamage;
			}
			set
			{
				this.pEntityDamage = value;
			}
		}

		// Token: 0x1700151D RID: 5405
		// (get) Token: 0x0600B2E2 RID: 45794 RVA: 0x0042D4B5 File Offset: 0x0042B6B5
		// (set) Token: 0x0600B2E3 RID: 45795 RVA: 0x0042D4BD File Offset: 0x0042B6BD
		public DataItem<int> ParticleIndex
		{
			get
			{
				return this.pParticleIndex;
			}
			set
			{
				this.pParticleIndex = value;
			}
		}

		// Token: 0x1700151E RID: 5406
		// (get) Token: 0x0600B2E4 RID: 45796 RVA: 0x0042D4C6 File Offset: 0x0042B6C6
		// (set) Token: 0x0600B2E5 RID: 45797 RVA: 0x0042D4CE File Offset: 0x0042B6CE
		public DataItem<int> RadiusBlocks
		{
			get
			{
				return this.pRadiusBlocks;
			}
			set
			{
				this.pRadiusBlocks = value;
			}
		}

		// Token: 0x1700151F RID: 5407
		// (get) Token: 0x0600B2E6 RID: 45798 RVA: 0x0042D4D7 File Offset: 0x0042B6D7
		// (set) Token: 0x0600B2E7 RID: 45799 RVA: 0x0042D4DF File Offset: 0x0042B6DF
		public DataItem<int> RadiusEntities
		{
			get
			{
				return this.pRadiusEntities;
			}
			set
			{
				this.pRadiusEntities = value;
			}
		}

		// Token: 0x0600B2E8 RID: 45800 RVA: 0x0042B63B File Offset: 0x0042983B
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x040086A6 RID: 34470
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pBlockDamage;

		// Token: 0x040086A7 RID: 34471
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pEntityDamage;

		// Token: 0x040086A8 RID: 34472
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pParticleIndex;

		// Token: 0x040086A9 RID: 34473
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRadiusBlocks;

		// Token: 0x040086AA RID: 34474
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRadiusEntities;

		// Token: 0x02001646 RID: 5702
		public static class Parser
		{
			// Token: 0x0600B2EA RID: 45802 RVA: 0x0042D4E8 File Offset: 0x0042B6E8
			public static ExplosionData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ExplosionData";
				Type type = Type.GetType(typeof(ExplosionData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ExplosionData explosionData = (ExplosionData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Explosion", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ExplosionData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Explosion", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "BlockDamage"))
						{
							if (!(name == "EntityDamage"))
							{
								if (!(name == "ParticleIndex"))
								{
									if (!(name == "RadiusBlocks"))
									{
										if (name == "RadiusEntities")
										{
											int startValue;
											try
											{
												startValue = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<int> pRadiusEntities = new DataItem<int>("RadiusEntities", startValue);
											explosionData.pRadiusEntities = pRadiusEntities;
										}
									}
									else
									{
										int startValue2;
										try
										{
											startValue2 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<int> pRadiusBlocks = new DataItem<int>("RadiusBlocks", startValue2);
										explosionData.pRadiusBlocks = pRadiusBlocks;
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
									DataItem<int> pParticleIndex = new DataItem<int>("ParticleIndex", startValue3);
									explosionData.pParticleIndex = pParticleIndex;
								}
							}
							else
							{
								int startValue4;
								try
								{
									startValue4 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
								DataItem<int> pEntityDamage = new DataItem<int>("EntityDamage", startValue4);
								explosionData.pEntityDamage = pEntityDamage;
							}
						}
						else
						{
							int startValue5;
							try
							{
								startValue5 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
							DataItem<int> pBlockDamage = new DataItem<int>("BlockDamage", startValue5);
							explosionData.pBlockDamage = pBlockDamage;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ExplosionData.Parser.knownAttributesMultiplicity)
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
				return explosionData;
			}

			// Token: 0x040086AB RID: 34475
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"BlockDamage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"EntityDamage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ParticleIndex",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RadiusBlocks",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RadiusEntities",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
