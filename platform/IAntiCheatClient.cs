using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001B58 RID: 7000
	public interface IAntiCheatClient : IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600D1C1 RID: 53697
		void Init(IPlatform _owner);

		// Token: 0x0600D1C2 RID: 53698
		bool ClientAntiCheatEnabled();

		// Token: 0x0600D1C3 RID: 53699
		bool GetUnhandledViolationMessage(out string _message);

		// Token: 0x0600D1C4 RID: 53700
		void WaitForRemoteAuth(Action onRemoteAuthSkippedOrComplete);

		// Token: 0x0600D1C5 RID: 53701
		void ConnectToServer([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] ValueTuple<PlatformUserIdentifierAbs, string> hostUserAndToken, Action onNoAntiCheatOrConnectionComplete, Action<string> onConnectionFailed);

		// Token: 0x0600D1C6 RID: 53702
		void HandleMessageFromServer(byte[] _data);

		// Token: 0x0600D1C7 RID: 53703
		void DisconnectFromServer();

		// Token: 0x0600D1C8 RID: 53704
		void Destroy();
	}
}
