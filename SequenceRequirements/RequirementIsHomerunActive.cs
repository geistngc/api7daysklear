using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200193E RID: 6462
	[Preserve]
	public class RequirementIsHomerunActive : BaseRequirement
	{
		// Token: 0x0600C746 RID: 51014 RVA: 0x004946F4 File Offset: 0x004928F4
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer == null)
			{
				return false;
			}
			bool flag = GameEventManager.Current.HomerunManager.HasHomerunActive(entityPlayer);
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600C747 RID: 51015 RVA: 0x0049472C File Offset: 0x0049292C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsHomerunActive
			{
				Invert = this.Invert
			};
		}
	}
}
