using System;
using System.Collections.Generic;

// Token: 0x020008A6 RID: 2214
public class CompanionGroup
{
	// Token: 0x170006AF RID: 1711
	public EntityAlive this[int index]
	{
		get
		{
			return this.MemberList[index];
		}
	}

	// Token: 0x170006B0 RID: 1712
	// (get) Token: 0x0600404F RID: 16463 RVA: 0x001928BB File Offset: 0x00190ABB
	public int Count
	{
		get
		{
			return this.MemberList.Count;
		}
	}

	// Token: 0x06004050 RID: 16464 RVA: 0x001928C8 File Offset: 0x00190AC8
	public void Add(EntityAlive entity)
	{
		this.MemberList.Add(entity);
		OnCompanionGroupChanged onGroupChanged = this.OnGroupChanged;
		if (onGroupChanged == null)
		{
			return;
		}
		onGroupChanged();
	}

	// Token: 0x06004051 RID: 16465 RVA: 0x001928E6 File Offset: 0x00190AE6
	public void Remove(EntityAlive entity)
	{
		this.MemberList.Remove(entity);
		OnCompanionGroupChanged onGroupChanged = this.OnGroupChanged;
		if (onGroupChanged == null)
		{
			return;
		}
		onGroupChanged();
	}

	// Token: 0x06004052 RID: 16466 RVA: 0x00192905 File Offset: 0x00190B05
	public int IndexOf(EntityAlive entity)
	{
		return this.MemberList.IndexOf(entity);
	}

	// Token: 0x040033DE RID: 13278
	public OnCompanionGroupChanged OnGroupChanged;

	// Token: 0x040033DF RID: 13279
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> MemberList = new List<EntityAlive>();
}
