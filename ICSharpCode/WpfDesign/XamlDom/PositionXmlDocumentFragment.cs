using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162D RID: 5677
	public class PositionXmlDocumentFragment : XmlDocumentFragment, IXmlLineInfo
	{
		// Token: 0x0600B26C RID: 45676 RVA: 0x0042AEB4 File Offset: 0x004290B4
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDocumentFragment(XmlDocument doc, IXmlLineInfo lineInfo) : base(doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B26D RID: 45677 RVA: 0x0042AEDF File Offset: 0x004290DF
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014F1 RID: 5361
		// (get) Token: 0x0600B26E RID: 45678 RVA: 0x0042AEE7 File Offset: 0x004290E7
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014F2 RID: 5362
		// (get) Token: 0x0600B26F RID: 45679 RVA: 0x0042AEEF File Offset: 0x004290EF
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x0400865F RID: 34399
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008660 RID: 34400
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008661 RID: 34401
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
