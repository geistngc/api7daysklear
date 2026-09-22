using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001630 RID: 5680
	public class PositionXmlProcessingInstruction : XmlProcessingInstruction, IXmlLineInfo
	{
		// Token: 0x0600B278 RID: 45688 RVA: 0x0042AF87 File Offset: 0x00429187
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlProcessingInstruction(string target, string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(target, data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B279 RID: 45689 RVA: 0x0042AFB7 File Offset: 0x004291B7
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014F7 RID: 5367
		// (get) Token: 0x0600B27A RID: 45690 RVA: 0x0042AFBF File Offset: 0x004291BF
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014F8 RID: 5368
		// (get) Token: 0x0600B27B RID: 45691 RVA: 0x0042AFC7 File Offset: 0x004291C7
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008668 RID: 34408
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008669 RID: 34409
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x0400866A RID: 34410
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
