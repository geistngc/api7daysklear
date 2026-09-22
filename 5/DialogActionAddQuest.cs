using System;
using UnityEngine.Scripting;

// Token: 0x020002E4 RID: 740
[Preserve]
public class DialogActionAddQuest : BaseDialogAction
{
	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06001558 RID: 5464 RVA: 0x0002F184 File Offset: 0x0002D384
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddQuest;
		}
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x000806B0 File Offset: 0x0007E8B0
	public override void PerformAction(EntityPlayer player)
	{
		if (this.Quest != null && this.Quest.QuestClass != null)
		{
			Quest quest = player.QuestJournal.FindNonSharedQuest(this.Quest.QuestCode);
			if (quest == null || !quest.Active)
			{
				LocalPlayerUI playerUI = LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
				XUiC_QuestOfferWindow.OpenQuestOfferWindow(playerUI.xui, this.Quest, this.ListIndex, XUiC_QuestOfferWindow.OfferTypes.Dialog, playerUI.xui.Dialog.Respondent.entityId, delegate(EntityNPC npc)
				{
					playerUI.xui.Dialog.Respondent = npc;
					playerUI.xui.Dialog.ReturnStatement = ((DialogResponseQuest)this.Owner).LastStatementID;
				});
				return;
			}
			GameManager.ShowTooltip((EntityPlayerLocal)player, Localization.Get("questunavailable", false, null), false, false, 0f);
		}
	}

	// Token: 0x04000E57 RID: 3671
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";

	// Token: 0x04000E58 RID: 3672
	public Quest Quest;

	// Token: 0x04000E59 RID: 3673
	public int ListIndex;
}
