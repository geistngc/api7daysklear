using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200067E RID: 1662
[Preserve]
public class CompareItemMetaFloat : TargetedCompareRequirementBase
{
	// Token: 0x06003503 RID: 13571 RVA: 0x001602A0 File Offset: 0x0015E4A0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		ItemValue itemValue = _params.ItemValue;
		if (itemValue == null || string.IsNullOrEmpty(this.metaKey))
		{
			return false;
		}
		float valueA;
		if (!itemValue.TryGetMetadata(this.metaKey, out valueA))
		{
			return false;
		}
		if (this.invert)
		{
			return !RequirementBase.compareValues(valueA, this.operation, this.value);
		}
		return RequirementBase.compareValues(valueA, this.operation, this.value);
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x00160314 File Offset: 0x0015E514
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "key")
		{
			this.metaKey = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002B37 RID: 11063
	[PublicizedFrom(EAccessModifier.Private)]
	public string metaKey;
}
