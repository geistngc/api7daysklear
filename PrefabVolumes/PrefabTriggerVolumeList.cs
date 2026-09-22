using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PrefabVolumes
{
	// Token: 0x020018BA RID: 6330
	public class PrefabTriggerVolumeList : PrefabVolumeListAbs<PrefabTriggerVolumeList, PrefabTriggerVolume>
	{
		// Token: 0x0600C35A RID: 50010 RVA: 0x004857D9 File Offset: 0x004839D9
		public PrefabTriggerVolumeList(Prefab _owner) : base(_owner)
		{
		}

		// Token: 0x17001800 RID: 6144
		// (get) Token: 0x0600C35B RID: 50011 RVA: 0x00080864 File Offset: 0x0007EA64
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Trigger;
			}
		}

		// Token: 0x17001801 RID: 6145
		// (get) Token: 0x0600C35C RID: 50012 RVA: 0x004857E2 File Offset: 0x004839E2
		public override SelectionCategory SelectionCategory
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return SelectionBoxManager.Instance.CategoryTriggerVolume;
			}
		}

		// Token: 0x0600C35D RID: 50013 RVA: 0x004857F0 File Offset: 0x004839F0
		public override void ReadFromProperties(DynamicProperties _properties)
		{
			this.List.Clear();
			Dictionary<string, string> values = _properties.Values;
			if (!values.ContainsKey("TriggerVolumeSize") || !values.ContainsKey("TriggerVolumeStart"))
			{
				return;
			}
			List<Vector3i> list = StringParsers.ParseList<Vector3i>(values["TriggerVolumeSize"], '#', (string _s, int _start, int _end) => StringParsers.ParseVector3i(_s, _start, _end, false));
			List<Vector3i> list2 = StringParsers.ParseList<Vector3i>(values["TriggerVolumeStart"], '#', (string _s, int _start, int _end) => StringParsers.ParseVector3i(_s, _start, _end, false));
			List<string> list3 = StringParsers.ParseList<string>(values["TriggerVolumeTriggers"], '#', (string _s, int _start, int _end) => _s.Substring(_start, (_end == -1) ? (_s.Length - _start) : (_end + 1 - _start)));
			for (int i = 0; i < list2.Count; i++)
			{
				Vector3i startPos = list2[i];
				Vector3i size = (i < list.Count) ? list[i] : Vector3i.one;
				PrefabTriggerVolume prefabTriggerVolume = new PrefabTriggerVolume();
				prefabTriggerVolume.Use(startPos, size);
				if (list3[i].Trim() != "")
				{
					prefabTriggerVolume.TriggersIndices.AddRange(StringParsers.ParseList<byte>(list3[i], ',', (string _s, int _start, int _end) => StringParsers.ParseUInt8(_s, _start, _end, NumberStyles.Integer)));
				}
				this.List.Add(prefabTriggerVolume);
				this.Owner.HandleAddingTriggerLayers(prefabTriggerVolume);
			}
		}

		// Token: 0x0600C35E RID: 50014 RVA: 0x00485980 File Offset: 0x00483B80
		public override void WriteToProperties(DynamicProperties _properties)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			foreach (PrefabTriggerVolume prefabTriggerVolume in this.List)
			{
				if (prefabTriggerVolume.Used)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('#');
						stringBuilder2.Append('#');
						stringBuilder3.Append('#');
					}
					for (int i = 0; i < prefabTriggerVolume.TriggersIndices.Count; i++)
					{
						if (i > 0)
						{
							stringBuilder3.Append(',');
						}
						stringBuilder3.Append(prefabTriggerVolume.TriggersIndices[i].ToString());
					}
					if (prefabTriggerVolume.TriggersIndices.Count == 0)
					{
						stringBuilder3.Append(" ");
					}
					stringBuilder.Append(prefabTriggerVolume.size.ToString());
					stringBuilder2.Append(prefabTriggerVolume.startPos.ToString());
				}
			}
			if (stringBuilder.Length > 0)
			{
				_properties.Values["TriggerVolumeSize"] = stringBuilder.ToString();
				_properties.Values["TriggerVolumeStart"] = stringBuilder2.ToString();
				_properties.Values["TriggerVolumeTriggers"] = stringBuilder3.ToString();
				return;
			}
			_properties.Values.Remove("TriggerVolumeSize");
			_properties.Values.Remove("TriggerVolumeStart");
			_properties.Values.Remove("TriggerVolumeTriggers");
		}

		// Token: 0x0600C35F RID: 50015 RVA: 0x00485B30 File Offset: 0x00483D30
		[PublicizedFrom(EAccessModifier.Protected)]
		public override int FindWorldVolume(World _world, Vector3i _min, Vector3i _max)
		{
			return _world.FindTriggerVolume(_min, _max);
		}

		// Token: 0x0600C360 RID: 50016 RVA: 0x00485B3C File Offset: 0x00483D3C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override int CreateWorldVolume(int _prefabVolumeIndex, PrefabTriggerVolume _prefabVolume, Vector3i _offset, Vector3i _volumeMin, Vector3i _volumeMax, World _world, Vector3i _volumeWorldMin, Vector3i _volumeWorldMax)
		{
			TriggerVolume volume = TriggerVolume.Create(_prefabVolume, _volumeWorldMin, _volumeWorldMax);
			return _world.AddTriggerVolume(volume);
		}

		// Token: 0x0600C361 RID: 50017 RVA: 0x00485B5C File Offset: 0x00483D5C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void AddWorldVolume(Chunk _chunk, int _volumeIndex)
		{
			_chunk.AddTriggerVolumeId(_volumeIndex);
		}

		// Token: 0x0600C362 RID: 50018 RVA: 0x00485B65 File Offset: 0x00483D65
		public override void CopyVolumesIntoWorld(World _world, Chunk _chunk, Vector3i _offset)
		{
			base.CopyVolumesIntoWorldCommon(_world, _chunk, _offset, SleeperVolume.chunkPadding);
		}
	}
}
