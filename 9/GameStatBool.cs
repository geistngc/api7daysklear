using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006B5 RID: 1717
[Preserve]
public class GameStatBool : RequirementBase
{
	// Token: 0x060035B7 RID: 13751 RVA: 0x00162B52 File Offset: 0x00160D52
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (GameStats.GetBool(this.GameStat))
		{
			return !this.invert;
		}
		return this.invert;
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x00162B7C File Offset: 0x00160D7C
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

	// Token: 0x04002B67 RID: 11111
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
