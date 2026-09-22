using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200194D RID: 6477
	[Preserve]
	public class RequirementVarString : BaseOperationRequirement
	{
		// Token: 0x0600C791 RID: 51089 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C792 RID: 51090 RVA: 0x00495108 File Offset: 0x00493308
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			string empty = string.Empty;
			this.Owner.EventVariables.ParseVarString(this.varName, ref empty);
			return empty;
		}

		// Token: 0x0600C793 RID: 51091 RVA: 0x00495134 File Offset: 0x00493334
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return this.valueText;
		}

		// Token: 0x0600C794 RID: 51092 RVA: 0x0049513C File Offset: 0x0049333C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarString.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarString.PropValue, ref this.valueText);
		}

		// Token: 0x0600C795 RID: 51093 RVA: 0x00495167 File Offset: 0x00493367
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarString
			{
				Invert = this.Invert,
				operation = this.operation,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x04009681 RID: 38529
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x04009682 RID: 38530
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04009683 RID: 38531
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x04009684 RID: 38532
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
