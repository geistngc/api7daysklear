using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x0200046F RID: 1135
public struct BodyDamage
{
	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06002228 RID: 8744 RVA: 0x000CE5B5 File Offset: 0x000CC7B5
	public bool HasLeftLeg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 96U) == 0U;
		}
	}

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06002229 RID: 8745 RVA: 0x000CE5C3 File Offset: 0x000CC7C3
	public bool HasRightLeg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 384U) == 0U;
		}
	}

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x0600222A RID: 8746 RVA: 0x000CE5D4 File Offset: 0x000CC7D4
	public bool HasLimbs
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 330U) != 330U;
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x0600222B RID: 8747 RVA: 0x000CE5EC File Offset: 0x000CC7EC
	public bool IsAnyLegMissing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 480U) > 0U;
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x0600222C RID: 8748 RVA: 0x000CE5FD File Offset: 0x000CC7FD
	public bool IsAnyArmOrLegMissing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 510U) > 0U;
		}
	}

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x0600222D RID: 8749 RVA: 0x000CE60E File Offset: 0x000CC80E
	public bool IsCrippled
	{
		get
		{
			return (this.Flags & 12288U) > 0U;
		}
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000CE620 File Offset: 0x000CC820
	public static BodyDamage Read(BinaryReader _br, int _version)
	{
		if (_version > 21)
		{
			return BodyDamage.ReadData(_br, _br.ReadInt32());
		}
		if (_version > 20)
		{
			return BodyDamage.ReadData(_br, 0);
		}
		if (_version > 19)
		{
			_br.ReadInt32();
		}
		return default(BodyDamage);
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000CE664 File Offset: 0x000CC864
	[PublicizedFrom(EAccessModifier.Private)]
	public static BodyDamage ReadData(BinaryReader br, int version)
	{
		BodyDamage result = default(BodyDamage);
		if (version >= 4)
		{
			result.damageType = (EnumDamageTypes)br.ReadInt32();
		}
		if (version >= 3)
		{
			result.Flags = br.ReadUInt32();
		}
		else
		{
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			if (br.ReadBoolean())
			{
				result.Flags |= 2U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 8U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 1U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 128U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 8192U;
			}
			if (version >= 1)
			{
				br.ReadInt16();
				br.ReadInt16();
				br.ReadInt16();
				br.ReadInt16();
				if (br.ReadBoolean())
				{
					result.Flags |= 4U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 16U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 64U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 256U;
				}
				if (version >= 2 && br.ReadBoolean())
				{
					result.Flags |= 32U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 4096U;
				}
			}
		}
		result.ShouldBeCrawler = (!result.HasLeftLeg || !result.HasRightLeg);
		return result;
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000CE7FE File Offset: 0x000CC9FE
	public void Write(BinaryWriter bw)
	{
		bw.Write(BodyDamage.cBinaryVersion);
		bw.Write((int)this.damageType);
		bw.Write(this.Flags);
	}

	// Token: 0x040017E0 RID: 6112
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cBinaryVersion = 4;

	// Token: 0x040017E1 RID: 6113
	public int StunKnee;

	// Token: 0x040017E2 RID: 6114
	public int StunProne;

	// Token: 0x040017E3 RID: 6115
	public float StunDuration;

	// Token: 0x040017E4 RID: 6116
	public EnumEntityStunType CurrentStun;

	// Token: 0x040017E5 RID: 6117
	public bool ShouldBeCrawler;

	// Token: 0x040017E6 RID: 6118
	public const uint cNoHead = 1U;

	// Token: 0x040017E7 RID: 6119
	public const uint cNoArmLUpper = 2U;

	// Token: 0x040017E8 RID: 6120
	public const uint cNoArmLLower = 4U;

	// Token: 0x040017E9 RID: 6121
	public const uint cNoArmRUpper = 8U;

	// Token: 0x040017EA RID: 6122
	public const uint cNoArmRLower = 16U;

	// Token: 0x040017EB RID: 6123
	public const uint cNoArm = 30U;

	// Token: 0x040017EC RID: 6124
	public const uint cNoLegLUpper = 32U;

	// Token: 0x040017ED RID: 6125
	public const uint cNoLegLLower = 64U;

	// Token: 0x040017EE RID: 6126
	public const uint cNoLegRUpper = 128U;

	// Token: 0x040017EF RID: 6127
	public const uint cNoLegRLower = 256U;

	// Token: 0x040017F0 RID: 6128
	public const uint cNoLeg = 480U;

	// Token: 0x040017F1 RID: 6129
	public const uint cCrippledLegL = 4096U;

	// Token: 0x040017F2 RID: 6130
	public const uint cCrippledLegR = 8192U;

	// Token: 0x040017F3 RID: 6131
	public uint Flags;

	// Token: 0x040017F4 RID: 6132
	public EnumDamageTypes damageType;

	// Token: 0x040017F5 RID: 6133
	public EnumBodyPartHit bodyPartHit;
}
