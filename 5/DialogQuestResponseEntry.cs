using System;
using UnityEngine.Scripting;

// Token: 0x02000301 RID: 769
[Preserve]
public class DialogQuestResponseEntry : BaseResponseEntry
{
	// Token: 0x060015CC RID: 5580 RVA: 0x00081BD0 File Offset: 0x0007FDD0
	public DialogQuestResponseEntry(string _questID, string _type, string _returnStatementID, int _listIndex, int _tier)
	{
		base.ID = _questID;
		this.ListIndex = _listIndex;
		base.ResponseType = BaseResponseEntry.ResponseTypes.QuestAdd;
		this.questType = _type;
		this.Tier = _tier;
		this.ReturnStatementID = _returnStatementID;
	}

	// Token: 0x04000EA5 RID: 3749
	public int ListIndex = -1;

	// Token: 0x04000EA6 RID: 3750
	public string ReturnStatementID = "";

	// Token: 0x04000EA7 RID: 3751
	public string questType = "";

	// Token: 0x04000EA8 RID: 3752
	public int Tier = -1;
}
