using System;
using Webserver.UrlHandlers;

namespace Webserver
{
	// Token: 0x02001ACD RID: 6861
	public static class WebServer
	{
		// Token: 0x0600CEA6 RID: 52902 RVA: 0x004B6550 File Offset: 0x004B4750
		public static void Init()
		{
			ModEvents.GameStartDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(WebServer.GameStartDone));
			ModEvents.WorldShuttingDown.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SWorldShuttingDownData>(WebServer.WorldShuttingDown));
		}

		// Token: 0x0600CEA7 RID: 52903 RVA: 0x004B657E File Offset: 0x004B477E
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GameStartDone(ref ModEvents.SGameStartDoneData _data)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return;
			}
			WebServer.webInstance = new Web();
			LogBuffer.Init();
			if (ItemIconHandler.Instance != null)
			{
				ThreadManager.StartCoroutine(ItemIconHandler.Instance.LoadIcons());
			}
		}

		// Token: 0x0600CEA8 RID: 52904 RVA: 0x004B65B3 File Offset: 0x004B47B3
		[PublicizedFrom(EAccessModifier.Private)]
		public static void WorldShuttingDown(ref ModEvents.SWorldShuttingDownData _data)
		{
			Web web = WebServer.webInstance;
			if (web == null)
			{
				return;
			}
			web.Disconnect();
		}

		// Token: 0x04009CEC RID: 40172
		[PublicizedFrom(EAccessModifier.Private)]
		public static Web webInstance;
	}
}
