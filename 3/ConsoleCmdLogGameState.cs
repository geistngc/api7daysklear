using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200023B RID: 571
[Preserve]
public class ConsoleCmdLogGameState : ConsoleCmdAbstract
{
	// Token: 0x06001125 RID: 4389 RVA: 0x0006CC19 File Offset: 0x0006AE19
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"loggamestate",
			"lgs"
		};
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06001126 RID: 4390 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0006CC31 File Offset: 0x0006AE31
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Log the current state of the game";
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0006CC38 File Offset: 0x0006AE38
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Writes information on the current state of the game (like memory usage,\nentities) to the log file. The section will use the message parameter\nin its header.\nUsage:\n   loggamestate <message> [true/false]\nMessage is a string that will be included in the header of the generated\nlog section. The optional boolean parameter specifies if this command\nshould be run on the client (true) instead of the server (false) which\nis the default.";
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0006CC40 File Offset: 0x0006AE40
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1 || _params.Count > 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1 or 2, found " + _params.Count.ToString() + ".");
			return;
		}
		string text = _params[0];
		bool flag = false;
		if (_params.Count == 2)
		{
			try
			{
				flag = ConsoleHelper.ParseParamBool(_params[1], false);
			}
			catch (ArgumentException)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid boolean value.");
				return;
			}
		}
		if (flag)
		{
			if (_senderInfo.RemoteClientInfo == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Second parameter may only be set to \"true\" if the command is executed from a game client.");
				return;
			}
			_senderInfo.RemoteClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("lgs \"" + text + "\"", true));
			return;
		}
		else
		{
			MicroStopwatch microStopwatch = new MicroStopwatch();
			this.WriteHeader(text);
			this.WriteGeneric();
			this.WriteCounts();
			this.WriteEntities();
			this.WritePlayers();
			this.WriteThreads();
			this.WriteUnityObjects();
			this.WriteGameObjects();
			this.WriteFooter(text);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Wrote game state to game log file, header includes \"{0}\", took {1} ms", text, microStopwatch.ElapsedMilliseconds));
		}
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0006CD80 File Offset: 0x0006AF80
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteGeneric()
	{
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements((GameManager.Instance.World != null) ? GameManager.Instance.World.worldTime : 0UL);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		int num = (int)Time.timeSinceLevelLoad;
		int num2 = num % 60;
		int num3 = num / 60 % 60;
		int num4 = num / 3600;
		this.newSection("Generic information");
		this.printLine("System time: {0}", new object[]
		{
			DateTime.Now.ToString("HH:mm:ss")
		});
		this.printLine("Game time:   Day {0}, {1:00}:{2:00}", new object[]
		{
			item,
			item2,
			item3
		});
		this.printLine("Game uptime: {0:00}:{1:00}:{2:00}", new object[]
		{
			num4,
			num3,
			num2
		});
		this.printLine("FPS:         {0}", new object[]
		{
			GameManager.Instance.fps.Counter.ToCultureInvariantString("F1")
		});
		this.printLine("Heap:        {0} MiB / max {1} MiB", new object[]
		{
			GC.GetTotalMemory(false) / 1024L / 1024L,
			GameManager.MaxMemoryConsumption / 1024L / 1024L
		});
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0006CEE4 File Offset: 0x0006B0E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteCounts()
	{
		this.newSection("Object counts");
		this.printLine("Active GameObjs:  {0}", new object[]
		{
			UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length
		});
		this.printLine("Script instances: {0}", new object[]
		{
			UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Length
		});
		this.printLine("Total Objects:    {0}", new object[]
		{
			UnityEngine.Object.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None).Length
		});
		if (GameManager.Instance.World != null)
		{
			this.printLine("Chunks:     {0}", new object[]
			{
				Chunk.InstanceCount
			});
			this.printLine("ChunkGOs:   {0}", new object[]
			{
				GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjectsCount()
			});
			this.printLine("Players:    {0}", new object[]
			{
				GameManager.Instance.World.Players.Count
			});
			this.printLine("Zombies:    {0}", new object[]
			{
				GameStats.GetInt(EnumGameStats.EnemyCount)
			});
			this.printLine("Entities:   {0} in world, {1} loaded", new object[]
			{
				GameManager.Instance.World.Entities.Count,
				Entity.InstanceCount
			});
			this.printLine("Items:      {0}", new object[]
			{
				EntityItem.ItemInstanceCount
			});
			this.printLine("ChunkObs:   {0}", new object[]
			{
				GameManager.Instance.World.m_ChunkManager.m_ObservedEntities.Count
			});
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0006D090 File Offset: 0x0006B290
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteEntities()
	{
		if (GameManager.Instance.World != null)
		{
			this.newSection("Entities");
			int num = 0;
			for (int i = GameManager.Instance.World.Entities.list.Count - 1; i >= 0; i--)
			{
				Entity entity = GameManager.Instance.World.Entities.list[i];
				this.printLine("{0,3}. id={1}, name={2}, pos={3}, lifetime={4}, remote={5}, dead={6}", new object[]
				{
					++num,
					entity.entityId,
					entity.ToString(),
					entity.GetPosition().ToCultureInvariantString(),
					entity.lifetime.ToCultureInvariantString("F1"),
					entity.isEntityRemote,
					entity.IsDead()
				});
			}
		}
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0006D174 File Offset: 0x0006B374
	[PublicizedFrom(EAccessModifier.Private)]
	public void WritePlayers()
	{
		if (GameManager.Instance.World != null)
		{
			this.newSection("Players");
			int num = 0;
			foreach (KeyValuePair<int, EntityPlayer> keyValuePair in GameManager.Instance.World.Players.dict)
			{
				string text = "<unknown>";
				PlatformUserIdentifierAbs platformUserIdentifierAbs = null;
				PlatformUserIdentifierAbs platformUserIdentifierAbs2 = null;
				ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(keyValuePair.Key);
				if (clientInfo != null)
				{
					text = clientInfo.ip;
					platformUserIdentifierAbs = clientInfo.PlatformId;
					platformUserIdentifierAbs2 = clientInfo.CrossplatformId;
				}
				this.printLine("{0,3}. id={1}, {2}, pos={3}, remote={4}, pltfmid={5}, crossid={6}, ip={7}, ping={8}", new object[]
				{
					++num,
					keyValuePair.Value.entityId,
					keyValuePair.Value.EntityName,
					keyValuePair.Value.position.ToCultureInvariantString(),
					keyValuePair.Value.isEntityRemote,
					((platformUserIdentifierAbs != null) ? platformUserIdentifierAbs.CombinedString : null) ?? "<unknown>",
					((platformUserIdentifierAbs2 != null) ? platformUserIdentifierAbs2.CombinedString : null) ?? "<unknown>",
					text,
					keyValuePair.Value.pingToServer
				});
			}
		}
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x0006D2F4 File Offset: 0x0006B4F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteThreads()
	{
		this.newSection("Threads");
		int num = 0;
		foreach (KeyValuePair<string, ThreadManager.ThreadInfo> keyValuePair in ThreadManager.ActiveThreads)
		{
			this.printLine("{0,3}. {1}", new object[]
			{
				++num,
				keyValuePair.Key
			});
		}
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0006D374 File Offset: 0x0006B574
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteUnityObjects()
	{
		SortedDictionary<string, int> sortedDictionary = new SortedDictionary<string, int>();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			string fullName = array[i].GetType().FullName;
			if (sortedDictionary.ContainsKey(fullName))
			{
				SortedDictionary<string, int> sortedDictionary2 = sortedDictionary;
				string key = fullName;
				int num = sortedDictionary2[key];
				sortedDictionary2[key] = num + 1;
			}
			else
			{
				sortedDictionary[fullName] = 1;
			}
		}
		this.newSection("Unity objects by type");
		foreach (KeyValuePair<string, int> keyValuePair in sortedDictionary)
		{
			this.printLine("{0,5} * {1}", new object[]
			{
				keyValuePair.Value,
				keyValuePair.Key
			});
		}
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0006D448 File Offset: 0x0006B648
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteGameObjects()
	{
		this.newSection("GameObjects");
		ConsoleCmdLogGameState.HierarchyElement hierarchyElement = new ConsoleCmdLogGameState.HierarchyElement("<top>", true);
		foreach (GameObject gameObject in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
		{
			if (gameObject.transform.parent == null)
			{
				hierarchyElement.childCount += this.TraverseScene(hierarchyElement, gameObject);
			}
		}
		this.PrintGameObjectHierarchy(hierarchyElement, 0, "");
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0006D4BC File Offset: 0x0006B6BC
	[PublicizedFrom(EAccessModifier.Private)]
	public int TraverseScene(ConsoleCmdLogGameState.HierarchyElement _parent, GameObject _go)
	{
		ConsoleCmdLogGameState.HierarchyElement hierarchyElement = new ConsoleCmdLogGameState.HierarchyElement(_go.name, _go.activeInHierarchy);
		_parent.children.Add(hierarchyElement);
		foreach (object obj in _go.transform)
		{
			Transform transform = (Transform)obj;
			hierarchyElement.childCount += this.TraverseScene(hierarchyElement, transform.gameObject);
		}
		return hierarchyElement.childCount + 1;
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0006D550 File Offset: 0x0006B750
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrintGameObjectHierarchy(ConsoleCmdLogGameState.HierarchyElement _he, int _indentation, string _prefix)
	{
		if (_he.active)
		{
			this.printLine("{0}{1} (children={2})", new object[]
			{
				_prefix,
				_he.name,
				_he.childCount
			});
			if (!this.prefixPerLevel.ContainsKey(_indentation + 1))
			{
				string value;
				if (_indentation == 0)
				{
					value = _prefix + ConsoleCmdLogGameState.indentationLastLevel;
				}
				else
				{
					value = ConsoleCmdLogGameState.indentationNormal + _prefix;
				}
				this.prefixPerLevel[_indentation + 1] = value;
			}
			for (int i = 0; i < _he.children.Count; i++)
			{
				ConsoleCmdLogGameState.HierarchyElement he = _he.children[i];
				this.PrintGameObjectHierarchy(he, _indentation + 1, this.prefixPerLevel[_indentation + 1]);
			}
		}
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0006D60B File Offset: 0x0006B80B
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteHeader(string _message)
	{
		Console.Out.WriteLine();
		Console.Out.WriteLine(ConsoleCmdLogGameState.startEndSep);
		Console.Out.WriteLine("WRITING GAME STATE: " + _message);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0006D63B File Offset: 0x0006B83B
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteFooter(string _message)
	{
		Console.Out.WriteLine();
		Console.Out.WriteLine("END OF GAME STATE: " + _message);
		Console.Out.WriteLine(ConsoleCmdLogGameState.startEndSep);
		Console.Out.WriteLine();
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0006D675 File Offset: 0x0006B875
	[PublicizedFrom(EAccessModifier.Private)]
	public void newSection(string _sectionName)
	{
		Console.Out.WriteLine();
		Console.Out.WriteLine(ConsoleCmdLogGameState.sectionSep);
		Console.Out.WriteLine(_sectionName + ":");
		Console.Out.WriteLine();
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0006D6AF File Offset: 0x0006B8AF
	[PublicizedFrom(EAccessModifier.Private)]
	public void print(string _format, params object[] _args)
	{
		Console.Out.Write(string.Format(_format, _args));
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0006D6C2 File Offset: 0x0006B8C2
	[PublicizedFrom(EAccessModifier.Private)]
	public void printLine(string _format, params object[] _args)
	{
		Console.Out.WriteLine(string.Format(_format, _args));
	}

	// Token: 0x04000CCE RID: 3278
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, string> prefixPerLevel = new Dictionary<int, string>();

	// Token: 0x04000CCF RID: 3279
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string indentationNormal = "   |  ";

	// Token: 0x04000CD0 RID: 3280
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string indentationLastLevel = "   +- ";

	// Token: 0x04000CD1 RID: 3281
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string startEndSep = new string('*', 100);

	// Token: 0x04000CD2 RID: 3282
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string sectionSep = new string('-', 50);

	// Token: 0x0200023C RID: 572
	[PublicizedFrom(EAccessModifier.Private)]
	public class HierarchyElement
	{
		// Token: 0x0600113A RID: 4410 RVA: 0x0006D71A File Offset: 0x0006B91A
		public HierarchyElement(string _name, bool _active)
		{
			this.name = _name;
			this.active = _active;
		}

		// Token: 0x04000CD3 RID: 3283
		public string name;

		// Token: 0x04000CD4 RID: 3284
		public bool active;

		// Token: 0x04000CD5 RID: 3285
		public int childCount;

		// Token: 0x04000CD6 RID: 3286
		public List<ConsoleCmdLogGameState.HierarchyElement> children = new List<ConsoleCmdLogGameState.HierarchyElement>();
	}
}
