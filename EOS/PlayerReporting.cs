using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Reports;

namespace Platform.EOS
{
	// Token: 0x02001D02 RID: 7426
	public class PlayerReporting : IPlayerReporting
	{
		// Token: 0x17001B72 RID: 7026
		// (get) Token: 0x0600DC41 RID: 56385 RVA: 0x004EEF24 File Offset: 0x004ED124
		public ReportsInterface reportsInterface
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((Api)this.owner.Api).PlatformInterface.GetReportsInterface();
			}
		}

		// Token: 0x17001B73 RID: 7027
		// (get) Token: 0x0600DC42 RID: 56386 RVA: 0x004EEF40 File Offset: 0x004ED140
		public ProductUserId localProductUserId
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
			}
		}

		// Token: 0x0600DC43 RID: 56387 RVA: 0x004EEF5C File Offset: 0x004ED15C
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600DC44 RID: 56388 RVA: 0x004EEF68 File Offset: 0x004ED168
		public IList<IPlayerReporting.PlayerReportCategory> ReportCategories()
		{
			if (this.reportCategories != null)
			{
				return this.reportCategories.list;
			}
			this.reportCategories = new DictionaryList<PlayerReportsCategory, IPlayerReporting.PlayerReportCategory>();
			foreach (PlayerReportsCategory playerReportsCategory in EnumUtils.Values<PlayerReportsCategory>())
			{
				if (playerReportsCategory != PlayerReportsCategory.Invalid)
				{
					this.reportCategories.Add(playerReportsCategory, new PlayerReporting.PlayerReportCategoryEos(playerReportsCategory, Localization.Get("xuiCategoryPlayerReport" + playerReportsCategory.ToStringCached<PlayerReportsCategory>(), false, null)));
				}
			}
			return this.reportCategories.list;
		}

		// Token: 0x0600DC45 RID: 56389 RVA: 0x004EF004 File Offset: 0x004ED204
		public void ReportPlayer(PlatformUserIdentifierAbs _reportedUserCross, IPlayerReporting.PlayerReportCategory _reportCategory, string _message, Action<bool> _reportCompleteCallback)
		{
			if (_message != null && _message.Length > 256)
			{
				Log.Out("[EOS-Report] Long message, might get truncated");
			}
			EosHelpers.AssertMainThread("PRep.Send");
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReporterUserId = this.localProductUserId,
				ReportedUserId = ((UserIdentifierEos)_reportedUserCross).ProductUserId,
				Category = ((PlayerReporting.PlayerReportCategoryEos)_reportCategory).Category,
				Message = _message
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.reportsInterface.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, delegate(ref SendPlayerBehaviorReportCompleteCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Report] Reporting player failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						_reportCompleteCallback(false);
						return;
					}
					Log.Out("[EOS-Report] Sent player report");
					_reportCompleteCallback(true);
				});
			}
		}

		// Token: 0x0600DC46 RID: 56390 RVA: 0x004EF0D4 File Offset: 0x004ED2D4
		public IPlayerReporting.PlayerReportCategory GetPlayerReportCategoryMapping(EnumReportCategory _reportCategory)
		{
			PlayerReportsCategory playerReportsCategory;
			if (_reportCategory != EnumReportCategory.Cheating)
			{
				if (_reportCategory != EnumReportCategory.VerbalAbuse)
				{
					playerReportsCategory = PlayerReportsCategory.Other;
				}
				else
				{
					playerReportsCategory = PlayerReportsCategory.VerbalAbuse;
				}
			}
			else
			{
				playerReportsCategory = PlayerReportsCategory.Cheating;
			}
			PlayerReportsCategory key = playerReportsCategory;
			IPlayerReporting.PlayerReportCategory result;
			if (!this.reportCategories.dict.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0400A6C7 RID: 42695
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A6C8 RID: 42696
		[PublicizedFrom(EAccessModifier.Private)]
		public DictionaryList<PlayerReportsCategory, IPlayerReporting.PlayerReportCategory> reportCategories;

		// Token: 0x02001D03 RID: 7427
		[PublicizedFrom(EAccessModifier.Private)]
		public class PlayerReportCategoryEos : IPlayerReporting.PlayerReportCategory
		{
			// Token: 0x0600DC48 RID: 56392 RVA: 0x004EF10F File Offset: 0x004ED30F
			public PlayerReportCategoryEos(PlayerReportsCategory _category, string _displayString)
			{
				this.Category = _category;
				this.displayString = _displayString;
			}

			// Token: 0x0600DC49 RID: 56393 RVA: 0x004EF125 File Offset: 0x004ED325
			public override string ToString()
			{
				return this.displayString;
			}

			// Token: 0x0400A6C9 RID: 42697
			public readonly PlayerReportsCategory Category;

			// Token: 0x0400A6CA RID: 42698
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly string displayString;
		}
	}
}
