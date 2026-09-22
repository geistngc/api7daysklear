using System;
using System.Collections;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001C1B RID: 7195
	public class PrivilegeState
	{
		// Token: 0x0600D5C0 RID: 54720 RVA: 0x004D29AD File Offset: 0x004D0BAD
		public PrivilegeState(XUserHandle userHandle, XUserPrivilege privilege)
		{
			this.m_userHandle = userHandle;
			this.m_privilege = privilege;
			this.m_has = false;
			this.m_denyReason = (XUserPrivilegeDenyReason)4294967295U;
		}

		// Token: 0x17001A89 RID: 6793
		// (get) Token: 0x0600D5C1 RID: 54721 RVA: 0x004D29D1 File Offset: 0x004D0BD1
		public bool Has
		{
			get
			{
				return this.m_has;
			}
		}

		// Token: 0x17001A8A RID: 6794
		// (get) Token: 0x0600D5C2 RID: 54722 RVA: 0x004D29D9 File Offset: 0x004D0BD9
		public XUserPrivilegeDenyReason DenyReason
		{
			get
			{
				return this.m_denyReason;
			}
		}

		// Token: 0x0600D5C3 RID: 54723 RVA: 0x004D29E4 File Offset: 0x004D0BE4
		public void ResolveSilent()
		{
			int hr = SDK.XUserCheckPrivilege(this.m_userHandle, XUserPrivilegeOptions.None, this.m_privilege, out this.m_has, out this.m_denyReason);
			XblHelpers.LogHR(hr, string.Format("{0} checked privilege '{1}' = {2} ({3})", new object[]
			{
				"XUserCheckPrivilege",
				this.m_privilege,
				this.m_has,
				this.m_denyReason
			}), false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				return;
			}
			this.m_has = false;
			this.m_denyReason = (XUserPrivilegeDenyReason)4294967295U;
		}

		// Token: 0x0600D5C4 RID: 54724 RVA: 0x004D2A70 File Offset: 0x004D0C70
		public IEnumerator ResolveWithPrompt(CoroutineCancellationToken _cancellationToken = null)
		{
			this.ResolveSilent();
			if (this.m_has)
			{
				yield break;
			}
			bool uiOpen = true;
			SDK.XUserResolvePrivilegeWithUiAsync(this.m_userHandle, XUserPrivilegeOptions.None, this.m_privilege, delegate(int hr)
			{
				CoroutineCancellationToken cancellationToken2 = _cancellationToken;
				if (cancellationToken2 != null && cancellationToken2.IsCancelled())
				{
					return;
				}
				try
				{
					XblHelpers.LogHR(hr, "XUserResolvePrivilegeWithUiCompleted", false);
				}
				finally
				{
					uiOpen = false;
				}
			});
			while (uiOpen)
			{
				CoroutineCancellationToken cancellationToken = _cancellationToken;
				if (cancellationToken != null && cancellationToken.IsCancelled())
				{
					yield break;
				}
				yield return null;
			}
			this.ResolveSilent();
			yield break;
		}

		// Token: 0x0400A2D2 RID: 41682
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUserHandle m_userHandle;

		// Token: 0x0400A2D3 RID: 41683
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUserPrivilege m_privilege;

		// Token: 0x0400A2D4 RID: 41684
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_has;

		// Token: 0x0400A2D5 RID: 41685
		[PublicizedFrom(EAccessModifier.Private)]
		public XUserPrivilegeDenyReason m_denyReason;
	}
}
