using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x02000769 RID: 1897
public class AuthorizationManager : IAuthorizationResponses
{
	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x06003840 RID: 14400 RVA: 0x00171DE8 File Offset: 0x0016FFE8
	public static AuthorizationManager Instance
	{
		get
		{
			AuthorizationManager result;
			if ((result = AuthorizationManager.instance) == null)
			{
				result = (AuthorizationManager.instance = new AuthorizationManager());
			}
			return result;
		}
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x00171E00 File Offset: 0x00170000
	public void Init()
	{
		new SortedList<string, IConsoleCommand>();
		ReflectionHelpers.FindTypesImplementingBase(typeof(IAuthorizer), delegate(Type _type)
		{
			IAuthorizer key = ReflectionHelpers.Instantiate<IAuthorizer>(_type);
			this.authorizers.Add(key, 0);
		}, false);
		foreach (IAuthorizer authorizer in this.authorizers.Keys)
		{
			authorizer.Init(this);
		}
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x00171E74 File Offset: 0x00170074
	public void Cleanup()
	{
		foreach (IAuthorizer authorizer in this.authorizers.Keys)
		{
			authorizer.Cleanup();
		}
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x00171EC4 File Offset: 0x001700C4
	public void ServerStart()
	{
		foreach (IAuthorizer authorizer in this.authorizers.Keys)
		{
			authorizer.ServerStart();
		}
		this.clientsInAuthorization.Clear();
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x00171F20 File Offset: 0x00170120
	public void ServerStop()
	{
		foreach (IAuthorizer authorizer in this.authorizers.Keys)
		{
			authorizer.ServerStop();
		}
		this.clientsInAuthorization.Clear();
	}

	// Token: 0x06003845 RID: 14405 RVA: 0x00171F7C File Offset: 0x0017017C
	public void Authorize(ClientInfo _clientInfo, string _playerName, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _platformUserAndToken, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _crossplatformUserAndToken, string _compatibilityVersion, ulong _discordUserId)
	{
		this.clientsInAuthorization.Add(_clientInfo);
		PlatformUserIdentifierAbs item = _platformUserAndToken.Item1;
		if (item != null)
		{
			item.DecodeTicket(_platformUserAndToken.Item2);
		}
		PlatformUserIdentifierAbs item2 = _crossplatformUserAndToken.Item1;
		if (item2 != null)
		{
			item2.DecodeTicket(_crossplatformUserAndToken.Item2);
		}
		_clientInfo.playerName = _playerName;
		_clientInfo.compatibilityVersion = _compatibilityVersion;
		_clientInfo.PlatformId = _platformUserAndToken.Item1;
		_clientInfo.CrossplatformId = _crossplatformUserAndToken.Item1;
		_clientInfo.DiscordUserId = _discordUserId;
		this.tryAuthorizer(0, _clientInfo);
	}

	// Token: 0x06003846 RID: 14406 RVA: 0x00172000 File Offset: 0x00170200
	[PublicizedFrom(EAccessModifier.Private)]
	public void tryAuthorizer(int _currentIndex, ClientInfo _clientInfo)
	{
		while (_currentIndex < this.authorizers.Count)
		{
			IAuthorizer authorizer = this.authorizers.Keys[_currentIndex];
			bool authorizerActive = authorizer.AuthorizerActive;
			bool flag = true;
			EPlatformIdentifier platformRestriction = authorizer.PlatformRestriction;
			if (platformRestriction < EPlatformIdentifier.Count)
			{
				flag = (platformRestriction == _clientInfo.PlatformId.PlatformIdentifier || (_clientInfo.CrossplatformId != null && platformRestriction == _clientInfo.CrossplatformId.PlatformIdentifier));
			}
			bool flag2 = !authorizerActive || !flag;
			_currentIndex++;
			if (!flag2)
			{
				if (authorizer.StateLocalizationKey != null)
				{
					_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageAuthState>().Setup(authorizer.StateLocalizationKey));
				}
				ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> valueTuple = authorizer.Authorize(_clientInfo);
				EAuthorizerSyncResult item = valueTuple.Item1;
				GameUtils.KickPlayerData? item2 = valueTuple.Item2;
				switch (item)
				{
				case EAuthorizerSyncResult.WaitAsync:
					return;
				case EAuthorizerSyncResult.SyncAllow:
					this.AuthorizationAccepted(authorizer, _clientInfo);
					return;
				case EAuthorizerSyncResult.SyncDeny:
					this.AuthorizationDenied(authorizer, _clientInfo, item2.Value);
					return;
				case EAuthorizerSyncResult.SyncFinalAllow:
					this.playerAllowed(_clientInfo);
					return;
				default:
					return;
				}
			}
		}
		this.playerAllowed(_clientInfo);
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x001720F6 File Offset: 0x001702F6
	public void AuthorizationDenied(IAuthorizer _authorizer, ClientInfo _clientInfo, GameUtils.KickPlayerData _kickPlayerData)
	{
		if (_authorizer != null)
		{
			Log.Out(string.Format("[Auth] {0} authorization failed: {1}", _authorizer.AuthorizerName, _clientInfo));
		}
		this.clientsInAuthorization.Remove(_clientInfo);
		GameUtils.KickPlayerForClientInfo(_clientInfo, _kickPlayerData);
	}

	// Token: 0x06003848 RID: 14408 RVA: 0x00172128 File Offset: 0x00170328
	public void AuthorizationAccepted(IAuthorizer _authorizer, ClientInfo _clientInfo)
	{
		Log.Out(string.Format("[Auth] {0} authorization successful: {1}", _authorizer.AuthorizerName, _clientInfo));
		int num = this.authorizers.IndexOfKey(_authorizer);
		if (!this.clientsInAuthorization.Contains(_clientInfo))
		{
			return;
		}
		this.tryAuthorizer(num + 1, _clientInfo);
	}

	// Token: 0x06003849 RID: 14409 RVA: 0x00172174 File Offset: 0x00170374
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerAllowed(ClientInfo _clientInfo)
	{
		this.clientsInAuthorization.Remove(_clientInfo);
		if (_clientInfo.loginDone)
		{
			return;
		}
		_clientInfo.loginDone = true;
		INetConnection[] netConnection = _clientInfo.netConnection;
		for (int i = 0; i < netConnection.Length; i++)
		{
			netConnection[i].UpgradeToFullConnection();
		}
		Log.Out("Allowing player with id " + _clientInfo.InternalId.CombinedString);
		_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageAuthState>().Setup("authstate_authenticated"));
		try
		{
			string data = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.ToString();
			PlatformLobbyId platformLobbyId = PlatformLobbyId.None;
			if (PlatformManager.NativePlatform.PlatformIdentifier == _clientInfo.PlatformId.PlatformIdentifier)
			{
				ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
				if (lobbyHost != null && lobbyHost.IsInLobby)
				{
					platformLobbyId = new PlatformLobbyId(PlatformManager.NativePlatform.PlatformIdentifier, PlatformManager.NativePlatform.LobbyHost.LobbyId);
					goto IL_EB;
				}
			}
			PlatformLobbyId platformLobbyId2;
			if (PlatformManager.ClientLobbyManager.TryGetLobbyId(_clientInfo.PlatformId.PlatformIdentifier, out platformLobbyId2))
			{
				platformLobbyId = platformLobbyId2;
			}
			IL_EB:
			ValueTuple<PlatformUserIdentifierAbs, string> platformUserAndToken;
			ValueTuple<PlatformUserIdentifierAbs, string> crossplatformUserAndToken;
			if (GameManager.IsDedicatedServer)
			{
				platformUserAndToken = default(ValueTuple<PlatformUserIdentifierAbs, string>);
				crossplatformUserAndToken = default(ValueTuple<PlatformUserIdentifierAbs, string>);
			}
			else
			{
				PlatformUserIdentifierAbs platformUserId = PlatformManager.NativePlatform.User.PlatformUserId;
				IAuthenticationClient authenticationClient = PlatformManager.NativePlatform.AuthenticationClient;
				platformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(platformUserId, (authenticationClient != null) ? authenticationClient.GetAuthTicket() : null);
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				PlatformUserIdentifierAbs item;
				if (crossplatformPlatform == null)
				{
					item = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					item = ((user != null) ? user.PlatformUserId : null);
				}
				IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
				string item2;
				if (crossplatformPlatform2 == null)
				{
					item2 = null;
				}
				else
				{
					IAuthenticationClient authenticationClient2 = crossplatformPlatform2.AuthenticationClient;
					item2 = ((authenticationClient2 != null) ? authenticationClient2.GetAuthTicket() : null);
				}
				crossplatformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(item, item2);
			}
			_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerLoginAnswer>().Setup(true, data, platformLobbyId, platformUserAndToken, crossplatformUserAndToken));
		}
		catch (Exception ex)
		{
			string str = "Exception in playerAllowed: ";
			Exception ex2 = ex;
			Log.Error(str + ((ex2 != null) ? ex2.ToString() : null));
			SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(_clientInfo, false, false);
		}
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x00172358 File Offset: 0x00170558
	public void Disconnect(ClientInfo _cInfo)
	{
		if (!ThreadManager.IsMainThread())
		{
			ThreadManager.AddSingleTaskMainThread("Auth.Disconnect-" + _cInfo.ClientNumber.ToString(), delegate(object _parameter)
			{
				this.Disconnect((ClientInfo)_parameter);
			}, _cInfo);
			return;
		}
		this.clientsInAuthorization.Remove(_cInfo);
		for (int i = this.authorizers.Keys.Count - 1; i >= 0; i--)
		{
			IAuthorizer authorizer = this.authorizers.Keys[i];
			bool authorizerActive = authorizer.AuthorizerActive;
			bool flag = true;
			EPlatformIdentifier platformRestriction = authorizer.PlatformRestriction;
			if (platformRestriction < EPlatformIdentifier.Count)
			{
				EPlatformIdentifier eplatformIdentifier = platformRestriction;
				PlatformUserIdentifierAbs platformId = _cInfo.PlatformId;
				bool flag2;
				if (eplatformIdentifier != ((platformId != null) ? platformId.PlatformIdentifier : EPlatformIdentifier.Count))
				{
					EPlatformIdentifier eplatformIdentifier2 = platformRestriction;
					PlatformUserIdentifierAbs crossplatformId = _cInfo.CrossplatformId;
					flag2 = (eplatformIdentifier2 == ((crossplatformId != null) ? crossplatformId.PlatformIdentifier : EPlatformIdentifier.Count));
				}
				else
				{
					flag2 = true;
				}
				flag = flag2;
			}
			if (authorizerActive && flag)
			{
				authorizer.Disconnect(_cInfo);
			}
		}
	}

	// Token: 0x04002E44 RID: 11844
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly SortedList<IAuthorizer, int> authorizers = new SortedList<IAuthorizer, int>(new AuthorizationManager.AuthorizerComparer());

	// Token: 0x04002E45 RID: 11845
	[PublicizedFrom(EAccessModifier.Private)]
	public static AuthorizationManager instance;

	// Token: 0x04002E46 RID: 11846
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<ClientInfo> clientsInAuthorization = new HashSet<ClientInfo>();

	// Token: 0x0200076A RID: 1898
	[PublicizedFrom(EAccessModifier.Private)]
	public class AuthorizerComparer : IComparer<IAuthorizer>
	{
		// Token: 0x0600384E RID: 14414 RVA: 0x0017247C File Offset: 0x0017067C
		public int Compare(IAuthorizer _x, IAuthorizer _y)
		{
			return _x.Order.CompareTo(_y.Order);
		}
	}
}
