using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Platform;
using UnityEngine;

// Token: 0x020013C9 RID: 5065
public static class DebugGameStats
{
	// Token: 0x06009F46 RID: 40774 RVA: 0x003C30D8 File Offset: 0x003C12D8
	public static void StartStatisticsUpdate(DebugGameStats.StatisticsUpdatedCallback callback)
	{
		if (DebugGameStats.m_memorySampler == null)
		{
			IPlatformMemory memory = PlatformManager.MultiPlatform.Memory;
			DebugGameStats.m_memorySampler = ((memory != null) ? memory.CreateSampler() : null);
		}
		IPlatformMemorySampler memorySampler = DebugGameStats.m_memorySampler;
		object obj;
		if (memorySampler == null)
		{
			obj = null;
		}
		else
		{
			IReadOnlyList<IPlatformMemoryStat> statistics = memorySampler.Statistics;
			if (statistics == null)
			{
				obj = null;
			}
			else
			{
				obj = statistics.FirstOrDefault((IPlatformMemoryStat s) => s.Name == "GameUsed");
			}
		}
		DebugGameStats.m_memoryGameUsedStat = (IPlatformMemoryStat<long>)obj;
		DebugGameStats.TryInitializeStatisticsDictionary();
		if (DebugGameStats.updateStatsCoroutine != null)
		{
			ThreadManager.StopCoroutine(DebugGameStats.updateStatsCoroutine);
		}
		if (DebugGameStats.updateDeltasCoroutine != null)
		{
			ThreadManager.StopCoroutine(DebugGameStats.updateDeltasCoroutine);
		}
		DebugGameStats.doStatisticsUpdate = true;
		DebugGameStats.updateStatsCoroutine = ThreadManager.StartCoroutine(DebugGameStats.UpdateStatisticsCo(callback));
		DebugGameStats.updateDeltasCoroutine = ThreadManager.StartCoroutine(DebugGameStats.UpdateDeltas());
	}

