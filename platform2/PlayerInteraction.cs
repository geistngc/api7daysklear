using System;

namespace Platform
{
	// Token: 0x02001B93 RID: 7059
	public struct PlayerInteraction
	{
		// Token: 0x0600D307 RID: 54023 RVA: 0x004CAB45 File Offset: 0x004C8D45
		public PlayerInteraction(PlayerData _playerData, PlayerInteractionType _type)
		{
			this.PlayerData = _playerData;
			this.Type = _type;
		}

		// Token: 0x0400A129 RID: 41257
		public PlayerData PlayerData;

		// Token: 0x0400A12A RID: 41258
		public PlayerInteractionType Type;
	}
}
