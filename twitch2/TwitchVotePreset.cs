using System;

namespace Twitch
{
	// Token: 0x0200186C RID: 6252
	public class TwitchVotePreset
	{
		// Token: 0x04009223 RID: 37411
		public string Name;

		// Token: 0x04009224 RID: 37412
		public bool IsDefault;

		// Token: 0x04009225 RID: 37413
		public bool IsEmpty;

		// Token: 0x04009226 RID: 37414
		public string Title;

		// Token: 0x04009227 RID: 37415
		public string Description;

		// Token: 0x04009228 RID: 37416
		public TwitchVotingManager.BossVoteSettings BossVoteSetting = TwitchVotingManager.BossVoteSettings.Standard;
	}
}
