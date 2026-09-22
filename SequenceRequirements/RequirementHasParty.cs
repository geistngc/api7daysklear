using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001934 RID: 6452
	[Preserve]
	public class RequirementHasParty : BaseRequirement
	{
		// Token: 0x0600C71B RID: 50971 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C71C RID: 50972 RVA: 0x004940E0 File Offset: 0x004922E0
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null && entityPlayer.Party != null)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C71D RID: 50973 RVA: 0x0049410F File Offset: 0x0049230F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasParty();
		}
	}
}
