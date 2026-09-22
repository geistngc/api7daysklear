using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200194B RID: 6475
	[Preserve]
	public class RequirementVarFloat : BaseOperationRequirement
	{
		// Token: 0x0600C783 RID: 51075 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C784 RID: 51076 RVA: 0x00494F84 File Offset: 0x00493184
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			int num = 0;
			this.Owner.EventVariables.ParseVarInt(this.varName, ref num);
			return num;
		}

		// Token: 0x0600C785 RID: 51077 RVA: 0x00494FB1 File Offset: 0x004931B1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600C786 RID: 51078 RVA: 0x00494FCE File Offset: 0x004931CE
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarFloat.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarFloat.PropValue, ref this.valueText);
		}

		// Token: 0x0600C787 RID: 51079 RVA: 0x00494FF9 File Offset: 0x004931F9
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarFloat
			{
				Invert = this.Invert,
				operation = this.operation,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x04009679 RID: 38521
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x0400967A RID: 38522
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400967B RID: 38523
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x0400967C RID: 38524
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
