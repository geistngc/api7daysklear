using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001631 RID: 5681
	public class PositionXmlSignificantWhitespace : XmlSignificantWhitespace, IXmlLineInfo
	{
		// Token: 0x0600B27C RID: 45692 RVA: 0x0042AFCF File Offset: 0x004291CF
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlSignificantWhitespace(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B27D RID: 45693 RVA: 0x0042AFFB File Offset: 0x004291FB
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014F9 RID: 5369
		// (get) Token: 0x0600B27E RID: 45694 RVA: 0x0042B003 File Offset: 0x00429203
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014FA RID: 5370
		// (get) Token: 0x0600B27F RID: 45695 RVA: 0x0042B00B File Offset: 0x0042920B
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x0400866B RID: 34411
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x0400866C RID: 34412
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x0400866D RID: 34413
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
