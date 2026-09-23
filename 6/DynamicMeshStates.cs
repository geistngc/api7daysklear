using System;

// Token: 0x0200036E RID: 878
[Flags]
public enum DynamicMeshStates : short
{
	// Token: 0x04001058 RID: 4184
	None = 0,
	// Token: 0x04001059 RID: 4185
	ThreadUpdating = 1,
	// Token: 0x0400105A RID: 4186
	SaveRequired = 2,
	// Token: 0x0400105B RID: 4187
	LoadRequired = 4,
	// Token: 0x0400105C RID: 4188
	UnloadMark1 = 8,
	// Token: 0x0400105D RID: 4189
	UnloadMark2 = 16,
	// Token: 0x0400105E RID: 4190
	UnloadMark3 = 32,
	// Token: 0x0400105F RID: 4191
	MarkedForDelete = 64,
	// Token: 0x04001060 RID: 4192
	LoadBoosted = 128,
	// Token: 0x04001061 RID: 4193
	MainThreadLoadRequest = 256,
	// Token: 0x04001062 RID: 4194
	FileMissing = 512,
	// Token: 0x04001063 RID: 4195
	Generating = 1024
}
