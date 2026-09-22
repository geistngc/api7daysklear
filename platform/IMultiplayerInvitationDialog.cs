using System;

namespace Platform
{
	// Token: 0x02001B6F RID: 7023
	public interface IMultiplayerInvitationDialog
	{
		// Token: 0x170019DE RID: 6622
		// (get) Token: 0x0600D22A RID: 53802
		bool CanShow { get; }

		// Token: 0x0600D22B RID: 53803
		void Init(IPlatform owner);

		// Token: 0x0600D22C RID: 53804
		void ShowInviteDialog();
	}
}
