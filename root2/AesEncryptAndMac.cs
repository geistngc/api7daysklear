using System;
using System.IO;
using System.Security.Cryptography;

// Token: 0x02000767 RID: 1895
public class AesEncryptAndMac : IEncryptionModule
{
	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x06003838 RID: 14392 RVA: 0x00171A03 File Offset: 0x0016FC03
	public byte[] EncryptionKey
	{
		get
		{
			return this.aes.Key;
		}
	}

	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x06003839 RID: 14393 RVA: 0x00171A10 File Offset: 0x0016FC10
	public byte[] IntegrityKey
	{
		get
		{
			return this.hmac.Key;
		}
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x00171A20 File Offset: 0x0016FC20
	public AesEncryptAndMac()
	{
		this.aes = Aes.Create();
		this.aes.GenerateKey();
		this.aes.GenerateIV();
		this.encryptor = this.aes.CreateEncryptor();
		this.decryptor = this.aes.CreateDecryptor();
		this.hmac = new HMACSHA256();
		this.random = RandomNumberGenerator.Create();
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x00171A98 File Offset: 0x0016FC98
	public AesEncryptAndMac(byte[] encryptionKey, byte[] integrityKey)
	{
		this.aes = Aes.Create();
		this.aes.Key = encryptionKey;
		this.aes.GenerateIV();
		this.encryptor = this.aes.CreateEncryptor();
		this.decryptor = this.aes.CreateDecryptor();
		this.hmac = new HMACSHA256(integrityKey);
		this.random = RandomNumberGenerator.Create();
	}

	// Token: 0x0600383C RID: 14396 RVA: 0x00171B14 File Offset: 0x0016FD14
	public unsafe bool EncryptStream(ClientInfo _cInfo, MemoryStream _stream)
	{
		int inputBlockSize = this.encryptor.InputBlockSize;
		Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)inputBlockSize], inputBlockSize);
		PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(false);
		int num = (int)_stream.Length;
		object obj = this.lockObj;
		byte[] hash;
		lock (obj)
		{
			using (CryptoStream cryptoStream = new CryptoStream(pooledExpandableMemoryStream, this.hmac, CryptoStreamMode.Write, true))
			{
				using (CryptoStream cryptoStream2 = new CryptoStream(cryptoStream, this.encryptor, CryptoStreamMode.Write))
				{
					this.random.GetBytes(span);
					cryptoStream2.Write(span);
					StreamUtils.Write(cryptoStream2, num);
					cryptoStream2.Write(_stream.GetBuffer(), 0, num);
				}
			}
			hash = this.hmac.Hash;
		}
		_stream.SetLength(0L);
		_stream.Write(hash);
		_stream.Write(pooledExpandableMemoryStream.GetBuffer(), 0, (int)pooledExpandableMemoryStream.Length);
		_stream.Position = 0L;
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return true;
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x00171C4C File Offset: 0x0016FE4C
	public unsafe bool DecryptStream(ClientInfo _cInfo, MemoryStream _stream)
	{
		int num = this.hmac.HashSize / 8;
		Span<byte> buffer = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
		_stream.Read(buffer);
		num = this.decryptor.OutputBlockSize;
		Span<byte> buffer2 = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
		int num2 = 0;
		object obj = this.lockObj;
		byte[] array;
		byte[] hash;
		lock (obj)
		{
			using (CryptoStream cryptoStream = new CryptoStream(_stream, this.hmac, CryptoStreamMode.Read, true))
			{
				using (CryptoStream cryptoStream2 = new CryptoStream(cryptoStream, this.decryptor, CryptoStreamMode.Read))
				{
					cryptoStream2.Read(buffer2);
					num2 = StreamUtils.ReadInt32(cryptoStream2);
					array = MemoryPools.poolByte.Alloc(num2);
					cryptoStream2.Read(array, 0, num2);
				}
			}
			hash = this.hmac.Hash;
		}
		if (buffer.Length != hash.Length)
		{
			Log.Error(string.Format("[EncryptionAgreement] decryption failure, MAC bytes not the same length. Calculated {0}, Remote {1}", this.hmac.Hash.Length, buffer.Length));
			return false;
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			if (*buffer[i] != hash[i])
			{
				Log.Error("[EncryptionAgreement] decryption failure, MAC did not match");
				return false;
			}
		}
		_stream.SetLength(0L);
		_stream.Write(array, 0, num2);
		_stream.Position = 0L;
		MemoryPools.poolByte.Free(array);
		return true;
	}

	// Token: 0x04002E3E RID: 11838
	[PublicizedFrom(EAccessModifier.Private)]
	public Aes aes;

	// Token: 0x04002E3F RID: 11839
	[PublicizedFrom(EAccessModifier.Private)]
	public ICryptoTransform encryptor;

	// Token: 0x04002E40 RID: 11840
	[PublicizedFrom(EAccessModifier.Private)]
	public ICryptoTransform decryptor;

	// Token: 0x04002E41 RID: 11841
	[PublicizedFrom(EAccessModifier.Private)]
	public HMAC hmac;

	// Token: 0x04002E42 RID: 11842
	[PublicizedFrom(EAccessModifier.Private)]
	public RandomNumberGenerator random;

	// Token: 0x04002E43 RID: 11843
	[PublicizedFrom(EAccessModifier.Private)]
	public object lockObj = new object();
}
