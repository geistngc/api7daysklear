using System;
using UnityEngine.Scripting;

// Token: 0x020002E6 RID: 742
[Preserve]
public class DialogActionCompleteQuest : BaseDialogAction
{
	// Token: 0x1700025D RID: 605
	// (get) Token: 0x0600155D RID: 5469 RVA: 0x00046EF6 File Offset: 0x000450F6
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.CompleteQuest;
		}
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000807E0 File Offset: 0x0007E9E0
	public override void PerformAction(EntityPlayer player)
	{
		if (base.ID != "")
		{
			Quest quest = player.QuestJournal.FindQuest(base.ID, (int)base.OwnerDialog.CurrentOwner.NPCInfo.QuestFaction);
			Convert.ToInt32(base.Value);
			if (quest != null && quest.Active)
			{
				QuestEventManager.Current.NPCInteracted(base.OwnerDialog.CurrentOwner);
				quest.RefreshQuestCompletion(QuestClass.CompletionTypes.TurnIn, null, true, null);
			}
		}
	}
}
