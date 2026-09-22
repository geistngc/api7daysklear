using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020002F6 RID: 758
[Preserve]
public class DialogRequirementCheckCVar : BaseDialogRequirement
{
	// Token: 0x17000271 RID: 625
	// (get) Token: 0x060015AB RID: 5547 RVA: 0x00081502 File Offset: 0x0007F702
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.CVar;
		}
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00081506 File Offset: 0x0007F706
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		int num = (int)player.GetCVar(base.ID);
		LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
		return num == StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
	}
}
