using System;
using Newtonsoft.Json;

// Token: 0x02000C84 RID: 3204
public class BitsUsedEvent
{
	// Token: 0x17000A1C RID: 2588
	// (get) Token: 0x0600626E RID: 25198 RVA: 0x0026D4CC File Offset: 0x0026B6CC
	// (set) Token: 0x0600626F RID: 25199 RVA: 0x0026D4D4 File Offset: 0x0026B6D4
	[JsonProperty("is_anonymous")]
	public bool IsAnonymous { get; set; }

	// Token: 0x17000A1D RID: 2589
	// (get) Token: 0x06006270 RID: 25200 RVA: 0x0026D4DD File Offset: 0x0026B6DD
	// (set) Token: 0x06006271 RID: 25201 RVA: 0x0026D4E5 File Offset: 0x0026B6E5
	[JsonProperty("user_id")]
	public string UserId { get; set; } = "";

	// Token: 0x17000A1E RID: 2590
	// (get) Token: 0x06006272 RID: 25202 RVA: 0x0026D4EE File Offset: 0x0026B6EE
	// (set) Token: 0x06006273 RID: 25203 RVA: 0x0026D4F6 File Offset: 0x0026B6F6
	[JsonProperty("user_login")]
	public string UserLogin { get; set; } = "";

	// Token: 0x17000A1F RID: 2591
	// (get) Token: 0x06006274 RID: 25204 RVA: 0x0026D4FF File Offset: 0x0026B6FF
	// (set) Token: 0x06006275 RID: 25205 RVA: 0x0026D507 File Offset: 0x0026B707
	[JsonProperty("user_name")]
	public string UserName { get; set; } = "";

	// Token: 0x17000A20 RID: 2592
	// (get) Token: 0x06006276 RID: 25206 RVA: 0x0026D510 File Offset: 0x0026B710
	// (set) Token: 0x06006277 RID: 25207 RVA: 0x0026D518 File Offset: 0x0026B718
	[JsonProperty("broadcaster_user_id")]
	public string BroadcasterUserId { get; set; } = "";

	// Token: 0x17000A21 RID: 2593
	// (get) Token: 0x06006278 RID: 25208 RVA: 0x0026D521 File Offset: 0x0026B721
	// (set) Token: 0x06006279 RID: 25209 RVA: 0x0026D529 File Offset: 0x0026B729
	[JsonProperty("broadcaster_user_login")]
	public string BroadcasterUserLogin { get; set; } = "";

	// Token: 0x17000A22 RID: 2594
	// (get) Token: 0x0600627A RID: 25210 RVA: 0x0026D532 File Offset: 0x0026B732
	// (set) Token: 0x0600627B RID: 25211 RVA: 0x0026D53A File Offset: 0x0026B73A
	[JsonProperty("broadcaster_user_name")]
	public string BroadcasterUserName { get; set; } = "";

	// Token: 0x17000A23 RID: 2595
	// (get) Token: 0x0600627C RID: 25212 RVA: 0x0026D543 File Offset: 0x0026B743
	// (set) Token: 0x0600627D RID: 25213 RVA: 0x0026D54B File Offset: 0x0026B74B
	[JsonProperty("bits")]
	public int Bits { get; set; }
}
