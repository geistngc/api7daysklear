using System;
using System.Collections.Generic;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001BF9 RID: 7161
	public class FriendsListXbl
	{
		// Token: 0x0600D4CD RID: 54477 RVA: 0x004CF1E3 File Offset: 0x004CD3E3
		public FriendsListXbl(SocialManagerXbl socialManager)
		{
			this.socialManager = socialManager;
			if (!socialManager.TryCreateUserGroup(XblPresenceFilter.All, XblRelationshipFilter.Friends, new SocialManagerXbl.UserGroupMembersChanged(this.OnFriendsListChanged), out this.friendsUserGroup))
			{
				Log.Error("[FriendsListXbl] failed to create friends social manager group");
				this.friendsUserGroup = null;
			}
		}

		// Token: 0x0600D4CE RID: 54478 RVA: 0x004CF220 File Offset: 0x004CD420
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnFriendsListChanged(ulong[] friends)
		{
			if (this.friendsXuidsTemp == null)
			{
				this.friendsXuidsTemp = new HashSet<ulong>();
			}
			foreach (ulong item in friends)
			{
				this.friendsXuidsTemp.Add(item);
			}
			HashSet<ulong> hashSet = this.friendsXuidsTemp;
			HashSet<ulong> hashSet2 = this.friendXuids;
			this.friendXuids = hashSet;
			this.friendsXuidsTemp = hashSet2;
			HashSet<ulong> hashSet3 = this.friendsXuidsTemp;
			if (hashSet3 != null)
			{
				hashSet3.Clear();
			}
			XblXuidMapper.ResolveUserIdentifiers(this.friendXuids);
		}

		// Token: 0x0600D4CF RID: 54479 RVA: 0x004CF29B File Offset: 0x004CD49B
		public bool IsFriend(ulong xuid)
		{
			if (this.friendsUserGroup == null)
			{
				Log.Error("[FriendsListXbl] could not check IsFriend, friends user group has not been initialized yet.");
				return false;
			}
			if (this.friendXuids == null)
			{
				Log.Error("[FriendsListXbl] could not check IsFriend, friends list has not been retrieved yet");
				return false;
			}
			return this.friendXuids.Contains(xuid);
		}

		// Token: 0x0400A22C RID: 41516
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SocialManagerXbl socialManager;

		// Token: 0x0400A22D RID: 41517
		[PublicizedFrom(EAccessModifier.Private)]
		public XblSocialManagerUserGroupHandle friendsUserGroup;

		// Token: 0x0400A22E RID: 41518
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<ulong> friendXuids;

		// Token: 0x0400A22F RID: 41519
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<ulong> friendsXuidsTemp;
	}
}
