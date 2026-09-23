using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Token: 0x02000C8B RID: 3211
[Serializable]
public class EventSubMessage
{
	// Token: 0x17000A31 RID: 2609
	// (get) Token: 0x060062AC RID: 25260 RVA: 0x0026DF8B File Offset: 0x0026C18B
	// (set) Token: 0x060062AD RID: 25261 RVA: 0x0026DF93 File Offset: 0x0026C193
	[JsonProperty("metadata")]
	public EventSubMetadata Metadata { get; set; } = new EventSubMetadata();

	// Token: 0x17000A32 RID: 2610
	// (get) Token: 0x060062AE RID: 25262 RVA: 0x0026DF9C File Offset: 0x0026C19C
	// (set) Token: 0x060062AF RID: 25263 RVA: 0x0026DFA4 File Offset: 0x0026C1A4
	[JsonProperty("payload")]
	public JObject Payload { get; set; }
}
