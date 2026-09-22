using System;
using System.Collections.Generic;

namespace Platform.MultiPlatform
{
	// Token: 0x02001CC1 RID: 7361
	public class PlayerInteractionsRecorderMulti : IPlayerInteractionsRecorder
	{
		// Token: 0x0600DA91 RID: 55953 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform owner)
		{
		}

		// Token: 0x0600DA92 RID: 55954 RVA: 0x004E6746 File Offset: 0x004E4946
		public void RecordPlayerInteraction(PlayerInteraction interaction)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform != null)
			{
				IPlayerInteractionsRecorder playerInteractionsRecorder = nativePlatform.PlayerInteractionsRecorder;
				if (playerInteractionsRecorder != null)
				{
					playerInteractionsRecorder.RecordPlayerInteraction(interaction);
				}
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			IPlayerInteractionsRecorder playerInteractionsRecorder2 = crossplatformPlatform.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder2 == null)
			{
				return;
			}
			playerInteractionsRecorder2.RecordPlayerInteraction(interaction);
		}

		// Token: 0x0600DA93 RID: 55955 RVA: 0x004E677E File Offset: 0x004E497E
		public void RecordPlayerInteractions(IEnumerable<PlayerInteraction> interactions)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform != null)
			{
				IPlayerInteractionsRecorder playerInteractionsRecorder = nativePlatform.PlayerInteractionsRecorder;
				if (playerInteractionsRecorder != null)
				{
					playerInteractionsRecorder.RecordPlayerInteractions(interactions);
				}
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			IPlayerInteractionsRecorder playerInteractionsRecorder2 = crossplatformPlatform.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder2 == null)
			{
				return;
			}
			playerInteractionsRecorder2.RecordPlayerInteractions(interactions);
		}

		// Token: 0x0600DA94 RID: 55956 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}
	}
}
