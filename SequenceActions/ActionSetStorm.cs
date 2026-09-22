using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019C2 RID: 6594
	[Preserve]
	public class ActionSetStorm : BaseAction
	{
		// Token: 0x0600C99F RID: 51615 RVA: 0x004A2644 File Offset: 0x004A0844
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			float floatValue = GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.timeText, 1f);
			WeatherManager.Instance.SetStorm(null, (int)(floatValue * 1000f));
			WeatherManager.Instance.TriggerUpdate();
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C9A0 RID: 51616 RVA: 0x004A2690 File Offset: 0x004A0890
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSetStorm.PropTime, ref this.timeText);
		}

		// Token: 0x0600C9A1 RID: 51617 RVA: 0x004A26AA File Offset: 0x004A08AA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetStorm
			{
				timeText = this.timeText
			};
		}

		// Token: 0x04009933 RID: 39219
		[PublicizedFrom(EAccessModifier.Protected)]
		public string timeText;

		// Token: 0x04009934 RID: 39220
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTime = "hours";
	}
}
