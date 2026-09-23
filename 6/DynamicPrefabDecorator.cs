using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using PrefabVolumes;
using UnityEngine;

// Token: 0x0200091E RID: 2334
public class DynamicPrefabDecorator : IDynamicDecorator, ISelectionBoxCallback
{
	// Token: 0x1400004D RID: 77
	// (add) Token: 0x060043A0 RID: 17312 RVA: 0x001A5638 File Offset: 0x001A3838
	// (remove) Token: 0x060043A1 RID: 17313 RVA: 0x001A5670 File Offset: 0x001A3870
	public event Action<PrefabInstance> OnPrefabLoaded;

	// Token: 0x1400004E RID: 78
	// (add) Token: 0x060043A2 RID: 17314 RVA: 0x001A56A8 File Offset: 0x001A38A8
	// (remove) Token: 0x060043A3 RID: 17315 RVA: 0x001A56E0 File Offset: 0x001A38E0
	public event Action<PrefabInstance> OnPrefabChanged;

	// Token: 0x1400004F RID: 79
	// (add) Token: 0x060043A4 RID: 17316 RVA: 0x001A5718 File Offset: 0x001A3918
	// (remove) Token: 0x060043A5 RID: 17317 RVA: 0x001A5750 File Offset: 0x001A3950
	public event Action<PrefabInstance> OnPrefabRemoved;

