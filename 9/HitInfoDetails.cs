using System;
using UnityEngine;

// Token: 0x02001301 RID: 4865
public struct HitInfoDetails
{
	// Token: 0x17001235 RID: 4661
	// (get) Token: 0x060099FF RID: 39423 RVA: 0x003A5C20 File Offset: 0x003A3E20
	public BlockValue blockValue
	{
		get
		{
			return this.voxelData.BlockValue;
		}
	}

	// Token: 0x17001236 RID: 4662
	// (get) Token: 0x06009A00 RID: 39424 RVA: 0x003A5C2D File Offset: 0x003A3E2D
	public WaterValue waterValue
	{
		get
		{
			return this.voxelData.WaterValue;
		}
	}

	// Token: 0x17001237 RID: 4663
	// (get) Token: 0x06009A01 RID: 39425 RVA: 0x003A5C3A File Offset: 0x003A3E3A
	public PropRef propRef
	{
		get
		{
			return this.propData.PropRef;
		}
	}

	// Token: 0x17001238 RID: 4664
	// (get) Token: 0x06009A02 RID: 39426 RVA: 0x003A5C47 File Offset: 0x003A3E47
	public PropValue propValue
	{
		get
		{
			return this.propData.PropValue;
		}
	}

	// Token: 0x17001239 RID: 4665
	// (get) Token: 0x06009A03 RID: 39427 RVA: 0x003A5C54 File Offset: 0x003A3E54
	public BlockValueRef blockValueRef
	{
		get
		{
			if (!this.propValue.IsAir)
			{
				return new BlockValueRef(this.propData.PropRef);
			}
			if (!this.blockValue.isair)
			{
				return new BlockValueRef(this.blockPos);
			}
			return BlockValueRef.None;
		}
	}

	// Token: 0x06009A04 RID: 39428 RVA: 0x003A5CA3 File Offset: 0x003A3EA3
	public void Clear()
	{
		this.pos = Vector3.zero;
		this.blockPos = Vector3i.zero;
		this.blockFace = BlockFace.Top;
		this.voxelData.Clear();
		this.propData.Clear();
		this.distanceSq = 0f;
	}

	// Token: 0x06009A05 RID: 39429 RVA: 0x003A5CE4 File Offset: 0x003A3EE4
	public void CopyFrom(HitInfoDetails _other)
	{
		this.pos = _other.pos;
		this.blockPos = _other.blockPos;
		this.blockFace = _other.blockFace;
		this.voxelData = _other.voxelData;
		this.propData = _other.propData;
		this.distanceSq = _other.distanceSq;
	}

	// Token: 0x06009A06 RID: 39430 RVA: 0x003A5D3C File Offset: 0x003A3F3C
	public HitInfoDetails Clone()
	{
		HitInfoDetails result = default(HitInfoDetails);
		result.CopyFrom(this);
		return result;
	}

	// Token: 0x0400741C RID: 29724
	public Vector3 pos;

	// Token: 0x0400741D RID: 29725
	public Vector3i blockPos;

	// Token: 0x0400741E RID: 29726
	public HitInfoDetails.VoxelData voxelData;

	// Token: 0x0400741F RID: 29727
	public HitInfoDetails.PropData propData;

	// Token: 0x04007420 RID: 29728
	public BlockFace blockFace;

	// Token: 0x04007421 RID: 29729
	public float distanceSq;

	// Token: 0x02001302 RID: 4866
	public struct VoxelData : IEquatable<HitInfoDetails.VoxelData>
	{
		// Token: 0x06009A07 RID: 39431 RVA: 0x003A5D5F File Offset: 0x003A3F5F
		public void Set(BlockValue _bv, WaterValue _wv)
		{
			this.BlockValue = _bv;
			this.WaterValue = _wv;
		}

