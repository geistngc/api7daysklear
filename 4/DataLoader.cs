using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x020013C6 RID: 5062
public static class DataLoader
{
	// Token: 0x06009F3A RID: 40762 RVA: 0x003C2F50 File Offset: 0x003C1150
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInResources(string _uri)
	{
		return _uri.IndexOf('#') < 0 && _uri.IndexOf('@') < 0;
	}

	// Token: 0x06009F3B RID: 40763 RVA: 0x003C2F6C File Offset: 0x003C116C
	public static DataLoader.DataPathIdentifier ParseDataPathIdentifier(string _inputUri)
	{
		if (_inputUri == null)
		{
			return new DataLoader.DataPathIdentifier(null, DataLoader.DataPathIdentifier.AssetLocation.Resources, false);
		}
		string text = ModManager.PatchModPathString(_inputUri);
		if (text != null)
		{
			_inputUri = text;
		}
		if (_inputUri.IndexOf('#') == 0 && _inputUri.IndexOf('?') > 0)
		{
			int num = _inputUri.IndexOf('?');
			string bundlePath = _inputUri.Substring(1, num - 1);
			_inputUri = _inputUri.Substring(num + 1);
			return new DataLoader.DataPathIdentifier(_inputUri, bundlePath, text != null);
		}
		if (_inputUri.IndexOf("@:") == 0)
		{
			return new DataLoader.DataPathIdentifier(_inputUri.Substring(2), DataLoader.DataPathIdentifier.AssetLocation.Addressable, text != null);
		}
		return new DataLoader.DataPathIdentifier(_inputUri, DataLoader.DataPathIdentifier.AssetLocation.Resources, false);
	}

	// Token: 0x06009F3C RID: 40764 RVA: 0x003C2FF9 File Offset: 0x003C11F9
	public static T LoadAsset<T>(DataLoader.DataPathIdentifier _identifier, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAsset<T>(_identifier, null, null, false, true, _ignoreDlcEntitlements).Asset;
	}

	// Token: 0x06009F3D RID: 40765 RVA: 0x003C300B File Offset: 0x003C120B
	public static T LoadAsset<T>(string _uri, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return DataLoader.LoadAsset<T>(DataLoader.ParseDataPathIdentifier(_uri), _ignoreDlcEntitlements);
	}

	// Token: 0x06009F3E RID: 40766 RVA: 0x003C3019 File Offset: 0x003C1219
	public static T LoadAsset<T>(AssetReference assetReference, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAssetFromAddressables<T>(assetReference, null, null, false, true, _ignoreDlcEntitlements).Asset;
	}

	// Token: 0x06009F3F RID: 40767 RVA: 0x003C302B File Offset: 0x003C122B
	public static void UnloadAsset(DataLoader.DataPathIdentifier _srcIdentifier, UnityEngine.Object _obj)
	{
		if (_srcIdentifier.IsBundle)
		{
			Resources.UnloadUnusedAssets();
			return;
		}
		Resources.UnloadAsset(_obj);
		if (_srcIdentifier.Location == DataLoader.DataPathIdentifier.AssetLocation.Addressable)
		{
			LoadManager.ReleaseAddressable<UnityEngine.Object>(_obj);
		}
	}

	// Token: 0x06009F40 RID: 40768 RVA: 0x003C3052 File Offset: 0x003C1252
	public static void UnloadAsset(string _uri, UnityEngine.Object _obj)
	{
		DataLoader.UnloadAsset(DataLoader.ParseDataPathIdentifier(_uri), _obj);
	}

	// Token: 0x06009F41 RID: 40769 RVA: 0x003C3060 File Offset: 0x003C1260
	public static void PreloadBundle(DataLoader.DataPathIdentifier _identifier)
	{
		if (_identifier.IsBundle)
		{
			AssetBundleManager.Instance.LoadAssetBundle(_identifier.BundlePath, _identifier.FromMod);
		}
	}

	// Token: 0x06009F42 RID: 40770 RVA: 0x003C3081 File Offset: 0x003C1281
	public static void PreloadBundle(string _uri)
	{
		DataLoader.PreloadBundle(DataLoader.ParseDataPathIdentifier(_uri));
	}

	// Token: 0x020013C7 RID: 5063
	public struct DataPathIdentifier
	{
		// Token: 0x170012C7 RID: 4807
		// (get) Token: 0x06009F43 RID: 40771 RVA: 0x003C308E File Offset: 0x003C128E
		public bool IsBundle
		{
			get
			{
				return this.Location == DataLoader.DataPathIdentifier.AssetLocation.Bundle;
			}
		}

		// Token: 0x06009F44 RID: 40772 RVA: 0x003C3099 File Offset: 0x003C1299
		public DataPathIdentifier(string _assetName, DataLoader.DataPathIdentifier.AssetLocation _location = DataLoader.DataPathIdentifier.AssetLocation.Resources, bool _fromMod = false)
		{
			this.BundlePath = null;
			this.AssetName = _assetName;
			this.Location = _location;
			this.FromMod = _fromMod;
		}

		// Token: 0x06009F45 RID: 40773 RVA: 0x003C30B7 File Offset: 0x003C12B7
		public DataPathIdentifier(string _assetName, string _bundlePath, bool _fromMod = false)
		{
			this.BundlePath = _bundlePath;
			this.AssetName = _assetName;
			this.Location = DataLoader.DataPathIdentifier.AssetLocation.Bundle;
			this.FromMod = _fromMod;
		}

		// Token: 0x040078EB RID: 30955
		public readonly DataLoader.DataPathIdentifier.AssetLocation Location;

		// Token: 0x040078EC RID: 30956
		public readonly string BundlePath;

		// Token: 0x040078ED RID: 30957
		public readonly string AssetName;

		// Token: 0x040078EE RID: 30958
		public readonly bool FromMod;

		// Token: 0x020013C8 RID: 5064
		public enum AssetLocation
		{
			// Token: 0x040078F0 RID: 30960
			Resources,
			// Token: 0x040078F1 RID: 30961
			Bundle,
			// Token: 0x040078F2 RID: 30962
			Addressable
		}
	}
}
