using System;
using UnityEngine.Scripting;

// Token: 0x020005EA RID: 1514
[Preserve]
public class AnimationDelayData
{
	// Token: 0x06003136 RID: 12598 RVA: 0x00140664 File Offset: 0x0013E864
	public static void InitStatic()
	{
		AnimationDelayData.AnimationDelay = new AnimationDelayData.AnimationDelays[100];
		for (int i = 0; i < AnimationDelayData.AnimationDelay.Length; i++)
		{
			AnimationDelayData.AnimationDelay[i] = new AnimationDelayData.AnimationDelays(0, 0f, 0f, Constants.cMinHolsterTime, Constants.cMinUnHolsterTime, false);
		}
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x001406B5 File Offset: 0x0013E8B5
	public static void Cleanup()
	{
		AnimationDelayData.InitStatic();
	}

	// Token: 0x04002636 RID: 9782
	public static AnimationDelayData.AnimationDelays[] AnimationDelay;

	// Token: 0x020005EB RID: 1515
	public struct AnimationDelays
	{
		// Token: 0x06003139 RID: 12601 RVA: 0x001406BC File Offset: 0x0013E8BC
		public AnimationDelays(int _carry, float _rayCast, float _rayCastMoving, float _holster, float _unholster, bool _twoHanded)
		{
			this.Carry = _carry;
			this.RayCast = _rayCast;
			this.RayCastMoving = _rayCastMoving;
			this.Holster = _holster;
			this.Unholster = _unholster;
			this.TwoHanded = _twoHanded;
		}

		// Token: 0x04002637 RID: 9783
		public int Carry;

		// Token: 0x04002638 RID: 9784
		public float RayCast;

		// Token: 0x04002639 RID: 9785
		public float RayCastMoving;

		// Token: 0x0400263A RID: 9786
		public float Holster;

		// Token: 0x0400263B RID: 9787
		public float Unholster;

		// Token: 0x0400263C RID: 9788
		public bool TwoHanded;
	}
}
