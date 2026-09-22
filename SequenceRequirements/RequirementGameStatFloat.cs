using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200192D RID: 6445
	[Preserve]
	public class RequirementGameStatFloat : BaseOperationRequirement
	{
		// Token: 0x0600C6F1 RID: 50929 RVA: 0x00493C2B File Offset: 0x00491E2B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return GameStats.GetFloat(this.GameStat);
		}

		// Token: 0x0600C6F2 RID: 50930 RVA: 0x00493C3D File Offset: 0x00491E3D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600C6F3 RID: 50931 RVA: 0x00493C5A File Offset: 0x00491E5A
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<EnumGameStats>(RequirementGameStatFloat.PropGameStat, ref this.GameStat);
			properties.ParseString(RequirementGameStatFloat.PropValue, ref this.valueText);
		}

		// Token: 0x0600C6F4 RID: 50932 RVA: 0x00493C85 File Offset: 0x00491E85
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGameStatFloat
			{
				Invert = this.Invert,
				operation = this.operation,
				GameStat = this.GameStat,
				valueText = this.valueText
			};
		}

		// Token: 0x04009632 RID: 38450
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGameStats GameStat = EnumGameStats.AnimalCount;

		// Token: 0x04009633 RID: 38451
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009634 RID: 38452
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameStat = "gamestat";

		// Token: 0x04009635 RID: 38453
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
