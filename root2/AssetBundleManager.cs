using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02001389 RID: 5001
public class AssetBundleManager
{
	// Token: 0x170012A0 RID: 4768
	// (get) Token: 0x06009DC3 RID: 40387 RVA: 0x003BC162 File Offset: 0x003BA362
	public static AssetBundleManager Instance
	{
		get
		{
			AssetBundleManager result;
			if ((result = AssetBundleManager.instance) == null)
			{
				result = (AssetBundleManager.instance = new AssetBundleManager());
			}
			return result;
		}
	}

	// Token: 0x06009DC4 RID: 40388 RVA: 0x003BC178 File Offset: 0x003BA378
	[PublicizedFrom(EAccessModifier.Private)]
	public AssetBundleManager()
	{
	}

	// Token: 0x06009DC5 RID: 40389 RVA: 0x003BC18C File Offset: 0x003BA38C
	public void LoadAssetBundle(string _name, bool _forceBundle = false)
	{
		string text;
		if (Path.IsPathRooted(_name))
		{
			text = _name;
		}
		else
		{
			text = string.Concat(new string[]
			{
				GameIO.GetApplicationPath(),
				"/Data/Bundles/Standalone",
				BundleTags.Tag,
				"/",
				_name
			});
		}
		string key = _name + 1.ToString();
		if (!this.dictAssetBundleRefs.ContainsKey(key))
		{
			string directoryName = Path.GetDirectoryName(text);
			if (!Directory.Exists(directoryName))
			{
				Log.Error("Loading AssetBundle \"" + text + "\" failed: Parent folder not found!");
				return;
			}
			string fileName = Path.GetFileName(text);
			text = null;
			foreach (string text2 in Directory.EnumerateFiles(directoryName))
			{
				if (Path.GetFileName(text2).EqualsCaseInsensitive(fileName))
				{
					text = text2;
					break;
				}
			}
			if (text == null)
			{
				Log.Error("Loading AssetBundle \"" + fileName + "\" failed: File not found!");
				return;
			}
			AssetBundle assetBundle = AssetBundle.LoadFromFile(text);
			if (assetBundle == null)
			{
				Log.Error("Loading AssetBundle \"" + text + "\" failed!");
				return;
			}
			AssetBundleManager.AssetBundleRef assetBundleRef = new AssetBundleManager.AssetBundleRef(text, 1);
			assetBundleRef.assetBundle = assetBundle;
			this.dictAssetBundleRefs.Add(key, assetBundleRef);
		}
	}

	// Token: 0x06009DC6 RID: 40390 RVA: 0x003BC2DC File Offset: 0x003BA4DC
	public T Get<T>(string _bundleName, string _objName, bool _forceBundle = false) where T : UnityEngine.Object
	{
		return this._get<T>(_bundleName, _objName, _forceBundle, false);
	}

	// Token: 0x06009DC7 RID: 40391 RVA: 0x003BC2E8 File Offset: 0x003BA4E8
	public T Get<T>(DataLoader.DataPathIdentifier _dpi, bool _useRelativePath, bool _forceBundle = false) where T : UnityEngine.Object
	{
		return this._get<T>(_dpi.BundlePath, _dpi.AssetName, _forceBundle, _useRelativePath);
	}

