using System;
using UnityEngine;

// Token: 0x020005EC RID: 1516
public class AnimationGunjointOffsetData
{
	// Token: 0x0600313A RID: 12602 RVA: 0x001406EC File Offset: 0x0013E8EC
	public static void InitStatic()
	{
		AnimationGunjointOffsetData.AnimationGunjointOffset = new AnimationGunjointOffsetData.AnimationGunjointOffsets[100];
		for (int i = 0; i < AnimationGunjointOffsetData.AnimationGunjointOffset.Length; i++)
		{
			AnimationGunjointOffsetData.AnimationGunjointOffset[i] = new AnimationGunjointOffsetData.AnimationGunjointOffsets(Vector3.zero, Vector3.zero);
		}
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x00140731 File Offset: 0x0013E931
	public static void Cleanup()
	{
		AnimationGunjointOffsetData.InitStatic();
	}

	// Token: 0x0400263D RID: 9789
	public static AnimationGunjointOffsetData.AnimationGunjointOffsets[] AnimationGunjointOffset;

	// Token: 0x020005ED RID: 1517
	public struct AnimationGunjointOffsets
	{
		// Token: 0x0600313D RID: 12605 RVA: 0x00140738 File Offset: 0x0013E938
		public AnimationGunjointOffsets(Vector3 _position, Vector3 _rotation)
		{
			this.position = _position;
			this.rotation = _rotation;
		}

		// Token: 0x0400263E RID: 9790
		public Vector3 position;

		// Token: 0x0400263F RID: 9791
		public Vector3 rotation;
	}
}
