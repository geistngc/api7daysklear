using System;
using UnityEngine;

// Token: 0x020013C0 RID: 5056
public class ClothFix : MonoBehaviour
{
	// Token: 0x06009F0F RID: 40719 RVA: 0x003C1E3F File Offset: 0x003C003F
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.cloth = base.GetComponent<Cloth>();
	}

	// Token: 0x06009F10 RID: 40720 RVA: 0x003C1E50 File Offset: 0x003C0050
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.cloth.enabled = false;
		this.cloth.enabled = true;
		MeshCollider[] components = base.GetComponents<MeshCollider>();
		for (int i = 0; i < components.Length; i++)
		{
			UnityEngine.Object.Destroy(components[i]);
		}
	}

	// Token: 0x06009F11 RID: 40721 RVA: 0x003C1E92 File Offset: 0x003C0092
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.cloth.enabled = false;
	}

	// Token: 0x040078D2 RID: 30930
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Cloth cloth;
}
