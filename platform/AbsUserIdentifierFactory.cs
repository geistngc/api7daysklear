using System;

namespace Platform
{
	// Token: 0x02001B55 RID: 6997
	public abstract class AbsUserIdentifierFactory
	{
		// Token: 0x0600D1B6 RID: 53686
		public abstract PlatformUserIdentifierAbs FromId(string _userId);

		// Token: 0x0600D1B7 RID: 53687 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsUserIdentifierFactory()
		{
		}
	}
}
