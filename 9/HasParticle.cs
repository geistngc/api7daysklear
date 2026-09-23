using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006AB RID: 1707
[Preserve]
public class HasParticle : TargetedCompareRequirementBase
{
	// Token: 0x06003593 RID: 13715 RVA: 0x0016230C File Offset: 0x0016050C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return _params.Self.HasParticle(this.particleName);
		}
		return !_params.Self.HasParticle(this.particleName);
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00162348 File Offset: 0x00160548
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "particle")
		{
			this.particleName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002B5E RID: 11102
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleName = "";
}
