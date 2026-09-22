using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193C RID: 6460
	[Preserve]
	public class RequirementInVehicle : BaseRequirement
	{
		// Token: 0x0600C73B RID: 51003 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C73C RID: 51004 RVA: 0x00494592 File Offset: 0x00492792
		public override bool CanPerform(Entity target)
		{
			if (target == null)
			{
				return false;
			}
			if (target.AttachedToEntity)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C73D RID: 51005 RVA: 0x004945BC File Offset: 0x004927BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInVehicle
			{
				Invert = this.Invert
			};
		}
	}
}
