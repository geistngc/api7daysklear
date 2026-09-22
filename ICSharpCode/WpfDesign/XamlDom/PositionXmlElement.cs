using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001629 RID: 5673
	public class PositionXmlElement : XmlElement, IXmlLineInfo
	{
		// Token: 0x0600B25C RID: 45660 RVA: 0x0042AD98 File Offset: 0x00428F98
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo) : base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B25D RID: 45661 RVA: 0x0042ADCA File Offset: 0x00428FCA
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014E9 RID: 5353
		// (get) Token: 0x0600B25E RID: 45662 RVA: 0x0042ADD2 File Offset: 0x00428FD2
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014EA RID: 5354
		// (get) Token: 0x0600B25F RID: 45663 RVA: 0x0042ADDA File Offset: 0x00428FDA
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008653 RID: 34387
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008654 RID: 34388
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008655 RID: 34389
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
