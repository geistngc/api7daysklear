using System;
using UnityEngine.Scripting;

// Token: 0x02000685 RID: 1669
[Preserve]
public class CatapultStrainAmount : TargetedCompareRequirementBase
{
	// Token: 0x0600351C RID: 13596 RVA: 0x0016081C File Offset: 0x0015EA1C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		ItemValue itemValue = _params.ItemValue;
		if (itemValue == null)
		{
			return false;
		}
		ItemActionCatapult itemActionCatapult = itemValue.ItemClass.Actions[0] as ItemActionCatapult;
		if (itemActionCatapult != null)
		{
			float strainPercent = itemActionCatapult.GetStrainPercent(_params.ItemInventoryData.actionData[0]);
			if (this.invert)
			{
				return !RequirementBase.compareValues(strainPercent, this.operation, this.value);
			}
			return RequirementBase.compareValues(strainPercent, this.operation, this.value);
		}
		else
		{
			ItemActionLauncher.ItemActionDataLauncher itemActionDataLauncher = _params.ItemActionData as ItemActionLauncher.ItemActionDataLauncher;
			if (itemActionDataLauncher == null)
			{
				return false;
			}
			float lastAttackStrainPercent = itemActionDataLauncher.lastAttackStrainPercent;
			if (this.invert)
			{
				return !RequirementBase.compareValues(lastAttackStrainPercent, this.operation, this.value);
			}
			return RequirementBase.compareValues(lastAttackStrainPercent, this.operation, this.value);
		}
	}
}
