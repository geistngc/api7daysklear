using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C8C RID: 7308
	[PublicizedFrom(EAccessModifier.Internal)]
	public class JoinSessionGameInviteListener : IJoinSessionGameInviteListener
	{
		// Token: 0x0600D8A1 RID: 55457 RVA: 0x004E07C5 File Offset: 0x004DE9C5
		public void Init(IPlatform _owner)
		{
			if (this.m_friends_serverchange == null)
			{
				this.m_friends_serverchange = Callback<GameServerChangeRequested_t>.Create(new Callback<GameServerChangeRequested_t>.DispatchDelegate(this.Friends_GameServerChangeRequested));
			}
			_owner.Api.ClientApiInitialized += delegate()
			{
				string[] commandLineArgs = Environment.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length - 1; i++)
				{
					ulong ulSteamID;
					if (commandLineArgs[i] == "+connect_lobby" && ulong.TryParse(commandLineArgs[i + 1], out ulSteamID))
					{
						this.SetLobby(new CSteamID(ulSteamID));
					}
				}
			};
		}

		// Token: 0x0600D8A2 RID: 55458 RVA: 0x004E0800 File Offset: 0x004DEA00
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		public ValueTuple<string, string> TakePendingInvite()
		{
			string item = this.pendingInvite;
			this.pendingInvite = null;
			string item2 = this.pendingPassword;
			this.pendingPassword = null;
			return new ValueTuple<string, string>(item, item2);
		}

		// Token: 0x0600D8A3 RID: 55459 RVA: 0x004E082E File Offset: 0x004DEA2E
		public IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null)
		{
			if (string.IsNullOrEmpty(_invite))
			{
				if (_onFinished != null)
				{
					_onFinished(false);
				}
				yield break;
			}
			if (_invite.StartsWith("Lobby:"))
			{
				ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
				if (lobbyHost != null)
				{
					lobbyHost.JoinLobby(_invite.Substring("Lobby:".Length), null);
				}
				if (_onFinished != null)
				{
					_onFinished(true);
				}
				yield break;
			}
			string[] array = _invite.Split(':', StringSplitOptions.None);
			string ip = "";
			int port = 0;
			if (array.Length == 2)
			{
				ip = array[0];
				port = Convert.ToInt32(array[1]);
			}
			yield return InviteManager.HandleIpPortInvite(ip, port, _password, _onFinished);
			yield break;
		}

		// Token: 0x0600D8A4 RID: 55460 RVA: 0x004E084B File Offset: 0x004DEA4B
		public string GetListenerIdentifier()
		{
			return "STM";
		}

		// Token: 0x0600D8A5 RID: 55461 RVA: 0x004E0852 File Offset: 0x004DEA52
		public void SetLobby(CSteamID _lobbyId)
		{
			this.pendingInvite = "Lobby:" + _lobbyId.m_SteamID.ToString();
		}

		// Token: 0x0600D8A6 RID: 55462 RVA: 0x004E0870 File Offset: 0x004DEA70
		[PublicizedFrom(EAccessModifier.Private)]
		public void Friends_GameServerChangeRequested(GameServerChangeRequested_t _value)
		{
			Log.Out("[Steamworks.NET] Friends_GameServerChangeRequested");
			this.pendingInvite = _value.m_rgchServer;
			this.pendingPassword = _value.m_rgchPassword;
		}

		// Token: 0x0400A4C4 RID: 42180
		[PublicizedFrom(EAccessModifier.Private)]
		public const string LobbyMarker = "Lobby:";

		// Token: 0x0400A4C5 RID: 42181
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingInvite;

		// Token: 0x0400A4C6 RID: 42182
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingPassword;

		// Token: 0x0400A4C7 RID: 42183
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<GameServerChangeRequested_t> m_friends_serverchange;
	}
}
