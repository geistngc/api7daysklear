using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class DismemberedPart
{
	// Token: 0x060005BC RID: 1468 RVA: 0x00029587 File Offset: 0x00027787
	public void SetDetachedTransform(Transform _detach, Transform _group)
	{
		this.detachT = _detach;
		this.detachRenderer = _group.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0002959C File Offset: 0x0002779C
	public void FadeDetached(float value)
	{
		if (this.detachRenderer)
		{
			if (!this.fadeOutInit)
			{
				this.detachMats.Clear();
				this.detachMats.AddRange(this.detachRenderer.materials);
				this.fadeOutInit = true;
			}
			for (int i = 0; i < this.detachMats.Count; i++)
			{
				Material material = this.detachMats[i];
				if (material.HasProperty("_Fade"))
				{
					material.SetFloat("_Fade", value);
				}
			}
			this.detachRenderer.SetMaterials(this.detachMats);
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00029634 File Offset: 0x00027834
	public DismemberedPart(DismemberedPartData _data, uint _bodyDamageFlag, EnumDamageTypes _damageType)
	{
		this.Data = _data;
		this.bodyDamageFlag = _bodyDamageFlag;
		this.damageType = _damageType;
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x00029688 File Offset: 0x00027888
	public string prefabPath
	{
		get
		{
			return this.Data.prefabPath;
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00029698 File Offset: 0x00027898
	public void Update()
	{
		this.elapsedTime += Time.deltaTime;
		if (this.pivotT && this.overrideHeadSize != 1f)
		{
			if (!this.startValuesSet)
			{
				this.startingHeadSize = this.overrideHeadSize;
				this.startingScale = this.pivotT.localScale;
				this.startValuesSet = true;
			}
			float t = this.elapsedTime / this.overrideHeadDismemberScaleTime;
			this.overrideHeadSize = Mathf.Lerp(this.startingHeadSize, 1f, t);
			this.pivotT.localScale = Vector3.Lerp(this.startingScale, Vector3.one, t);
		}
		if (!this.ReadyForCleanup)
		{
			if (this.elapsedTime > this.lifeTime - 0.5f)
			{
				this.fadeTime += Time.deltaTime;
				this.FadeDetached(Mathf.Lerp(1f, 0f, this.fadeTime / 0.5f));
			}
			if (this.elapsedTime >= this.lifeTime)
			{
				this.ReadyForCleanup = true;
			}
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x000297A2 File Offset: 0x000279A2
	public void Hide()
	{
		if (this.prefabT)
		{
			this.prefabT.gameObject.SetActive(false);
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x000297C2 File Offset: 0x000279C2
	public void CleanupDetached()
	{
		if (this.detachT)
		{
			UnityEngine.Object.Destroy(this.detachT.gameObject);
			this.detachT = null;
		}
	}

	// Token: 0x0400064C RID: 1612
	public DismemberedPartData Data;

	// Token: 0x0400064D RID: 1613
	public uint bodyDamageFlag;

	// Token: 0x0400064E RID: 1614
	public EnumDamageTypes damageType;

	// Token: 0x0400064F RID: 1615
	public Transform prefabT;

	// Token: 0x04000650 RID: 1616
	public Transform detachT;

	// Token: 0x04000651 RID: 1617
	public bool ReadyForCleanup;

	// Token: 0x04000652 RID: 1618
	public float lifeTime = 10f;

	// Token: 0x04000653 RID: 1619
	[PublicizedFrom(EAccessModifier.Private)]
	public float elapsedTime;

	// Token: 0x04000654 RID: 1620
	public Transform targetT;

	// Token: 0x04000655 RID: 1621
	public Transform pivotT;

	// Token: 0x04000656 RID: 1622
	public float overrideHeadSize = 1f;

	// Token: 0x04000657 RID: 1623
	public float overrideHeadDismemberScaleTime = 1f;

	// Token: 0x04000658 RID: 1624
	[PublicizedFrom(EAccessModifier.Private)]
	public bool startValuesSet;

	// Token: 0x04000659 RID: 1625
	[PublicizedFrom(EAccessModifier.Private)]
	public float startingHeadSize;

	// Token: 0x0400065A RID: 1626
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startingScale;

	// Token: 0x0400065B RID: 1627
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshRenderer detachRenderer;

	// Token: 0x0400065C RID: 1628
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Material> detachMats = new List<Material>();

	// Token: 0x0400065D RID: 1629
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cFadeTime = 0.5f;

	// Token: 0x0400065E RID: 1630
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeTime;

	// Token: 0x0400065F RID: 1631
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fadeOutInit;
}
