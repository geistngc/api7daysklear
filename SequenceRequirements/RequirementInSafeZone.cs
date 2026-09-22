using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193A RID: 6458
	[Preserve]
	public class RequirementInSafeZone : BaseRequirement
	{
		// Token: 0x0600C735 RID: 50997 RVA: 0x004944E4 File Offset: 0x004926E4
		public override bool CanPerform(Entity target)
		{
			if (!GameManager.Instance.World.CanPlaceBlockAt(new Vector3i((target == null) ? this.Owner.TargetPosition : target.position), null, false))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C736 RID: 50998 RVA: 0x00494535 File Offset: 0x00492735
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInSafeZone();
		}
	}
}
