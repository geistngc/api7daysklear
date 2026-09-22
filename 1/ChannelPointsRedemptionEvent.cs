using System;
using Newtonsoft.Json;

// Token: 0x02000C85 RID: 3205
public class ChannelPointsRedemptionEvent
{
	// Token: 0x17000A24 RID: 2596
	// (get) Token: 0x0600627F RID: 25215 RVA: 0x0026D5A9 File Offset: 0x0026B7A9
	// (set) Token: 0x06006280 RID: 25216 RVA: 0x0026D5B1 File Offset: 0x0026B7B1
	[JsonProperty("user_id")]
	public string UserId { get; set; } = "";

	// Token: 0x17000A25 RID: 2597
	// (get) Token: 0x06006281 RID: 25217 RVA: 0x0026D5BA File Offset: 0x0026B7BA
	// (set) Token: 0x06006282 RID: 25218 RVA: 0x0026D5C2 File Offset: 0x0026B7C2
	[JsonProperty("user_login")]
	public string UserLogin { get; set; } = "";

	// Token: 0x17000A26 RID: 2598
	// (get) Token: 0x06006283 RID: 25219 RVA: 0x0026D5CB File Offset: 0x0026B7CB
	// (set) Token: 0x06006284 RID: 25220 RVA: 0x0026D5D3 File Offset: 0x0026B7D3
	[JsonProperty("user_name")]
	public string UserName { get; set; } = "";

	// Token: 0x17000A27 RID: 2599
	// (get) Token: 0x06006285 RID: 25221 RVA: 0x0026D5DC File Offset: 0x0026B7DC
	// (set) Token: 0x06006286 RID: 25222 RVA: 0x0026D5E4 File Offset: 0x0026B7E4
	[JsonProperty("reward")]
	public Reward Reward { get; set; } = new Reward();
}
