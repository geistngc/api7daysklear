using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Webserver.Commands
{
	// Token: 0x02001B10 RID: 6928
	[Preserve]
	public class EnableOpenIDDebug : ConsoleCmdAbstract
	{
		// Token: 0x0600D01D RID: 53277 RVA: 0x004BE8FB File Offset: 0x004BCAFB
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getDescription()
		{
			return "enable/disable OpenID debugging";
		}

		// Token: 0x0600D01E RID: 53278 RVA: 0x004BE902 File Offset: 0x004BCB02
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string[] getCommands()
		{
			return new string[]
			{
				"openiddebug"
			};
		}

		// Token: 0x0600D01F RID: 53279 RVA: 0x004BE914 File Offset: 0x004BCB14
		public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
		{
			if (_params.Count != 1)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Current state: {0}", OpenID.debugOpenId));
				return;
			}
			OpenID.debugOpenId = _params[0].Equals("1");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Set OpenID debugging to {0}", _params[0].Equals("1")));
		}
	}
}
