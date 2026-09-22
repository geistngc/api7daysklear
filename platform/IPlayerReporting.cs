using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001B95 RID: 7061
	public interface IPlayerReporting
	{
		// Token: 0x0600D30C RID: 54028
		void Init(IPlatform _owner);

		// Token: 0x0600D30D RID: 54029
		IList<IPlayerReporting.PlayerReportCategory> ReportCategories();

		// Token: 0x0600D30E RID: 54030
		void ReportPlayer(PlatformUserIdentifierAbs _reportedUserCross, IPlayerReporting.PlayerReportCategory _reportCategory, string _message, Action<bool> _reportCompleteCallback);

		// Token: 0x0600D30F RID: 54031
		IPlayerReporting.PlayerReportCategory GetPlayerReportCategoryMapping(EnumReportCategory _reportCategory);

		// Token: 0x02001B96 RID: 7062
		public abstract class PlayerReportCategory
		{
			// Token: 0x0600D310 RID: 54032
			public abstract override string ToString();

			// Token: 0x0600D311 RID: 54033 RVA: 0x0000640C File Offset: 0x0000460C
			[PublicizedFrom(EAccessModifier.Protected)]
			public PlayerReportCategory()
			{
			}
		}
	}
}
