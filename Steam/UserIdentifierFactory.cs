using System;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x02001CA6 RID: 7334
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.Steam)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600D988 RID: 55688 RVA: 0x004E466C File Offset: 0x004E286C
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierSteam(_userId);
		}
	}
}
