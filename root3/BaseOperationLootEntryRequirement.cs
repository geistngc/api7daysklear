using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000623 RID: 1571
[Preserve]
public class BaseOperationLootEntryRequirement : BaseLootEntryRequirement
{
	// Token: 0x0600330A RID: 13066 RVA: 0x0014CC7C File Offset: 0x0014AE7C
	public override void Init(XElement e)
	{
		base.Init(e);
		string value = "";
		if (e.ParseAttribute("operation", ref value))
		{
			this.operation = Enum.Parse<BaseOperationLootEntryRequirement.OperationTypes>(value);
		}
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x0014CCB8 File Offset: 0x0014AEB8
	public override bool CheckRequirement(EntityPlayer player)
	{
		float num = this.LeftSide(player);
		float num2 = this.RightSide(player);
		switch (this.operation)
		{
		case BaseOperationLootEntryRequirement.OperationTypes.Equals:
		case BaseOperationLootEntryRequirement.OperationTypes.EQ:
		case BaseOperationLootEntryRequirement.OperationTypes.E:
			return num == num2;
		case BaseOperationLootEntryRequirement.OperationTypes.NotEquals:
		case BaseOperationLootEntryRequirement.OperationTypes.NEQ:
		case BaseOperationLootEntryRequirement.OperationTypes.NE:
			return num != num2;
		case BaseOperationLootEntryRequirement.OperationTypes.Less:
		case BaseOperationLootEntryRequirement.OperationTypes.LessThan:
		case BaseOperationLootEntryRequirement.OperationTypes.LT:
			return num < num2;
		case BaseOperationLootEntryRequirement.OperationTypes.Greater:
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterThan:
		case BaseOperationLootEntryRequirement.OperationTypes.GT:
			return num > num2;
		case BaseOperationLootEntryRequirement.OperationTypes.LessOrEqual:
		case BaseOperationLootEntryRequirement.OperationTypes.LessThanOrEqualTo:
		case BaseOperationLootEntryRequirement.OperationTypes.LTE:
			return num <= num2;
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterOrEqual:
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterThanOrEqualTo:
		case BaseOperationLootEntryRequirement.OperationTypes.GTE:
			return num >= num2;
		default:
			return true;
		}
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float LeftSide(EntityPlayer player)
	{
		return 0f;
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float RightSide(EntityPlayer player)
	{
		return 0f;
	}

	// Token: 0x040028C2 RID: 10434
	[PublicizedFrom(EAccessModifier.Protected)]
	public BaseOperationLootEntryRequirement.OperationTypes operation = BaseOperationLootEntryRequirement.OperationTypes.Equals;

	// Token: 0x040028C3 RID: 10435
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOperation = "operation";

	// Token: 0x02000624 RID: 1572
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum OperationTypes
	{
		// Token: 0x040028C5 RID: 10437
		None,
		// Token: 0x040028C6 RID: 10438
		Equals,
		// Token: 0x040028C7 RID: 10439
		EQ,
		// Token: 0x040028C8 RID: 10440
		E,
		// Token: 0x040028C9 RID: 10441
		NotEquals,
		// Token: 0x040028CA RID: 10442
		NEQ,
		// Token: 0x040028CB RID: 10443
		NE,
		// Token: 0x040028CC RID: 10444
		Less,
		// Token: 0x040028CD RID: 10445
		LessThan,
		// Token: 0x040028CE RID: 10446
		LT,
		// Token: 0x040028CF RID: 10447
		Greater,
		// Token: 0x040028D0 RID: 10448
		GreaterThan,
		// Token: 0x040028D1 RID: 10449
		GT,
		// Token: 0x040028D2 RID: 10450
		LessOrEqual,
		// Token: 0x040028D3 RID: 10451
		LessThanOrEqualTo,
		// Token: 0x040028D4 RID: 10452
		LTE,
		// Token: 0x040028D5 RID: 10453
		GreaterOrEqual,
		// Token: 0x040028D6 RID: 10454
		GreaterThanOrEqualTo,
		// Token: 0x040028D7 RID: 10455
		GTE
	}
}
