using System;
using UnityEngine.Scripting;

// Token: 0x020002FD RID: 765
[Preserve]
public class DialogRequirementSkill : BaseDialogRequirement
{
	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060015C2 RID: 5570 RVA: 0x00081B10 File Offset: 0x0007FD10
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Skill;
		}
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x00081B14 File Offset: 0x0007FD14
	public override string GetRequiredDescription(EntityPlayer player)
	{
		ProgressionValue progressionValue = player.Progression.GetProgressionValue(base.ID);
		return string.Format("({0} {1})", Localization.Get(progressionValue.ProgressionClass.NameKey, false, null), Convert.ToInt32(base.Value));
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x00081B5F File Offset: 0x0007FD5F
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return player.Progression.GetProgressionValue(base.ID).Level > Convert.ToInt32(base.Value);
	}
}
