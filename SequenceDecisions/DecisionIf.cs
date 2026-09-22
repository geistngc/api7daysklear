using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceDecisions
{
	// Token: 0x02001958 RID: 6488
	[Preserve]
	public class DecisionIf : BaseDecision
	{
		// Token: 0x0600C7C8 RID: 51144 RVA: 0x00496A3D File Offset: 0x00494C3D
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (!this.runActions)
			{
				this.runActions = this.CheckCondition();
			}
			if (!this.runActions)
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
			{
				this.runActions = false;
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600C7C9 RID: 51145 RVA: 0x00496A70 File Offset: 0x00494C70
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckCondition()
		{
			if (this.Requirements != null)
			{
				DecisionIf.ConditionTypes conditionType = this.ConditionType;
				if (conditionType == DecisionIf.ConditionTypes.Any)
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
				if (conditionType == DecisionIf.ConditionTypes.All)
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

		// Token: 0x0600C7CA RID: 51146 RVA: 0x00496B10 File Offset: 0x00494D10
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<DecisionIf.ConditionTypes>(DecisionIf.PropConditionType, ref this.ConditionType);
		}

		// Token: 0x0600C7CB RID: 51147 RVA: 0x00496B2A File Offset: 0x00494D2A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new DecisionIf
			{
				ConditionType = this.ConditionType
			};
		}

		// Token: 0x040096B8 RID: 38584
		public DecisionIf.ConditionTypes ConditionType = DecisionIf.ConditionTypes.All;

		// Token: 0x040096B9 RID: 38585
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropConditionType = "condition_type";

		// Token: 0x040096BA RID: 38586
		[PublicizedFrom(EAccessModifier.Private)]
		public bool runActions;

		// Token: 0x02001959 RID: 6489
		public enum ConditionTypes
		{
			// Token: 0x040096BC RID: 38588
			Any,
			// Token: 0x040096BD RID: 38589
			All
		}
	}
}
