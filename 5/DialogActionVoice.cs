using System;
using UnityEngine.Scripting;

// Token: 0x020002E8 RID: 744
[Preserve]
public class DialogActionVoice : BaseDialogAction
{
	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06001563 RID: 5475 RVA: 0x00080A2D File Offset: 0x0007EC2D
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.Voice;
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00080A30 File Offset: 0x0007EC30
	public override void PerformAction(EntityPlayer player)
	{
		LocalPlayerUI.primaryUI.xui.Dialog.Respondent.PlayVoiceSetEntry(base.ID, player, true, true);
	}

	// Token: 0x04000E5D RID: 3677
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
