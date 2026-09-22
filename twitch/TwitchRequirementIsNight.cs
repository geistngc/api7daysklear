using System;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x02001860 RID: 6240
	[Preserve]
	public class TwitchRequirementIsNight : BaseTwitchRequirement
	{
		// Token: 0x0600C0B8 RID: 49336 RVA: 0x004778CE File Offset: 0x00475ACE
		public override bool CanPerform(Entity entity)
		{
			if (!this.Invert)
			{
				return !GameManager.Instance.World.IsDaytime();
			}
			return GameManager.Instance.World.IsDaytime();
		}
	}
}