		// Token: 0x06009A08 RID: 39432 RVA: 0x003A5D70 File Offset: 0x003A3F70
		public static HitInfoDetails.VoxelData GetFrom(ChunkCluster _cc, Vector3i _blockPos)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _cc.GetBlock(_blockPos),
				WaterValue = _cc.GetWater(_blockPos)
			};
		}

		// Token: 0x06009A09 RID: 39433 RVA: 0x003A5DA4 File Offset: 0x003A3FA4
		public static HitInfoDetails.VoxelData GetFrom(World _world, Vector3i _blockPos)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _world.GetBlock(_blockPos),
				WaterValue = _world.GetWater(_blockPos)
			};
		}

		// Token: 0x06009A0A RID: 39434 RVA: 0x003A5DD8 File Offset: 0x003A3FD8
		public static HitInfoDetails.VoxelData GetFrom(IChunk _chunk, int _x, int _y, int _z)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _chunk.GetBlock(_x, _y, _z),
				WaterValue = _chunk.GetWater(_x, _y, _z)
			};
		}

		// Token: 0x06009A0B RID: 39435 RVA: 0x003A5E0E File Offset: 0x003A400E
		public bool IsOnlyAir()
		{
			return this.BlockValue.isair && !this.WaterValue.HasMass();
		}

		// Token: 0x06009A0C RID: 39436 RVA: 0x003A5E2D File Offset: 0x003A402D
		public bool IsOnlyWater()
		{
			return this.BlockValue.isair && this.WaterValue.HasMass();
		}

		// Token: 0x06009A0D RID: 39437 RVA: 0x003A5E49 File Offset: 0x003A4049
		public bool Equals(HitInfoDetails.VoxelData _other)
		{
			return this.BlockValue.Equals(_other.BlockValue) && this.WaterValue.HasMass() == _other.WaterValue.HasMass();
		}

		// Token: 0x06009A0E RID: 39438 RVA: 0x003A5E79 File Offset: 0x003A4079
		public void Clear()
		{
			this.BlockValue = BlockValue.Air;
			this.WaterValue = WaterValue.Empty;
		}

		// Token: 0x04007422 RID: 29730
		public BlockValue BlockValue;

		// Token: 0x04007423 RID: 29731
		public WaterValue WaterValue;
	}

	// Token: 0x02001303 RID: 4867
	public struct PropData : IEquatable<HitInfoDetails.PropData>
	{
		// Token: 0x1700123A RID: 4666
		// (get) Token: 0x06009A0F RID: 39439 RVA: 0x003A5E91 File Offset: 0x003A4091
		// (set) Token: 0x06009A10 RID: 39440 RVA: 0x003A5E99 File Offset: 0x003A4099
		public PropRef PropRef { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700123B RID: 4667
		// (get) Token: 0x06009A11 RID: 39441 RVA: 0x003A5EA2 File Offset: 0x003A40A2
		// (set) Token: 0x06009A12 RID: 39442 RVA: 0x003A5EAA File Offset: 0x003A40AA
		public PropValue PropValue { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06009A13 RID: 39443 RVA: 0x003A5EB4 File Offset: 0x003A40B4
		public static HitInfoDetails.PropData GetFrom(ChunkCluster cc, Transform transform)
		{
			PropReference propReference;
			if (!transform || !transform.TryGetComponent<PropReference>(out propReference))
			{
				return HitInfoDetails.PropData.Empty;
			}
			PropRef propRef = propReference.PropRef;
			PropValue prop = cc.GetProp(propRef);
			if (prop.IsAir)
			{
				return HitInfoDetails.PropData.Empty;
			}
			return new HitInfoDetails.PropData
			{
				PropRef = propRef,
				PropValue = prop
			};
		}

		// Token: 0x06009A14 RID: 39444 RVA: 0x003A5F10 File Offset: 0x003A4110
		public override string ToString()
		{
			return string.Format("PropRef=[{0}], PropValue=[{1}]", this.PropRef, this.PropValue);
		}

		// Token: 0x06009A15 RID: 39445 RVA: 0x003A5F32 File Offset: 0x003A4132
		public void Clear()
		{
			this.PropRef = PropRef.Default;
			this.PropValue = HitInfoDetails.PropData.Empty.PropValue;
		}

		// Token: 0x06009A16 RID: 39446 RVA: 0x003A5F50 File Offset: 0x003A4150
		public bool Equals(HitInfoDetails.PropData other)
		{
			return this.PropRef.Equals(other.PropRef) && this.PropValue.Equals(other.PropValue);
		}

		// Token: 0x06009A17 RID: 39447 RVA: 0x003A5F8C File Offset: 0x003A418C
		public override bool Equals(object obj)
		{
			if (obj is HitInfoDetails.PropData)
			{
				HitInfoDetails.PropData other = (HitInfoDetails.PropData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06009A18 RID: 39448 RVA: 0x003A5FB1 File Offset: 0x003A41B1
		public override int GetHashCode()
		{
			return HashCode.Combine<PropRef, PropValue>(this.PropRef, this.PropValue);
		}

		// Token: 0x06009A19 RID: 39449 RVA: 0x003A5FC4 File Offset: 0x003A41C4
		public static bool operator ==(HitInfoDetails.PropData left, HitInfoDetails.PropData right)
		{
			return left.Equals(right);
		}

		// Token: 0x06009A1A RID: 39450 RVA: 0x003A5FCE File Offset: 0x003A41CE
		public static bool operator !=(HitInfoDetails.PropData left, HitInfoDetails.PropData right)
		{
			return !left.Equals(right);
		}

		// Token: 0x04007424 RID: 29732
		public static readonly HitInfoDetails.PropData Empty = new HitInfoDetails.PropData
		{
			PropRef = PropRef.Default,
			PropValue = PropValue.AIR
		};
	}
}