	// Token: 0x06009F47 RID: 40775 RVA: 0x003C319C File Offset: 0x003C139C
	public static void TryInitializeStatisticsDictionary()
	{
		if (DebugGameStats.statisticsDictionary.Keys.Count > 0)
		{
			return;
		}
		foreach (FieldInfo fieldInfo in typeof(DebugGameStats.Statistics).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
		{
			if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
			{
				string key = (string)fieldInfo.GetValue(null);
				if (!DebugGameStats.statisticsDictionary.ContainsKey(key))
				{
					DebugGameStats.statisticsDictionary[key] = string.Empty;
				}
			}
		}
	}

	// Token: 0x06009F48 RID: 40776 RVA: 0x003C3231 File Offset: 0x003C1431
	public static void StopStatisticsUpdate()
	{
		DebugGameStats.doStatisticsUpdate = false;
		DebugGameStats.updateStatsCoroutine = null;
		DebugGameStats.updateDeltasCoroutine = null;
	}

	// Token: 0x06009F49 RID: 40777 RVA: 0x003C3248 File Offset: 0x003C1448
	public static string GetHeader(char separator)
	{
		DebugGameStats.TryInitializeStatisticsDictionary();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in DebugGameStats.statisticsDictionary)
		{
			stringBuilder.Append(keyValuePair.Key);
			stringBuilder.Append(separator);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06009F4A RID: 40778 RVA: 0x003C32BC File Offset: 0x003C14BC
	public static string GetCurrentStatsString(char separator = ',')
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in DebugGameStats.statisticsDictionary)
		{
			stringBuilder.Append(keyValuePair.Value);
			stringBuilder.Append(separator);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06009F4B RID: 40779 RVA: 0x003C332C File Offset: 0x003C152C
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator UpdateDeltas()
	{
		while (DebugGameStats.doStatisticsUpdate)
		{
			DebugGameStats.deltaTextureMemory += (long)(Texture.currentTextureMemory - (ulong)DebugGameStats.m_textureMemoryPrevFrame);
			DebugGameStats.m_textureMemoryPrevFrame = (long)Texture.currentTextureMemory;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06009F4C RID: 40780 RVA: 0x003C3334 File Offset: 0x003C1534
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator UpdateStatisticsCo(DebugGameStats.StatisticsUpdatedCallback callback)
	{
		long num = 0L;
		while (DebugGameStats.doStatisticsUpdate)
		{
			DebugGameStats.statisticsDictionary["gamestats.ingametime"] = (GameTimer.Instance.ticksSincePlayfieldLoaded / 20UL).ToString();
			if (DebugGameStats.m_memorySampler != null)
			{
				DebugGameStats.m_memorySampler.Sample();
				if (DebugGameStats.m_memoryGameUsedStat != null && DebugGameStats.m_memoryGameUsedStat.TryGet(MemoryStatColumn.Current, out num))
				{
					DebugGameStats.statisticsDictionary["gamestats.nativegameused"] = (num / 1024L).ToString();
				}
			}
			DebugGameStats.statisticsDictionary["gamestats.entityinstances"] = Entity.InstanceCount.ToString();
			DebugGameStats.statisticsDictionary["gamestats.maxusedchunks"] = Chunk.InstanceCount.ToString();
			DebugGameStats.statisticsDictionary["gamestats.displayedprefabs"] = GameManager.Instance.prefabLODManager.displayedPrefabs.Count.ToString();
			long currentTextureMemory = (long)Texture.currentTextureMemory;
			DebugGameStats.statisticsDictionary["gamestats.texturememorydelta60"] = (DebugGameStats.deltaTextureMemory / 1024L).ToString();
			DebugGameStats.deltaTextureMemory = 0L;
			DebugGameStats.statisticsDictionary["gamestats.texturememorycurrent"] = (currentTextureMemory / 1024L).ToString();
			DebugGameStats.statisticsDictionary["gamestats.textureMemorydesired"] = (Texture.desiredTextureMemory / 1024UL).ToString();
			Log.Out(string.Concat(new string[]
			{
				"60sec delta: ",
				DebugGameStats.statisticsDictionary["gamestats.texturememorydelta60"],
				",current: ",
				DebugGameStats.statisticsDictionary["gamestats.texturememorycurrent"],
				",desired: ",
				DebugGameStats.statisticsDictionary["gamestats.textureMemorydesired"]
			}));
			int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode);
			if (@int - 1 <= 1)
			{
				DebugGameStats.statisticsDictionary["gamestats.VideoScalingSetting"] = string.Format("{0}, Quality preset {1}, FSR preset {2}", GameOptionsPlatforms.UpscalerMode.ToString(@int), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFSRPreset));
			}
			else
			{
				DebugGameStats.statisticsDictionary["gamestats.VideoScalingSetting"] = string.Format("{0}, Quality preset {1}", GameOptionsPlatforms.UpscalerMode.ToString(@int), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset));
			}
			if (GameManager.Instance.World != null)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
				{
					DebugGameStats.statisticsDictionary["gamestats.ConnectionStatus"] = "Connected";
				}
				else
				{
					DebugGameStats.statisticsDictionary["gamestats.ConnectionStatus"] = "Disconnected";
				}
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "SinglePlayer";
				}
				else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "MultiplayerHostOrServer";
				}
				else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "Client";
				}
				else
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "Unknown";
				}
				Dictionary<string, string> dictionary = DebugGameStats.statisticsDictionary;
				string key = "gamestats.gameMode";
				GameMode gameMode = GameManager.Instance.GetGameStateManager().GetGameMode();
				dictionary[key] = ((gameMode != null) ? gameMode.GetName() : null);
				DebugGameStats.statisticsDictionary["gamestats.PlayerCount"] = string.Format("Clients: {0}", SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount());
				DebugGameStats.statisticsDictionary["gamestats.worldentities"] = GameManager.Instance.World.Entities.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.chunkobservers"] = GameManager.Instance.World.m_ChunkManager.m_ObservedEntities.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.syncedchunks"] = GameManager.Instance.World.ChunkCache.chunks.list.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.chunkgameobjects"] = GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjectsCount().ToString();
				DebugGameStats.statisticsDictionary["gamestats.worldtime"] = ValueDisplayFormatters.WorldTime(GameManager.Instance.World.worldTime, "Day {0}, {1:00}:{2:00}");
				DebugGameStats.statisticsDictionary["gamestats.isbloodmoon"] = GameManager.Instance.World.isEventBloodMoon.ToString();
				DebugGameStats.statisticsDictionary["gamestats.SandboxPreset"] = GamePrefs.GetString(EnumGamePrefs.SandboxPreset);
				DebugGameStats.statisticsDictionary["gamestats.SandboxCode"] = GamePrefs.GetString(EnumGamePrefs.SandboxCode);
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer)
				{
					DebugGameStats.statisticsDictionary["gamestats.currentPlayerBiome"] = string.Format("Player biome {0}", primaryPlayer.biomeStandingOn);
				}
				if (WeatherManager.Instance)
				{
					Color fogColor = SkyManager.GetFogColor();
					WeatherManager.BiomeWeather currentWeather = WeatherManager.currentWeather;
					bool flag = currentWeather != null;
					DebugGameStats.statisticsDictionary["gamestats.weathersystemstatus"] = string.Concat(new string[]
					{
						"WM status: ",
						WeatherManager.Instance.ToString(),
						"\n PlayerCurrent: <Temp: ",
						WeatherManager.GetTemperature().ToCultureInvariantString(),
						" Clouds ",
						(WeatherManager.GetCloudThickness() * 0.01f).ToCultureInvariantString(),
						" Fog density ",
						SkyManager.GetFogDensity().ToCultureInvariantString(),
						", start ",
						SkyManager.GetFogStart().ToCultureInvariantString(),
						", end ",
						SkyManager.GetFogEnd().ToCultureInvariantString(),
						" FogColor ",
						fogColor.r.ToCultureInvariantString(),
						" ",
						fogColor.g.ToCultureInvariantString(),
						" ",
						fogColor.b.ToCultureInvariantString(),
						" Rain  ",
						((WeatherManager.forceRain >= 0f) ? WeatherManager.forceRain : (flag ? currentWeather.rainParam.value : 0f)).ToCultureInvariantString(),
						" Snowfall  ",
						((WeatherManager.forceSnowfall >= 0f) ? WeatherManager.forceSnowfall : (flag ? currentWeather.snowFallParam.value : 0f)).ToCultureInvariantString(),
						" Temperature ",
						WeatherManager.GetTemperature().ToCultureInvariantString(),
						" Wind ",
						WeatherManager.GetWindSpeed().ToCultureInvariantString(),
						">"
					});
				}
				if (GameManager.Instance.World.GetLocalPlayers().Count > 0)
				{
					if (!DiscordManager.Instance.IsInitialized)
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = (DiscordManager.Instance.Settings.DiscordDisabled ? "Disabled and not initialized" : "Not initialized");
					}
					else if (!DiscordManager.Instance.IsReady)
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = (DiscordManager.Instance.Settings.DiscordDisabled ? "Disabled and not ready" : "Not ready");
					}
					else
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = "Discord enabled initialized and ready";
						DebugGameStats.statisticsDictionary["gamestats.discordsettings"] = SdPlayerPrefs.GetString("DiscordSettings");
					}
					DebugGameStats.statisticsDictionary["gamestats.localplayerpos"] = GameManager.Instance.World.GetLocalPlayers()[0].position.ToString();
				}
			}
			Log.Out("[Backtrace] Updated Statistics");
			callback(DebugGameStats.statisticsDictionary);
			yield return new WaitForSeconds(60f);
		}
		yield break;
	}

	// Token: 0x040078F3 RID: 30963
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> statisticsDictionary = new Dictionary<string, string>();

	// Token: 0x040078F4 RID: 30964
	[PublicizedFrom(EAccessModifier.Private)]
	public static IPlatformMemorySampler m_memorySampler;

	// Token: 0x040078F5 RID: 30965
	[PublicizedFrom(EAccessModifier.Private)]
	public static IPlatformMemoryStat<long> m_memoryGameUsedStat;

	// Token: 0x040078F6 RID: 30966
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool doStatisticsUpdate = false;

	// Token: 0x040078F7 RID: 30967
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine updateStatsCoroutine;

	// Token: 0x040078F8 RID: 30968
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine updateDeltasCoroutine;

	// Token: 0x040078F9 RID: 30969
	[PublicizedFrom(EAccessModifier.Private)]
	public static long deltaTextureMemory = 0L;

	// Token: 0x040078FA RID: 30970
	[PublicizedFrom(EAccessModifier.Private)]
	public static long m_textureMemoryPrevFrame = 0L;

	// Token: 0x020013CA RID: 5066
	[PublicizedFrom(EAccessModifier.Private)]
	public static class Statistics
	{
		// Token: 0x040078FB RID: 30971
		public const string InGameTimeKey = "gamestats.ingametime";

		// Token: 0x040078FC RID: 30972
		public const string ManagedHeapSizeKey = "gamestats.managedheap";

		// Token: 0x040078FD RID: 30973
		public const string NativeGameUsedKey = "gamestats.nativegameused";

		// Token: 0x040078FE RID: 30974
		public const string TextureMemoryDesiredKey = "gamestats.textureMemorydesired";

		// Token: 0x040078FF RID: 30975
		public const string TextureMemoryCurrentKey = "gamestats.texturememorycurrent";

		// Token: 0x04007900 RID: 30976
		public const string TextureMemoryDelta60Key = "gamestats.texturememorydelta60";

		// Token: 0x04007901 RID: 30977
		public const string WorldEntitiesCountKey = "gamestats.worldentities";

		// Token: 0x04007902 RID: 30978
		public const string EntityInstanceCountKey = "gamestats.entityinstances";

		// Token: 0x04007903 RID: 30979
		public const string ChunkObserverCountKey = "gamestats.chunkobservers";

		// Token: 0x04007904 RID: 30980
		public const string MaxUsedChunkCountKey = "gamestats.maxusedchunks";

		// Token: 0x04007905 RID: 30981
		public const string SyncedChunkCountKey = "gamestats.syncedchunks";

		// Token: 0x04007906 RID: 30982
		public const string ChunkGameObjectCountKey = "gamestats.chunkgameobjects";

		// Token: 0x04007907 RID: 30983
		public const string DisplayedPrefabCountKey = "gamestats.displayedprefabs";

		// Token: 0x04007908 RID: 30984
		public const string LocalPlayerPositionKey = "gamestats.localplayerpos";

		// Token: 0x04007909 RID: 30985
		public const string WorldTimeKey = "gamestats.worldtime";

		// Token: 0x0400790A RID: 30986
		public const string BloodMoonKey = "gamestats.isbloodmoon";

		// Token: 0x0400790B RID: 30987
		public const string GameModeKey = "gamestats.gameMode";

		// Token: 0x0400790C RID: 30988
		public const string PlayerCountKey = "gamestats.PlayerCount";

		// Token: 0x0400790D RID: 30989
		public const string ConnectStatusKey = "gamestats.ConnectionStatus";

		// Token: 0x0400790E RID: 30990
		public const string HostStatusKey = "gamestats.HostStatus";

		// Token: 0x0400790F RID: 30991
		public const string CurrentBiomeKey = "gamestats.currentPlayerBiome";

		// Token: 0x04007910 RID: 30992
		public const string WeatherStatusKey = "gamestats.weathersystemstatus";

		// Token: 0x04007911 RID: 30993
		public const string VideoScalingSetting = "gamestats.VideoScalingSetting";

		// Token: 0x04007912 RID: 30994
		public const string DiscordPluginStatusKey = "gamestats.discordstatus";

		// Token: 0x04007913 RID: 30995
		public const string DiscordPluginSettingsKey = "gamestats.discordsettings";

		// Token: 0x04007914 RID: 30996
		public const string SandboxPresetKey = "gamestats.SandboxPreset";

		// Token: 0x04007915 RID: 30997
		public const string SandboxCodeKey = "gamestats.SandboxCode";
	}

	// Token: 0x020013CB RID: 5067
	// (Invoke) Token: 0x06009F4F RID: 40783
	public delegate void StatisticsUpdatedCallback(Dictionary<string, string> statisticsDictionary);
}
