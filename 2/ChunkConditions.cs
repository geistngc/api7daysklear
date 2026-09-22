using System;

// Token: 0x0200151F RID: 5407
public static class ChunkConditions
{
	// Token: 0x04007E85 RID: 32389
	public static readonly ChunkConditions.Delegate Decorated = (Chunk chunk) => !chunk.NeedsDecoration && !chunk.NeedsLightCalculation;

	// Token: 0x04007E86 RID: 32390
	public static readonly ChunkConditions.Delegate MeshesCopied = (Chunk chunk) => !chunk.InProgressDecorating && !chunk.InProgressLighting && !chunk.InProgressRegeneration && !chunk.InProgressCopying && !chunk.NeedsDecoration && !chunk.NeedsLightCalculation && !chunk.NeedsRegeneration && !chunk.NeedsCopying;

	// Token: 0x04007E87 RID: 32391
	public static readonly ChunkConditions.Delegate Displayed = (Chunk chunk) => ChunkConditions.MeshesCopied(chunk) && chunk.displayState == Chunk.DisplayState.Done;

	// Token: 0x02001520 RID: 5408
	// (Invoke) Token: 0x0600A9FC RID: 43516
	public delegate bool Delegate(Chunk chunk);
}
