using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PrefabVolumes
{
	// Token: 0x020018B0 RID: 6320
	public abstract class PrefabVolumeListAbs<TVolumeList, TVolume> : PrefabVolumeListAbs where TVolumeList : PrefabVolumeListAbs<TVolumeList, TVolume> where TVolume : PrefabVolumeAbs<TVolume>, new()
	{
		// Token: 0x170017F4 RID: 6132
		// (get) Token: 0x0600C308 RID: 49928
		public abstract SelectionCategory SelectionCategory { [PublicizedFrom(EAccessModifier.Protected)] get; }

		// Token: 0x170017F5 RID: 6133
		// (get) Token: 0x0600C309 RID: 49929 RVA: 0x00483DD4 File Offset: 0x00481FD4
		public override bool AnyUsedEntry
		{
			get
			{
				if (this.List.Count > 0)
				{
					return this.List.FindIndex((TVolume _volume) => _volume.Used) >= 0;
				}
				return false;
			}
		}

		// Token: 0x170017F6 RID: 6134
		// (get) Token: 0x0600C30A RID: 49930 RVA: 0x00483E21 File Offset: 0x00482021
		public override int Count
		{
			get
			{
				return this.List.Count;
			}
		}

		// Token: 0x170017F7 RID: 6135
		public TVolume this[int _index]
		{
			get
			{
				return this.List[_index];
			}
		}

		// Token: 0x0600C30C RID: 49932 RVA: 0x00483E3C File Offset: 0x0048203C
		public override PrefabVolumeAbs Get(int _index)
		{
			return this[_index];
		}

		// Token: 0x0600C30D RID: 49933 RVA: 0x00483E4A File Offset: 0x0048204A
		[PublicizedFrom(EAccessModifier.Protected)]
		public PrefabVolumeListAbs(Prefab _owner)
		{
			this.Owner = _owner;
		}

		// Token: 0x0600C30E RID: 49934 RVA: 0x00483E64 File Offset: 0x00482064
		public override void CopyFrom(PrefabVolumeListAbs _other)
		{
			TVolumeList tvolumeList = _other as TVolumeList;
			if (tvolumeList == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"PrefabVolumeList.CopyFrom: Other list (",
					_other.GetType().Name,
					") is not the same type as the current instance (",
					typeof(TVolumeList).Name,
					")"
				}));
				return;
			}
			for (int i = 0; i < tvolumeList.List.Count; i++)
			{
				this.List.Add(tvolumeList.List[i].CloneGeneric());
			}
		}

		// Token: 0x0600C30F RID: 49935 RVA: 0x00483F10 File Offset: 0x00482110
		public override void SendAllVolumesToClient(ClientInfo _clientInfo, int _prefabInstanceId)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEditorUpdateVolume>().Setup(NetPackageEditorUpdateVolume.EChangeType.Added, _prefabInstanceId, i, this.List[i]));
			}
		}

		// Token: 0x0600C310 RID: 49936 RVA: 0x00483F58 File Offset: 0x00482158
		public override void Move(Vector3i _moveDistance)
		{
			foreach (TVolume tvolume in this.List)
			{
				tvolume.Move(_moveDistance);
			}
		}

		// Token: 0x0600C311 RID: 49937 RVA: 0x00483FB0 File Offset: 0x004821B0
		public override void RotateY(bool _bLeft, Vector3i _prefabSize)
		{
			foreach (TVolume tvolume in this.List)
			{
				tvolume.RotateY(_bLeft, _prefabSize);
			}
		}

		// Token: 0x0600C312 RID: 49938 RVA: 0x00484008 File Offset: 0x00482208
		[PublicizedFrom(EAccessModifier.Protected)]
		[return: TupleElementNames(new string[]
		{
			"volume",
			"index",
			"name"
		})]
		public ValueTuple<TVolume, int, string> PrepareNewEntry(string _prefabInstanceName, Vector3i _startPos, Vector3i _size)
		{
			int num = -1;
			TVolume tvolume = default(TVolume);
			for (int i = 0; i < this.List.Count; i++)
			{
				if (!this.List[i].Used)
				{
					num = i;
					tvolume = this.List[i];
					break;
				}
			}
			if (tvolume == null)
			{
				tvolume = Activator.CreateInstance<TVolume>();
				num = this.List.Count;
				this.List.Add(tvolume);
			}
			tvolume.Reset();
			tvolume.Use(_startPos, _size);
			return new ValueTuple<TVolume, int, string>(tvolume, num, string.Format("{0}_{1}", _prefabInstanceName, num));
		}

		// Token: 0x0600C313 RID: 49939 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool CanCreateVolume(string _prefabInstanceName, Vector3i _bbPos, Vector3i _startPos, Vector3i _size)
		{
			return true;
		}

		// Token: 0x0600C314 RID: 49940 RVA: 0x004840B4 File Offset: 0x004822B4
		[return: TupleElementNames(new string[]
		{
			"volumeIndex",
			"volume",
			"box"
		})]
		public override ValueTuple<int, PrefabVolumeAbs, SelectionBox> AddNewVolume(string _prefabInstanceName, Vector3i _bbPos, Vector3i _startPos, Vector3i _size)
		{
			ValueTuple<TVolume, int, string> valueTuple = this.PrepareNewEntry(_prefabInstanceName, _startPos, _size);
			TVolume item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			string item3 = valueTuple.Item3;
			SelectionBox selectionBox = this.AddSelectionBox(item, item3, _bbPos + _startPos);
			SelectionBoxManager.Instance.SetActive(selectionBox, true);
			return new ValueTuple<int, PrefabVolumeAbs, SelectionBox>(item2, item, selectionBox);
		}

		// Token: 0x0600C315 RID: 49941 RVA: 0x00484108 File Offset: 0x00482308
		[return: TupleElementNames(new string[]
		{
			"volumeIndex",
			"volume",
			"box"
		})]
		public override ValueTuple<int, PrefabVolumeAbs, SelectionBox> CloneVolume(string _prefabInstanceName, Vector3i _bbPos, int _existingIndex, Vector3i _offset)
		{
			PrefabVolumeAbs prefabVolumeAbs = this.List[_existingIndex];
			ValueTuple<int, PrefabVolumeAbs, SelectionBox> valueTuple = this.AddNewVolume(_prefabInstanceName, _bbPos, prefabVolumeAbs.startPos + _offset, prefabVolumeAbs.size);
			int item = valueTuple.Item1;
			PrefabVolumeAbs item2 = valueTuple.Item2;
			SelectionBox item3 = valueTuple.Item3;
			prefabVolumeAbs.CopyValues(item2, true);
			item3.UserData = item2;
			return new ValueTuple<int, PrefabVolumeAbs, SelectionBox>(item, item2, item3);
		}

		// Token: 0x0600C316 RID: 49942 RVA: 0x00484170 File Offset: 0x00482370
		public override void SetVolume(PrefabInstance _prefabInstance, int _index, PrefabVolumeAbs _volumeSettings)
		{
			TVolume tvolume = _volumeSettings as TVolume;
			if (tvolume == null)
			{
				throw new ArgumentException("Can not add volume of type " + _volumeSettings.VolumeType.ToStringCached<PrefabVolumeAbs.EVolumeType>() + " to list of type " + this.VolumeType.ToStringCached<PrefabVolumeAbs.EVolumeType>(), "_volumeSettings");
			}
			this.SetVolume(_prefabInstance, _index, tvolume);
		}

		// Token: 0x0600C317 RID: 49943 RVA: 0x004841CC File Offset: 0x004823CC
		public virtual void SetVolume(PrefabInstance _prefabInstance, int _index, TVolume _volumeSettings)
		{
			while (_index >= this.List.Count)
			{
				this.List.Add(Activator.CreateInstance<TVolume>());
			}
			bool used = this.List[_index].Used;
			this.List[_index] = _volumeSettings;
			string name = _prefabInstance.name + "_" + _index.ToString();
			if (!_volumeSettings.Used)
			{
				if (used)
				{
					this.SelectionCategory.RemoveBox(name);
				}
				return;
			}
			if (!used)
			{
				SelectionBox box = this.AddSelectionBox(_volumeSettings, name, _prefabInstance.boundingBoxPosition + _volumeSettings.startPos);
				SelectionBoxManager.Instance.SetActive(box, true);
				return;
			}
			SelectionBox selectionBox;
			this.SelectionCategory.TryGetBox(name, out selectionBox);
			selectionBox.SetPositionAndSize(_prefabInstance.boundingBoxPosition + _volumeSettings.startPos, _volumeSettings.size);
			selectionBox.UserData = _volumeSettings;
		}

		// Token: 0x0600C318 RID: 49944 RVA: 0x004842C3 File Offset: 0x004824C3
		public virtual SelectionBox AddSelectionBox(TVolume _volume, string _name, Vector3i _pos)
		{
			SelectionBox selectionBox = this.SelectionCategory.AddBox(_name, _pos, _volume.size, false, false);
			selectionBox.UserData = _volume;
			return selectionBox;
		}

		// Token: 0x0600C319 RID: 49945 RVA: 0x004842EC File Offset: 0x004824EC
		public override void CreateSelectionBoxes(PrefabInstance _prefabInstance)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				TVolume tvolume = this.List[i];
				this.AddSelectionBox(tvolume, _prefabInstance.name + "_" + i.ToString(), _prefabInstance.boundingBoxPosition + tvolume.startPos);
			}
		}

		// Token: 0x0600C31A RID: 49946 RVA: 0x00484354 File Offset: 0x00482554
		public override void ApplyVolumesToSelectionBoxes(PrefabInstance _prefabInstance)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				TVolume tvolume = this.List[i];
				if (tvolume.Used)
				{
					SelectionBox box = this.SelectionCategory.GetBox(_prefabInstance.name + "_" + i.ToString());
					if (box != null)
					{
						box.SetPositionAndSize(_prefabInstance.boundingBoxPosition + tvolume.startPos, tvolume.size);
					}
				}
			}
		}

		// Token: 0x0600C31B RID: 49947 RVA: 0x004843E4 File Offset: 0x004825E4
		public override void RemoveSelectionBoxes(PrefabInstance _prefabInstance)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				if (this.List[i].Used)
				{
					this.SelectionCategory.RemoveBox(_prefabInstance.name + "_" + i.ToString());
				}
			}
		}

		// Token: 0x0600C31C RID: 49948 RVA: 0x00484444 File Offset: 0x00482644
		public override void RemoveVolumes(PrefabInstance _prefabInstance)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				TVolume tvolume = this.List[i];
				tvolume.MarkUnused();
				this.SetVolume(_prefabInstance, i, tvolume);
			}
		}

		// Token: 0x0600C31D RID: 49949 RVA: 0x001008C9 File Offset: 0x000FEAC9
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual int FindWorldVolume(World _world, Vector3i _min, Vector3i _max)
		{
			return -1;
		}

		// Token: 0x0600C31E RID: 49950 RVA: 0x001008C9 File Offset: 0x000FEAC9
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual int CreateWorldVolume(int _prefabVolumeIndex, TVolume _prefabVolume, Vector3i _offset, Vector3i _volumeMin, Vector3i _volumeMax, World _world, Vector3i _volumeWorldMin, Vector3i _volumeWorldMax)
		{
			return -1;
		}

		// Token: 0x0600C31F RID: 49951 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void AddWorldVolume(Chunk _chunk, int _volumeIndex)
		{
		}

		// Token: 0x0600C320 RID: 49952 RVA: 0x00484488 File Offset: 0x00482688
		[PublicizedFrom(EAccessModifier.Protected)]
		public void CopyVolumesIntoWorldCommon(World _world, Chunk _chunk, Vector3i _offset, Vector3i _padding)
		{
			Vector3i vector3i = Vector3i.zero;
			Vector3i vector3i2 = Vector3i.zero;
			if (_chunk != null)
			{
				vector3i = _chunk.GetWorldPos();
				vector3i2 = vector3i + new Vector3i(16, 256, 16);
			}
			for (int i = 0; i < this.List.Count; i++)
			{
				TVolume tvolume = this.List[i];
				if (tvolume.Used && (World.SandboxUseTraderArea == TraderAreaStates.Default || (tvolume.VolumeType != PrefabVolumeAbs.EVolumeType.Teleport && tvolume.VolumeType != PrefabVolumeAbs.EVolumeType.Wall)))
				{
					Vector3i startPos = tvolume.startPos;
					Vector3i volumeMax = startPos + tvolume.size;
					Vector3i vector3i3 = startPos + _offset;
					Vector3i vector3i4 = vector3i3 + tvolume.size;
					Vector3i vector3i5 = vector3i3 - _padding;
					Vector3i vector3i6 = vector3i4 + _padding;
					if (_chunk != null)
					{
						if (vector3i5.x < vector3i2.x && vector3i6.x > vector3i.x && vector3i5.y < vector3i2.y && vector3i6.y > vector3i.y && vector3i5.z < vector3i2.z && vector3i6.z > vector3i.z)
						{
							int num = this.FindWorldVolume(_world, vector3i3, vector3i4);
							if (num < 0)
							{
								num = this.CreateWorldVolume(i, tvolume, _offset, startPos, volumeMax, _world, vector3i3, vector3i4);
							}
							this.AddWorldVolume(_chunk, num);
						}
					}
					else
					{
						int num2 = this.FindWorldVolume(_world, vector3i3, vector3i4);
						if (num2 < 0)
						{
							num2 = this.CreateWorldVolume(i, tvolume, _offset, startPos, volumeMax, _world, vector3i3, vector3i4);
						}
						int num3 = World.toChunkXZ(vector3i5.x);
						int num4 = World.toChunkXZ(vector3i6.x - 1);
						int num5 = World.toChunkXZ(vector3i5.z);
						int num6 = World.toChunkXZ(vector3i6.z - 1);
						for (int j = num3; j <= num4; j++)
						{
							for (int k = num5; k <= num6; k++)
							{
								Chunk chunk = (Chunk)_world.GetChunkSync(j, k);
								if (chunk != null)
								{
									this.AddWorldVolume(chunk, num2);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600C321 RID: 49953 RVA: 0x000027FC File Offset: 0x000009FC
		public override void CopyVolumesIntoWorld(World _world, Chunk _chunk, Vector3i _offset)
		{
		}

		// Token: 0x0400942B RID: 37931
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly Prefab Owner;

		// Token: 0x0400942C RID: 37932
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly List<TVolume> List = new List<TVolume>();
	}
}
