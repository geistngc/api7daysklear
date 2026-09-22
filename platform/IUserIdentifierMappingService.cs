using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001BC0 RID: 7104
	public interface IUserIdentifierMappingService
	{
		// Token: 0x0600D3A1 RID: 54177
		bool CanQuery(PlatformUserIdentifierAbs _id);

		// Token: 0x0600D3A2 RID: 54178
		void QueryMappedAccountDetails(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform, MappedAccountQueryCallback _callback);

		// Token: 0x0600D3A3 RID: 54179
		void QueryMappedAccountsDetails(IReadOnlyList<MappedAccountRequest> _requests, MappedAccountsQueryCallback _callback);

		// Token: 0x0600D3A4 RID: 54180
		bool CanReverseQuery(EPlatformIdentifier _platform, string _platformId);

		// Token: 0x0600D3A5 RID: 54181
		void ReverseQueryMappedAccountDetails(EPlatformIdentifier _platform, string _platformId, MappedAccountReverseQueryCallback _callback);

		// Token: 0x0600D3A6 RID: 54182
		void ReverseQueryMappedAccountsDetails(IReadOnlyList<MappedAccountReverseRequest> _requests, MappedAccountsReverseQueryCallback _callback);
	}
}
