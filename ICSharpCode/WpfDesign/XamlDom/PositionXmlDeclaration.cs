using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001634 RID: 5684
	public class PositionXmlDeclaration : XmlDeclaration, IXmlLineInfo
	{
		// Token: 0x0600B288 RID: 45704 RVA: 0x0042B09B File Offset: 0x0042929B
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDeclaration(string version, string encoding, string standalone, XmlDocument doc, IXmlLineInfo lineInfo) : base(version, encoding, standalone, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B289 RID: 45705 RVA: 0x0042B0CD File Offset: 0x004292CD
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014FF RID: 5375
		// (get) Token: 0x0600B28A RID: 45706 RVA: 0x0042B0D5 File Offset: 0x004292D5
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001500 RID: 5376
		// (get) Token: 0x0600B28B RID: 45707 RVA: 0x0042B0DD File Offset: 0x004292DD
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008674 RID: 34420
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008675 RID: 34421
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008676 RID: 34422
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
