using System;

namespace Platform
{
	// Token: 0x02001BF8 RID: 7160
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class UserIdentifierFactoryAttribute : Attribute
	{
		// Token: 0x0600D4CC RID: 54476 RVA: 0x004CF1D4 File Offset: 0x004CD3D4
		public UserIdentifierFactoryAttribute(EPlatformIdentifier _targetPlatform)
		{
			this.TargetPlatform = _targetPlatform;
		}

		// Token: 0x0400A22B RID: 41515
		public readonly EPlatformIdentifier TargetPlatform;
	}
}
