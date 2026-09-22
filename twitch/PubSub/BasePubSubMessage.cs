using System;

namespace Twitch.PubSub
{
	// Token: 0x0200187C RID: 6268
	public class BasePubSubMessage
	{
		// Token: 0x170017AE RID: 6062
		// (get) Token: 0x0600C170 RID: 49520 RVA: 0x0047C233 File Offset: 0x0047A433
		// (set) Token: 0x0600C171 RID: 49521 RVA: 0x0047C23B File Offset: 0x0047A43B
		public string type { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170017AF RID: 6063
		// (get) Token: 0x0600C172 RID: 49522 RVA: 0x0047C244 File Offset: 0x0047A444
		// (set) Token: 0x0600C173 RID: 49523 RVA: 0x0047C24C File Offset: 0x0047A44C
		public string nonce { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

		// Token: 0x0600C174 RID: 49524 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void ReceiveData(string data)
		{
		}
	}
}
