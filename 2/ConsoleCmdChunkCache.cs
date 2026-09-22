using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x020001F8 RID: 504
[Preserve]
public class ConsoleCmdChunkCache : ConsoleCmdAbstract
{
	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x000645DB File Offset: 0x000627DB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"chunkcache",
			"cc"
		};
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000F7B RID: 3963 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x000645F4 File Offset: 0x000627F4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int[] array = new int[1];
		ReaderWriterLockSlim syncRoot = GameManager.Instance.World.ChunkCache.GetSyncRoot();
		lock (syncRoot)
		{
			foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArray())
			{
				int usedMem = chunk.GetUsedMem();
				int[] array2;
				chunk.GetTextureChannelMemory(out array2);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] += array2[i];
					num5 += array2[i];
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					num++.ToString(),
					". ",
					chunk.X.ToString(),
					", ",
					chunk.Z.ToString(),
					"  M=",
					(usedMem / 1024).ToString(),
					"k",
					chunk.IsDisplayed ? "D" : ""
				}));
				num2 += (chunk.IsDisplayed ? 1 : 0);
				num3 += usedMem;
				num4 += chunk.MeshLayerCount;
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Chunks: " + GameManager.Instance.World.ChunkCache.Count().ToString());
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Chunk Memory: " + (num3 / 1048576).ToString() + "MB");
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Texture Memory Total: {0:F2}MB", (float)num5 / 1048576f));
		for (int j = 0; j < array.Length; j++)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Texture Memory {0}: {1:F2}MB", j, (float)array[j] / 1048576f));
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Displayed: " + num2.ToString());
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("VML: " + num4.ToString());
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x000648AC File Offset: 0x00062AAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "shows all loaded chunks in cache";
	}
}
