using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace MapRendering.Commands
{
	// Token: 0x02001AB9 RID: 6841
	[Preserve]
	public class EnableRendering : ConsoleCmdAbstract
	{
		// Token: 0x0600CE51 RID: 52817 RVA: 0x004B23D0 File Offset: 0x004B05D0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getDescription()
		{
			return "Disable live map rendering";
		}

		// Token: 0x0600CE52 RID: 52818 RVA: 0x004B23D7 File Offset: 0x004B05D7
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getHelp()
		{
			return "\n\t\t\t|Usage:\n\t\t\t|  1. enablerendering\n\t\t\t|  2. enablerendering <0/1>\n\t\t\t|1. Show current state of renderer\n\t\t\t|2. Disable/enable renderer\n\t\t\t|NOTE: This command can only turn the renderer off, it can not turn it on if it is not enabled in the serverconfig!\n\t\t\t".Unindent(true);
		}

		// Token: 0x0600CE53 RID: 52819 RVA: 0x004B23E4 File Offset: 0x004B05E4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string[] getCommands()
		{
			return new string[]
			{
				"enablerendering"
			};
		}

		// Token: 0x0600CE54 RID: 52820 RVA: 0x004B23F4 File Offset: 0x004B05F4
		public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
		{
			if (_params.Count != 1)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Current state: {0}{1}", MapRenderer.Enabled && MapRenderer.renderingEnabled, (!MapRenderer.Enabled) ? " (disabled by serverconfig!)" : ""));
				return;
			}
			MapRenderer.renderingEnabled = _params[0].Equals("1");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Set live map rendering to {0}", _params[0].Equals("1")));
		}
	}
}
