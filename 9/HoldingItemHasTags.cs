using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200067F RID: 1663
[Preserve]
public class HoldingItemHasTags : TargetedCompareRequirementBase
{
	// Token: 0x06003506 RID: 13574 RVA: 0x00160354 File Offset: 0x0015E554
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag;
		if (!this.hasAllTags)
		{
			flag = this.target.inventory.holdingItem.HasAnyTags(this.holdingItemTags);
		}
		else
		{
			flag = this.target.inventory.holdingItem.HasAllTags(this.holdingItemTags);
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x00160010 File Offset: 0x0015E210
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0}Male", this.invert ? "NOT " : ""));
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x001603C0 File Offset: 0x0015E5C0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.holdingItemTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
				return true;
			}
			if (localName == "has_all_tags")
			{
				this.hasAllTags = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002B38 RID: 11064
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> holdingItemTags;

	// Token: 0x04002B39 RID: 11065
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