	// Token: 0x060043A6 RID: 17318 RVA: 0x001A5788 File Offset: 0x001A3988
	public DynamicPrefabDecorator(PrefabCache _prefabCache)
	{
		this.prefabCache = _prefabCache;
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x001A5845 File Offset: 0x001A3A45
	public IEnumerator Load(string _path, bool _skipBlockData = false)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			yield break;
		}
		if (!SdFile.Exists(_path + "/prefabs.xml"))
		{
			yield break;
		}
		MicroStopwatch msw = new MicroStopwatch(true);
		XmlFile xmlFile;
		try
		{
			this.id = 0;
			xmlFile = new XmlFile(_path, "prefabs", false, false);
		}
		catch (Exception ex)
		{
			Log.Error("Loading prefabs xml file for level '" + Path.GetFileName(_path) + "': " + ex.Message);
			Log.Exception(ex);
			yield break;
		}
		int i = 0;
		int totalPrefabs = xmlFile.XmlDoc.Root.Elements("decoration").Count<XElement>();
		LocalPlayerUI ui = LocalPlayerUI.primaryUI;
		bool progressWindowOpen = ui && ui.windowManager.IsWindowOpen(XUiC_ProgressWindow.ID);
		ConnectionManager instance = SingletonMonoBehaviour<ConnectionManager>.Instance;
		bool loadAreas = instance != null && instance.IsServer;
		foreach (XElement element in xmlFile.XmlDoc.Root.Elements("decoration"))
		{
			try
			{
				int num = i;
				i = num + 1;
				if (element.HasAttribute("name"))
				{
					string attribute = element.GetAttribute("name");
					Vector3i vector3i = Vector3i.Parse(element.GetAttribute("position"));
					bool flag;
					StringParsers.TryParseBool(element.GetAttribute("y_is_groundlevel"), out flag);
					byte rotation = 0;
					if (element.HasAttribute("rotation"))
					{
						rotation = byte.Parse(element.GetAttribute("rotation"));
					}
					Prefab prefabRotated = this.prefabCache.GetPrefabRotated(attribute, (int)rotation, true, true, false, _skipBlockData);
					if (prefabRotated == null)
					{
						Log.Warning("Could not load prefab '" + attribute + "'. Skipping it");
						continue;
					}
					if (flag)
					{
						vector3i.y += prefabRotated.yOffset;
					}
					if (prefabRotated.bTraderArea && loadAreas)
					{
						this.AddTrader(new TraderArea(vector3i, prefabRotated.size, prefabRotated.TraderAreaProtect, prefabRotated.TeleportVolumeList));
					}
					if (!prefabRotated.bAllowDecorations && loadAreas)
					{
						this.AddDecoSuppressArea(vector3i, prefabRotated.size);
					}
					num = this.id;
					this.id = num + 1;
					PrefabInstance prefabInstance = new PrefabInstance(num, prefabRotated.location, vector3i, rotation, prefabRotated, 0);
					this.AddWorldPrefab(prefabInstance, prefabInstance.prefab.HasQuestTag());
				}
			}
			catch (Exception ex2)
			{
				Log.Error("Loading prefabs xml file for level '" + Path.GetFileName(_path) + "': " + ex2.Message);
				Log.Exception(ex2);
			}
			if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				if (progressWindowOpen)
				{
					XUiC_ProgressWindow.SetText(ui, string.Format(Localization.Get("uiLoadCreatingWorldPrefabs", false, null), Math.Min(100.0, 105.0 * (double)i / (double)totalPrefabs).ToString("0")), true);
				}
				yield return null;
				msw.ResetAndRestart();
			}
		}
		IEnumerator<XElement> enumerator = null;
		if (progressWindowOpen)
		{
			XUiC_ProgressWindow.SetText(ui, string.Format(Localization.Get("uiLoadCreatingWorldPrefabs", false, null), "100"), true);
			yield return null;
		}
		this.SortPrefabs();
		XUiC_ProgressWindow.SetText(ui, Localization.Get("uiLoadCreatingWorld", false, null), true);
		yield return null;
		yield break;
		yield break;
	}

	// Token: 0x060043A8 RID: 17320 RVA: 0x001A5864 File Offset: 0x001A3A64
	[PublicizedFrom(EAccessModifier.Private)]
	public void SortPrefabs()
	{
		object obj = this.listsLock;
		lock (obj)
		{
			this.allPrefabsSorted.Clear();
			this.allPrefabsSorted.AddRange(this.allPrefabs);
			this.allPrefabsSorted.Sort((PrefabInstance a, PrefabInstance b) => a.boundingBoxPosition.x.CompareTo(b.boundingBoxPosition.x));
			this.isSortNeeded = false;
		}
	}

	// Token: 0x060043A9 RID: 17321 RVA: 0x001A58EC File Offset: 0x001A3AEC
	public int GetNextId()
	{
		int num = this.id;
		this.id = num + 1;
		return num;
	}

	// Token: 0x060043AA RID: 17322 RVA: 0x001A590C File Offset: 0x001A3B0C
	public bool Save(string _path)
	{
		bool result;
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.CreateXmlDeclaration();
			XmlElement node = xmlDocument.AddXmlElement("prefabs");
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				if (prefabInstance != null)
				{
					string value = "";
					Vector3i boundingBoxPosition = prefabInstance.boundingBoxPosition;
					if (prefabInstance.prefab != null && prefabInstance.prefab.location.Type != PathAbstractions.EAbstractedLocationType.None)
					{
						value = prefabInstance.prefab.PrefabName;
						boundingBoxPosition.y -= prefabInstance.prefab.yOffset;
					}
					else if (prefabInstance.location.Type != PathAbstractions.EAbstractedLocationType.None)
					{
						value = prefabInstance.location.Name;
					}
					node.AddXmlElement("decoration").SetAttrib("type", "model").SetAttrib("name", value).SetAttrib("position", boundingBoxPosition.ToStringNoBlanks()).SetAttrib("rotation", prefabInstance.rotation.ToString()).SetAttrib("y_is_groundlevel", "true");
				}
			}
			xmlDocument.SdSave(_path + "/prefabs.xml");
			result = true;
		}
		catch (Exception ex)
		{
			Log.Error(ex.ToString());
			Log.Error(ex.StackTrace);
			result = false;
		}
		return result;
	}

	// Token: 0x060043AB RID: 17323 RVA: 0x001A5A74 File Offset: 0x001A3C74
	public void Cleanup()
	{
		this.prefabMeshExisting.Clear();
	}

	// Token: 0x060043AC RID: 17324 RVA: 0x001A5A84 File Offset: 0x001A3C84
	public void GetAllPrefabs(List<PrefabInstance> _prefabs)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			_prefabs.AddRange(this.allPrefabs);
		}
	}

	// Token: 0x060043AD RID: 17325 RVA: 0x001A5ACC File Offset: 0x001A3CCC
	public void GetPOIPrefabs(List<PrefabInstance> _prefabs)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			_prefabs.AddRange(this.poiPrefabs);
		}
	}

	// Token: 0x060043AE RID: 17326 RVA: 0x001A5B14 File Offset: 0x001A3D14
	public void GetWorldPrefabs(List<PrefabInstance> _prefabs)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			_prefabs.AddRange(this.worldPrefabs);
		}
	}

	// Token: 0x060043AF RID: 17327 RVA: 0x001A5B5C File Offset: 0x001A3D5C
	public void AddWorldPrefab(PrefabInstance _pi, bool _isPOI = false)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			this.allPrefabs.Add(_pi);
			this.worldPrefabs.Add(_pi);
			if (_isPOI)
			{
				this.poiPrefabs.Add(_pi);
			}
			this.isSortNeeded = true;
		}
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x001A5BC4 File Offset: 0x001A3DC4
	public IEnumerator RequestWorldPOIMetadataFromServer()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Error("[DynamicPrefabDecorator] Only a client can request POI metadata.");
			yield break;
		}
		if (this.poiMetadataRequestPending)
		{
			yield break;
		}
		this.poiMetadataRequestPending = true;
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePOIMetadataRequest>().Setup(), false);
		while (this.poiMetadataRequestPending)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				this.poiMetadataRequestPending = false;
				yield break;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060043B1 RID: 17329 RVA: 0x001A5BD4 File Offset: 0x001A3DD4
	public void SendPOIMetadataToClient(ClientInfo _client)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			Log.Error("[DynamicPrefabDecorator] Only the server can send POI metadata.");
			return;
		}
		List<PrefabInstance> list = new List<PrefabInstance>();
		this.GetWorldPrefabs(list);
		_client.SendPackage(NetPackageManager.GetPackage<NetPackagePOIMetadataResponse>().Setup(list));
	}

	// Token: 0x060043B2 RID: 17330 RVA: 0x001A5C18 File Offset: 0x001A3E18
	public void ProcessPOIMetadataReceived(List<PrefabInstance.POIMetadata> _poiMetadatas)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Error("[DynamicPrefabDecorator] Only a client can receive POI metadata.");
			return;
		}
		if (_poiMetadatas != null)
		{
			for (int i = 0; i < _poiMetadatas.Count; i++)
			{
				PrefabInstance.POIMetadata poimetadata = _poiMetadatas[i];
				int num = this.id;
				this.id = num + 1;
				PrefabInstance prefabInstance = poimetadata.ToPrefabInstance(num);
				this.AddWorldPrefab(prefabInstance, prefabInstance.prefab.HasQuestTag());
			}
		}
		this.poiMetadataRequestPending = false;
		this.SortPrefabs();
	}

	// Token: 0x060043B3 RID: 17331 RVA: 0x001A5C90 File Offset: 0x001A3E90
	public void RemoveWorldPrefab(PrefabInstance _pi)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			if (!this.worldPrefabs.Remove(_pi))
			{
				Log.Warning(string.Format("{0} is not a world prefab", _pi));
			}
			this.allPrefabs.Remove(_pi);
			this.poiPrefabs.Remove(_pi);
			this.allPrefabsSorted.Remove(_pi);
		}
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x001A5D10 File Offset: 0x001A3F10
	public void AddEventPrefab(PrefabInstance _pi)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			this.allPrefabs.Add(_pi);
			this.isSortNeeded = true;
		}
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x001A5D60 File Offset: 0x001A3F60
	public void RemoveEventPrefab(PrefabInstance _pi)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			this.allPrefabs.Remove(_pi);
			this.allPrefabsSorted.Remove(_pi);
		}
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x001A5DB4 File Offset: 0x001A3FB4
	public void ClearTraders()
	{
		this.traderStore.Clear();
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x001A5DC1 File Offset: 0x001A3FC1
	public void AddTrader(TraderArea _ta)
	{
		this.traderStore.Add(_ta);
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x001A5DCF File Offset: 0x001A3FCF
	public List<TraderArea> GetTraderAreas()
	{
		return this.traderStore.Areas;
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x001A5DDC File Offset: 0x001A3FDC
	public bool IsWithinTraderArea(Vector3i _minPos, Vector3i _maxPos)
	{
		return this.traderStore.OverlapsBounds(_minPos, _maxPos);
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x001A5DEB File Offset: 0x001A3FEB
	public TraderArea GetTraderAtPosition(Vector3i _pos, int _padding)
	{
		return this.traderStore.GetAt(_pos.x, _pos.z, _padding);
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x001A5E05 File Offset: 0x001A4005
	public TraderArea GetTraderOuterAtPosition(Vector3i _pos, int _depth)
	{
		return this.traderStore.GetOuterAt(_pos.x, _pos.z, _depth);
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x001A5E1F File Offset: 0x001A401F
	public void ClearDecoSuppressAreas()
	{
		this.decoSuppressStore.Clear();
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x001A5E2C File Offset: 0x001A402C
	public void AddDecoSuppressArea(Vector3i _pos, Vector3i _size)
	{
		this.decoSuppressStore.Add(new DecoSuppressArea(_pos, _size));
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x001A5E40 File Offset: 0x001A4040
	public bool IsWithinDecoSuppressArea(Vector3i _minPos, Vector3i _maxPos)
	{
		return this.decoSuppressStore.OverlapsBounds(_minPos, _maxPos);
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x001A5E4F File Offset: 0x001A404F
	public bool IsDecorationSuppressedAt(int _x, int _z)
	{
		return this.decoSuppressStore.ContainsPoint(_x, _z, 0);
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x001A5E60 File Offset: 0x001A4060
	public void CopyAllPrefabsIntoWorld(World _world, bool _bOverwriteExistingBlocks = false)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				if (this.allPrefabs[i].standaloneBlockSize == 0)
				{
					this.allPrefabs[i].CopyIntoWorld(_world, true, _bOverwriteExistingBlocks, FastTags<TagGroup.Global>.none);
				}
				else
				{
					Log.Warning("Prefab with standaloneBlockSize={0} not supported", new object[]
					{
						this.allPrefabs[i].standaloneBlockSize
					});
				}
			}
		}
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x001A5F08 File Offset: 0x001A4108
	public void CleanAllPrefabsFromWorld(World _world)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				this.allPrefabs[i].CleanFromWorld(_world, true);
			}
		}
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x001A5F6C File Offset: 0x001A416C
	public void ClearAllPrefabs()
	{
		object obj = this.listsLock;
		lock (obj)
		{
			foreach (PrefabInstance prefabInstance in this.allPrefabs)
			{
				this.CallPrefabRemovedEvent(prefabInstance);
			}
			this.allPrefabs.Clear();
			this.poiPrefabs.Clear();
			this.worldPrefabs.Clear();
			this.allPrefabsSorted.Clear();
		}
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x001A6014 File Offset: 0x001A4214
	[PublicizedFrom(EAccessModifier.Private)]
	public void CallPrefabRemovedEvent(PrefabInstance _prefabInstance)
	{
		if (this.OnPrefabRemoved != null)
		{
			this.OnPrefabRemoved(_prefabInstance);
		}
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x001A602C File Offset: 0x001A422C
	public void CallPrefabChangedEvent(PrefabInstance _prefabInstance)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			this.isSortNeeded = true;
		}
		if (this.OnPrefabChanged != null)
		{
			this.OnPrefabChanged(_prefabInstance);
		}
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x001A6084 File Offset: 0x001A4284
	public void CreateBoundingBoxes()
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				this.allPrefabs[i].CreateBoundingBox(false);
			}
		}
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x001A60E8 File Offset: 0x001A42E8
	public PrefabInstance GetPrefab(int _id)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				if (this.allPrefabs[i].id == _id)
				{
					return this.allPrefabs[i];
				}
			}
		}
		return null;
	}

	// Token: 0x060043C7 RID: 17351 RVA: 0x001A6160 File Offset: 0x001A4360
	public bool IsActivePrefab(int _id)
	{
		PrefabInstance prefab = this.GetPrefab(_id);
		return prefab != null && prefab == this.ActivePrefab;
	}

	// Token: 0x060043C8 RID: 17352 RVA: 0x001A6184 File Offset: 0x001A4384
	public PrefabInstance CreateNewPrefabAndActivate(PathAbstractions.AbstractedLocation _location, Vector3i _position, Prefab _bad, bool _bSetActive = true)
	{
		if (_bad == null)
		{
			_bad = new Prefab(new Vector3i(3, 3, 3));
		}
		PrefabInstance prefabInstance = new PrefabInstance(this.GetNextId(), _location, _position, 0, _bad, 0);
		this.AddWorldPrefab(prefabInstance, false);
		prefabInstance.CreateBoundingBox(true);
		if (_bSetActive)
		{
			SelectionBox box;
			SelectionBoxManager.Instance.CategoryDynamicPrefab.TryGetBox(prefabInstance.name, out box);
			SelectionBoxManager.Instance.SetActive(box, true);
		}
		if (this.OnPrefabLoaded != null)
		{
			this.OnPrefabLoaded(prefabInstance);
		}
		return prefabInstance;
	}

	// Token: 0x060043C9 RID: 17353 RVA: 0x001A6200 File Offset: 0x001A4400
	public PrefabInstance RemoveActivePrefab(World _world)
	{
		if (this.ActivePrefab == null)
		{
			return null;
		}
		PrefabInstance activePrefab = this.ActivePrefab;
		this.RemovePrefabAndSelection(_world, activePrefab, true);
		this.ActivePrefab = null;
		return activePrefab;
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x001A6230 File Offset: 0x001A4430
	public void RemovePrefabAndSelection(World _world, PrefabInstance _prefab, bool _bCleanFromWorld)
	{
		if (_bCleanFromWorld)
		{
			_prefab.CleanFromWorld(_world, true);
		}
		this.RemoveWorldPrefab(_prefab);
		SelectionBoxManager.Instance.CategoryDynamicPrefab.RemoveBox(_prefab.name);
		foreach (PrefabVolumeListAbs prefabVolumeListAbs in _prefab.prefab.AllVolumeLists)
		{
			prefabVolumeListAbs.RemoveVolumes(_prefab);
		}
		SelectionBoxManager.Instance.CategorySleeperVolume.RemoveBox(_prefab.name);
		this.CallPrefabRemovedEvent(_prefab);
	}

	// Token: 0x060043CB RID: 17355 RVA: 0x001A62CC File Offset: 0x001A44CC
	public virtual void DecorateChunk(World _world, Chunk _chunk)
	{
		this.DecorateChunk(_world, _chunk, false);
	}

	// Token: 0x060043CC RID: 17356 RVA: 0x001A62D8 File Offset: 0x001A44D8
	public void DecorateChunk(World _world, Chunk _chunk, bool _bForceOverwriteBlocks = false)
	{
		List<PrefabInstance> value = this.decorateChunkPIs.Value;
		object obj = this.listsLock;
		lock (obj)
		{
			int blockWorldPosX = _chunk.GetBlockWorldPosX(0);
			int blockWorldPosZ = _chunk.GetBlockWorldPosZ(0);
			this.GetPrefabsAtXZ(blockWorldPosX, blockWorldPosX + 15, blockWorldPosZ, blockWorldPosZ + 15, value);
			value.Sort(new Comparison<PrefabInstance>(this.prefabInstanceSizeComparison));
		}
		for (int i = 0; i < value.Count; i++)
		{
			PrefabInstance prefabInstance = value[i];
			if (prefabInstance.Overlaps(_chunk))
			{
				prefabInstance.CopyIntoChunk(_world, _chunk, _bForceOverwriteBlocks, default(FastTags<TagGroup.Global>));
			}
		}
	}

	// Token: 0x060043CD RID: 17357 RVA: 0x001A6394 File Offset: 0x001A4594
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceSizeComparison(PrefabInstance _a, PrefabInstance _b)
	{
		int value = _a.boundingBoxSize.x * _a.boundingBoxSize.z;
		return (_b.boundingBoxSize.x * _b.boundingBoxSize.z).CompareTo(value);
	}

	// Token: 0x060043CE RID: 17358 RVA: 0x001A63DC File Offset: 0x001A45DC
	public bool IsEntityInPrefab(int _entityId)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				if (this.allPrefabs[i].Contains(_entityId))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060043CF RID: 17359 RVA: 0x001A6448 File Offset: 0x001A4648
	public bool OnSelectionBoxActivated(SelectionBox _box, bool _bActivated)
	{
		if (!_bActivated)
		{
			this.ActivePrefab = null;
			return true;
		}
		PrefabInstance prefabInstance = _box.UserData as PrefabInstance;
		if (prefabInstance != null)
		{
			this.ActivePrefab = prefabInstance;
			this.ActivePrefab.UpdateImposterView();
		}
		else
		{
			Log.Error("Selected prefab SelectionBox has no PrefabInstance assigned");
			StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_box.name, '.', 1, 0, -1);
			int num;
			if (separatorPositions.TotalFound >= 1 && StringParsers.TryParseSInt32(_box.name, out num, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer))
			{
				this.ActivePrefab = this.GetPrefab(num);
			}
		}
		return true;
	}

	// Token: 0x060043D0 RID: 17360 RVA: 0x001A64D6 File Offset: 0x001A46D6
	public void OnSelectionBoxMoved(SelectionBox _box, Vector3 _moveVector)
	{
		if (this.ActivePrefab != null)
		{
			this.ActivePrefab.MoveBoundingBox(new Vector3i(_moveVector));
			this.ActivePrefab.UpdateImposterView();
		}
	}

	// Token: 0x060043D1 RID: 17361 RVA: 0x001A64FC File Offset: 0x001A46FC
	public void OnSelectionBoxSized(SelectionBox _box, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		if (GameManager.Instance.IsEditMode() && !PrefabEditModeManager.Instance.IsActive())
		{
			return;
		}
		if (this.ActivePrefab != null)
		{
			this.ActivePrefab.ResizeBoundingBox(new Vector3i(_dEast + _dWest, _dTop + _dBottom, _dNorth + _dSouth));
			this.ActivePrefab.MoveBoundingBox(new Vector3i(-_dWest, -_dBottom, -_dSouth));
		}
	}

	// Token: 0x060043D2 RID: 17362 RVA: 0x000027FC File Offset: 0x000009FC
	public void OnSelectionBoxMirrored(Vector3i _axis)
	{
	}

	// Token: 0x060043D3 RID: 17363 RVA: 0x001A655F File Offset: 0x001A475F
	public bool OnSelectionBoxDelete(SelectionBox _box, bool _checkCanDeleteOnly)
	{
		PrefabInstance prefabInstance = _box.UserData as PrefabInstance;
		if (prefabInstance != null)
		{
			prefabInstance.DestroyImposterView();
		}
		return false;
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x001A6578 File Offset: 0x001A4778
	public bool OnSelectionBoxIsAvailable(EnumSelectionBoxAvailabilities _criteria)
	{
		if (_criteria == EnumSelectionBoxAvailabilities.CanResize)
		{
			return PrefabEditModeManager.Instance.IsActive();
		}
		return _criteria == EnumSelectionBoxAvailabilities.CanShowProperties;
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x001A6590 File Offset: 0x001A4790
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
		XUiC_EditorPanelSelector childByType = _windowManager.playerUI.xui.FindWindowGroupByName(XUiC_EditorPanelSelector.ID).GetChildByType<XUiC_EditorPanelSelector>();
		if (childByType == null)
		{
			return;
		}
		childByType.SetSelected("prefabList");
		_windowManager.SwitchVisible(XUiC_InGameMenuWindow.ID, true);
	}

	// Token: 0x060043D6 RID: 17366 RVA: 0x001A65D3 File Offset: 0x001A47D3
	public void OnSelectionBoxRotated(SelectionBox _box)
	{
		this.ActivePrefab.RotateAroundY();
		this.ActivePrefab.UpdateImposterView();
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x000027FC File Offset: 0x000009FC
	public void OnSelectionBoxUserDataChanged(SelectionBox _box)
	{
	}

	// Token: 0x060043D8 RID: 17368 RVA: 0x001A65EC File Offset: 0x001A47EC
	[PublicizedFrom(EAccessModifier.Private)]
	public int PrefabBinarySearch(int x)
	{
		object obj = this.listsLock;
		int result;
		lock (obj)
		{
			if (this.isSortNeeded)
			{
				this.SortPrefabs();
			}
			int num = x - 200;
			int i = 0;
			int num2 = this.allPrefabsSorted.Count;
			while (i < num2)
			{
				int num3 = (i + num2) / 2;
				if (this.allPrefabsSorted[num3].boundingBoxPosition.x < num)
				{
					i = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
			}
			result = i;
		}
		return result;
	}

	// Token: 0x060043D9 RID: 17369 RVA: 0x001A6684 File Offset: 0x001A4884
	public PrefabInstance GetPrefabAtPosition(Vector3 _position, FastTags<TagGroup.Poi>? _excludeTags = null, FastTags<TagGroup.Poi>? _requiredTags = null)
	{
		FastTags<TagGroup.Poi> value = _excludeTags.GetValueOrDefault();
		if (_excludeTags == null)
		{
			value = DynamicPrefabDecorator.streetTileTag;
			_excludeTags = new FastTags<TagGroup.Poi>?(value);
		}
		value = _requiredTags.GetValueOrDefault();
		if (_requiredTags == null)
		{
			value = FastTags<TagGroup.Poi>.none;
			_requiredTags = new FastTags<TagGroup.Poi>?(value);
		}
		bool flag = !_excludeTags.Value.IsEmpty;
		bool flag2 = !_requiredTags.Value.IsEmpty;
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			PrefabInstance prefabInstance = null;
			Vector3i vector3i = Vector3i.Floor(_position);
			int i = this.PrefabBinarySearch(vector3i.x);
			int count = this.allPrefabsSorted.Count;
			while (i < count)
			{
				PrefabInstance prefabInstance2 = this.allPrefabsSorted[i];
				int num = vector3i.x - prefabInstance2.boundingBoxPosition.x;
				if (num < 0)
				{
					break;
				}
				if (num < prefabInstance2.boundingBoxSize.x)
				{
					int num2 = vector3i.z - prefabInstance2.boundingBoxPosition.z;
					if (num2 >= 0 && num2 < prefabInstance2.boundingBoxSize.z)
					{
						int num3 = vector3i.y - prefabInstance2.boundingBoxPosition.y;
						if (num3 >= 0 && num3 < prefabInstance2.boundingBoxSize.y && (!flag || !prefabInstance2.prefab.Tags.Test_AnySet(_excludeTags.Value)) && (!flag2 || prefabInstance2.prefab.Tags.Test_AllSet(_requiredTags.Value)))
						{
							prefabInstance = prefabInstance2;
							for (i++; i < count; i++)
							{
								prefabInstance2 = this.allPrefabsSorted[i];
								num = vector3i.x - prefabInstance2.boundingBoxPosition.x;
								if (num < 0)
								{
									break;
								}
								if (num < prefabInstance2.boundingBoxSize.x)
								{
									num2 = vector3i.z - prefabInstance2.boundingBoxPosition.z;
									if (num2 >= 0 && num2 < prefabInstance2.boundingBoxSize.z)
									{
										num3 = vector3i.y - prefabInstance2.boundingBoxPosition.y;
										if (num3 >= 0 && num3 < prefabInstance2.boundingBoxSize.y && (!flag || !prefabInstance2.prefab.Tags.Test_AnySet(_excludeTags.Value)) && (!flag2 || prefabInstance2.prefab.Tags.Test_AllSet(_requiredTags.Value)))
										{
											if (prefabInstance.boundingBoxPosition.x != prefabInstance2.boundingBoxPosition.x || prefabInstance.boundingBoxSize.x >= prefabInstance2.boundingBoxPosition.x)
											{
												prefabInstance = prefabInstance2;
												break;
											}
											break;
										}
									}
								}
							}
							break;
						}
					}
				}
				i++;
			}
			result = prefabInstance;
		}
		return result;
	}

	// Token: 0x060043DA RID: 17370 RVA: 0x001A6994 File Offset: 0x001A4B94
	public void GetPrefabsAtXZ(int _xMin, int _xMax, int _zMin, int _zMax, List<PrefabInstance> _list)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			_list.Clear();
			int num = this.PrefabBinarySearch(_xMin);
			int count = this.allPrefabsSorted.Count;
			for (int i = num; i < count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabsSorted[i];
				if (prefabInstance.boundingBoxPosition.x > _xMax)
				{
					break;
				}
				if (prefabInstance.boundingBoxPosition.x + prefabInstance.boundingBoxSize.x > _xMin && prefabInstance.boundingBoxPosition.z <= _zMax && prefabInstance.boundingBoxPosition.z + prefabInstance.boundingBoxSize.z > _zMin)
				{
					_list.Add(prefabInstance);
				}
			}
		}
	}

	// Token: 0x060043DB RID: 17371 RVA: 0x001A6A64 File Offset: 0x001A4C64
	public bool HasPrefabsAtXZ(int _xMin, int _xMax, int _zMin, int _zMax)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = this.PrefabBinarySearch(_xMin); i < this.allPrefabsSorted.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabsSorted[i];
				if (prefabInstance.boundingBoxPosition.x > _xMax)
				{
					break;
				}
				if (prefabInstance.boundingBoxPosition.x + prefabInstance.boundingBoxSize.x > _xMin && prefabInstance.boundingBoxPosition.z <= _zMax && prefabInstance.boundingBoxPosition.z + prefabInstance.boundingBoxSize.z > _zMin)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060043DC RID: 17372 RVA: 0x001A6B24 File Offset: 0x001A4D24
	public virtual void GetPrefabsAround(Vector3 _position, float _nearDistance, float _farDistance, Dictionary<int, PrefabInstance> _prefabsFar, Dictionary<int, PrefabInstance> _prefabsNear)
	{
		object obj = this.listsLock;
		lock (obj)
		{
			Vector2 vector = new Vector2(_position.x, _position.z);
			float num = _farDistance * _farDistance;
			for (int i = this.PrefabBinarySearch((int)(vector.x - _farDistance)); i < this.allPrefabsSorted.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabsSorted[i];
				if (_position.x - (float)prefabInstance.boundingBoxPosition.x < -_farDistance)
				{
					break;
				}
				float num2 = _position.x - ((float)prefabInstance.boundingBoxPosition.x + (float)prefabInstance.boundingBoxSize.x * 0.5f);
				float num3 = _position.z - ((float)prefabInstance.boundingBoxPosition.z + (float)prefabInstance.boundingBoxSize.z * 0.5f);
				if (num2 * num2 + num3 * num3 <= num)
				{
					Vector2 vector2;
					vector2.x = (float)prefabInstance.boundingBoxPosition.x;
					vector2.y = (float)prefabInstance.boundingBoxPosition.z;
					Vector2 vector3;
					vector3.x = vector2.x + (float)prefabInstance.boundingBoxSize.x;
					vector3.y = vector2.y;
					Vector2 vector4;
					vector4.x = vector2.x;
					vector4.y = vector2.y + (float)prefabInstance.boundingBoxSize.z;
					Vector2 a;
					a.x = vector3.x;
					a.y = vector4.y;
					if (!DynamicMeshManager.IsOutsideDistantTerrain(vector2.x, vector3.x, vector2.y, vector4.y))
					{
						Vector2 vector5 = vector2 - vector;
						if (Utils.FastMax(Utils.FastAbs(vector5.x), Utils.FastAbs(vector5.y)) < _nearDistance)
						{
							Vector2 vector6 = vector3 - vector;
							if (Utils.FastMax(Utils.FastAbs(vector6.x), Utils.FastAbs(vector6.y)) < _nearDistance)
							{
								Vector2 vector7 = vector4 - vector;
								if (Utils.FastMax(Utils.FastAbs(vector7.x), Utils.FastAbs(vector7.y)) < _nearDistance)
								{
									Vector2 vector8 = a - vector;
									if (Utils.FastMax(Utils.FastAbs(vector8.x), Utils.FastAbs(vector8.y)) < _nearDistance)
									{
										_prefabsNear.Add(prefabInstance.id, prefabInstance);
										goto IL_2D7;
									}
								}
							}
						}
						string text = (prefabInstance.prefab.distantPOIOverride == null) ? prefabInstance.prefab.PrefabName : prefabInstance.prefab.distantPOIOverride;
						bool flag2;
						if (!this.prefabMeshExisting.TryGetValue(text, out flag2))
						{
							flag2 = (PathAbstractions.PrefabImpostersSearchPaths.GetLocation(text, null, null).Type != PathAbstractions.EAbstractedLocationType.None);
							this.prefabMeshExisting[text] = flag2;
						}
						if (flag2)
						{
							_prefabsFar.Add(prefabInstance.id, prefabInstance);
						}
					}
				}
				IL_2D7:;
			}
		}
	}

	// Token: 0x060043DD RID: 17373 RVA: 0x001A6E48 File Offset: 0x001A5048
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ValidPrefabForQuest(EntityTrader trader, PrefabInstance prefab, FastTags<TagGroup.Global> questTag, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1, BiomeFilterTypes biomeFilterType = BiomeFilterTypes.SameBiome, string biomeFilter = "")
	{
		if (!prefab.prefab.SleeperVolumeList.AnyUsedEntry || !prefab.prefab.GetQuestTag(questTag))
		{
			return false;
		}
		Vector2 vector = new Vector2((float)prefab.boundingBoxPosition.x, (float)prefab.boundingBoxPosition.z);
		if (usedPOILocations != null && usedPOILocations.Contains(vector))
		{
			return false;
		}
		ulong num;
		if (QuestEventManager.Current.CheckForPOILockouts(entityIDforQuests, vector, out num) != QuestEventManager.POILockoutReasonTypes.None)
		{
			return false;
		}
		new Vector2((float)prefab.boundingBoxPosition.x + (float)prefab.boundingBoxSize.x / 2f, (float)prefab.boundingBoxPosition.z + (float)prefab.boundingBoxSize.z / 2f);
		if (biomeFilterType != BiomeFilterTypes.AnyBiome)
		{
			string[] array = null;
			BiomeDefinition biomeAt = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider().GetBiomeAt((int)vector.x, (int)vector.y);
			if (biomeFilterType == BiomeFilterTypes.OnlyBiome)
			{
				if (biomeAt.m_sBiomeName != biomeFilter)
				{
					return false;
				}
			}
			else if (biomeFilterType == BiomeFilterTypes.ExcludeBiome)
			{
				if (array == null)
				{
					array = biomeFilter.Split(',', StringSplitOptions.None);
				}
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (biomeAt.m_sBiomeName == array[i])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return false;
				}
			}
			else if (biomeFilterType == BiomeFilterTypes.SameBiome && trader != null)
			{
				BiomeDefinition biomeAt2 = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider().GetBiomeAt((int)trader.position.x, (int)trader.position.z);
				if (biomeAt != biomeAt2)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060043DE RID: 17374 RVA: 0x001A6FDC File Offset: 0x001A51DC
	public static PrefabInstance GetRandomPOINearTrader(EntityTrader trader, FastTags<TagGroup.Global> questTag, byte difficulty, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1, BiomeFilterTypes biomeFilterType = BiomeFilterTypes.SameBiome, string biomeFilter = "")
	{
		QuestEventManager questEventManager = QuestEventManager.Current;
		GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
		int num = trader.PreferredDistanceIndex;
		for (int i = 0; i < 3; i++)
		{
			num %= 3;
			List<PrefabInstance> prefabsForTrader = questEventManager.GetPrefabsForTrader(trader.traderArea, (int)difficulty, num, gameRandom);
			if (prefabsForTrader != null)
			{
				for (int j = 0; j < prefabsForTrader.Count; j++)
				{
					PrefabInstance prefabInstance = prefabsForTrader[j];
					if (DynamicPrefabDecorator.ValidPrefabForQuest(trader, prefabInstance, questTag, usedPOILocations, entityIDforQuests, biomeFilterType, biomeFilter))
					{
						return prefabInstance;
					}
				}
			}
			num++;
		}
		return null;
	}

	// Token: 0x060043DF RID: 17375 RVA: 0x001A7068 File Offset: 0x001A5268
	public static PrefabInstance GetRandomPOINearWorldPos(Vector2 worldPos, int minSearchDistance, int maxSearchDistance, FastTags<TagGroup.Global> questTag, byte difficulty, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1, BiomeFilterTypes biomeFilterType = BiomeFilterTypes.SameBiome, string biomeFilter = "")
	{
		List<PrefabInstance> prefabsByDifficultyTier = QuestEventManager.Current.GetPrefabsByDifficultyTier((int)difficulty);
		if (prefabsByDifficultyTier == null)
		{
			return null;
		}
		string[] array = null;
		BiomeDefinition biomeAt = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider().GetBiomeAt((int)worldPos.x, (int)worldPos.y);
		World world = GameManager.Instance.World;
		for (int i = 0; i < 50; i++)
		{
			int index = world.GetGameRandom().RandomRange(prefabsByDifficultyTier.Count);
			PrefabInstance prefabInstance = prefabsByDifficultyTier[index];
			if (prefabInstance.prefab.SleeperVolumeList.AnyUsedEntry && prefabInstance.prefab.GetQuestTag(questTag) && prefabInstance.prefab.DifficultyTier == difficulty)
			{
				Vector2 vector = new Vector2((float)prefabInstance.boundingBoxPosition.x, (float)prefabInstance.boundingBoxPosition.z);
				ulong num;
				if ((usedPOILocations == null || !usedPOILocations.Contains(vector)) && QuestEventManager.Current.CheckForPOILockouts(entityIDforQuests, vector, out num) == QuestEventManager.POILockoutReasonTypes.None)
				{
					Vector2 b = new Vector2((float)prefabInstance.boundingBoxPosition.x + (float)prefabInstance.boundingBoxSize.x / 2f, (float)prefabInstance.boundingBoxPosition.z + (float)prefabInstance.boundingBoxSize.z / 2f);
					if (biomeFilterType != BiomeFilterTypes.AnyBiome)
					{
						BiomeDefinition biomeAt2 = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider().GetBiomeAt((int)vector.x, (int)vector.y);
						if (biomeFilterType == BiomeFilterTypes.OnlyBiome)
						{
							if (biomeAt2.m_sBiomeName != biomeFilter)
							{
								goto IL_1FC;
							}
						}
						else if (biomeFilterType == BiomeFilterTypes.ExcludeBiome)
						{
							if (array == null)
							{
								array = biomeFilter.Split(',', StringSplitOptions.None);
							}
							bool flag = false;
							for (int j = 0; j < array.Length; j++)
							{
								if (biomeAt2.m_sBiomeName == array[j])
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								goto IL_1FC;
							}
						}
						else if (biomeFilterType == BiomeFilterTypes.SameBiome && biomeAt2 != biomeAt)
						{
							goto IL_1FC;
						}
					}
					float sqrMagnitude = (worldPos - b).sqrMagnitude;
					if (sqrMagnitude < (float)maxSearchDistance && sqrMagnitude > (float)minSearchDistance)
					{
						return prefabInstance;
					}
				}
			}
			IL_1FC:;
		}
		return null;
	}

	// Token: 0x060043E0 RID: 17376 RVA: 0x001A7284 File Offset: 0x001A5484
	public virtual PrefabInstance GetClosestPOIToWorldPos(FastTags<TagGroup.Global> questTag, Vector2 worldPos, List<Vector2> excludeList = null, int maxSearchDistanceSquared = -1, bool ignoreCurrentPOI = false, BiomeFilterTypes biomeFilterType = BiomeFilterTypes.SameBiome, string biomeFilter = "", string questKey = "")
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			List<Tuple<PrefabInstance, Vector2>> list = new List<Tuple<PrefabInstance, Vector2>>();
			string[] array = null;
			Vector3 pos = new Vector3(worldPos.x, 0f, worldPos.y);
			IBiomeProvider biomeProvider = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider();
			BiomeDefinition biomeDefinition = (biomeProvider != null) ? biomeProvider.GetBiomeAt((int)worldPos.x, (int)worldPos.y) : null;
			for (int i = 0; i < this.poiPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.poiPrefabs[i];
				if (!prefabInstance.prefab.PrefabName.Contains("rwg_tile") && (prefabInstance.prefab.GetQuestTag(questTag) || questTag.IsEmpty))
				{
					if (ignoreCurrentPOI)
					{
						pos.y = (float)prefabInstance.boundingBoxPosition.y;
						if (prefabInstance.Overlaps(pos, 0f))
						{
							goto IL_1FC;
						}
					}
					Vector2 vector = new Vector2((float)prefabInstance.boundingBoxPosition.x + (float)prefabInstance.boundingBoxSize.x / 2f, (float)prefabInstance.boundingBoxPosition.z + (float)prefabInstance.boundingBoxSize.z / 2f);
					if (excludeList == null || !excludeList.Contains(new Vector2((float)prefabInstance.boundingBoxPosition.x, (float)prefabInstance.boundingBoxPosition.z)))
					{
						if (biomeFilterType != BiomeFilterTypes.AnyBiome)
						{
							BiomeDefinition biomeDefinition2 = (biomeProvider != null) ? biomeProvider.GetBiomeAt((int)vector.x, (int)vector.y) : null;
							if (biomeFilterType == BiomeFilterTypes.OnlyBiome)
							{
								if (biomeDefinition2.m_sBiomeName != biomeFilter)
								{
									goto IL_1FC;
								}
							}
							else if (biomeFilterType == BiomeFilterTypes.ExcludeBiome)
							{
								if (array == null)
								{
									array = biomeFilter.Split(',', StringSplitOptions.None);
								}
								bool flag2 = false;
								for (int j = 0; j < array.Length; j++)
								{
									if (biomeDefinition2.m_sBiomeName == array[j])
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									goto IL_1FC;
								}
							}
							else if (biomeFilterType == BiomeFilterTypes.SameBiome && biomeDefinition2 != biomeDefinition)
							{
								goto IL_1FC;
							}
						}
						list.Add(new Tuple<PrefabInstance, Vector2>(prefabInstance, vector));
					}
				}
				IL_1FC:;
			}
			float maxSearchDistanceSquared2 = (maxSearchDistanceSquared < 0) ? float.MaxValue : ((float)maxSearchDistanceSquared);
			PrefabInstance prefabInstance2;
			if (string.Compare(questKey, "traderquest", true) == 0)
			{
				prefabInstance2 = this.chooseBestTrader(list, worldPos, maxSearchDistanceSquared2);
			}
			else
			{
				prefabInstance2 = this.chooseClosestPrefab(list, worldPos, maxSearchDistanceSquared2);
			}
			result = prefabInstance2;
		}
		return result;
	}

	// Token: 0x060043E1 RID: 17377 RVA: 0x001A750C File Offset: 0x001A570C
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance chooseClosestPrefab(List<Tuple<PrefabInstance, Vector2>> prefabCandidates, Vector2 worldPos, float maxSearchDistanceSquared)
	{
		PrefabInstance result = null;
		foreach (Tuple<PrefabInstance, Vector2> tuple in prefabCandidates)
		{
			float sqrMagnitude = (worldPos - tuple.Item2).sqrMagnitude;
			if (sqrMagnitude < maxSearchDistanceSquared)
			{
				maxSearchDistanceSquared = sqrMagnitude;
				result = tuple.Item1;
			}
		}
		return result;
	}

	// Token: 0x060043E2 RID: 17378 RVA: 0x001A757C File Offset: 0x001A577C
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance chooseBestTrader(List<Tuple<PrefabInstance, Vector2>> prefabCandidates, Vector2 worldPos, float maxSearchDistanceSquared)
	{
		PrefabInstance result = null;
		int num = 0;
		foreach (Tuple<PrefabInstance, Vector2> tuple in prefabCandidates)
		{
			if ((worldPos - tuple.Item2).sqrMagnitude <= maxSearchDistanceSquared)
			{
				TraderArea traderAtPosition = this.GetTraderAtPosition(new Vector3i(tuple.Item2.x, 0f, tuple.Item2.y), 0);
				if (traderAtPosition != null)
				{
					int traderPoiCount = QuestEventManager.Current.GetTraderPoiCount(traderAtPosition, 1, 0);
					if (num < traderPoiCount)
					{
						num = traderPoiCount;
						result = tuple.Item1;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060043E3 RID: 17379 RVA: 0x001A762C File Offset: 0x001A582C
	public virtual PrefabInstance GetPrefabFromWorldPos(int x, int z)
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				if (this.allPrefabs[i].boundingBoxPosition.x == x && this.allPrefabs[i].boundingBoxPosition.z == z && !this.allPrefabs[i].prefab.PrefabName.Contains("rwg_tile") && !this.allPrefabs[i].prefab.PrefabName.Contains("part_"))
				{
					return this.allPrefabs[i];
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060043E4 RID: 17380 RVA: 0x001A770C File Offset: 0x001A590C
	public virtual PrefabInstance GetPrefabFromWorldPosInside(int _x, int _z)
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				int x = prefabInstance.boundingBoxPosition.x;
				int z = prefabInstance.boundingBoxPosition.z;
				if (x <= _x && z <= _z && x + prefabInstance.boundingBoxSize.x >= _x && z + prefabInstance.boundingBoxSize.z >= _z)
				{
					return this.allPrefabs[i];
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060043E5 RID: 17381 RVA: 0x001A77C4 File Offset: 0x001A59C4
	public virtual PrefabInstance GetPrefabFromWorldPosInsideWithOffset(int _x, int _z, int _offset)
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				int num = prefabInstance.boundingBoxPosition.x - _offset;
				int num2 = prefabInstance.boundingBoxPosition.z - _offset;
				int num3 = prefabInstance.boundingBoxPosition.x + prefabInstance.boundingBoxSize.x + _offset;
				int num4 = prefabInstance.boundingBoxPosition.z + prefabInstance.boundingBoxSize.z + _offset;
				if (num <= _x && num2 <= _z && num3 >= _x && num4 >= _z)
				{
					return this.allPrefabs[i];
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060043E6 RID: 17382 RVA: 0x001A78A0 File Offset: 0x001A5AA0
	public virtual PrefabInstance GetPrefabFromWorldPosInside(int _x, int _y, int _z)
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				int x = prefabInstance.boundingBoxPosition.x;
				int y = prefabInstance.boundingBoxPosition.y;
				int z = prefabInstance.boundingBoxPosition.z;
				if (x <= _x && y <= _y && z <= _z && x + prefabInstance.boundingBoxSize.x >= _x && y + prefabInstance.boundingBoxSize.y >= _y && z + prefabInstance.boundingBoxSize.z >= _z)
				{
					return this.allPrefabs[i];
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x001A7984 File Offset: 0x001A5B84
	public virtual List<PrefabInstance> GetPrefabsFromWorldPosInside(Vector3 _pos, FastTags<TagGroup.Global> _questTags)
	{
		_pos += this.boundsPad;
		List<PrefabInstance> list = new List<PrefabInstance>();
		Bounds bounds = default(Bounds);
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				if (prefabInstance.prefab.GetQuestTag(_questTags))
				{
					bounds.SetMinMax(prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxPosition + prefabInstance.boundingBoxSize - this.boundsPad);
					if (bounds.Contains(_pos))
					{
						list.AddRange(this.GetPrefabsIntersecting(prefabInstance));
					}
				}
			}
		}
		list = (from pi in list
		orderby pi.boundingBoxSize.x * pi.boundingBoxSize.z descending
		select pi).ToList<PrefabInstance>();
		return list;
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x001A7A90 File Offset: 0x001A5C90
	public virtual List<PrefabInstance> GetPrefabsIntersecting(PrefabInstance parentPI)
	{
		List<PrefabInstance> list = new List<PrefabInstance>();
		list.Add(parentPI);
		Bounds bounds = default(Bounds);
		bounds.SetMinMax(parentPI.boundingBoxPosition, parentPI.boundingBoxPosition + parentPI.boundingBoxSize - this.boundsPad);
		float num = bounds.size.x * bounds.size.z;
		Bounds bounds2 = default(Bounds);
		object obj = this.listsLock;
		lock (obj)
		{
			for (int i = 0; i < this.allPrefabs.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabs[i];
				if (parentPI != prefabInstance)
				{
					bounds2.SetMinMax(prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxPosition + prefabInstance.boundingBoxSize - this.boundsPad);
					if (bounds.Intersects(bounds2) && bounds2.size.x * bounds2.size.z < num && !list.Contains(prefabInstance))
					{
						list.Add(prefabInstance);
					}
				}
			}
		}
		list = (from pi in list
		orderby pi.boundingBoxSize.x * pi.boundingBoxSize.z descending
		select pi).ToList<PrefabInstance>();
		return list;
	}

	// Token: 0x060043E9 RID: 17385 RVA: 0x001A7C08 File Offset: 0x001A5E08
	public PrefabInstance FindVolumeOwner(PrefabVolumeAbs.EVolumeType _volumeType, Vector3i _worldMin, Vector3i _worldMax)
	{
		object obj = this.listsLock;
		PrefabInstance result;
		lock (obj)
		{
			for (int i = this.PrefabBinarySearch(_worldMin.x); i < this.allPrefabsSorted.Count; i++)
			{
				PrefabInstance prefabInstance = this.allPrefabsSorted[i];
				if (prefabInstance.boundingBoxPosition.x > _worldMax.x)
				{
					break;
				}
				if (prefabInstance.boundingBoxPosition.x + prefabInstance.boundingBoxSize.x > _worldMin.x && prefabInstance.boundingBoxPosition.z <= _worldMax.z && prefabInstance.boundingBoxPosition.z + prefabInstance.boundingBoxSize.z > _worldMin.z && prefabInstance.boundingBoxPosition.y <= _worldMax.y && prefabInstance.boundingBoxPosition.y + prefabInstance.boundingBoxSize.y > _worldMin.y)
				{
					PrefabVolumeListAbs prefabVolumeListAbs = prefabInstance.prefab.AllVolumeListsByType[_volumeType];
					for (int j = 0; j < prefabVolumeListAbs.Count; j++)
					{
						PrefabVolumeAbs prefabVolumeAbs = prefabVolumeListAbs.Get(j);
						Vector3i one = prefabVolumeAbs.startPos + prefabInstance.boundingBoxPosition;
						Vector3i vector3i = one + prefabVolumeAbs.size;
						if (one.Equals(_worldMin) && vector3i.Equals(_worldMax))
						{
							return prefabInstance;
						}
					}
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x060043EA RID: 17386 RVA: 0x001A7DA0 File Offset: 0x001A5FA0
	public IEnumerator CopyWorldPrefabHeightsIntoHeightMap(int _heightMapWidth, int _heightMapHeight, IBackedArray<ushort> _heightData, int _heightMapScale = 1, ushort[] _topTextures = null)
	{
		MicroStopwatch yieldMs = new MicroStopwatch(true);
		if (this.blockValueTerrainFiller.isair)
		{
			this.blockValueTerrainFiller = Block.GetBlockValue(Constants.cTerrainFillerBlockName, false);
			this.blockValueTerrainFiller2 = Block.GetBlockValue(Constants.cTerrainFiller2BlockName, false);
		}
		int num;
		for (int i = 0; i < this.worldPrefabs.Count; i = num + 1)
		{
			PrefabInstance prefabInstance = this.worldPrefabs[i];
			if (prefabInstance.prefab != null)
			{
				this.copyPrefabsIntoHeightMap(prefabInstance, _heightMapWidth, _heightMapHeight, _heightData, _heightMapScale, _topTextures);
				if (yieldMs.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					yieldMs.ResetAndRestart();
				}
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x001A7DD4 File Offset: 0x001A5FD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void copyPrefabsIntoHeightMap(PrefabInstance _pi, int _heightMapWidth, int _heightMapHeight, IBackedArray<ushort> _heightData, int _heightMapScale, ushort[] _topTextures = null)
	{
		using (IBackedArrayView<ushort> backedArrayView = BackedArrays.CreateSingleView<ushort>(_heightData, BackedArrayHandleMode.ReadWrite, 0))
		{
			int rotation = (int)_pi.rotation;
			Prefab prefab = _pi.prefab;
			int yOffset = prefab.yOffset;
			Vector3i size = prefab.size;
			int x = _pi.boundingBoxPosition.x;
			int y = _pi.boundingBoxPosition.y;
			int z = _pi.boundingBoxPosition.z;
			IChunkProvider chunkProvider = GameManager.Instance.World.ChunkCache.ChunkProvider;
			bool flag = chunkProvider != null && chunkProvider.WorldInfo.RandomGeneratedWorld;
			if (_pi.boundingBoxPosition.x < -_heightMapWidth / 2 || _pi.boundingBoxPosition.x + size.x > _heightMapWidth / 2 || _pi.boundingBoxPosition.z < -_heightMapHeight / 2 || _pi.boundingBoxPosition.z + size.z > _heightMapHeight / 2)
			{
				Log.Warning(string.Format("Prefab {0} outside of the world bounds (position {1})", _pi.name, _pi.boundingBoxPosition));
			}
			for (int i = (size.z + _heightMapScale - 1) % _heightMapScale; i < size.z; i += _heightMapScale)
			{
				int num = i + z;
				int num2 = (num / _heightMapScale + _heightMapHeight / 2) * _heightMapWidth;
				int num3 = (num + _heightMapHeight / 2) * _heightMapScale * _heightMapWidth;
				for (int j = (size.x + _heightMapScale - 1) % _heightMapScale; j < size.x; j += _heightMapScale)
				{
					int num4 = j + x;
					int num5 = (num4 / _heightMapScale + _heightMapWidth / 2 + num2) % _heightData.Length;
					int num6 = (num4 + _heightMapWidth / 2) * _heightMapScale + num3;
					for (int k = 0; k < size.y; k++)
					{
						BlockValue blockNoDamage = prefab.GetBlockNoDamage(rotation, j, k, i);
						WaterValue water = prefab.GetWater(j, k, i);
						Block block = blockNoDamage.Block;
						if (blockNoDamage.isair || block == null || !block.shape.IsTerrain() || water.HasMass())
						{
							if (k > -yOffset)
							{
								break;
							}
						}
						else
						{
							sbyte density = prefab.GetDensity(rotation, j, k, i);
							float num7 = (float)(y + k);
							num7 += (float)(-(float)density) / 128f - 1f;
							if (num7 > 0f)
							{
								ushort num8 = (ushort)(num7 / 0.0038910506f);
								if (blockNoDamage.type != this.blockValueTerrainFiller2.type || num8 <= backedArrayView[num5])
								{
									if (num5 >= 0 && num5 < _heightData.Length && (flag || num8 > backedArrayView[num5]))
									{
										backedArrayView[num5] = num8;
									}
									if (block != null && _topTextures != null && !blockNoDamage.isair && blockNoDamage.type != this.blockValueTerrainFiller.type && blockNoDamage.type != this.blockValueTerrainFiller2.type)
									{
										int sideTextureId = block.GetSideTextureId(blockNoDamage, BlockFace.Top, 0);
										if (num6 >= 0 && num6 < _topTextures.Length)
										{
											_topTextures[num6] = (ushort)sideTextureId;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x001A80EC File Offset: 0x001A62EC
	public void CalculateStats(out int basePrefabCount, out int rotatedPrefabsCount, out int activePrefabCount, out int basePrefabBytes, out int rotatedPrefabBytes, out int activePrefabBytes)
	{
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		this.prefabCache.CalculateStats(out basePrefabCount, out rotatedPrefabsCount, out basePrefabBytes, out rotatedPrefabBytes);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			activePrefabCount = -1;
			activePrefabBytes = -1;
			return;
		}
		List<EntityPlayer> list = GameManager.Instance.World.Players.list;
		HashSet<Prefab> hashSet = new HashSet<Prefab>();
		List<PrefabInstance> list2 = new List<PrefabInstance>();
		foreach (EntityPlayer entityPlayer in list)
		{
			foreach (long key in entityPlayer.ChunkObserver.chunksAround.list)
			{
				IChunk chunkSync = chunkCache.GetChunkSync(key);
				if (chunkSync != null)
				{
					Vector3i worldPos = chunkSync.GetWorldPos();
					this.GetPrefabsAtXZ(worldPos.x, worldPos.x + 15, worldPos.z, worldPos.z + 15, list2);
					foreach (PrefabInstance prefabInstance in list2)
					{
						hashSet.Add(prefabInstance.prefab);
					}
				}
			}
		}
		activePrefabCount = hashSet.Count;
		activePrefabBytes = 0;
		foreach (Prefab prefab in hashSet)
		{
			activePrefabBytes += prefab.EstimateOwnedBytes();
		}
	}

	// Token: 0x040036A7 RID: 13991
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPrefabMaxRadius = 200;

	// Token: 0x040036A8 RID: 13992
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabCache prefabCache;

	// Token: 0x040036A9 RID: 13993
	[PublicizedFrom(EAccessModifier.Private)]
	public object listsLock = new object();

	// Token: 0x040036AA RID: 13994
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> allPrefabs = new List<PrefabInstance>();

	// Token: 0x040036AB RID: 13995
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> poiPrefabs = new List<PrefabInstance>();

	// Token: 0x040036AC RID: 13996
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> worldPrefabs = new List<PrefabInstance>();

	// Token: 0x040036AD RID: 13997
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PrefabInstance> allPrefabsSorted = new List<PrefabInstance>();

	// Token: 0x040036AE RID: 13998
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSortNeeded = true;

	// Token: 0x040036AF RID: 13999
	[PublicizedFrom(EAccessModifier.Private)]
	public DesignatedAreaStore<TraderArea> traderStore = new DesignatedAreaStore<TraderArea>();

	// Token: 0x040036B0 RID: 14000
	[PublicizedFrom(EAccessModifier.Private)]
	public DesignatedAreaStore<DecoSuppressArea> decoSuppressStore = new DesignatedAreaStore<DecoSuppressArea>();

	// Token: 0x040036B1 RID: 14001
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;

	// Token: 0x040036B2 RID: 14002
	public PrefabInstance ActivePrefab;

	// Token: 0x040036B3 RID: 14003
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, bool> prefabMeshExisting = new Dictionary<string, bool>();

	// Token: 0x040036B7 RID: 14007
	public static readonly FastTags<TagGroup.Poi> streetTileTag = FastTags<TagGroup.Poi>.Parse("streettile");

	// Token: 0x040036B8 RID: 14008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool poiMetadataRequestPending;

	// Token: 0x040036B9 RID: 14009
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadLocal<List<PrefabInstance>> decorateChunkPIs = new ThreadLocal<List<PrefabInstance>>(() => new List<PrefabInstance>());

	// Token: 0x040036BA RID: 14010
	public static int PrefabPreviewLimit = 0;

	// Token: 0x040036BB RID: 14011
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Vector3 boundsPad = new Vector3(0.001f, 0.001f, 0.001f);

	// Token: 0x040036BC RID: 14012
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValueTerrainFiller;

	// Token: 0x040036BD RID: 14013
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValueTerrainFiller2;
}
