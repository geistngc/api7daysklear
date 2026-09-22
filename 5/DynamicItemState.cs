using System;

// Token: 0x0200037A RID: 890
public enum DynamicItemState : byte
{
	// Token: 0x040010D4 RID: 4308
	Waiting,
	// Token: 0x040010D5 RID: 4309
	UpdateRequired,
	// Token: 0x040010D6 RID: 4310
	Empty,
	// Token: 0x040010D7 RID: 4311
	LoadRequested,
	// Token: 0x040010D8 RID: 4312
	Loading,
	// Token: 0x040010D9 RID: 4313
	Loaded,
	// Token: 0x040010DA RID: 4314
	ReadyToDelete,
	// Token: 0x040010DB RID: 4315
	Invalid
}
