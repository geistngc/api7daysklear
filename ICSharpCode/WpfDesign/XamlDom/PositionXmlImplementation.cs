using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001635 RID: 5685
	public class PositionXmlImplementation : XmlImplementation
	{
		// Token: 0x0600B28C RID: 45708 RVA: 0x0042B0E5 File Offset: 0x004292E5
		public override XmlDocument CreateDocument()
		{
			return new PositionXmlDocument();
		}
	}
}
