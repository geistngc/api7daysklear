using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Token: 0x0200076D RID: 1901
public class ClientInfoCollection
{
	// Token: 0x06003858 RID: 14424 RVA: 0x0017269C File Offset: 0x0017089C
	public ClientInfoCollection()
	{
		this.List = new ReadOnlyCollection<ClientInfo>(this.list);
	}

	// Token: 0x06003859 RID: 14425 RVA: 0x001726F7 File Offset: 0x001708F7
	public void Add(ClientInfo _cInfo)
	{
		this.list.Add(_cInfo);
		this.clientNumberMap.Add(_cInfo.ClientNumber, _cInfo);
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x00172717 File Offset: 0x00170917
	public void Clear()
	{
		this.list.Clear();
		this.clientNumberMap.Clear();
		this.entityIdMap.Clear();
		this.litenetPeerMap.Clear();
		this.userIdMap.Clear();
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x00172750 File Offset: 0x00170950
	public bool Contains(ClientInfo _cInfo)
	{
		return this.list.Contains(_cInfo);
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x00172760 File Offset: 0x00170960
	public void Remove(ClientInfo _cInfo)
	{
		this.list.Remove(_cInfo);
		this.clientNumberMap.Remove(_cInfo.ClientNumber);
		this.entityIdMap.Remove(_cInfo.entityId);
		if (_cInfo.litenetPeerConnectId >= 0L)
		{
			this.litenetPeerMap.Remove(_cInfo.litenetPeerConnectId);
		}
		if (_cInfo.PlatformId != null)
		{
			this.userIdMap.Remove(_cInfo.PlatformId);
		}
		if (_cInfo.CrossplatformId != null)
		{
			this.userIdMap.Remove(_cInfo.CrossplatformId);
		}
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x0600385D RID: 14429 RVA: 0x001727EE File Offset: 0x001709EE
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x001727FC File Offset: 0x001709FC
	public ClientInfo ForClientNumber(int _clientNumber)
	{
		ClientInfo result;
		if (this.clientNumberMap.TryGetValue(_clientNumber, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x0017281C File Offset: 0x00170A1C
	public ClientInfo ForEntityId(int _entityId)
	{
		ClientInfo result;
		if (this.entityIdMap.TryGetValue(_entityId, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (clientInfo.entityId == _entityId)
			{
				this.entityIdMap.Add(_entityId, clientInfo);
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x00172878 File Offset: 0x00170A78
	public ClientInfo ForLiteNetPeer(long _peerConnectId)
	{
		ClientInfo result;
		if (this.litenetPeerMap.TryGetValue(_peerConnectId, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (clientInfo.litenetPeerConnectId == _peerConnectId)
			{
				this.litenetPeerMap.Add(_peerConnectId, clientInfo);
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x001728D4 File Offset: 0x00170AD4
	public ClientInfo ForUserId(PlatformUserIdentifierAbs _userIdentifier)
	{
		ClientInfo result;
		if (this.userIdMap.TryGetValue(_userIdentifier, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (_userIdentifier.Equals(clientInfo.PlatformId))
			{
				this.userIdMap[_userIdentifier] = clientInfo;
				return clientInfo;
			}
			if (_userIdentifier.Equals(clientInfo.CrossplatformId))
			{
				this.userIdMap[_userIdentifier] = clientInfo;
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x00172954 File Offset: 0x00170B54
	public ClientInfo GetForPlayerName(string _playerName, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		if (_ignoreBlanks)
		{
			_playerName = _playerName.Replace(" ", "");
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			string text = clientInfo.playerName ?? string.Empty;
			if (_ignoreBlanks)
			{
				text = text.Replace(" ", "");
			}
			if (string.Equals(text, _playerName, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
			{
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x001729D0 File Offset: 0x00170BD0
	public ClientInfo GetForNameOrId(string _nameOrId, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		int entityId;
		if (int.TryParse(_nameOrId, out entityId))
		{
			ClientInfo clientInfo = this.ForEntityId(entityId);
			if (clientInfo != null)
			{
				return clientInfo;
			}
		}
		PlatformUserIdentifierAbs userIdentifier;
		if (PlatformUserIdentifierAbs.TryFromCombinedString(_nameOrId, out userIdentifier))
		{
			ClientInfo clientInfo2 = this.ForUserId(userIdentifier);
			if (clientInfo2 != null)
			{
				return clientInfo2;
			}
		}
		return this.GetForPlayerName(_nameOrId, _ignoreCase, _ignoreBlanks);
	}

	// Token: 0x04002E63 RID: 11875
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ClientInfo> list = new List<ClientInfo>();

	// Token: 0x04002E64 RID: 11876
	public readonly ReadOnlyCollection<ClientInfo> List;

	// Token: 0x04002E65 RID: 11877
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, ClientInfo> clientNumberMap = new Dictionary<int, ClientInfo>();

	// Token: 0x04002E66 RID: 11878
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, ClientInfo> entityIdMap = new Dictionary<int, ClientInfo>();

	// Token: 0x04002E67 RID: 11879
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<long, ClientInfo> litenetPeerMap = new Dictionary<long, ClientInfo>();

	// Token: 0x04002E68 RID: 11880
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, ClientInfo> userIdMap = new Dictionary<PlatformUserIdentifierAbs, ClientInfo>();
}
