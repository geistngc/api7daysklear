using System;
using System.Text.RegularExpressions;
using Epic.OnlineServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Platform.EOS
{
	// Token: 0x02001D2E RID: 7470
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierEos : PlatformUserIdentifierAbs
	{
		// Token: 0x17001B94 RID: 7060
		// (get) Token: 0x0600DD45 RID: 56645 RVA: 0x0002F184 File Offset: 0x0002D384
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.EOS;
			}
		}

		// Token: 0x17001B95 RID: 7061
		// (get) Token: 0x0600DD46 RID: 56646 RVA: 0x004F5799 File Offset: 0x004F3999
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS);

		// Token: 0x17001B96 RID: 7062
		// (get) Token: 0x0600DD47 RID: 56647 RVA: 0x004F57A1 File Offset: 0x004F39A1
		public override string ReadablePlatformUserIdentifier
		{
			get
			{
				return this.ProductUserIdString;
			}
		}

		// Token: 0x17001B97 RID: 7063
		// (get) Token: 0x0600DD48 RID: 56648 RVA: 0x004F57A9 File Offset: 0x004F39A9
		public override string CombinedString { get; }

		// Token: 0x0600DD49 RID: 56649 RVA: 0x004F57B1 File Offset: 0x004F39B1
		public static string CreateCombinedString(string _puidString)
		{
			return PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS) + "_" + _puidString;
		}

		// Token: 0x0600DD4A RID: 56650 RVA: 0x004F57C4 File Offset: 0x004F39C4
		public static string CreateCombinedString(ProductUserId _puid)
		{
			return PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS) + "_" + UserIdentifierEos.CreateStringFromPuid(_puid);
		}

		// Token: 0x17001B98 RID: 7064
		// (get) Token: 0x0600DD4B RID: 56651 RVA: 0x004F57DC File Offset: 0x004F39DC
		public string ProductUserIdString
		{
			get
			{
				string result;
				if ((result = this.productUserIdString) == null)
				{
					result = (this.productUserIdString = UserIdentifierEos.CreateStringFromPuid(this.productUserId));
				}
				return result;
			}
		}

		// Token: 0x17001B99 RID: 7065
		// (get) Token: 0x0600DD4C RID: 56652 RVA: 0x004F5808 File Offset: 0x004F3A08
		public ProductUserId ProductUserId
		{
			get
			{
				ProductUserId result;
				if ((result = this.productUserId) == null)
				{
					result = (this.productUserId = UserIdentifierEos.CreatePuidFromString(this.productUserIdString));
				}
				return result;
			}
		}

		// Token: 0x17001B9A RID: 7066
		// (get) Token: 0x0600DD4D RID: 56653 RVA: 0x004F5833 File Offset: 0x004F3A33
		// (set) Token: 0x0600DD4E RID: 56654 RVA: 0x004F583B File Offset: 0x004F3A3B
		public string Ticket
		{
			get
			{
				return this.ticket;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this.ticket = value;
			}
		}

		// Token: 0x0600DD4F RID: 56655 RVA: 0x004F5844 File Offset: 0x004F3A44
		public UserIdentifierEos(string _puid)
		{
			if (string.IsNullOrEmpty(_puid))
			{
				throw new ArgumentException("Empty or null PUID", "_puid");
			}
			if (!UserIdentifierEos.puidMatcher.IsMatch(_puid))
			{
				throw new ArgumentException("Invalid PUID '" + _puid + "'", "_puid");
			}
			this.productUserIdString = _puid;
			this.CombinedString = UserIdentifierEos.CreateCombinedString(_puid);
			this.hashcode = (this.ProductUserIdString.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600DD50 RID: 56656 RVA: 0x004F58D4 File Offset: 0x004F3AD4
		public UserIdentifierEos(ProductUserId _puid)
		{
			if (_puid == null)
			{
				throw new ArgumentException("Null PUID", "_puid");
			}
			this.productUserId = _puid;
			this.CombinedString = UserIdentifierEos.CreateCombinedString(this.ProductUserIdString);
			this.hashcode = (this.ProductUserIdString.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600DD51 RID: 56657 RVA: 0x004F5944 File Offset: 0x004F3B44
		public static string CreateStringFromPuid(ProductUserId _puid)
		{
			if (!ThreadManager.IsMainThread())
			{
				Log.Warning("CreateStringFromPuid NOT ON MAIN THREAD! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
			}
			if (_puid == null)
			{
				Log.Error("CreateStringFromPuid with null PUID! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
				return null;
			}
			return _puid.ToString();
		}

		// Token: 0x0600DD52 RID: 56658 RVA: 0x004F599B File Offset: 0x004F3B9B
		public static ProductUserId CreatePuidFromString(string _puidString)
		{
			if (_puidString == null)
			{
				Log.Error("CreatePuidFromString with null PUID string! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
				return null;
			}
			return ProductUserId.FromString(_puidString);
		}

		// Token: 0x0600DD53 RID: 56659 RVA: 0x004F59C6 File Offset: 0x004F3BC6
		public override bool DecodeTicket(string _ticket)
		{
			if (string.IsNullOrEmpty(_ticket))
			{
				return false;
			}
			this.Ticket = _ticket;
			return true;
		}

		// Token: 0x0600DD54 RID: 56660 RVA: 0x004F59DC File Offset: 0x004F3BDC
		public override bool Equals(PlatformUserIdentifierAbs _other)
		{
			if (_other == null)
			{
				return false;
			}
			if (this == _other)
			{
				return true;
			}
			UserIdentifierEos userIdentifierEos = _other as UserIdentifierEos;
			return userIdentifierEos != null && string.Equals(userIdentifierEos.ProductUserIdString, this.ProductUserIdString, StringComparison.Ordinal);
		}

		// Token: 0x0600DD55 RID: 56661 RVA: 0x004F5A12 File Offset: 0x004F3C12
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400A778 RID: 42872
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex puidMatcher = new Regex("^[0-9a-fA-F]{8,32}$", RegexOptions.Compiled);

		// Token: 0x0400A77B RID: 42875
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public string ticket;

		// Token: 0x0400A77C RID: 42876
		[PublicizedFrom(EAccessModifier.Private)]
		public string productUserIdString;

		// Token: 0x0400A77D RID: 42877
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public ProductUserId productUserId;

		// Token: 0x0400A77E RID: 42878
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
