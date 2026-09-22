using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PrefabVolumes
{
	// Token: 0x020018C6 RID: 6342
	public class Marker : PrefabVolumeAbs<Marker>
	{
		// Token: 0x17001813 RID: 6163
		// (get) Token: 0x0600C3B1 RID: 50097 RVA: 0x00486C32 File Offset: 0x00484E32
		public bool PartDirty
		{
			get
			{
				return this.partDirty;
			}
		}

		// Token: 0x17001814 RID: 6164
		// (get) Token: 0x0600C3B2 RID: 50098 RVA: 0x00486C3A File Offset: 0x00484E3A
		// (set) Token: 0x0600C3B3 RID: 50099 RVA: 0x00486C42 File Offset: 0x00484E42
		public string GroupName
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.groupName;
			}
			set
			{
				if (this.groupName != value)
				{
					this.color = default(Color);
					this.groupId = -1;
					this.groupName = value;
				}
			}
		}

		// Token: 0x17001815 RID: 6165
		// (get) Token: 0x0600C3B4 RID: 50100 RVA: 0x00486C6C File Offset: 0x00484E6C
		public Color GroupColor
		{
			get
			{
				if (this.color == default(Color))
				{
					GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(this.GroupId);
					this.color = new Color32((byte)tempGameRandom.RandomRange(0, 256), (byte)tempGameRandom.RandomRange(0, 256), (byte)tempGameRandom.RandomRange(0, 256), (this.MarkerType == Marker.MarkerTypes.PartSpawn) ? 32 : 128);
				}
				return this.color;
			}
		}

		// Token: 0x17001816 RID: 6166
		// (get) Token: 0x0600C3B5 RID: 50101 RVA: 0x00486CF0 File Offset: 0x00484EF0
		public int GroupId
		{
			get
			{
				if (this.groupId == -1)
				{
					this.groupId = this.GroupName.GetHashCode();
				}
				return this.groupId;
			}
		}

		// Token: 0x17001817 RID: 6167
		// (get) Token: 0x0600C3B6 RID: 50102 RVA: 0x00486395 File Offset: 0x00484595
		// (set) Token: 0x0600C3B7 RID: 50103 RVA: 0x0048639D File Offset: 0x0048459D
		public override Vector3i startPos
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

		// Token: 0x17001818 RID: 6168
		// (get) Token: 0x0600C3B8 RID: 50104 RVA: 0x004863A6 File Offset: 0x004845A6
		// (set) Token: 0x0600C3B9 RID: 50105 RVA: 0x00486D12 File Offset: 0x00484F12
		public override Vector3i size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.sizeInternal;
			}
			set
			{
				if (this.sizeInternal != value)
				{
					this.sizeInternal = value;
					this.partDirty = true;
				}
			}
		}

		// Token: 0x17001819 RID: 6169
		// (get) Token: 0x0600C3BA RID: 50106 RVA: 0x00080A2D File Offset: 0x0007EC2D
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Marker;
			}
		}

		// Token: 0x1700181A RID: 6170
		// (get) Token: 0x0600C3BB RID: 50107 RVA: 0x00486D30 File Offset: 0x00484F30
		public override int SerializedSize
		{
			get
			{
				int num = 26;
				string text = this.GroupName;
				int num2 = num + (1 + ((text != null) ? new int?(text.Length) : null)).GetValueOrDefault() + 100;
				string text2 = this.PartToSpawn;
				return num2 + (1 + ((text2 != null) ? new int?(text2.Length) : null)).GetValueOrDefault() + 1 + 4;
			}
		}

		// Token: 0x1700181B RID: 6171
		// (get) Token: 0x0600C3BC RID: 50108 RVA: 0x00486DDC File Offset: 0x00484FDC
		// (set) Token: 0x0600C3BD RID: 50109 RVA: 0x00486DE4 File Offset: 0x00484FE4
		public Marker.MarkerTypes MarkerType
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.markerType;
			}
			set
			{
				if (this.markerType != value)
				{
					this.markerType = value;
					this.partDirty = true;
				}
			}
		}

		// Token: 0x1700181C RID: 6172
		// (get) Token: 0x0600C3BE RID: 50110 RVA: 0x00486DFD File Offset: 0x00484FFD
		// (set) Token: 0x0600C3BF RID: 50111 RVA: 0x00486E05 File Offset: 0x00485005
		public FastTags<TagGroup.Poi> Tags
		{
			get
			{
				return this.tags;
			}
			set
			{
				this.tags = value;
			}
		}

		// Token: 0x1700181D RID: 6173
		// (get) Token: 0x0600C3C0 RID: 50112 RVA: 0x00486E0E File Offset: 0x0048500E
		// (set) Token: 0x0600C3C1 RID: 50113 RVA: 0x00486E16 File Offset: 0x00485016
		public string PartToSpawn
		{
			get
			{
				return this.partToSpawn;
			}
			set
			{
				if (this.partToSpawn != value)
				{
					this.partToSpawn = value;
					this.partDirty = true;
				}
			}
		}

		// Token: 0x1700181E RID: 6174
		// (get) Token: 0x0600C3C2 RID: 50114 RVA: 0x00486E34 File Offset: 0x00485034
		// (set) Token: 0x0600C3C3 RID: 50115 RVA: 0x00486E3C File Offset: 0x0048503C
		public byte Rotations
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.rotations;
			}
			set
			{
				if (this.rotations != value)
				{
					this.rotations = value;
					this.partDirty = true;
				}
			}
		}

		// Token: 0x1700181F RID: 6175
		// (get) Token: 0x0600C3C4 RID: 50116 RVA: 0x00486E55 File Offset: 0x00485055
		// (set) Token: 0x0600C3C5 RID: 50117 RVA: 0x00486E5D File Offset: 0x0048505D
		public float PartChanceToSpawn
		{
			get
			{
				return this.partChanceToSpawn;
			}
			set
			{
				if (!Mathf.Approximately((float)this.rotations, value))
				{
					this.partChanceToSpawn = value;
					this.partDirty = true;
				}
			}
		}

		// Token: 0x0600C3C6 RID: 50118 RVA: 0x00486E7C File Offset: 0x0048507C
		public override void Reset()
		{
			base.Reset();
			this.markerType = Marker.MarkerTypes.None;
			this.tags = FastTags<TagGroup.Poi>.none;
			this.groupId = -1;
			this.groupName = "new";
			this.color = default(Color);
			this.partToSpawn = null;
			this.rotations = 0;
			this.partChanceToSpawn = 0f;
			this.partDirty = false;
		}

		// Token: 0x0600C3C7 RID: 50119 RVA: 0x00486EE0 File Offset: 0x004850E0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void CopyValues(PrefabVolumeAbs<Marker> _target, bool _nonBasicOnly = false)
		{
			if (!_nonBasicOnly)
			{
				base.CopyValues(_target, false);
			}
			Marker marker = (Marker)_target;
			marker.MarkerType = this.MarkerType;
			marker.GroupName = this.GroupName;
			marker.Tags = this.Tags;
			marker.PartToSpawn = this.PartToSpawn;
			marker.Rotations = this.Rotations;
			marker.PartChanceToSpawn = this.PartChanceToSpawn;
		}

		// Token: 0x0600C3C8 RID: 50120 RVA: 0x00486F48 File Offset: 0x00485148
		public override void Read(PooledBinaryReader _br)
		{
			base.Read(_br);
			this.MarkerType = (Marker.MarkerTypes)_br.ReadByte();
			this.GroupName = StreamUtils.ReadString(_br);
			this.Tags = FastTags<TagGroup.Poi>.Parse(_br.ReadString());
			this.PartToSpawn = StreamUtils.ReadString(_br);
			this.Rotations = _br.ReadByte();
			this.PartChanceToSpawn = _br.ReadSingle();
		}

		// Token: 0x0600C3C9 RID: 50121 RVA: 0x00486FAC File Offset: 0x004851AC
		public override void Write(PooledBinaryWriter _bw)
		{
			base.Write(_bw);
			_bw.Write((byte)this.MarkerType);
			StreamUtils.Write(_bw, this.GroupName);
			_bw.Write(this.Tags.ToString());
			StreamUtils.Write(_bw, this.PartToSpawn);
			_bw.Write(this.Rotations);
			_bw.Write(this.PartChanceToSpawn);
		}

		// Token: 0x0400945C RID: 37980
		[PublicizedFrom(EAccessModifier.Private)]
		public Marker.MarkerTypes markerType;

		// Token: 0x0400945D RID: 37981
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Poi> tags;

		// Token: 0x0400945E RID: 37982
		[PublicizedFrom(EAccessModifier.Private)]
		public int groupId = -1;

		// Token: 0x0400945F RID: 37983
		[PublicizedFrom(EAccessModifier.Private)]
		public string groupName;

		// Token: 0x04009460 RID: 37984
		[PublicizedFrom(EAccessModifier.Private)]
		public Color color;

		// Token: 0x04009461 RID: 37985
		[PublicizedFrom(EAccessModifier.Private)]
		public string partToSpawn;

		// Token: 0x04009462 RID: 37986
		[PublicizedFrom(EAccessModifier.Private)]
		public byte rotations;

		// Token: 0x04009463 RID: 37987
		[PublicizedFrom(EAccessModifier.Private)]
		public float partChanceToSpawn = 1f;

		// Token: 0x04009464 RID: 37988
		[PublicizedFrom(EAccessModifier.Private)]
		public bool partDirty = true;

		// Token: 0x04009465 RID: 37989
		public static readonly List<Vector3i> MarkerSizes = new List<Vector3i>
		{
			Vector3i.one,
			new Vector3i(25, 0, 25),
			new Vector3i(42, 0, 42),
			new Vector3i(60, 0, 60),
			new Vector3i(100, 0, 100)
		};

		// Token: 0x020018C7 RID: 6343
		public enum MarkerTypes : byte
		{
			// Token: 0x04009467 RID: 37991
			None,
			// Token: 0x04009468 RID: 37992
			POISpawn,
			// Token: 0x04009469 RID: 37993
			RoadExit,
			// Token: 0x0400946A RID: 37994
			PartSpawn
		}

		// Token: 0x020018C8 RID: 6344
		public enum MarkerSize : byte
		{
			// Token: 0x0400946C RID: 37996
			One,
			// Token: 0x0400946D RID: 37997
			ExtraSmall,
			// Token: 0x0400946E RID: 37998
			Small,
			// Token: 0x0400946F RID: 37999
			Medium,
			// Token: 0x04009470 RID: 38000
			Large,
			// Token: 0x04009471 RID: 38001
			Custom
		}
	}
}
