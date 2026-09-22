using System;
using UnityEngine.Scripting;

// Token: 0x02000973 RID: 2419
[Preserve]
public class BaseQuestCriteria
{
	// Token: 0x060046CD RID: 18125 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void HandleVariables()
	{
	}

	// Token: 0x060046CE RID: 18126 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CheckForQuestGiver(EntityNPC entity)
	{
		return true;
	}

	// Token: 0x060046CF RID: 18127 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CheckForPlayer(EntityPlayer player)
	{
		return true;
	}

	// Token: 0x040038E8 RID: 14568
	public string ID;

	// Token: 0x040038E9 RID: 14569
	public string Value;

	// Token: 0x040038EA RID: 14570
	public QuestClass OwnerQuestClass;

	// Token: 0x040038EB RID: 14571
	public BaseQuestCriteria.CriteriaTypes CriteriaType;

	// Token: 0x02000974 RID: 2420
	public enum CriteriaTypes
	{
		// Token: 0x040038ED RID: 14573
		QuestGiver,
		// Token: 0x040038EE RID: 14574
		Player
	}
}
