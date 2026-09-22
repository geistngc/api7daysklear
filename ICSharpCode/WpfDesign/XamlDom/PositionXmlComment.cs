using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162C RID: 5676
	public class PositionXmlComment : XmlComment, IXmlLineInfo
	{
		// Token: 0x0600B268 RID: 45672 RVA: 0x0042AE70 File Offset: 0x00429070
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlComment(string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B269 RID: 45673 RVA: 0x0042AE9C File Offset: 0x0042909C
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014EF RID: 5359
		// (get) Token: 0x0600B26A RID: 45674 RVA: 0x0042AEA4 File Offset: 0x004290A4
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014F0 RID: 5360
		// (get) Token: 0x0600B26B RID: 45675 RVA: 0x0042AEAC File Offset: 0x004290AC
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x0400865C RID: 34396
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x0400865D RID: 34397
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x0400865E RID: 34398
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
