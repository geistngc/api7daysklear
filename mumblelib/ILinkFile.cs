using System;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020016A1 RID: 5793
	public interface ILinkFile : IDisposable
	{
		// Token: 0x17001608 RID: 5640
		// (get) Token: 0x0600B59B RID: 46491
		// (set) Token: 0x0600B59A RID: 46490
		uint UIVersion { get; set; }

		// Token: 0x0600B59C RID: 46492
		void Tick();

		// Token: 0x17001609 RID: 5641
		// (set) Token: 0x0600B59D RID: 46493
		Vector3 AvatarPosition { set; }

		// Token: 0x1700160A RID: 5642
		// (set) Token: 0x0600B59E RID: 46494
		Vector3 AvatarForward { set; }

		// Token: 0x1700160B RID: 5643
		// (set) Token: 0x0600B59F RID: 46495
		Vector3 AvatarTop { set; }

		// Token: 0x1700160C RID: 5644
		// (set) Token: 0x0600B5A0 RID: 46496
		string Name { set; }

		// Token: 0x1700160D RID: 5645
		// (set) Token: 0x0600B5A1 RID: 46497
		Vector3 CameraPosition { set; }

		// Token: 0x1700160E RID: 5646
		// (set) Token: 0x0600B5A2 RID: 46498
		Vector3 CameraForward { set; }

		// Token: 0x1700160F RID: 5647
		// (set) Token: 0x0600B5A3 RID: 46499
		Vector3 CameraTop { set; }

		// Token: 0x17001610 RID: 5648
		// (set) Token: 0x0600B5A4 RID: 46500
		string Identity { set; }

		// Token: 0x17001611 RID: 5649
		// (set) Token: 0x0600B5A5 RID: 46501
		string Context { set; }

		// Token: 0x17001612 RID: 5650
		// (set) Token: 0x0600B5A6 RID: 46502
		string Description { set; }
	}
}
