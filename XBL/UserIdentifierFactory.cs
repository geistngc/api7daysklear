using System;
using UnityEngine.Scripting;

namespace Platform.XBL
{
	// Token: 0x02001C15 RID: 7189
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.XBL)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600D5A3 RID: 54691 RVA: 0x004D2564 File Offset: 0x004D0764
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierXbl(_userId);
		}
	}
}
