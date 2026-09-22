using System;

// Token: 0x02000359 RID: 857
public class ChunkPreviewData
{
	// Token: 0x170002EF RID: 751
	// (get) Token: 0x060018CA RID: 6346 RVA: 0x0008C2D1 File Offset: 0x0008A4D1
	// (set) Token: 0x060018CB RID: 6347 RVA: 0x0008C2D9 File Offset: 0x0008A4D9
	public Vector3i WorldPosition { get; set; }

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x060018CC RID: 6348 RVA: 0x0008C2E2 File Offset: 0x0008A4E2
	// (set) Token: 0x060018CD RID: 6349 RVA: 0x0008C2EA File Offset: 0x0008A4EA
	public Prefab PrefabData { get; set; }

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x060018CE RID: 6350 RVA: 0x0008C2F3 File Offset: 0x0008A4F3
	// (set) Token: 0x060018CF RID: 6351 RVA: 0x0008C2FB File Offset: 0x0008A4FB
	public PrefabInstance PrefabInstance { get; set; }
}
