using System;
using System.Collections.Generic;

// Token: 0x02000977 RID: 2423
public class BaseQuestData
{
	// Token: 0x060046D5 RID: 18133 RVA: 0x001BF864 File Offset: 0x001BDA64
	public void AddSharedQuester(int _entityID)
	{
		if (!this.entityList.Contains(_entityID))
		{
			this.entityList.Add(_entityID);
			EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(_entityID) as EntityPlayer;
			if (entityPlayer != null)
			{
				this.OnAdd(entityPlayer);
			}
		}
	}

	// Token: 0x060046D6 RID: 18134 RVA: 0x001BF8AC File Offset: 0x001BDAAC
	public void RemoveSharedQuester(EntityPlayer _player)
	{
		if (this.entityList.Contains(_player.entityId))
		{
			this.entityList.Remove(_player.entityId);
		}
		if (this.entityList.Count == 0)
		{
			this.OnRemove(_player);
			this.RemoveFromDictionary();
		}
	}

	// Token: 0x060046D7 RID: 18135 RVA: 0x001BF8F8 File Offset: 0x001BDAF8
	public bool ContainsEntity(int _entityID)
	{
		return this.entityList.Contains(_entityID);
	}

	// Token: 0x060046D8 RID: 18136 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetModifier(string _name)
	{
	}

	// Token: 0x060046D9 RID: 18137 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RemoveFromDictionary()
	{
	}

	// Token: 0x060046DA RID: 18138 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCreated()
	{
	}

	// Token: 0x060046DB RID: 18139 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnAdd(EntityPlayer player)
	{
	}

	// Token: 0x060046DC RID: 18140 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnRemove(EntityPlayer player)
	{
	}

	// Token: 0x060046DD RID: 18141 RVA: 0x001BF906 File Offset: 0x001BDB06
	public void Remove()
	{
		this.entityList.Clear();
		this.OnRemove(null);
	}

	// Token: 0x040038EF RID: 14575
	[PublicizedFrom(EAccessModifier.Protected)]
	public int questCode;

	// Token: 0x040038F0 RID: 14576
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<int> entityList = new List<int>();
}
