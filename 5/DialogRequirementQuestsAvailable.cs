using System;
using SandboxOptions;
using UnityEngine.Scripting;

// Token: 0x020002F8 RID: 760
[Preserve]
public class DialogRequirementQuestsAvailable : BaseDialogRequirement
{
	// Token: 0x17000273 RID: 627
	// (get) Token: 0x060015B1 RID: 5553 RVA: 0x0002F184 File Offset: 0x0002D384
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestsAvailable;
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x00032163 File Offset: 0x00030363
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x000815EC File Offset: 0x0007F7EC
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(GameManager.Instance.World.GetPrimaryPlayer());
		EntityTrader entityTrader = uiforPlayer.xui.Dialog.Respondent as EntityTrader;
		if (!SandboxOptionManager.GetBool(SandboxOptions.QuestsEnabled))
		{
			return false;
		}
		bool result = false;
		if (entityTrader.activeQuests != null)
		{
			int currentFactionTier = uiforPlayer.entityPlayer.QuestJournal.GetCurrentFactionTier(entityTrader.NPCInfo.QuestFaction, 0, false);
			for (int i = 0; i < entityTrader.activeQuests.Count; i++)
			{
				if (entityTrader.activeQuests[i].QuestClass.QuestType == base.Value && (!entityTrader.activeQuests[i].QuestClass.ExtraTags.Test_AnySet(DialogRequirementQuestsAvailable.introtag) || QuestJournal.IntroQuestEnabled) && (int)entityTrader.activeQuests[i].QuestClass.DifficultyTier <= currentFactionTier && (entityTrader.activeQuests[i].QuestClass.Repeatable || uiforPlayer.entityPlayer.QuestJournal.FindActiveOrCompleteQuest(entityTrader.activeQuests[i].ID, (int)entityTrader.NPCInfo.QuestFaction) == null))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x04000E96 RID: 3734
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> introtag = FastTags<TagGroup.Global>.Parse("introquest");
}
