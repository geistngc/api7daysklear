using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020006B4 RID: 1716
[Preserve]
public class HasAttachedPrefab : TargetedCompareRequirementBase
{
	// Token: 0x060035B3 RID: 13747 RVA: 0x001629F0 File Offset: 0x00160BF0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		Transform transform = null;
		if (this.parent_transform_path != null)
		{
			transform = GameUtils.FindDeepChildActive(_params.Self.RootTransform, this.parent_transform_path);
		}
		Transform x;
		if (transform == null)
		{
			x = GameUtils.FindDeepChildActive(_params.Self.RootTransform, "tempPrefab_" + this.prefabName);
		}
		else
		{
			x = GameUtils.FindDeepChildActive(transform, "tempPrefab_" + this.prefabName);
		}
		if (x != null)
		{
			return !this.invert;
		}
		return this.invert;
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x00162A87 File Offset: 0x00160C87
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Does {0}Have Attached Prefab", this.invert ? "NOT " : ""));
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x00162AB0 File Offset: 0x00160CB0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "prefab" || localName == "prefab_name")
			{
				this.prefabName = _attribute.Value;
				if (this.prefabName.Contains("/"))
				{
					this.prefabName = this.prefabName.Substring(this.prefabName.LastIndexOf("/") + 1);
				}
				return true;
			}
			if (localName == "parent_transform")
			{
				this.parent_transform_path = _attribute.Value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002B65 RID: 11109
	[PublicizedFrom(EAccessModifier.Private)]
	public string prefabName;

	// Token: 0x04002B66 RID: 11110
	[PublicizedFrom(EAccessModifier.Private)]
	public string parent_transform_path;
}
