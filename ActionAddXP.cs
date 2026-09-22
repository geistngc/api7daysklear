using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001969 RID: 6505
	[Preserve]
	public class ActionAddXP : ActionBaseClientAction
	{
		// Token: 0x0600C815 RID: 51221 RVA: 0x00498550 File Offset: 0x00496750
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.xpAmount = GameEventManager.GetIntValue(entityPlayer, this.xpAmountText, 0);
				if (this.xpAmount < 0)
				{
					this.xpAmount = 0;
				}
				entityPlayer.Progression.AddLevelExp(this.xpAmount, "_xpOther", Progression.XPTypes.Other, true, true, -1, null);
			}
		}

		// Token: 0x0600C816 RID: 51222 RVA: 0x004985A6 File Offset: 0x004967A6
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddXP.PropXPAmount, ref this.xpAmountText);
		}

		// Token: 0x0600C817 RID: 51223 RVA: 0x004985C0 File Offset: 0x004967C0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddXP
			{
				xpAmountText = this.xpAmountText
			};
		}

		// Token: 0x04009721 RID: 38689
		[PublicizedFrom(EAccessModifier.Protected)]
		public string xpAmountText;

		// Token: 0x04009722 RID: 38690
		[PublicizedFrom(EAccessModifier.Protected)]
		public int xpAmount;

		// Token: 0x04009723 RID: 38691
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropXPAmount = "xp_amount";
	}
}
