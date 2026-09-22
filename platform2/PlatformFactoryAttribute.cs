using System;

namespace Platform
{
	// Token: 0x02001BD4 RID: 7124
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PlatformFactoryAttribute : Attribute
	{
		// Token: 0x0600D3FF RID: 54271 RVA: 0x004CBF9C File Offset: 0x004CA19C
		public PlatformFactoryAttribute(EPlatformIdentifier _targetPlatform)
		{
			this.TargetPlatform = _targetPlatform;
		}

		// Token: 0x0400A1A4 RID: 41380
		public readonly EPlatformIdentifier TargetPlatform;
	}
}
