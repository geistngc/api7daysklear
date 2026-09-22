using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001933 RID: 6451
	[Preserve]
	public class RequirementHasHeld : BaseRequirement
	{
		// Token: 0x0600C716 RID: 50966 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C717 RID: 50967 RVA: 0x004940A4 File Offset: 0x004922A4
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return entityPlayer != null && !entityPlayer.inventory.holdingItemStack.IsEmpty();
		}

		// Token: 0x0600C718 RID: 50968 RVA: 0x004940D0 File Offset: 0x004922D0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
		}

		// Token: 0x0600C719 RID: 50969 RVA: 0x004940D9 File Offset: 0x004922D9
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasHeld();
		}
	}
}
