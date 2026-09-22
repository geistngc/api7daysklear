using System;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001628 RID: 5672
	public class PositionXmlDocument : XmlDocument
	{
		// Token: 0x0600B247 RID: 45639 RVA: 0x0042ABE0 File Offset: 0x00428DE0
		public PositionXmlDocument()
		{
			this.setImplementation();
		}

		// Token: 0x0600B248 RID: 45640 RVA: 0x0042ABEE File Offset: 0x00428DEE
		[PublicizedFrom(EAccessModifier.ProtectedInternal)]
		public PositionXmlDocument(XmlImplementation _imp) : base(_imp)
		{
			this.setImplementation();
		}

		// Token: 0x0600B249 RID: 45641 RVA: 0x0042ABFD File Offset: 0x00428DFD
		public PositionXmlDocument(XmlNameTable _nt) : base(_nt)
		{
			this.setImplementation();
		}

		// Token: 0x0600B24A RID: 45642 RVA: 0x0042AC0C File Offset: 0x00428E0C
		[PublicizedFrom(EAccessModifier.Private)]
		public void setImplementation()
		{
			if (this.implementationFieldInfo == null)
			{
				this.implementationFieldInfo = typeof(XmlDocument).GetField("implementation", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			this.implementationFieldInfo.SetValue(this, new PositionXmlImplementation());
		}

		// Token: 0x0600B24B RID: 45643 RVA: 0x0042AC49 File Offset: 0x00428E49
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlElement(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x0600B24C RID: 45644 RVA: 0x0042AC5A File Offset: 0x00428E5A
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x0600B24D RID: 45645 RVA: 0x0042AC6B File Offset: 0x00428E6B
		public override XmlCDataSection CreateCDataSection(string data)
		{
			return new PositionXmlCDataSection(data, this, this.lineInfo);
		}

		// Token: 0x0600B24E RID: 45646 RVA: 0x0042AC7A File Offset: 0x00428E7A
		public override XmlComment CreateComment(string data)
		{
			return new PositionXmlComment(data, this, this.lineInfo);
		}

		// Token: 0x0600B24F RID: 45647 RVA: 0x0042AC5A File Offset: 0x00428E5A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x0600B250 RID: 45648 RVA: 0x0042AC89 File Offset: 0x00428E89
		public override XmlDocumentFragment CreateDocumentFragment()
		{
			return new PositionXmlDocumentFragment(this, this.lineInfo);
		}

		// Token: 0x0600B251 RID: 45649 RVA: 0x0042AC97 File Offset: 0x00428E97
		public override XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new PositionXmlDocumentType(name, publicId, systemId, internalSubset, this, this.lineInfo);
		}

		// Token: 0x0600B252 RID: 45650 RVA: 0x0042ACAA File Offset: 0x00428EAA
		public override XmlEntityReference CreateEntityReference(string name)
		{
			return new PositionXmlEntityReference(name, this, this.lineInfo);
		}

		// Token: 0x0600B253 RID: 45651 RVA: 0x0042ACB9 File Offset: 0x00428EB9
		public override XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE1: " + name);
			return base.CreateNode(nodeTypeString, name, namespaceURI);
		}

		// Token: 0x0600B254 RID: 45652 RVA: 0x0042ACD4 File Offset: 0x00428ED4
		public override XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE2: " + name);
			return base.CreateNode(type, name, namespaceURI);
		}

		// Token: 0x0600B255 RID: 45653 RVA: 0x0042ACEF File Offset: 0x00428EEF
		public override XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE3: " + name);
			return base.CreateNode(type, prefix, name, namespaceURI);
		}

		// Token: 0x0600B256 RID: 45654 RVA: 0x0042AD0C File Offset: 0x00428F0C
		public override XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new PositionXmlProcessingInstruction(target, data, this, this.lineInfo);
		}

		// Token: 0x0600B257 RID: 45655 RVA: 0x0042AD1C File Offset: 0x00428F1C
		public override XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			return new PositionXmlSignificantWhitespace(text, this, this.lineInfo);
		}

		// Token: 0x0600B258 RID: 45656 RVA: 0x0042AD2B File Offset: 0x00428F2B
		public override XmlText CreateTextNode(string text)
		{
			return new PositionXmlText(text, this, this.lineInfo);
		}

		// Token: 0x0600B259 RID: 45657 RVA: 0x0042AD3A File Offset: 0x00428F3A
		public override XmlWhitespace CreateWhitespace(string text)
		{
			return new PositionXmlWhitespace(text, this, this.lineInfo);
		}

		// Token: 0x0600B25A RID: 45658 RVA: 0x0042AD49 File Offset: 0x00428F49
		public override XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new PositionXmlDeclaration(version, encoding, standalone, this, this.lineInfo);
		}

		// Token: 0x0600B25B RID: 45659 RVA: 0x0042AD5C File Offset: 0x00428F5C
		public override void Load(XmlReader reader)
		{
			this.lineInfo = (reader as IXmlLineInfo);
			try
			{
				base.Load(reader);
			}
			finally
			{
				this.lineInfo = null;
			}
		}

		// Token: 0x04008651 RID: 34385
		[PublicizedFrom(EAccessModifier.Private)]
		public IXmlLineInfo lineInfo;

		// Token: 0x04008652 RID: 34386
		[PublicizedFrom(EAccessModifier.Private)]
		public FieldInfo implementationFieldInfo;
	}
}
