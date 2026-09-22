using System;
using UnityEngine.Scripting;

// Token: 0x020002F3 RID: 755
[Preserve]
public class DialogRequirementAdmin : BaseDialogRequirement
{
	// Token: 0x1700026E RID: 622
	// (get) Token: 0x060015A0 RID: 5536 RVA: 0x0006037D File Offset: 0x0005E57D
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Admin;
		}
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0008145C File Offset: 0x0007F65C
	public override void SetupRequirement()
	{
		string description = Localization.Get("RequirementAdmin_keyword", false, null);
		base.Description = description;
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x0008147D File Offset: 0x0007F67D
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled);
	}

	// Token: 0x04000E94 RID: 3732
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
