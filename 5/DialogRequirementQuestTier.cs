using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020002FB RID: 763
[Preserve]
public class DialogRequirementQuestTier : BaseDialogRequirement
{
	// Token: 0x17000275 RID: 629
	// (get) Token: 0x060015BA RID: 5562 RVA: 0x00046EF6 File Offset: 0x000450F6
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestTier;
		}
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00032163 File Offset: 0x00030363
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00081994 File Offset: 0x0007FB94
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		EntityTrader entityTrader = talkingTo as EntityTrader;
		if (entityTrader == null)
		{
			return false;
		}
		int num = StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
		if (player.QuestJournal.GetCurrentFactionTier(entityTrader.NPCInfo.QuestFaction, 0, false) < num)
		{
			return false;
		}
		for (int i = 0; i < entityTrader.activeQuests.Count; i++)
		{
			if ((int)entityTrader.activeQuests[i].QuestClass.DifficultyTier == num && entityTrader.activeQuests[i].QuestClass.UniqueKey == base.Tag)
			{
				return true;
			}
		}
		return false;
	}
}
