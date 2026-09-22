using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001BA6 RID: 7078
	public interface IServerListInterface
	{
		// Token: 0x17001A18 RID: 6680
		// (get) Token: 0x0600D348 RID: 54088
		bool IsPrefiltered { get; }

		// Token: 0x0600D349 RID: 54089
		void Init(IPlatform _owner);

		// Token: 0x0600D34A RID: 54090
		void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _sessionSearchErrorCallback);

		// Token: 0x17001A19 RID: 6681
		// (get) Token: 0x0600D34B RID: 54091
		bool IsRefreshing { get; }

		// Token: 0x0600D34C RID: 54092
		void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters);

		// Token: 0x0600D34D RID: 54093
		void StopSearch();

		// Token: 0x0600D34E RID: 54094
		void Disconnect();

		// Token: 0x0600D34F RID: 54095
		void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback);

		// Token: 0x02001BA7 RID: 7079
		public class ServerFilter
		{
			// Token: 0x0600D350 RID: 54096 RVA: 0x004CAF78 File Offset: 0x004C9178
			public ServerFilter(string _name, IServerListInterface.ServerFilter.EServerFilterType _type = IServerListInterface.ServerFilter.EServerFilterType.Any, int _intMinValue = 0, int _intMaxValue = 0, bool _boolValue = false, string _stringNeedle = null)
			{
				this.Name = _name;
				this.Type = _type;
				this.IntMinValue = _intMinValue;
				this.IntMaxValue = _intMaxValue;
				this.BoolValue = _boolValue;
				this.StringNeedle = _stringNeedle;
			}

			// Token: 0x0400A13D RID: 41277
			public readonly string Name;

			// Token: 0x0400A13E RID: 41278
			public readonly IServerListInterface.ServerFilter.EServerFilterType Type;

			// Token: 0x0400A13F RID: 41279
			public readonly int IntMinValue;

			// Token: 0x0400A140 RID: 41280
			public readonly int IntMaxValue;

			// Token: 0x0400A141 RID: 41281
			public readonly bool BoolValue;

			// Token: 0x0400A142 RID: 41282
			public readonly string StringNeedle;

			// Token: 0x02001BA8 RID: 7080
			public enum EServerFilterType
			{
				// Token: 0x0400A144 RID: 41284
				Any,
				// Token: 0x0400A145 RID: 41285
				BoolValue,
				// Token: 0x0400A146 RID: 41286
				IntValue,
				// Token: 0x0400A147 RID: 41287
				IntNotValue,
				// Token: 0x0400A148 RID: 41288
				IntMin,
				// Token: 0x0400A149 RID: 41289
				IntMax,
				// Token: 0x0400A14A RID: 41290
				IntRange,
				// Token: 0x0400A14B RID: 41291
				StringValue,
				// Token: 0x0400A14C RID: 41292
				StringContains
			}
		}
	}
}
