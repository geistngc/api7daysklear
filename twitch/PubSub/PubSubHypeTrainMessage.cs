using System;
using UnityEngine;

namespace Twitch.PubSub
{
	// Token: 0x02001887 RID: 6279
	public class PubSubHypeTrainMessage : BasePubSubMessage
	{
		// Token: 0x0600C1AE RID: 49582 RVA: 0x0047C420 File Offset: 0x0047A620
		public static PubSubHypeTrainMessage Deserialize(string message)
		{
			Debug.LogWarning("HypeTrainMessage:\n" + message);
			return new PubSubHypeTrainMessage();
		}

		// Token: 0x040092D6 RID: 37590
		public PubSubHypeTrainMessage.HypeTrainData data;

		// Token: 0x02001888 RID: 6280
		public class HypeTrainData : EventArgs
		{
			// Token: 0x170017C6 RID: 6086
			// (get) Token: 0x0600C1B0 RID: 49584 RVA: 0x0047C437 File Offset: 0x0047A637
			// (set) Token: 0x0600C1B1 RID: 49585 RVA: 0x0047C43F File Offset: 0x0047A63F
			public string user_name { get; set; }
		}
	}
}
