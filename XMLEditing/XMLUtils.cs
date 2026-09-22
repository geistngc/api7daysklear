using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace XMLEditing
{
	// Token: 0x02001671 RID: 5745
	public static class XMLUtils
	{
		// Token: 0x170015A6 RID: 5542
		// (get) Token: 0x0600B456 RID: 46166 RVA: 0x004389EB File Offset: 0x00436BEB
		public static string DefaultBlocksFilePath
		{
			get
			{
				return GameIO.GetGameDir("Data/Config") + "/blocks.xml";
			}
		}

		// Token: 0x0600B457 RID: 46167 RVA: 0x00438A04 File Offset: 0x00436C04
		public static XDocument LoadXDocument(string filePath)
		{
			XDocument result;
			using (Stream stream = SdFile.OpenRead(filePath))
			{
				result = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
			}
			return result;
		}

		// Token: 0x0600B458 RID: 46168 RVA: 0x00438A40 File Offset: 0x00436C40
		public static XElement SetArray(XElement _element, string _arrayName)
		{
			XElement xelement = (from e in _element.Elements(XNames.array)
			where e.GetAttribute(XNames.name) == _arrayName
			select e).FirstOrDefault<XElement>();
			if (xelement == null)
			{
				xelement = new XElement(XNames.array, new XAttribute(XNames.name, _arrayName));
				_element.Add("\t");
				_element.Add(xelement);
				_element.Add("\r\n");
				_element.Add("\t");
			}
			return xelement;
		}

		// Token: 0x0600B459 RID: 46169 RVA: 0x00438AC4 File Offset: 0x00436CC4
		public static XElement SetProperty(XElement _element, string _propertyName, XName _attribName, string _value)
		{
			XElement xelement = (from e in _element.Elements(XNames.property)
			where e.GetAttribute(XNames.name) == _propertyName
			select e).FirstOrDefault<XElement>();
			if (xelement == null)
			{
				xelement = new XElement(XNames.property, new XAttribute(XNames.name, _propertyName));
				_element.Add("\t");
				_element.Add(xelement);
				_element.Add("\r\n");
				_element.Add("\t");
			}
			xelement.SetAttributeValue(_attribName, _value);
			return xelement;
		}

		// Token: 0x0600B45A RID: 46170 RVA: 0x00438B50 File Offset: 0x00436D50
		public static bool IsChildOf(this XElement element, XElement parent)
		{
			for (XElement xelement = element; xelement != null; xelement = xelement.Parent)
			{
				if (xelement == parent)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B45B RID: 46171 RVA: 0x00438B74 File Offset: 0x00436D74
		public static void SaveXDocument(XDocument doc, string filePath, bool omitXmlDeclaration = false)
		{
			using (Stream stream = SdFile.Create(filePath))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					IndentChars = "\t",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace,
					OmitXmlDeclaration = omitXmlDeclaration
				}))
				{
					doc.WriteTo(xmlWriter);
				}
			}
		}

		// Token: 0x0600B45C RID: 46172 RVA: 0x00438C08 File Offset: 0x00436E08
		public static void CleanAndRepairBlocksXML()
		{
			string text = SdFile.ReadAllText(XMLUtils.DefaultBlocksFilePath);
			string pattern = "<block [^>]*>[\\s\\S]*?</block>";
			text = Regex.Replace(text, pattern, (Match m) => Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(m.Value, "\r\n\\s*\r\n", "\r\n"), "/>\r\n\t(\t)?(<property name=\"(ModelOffset|OversizedBounds)\")", "/>\r\n\t$2"), "\r\n\t</block>", "\r\n</block>"), "\r\n\t <!--[\\s\\S]*?-->\r\n", "\r\n"), "(name=\"MeshDamage\" value=\")([^\"]+)\"", delegate(Match match)
			{
				string str = Regex.Replace(match.Groups[2].Value, " {2,3}", "\r\n\t\t");
				return match.Groups[1].Value + str + "\"";
			}), " />", "/>"), RegexOptions.Singleline);
			SdFile.WriteAllText(XMLUtils.DefaultBlocksFilePath, text);
		}

		// Token: 0x0600B45D RID: 46173 RVA: 0x00438C5C File Offset: 0x00436E5C
		public static HashSet<string> ParseStringList(string targetListString, char separator)
		{
			string[] array = targetListString.Split(new char[]
			{
				separator
			}, StringSplitOptions.RemoveEmptyEntries);
			HashSet<string> hashSet = new HashSet<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string item = array2[i].Trim();
				hashSet.Add(item);
			}
			return hashSet;
		}

		// Token: 0x0600B45E RID: 46174 RVA: 0x00438CA4 File Offset: 0x00436EA4
		public static HashSet<string> GetReplacementBlockNames(HashSet<string> targetNames)
		{
			XElement root = XMLUtils.LoadXDocument(GameIO.GetGameDir("Data/Config") + "/blockplaceholders.xml").Root;
			if (root == null || !root.HasElements)
			{
				throw new Exception("No element <blockplaceholders> found!");
			}
			Dictionary<string, XElement> dictionary = new CaseInsensitiveStringDictionary<XElement>();
			foreach (XElement xelement in root.Elements("placeholder"))
			{
				string attribute = xelement.GetAttribute(XNames.name);
				dictionary[attribute] = xelement;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string text in targetNames)
			{
				string key = text.Trim();
				XElement xelement2;
				if (dictionary.TryGetValue(key, out xelement2))
				{
					foreach (XElement element in xelement2.Elements(XNames.block))
					{
						hashSet.Add(element.GetAttribute(XNames.name).Trim());
					}
				}
			}
			return hashSet;
		}

		// Token: 0x0600B45F RID: 46175 RVA: 0x00438DEC File Offset: 0x00436FEC
		public static bool AllAttributesAreEqual(XElement elementA, XElement elementB, StringComparison comparisonType)
		{
			if (elementB.Attributes().Count<XAttribute>() != elementA.Attributes().Count<XAttribute>())
			{
				return false;
			}
			foreach (XAttribute xattribute in elementA.Attributes())
			{
				XAttribute xattribute2 = elementB.Attribute(xattribute.Name);
				if (xattribute2 == null)
				{
					return false;
				}
				string a = xattribute.Value.Trim();
				string b = xattribute2.Value.Trim();
				if (!string.Equals(a, b, comparisonType))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B460 RID: 46176 RVA: 0x00438E8C File Offset: 0x0043708C
		public static void PopulateReplacementMap(Dictionary<string, HashSet<string>> replacementMap)
		{
			replacementMap.Clear();
			XElement root = XMLUtils.LoadXDocument(GameIO.GetGameDir("Data/Config") + "/blockplaceholders.xml").Root;
			if (root == null || !root.HasElements)
			{
				throw new Exception("No element <blockplaceholders> found!");
			}
			foreach (XElement xelement in root.Elements("placeholder"))
			{
				string key = xelement.GetAttribute(XNames.name).Trim();
				HashSet<string> hashSet = new HashSet<string>();
				replacementMap[key] = hashSet;
				foreach (XElement element in xelement.Elements(XNames.block))
				{
					hashSet.Add(element.GetAttribute(XNames.name).Trim());
				}
			}
		}

		// Token: 0x04008775 RID: 34677
		public const string IndentChars = "\t";

		// Token: 0x04008776 RID: 34678
		public const string NewLineChars = "\r\n";
	}
}
