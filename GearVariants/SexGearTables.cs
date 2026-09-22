using System;
using System.Collections.Generic;
using UnityEngine;

namespace GearVariants
{
	// Token: 0x020016F4 RID: 5876
	[Serializable]
	public class SexGearTables
	{
		// Token: 0x0600B75D RID: 46941 RVA: 0x004427A3 File Offset: 0x004409A3
		public StringTable2D GetTable(GearVariantMatrixSO.GearPart part)
		{
			switch (part)
			{
			case GearVariantMatrixSO.GearPart.Head:
				return this.head;
			case GearVariantMatrixSO.GearPart.Hands:
				return this.hands;
			case GearVariantMatrixSO.GearPart.Feet:
				return this.feet;
			default:
				return this.hands;
			}
		}

		// Token: 0x0600B75E RID: 46942 RVA: 0x004427D4 File Offset: 0x004409D4
		public void EnsureShapes()
		{
			while (this.gearGuids.Count < this.gearPaths.Count)
			{
				this.gearGuids.Add(string.Empty);
			}
			if (this.gearGuids.Count > this.gearPaths.Count)
			{
				this.gearGuids.RemoveRange(this.gearPaths.Count, this.gearGuids.Count - this.gearPaths.Count);
			}
			this.head.EnsureShapeSymmetric(this.gearPaths, this.gearGuids);
			this.hands.EnsureShapeSymmetric(this.gearPaths, this.gearGuids);
			this.feet.EnsureShapeSymmetric(this.gearPaths, this.gearGuids);
		}

		// Token: 0x0600B75F RID: 46943 RVA: 0x00442898 File Offset: 0x00440A98
		public void ApplyUnifiedReorder(IList<string> newOrder)
		{
			if (newOrder == null)
			{
				return;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
			for (int i = 0; i < this.gearPaths.Count; i++)
			{
				dictionary[this.gearPaths[i]] = i;
			}
			int[] array = new int[newOrder.Count];
			for (int j = 0; j < newOrder.Count; j++)
			{
				int num;
				if (!dictionary.TryGetValue(newOrder[j], out num))
				{
					num = Mathf.Clamp(j, 0, this.gearPaths.Count - 1);
				}
				array[j] = num;
			}
			List<string> list = new List<string>(newOrder);
			List<string> list2 = new List<string>(list.Count);
			for (int k = 0; k < list.Count; k++)
			{
				int num2 = array[k];
				string item = (num2 >= 0 && num2 < this.gearGuids.Count) ? this.gearGuids[num2] : string.Empty;
				list2.Add(item);
			}
			this.gearPaths = list;
			this.gearGuids = list2;
			this.head.ApplyReorder(array);
			this.hands.ApplyReorder(array);
			this.feet.ApplyReorder(array);
		}

		// Token: 0x04008953 RID: 35155
		public List<string> gearPaths = new List<string>();

		// Token: 0x04008954 RID: 35156
		public List<string> gearGuids = new List<string>();

		// Token: 0x04008955 RID: 35157
		public StringTable2D head = new StringTable2D();

		// Token: 0x04008956 RID: 35158
		public StringTable2D hands = new StringTable2D();

		// Token: 0x04008957 RID: 35159
		public StringTable2D feet = new StringTable2D();
	}
}
