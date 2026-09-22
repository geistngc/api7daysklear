using System;
using System.Collections.Generic;

namespace PrefabVolumes
{
	// Token: 0x020018C1 RID: 6337
	public class PrefabSleeperVolume : PrefabVolumeAbs<PrefabSleeperVolume>
	{
		// Token: 0x0600C38D RID: 50061 RVA: 0x00486764 File Offset: 0x00484964
		public override void Reset()
		{
			base.Reset();
			this.groupName = "GroupGenericZombie";
			this.isPriority = false;
			this.isQuestExclude = false;
			this.spawnCountMin = 5;
			this.spawnCountMax = 6;
			this.groupId = 0;
			this.flags = 0;
			this.minScript = null;
		}

		// Token: 0x0600C38E RID: 50062 RVA: 0x004867B4 File Offset: 0x004849B4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void CopyValues(PrefabVolumeAbs<PrefabSleeperVolume> _target, bool _nonBasicOnly = false)
		{
			if (!_nonBasicOnly)
			{
				base.CopyValues(_target, false);
			}
			PrefabSleeperVolume prefabSleeperVolume = (PrefabSleeperVolume)_target;
			prefabSleeperVolume.groupId = this.groupId;
			prefabSleeperVolume.groupName = this.groupName;
			prefabSleeperVolume.isPriority = this.isPriority;
			prefabSleeperVolume.isQuestExclude = this.isQuestExclude;
			prefabSleeperVolume.spawnCountMin = this.spawnCountMin;
			prefabSleeperVolume.spawnCountMax = this.spawnCountMax;
			prefabSleeperVolume.triggeredByIndices.AddRange(this.triggeredByIndices);
			prefabSleeperVolume.flags = this.flags;
			prefabSleeperVolume.minScript = this.minScript;
		}

		// Token: 0x17001809 RID: 6153
		// (get) Token: 0x0600C38F RID: 50063 RVA: 0x00010E62 File Offset: 0x0000F062
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Sleeper;
			}
		}

		// Token: 0x1700180A RID: 6154
		// (get) Token: 0x0600C390 RID: 50064 RVA: 0x00486844 File Offset: 0x00484A44
		public override int SerializedSize
		{
			get
			{
				int num = 25;
				string text = this.groupName;
				int num2 = num + (1 + ((text != null) ? new int?(text.Length) : null)).GetValueOrDefault() + 1 + 1 + 2 + 2 + 2 + 4;
				string text2 = this.minScript;
				return num2 + (1 + ((text2 != null) ? new int?(text2.Length) : null)).GetValueOrDefault();
			}
		}

		// Token: 0x0600C391 RID: 50065 RVA: 0x004868F8 File Offset: 0x00484AF8
		public override void Read(PooledBinaryReader _br)
		{
			base.Read(_br);
			this.groupName = _br.ReadString();
			this.isPriority = _br.ReadBoolean();
			this.isQuestExclude = _br.ReadBoolean();
			this.spawnCountMin = _br.ReadInt16();
			this.spawnCountMax = _br.ReadInt16();
			this.groupId = _br.ReadInt16();
			this.flags = _br.ReadInt32();
			this.minScript = _br.ReadString();
		}

		// Token: 0x0600C392 RID: 50066 RVA: 0x0048696C File Offset: 0x00484B6C
		public override void Write(PooledBinaryWriter _bw)
		{
			base.Write(_bw);
			_bw.Write(this.groupName);
			_bw.Write(this.isPriority);
			_bw.Write(this.isQuestExclude);
			_bw.Write(this.spawnCountMin);
			_bw.Write(this.spawnCountMax);
			_bw.Write(this.groupId);
			_bw.Write(this.flags);
			_bw.Write(this.minScript ?? "");
		}

		// Token: 0x0600C393 RID: 50067 RVA: 0x004869E9 File Offset: 0x00484BE9
		public void SetTrigger(SleeperVolume.ETriggerType _type)
		{
			this.flags = ((this.flags & -8) | (int)_type);
		}

		// Token: 0x0600C394 RID: 50068 RVA: 0x004869FC File Offset: 0x00484BFC
		public void SetTriggeredByFlag(byte _index)
		{
			if (!this.triggeredByIndices.Contains(_index))
			{
				this.triggeredByIndices.Add(_index);
			}
		}

		// Token: 0x0600C395 RID: 50069 RVA: 0x00486A18 File Offset: 0x00484C18
		public void ClearTriggeredBy()
		{
			this.triggeredByIndices.Clear();
		}

		// Token: 0x0600C396 RID: 50070 RVA: 0x00486A25 File Offset: 0x00484C25
		public void RemoveTriggeredByFlag(byte _index)
		{
			this.triggeredByIndices.Remove(_index);
		}

		// Token: 0x0600C397 RID: 50071 RVA: 0x00486A34 File Offset: 0x00484C34
		public bool HasTriggeredBy(byte _index)
		{
			return this.triggeredByIndices.Contains(_index);
		}

		// Token: 0x0600C398 RID: 50072 RVA: 0x00486A44 File Offset: 0x00484C44
		public void ToggleTriggeredByFlag(byte layer)
		{
			int num = this.triggeredByIndices.IndexOf(layer);
			if (num >= 0)
			{
				this.triggeredByIndices.RemoveAt(num);
				return;
			}
			this.triggeredByIndices.Add(layer);
		}

		// Token: 0x0600C399 RID: 50073 RVA: 0x00486A7B File Offset: 0x00484C7B
		public bool HasAnyTriggeredBy()
		{
			return this.triggeredByIndices.Count > 0;
		}

		// Token: 0x04009452 RID: 37970
		public string groupName;

		// Token: 0x04009453 RID: 37971
		public bool isPriority;

		// Token: 0x04009454 RID: 37972
		public bool isQuestExclude;

		// Token: 0x04009455 RID: 37973
		public short spawnCountMin;

		// Token: 0x04009456 RID: 37974
		public short spawnCountMax;

		// Token: 0x04009457 RID: 37975
		public short groupId;

		// Token: 0x04009458 RID: 37976
		public int flags;

		// Token: 0x04009459 RID: 37977
		public string minScript;

		// Token: 0x0400945A RID: 37978
		public readonly List<byte> triggeredByIndices = new List<byte>();
	}
}
