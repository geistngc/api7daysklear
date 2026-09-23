using System;

// Token: 0x02000B42 RID: 2882
public interface IBlockAccess
{
	// Token: 0x06005753 RID: 22355
	BlockValue GetBlock(int x, int y, int z);

	// Token: 0x06005754 RID: 22356 RVA: 0x001B2C80 File Offset: 0x001B0E80
	BlockValue GetBlock(Vector3i pos)
	{
		return IBlockAccess.DefaultGetBlock(this, pos);
	}

	// Token: 0x06005755 RID: 22357 RVA: 0x001B2C89 File Offset: 0x001B0E89
	BlockValue GetBlock(BlockValueRef bvRef)
	{
		return IBlockAccess.DefaultGetBlock(this, bvRef);
	}

	// Token: 0x06005756 RID: 22358
	PropValue GetProp(int chunkX, int chunkZ, int propId);

	// Token: 0x06005757 RID: 22359
	PropValue GetProp(long chunkKey, int propId);

	// Token: 0x06005758 RID: 22360 RVA: 0x001B2E15 File Offset: 0x001B1015
	PropValue GetProp(Vector2i chunkPos, int propId)
	{
		return IBlockAccess.DefaultGetProp(this, chunkPos, propId);
	}

	// Token: 0x06005759 RID: 22361 RVA: 0x001B2E1F File Offset: 0x001B101F
	PropValue GetProp(PropRef propRef)
	{
		return IBlockAccess.DefaultGetProp(this, propRef);
	}

	// Token: 0x0600575A RID: 22362 RVA: 0x00218BF7 File Offset: 0x00216DF7
	[PublicizedFrom(EAccessModifier.Protected)]
	public static BlockValue DefaultGetBlock(IBlockAccess iba, Vector3i pos)
	{
		return iba.GetBlock(pos.x, pos.y, pos.z);
	}

	// Token: 0x0600575B RID: 22363 RVA: 0x00218C14 File Offset: 0x00216E14
	[PublicizedFrom(EAccessModifier.Protected)]
	public static BlockValue DefaultGetBlock(IBlockAccess iba, BlockValueRef bvRef)
	{
		BlockValue result;
		switch (bvRef.Type)
		{
		case BlockValueRefType.None:
			result = BlockValue.Air;
			break;
		case BlockValueRefType.Block:
			result = iba.GetBlock(bvRef.BlockPosition);
			break;
		case BlockValueRefType.Prop:
			result = iba.GetProp(bvRef.PropReference).blockValue;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x0600575C RID: 22364 RVA: 0x00218C6E File Offset: 0x00216E6E
	[PublicizedFrom(EAccessModifier.Protected)]
	public static PropValue DefaultGetProp(IBlockAccess iba, Vector2i chunkPos, int propId)
	{
		return iba.GetProp(chunkPos.x, chunkPos.y, propId);
	}

	// Token: 0x0600575D RID: 22365 RVA: 0x00218C83 File Offset: 0x00216E83
	[PublicizedFrom(EAccessModifier.Protected)]
	public static PropValue DefaultGetProp(IBlockAccess iba, PropRef propRef)
	{
		return iba.GetProp(propRef.ChunkPos, propRef.PropId);
	}
}
