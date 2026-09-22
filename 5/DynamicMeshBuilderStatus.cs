using System;

// Token: 0x0200039A RID: 922
public enum DynamicMeshBuilderStatus
{
	// Token: 0x040011B8 RID: 4536
	Ready,
	// Token: 0x040011B9 RID: 4537
	StartingExport,
	// Token: 0x040011BA RID: 4538
	StartingGeneration,
	// Token: 0x040011BB RID: 4539
	StartingRegionRegen,
	// Token: 0x040011BC RID: 4540
	StartingPreview,
	// Token: 0x040011BD RID: 4541
	Running,
	// Token: 0x040011BE RID: 4542
	Complete,
	// Token: 0x040011BF RID: 4543
	PreviewComplete,
	// Token: 0x040011C0 RID: 4544
	Stopped,
	// Token: 0x040011C1 RID: 4545
	Error
}
