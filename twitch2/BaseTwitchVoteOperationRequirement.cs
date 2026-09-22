using System;

namespace Twitch
{
	// Token: 0x02001876 RID: 6262
	public class BaseTwitchVoteOperationRequirement : BaseTwitchVoteRequirement
	{
		// Token: 0x0600C154 RID: 49492 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600C155 RID: 49493 RVA: 0x0047BFA8 File Offset: 0x0047A1A8
		public override bool CanPerform(EntityPlayer player)
		{
			float num = this.LeftSide(player);
			float num2 = this.RightSide(player);
			switch (this.operation)
			{
			case BaseTwitchVoteOperationRequirement.OperationTypes.Equals:
			case BaseTwitchVoteOperationRequirement.OperationTypes.EQ:
			case BaseTwitchVoteOperationRequirement.OperationTypes.E:
				return num == num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.NotEquals:
			case BaseTwitchVoteOperationRequirement.OperationTypes.NEQ:
			case BaseTwitchVoteOperationRequirement.OperationTypes.NE:
				return num != num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.Less:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessThan:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LT:
				return num < num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.Greater:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterThan:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GT:
				return num > num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessOrEqual:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessThanOrEqualTo:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LTE:
				return num <= num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterOrEqual:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterThanOrEqualTo:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GTE:
				return num >= num2;
			default:
				return true;
			}
		}

		// Token: 0x0600C156 RID: 49494 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float LeftSide(EntityPlayer player)
		{
			return 0f;
		}

		// Token: 0x0600C157 RID: 49495 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float RightSide(EntityPlayer player)
		{
			return 0f;
		}

		// Token: 0x0600C158 RID: 49496 RVA: 0x0047C046 File Offset: 0x0047A246
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BaseTwitchVoteOperationRequirement.OperationTypes>(BaseTwitchVoteOperationRequirement.PropOperation, ref this.operation);
		}

		// Token: 0x0400929D RID: 37533
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseTwitchVoteOperationRequirement.OperationTypes operation;

		// Token: 0x0400929E RID: 37534
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x02001877 RID: 6263
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OperationTypes
		{
			// Token: 0x040092A0 RID: 37536
			None,
			// Token: 0x040092A1 RID: 37537
			Equals,
			// Token: 0x040092A2 RID: 37538
			EQ,
			// Token: 0x040092A3 RID: 37539
			E,
			// Token: 0x040092A4 RID: 37540
			NotEquals,
			// Token: 0x040092A5 RID: 37541
			NEQ,
			// Token: 0x040092A6 RID: 37542
			NE,
			// Token: 0x040092A7 RID: 37543
			Less,
			// Token: 0x040092A8 RID: 37544
			LessThan,
			// Token: 0x040092A9 RID: 37545
			LT,
			// Token: 0x040092AA RID: 37546
			Greater,
			// Token: 0x040092AB RID: 37547
			GreaterThan,
			// Token: 0x040092AC RID: 37548
			GT,
			// Token: 0x040092AD RID: 37549
			LessOrEqual,
			// Token: 0x040092AE RID: 37550
			LessThanOrEqualTo,
			// Token: 0x040092AF RID: 37551
			LTE,
			// Token: 0x040092B0 RID: 37552
			GreaterOrEqual,
			// Token: 0x040092B1 RID: 37553
			GreaterThanOrEqualTo,
			// Token: 0x040092B2 RID: 37554
			GTE
		}
	}
}
