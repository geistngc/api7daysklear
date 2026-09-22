using System;

namespace Platform.MultiPlatform
{
	// Token: 0x02001CC2 RID: 7362
	public class RichPresence : IRichPresence
	{
		// Token: 0x0600DA96 RID: 55958 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600DA97 RID: 55959 RVA: 0x004E67B6 File Offset: 0x004E49B6
		public void UpdateRichPresence(IRichPresence.PresenceStates _state)
		{
			IRichPresence richPresence = PlatformManager.NativePlatform.RichPresence;
			if (richPresence != null)
			{
				richPresence.UpdateRichPresence(_state);
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			IRichPresence richPresence2 = crossplatformPlatform.RichPresence;
			if (richPresence2 == null)
			{
				return;
			}
			richPresence2.UpdateRichPresence(_state);
		}
	}
}
