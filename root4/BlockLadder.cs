using System;
using UnityEngine.Scripting;

// Token: 0x02000125 RID: 293
[Preserve]
public class BlockLadder : Block
{
	// Token: 0x060007BC RID: 1980 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsElevator()
	{
		return true;
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00036D10 File Offset: 0x00034F10
	public override bool IsElevator(int rotation)
	{
		return BlockLadder.climbableRotations[rotation] > 0;
	}

	// Token: 0x04000915 RID: 2325
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] climbableRotations = new byte[]
	{
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		0,
		1,
		0,
		1,
		1,
		0,
		1,
		0,
		0,
		1,
		0,
		1,
		1,
		0,
		1,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};
}
