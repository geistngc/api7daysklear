using System;
using UnityEngine.Scripting;

namespace Platform.XBL
{
	// Token: 0x02001C16 RID: 7190
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierXbl : PlatformUserIdentifierAbs
	{
		// Token: 0x17001A80 RID: 6784
		// (get) Token: 0x0600D5A5 RID: 54693 RVA: 0x00080864 File Offset: 0x0007EA64
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.XBL;
			}
		}

		// Token: 0x17001A81 RID: 6785
		// (get) Token: 0x0600D5A6 RID: 54694 RVA: 0x004D2574 File Offset: 0x004D0774
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.XBL);

		// Token: 0x17001A82 RID: 6786
		// (get) Token: 0x0600D5A7 RID: 54695 RVA: 0x004D257C File Offset: 0x004D077C
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001A83 RID: 6787
		// (get) Token: 0x0600D5A8 RID: 54696 RVA: 0x004D2584 File Offset: 0x004D0784
		public override string CombinedString { get; }

		// Token: 0x17001A84 RID: 6788
		// (get) Token: 0x0600D5A9 RID: 54697 RVA: 0x004D258C File Offset: 0x004D078C
		public ulong Xuid
		{
			get
			{
				return XblXuidMapper.GetXuid(this);
			}
		}

		// Token: 0x0600D5AA RID: 54698 RVA: 0x004D2594 File Offset: 0x004D0794
		public UserIdentifierXbl(string _pxuid)
		{
			this.pxuid = _pxuid;
			this.ReadablePlatformUserIdentifier = _pxuid;
			this.CombinedString = this.PlatformIdentifierString + "_" + _pxuid;
			this.hashcode = (_pxuid.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600D5AB RID: 54699 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600D5AC RID: 54700 RVA: 0x004D25F4 File Offset: 0x004D07F4
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
			UserIdentifierXbl userIdentifierXbl = _other as UserIdentifierXbl;
			return userIdentifierXbl != null && string.Equals(userIdentifierXbl.pxuid, this.pxuid, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600D5AD RID: 54701 RVA: 0x004D262A File Offset: 0x004D082A
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400A2BC RID: 41660
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string pxuid;

		// Token: 0x0400A2BD RID: 41661
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
