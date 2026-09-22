using System;
using System.IO;
using UnityEngine;

// Token: 0x02000167 RID: 359
public struct BlockValueRef : IEquatable<BlockValueRef>
{
	// Token: 0x06000A19 RID: 2585 RVA: 0x0004399B File Offset: 0x00041B9B
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValueRef(BlockValueRefType type, Vector3i blockPosition, PropRef propRef)
	{
		this.Type = type;
		this.BlockPosition = blockPosition;
		this.PropReference = propRef;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000439B2 File Offset: 0x00041BB2
	public BlockValueRef(int x, int y, int z)
	{
		this = new BlockValueRef(new Vector3i(x, y, z));
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x000439C4 File Offset: 0x00041BC4
	public BlockValueRef(Vector3i pos)
	{
		this = new BlockValueRef(BlockValueRefType.Block, pos, default(PropRef));
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x000439E2 File Offset: 0x00041BE2
	public BlockValueRef(PropRef propRef)
	{
		this = new BlockValueRef(BlockValueRefType.Prop, Vector3i.zero, propRef);
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x000439F1 File Offset: 0x00041BF1
	public bool TryGetBlockPos(out Vector3i pos)
	{
		if (this.Type != BlockValueRefType.Block)
		{
			pos = Vector3i.zero;
			return false;
		}
		pos = this.BlockPosition;
		return true;
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x00043A16 File Offset: 0x00041C16
	public bool TryGetPropRef(out PropRef propRef)
	{
		if (this.Type != BlockValueRefType.Prop)
		{
			propRef = PropRef.Default;
			return false;
		}
		propRef = this.PropReference;
		return true;
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00043A3C File Offset: 0x00041C3C
	public Vector3 ToVector3(WorldBase world)
	{
		Vector3 result;
		switch (this.Type)
		{
		case BlockValueRefType.None:
			result = Vector3.zero;
			break;
		case BlockValueRefType.Block:
			result = this.BlockPosition.ToVector3();
			break;
		case BlockValueRefType.Prop:
		{
			IChunk chunkSync = world.GetChunkSync(this.PropReference);
			result = ((chunkSync != null) ? chunkSync.GetWorldPos() : Vector3i.zero) + world.GetProp(this.PropReference).position;
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00043AC0 File Offset: 0x00041CC0
	public Vector3 ToVector3Center(WorldBase world)
	{
		Vector3 result;
		switch (this.Type)
		{
		case BlockValueRefType.None:
			result = Vector3.zero;
			break;
		case BlockValueRefType.Block:
			result = this.BlockPosition.ToVector3Center();
			break;
		case BlockValueRefType.Prop:
		{
			IChunk chunkSync = world.GetChunkSync(this.PropReference);
			result = ((chunkSync != null) ? chunkSync.GetWorldPos() : Vector3i.zero) + world.GetProp(this.PropReference).position + new Vector3(0.5f, 0.5f, 0.5f);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00043B5C File Offset: 0x00041D5C
	public Vector3 ToVector3CenterXZ(WorldBase world)
	{
		Vector3 result;
		switch (this.Type)
		{
		case BlockValueRefType.None:
			result = Vector3.zero;
			break;
		case BlockValueRefType.Block:
			result = this.BlockPosition.ToVector3CenterXZ();
			break;
		case BlockValueRefType.Prop:
		{
			IChunk chunkSync = world.GetChunkSync(this.PropReference);
			result = ((chunkSync != null) ? chunkSync.GetWorldPos() : Vector3i.zero) + world.GetProp(this.PropReference).position + new Vector3(0.5f, 0f, 0.5f);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x00043BF7 File Offset: 0x00041DF7
	public Vector3i ToBlockPos(WorldBase world)
	{
		return World.worldToBlockPos(this.ToVector3Center(world));
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x00043C08 File Offset: 0x00041E08
	public static BlockValueRef Create(WorldRayHitInfo hitInfo)
	{
		if (!hitInfo.hit.propValue.IsAir)
		{
			return new BlockValueRef(hitInfo.hit.propData.PropRef);
		}
		if (!hitInfo.hit.blockValue.isair)
		{
			return new BlockValueRef(hitInfo.hit.blockPos);
		}
		return BlockValueRef.None;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x00043C6C File Offset: 0x00041E6C
	public static BlockValueRef Read(BinaryReader br)
	{
		BlockValueRef result;
		switch (br.ReadByte())
		{
		case 0:
			result = BlockValueRef.None;
			break;
		case 1:
			result = new BlockValueRef(StreamUtils.ReadVector3i(br));
			break;
		case 2:
			result = new BlockValueRef(PropRef.Read(br));
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00043CC0 File Offset: 0x00041EC0
	public readonly void Write(BinaryWriter bw)
	{
		bw.Write((byte)this.Type);
		switch (this.Type)
		{
		case BlockValueRefType.None:
			return;
		case BlockValueRefType.Block:
			StreamUtils.Write(bw, this.BlockPosition);
			return;
		case BlockValueRefType.Prop:
			this.PropReference.Write(bw);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00043D14 File Offset: 0x00041F14
	public override string ToString()
	{
		object arg = this.Type;
		string arg2;
		switch (this.Type)
		{
		case BlockValueRefType.None:
			arg2 = "";
			break;
		case BlockValueRefType.Block:
			arg2 = string.Format(", BlockPos=({0})", this.BlockPosition);
			break;
		case BlockValueRefType.Prop:
			arg2 = string.Format(", PropRef=[{0}]", this.PropReference);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return string.Format("Type={0}{1}", arg, arg2);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00043D94 File Offset: 0x00041F94
	public static implicit operator Vector3i(BlockValueRef bvRef)
	{
		Vector3i result;
		if (bvRef.TryGetBlockPos(out result))
		{
			return result;
		}
		Log.Warning(string.Format("[PROPS] BlockValueRef implicitly converted to Vector3i but type was {0}.", bvRef.Type));
		return Vector3i.zero;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x00043DD0 File Offset: 0x00041FD0
	public static implicit operator PropRef(BlockValueRef bvRef)
	{
		PropRef result;
		if (bvRef.TryGetPropRef(out result))
		{
			return result;
		}
		Log.Warning(string.Format("[PROPS] BlockValueRef implicitly converted to PropRef but type was {0}.", bvRef.Type));
		return PropRef.Default;
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x00043E09 File Offset: 0x00042009
	public static implicit operator BlockValueRef(Vector3i pos)
	{
		return new BlockValueRef(pos);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00043E11 File Offset: 0x00042011
	public static implicit operator BlockValueRef(PropRef propRef)
	{
		return new BlockValueRef(propRef);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x00043E1C File Offset: 0x0004201C
	public bool Equals(BlockValueRef other)
	{
		return this.Type == other.Type && this.BlockPosition.Equals(other.BlockPosition) && this.PropReference.Equals(other.PropReference);
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x00043E64 File Offset: 0x00042064
	public override bool Equals(object obj)
	{
		if (obj is BlockValueRef)
		{
			BlockValueRef other = (BlockValueRef)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00043E89 File Offset: 0x00042089
	public override int GetHashCode()
	{
		return HashCode.Combine<int, Vector3i, PropRef>((int)this.Type, this.BlockPosition, this.PropReference);
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00043EA2 File Offset: 0x000420A2
	public static bool operator ==(BlockValueRef left, BlockValueRef right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00043EAC File Offset: 0x000420AC
	public static bool operator !=(BlockValueRef left, BlockValueRef right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000A14 RID: 2580
	public static readonly BlockValueRef None = new BlockValueRef(BlockValueRefType.None, Vector3i.zero, default(PropRef));

	// Token: 0x04000A15 RID: 2581
	public readonly BlockValueRefType Type;

	// Token: 0x04000A16 RID: 2582
	public readonly Vector3i BlockPosition;

	// Token: 0x04000A17 RID: 2583
	public readonly PropRef PropReference;
}
