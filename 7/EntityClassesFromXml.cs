using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x02000D06 RID: 3334
public class EntityClassesFromXml
{
	// Token: 0x060065DC RID: 26076 RVA: 0x0027E4EC File Offset: 0x0027C6EC
	public static IEnumerator LoadMain(XmlFile _xmlFile)
	{
		return EntityClassesFromXml.Load(_xmlFile, false);
	}

	// Token: 0x060065DD RID: 26077 RVA: 0x0027E4F5 File Offset: 0x0027C6F5
	public static IEnumerator LoadAppend(XmlFile _xmlFile)
	{
		return EntityClassesFromXml.Load(_xmlFile, true);
	}

	// Token: 0x060065DE RID: 26078 RVA: 0x0027E4FE File Offset: 0x0027C6FE
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator Load(XmlFile _xmlFile, bool _append)
	{
		MicroStopwatch msw = new MicroStopwatch(true);
		if (!_append)
		{
			EntityClass.list.Clear();
			EntityClass.sColors.Clear();
		}
		Dictionary<int, XElement> sEntityClassElements = new Dictionary<int, XElement>();
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <entity_classes> found!");
		}
		foreach (XElement xelement in root.Elements())
		{
			string localName = xelement.Name.LocalName;
			if (localName == "color")
			{
				XElement element = xelement;
				string attribute = element.GetAttribute("name");
				string attribute2 = element.GetAttribute("value");
				EntityClass.sColors.Add(attribute, StringParsers.ParseColor(attribute2));
			}
			else
			{
				if (localName == "replace_properties")
				{
					EntityClassesFromXml.sReplaceProperties.Clear();
					using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XElement xelement2 = enumerator2.Current;
							if (xelement2.Name == "property")
							{
								EntityClassesFromXml.sReplaceProperties.Add("^" + xelement2.GetAttribute("name"), xelement2.GetAttribute("value"));
							}
						}
						goto IL_22A;
					}
				}
				if (localName == "replace_passive_effect")
				{
					EntityClassesFromXml.sReplacePassiveEffects.Clear();
					foreach (XElement xelement3 in xelement.Elements())
					{
						if (xelement3.Name == "property")
						{
							EntityClassesFromXml.sReplacePassiveEffects.Add("^" + xelement3.GetAttribute("name"), xelement3.GetAttribute("value"));
						}
					}
				}
			}
			IL_22A:
			if (localName == "entity_class")
			{
				XElement xelement4 = xelement;
				string attribute3 = xelement4.GetAttribute("name");
				if (attribute3.Length == 0)
				{
					throw new Exception("Attribute 'name' missing on property in entity_class");
				}
				EntityClass entityClass = new EntityClass();
				entityClass.entityClassName = attribute3;
				int num = EntityClass.FromString(entityClass.entityClassName);
				if (!sEntityClassElements.TryAdd(num, xelement4))
				{
					string attribute4 = sEntityClassElements[num].GetAttribute("name");
					throw new ArgumentException(string.Concat(new string[]
					{
						"Can not add entity '",
						attribute3,
						"' with conflicting hash to existing entity '",
						attribute4,
						"'"
					}));
				}
				string attribute5 = xelement4.GetAttribute("extends");
				if (attribute5.Length > 0)
				{
					int num2 = EntityClass.FromString(attribute5);
					if (!EntityClass.list.ContainsKey(num2))
					{
						throw new Exception("Did not find 'extends' entity '" + attribute5 + "'");
					}
					HashSet<string> hashSet = new HashSet<string>();
					if (xelement4.HasAttribute("ignore"))
					{
						foreach (string text in xelement4.GetAttribute("ignore").Split(',', StringSplitOptions.None))
						{
							hashSet.Add(text.Trim());
						}
					}
					hashSet.Add("HideInSpawnMenu");
					entityClass.CopyFrom(EntityClass.list[num2], hashSet);
					entityClass.Effects = MinEffectController.ParseXml(xelement4, sEntityClassElements[num2], MinEffectController.SourceParentType.EntityClass, num);
				}
				else
				{
					entityClass.Effects = MinEffectController.ParseXml(xelement4, null, MinEffectController.SourceParentType.EntityClass, num);
				}
				foreach (XElement xelement5 in xelement4.Elements())
				{
					string localName2 = xelement5.Name.LocalName;
					if (localName2 == "property")
					{
						entityClass.Properties.Add(xelement5, true);
					}
					else if (localName2 == XNames.array)
					{
						entityClass.Properties.AddArray(xelement5);
					}
					else if (localName2 == "drop")
					{
						XElement element2 = xelement5;
						int minCount = 1;
						int maxCount = 1;
						if (element2.HasAttribute("count"))
						{
							StringParsers.ParseMinMaxCount(element2.GetAttribute("count"), out minCount, out maxCount);
						}
						float prob = 1f;
						if (element2.HasAttribute("prob"))
						{
							prob = StringParsers.ParseFloat(element2.GetAttribute("prob"), 0, -1, NumberStyles.Any);
						}
						string attribute6 = element2.GetAttribute("name");
						EnumDropEvent eEvent = EnumDropEvent.Destroy;
						if (element2.HasAttribute("event"))
						{
							eEvent = EnumUtils.Parse<EnumDropEvent>(element2.GetAttribute("event"), false);
						}
						float stickChance = 0f;
						if (element2.HasAttribute("stick_chance"))
						{
							stickChance = StringParsers.ParseFloat(element2.GetAttribute("stick_chance"), 0, -1, NumberStyles.Any);
						}
						string toolCategory = null;
						if (element2.HasAttribute("tool_category"))
						{
							toolCategory = element2.GetAttribute("tool_category");
						}
						string tag = "";
						if (element2.HasAttribute("tag"))
						{
							tag = element2.GetAttribute("tag");
						}
						entityClass.AddDroppedId(eEvent, attribute6, minCount, maxCount, prob, stickChance, toolCategory, tag);
					}
				}
				EntityClass.list[num] = entityClass;
				entityClass.Init();
			}
			if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				msw.ResetAndRestart();
			}
		}
		IEnumerator<XElement> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060065DF RID: 26079 RVA: 0x0027E514 File Offset: 0x0027C714
	public static string ReplaceProperty(string _value)
	{
		if (_value.Length > 0 && _value[0] == '^')
		{
			_value = EntityClassesFromXml.sReplaceProperties[_value];
		}
		return _value;
	}

	// Token: 0x04004F0B RID: 20235
	public const char cReplaceChar = '^';

	// Token: 0x04004F0C RID: 20236
	public static Dictionary<string, string> sReplaceProperties = new Dictionary<string, string>();

	// Token: 0x04004F0D RID: 20237
	public static Dictionary<string, string> sReplacePassiveEffects = new Dictionary<string, string>();
}
