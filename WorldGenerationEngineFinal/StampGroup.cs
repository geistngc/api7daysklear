using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200172A RID: 5930
	public class StampGroup
	{
		// Token: 0x0600B874 RID: 47220 RVA: 0x0044999B File Offset: 0x00447B9B
		public StampGroup(string _name)
		{
			this.Name = _name;
			this.Stamps = new List<Stamp>();
		}

		// Token: 0x04008A21 RID: 35361
		public string Name;

		// Token: 0x04008A22 RID: 35362
		public List<Stamp> Stamps;
	}
}
