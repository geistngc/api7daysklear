using System;
using System.Collections.Generic;
using System.Security.Cryptography;

// Token: 0x02000763 RID: 1891
public class AntiCheatEncryptionAuthServer
{
	// Token: 0x06003823 RID: 14371 RVA: 0x0017150F File Offset: 0x0016F70F
	public void Start(AntiCheatEncryptionAuthServer.KeyExchangeCompleteDelegate completeDelegate, AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate failedDelegate)
	{
		this.OnExchangeCompleted = completeDelegate;
		this.OnExchangeFailed = failedDelegate;
		this.pendingEncryptionModules = new Dictionary<ClientInfo, IEncryptionModule>();
		ConnectionManager.OnClientDisconnected += this.OnClientDisconnected;
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x0017153C File Offset: 0x0016F73C
	public bool TryStartKeyExchange(ClientInfo clientInfo)
	{
		if (!AntiCheatEncryptionAuthServer.platformPublicKeys.ContainsKey(clientInfo.device))
		{
			Log.Out(string.Format("[EncryptionAgreement] Unsupported client device {0}", clientInfo.device));
			return false;
		}
		Log.Out("[EncryptionAgreement] starting key exchange for " + clientInfo.playerName);
		clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEncryptionRequest>().Setup());
		return true;
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x001715A0 File Offset: 0x0016F7A0
	public void SendSharedKey(ClientInfo clientInfo, string exchangePublicKeyParamsXml, byte[] hash, byte[] signedHash)
	{
		string xmlString;
		if (AntiCheatEncryptionAuthServer.platformPublicKeys.TryGetValue(clientInfo.device, out xmlString))
		{
			Log.Out("[EncryptionAgreement] Client " + clientInfo.playerName + " authenticating device");
			using (RSA rsa = RSA.Create())
			{
				rsa.FromXmlString(xmlString);
				RSAPKCS1SignatureDeformatter rsapkcs1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
				rsapkcs1SignatureDeformatter.SetHashAlgorithm("SHA512");
				if (!rsapkcs1SignatureDeformatter.VerifySignature(hash, signedHash))
				{
					Log.Warning("[EncryptionAgreement] Client " + clientInfo.playerName + " signature failed to verify");
					AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate onExchangeFailed = this.OnExchangeFailed;
					if (onExchangeFailed != null)
					{
						onExchangeFailed(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionAgreementInvalidSignature, 0, default(DateTime), ""));
					}
				}
				else
				{
					AesEncryptAndMac aesEncryptAndMac = new AesEncryptAndMac();
					using (RSA rsa2 = RSA.Create())
					{
						rsa2.FromXmlString(exchangePublicKeyParamsXml);
						byte[] encryptionKey = rsa2.Encrypt(aesEncryptAndMac.EncryptionKey, RSAEncryptionPadding.Pkcs1);
						byte[] integrityKey = rsa2.Encrypt(aesEncryptAndMac.IntegrityKey, RSAEncryptionPadding.Pkcs1);
						clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEncryptionSharedKey>().Setup(encryptionKey, integrityKey));
						this.pendingEncryptionModules.Add(clientInfo, aesEncryptAndMac);
					}
				}
			}
			return;
		}
		Log.Error(string.Format("[EncryptionAgreement] Cannot complete key exchange for {0}, unsupported client device {1}", clientInfo.playerName, clientInfo.device));
		AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate onExchangeFailed2 = this.OnExchangeFailed;
		if (onExchangeFailed2 == null)
		{
			return;
		}
		onExchangeFailed2(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionAgreementError, 0, default(DateTime), ""));
	}

	// Token: 0x06003826 RID: 14374 RVA: 0x00171720 File Offset: 0x0016F920
	public void CompleteKeyExchange(ClientInfo clientInfo, bool wasSuccessful)
	{
		IEncryptionModule encryptionModule;
		if (!wasSuccessful)
		{
			Log.Error("[EncryptionAgreement] Client " + clientInfo.playerName + " had an error during key exchange");
			this.pendingEncryptionModules.Remove(clientInfo);
			AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate onExchangeFailed = this.OnExchangeFailed;
			if (onExchangeFailed == null)
			{
				return;
			}
			onExchangeFailed(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionAgreementError, 0, default(DateTime), ""));
			return;
		}
		else if (!this.pendingEncryptionModules.TryGetValue(clientInfo, out encryptionModule))
		{
			Log.Error("[EncryptionAgreement] Client " + clientInfo.playerName + " tried to complete key exchange but had no encryption module ready");
			AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate onExchangeFailed2 = this.OnExchangeFailed;
			if (onExchangeFailed2 == null)
			{
				return;
			}
			onExchangeFailed2(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionAgreementError, 0, default(DateTime), ""));
			return;
		}
		else
		{
			Log.Out("[EncryptionAgreement] Client " + clientInfo.playerName + " enabling encryption");
			AntiCheatEncryptionAuthServer.KeyExchangeCompleteDelegate onExchangeCompleted = this.OnExchangeCompleted;
			if (onExchangeCompleted == null)
			{
				return;
			}
			onExchangeCompleted(clientInfo, encryptionModule);
			return;
		}
	}

	// Token: 0x06003827 RID: 14375 RVA: 0x001717FD File Offset: 0x0016F9FD
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClientDisconnected(ClientInfo clientInfo)
	{
		this.CancelKeyExchange(clientInfo);
	}

	// Token: 0x06003828 RID: 14376 RVA: 0x00171806 File Offset: 0x0016FA06
	public void CancelKeyExchange(ClientInfo clientInfo)
	{
		this.pendingEncryptionModules.Remove(clientInfo);
	}

	// Token: 0x06003829 RID: 14377 RVA: 0x00171815 File Offset: 0x0016FA15
	public void Stop()
	{
		ConnectionManager.OnClientDisconnected -= this.OnClientDisconnected;
		Dictionary<ClientInfo, IEncryptionModule> dictionary = this.pendingEncryptionModules;
		if (dictionary != null)
		{
			dictionary.Clear();
		}
		this.pendingEncryptionModules = null;
		this.OnExchangeCompleted = null;
		this.OnExchangeFailed = null;
	}

	// Token: 0x04002E39 RID: 11833
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<ClientInfo.EDeviceType, string> platformPublicKeys = new Dictionary<ClientInfo.EDeviceType, string>
	{
		{
			ClientInfo.EDeviceType.Xbox,
			"<RSAKeyValue><Modulus>o/zBUXWrPeOf/slmoNVAAtXZwk59QG3tVlcYafh2TcXZxsFLeWEpluIANHDQrRe9O1RGfCAxSd/ikpe/lfmsh8zP4Zcu7ZjQF8IMkQOIYJhbAAM+nVKM6FA06eKm15OyTJGvFWAbFD/TtZN1VFEZ25wveksFfYPEQYrA8LaY9iU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
		},
		{
			ClientInfo.EDeviceType.PlayStation,
			"<RSAKeyValue><Modulus>gClxi32aMHwKq/AWx83x6s8TWZ1Hdi/ONARtm34O5C8JJ4ma96m8iL9ITkn5p8tpunFPRXjdjxbxjGTm+E7vnpTA+aR0SQzcFdYpRkHDA3BwEY86Qr10Bj7TK9prxlf7Jf3eGyb/tV52jTC7YD+w5yaE8EzO5bJTRKrwGIiIZOk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
		}
	};

	// Token: 0x04002E3A RID: 11834
	[PublicizedFrom(EAccessModifier.Private)]
	public AntiCheatEncryptionAuthServer.KeyExchangeCompleteDelegate OnExchangeCompleted;

	// Token: 0x04002E3B RID: 11835
	[PublicizedFrom(EAccessModifier.Private)]
	public AntiCheatEncryptionAuthServer.KeyExchangeFailedDelegate OnExchangeFailed;

	// Token: 0x04002E3C RID: 11836
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<ClientInfo, IEncryptionModule> pendingEncryptionModules;

	// Token: 0x02000764 RID: 1892
	// (Invoke) Token: 0x0600382D RID: 14381
	public delegate void KeyExchangeCompleteDelegate(ClientInfo clientInfo, IEncryptionModule encryptionModule);

	// Token: 0x02000765 RID: 1893
	// (Invoke) Token: 0x06003831 RID: 14385
	public delegate void KeyExchangeFailedDelegate(ClientInfo clientInfo, GameUtils.KickPlayerData reason);
}
