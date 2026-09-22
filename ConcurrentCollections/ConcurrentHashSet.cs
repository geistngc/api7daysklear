using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ConcurrentCollections
{
	// Token: 0x02001AAB RID: 6827
	[DebuggerDisplay("Count = {Count}")]
	public class ConcurrentHashSet<T> : IReadOnlyCollection<T>, IEnumerable<!0>, IEnumerable, ICollection<T>
	{
		// Token: 0x1700195F RID: 6495
		// (get) Token: 0x0600CDEA RID: 52714 RVA: 0x004AFEFB File Offset: 0x004AE0FB
		public static int DefaultConcurrencyLevel
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return Math.Max(2, Environment.ProcessorCount);
			}
		}

		// Token: 0x17001960 RID: 6496
		// (get) Token: 0x0600CDEB RID: 52715 RVA: 0x004AFF08 File Offset: 0x004AE108
		public int Count
		{
			get
			{
				int num = 0;
				int toExclusive = 0;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					for (int i = 0; i < this._tables.CountPerLock.Length; i++)
					{
						num += this._tables.CountPerLock[i];
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return num;
			}
		}

		// Token: 0x17001961 RID: 6497
		// (get) Token: 0x0600CDEC RID: 52716 RVA: 0x004AFF70 File Offset: 0x004AE170
		public bool IsEmpty
		{
			get
			{
				int toExclusive = 0;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					for (int i = 0; i < this._tables.CountPerLock.Length; i++)
					{
						if (this._tables.CountPerLock[i] != 0)
						{
							return false;
						}
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return true;
			}
		}

		// Token: 0x0600CDED RID: 52717 RVA: 0x004AFFD8 File Offset: 0x004AE1D8
		public ConcurrentHashSet() : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, null)
		{
		}

		// Token: 0x0600CDEE RID: 52718 RVA: 0x004AFFE9 File Offset: 0x004AE1E9
		public ConcurrentHashSet(Action<T> onRemovalFaiuire) : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, null)
		{
			this.OnRemovalFailure = onRemovalFaiuire;
		}

		// Token: 0x0600CDEF RID: 52719 RVA: 0x004B0001 File Offset: 0x004AE201
		public ConcurrentHashSet(int concurrencyLevel, int capacity) : this(concurrencyLevel, capacity, false, null)
		{
		}

		// Token: 0x0600CDF0 RID: 52720 RVA: 0x004B000D File Offset: 0x004AE20D
		public ConcurrentHashSet(IEnumerable<T> collection) : this(collection, null)
		{
		}

		// Token: 0x0600CDF1 RID: 52721 RVA: 0x004B0017 File Offset: 0x004AE217
		public ConcurrentHashSet(IEqualityComparer<T> comparer) : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, comparer)
		{
		}

		// Token: 0x0600CDF2 RID: 52722 RVA: 0x004B0028 File Offset: 0x004AE228
		public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.InitializeFromCollection(collection);
		}

		// Token: 0x0600CDF3 RID: 52723 RVA: 0x004B0046 File Offset: 0x004AE246
		public ConcurrentHashSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(concurrencyLevel, 31, false, comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.InitializeFromCollection(collection);
		}

		// Token: 0x0600CDF4 RID: 52724 RVA: 0x004B0068 File Offset: 0x004AE268
		public ConcurrentHashSet(int concurrencyLevel, int capacity, IEqualityComparer<T> comparer) : this(concurrencyLevel, capacity, false, comparer)
		{
		}

		// Token: 0x0600CDF5 RID: 52725 RVA: 0x004B0074 File Offset: 0x004AE274
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentHashSet(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<T> comparer)
		{
			if (concurrencyLevel < 1)
			{
				throw new ArgumentOutOfRangeException("concurrencyLevel");
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity < concurrencyLevel)
			{
				capacity = concurrencyLevel;
			}
			object[] array = new object[concurrencyLevel];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new object();
			}
			int[] countPerLock = new int[array.Length];
			ConcurrentHashSet<T>.Node[] array2 = new ConcurrentHashSet<T>.Node[capacity];
			this._tables = new ConcurrentHashSet<T>.Tables(array2, array, countPerLock);
			this._growLockArray = growLockArray;
			this._budget = array2.Length / array.Length;
			this._comparer = (comparer ?? EqualityComparer<T>.Default);
		}

		// Token: 0x0600CDF6 RID: 52726 RVA: 0x004B010E File Offset: 0x004AE30E
		public bool Add(T item)
		{
			return this.AddInternal(item, this._comparer.GetHashCode(item), true);
		}

		// Token: 0x0600CDF7 RID: 52727 RVA: 0x004B0124 File Offset: 0x004AE324
		public void Clear()
		{
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				ConcurrentHashSet<T>.Tables tables = new ConcurrentHashSet<T>.Tables(new ConcurrentHashSet<T>.Node[31], this._tables.Locks, new int[this._tables.CountPerLock.Length]);
				this._tables = tables;
				this._budget = Math.Max(1, tables.Buckets.Length / tables.Locks.Length);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600CDF8 RID: 52728 RVA: 0x004B01AC File Offset: 0x004AE3AC
		public bool Contains(T item)
		{
			int hashCode = this._comparer.GetHashCode(item);
			ConcurrentHashSet<T>.Tables tables = this._tables;
			int bucket = ConcurrentHashSet<T>.GetBucket(hashCode, tables.Buckets.Length);
			for (ConcurrentHashSet<T>.Node node = Volatile.Read<ConcurrentHashSet<T>.Node>(ref tables.Buckets[bucket]); node != null; node = node.Next)
			{
				if (hashCode == node.Hashcode && this._comparer.Equals(node.Item, item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600CDF9 RID: 52729 RVA: 0x004B0220 File Offset: 0x004AE420
		public bool TryRemove(T item)
		{
			int hashCode = this._comparer.GetHashCode(item);
			for (;;)
			{
				ConcurrentHashSet<T>.Tables tables = this._tables;
				int num;
				int num2;
				ConcurrentHashSet<T>.GetBucketAndLockNo(hashCode, out num, out num2, tables.Buckets.Length, tables.Locks.Length);
				object obj = tables.Locks[num2];
				lock (obj)
				{
					if (tables != this._tables)
					{
						continue;
					}
					ConcurrentHashSet<T>.Node node = null;
					for (ConcurrentHashSet<T>.Node node2 = tables.Buckets[num]; node2 != null; node2 = node2.Next)
					{
						if (hashCode == node2.Hashcode && this._comparer.Equals(node2.Item, item))
						{
							if (node == null)
							{
								Volatile.Write<ConcurrentHashSet<T>.Node>(ref tables.Buckets[num], node2.Next);
							}
							else
							{
								node.Next = node2.Next;
							}
							tables.CountPerLock[num2]--;
							return true;
						}
						node = node2;
					}
				}
				break;
			}
			Action<T> onRemovalFailure = this.OnRemovalFailure;
			if (onRemovalFailure != null)
			{
				onRemovalFailure(item);
			}
			return false;
		}

		// Token: 0x0600CDFA RID: 52730 RVA: 0x004B0340 File Offset: 0x004AE540
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600CDFB RID: 52731 RVA: 0x004B0348 File Offset: 0x004AE548
		public IEnumerator<T> GetEnumerator()
		{
			ConcurrentHashSet<T>.Node[] buckets = this._tables.Buckets;
			int num;
			for (int i = 0; i < buckets.Length; i = num + 1)
			{
				ConcurrentHashSet<T>.Node current;
				for (current = Volatile.Read<ConcurrentHashSet<T>.Node>(ref buckets[i]); current != null; current = current.Next)
				{
					yield return current.Item;
				}
				current = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x0600CDFC RID: 52732 RVA: 0x004B0357 File Offset: 0x004AE557
		[PublicizedFrom(EAccessModifier.Private)]
		public void Add(T item)
		{
			this.Add(item);
		}

		// Token: 0x17001962 RID: 6498
		// (get) Token: 0x0600CDFD RID: 52733 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsReadOnly
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return false;
			}
		}

		// Token: 0x0600CDFE RID: 52734 RVA: 0x004B0364 File Offset: 0x004AE564
		[PublicizedFrom(EAccessModifier.Private)]
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				int num = 0;
				int num2 = 0;
				while (num2 < this._tables.Locks.Length && num >= 0)
				{
					num += this._tables.CountPerLock[num2];
					num2++;
				}
				if (array.Length - num < arrayIndex || num < 0)
				{
					throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the set is greater than the available space from index to the end of the destination array.");
				}
				this.CopyToItems(array, arrayIndex);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600CDFF RID: 52735 RVA: 0x004B0408 File Offset: 0x004AE608
		[PublicizedFrom(EAccessModifier.Private)]
		public bool Remove(T item)
		{
			return this.TryRemove(item);
		}

		// Token: 0x0600CE00 RID: 52736 RVA: 0x004B0414 File Offset: 0x004AE614
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitializeFromCollection(IEnumerable<T> collection)
		{
			foreach (T t in collection)
			{
				this.AddInternal(t, this._comparer.GetHashCode(t), false);
			}
			if (this._budget == 0)
			{
				this._budget = this._tables.Buckets.Length / this._tables.Locks.Length;
			}
		}

		// Token: 0x0600CE01 RID: 52737 RVA: 0x004B0498 File Offset: 0x004AE698
		public bool TryFirst(out T returnValue)
		{
			if (this._tables == null || this._tables.Buckets == null || this._tables.Buckets.Length == 0)
			{
				returnValue = default(T);
				return false;
			}
			ConcurrentHashSet<T>.Node node = this._tables.Buckets.FirstOrDefault((ConcurrentHashSet<T>.Node d) => d != null);
			if (node == null)
			{
				returnValue = default(T);
				return false;
			}
			returnValue = node.Item;
			return true;
		}

		// Token: 0x0600CE02 RID: 52738 RVA: 0x004B0524 File Offset: 0x004AE724
		public bool TryRemoveFirst(out T returnValue)
		{
			if (this._tables == null || this._tables.Buckets == null || this._tables.Buckets.Length == 0)
			{
				returnValue = default(T);
				return false;
			}
			ConcurrentHashSet<T>.Node node = this._tables.Buckets.FirstOrDefault((ConcurrentHashSet<T>.Node d) => d != null);
			if (node == null)
			{
				returnValue = default(T);
				return false;
			}
			returnValue = node.Item;
			this.TryRemove(returnValue);
			return true;
		}

		// Token: 0x0600CE03 RID: 52739 RVA: 0x004B05BC File Offset: 0x004AE7BC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool AddInternal(T item, int hashcode, bool acquireLock)
		{
			checked
			{
				ConcurrentHashSet<T>.Tables tables;
				bool flag;
				for (;;)
				{
					tables = this._tables;
					int num;
					int num2;
					ConcurrentHashSet<T>.GetBucketAndLockNo(hashcode, out num, out num2, tables.Buckets.Length, tables.Locks.Length);
					flag = false;
					bool flag2 = false;
					try
					{
						if (acquireLock)
						{
							Monitor.Enter(tables.Locks[num2], ref flag2);
						}
						if (tables != this._tables)
						{
							continue;
						}
						for (ConcurrentHashSet<T>.Node node = tables.Buckets[num]; node != null; node = node.Next)
						{
							if (hashcode == node.Hashcode && this._comparer.Equals(node.Item, item))
							{
								return false;
							}
						}
						Volatile.Write<ConcurrentHashSet<T>.Node>(ref tables.Buckets[num], new ConcurrentHashSet<T>.Node(item, hashcode, tables.Buckets[num]));
						tables.CountPerLock[num2]++;
						if (tables.CountPerLock[num2] > this._budget)
						{
							flag = true;
						}
					}
					finally
					{
						if (flag2)
						{
							Monitor.Exit(tables.Locks[num2]);
						}
					}
					break;
				}
				if (flag)
				{
					this.GrowTable(tables);
				}
				return true;
			}
		}

		// Token: 0x0600CE04 RID: 52740 RVA: 0x004B06C8 File Offset: 0x004AE8C8
		[PublicizedFrom(EAccessModifier.Private)]
		public static int GetBucket(int hashcode, int bucketCount)
		{
			return (hashcode & int.MaxValue) % bucketCount;
		}

		// Token: 0x0600CE05 RID: 52741 RVA: 0x004B06D3 File Offset: 0x004AE8D3
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount, int lockCount)
		{
			bucketNo = (hashcode & int.MaxValue) % bucketCount;
			lockNo = bucketNo % lockCount;
		}

		// Token: 0x0600CE06 RID: 52742 RVA: 0x004B06E8 File Offset: 0x004AE8E8
		[PublicizedFrom(EAccessModifier.Private)]
		public void GrowTable(ConcurrentHashSet<T>.Tables tables)
		{
			int toExclusive = 0;
			try
			{
				this.AcquireLocks(0, 1, ref toExclusive);
				if (tables == this._tables)
				{
					long num = 0L;
					for (int i = 0; i < tables.CountPerLock.Length; i++)
					{
						num += (long)tables.CountPerLock[i];
					}
					if (num < (long)(tables.Buckets.Length / 4))
					{
						this._budget = 2 * this._budget;
						if (this._budget < 0)
						{
							this._budget = int.MaxValue;
						}
					}
					else
					{
						int num2 = 0;
						bool flag = false;
						object[] array;
						checked
						{
							try
							{
								num2 = tables.Buckets.Length * 2 + 1;
								while (num2 % 3 == 0 || num2 % 5 == 0 || num2 % 7 == 0)
								{
									num2 += 2;
								}
								if (num2 > 2146435071)
								{
									flag = true;
								}
							}
							catch (OverflowException)
							{
								flag = true;
							}
							if (flag)
							{
								num2 = 2146435071;
								this._budget = int.MaxValue;
							}
							this.AcquireLocks(1, tables.Locks.Length, ref toExclusive);
							array = tables.Locks;
						}
						if (this._growLockArray && tables.Locks.Length < 1024)
						{
							array = new object[tables.Locks.Length * 2];
							Array.Copy(tables.Locks, 0, array, 0, tables.Locks.Length);
							for (int j = tables.Locks.Length; j < array.Length; j++)
							{
								array[j] = new object();
							}
						}
						ConcurrentHashSet<T>.Node[] array2 = new ConcurrentHashSet<T>.Node[num2];
						int[] array3 = new int[array.Length];
						for (int k = 0; k < tables.Buckets.Length; k++)
						{
							checked
							{
								ConcurrentHashSet<T>.Node next;
								for (ConcurrentHashSet<T>.Node node = tables.Buckets[k]; node != null; node = next)
								{
									next = node.Next;
									int num3;
									int num4;
									ConcurrentHashSet<T>.GetBucketAndLockNo(node.Hashcode, out num3, out num4, array2.Length, array.Length);
									array2[num3] = new ConcurrentHashSet<T>.Node(node.Item, node.Hashcode, array2[num3]);
									array3[num4]++;
								}
							}
						}
						this._budget = Math.Max(1, array2.Length / array.Length);
						this._tables = new ConcurrentHashSet<T>.Tables(array2, array, array3);
					}
				}
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600CE07 RID: 52743 RVA: 0x004B0928 File Offset: 0x004AEB28
		[PublicizedFrom(EAccessModifier.Private)]
		public void AcquireAllLocks(ref int locksAcquired)
		{
			this.AcquireLocks(0, 1, ref locksAcquired);
			this.AcquireLocks(1, this._tables.Locks.Length, ref locksAcquired);
		}

		// Token: 0x0600CE08 RID: 52744 RVA: 0x004B094C File Offset: 0x004AEB4C
		[PublicizedFrom(EAccessModifier.Private)]
		public void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
		{
			object[] locks = this._tables.Locks;
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				bool flag = false;
				try
				{
					Monitor.Enter(locks[i], ref flag);
				}
				finally
				{
					if (flag)
					{
						locksAcquired++;
					}
				}
			}
		}

		// Token: 0x0600CE09 RID: 52745 RVA: 0x004B099C File Offset: 0x004AEB9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReleaseLocks(int fromInclusive, int toExclusive)
		{
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				Monitor.Exit(this._tables.Locks[i]);
			}
		}

		// Token: 0x0600CE0A RID: 52746 RVA: 0x004B09CC File Offset: 0x004AEBCC
		[PublicizedFrom(EAccessModifier.Private)]
		public void CopyToItems(T[] array, int index)
		{
			foreach (ConcurrentHashSet<T>.Node node in this._tables.Buckets)
			{
				while (node != null)
				{
					array[index] = node.Item;
					index++;
					node = node.Next;
				}
			}
		}

		// Token: 0x04009C50 RID: 40016
		[PublicizedFrom(EAccessModifier.Private)]
		public const int DefaultCapacity = 31;

		// Token: 0x04009C51 RID: 40017
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxLockNumber = 1024;

		// Token: 0x04009C52 RID: 40018
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEqualityComparer<T> _comparer;

		// Token: 0x04009C53 RID: 40019
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool _growLockArray;

		// Token: 0x04009C54 RID: 40020
		[PublicizedFrom(EAccessModifier.Private)]
		public int _budget;

		// Token: 0x04009C55 RID: 40021
		[PublicizedFrom(EAccessModifier.Private)]
		public volatile ConcurrentHashSet<T>.Tables _tables;

		// Token: 0x04009C56 RID: 40022
		public Action<T> OnRemovalFailure;

		// Token: 0x02001AAC RID: 6828
		[PublicizedFrom(EAccessModifier.Private)]
		public class Tables
		{
			// Token: 0x0600CE0B RID: 52747 RVA: 0x004B0A19 File Offset: 0x004AEC19
			public Tables(ConcurrentHashSet<T>.Node[] buckets, object[] locks, int[] countPerLock)
			{
				this.Buckets = buckets;
				this.Locks = locks;
				this.CountPerLock = countPerLock;
			}

			// Token: 0x04009C57 RID: 40023
			public readonly ConcurrentHashSet<T>.Node[] Buckets;

			// Token: 0x04009C58 RID: 40024
			public readonly object[] Locks;

			// Token: 0x04009C59 RID: 40025
			public volatile int[] CountPerLock;
		}

		// Token: 0x02001AAD RID: 6829
		[PublicizedFrom(EAccessModifier.Private)]
		public class Node
		{
			// Token: 0x0600CE0C RID: 52748 RVA: 0x004B0A38 File Offset: 0x004AEC38
			public Node(T item, int hashcode, ConcurrentHashSet<T>.Node next)
			{
				this.Item = item;
				this.Hashcode = hashcode;
				this.Next = next;
			}

			// Token: 0x04009C5A RID: 40026
			public readonly T Item;

			// Token: 0x04009C5B RID: 40027
			public readonly int Hashcode;

			// Token: 0x04009C5C RID: 40028
			public volatile ConcurrentHashSet<T>.Node Next;
		}
	}
}
