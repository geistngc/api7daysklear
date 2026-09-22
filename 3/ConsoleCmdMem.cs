using System;
using System.Collections;
using System.Collections.Generic;
using SystemInformation;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000244 RID: 580
[Preserve]
public class ConsoleCmdMem : ConsoleCmdAbstract
{
	// Token: 0x06001160 RID: 4448 RVA: 0x0006E317 File Offset: 0x0006C517
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"mem"
		};
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06001161 RID: 4449 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0006E327 File Offset: 0x0006C527
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Prints memory information and unloads resources or changes garbage collector";
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0006E32E File Offset: 0x0006C52E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\nclean - cleanup memory pools\npools - list memory pools\ngc - show GC info\ngc alloc <value> - allocate k value of temp memory\ngc perm <value> - allocate k value of permanent memory\ngc clearperm - clear permanent allocations list\ngc c - run collection\ngc enable <value> - enable GC (0 or 1)\ngc inc <value> - run incremental collect for value ms\ngc inctime <value> - set incremental collect time in ms\nobj [mode] - list object pool (active or all)\nobjs - shrink object pool\nlog [interval] - start/stop logging of performance data to a file";
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0006E338 File Offset: 0x0006C538
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count >= 1 && _params[0] == "gc")
		{
			if (_params.Count == 1)
			{
				string text = GarbageCollector.GCMode.ToString();
				string line = string.Format("gc {0}, mem {1}k, count {2}, isInc {3}, inc time {4}ms", new object[]
				{
					text,
					GC.GetTotalMemory(false) / 1024L,
					GC.CollectionCount(0),
					GarbageCollector.isIncremental,
					GarbageCollector.incrementalTimeSliceNanoseconds / 1000000UL
				});
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(line);
			}
			if (_params.Count >= 2)
			{
				int num = 0;
				if (_params.Count >= 3)
				{
					int.TryParse(_params[2], out num);
				}
				string text2 = _params[1];
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num2 <= 2968531819U)
				{
					if (num2 <= 2833882183U)
					{
						if (num2 != 2041153668U)
						{
							if (num2 == 2833882183U)
							{
								if (text2 == "inc")
								{
									GarbageCollector.CollectIncremental((ulong)((long)num * 1000000L));
									return;
								}
							}
						}
						else if (text2 == "force")
						{
							GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
							return;
						}
					}
					else if (num2 != 2945169614U)
					{
						if (num2 == 2968531819U)
						{
							if (text2 == "perm")
							{
								ConsoleCmdMem.permanentAllocs.Add(new byte[num * 1024]);
								return;
							}
						}
					}
					else if (text2 == "enable")
					{
						GarbageCollector.GCMode = ((num == 0) ? GarbageCollector.Mode.Disabled : GarbageCollector.Mode.Enabled);
						return;
					}
				}
				else if (num2 <= 3132209942U)
				{
					if (num2 != 3002525270U)
					{
						if (num2 == 3132209942U)
						{
							if (text2 == "alloc")
							{
								new byte[num * 1024];
								return;
							}
						}
					}
					else if (text2 == "inctime")
					{
						GarbageCollector.incrementalTimeSliceNanoseconds = (ulong)((long)num * 1000000L);
						return;
					}
				}
				else if (num2 != 3859557458U)
				{
					if (num2 == 4057157156U)
					{
						if (text2 == "clearperm")
						{
							ConsoleCmdMem.permanentAllocs.Clear();
							return;
						}
					}
				}
				else if (text2 == "c")
				{
					GC.Collect();
					return;
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown gc command");
			}
			return;
		}
		if (_params.Count >= 1)
		{
			if (_params[0] == "obj")
			{
				GameObjectPool.Instance.CmdList((_params.Count >= 2) ? _params[1] : null);
				return;
			}
			if (_params[0] == "objs")
			{
				GameObjectPool.Instance.CmdShrink();
				return;
			}
			if (_params[0].EqualsCaseInsensitive("log"))
			{
				if (this.loggingEnabed)
				{
					this.loggingEnabed = false;
					return;
				}
				float interval = 2f;
				if (_params.Count > 1 && !StringParsers.TryParseFloat(_params[1], out interval))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Interval argument is not a valid float");
					return;
				}
				DateTime now = DateTime.Now;
				string text3 = GameIO.GetGamePath() + string.Format("/perflog_{0:0000}-{1:00}-{2:00}__{3:00}-{4:00}-{5:00}.txt", new object[]
				{
					now.Year,
					now.Month,
					now.Day,
					now.Hour,
					now.Minute,
					now.Second
				});
				this.loggingEnabed = true;
				ThreadManager.StartCoroutine(this.logCoroutine(text3, interval));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Started performance logging to " + text3);
				return;
			}
			else if (_params[0] == "prefab")
			{
				int num3;
				int num4;
				int num5;
				int num6;
				int num7;
				int num8;
				GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator().CalculateStats(out num3, out num4, out num5, out num6, out num7, out num8);
				Log.Out("\nBase Prefabs - Count: {0}, Memory: {1:F2} MB\nRotated Prefabs - Count: {2}, Memory: {3:F2} MB\nActive Prefabs - Count: {4}, Memory: {5:F2} MB\n", new object[]
				{
					num3,
					(double)num6 * 9.5367431640625E-07,
					num4,
					(double)num7 * 9.5367431640625E-07,
					num5,
					(double)num8 * 9.5367431640625E-07
				});
			}
		}
		Resources.UnloadUnusedAssets();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ConsoleCmdMem.GetStats(true, GameManager.Instance));
		World world = GameManager.Instance.World;
		if (world != null && world.m_ChunkManager.m_ObservedEntities.Count > 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Observers");
			List<ChunkManager.ChunkObserver> observedEntities = world.m_ChunkManager.m_ObservedEntities;
			for (int i = 0; i < observedEntities.Count; i++)
			{
				ChunkManager.ChunkObserver chunkObserver = observedEntities[i];
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" id=" + chunkObserver.id.ToString());
			}
		}
		if (_params.Count > 0 && _params[0] == "clear")
		{
			MemoryPools.Cleanup();
		}
		if (_params.Count > 0 && _params[0] == "pools")
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(MemoryPools.GetDebugInfo());
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(MemoryPools.GetDebugInfoEx());
		}
		if (_params.Count > 0 && _params[0] == "arraypools")
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(MemoryPools.GetDebugInfoArrays());
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0006E8F8 File Offset: 0x0006CAF8
	public static string GetStats(bool _bDoGc, GameManager _gm)
	{
		ConsoleCmdMem.FillStats(_bDoGc, _gm);
		World world = _gm.World;
		return string.Concat(new string[]
		{
			"Time: ",
			ConsoleCmdMem.Stats[0],
			"m FPS: ",
			ConsoleCmdMem.Stats[1],
			" Heap: ",
			ConsoleCmdMem.Stats[2],
			"MB Max: ",
			ConsoleCmdMem.Stats[3],
			"MB Chunks: ",
			ConsoleCmdMem.Stats[4],
			" CGO: ",
			ConsoleCmdMem.Stats[5],
			" Ply: ",
			ConsoleCmdMem.Stats[6],
			" Zom: ",
			ConsoleCmdMem.Stats[7],
			" Ent: ",
			ConsoleCmdMem.Stats[8],
			" (",
			ConsoleCmdMem.Stats[9],
			") Items: ",
			ConsoleCmdMem.Stats[10],
			" CO: ",
			((world != null) ? world.m_ChunkManager.m_ObservedEntities.Count : 0).ToString(),
			" RSS: ",
			ConsoleCmdMem.Stats[11],
			"MB"
		});
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0006EA40 File Offset: 0x0006CC40
	public static void FillStats(bool _bDoGc, GameManager _gm)
	{
		World world = _gm.World;
		long totalMemory = GC.GetTotalMemory(_bDoGc);
		ConsoleCmdMem.Stats[0] = (Time.timeSinceLevelLoad / 60f).ToCultureInvariantString("F2");
		ConsoleCmdMem.Stats[1] = _gm.fps.Counter.ToCultureInvariantString("F2");
		ConsoleCmdMem.Stats[2] = ((float)totalMemory / 1048576f).ToCultureInvariantString("0.0");
		ConsoleCmdMem.Stats[3] = ((float)GameManager.MaxMemoryConsumption / 1048576f).ToCultureInvariantString("0.0");
		ConsoleCmdMem.Stats[4] = Chunk.InstanceCount.ToString();
		ConsoleCmdMem.Stats[5] = ((world != null) ? world.m_ChunkManager.GetDisplayedChunkGameObjectsCount().ToString() : "");
		ConsoleCmdMem.Stats[6] = ((world != null) ? world.Players.list.Count.ToString() : "");
		ConsoleCmdMem.Stats[7] = GameStats.GetInt(EnumGameStats.EnemyCount).ToString();
		ConsoleCmdMem.Stats[8] = ((world != null) ? world.Entities.Count.ToString() : "");
		ConsoleCmdMem.Stats[9] = Entity.InstanceCount.ToString();
		ConsoleCmdMem.Stats[10] = EntityItem.ItemInstanceCount.ToString();
		ConsoleCmdMem.Stats[11] = ((float)GetRSS.GetCurrentRSS() / 1024f / 1024f).ToCultureInvariantString("0.0");
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0006EBA9 File Offset: 0x0006CDA9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator logCoroutine(string _filename, float _interval)
	{
		WaitForSeconds wait = new WaitForSeconds(_interval);
		while (this.loggingEnabed)
		{
			DateTime now = DateTime.Now;
			string contents = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00} 0.000 INF {6}\r\n", new object[]
			{
				now.Year,
				now.Month,
				now.Day,
				now.Hour,
				now.Minute,
				now.Second,
				ConsoleCmdMem.GetStats(false, GameManager.Instance)
			});
			SdFile.AppendAllText(_filename, contents);
			yield return wait;
		}
		Log.Out("Stopped performance logging");
		yield break;
	}

	// Token: 0x04000CE0 RID: 3296
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<byte[]> permanentAllocs = new List<byte[]>();

	// Token: 0x04000CE1 RID: 3297
	public static string[] Stats = new string[12];

	// Token: 0x04000CE2 RID: 3298
	[PublicizedFrom(EAccessModifier.Private)]
	public bool loggingEnabed;
}
