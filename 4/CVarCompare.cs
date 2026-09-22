using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200066A RID: 1642
[Preserve]
public class CVarCompare : TargetedCompareRequirementBase
{
	// Token: 0x060034D9 RID: 13529 RVA: 0x0015F7F0 File Offset: 0x0015D9F0
	public override bool IsValid(MinEventParams _params)
	{
		return base.IsValid(_params) && this.invert != RequirementBase.compareValues(this.target.Buffs.GetCustomVar(this.cvarCompareName), this.operation, this.value);
	}

	// Token: 0x060034DA RID: 13530 RVA: 0x0015F82F File Offset: 0x0015DA2F
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("cvar.{0} {1} {2}", this.cvarCompareName, this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x060034DB RID: 13531 RVA: 0x0015F860 File Offset: 0x0015DA60
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "cvar")
		{
			this.cvarCompareName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002B23 RID: 11043
	[PublicizedFrom(EAccessModifier.Protected)]
	public string cvarCompareName;
}
