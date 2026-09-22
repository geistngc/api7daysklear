using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020013B0 RID: 5040
public class BoundsUtils
{
	// Token: 0x06009EC9 RID: 40649 RVA: 0x003C0AAF File Offset: 0x003BECAF
	public static Bounds BoundsForMinMax(float mnx, float mny, float mnz, float mxx, float mxy, float mxz)
	{
		return BoundsUtils.BoundsForMinMax(new Vector3(mnx, mny, mnz), new Vector3(mxx, mxy, mxz));
	}

	// Token: 0x06009ECA RID: 40650 RVA: 0x003C0AC8 File Offset: 0x003BECC8
	public static Bounds BoundsForMinMax(Vector3 _v1, Vector3 _v2)
	{
		Vector3 vector = _v2 - _v1;
		return new Bounds(_v1 + vector / 2f, vector);
	}

	// Token: 0x06009ECB RID: 40651 RVA: 0x003C0AF4 File Offset: 0x003BECF4
	public static Bounds ExpandBounds(Bounds bounds, float x, float y, float z)
	{
		bounds.Expand(new Vector3(x, y, z));
		return bounds;
	}

	// Token: 0x06009ECC RID: 40652 RVA: 0x003C0B06 File Offset: 0x003BED06
	public static Bounds ContractBounds(Bounds bounds, float x, float y, float z)
	{
		return BoundsUtils.ExpandBounds(bounds, -x, -y, -z);
	}

