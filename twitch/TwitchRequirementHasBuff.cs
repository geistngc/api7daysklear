using System;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x0200185E RID: 6238
	[Preserve]
	public class TwitchRequirementHasBuff : BaseTwitchRequirement
	{
		// Token: 0x0600C0AB RID: 49323 RVA: 0x0047774C File Offset: 0x0047594C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.BuffList = this.BuffName.Split(',', StringSplitOptions.None);
		}

		// Token: 0x0600C0AC RID: 49324 RVA: 0x00477764 File Offset: 0x00475964
		public override bool CanPerform(Entity entity)
		{
			EntityPlayer entityPlayer = entity as EntityPlayer;
			if (entityPlayer != null)
			{
				for (int i = 0; i < this.BuffList.Length; i++)
				{
					if (!this.CheckBuff(entityPlayer, this.BuffList[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600C0AD RID: 49325 RVA: 0x004777A4 File Offset: 0x004759A4
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool CheckBuff(EntityPlayer player, string buffName)
		{
			if (player.Buffs.HasBuff(buffName))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C0AE RID: 49326 RVA: 0x004777C4 File Offset: 0x004759C4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(TwitchRequirementHasBuff.PropBuffName, ref this.BuffName);
		}

		// Token: 0x04009198 RID: 37272
		[PublicizedFrom(EAccessModifier.Protected)]
		public string BuffName = "";

		// Token: 0x04009199 RID: 37273
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] BuffList;

		// Token: 0x0400919A RID: 37274
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBuffName = "buff_name";
	}
}
