using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class DismemberedPartData
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060005B3 RID: 1459 RVA: 0x0002950E File Offset: 0x0002770E
	// (set) Token: 0x060005B4 RID: 1460 RVA: 0x00029516 File Offset: 0x00027716
	public Vector3 rot { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060005B5 RID: 1461 RVA: 0x0002951F File Offset: 0x0002771F
	// (set) Token: 0x060005B6 RID: 1462 RVA: 0x00029527 File Offset: 0x00027727
	public bool hasRotOffset { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060005B7 RID: 1463 RVA: 0x00029530 File Offset: 0x00027730
	public void SetRot(Vector3 _rot)
	{
		this.hasRotOffset = true;
		this.rot = _rot;
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00029540 File Offset: 0x00027740
	// (set) Token: 0x060005B9 RID: 1465 RVA: 0x00029548 File Offset: 0x00027748
	public bool Invalid { get; set; }

	// Token: 0x060005BA RID: 1466 RVA: 0x00029551 File Offset: 0x00027751
	public string Log()
	{
		return string.Format(" property: {0} prefabPath: {1} target: {2} damageTag {3}", new object[]
		{
			this.propertyKey,
			this.prefabPath,
			this.targetBone,
			this.damageTypeKey
		});
	}

	// Token: 0x04000633 RID: 1587
	public string propertyKey;

	// Token: 0x04000634 RID: 1588
	public string prefabPath;

	// Token: 0x04000635 RID: 1589
	public string targetBone;

	// Token: 0x04000636 RID: 1590
	public string damageTypeKey;

	// Token: 0x04000637 RID: 1591
	public bool isDetachable;

	// Token: 0x04000638 RID: 1592
	public Vector3 pos;

	// Token: 0x04000639 RID: 1593
	public Vector3 scale;

	// Token: 0x0400063A RID: 1594
	public Vector3 offset;

	// Token: 0x0400063B RID: 1595
	public bool attachToParent;

	// Token: 0x0400063C RID: 1596
	public string[] particlePaths;

	// Token: 0x0400063D RID: 1597
	public bool useMask;

	// Token: 0x0400063E RID: 1598
	public bool isLinked;

	// Token: 0x0400063F RID: 1599
	public bool scaleOutLimb;

	// Token: 0x04000640 RID: 1600
	public string solTarget;

	// Token: 0x04000641 RID: 1601
	public Vector3 solScale;

	// Token: 0x04000642 RID: 1602
	public bool hasSolScale;

	// Token: 0x04000643 RID: 1603
	public string childTargetObj;

	// Token: 0x04000644 RID: 1604
	public string insertBoneObj;

	// Token: 0x04000645 RID: 1605
	public string addScalePoint;

	// Token: 0x04000646 RID: 1606
	public string maskScaleBlend;

	// Token: 0x04000647 RID: 1607
	public string setFixedValues;

	// Token: 0x04000648 RID: 1608
	public string dismemberMatPath;
}
