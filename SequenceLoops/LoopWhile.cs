using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceLoops
{
	// Token: 0x02001950 RID: 6480
	[Preserve]
	public class LoopWhile : BaseLoop
	{
		// Token: 0x1700189B RID: 6299
		// (get) Token: 0x0600C7A5 RID: 51109 RVA: 0x00010E62 File Offset: 0x0000F062
		public override bool UseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600C7A6 RID: 51110 RVA: 0x00495630 File Offset: 0x00493830
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (!this.runLoop)
			{
				this.runLoop = this.CheckCondition();
			}
			if (this.runLoop)
			{
				if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
				{
					this.CurrentPhase = 0;
					for (int i = 0; i < this.Actions.Count; i++)
					{
						this.Actions[i].Reset();
					}
					this.runLoop = false;
				}
				return BaseAction.ActionCompleteStates.InComplete;
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600C7A7 RID: 51111 RVA: 0x0049569C File Offset: 0x0049389C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckCondition()
		{
			if (this.Requirements != null)
			{
				LoopWhile.ConditionTypes conditionType = this.ConditionType;
				if (conditionType == LoopWhile.ConditionTypes.Any)
				{
					bool result = false;
					for (int i = 0; i < this.Requirements.Count; i++)
					{
						if (this.Requirements[i].CanPerform(base.Owner.Target))
						{
							result = true;
							break;
						}
					}
					return result;
				}
				if (conditionType == LoopWhile.ConditionTypes.All)
				{
					for (int j = 0; j < this.Requirements.Count; j++)
					{
						if (!this.Requirements[j].CanPerform(base.Owner.Target))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600C7A8 RID: 51112 RVA: 0x0049573C File Offset: 0x0049393C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<LoopWhile.ConditionTypes>(LoopWhile.PropConditionType, ref this.ConditionType);
		}

		// Token: 0x0600C7A9 RID: 51113 RVA: 0x00495756 File Offset: 0x00493956
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new LoopWhile
			{
				ConditionType = this.ConditionType
			};
		}

		// Token: 0x0400968C RID: 38540
		public LoopWhile.ConditionTypes ConditionType = LoopWhile.ConditionTypes.All;

		// Token: 0x0400968D RID: 38541
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropConditionType = "condition_type";

		// Token: 0x0400968E RID: 38542
		[PublicizedFrom(EAccessModifier.Private)]
		public bool runLoop;

		// Token: 0x02001951 RID: 6481
		public enum ConditionTypes
		{
			// Token: 0x04009690 RID: 38544
			Any,
			// Token: 0x04009691 RID: 38545
			All
		}
	}
}
