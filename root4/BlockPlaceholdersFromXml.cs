using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

// Token: 0x02000CF5 RID: 3317
public class BlockPlaceholdersFromXml
{
	// Token: 0x0600657A RID: 25978 RVA: 0x0027ACD0 File Offset: 0x00278ED0
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		BlockPlaceholderMap.InitStatic();
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <placeholder> found!");
		}
		using (IEnumerator<XElement> enumerator = root.Elements().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				if (!xelement.HasAttribute("name"))
				{
					throw new Exception("Attribute 'name' missing on placeholder");
				}
				string attribute = xelement.GetAttribute("name");
				BlockValue blockValue = ItemClass.GetItem(attribute, false).ToBlockValue(false);
				foreach (XElement element in xelement.Elements("block"))
				{
					if (!element.HasAttribute("name"))
					{
						throw new Exception("Attribute 'name' missing on placeholder block of '" + attribute + "'");
					}
					string attribute2 = element.GetAttribute("name");
					BlockValue targetValue = ItemClass.GetItem(attribute2, false).ToBlockValue(false);
					float num = 1f;
					if (element.HasAttribute("prob") && !StringParsers.TryParseFloat(element.GetAttribute("prob"), out num))
					{
						throw new Exception(string.Concat(new string[]
						{
							"Parsing error prob '",
							element.GetAttribute("prob"),
							"' in '",
							attribute2,
							"'"
						}));
					}
					if (num <= 0f)
					{
						throw new Exception(string.Concat(new string[]
						{
							"Parsing error prob '",
							element.GetAttribute("prob"),
							"' in '",
							attribute2,
							"': can not be negative!"
						}));
					}
					string biome = null;
					if (element.HasAttribute("biome"))
					{
						biome = element.GetAttribute("biome");
					}
					FastTags<TagGroup.Global> questTags = FastTags<TagGroup.Global>.none;
					if (element.HasAttribute("questtag"))
					{
						questTags = FastTags<TagGroup.Global>.Parse(element.GetAttribute("questtag"));
					}
					bool randomRotation = false;
					if (element.HasAttribute("randomrotation"))
					{
						randomRotation = StringParsers.ParseBool(element.GetAttribute("randomrotation"), 0, -1, true);
					}
					string sandboxOption = "";
					if (element.HasAttribute("sandboxoption"))
					{
						sandboxOption = element.GetAttribute("sandboxoption");
					}
					if (!questTags.IsEmpty)
					{
						BlockPlaceholderMap.Instance.AddQuestResetPlaceholder(blockValue, targetValue, num, biome, randomRotation, questTags);
					}
					else
					{
						BlockPlaceholderMap.Instance.AddPlaceholder(blockValue, targetValue, num, biome, randomRotation, sandboxOption);
					}
				}
				BlockPlaceholderMap.Instance.AdjustData(blockValue);
			}
			yield break;
		}
		yield break;
	}
}
