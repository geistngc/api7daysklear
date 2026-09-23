using System;
using System.Collections.Generic;

// Token: 0x0200141D RID: 5149
public class HashSetList<T>
{
	// Token: 0x0600A1BF RID: 41407 RVA: 0x003CD05F File Offset: 0x003CB25F
	public void Add(T _value)
	{
		if (this.hashSet.Add(_value))
		{
			this.list.Add(_value);
		}
	}

	// Token: 0x0600A1C0 RID: 41408 RVA: 0x003CD07B File Offset: 0x003CB27B
	public void Remove(T _value)
	{
		if (this.hashSet.Remove(_value))
		{
			this.list.Remove(_value);
		}
	}

	// Token: 0x0600A1C1 RID: 41409 RVA: 0x003CD098 File Offset: 0x003CB298
	public void Clear()
	{
		this.list.Clear();
		this.hashSet.Clear();
	}

	// Token: 0x04007A00 RID: 31232
	public HashSet<T> hashSet = new HashSet<T>();

	// Token: 0x04007A01 RID: 31233
	public List<T> list = new List<T>();
}
