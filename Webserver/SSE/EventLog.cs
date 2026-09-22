using System;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.UrlHandlers;
using Webserver.WebAPI.APIs;

namespace Webserver.SSE
{
	// Token: 0x02001AFF RID: 6911
	[Preserve]
	public class EventLog : AbsEvent
	{
		// Token: 0x0600CFCA RID: 53194 RVA: 0x004BD0FA File Offset: 0x004BB2FA
		public EventLog(SseHandler _parent) : base(_parent, true, "log")
		{
			LogBuffer.EntryAdded += this.LogCallback;
		}

		// Token: 0x0600CFCB RID: 53195 RVA: 0x004BD11C File Offset: 0x004BB31C
		[PublicizedFrom(EAccessModifier.Private)]
		public void LogCallback(LogBuffer.LogEntry _logEntry)
		{
			JsonWriter jsonWriter = default(JsonWriter);
			LogApi.WriteLogMessageObject(ref jsonWriter, _logEntry);
			base.SendData("logLine", jsonWriter.ToString());
		}
	}
}
