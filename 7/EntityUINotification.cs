using System;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public interface EntityUINotification
{
	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000E94 RID: 3732
	BuffValue Buff { get; }

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000E95 RID: 3733
	string Icon { get; }

	// Token: 0x06000E96 RID: 3734
	Color GetColor();

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000E97 RID: 3735
	float CurrentValue { get; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000E98 RID: 3736
	string Units { get; }

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000E99 RID: 3737
	EnumEntityUINotificationDisplayMode DisplayMode { get; }

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000E9A RID: 3738
	EnumEntityUINotificationSubject Subject { get; }
}
