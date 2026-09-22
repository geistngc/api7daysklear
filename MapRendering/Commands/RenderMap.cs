using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace MapRendering.Commands
{
	// Token: 0x02001ABA RID: 6842
	[Preserve]
	public class RenderMap : ConsoleCmdAbstract
	{
		// Token: 0x0600CE56 RID: 52822 RVA: 0x004B2486 File Offset: 0x004B0686
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getDescription()
		{
			return "render the current map to a file";
		}

		// Token: 0x0600CE57 RID: 52823 RVA: 0x004B248D File Offset: 0x004B068D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string[] getCommands()
		{
			return new string[]
			{
				"rendermap"
			};
		}

		// Token: 0x0600CE58 RID: 52824 RVA: 0x004B249D File Offset: 0x004B069D
		public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
		{
			if (!MapRenderer.HasInstance)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Renderer not enabled");
				return;
			}
			MapRenderer.Instance.RenderFullMap();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Render map done");
		}
	}
}
