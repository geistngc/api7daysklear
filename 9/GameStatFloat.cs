using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006B7 RID: 1719
[Preserve]
public class GameStatFloat : TargetedCompareRequirementBase
{
	// Token: 0x060035BD RID: 13757 RVA: 0x00162C7C File Offset: 0x00160E7C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		float @float = GameStats.GetFloat(this.GameStat);
		if (this.invert)
		{
			return !RequirementBase.compareValues(@float, this.operation, this.value);
		}
		return RequirementBase.compareValues(@float, this.operation, this.value);
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x00162CD0 File Offset: 0x00160ED0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "gamestat")
		{
			this.GameStat = Enum.Parse<EnumGameStats>(_attribute.Value);
			return true;
		}
		return flag;
	}

	// Token: 0x04002B69 RID: 11113
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
