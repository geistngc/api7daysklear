using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	// Token: 0x020016BC RID: 5820
	[DebuggerDisplay("Count={Count}")]
	[Serializable]
	public class HashSetLong : ICollection<long>, IEnumerable<long>, IEnumerable, ISerializable, IDeserializationCallback
	{
		// Token: 0x17001629 RID: 5673
		// (get) Token: 0x0600B5D2 RID: 46546 RVA: 0x0043C12B File Offset: 0x0043A32B
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x0600B5D3 RID: 46547 RVA: 0x0043C133 File Offset: 0x0043A333
		public HashSetLong()
		{
			this.Init(10, null);
		}

		// Token: 0x0600B5D4 RID: 46548 RVA: 0x0043C144 File Offset: 0x0043A344
		public HashSetLong(IEqualityComparer<long> comparer)
		{
			this.Init(10, comparer);
		}

		// Token: 0x0600B5D5 RID: 46549 RVA: 0x0043C155 File Offset: 0x0043A355
		public HashSetLong(IEnumerable<long> collection) : this(collection, null)
		{
		}

		// Token: 0x0600B5D6 RID: 46550 RVA: 0x0043C160 File Offset: 0x0043A360
		public HashSetLong(IEnumerable<long> collection, IEqualityComparer<long> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int capacity = 0;
			ICollection<long> collection2 = collection as ICollection<long>;
			if (collection2 != null)
			{
				capacity = collection2.Count;
			}
			this.Init(capacity, comparer);
			foreach (long item in collection)
			{
				this.Add(item);
			}
		}

		// Token: 0x0600B5D7 RID: 46551 RVA: 0x0043C1D8 File Offset: 0x0043A3D8
		[PublicizedFrom(EAccessModifier.Protected)]
		public HashSetLong(SerializationInfo info, StreamingContext context)
		{
			this.si = info;
		}

		// Token: 0x0600B5D8 RID: 46552 RVA: 0x0043C1E8 File Offset: 0x0043A3E8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init(int capacity, IEqualityComparer<long> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.comparer = (comparer ?? EqualityComparer<long>.Default);
			if (capacity == 0)
			{
				capacity = 10;
			}
			capacity = (int)((float)capacity / 0.9f) + 1;
			this.InitArrays(capacity);
			this.generation = 0;
		}

		// Token: 0x0600B5D9 RID: 46553 RVA: 0x0043C238 File Offset: 0x0043A438
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitArrays(int size)
		{
			this.table = new int[size];
			this.links = new HashSetLong.Link[size];
			this.empty_slot = -1;
			this.slots = new long[size];
			this.touched = 0;
			this.threshold = (int)((float)this.table.Length * 0.9f);
			if (this.threshold == 0 && this.table.Length != 0)
			{
				this.threshold = 1;
			}
		}

		// Token: 0x0600B5DA RID: 46554 RVA: 0x0043C2A8 File Offset: 0x0043A4A8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool SlotsContainsAt(int index, int hash, long item)
		{
			HashSetLong.Link link;
			for (int num = this.table[index] - 1; num != -1; num = link.Next)
			{
				link = this.links[num];
				if (link.HashCode == hash && item == this.slots[num])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B5DB RID: 46555 RVA: 0x0043C2F1 File Offset: 0x0043A4F1
		public void CopyTo(long[] array)
		{
			this.CopyTo(array, 0, this.count);
		}

		// Token: 0x0600B5DC RID: 46556 RVA: 0x0043C301 File Offset: 0x0043A501
		public void CopyTo(long[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, this.count);
		}

		// Token: 0x0600B5DD RID: 46557 RVA: 0x0043C314 File Offset: 0x0043A514
		public void CopyTo(long[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (arrayIndex > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
			int num = 0;
			int num2 = 0;
			while (num < this.touched && num2 < count)
			{
				if (this.GetLinkHashCode(num) != 0)
				{
					array[arrayIndex++] = this.slots[num];
				}
				num++;
			}
		}

		// Token: 0x0600B5DE RID: 46558 RVA: 0x0043C394 File Offset: 0x0043A594
		[PublicizedFrom(EAccessModifier.Private)]
		public void Resize()
		{
			int num = HashSetLong.PrimeHelper.ToPrime(this.table.Length << 1 | 1);
			int[] array = new int[num];
			HashSetLong.Link[] array2 = new HashSetLong.Link[num];
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.links[num2].Next)
				{
					int num3 = ((array2[num2].HashCode = (((int)this.slots[num2] ^ (int)(this.slots[num2] >> 32)) | int.MinValue)) & int.MaxValue) % num;
					array2[num2].Next = array[num3] - 1;
					array[num3] = num2 + 1;
				}
			}
			this.table = array;
			this.links = array2;
			long[] destinationArray = new long[num];
			Array.Copy(this.slots, 0, destinationArray, 0, this.touched);
			this.slots = destinationArray;
			this.threshold = (int)((float)num * 0.9f);
		}

		// Token: 0x0600B5DF RID: 46559 RVA: 0x0043C49A File Offset: 0x0043A69A
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetLinkHashCode(int index)
		{
			return this.links[index].HashCode & int.MinValue;
		}

		// Token: 0x0600B5E0 RID: 46560 RVA: 0x0043C4B4 File Offset: 0x0043A6B4
		public bool Add(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			if (this.SlotsContainsAt(num2, num, item))
			{
				return false;
			}
			int num3 = this.count + 1;
			this.count = num3;
			if (num3 > this.threshold)
			{
				this.Resize();
				num2 = (num & int.MaxValue) % this.table.Length;
			}
			int num4 = this.empty_slot;
			if (num4 == -1)
			{
				num3 = this.touched;
				this.touched = num3 + 1;
				num4 = num3;
			}
			else
			{
				this.empty_slot = this.links[num4].Next;
			}
			this.links[num4].HashCode = num;
			this.links[num4].Next = this.table[num2] - 1;
			this.table[num2] = num4 + 1;
			this.slots[num4] = item;
			this.generation++;
			return true;
		}

		// Token: 0x1700162A RID: 5674
		// (get) Token: 0x0600B5E1 RID: 46561 RVA: 0x0043C5A4 File Offset: 0x0043A7A4
		public IEqualityComparer<long> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		// Token: 0x0600B5E2 RID: 46562 RVA: 0x0043C5AC File Offset: 0x0043A7AC
		public void Clear()
		{
			this.count = 0;
			Array.Clear(this.table, 0, this.table.Length);
			Array.Clear(this.slots, 0, this.slots.Length);
			Array.Clear(this.links, 0, this.links.Length);
			this.empty_slot = -1;
			this.touched = 0;
			this.generation++;
		}

		// Token: 0x0600B5E3 RID: 46563 RVA: 0x0043C618 File Offset: 0x0043A818
		public bool Contains(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int index = (num & int.MaxValue) % this.table.Length;
			return this.SlotsContainsAt(index, num, item);
		}

		// Token: 0x0600B5E4 RID: 46564 RVA: 0x0043C650 File Offset: 0x0043A850
		public bool Remove(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			int num3 = this.table[num2] - 1;
			if (num3 == -1)
			{
				return false;
			}
			int num4 = -1;
			do
			{
				HashSetLong.Link link = this.links[num3];
				if (link.HashCode == num && this.slots[num3] == item)
				{
					break;
				}
				num4 = num3;
				num3 = link.Next;
			}
			while (num3 != -1);
			if (num3 == -1)
			{
				return false;
			}
			this.count--;
			if (num4 == -1)
			{
				this.table[num2] = this.links[num3].Next + 1;
			}
			else
			{
				this.links[num4].Next = this.links[num3].Next;
			}
			this.links[num3].Next = this.empty_slot;
			this.empty_slot = num3;
			this.links[num3].HashCode = 0;
			this.slots[num3] = 0L;
			this.generation++;
			return true;
		}

		// Token: 0x0600B5E5 RID: 46565 RVA: 0x0043C764 File Offset: 0x0043A964
		public int RemoveWhere(Predicate<long> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<long> list = new List<long>();
			foreach (long num in this)
			{
				if (match(num))
				{
					list.Add(num);
				}
			}
			foreach (long item in list)
			{
				this.Remove(item);
			}
			return list.Count;
		}

		// Token: 0x0600B5E6 RID: 46566 RVA: 0x0043C818 File Offset: 0x0043AA18
		public void TrimExcess()
		{
			this.Resize();
		}

		// Token: 0x0600B5E7 RID: 46567 RVA: 0x0043C820 File Offset: 0x0043AA20
		public void IntersectWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong other_set = this.ToSet(other);
			this.RemoveWhere((long item) => !other_set.Contains(item));
		}

		// Token: 0x0600B5E8 RID: 46568 RVA: 0x0043C864 File Offset: 0x0043AA64
		public void ExceptWithHashSetLong(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Remove(item);
			}
		}

		// Token: 0x0600B5E9 RID: 46569 RVA: 0x0043C8C4 File Offset: 0x0043AAC4
		public void ExceptWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Remove(item);
			}
		}

		// Token: 0x0600B5EA RID: 46570 RVA: 0x0043C91C File Offset: 0x0043AB1C
		public bool Overlaps(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				if (this.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B5EB RID: 46571 RVA: 0x0043C97C File Offset: 0x0043AB7C
		public bool SetEquals(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			if (this.count != hashSetLong.Count)
			{
				return false;
			}
			foreach (long item in this)
			{
				if (!hashSetLong.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B5EC RID: 46572 RVA: 0x0043C9FC File Offset: 0x0043ABFC
		public void SymmetricExceptWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in this.ToSet(other))
			{
				if (!this.Add(item))
				{
					this.Remove(item);
				}
			}
		}

		// Token: 0x0600B5ED RID: 46573 RVA: 0x0043CA68 File Offset: 0x0043AC68
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetLong ToSet(IEnumerable<long> enumerable)
		{
			HashSetLong hashSetLong = enumerable as HashSetLong;
			if (hashSetLong == null || !this.Comparer.Equals(hashSetLong.Comparer))
			{
				hashSetLong = new HashSetLong(enumerable, this.Comparer);
			}
			return hashSetLong;
		}

		// Token: 0x0600B5EE RID: 46574 RVA: 0x0043CAA0 File Offset: 0x0043ACA0
		public void UnionWithHashSetLong(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Add(item);
			}
		}

		// Token: 0x0600B5EF RID: 46575 RVA: 0x0043CB00 File Offset: 0x0043AD00
		public void UnionWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Add(item);
			}
		}

		// Token: 0x0600B5F0 RID: 46576 RVA: 0x0043CB58 File Offset: 0x0043AD58
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckIsSubsetOf(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in this)
			{
				if (!other.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B5F1 RID: 46577 RVA: 0x0043CBC0 File Offset: 0x0043ADC0
		public bool IsSubsetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.count == 0)
			{
				return true;
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count <= hashSetLong.Count && this.CheckIsSubsetOf(hashSetLong);
		}

		// Token: 0x0600B5F2 RID: 46578 RVA: 0x0043CC04 File Offset: 0x0043AE04
		public bool IsProperSubsetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.count == 0)
			{
				return true;
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count < hashSetLong.Count && this.CheckIsSubsetOf(hashSetLong);
		}

		// Token: 0x0600B5F3 RID: 46579 RVA: 0x0043CC48 File Offset: 0x0043AE48
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckIsSupersetOf(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				if (!this.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B5F4 RID: 46580 RVA: 0x0043CCB0 File Offset: 0x0043AEB0
		public bool IsSupersetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count >= hashSetLong.Count && this.CheckIsSupersetOf(hashSetLong);
		}

		// Token: 0x0600B5F5 RID: 46581 RVA: 0x0043CCEC File Offset: 0x0043AEEC
		public bool IsProperSupersetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count > hashSetLong.Count && this.CheckIsSupersetOf(hashSetLong);
		}

		// Token: 0x0600B5F6 RID: 46582 RVA: 0x0043CD26 File Offset: 0x0043AF26
		public static IEqualityComparer<HashSetLong> CreateSetComparer()
		{
			return HashSetLong.setComparer;
		}

		// Token: 0x0600B5F7 RID: 46583 RVA: 0x0043CD30 File Offset: 0x0043AF30
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this.generation);
			info.AddValue("Comparer", this.comparer, typeof(IEqualityComparer<long>));
			info.AddValue("Capacity", (this.table == null) ? 0 : this.table.Length);
			if (this.table != null)
			{
				long[] array = new long[this.count];
				this.CopyTo(array);
				info.AddValue("Elements", array, typeof(long[]));
			}
		}

		// Token: 0x0600B5F8 RID: 46584 RVA: 0x0043CDC8 File Offset: 0x0043AFC8
		public virtual void OnDeserialization(object sender)
		{
			if (this.si != null)
			{
				this.generation = (int)this.si.GetValue("Version", typeof(int));
				this.comparer = (IEqualityComparer<long>)this.si.GetValue("Comparer", typeof(IEqualityComparer<long>));
				int num = (int)this.si.GetValue("Capacity", typeof(int));
				this.empty_slot = -1;
				if (num > 0)
				{
					this.table = new int[num];
					this.slots = new long[num];
					long[] array = (long[])this.si.GetValue("Elements", typeof(long[]));
					if (array == null)
					{
						throw new SerializationException("Missing Elements");
					}
					for (int i = 0; i < array.Length; i++)
					{
						this.Add(array[i]);
					}
				}
				else
				{
					this.table = null;
				}
				this.si = null;
			}
		}

		// Token: 0x0600B5F9 RID: 46585 RVA: 0x0043CEC3 File Offset: 0x0043B0C3
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator<long> GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x1700162B RID: 5675
		// (get) Token: 0x0600B5FA RID: 46586 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsReadOnly
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return false;
			}
		}

		// Token: 0x0600B5FB RID: 46587 RVA: 0x0043CED0 File Offset: 0x0043B0D0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Add(long item)
		{
			this.Add(item);
		}

		// Token: 0x0600B5FC RID: 46588 RVA: 0x0043CEC3 File Offset: 0x0043B0C3
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x0600B5FD RID: 46589 RVA: 0x0043CEDA File Offset: 0x0043B0DA
		public HashSetLong.Enumerator GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x04008866 RID: 34918
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int INITIAL_SIZE = 10;

		// Token: 0x04008867 RID: 34919
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const float DEFAULT_LOAD_FACTOR = 0.9f;

		// Token: 0x04008868 RID: 34920
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int NO_SLOT = -1;

		// Token: 0x04008869 RID: 34921
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int HASH_FLAG = -2147483648;

		// Token: 0x0400886A RID: 34922
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int[] table;

		// Token: 0x0400886B RID: 34923
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public HashSetLong.Link[] links;

		// Token: 0x0400886C RID: 34924
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public long[] slots;

		// Token: 0x0400886D RID: 34925
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int touched;

		// Token: 0x0400886E RID: 34926
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int empty_slot;

		// Token: 0x0400886F RID: 34927
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int count;

		// Token: 0x04008870 RID: 34928
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int threshold;

		// Token: 0x04008871 RID: 34929
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public IEqualityComparer<long> comparer;

		// Token: 0x04008872 RID: 34930
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public SerializationInfo si;

		// Token: 0x04008873 RID: 34931
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int generation;

		// Token: 0x04008874 RID: 34932
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static readonly HashSetLong.HashSetEqualityComparer setComparer = new HashSetLong.HashSetEqualityComparer();

		// Token: 0x020016BD RID: 5821
		[PublicizedFrom(EAccessModifier.Private)]
		public struct Link
		{
			// Token: 0x04008875 RID: 34933
			public int HashCode;

			// Token: 0x04008876 RID: 34934
			public int Next;
		}

		// Token: 0x020016BE RID: 5822
		[PublicizedFrom(EAccessModifier.Private)]
		public class HashSetEqualityComparer : IEqualityComparer<HashSetLong>
		{
			// Token: 0x0600B5FF RID: 46591 RVA: 0x0043CEF0 File Offset: 0x0043B0F0
			public bool Equals(HashSetLong lhs, HashSetLong rhs)
			{
				if (lhs == rhs)
				{
					return true;
				}
				if (lhs == null || rhs == null || lhs.Count != rhs.Count)
				{
					return false;
				}
				foreach (long item in lhs)
				{
					if (!rhs.Contains(item))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x0600B600 RID: 46592 RVA: 0x0043CF64 File Offset: 0x0043B164
			public int GetHashCode(HashSetLong hashset)
			{
				if (hashset == null)
				{
					return 0;
				}
				IEqualityComparer<long> @default = EqualityComparer<long>.Default;
				int num = 0;
				foreach (long obj in hashset)
				{
					num ^= @default.GetHashCode(obj);
				}
				return num;
			}
		}

		// Token: 0x020016BF RID: 5823
		[Serializable]
		public struct Enumerator : IEnumerator<long>, IEnumerator, IDisposable
		{
			// Token: 0x0600B602 RID: 46594 RVA: 0x0043CFC4 File Offset: 0x0043B1C4
			[PublicizedFrom(EAccessModifier.Internal)]
			public Enumerator(HashSetLong hashset)
			{
				this = default(HashSetLong.Enumerator);
				this.hashset = hashset;
				this.stamp = hashset.generation;
			}

			// Token: 0x0600B603 RID: 46595 RVA: 0x0043CFE0 File Offset: 0x0043B1E0
			public bool MoveNext()
			{
				this.CheckState();
				if (this.next < 0)
				{
					return false;
				}
				while (this.next < this.hashset.touched)
				{
					int num = this.next;
					this.next = num + 1;
					int num2 = num;
					if (this.hashset.GetLinkHashCode(num2) != 0)
					{
						this.current = this.hashset.slots[num2];
						return true;
					}
				}
				this.next = -1;
				return false;
			}

			// Token: 0x1700162C RID: 5676
			// (get) Token: 0x0600B604 RID: 46596 RVA: 0x0043D04E File Offset: 0x0043B24E
			public long Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x1700162D RID: 5677
			// (get) Token: 0x0600B605 RID: 46597 RVA: 0x0043D056 File Offset: 0x0043B256
			public object Current
			{
				[PublicizedFrom(EAccessModifier.Private)]
				get
				{
					this.CheckState();
					if (this.next <= 0)
					{
						throw new InvalidOperationException("Current is not valid");
					}
					return this.current;
				}
			}

			// Token: 0x0600B606 RID: 46598 RVA: 0x0043D07D File Offset: 0x0043B27D
			[PublicizedFrom(EAccessModifier.Private)]
			public void Reset()
			{
				this.CheckState();
				this.next = 0;
			}

			// Token: 0x0600B607 RID: 46599 RVA: 0x0043D08C File Offset: 0x0043B28C
			public void Dispose()
			{
				this.hashset = null;
			}

			// Token: 0x0600B608 RID: 46600 RVA: 0x0043D095 File Offset: 0x0043B295
			[PublicizedFrom(EAccessModifier.Private)]
			public void CheckState()
			{
				if (this.hashset == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.hashset.generation != this.stamp)
				{
					throw new InvalidOperationException("HashSet have been modified while it was iterated over");
				}
			}

			// Token: 0x04008877 RID: 34935
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public HashSetLong hashset;

			// Token: 0x04008878 RID: 34936
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public int next;

			// Token: 0x04008879 RID: 34937
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public int stamp;

			// Token: 0x0400887A RID: 34938
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public long current;
		}

		// Token: 0x020016C0 RID: 5824
		[PublicizedFrom(EAccessModifier.Private)]
		public static class PrimeHelper
		{
			// Token: 0x0600B609 RID: 46601 RVA: 0x0043D0C4 File Offset: 0x0043B2C4
			[PublicizedFrom(EAccessModifier.Private)]
			public static bool TestPrime(int x)
			{
				if ((x & 1) != 0)
				{
					int num = (int)Math.Sqrt((double)x);
					for (int i = 3; i < num; i += 2)
					{
						if (x % i == 0)
						{
							return false;
						}
					}
					return true;
				}
				return x == 2;
			}

			// Token: 0x0600B60A RID: 46602 RVA: 0x0043D0F8 File Offset: 0x0043B2F8
			[PublicizedFrom(EAccessModifier.Private)]
			public static int CalcPrime(int x)
			{
				for (int i = (x & -2) - 1; i < 2147483647; i += 2)
				{
					if (HashSetLong.PrimeHelper.TestPrime(i))
					{
						return i;
					}
				}
				return x;
			}

			// Token: 0x0600B60B RID: 46603 RVA: 0x0043D128 File Offset: 0x0043B328
			public static int ToPrime(int x)
			{
				for (int i = 0; i < HashSetLong.PrimeHelper.primes_table.Length; i++)
				{
					if (x <= HashSetLong.PrimeHelper.primes_table[i])
					{
						return HashSetLong.PrimeHelper.primes_table[i];
					}
				}
				return HashSetLong.PrimeHelper.CalcPrime(x);
			}

			// Token: 0x0400887B RID: 34939
			[PublicizedFrom(EAccessModifier.Private)]
			public static readonly int[] primes_table = new int[]
			{
				11,
				19,
				37,
				73,
				109,
				163,
				251,
				367,
				557,
				823,
				1237,
				1861,
				2777,
				4177,
				6247,
				9371,
				14057,
				21089,
				31627,
				47431,
				71143,
				106721,
				160073,
				240101,
				360163,
				540217,
				810343,
				1215497,
				1823231,
				2734867,
				4102283,
				6153409,
				9230113,
				13845163
			};
		}
	}
}
