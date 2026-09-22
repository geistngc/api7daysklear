using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;

namespace Platform.XBL
{
	// Token: 0x02001C06 RID: 7174
	public class SocialManagerXbl
	{
		// Token: 0x0600D52C RID: 54572 RVA: 0x004D086C File Offset: 0x004CEA6C
		public SocialManagerXbl(XUserHandle user)
		{
			int hr = SDK.XBL.XblSocialManagerAddLocalUser(user, XblSocialManagerExtraDetailLevel.NoExtraDetail);
			if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				XblHelpers.LogHR(hr, "XblSocialManagerAddLocalUser", false);
				return;
			}
			this.localUser = user;
		}

		// Token: 0x0600D52D RID: 54573 RVA: 0x004D08B0 File Offset: 0x004CEAB0
		public bool TryCreateUserGroup(XblPresenceFilter presenceFilter, XblRelationshipFilter relationshipFilter, SocialManagerXbl.UserGroupMembersChanged callback, out XblSocialManagerUserGroupHandle handle)
		{
			if (this.localUser == null)
			{
				Log.Error("[XBL] Social users lookup not available as the local user was not registered");
				handle = null;
				return false;
			}
			if (callback == null)
			{
				Log.Error("[XBL] TryCreateUserGroup null callback not permitted");
				handle = null;
				return false;
			}
			int hr = SDK.XBL.XblSocialManagerCreateSocialUserGroupFromFilters(this.localUser, presenceFilter, relationshipFilter, out handle);
			if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				Log.Error(string.Format("[XBL] Failed to create user group for {0} {1}", presenceFilter, relationshipFilter));
				XblHelpers.LogHR(hr, "XblSocialManagerCreateSocialUserGroupFromFilters", false);
				handle = null;
				return false;
			}
			this.userGroups.Add(handle, new SocialManagerXbl.UserGroup(handle, callback));
			if (this.updateCoroutine == null)
			{
				this.updateCoroutine = ThreadManager.StartCoroutine(this.UpdateSocialManagerCoroutine());
			}
			return true;
		}

		// Token: 0x0600D52E RID: 54574 RVA: 0x004D0964 File Offset: 0x004CEB64
		public void DestroyUserGroup(XblSocialManagerUserGroupHandle handle)
		{
			this.userGroups.Remove(handle);
			int hr = SDK.XBL.XblSocialManagerDestroySocialUserGroup(handle);
			if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				XblHelpers.LogHR(hr, "XblSocialManagerCreateSocialUserGroupFromFilters", false);
			}
			if (this.userGroups.Count == 0 && this.updateCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.updateCoroutine);
				this.updateCoroutine = null;
			}
		}

		// Token: 0x0600D52F RID: 54575 RVA: 0x004D09C0 File Offset: 0x004CEBC0
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator UpdateSocialManagerCoroutine()
		{
			int hr;
			for (;;)
			{
				XblSocialManagerEvent[] array;
				hr = SDK.XBL.XblSocialManagerDoWork(out array);
				if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
				{
					break;
				}
				if (array != null && array.Length != 0)
				{
					if (SocialManagerXbl.IsUpdateRequired(array))
					{
						foreach (SocialManagerXbl.UserGroup userGroup in this.userGroups.Values)
						{
							userGroup.NotifyChanged();
						}
					}
					foreach (XblSocialManagerEvent xblSocialManagerEvent in array)
					{
						if (xblSocialManagerEvent.EventType == XblSocialManagerEventType.SocialUserGroupLoaded)
						{
							SocialManagerXbl.UserGroup userGroup2;
							if (!this.userGroups.TryGetValue(xblSocialManagerEvent.LoadedGroup, out userGroup2))
							{
								Log.Error("[XBL] LoadedGroup did not match saved handle");
							}
							else
							{
								userGroup2.isLoaded = true;
								userGroup2.NotifyChanged();
							}
						}
					}
				}
				yield return null;
			}
			XblHelpers.LogHR(hr, "XblAchievementsManagerDoWork", false);
			yield break;
			yield break;
		}

		// Token: 0x0600D530 RID: 54576 RVA: 0x004D09D0 File Offset: 0x004CEBD0
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool IsUpdateRequired(XblSocialManagerEvent[] socialEvents)
		{
			for (int i = 0; i < socialEvents.Length; i++)
			{
				switch (socialEvents[i].EventType)
				{
				case XblSocialManagerEventType.UsersAddedToSocialGraph:
				case XblSocialManagerEventType.UsersRemovedFromSocialGraph:
				case XblSocialManagerEventType.SocialRelationshipsChanged:
					return true;
				case XblSocialManagerEventType.PresenceChanged:
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400A263 RID: 41571
		[PublicizedFrom(EAccessModifier.Private)]
		public XUserHandle localUser;

		// Token: 0x0400A264 RID: 41572
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine updateCoroutine;

		// Token: 0x0400A265 RID: 41573
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<XblSocialManagerUserGroupHandle, SocialManagerXbl.UserGroup> userGroups = new Dictionary<XblSocialManagerUserGroupHandle, SocialManagerXbl.UserGroup>();

		// Token: 0x02001C07 RID: 7175
		// (Invoke) Token: 0x0600D532 RID: 54578
		public delegate void UserGroupMembersChanged(ulong[] members);

		// Token: 0x02001C08 RID: 7176
		[PublicizedFrom(EAccessModifier.Private)]
		public class UserGroup
		{
			// Token: 0x0600D535 RID: 54581 RVA: 0x004D0A27 File Offset: 0x004CEC27
			public UserGroup(XblSocialManagerUserGroupHandle handle, SocialManagerXbl.UserGroupMembersChanged membersChangedCallback)
			{
				this.handle = handle;
				this.membersChangedCallback = membersChangedCallback;
			}

			// Token: 0x0600D536 RID: 54582 RVA: 0x004D0A40 File Offset: 0x004CEC40
			public void NotifyChanged()
			{
				if (!this.isLoaded)
				{
					return;
				}
				ulong[] array = this.membersCache;
				int hr = SDK.XBL.XblSocialManagerUserGroupGetUsersTrackedByGroup(this.handle, out this.membersCache);
				if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
				{
					XblHelpers.LogHR(hr, "XblSocialManagerUserGroupGetUsersTrackedByGroup", false);
					this.membersCache = null;
					return;
				}
				Array.Sort<ulong>(this.membersCache);
				if (array != null && array.Length == this.membersCache.Length && this.membersCache.SequenceEqual(array))
				{
					Log.Out("[XBL] social manager skipping user group update as member list didn't change");
					return;
				}
				ulong[] array2 = new ulong[this.membersCache.Length];
				Array.Copy(this.membersCache, array2, this.membersCache.Length);
				this.membersChangedCallback(array2);
			}

			// Token: 0x0400A266 RID: 41574
			public readonly XblSocialManagerUserGroupHandle handle;

			// Token: 0x0400A267 RID: 41575
			public readonly SocialManagerXbl.UserGroupMembersChanged membersChangedCallback;

			// Token: 0x0400A268 RID: 41576
			public bool isLoaded;

			// Token: 0x0400A269 RID: 41577
			[PublicizedFrom(EAccessModifier.Private)]
			public ulong[] membersCache;
		}
	}
}
