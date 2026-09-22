using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000266 RID: 614
[Preserve]
public class ConsoleCmdPrefab : ConsoleCmdAbstract
{
	// Token: 0x06001238 RID: 4664 RVA: 0x00071EFF File Offset: 0x000700FF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"prefab"
		};
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00071F0F File Offset: 0x0007010F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Prefab commands";
	}

	// Token: 0x0600123A RID: 4666 RVA: 0x00071F18 File Offset: 0x00070118
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return string.Concat(new string[]
		{
			"\r\n\t\t\t|Usage:\r\n\t\t\t|  1. ",
			this.PrimaryCommand,
			" load [prefab name]\r\n            |      Load given prefab. If no prefab is given show prefab explorer window.\r\n\t\t\t|  2. ",
			this.PrimaryCommand,
			" save\r\n            |      Save current prefab.\r\n\t\t\t|  3. ",
			this.PrimaryCommand,
			" clear\r\n            |      Clear prefab editor contents.\r\n\t\t\t|  4. ",
			this.PrimaryCommand,
			" thumbnail [name]\r\n            |      Update prefab thumbnail of given prefab. If no prefab is given applies to the loaded prefab.\r\n\t\t\t|  5. ",
			this.PrimaryCommand,
			" thumbnail bulk\r\n            |      Create thumbnails for all prefabs that do not have one yet.\r\n\t\t\t|  6. ",
			this.PrimaryCommand,
			" stats\r\n            |      Create CSV file with basic stats of all prefabs.\r\n\t\t\t|  7. ",
			this.PrimaryCommand,
			" convert <name>\r\n            |      Load, simplify, combine and export named prefab.\r\n\t\t\t|  8. ",
			this.PrimaryCommand,
			" bulk [count]\r\n            |      Do \"convert\" on all prefabs (optionally limited to first \"count\" prefabs).\r\n\t\t\t|  9. ",
			this.PrimaryCommand,
			" bulkins\r\n            |      Update inside information (.ins) for all prefabs.\r\n\t\t\t|  10. ",
			this.PrimaryCommand,
			" density <needle> <set>\r\n            |      Change density of all non-air blocks in current pefab that have <needle> density to <set>.\r\n\t\t\t|  11. ",
			this.PrimaryCommand,
			" simplify\r\n            |      Simplify current prefab for imposter generation.\r\n\t\t\t|  12. ",
			this.PrimaryCommand,
			" simplify1\r\n            |      Simplify current prefab for imposter generation, keep more complex blocks.\r\n\t\t\t|  13. ",
			this.PrimaryCommand,
			" combine\r\n            |      Combine meshes of current prefab into one.\r\n\t\t\t|  14. ",
			this.PrimaryCommand,
			" export\r\n            |      Export combined meshes as imposter for current prefab.\r\n\t\t\t"
		}).Unindent(true);
	}

	// Token: 0x0600123B RID: 4667 RVA: 0x00072044 File Offset: 0x00070244
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GameManager.Instance.IsEditMode())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command has to be run while in Prefab Editor!");
			return;
		}
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0];
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num > 1647949432U)
		{
			if (num <= 2000286253U)
			{
				if (num != 1781603564U)
				{
					if (num != 1924728219U)
					{
						if (num != 2000286253U)
						{
							return;
						}
						if (!(text == "bulk"))
						{
							return;
						}
						ConsoleCmdPrefab.prefabsToConvert.Clear();
						ConsoleCmdPrefab.prefabsToConvert.AddRange(PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, false, null));
						ConsoleCmdPrefab.processedCount = 0;
						ConsoleCmdPrefab.processedSW.ResetAndRestart();
						ConsoleCmdPrefab.stopCount = int.MaxValue;
						if (_params.Count >= 2)
						{
							ConsoleCmdPrefab.stopCount = int.Parse(_params[1]);
						}
						this.convertBulk();
						return;
					}
					else
					{
						if (!(text == "density"))
						{
							return;
						}
						if (_params.Count < 3)
						{
							SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Two arguments expected: Original density and replace value");
							return;
						}
						int densityMatch = int.Parse(_params[1]);
						int densitySet = int.Parse(_params[2]);
						PrefabHelpers.DensityChange(densityMatch, densitySet);
						return;
					}
				}
				else
				{
					if (!(text == "stats"))
					{
						return;
					}
					SdFile.WriteAllText(ConsoleCmdPrefab.PrefabStatsFilename, "Prefab,TotalVerts,TotalTris,LightsVolumePortion,SizeX,SizeY,SizeZ,Volume,LightsVolume\n", Encoding.UTF8);
					PrefabHelpers.IteratePrefabs(true, null, new Action<PathAbstractions.AbstractedLocation, Prefab>(ConsoleCmdPrefab.getPrefabStats), null, null, delegate
					{
						Log.Out("Prefab stats written to: " + ConsoleCmdPrefab.PrefabStatsFilename);
					});
				}
			}
			else if (num <= 3859241449U)
			{
				if (num != 3439296072U)
				{
					if (num != 3859241449U)
					{
						return;
					}
					if (!(text == "load"))
					{
						return;
					}
					if (_params.Count < 2)
					{
						LocalPlayerUI.GetUIForPlayer(GameManager.Instance.World.GetLocalPlayers()[0]).windowManager.Open(XUiC_PrefabList.ID, true);
						return;
					}
					PrefabEditModeManager.Instance.LoadVoxelPrefab(PathAbstractions.PrefabsSearchPaths.GetLocation(_params[1], null, null), false, false);
					return;
				}
				else
				{
					if (!(text == "save"))
					{
						return;
					}
					if (PrefabEditModeManager.Instance.VoxelPrefab == null)
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No prefab loaded");
						return;
					}
					PrefabEditModeManager.Instance.SaveVoxelPrefab();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Saved prefab {0} with size {1}", PrefabEditModeManager.Instance.VoxelPrefab.location, PrefabEditModeManager.Instance.VoxelPrefab.size));
					return;
				}
			}
			else if (num != 4122430407U)
			{
				if (num != 4211608755U)
				{
					return;
				}
				if (!(text == "export"))
				{
					return;
				}
				if (PrefabEditModeManager.Instance.VoxelPrefab == null)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No prefab loaded");
					return;
				}
				PrefabHelpers.export();
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Exported");
				return;
			}
			else
			{
				if (!(text == "thumbnail"))
				{
					return;
				}
				ConsoleCmdPrefab.prefabsToThumbnail.Clear();
				if (_params.Count == 2 && _params[1] == "bulk")
				{
					foreach (PathAbstractions.AbstractedLocation item in PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, false, null))
					{
						if (!SdFile.Exists(item.FullPathNoExtension + ".jpg"))
						{
							ConsoleCmdPrefab.prefabsToThumbnail.Add(item);
						}
					}
					this.thumbnailBulk();
					return;
				}
				if (_params.Count == 2)
				{
					if (PrefabEditModeManager.Instance.VoxelPrefab == null || PrefabEditModeManager.Instance.VoxelPrefab.PrefabName != _params[1])
					{
						PrefabEditModeManager.Instance.LoadVoxelPrefab(PathAbstractions.PrefabsSearchPaths.GetLocation(_params[1], null, null), true, true);
						if (PrefabEditModeManager.Instance.VoxelPrefab != null)
						{
							ThreadManager.StartCoroutine(this.thumbnailWaitForAllChunksBuilt(PrefabEditModeManager.Instance.VoxelPrefab.location, 0f));
							return;
						}
					}
				}
				else if (PrefabEditModeManager.Instance.VoxelPrefab != null)
				{
					ThreadManager.StartCoroutine(this.thumbnailWaitForAllChunksBuilt(PrefabEditModeManager.Instance.VoxelPrefab.location, 3f));
					return;
				}
			}
			return;
		}
		if (num <= 1117089386U)
		{
			if (num != 742177089U)
			{
				if (num != 948175754U)
				{
					if (num != 1117089386U)
					{
						return;
					}
					if (!(text == "simplify"))
					{
						return;
					}
					if (PrefabEditModeManager.Instance.VoxelPrefab == null)
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No prefab loaded");
						return;
					}
					PrefabHelpers.SimplifyPrefab(false);
					return;
				}
				else
				{
					if (!(text == "combine"))
					{
						return;
					}
					if (PrefabEditModeManager.Instance.VoxelPrefab == null)
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No prefab loaded");
						return;
					}
					PrefabHelpers.combine(true);
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Combined");
					return;
				}
			}
			else
			{
				if (!(text == "simplify1"))
				{
					return;
				}
				if (PrefabEditModeManager.Instance.VoxelPrefab == null)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No prefab loaded");
					return;
				}
				PrefabHelpers.SimplifyPrefab(true);
				return;
			}
		}
		else if (num != 1196198511U)
		{
			if (num != 1550717474U)
			{
				if (num != 1647949432U)
				{
					return;
				}
				if (!(text == "convert"))
				{
					return;
				}
				if (_params.Count < 2)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Please specify prefab to load");
					return;
				}
				PrefabEditModeManager.Instance.LoadVoxelPrefab(PathAbstractions.PrefabsSearchPaths.GetLocation(_params[1], null, null), false, false);
				PrefabHelpers.convert(new Action(PrefabHelpers.Cleanup));
				return;
			}
			else
			{
				if (!(text == "clear"))
				{
					return;
				}
				PrefabEditModeManager.Instance.ClearImposterPrefab();
				ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
				foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync())
				{
					GameManager.Instance.World.m_ChunkManager.RemoveChunk(chunk.Key);
				}
				chunkCache.Clear();
				return;
			}
		}
		else
		{
			if (!(text == "bulkins"))
			{
				return;
			}
			ConsoleCmdPrefab.prefabsToConvert.Clear();
			ConsoleCmdPrefab.prefabsToConvert.AddRange(PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, false, null));
			this.convertBulkInsideOutside();
			return;
		}
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x000726F8 File Offset: 0x000708F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void getPrefabStats(PathAbstractions.AbstractedLocation _path, Prefab _prefab)
	{
		WorldStats worldStats = WorldStats.CaptureWorldStats();
		_prefab.RenderingCostStats = worldStats;
		_prefab.SaveXMLData(_path);
		SdFile.AppendAllText(ConsoleCmdPrefab.PrefabStatsFilename, string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\n", new object[]
		{
			_path.Name,
			worldStats.TotalVertices,
			worldStats.TotalTriangles,
			worldStats.LightsVolume / (float)_prefab.size.Volume(),
			_prefab.size.x,
			_prefab.size.y,
			_prefab.size.z,
			_prefab.size.Volume(),
			worldStats.LightsVolume
		}));
	}

	// Token: 0x0600123D RID: 4669 RVA: 0x000727D0 File Offset: 0x000709D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void thumbnailBulk()
	{
		if (ConsoleCmdPrefab.prefabsToThumbnail.Count == 0)
		{
			return;
		}
		PathAbstractions.AbstractedLocation location = PathAbstractions.AbstractedLocation.None;
		while (ConsoleCmdPrefab.prefabsToThumbnail.Count != 0)
		{
			location = ConsoleCmdPrefab.prefabsToThumbnail[0];
			ConsoleCmdPrefab.prefabsToThumbnail.RemoveAt(0);
			if (PrefabEditModeManager.Instance.LoadVoxelPrefab(location, true, true))
			{
				break;
			}
		}
		GameManager.Instance.World.GetLocalPlayers()[0].SetPosition(new Vector3(0f, (float)PrefabEditModeManager.Instance.VoxelPrefab.size.y * 2f / 3f, (float)(-(float)PrefabEditModeManager.Instance.VoxelPrefab.size.z)), true);
		ThreadManager.StartCoroutine(this.thumbnailWaitForAllChunksBuilt(location, 0f));
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x00072890 File Offset: 0x00070A90
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator thumbnailWaitForAllChunksBuilt(PathAbstractions.AbstractedLocation _location, float _delay = 0f)
	{
		if (_delay > 0f)
		{
			yield return new WaitForSeconds(_delay);
		}
		ChunkCluster cc = GameManager.Instance.World.ChunkCache;
		List<Chunk> chunkArrayCopySync = cc.GetChunkArrayCopySync();
		foreach (Chunk c in chunkArrayCopySync)
		{
			if (!cc.IsOnBorder(c))
			{
				if (!c.IsEmpty())
				{
					while (c.NeedsRegeneration || c.NeedsCopying)
					{
						yield return new WaitForSeconds(1f);
					}
					c = null;
				}
			}
		}
		List<Chunk>.Enumerator enumerator = default(List<Chunk>.Enumerator);
		GameUtils.TakeScreenShot(GameUtils.EScreenshotMode.File, _location.FullPathNoExtension, 0.1f, true, 280, 210, false);
		if (ConsoleCmdPrefab.prefabsToThumbnail.Count > 0)
		{
			this.thumbnailBulk();
		}
		yield break;
		yield break;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x000728B0 File Offset: 0x00070AB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void mergeBulk()
	{
		if (ConsoleCmdPrefab.prefabsToMerge.Count == 0)
		{
			return;
		}
		while (ConsoleCmdPrefab.prefabsToMerge.Count != 0)
		{
			PathAbstractions.AbstractedLocation location = ConsoleCmdPrefab.prefabsToMerge[0];
			ConsoleCmdPrefab.prefabsToMerge.RemoveAt(0);
			if (PrefabEditModeManager.Instance.LoadVoxelPrefab(location, true, true))
			{
				PrefabHelpers.mergePrefab(false);
				PrefabEditModeManager.Instance.SaveVoxelPrefab();
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Saved prefab {0} with size {1}", PrefabEditModeManager.Instance.VoxelPrefab.location, PrefabEditModeManager.Instance.VoxelPrefab.size));
			}
		}
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00072950 File Offset: 0x00070B50
	[PublicizedFrom(EAccessModifier.Private)]
	public void convertBulk()
	{
		if (ConsoleCmdPrefab.prefabsToConvert.Count == 0 || ConsoleCmdPrefab.processedCount >= ConsoleCmdPrefab.stopCount)
		{
			PrefabHelpers.Cleanup();
			Log.Out("-- Prefab bulk {0}, done in {1}! --", new object[]
			{
				ConsoleCmdPrefab.processedCount,
				(float)ConsoleCmdPrefab.processedSW.ElapsedMilliseconds * 0.001f
			});
			return;
		}
		PathAbstractions.AbstractedLocation abstractedLocation = PathAbstractions.AbstractedLocation.None;
		while (ConsoleCmdPrefab.prefabsToConvert.Count != 0)
		{
			abstractedLocation = ConsoleCmdPrefab.prefabsToConvert[0];
			ConsoleCmdPrefab.prefabsToConvert.RemoveAt(0);
			if (PrefabEditModeManager.Instance.LoadVoxelPrefab(abstractedLocation, true, false))
			{
				break;
			}
		}
		ConsoleCmdPrefab.processedCount++;
		Log.Out("Prefab #{0}, {1}", new object[]
		{
			ConsoleCmdPrefab.processedCount,
			abstractedLocation
		});
		PrefabHelpers.convert(new Action(this.convertBulk));
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00072A2C File Offset: 0x00070C2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void convertBulkInsideOutside()
	{
		if (ConsoleCmdPrefab.prefabsToConvert.Count == 0)
		{
			PrefabHelpers.Cleanup();
			return;
		}
		PathAbstractions.AbstractedLocation location = PathAbstractions.AbstractedLocation.None;
		while (ConsoleCmdPrefab.prefabsToConvert.Count != 0)
		{
			location = ConsoleCmdPrefab.prefabsToConvert[0];
			ConsoleCmdPrefab.prefabsToConvert.RemoveAt(0);
			if (PrefabEditModeManager.Instance.LoadVoxelPrefab(location, true, false) && !PrefabEditModeManager.Instance.VoxelPrefab.bExcludePOICulling)
			{
				break;
			}
		}
		Log.Out("Processing " + location.ToString());
		PrefabHelpers.convertInsideOutside(new Action(this.convertBulkInsideOutside));
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00072AC4 File Offset: 0x00070CC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void restore()
	{
		World world = GameManager.Instance.World;
		EntityPlayerLocal entityPlayerLocal = world.GetLocalPlayers()[0];
		Chunk chunkSync = world.ChunkCache.GetChunkSync(World.toChunkXZ(entityPlayerLocal.GetBlockPosition().x), World.toChunkXZ(entityPlayerLocal.GetBlockPosition().z));
		if (chunkSync != null)
		{
			chunkSync.RestoreCulledBlocks(world);
			chunkSync.NeedsRegeneration = true;
		}
	}

	// Token: 0x04000D24 RID: 3364
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly MicroStopwatch processedSW = new MicroStopwatch();

	// Token: 0x04000D25 RID: 3365
	[PublicizedFrom(EAccessModifier.Private)]
	public static int processedCount;

	// Token: 0x04000D26 RID: 3366
	[PublicizedFrom(EAccessModifier.Private)]
	public static int stopCount;

	// Token: 0x04000D27 RID: 3367
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<PathAbstractions.AbstractedLocation> prefabsToConvert = new List<PathAbstractions.AbstractedLocation>();

	// Token: 0x04000D28 RID: 3368
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<PathAbstractions.AbstractedLocation> prefabsToMerge = new List<PathAbstractions.AbstractedLocation>();

	// Token: 0x04000D29 RID: 3369
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<PathAbstractions.AbstractedLocation> prefabsToThumbnail = new List<PathAbstractions.AbstractedLocation>();

	// Token: 0x04000D2A RID: 3370
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string PrefabStatsFilename = GameIO.GetDeviceLocalUserGameDataDir() + "/_prefabstats.csv";
}
