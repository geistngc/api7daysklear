using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x02001862 RID: 6242
	[Preserve]
	public class TwitchRequirementSandboxFloat : BaseTwitchOperationRequirement
	{
		// Token: 0x0600C0BE RID: 49342 RVA: 0x0047796B File Offset: 0x00475B6B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return SandboxOptionManager.GetFloat(this.sandboxOption);
		}

		// Token: 0x0600C0BF RID: 49343 RVA: 0x0047797D File Offset: 0x00475B7D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return this.value;
		}

		// Token: 0x0600C0C0 RID: 49344 RVA: 0x0047798C File Offset: 0x00475B8C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string text = "";
			properties.ParseString(TwitchRequirementSandboxFloat.PropSandbox, ref text);
			this.sandboxOption = Enum.Parse<SandboxOptions>(text);
			properties.ParseFloat(TwitchRequirementSandboxFloat.PropValue, ref this.value);
		}

		// Token: 0x040091A1 RID: 37281
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions sandboxOption;

		// Token: 0x040091A2 RID: 37282
		[PublicizedFrom(EAccessModifier.Protected)]
		public float value;

		// Token: 0x040091A3 RID: 37283
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandbox = "sandbox_option";

		// Token: 0x040091A4 RID: 37284
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
