using System;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x0200185F RID: 6239
	[Preserve]
	public class TwitchRequirementHasProgression : BaseTwitchOperationRequirement
	{
		// Token: 0x0600C0B1 RID: 49329 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C0B2 RID: 49330 RVA: 0x00477800 File Offset: 0x00475A00
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity entity)
		{
			EntityPlayer entityPlayer = entity as EntityPlayer;
			if (entityPlayer != null)
			{
				return entityPlayer.Progression.GetProgressionValue(this.SkillName).CalculatedLevel(entityPlayer);
			}
			return 0;
		}

		// Token: 0x0600C0B3 RID: 49331 RVA: 0x0047783A File Offset: 0x00475A3A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity entity)
		{
			return this.Level;
		}

		// Token: 0x0600C0B4 RID: 49332 RVA: 0x00477847 File Offset: 0x00475A47
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool CheckPerk(EntityPlayer player, string buffName)
		{
			if (player.Progression.GetProgressionValue(buffName).CalculatedLevel(player) >= this.Level)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C0B5 RID: 49333 RVA: 0x00477873 File Offset: 0x00475A73
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(TwitchRequirementHasProgression.PropSkillName, ref this.SkillName);
			properties.ParseInt(TwitchRequirementHasProgression.PropLevel, ref this.Level);
		}

		// Token: 0x0400919B RID: 37275
		[PublicizedFrom(EAccessModifier.Protected)]
		public string SkillName = "";

		// Token: 0x0400919C RID: 37276
		[PublicizedFrom(EAccessModifier.Protected)]
		public int Level = 1;

		// Token: 0x0400919D RID: 37277
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSkillName = "skill_name";

		// Token: 0x0400919E RID: 37278
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropLevel = "level";
	}
}
