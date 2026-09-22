using System;

namespace Platform
{
	// Token: 0x02001BA9 RID: 7081
	public interface ITextCensor
	{
		// Token: 0x0600D351 RID: 54097
		void Init(IPlatform _owner);

		// Token: 0x0600D352 RID: 54098
		void Update();

		// Token: 0x0600D353 RID: 54099
		void CensorProfanity(string _input, PlatformUserIdentifierAbs _author, Action<CensoredTextResult> _censoredCallback);
	}
}
