using System;
using UnityEngine.Scripting;

// Token: 0x020002F4 RID: 756
[Preserve]
public class DialogRequirementBuff : BaseDialogRequirement
{
	// Token: 0x1700026F RID: 623
	// (get) Token: 0x060015A4 RID: 5540 RVA: 0x00010E62 File Offset: 0x0000F062
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Buff;
		}
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x0008149C File Offset: 0x0007F69C
	public override void SetupRequirement()
	{
		string arg = Localization.Get("RequirementBuff_keyword", false, null);
		base.Description = string.Format("{0} {1}", arg, BuffManager.GetBuff(base.ID).Name);
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return false;
	}

	// Token: 0x04000E95 RID: 3733
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
