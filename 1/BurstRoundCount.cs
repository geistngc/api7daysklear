using System;
using System.Collections.Generic;

// Token: 0x020006A5 RID: 1701
public class BurstRoundCount : TargetedCompareRequirementBase
{
	// Token: 0x0600357F RID: 13695 RVA: 0x00161EC0 File Offset: 0x001600C0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.ItemValue.IsEmpty())
		{
			return false;
		}
		ItemActionRanged itemActionRanged = _params.ItemValue.ItemClass.Actions[0] as ItemActionRanged;
		if (itemActionRanged == null)
		{
			return false;
		}
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)itemActionRanged.GetBurstCount(this.target.inventory.holdingItemData.actionData[0]), this.operation, this.value);
		}
		return RequirementBase.compareValues((float)itemActionRanged.GetBurstCount(this.target.inventory.holdingItemData.actionData[0]), this.operation, this.value);
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00161E82 File Offset: 0x00160082
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Rounds in Magazine: {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}
}
