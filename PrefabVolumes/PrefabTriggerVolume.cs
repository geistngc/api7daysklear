using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace PrefabVolumes
{
	// Token: 0x020018C5 RID: 6341
	[Preserve]
	public class PrefabTriggerVolume : PrefabVolumeAbs<PrefabTriggerVolume>
	{
		// Token: 0x17001811 RID: 6161
		// (get) Token: 0x0600C3A4 RID: 50084 RVA: 0x00080864 File Offset: 0x0007EA64
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Trigger;
			}
		}

		// Token: 0x17001812 RID: 6162
		// (get) Token: 0x0600C3A5 RID: 50085 RVA: 0x00486AB6 File Offset: 0x00484CB6
		public override int SerializedSize
		{
			get
			{
				return 26 + this.TriggersIndices.Count;
			}
		}

		// Token: 0x0600C3A6 RID: 50086 RVA: 0x00486AC6 File Offset: 0x00484CC6
		public override void Reset()
		{
			base.Reset();
			this.TriggersIndices.Clear();
		}

		// Token: 0x0600C3A7 RID: 50087 RVA: 0x00486ADC File Offset: 0x00484CDC
		public override void Read(PooledBinaryReader _br)
		{
			base.Read(_br);
			this.TriggersIndices.Clear();
			int num = (int)_br.ReadByte();
			for (int i = 0; i < num; i++)
			{
				this.TriggersIndices.Add(_br.ReadByte());
			}
		}

		// Token: 0x0600C3A8 RID: 50088 RVA: 0x00486B20 File Offset: 0x00484D20
		public override void Write(PooledBinaryWriter _bw)
		{
			base.Write(_bw);
			_bw.Write((byte)this.TriggersIndices.Count);
			for (int i = 0; i < this.TriggersIndices.Count; i++)
			{
				_bw.Write(this.TriggersIndices[i]);
			}
		}

		// Token: 0x0600C3A9 RID: 50089 RVA: 0x00486B6E File Offset: 0x00484D6E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void CopyValues(PrefabVolumeAbs<PrefabTriggerVolume> _target, bool _nonBasicOnly = false)
		{
			if (!_nonBasicOnly)
			{
				base.CopyValues(_target, false);
			}
			((PrefabTriggerVolume)_target).TriggersIndices.AddRange(this.TriggersIndices);
		}

		// Token: 0x0600C3AA RID: 50090 RVA: 0x00486B91 File Offset: 0x00484D91
		public void SetTriggersFlag(byte _index)
		{
			if (!this.TriggersIndices.Contains(_index))
			{
				this.TriggersIndices.Add(_index);
			}
		}

		// Token: 0x0600C3AB RID: 50091 RVA: 0x00486BAD File Offset: 0x00484DAD
		public void RemoveTriggersFlag(byte _index)
		{
			this.TriggersIndices.Remove(_index);
		}

		// Token: 0x0600C3AC RID: 50092 RVA: 0x00486BBC File Offset: 0x00484DBC
		public void RemoveAllTriggersFlags()
		{
			this.TriggersIndices.Clear();
		}

		// Token: 0x0600C3AD RID: 50093 RVA: 0x00486BC9 File Offset: 0x00484DC9
		public bool HasTriggers(byte _index)
		{
			return this.TriggersIndices.Contains(_index);
		}

		// Token: 0x0600C3AE RID: 50094 RVA: 0x00486BD8 File Offset: 0x00484DD8
		public void ToggleTriggersFlag(byte layer)
		{
			int num = this.TriggersIndices.IndexOf(layer);
			if (num >= 0)
			{
				this.TriggersIndices.RemoveAt(num);
				return;
			}
			this.TriggersIndices.Add(layer);
		}

		// Token: 0x0600C3AF RID: 50095 RVA: 0x00486C0F File Offset: 0x00484E0F
		public bool HasAnyTriggers()
		{
			return this.TriggersIndices.Count > 0;
		}

		// Token: 0x0400945B RID: 37979
		public readonly List<byte> TriggersIndices = new List<byte>();
	}
}
