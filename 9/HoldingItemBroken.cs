using System;
using UnityEngine.Scripting;

// Token: 0x020006A0 RID: 1696
[Preserve]
public class HoldingItemBroken : TargetedCompareRequirementBase
{
	// Token: 0x06003573 RID: 13683 RVA: 0x00161B84 File Offset: 0x0015FD84
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target == null)
		{
			return false;
		}
		bool flag = this.target.inventory.holdingItemItemValue.PercentUsesLeft <= 0f;
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}
}
