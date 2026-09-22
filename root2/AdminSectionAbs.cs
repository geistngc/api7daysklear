using System;
using System.Xml;

// Token: 0x0200008D RID: 141
public abstract class AdminSectionAbs
{
	// Token: 0x060002B9 RID: 697 RVA: 0x00015208 File Offset: 0x00013408
	[PublicizedFrom(EAccessModifier.Protected)]
	public AdminSectionAbs(AdminTools _parent, string _sectionTypeName)
	{
		this.Parent = _parent;
		this.SectionTypeName = _sectionTypeName;
	}

	// Token: 0x060002BA RID: 698
	public abstract void Clear();

	// Token: 0x060002BB RID: 699 RVA: 0x00015220 File Offset: 0x00013420
	public virtual void Parse(XmlNode _parentNode)
	{
		foreach (object obj in _parentNode.ChildNodes)
		{
			XmlNode xmlNode = (XmlNode)obj;
			if (xmlNode.NodeType != XmlNodeType.Comment)
			{
				if (xmlNode.NodeType != XmlNodeType.Element)
				{
					Log.Warning("Unexpected XML node found in '" + this.SectionTypeName + "' section: " + xmlNode.OuterXml);
				}
				else
				{
					XmlElement childElement = (XmlElement)xmlNode;
					this.ParseElement(childElement);
				}
			}
		}
	}

	// Token: 0x060002BC RID: 700
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void ParseElement(XmlElement _childElement);

	// Token: 0x060002BD RID: 701
	public abstract void Save(XmlElement _root);

	// Token: 0x04000360 RID: 864
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly AdminTools Parent;

	// Token: 0x04000361 RID: 865
	public readonly string SectionTypeName;
}
