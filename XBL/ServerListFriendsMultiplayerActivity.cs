using System;
using System.Collections.Generic;
using System.Threading;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001C04 RID: 7172
	public class ServerListFriendsMultiplayerActivity : IServerListInterface
	{
		// Token: 0x17001A6A RID: 6762
		// (get) Token: 0x0600D51E RID: 54558 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001A6B RID: 6763
		// (get) Token: 0x0600D51F RID: 54559 RVA: 0x004D059E File Offset: 0x004CE79E
		public bool IsRefreshing
		{
			get
			{
				return this.isLoadingFriendsList || this.activitySearchCount > 0 || this.sessionSearchCount > 0;
			}
		}

		// Token: 0x0600D520 RID: 54560 RVA: 0x004D05BC File Offset: 0x004CE7BC
		public void Init(IPlatform _owner)
		{
			this.user = (User)_owner.User;
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			this.serverLookup = ((crossplatformPlatform != null) ? crossplatformPlatform.ServerLookupInterface : null);
			if (this.serverLookup == null)
			{
				Log.Error("[XBL] no crossplatform server lookup interface provided, friends session search is not possible");
				return;
			}
		}

		// Token: 0x0600D521 RID: 54561 RVA: 0x004D05F9 File Offset: 0x004CE7F9
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _sessionSearchErrorCallback)
		{
			if (this.serverLookup == null)
			{
				return;
			}
			this.gameServerFoundCallback = _serverFound;
		}

		// Token: 0x0600D522 RID: 54562 RVA: 0x004D060C File Offset: 0x004CE80C
		public void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			if (this.serverLookup == null)
			{
				return;
			}
			XblSocialManagerUserGroupHandle xblSocialManagerUserGroupHandle;
			if (!this.user.SocialManager.TryCreateUserGroup(XblPresenceFilter.TitleOnline, XblRelationshipFilter.Friends, new SocialManagerXbl.UserGroupMembersChanged(this.OnlineFriendsListUpdated), out xblSocialManagerUserGroupHandle))
			{
				Log.Error("[XBL] could not create friends user group, friends session search will fail");
				return;
			}
			Log.Out("[XBL] ServerListFriendsMultiplayerActivity starting search");
			this.userGroupHandle = xblSocialManagerUserGroupHandle;
			this.isLoadingFriendsList = true;
		}

		// Token: 0x0600D523 RID: 54563 RVA: 0x004D0667 File Offset: 0x004CE867
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnlineFriendsListUpdated(ulong[] _users)
		{
			this.isLoadingFriendsList = false;
			if (_users == null || _users.Length == 0)
			{
				return;
			}
			Interlocked.Increment(ref this.activitySearchCount);
			this.user.MultiplayerActivityQueryManager.GetActivityAsync(_users, new MultiplayerActivityQueryManager.OnGetActivityComplete(this.OnActivitiesRetrieved));
		}

		// Token: 0x0600D524 RID: 54564 RVA: 0x004D06A4 File Offset: 0x004CE8A4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnActivitiesRetrieved(ulong[] _searchedXuids, List<XblMultiplayerActivityInfo> _results)
		{
			int num = 0;
			using (List<XblMultiplayerActivityInfo>.Enumerator enumerator = _results.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!string.IsNullOrEmpty(enumerator.Current.ConnectionString))
					{
						num++;
					}
				}
			}
			if (num == 0 || this.gameServerFoundCallback == null)
			{
				Interlocked.Decrement(ref this.activitySearchCount);
				return;
			}
			ThreadManager.AddSingleTaskMainThread("SearchXboxActivitySessions", delegate(object param)
			{
				foreach (XblMultiplayerActivityInfo xblMultiplayerActivityInfo in _results)
				{
					if (xblMultiplayerActivityInfo.JoinRestriction != XblMultiplayerActivityJoinRestriction.InviteOnly && !string.IsNullOrEmpty(xblMultiplayerActivityInfo.ConnectionString))
					{
						GameServerInfo gameServerInfo = new GameServerInfo();
						gameServerInfo.SetValue(GameInfoString.UniqueId, xblMultiplayerActivityInfo.ConnectionString);
						Interlocked.Increment(ref this.sessionSearchCount);
						this.serverLookup.GetSingleServerDetails(gameServerInfo, EServerRelationType.Friends, new GameServerFoundCallback(this.OnServerFound));
					}
				}
				Interlocked.Decrement(ref this.activitySearchCount);
			}, null);
		}

		// Token: 0x0600D525 RID: 54565 RVA: 0x004D0744 File Offset: 0x004CE944
		public void OnServerFound(IPlatform _sourcePlatform, GameServerInfo _info, EServerRelationType _source)
		{
			if (_info == null)
			{
				return;
			}
			_info.IsFriends = true;
			GameServerFoundCallback gameServerFoundCallback = this.gameServerFoundCallback;
			if (gameServerFoundCallback != null)
			{
				gameServerFoundCallback(_sourcePlatform, _info, _source);
			}
			Interlocked.Decrement(ref this.sessionSearchCount);
		}

		// Token: 0x0600D526 RID: 54566 RVA: 0x004D0771 File Offset: 0x004CE971
		public void StopSearch()
		{
			if (this.userGroupHandle != null)
			{
				this.user.SocialManager.DestroyUserGroup(this.userGroupHandle);
			}
			this.userGroupHandle = null;
		}

		// Token: 0x0600D527 RID: 54567 RVA: 0x004D079E File Offset: 0x004CE99E
		public void Disconnect()
		{
			this.StopSearch();
			this.gameServerFoundCallback = null;
		}

		// Token: 0x0600D528 RID: 54568 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400A25A RID: 41562
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x0400A25B RID: 41563
		[PublicizedFrom(EAccessModifier.Private)]
		public User user;

		// Token: 0x0400A25C RID: 41564
		[PublicizedFrom(EAccessModifier.Private)]
		public IServerListInterface serverLookup;

		// Token: 0x0400A25D RID: 41565
		[PublicizedFrom(EAccessModifier.Private)]
		public XblSocialManagerUserGroupHandle userGroupHandle;

		// Token: 0x0400A25E RID: 41566
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isLoadingFriendsList;

		// Token: 0x0400A25F RID: 41567
		[PublicizedFrom(EAccessModifier.Private)]
		public int activitySearchCount;

		// Token: 0x0400A260 RID: 41568
		[PublicizedFrom(EAccessModifier.Private)]
		public int sessionSearchCount;
	}
}
