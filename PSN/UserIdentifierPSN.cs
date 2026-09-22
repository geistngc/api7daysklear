using System;
using UnityEngine.Scripting;

namespace Platform.PSN
{
	// Token: 0x02001CBE RID: 7358
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierPSN : PlatformUserIdentifierAbs
	{
		// Token: 0x17001B1E RID: 6942
		// (get) Token: 0x0600DA4B RID: 55883 RVA: 0x00080A2D File Offset: 0x0007EC2D
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.PSN;
			}
		}

		// Token: 0x17001B1F RID: 6943
		// (get) Token: 0x0600DA4C RID: 55884 RVA: 0x004E639F File Offset: 0x004E459F
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.PSN);

		// Token: 0x17001B20 RID: 6944
		// (get) Token: 0x0600DA4D RID: 55885 RVA: 0x004E63A7 File Offset: 0x004E45A7
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001B21 RID: 6945
		// (get) Token: 0x0600DA4E RID: 55886 RVA: 0x004E63AF File Offset: 0x004E45AF
		public override string CombinedString { get; }

		// Token: 0x17001B22 RID: 6946
		// (get) Token: 0x0600DA4F RID: 55887 RVA: 0x004E63B7 File Offset: 0x004E45B7
		// (set) Token: 0x0600DA50 RID: 55888 RVA: 0x004E63BF File Offset: 0x004E45BF
		public ulong AccountId { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600DA51 RID: 55889 RVA: 0x004E63C8 File Offset: 0x004E45C8
		public UserIdentifierPSN(ulong _accountId)
		{
			this.AccountId = _accountId;
			this.ReadablePlatformUserIdentifier = this.AccountId.ToString();
			this.CombinedString = this.PlatformIdentifierString + "_" + this.ReadablePlatformUserIdentifier;
		}

		// Token: 0x0600DA52 RID: 55890 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600DA53 RID: 55891 RVA: 0x004E641E File Offset: 0x004E461E
		public override bool Equals(PlatformUserIdentifierAbs _other)
		{
			return _other != null && (this == _other || (_other is UserIdentifierPSN && this.AccountId == (_other as UserIdentifierPSN).AccountId));
		}

		// Token: 0x0600DA54 RID: 55892 RVA: 0x004E6448 File Offset: 0x004E4648
		public override int GetHashCode()
		{
			return this.AccountId.GetHashCode();
		}
	}
}
