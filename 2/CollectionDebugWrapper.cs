using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020013D0 RID: 5072
public class CollectionDebugWrapper<T> : EnumerableDebugWrapper<T>, ICollection<T>, IEnumerable<!0>, IEnumerable
{
	// Token: 0x06009F65 RID: 40805 RVA: 0x003C3BFC File Offset: 0x003C1DFC
	public CollectionDebugWrapper(ICollection<T> collection) : this(null, collection)
	{
	}

	// Token: 0x06009F66 RID: 40806 RVA: 0x003C3C06 File Offset: 0x003C1E06
	public CollectionDebugWrapper(DebugWrapper parent, ICollection<T> collection) : base(parent, collection)
	{
		this.m_collection = collection;
	}

	// Token: 0x06009F67 RID: 40807 RVA: 0x003C3C18 File Offset: 0x003C1E18
	public void Add(T item)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_collection.Add(item);
		}
	}

	// Token: 0x06009F68 RID: 40808 RVA: 0x003C3C54 File Offset: 0x003C1E54
	public void Clear()
	{
		using (base.DebugReadWriteScope())
		{
			this.m_collection.Clear();
		}
	}

	// Token: 0x06009F69 RID: 40809 RVA: 0x003C3C90 File Offset: 0x003C1E90
	public bool Contains(T item)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_collection.Contains(item);
		}
		return result;
	}

	// Token: 0x06009F6A RID: 40810 RVA: 0x003C3CD0 File Offset: 0x003C1ED0
	public void CopyTo(T[] array, int arrayIndex)
	{
		using (base.DebugReadScope())
		{
			this.m_collection.CopyTo(array, arrayIndex);
		}
	}

	// Token: 0x06009F6B RID: 40811 RVA: 0x003C3D10 File Offset: 0x003C1F10
	public bool Remove(T item)
	{
		bool result;
		using (base.DebugReadWriteScope())
		{
			result = this.m_collection.Remove(item);
		}
		return result;
	}

	// Token: 0x170012CD RID: 4813
	// (get) Token: 0x06009F6C RID: 40812 RVA: 0x003C3D50 File Offset: 0x003C1F50
	public int Count
	{
		get
		{
			return this.m_collection.Count;
		}
	}

	// Token: 0x170012CE RID: 4814
	// (get) Token: 0x06009F6D RID: 40813 RVA: 0x003C3D5D File Offset: 0x003C1F5D
	public bool IsReadOnly
	{
		get
		{
			return this.m_collection.IsReadOnly;
		}
	}

	// Token: 0x0400791E RID: 31006
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ICollection<T> m_collection;
}
