using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020019B9 RID: 6585
	[Preserve]
	public class ActionSetEventFlag : ActionBaseTargetAction
	{
		// Token: 0x0600C97C RID: 51580 RVA: 0x004A1EA1 File Offset: 0x004A00A1
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			GameEventManager.Current.SetGameEventFlag(this.eventFlag, this.enable, GameEventManager.GetFloatValue(target as EntityAlive, this.durationText, 0f), false);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C97D RID: 51581 RVA: 0x004A1ED1 File Offset: 0x004A00D1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<GameEventManager.GameEventFlagTypes>(ActionSetEventFlag.PropEventFlag, ref this.eventFlag);
			properties.ParseBool(ActionSetEventFlag.PropEnable, ref this.enable);
			properties.ParseString(ActionSetEventFlag.PropDuration, ref this.durationText);
		}

		// Token: 0x0600C97E RID: 51582 RVA: 0x004A1F0D File Offset: 0x004A010D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetEventFlag
			{
				targetGroup = this.targetGroup,
				eventFlag = this.eventFlag,
				enable = this.enable,
				durationText = this.durationText
			};
		}

		// Token: 0x0400990E RID: 39182
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameEventManager.GameEventFlagTypes eventFlag = GameEventManager.GameEventFlagTypes.Invalid;

		// Token: 0x0400990F RID: 39183
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool enable;

		// Token: 0x04009910 RID: 39184
		[PublicizedFrom(EAccessModifier.Protected)]
		public string durationText;

		// Token: 0x04009911 RID: 39185
		public static string PropEventFlag = "event_flag";

		// Token: 0x04009912 RID: 39186
		public static string PropEnable = "enable";

		// Token: 0x04009913 RID: 39187
		public static string PropDuration = "duration";
	}
}
