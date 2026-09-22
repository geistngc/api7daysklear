using System;

namespace Twitch
{
	// Token: 0x0200187B RID: 6267
	public class TwitchVoteRequirementIsNight : BaseTwitchVoteRequirement
	{
		// Token: 0x0600C16E RID: 49518 RVA: 0x0047C207 File Offset: 0x0047A407
		public override bool CanPerform(EntityPlayer player)
		{
			if (!this.Invert)
			{
				return !GameManager.Instance.World.IsDaytime();
			}
			return GameManager.Instance.World.IsDaytime();
		}
	}
}
