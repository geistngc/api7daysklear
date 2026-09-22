using System;
using SandboxOptions;
using UnityEngine.Scripting;

namespace Twitch
{
	// Token: 0x02001863 RID: 6243
	[Preserve]
	public class TwitchRequirementSandboxInt : BaseTwitchOperationRequirement
	{
		// Token: 0x0600C0C3 RID: 49347 RVA: 0x004779E6 File Offset: 0x00475BE6
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			return SandboxOptionManager.GetFloat(this.sandboxOption);
		}

		// Token: 0x0600C0C4 RID: 49348 RVA: 0x004779F8 File Offset: 0x00475BF8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return this.value;
		}

		// Token: 0x0600C0C5 RID: 49349 RVA: 0x00477A08 File Offset: 0x00475C08
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string text = "";
			properties.ParseString(TwitchRequirementSandboxInt.PropSandbox, ref text);
			this.sandboxOption = Enum.Parse<SandboxOptions>(text);
			properties.ParseInt(TwitchRequirementSandboxInt.PropValue, ref this.value);
		}

		// Token: 0x040091A5 RID: 37285
		[PublicizedFrom(EAccessModifier.Protected)]
		public SandboxOptions sandboxOption;

		// Token: 0x040091A6 RID: 37286
		[PublicizedFrom(EAccessModifier.Protected)]
		public int value;

		// Token: 0x040091A7 RID: 37287
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSandbox = "sandbox_option";

		// Token: 0x040091A8 RID: 37288
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
