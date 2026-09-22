using System;
using System.Collections.Generic;

// Token: 0x02000276 RID: 630
public static class ChunkResetCommandHelpers
{
	// Token: 0x060012A5 RID: 4773 RVA: 0x000745DC File Offset: 0x000727DC
	public static bool TryParseProtectionMode(List<string> _params, out ChunkProtectionLevel _protection, out int _pmode)
	{
		_protection = ChunkProtectionLevel.All;
		_pmode = 0;
		int num = ChunkResetCommandHelpers.FindFlag(_params, new string[]
		{
			"-p",
			"-pmode"
		});
		if (num < 0)
		{
			return true;
		}
		int num2;
		if (num + 1 >= _params.Count || !int.TryParse(_params[num + 1], out num2))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid argument: '" + _params[num] + "' requires an integer value.");
			return false;
		}
		switch (num2)
		{
		case 0:
			_protection = ChunkProtectionLevel.All;
			break;
		case 1:
			_protection = ~ChunkProtectionLevel.CurrentlySynced;
			break;
		case 2:
			_protection = ChunkProtectionLevel.None;
			break;
		case 3:
			_protection = ChunkProtectionLevel.CurrentlySynced;
			break;
		default:
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Invalid argument: '{0}' is not a supported protection mode value.", num2));
			return false;
		}
		_pmode = num2;
		return true;
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x000746A8 File Offset: 0x000728A8
	public static bool TryParseGroupingMode(List<string> _params, out EnumResetUnprotectedChunksGroupingMode _grouping)
	{
		_grouping = EnumResetUnprotectedChunksGroupingMode.GroupedPOIs;
		int num = ChunkResetCommandHelpers.FindFlag(_params, new string[]
		{
			"-g",
			"-gmode"
		});
		if (num < 0)
		{
			return true;
		}
		int num2;
		if (num + 1 >= _params.Count || !int.TryParse(_params[num + 1], out num2) || num2 < 1 || num2 > 3)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid argument: '" + _params[num] + "' requires an integer value of 1, 2, or 3.");
			return false;
		}
		_grouping = (EnumResetUnprotectedChunksGroupingMode)num2;
		return true;
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00074728 File Offset: 0x00072928
	public static bool TryParseRegion(List<string> _params, out int? _regionX, out int? _regionZ)
	{
		_regionX = null;
		_regionZ = null;
		int num = ChunkResetCommandHelpers.FindFlag(_params, new string[]
		{
			"-r",
			"-region"
		});
		if (num < 0)
		{
			return true;
		}
		int value;
		int value2;
		if (num + 2 >= _params.Count || !int.TryParse(_params[num + 1], out value) || !int.TryParse(_params[num + 2], out value2))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid argument: '" + _params[num] + "' requires two integer coordinates (x, z).");
			return false;
		}
		_regionX = new int?(value);
		_regionZ = new int?(value2);
		return true;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000747D0 File Offset: 0x000729D0
	public static void ExecuteReset(ChunkProtectionLevel _protectionMask, EnumResetUnprotectedChunksGroupingMode _groupingMode, int? _regionX, int? _regionZ, string _opName)
	{
		World world = GameManager.Instance.World;
		ChunkCluster chunkCache = world.ChunkCache;
		ChunkProviderGenerateWorld chunkProviderGenerateWorld = chunkCache.ChunkProvider as ChunkProviderGenerateWorld;
		if (chunkProviderGenerateWorld == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(_opName + " failed: ChunkProviderGenerateWorld could not be found for current world instance.");
			return;
		}
		chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
		HashSetLong hashSetLong = (_regionX != null && _regionZ != null) ? chunkProviderGenerateWorld.ResetRegion(_regionX.Value, _regionZ.Value, _protectionMask, _groupingMode) : chunkProviderGenerateWorld.ResetAllChunks(_protectionMask, _groupingMode);
		if ((_protectionMask & ChunkProtectionLevel.CurrentlySynced) == ChunkProtectionLevel.None)
		{
			HashSetLong hashSetLong2 = new HashSetLong();
			HashSetLong hashSetLong3 = new HashSetLong();
			foreach (long num in hashSetLong)
			{
				if (chunkCache.ContainsChunkSync(num))
				{
					hashSetLong2.Add(num);
				}
			}
			if (hashSetLong2.Count > 0)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Regenerating {0} synced chunks.", hashSetLong2.Count));
				foreach (long num2 in hashSetLong2)
				{
					if (!chunkProviderGenerateWorld.GenerateSingleChunk(chunkCache, num2, true))
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} failed regenerating chunk at world XZ position: {1}, {2}", _opName, WorldChunkCache.extractX(num2) << 4, WorldChunkCache.extractZ(num2) << 4));
					}
					else
					{
						hashSetLong3.Add(num2);
					}
				}
				world.m_ChunkManager.ResendChunksToClients(hashSetLong3);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Regeneration complete.");
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} complete. Reset {1} chunks.", _opName, hashSetLong.Count));
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000749A8 File Offset: 0x00072BA8
	[PublicizedFrom(EAccessModifier.Private)]
	public static int FindFlag(List<string> _params, params string[] _flags)
	{
		for (int i = 0; i < _params.Count; i++)
		{
			string a = _params[i].ToLowerInvariant();
			for (int j = 0; j < _flags.Length; j++)
			{
				if (a == _flags[j])
				{
					return i;
				}
			}
		}
		return -1;
	}
}
