using System;

// Token: 0x02000B9C RID: 2972
[Flags]
public enum ChunkProtectionLevel
{
	// Token: 0x0400458E RID: 17806
	None = 0,
	// Token: 0x0400458F RID: 17807
	NearOfflinePlayer = 1,
	// Token: 0x04004590 RID: 17808
	NearBedroll = 2,
	// Token: 0x04004591 RID: 17809
	NearSupplyCrate = 4,
	// Token: 0x04004592 RID: 17810
	NearQuestObjective = 8,
	// Token: 0x04004593 RID: 17811
	NearDroppedBackpack = 16,
	// Token: 0x04004594 RID: 17812
	NearVehicle = 32,
	// Token: 0x04004595 RID: 17813
	NearLandClaim = 64,
	// Token: 0x04004596 RID: 17814
	OfflinePlayer = 128,
	// Token: 0x04004597 RID: 17815
	Bedroll = 256,
	// Token: 0x04004598 RID: 17816
	SupplyCrate = 512,
	// Token: 0x04004599 RID: 17817
	QuestObjective = 1024,
	// Token: 0x0400459A RID: 17818
	DroppedBackpack = 2048,
	// Token: 0x0400459B RID: 17819
	Trader = 4096,
	// Token: 0x0400459C RID: 17820
	Drone = 8192,
	// Token: 0x0400459D RID: 17821
	Vehicle = 16384,
	// Token: 0x0400459E RID: 17822
	LandClaim = 32768,
	// Token: 0x0400459F RID: 17823
	CurrentlySynced = 65536,
	// Token: 0x040045A0 RID: 17824
	All = -1
}
