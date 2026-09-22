using System;
using InControl;

namespace Platform
{
	// Token: 0x02001BC8 RID: 7112
	public interface IUtils
	{
		// Token: 0x0600D3B9 RID: 54201
		void Init(IPlatform _owner);

		// Token: 0x0600D3BA RID: 54202
		bool OpenBrowser(string _url);

		// Token: 0x0600D3BB RID: 54203
		void ControllerDisconnected(InputDevice inputDevice);

		// Token: 0x0600D3BC RID: 54204
		string GetPlatformLanguage();

		// Token: 0x0600D3BD RID: 54205
		string GetAppLanguage();

		// Token: 0x0600D3BE RID: 54206
		string GetCountry();

		// Token: 0x0600D3BF RID: 54207
		string GetStoreBranchName();

		// Token: 0x0600D3C0 RID: 54208
		void ClearTempFiles();

		// Token: 0x0600D3C1 RID: 54209
		string GetTempFileName(string prefix = "", string suffix = "");

		// Token: 0x0600D3C2 RID: 54210
		bool? IsFamilyShare();

		// Token: 0x0600D3C3 RID: 54211
		string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform = EPlatformIdentifier.None);
	}
}
