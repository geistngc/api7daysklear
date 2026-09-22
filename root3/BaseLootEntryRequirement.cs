using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000622 RID: 1570
[Preserve]
public class BaseLootEntryRequirement
{
	// Token: 0x06003307 RID: 13063 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Init(XElement e)
	{
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CheckRequirement(EntityPlayer player)
	{
		return true;
	}
}
