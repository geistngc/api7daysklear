using System;
using JetBrains.Annotations;

// Token: 0x0200111D RID: 4381
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
[MeansImplicitUse]
public class XuiXmlAttributeAttribute : Attribute
{
	// Token: 0x06008AA6 RID: 35494 RVA: 0x0034CE65 File Offset: 0x0034B065
	public XuiXmlAttributeAttribute(string _attributeName, bool _override = false)
	{
		this.AttributeName = _attributeName;
		this.Override = _override;
	}

	// Token: 0x040066E3 RID: 26339
	public readonly string AttributeName;

	// Token: 0x040066E4 RID: 26340
	public readonly bool Override;
}
