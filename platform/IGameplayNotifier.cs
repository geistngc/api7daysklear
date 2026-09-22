using System;

namespace Platform
{
	// Token: 0x02001B6A RID: 7018
	public interface IGameplayNotifier
	{
		// Token: 0x0600D213 RID: 53779
		void Init(IPlatform platform);

		// Token: 0x0600D214 RID: 53780
		void GameplayStart(bool isOnlineMultiplayer, bool isCrossplayEnabled);

		// Token: 0x0600D215 RID: 53781
		void EndOnlineMultiplayer();

		// Token: 0x0600D216 RID: 53782
		void GameplayEnd();
	}
}
