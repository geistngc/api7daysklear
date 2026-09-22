using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001940 RID: 6464
	[Preserve]
	public class RequirementIsTwitchActive : BaseRequirement
	{
		// Token: 0x0600C74C RID: 51020 RVA: 0x0049479C File Offset: 0x0049299C
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer == null)
			{
				return false;
			}
			if (!this.Invert)
			{
				return entityPlayer.TwitchEnabled;
			}
			return !entityPlayer.TwitchEnabled;
		}

		// Token: 0x0600C74D RID: 51021 RVA: 0x004947CD File Offset: 0x004929CD
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsTwitchActive
			{
				Invert = this.Invert
			};
		}
	}
}
