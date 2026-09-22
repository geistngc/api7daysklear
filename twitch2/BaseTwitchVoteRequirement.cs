using System;

namespace Twitch
{
	// Token: 0x02001878 RID: 6264
	public class BaseTwitchVoteRequirement
	{
		// Token: 0x0600C15B RID: 49499 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600C15C RID: 49500 RVA: 0x0047C074 File Offset: 0x0047A274
		public void Init()
		{
			this.OnInit();
		}

		// Token: 0x0600C15D RID: 49501 RVA: 0x0002003D File Offset: 0x0001E23D
		public virtual bool CanPerform(EntityPlayer player)
		{
			return true;
		}

		// Token: 0x0600C15E RID: 49502 RVA: 0x0047C07C File Offset: 0x0047A27C
		public virtual void ParseProperties(DynamicProperties properties)
		{
			if (properties.Values.ContainsKey(BaseTwitchVoteRequirement.PropInvert))
			{
				this.Invert = StringParsers.ParseBool(properties.Values[BaseTwitchVoteRequirement.PropInvert], 0, -1, true);
			}
		}

		// Token: 0x040092B3 RID: 37555
		public TwitchVote Owner;

		// Token: 0x040092B4 RID: 37556
		public bool Invert;

		// Token: 0x040092B5 RID: 37557
		public static string PropInvert = "invert";
	}
}
