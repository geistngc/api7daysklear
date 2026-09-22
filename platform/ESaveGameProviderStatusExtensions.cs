using System;

namespace Platform
{
	// Token: 0x02001B91 RID: 7057
	public static class ESaveGameProviderStatusExtensions
	{
		// Token: 0x0600D306 RID: 54022 RVA: 0x004CAB16 File Offset: 0x004C8D16
		public static bool IsTerminal(this ESaveGameProviderStatus status)
		{
			switch (status)
			{
			case ESaveGameProviderStatus.Uninitialized:
			case ESaveGameProviderStatus.TemporaryError:
				return false;
			case ESaveGameProviderStatus.Ok:
			case ESaveGameProviderStatus.PermanentError:
				return true;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
		}
	}
}
