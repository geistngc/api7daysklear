using System;
using System.Collections.Generic;
using System.Text;

namespace Platform
{
	// Token: 0x02001B8A RID: 7050
	public sealed class PlatformMemoryStat<T> : IPlatformMemoryStat<T>, IPlatformMemoryStat
	{
		// Token: 0x0600D2C1 RID: 53953 RVA: 0x004CA597 File Offset: 0x004C8797
		[PublicizedFrom(EAccessModifier.Private)]
		public PlatformMemoryStat(string name)
		{
			this.m_columnValues = new EnumDictionary<MemoryStatColumn, T>();
			this.m_columnLastValues = new EnumDictionary<MemoryStatColumn, T>();
			this.Name = name;
		}

		// Token: 0x17001A11 RID: 6673
		// (get) Token: 0x0600D2C2 RID: 53954 RVA: 0x004CA5BC File Offset: 0x004C87BC
		public string Name { get; }

		// Token: 0x0600D2C3 RID: 53955 RVA: 0x004CA5C4 File Offset: 0x004C87C4
		public void UpdateLast()
		{
			foreach (KeyValuePair<MemoryStatColumn, T> keyValuePair in this.m_columnValues)
			{
				MemoryStatColumn memoryStatColumn;
				T t;
				keyValuePair.Deconstruct(out memoryStatColumn, out t);
				MemoryStatColumn key = memoryStatColumn;
				T value = t;
				this.m_columnLastValues[key] = value;
			}
		}

		// Token: 0x0600D2C4 RID: 53956 RVA: 0x004CA630 File Offset: 0x004C8830
		public void RenderColumn(StringBuilder builder, MemoryStatColumn column, bool delta)
		{
			T t;
			if (!this.m_columnValues.TryGetValue(column, out t))
			{
				return;
			}
			if (!delta)
			{
				PlatformMemoryRenderValue<T> renderValue = this.RenderValue;
				if (renderValue == null)
				{
					return;
				}
				renderValue(builder, t);
				return;
			}
			else
			{
				T last = this.m_columnLastValues[column];
				PlatformMemoryRenderDelta<T> renderDelta = this.RenderDelta;
				if (renderDelta == null)
				{
					return;
				}
				renderDelta(builder, t, last);
				return;
			}
		}

		// Token: 0x14000124 RID: 292
		// (add) Token: 0x0600D2C5 RID: 53957 RVA: 0x004CA684 File Offset: 0x004C8884
		// (remove) Token: 0x0600D2C6 RID: 53958 RVA: 0x004CA6BC File Offset: 0x004C88BC
		public event PlatformMemoryColumnChangedHandler<T> ColumnSetAfter;

		// Token: 0x17001A12 RID: 6674
		// (get) Token: 0x0600D2C7 RID: 53959 RVA: 0x004CA6F1 File Offset: 0x004C88F1
		// (set) Token: 0x0600D2C8 RID: 53960 RVA: 0x004CA6F9 File Offset: 0x004C88F9
		public PlatformMemoryRenderValue<T> RenderValue { get; set; }

		// Token: 0x17001A13 RID: 6675
		// (get) Token: 0x0600D2C9 RID: 53961 RVA: 0x004CA702 File Offset: 0x004C8902
		// (set) Token: 0x0600D2CA RID: 53962 RVA: 0x004CA70A File Offset: 0x004C890A
		public PlatformMemoryRenderDelta<T> RenderDelta { get; set; }

		// Token: 0x0600D2CB RID: 53963 RVA: 0x004CA713 File Offset: 0x004C8913
		public void Set(MemoryStatColumn column, T value)
		{
			this.m_columnValues[column] = value;
			if (!this.m_columnLastValues.ContainsKey(column))
			{
				this.m_columnLastValues[column] = value;
			}
			PlatformMemoryColumnChangedHandler<T> columnSetAfter = this.ColumnSetAfter;
			if (columnSetAfter == null)
			{
				return;
			}
			columnSetAfter(column, value);
		}

		// Token: 0x0600D2CC RID: 53964 RVA: 0x004CA74F File Offset: 0x004C894F
		public bool TryGet(MemoryStatColumn column, out T value)
		{
			return this.m_columnValues.TryGetValue(column, out value);
		}

		// Token: 0x0600D2CD RID: 53965 RVA: 0x004CA75E File Offset: 0x004C895E
		public bool TryGetLast(MemoryStatColumn column, out T value)
		{
			return this.m_columnLastValues.TryGetValue(column, out value);
		}

		// Token: 0x0600D2CE RID: 53966 RVA: 0x004CA76D File Offset: 0x004C896D
		public static IPlatformMemoryStat<T> Create(string name)
		{
			return new PlatformMemoryStat<T>(name);
		}

		// Token: 0x0400A11A RID: 41242
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumDictionary<MemoryStatColumn, T> m_columnValues;

		// Token: 0x0400A11B RID: 41243
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumDictionary<MemoryStatColumn, T> m_columnLastValues;
	}
}
