using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001632 RID: 5682
	public class PositionXmlText : XmlText, IXmlLineInfo
	{
		// Token: 0x0600B280 RID: 45696 RVA: 0x0042B013 File Offset: 0x00429213
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlText(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B281 RID: 45697 RVA: 0x0042B03F File Offset: 0x0042923F
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014FB RID: 5371
		// (get) Token: 0x0600B282 RID: 45698 RVA: 0x0042B047 File Offset: 0x00429247
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014FC RID: 5372
		// (get) Token: 0x0600B283 RID: 45699 RVA: 0x0042B04F File Offset: 0x0042924F
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x0400866E RID: 34414
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x0400866F RID: 34415
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008670 RID: 34416
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
