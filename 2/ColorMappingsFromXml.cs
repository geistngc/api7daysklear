using System;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using XMLData;

// Token: 0x02000D00 RID: 3328
public class ColorMappingsFromXml : MonoBehaviour
{
	// Token: 0x060065BB RID: 26043 RVA: 0x0027D63E File Offset: 0x0027B83E
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <root> found!");
		}
		ColorMappingData.Instance.ColorFromID.Clear();
		ColorMappingData.Instance.IDFromName.Clear();
		ColorMappingData.Instance.NameFromID.Clear();
		foreach (XElement element in root.Elements("color"))
		{
			int num = int.Parse(element.GetAttribute("id"));
			string attribute = element.GetAttribute("name");
			Color value;
			if (ColorUtility.TryParseHtmlString(element.GetAttribute("value"), out value))
			{
				ColorMappingData.Instance.ColorFromID.Add(num, value);
				ColorMappingData.Instance.IDFromName.Add(attribute, num);
				ColorMappingData.Instance.NameFromID.Add(num, attribute);
			}
			else
			{
				Log.Warning(string.Format("No color value for {0} and {1}", num, attribute));
			}
		}
		yield return null;
		yield break;
	}
}
