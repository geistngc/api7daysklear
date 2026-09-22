using System;
using Steamworks;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x02001CA7 RID: 7335
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierSteam : PlatformUserIdentifierAbs
	{
		// Token: 0x17001AFC RID: 6908
		// (get) Token: 0x0600D98A RID: 55690 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.Steam;
			}
		}

		// Token: 0x17001AFD RID: 6909
		// (get) Token: 0x0600D98B RID: 55691 RVA: 0x004E4674 File Offset: 0x004E2874
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.Steam);

		// Token: 0x17001AFE RID: 6910
		// (get) Token: 0x0600D98C RID: 55692 RVA: 0x004E467C File Offset: 0x004E287C
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001AFF RID: 6911
		// (get) Token: 0x0600D98D RID: 55693 RVA: 0x004E4684 File Offset: 0x004E2884
		public override string CombinedString { get; }

		// Token: 0x17001B00 RID: 6912
		// (get) Token: 0x0600D98E RID: 55694 RVA: 0x004E468C File Offset: 0x004E288C
		// (set) Token: 0x0600D98F RID: 55695 RVA: 0x004E4694 File Offset: 0x004E2894
		public byte[] Ticket
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

		// Token: 0x0600D990 RID: 55696 RVA: 0x004E46A0 File Offset: 0x004E28A0
		public UserIdentifierSteam(string _steamId)
		{
			ulong steamId;
			if (_steamId.Length != 17 || !ulong.TryParse(_steamId, out steamId))
			{
				throw new ArgumentException("Not a valid SteamID: " + _steamId, "_steamId");
			}
			this.SteamId = steamId;
			this.ReadablePlatformUserIdentifier = _steamId;
			this.CombinedString = this.PlatformIdentifierString + "_" + _steamId;
			this.hashcode = (steamId.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600D991 RID: 55697 RVA: 0x004E4728 File Offset: 0x004E2928
		public UserIdentifierSteam(ulong _steamId)
		{
			if (_steamId < 10000000000000000UL || _steamId > 99999999999999999UL)
			{
				throw new ArgumentException("Not a valid SteamID: " + _steamId.ToString(), "_steamId");
			}
			this.SteamId = _steamId;
			this.ReadablePlatformUserIdentifier = _steamId.ToString();
			this.CombinedString = this.PlatformIdentifierString + "_" + _steamId.ToString();
			this.hashcode = (_steamId.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600D992 RID: 55698 RVA: 0x004E47C8 File Offset: 0x004E29C8
		public UserIdentifierSteam(CSteamID _steamId)
		{
			this.SteamId = _steamId.m_SteamID;
			this.ReadablePlatformUserIdentifier = _steamId.ToString();
			string platformIdentifierString = this.PlatformIdentifierString;
			string str = "_";
			CSteamID csteamID = _steamId;
			this.CombinedString = platformIdentifierString + str + csteamID.ToString();
			this.hashcode = (_steamId.m_SteamID.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600D993 RID: 55699 RVA: 0x004E484C File Offset: 0x004E2A4C
		public override bool DecodeTicket(string _ticket)
		{
			if (string.IsNullOrEmpty(_ticket))
			{
				return false;
			}
			try
			{
				this.Ticket = Convert.FromBase64String(_ticket);
			}
			catch (FormatException ex)
			{
				Log.Error("Convert.FromBase64String: " + ex.Message);
				Log.Exception(ex);
				return false;
			}
			return true;
		}

		// Token: 0x0600D994 RID: 55700 RVA: 0x004E48A8 File Offset: 0x004E2AA8
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
			UserIdentifierSteam userIdentifierSteam = _other as UserIdentifierSteam;
			return userIdentifierSteam != null && userIdentifierSteam.SteamId == this.SteamId;
		}

		// Token: 0x0600D995 RID: 55701 RVA: 0x004E48DA File Offset: 0x004E2ADA
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400A551 RID: 42321
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public byte[] ticket;

		// Token: 0x0400A552 RID: 42322
		public UserIdentifierSteam OwnerId;

		// Token: 0x0400A553 RID: 42323
		public readonly ulong SteamId;

		// Token: 0x0400A554 RID: 42324
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
