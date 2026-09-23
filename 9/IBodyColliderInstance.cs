using System;
using UnityEngine;

// Token: 0x020008FE RID: 2302
public interface IBodyColliderInstance
{
	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x060042B7 RID: 17079
	Transform Transform { get; }

	// Token: 0x17000700 RID: 1792
	// (set) Token: 0x060042B8 RID: 17080
	EnumColliderMode ColliderMode { set; }

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x060042B9 RID: 17081
	PhysicsBodyColliderConfiguration Config { get; }
}
