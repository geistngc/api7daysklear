using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020006A9 RID: 1705
[Preserve]
public class BlockStandingOn : TargetedCompareRequirementBase
{
	// Token: 0x0600358B RID: 13707 RVA: 0x00162144 File Offset: 0x00160344
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag;
		if (this.hasAllTags)
		{
			flag = this.target.blockValueStandingOn.Block.HasAllFastTags(this.blockTags);
		}
		else
		{
			flag = this.target.blockValueStandingOn.Block.HasAnyFastTags(this.blockTags);
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x0600358C RID: 13708 RVA: 0x00160010 File Offset: 0x0015E210
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0}Male", this.invert ? "NOT " : ""));
	}

	// Token: 0x0600358D RID: 13709 RVA: 0x001621B0 File Offset: 0x001603B0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.blockTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
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

	// Token: 0x04002B5A RID: 11098
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> blockTags;

	// Token: 0x04002B5B RID: 11099
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
