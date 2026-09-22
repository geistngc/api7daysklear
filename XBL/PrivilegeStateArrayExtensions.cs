using System;
using System.Collections;

namespace Platform.XBL
{
	// Token: 0x02001C19 RID: 7193
	public static class PrivilegeStateArrayExtensions
	{
		// Token: 0x0600D5B7 RID: 54711 RVA: 0x004D2888 File Offset: 0x004D0A88
		public static bool Has(this PrivilegeState[] privilegeStates)
		{
			for (int i = 0; i < privilegeStates.Length; i++)
			{
				if (!privilegeStates[i].Has)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600D5B8 RID: 54712 RVA: 0x004D28B4 File Offset: 0x004D0AB4
		public static void ResolveSilent(this PrivilegeState[] privilegeStates)
		{
			for (int i = 0; i < privilegeStates.Length; i++)
			{
				privilegeStates[i].ResolveSilent();
			}
		}

		// Token: 0x0600D5B9 RID: 54713 RVA: 0x004D28D9 File Offset: 0x004D0AD9
		public static IEnumerator ResolveWithPrompt(this PrivilegeState[] privilegeStates, CoroutineCancellationToken _cancellationToken)
		{
			foreach (PrivilegeState privilegeState in privilegeStates)
			{
				yield return privilegeState.ResolveWithPrompt(_cancellationToken);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			PrivilegeState[] array = null;
			yield break;
		}
	}
}
