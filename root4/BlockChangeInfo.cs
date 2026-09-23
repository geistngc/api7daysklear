using System;
using System.IO;

// Token: 0x02000AFB RID: 2811
public sealed class BlockChangeInfo
{
	// Token: 0x060053BD RID: 21437 RVA: 0x0020139D File Offset: 0x001FF59D
	public BlockChangeInfo()
	{
		this.blockValueRef = BlockValueRef.None;
		this.blockValue = BlockValue.Air;
		this.density = MarchingCubes.DensityAir;
		this.changedByEntityId = -1;
	}

	// Token: 0x060053BE RID: 21438 RVA: 0x002013D4 File Offset: 0x001FF5D4
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue)
	{
		this.blockValueRef = _bvRef;
		this.blockValue = _blockValue;
		this.bChangeBlockValue = true;
		this.bUpdateLight = false;
	}

	// Token: 0x060053BF RID: 21439 RVA: 0x002013FF File Offset: 0x001FF5FF
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, bool _updateLight, bool _bOnlyDamage = false) : this(_bvRef, _blockValue, _updateLight)
	{
		this.bChangeDamage = _bOnlyDamage;
	}

	// Token: 0x060053C0 RID: 21440 RVA: 0x00201412 File Offset: 0x001FF612
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, int _changedEntityId) : this(_bvRef, _blockValue)
	{
		this.changedByEntityId = _changedEntityId;
	}

	// Token: 0x060053C1 RID: 21441 RVA: 0x00201423 File Offset: 0x001FF623
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, bool _updateLight) : this(_bvRef, _blockValue)
	{
		this.bUpdateLight = _updateLight;
	}

	// Token: 0x060053C2 RID: 21442 RVA: 0x00201434 File Offset: 0x001FF634
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, bool _updateLight, int _changingEntityId) : this(_bvRef, _blockValue, _updateLight)
	{
		this.changedByEntityId = _changingEntityId;
	}

	// Token: 0x060053C3 RID: 21443 RVA: 0x00201447 File Offset: 0x001FF647
	public BlockChangeInfo(BlockValueRef _bvRef, sbyte _density, bool _bForceDensityChange = false)
	{
		this.blockValueRef = _bvRef;
		this.density = _density;
		this.bChangeDensity = true;
		this.bForceDensity = _bForceDensityChange;
		this.changedByEntityId = -1;
	}

	// Token: 0x060053C4 RID: 21444 RVA: 0x00201479 File Offset: 0x001FF679
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, sbyte _density)
	{
		this.blockValueRef = _bvRef;
		this.blockValue = _blockValue;
		this.bChangeBlockValue = true;
		this.density = _density;
		this.bChangeDensity = true;
		this.bUpdateLight = true;
		this.changedByEntityId = -1;
	}

	// Token: 0x060053C5 RID: 21445 RVA: 0x002014B9 File Offset: 0x001FF6B9
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, sbyte _density, int _changedByEntityId) : this(_bvRef, _blockValue, _density)
	{
		this.changedByEntityId = _changedByEntityId;
	}

	// Token: 0x060053C6 RID: 21446 RVA: 0x002014CC File Offset: 0x001FF6CC
	public BlockChangeInfo(BlockValueRef _bvRef, BlockValue _blockValue, sbyte _density, TextureFullArray _tex) : this(_bvRef, _blockValue, _density)
	{
		this.bChangeTexture = true;
		this.textureFull = _tex;
		this.changedByEntityId = -1;
	}

	// Token: 0x060053C7 RID: 21447 RVA: 0x002014F0 File Offset: 0x001FF6F0
	public void Read(BinaryReader _br)
	{
		this.blockValueRef = BlockValueRef.Read(_br);
		this.changedByEntityId = _br.ReadInt32();
		BlockChangeInfo.Flags flags = (BlockChangeInfo.Flags)_br.ReadByte();
		this.bChangeBlockValue = ((flags & BlockChangeInfo.Flags.ChangeBlockValue) > (BlockChangeInfo.Flags)0);
		this.bChangeDensity = ((flags & BlockChangeInfo.Flags.ChangeDensity) > (BlockChangeInfo.Flags)0);
		this.bForceDensity = ((flags & BlockChangeInfo.Flags.ForceDensity) > (BlockChangeInfo.Flags)0);
		this.bUpdateLight = ((flags & BlockChangeInfo.Flags.UpdateLight) > (BlockChangeInfo.Flags)0);
		this.bChangeDamage = ((flags & BlockChangeInfo.Flags.ChangeDamage) > (BlockChangeInfo.Flags)0);
		this.bChangeTexture = ((flags & BlockChangeInfo.Flags.ChangeTexture) > (BlockChangeInfo.Flags)0);
		if (this.bChangeBlockValue)
		{
			this.blockValue = BlockValue.Read(_br);
		}
		if (this.bChangeDensity)
		{
			this.density = _br.ReadSByte();
		}
		if (this.bChangeTexture)
		{
			this.textureFull.Read(_br, 1);
		}
	}

	// Token: 0x060053C8 RID: 21448 RVA: 0x002015A4 File Offset: 0x001FF7A4
	public void Write(BinaryWriter _bw)
	{
		this.blockValueRef.Write(_bw);
		_bw.Write(this.changedByEntityId);
		BlockChangeInfo.Flags flags = (BlockChangeInfo.Flags)0;
		flags |= (this.bChangeBlockValue ? BlockChangeInfo.Flags.ChangeBlockValue : ((BlockChangeInfo.Flags)0));
		flags |= (this.bChangeDensity ? BlockChangeInfo.Flags.ChangeDensity : ((BlockChangeInfo.Flags)0));
		flags |= (this.bForceDensity ? BlockChangeInfo.Flags.ForceDensity : ((BlockChangeInfo.Flags)0));
		flags |= (this.bUpdateLight ? BlockChangeInfo.Flags.UpdateLight : ((BlockChangeInfo.Flags)0));
		flags |= (this.bChangeDamage ? BlockChangeInfo.Flags.ChangeDamage : ((BlockChangeInfo.Flags)0));
		flags |= (this.bChangeTexture ? BlockChangeInfo.Flags.ChangeTexture : ((BlockChangeInfo.Flags)0));
		_bw.Write((byte)flags);
		if (this.bChangeBlockValue)
		{
			this.blockValue.Write(_bw);
		}
		if (this.bChangeDensity)
		{
			_bw.Write(this.density);
		}
		if (this.bChangeTexture)
		{
			this.textureFull.Write(_bw);
		}
	}

	// Token: 0x0400417C RID: 16764
	public static BlockChangeInfo Empty = new BlockChangeInfo
	{
		blockValueRef = BlockValueRef.None
	};

	// Token: 0x0400417D RID: 16765
	public BlockValueRef blockValueRef;

	// Token: 0x0400417E RID: 16766
	public bool bChangeBlockValue;

	// Token: 0x0400417F RID: 16767
	public bool bChangeDamage;

	// Token: 0x04004180 RID: 16768
	public BlockValue blockValue;

	// Token: 0x04004181 RID: 16769
	public bool bChangeDensity;

	// Token: 0x04004182 RID: 16770
	public bool bForceDensity;

	// Token: 0x04004183 RID: 16771
	public sbyte density;

	// Token: 0x04004184 RID: 16772
	public bool bUpdateLight;

	// Token: 0x04004185 RID: 16773
	public bool bChangeTexture;

	// Token: 0x04004186 RID: 16774
	public TextureFullArray textureFull;

	// Token: 0x04004187 RID: 16775
	public int changedByEntityId = -1;

	// Token: 0x02000AFC RID: 2812
	[Flags]
	[PublicizedFrom(EAccessModifier.Private)]
	public enum Flags : byte
	{
		// Token: 0x04004189 RID: 16777
		ChangeBlockValue = 1,
		// Token: 0x0400418A RID: 16778
		ChangeDamage = 2,
		// Token: 0x0400418B RID: 16779
		ChangeDensity = 4,
		// Token: 0x0400418C RID: 16780
		ForceDensity = 8,
		// Token: 0x0400418D RID: 16781
		UpdateLight = 16,
		// Token: 0x0400418E RID: 16782
		ChangeTexture = 32
	}
}
