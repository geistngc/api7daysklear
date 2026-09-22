using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001968 RID: 6504
	[Preserve]
	public class ActionAddStartingItems : ActionBaseClientAction
	{
		// Token: 0x0600C812 RID: 51218 RVA: 0x00498520 File Offset: 0x00496720
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.SetupStartingItems();
			}
		}

		// Token: 0x0600C813 RID: 51219 RVA: 0x0049853D File Offset: 0x0049673D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddStartingItems
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
