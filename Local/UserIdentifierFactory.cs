using System;
using UnityEngine.Scripting;

namespace Platform.Local
{
	// Token: 0x02001CCF RID: 7375
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.Local)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600DAFD RID: 56061 RVA: 0x004E7672 File Offset: 0x004E5872
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierLocal(_userId);
		}
	}
}
