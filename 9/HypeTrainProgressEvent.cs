using System;
using Newtonsoft.Json;

// Token: 0x02000C8D RID: 3213
public class HypeTrainProgressEvent
{
	// Token: 0x17000A36 RID: 2614
	// (get) Token: 0x060062B8 RID: 25272 RVA: 0x0026E01C File Offset: 0x0026C21C
	// (set) Token: 0x060062B9 RID: 25273 RVA: 0x0026E024 File Offset: 0x0026C224
	[JsonProperty("level")]
	public int Level { get; set; }
}
