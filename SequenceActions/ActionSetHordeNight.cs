using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019BC RID: 6588
	[Preserve]
	public class ActionSetHordeNight : BaseAction
	{
		// Token: 0x0600C986 RID: 51590 RVA: 0x004A2041 File Offset: 0x004A0241
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (GameManager.Instance != null && GameManager.Instance.World != null)
			{
				GameManager.Instance.World.aiDirector.BloodMoonComponent.SetForToday(this.keepBMDay);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C987 RID: 51591 RVA: 0x004A207D File Offset: 0x004A027D
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionSetHordeNight.PropKeepBMDay, ref this.keepBMDay);
		}

		// Token: 0x0600C988 RID: 51592 RVA: 0x004A2097 File Offset: 0x004A0297
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetHordeNight
			{
				keepBMDay = this.keepBMDay
			};
		}

		// Token: 0x04009919 RID: 39193
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool keepBMDay = true;

		// Token: 0x0400991A RID: 39194
		public static string PropKeepBMDay = "keep_bm_day";
	}
}
