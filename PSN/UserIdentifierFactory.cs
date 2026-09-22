using System;
using UnityEngine.Scripting;

namespace Platform.PSN
{
	// Token: 0x02001CBD RID: 7357
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.PSN)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600DA49 RID: 55881 RVA: 0x004E635C File Offset: 0x004E455C
		public override PlatformUserIdentifierAbs FromId(string _idString)
		{
			Log.Out("[PSN] Creating PSN user identifier from: {0}", new object[]
			{
				_idString
			});
			ulong accountId;
			if (StringParsers.TryParseUInt64(_idString, out accountId))
			{
				return new UserIdentifierPSN(accountId);
			}
			Log.Warning("[PSN] Could not parse PSN user from " + _idString);
			return null;
		}
	}
}
