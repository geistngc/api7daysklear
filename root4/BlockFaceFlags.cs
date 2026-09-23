using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020013AE RID: 5038
public static class BlockFaceFlags
{
	// Token: 0x06009EB7 RID: 40631 RVA: 0x003C0578 File Offset: 0x003BE778
	[PublicizedFrom(EAccessModifier.Private)]
	static BlockFaceFlags()
	{
		for (int i = 0; i < 24; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				BlockFaceFlags.faceRotShiftValues[i * 6 + j] = (int)(((BlockFace)j).RotateFace(i) - (BlockFace)j);
			}
		}
	}

	// Token: 0x06009EB8 RID: 40632 RVA: 0x003C05F0 File Offset: 0x003BE7F0
	public static BlockFaceFlag RotateFlags(BlockFaceFlag mask, byte blockRotation)
	{
		if (mask == BlockFaceFlag.None || mask == BlockFaceFlag.All || blockRotation > 23)
		{
			return mask;
		}
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			int num2 = (int)(mask & (BlockFaceFlag)(1 << i));
			if (num2 != 0)
			{
				int num3 = BlockFaceFlags.faceRotShiftValues[(int)(blockRotation * 6) + i];
				if (num3 > 0)
				{
					num2 <<= num3;
				}
				else
				{
					num2 >>= -num3;
				}
				num |= num2;
			}
		}
		return (BlockFaceFlag)num;
	}

	// Token: 0x06009EB9 RID: 40633 RVA: 0x003C0652 File Offset: 0x003BE852
	public static BlockFace ToBlockFace(BlockFaceFlag flags)
	{
		if ((flags & BlockFaceFlag.Top) != BlockFaceFlag.None)
		{
			return BlockFace.Top;
		}
		if ((flags & BlockFaceFlag.Bottom) != BlockFaceFlag.None)
		{
			return BlockFace.Bottom;
		}
		if ((flags & BlockFaceFlag.North) != BlockFaceFlag.None)
		{
			return BlockFace.North;
		}
		if ((flags & BlockFaceFlag.South) != BlockFaceFlag.None)
		{
			return BlockFace.South;
		}
		if ((flags & BlockFaceFlag.East) != BlockFaceFlag.None)
		{
			return BlockFace.East;
		}
		if ((flags & BlockFaceFlag.West) != BlockFaceFlag.None)
		{
			return BlockFace.West;
		}
		return BlockFace.None;
	}

	// Token: 0x06009EBA RID: 40634 RVA: 0x003C0685 File Offset: 0x003BE885
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BlockFaceFlag FromBlockFace(BlockFace face)
	{
		if (face == BlockFace.None)
		{
			return BlockFaceFlag.None;
		}
		return (BlockFaceFlag)(1 << (int)face);
	}

	// Token: 0x06009EBB RID: 40635 RVA: 0x003C0697 File Offset: 0x003BE897
	public static BlockFace OppositeFace(BlockFace face)
	{
		switch (face)
		{
		case BlockFace.Top:
			return BlockFace.Bottom;
		case BlockFace.Bottom:
			return BlockFace.Top;
		case BlockFace.North:
			return BlockFace.South;
		case BlockFace.West:
			return BlockFace.East;
		case BlockFace.South:
			return BlockFace.North;
		case BlockFace.East:
			return BlockFace.West;
		default:
			return BlockFace.None;
		}
	}

	// Token: 0x06009EBC RID: 40636 RVA: 0x003C06CC File Offset: 0x003BE8CC
	public static BlockFace NearestFaceForDirection(Vector3 direction, float dotTolerance = 0.8f)
	{
		float num = -2f;
		BlockFace result = BlockFace.None;
		for (BlockFace blockFace = BlockFace.Top; blockFace <= BlockFace.East; blockFace += 1)
		{
			float num2 = Vector3.Dot(BlockFaceFlags.OffsetForFace(blockFace), direction);
			if (num2 > num)
			{
				num = num2;
				result = blockFace;
			}
		}
		if (num < dotTolerance)
		{
			return BlockFace.None;
		}
		return result;
	}

	// Token: 0x06009EBD RID: 40637 RVA: 0x003C0714 File Offset: 0x003BE914
	public static Vector3 OffsetForFace(BlockFace face)
	{
		switch (face)
		{
		case BlockFace.Top:
			return Vector3.up;
		case BlockFace.Bottom:
			return Vector3.down;
		case BlockFace.North:
			return Vector3.forward;
		case BlockFace.West:
			return Vector3.left;
		case BlockFace.South:
			return Vector3.back;
		case BlockFace.East:
			return Vector3.right;
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x06009EBE RID: 40638 RVA: 0x003C076C File Offset: 0x003BE96C
	public static Vector3i OffsetIForFace(BlockFace face)
	{
		switch (face)
		{
		case BlockFace.Top:
			return Vector3i.up;
		case BlockFace.Bottom:
			return Vector3i.down;
		case BlockFace.North:
			return Vector3i.forward;
		case BlockFace.West:
			return Vector3i.left;
		case BlockFace.South:
			return Vector3i.back;
		case BlockFace.East:
			return Vector3i.right;
		default:
			return Vector3i.zero;
		}
	}

	// Token: 0x06009EBF RID: 40639 RVA: 0x003C07C2 File Offset: 0x003BE9C2
	public static BlockFaceFlag OppositeFaceFlag(BlockFace face)
	{
		return BlockFaceFlags.FromBlockFace(BlockFaceFlags.OppositeFace(face));
	}

	// Token: 0x06009EC0 RID: 40640 RVA: 0x003C07CF File Offset: 0x003BE9CF
	public static float YawForDirection(BlockFace face)
	{
		switch (face)
		{
		case BlockFace.West:
			return 270f;
		case BlockFace.South:
			return 180f;
		case BlockFace.East:
			return 90f;
		default:
			return 0f;
		}
	}

	// Token: 0x06009EC1 RID: 40641 RVA: 0x003C0800 File Offset: 0x003BEA00
	public static BlockFaceFlag FrontSidesFromPosition(Vector3i blockPos, Vector3 entityPos)
	{
		BlockFaceFlag blockFaceFlag = BlockFaceFlag.None;
		if (entityPos.x < (float)blockPos.x)
		{
			blockFaceFlag |= BlockFaceFlag.West;
		}
		if (entityPos.x >= (float)(blockPos.x + 1))
		{
			blockFaceFlag |= BlockFaceFlag.East;
		}
		if (entityPos.y < (float)blockPos.y)
		{
			blockFaceFlag |= BlockFaceFlag.Bottom;
		}
		if (entityPos.y >= (float)(blockPos.y + 1))
		{
			blockFaceFlag |= BlockFaceFlag.Top;
		}
		if (entityPos.z < (float)blockPos.z)
		{
			blockFaceFlag |= BlockFaceFlag.South;
		}
		if (entityPos.z >= (float)(blockPos.z + 1))
		{
			blockFaceFlag |= BlockFaceFlag.North;
		}
		return blockFaceFlag;
	}

	// Token: 0x06009EC2 RID: 40642 RVA: 0x003C088C File Offset: 0x003BEA8C
	public static string SerializeFaceFlags(BlockFaceFlag coverFaceMask)
	{
		string text = string.Empty;
		bool flag = true;
		for (int i = 0; i < BlockFaceFlags.cubeSideFaceFlags.Length; i++)
		{
			if ((coverFaceMask & BlockFaceFlags.cubeSideFaceFlags[i]) != BlockFaceFlag.None)
			{
				string arg = flag ? "" : ",";
				text += string.Format("{0}{1}", arg, BlockFaceFlags.cubeSideFaceChars[i]);
				flag = false;
			}
		}
		return text;
	}

	// Token: 0x0400789C RID: 30876
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly int[] faceRotShiftValues = new int[144];

	// Token: 0x0400789D RID: 30877
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly BlockFaceFlag[] cubeSideFaceFlags = new BlockFaceFlag[]
	{
		BlockFaceFlag.Top,
		BlockFaceFlag.Bottom,
		BlockFaceFlag.North,
		BlockFaceFlag.West,
		BlockFaceFlag.South,
		BlockFaceFlag.East
	};

	// Token: 0x0400789E RID: 30878
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] cubeSideFaceChars = new char[]
	{
		'T',
		'B',
		'N',
		'W',
		'S',
		'E'
	};
}
