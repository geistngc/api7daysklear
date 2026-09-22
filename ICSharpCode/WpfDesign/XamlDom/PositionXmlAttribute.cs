using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162A RID: 5674
	public class PositionXmlAttribute : XmlAttribute, IXmlLineInfo
	{
		// Token: 0x0600B260 RID: 45664 RVA: 0x0042ADE2 File Offset: 0x00428FE2
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo) : base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B261 RID: 45665 RVA: 0x0042AE14 File Offset: 0x00429014
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014EB RID: 5355
		// (get) Token: 0x0600B262 RID: 45666 RVA: 0x0042AE1C File Offset: 0x0042901C
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014EC RID: 5356
		// (get) Token: 0x0600B263 RID: 45667 RVA: 0x0042AE24 File Offset: 0x00429024
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008656 RID: 34390
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008657 RID: 34391
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008658 RID: 34392
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
