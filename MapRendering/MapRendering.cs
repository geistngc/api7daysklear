using System;
using Webserver;
using Webserver.UrlHandlers;

namespace MapRendering
{
	// Token: 0x02001AB5 RID: 6837
	public static class MapRendering
	{
		// Token: 0x0600CE41 RID: 52801 RVA: 0x004B1F08 File Offset: 0x004B0108
		public static void Init()
		{
			ModEvents.GameStartDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(MapRendering.GameStartDone));
			ModEvents.GameShutdown.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameShutdownData>(MapRendering.GameShutdown));
			ModEvents.CalcChunkColorsDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SCalcChunkColorsDoneData>(MapRendering.CalcChunkColorsDone));
			Web.ServerInitialized += delegate(Web _web)
			{
				if (!MapRenderer.Enabled)
				{
					return;
				}
				_web.RegisterPathHandler("/map/", new StaticHandler(GameIO.GetSaveGameDir() + "/map", MapRenderer.GetTileCache(), false, "web.map"));
				_web.OpenApiHelpers.RegisterCustomSpec(typeof(MapRendering).Assembly, "MapTileHandler", "/map/");
			};
		}

		// Token: 0x0600CE42 RID: 52802 RVA: 0x004B1F7B File Offset: 0x004B017B
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GameStartDone(ref ModEvents.SGameStartDoneData _data)
		{
			if (!MapRenderer.Enabled)
			{
				return;
			}
			MapRenderer instance = MapRenderer.Instance;
		}

		// Token: 0x0600CE43 RID: 52803 RVA: 0x004B1F8B File Offset: 0x004B018B
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GameShutdown(ref ModEvents.SGameShutdownData _data)
		{
			MapRenderer.Shutdown();
		}

		// Token: 0x0600CE44 RID: 52804 RVA: 0x004B1F92 File Offset: 0x004B0192
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CalcChunkColorsDone(ref ModEvents.SCalcChunkColorsDoneData _data)
		{
			MapRenderer.RenderSingleChunk(_data.Chunk);
		}

		// Token: 0x04009C87 RID: 40071
		[PublicizedFrom(EAccessModifier.Private)]
		public const string mapTilesBaseUrl = "/map/";
	}
}
