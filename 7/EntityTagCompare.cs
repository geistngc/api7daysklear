using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200068A RID: 1674
[Preserve]
public class EntityTagCompare : TargetedCompareRequirementBase
{
	// Token: 0x0600352C RID: 13612 RVA: 0x00160C80 File Offset: 0x0015EE80
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
				return this.target.HasAllTags(this.tagsToCompare);
			}
			return !this.target.HasAllTags(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.HasAnyTags(this.tagsToCompare);
			}
			return !this.target.HasAnyTags(this.tagsToCompare);
		}
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x00160D00 File Offset: 0x0015EF00
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

	// Token: 0x04002B48 RID: 11080
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x04002B49 RID: 11081
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
