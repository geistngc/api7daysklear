using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019B8 RID: 6584
	[Preserve]
	public class ActionSetDayTime : BaseAction
	{
		// Token: 0x0600C977 RID: 51575 RVA: 0x004A1D88 File Offset: 0x0049FF88
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			ulong worldTime = world.worldTime;
			int num = GameUtils.WorldTimeToDays(worldTime);
			int num2 = GameUtils.WorldTimeToHours(worldTime);
			int num3 = GameUtils.WorldTimeToMinutes(worldTime);
			ulong time = GameUtils.DayTimeToWorldTime((this.day < 1) ? num : this.day, (this.hours < 0) ? num2 : this.hours, (this.minutes < 0) ? num3 : this.minutes);
			world.SetTimeJump(time, true);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C978 RID: 51576 RVA: 0x004A1DFD File Offset: 0x0049FFFD
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseInt(ActionSetDayTime.PropDay, ref this.day);
			properties.ParseInt(ActionSetDayTime.PropHours, ref this.hours);
			properties.ParseInt(ActionSetDayTime.PropMinutes, ref this.minutes);
		}

		// Token: 0x0600C979 RID: 51577 RVA: 0x004A1E39 File Offset: 0x004A0039
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetDayTime
			{
				day = this.day,
				hours = this.hours,
				minutes = this.minutes
			};
		}

		// Token: 0x04009908 RID: 39176
		[PublicizedFrom(EAccessModifier.Protected)]
		public int day = -1;

		// Token: 0x04009909 RID: 39177
		[PublicizedFrom(EAccessModifier.Protected)]
		public int hours = -1;

		// Token: 0x0400990A RID: 39178
		[PublicizedFrom(EAccessModifier.Protected)]
		public int minutes = -1;

		// Token: 0x0400990B RID: 39179
		public static string PropDay = "day";

		// Token: 0x0400990C RID: 39180
		public static string PropHours = "hours";

		// Token: 0x0400990D RID: 39181
		public static string PropMinutes = "minutes";
	}
}
