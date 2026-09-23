using System;
using Newtonsoft.Json;

// Token: 0x02000C8C RID: 3212
[Serializable]
public class EventSubMetadata
{
	// Token: 0x17000A33 RID: 2611
	// (get) Token: 0x060062B1 RID: 25265 RVA: 0x0026DFC0 File Offset: 0x0026C1C0
	// (set) Token: 0x060062B2 RID: 25266 RVA: 0x0026DFC8 File Offset: 0x0026C1C8
	[JsonProperty("message_id")]
	public string MessageId { get; set; } = string.Empty;

	// Token: 0x17000A34 RID: 2612
	// (get) Token: 0x060062B3 RID: 25267 RVA: 0x0026DFD1 File Offset: 0x0026C1D1
	// (set) Token: 0x060062B4 RID: 25268 RVA: 0x0026DFD9 File Offset: 0x0026C1D9
	[JsonProperty("message_type")]
	public string MessageType { get; set; } = string.Empty;

	// Token: 0x17000A35 RID: 2613
	// (get) Token: 0x060062B5 RID: 25269 RVA: 0x0026DFE2 File Offset: 0x0026C1E2
	// (set) Token: 0x060062B6 RID: 25270 RVA: 0x0026DFEA File Offset: 0x0026C1EA
	[JsonProperty("message_timestamp")]
	public string MessageTimestamp { get; set; } = string.Empty;
}
