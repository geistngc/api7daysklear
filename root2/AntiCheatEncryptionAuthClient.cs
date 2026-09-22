using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000766 RID: 1894
public class AntiCheatEncryptionAuthClient
{
	// Token: 0x06003834 RID: 14388 RVA: 0x000880CC File Offset: 0x000862CC
	[PublicizedFrom(EAccessModifier.Private)]
	public RSA GetSigningKey()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x00171874 File Offset: 0x0016FA74
	public void StartKeyExchange()
	{
		Log.Out("[EncryptionAgreement] checking signing key");
		RSA signingKey;
		try
		{
			signingKey = this.GetSigningKey();
		}
		catch (Exception e)
		{
			Log.Exception(e);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageKeyExchangeComplete>().Setup(false), false);
			return;
		}
		Log.Out("[EncryptionAgreement] creating key exchange params");
		this.keyExchangeSessionPair = RSA.Create(2048);
		string text = this.keyExchangeSessionPair.ToXmlString(false);
		Log.Out("[EncryptionAgreement] signing params");
		using (SHA512 sha = SHA512.Create())
		{
			byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
			RSAPKCS1SignatureFormatter rsapkcs1SignatureFormatter = new RSAPKCS1SignatureFormatter(signingKey);
			rsapkcs1SignatureFormatter.SetHashAlgorithm("SHA512");
			byte[] signedHash = rsapkcs1SignatureFormatter.CreateSignature(array);
			Log.Out("[EncryptionAgreement] sending params to server");
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEncryptionPublicKey>().Setup(text, array, signedHash), false);
			signingKey.Dispose();
		}
	}

	// Token: 0x06003836 RID: 14390 RVA: 0x00171968 File Offset: 0x0016FB68
	public void CompleteKeyExchange(byte[] protectedEncryptionKey, byte[] protectedIntegrityKey)
	{
		Log.Out("[EncryptionAgreement] received shared keys");
		byte[] encryptionKey = this.keyExchangeSessionPair.Decrypt(protectedEncryptionKey, RSAEncryptionPadding.Pkcs1);
		byte[] integrityKey = this.keyExchangeSessionPair.Decrypt(protectedIntegrityKey, RSAEncryptionPadding.Pkcs1);
		AesEncryptAndMac encryptionModule = new AesEncryptAndMac(encryptionKey, integrityKey);
		INetConnection[] connectionToServer = SingletonMonoBehaviour<ConnectionManager>.Instance.GetConnectionToServer();
		for (int i = 0; i < connectionToServer.Length; i++)
		{
			connectionToServer[i].SetEncryptionModule(encryptionModule);
		}
		Log.Out("[EncryptionAgreement] sending reply");
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageKeyExchangeComplete>().Setup(true), false);
		RSA rsa = this.keyExchangeSessionPair;
		if (rsa != null)
		{
			rsa.Dispose();
		}
		this.keyExchangeSessionPair = null;
	}

	// Token: 0x04002E3D RID: 11837
	[PublicizedFrom(EAccessModifier.Private)]
	public RSA keyExchangeSessionPair;
}
