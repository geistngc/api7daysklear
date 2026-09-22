using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192E RID: 6446
	[Preserve]
	public class RequirementGameStatInt : BaseOperationRequirement
	{
		// Token: 0x0600C6F7 RID: 50935 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C6F8 RID: 50936 RVA: 0x00493CE2 File Offset: 0x00491EE2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return GameStats.GetInt(this.GameStat);
		}

		// Token: 0x0600C6F9 RID: 50937 RVA: 0x00493CF4 File Offset: 0x00491EF4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600C6FA RID: 50938 RVA: 0x00493D0D File Offset: 0x00491F0D
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<EnumGameStats>(RequirementGameStatInt.PropGameStat, ref this.GameStat);
			properties.ParseString(RequirementGameStatInt.PropValue, ref this.valueText);
		}

		// Token: 0x0600C6FB RID: 50939 RVA: 0x00493D38 File Offset: 0x00491F38
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGameStatInt
			{
				Invert = this.Invert,
				operation = this.operation,
				GameStat = this.GameStat,
				valueText = this.valueText
			};
		}

		// Token: 0x04009636 RID: 38454
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGameStats GameStat = EnumGameStats.AnimalCount;

		// Token: 0x04009637 RID: 38455
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009638 RID: 38456
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameStat = "gamestat";

		// Token: 0x04009639 RID: 38457
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
