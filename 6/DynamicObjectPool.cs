using System;
using System.Collections.Generic;

// Token: 0x02001448 RID: 5192
public class DynamicObjectPool<T> where T : new()
{
	// Token: 0x0600A310 RID: 41744 RVA: 0x003D1C68 File Offset: 0x003CFE68
	public DynamicObjectPool(int objectsPerBlock)
	{
		this.Init(objectsPerBlock, int.MaxValue);
	}

	// Token: 0x0600A311 RID: 41745 RVA: 0x003D1C7C File Offset: 0x003CFE7C
	public DynamicObjectPool(int objectsPerBlock, int maxObjects)
	{
		this.Init(objectsPerBlock, maxObjects);
	}

	// Token: 0x0600A312 RID: 41746 RVA: 0x003D1C8C File Offset: 0x003CFE8C
	public void AllocateBlock(int numObjects)
	{
		for (int i = 0; i < numObjects; i++)
		{
			this.push(Activator.CreateInstance<T>());
		}
		this.m_numAllocatedObjects += numObjects;
	}

	// Token: 0x0600A313 RID: 41747 RVA: 0x003D1CBE File Offset: 0x003CFEBE
	public T Allocate()
	{
		if (this.m_numFreeObjects < 1)
		{
			this.AllocateBlock(this.m_numObjectsPerBlock);
		}
		this.m_numUsedObjects++;
		return this.pop();
	}

	// Token: 0x0600A314 RID: 41748 RVA: 0x003D1CEC File Offset: 0x003CFEEC
	public T[] Allocate(int numToAllocate)
	{
		if (numToAllocate < 1)
		{
			return null;
		}
		T[] array = new T[numToAllocate];
		while (this.m_numFreeObjects < numToAllocate)
		{
			this.AllocateBlock(this.m_numObjectsPerBlock);
		}
		for (int i = 0; i < numToAllocate; i++)
		{
			array[i] = this.Allocate();
		}
		return array;
	}

	// Token: 0x0600A315 RID: 41749 RVA: 0x003D1D37 File Offset: 0x003CFF37
	public void Free(T obj)
	{
		this.push(obj);
		this.m_numUsedObjects--;
	}

	// Token: 0x0600A316 RID: 41750 RVA: 0x003D1D50 File Offset: 0x003CFF50
	public void Free(T[] array)
	{
		foreach (T obj in array)
		{
			this.Free(obj);
		}
	}

	// Token: 0x0600A317 RID: 41751 RVA: 0x003D1D7C File Offset: 0x003CFF7C
	public void Compact()
	{
		this.m_numFreeObjects = 0;
		this.m_numAllocatedObjects -= this.m_freeObjects.Count;
		this.m_freeObjects = new List<T>(this.m_numObjectsPerBlock);
	}

	// Token: 0x17001328 RID: 4904
	// (get) Token: 0x0600A318 RID: 41752 RVA: 0x003D1DAE File Offset: 0x003CFFAE
	public int NumAllocatedObjects
	{
		get
		{
			return this.m_numAllocatedObjects;
		}
	}

	// Token: 0x17001329 RID: 4905
	// (get) Token: 0x0600A319 RID: 41753 RVA: 0x003D1DB6 File Offset: 0x003CFFB6
	public int NumUsedObjects
	{
		get
		{
			return this.m_numUsedObjects;
		}
	}

	// Token: 0x1700132A RID: 4906
	// (get) Token: 0x0600A31A RID: 41754 RVA: 0x003D1DBE File Offset: 0x003CFFBE
	public int NumFreeObjects
	{
		get
		{
			return this.m_numFreeObjects;
		}
	}

	// Token: 0x1700132B RID: 4907
	// (get) Token: 0x0600A31B RID: 41755 RVA: 0x003D1DC6 File Offset: 0x003CFFC6
	public int MaxObjects
	{
		get
		{
			return this.m_maxObjects;
		}
	}

	// Token: 0x0600A31C RID: 41756 RVA: 0x003D1DCE File Offset: 0x003CFFCE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init(int objectsPerBlock, int maxObjects)
	{
		this.m_numObjectsPerBlock = objectsPerBlock;
		this.m_maxObjects = maxObjects;
		this.m_freeObjects = new List<T>(objectsPerBlock);
	}

	// Token: 0x0600A31D RID: 41757 RVA: 0x003D1DEC File Offset: 0x003CFFEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void push(T t)
	{
		if (this.m_numFreeObjects >= this.m_freeObjects.Count)
		{
			this.m_freeObjects.Add(t);
		}
		else
		{
			this.m_freeObjects[this.m_numFreeObjects] = t;
		}
		this.m_numFreeObjects++;
	}

	// Token: 0x0600A31E RID: 41758 RVA: 0x003D1E3C File Offset: 0x003D003C
	[PublicizedFrom(EAccessModifier.Private)]
	public T pop()
	{
		if (this.m_numFreeObjects < 1)
		{
			return default(T);
		}
		List<T> freeObjects = this.m_freeObjects;
		int num = this.m_numFreeObjects - 1;
		this.m_numFreeObjects = num;
		return freeObjects[num];
	}

	// Token: 0x04007A86 RID: 31366
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numAllocatedObjects;

	// Token: 0x04007A87 RID: 31367
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numUsedObjects;

	// Token: 0x04007A88 RID: 31368
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numFreeObjects;

	// Token: 0x04007A89 RID: 31369
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numObjectsPerBlock;

	// Token: 0x04007A8A RID: 31370
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_maxObjects;

	// Token: 0x04007A8B RID: 31371
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> m_freeObjects;
}
