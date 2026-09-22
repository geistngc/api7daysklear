using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001218 RID: 4632
public class AssetMappings
{
	// Token: 0x17001198 RID: 4504
	// (get) Token: 0x060093F3 RID: 37875 RVA: 0x0037FDD0 File Offset: 0x0037DFD0
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x060093F4 RID: 37876 RVA: 0x0037FDDD File Offset: 0x0037DFDD
	public void Add(string name, string address)
	{
		this.list.Add(new AssetMappings.AssetAddress
		{
			name = name,
			address = address
		});
	}

	// Token: 0x060093F5 RID: 37877 RVA: 0x0037FE00 File Offset: 0x0037E000
	public Dictionary<string, string> ToDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (AssetMappings.AssetAddress assetAddress in this.list)
		{
			dictionary.Add(assetAddress.name, assetAddress.address);
		}
		return dictionary;
	}

	// Token: 0x04006EE1 RID: 28385
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AssetMappings.AssetAddress> list = new List<AssetMappings.AssetAddress>();

	// Token: 0x02001219 RID: 4633
	[Serializable]
	public class AssetAddress
	{
		// Token: 0x04006EE2 RID: 28386
		public string name;

		// Token: 0x04006EE3 RID: 28387
		public string address;
	}
}
