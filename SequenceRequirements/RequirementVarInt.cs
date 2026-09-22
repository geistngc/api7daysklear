using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200194C RID: 6476
	[Preserve]
	public class RequirementVarInt : BaseOperationRequirement
	{
		// Token: 0x0600C78A RID: 51082 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C78B RID: 51083 RVA: 0x00495048 File Offset: 0x00493248
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object LeftSide(Entity target)
		{
			int num = 0;
			this.Owner.EventVariables.ParseVarInt(this.varName, ref num);
			return num;
		}

		// Token: 0x0600C78C RID: 51084 RVA: 0x00495075 File Offset: 0x00493275
		[PublicizedFrom(EAccessModifier.Protected)]
		public override object RightSide(Entity target)
		{
			return GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600C78D RID: 51085 RVA: 0x0049508E File Offset: 0x0049328E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarInt.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarInt.PropValue, ref this.valueText);
		}

		// Token: 0x0600C78E RID: 51086 RVA: 0x004950B9 File Offset: 0x004932B9
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarInt
			{
				Invert = this.Invert,
				operation = this.operation,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x0400967D RID: 38525
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x0400967E RID: 38526
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400967F RID: 38527
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x04009680 RID: 38528
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
