using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006B2 RID: 1714
[Preserve]
public class ArmorGroupLowestQuality : TargetedCompareRequirementBase
{
	// Token: 0x060035AC RID: 13740 RVA: 0x001628A8 File Offset: 0x00160AA8
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int armorGroupLowestQuality = this.target.equipment.GetArmorGroupLowestQuality(this.armorGroupName);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)armorGroupLowestQuality, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)armorGroupLowestQuality, this.operation, this.value);
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x00162909 File Offset: 0x00160B09
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("ArmorGroupLowestQuality: {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x00162948 File Offset: 0x00160B48
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "group_name")
		{
			this.armorGroupName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002B64 RID: 11108
	[PublicizedFrom(EAccessModifier.Private)]
	public string armorGroupName;
}
