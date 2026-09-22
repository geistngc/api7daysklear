using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001B6B RID: 7019
	public interface IJoinSessionGameInviteListener
	{
		// Token: 0x0600D217 RID: 53783
		void Init(IPlatform _owner);

		// Token: 0x0600D218 RID: 53784
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		ValueTuple<string, string> TakePendingInvite();

		// Token: 0x0600D219 RID: 53785
		IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null);

		// Token: 0x0600D21A RID: 53786
		string GetListenerIdentifier();
	}
}
