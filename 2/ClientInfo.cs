using System;
using System.Collections.Generic;
using Platform.Local;
using Platform.Steam;

// Token: 0x0200076B RID: 1899
public class ClientInfo : IEquatable<ClientInfo>
{
	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x06003850 RID: 14416 RVA: 0x0017249D File Offset: 0x0017069D
	public PlatformUserIdentifierAbs InternalId
	{
		get
		{
			return this.CrossplatformId ?? this.PlatformId;
		}
	}

	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x06003851 RID: 14417 RVA: 0x001724AF File Offset: 0x001706AF
	public string ip
	{
		get
		{
			return this.network.GetIP(this);
		}
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x001724C0 File Offset: 0x001706C0
	public ClientInfo()
	{
		int num;
		do
		{
			num = ++ClientInfo.lastClientNumber;
			if (num > 1000000)
			{
				num = (ClientInfo.lastClientNumber = 1);
			}
		}
		while (SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForClientNumber(num) != null);
		this.ClientNumber = num;
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x00172548 File Offset: 0x00170748
	public override string ToString()
	{
		string text = null;
		UserIdentifierSteam userIdentifierSteam = this.PlatformId as UserIdentifierSteam;
		if (userIdentifierSteam != null)
		{
			UserIdentifierSteam ownerId = userIdentifierSteam.OwnerId;
			text = ((ownerId != null) ? ownerId.CombinedString : null);
		}
		string format = "EntityID={0}, PltfmId='{1}', CrossId='{2}', OwnerID='{3}', PlayerName='{4}', ClientNumber='{5}'";
		object[] array = new object[6];
		array[0] = this.entityId;
		int num = 1;
		PlatformUserIdentifierAbs platformId = this.PlatformId;
		array[num] = (((platformId != null) ? platformId.CombinedString : null) ?? "<unknown>");
		int num2 = 2;
		PlatformUserIdentifierAbs crossplatformId = this.CrossplatformId;
		array[num2] = (((crossplatformId != null) ? crossplatformId.CombinedString : null) ?? "<unknown/none>");
		array[3] = (text ?? "<unknown/none>");
		array[4] = this.playerName;
		array[5] = this.ClientNumber;
		return string.Format(format, array);
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x001725F7 File Offset: 0x001707F7
	public void UpdatePing()
	{
		this.ping = this.network.GetPing(this);
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x0017260C File Offset: 0x0017080C
	public void SendPackage(NetPackage _package)
	{
		if (!_package.AllowedBeforeAuth && !this.loginDone)
		{
			Log.Warning(string.Format("Ignoring {0}, not logged in yet", _package));
			return;
		}
		this.netConnection[_package.Channel].AddToSendQueue(_package);
		if (_package.FlushQueue)
		{
			this.netConnection[_package.Channel].FlushSendQueue();
		}
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x00172668 File Offset: 0x00170868
	public void SetAntiCheatEncryption(IEncryptionModule encryptionModule)
	{
		INetConnection[] array = this.netConnection;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetEncryptionModule(encryptionModule);
		}
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x00172693 File Offset: 0x00170893
	public bool Equals(ClientInfo _other)
	{
		return this == _other;
	}

	// Token: 0x04002E47 RID: 11847
	[PublicizedFrom(EAccessModifier.Private)]
	public static int lastClientNumber;

	// Token: 0x04002E48 RID: 11848
	public INetworkServer network;

	// Token: 0x04002E49 RID: 11849
	public readonly int ClientNumber;

	// Token: 0x04002E4A RID: 11850
	public long litenetPeerConnectId = -1L;

	// Token: 0x04002E4B RID: 11851
	public PlatformUserIdentifierAbs PlatformId = new UserIdentifierLocal("<none>");

	// Token: 0x04002E4C RID: 11852
	public PlatformUserIdentifierAbs CrossplatformId;

	// Token: 0x04002E4D RID: 11853
	public ulong DiscordUserId;

	// Token: 0x04002E4E RID: 11854
	public bool requiresAntiCheat = true;

	// Token: 0x04002E4F RID: 11855
	public ClientInfo.EDeviceType device = ClientInfo.EDeviceType.Unknown;

	// Token: 0x04002E50 RID: 11856
	public bool loginDone;

	// Token: 0x04002E51 RID: 11857
	public bool acAuthDone;

	// Token: 0x04002E52 RID: 11858
	public INetConnection[] netConnection;

	// Token: 0x04002E53 RID: 11859
	public bool bAttachedToEntity;

	// Token: 0x04002E54 RID: 11860
	public int entityId = -1;

	// Token: 0x04002E55 RID: 11861
	public string playerName;

	// Token: 0x04002E56 RID: 11862
	public string compatibilityVersion;

	// Token: 0x04002E57 RID: 11863
	public readonly Dictionary<string, int> groupMemberships = new Dictionary<string, int>(StringComparer.Ordinal);

	// Token: 0x04002E58 RID: 11864
	public int groupMembershipsWaiting;

	// Token: 0x04002E59 RID: 11865
	public PlayerDataFile latestPlayerData;

	// Token: 0x04002E5A RID: 11866
	public int ping;

	// Token: 0x04002E5B RID: 11867
	public bool disconnecting;

	// Token: 0x0200076C RID: 1900
	public enum EDeviceType
	{
		// Token: 0x04002E5D RID: 11869
		Linux,
		// Token: 0x04002E5E RID: 11870
		Mac,
		// Token: 0x04002E5F RID: 11871
		Windows,
		// Token: 0x04002E60 RID: 11872
		PlayStation,
		// Token: 0x04002E61 RID: 11873
		Xbox,
		// Token: 0x04002E62 RID: 11874
		Unknown
	}
}
