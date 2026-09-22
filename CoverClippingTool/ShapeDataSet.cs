using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CoverClippingTool
{
	// Token: 0x0200167D RID: 5757
	public class ShapeDataSet
	{
		// Token: 0x0600B476 RID: 46198 RVA: 0x00439384 File Offset: 0x00437584
		public ShapeDataSet(DataSource source)
		{
			this.Source = source;
		}

		// Token: 0x04008796 RID: 34710
		public readonly DataSource Source;

		// Token: 0x04008797 RID: 34711
		public readonly Dictionary<string, XElement> Data = new Dictionary<string, XElement>(StringComparer.OrdinalIgnoreCase);
	}
}
