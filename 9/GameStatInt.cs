using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006B6 RID: 1718
[Preserve]
public class GameStatInt : TargetedCompareRequirementBase
{
	// Token: 0x060035BA RID: 13754 RVA: 0x00162BD0 File Offset: 0x00160DD0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int @int = GameStats.GetInt(this.GameStat);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)@int, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)@int, this.operation, this.value);
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x00162C28 File Offset: 0x00160E28
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

	// Token: 0x04002B68 RID: 11112
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
