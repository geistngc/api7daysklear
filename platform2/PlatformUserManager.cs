using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001BD8 RID: 7128
	public static class PlatformUserManager
	{
		// Token: 0x0600D421 RID: 54305 RVA: 0x004CCA05 File Offset: 0x004CAC05
		[Conditional("PLATFORM_USER_MANAGER_DEBUG")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string message)
		{
			Log.Out("[PlatformUserManager] " + message);
		}

		// Token: 0x0600D422 RID: 54306 RVA: 0x004CCA05 File Offset: 0x004CAC05
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string message)
		{
			Log.Out("[PlatformUserManager] " + message);
		}

		// Token: 0x0600D423 RID: 54307 RVA: 0x004CCA17 File Offset: 0x004CAC17
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogWarning(string message)
		{
			Log.Warning("[PlatformUserManager] " + message);
		}

		// Token: 0x0600D424 RID: 54308 RVA: 0x004CCA29 File Offset: 0x004CAC29
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string message)
		{
			Log.Error("[PlatformUserManager] " + message);
		}

		// Token: 0x14000128 RID: 296
		// (add) Token: 0x0600D425 RID: 54309 RVA: 0x004CCA3C File Offset: 0x004CAC3C
		// (remove) Token: 0x0600D426 RID: 54310 RVA: 0x004CCA70 File Offset: 0x004CAC70
		public static event PlatformUserBlockedStateChangedHandler BlockedStateChanged;

		// Token: 0x14000129 RID: 297
		// (add) Token: 0x0600D427 RID: 54311 RVA: 0x004CCAA4 File Offset: 0x004CACA4
		// (remove) Token: 0x0600D428 RID: 54312 RVA: 0x004CCAD8 File Offset: 0x004CACD8
		public static event PlatformUserDetailsUpdatedHandler DetailsUpdated;

		// Token: 0x0600D429 RID: 54313 RVA: 0x004CCB0C File Offset: 0x004CAD0C
		public static void Init()
		{
			PlatformUserManager.s_primaryIdToPlatform = new Dictionary<PlatformUserIdentifierAbs, PlatformUserManager.PlatformUserData>();
			PlatformUserManager.s_primaryIdToPlatformLock = new ReaderWriterLockSlim();
			PlatformUserManager.s_nativeIdToPrimaryIds = new OneToManyDictionary<PlatformUserIdentifierAbs, PlatformUserIdentifierAbs>();
			PlatformUserManager.s_nativeIdToPrimaryIdsLock = new ReaderWriterLockSlim();
			PlatformUserManager.s_nativeUserIdsSeen = new HashSet<PlatformUserIdentifierAbs>();
			PlatformUserManager.s_nativeUserIdsSeenLock = new ReaderWriterLockSlim();
			PlatformUserManager.s_lastPermissions = EUserPerms.All;
			PlatformUserManager.s_persistentPlayerListLast = null;
			PlatformUserManager.s_persistentIdsTemp = new HashSet<PlatformUserIdentifierAbs>();
			PlatformUserManager.s_persistentIdsLast = new HashSet<PlatformUserIdentifierAbs>();
			PlatformUserManager.s_blockedUsersToUpdate = new HashSet<PlatformUserManager.PlatformUserData>();
			PlatformUserManager.s_blockedUsersToUpdateLock = new ReaderWriterLockSlim();
			PlatformUserManager.s_blockedDataCurrentlyUpdating = new List<PlatformUserManager.PlatformUserBlockedResults>();
			PlatformUserManager.s_blockedDataCurrentlyUpdatingReadOnly = new ReadOnlyListWrapper<PlatformUserManager.PlatformUserBlockedResults, IPlatformUserBlockedResults>(PlatformUserManager.s_blockedDataCurrentlyUpdating);
			PlatformManager.MultiPlatform.User.UserBlocksChanged += PlatformUserManager.OnPlatformUserBlocksChanged;
			PlatformUserManager.s_userDetailsToUpdate = new HashSet<PlatformUserManager.PlatformUserData>();
			PlatformUserManager.s_userDetailsCurrentlyUpdating = new List<PlatformUserManager.PlatformUserDetailsResult>();
			PlatformUserManager.s_userDetailsToUpdateLock = new ReaderWriterLockSlim();
			PlatformUserManager.s_enabled = true;
		}

		// Token: 0x0600D42A RID: 54314 RVA: 0x004CCBE4 File Offset: 0x004CADE4
		public static void Destroy()
		{
			PlatformUserManager.s_enabled = false;
			PlatformUserManager.s_userDetailsToUpdateLock = null;
			PlatformUserManager.s_userDetailsCurrentlyUpdating = null;
			PlatformUserManager.s_userDetailsToUpdate = null;
			PlatformManager.MultiPlatform.User.UserBlocksChanged -= PlatformUserManager.OnPlatformUserBlocksChanged;
			PlatformUserManager.s_blockedDataCurrentlyUpdatingReadOnly = null;
			PlatformUserManager.s_blockedDataCurrentlyUpdating = null;
			ReaderWriterLockSlim readerWriterLockSlim = PlatformUserManager.s_blockedUsersToUpdateLock;
			if (readerWriterLockSlim != null)
			{
				readerWriterLockSlim.Dispose();
			}
			PlatformUserManager.s_blockedUsersToUpdateLock = null;
			PlatformUserManager.s_blockedUsersToUpdate = null;
			PlatformUserManager.s_persistentIdsLast = null;
			PlatformUserManager.s_persistentIdsTemp = null;
			PlatformUserManager.s_persistentPlayerListLast = null;
			PlatformUserManager.s_lastPermissions = (EUserPerms)0;
			ReaderWriterLockSlim readerWriterLockSlim2 = PlatformUserManager.s_nativeUserIdsSeenLock;
			if (readerWriterLockSlim2 != null)
			{
				readerWriterLockSlim2.Dispose();
			}
			PlatformUserManager.s_nativeUserIdsSeenLock = null;
			PlatformUserManager.s_nativeUserIdsSeen = null;
			ReaderWriterLockSlim readerWriterLockSlim3 = PlatformUserManager.s_nativeIdToPrimaryIdsLock;
			if (readerWriterLockSlim3 != null)
			{
				readerWriterLockSlim3.Dispose();
			}
			PlatformUserManager.s_nativeIdToPrimaryIdsLock = null;
			PlatformUserManager.s_nativeIdToPrimaryIds = null;
			ReaderWriterLockSlim readerWriterLockSlim4 = PlatformUserManager.s_primaryIdToPlatformLock;
			if (readerWriterLockSlim4 != null)
			{
				readerWriterLockSlim4.Dispose();
			}
			PlatformUserManager.s_primaryIdToPlatformLock = null;
			PlatformUserManager.s_primaryIdToPlatform = null;
		}

		// Token: 0x0600D42B RID: 54315 RVA: 0x004CCCB8 File Offset: 0x004CAEB8
		public static void Update()
		{
			if (!PlatformUserManager.s_enabled)
			{
				return;
			}
			try
			{
				PlatformUserManager.UpdatePermissions();
				PlatformUserManager.UpdatePersistentIds();
				PlatformUserManager.UpdateUserDetails();
				PlatformUserManager.UpdateBlockedStates();
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
		}

		// Token: 0x0600D42C RID: 54316 RVA: 0x004CCCFC File Offset: 0x004CAEFC
		public static IPlatformUserData GetOrCreate(PlatformUserIdentifierAbs primaryId)
		{
			if (primaryId == null)
			{
				return null;
			}
			PlatformUserManager.PlatformUserData platformUserData;
			using (PlatformUserManager.s_primaryIdToPlatformLock.UpgradableReadLockScope())
			{
				PlatformUserManager.PlatformUserData result;
				if (PlatformUserManager.s_primaryIdToPlatform.TryGetValue(primaryId, out result))
				{
					return result;
				}
				using (PlatformUserManager.s_primaryIdToPlatformLock.WriteLockScope())
				{
					platformUserData = new PlatformUserManager.PlatformUserData(primaryId);
					PlatformUserManager.s_primaryIdToPlatform.Add(primaryId, platformUserData);
				}
			}
			PlatformUserManager.OnUserAdded(primaryId, true);
			return platformUserData;
		}

		// Token: 0x0600D42D RID: 54317 RVA: 0x004CCD90 File Offset: 0x004CAF90
		public static bool TryGetNativePlatform(PlatformUserIdentifierAbs primaryId, out EPlatformIdentifier platform)
		{
			if (primaryId == null)
			{
				platform = EPlatformIdentifier.None;
				return false;
			}
			bool result;
			using (PlatformUserManager.s_primaryIdToPlatformLock.ReadLockScope())
			{
				PlatformUserManager.PlatformUserData platformUserData;
				if (!PlatformUserManager.s_primaryIdToPlatform.TryGetValue(primaryId, out platformUserData))
				{
					platform = EPlatformIdentifier.None;
					result = false;
				}
				else
				{
					PlatformUserIdentifierAbs nativeId = platformUserData.NativeId;
					if (nativeId == null)
					{
						platform = EPlatformIdentifier.None;
						result = false;
					}
					else
					{
						platform = nativeId.PlatformIdentifier;
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600D42E RID: 54318 RVA: 0x004CCE04 File Offset: 0x004CB004
		public static int TryGetByNative(PlatformUserIdentifierAbs nativeId, Span<PlatformUserIdentifierAbs> primaryIds)
		{
			if (nativeId == null)
			{
				return 0;
			}
			int num;
			using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.ReadLockScope())
			{
				num = PlatformUserManager.s_nativeIdToPrimaryIds.TryGetByKey(nativeId, primaryIds);
			}
			if (num >= 3)
			{
				PlatformUserManager.LogWarning(string.Format("Expected number of values returned {0} to be less than the limit of PrimaryIds per NativeId ({1}).", num, 3));
			}
			return num;
		}

		// Token: 0x0600D42F RID: 54319 RVA: 0x004CCE70 File Offset: 0x004CB070
		public static IEnumerator ResolveUserBlockedCoroutine(IPlatformUserData data)
		{
			for (;;)
			{
				using (PlatformUserManager.s_blockedUsersToUpdateLock.ReadLockScope())
				{
					if (!PlatformUserManager.s_blockedUsersToUpdate.Contains((PlatformUserManager.PlatformUserData)data))
					{
						yield break;
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600D430 RID: 54320 RVA: 0x004CCE7F File Offset: 0x004CB07F
		public static IEnumerator ResolveUserDetailsCoroutine(IPlatformUserData data)
		{
			for (;;)
			{
				using (PlatformUserManager.s_userDetailsToUpdateLock.ReadLockScope())
				{
					if (!PlatformUserManager.s_userDetailsToUpdate.Contains((PlatformUserManager.PlatformUserData)data))
					{
						yield break;
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600D431 RID: 54321 RVA: 0x004CCE90 File Offset: 0x004CB090
		public static bool AreUsersPendingResolve(IReadOnlyList<IPlatformUserData> users)
		{
			if (users == null || users.Count == 0)
			{
				return false;
			}
			using (PlatformUserManager.s_blockedUsersToUpdateLock.ReadLockScope())
			{
				for (int i = 0; i < users.Count; i++)
				{
					if (PlatformUserManager.s_blockedUsersToUpdate.Contains((PlatformUserManager.PlatformUserData)users[i]))
					{
						return true;
					}
				}
			}
			using (PlatformUserManager.s_userDetailsToUpdateLock.ReadLockScope())
			{
				for (int j = 0; j < users.Count; j++)
				{
					if (PlatformUserManager.s_userDetailsToUpdate.Contains((PlatformUserManager.PlatformUserData)users[j]))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600D432 RID: 54322 RVA: 0x004CCF5C File Offset: 0x004CB15C
		public static IEnumerator ResolveUserBlocksCoroutine(IReadOnlyList<IPlatformUserData> users)
		{
			if (users == null || users.Count == 0)
			{
				yield break;
			}
			foreach (IPlatformUserData data in users)
			{
				yield return PlatformUserManager.ResolveUserBlockedCoroutine(data);
			}
			IEnumerator<IPlatformUserData> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600D433 RID: 54323 RVA: 0x004CCF6B File Offset: 0x004CB16B
		public static IEnumerator ResolveUsersDetailsCoroutine(IReadOnlyList<IPlatformUserData> users)
		{
			if (users == null || users.Count == 0)
			{
				yield break;
			}
			foreach (IPlatformUserData data in users)
			{
				yield return PlatformUserManager.ResolveUserDetailsCoroutine(data);
			}
			IEnumerator<IPlatformUserData> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600D434 RID: 54324 RVA: 0x004CCF7C File Offset: 0x004CB17C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void OnUserAdded(PlatformUserIdentifierAbs userId, bool isPrimary)
		{
			if (!ThreadManager.IsMainThread())
			{
				ThreadManager.AddSingleTaskMainThread("PlatformUserManager.OnUserAdded", delegate(object _)
				{
					PlatformUserManager.OnUserAdded(userId, isPrimary);
				}, null);
				return;
			}
			PlatformManager.MultiPlatform.UserAdded(userId, isPrimary);
		}

		// Token: 0x0600D435 RID: 54325 RVA: 0x004CCFD4 File Offset: 0x004CB1D4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void OnBlockedStateChanged(PlatformUserManager.PlatformUserData userData, EBlockType type, EUserBlockState nextBlockState)
		{
			if (!ThreadManager.IsMainThread())
			{
				ThreadManager.AddSingleTaskMainThread("PlatformUserManager.OnBlockedStateChanged", delegate(object _)
				{
					PlatformUserBlockedStateChangedHandler blockedStateChanged2 = PlatformUserManager.BlockedStateChanged;
					if (blockedStateChanged2 == null)
					{
						return;
					}
					blockedStateChanged2(userData, type, nextBlockState);
				}, null);
				return;
			}
			PlatformUserBlockedStateChangedHandler blockedStateChanged = PlatformUserManager.BlockedStateChanged;
			if (blockedStateChanged == null)
			{
				return;
			}
			blockedStateChanged(userData, type, nextBlockState);
		}

		// Token: 0x0600D436 RID: 54326 RVA: 0x004CD03C File Offset: 0x004CB23C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void UpdatePermissions()
		{
			if (Time.frameCount % 60 != 0)
			{
				return;
			}
			EUserPerms permissions = PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All);
			if ((PlatformUserManager.s_lastPermissions ^ permissions).HasCommunication())
			{
				PlatformUserManager.MarkBlockedStateChangedAll();
			}
			PlatformUserManager.s_lastPermissions = permissions;
		}

		// Token: 0x0600D437 RID: 54327 RVA: 0x004CD078 File Offset: 0x004CB278
		[PublicizedFrom(EAccessModifier.Private)]
		public static void UpdatePersistentIds()
		{
			if (Time.frameCount % 300 != 0)
			{
				return;
			}
			PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
			if (persistentPlayers == null)
			{
				return;
			}
			if (PlatformUserManager.s_persistentPlayerListLast != persistentPlayers)
			{
				PlatformUserManager.s_persistentIdsLast.Clear();
				PlatformUserManager.s_persistentPlayerListLast = persistentPlayers;
			}
			ICollection<PlatformUserIdentifierAbs> players = persistentPlayers.Players.Keys;
			PlatformUserManager.s_persistentIdsLast.RemoveWhere((PlatformUserIdentifierAbs last) => !players.Contains(last));
			PlatformUserManager.s_persistentIdsTemp.Clear();
			foreach (PlatformUserIdentifierAbs item in players)
			{
				if (!PlatformUserManager.s_persistentIdsLast.Contains(item))
				{
					PlatformUserManager.s_persistentIdsLast.Add(item);
					PlatformUserManager.s_persistentIdsTemp.Add(item);
				}
			}
			foreach (PlatformUserIdentifierAbs primaryId in PlatformUserManager.s_persistentIdsTemp)
			{
				IPlatformUserData orCreate = PlatformUserManager.GetOrCreate(primaryId);
				foreach (IPlatformUserBlockedData platformUserBlockedData in orCreate.Blocked.Values)
				{
					platformUserBlockedData.Locally = false;
				}
				orCreate.MarkBlockedStateChanged();
			}
		}

		// Token: 0x0600D438 RID: 54328 RVA: 0x004CD1E0 File Offset: 0x004CB3E0
		[PublicizedFrom(EAccessModifier.Private)]
		public static void UpdateUserDetails()
		{
			if (PlatformUserManager.s_userDetailsCurrentlyUpdating.Count > 0)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			using (PlatformUserManager.s_userDetailsToUpdateLock.UpgradableReadLockScope())
			{
				if (PlatformUserManager.s_userDetailsToUpdate.Count <= 0)
				{
					return;
				}
				foreach (PlatformUserManager.PlatformUserData platformUserData in PlatformUserManager.s_userDetailsToUpdate)
				{
					if (platformUserData.NextDetailRetryTime <= realtimeSinceStartup)
					{
						PlatformUserManager.s_userDetailsCurrentlyUpdating.Add(new PlatformUserManager.PlatformUserDetailsResult(platformUserData));
					}
				}
			}
			if (PlatformUserManager.s_userDetailsCurrentlyUpdating.Count > 0)
			{
				ThreadManager.StartCoroutine(PlatformUserManager.ResolveUserDetailsCoroutine());
			}
		}

		// Token: 0x0600D439 RID: 54329 RVA: 0x004CD2A4 File Offset: 0x004CB4A4
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator ResolveUserDetailsCoroutine()
		{
			try
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform != null) ? crossplatformPlatform.UserDetailsService : null) != null)
				{
					List<UserDetailsRequest> list = null;
					List<int> list2 = null;
					for (int i = 0; i < PlatformUserManager.s_userDetailsCurrentlyUpdating.Count; i++)
					{
						PlatformUserManager.PlatformUserDetailsResult platformUserDetailsResult = PlatformUserManager.s_userDetailsCurrentlyUpdating[i];
						if (platformUserDetailsResult.UserData.NativeId != null)
						{
							if (list == null)
							{
								list = new List<UserDetailsRequest>();
							}
							if (list2 == null)
							{
								list2 = new List<int>();
							}
							list.Add(new UserDetailsRequest(platformUserDetailsResult.UserData.PrimaryId, platformUserDetailsResult.UserData.NativeId.PlatformIdentifier));
							list2.Add(i);
						}
					}
					if (list != null)
					{
						yield return PlatformUserManager.<ResolveUserDetailsCoroutine>g__ResolveUserDetails|50_0(PlatformManager.CrossplatformPlatform.UserDetailsService, list, list2, PlatformUserManager.s_userDetailsCurrentlyUpdating);
					}
				}
				if (PlatformManager.NativePlatform.UserDetailsService != null)
				{
					List<UserDetailsRequest> list3 = null;
					List<int> list4 = null;
					for (int j = 0; j < PlatformUserManager.s_userDetailsCurrentlyUpdating.Count; j++)
					{
						PlatformUserManager.PlatformUserDetailsResult platformUserDetailsResult2 = PlatformUserManager.s_userDetailsCurrentlyUpdating[j];
						if (platformUserDetailsResult2.UserData.NativeId != null)
						{
							if (list3 == null)
							{
								list3 = new List<UserDetailsRequest>();
							}
							if (list4 == null)
							{
								list4 = new List<int>();
							}
							list3.Add(new UserDetailsRequest(platformUserDetailsResult2.UserData.NativeId));
							list4.Add(j);
						}
					}
					if (list3 != null)
					{
						yield return PlatformUserManager.<ResolveUserDetailsCoroutine>g__ResolveUserDetails|50_0(PlatformManager.NativePlatform.UserDetailsService, list3, list4, PlatformUserManager.s_userDetailsCurrentlyUpdating);
					}
				}
				foreach (PlatformUserManager.PlatformUserDetailsResult platformUserDetailsResult3 in PlatformUserManager.s_userDetailsCurrentlyUpdating)
				{
					if (!string.IsNullOrEmpty(platformUserDetailsResult3.Name))
					{
						platformUserDetailsResult3.UserData.Name = platformUserDetailsResult3.Name;
						PlatformUserDetailsUpdatedHandler detailsUpdated = PlatformUserManager.DetailsUpdated;
						if (detailsUpdated != null)
						{
							detailsUpdated(platformUserDetailsResult3.UserData, platformUserDetailsResult3.Name);
						}
					}
				}
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				using (PlatformUserManager.s_userDetailsToUpdateLock.WriteLockScope())
				{
					foreach (PlatformUserManager.PlatformUserDetailsResult platformUserDetailsResult4 in PlatformUserManager.s_userDetailsCurrentlyUpdating)
					{
						PlatformUserManager.PlatformUserData userData = platformUserDetailsResult4.UserData;
						if (!string.IsNullOrEmpty(platformUserDetailsResult4.Name))
						{
							userData.DetailAttempts = 0;
							userData.NextDetailRetryTime = 0f;
							PlatformUserManager.s_userDetailsToUpdate.Remove(userData);
						}
						else
						{
							userData.DetailAttempts++;
							if (userData.DetailAttempts >= 5)
							{
								PlatformUserManager.LogWarning(string.Format("[PLATFORM] User details lookup gave up after {0} attempts.", userData.DetailAttempts));
								userData.DetailAttempts = 0;
								userData.NextDetailRetryTime = 0f;
								PlatformUserManager.s_userDetailsToUpdate.Remove(userData);
							}
							else
							{
								float num = 0.5f * (float)(1 << userData.DetailAttempts - 1);
								if (num > 30f)
								{
									num = 30f;
								}
								userData.NextDetailRetryTime = realtimeSinceStartup + num;
							}
						}
					}
				}
			}
			finally
			{
				PlatformUserManager.s_userDetailsCurrentlyUpdating.Clear();
			}
			yield break;
			yield break;
		}

		// Token: 0x0600D43A RID: 54330 RVA: 0x004CD2AC File Offset: 0x004CB4AC
		[PublicizedFrom(EAccessModifier.Private)]
		public static void UpdateBlockedStates()
		{
			if (PlatformUserManager.s_blockedDataCurrentlyUpdating.Count > 0 || PlatformUserManager.s_blockedUsersToUpdate.Count <= 0)
			{
				return;
			}
			PlatformUserManager.PlatformUserData item;
			bool flag;
			using (PlatformUserManager.s_primaryIdToPlatformLock.ReadLockScope())
			{
				flag = PlatformUserManager.s_primaryIdToPlatform.TryGetValue(PlatformManager.MultiPlatform.User.PlatformUserId, out item);
			}
			using (PlatformUserManager.s_blockedUsersToUpdateLock.UpgradableReadLockScope())
			{
				if (flag)
				{
					using (PlatformUserManager.s_blockedUsersToUpdateLock.WriteLockScope())
					{
						PlatformUserManager.s_blockedUsersToUpdate.Remove(item);
					}
				}
				foreach (PlatformUserManager.PlatformUserData userData in PlatformUserManager.s_blockedUsersToUpdate)
				{
					PlatformUserManager.s_blockedDataCurrentlyUpdating.Add(new PlatformUserManager.PlatformUserBlockedResults(userData));
				}
			}
			if (PlatformUserManager.s_blockedDataCurrentlyUpdating.Count > 0)
			{
				ThreadManager.StartCoroutine(PlatformUserManager.UpdateBlockedStatesCoroutine());
			}
		}

		// Token: 0x0600D43B RID: 54331 RVA: 0x004CD3DC File Offset: 0x004CB5DC
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator UpdateBlockedStatesCoroutine()
		{
			try
			{
				yield return PlatformManager.MultiPlatform.User.ResolveUserBlocks(PlatformUserManager.s_blockedDataCurrentlyUpdatingReadOnly);
				if (BlockedPlayerList.Instance != null)
				{
					yield return PlatformUserManager.ResolveUserBlocksFromBlockList(PlatformUserManager.s_blockedDataCurrentlyUpdatingReadOnly);
				}
				foreach (PlatformUserManager.PlatformUserBlockedResults platformUserBlockedResults in PlatformUserManager.s_blockedDataCurrentlyUpdating)
				{
					if (!platformUserBlockedResults.HasErrored)
					{
						foreach (EBlockType key in EnumUtils.Values<EBlockType>())
						{
							platformUserBlockedResults.User.Blocked[key].RefreshBlockedState(platformUserBlockedResults.IsBlocked[key]);
						}
					}
				}
				using (PlatformUserManager.s_blockedUsersToUpdateLock.WriteLockScope())
				{
					foreach (PlatformUserManager.PlatformUserBlockedResults platformUserBlockedResults2 in PlatformUserManager.s_blockedDataCurrentlyUpdating)
					{
						PlatformUserManager.s_blockedUsersToUpdate.Remove(platformUserBlockedResults2.User);
					}
				}
			}
			finally
			{
				PlatformUserManager.s_blockedDataCurrentlyUpdating.Clear();
			}
			yield break;
			yield break;
		}

		// Token: 0x0600D43C RID: 54332 RVA: 0x004CD3E4 File Offset: 0x004CB5E4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void MarkBlockedStateChangedAll()
		{
			using (PlatformUserManager.s_primaryIdToPlatformLock.ReadLockScope())
			{
				foreach (PlatformUserManager.PlatformUserData platformUserData in PlatformUserManager.s_primaryIdToPlatform.Values)
				{
					platformUserData.MarkBlockedStateChanged();
				}
			}
		}

		// Token: 0x0600D43D RID: 54333 RVA: 0x004CD460 File Offset: 0x004CB660
		[PublicizedFrom(EAccessModifier.Private)]
		public static void OnPlatformUserBlocksChanged(IReadOnlyCollection<PlatformUserIdentifierAbs> userIds)
		{
			if (userIds == null)
			{
				PlatformUserManager.MarkBlockedStateChangedAll();
				return;
			}
			using (PlatformUserManager.s_primaryIdToPlatformLock.ReadLockScope())
			{
				foreach (PlatformUserIdentifierAbs key in userIds)
				{
					PlatformUserManager.PlatformUserData platformUserData;
					if (PlatformUserManager.s_primaryIdToPlatform.TryGetValue(key, out platformUserData))
					{
						platformUserData.MarkBlockedStateChanged();
					}
				}
			}
			using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.ReadLockScope())
			{
				foreach (PlatformUserIdentifierAbs key2 in userIds)
				{
					IReadOnlyCollection<PlatformUserIdentifierAbs> readOnlyCollection;
					if (PlatformUserManager.s_nativeIdToPrimaryIds.TryGetByKey(key2, out readOnlyCollection))
					{
						using (PlatformUserManager.s_primaryIdToPlatformLock.ReadLockScope())
						{
							foreach (PlatformUserIdentifierAbs key3 in readOnlyCollection)
							{
								PlatformUserManager.s_primaryIdToPlatform[key3].MarkBlockedStateChanged();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600D43E RID: 54334 RVA: 0x004CD5B8 File Offset: 0x004CB7B8
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool CanCheckUserDetails()
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			return ((crossplatformPlatform != null) ? crossplatformPlatform.UserDetailsService : null) != null || PlatformManager.NativePlatform.UserDetailsService != null;
		}

		// Token: 0x0600D43F RID: 54335 RVA: 0x004CD5DC File Offset: 0x004CB7DC
		public static IEnumerator ResolveUserBlocksFromBlockList(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			if (BlockedPlayerList.Instance == null)
			{
				yield break;
			}
			while (BlockedPlayerList.Instance.PendingResolve())
			{
				yield return null;
			}
			using (IEnumerator<BlockedPlayerList.ListEntry> enumerator = BlockedPlayerList.Instance.GetEntriesOrdered(true, false).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BlockedPlayerList.ListEntry listEntry = enumerator.Current;
					PlatformUserIdentifierAbs primaryId = listEntry.PlayerData.PrimaryId;
					foreach (IPlatformUserBlockedResults platformUserBlockedResults in _results)
					{
						if (platformUserBlockedResults.User.PrimaryId.Equals(primaryId))
						{
							platformUserBlockedResults.BlockAll();
							break;
						}
					}
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x0600D440 RID: 54336 RVA: 0x004CD5EB File Offset: 0x004CB7EB
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static IEnumerator <ResolveUserDetailsCoroutine>g__ResolveUserDetails|50_0(IUserDetailsService service, IReadOnlyList<UserDetailsRequest> requests, IReadOnlyList<int> resultsIndices, List<PlatformUserManager.PlatformUserDetailsResult> results)
		{
			PlatformUserManager.<>c__DisplayClass50_0 CS$<>8__locals1 = new PlatformUserManager.<>c__DisplayClass50_0();
			CS$<>8__locals1.resultsIndices = resultsIndices;
			CS$<>8__locals1.results = results;
			CS$<>8__locals1.requests = requests;
			CS$<>8__locals1.inProgress = true;
			service.RequestUserDetailsUpdate(CS$<>8__locals1.requests, new UserDetailsRequestCompleteHandler(CS$<>8__locals1.<ResolveUserDetailsCoroutine>g__OnComplete|1));
			while (CS$<>8__locals1.inProgress)
			{
				yield return true;
			}
			yield break;
		}

		// Token: 0x0400A1B6 RID: 41398
		public const int PrimaryIdsPerNativeIdLimit = 3;

		// Token: 0x0400A1B7 RID: 41399
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool s_enabled;

		// Token: 0x0400A1B8 RID: 41400
		[PublicizedFrom(EAccessModifier.Private)]
		public static Dictionary<PlatformUserIdentifierAbs, PlatformUserManager.PlatformUserData> s_primaryIdToPlatform;

		// Token: 0x0400A1B9 RID: 41401
		[PublicizedFrom(EAccessModifier.Private)]
		public static ReaderWriterLockSlim s_primaryIdToPlatformLock;

		// Token: 0x0400A1BA RID: 41402
		[PublicizedFrom(EAccessModifier.Private)]
		public static OneToManyDictionary<PlatformUserIdentifierAbs, PlatformUserIdentifierAbs> s_nativeIdToPrimaryIds;

		// Token: 0x0400A1BB RID: 41403
		[PublicizedFrom(EAccessModifier.Private)]
		public static ReaderWriterLockSlim s_nativeIdToPrimaryIdsLock;

		// Token: 0x0400A1BC RID: 41404
		[PublicizedFrom(EAccessModifier.Private)]
		public static HashSet<PlatformUserIdentifierAbs> s_nativeUserIdsSeen;

		// Token: 0x0400A1BD RID: 41405
		[PublicizedFrom(EAccessModifier.Private)]
		public static ReaderWriterLockSlim s_nativeUserIdsSeenLock;

		// Token: 0x0400A1BE RID: 41406
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PermissionFrameFrequency = 60;

		// Token: 0x0400A1BF RID: 41407
		[PublicizedFrom(EAccessModifier.Private)]
		public static EUserPerms s_lastPermissions;

		// Token: 0x0400A1C0 RID: 41408
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PersistentFrameFrequency = 300;

		// Token: 0x0400A1C1 RID: 41409
		[PublicizedFrom(EAccessModifier.Private)]
		public static PersistentPlayerList s_persistentPlayerListLast;

		// Token: 0x0400A1C2 RID: 41410
		[PublicizedFrom(EAccessModifier.Private)]
		public static HashSet<PlatformUserIdentifierAbs> s_persistentIdsTemp;

		// Token: 0x0400A1C3 RID: 41411
		[PublicizedFrom(EAccessModifier.Private)]
		public static HashSet<PlatformUserIdentifierAbs> s_persistentIdsLast;

		// Token: 0x0400A1C4 RID: 41412
		[PublicizedFrom(EAccessModifier.Private)]
		public static HashSet<PlatformUserManager.PlatformUserData> s_blockedUsersToUpdate;

		// Token: 0x0400A1C5 RID: 41413
		[PublicizedFrom(EAccessModifier.Private)]
		public static ReaderWriterLockSlim s_blockedUsersToUpdateLock;

		// Token: 0x0400A1C6 RID: 41414
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<PlatformUserManager.PlatformUserBlockedResults> s_blockedDataCurrentlyUpdating;

		// Token: 0x0400A1C7 RID: 41415
		[PublicizedFrom(EAccessModifier.Private)]
		public static IReadOnlyList<IPlatformUserBlockedResults> s_blockedDataCurrentlyUpdatingReadOnly;

		// Token: 0x0400A1C9 RID: 41417
		[PublicizedFrom(EAccessModifier.Private)]
		public static HashSet<PlatformUserManager.PlatformUserData> s_userDetailsToUpdate;

		// Token: 0x0400A1CA RID: 41418
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<PlatformUserManager.PlatformUserDetailsResult> s_userDetailsCurrentlyUpdating;

		// Token: 0x0400A1CB RID: 41419
		[PublicizedFrom(EAccessModifier.Private)]
		public static ReaderWriterLockSlim s_userDetailsToUpdateLock;

		// Token: 0x0400A1CC RID: 41420
		[PublicizedFrom(EAccessModifier.Private)]
		public const int UserDetailsMaxAttempts = 5;

		// Token: 0x0400A1CD RID: 41421
		[PublicizedFrom(EAccessModifier.Private)]
		public const float UserDetailsRetryBaseDelay = 0.5f;

		// Token: 0x0400A1CE RID: 41422
		[PublicizedFrom(EAccessModifier.Private)]
		public const float UserDetailsRetryMaxDelay = 30f;

		// Token: 0x02001BD9 RID: 7129
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class PlatformUserData : IPlatformUserData, IPlatformUser
		{
			// Token: 0x0600D441 RID: 54337 RVA: 0x004CD610 File Offset: 0x004CB810
			public PlatformUserData(PlatformUserIdentifierAbs primaryId)
			{
				this.PrimaryId = primaryId;
				this.m_userBlockedStates = new EnumDictionary<EBlockType, PlatformUserManager.PlatformUserBlockedData>();
				this.m_userBlockedStatesReadOnly = new ReadOnlyDictionaryWrapper<EBlockType, PlatformUserManager.PlatformUserBlockedData, IPlatformUserBlockedData>(this.m_userBlockedStates);
				foreach (EBlockType eblockType in EnumUtils.Values<EBlockType>())
				{
					this.m_userBlockedStates[eblockType] = new PlatformUserManager.PlatformUserBlockedData(this, eblockType);
				}
				this.RequestUserDetailsUpdate();
			}

			// Token: 0x0600D442 RID: 54338 RVA: 0x004CD698 File Offset: 0x004CB898
			public override string ToString()
			{
				string format = "{0}[PrimaryId={1}, NativeId={2}, Name={3}, {4}]";
				object[] array = new object[5];
				array[0] = "PlatformUserData";
				array[1] = this.PrimaryId;
				array[2] = this.NativeId;
				array[3] = this.Name;
				array[4] = string.Join(", ", from kv in this.Blocked
				select string.Format("Blocked[{0}]={1}", kv.Key, kv.Value));
				return string.Format(format, array);
			}

			// Token: 0x17001A40 RID: 6720
			// (get) Token: 0x0600D443 RID: 54339 RVA: 0x004CD70F File Offset: 0x004CB90F
			public PlatformUserIdentifierAbs PrimaryId { get; }

			// Token: 0x17001A41 RID: 6721
			// (get) Token: 0x0600D444 RID: 54340 RVA: 0x004CD718 File Offset: 0x004CB918
			// (set) Token: 0x0600D445 RID: 54341 RVA: 0x004CD76C File Offset: 0x004CB96C
			public PlatformUserIdentifierAbs NativeId
			{
				get
				{
					PlatformUserIdentifierAbs result;
					using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.ReadLockScope())
					{
						PlatformUserIdentifierAbs platformUserIdentifierAbs;
						result = (PlatformUserManager.s_nativeIdToPrimaryIds.TryGetByValue(this.PrimaryId, out platformUserIdentifierAbs) ? platformUserIdentifierAbs : null);
					}
					return result;
				}
				set
				{
					if (value == null)
					{
						return;
					}
					using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.UpgradableReadLockScope())
					{
						PlatformUserIdentifierAbs platformUserIdentifierAbs;
						if (PlatformUserManager.s_nativeIdToPrimaryIds.TryGetByValue(this.PrimaryId, out platformUserIdentifierAbs))
						{
							if (platformUserIdentifierAbs.Equals(value))
							{
								return;
							}
							using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.WriteLockScope())
							{
								PlatformUserManager.s_nativeIdToPrimaryIds.RemoveByValue(this.PrimaryId);
								PlatformUserManager.s_nativeIdToPrimaryIds.Add(value, this.PrimaryId);
								PlatformUserManager.LogError(string.Format("Primary ID '{0}' was be remapped from Native ID '{1}' to Native ID '{2}'.", this.PrimaryId, platformUserIdentifierAbs, value));
								goto IL_BF;
							}
						}
						using (PlatformUserManager.s_nativeIdToPrimaryIdsLock.WriteLockScope())
						{
							PlatformUserManager.s_nativeIdToPrimaryIds.Add(value, this.PrimaryId);
						}
					}
					IL_BF:
					bool flag;
					using (PlatformUserManager.s_nativeUserIdsSeenLock.UpgradableReadLockScope())
					{
						if (PlatformUserManager.s_nativeUserIdsSeen.Contains(value))
						{
							flag = false;
						}
						else
						{
							using (PlatformUserManager.s_nativeUserIdsSeenLock.WriteLockScope())
							{
								flag = PlatformUserManager.s_nativeUserIdsSeen.Add(value);
							}
						}
					}
					if (flag)
					{
						PlatformUserManager.OnUserAdded(value, false);
						this.RequestUserDetailsUpdate();
					}
				}
			}

			// Token: 0x17001A42 RID: 6722
			// (get) Token: 0x0600D446 RID: 54342 RVA: 0x004CD8DC File Offset: 0x004CBADC
			// (set) Token: 0x0600D447 RID: 54343 RVA: 0x004CD8E4 File Offset: 0x004CBAE4
			public string Name { get; set; }

			// Token: 0x0600D448 RID: 54344 RVA: 0x004CD8F0 File Offset: 0x004CBAF0
			public void RequestUserDetailsUpdate()
			{
				if (!PlatformUserManager.CanCheckUserDetails())
				{
					return;
				}
				using (PlatformUserManager.s_userDetailsToUpdateLock.WriteLockScope())
				{
					this.DetailAttempts = 0;
					this.NextDetailRetryTime = 0f;
					PlatformUserManager.s_userDetailsToUpdate.Add(this);
				}
			}

			// Token: 0x17001A43 RID: 6723
			// (get) Token: 0x0600D449 RID: 54345 RVA: 0x004CD950 File Offset: 0x004CBB50
			public IReadOnlyDictionary<EBlockType, PlatformUserManager.PlatformUserBlockedData> Blocked
			{
				get
				{
					return this.m_userBlockedStates;
				}
			}

			// Token: 0x17001A44 RID: 6724
			// (get) Token: 0x0600D44A RID: 54346 RVA: 0x004CD958 File Offset: 0x004CBB58
			public IReadOnlyDictionary<EBlockType, IPlatformUserBlockedData> Blocked
			{
				[PublicizedFrom(EAccessModifier.Private)]
				get
				{
					return this.m_userBlockedStatesReadOnly;
				}
			}

			// Token: 0x0600D44B RID: 54347 RVA: 0x004CD960 File Offset: 0x004CBB60
			public void MarkBlockedStateChanged()
			{
				if (GameManager.IsDedicatedServer)
				{
					return;
				}
				using (PlatformUserManager.s_blockedUsersToUpdateLock.UpgradableReadLockScope())
				{
					if (!PlatformUserManager.s_blockedUsersToUpdate.Contains(this))
					{
						using (PlatformUserManager.s_blockedUsersToUpdateLock.WriteLockScope())
						{
							PlatformUserManager.s_blockedUsersToUpdate.Add(this);
						}
					}
				}
			}

			// Token: 0x0400A1D0 RID: 41424
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly EnumDictionary<EBlockType, PlatformUserManager.PlatformUserBlockedData> m_userBlockedStates;

			// Token: 0x0400A1D1 RID: 41425
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly IReadOnlyDictionary<EBlockType, IPlatformUserBlockedData> m_userBlockedStatesReadOnly;

			// Token: 0x0400A1D4 RID: 41428
			public int DetailAttempts;

			// Token: 0x0400A1D5 RID: 41429
			public float NextDetailRetryTime;
		}

		// Token: 0x02001BDB RID: 7131
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class PlatformUserBlockedData : IPlatformUserBlockedData
		{
			// Token: 0x0600D44F RID: 54351 RVA: 0x004CDA0B File Offset: 0x004CBC0B
			public PlatformUserBlockedData(PlatformUserManager.PlatformUserData userData, EBlockType blockType)
			{
				this.m_userData = userData;
				this.Type = blockType;
				this.m_blockedLocally = false;
				this.State = EUserBlockState.NotBlocked;
			}

			// Token: 0x0600D450 RID: 54352 RVA: 0x004CDA30 File Offset: 0x004CBC30
			public override string ToString()
			{
				return string.Format("{0}[Type={1}, State={2}, Locally={3}]", new object[]
				{
					"PlatformUserBlockedData",
					this.Type,
					this.State,
					this.Locally
				});
			}

			// Token: 0x17001A45 RID: 6725
			// (get) Token: 0x0600D451 RID: 54353 RVA: 0x004CDA7F File Offset: 0x004CBC7F
			public EBlockType Type { get; }

			// Token: 0x17001A46 RID: 6726
			// (get) Token: 0x0600D452 RID: 54354 RVA: 0x004CDA87 File Offset: 0x004CBC87
			// (set) Token: 0x0600D453 RID: 54355 RVA: 0x004CDA8F File Offset: 0x004CBC8F
			public EUserBlockState State { get; [PublicizedFrom(EAccessModifier.Private)] set; }

			// Token: 0x17001A47 RID: 6727
			// (get) Token: 0x0600D454 RID: 54356 RVA: 0x004CDA98 File Offset: 0x004CBC98
			// (set) Token: 0x0600D455 RID: 54357 RVA: 0x004CDAA0 File Offset: 0x004CBCA0
			public bool Locally
			{
				get
				{
					return this.m_blockedLocally;
				}
				set
				{
					this.m_blockedLocally = value;
					this.RefreshBlockedState(this.State == EUserBlockState.ByPlatform);
				}
			}

			// Token: 0x0600D456 RID: 54358 RVA: 0x004CDAB8 File Offset: 0x004CBCB8
			public void RefreshBlockedState(bool isBlockedByPlatform)
			{
				EUserBlockState state = this.State;
				EUserBlockState euserBlockState;
				if (isBlockedByPlatform)
				{
					euserBlockState = EUserBlockState.ByPlatform;
				}
				else if (this.Locally)
				{
					euserBlockState = EUserBlockState.InGame;
				}
				else
				{
					euserBlockState = EUserBlockState.NotBlocked;
				}
				if (euserBlockState == EUserBlockState.ByPlatform)
				{
					this.m_blockedLocally = false;
				}
				this.State = euserBlockState;
				if (state != euserBlockState)
				{
					PlatformUserManager.OnBlockedStateChanged(this.m_userData, this.Type, euserBlockState);
				}
			}

			// Token: 0x0400A1D8 RID: 41432
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly PlatformUserManager.PlatformUserData m_userData;

			// Token: 0x0400A1D9 RID: 41433
			[PublicizedFrom(EAccessModifier.Private)]
			public bool m_blockedLocally;
		}

		// Token: 0x02001BDC RID: 7132
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class PlatformUserBlockedResults : IPlatformUserBlockedResults
		{
			// Token: 0x0600D457 RID: 54359 RVA: 0x004CDB08 File Offset: 0x004CBD08
			public PlatformUserBlockedResults(PlatformUserManager.PlatformUserData userData)
			{
				this.m_userData = userData;
				this.IsBlocked = new EnumDictionary<EBlockType, bool>();
				foreach (EBlockType key in EnumUtils.Values<EBlockType>())
				{
					this.IsBlocked[key] = false;
				}
				this.HasErrored = false;
			}

			// Token: 0x0600D458 RID: 54360 RVA: 0x004CDB7C File Offset: 0x004CBD7C
			public override string ToString()
			{
				string format = "{0}[{1}, HasErrored={2}, {3}.{4}={5}, {6}.{7}={8}]";
				object[] array = new object[9];
				array[0] = "PlatformUserBlockedResults";
				array[1] = string.Join(", ", from kv in this.IsBlocked
				select string.Format("IsBlocked[{0}]={1}", kv.Key, kv.Value));
				array[2] = this.HasErrored;
				array[3] = "User";
				array[4] = "PrimaryId";
				array[5] = this.User.PrimaryId;
				array[6] = "User";
				array[7] = "NativeId";
				array[8] = this.User.NativeId;
				return string.Format(format, array);
			}

			// Token: 0x17001A48 RID: 6728
			// (get) Token: 0x0600D459 RID: 54361 RVA: 0x004CDC23 File Offset: 0x004CBE23
			public PlatformUserManager.PlatformUserData User
			{
				get
				{
					return this.m_userData;
				}
			}

			// Token: 0x17001A49 RID: 6729
			// (get) Token: 0x0600D45A RID: 54362 RVA: 0x004CDC2B File Offset: 0x004CBE2B
			public EnumDictionary<EBlockType, bool> IsBlocked { get; }

			// Token: 0x17001A4A RID: 6730
			// (get) Token: 0x0600D45B RID: 54363 RVA: 0x004CDC33 File Offset: 0x004CBE33
			// (set) Token: 0x0600D45C RID: 54364 RVA: 0x004CDC3B File Offset: 0x004CBE3B
			public bool HasErrored { get; [PublicizedFrom(EAccessModifier.Private)] set; }

			// Token: 0x17001A4B RID: 6731
			// (get) Token: 0x0600D45D RID: 54365 RVA: 0x004CDC23 File Offset: 0x004CBE23
			public IPlatformUser User
			{
				[PublicizedFrom(EAccessModifier.Private)]
				get
				{
					return this.m_userData;
				}
			}

			// Token: 0x0600D45E RID: 54366 RVA: 0x004CDC44 File Offset: 0x004CBE44
			public void Block(EBlockType blockType)
			{
				this.IsBlocked[blockType] = true;
			}

			// Token: 0x0600D45F RID: 54367 RVA: 0x004CDC53 File Offset: 0x004CBE53
			public void Error()
			{
				this.HasErrored = true;
			}

			// Token: 0x0400A1DC RID: 41436
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly PlatformUserManager.PlatformUserData m_userData;
		}

		// Token: 0x02001BDE RID: 7134
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class PlatformUserDetailsResult
		{
			// Token: 0x0600D463 RID: 54371 RVA: 0x004CDC8C File Offset: 0x004CBE8C
			public PlatformUserDetailsResult(PlatformUserManager.PlatformUserData userData)
			{
				this.UserData = userData;
			}

			// Token: 0x0400A1E1 RID: 41441
			public readonly PlatformUserManager.PlatformUserData UserData;

			// Token: 0x0400A1E2 RID: 41442
			public string Name;
		}
	}
}
