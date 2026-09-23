using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000688 RID: 1672
[Preserve]
public class EntityHasMovementTag : TargetedCompareRequirementBase
{
	// Token: 0x06003526 RID: 13606 RVA: 0x00160A80 File Offset: 0x0015EC80
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.hasAllTags)
		{
			if (!this.invert)
			{
				return this.target.CurrentMovementTag.Test_AllSet(this.tagsToCompare);
			}
			return !this.target.CurrentMovementTag.Test_AllSet(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.CurrentMovementTag.Test_AnySet(this.tagsToCompare);
			}
			return !this.target.CurrentMovementTag.Test_AnySet(this.tagsToCompare);
		}
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00160B14 File Offset: 0x0015ED14
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.tagsToCompare = FastTags<TagGroup.Global>.Parse(_attribute.Value);
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

	// Token: 0x04002B44 RID: 11076
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x04002B45 RID: 11077
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
