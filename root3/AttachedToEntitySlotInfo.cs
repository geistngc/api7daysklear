using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F4 RID: 1012
[Preserve]
public class AttachedToEntitySlotInfo
{
	// Token: 0x04001483 RID: 5251
	public int slotIdx;

	// Token: 0x04001484 RID: 5252
	public Transform enterParentTransform;

	// Token: 0x04001485 RID: 5253
	public Vector3 enterPosition;

	// Token: 0x04001486 RID: 5254
	public Vector3 enterRotation;

	// Token: 0x04001487 RID: 5255
	public Vector2 pitchRestriction;

	// Token: 0x04001488 RID: 5256
	public Vector2 yawRestriction;

	// Token: 0x04001489 RID: 5257
	public bool bKeep3rdPersonModelVisible;

	// Token: 0x0400148A RID: 5258
	public bool bAllow3rdPerson;

	// Token: 0x0400148B RID: 5259
	public bool bReplaceLocalInventory;

	// Token: 0x0400148C RID: 5260
	public List<AttachedToEntitySlotExit> exits = new List<AttachedToEntitySlotExit>();
}
