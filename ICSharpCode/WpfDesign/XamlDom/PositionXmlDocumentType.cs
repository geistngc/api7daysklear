using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162E RID: 5678
	public class PositionXmlDocumentType : XmlDocumentType, IXmlLineInfo
	{
		// Token: 0x0600B270 RID: 45680 RVA: 0x0042AEF7 File Offset: 0x004290F7
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDocumentType(string name, string publicId, string systemId, string internalSubset, XmlDocument doc, IXmlLineInfo lineInfo) : base(name, publicId, systemId, internalSubset, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B271 RID: 45681 RVA: 0x0042AF2B File Offset: 0x0042912B
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014F3 RID: 5363
		// (get) Token: 0x0600B272 RID: 45682 RVA: 0x0042AF33 File Offset: 0x00429133
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014F4 RID: 5364
		// (get) Token: 0x0600B273 RID: 45683 RVA: 0x0042AF3B File Offset: 0x0042913B
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008662 RID: 34402
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008663 RID: 34403
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008664 RID: 34404
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
