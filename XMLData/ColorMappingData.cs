using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace XMLData
{
	// Token: 0x0200163C RID: 5692
	[Preserve]
	public class ColorMappingData : IXMLData
	{
		// Token: 0x17001501 RID: 5377
		// (get) Token: 0x0600B29E RID: 45726 RVA: 0x0042B5A7 File Offset: 0x004297A7
		public static ColorMappingData Instance
		{
			get
			{
				ColorMappingData result;
				if ((result = ColorMappingData.instance) == null)
				{
					result = (ColorMappingData.instance = new ColorMappingData());
				}
				return result;
			}
		}

		// Token: 0x0600B29F RID: 45727 RVA: 0x0042B5BD File Offset: 0x004297BD
		[PublicizedFrom(EAccessModifier.Private)]
		public ColorMappingData()
		{
			this.IDFromName = new Dictionary<string, int>();
			this.NameFromID = new Dictionary<int, string>();
			this.ColorFromID = new Dictionary<int, Color>();
		}

		// Token: 0x0400867C RID: 34428
		[PublicizedFrom(EAccessModifier.Private)]
		public static ColorMappingData instance;

		// Token: 0x0400867D RID: 34429
		public Dictionary<string, int> IDFromName;

		// Token: 0x0400867E RID: 34430
		public Dictionary<int, string> NameFromID;

		// Token: 0x0400867F RID: 34431
		public Dictionary<int, Color> ColorFromID;
	}
}
