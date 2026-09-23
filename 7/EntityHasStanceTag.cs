using System;
using System.Xml.Linq;

// Token: 0x02000689 RID: 1673
public class EntityHasStanceTag : TargetedCompareRequirementBase
{
	// Token: 0x06003529 RID: 13609 RVA: 0x00160B80 File Offset: 0x0015ED80
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
				return this.target.CurrentStanceTag.Test_AllSet(this.tagsToCompare);
			}
			return !this.target.CurrentStanceTag.Test_AllSet(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.CurrentStanceTag.Test_AnySet(this.tagsToCompare);
			}
			return !this.target.CurrentStanceTag.Test_AnySet(this.tagsToCompare);
		}
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x00160C14 File Offset: 0x0015EE14
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

	// Token: 0x04002B46 RID: 11078
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x04002B47 RID: 11079
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
