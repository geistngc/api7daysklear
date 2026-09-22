using System;
using System.Collections.Generic;

namespace Platform.EOS
{
	// Token: 0x02001D11 RID: 7441
	[PublicizedFrom(EAccessModifier.Internal)]
	public struct SanctionsCheckResult
	{
		// Token: 0x17001B7E RID: 7038
		// (get) Token: 0x0600DC90 RID: 56464 RVA: 0x004F0BD6 File Offset: 0x004EEDD6
		public readonly DateTime LongestExpiry { get; }

		// Token: 0x17001B7F RID: 7039
		// (get) Token: 0x0600DC91 RID: 56465 RVA: 0x004F0BDE File Offset: 0x004EEDDE
		public readonly bool HasActiveSanctions { get; }

		// Token: 0x17001B80 RID: 7040
		// (get) Token: 0x0600DC92 RID: 56466 RVA: 0x004F0BE6 File Offset: 0x004EEDE6
		public readonly bool Success { get; }

		// Token: 0x0600DC93 RID: 56467 RVA: 0x004F0BF0 File Offset: 0x004EEDF0
		public SanctionsCheckResult(List<EOSSanction> sanctions)
		{
			this.Success = 1;
			this.LongestExpiry = DateTime.MinValue;
			this.ReasonForSanction = string.Empty;
			this.KickReason = default(GameUtils.KickPlayerData);
			if (sanctions == null || sanctions.Count == 0)
			{
				this.HasActiveSanctions = 0;
				return;
			}
			this.HasActiveSanctions = 1;
			EOSSanction eossanction = sanctions[0];
			foreach (EOSSanction eossanction2 in sanctions)
			{
				if (eossanction2.expiry == DateTime.MaxValue || eossanction2.expiry == default(DateTime))
				{
					Log.Out("[EOS] Sanctioned Until: Forever");
					this.ReasonForSanction = SanctionsCheckResult.GetReasonMessage(default(DateTime), GameUtils.EKickReason.CrossPlatformAuthenticationFailed, 9, "Sanction: [" + eossanction2.ReferenceId + "]");
					this.ReasonForSanction = string.Format(Localization.Get("auth_banned_forever", false, null), Array.Empty<object>());
					this.KickReason = default(GameUtils.KickPlayerData);
					this.LongestExpiry = DateTime.MaxValue;
					break;
				}
				if (eossanction2.expiry > eossanction.expiry)
				{
					eossanction = eossanction2;
				}
			}
			Log.Out("[EOS] Sanctioned Until: " + this.LongestExpiry.ToLongDateString());
			this.ReasonForSanction = SanctionsCheckResult.GetReasonMessage(eossanction.expiry, GameUtils.EKickReason.CrossPlatformAuthenticationFailed, 9, "Sanction: [" + eossanction.ReferenceId + "]");
			this.KickReason = new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossPlatformAuthenticationFailed, 9, eossanction.expiry, "Sanction: [" + eossanction.ReferenceId + "]");
			this.LongestExpiry = eossanction.expiry;
		}

		// Token: 0x0600DC94 RID: 56468 RVA: 0x004F0DB4 File Offset: 0x004EEFB4
		public SanctionsCheckResult(DateTime banUntil, GameUtils.EKickReason reason, int apiResponseEnum, string customReason)
		{
			this.KickReason = new GameUtils.KickPlayerData(reason, apiResponseEnum, banUntil, customReason);
			this.LongestExpiry = default(DateTime);
			this.HasActiveSanctions = 0;
			this.ReasonForSanction = SanctionsCheckResult.GetReasonMessage(banUntil, reason, apiResponseEnum, customReason);
			this.Success = 0;
		}

		// Token: 0x0600DC95 RID: 56469 RVA: 0x004F0DF0 File Offset: 0x004EEFF0
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetReasonMessage(DateTime banUntil, GameUtils.EKickReason reason, int apiResponseEnum, string customReason)
		{
			if (reason == GameUtils.EKickReason.Banned)
			{
				return SanctionsCheckResult.BannedMessage(banUntil, customReason);
			}
			if (reason == GameUtils.EKickReason.PlatformAuthenticationFailed)
			{
				return SanctionsCheckResult.GetAuthFailedMessage(banUntil, apiResponseEnum, customReason);
			}
			if (reason == GameUtils.EKickReason.CrossPlatformAuthenticationFailed)
			{
				return SanctionsCheckResult.GetAuthFailedMessage(banUntil, apiResponseEnum, customReason);
			}
			return Localization.Get("auth_unknown", false, null);
		}

		// Token: 0x0600DC96 RID: 56470 RVA: 0x004F0E28 File Offset: 0x004EF028
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetAuthFailedMessage(DateTime banUntil, int apiResponseEnum, string customReason)
		{
			switch (apiResponseEnum)
			{
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 7:
			case 8:
				return string.Format(Localization.Get("platformauth_" + ((EUserAuthenticationResult)apiResponseEnum).ToStringCached<EUserAuthenticationResult>(), false, null), PlatformManager.NativePlatform.PlatformDisplayName);
			case 5:
				return string.Format(Localization.Get("auth_timeout", false, null), PlatformManager.CrossplatformPlatform.PlatformDisplayName);
			case 9:
				return SanctionsCheckResult.BannedMessage(banUntil, customReason);
			default:
				return Localization.Get("auth_unknown", false, null);
			}
		}

		// Token: 0x0600DC97 RID: 56471 RVA: 0x004F0EBC File Offset: 0x004EF0BC
		[PublicizedFrom(EAccessModifier.Private)]
		public static string BannedMessage(DateTime banUntil, string customReason)
		{
			if (!(banUntil == default(DateTime)) && !(banUntil == DateTime.MaxValue))
			{
				return string.Format("\n" + Localization.Get("auth_sanctioned", false, null), banUntil.ToCultureInvariantString());
			}
			return Localization.Get("auth_sanctioned_forever", false, null);
		}

		// Token: 0x0400A708 RID: 42760
		public GameUtils.KickPlayerData KickReason;

		// Token: 0x0400A709 RID: 42761
		public string ReasonForSanction;
	}
}
