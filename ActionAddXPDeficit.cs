using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200196A RID: 6506
	[Preserve]
	public class ActionAddXPDeficit : ActionBaseClientAction
	{
		// Token: 0x0600C81A RID: 51226 RVA: 0x004985E0 File Offset: 0x004967E0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.xpAmount = GameEventManager.GetIntValue(entityPlayer, this.xpAmountText, 0);
				if (this.xpAmount == 0)
				{
					entityPlayer.Progression.AddXPDeficit();
					return;
				}
				entityPlayer.Progression.ExpDeficit += this.xpAmount;
			}
		}

		// Token: 0x0600C81B RID: 51227 RVA: 0x00498636 File Offset: 0x00496836
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddXPDeficit.PropXPAmount, ref this.xpAmountText);
		}

		// Token: 0x0600C81C RID: 51228 RVA: 0x00498650 File Offset: 0x00496850
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddXPDeficit
			{
				xpAmountText = this.xpAmountText
			};
		}

		// Token: 0x04009724 RID: 38692
		[PublicizedFrom(EAccessModifier.Protected)]
		public string xpAmountText;

		// Token: 0x04009725 RID: 38693
		[PublicizedFrom(EAccessModifier.Protected)]
		public int xpAmount;

		// Token: 0x04009726 RID: 38694
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropXPAmount = "xp_amount";
	}
}
