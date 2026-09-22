using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001B5A RID: 7002
	public interface IAntiCheatServer : IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600D1CA RID: 53706
		void Init(IPlatform _owner);

		// Token: 0x0600D1CB RID: 53707
		void Update();

		// Token: 0x0600D1CC RID: 53708
		bool StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate);

		// Token: 0x0600D1CD RID: 53709
		bool RegisterUser(ClientInfo _client);

		// Token: 0x0600D1CE RID: 53710
		void FreeUser(ClientInfo _client);

		// Token: 0x0600D1CF RID: 53711
		void HandleMessageFromClient(ClientInfo _cInfo, byte[] _data);

		// Token: 0x0600D1D0 RID: 53712
		void StopServer();

		// Token: 0x0600D1D1 RID: 53713
		void Destroy();

		// Token: 0x0600D1D2 RID: 53714
		bool ServerEacEnabled();

		// Token: 0x0600D1D3 RID: 53715
		bool ServerEacAvailable();

		// Token: 0x0600D1D4 RID: 53716
		bool GetHostUserIdAndToken([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] out ValueTuple<PlatformUserIdentifierAbs, string> _hostUserIdAndToken);
	}
}
