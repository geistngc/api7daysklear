using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Webserver.FileCache
{
	// Token: 0x02001B0E RID: 6926
	[Preserve]
	public class InvalidateCachesCmd : ConsoleCmdAbstract
	{
		// Token: 0x0600D015 RID: 53269 RVA: 0x004BE734 File Offset: 0x004BC934
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string[] getCommands()
		{
			return new string[]
			{
				"invalidatecaches"
			};
		}

		// Token: 0x0600D016 RID: 53270 RVA: 0x004BE744 File Offset: 0x004BC944
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getDescription()
		{
			return "Invalidate contents of web file caches";
		}

		// Token: 0x0600D017 RID: 53271 RVA: 0x004BE74B File Offset: 0x004BC94B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string getHelp()
		{
			return "TODO";
		}

		// Token: 0x0600D018 RID: 53272 RVA: 0x004BE754 File Offset: 0x004BC954
		public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
		{
			ValueTuple<int, int> valueTuple = AbstractCache.InvalidateAllCaches();
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Caches invalidated, dropped {0} files with {1} Bytes", item, item2));
		}
	}
}
