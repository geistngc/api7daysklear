using System;
using JetBrains.Annotations;

// Token: 0x02001120 RID: 4384
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
[MeansImplicitUse]
public class XuiXmlBindingAttribute : Attribute
{
	// Token: 0x06008AA9 RID: 35497 RVA: 0x0034CE7B File Offset: 0x0034B07B
	public XuiXmlBindingAttribute(string _bindingName)
	{
		this.BindingName = _bindingName;
	}

	// Token: 0x040066E5 RID: 26341
	public readonly string BindingName;
}
