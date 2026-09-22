using System;
using UnityEngine.Scripting;

namespace Platform.EOS
{
	// Token: 0x02001D2F RID: 7471
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.EOS)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600DD57 RID: 56663 RVA: 0x004F5A2C File Offset: 0x004F3C2C
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierEos(_userId);
		}
	}
}
