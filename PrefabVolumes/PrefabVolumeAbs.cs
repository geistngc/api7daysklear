using System;
using System.Runtime.CompilerServices;

namespace PrefabVolumes
{
	// Token: 0x020018BE RID: 6334
	public abstract class PrefabVolumeAbs
	{
		// Token: 0x17001804 RID: 6148
		// (get) Token: 0x0600C375 RID: 50037 RVA: 0x0048638D File Offset: 0x0048458D
		public bool Used
		{
			get
			{
				return this.used;
			}
		}

		// Token: 0x17001805 RID: 6149
		// (get) Token: 0x0600C376 RID: 50038 RVA: 0x00486395 File Offset: 0x00484595
		// (set) Token: 0x0600C377 RID: 50039 RVA: 0x0048639D File Offset: 0x0048459D
		public virtual Vector3i startPos
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.startPosInternal;
			}
			set
			{
				this.startPosInternal = value;
			}
		}

		// Token: 0x17001806 RID: 6150
		// (get) Token: 0x0600C378 RID: 50040 RVA: 0x004863A6 File Offset: 0x004845A6
		// (set) Token: 0x0600C379 RID: 50041 RVA: 0x004863AE File Offset: 0x004845AE
		public virtual Vector3i size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.sizeInternal;
			}
			set
			{
				this.sizeInternal = value;
			}
		}

		// Token: 0x17001807 RID: 6151
		// (get) Token: 0x0600C37A RID: 50042
		public abstract PrefabVolumeAbs.EVolumeType VolumeType { get; }

		// Token: 0x17001808 RID: 6152
		// (get) Token: 0x0600C37B RID: 50043
		public abstract int SerializedSize { get; }

		// Token: 0x0600C37C RID: 50044 RVA: 0x004863B7 File Offset: 0x004845B7
		public void Use(Vector3i _startPos, Vector3i _size)
		{
			this.used = true;
			this.startPos = _startPos;
			this.size = _size;
		}

		// Token: 0x0600C37D RID: 50045 RVA: 0x004863D0 File Offset: 0x004845D0
		public virtual void Reset()
		{
			this.used = false;
			this.startPos = default(Vector3i);
			this.size = default(Vector3i);
		}

		// Token: 0x0600C37E RID: 50046 RVA: 0x00486402 File Offset: 0x00484602
		public void MarkUnused()
		{
			this.used = false;
		}

		// Token: 0x0600C37F RID: 50047 RVA: 0x0048640B File Offset: 0x0048460B
		public virtual void Read(PooledBinaryReader _br)
		{
			this.used = _br.ReadBoolean();
			this.startPos = StreamUtils.ReadVector3i(_br);
			this.size = StreamUtils.ReadVector3i(_br);
		}

		// Token: 0x0600C380 RID: 50048 RVA: 0x00486431 File Offset: 0x00484631
		public virtual void Write(PooledBinaryWriter _bw)
		{
			_bw.Write(this.Used);
			StreamUtils.Write(_bw, this.startPos);
			StreamUtils.Write(_bw, this.size);
		}

		// Token: 0x0600C381 RID: 50049 RVA: 0x00486458 File Offset: 0x00484658
		public virtual void RotateY(bool _bLeft, Vector3i _prefabSize)
		{
			Vector3i size = this.size;
			Vector3i startPos = this.startPos;
			Vector3i vector3i = startPos + size;
			if (_bLeft)
			{
				startPos = new Vector3i(_prefabSize.z - startPos.z, startPos.y, startPos.x);
				vector3i = new Vector3i(_prefabSize.z - vector3i.z, vector3i.y, vector3i.x);
			}
			else
			{
				startPos = new Vector3i(startPos.z, startPos.y, _prefabSize.x - startPos.x);
				vector3i = new Vector3i(vector3i.z, vector3i.y, _prefabSize.x - vector3i.x);
			}
			if (startPos.x > vector3i.x)
			{
				MathUtils.Swap(ref startPos.x, ref vector3i.x);
			}
			if (startPos.z > vector3i.z)
			{
				MathUtils.Swap(ref startPos.z, ref vector3i.z);
			}
			this.startPos = startPos;
			MathUtils.Swap(ref size.x, ref size.z);
			this.size = size;
		}

		// Token: 0x0600C382 RID: 50050 RVA: 0x00486563 File Offset: 0x00484763
		public void Move(Vector3i _moveVector)
		{
			this.startPos += _moveVector;
		}

		// Token: 0x0600C383 RID: 50051 RVA: 0x00486578 File Offset: 0x00484778
		public void Resize(int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest, bool _allowXBelow1 = false, bool _allowYBelow1 = false, bool _allowZBelow1 = false)
		{
			Vector3i vector3i = this.size;
			vector3i += new Vector3i(_dEast + _dWest, _dTop + _dBottom, _dNorth + _dSouth);
			this.startPos += new Vector3i(-_dWest, -_dBottom, -_dSouth);
			if (!_allowXBelow1 && vector3i.x < 2)
			{
				vector3i = new Vector3i(1, vector3i.y, vector3i.z);
			}
			if (!_allowYBelow1 && vector3i.y < 2)
			{
				vector3i = new Vector3i(vector3i.x, 1, vector3i.z);
			}
			if (!_allowZBelow1 && vector3i.z < 2)
			{
				vector3i = new Vector3i(vector3i.x, vector3i.y, 1);
			}
			this.size = vector3i;
		}

		// Token: 0x0600C384 RID: 50052
		public abstract PrefabVolumeAbs Clone();

		// Token: 0x0600C385 RID: 50053
		public abstract void CopyValues(PrefabVolumeAbs _target, bool _nonBasicOnly = false);

		// Token: 0x0600C386 RID: 50054 RVA: 0x0048662C File Offset: 0x0048482C
		public static PrefabVolumeAbs CreateByType(PrefabVolumeAbs.EVolumeType _volumeType)
		{
			PrefabVolumeAbs result;
			switch (_volumeType)
			{
			case PrefabVolumeAbs.EVolumeType.Sleeper:
				result = new PrefabSleeperVolume();
				break;
			case PrefabVolumeAbs.EVolumeType.Teleport:
				result = new PrefabTeleportVolume();
				break;
			case PrefabVolumeAbs.EVolumeType.Info:
				result = new PrefabInfoVolume();
				break;
			case PrefabVolumeAbs.EVolumeType.Wall:
				result = new PrefabWallVolume();
				break;
			case PrefabVolumeAbs.EVolumeType.Trigger:
				result = new PrefabTriggerVolume();
				break;
			case PrefabVolumeAbs.EVolumeType.Marker:
				result = new Marker();
				break;
			default:
				throw new ArgumentOutOfRangeException("_volumeType", string.Format("Invalid volumeType {0}", _volumeType));
			}
			return result;
		}

		// Token: 0x0600C387 RID: 50055 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public PrefabVolumeAbs()
		{
		}

		// Token: 0x04009448 RID: 37960
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool used;

		// Token: 0x04009449 RID: 37961
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i startPosInternal;

		// Token: 0x0400944A RID: 37962
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i sizeInternal;

		// Token: 0x020018BF RID: 6335
		public enum EVolumeType : byte
		{
			// Token: 0x0400944C RID: 37964
			Sleeper,
			// Token: 0x0400944D RID: 37965
			Teleport,
			// Token: 0x0400944E RID: 37966
			Info,
			// Token: 0x0400944F RID: 37967
			Wall,
			// Token: 0x04009450 RID: 37968
			Trigger,
			// Token: 0x04009451 RID: 37969
			Marker
		}
	}
}
