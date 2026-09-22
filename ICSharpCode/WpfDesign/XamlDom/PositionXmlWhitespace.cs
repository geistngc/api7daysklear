using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001633 RID: 5683
	public class PositionXmlWhitespace : XmlWhitespace, IXmlLineInfo
	{
		// Token: 0x0600B284 RID: 45700 RVA: 0x0042B057 File Offset: 0x00429257
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlWhitespace(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B285 RID: 45701 RVA: 0x0042B083 File Offset: 0x00429283
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014FD RID: 5373
		// (get) Token: 0x0600B286 RID: 45702 RVA: 0x0042B08B File Offset: 0x0042928B
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014FE RID: 5374
		// (get) Token: 0x0600B287 RID: 45703 RVA: 0x0042B093 File Offset: 0x00429293
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008671 RID: 34417
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008672 RID: 34418
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008673 RID: 34419
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
