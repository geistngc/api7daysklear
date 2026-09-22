using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193B RID: 6459
	[Preserve]
	public class RequirementInTraderArea : BaseRequirement
	{
		// Token: 0x0600C738 RID: 51000 RVA: 0x0049453C File Offset: 0x0049273C
		public override bool CanPerform(Entity target)
		{
			if (!GameManager.Instance.World.IsWithinTraderArea(new Vector3i((target == null) ? this.Owner.TargetPosition : target.position)))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C739 RID: 51001 RVA: 0x0049458B File Offset: 0x0049278B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInTraderArea();
		}
	}
}
