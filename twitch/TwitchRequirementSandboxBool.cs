using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x02001861 RID: 6241
	[Preserve]
	public class TwitchRequirementSandboxBool : BaseTwitchOperationRequirement
	{
		// Token: 0x0600C0BA RID: 49338 RVA: 0x00477902 File Offset: 0x00475B02
		public override bool CanPerform(Entity target)
		{
			if (SandboxOptionManager.GetBool(this.sandboxOption))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600C0BB RID: 49339 RVA: 0x00477924 File Offset: 0x00475B24
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string value = "";
			properties.ParseString(TwitchRequirementSandboxBool.PropSandbox, ref value);
			this.sandboxOption = Enum.Parse<SandboxOptions>(value);
		}

		// Token: 0x0400919F RID: 37279
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions sandboxOption;

		// Token: 0x040091A0 RID: 37280
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandbox = "sandbox_option";
	}
}