	// Token: 0x06009DC8 RID: 40392 RVA: 0x003BC300 File Offset: 0x003BA500
	[PublicizedFrom(EAccessModifier.Private)]
	public T _get<T>(string _bundleName, string _objName, bool _forceBundle = false, bool _useRelativePath = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return default(T);
		}
		if (!_useRelativePath)
		{
			if (_objName.IndexOf('/') > 0)
			{
				_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
			}
			_objName = GameIO.RemoveFileExtension(_objName);
		}
		return assetBundleRef.assetBundle.LoadAsset<T>(_objName);
	}

	// Token: 0x06009DC9 RID: 40393 RVA: 0x003BC36C File Offset: 0x003BA56C
	public AssetBundleManager.AssetBundleRequestTFP GetAsync<T>(string _bundleName, string _objName, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		if (_objName.IndexOf('/') > 0)
		{
			_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
		}
		return new AssetBundleManager.AssetBundleRequestTFP(assetBundleRef.assetBundle.LoadAssetAsync<T>(GameIO.RemoveFileExtension(_objName)));
	}

	// Token: 0x06009DCA RID: 40394 RVA: 0x003BC3D0 File Offset: 0x003BA5D0
	public bool Contains(string _bundleName, string _objName, bool _forceBundle = false)
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return false;
		}
		if (_objName.IndexOf('/') > 0)
		{
			_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
		}
		return assetBundleRef.assetBundle.Contains(GameIO.RemoveFileExtension(_objName));
	}

	// Token: 0x06009DCB RID: 40395 RVA: 0x003BC430 File Offset: 0x003BA630
	public T[] GetAllObjects<T>(string _bundleName, string _subpath = null, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return assetBundleRef.assetBundle.LoadAllAssets<T>();
	}

	// Token: 0x06009DCC RID: 40396 RVA: 0x003BC46C File Offset: 0x003BA66C
	public AssetBundleManager.AssetBundleMassRequestTFP GetAllObjectsAsync<T>(string _bundleName, string _subpath = null, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return new AssetBundleManager.AssetBundleMassRequestTFP(assetBundleRef.assetBundle.LoadAllAssetsAsync<T>());
	}

	// Token: 0x06009DCD RID: 40397 RVA: 0x003BC4AC File Offset: 0x003BA6AC
	public string[] GetAllAssetNames(string _bundleName, bool _forceBundle = false)
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return assetBundleRef.assetBundle.GetAllAssetNames();
	}

	// Token: 0x06009DCE RID: 40398 RVA: 0x003BC4E8 File Offset: 0x003BA6E8
	public void Unload(string _name, bool _forceBundle = false)
	{
		string key = _name + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			assetBundleRef.assetBundle.Unload(true);
			assetBundleRef.assetBundle = null;
			this.dictAssetBundleRefs.Remove(key);
		}
	}

	// Token: 0x06009DCF RID: 40399 RVA: 0x003BC538 File Offset: 0x003BA738
	public void UnloadAll(bool _forceBundle = false)
	{
		foreach (string key in this.dictAssetBundleRefs.Keys)
		{
			this.dictAssetBundleRefs[key].assetBundle.Unload(true);
			this.dictAssetBundleRefs[key].assetBundle = null;
		}
		this.dictAssetBundleRefs.Clear();
	}

	// Token: 0x04007817 RID: 30743
	[PublicizedFrom(EAccessModifier.Private)]
	public static AssetBundleManager instance;

	// Token: 0x04007818 RID: 30744
	[PublicizedFrom(EAccessModifier.Private)]
	public const int version = 1;

	// Token: 0x04007819 RID: 30745
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AssetBundleManager.AssetBundleRef> dictAssetBundleRefs = new CaseInsensitiveStringDictionary<AssetBundleManager.AssetBundleRef>();

	// Token: 0x0200138A RID: 5002
	public class AssetBundleRequestTFP : CustomYieldInstruction
	{
		// Token: 0x170012A1 RID: 4769
		// (get) Token: 0x06009DD0 RID: 40400 RVA: 0x003BC5C0 File Offset: 0x003BA7C0
		public UnityEngine.Object Asset
		{
			get
			{
				if (!this.IsBundleLoad)
				{
					return this.asset;
				}
				return this.request.asset;
			}
		}

		// Token: 0x170012A2 RID: 4770
		// (get) Token: 0x06009DD1 RID: 40401 RVA: 0x003BC5DC File Offset: 0x003BA7DC
		public bool IsDone
		{
			get
			{
				return !this.IsBundleLoad || this.request.isDone;
			}
		}

		// Token: 0x170012A3 RID: 4771
		// (get) Token: 0x06009DD2 RID: 40402 RVA: 0x003BC5F3 File Offset: 0x003BA7F3
		public override bool keepWaiting
		{
			get
			{
				return this.IsBundleLoad && !this.request.isDone;
			}
		}

		// Token: 0x170012A4 RID: 4772
		// (get) Token: 0x06009DD3 RID: 40403 RVA: 0x003BC60D File Offset: 0x003BA80D
		public bool IsBundleLoad
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.request != null;
			}
		}

		// Token: 0x06009DD4 RID: 40404 RVA: 0x003BC618 File Offset: 0x003BA818
		public AssetBundleRequestTFP(UnityEngine.Object _asset)
		{
			this.asset = _asset;
		}

		// Token: 0x06009DD5 RID: 40405 RVA: 0x003BC627 File Offset: 0x003BA827
		public AssetBundleRequestTFP(AssetBundleRequest _request)
		{
			this.request = _request;
		}

		// Token: 0x0400781A RID: 30746
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly UnityEngine.Object asset;

		// Token: 0x0400781B RID: 30747
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AssetBundleRequest request;
	}

	// Token: 0x0200138B RID: 5003
	public class AssetBundleMassRequestTFP : CustomYieldInstruction
	{
		// Token: 0x170012A5 RID: 4773
		// (get) Token: 0x06009DD6 RID: 40406 RVA: 0x003BC636 File Offset: 0x003BA836
		public UnityEngine.Object[] Assets
		{
			get
			{
				if (!this.IsBundleLoad)
				{
					return this.assets;
				}
				return this.request.allAssets;
			}
		}

		// Token: 0x170012A6 RID: 4774
		// (get) Token: 0x06009DD7 RID: 40407 RVA: 0x003BC652 File Offset: 0x003BA852
		public bool IsDone
		{
			get
			{
				return !this.IsBundleLoad || this.request.isDone;
			}
		}

		// Token: 0x170012A7 RID: 4775
		// (get) Token: 0x06009DD8 RID: 40408 RVA: 0x003BC669 File Offset: 0x003BA869
		public override bool keepWaiting
		{
			get
			{
				return this.IsBundleLoad && !this.request.isDone;
			}
		}

		// Token: 0x170012A8 RID: 4776
		// (get) Token: 0x06009DD9 RID: 40409 RVA: 0x003BC683 File Offset: 0x003BA883
		public bool IsBundleLoad
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.request != null;
			}
		}

		// Token: 0x06009DDA RID: 40410 RVA: 0x003BC68E File Offset: 0x003BA88E
		public AssetBundleMassRequestTFP(List<UnityEngine.Object> _assets)
		{
			this.assets = _assets.ToArray();
		}

		// Token: 0x06009DDB RID: 40411 RVA: 0x003BC6A2 File Offset: 0x003BA8A2
		public AssetBundleMassRequestTFP(AssetBundleRequest _request)
		{
			this.request = _request;
		}

		// Token: 0x0400781C RID: 30748
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly UnityEngine.Object[] assets;

		// Token: 0x0400781D RID: 30749
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AssetBundleRequest request;
	}

	// Token: 0x0200138C RID: 5004
	[PublicizedFrom(EAccessModifier.Private)]
	public class AssetBundleRef
	{
		// Token: 0x06009DDC RID: 40412 RVA: 0x003BC6B1 File Offset: 0x003BA8B1
		public AssetBundleRef(string _url, int _version)
		{
			this.url = _url;
			this.version = _version;
		}

		// Token: 0x0400781E RID: 30750
		public AssetBundle assetBundle;

		// Token: 0x0400781F RID: 30751
		public int version;

		// Token: 0x04007820 RID: 30752
		public string url;
	}
}
