using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001945 RID: 6469
	[Preserve]
	public class RequirementProgression : BaseOperationRequirement
	{
		// Token: 0x0600C75D RID: 51037 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C75E RID: 51038 RVA: 0x00494B10 File Offset: 0x00492D10
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null && entityAlive.Progression != null)
			{
				this.pv = entityAlive.Progression.GetProgressionValue(this.progressionName);
				if (this.pv != null)
				{
					return this.pv.GetCalculatedLevel(entityAlive);
				}
			}
			return 0;
		}

		// Token: 0x0600C75F RID: 51039 RVA: 0x00494B66 File Offset: 0x00492D66
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600C760 RID: 51040 RVA: 0x00494B7F File Offset: 0x00492D7F
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementProgression.PropProgressionName, ref this.progressionName);
			properties.ParseString(RequirementProgression.PropValue, ref this.valueText);
		}

		// Token: 0x0600C761 RID: 51041 RVA: 0x00494BAA File Offset: 0x00492DAA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementProgression
			{
				Invert = this.Invert,
				operation = this.operation,
				progressionName = this.progressionName,
				valueText = this.valueText
			};
		}

		// Token: 0x04009661 RID: 38497
		[PublicizedFrom(EAccessModifier.Protected)]
		public string progressionName = "";

		// Token: 0x04009662 RID: 38498
		[PublicizedFrom(EAccessModifier.Protected)]
		public ProgressionValue pv;

		// Token: 0x04009663 RID: 38499
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009664 RID: 38500
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropProgressionName = "name";

		// Token: 0x04009665 RID: 38501
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
