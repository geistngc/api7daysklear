using System;
using UnityEngine.Scripting;

namespace Platform.Local
{
	// Token: 0x02001CD0 RID: 7376
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierLocal : PlatformUserIdentifierAbs
	{
		// Token: 0x17001B58 RID: 7000
		// (get) Token: 0x0600DAFF RID: 56063 RVA: 0x0002003D File Offset: 0x0001E23D
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.Local;
			}
		}

		// Token: 0x17001B59 RID: 7001
		// (get) Token: 0x0600DB00 RID: 56064 RVA: 0x004E767A File Offset: 0x004E587A
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.Local);

		// Token: 0x17001B5A RID: 7002
		// (get) Token: 0x0600DB01 RID: 56065 RVA: 0x004E7682 File Offset: 0x004E5882
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001B5B RID: 7003
		// (get) Token: 0x0600DB02 RID: 56066 RVA: 0x004E768A File Offset: 0x004E588A
		public override string CombinedString { get; }

		// Token: 0x0600DB03 RID: 56067 RVA: 0x004E7694 File Offset: 0x004E5894
		public UserIdentifierLocal(string _playername)
		{
			if (string.IsNullOrEmpty(_playername))
			{
				throw new ArgumentException("Playername must not be empty", "_playername");
			}
			this.PlayerName = _playername;
			this.ReadablePlatformUserIdentifier = _playername;
			this.CombinedString = this.PlatformIdentifierString + "_" + _playername;
			this.hashcode = (_playername.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600DB04 RID: 56068 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600DB05 RID: 56069 RVA: 0x004E770C File Offset: 0x004E590C
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
			UserIdentifierLocal userIdentifierLocal = _other as UserIdentifierLocal;
			return userIdentifierLocal != null && userIdentifierLocal.PlayerName == this.PlayerName;
		}

		// Token: 0x0600DB06 RID: 56070 RVA: 0x004E7741 File Offset: 0x004E5941
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400A5D9 RID: 42457
		public readonly string PlayerName;

		// Token: 0x0400A5DA RID: 42458
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
