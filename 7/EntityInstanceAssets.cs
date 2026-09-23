using System;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class EntityInstanceAssets
{
	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x06002643 RID: 9795 RVA: 0x000EA9CD File Offset: 0x000E8BCD
	// (set) Token: 0x06002644 RID: 9796 RVA: 0x000EA9D5 File Offset: 0x000E8BD5
	public Transform PrefabT { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002645 RID: 9797 RVA: 0x000EA9DE File Offset: 0x000E8BDE
	public bool IsLoadComplete
	{
		get
		{
			return this.prefabHandle.IsDone;
		}
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002646 RID: 9798 RVA: 0x000EA9EB File Offset: 0x000E8BEB
	public bool IsLoadSuccessful
	{
		get
		{
			return this.IsLoadComplete && !(this.PrefabT == null);
		}
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000EAA08 File Offset: 0x000E8C08
	public void Load(bool _loadSync, EntityClass _ec, bool isLocalPlayer)
	{
		if (isLocalPlayer)
		{
			this.prefabHandle = LoadManager.LoadAsset<GameObject>("Prefabs/prefabEntityPlayerLocal", null, null, false, true, false);
			this.PrefabT = this.prefabHandle.Asset.transform;
			return;
		}
		this.prefabHandle = LoadManager.LoadAsset<GameObject>(_ec.prefabPath, new Action<GameObject>(this.OnPrefabLoaded), null, false, _loadSync, false);
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000EAA65 File Offset: 0x000E8C65
	public void OnPrefabLoaded(GameObject prefab)
	{
		this.PrefabT = prefab.transform;
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000EAA73 File Offset: 0x000E8C73
	public void WaitForComplete()
	{
		if (!this.prefabHandle.IsDone)
		{
			this.prefabHandle.WaitForComplete();
		}
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000EAA8D File Offset: 0x000E8C8D
	public void Release()
	{
		LoadManager.AssetRequestTask<GameObject> assetRequestTask = this.prefabHandle;
		if (assetRequestTask == null)
		{
			return;
		}
		assetRequestTask.Release();
	}

	// Token: 0x04001C77 RID: 7287
	[PublicizedFrom(EAccessModifier.Private)]
	public LoadManager.AssetRequestTask<GameObject> prefabHandle;
}
