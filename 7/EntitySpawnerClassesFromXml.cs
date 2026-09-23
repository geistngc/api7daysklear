using System;
using System.Xml.Linq;

// Token: 0x02000D0A RID: 3338
public class EntitySpawnerClassesFromXml
{
	// Token: 0x060065F5 RID: 26101 RVA: 0x0027F040 File Offset: 0x0027D240
	public static bool LoadEntitySpawnerClasses(XDocument _spawnXml)
	{
		XElement root = _spawnXml.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <spawning> found!");
		}
		foreach (XElement xelement in root.Elements("entityspawner"))
		{
			if (!xelement.HasAttribute("name"))
			{
				throw new Exception("Attribute 'name' missing on property in entityspawner");
			}
			string attribute = xelement.GetAttribute("name");
			EntitySpawnerClassForDay entitySpawnerClassForDay = new EntitySpawnerClassForDay();
			entitySpawnerClassForDay.bDynamicSpawner = (xelement.HasAttribute("dynamic") && StringParsers.ParseBool(xelement.GetAttribute("dynamic"), 0, -1, true));
			entitySpawnerClassForDay.bWrapDays = (xelement.HasAttribute("wrapMode") && xelement.GetAttribute("wrapMode") == "wrap");
			entitySpawnerClassForDay.bClampDays = (xelement.HasAttribute("wrapMode") && xelement.GetAttribute("wrapMode") == "clamp");
			foreach (XElement xelement2 in xelement.Elements("day"))
			{
				Vector2i zero = Vector2i.zero;
				if (!xelement2.GetAttribute(XNames.value).Equals("*"))
				{
					if (xelement2.GetAttribute(XNames.value).Contains(","))
					{
						int x;
						int y;
						StringParsers.ParseMinMaxCount(xelement2.GetAttribute(XNames.value), out x, out y);
						zero.x = x;
						zero.y = y;
					}
					else
					{
						zero.x = int.Parse(xelement2.GetAttribute(XNames.value));
						zero.y = zero.x;
					}
				}
				for (int i = zero.x; i <= zero.y; i++)
				{
					EntitySpawnerClass entitySpawnerClass = new EntitySpawnerClass();
					entitySpawnerClass.name = attribute;
					foreach (XElement propertyNode in xelement2.Elements(XNames.property))
					{
						entitySpawnerClass.Properties.Add(propertyNode, false);
					}
					entitySpawnerClass.Init();
					entitySpawnerClassForDay.AddForDay(i, entitySpawnerClass);
				}
			}
			if (entitySpawnerClassForDay.Count() == 0)
			{
				throw new Exception("Empty entityspawner not allowed: " + attribute);
			}
			EntitySpawnerClass.list[attribute] = entitySpawnerClassForDay;
		}
		return true;
	}
}
