using System;
using System.Runtime.CompilerServices;

namespace PrefabVolumes
{
	// Token: 0x020018AF RID: 6319
	public abstract class PrefabVolumeListAbs
	{
		// Token: 0x170017F1 RID: 6129
		// (get) Token: 0x0600C2F4 RID: 49908
		public abstract PrefabVolumeAbs.EVolumeType VolumeType { get; }

		// Token: 0x170017F2 RID: 6130
		// (get) Token: 0x0600C2F5 RID: 49909
		public abstract bool AnyUsedEntry { get; }

		// Token: 0x170017F3 RID: 6131
		// (get) Token: 0x0600C2F6 RID: 49910
		public abstract int Count { get; }

		// Token: 0x0600C2F7 RID: 49911
		public abstract PrefabVolumeAbs Get(int _index);

		// Token: 0x0600C2F8 RID: 49912
		public abstract void CopyFrom(PrefabVolumeListAbs _other);

		// Token: 0x0600C2F9 RID: 49913
		public abstract void ReadFromProperties(DynamicProperties _properties);

		// Token: 0x0600C2FA RID: 49914
		public abstract void WriteToProperties(DynamicProperties _properties);

		// Token: 0x0600C2FB RID: 49915
		public abstract void SendAllVolumesToClient(ClientInfo _clientInfo, int _prefabInstanceId);

		// Token: 0x0600C2FC RID: 49916
		public abstract void Move(Vector3i _moveDistance);

		// Token: 0x0600C2FD RID: 49917
		public abstract void RotateY(bool _bLeft, Vector3i _prefabSize);

		// Token: 0x0600C2FE RID: 49918
		public abstract bool CanCreateVolume(string _prefabInstanceName, Vector3i _bbPos, Vector3i _startPos, Vector3i _size);

		// Token: 0x0600C2FF RID: 49919
		[return: TupleElementNames(new string[]
		{
			"volumeIndex",
			"volume",
			"box"
		})]
		public abstract ValueTuple<int, PrefabVolumeAbs, SelectionBox> AddNewVolume(string _prefabInstanceName, Vector3i _bbPos, Vector3i _startPos, Vector3i _size);

		// Token: 0x0600C300 RID: 49920
		[return: TupleElementNames(new string[]
		{
			"volumeIndex",
			"volume",
			"box"
		})]
		public abstract ValueTuple<int, PrefabVolumeAbs, SelectionBox> CloneVolume(string _prefabInstanceName, Vector3i _bbPos, int _existingIndex, Vector3i _offset);

		// Token: 0x0600C301 RID: 49921
		public abstract void SetVolume(PrefabInstance _prefabInstance, int _index, PrefabVolumeAbs _volumeSettings);

		// Token: 0x0600C302 RID: 49922
		public abstract void CreateSelectionBoxes(PrefabInstance _prefabInstance);

		// Token: 0x0600C303 RID: 49923
		public abstract void ApplyVolumesToSelectionBoxes(PrefabInstance _prefabInstance);

		// Token: 0x0600C304 RID: 49924
		public abstract void RemoveSelectionBoxes(PrefabInstance _prefabInstance);

		// Token: 0x0600C305 RID: 49925
		public abstract void RemoveVolumes(PrefabInstance _prefabInstance);

		// Token: 0x0600C306 RID: 49926
		public abstract void CopyVolumesIntoWorld(World _world, Chunk _chunk, Vector3i _offset);

		// Token: 0x0600C307 RID: 49927 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public PrefabVolumeListAbs()
		{
		}
	}
}