	// Token: 0x06009ECD RID: 40653 RVA: 0x003C0B14 File Offset: 0x003BED14
	public static float ClipBoundsMoveY(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.x > min.x && bmins.x < max.x && bmaxs.z > min.z && bmins.z < max.z)
		{
			if (move > 0f && min.y >= bmaxs.y)
			{
				move = MathUtils.Clamp(min.y - bmaxs.y, 0f, move);
			}
			else if (move < 0f && max.y <= bmins.y)
			{
				move = MathUtils.Clamp(max.y - bmins.y, move, 0f);
			}
			else if (move < 0f)
			{
				float num = max.y - bmins.y;
				if (num < 0.2f)
				{
					move = num;
				}
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06009ECE RID: 40654 RVA: 0x003C0C1C File Offset: 0x003BEE1C
	public static float ClipBoundsMoveX(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.y > min.y && bmins.y < max.y && bmaxs.z > min.z && bmins.z < max.z)
		{
			if (move > 0f && min.x >= bmaxs.x)
			{
				move = MathUtils.Clamp(min.x - bmaxs.x, 0f, move);
			}
			else if (move < 0f && max.x <= bmins.x)
			{
				move = MathUtils.Clamp(max.x - bmins.x, move, 0f);
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06009ECF RID: 40655 RVA: 0x003C0CFC File Offset: 0x003BEEFC
	public static float ClipBoundsMoveZ(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.x > min.x && bmins.x < max.x && bmaxs.y > min.y && bmins.y < max.y)
		{
			if (move > 0f && min.z >= bmaxs.z)
			{
				move = MathUtils.Clamp(min.z - bmaxs.z, 0f, move);
			}
			else if (move < 0f && max.z <= bmins.z)
			{
				move = MathUtils.Clamp(max.z - bmins.z, move, 0f);
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06009ED0 RID: 40656 RVA: 0x003C0DDC File Offset: 0x003BEFDC
	public static Vector3 ClipBoundsMove(Bounds bounds, Vector3 move, IList<Bounds> colliderList, int numColliders)
	{
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		move.y = BoundsUtils.ClipBoundsMoveY(min, max, move.y, colliderList, numColliders);
		min.y += move.y;
		max.y += move.y;
		move.x = BoundsUtils.ClipBoundsMoveX(min, max, move.x, colliderList, numColliders);
		min.x += move.x;
		max.x += move.x;
		move.z = BoundsUtils.ClipBoundsMoveZ(min, max, move.z, colliderList, numColliders);
		return move;
	}

	// Token: 0x06009ED1 RID: 40657 RVA: 0x003C0E80 File Offset: 0x003BF080
	public static float ClipBoundsMoveY(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.x > min.x + 0f && bmins.x < max.x - 0f && bmaxs.z > min.z + 0f && bmins.z < max.z - 0f)
				{
					if (move > 0f && min.y >= bmaxs.y + 0f)
					{
						move = MathUtils.Clamp(min.y - bmaxs.y, 0f, move);
					}
					else if (move < 0f && max.y <= bmins.y - 0f)
					{
						move = MathUtils.Clamp(max.y - bmins.y, move, 0f);
					}
					else if (move < 0f)
					{
						float num = max.y - bmins.y;
						if (num < 0.2f)
						{
							move = num;
						}
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06009ED2 RID: 40658 RVA: 0x003C0FCC File Offset: 0x003BF1CC
	public static float ClipBoundsMoveX(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.y > min.y + 0f && bmins.y < max.y - 0f && bmaxs.z > min.z + 0f && bmins.z < max.z - 0f)
				{
					if (move > 0f && min.x >= bmaxs.x + 0f)
					{
						move = MathUtils.Clamp(min.x - bmaxs.x, 0f, move);
					}
					else if (move < 0f && max.x <= bmins.x - 0f)
					{
						move = MathUtils.Clamp(max.x - bmins.x, move, 0f);
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06009ED3 RID: 40659 RVA: 0x003C10F0 File Offset: 0x003BF2F0
	public static float ClipBoundsMoveZ(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.x > min.x + 0f && bmins.x < max.x - 0f && bmaxs.y > min.y + 0f && bmins.y < max.y - 0f)
				{
					if (move > 0f && min.z >= bmaxs.z + 0f)
					{
						move = MathUtils.Clamp(min.z - bmaxs.z, 0f, move);
					}
					else if (move < 0f && max.z <= bmins.z - 0f)
					{
						move = MathUtils.Clamp(max.z - bmins.z, move, 0f);
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06009ED4 RID: 40660 RVA: 0x003C1214 File Offset: 0x003BF414
	public static bool Intersects(Bounds bounds, Vector3 min1, Vector3 max1)
	{
		Vector3 min2 = bounds.min;
		Vector3 max2 = bounds.max;
		return min2.x <= max1.x && max2.x >= min1.x && min2.y <= max1.y && max2.y >= min1.y && min2.z <= max1.z && max2.z >= min1.z;
	}

	// Token: 0x06009ED5 RID: 40661 RVA: 0x003C128C File Offset: 0x003BF48C
	public static Bounds ExpandDirectional(Bounds bounds, Vector3 move)
	{
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (move.x < 0f)
		{
			min.x += move.x;
		}
		else
		{
			max.x += move.x;
		}
		if (move.y < 0f)
		{
			min.y += move.y;
		}
		else
		{
			max.y += move.y;
		}
		if (move.z < 0f)
		{
			min.z += move.z;
		}
		else
		{
			max.z += move.z;
		}
		bounds.SetMinMax(min, max);
		return bounds;
	}

	// Token: 0x06009ED6 RID: 40662 RVA: 0x003C1348 File Offset: 0x003BF548
	public static void WriteBounds(BinaryWriter _bw, Bounds bounds)
	{
		_bw.Write(bounds.min.x);
		_bw.Write(bounds.min.y);
		_bw.Write(bounds.min.z);
		_bw.Write(bounds.max.x);
		_bw.Write(bounds.max.y);
		_bw.Write(bounds.max.z);
	}

	// Token: 0x06009ED7 RID: 40663 RVA: 0x003C13C4 File Offset: 0x003BF5C4
	public static Bounds ReadBounds(BinaryReader _br)
	{
		Bounds result = default(Bounds);
		result.SetMinMax(new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle()), new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle()));
		return result;
	}

	// Token: 0x040078A1 RID: 30881
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kClipEpsilon = 0f;

	// Token: 0x040078A2 RID: 30882
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinMoveClamp = 0.0001f;
}
