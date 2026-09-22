using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CoverClippingTool
{
	// Token: 0x0200167B RID: 5755
	public class ArrayValue
	{
		// Token: 0x0600B472 RID: 46194 RVA: 0x004391AA File Offset: 0x004373AA
		public ArrayValue(XElement element, List<Dictionary<string, PropertyValue>> items)
		{
			this.Element = element;
			this.Items = items;
		}

		// Token: 0x0600B473 RID: 46195 RVA: 0x004391C0 File Offset: 0x004373C0
		public ArrayValue Clone()
		{
			ArrayValue arrayValue = new ArrayValue(this.Element, new List<Dictionary<string, PropertyValue>>());
			foreach (Dictionary<string, PropertyValue> dictionary in this.Items)
			{
				Dictionary<string, PropertyValue> dictionary2 = new Dictionary<string, PropertyValue>();
				foreach (KeyValuePair<string, PropertyValue> keyValuePair in dictionary)
				{
					dictionary2[keyValuePair.Key] = keyValuePair.Value.Clone();
				}
				arrayValue.Items.Add(dictionary2);
			}
			return arrayValue;
		}

		// Token: 0x04008790 RID: 34704
		public readonly List<Dictionary<string, PropertyValue>> Items;

		// Token: 0x04008791 RID: 34705
		public readonly XElement Element;
	}
}
