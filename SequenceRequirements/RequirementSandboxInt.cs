using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001949 RID: 6473
	[Preserve]
	public class RequirementSandboxInt : BaseOperationRequirement
	{
		// Token: 0x0600C777 RID: 51063 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C778 RID: 51064 RVA: 0x00494E10 File Offset: 0x00493010
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return SandboxOptionManager.GetInt(this.Option);
		}

		// Token: 0x0600C779 RID: 51065 RVA: 0x00494E22 File Offset: 0x00493022
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600C77A RID: 51066 RVA: 0x00494E3B File Offset: 0x0049303B
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<SandboxOptions>(RequirementSandboxInt.PropSandboxOption, ref this.Option);
			properties.ParseString(RequirementSandboxInt.PropValue, ref this.valueText);
		}

		// Token: 0x0600C77B RID: 51067 RVA: 0x00494E66 File Offset: 0x00493066
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementSandboxInt
			{
				Invert = this.Invert,
				operation = this.operation,
				Option = this.Option,
				valueText = this.valueText
			};
		}

		// Token: 0x04009671 RID: 38513
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions Option = SandboxOptions.Max;

		// Token: 0x04009672 RID: 38514
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandboxOption = "option";

		// Token: 0x04009673 RID: 38515
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009674 RID: 38516
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
