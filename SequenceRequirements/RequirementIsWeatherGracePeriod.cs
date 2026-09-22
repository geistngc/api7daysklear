using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001941 RID: 6465
	[Preserve]
	public class RequirementIsWeatherGracePeriod : BaseRequirement
	{
		// Token: 0x0600C74F RID: 51023 RVA: 0x004947E0 File Offset: 0x004929E0
		public override bool CanPerform(Entity target)
		{
			bool flag = GameManager.Instance.World.GetWorldTime() <= 30000UL;
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600C750 RID: 51024 RVA: 0x00494816 File Offset: 0x00492A16
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsWeatherGracePeriod
			{
				Invert = this.Invert
			};
		}
	}
}
