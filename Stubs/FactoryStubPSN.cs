using System;
using UnityEngine.Scripting;

namespace Platform.Stubs
{
	// Token: 0x02001C7E RID: 7294
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.PSN)]
	public class FactoryStubPSN : AbsPlatform
	{
		// Token: 0x0600D843 RID: 55363 RVA: 0x004DF1D0 File Offset: 0x004DD3D0
		public override void CreateInstances()
		{
			if (!base.AsServerOnly)
			{
				throw new NotSupportedException("This platform can only be used as a server platform.");
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null || crossplatformPlatform.PlatformIdentifier != EPlatformIdentifier.EOS)
			{
				throw new NotSupportedException("This server platform requires EOS as the cross-platform.");
			}
		}
	}
}
