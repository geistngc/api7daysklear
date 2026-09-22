using System;
using System.Collections;
using System.Linq;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001C17 RID: 7191
	public class UserPrivilegeHelper
	{
		// Token: 0x0600D5AE RID: 54702 RVA: 0x004D2634 File Offset: 0x004D0834
		public UserPrivilegeHelper(XUserHandle userHandle)
		{
			this.AllAllowed = new PrivilegeState[]
			{
				this.Multiplayer = new PrivilegeState(userHandle, XUserPrivilege.Multiplayer),
				this.Communications = new PrivilegeState(userHandle, XUserPrivilege.Communications),
				this.CrossPlay = new PrivilegeState(userHandle, XUserPrivilege.CrossPlay),
				this.UserGeneratedContent = new PrivilegeState(userHandle, XUserPrivilege.UserGeneratedContent)
			};
			this.MultiplayerAllowed = new PrivilegeState[]
			{
				this.Multiplayer,
				this.UserGeneratedContent
			};
			this.CommunicationAllowed = new PrivilegeState[]
			{
				this.Communications
			};
			this.CrossPlayAllowed = new PrivilegeState[]
			{
				this.CrossPlay
			};
		}

		// Token: 0x0600D5AF RID: 54703 RVA: 0x004D26F7 File Offset: 0x004D08F7
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator ResolveAllowed(bool canPrompt, CoroutineCancellationToken _cancellationToken = null, params PrivilegeState[] privilegeStates)
		{
			if (!canPrompt)
			{
				privilegeStates.ResolveSilent();
				return Enumerable.Empty<object>().GetEnumerator();
			}
			return privilegeStates.ResolveWithPrompt(_cancellationToken);
		}

		// Token: 0x0600D5B0 RID: 54704 RVA: 0x004D2714 File Offset: 0x004D0914
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			if (_perms.HasMultiplayer() || _perms.HasHostMultiplayer())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.MultiplayerAllowed);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			if (_perms.HasCommunication())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.CommunicationAllowed);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			if (_perms.HasCrossplay())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.CrossPlayAllowed);
				if (_cancellationToken != null)
				{
					_cancellationToken.IsCancelled();
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x0400A2BE RID: 41662
		public readonly PrivilegeState Multiplayer;

		// Token: 0x0400A2BF RID: 41663
		public readonly PrivilegeState Communications;

		// Token: 0x0400A2C0 RID: 41664
		public readonly PrivilegeState CrossPlay;

		// Token: 0x0400A2C1 RID: 41665
		public readonly PrivilegeState UserGeneratedContent;

		// Token: 0x0400A2C2 RID: 41666
		public readonly PrivilegeState[] AllAllowed;

		// Token: 0x0400A2C3 RID: 41667
		public readonly PrivilegeState[] MultiplayerAllowed;

		// Token: 0x0400A2C4 RID: 41668
		public readonly PrivilegeState[] CommunicationAllowed;

		// Token: 0x0400A2C5 RID: 41669
		public readonly PrivilegeState[] CrossPlayAllowed;
	}
}
