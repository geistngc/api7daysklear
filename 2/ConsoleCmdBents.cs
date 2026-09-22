using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001EC RID: 492
[Preserve]
public class ConsoleCmdBents : ConsoleCmdAbstract
{
	// Token: 0x06000F31 RID: 3889 RVA: 0x000631F1 File Offset: 0x000613F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"bents"
		};
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x00063201 File Offset: 0x00061401
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Switches block entities on/off or counts them";
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x00063208 File Offset: 0x00061408
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Use 'on' or 'off', or 'info' to count";
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x00063210 File Offset: 0x00061410
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Specify 'on', 'off' or 'info'");
			return;
		}
		if (_params[0] == "on")
		{
			this.setAll(true, null);
			return;
		}
		if (_params[0] == "off")
		{
			this.setAll(false, null);
			return;
		}
		if (_params[0] == "info")
		{
			this.bentsPerName.Clear();
			int num = this.countAll(this.bentsPerName, null);
			int num2 = 1;
			foreach (KeyValuePair<string, int> keyValuePair in this.bentsPerName)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format(" {0,3}. {1} = {2}", num2, keyValuePair.Key, keyValuePair.Value));
				num2++;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Total: " + num.ToString());
			return;
		}
		if (_params[0] == "cullon")
		{
			int num3 = 0;
			MicroStopwatch microStopwatch = new MicroStopwatch();
			foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArray())
			{
				num3 += chunk.EnableInsideBlockEntities(true);
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Setting " + num3.ToString() + " to ON took " + microStopwatch.ElapsedMilliseconds.ToString());
			return;
		}
		if (_params[0] == "culloff")
		{
			int num4 = 0;
			MicroStopwatch microStopwatch2 = new MicroStopwatch();
			foreach (Chunk chunk2 in GameManager.Instance.World.ChunkCache.GetChunkArray())
			{
				num4 += chunk2.EnableInsideBlockEntities(false);
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Setting " + num4.ToString() + " to OFF took " + microStopwatch2.ElapsedMilliseconds.ToString());
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown parameter");
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0006348C File Offset: 0x0006168C
	[PublicizedFrom(EAccessModifier.Private)]
	public void setAll(bool _bOn, Transform _t = null)
	{
		if (_t == null)
		{
			foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.m_ChunkManager.GetUsedChunkGameObjects())
			{
				this.setAll(_bOn, chunkGameObject.transform);
			}
			return;
		}
		for (int i = 0; i < _t.childCount; i++)
		{
			Transform child = _t.GetChild(i);
			if (child.name == "_BlockEntities")
			{
				child.gameObject.SetActive(_bOn);
			}
			else
			{
				this.setAll(_bOn, child);
			}
		}
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x00063540 File Offset: 0x00061740
	[PublicizedFrom(EAccessModifier.Private)]
	public int countAll(Dictionary<string, int> _bentsPerName, Transform _t = null)
	{
		int num = 0;
		if (_t == null)
		{
			foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.m_ChunkManager.GetUsedChunkGameObjects())
			{
				num += this.countAll(_bentsPerName, chunkGameObject.transform);
			}
			return num;
		}
		for (int i = 0; i < _t.childCount; i++)
		{
			Transform child = _t.GetChild(i);
			if (child.name == "_BlockEntities")
			{
				num += child.childCount;
				for (int j = 0; j < child.childCount; j++)
				{
					string name = child.GetChild(j).name;
					int num2;
					_bentsPerName[name] = (_bentsPerName.TryGetValue(name, out num2) ? (num2 + 1) : 1);
				}
			}
			else
			{
				num += this.countAll(_bentsPerName, child);
			}
		}
		return num;
	}

	// Token: 0x04000C8F RID: 3215
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, int> bentsPerName = new Dictionary<string, int>();
}
