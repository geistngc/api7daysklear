using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001947 RID: 6471
	[Preserve]
	public class RequirementSandboxBool : BaseRequirement
	{
		// Token: 0x0600C76B RID: 51051 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C76C RID: 51052 RVA: 0x00494CDF File Offset: 0x00492EDF
		public override bool CanPerform(Entity target)
		{
			if (SandboxOptionManager.GetBool(this.Option))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C76D RID: 51053 RVA: 0x00494CFE File Offset: 0x00492EFE
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<SandboxOptions>(RequirementSandboxBool.PropSandboxOption, ref this.Option);
		}

		// Token: 0x0600C76E RID: 51054 RVA: 0x00494D18 File Offset: 0x00492F18
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementSandboxBool
			{
				Invert = this.Invert,
				Option = this.Option
			};
		}

		// Token: 0x0400966B RID: 38507
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions Option = SandboxOptions.Max;

		// Token: 0x0400966C RID: 38508
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandboxOption = "option";
	}
}
