using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x02000CF3 RID: 3315
public class BiomeSpawningFromXml
{
	// Token: 0x06006572 RID: 25970 RVA: 0x0027A966 File Offset: 0x00278B66
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <spawning> found!");
		}
		using (IEnumerator<XElement> enumerator = root.Elements("biome").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				string attribute = xelement.GetAttribute("name");
				if (attribute.Length == 0)
				{
					throw new Exception("Attribute 'name' missing on biome tag");
				}
				BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = new BiomeSpawnEntityGroupList();
				BiomeSpawningClass.list[attribute] = biomeSpawnEntityGroupList;
				foreach (XElement element in xelement.Elements("spawn"))
				{
					string attribute2 = element.GetAttribute("id");
					int hashCode = attribute2.GetHashCode();
					if (biomeSpawnEntityGroupList.Find(hashCode) != null)
					{
						throw new Exception(string.Concat(new string[]
						{
							"Duplicate id hash '",
							attribute2,
							"' in biome '",
							attribute,
							"'"
						}));
					}
					int maxCount = 1;
					if (element.HasAttribute("maxcount"))
					{
						maxCount = int.Parse(element.GetAttribute("maxcount"));
					}
					int[] array = null;
					if (element.HasAttribute("respawndelay"))
					{
						string[] array2 = element.GetAttribute("respawndelay").Split(',', StringSplitOptions.None);
						array = new int[array2.Length];
						for (int i = 0; i < array2.Length; i++)
						{
							array[i] = (int)(StringParsers.ParseFloat(array2[i], 0, -1, NumberStyles.Any) * 24000f);
						}
					}
					EDaytime daytime = EDaytime.Any;
					if (element.HasAttribute("time"))
					{
						daytime = EnumUtils.Parse<EDaytime>(element.GetAttribute("time"), false);
					}
					BiomeSpawnEntityGroupData.eType type = BiomeSpawnEntityGroupData.eType.Normal;
					string attribute3 = element.GetAttribute("type");
					if (attribute3.Length > 0)
					{
						type = EnumUtils.Parse<BiomeSpawnEntityGroupData.eType>(attribute3, true);
					}
					BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = new BiomeSpawnEntityGroupData(hashCode, maxCount, array, daytime, type);
					string attribute4 = element.GetAttribute("tags");
					if (attribute4.Length > 0)
					{
						biomeSpawnEntityGroupData.POITags = FastTags<TagGroup.Poi>.Parse(attribute4);
					}
					attribute4 = element.GetAttribute("notags");
					if (attribute4.Length > 0)
					{
						biomeSpawnEntityGroupData.noPOITags = FastTags<TagGroup.Poi>.Parse(attribute4);
					}
					string attribute5 = element.GetAttribute("entitygroup");
					if (attribute5.Length == 0)
					{
						throw new Exception("Missing attribute 'entitygroup' in entitygroup of biome '" + attribute + "'");
					}
					if (!EntityGroups.list.ContainsKey(attribute5))
					{
						throw new Exception("Entity group '" + attribute5 + "' not existing!");
					}
					biomeSpawnEntityGroupData.entityGroupName = attribute5;
					biomeSpawnEntityGroupList.list.Add(biomeSpawnEntityGroupData);
				}
			}
			yield break;
		}
		yield break;
	}
}
