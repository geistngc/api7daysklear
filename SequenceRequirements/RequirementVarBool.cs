using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200194A RID: 6474
	[Preserve]
	public class RequirementVarBool : BaseRequirement
	{
		// Token: 0x0600C77E RID: 51070 RVA: 0x00494EC8 File Offset: 0x004930C8
		public override bool CanPerform(Entity target)
		{
			if (target is EntityAlive)
			{
				bool flag = false;
				this.Owner.EventVariables.ParseBool(this.varName, ref flag);
				if (flag == StringParsers.ParseBool(this.valueText, 0, -1, true))
				{
					return !this.Invert;
				}
			}
			return this.Invert;
		}

		// Token: 0x0600C77F RID: 51071 RVA: 0x00494F18 File Offset: 0x00493118
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarBool.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarBool.PropValue, ref this.valueText);
		}

		// Token: 0x0600C780 RID: 51072 RVA: 0x00494F43 File Offset: 0x00493143
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarBool
			{
				Invert = this.Invert,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x04009675 RID: 38517
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x04009676 RID: 38518
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009677 RID: 38519
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x04009678 RID: 38520
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
