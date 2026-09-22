using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192C RID: 6444
	[Preserve]
	public class RequirementGameStatBool : BaseRequirement
	{
		// Token: 0x0600C6EB RID: 50923 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6EC RID: 50924 RVA: 0x00493BB7 File Offset: 0x00491DB7
		public override bool CanPerform(Entity target)
		{
			if (GameStats.GetBool(this.GameStat))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C6ED RID: 50925 RVA: 0x00493BD6 File Offset: 0x00491DD6
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<EnumGameStats>(RequirementGameStatBool.PropGameStat, ref this.GameStat);
		}

		// Token: 0x0600C6EE RID: 50926 RVA: 0x00493BF0 File Offset: 0x00491DF0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGameStatBool
			{
				Invert = this.Invert,
				GameStat = this.GameStat
			};
		}

		// Token: 0x04009630 RID: 38448
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGameStats GameStat = EnumGameStats.AnimalCount;

		// Token: 0x04009631 RID: 38449
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameStat = "gamestat";
	}
}
