using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001BB2 RID: 7090
	public static class EUserPermsExtensions
	{
		// Token: 0x0600D37B RID: 54139 RVA: 0x004CB019 File Offset: 0x004C9219
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasMultiplayer(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Multiplayer);
		}

		// Token: 0x0600D37C RID: 54140 RVA: 0x004CB02C File Offset: 0x004C922C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasCommunication(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Communication);
		}

		// Token: 0x0600D37D RID: 54141 RVA: 0x004CB03F File Offset: 0x004C923F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasCrossplay(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Crossplay);
		}

		// Token: 0x0600D37E RID: 54142 RVA: 0x004CB052 File Offset: 0x004C9252
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasHostMultiplayer(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.HostMultiplayer);
		}
	}
}
