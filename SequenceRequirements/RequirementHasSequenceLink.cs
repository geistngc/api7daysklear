using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001935 RID: 6453
	[Preserve]
	public class RequirementHasSequenceLink : BaseRequirement
	{
		// Token: 0x0600C71F RID: 50975 RVA: 0x00494116 File Offset: 0x00492316
		public override bool CanPerform(Entity target)
		{
			if (GameEventManager.Current.HasSequenceLink(this.Owner))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C720 RID: 50976 RVA: 0x0049413A File Offset: 0x0049233A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasSequenceLink
			{
				Invert = this.Invert
			};
		}
	}
}
