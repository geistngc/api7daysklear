using System;
using System.Collections.Generic;

namespace GearVariants
{
	// Token: 0x020016F2 RID: 5874
	[Serializable]
	public class StringTable2D
	{
		// Token: 0x0600B756 RID: 46934 RVA: 0x004422DC File Offset: 0x004404DC
		public void EnsureShapeSymmetric(IReadOnlyList<string> keys, IReadOnlyList<string> guids)
		{
			IReadOnlyList<string> readOnlyList = keys ?? Array.Empty<string>();
			IReadOnlyList<string> readOnlyList2 = guids ?? Array.Empty<string>();
			Dictionary<string, StringTable2D.Row> dictionary = new Dictionary<string, StringTable2D.Row>(StringComparer.Ordinal);
			Dictionary<string, StringTable2D.Row> dictionary2 = new Dictionary<string, StringTable2D.Row>(StringComparer.Ordinal);
			foreach (StringTable2D.Row row in this.rows)
			{
				if (!string.IsNullOrEmpty(row.rowGuid) && !dictionary.ContainsKey(row.rowGuid))
				{
					dictionary.Add(row.rowGuid, row);
				}
				if (!string.IsNullOrEmpty(row.rowKey) && !dictionary2.ContainsKey(row.rowKey))
				{
					dictionary2.Add(row.rowKey, row);
				}
			}
			this.rowKeys.Clear();
			this.columnKeys.Clear();
			this.rowKeys.AddRange(readOnlyList);
			this.columnKeys.AddRange(readOnlyList);
			int count = this.columnKeys.Count;
			List<StringTable2D.Row> list = new List<StringTable2D.Row>(readOnlyList.Count);
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				string text = readOnlyList[i];
				string text2 = (i < readOnlyList2.Count) ? readOnlyList2[i] : string.Empty;
				StringTable2D.Row row2 = null;
				StringTable2D.Row row3;
				StringTable2D.Row row4;
				if (!string.IsNullOrEmpty(text2) && dictionary.TryGetValue(text2, out row3))
				{
					row2 = row3;
				}
				else if (!string.IsNullOrEmpty(text) && dictionary2.TryGetValue(text, out row4))
				{
					row2 = row4;
				}
				if (row2 == null)
				{
					row2 = new StringTable2D.Row
					{
						rowKey = text,
						rowGuid = text2,
						cellValues = new List<string>(count)
					};
				}
				else
				{
					row2.rowKey = text;
					row2.rowGuid = text2;
				}
				if (row2.cellValues == null)
				{
					row2.cellValues = new List<string>();
				}
				while (row2.cellValues.Count < count)
				{
					row2.cellValues.Add(string.Empty);
				}
				if (row2.cellValues.Count > count)
				{
					row2.cellValues.RemoveRange(count, row2.cellValues.Count - count);
				}
				list.Add(row2);
			}
			this.rows = list;
		}

		// Token: 0x0600B757 RID: 46935 RVA: 0x00442520 File Offset: 0x00440720
		public string Get(int rowIdx, int colIdx)
		{
			return this.rows[rowIdx].cellValues[colIdx];
		}

		// Token: 0x0600B758 RID: 46936 RVA: 0x00442539 File Offset: 0x00440739
		public void Set(int rowIdx, int colIdx, string value)
		{
			this.rows[rowIdx].cellValues[colIdx] = (value ?? string.Empty);
		}

		// Token: 0x0600B759 RID: 46937 RVA: 0x0044255C File Offset: 0x0044075C
		public void ClampValuesToValidOptions(Func<string, HashSet<string>> getValidOptionsForRowKey)
		{
			for (int i = 0; i < this.rows.Count; i++)
			{
				StringTable2D.Row row = this.rows[i];
				HashSet<string> hashSet = (getValidOptionsForRowKey != null) ? getValidOptionsForRowKey(row.rowKey) : null;
				if (hashSet != null)
				{
					for (int j = 0; j < row.cellValues.Count; j++)
					{
						string item = row.cellValues[j] ?? string.Empty;
						if (!hashSet.Contains(item))
						{
							row.cellValues[j] = string.Empty;
						}
					}
				}
			}
		}

		// Token: 0x0600B75A RID: 46938 RVA: 0x004425EC File Offset: 0x004407EC
		public void ApplyReorder(IReadOnlyList<int> newToOld)
		{
			int count = this.rowKeys.Count;
			if (newToOld == null || newToOld.Count != count)
			{
				return;
			}
			List<StringTable2D.Row> list = this.rows;
			List<StringTable2D.Row> list2 = new List<StringTable2D.Row>(count);
			for (int i = 0; i < count; i++)
			{
				list2.Add(null);
			}
			for (int j = 0; j < count; j++)
			{
				int index = newToOld[j];
				list2[j] = list[index];
			}
			this.rows = list2;
			for (int k = 0; k < this.rows.Count; k++)
			{
				List<string> cellValues = this.rows[k].cellValues;
				List<string> list3 = new List<string>(new string[count]);
				for (int l = 0; l < count; l++)
				{
					int num = newToOld[l];
					list3[l] = ((num >= 0 && num < cellValues.Count) ? cellValues[num] : string.Empty);
				}
				this.rows[k].cellValues = list3;
			}
			List<string> list4 = this.rowKeys;
			List<string> list5 = this.columnKeys;
			List<string> list6 = new List<string>(count);
			List<string> list7 = new List<string>(count);
			for (int m = 0; m < count; m++)
			{
				list6.Add(list4[newToOld[m]]);
				list7.Add(list5[newToOld[m]]);
			}
			this.rowKeys = list6;
			this.columnKeys = list7;
		}

		// Token: 0x0400894D RID: 35149
		public List<string> rowKeys = new List<string>();

		// Token: 0x0400894E RID: 35150
		public List<string> columnKeys = new List<string>();

		// Token: 0x0400894F RID: 35151
		public List<StringTable2D.Row> rows = new List<StringTable2D.Row>();

		// Token: 0x020016F3 RID: 5875
		[Serializable]
		public class Row
		{
			// Token: 0x04008950 RID: 35152
			public string rowKey;

			// Token: 0x04008951 RID: 35153
			public string rowGuid;

			// Token: 0x04008952 RID: 35154
			public List<string> cellValues = new List<string>();
		}
	}
}
