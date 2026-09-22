using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192A RID: 6442
	[Preserve]
	public class RequirementFullHealth : BaseRequirement
	{
		// Token: 0x0600C6E0 RID: 50912 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6E1 RID: 50913 RVA: 0x00493AB8 File Offset: 0x00491CB8
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			if (entityAlive.Stats.Health.Value == entityAlive.Stats.Health.Max)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C6E2 RID: 50914 RVA: 0x00493B05 File Offset: 0x00491D05
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementFullHealth
			{
				Invert = this.Invert
			};
		}
	}
}
