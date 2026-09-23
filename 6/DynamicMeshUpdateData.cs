using System;

// Token: 0x0200038B RID: 907
public class DynamicMeshUpdateData
{
	// Token: 0x06001B1A RID: 6938 RVA: 0x000A1147 File Offset: 0x0009F347
	public string ToDebugLocation()
	{
		return this.ChunkPosition.x.ToString() + "," + this.ChunkPosition.z.ToString();
	}

	// Token: 0x04001167 RID: 4455
	public Vector3i ChunkPosition;

	// Token: 0x04001168 RID: 4456
	public long Key;

	// Token: 0x04001169 RID: 4457
	public float MaxTime;

	// Token: 0x0400116A RID: 4458
	public float UpdateTime;

	// Token: 0x0400116B RID: 4459
	public bool IsUrgent;

	// Token: 0x0400116C RID: 4460
	public bool AddToThread;
}
