using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019B5 RID: 6581
	[Preserve]
	public class ActionResetSleepers : BaseAction
	{
		// Token: 0x0600C967 RID: 51559 RVA: 0x004A15B2 File Offset: 0x0049F7B2
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			GameManager.Instance.World.ResetSleeperVolumes();
			Log.Out("Reset all sleeper volumes");
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C968 RID: 51560 RVA: 0x004A15CE File Offset: 0x0049F7CE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionResetSleepers();
		}
	}
}
