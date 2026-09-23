using System;
using UnityEngine;

// Token: 0x020013AC RID: 5036
public static class BlockFaces
{
	// Token: 0x06009EB4 RID: 40628 RVA: 0x003C0390 File Offset: 0x003BE590
	public static BlockFace CharToFace(char c)
	{
		if (c <= 'W')
		{
			if (c <= 'E')
			{
				if (c == 'B')
				{
					return BlockFace.Bottom;
				}
				if (c != 'E')
				{
					return BlockFace.None;
				}
				return BlockFace.East;
			}
			else
			{
				if (c == 'N')
				{
					return BlockFace.North;
				}
				switch (c)
				{
				case 'S':
					return BlockFace.South;
				case 'T':
					break;
				case 'U':
				case 'V':
					return BlockFace.None;
				case 'W':
					return BlockFace.West;
				default:
					return BlockFace.None;
				}
			}
		}
		else if (c <= 'e')
		{
			if (c == 'b')
			{
				return BlockFace.Bottom;
			}
			if (c != 'e')
			{
				return BlockFace.None;
			}
			return BlockFace.East;
		}
		else
		{
			if (c == 'n')
			{
				return BlockFace.North;
			}
			switch (c)
			{
			case 's':
				return BlockFace.South;
			case 't':
				break;
			case 'u':
			case 'v':
				return BlockFace.None;
			case 'w':
				return BlockFace.West;
			default:
				return BlockFace.None;
			}
		}
		return BlockFace.Top;
	}

	// Token: 0x06009EB5 RID: 40629 RVA: 0x003C0420 File Offset: 0x003BE620
	public static BlockFace RotateFace(this BlockFace face, int rotation)
	{
		Vector3 vector = Vector3.zero;
		switch (face)
		{
		case BlockFace.Top:
			vector = Vector3.up;
			break;
		case BlockFace.Bottom:
			vector = Vector3.down;
			break;
		case BlockFace.North:
			vector = Vector3.forward;
			break;
		case BlockFace.West:
			vector = Vector3.left;
			break;
		case BlockFace.South:
			vector = Vector3.back;
			break;
		case BlockFace.East:
			vector = Vector3.right;
			break;
		}
		vector = BlockShapeNew.GetRotationStatic(rotation) * vector;
		if (vector.y > 0.9f)
		{
			return BlockFace.Top;
		}
		if (vector.y < -0.9f)
		{
			return BlockFace.Bottom;
		}
		if (vector.z > 0.9f)
		{
			return BlockFace.North;
		}
		if (vector.x < -0.9f)
		{
			return BlockFace.West;
		}
		if (vector.z < -0.9f)
		{
			return BlockFace.South;
		}
		if (vector.x > 0.9f)
		{
			return BlockFace.East;
		}
		return face;
	}

	// Token: 0x06009EB6 RID: 40630 RVA: 0x003C04EC File Offset: 0x003BE6EC
	public static Vector3 GetNormal(this BlockFace face)
	{
		Vector3 result;
		switch (face)
		{
		case BlockFace.Top:
			result = Vector3.up;
			break;
		case BlockFace.Bottom:
			result = Vector3.down;
			break;
		case BlockFace.North:
			result = Vector3.forward;
			break;
		case BlockFace.West:
			result = Vector3.left;
			break;
		case BlockFace.South:
			result = Vector3.back;
			break;
		case BlockFace.East:
			result = Vector3.right;
			break;
		case BlockFace.Middle:
			result = Vector3.zero;
			break;
		default:
			if (face != BlockFace.None)
			{
				throw new ArgumentOutOfRangeException("face", face, null);
			}
			result = Vector3.zero;
			break;
		}
		return result;
	}
}
