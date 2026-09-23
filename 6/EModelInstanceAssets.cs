using System;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class EModelInstanceAssets
{
	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06001E85 RID: 7813 RVA: 0x000B970D File Offset: 0x000B790D
	// (set) Token: 0x06001E86 RID: 7814 RVA: 0x000B9715 File Offset: 0x000B7915
	public Transform Mesh { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06001E87 RID: 7815 RVA: 0x000B971E File Offset: 0x000B791E
	// (set) Token: 0x06001E88 RID: 7816 RVA: 0x000B9726 File Offset: 0x000B7926
	public Material AltMaterial { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06001E89 RID: 7817 RVA: 0x000B972F File Offset: 0x000B792F
	public bool IsLoadComplete
	{
		get
		{
			LoadManager.AssetRequestTask<GameObject> assetRequestTask = this.meshHandle;
			if (assetRequestTask == null || assetRequestTask.IsDone)
			{
				LoadManager.AssetRequestTask<Material> assetRequestTask2 = this.altMaterialHandle;
				return assetRequestTask2 == null || assetRequestTask2.IsDone;
			}
			return false;
		}
	}

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06001E8A RID: 7818 RVA: 0x000B9758 File Offset: 0x000B7958
	public bool IsLoadSuccessful
	{
		get
		{
			return this.IsLoadComplete && (this.meshHandle == null || !(this.Mesh == null));
		}
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x000B9780 File Offset: 0x000B7980
	public void Load(bool _loadSync, EntityCreationData _ecd, EntityClass _ec)
	{
		if (!string.IsNullOrEmpty(_ec.meshPath))
		{
			this.meshHandle = LoadManager.LoadAsset<GameObject>(_ec.meshPath, delegate(GameObject mesh)
			{
				this.OnMeshLoaded(mesh, _ec.meshPath, _ec.entityClassName);
			}, null, false, _loadSync, false);
		}
		if (_ec.AltMatNames != null)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(_ecd.id);
			int num = gameRandom.RandomRange(_ec.AltMatNames.Length + 1) - 1;
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			if (num >= 0)
			{
				string assetPath = _ec.AltMatNames[num];
				this.altMaterialHandle = LoadManager.LoadAsset<Material>(assetPath, delegate(Material mat)
				{
					this.OnAltMaterialLoaded(mat, assetPath, _ec.entityClassName);
				}, null, false, _loadSync, false);
			}
		}
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000B9864 File Offset: 0x000B7A64
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnMeshLoaded(GameObject mesh, string assetPath, string entityClassName)
	{
		if (mesh == null)
		{
			Log.Error(string.Concat(new string[]
			{
				"Could not load mesh '",
				assetPath,
				"' for entity_class '",
				entityClassName,
				"'"
			}));
			return;
		}
		this.Mesh = mesh.transform;
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x000B98B8 File Offset: 0x000B7AB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnAltMaterialLoaded(Material material, string assetPath, string entityClassName)
	{
		if (material == null)
		{
			Log.Error(string.Concat(new string[]
			{
				"Could not load alt material '",
				assetPath,
				"' for entity_class '",
				entityClassName,
				"'"
			}));
			return;
		}
		this.AltMaterial = material;
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000B9908 File Offset: 0x000B7B08
	public void WaitForComplete()
	{
		if (this.meshHandle != null && !this.meshHandle.IsDone)
		{
			this.meshHandle.WaitForComplete();
		}
		if (this.altMaterialHandle != null && !this.altMaterialHandle.IsDone)
		{
			this.altMaterialHandle.WaitForComplete();
		}
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000B9955 File Offset: 0x000B7B55
	public void Release()
	{
		LoadManager.AssetRequestTask<GameObject> assetRequestTask = this.meshHandle;
		if (assetRequestTask != null)
		{
			assetRequestTask.Release();
		}
		LoadManager.AssetRequestTask<Material> assetRequestTask2 = this.altMaterialHandle;
		if (assetRequestTask2 == null)
		{
			return;
		}
		assetRequestTask2.Release();
	}

	// Token: 0x04001474 RID: 5236
	[PublicizedFrom(EAccessModifier.Private)]
	public LoadManager.AssetRequestTask<GameObject> meshHandle;

	// Token: 0x04001475 RID: 5237
	[PublicizedFrom(EAccessModifier.Private)]
	public LoadManager.AssetRequestTask<Material> altMaterialHandle;
}
