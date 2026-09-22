using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200162F RID: 5679
	public class PositionXmlEntityReference : XmlEntityReference, IXmlLineInfo
	{
		// Token: 0x0600B274 RID: 45684 RVA: 0x0042AF43 File Offset: 0x00429143
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlEntityReference(string name, XmlDocument doc, IXmlLineInfo lineInfo) : base(name, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x0600B275 RID: 45685 RVA: 0x0042AF6F File Offset: 0x0042916F
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x170014F5 RID: 5365
		// (get) Token: 0x0600B276 RID: 45686 RVA: 0x0042AF77 File Offset: 0x00429177
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x170014F6 RID: 5366
		// (get) Token: 0x0600B277 RID: 45687 RVA: 0x0042AF7F File Offset: 0x0042917F
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x04008665 RID: 34405
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x04008666 RID: 34406
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x04008667 RID: 34407
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
