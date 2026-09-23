using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000686 RID: 1670
[Preserve]
public class HasBuff : TargetedCompareRequirementBase
{
	// Token: 0x0600351E RID: 13598 RVA: 0x001608EC File Offset: 0x0015EAEC
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int num = this.buffNames.Length;
		for (int i = 0; i < num; i++)
		{
			if (this.target.Buffs.HasBuff(this.buffNames[i]))
			{
				return !this.invert;
			}
		}
		return this.invert;
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x00160944 File Offset: 0x0015EB44
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "buff")
		{
			this.buffNames = _attribute.Value.ToLower().Split(RequirementBase.commaSeparator);
			return true;
		}
		return flag;
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x00160991 File Offset: 0x0015EB91
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Target does {0}have buff '{1}(0)'", this.invert ? "NOT " : "", this.buffNames));
	}

	// Token: 0x04002B42 RID: 11074
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] buffNames;
}
