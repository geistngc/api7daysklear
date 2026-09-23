using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000486 RID: 1158
[Preserve]
public class EntityAnimalStag : EntityAnimal
{
	// Token: 0x060023F6 RID: 9206 RVA: 0x000DA17C File Offset: 0x000D837C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.85f, 0f);
			component.size = new Vector3(0.8f, 1.6f, 0.8f);
		}
		base.Awake();
	}
}
