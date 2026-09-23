using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020013F6 RID: 5110
public class EntityMeshCache : MonoBehaviour
{
	// Token: 0x0600A06E RID: 41070 RVA: 0x003C72D6 File Offset: 0x003C54D6
	public void InitData(List<CachedMeshData> collection)
	{
		this.collection = new List<CachedMeshData>(collection);
	}

	// Token: 0x0600A06F RID: 41071 RVA: 0x003C72E4 File Offset: 0x003C54E4
	public bool TryGetMeshData(string name, out CachedMeshData data)
	{
		name = name.Replace(" Instance", "");
		foreach (CachedMeshData cachedMeshData in this.collection)
		{
			if (cachedMeshData.name == name)
			{
				data = cachedMeshData;
				return true;
			}
		}
		Log.Warning("Could not find {0} in entity mesh cache for prefab: {1}", new object[]
		{
			name,
			base.gameObject.name
		});
		data = new CachedMeshData();
		return false;
	}

	// Token: 0x0600A070 RID: 41072 RVA: 0x003C7384 File Offset: 0x003C5584
	public bool EqualsCollection(List<CachedMeshData> otherCollection)
	{
		if (otherCollection.Count != this.collection.Count)
		{
			return false;
		}
		for (int i = 0; i < this.collection.Count; i++)
		{
			if (!this.collection[i].ApproximatelyEquals(otherCollection[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400796E RID: 31086
	[Header("This collection is filled on import. Do not edit manually")]
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<CachedMeshData> collection;
}
