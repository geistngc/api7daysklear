using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193F RID: 6463
	[Preserve]
	public class RequirementIsIndoors : BaseRequirement
	{
		// Token: 0x0600C749 RID: 51017 RVA: 0x00494740 File Offset: 0x00492940
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			if (!this.Invert)
			{
				return entityAlive.Stats.AmountEnclosed > 0f;
			}
			return entityAlive.Stats.AmountEnclosed <= 0f;
		}

		// Token: 0x0600C74A RID: 51018 RVA: 0x00494789 File Offset: 0x00492989
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsIndoors
			{
				Invert = this.Invert
			};
		}
	}
}
