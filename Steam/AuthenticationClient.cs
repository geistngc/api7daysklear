using System;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C84 RID: 7300
	public class AuthenticationClient : IAuthenticationClient
	{
		// Token: 0x0600D86B RID: 55403 RVA: 0x004DFB2B File Offset: 0x004DDD2B
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600D86C RID: 55404 RVA: 0x004DFB34 File Offset: 0x004DDD34
		public string GetAuthTicket()
		{
			if (!this.registeredDisconnectEvent)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.OnDisconnectFromServer += this.OnDisconnectFromServer;
				this.registeredDisconnectEvent = true;
			}
			byte[] array = new byte[1024];
			Log.Out("[Steamworks.NET] Auth.GetAuthTicket()");
			if (this.ticketHandle != HAuthTicket.Invalid)
			{
				SteamUser.CancelAuthTicket(this.ticketHandle);
				this.ticketHandle = HAuthTicket.Invalid;
			}
			SteamNetworkingIdentity steamNetworkingIdentity = new SteamNetworkingIdentity
			{
				m_eType = ESteamNetworkingIdentityType.k_ESteamNetworkingIdentityType_Invalid
			};
			uint num;
			this.ticketHandle = SteamUser.GetAuthSessionTicket(array, array.Length, out num, ref steamNetworkingIdentity);
			return Convert.ToBase64String(array);
		}

		// Token: 0x0600D86D RID: 55405 RVA: 0x004DFBCE File Offset: 0x004DDDCE
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisconnectFromServer()
		{
			if (this.ticketHandle != HAuthTicket.Invalid)
			{
				SteamUser.CancelAuthTicket(this.ticketHandle);
				this.ticketHandle = HAuthTicket.Invalid;
			}
		}

		// Token: 0x0600D86E RID: 55406 RVA: 0x004DFBF8 File Offset: 0x004DDDF8
		public void AuthenticateServer(ClientAuthenticateServerContext _context)
		{
			_context.Success();
		}

		// Token: 0x0600D86F RID: 55407 RVA: 0x004DFC00 File Offset: 0x004DDE00
		public void Destroy()
		{
			this.OnDisconnectFromServer();
		}

		// Token: 0x0400A4B2 RID: 42162
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A4B3 RID: 42163
		[PublicizedFrom(EAccessModifier.Private)]
		public HAuthTicket ticketHandle = HAuthTicket.Invalid;

		// Token: 0x0400A4B4 RID: 42164
		[PublicizedFrom(EAccessModifier.Private)]
		public bool registeredDisconnectEvent;
	}
}
