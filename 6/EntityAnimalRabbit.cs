using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000484 RID: 1156
[Preserve]
public class EntityAnimalRabbit : EntityAnimal
{
	// Token: 0x060023F1 RID: 9201 RVA: 0x000DA0C0 File Offset: 0x000D82C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.15f, 0f);
			component.size = new Vector3(0.4f, 0.4f, 0.4f);
		}
		base.Awake();
		Transform transform = base.transform.Find("Graphics/BlobShadowProjector");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsAttackValid()
	{
		return false;
	}
}
