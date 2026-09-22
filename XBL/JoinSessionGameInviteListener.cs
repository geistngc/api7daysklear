using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001C3A RID: 7226
	[PublicizedFrom(EAccessModifier.Internal)]
	public class JoinSessionGameInviteListener : IJoinSessionGameInviteListener
	{
		// Token: 0x0600D651 RID: 54865 RVA: 0x004D4FC2 File Offset: 0x004D31C2
		public void Init(IPlatform _owner)
		{
			PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _platform)
			{
				XGameInviteRegistrationToken xgameInviteRegistrationToken;
				XblHelpers.Succeeded(SDK.XGameInviteRegisterForEvent(new XGameInviteEventCallback(this.InviteReceivedCallback), out xgameInviteRegistrationToken), "Register for invite event", true, true);
			};
		}

		// Token: 0x0600D652 RID: 54866 RVA: 0x004D4FE0 File Offset: 0x004D31E0
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		public ValueTuple<string, string> TakePendingInvite()
		{
			object obj = this.inviteLock;
			ValueTuple<string, string> result;
			lock (obj)
			{
				string item = this.pendingInvite;
				this.pendingInvite = null;
				string item2 = this.pendingPassword;
				this.pendingPassword = null;
				result = new ValueTuple<string, string>(item, item2);
			}
			return result;
		}

		// Token: 0x0600D653 RID: 54867 RVA: 0x004D5040 File Offset: 0x004D3240
		public IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null)
		{
			yield return InviteManager.HandleSessionIdInvite(_invite, _password, _onFinished);
			yield break;
		}

		// Token: 0x0600D654 RID: 54868 RVA: 0x004D505D File Offset: 0x004D325D
		public string GetListenerIdentifier()
		{
			return "XBL";
		}

		// Token: 0x0600D655 RID: 54869 RVA: 0x004D5064 File Offset: 0x004D3264
		[PublicizedFrom(EAccessModifier.Private)]
		public void InviteReceivedCallback(IntPtr _, string _inviteuri)
		{
			Log.Out("[XBL] Invite received: '" + _inviteuri + "'");
			string text = this.ParseInviteUri(_inviteuri);
			if (text == null)
			{
				Log.Error("[XBL] Received invite but could not extract connect information");
				return;
			}
			object obj = this.inviteLock;
			lock (obj)
			{
				this.pendingInvite = text;
			}
		}

		// Token: 0x0600D656 RID: 54870 RVA: 0x004D50D0 File Offset: 0x004D32D0
		[PublicizedFrom(EAccessModifier.Private)]
		public string ParseInviteUri(string _inviteUri)
		{
			Match match = JoinSessionGameInviteListener.msInviteUriMatcher.Match(_inviteUri);
			if (!match.Success)
			{
				return null;
			}
			string[] array = match.Groups[3].Value.Split('&', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('=', StringSplitOptions.None);
				if (array2[0].EqualsCaseInsensitive("connectionString"))
				{
					return array2[1];
				}
			}
			return null;
		}

		// Token: 0x0400A387 RID: 41863
		[PublicizedFrom(EAccessModifier.Private)]
		public object inviteLock = new object();

		// Token: 0x0400A388 RID: 41864
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingInvite;

		// Token: 0x0400A389 RID: 41865
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingPassword;

		// Token: 0x0400A38A RID: 41866
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex msInviteUriMatcher = new Regex("^ms-xbl-(\\w+):\\/\\/(\\w+)\\/?\\?(.*)$", RegexOptions.Compiled);
	}
}
