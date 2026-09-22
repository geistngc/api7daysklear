using System;
using System.Xml.Linq;

namespace CoverClippingTool
{
	// Token: 0x0200167A RID: 5754
	public struct PropertyValue
	{
		// Token: 0x0600B470 RID: 46192 RVA: 0x00439187 File Offset: 0x00437387
		public PropertyValue(XElement element, string value)
		{
			this.Element = element;
			this.Value = value;
		}

		// Token: 0x0600B471 RID: 46193 RVA: 0x00439197 File Offset: 0x00437397
		public PropertyValue Clone()
		{
			return new PropertyValue(this.Element, this.Value);
		}

		// Token: 0x0400878E RID: 34702
		public readonly string Value;

		// Token: 0x0400878F RID: 34703
		public readonly XElement Element;
	}
}
