using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;

namespace Platform.EOS
{
	// Token: 0x02001CF2 RID: 7410
	public class EosUserIdMapper : IUserIdentifierMappingService
	{
		// Token: 0x0600DBD7 RID: 56279 RVA: 0x004EC1B9 File Offset: 0x004EA3B9
		public EosUserIdMapper(Api _eosApi, User _eosUser)
		{
			this.api = _eosApi;
			this.user = _eosUser;
		}

		// Token: 0x0600DBD8 RID: 56280 RVA: 0x004EC1CF File Offset: 0x004EA3CF
		public bool CanQuery(PlatformUserIdentifierAbs _id)
		{
			return _id is UserIdentifierEos;
		}

		// Token: 0x0600DBD9 RID: 56281 RVA: 0x004EC1DC File Offset: 0x004EA3DC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool TryValidateUser(out ProductUserId loggedInUser, out MappedAccountQueryResult failureResult)
		{
			loggedInUser = null;
			failureResult = MappedAccountQueryResult.QueryFailed;
			PlatformUserIdentifierAbs platformUserId = this.user.PlatformUserId;
			if (platformUserId == null)
			{
				Log.Out("[EOS] Cannot query mapped account details yet - EOS does not have info yet");
				failureResult = MappedAccountQueryResult.NotReady;
				return false;
			}
			UserIdentifierEos userIdentifierEos = platformUserId as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS] Cannot query mapped account details. EosUserIdMapper got non-EOS id type: {0}", platformUserId));
				return false;
			}
			loggedInUser = userIdentifierEos.ProductUserId;
			if (loggedInUser == null)
			{
				Log.Out(string.Format("[EOS] Cannot query mapped account details - EOS does not have info yet for {0}", userIdentifierEos));
				failureResult = MappedAccountQueryResult.NotReady;
				return false;
			}
			return true;
		}

		// Token: 0x0600DBDA RID: 56282 RVA: 0x004EC254 File Offset: 0x004EA454
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool TryValidateRequest(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform, out ProductUserId _puid)
		{
			UserIdentifierEos userIdentifierEos = _id as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS] Cannot retrieve mapped account details, {0} is not an eos product user id", _id));
				_puid = null;
				return false;
			}
			_puid = userIdentifierEos.ProductUserId;
			if (!EosHelpers.PlatformIdentifierMappings.ContainsKey(_platform))
			{
				Log.Error(string.Format("[EOS] Cannot retrieve mapped acount details, target platform {0} does not map to a known external account type", _platform));
				return false;
			}
			return true;
		}

		// Token: 0x0600DBDB RID: 56283 RVA: 0x004EC2B0 File Offset: 0x004EA4B0
		public void QueryMappedAccountDetails(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform, MappedAccountQueryCallback _callback)
		{
			ProductUserId loggedInUser;
			MappedAccountQueryResult result;
			if (!this.TryValidateUser(out loggedInUser, out result))
			{
				_callback(result, _platform, null, null);
				return;
			}
			ProductUserId puid;
			if (!EosUserIdMapper.TryValidateRequest(_id, _platform, out puid))
			{
				_callback(MappedAccountQueryResult.QueryFailed, _platform, null, null);
				return;
			}
			this.QueryMappedExternalAccount(loggedInUser, puid, _platform, _callback);
		}

		// Token: 0x0600DBDC RID: 56284 RVA: 0x004EC2F4 File Offset: 0x004EA4F4
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueryMappedExternalAccount(ProductUserId _loggedInUser, ProductUserId _puid, EPlatformIdentifier _platform, MappedAccountQueryCallback _callback)
		{
			if (!ThreadManager.IsMainThread())
			{
				ThreadManager.AddSingleTaskMainThread("QueryEosMappedAccount", delegate(object _)
				{
					this.QueryMappedExternalAccount(_loggedInUser, _puid, _platform, _callback);
				}, null);
				return;
			}
			if (this.api.ConnectInterface == null)
			{
				Log.Out("[EOS] QueryProductUserIdMappings failed, connect interface null");
				_callback(MappedAccountQueryResult.QueryFailed, _platform, null, null);
				return;
			}
			ExternalAccountType externalAccountType;
			if (!EosHelpers.PlatformIdentifierMappings.TryGetValue(_platform, out externalAccountType))
			{
				Log.Out(string.Format("[EOS] Unknown external account type for {0}", _platform));
				_callback(MappedAccountQueryResult.QueryFailed, _platform, null, null);
				return;
			}
			QueryProductUserIdMappingsOptions queryProductUserIdMappingsOptions = default(QueryProductUserIdMappingsOptions);
			queryProductUserIdMappingsOptions.LocalUserId = _loggedInUser;
			queryProductUserIdMappingsOptions.ProductUserIds = new ProductUserId[]
			{
				_puid
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.api.ConnectInterface.QueryProductUserIdMappings(ref queryProductUserIdMappingsOptions, queryProductUserIdMappingsOptions.ProductUserIds, delegate(ref QueryProductUserIdMappingsCallbackInfo _response)
				{
					if (_response.ResultCode != Result.Success)
					{
						Log.Out(string.Format("[EOS] QueryProductUserIdMappings failed {0}", _response.ResultCode));
						_callback(MappedAccountQueryResult.QueryFailed, _platform, null, null);
						return;
					}
					if (this.api.ConnectInterface == null)
					{
						Log.Out("[EOS] QueryProductUserIdMappings failed, connect interface null");
						_callback(MappedAccountQueryResult.QueryFailed, _platform, null, null);
						return;
					}
					ProductUserId targetUserId = ((ProductUserId[])_response.ClientData)[0];
					CopyProductUserExternalAccountByAccountTypeOptions copyOptions = default(CopyProductUserExternalAccountByAccountTypeOptions);
					copyOptions.TargetUserId = targetUserId;
					copyOptions.AccountIdType = externalAccountType;
					ExternalAccountInfo externalAccountInfo;
					if (!this.TryCopyResult(copyOptions, out externalAccountInfo))
					{
						_callback(MappedAccountQueryResult.MappingNotFound, _platform, null, null);
						return;
					}
					if (Api.DebugLevel == Api.EDebugLevel.Verbose)
					{
						Log.Out(string.Format("[EOS] found external account for {0}: Type: {1}, Id: {2}", _puid, externalAccountType, externalAccountInfo.AccountId));
					}
					_callback(MappedAccountQueryResult.Success, _platform, externalAccountInfo.AccountId, externalAccountInfo.DisplayName);
				});
			}
		}

		// Token: 0x0600DBDD RID: 56285 RVA: 0x004EC440 File Offset: 0x004EA640
		[PublicizedFrom(EAccessModifier.Private)]
		public bool TryCopyResult(CopyProductUserExternalAccountByAccountTypeOptions _copyOptions, out ExternalAccountInfo _externalAccountInfo)
		{
			object lockObject = AntiCheatCommon.LockObject;
			ExternalAccountInfo? externalAccountInfo;
			Result result;
			lock (lockObject)
			{
				result = this.api.ConnectInterface.CopyProductUserExternalAccountByAccountType(ref _copyOptions, out externalAccountInfo);
			}
			if (result != Result.Success)
			{
				Log.Out(string.Format("[EOS] {0} copy failed. Result: {1}", _copyOptions.TargetUserId, result));
				_externalAccountInfo = default(ExternalAccountInfo);
				return false;
			}
			if (externalAccountInfo == null)
			{
				Log.Out(string.Format("[EOS] {0} copy failed, null info", _copyOptions.TargetUserId));
				_externalAccountInfo = default(ExternalAccountInfo);
				return false;
			}
			_externalAccountInfo = externalAccountInfo.Value;
			return true;
		}

		// Token: 0x0600DBDE RID: 56286 RVA: 0x004EC4EC File Offset: 0x004EA6EC
		public void QueryMappedAccountsDetails(IReadOnlyList<MappedAccountRequest> _requests, MappedAccountsQueryCallback _callback)
		{
			if (_requests.Count == 0)
			{
				_callback(_requests);
				return;
			}
			ProductUserId loggedInUser;
			MappedAccountQueryResult result;
			if (!this.TryValidateUser(out loggedInUser, out result))
			{
				foreach (MappedAccountRequest mappedAccountRequest in _requests)
				{
					mappedAccountRequest.Result = result;
				}
				_callback(_requests);
				return;
			}
			this.QueryMappedExternalAccounts(loggedInUser, _requests, _callback);
		}

		// Token: 0x0600DBDF RID: 56287 RVA: 0x004EC560 File Offset: 0x004EA760
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueryMappedExternalAccounts(ProductUserId _loggedInUser, IReadOnlyList<MappedAccountRequest> _requests, MappedAccountsQueryCallback _callback)
		{
			if (!ThreadManager.IsMainThread())
			{
				ThreadManager.AddSingleTaskMainThread("QueryEosMappedAccounts", delegate(object _)
				{
					this.QueryMappedExternalAccounts(_loggedInUser, _requests, _callback);
				}, null);
				return;
			}
			if (this.api.ConnectInterface == null)
			{
				Log.Out("[EOS] QueryProductUserIdMappings failed, connect interface null");
				foreach (MappedAccountRequest mappedAccountRequest in _requests)
				{
					mappedAccountRequest.Result = MappedAccountQueryResult.QueryFailed;
				}
				_callback(_requests);
				return;
			}
			List<ProductUserId> puids = null;
			List<int> requestIndices = null;
			for (int i = 0; i < _requests.Count; i++)
			{
				MappedAccountRequest mappedAccountRequest2 = _requests[i];
				ProductUserId item;
				if (!EosUserIdMapper.TryValidateRequest(mappedAccountRequest2.Id, mappedAccountRequest2.Platform, out item))
				{
					mappedAccountRequest2.Result = MappedAccountQueryResult.QueryFailed;
				}
				else
				{
					if (puids == null)
					{
						puids = new List<ProductUserId>();
					}
					if (requestIndices == null)
					{
						requestIndices = new List<int>();
					}
					puids.Add(item);
					requestIndices.Add(i);
				}
			}
			if (puids == null)
			{
				_callback(_requests);
				return;
			}
			QueryProductUserIdMappingsOptions queryProductUserIdMappingsOptions = default(QueryProductUserIdMappingsOptions);
			queryProductUserIdMappingsOptions.LocalUserId = _loggedInUser;
			queryProductUserIdMappingsOptions.ProductUserIds = puids.ToArray();
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.api.ConnectInterface.QueryProductUserIdMappings(ref queryProductUserIdMappingsOptions, null, delegate(ref QueryProductUserIdMappingsCallbackInfo _response)
				{
					if (_response.ResultCode != Result.Success)
					{
						Log.Out(string.Format("[EOS] QueryProductUserIdMappings failed {0}", _response.ResultCode));
						foreach (MappedAccountRequest mappedAccountRequest3 in _requests)
						{
							mappedAccountRequest3.Result = MappedAccountQueryResult.QueryFailed;
						}
						_callback(_requests);
						return;
					}
					if (this.api.ConnectInterface == null)
					{
						Log.Out("[EOS] QueryProductUserIdMappings failed, connect interface null");
						foreach (MappedAccountRequest mappedAccountRequest4 in _requests)
						{
							mappedAccountRequest4.Result = MappedAccountQueryResult.QueryFailed;
						}
						_callback(_requests);
						return;
					}
					CopyProductUserExternalAccountByAccountTypeOptions copyOptions = default(CopyProductUserExternalAccountByAccountTypeOptions);
					for (int j = 0; j < puids.Count; j++)
					{
						int index = requestIndices[j];
						MappedAccountRequest mappedAccountRequest5 = _requests[index];
						ProductUserId productUserId = puids[j];
						copyOptions.TargetUserId = productUserId;
						copyOptions.AccountIdType = EosHelpers.PlatformIdentifierMappings[mappedAccountRequest5.Platform];
						ExternalAccountInfo externalAccountInfo;
						if (!this.TryCopyResult(copyOptions, out externalAccountInfo))
						{
							mappedAccountRequest5.Result = MappedAccountQueryResult.MappingNotFound;
						}
						else
						{
							mappedAccountRequest5.MappedAccountId = externalAccountInfo.AccountId;
							mappedAccountRequest5.DisplayName = externalAccountInfo.DisplayName;
							mappedAccountRequest5.Result = MappedAccountQueryResult.Success;
							if (Api.DebugLevel == Api.EDebugLevel.Verbose)
							{
								Log.Out(string.Format("[EOS] found external account for {0}: Type: {1}, Id: {2}", productUserId, externalAccountInfo.AccountIdType, externalAccountInfo.AccountId));
							}
						}
					}
					_callback(_requests);
				});
			}
		}

		// Token: 0x0600DBE0 RID: 56288 RVA: 0x004EC73C File Offset: 0x004EA93C
		public bool CanReverseQuery(EPlatformIdentifier _platform, string _platformId)
		{
			return EosHelpers.PlatformIdentifierMappings.ContainsKey(_platform);
		}

		// Token: 0x0600DBE1 RID: 56289 RVA: 0x004EC749 File Offset: 0x004EA949
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool TryValidateReverseRequest(EPlatformIdentifier _platform, out ExternalAccountType _externalAccountType)
		{
			if (!EosHelpers.PlatformIdentifierMappings.TryGetValue(_platform, out _externalAccountType))
			{
				Log.Error(string.Format("[EOS] Cannot retrieve reverse mapped account details, target platform {0} does not map to a known external account type", _platform));
				return false;
			}
			return true;
		}

		// Token: 0x0600DBE2 RID: 56290 RVA: 0x004EC774 File Offset: 0x004EA974
		public void ReverseQueryMappedAccountDetails(EPlatformIdentifier _platform, string _platformId, MappedAccountReverseQueryCallback _callback)
		{
			EosUserIdMapper.<>c__DisplayClass13_0 CS$<>8__locals1 = new EosUserIdMapper.<>c__DisplayClass13_0();
			CS$<>8__locals1._callback = _callback;
			this.ReverseQueryMappedAccountsDetails(new MappedAccountReverseRequest[]
			{
				new MappedAccountReverseRequest(_platform, _platformId)
			}, new MappedAccountsReverseQueryCallback(CS$<>8__locals1.<ReverseQueryMappedAccountDetails>g__Callback|0));
		}

		// Token: 0x0600DBE3 RID: 56291 RVA: 0x004EC7B0 File Offset: 0x004EA9B0
		public void ReverseQueryMappedAccountsDetails(IReadOnlyList<MappedAccountReverseRequest> _requests, MappedAccountsReverseQueryCallback _callback)
		{
			if (_requests.Count == 0)
			{
				_callback(_requests);
				return;
			}
			ProductUserId loggedInUser;
			MappedAccountQueryResult result;
			if (!this.TryValidateUser(out loggedInUser, out result))
			{
				foreach (MappedAccountReverseRequest mappedAccountReverseRequest in _requests)
				{
					mappedAccountReverseRequest.Result = result;
				}
				_callback(_requests);
				return;
			}
			this.QueryExternalAccountMappings(loggedInUser, _requests, _callback);
		}

		// Token: 0x0600DBE4 RID: 56292 RVA: 0x004EC824 File Offset: 0x004EAA24
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueryExternalAccountMappings(ProductUserId _loggedInUser, IReadOnlyList<MappedAccountReverseRequest> _requests, MappedAccountsReverseQueryCallback _callback)
		{
			EosUserIdMapper.<>c__DisplayClass15_0 CS$<>8__locals1 = new EosUserIdMapper.<>c__DisplayClass15_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._loggedInUser = _loggedInUser;
			CS$<>8__locals1._requests = _requests;
			CS$<>8__locals1._callback = _callback;
			if (!ThreadManager.IsMainThread())
			{
				ThreadManager.AddSingleTaskMainThread("QueryEosExternalAccountMappings", delegate(object _)
				{
					CS$<>8__locals1.<>4__this.QueryExternalAccountMappings(CS$<>8__locals1._loggedInUser, CS$<>8__locals1._requests, CS$<>8__locals1._callback);
				}, null);
				return;
			}
			if (this.api.ConnectInterface == null)
			{
				Log.Out("[EOS] QueryExternalAccountMappings failed, connect interface null");
				foreach (MappedAccountReverseRequest mappedAccountReverseRequest in CS$<>8__locals1._requests)
				{
					mappedAccountReverseRequest.Result = MappedAccountQueryResult.QueryFailed;
				}
				CS$<>8__locals1._callback(CS$<>8__locals1._requests);
				return;
			}
			CS$<>8__locals1.requestsByType = null;
			foreach (MappedAccountReverseRequest mappedAccountReverseRequest2 in CS$<>8__locals1._requests)
			{
				ExternalAccountType key;
				if (!EosUserIdMapper.TryValidateReverseRequest(mappedAccountReverseRequest2.Platform, out key))
				{
					mappedAccountReverseRequest2.Result = MappedAccountQueryResult.QueryFailed;
				}
				else
				{
					if (CS$<>8__locals1.requestsByType == null)
					{
						CS$<>8__locals1.requestsByType = new Dictionary<ExternalAccountType, List<MappedAccountReverseRequest>>();
					}
					List<MappedAccountReverseRequest> list;
					if (!CS$<>8__locals1.requestsByType.TryGetValue(key, out list))
					{
						list = new List<MappedAccountReverseRequest>();
						CS$<>8__locals1.requestsByType[key] = list;
					}
					list.Add(mappedAccountReverseRequest2);
				}
			}
			if (CS$<>8__locals1.requestsByType == null)
			{
				CS$<>8__locals1._callback(CS$<>8__locals1._requests);
				return;
			}
			CS$<>8__locals1.requestsFinished = 0;
			foreach (KeyValuePair<ExternalAccountType, List<MappedAccountReverseRequest>> keyValuePair in CS$<>8__locals1.requestsByType)
			{
				ExternalAccountType externalAccountType;
				List<MappedAccountReverseRequest> list2;
				keyValuePair.Deconstruct(out externalAccountType, out list2);
				ExternalAccountType externalAccountType2 = externalAccountType;
				List<MappedAccountReverseRequest> requests = list2;
				this.QueryExternalAccountMappings(CS$<>8__locals1._loggedInUser, externalAccountType2, requests, new Action(CS$<>8__locals1.<QueryExternalAccountMappings>g__Callback|1));
			}
		}

		// Token: 0x0600DBE5 RID: 56293 RVA: 0x004ECA00 File Offset: 0x004EAC00
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueryExternalAccountMappings(ProductUserId _loggedInUser, ExternalAccountType _externalAccountType, IReadOnlyList<MappedAccountReverseRequest> _requests, Action _callback)
		{
			Utf8String[] externalAccountIds = new Utf8String[_requests.Count];
			for (int i = 0; i < _requests.Count; i++)
			{
				externalAccountIds[i] = _requests[i].Id;
			}
			QueryExternalAccountMappingsOptions queryExternalAccountMappingsOptions = new QueryExternalAccountMappingsOptions
			{
				LocalUserId = _loggedInUser,
				AccountIdType = _externalAccountType,
				ExternalAccountIds = externalAccountIds
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.api.ConnectInterface.QueryExternalAccountMappings(ref queryExternalAccountMappingsOptions, null, delegate(ref QueryExternalAccountMappingsCallbackInfo _response)
				{
					if (_response.ResultCode != Result.Success)
					{
						Log.Out(string.Format("[EOS] QueryProductUserIdMappings failed {0}", _response.ResultCode));
						foreach (MappedAccountReverseRequest mappedAccountReverseRequest in _requests)
						{
							mappedAccountReverseRequest.Result = MappedAccountQueryResult.QueryFailed;
						}
						_callback();
						return;
					}
					if (this.api.ConnectInterface == null)
					{
						Log.Out("[EOS] QueryProductUserIdMappings failed, connect interface null");
						foreach (MappedAccountReverseRequest mappedAccountReverseRequest2 in _requests)
						{
							mappedAccountReverseRequest2.Result = MappedAccountQueryResult.QueryFailed;
						}
						_callback();
						return;
					}
					GetExternalAccountMappingsOptions getExternalAccountMappingsOptions = new GetExternalAccountMappingsOptions
					{
						LocalUserId = _loggedInUser,
						AccountIdType = _externalAccountType
					};
					for (int j = 0; j < externalAccountIds.Length; j++)
					{
						getExternalAccountMappingsOptions.TargetExternalUserId = externalAccountIds[j];
						MappedAccountReverseRequest mappedAccountReverseRequest3 = _requests[j];
						object lockObject2 = AntiCheatCommon.LockObject;
						ProductUserId externalAccountMapping;
						lock (lockObject2)
						{
							externalAccountMapping = this.api.ConnectInterface.GetExternalAccountMapping(ref getExternalAccountMappingsOptions);
						}
						if (externalAccountMapping == null || !externalAccountMapping.IsValid())
						{
							mappedAccountReverseRequest3.Result = MappedAccountQueryResult.MappingNotFound;
						}
						else
						{
							mappedAccountReverseRequest3.Result = MappedAccountQueryResult.Success;
							mappedAccountReverseRequest3.PlatformId = new UserIdentifierEos(externalAccountMapping);
							if (Api.DebugLevel == Api.EDebugLevel.Verbose)
							{
								Log.Out(string.Format("[EOS] found EOS account {0} from external account: Type: {1}, Id: {2}", externalAccountMapping, _externalAccountType, mappedAccountReverseRequest3.Id));
							}
						}
					}
					_callback();
				});
			}
		}

		// Token: 0x0400A677 RID: 42615
		[PublicizedFrom(EAccessModifier.Private)]
		public Api api;

		// Token: 0x0400A678 RID: 42616
		[PublicizedFrom(EAccessModifier.Private)]
		public User user;
	}
}
