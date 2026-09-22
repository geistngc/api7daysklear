using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162B RID: 5675
	public class PositionXmlCDataSection : XmlCDataSection, IXmlLineInfo
	{
		// Token: 0x0600B264 RID: 45668 RVA: 0x0042AE2C File Offset: 0x0042902C
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlCDataSection(string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B265 RID: 45669 RVA: 0x0042AE58 File Offset: 0x00429058
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014ED RID: 5357
		// (get) Token: 0x0600B266 RID: 45670 RVA: 0x0042AE60 File Offset: 0x00429060
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014EE RID: 5358
		// (get) Token: 0x0600B267 RID: 45671 RVA: 0x0042AE68 File Offset: 0x00429068
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008659 RID: 34393
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x0400865A RID: 34394
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x0400865B RID: 34395
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
