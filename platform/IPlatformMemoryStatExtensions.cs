using System;

namespace Platform
{
	// Token: 0x02001B84 RID: 7044
	public static class IPlatformMemoryStatExtensions
	{
		// Token: 0x0600D2AD RID: 53933 RVA: 0x004CA358 File Offset: 0x004C8558
		public static IPlatformMemoryStat<T> AddColumnSetHandler<T>(this IPlatformMemoryStat<T> stat, PlatformMemoryColumnChangedHandler<T> handler)
		{
			stat.ColumnSetAfter += handler;
			return stat;
		}

		// Token: 0x0600D2AE RID: 53934 RVA: 0x004CA364 File Offset: 0x004C8564
		public static IPlatformMemoryStat<T> WithUpdatePeak<T>(this IPlatformMemoryStat<T> stat) where T : IComparable<T>
		{
			return stat.AddColumnSetHandler(delegate(MemoryStatColumn column, T value)
			{
				if (column != MemoryStatColumn.Current)
				{
					return;
				}
				T other;
				if (!stat.TryGet(MemoryStatColumn.Peak, out other) || value.CompareTo(other) > 0)
				{
					stat.Set(MemoryStatColumn.Peak, value);
				}
			});
		}

		// Token: 0x0600D2AF RID: 53935 RVA: 0x004CA398 File Offset: 0x004C8598
		public static IPlatformMemoryStat<T> WithUpdateMin<T>(this IPlatformMemoryStat<T> stat) where T : IComparable<T>
		{
			return stat.AddColumnSetHandler(delegate(MemoryStatColumn column, T value)
			{
				if (column != MemoryStatColumn.Current)
				{
					return;
				}
				T other;
				if (!stat.TryGet(MemoryStatColumn.Min, out other) || value.CompareTo(other) < 0)
				{
					stat.Set(MemoryStatColumn.Min, value);
				}
			});
		}

		// Token: 0x0600D2B0 RID: 53936 RVA: 0x004CA3C9 File Offset: 0x004C85C9
		public static bool TryGetCurrentAndLast<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column, out T current, out T last)
		{
			if (!stat.TryGet(column, out current))
			{
				last = default(T);
				return false;
			}
			return stat.TryGetLast(column, out last);
		}

		// Token: 0x0600D2B1 RID: 53937 RVA: 0x004CA3E8 File Offset: 0x004C85E8
		public static bool HasColumnChanged<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column, PlatformMemoryStatHasChangedSignificantly<T> checkCurrentVsLast)
		{
			T current;
			T last;
			return stat.TryGetCurrentAndLast(column, out current, out last) && checkCurrentVsLast(current, last);
		}

		// Token: 0x0600D2B2 RID: 53938 RVA: 0x004CA40C File Offset: 0x004C860C
		public static bool HasColumnIncreased<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column) where T : IComparable<T>
		{
			return stat.HasColumnChanged(column, (T current, T last) => current.CompareTo(last) > 0);
		}

		// Token: 0x0600D2B3 RID: 53939 RVA: 0x004CA434 File Offset: 0x004C8634
		public static bool HasColumnDecreased<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column) where T : IComparable<T>
		{
			return stat.HasColumnChanged(column, (T current, T last) => current.CompareTo(last) < 0);
		}

		// Token: 0x0600D2B4 RID: 53940 RVA: 0x004CA45C File Offset: 0x004C865C
		public static bool HasBytesChangedSignificantly(this IPlatformMemoryStat<long> stat, MemoryStatColumn column)
		{
			return stat.HasColumnChanged(column, delegate(long current, long last)
			{
				long num = Math.Abs(current - last);
				long num2;
				long num3;
				if (stat.TryGet(MemoryStatColumn.Limit, out num2) && last > num2 / 2L)
				{
					num3 = Math.Abs(num2 - last);
				}
				else
				{
					num3 = Math.Abs(last);
				}
				return num >= num3 / 128L;
			});
		}
	}
}
