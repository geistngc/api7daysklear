using System;

namespace Platform
{
	// Token: 0x02001BC9 RID: 7113
	public interface IVirtualKeyboard
	{
		// Token: 0x0600D3C4 RID: 54212
		void Init(IPlatform _owner);

		// Token: 0x0600D3C5 RID: 54213
		string Open(string _title, string _defaultText, Action<bool, string> _onTextReceived, UIInput.InputType _mode = UIInput.InputType.Standard, bool _multiLine = false, uint singleLineLength = 200U);

		// Token: 0x0600D3C6 RID: 54214
		void Destroy();
	}
}
