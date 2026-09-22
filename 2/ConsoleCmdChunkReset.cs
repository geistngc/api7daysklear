using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001F9 RID: 505
[Preserve]
public class ConsoleCmdChunkReset : ConsoleCmdAbstract
{
	// Token: 0x06000F7F RID: 3967 RVA: 0x000648B3 File Offset: 0x00062AB3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"chunkreset",
			"cr"
		};
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000648CB File Offset: 0x00062ACB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "resets the specified chunks";
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x000648D2 File Offset: 0x00062AD2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. chunkreset <x1> <z1> <x2> <z2>\n  2. chunkreset [f]\n1. Rebuilds the chunks that contain the given coordinate range.\n2. Can only be executed by a player in the ingame console! Behaviour depends on whether the\n   player is currently within the bounds of a POI:\n   Within a POI: The POI is reset.\n   Not within a POI: The chunk the player is in and the eight chunks around that one are\n     rebuilt. Not deco! Does not reload POI data!\n   d - regen deco\n   f - fully regenerates chunks (may cause double entities!)\n   u, utimed, ue - Unload chunks or entities\n   nq - Enqueue a 3x3 group of chunks centred around the player to be reset when unsynced, unless they are otherwise protected from the chunk reset system.";
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x000648D9 File Offset: 0x00062AD9
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager.Instance.StartCoroutine(this.execute(_params, _senderInfo));
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x000648EE File Offset: 0x00062AEE
	public IEnumerator execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		World world = GameManager.Instance.World;
		if (_params.Count >= 2)
		{
			int num;
			if (!int.TryParse(_params[0], out num))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x1 is not a valid integer");
				yield break;
			}
			int num2;
			if (!int.TryParse(_params[1], out num2))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z1 is not a valid integer");
				yield break;
			}
			int num3 = num;
			int num4 = num2;
			if (_params.Count >= 3 && !int.TryParse(_params[2], out num3))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x2 is not a valid integer");
				yield break;
			}
			if (_params.Count >= 4 && !int.TryParse(_params[3], out num4))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z2 is not a valid integer");
				yield break;
			}
			Vector2i vector2i = new Vector2i((num <= num3) ? num : num3, (num2 <= num4) ? num2 : num4);
			Vector2i vector2i2 = new Vector2i((num <= num3) ? num3 : num, (num2 <= num4) ? num4 : num2);
			if (vector2i2.x - vector2i.x > 16384 || vector2i2.y - vector2i.y > 16384)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("area too big");
				yield break;
			}
			vector2i = World.toChunkXZ(vector2i);
			vector2i2 = World.toChunkXZ(vector2i2);
			HashSetLong hashSetLong = new HashSetLong();
			for (int i = vector2i.x; i <= vector2i2.x; i++)
			{
				for (int j = vector2i.y; j <= vector2i2.y; j++)
				{
					hashSetLong.Add(WorldChunkCache.MakeChunkKey(i, j));
				}
			}
			ChunkCluster chunkCache = world.ChunkCache;
			ChunkProviderGenerateWorld chunkProviderGenerateWorld = world.ChunkCache.ChunkProvider as ChunkProviderGenerateWorld;
			if (chunkProviderGenerateWorld != null)
			{
				LockManager.Instance.ForceUnlockByChunk(hashSetLong);
				chunkProviderGenerateWorld.RemoveChunks(hashSetLong);
				foreach (long key in hashSetLong)
				{
					if (!chunkProviderGenerateWorld.GenerateSingleChunk(chunkCache, key, true))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Failed regenerating chunk at position {0}/{1}", WorldChunkCache.extractX(key) << 4, WorldChunkCache.extractZ(key) << 4));
					}
				}
				GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(hashSetLong);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Reset chunks covering area {0}/{1} to {2}/{3} (chunk coordinates {4} to {5}).", new object[]
				{
					num,
					num2,
					num3,
					num4,
					vector2i,
					vector2i2
				}));
				if (!(DynamicMeshManager.Instance != null))
				{
					goto IL_968;
				}
				using (HashSetLong.Enumerator enumerator = hashSetLong.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						long key2 = enumerator.Current;
						DynamicMeshManager.Instance.AddChunk(key2, true, true, null);
					}
					yield break;
				}
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Can not reset chunks on this game");
		}
		else
		{
			if (_params.Count > 1 || (_senderInfo.RemoteClientInfo == null && (!_senderInfo.IsLocalGame || GameManager.IsDedicatedServer)))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid arguments, please see command help.");
				yield break;
			}
			Vector3 position;
			if (_senderInfo.RemoteClientInfo != null)
			{
				position = world.Players.dict[_senderInfo.RemoteClientInfo.entityId].position;
			}
			else
			{
				position = world.GetLocalPlayers()[0].position;
			}
			int num = World.toChunkXZ((int)position.x) - 1;
			int num2 = World.toChunkXZ((int)position.z) - 1;
			int num3 = num + 2;
			int num4 = num2 + 2;
			HashSetLong chunks = new HashSetLong();
			for (int k = num; k <= num3; k++)
			{
				for (int l = num2; l <= num4; l++)
				{
					chunks.Add(WorldChunkCache.MakeChunkKey(k, l));
				}
			}
			if (_params.Count == 1)
			{
				ChunkCluster chunkCache2 = world.ChunkCache;
				ChunkProviderGenerateWorld chunkProviderGenerateWorld2 = chunkCache2.ChunkProvider as ChunkProviderGenerateWorld;
				if (chunkProviderGenerateWorld2 == null)
				{
					yield break;
				}
				if (_params[0] == "nq")
				{
					foreach (long chunkKey in chunks)
					{
						chunkProviderGenerateWorld2.RequestChunkReset(chunkKey);
					}
					yield break;
				}
				if (_params[0] == "d")
				{
					LockManager.Instance.ForceUnlockByChunk(chunks);
					foreach (long key3 in chunks)
					{
						Chunk chunkSync = chunkCache2.GetChunkSync(key3);
						if (chunkSync != null)
						{
							chunkSync.NeedsLightDecoration = true;
							chunkSync.NeedsLightCalculation = true;
						}
					}
					GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(chunks);
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Generate deco around player");
				}
				else if (_params[0] == "f")
				{
					LockManager.Instance.ForceUnlockByChunk(chunks);
					foreach (long key4 in chunks)
					{
						if (!chunkProviderGenerateWorld2.GenerateSingleChunk(chunkCache2, key4, true))
						{
							SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Failed regenerating chunk at position {0}/{1}", WorldChunkCache.extractX(key4) << 4, WorldChunkCache.extractZ(key4) << 4));
						}
					}
					GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(chunks);
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Generate chunks around player");
				}
				else
				{
					if (_params[0] == "r")
					{
						using (HashSetLong.Enumerator enumerator = chunks.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								long key5 = enumerator.Current;
								Chunk chunkSync2 = chunkCache2.GetChunkSync(key5);
								if (chunkSync2 != null)
								{
									chunkSync2.NeedsRegeneration = true;
								}
							}
							goto IL_900;
						}
					}
					if (_params[0] == "u")
					{
						foreach (long chunkKey2 in chunks)
						{
							world.m_ChunkManager.RemoveChunk(chunkKey2);
						}
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unload around player");
					}
					else if (_params[0] == "utimed")
					{
						if (this.unloadTimed != null)
						{
							GameManager.Instance.StopCoroutine(this.unloadTimed);
							this.unloadTimed = null;
						}
						else
						{
							this.unloadTimed = GameManager.Instance.StartCoroutine(this.UnloadTimed());
						}
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unload timed at player {0}", new object[]
						{
							this.unloadTimed != null
						});
					}
					else if (_params[0] == "ue")
					{
						foreach (long key6 in chunks)
						{
							Chunk chunkSync3 = chunkCache2.GetChunkSync(key6);
							if (chunkSync3 != null)
							{
								for (int m = 0; m < chunkSync3.entityLists.Length; m++)
								{
									List<Entity> list = chunkSync3.entityLists[m];
									for (int n = list.Count - 1; n >= 0; n--)
									{
										Entity entity = list[n];
										if (!entity.bWillRespawn && (!(entity.AttachedMainEntity != null) || !entity.AttachedMainEntity.bWillRespawn))
										{
											world.unloadEntity(entity, EnumRemoveEntityReason.Unloaded);
											list.RemoveAt(n);
										}
									}
								}
							}
						}
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("UnloadEntities around player");
					}
				}
			}
			else
			{
				DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
				List<PrefabInstance> prefabsFromWorldPosInside;
				if (dynamicPrefabDecorator != null && (prefabsFromWorldPosInside = dynamicPrefabDecorator.GetPrefabsFromWorldPosInside(position, FastTags<TagGroup.Global>.none)) != null)
				{
					yield return world.ResetPOIS(prefabsFromWorldPosInside, QuestEventManager.manualResetTag, -1, null, null);
				}
				else
				{
					LockManager.Instance.ForceUnlockByChunk(chunks);
					world.RebuildTerrain(chunks, Vector3i.zero, Vector3i.zero, false, true, true, false);
					GameManager.Instance.World.m_ChunkManager.ResendChunksToClients(chunks);
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Reset chunks around player");
				}
			}
			IL_900:
			if (DynamicMeshManager.Instance != null)
			{
				foreach (long key7 in chunks)
				{
					DynamicMeshManager.Instance.AddChunk(key7, true, true, null);
				}
			}
			chunks = null;
		}
		IL_968:
		yield break;
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0006490B File Offset: 0x00062B0B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator UnloadTimed()
	{
		int num;
		for (int i = 0; i < 99999; i = num)
		{
			World world = GameManager.Instance.World;
			if (world == null)
			{
				break;
			}
			EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
			if (!primaryPlayer)
			{
				break;
			}
			Vector3i blockPosition = primaryPlayer.GetBlockPosition();
			for (int j = -16; j <= 16; j += 16)
			{
				for (int k = -16; k <= 16; k += 16)
				{
					int x = World.toChunkXZ(blockPosition.x + k);
					int y = World.toChunkXZ(blockPosition.z + j);
					long chunkKey = WorldChunkCache.MakeChunkKey(x, y);
					world.m_ChunkManager.RemoveChunk(chunkKey);
				}
			}
			yield return new WaitForSeconds(1.5f);
			num = i + 1;
		}
		this.unloadTimed = null;
		yield break;
	}

	// Token: 0x04000C99 RID: 3225
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine unloadTimed;
}
