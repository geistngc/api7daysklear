using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192F RID: 6447
	[Preserve]
	public class RequirementGroupLiveCount : BaseOperationRequirement
	{
		// Token: 0x0600C6FE RID: 50942 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6FF RID: 50943 RVA: 0x00493D95 File Offset: 0x00491F95
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return this.Owner.GetEntityGroupLiveCount(this.targetGroup);
		}

		// Token: 0x0600C700 RID: 50944 RVA: 0x00493DAD File Offset: 0x00491FAD
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600C701 RID: 50945 RVA: 0x00493DC6 File Offset: 0x00491FC6
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementGroupLiveCount.PropTargetGroup, ref this.targetGroup);
			properties.ParseString(RequirementGroupLiveCount.PropCount, ref this.valueText);
		}

		// Token: 0x0600C702 RID: 50946 RVA: 0x00493DF1 File Offset: 0x00491FF1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGroupLiveCount
			{
				Invert = this.Invert,
				operation = this.operation,
				targetGroup = this.targetGroup,
				valueText = this.valueText
			};
		}

		// Token: 0x0400963A RID: 38458
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x0400963B RID: 38459
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400963C RID: 38460
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";

		// Token: 0x0400963D RID: 38461
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropCount = "count";
	}
}
