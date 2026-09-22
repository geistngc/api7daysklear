using System;
using System.Collections.Generic;

namespace CoverClippingTool
{
	// Token: 0x0200167C RID: 5756
	public class ShapeData
	{
		// Token: 0x0600B474 RID: 46196 RVA: 0x00439284 File Offset: 0x00437484
		public ShapeData(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600B475 RID: 46197 RVA: 0x004392B4 File Offset: 0x004374B4
		public ShapeData Clone(string name)
		{
			ShapeData shapeData = new ShapeData(name);
			foreach (KeyValuePair<string, PropertyValue> keyValuePair in this.Properties)
			{
				shapeData.Properties[keyValuePair.Key] = keyValuePair.Value.Clone();
			}
			foreach (KeyValuePair<string, ArrayValue> keyValuePair2 in this.Arrays)
			{
				shapeData.Arrays[keyValuePair2.Key] = keyValuePair2.Value.Clone();
			}
			return shapeData;
		}

		// Token: 0x04008792 RID: 34706
		public readonly string Name;

		// Token: 0x04008793 RID: 34707
		public ShapeData Extends;

		// Token: 0x04008794 RID: 34708
		public Dictionary<string, PropertyValue> Properties = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04008795 RID: 34709
		public Dictionary<string, ArrayValue> Arrays = new Dictionary<string, ArrayValue>(StringComparer.OrdinalIgnoreCase);
	}
}
