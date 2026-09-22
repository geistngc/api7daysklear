using System;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class BlockSwitchSingleController : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000615 RID: 1557 RVA: 0x0002B8AB File Offset: 0x00029AAB
	// (set) Token: 0x06000616 RID: 1558 RVA: 0x0002B8B3 File Offset: 0x00029AB3
	public bool Activated
	{
		get
		{
			return this.activated;
		}
		set
		{
			this.activated = value;
			this.SetState();
		}
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0002B8C2 File Offset: 0x00029AC2
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.SetState();
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0002B8B3 File Offset: 0x00029AB3
	public void SetState(bool _activated)
	{
		this.activated = _activated;
		this.SetState();
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0002B8CA File Offset: 0x00029ACA
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetState()
	{
		if (this.ItemPrefab != null)
		{
			this.ItemPrefab.SetActive(!this.activated);
		}
	}

	// Token: 0x040006C6 RID: 1734
	public GameObject ItemPrefab;

	// Token: 0x040006C7 RID: 1735
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool activated;
}
