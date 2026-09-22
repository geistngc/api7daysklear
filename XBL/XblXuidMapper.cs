using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Platform.XBL
{
	// Token: 0x02001C25 RID: 7205
	public static class XblXuidMapper
	{
		// Token: 0x14000131 RID: 305
		// (add) Token: 0x0600D5F6 RID: 54774 RVA: 0x004D3234 File Offset: 0x004D1434
		// (remove) Token: 0x0600D5F7 RID: 54775 RVA: 0x004D3268 File Offset: 0x004D1468
		public static event XblXuidMapper.XuidMappedHandler XuidMapped;

		// Token: 0x14000132 RID: 306
		// (add) Token: 0x0600D5F8 RID: 54776 RVA: 0x004D329C File Offset: 0x004D149C
		// (remove) Token: 0x0600D5F9 RID: 54777 RVA: 0x004D32D0 File Offset: 0x004D14D0
		public static event XblXuidMapper.UserIdentifierMappedHandler UserIdentifierMapped;

		// Token: 0x0600D5FB RID: 54779 RVA: 0x004D3375 File Offset: 0x004D1575
		public static void Enable()
		{
			if (XblXuidMapper.s_enabled)
			{
				return;
			}
			XblXuidMapper.s_enabled = true;
			Log.Out("[XBL-XuidMapper] Enabled.");
		}

		// Token: 0x0600D5FC RID: 54780 RVA: 0x004D3390 File Offset: 0x004D1590
		public static ulong GetXuid(PlatformUserIdentifierAbs userId)
		{
			if (!XblXuidMapper.s_enabled)
			{
				return 0UL;
			}
			XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(userId);
			using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
			{
				ulong result;
				if (XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out result))
				{
					return result;
				}
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				XblXuidMapper.s_mappingService = ((crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null);
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				Log.Error("[XBL-XuidMapper] ID mapping service required to identify Xbl users");
				return 0UL;
			}
			XblXuidMapper.ResolveXuid(xuidState);
			return 0UL;
		}

		// Token: 0x0600D5FD RID: 54781 RVA: 0x004D3428 File Offset: 0x004D1628
		public static void SetXuid(PlatformUserIdentifierAbs userId, ulong xuid)
		{
			if (xuid == 0UL)
			{
				return;
			}
			XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(userId);
			ulong num;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				if (!XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out num))
				{
					num = 0UL;
				}
				if (num == xuid)
				{
					return;
				}
				using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
				{
					XblXuidMapper.s_xuidStateToXuid.RemoveByKey(xuidState);
					XblXuidMapper.s_xuidStateToXuid.RemoveByValue(xuid);
					XblXuidMapper.s_xuidStateToXuid.Add(xuidState, xuid);
				}
				XblXuidMapper.XuidState obj = xuidState;
				lock (obj)
				{
					xuidState.InProgress = false;
					xuidState.AttemptsCompleted++;
				}
			}
			if (num != 0UL)
			{
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					IReadOnlyCollection<PlatformUserIdentifierAbs> values;
					XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, out values);
					Log.Warning(string.Format("[XBL-XuidMapper] Unexpected mapping change Xuid changed from '{0}' to '{1}' for UserIds: {2}", num, xuid, string.Join<PlatformUserIdentifierAbs>(", ", values)));
				}
			}
			object obj2 = XblXuidMapper.s_xuidMappedResultTempLock;
			lock (obj2)
			{
				XblXuidMapper.s_xuidMappedResultTemp.Clear();
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, XblXuidMapper.s_xuidMappedResultTemp);
				}
				XblXuidMapper.XuidMappedHandler xuidMapped = XblXuidMapper.XuidMapped;
				if (xuidMapped != null)
				{
					xuidMapped(XblXuidMapper.s_xuidMappedResultTemp, xuid);
				}
				XblXuidMapper.s_xuidMappedResultTemp.Clear();
			}
		}

		// Token: 0x0600D5FE RID: 54782 RVA: 0x004D3600 File Offset: 0x004D1800
		[PublicizedFrom(EAccessModifier.Private)]
		public static void ResolveXuid(XblXuidMapper.XuidState xuidState)
		{
			XblXuidMapper.<>c__DisplayClass24_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass24_0();
			CS$<>8__locals1.xuidState = xuidState;
			CS$<>8__locals1.userIds = null;
			XblXuidMapper.XuidState xuidState2 = CS$<>8__locals1.xuidState;
			lock (xuidState2)
			{
				if (CS$<>8__locals1.xuidState.InProgress)
				{
					return;
				}
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					IReadOnlyCollection<PlatformUserIdentifierAbs> readOnlyCollection;
					if (XblXuidMapper.s_xuidStateToUserId.TryGetByKey(CS$<>8__locals1.xuidState, out readOnlyCollection))
					{
						foreach (PlatformUserIdentifierAbs platformUserIdentifierAbs in readOnlyCollection)
						{
							if (XblXuidMapper.s_mappingService.CanQuery(platformUserIdentifierAbs))
							{
								using (XblXuidMapper.s_userIdsWithNoXblMappingLock.ReadLockScope())
								{
									if (XblXuidMapper.s_userIdsWithNoXblMapping.Contains(platformUserIdentifierAbs))
									{
										continue;
									}
								}
								if (CS$<>8__locals1.userIds == null)
								{
									CS$<>8__locals1.userIds = new List<PlatformUserIdentifierAbs>();
								}
								CS$<>8__locals1.userIds.Add(platformUserIdentifierAbs);
							}
						}
					}
				}
				if (CS$<>8__locals1.userIds == null || CS$<>8__locals1.userIds.Count <= 0)
				{
					return;
				}
				CS$<>8__locals1.attempt = CS$<>8__locals1.xuidState.AttemptsCompleted + 1;
				CS$<>8__locals1.xuidState.InProgress = true;
			}
			CS$<>8__locals1.userIdsIndex = -1;
			CS$<>8__locals1.<ResolveXuid>g__ProcessNextId|1();
		}

		// Token: 0x0600D5FF RID: 54783 RVA: 0x004D37B0 File Offset: 0x004D19B0
		public static void ResolveUserIdentifiers(IReadOnlyCollection<ulong> xuids)
		{
			XblXuidMapper.<>c__DisplayClass25_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass25_0();
			if (XblXuidMapper.s_mappingService == null)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				XblXuidMapper.s_mappingService = ((crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null);
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				return;
			}
			using (XblXuidMapper.s_xuidsCheckedForUserIdentifiersLock.ReadLockScope())
			{
				CS$<>8__locals1.xuidsToRequest = (from xuid in xuids
				where !XblXuidMapper.s_xuidsCheckedForUserIdentifiers.Contains(xuid)
				where XblXuidMapper.s_mappingService.CanReverseQuery(EPlatformIdentifier.XBL, xuid.ToString())
				select xuid).ToArray<ulong>();
			}
			if (CS$<>8__locals1.xuidsToRequest.Length == 0)
			{
				return;
			}
			CS$<>8__locals1.requests = (from xuid in CS$<>8__locals1.xuidsToRequest
			select new MappedAccountReverseRequest(EPlatformIdentifier.XBL, xuid.ToString())).ToArray<MappedAccountReverseRequest>();
			Log.Out("[XBL-XuidMapper] Reverse mapping XUID(s) '" + string.Join<ulong>("','", xuids) + "'...");
			XblXuidMapper.s_mappingService.ReverseQueryMappedAccountsDetails(CS$<>8__locals1.requests, new MappedAccountsReverseQueryCallback(CS$<>8__locals1.<ResolveUserIdentifiers>g__ResolveUserIdentifiersCallback|1));
		}

		// Token: 0x0600D600 RID: 54784 RVA: 0x004D38E4 File Offset: 0x004D1AE4
		[PublicizedFrom(EAccessModifier.Private)]
		public static XblXuidMapper.XuidState GetXuidState(PlatformUserIdentifierAbs userId)
		{
			XblXuidMapper.XuidState result;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				XblXuidMapper.XuidState xuidState;
				if (XblXuidMapper.s_xuidStateToUserId.TryGetByValue(userId, out xuidState))
				{
					result = xuidState;
				}
				else
				{
					using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
					{
						PlatformUserIdentifierAbs[] array = new PlatformUserIdentifierAbs[4];
						int num = PlatformUserManager.TryGetByNative(userId, array.AsSpan(0, array.Length - 1));
						array[num] = userId;
						int num2 = num + 1;
						int num3 = 0;
						while (num3 < num2 && !XblXuidMapper.s_xuidStateToUserId.TryGetByValue(array[num3], out xuidState))
						{
							num3++;
						}
						if (xuidState == null)
						{
							xuidState = new XblXuidMapper.XuidState();
						}
						for (int i = 0; i < num2; i++)
						{
							PlatformUserIdentifierAbs value = array[i];
							XblXuidMapper.XuidState xuidState2;
							if (!XblXuidMapper.s_xuidStateToUserId.TryGetByValue(value, out xuidState2))
							{
								XblXuidMapper.s_xuidStateToUserId.Add(xuidState, value);
							}
							else if (xuidState2 != xuidState)
							{
								XblXuidMapper.s_xuidStateToUserId.RemoveByValue(value);
								XblXuidMapper.s_xuidStateToUserId.Add(xuidState, value);
								Log.Error(string.Format("[XBL-XuidMapper] Unexpected state merge. UserId '{0}' already had state but has been merged with UserIds: '{1}'.", array[i], string.Join<PlatformUserIdentifierAbs>("', '", array.Take(num2))));
							}
						}
					}
					result = xuidState;
				}
			}
			return result;
		}

		// Token: 0x0600D601 RID: 54785 RVA: 0x004D3A48 File Offset: 0x004D1C48
		[PublicizedFrom(EAccessModifier.Private)]
		public static XblXuidMapper.XuidState GetXuidState(ulong xuid)
		{
			if (xuid == 0UL)
			{
				return XblXuidMapper.EmptyXuidState;
			}
			XblXuidMapper.XuidState result;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				XblXuidMapper.XuidState xuidState;
				if (XblXuidMapper.s_xuidStateToXuid.TryGetByValue(xuid, out xuidState))
				{
					result = xuidState;
				}
				else
				{
					using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
					{
						xuidState = new XblXuidMapper.XuidState();
						XblXuidMapper.s_xuidStateToXuid.Add(xuidState, xuid);
					}
					result = xuidState;
				}
			}
			return result;
		}

		// Token: 0x0600D602 RID: 54786 RVA: 0x004D3AD8 File Offset: 0x004D1CD8
		public static void ResolveXuids(IReadOnlyList<XuidResolveRequest> _requests, Action<IReadOnlyList<XuidResolveRequest>> _onComplete)
		{
			XblXuidMapper.<>c__DisplayClass29_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass29_0();
			CS$<>8__locals1._requests = _requests;
			CS$<>8__locals1._onComplete = _onComplete;
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			IUserIdentifierMappingService userIdentifierMappingService = (crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null;
			if (userIdentifierMappingService == null)
			{
				Log.Error("[XBL-XuidMapper] Cannot resolve xuids, no mapping service available");
				CS$<>8__locals1._onComplete(CS$<>8__locals1._requests);
				return;
			}
			List<MappedAccountRequest> list = null;
			CS$<>8__locals1.requestIndices = null;
			int i = 0;
			while (i < CS$<>8__locals1._requests.Count)
			{
				XuidResolveRequest xuidResolveRequest = CS$<>8__locals1._requests[i];
				XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(xuidResolveRequest.Id);
				IReadOnlyCollection<PlatformUserIdentifierAbs> readOnlyCollection;
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					ulong num;
					if (!XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out num))
					{
						num = 0UL;
					}
					if (num != 0UL)
					{
						xuidResolveRequest.Xuid = num;
						xuidResolveRequest.IsSuccess = true;
						goto IL_169;
					}
					if (!XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, out readOnlyCollection))
					{
						goto IL_169;
					}
				}
				goto IL_D0;
				IL_169:
				i++;
				continue;
				IL_D0:
				foreach (PlatformUserIdentifierAbs platformUserIdentifierAbs in readOnlyCollection)
				{
					if (userIdentifierMappingService.CanQuery(platformUserIdentifierAbs))
					{
						using (XblXuidMapper.s_userIdsWithNoXblMappingLock.ReadLockScope())
						{
							if (XblXuidMapper.s_userIdsWithNoXblMapping.Contains(platformUserIdentifierAbs))
							{
								continue;
							}
						}
						if (list == null)
						{
							list = new List<MappedAccountRequest>();
						}
						if (CS$<>8__locals1.requestIndices == null)
						{
							CS$<>8__locals1.requestIndices = new List<int>();
						}
						list.Add(new MappedAccountRequest(platformUserIdentifierAbs, EPlatformIdentifier.XBL));
						CS$<>8__locals1.requestIndices.Add(i);
						break;
					}
				}
				goto IL_169;
			}
			if (list == null)
			{
				CS$<>8__locals1._onComplete(CS$<>8__locals1._requests);
				return;
			}
			userIdentifierMappingService.QueryMappedAccountsDetails(list, new MappedAccountsQueryCallback(CS$<>8__locals1.<ResolveXuids>g__Callback|0));
		}

		// Token: 0x0400A343 RID: 41795
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool s_enabled;

		// Token: 0x0400A344 RID: 41796
		[PublicizedFrom(EAccessModifier.Private)]
		public static IUserIdentifierMappingService s_mappingService;

		// Token: 0x0400A345 RID: 41797
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblXuidMapper.XuidState EmptyXuidState = new XblXuidMapper.XuidState();

		// Token: 0x0400A348 RID: 41800
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly OneToManyDictionary<XblXuidMapper.XuidState, PlatformUserIdentifierAbs> s_xuidStateToUserId = new OneToManyDictionary<XblXuidMapper.XuidState, PlatformUserIdentifierAbs>();

		// Token: 0x0400A349 RID: 41801
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly BiDictionary<XblXuidMapper.XuidState, ulong> s_xuidStateToXuid = new BiDictionary<XblXuidMapper.XuidState, ulong>();

		// Token: 0x0400A34A RID: 41802
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_xuidStateDictionaryLock = new ReaderWriterLockSlim();

		// Token: 0x0400A34B RID: 41803
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<PlatformUserIdentifierAbs> s_userIdsWithNoXblMapping = new HashSet<PlatformUserIdentifierAbs>();

		// Token: 0x0400A34C RID: 41804
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_userIdsWithNoXblMappingLock = new ReaderWriterLockSlim();

		// Token: 0x0400A34D RID: 41805
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<ulong> s_xuidsCheckedForUserIdentifiers = new HashSet<ulong>();

		// Token: 0x0400A34E RID: 41806
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_xuidsCheckedForUserIdentifiersLock = new ReaderWriterLockSlim();

		// Token: 0x0400A34F RID: 41807
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly List<PlatformUserIdentifierAbs> s_xuidMappedResultTemp = new List<PlatformUserIdentifierAbs>();

		// Token: 0x0400A350 RID: 41808
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object s_xuidMappedResultTempLock = new object();

		// Token: 0x02001C26 RID: 7206
		// (Invoke) Token: 0x0600D604 RID: 54788
		public delegate void XuidMappedHandler(IReadOnlyCollection<PlatformUserIdentifierAbs> userIds, ulong xuid);

		// Token: 0x02001C27 RID: 7207
		// (Invoke) Token: 0x0600D608 RID: 54792
		public delegate void UserIdentifierMappedHandler(ulong xuid, PlatformUserIdentifierAbs userId);

		// Token: 0x02001C28 RID: 7208
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class XuidState
		{
			// Token: 0x0400A351 RID: 41809
			public int AttemptsCompleted;

			// Token: 0x0400A352 RID: 41810
			public bool InProgress;
		}
	}
}
