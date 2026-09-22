using System;
using SandboxOptions;
using UnityEngine.Scripting;

// Token: 0x020002F5 RID: 757
[Preserve]
public class DialogRequirementCanTrade : BaseDialogRequirement
{
	// Token: 0x17000270 RID: 624
	// (get) Token: 0x060015A8 RID: 5544 RVA: 0x000814EA File Offset: 0x0007F6EA
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.CanTrade;
		}
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x000814EE File Offset: 0x0007F6EE
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return SandboxOptionManager.GetBool(SandboxOptions.TradersEnabled);
	}
}
