using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006AA RID: 1706
[Preserve]
public class HitLocation : TargetedCompareRequirementBase
{
	// Token: 0x0600358F RID: 13711 RVA: 0x0016221A File Offset: 0x0016041A
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return (this.bodyParts & _params.DamageResponse.HitBodyPart) > EnumBodyPartHit.None;
		}
		return (this.bodyParts & _params.DamageResponse.HitBodyPart) == EnumBodyPartHit.None;
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x0016225A File Offset: 0x0016045A
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0} hit location: ", this.invert ? "NOT " : "", this.bodyPartNames));
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x00162288 File Offset: 0x00160488
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "body_parts")
		{
			this.bodyPartNames = _attribute.Value;
			string[] array = this.bodyPartNames.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.bodyParts |= EnumUtils.Parse<EnumBodyPartHit>(array[i], true);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04002B5C RID: 11100
	[PublicizedFrom(EAccessModifier.Private)]
	public string bodyPartNames = "";

	// Token: 0x04002B5D RID: 11101
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumBodyPartHit bodyParts;
}
