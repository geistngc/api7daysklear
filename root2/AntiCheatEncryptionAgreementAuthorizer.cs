using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200078F RID: 1935
[Preserve]
public class AntiCheatEncryptionAgreementAuthorizer : AuthorizerAbs
{
	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x060039A4 RID: 14756 RVA: 0x00178F38 File Offset: 0x00177138
	public override int Order
	{
		get
		{
			return 601;
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x060039A5 RID: 14757 RVA: 0x00178F3F File Offset: 0x0017713F
	public override string AuthorizerName
	{
		get
		{
			return "Encryption";
		}
	}

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x060039A6 RID: 14758 RVA: 0x00178F46 File Offset: 0x00177146
	public override string StateLocalizationKey
	{
		get
		{
			return "authstate_encryption";
		}
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x00178F4D File Offset: 0x0017714D
	public override void ServerStart()
	{
		base.ServerStart();
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthServer.Start(new AntiCheatEncryptionAuthServer.KeyExchangeCompleteDelegate(this.KeyExchangeCompleted), new AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate(this.KeyExchangeFailed));
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x00178F7C File Offset: 0x0017717C
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer != null && antiCheatServer.EncryptionAvailable() && _clientInfo.requiresAntiCheat)
		{
			if (_clientInfo.acAuthDone)
			{
				_clientInfo.SetAntiCheatEncryption(PlatformManager.MultiPlatform.AntiCheatServer);
			}
			else
			{
				Log.Warning("Server EAC AntiCheat encryption is available but " + _clientInfo.playerName + " did not complete EAC auth, encryption is disabled for this client");
			}
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		if (!GameManager.IsDedicatedServer)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthServer.TryStartKeyExchange(_clientInfo))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x00179035 File Offset: 0x00177235
	[PublicizedFrom(EAccessModifier.Private)]
	public void KeyExchangeCompleted(ClientInfo _clientInfo, IEncryptionModule _encryptionModule)
	{
		_clientInfo.SetAntiCheatEncryption(_encryptionModule);
		this.authResponsesHandler.AuthorizationAccepted(this, _clientInfo);
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x0017895F File Offset: 0x00176B5F
	[PublicizedFrom(EAccessModifier.Private)]
	public void KeyExchangeFailed(ClientInfo _clientInfo, GameUtils.KickPlayerData _reason)
	{
		this.authResponsesHandler.AuthorizationDenied(this, _clientInfo, _reason);
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x0017904B File Offset: 0x0017724B
	public override void ServerStop()
	{
		base.ServerStop();
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthServer.Stop();
	}
}
