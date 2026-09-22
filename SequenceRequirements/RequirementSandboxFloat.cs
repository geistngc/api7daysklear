using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001948 RID: 6472
	[Preserve]
	public class RequirementSandboxFloat : BaseOperationRequirement
	{
		// Token: 0x0600C771 RID: 51057 RVA: 0x00494D56 File Offset: 0x00492F56
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return SandboxOptionManager.GetFloat(this.Option);
		}

		// Token: 0x0600C772 RID: 51058 RVA: 0x00494D68 File Offset: 0x00492F68
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600C773 RID: 51059 RVA: 0x00494D85 File Offset: 0x00492F85
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<SandboxOptions>(RequirementSandboxFloat.PropSandboxOption, ref this.Option);
			properties.ParseString(RequirementSandboxFloat.PropValue, ref this.valueText);
		}

		// Token: 0x0600C774 RID: 51060 RVA: 0x00494DB0 File Offset: 0x00492FB0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementSandboxFloat
			{
				Invert = this.Invert,
				operation = this.operation,
				Option = this.Option,
				valueText = this.valueText
			};
		}

		// Token: 0x0400966D RID: 38509
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions Option = SandboxOptions.Max;

		// Token: 0x0400966E RID: 38510
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400966F RID: 38511
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandboxOption = "option";

		// Token: 0x04009670 RID: 38512
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
