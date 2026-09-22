using System;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x0200185C RID: 6236
	[Preserve]
	public class BaseTwitchRequirement
	{
		// Token: 0x0600C0A5 RID: 49317 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600C0A6 RID: 49318 RVA: 0x00477703 File Offset: 0x00475903
		public void Init()
		{
			this.OnInit();
		}

		// Token: 0x0600C0A7 RID: 49319 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual bool CanPerform(Entity player)
		{
			return true;
		}

		// Token: 0x0600C0A8 RID: 49320 RVA: 0x0047770B File Offset: 0x0047590B
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			properties.ParseBool(BaseTwitchRequirement.PropInvert, ref this.Invert);
			properties.ParseBool(BaseTwitchRequirement.PropHidesAction, ref this.HideAction);
		}

		// Token: 0x0400918B RID: 37259
		public BaseTwitchRequirement.OwnerTypes OwnerType;

		// Token: 0x0400918C RID: 37260
		public TwitchAction OwnerAction;

		// Token: 0x0400918D RID: 37261
		public TwitchVote OwnerVote;

		// Token: 0x0400918E RID: 37262
		public BaseTwitchEventEntry OwnerEvent;

		// Token: 0x0400918F RID: 37263
		public DynamicProperties Properties;

		// Token: 0x04009190 RID: 37264
		public bool HideAction;

		// Token: 0x04009191 RID: 37265
		public bool Invert;

		// Token: 0x04009192 RID: 37266
		public static string PropInvert = "invert";

		// Token: 0x04009193 RID: 37267
		public static string PropHidesAction = "hide_action";

		// Token: 0x0200185D RID: 6237
		public enum OwnerTypes
		{
			// Token: 0x04009195 RID: 37269
			Action,
			// Token: 0x04009196 RID: 37270
			Vote,
			// Token: 0x04009197 RID: 37271
			Event
		}
	}
}
