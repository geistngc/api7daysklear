using System;

// Token: 0x02000928 RID: 2344
public class EventPrefabsClient
{
	// Token: 0x0600441F RID: 17439 RVA: 0x001A93A8 File Offset: 0x001A75A8
	public EventPrefabsClient(PrefabCache prefabCache, DynamicPrefabDecorator dynamicPrefabDecorator)
	{
		this.prefabCache = prefabCache;
		this.dpd = dynamicPrefabDecorator;
	}

	// Token: 0x06004420 RID: 17440 RVA: 0x001A93C0 File Offset: 0x001A75C0
	public void TryAdd(int id, string prefabName, byte rotation, Vector3i position)
	{
		if (this.dpd.GetPrefab(id) != null)
		{
			Log.Error(string.Format("[{0}] Cannot add prefab, prefab already exists with id {1}", "EventPrefabsClient", id));
			return;
		}
		Prefab prefabRotated = this.prefabCache.GetPrefabRotated(prefabName, (int)rotation, true, true, false, false);
		if (prefabRotated == null)
		{
			Log.Error("[EventPrefabsClient] Could not load prefab '" + prefabName + "'");
			return;
		}
		Log.Out(string.Format("EventPrefabsClient Add {0} {1}", id, prefabName));
		PrefabInstance prefabInstance = new PrefabInstance(id, prefabRotated.location, position, rotation, prefabRotated, 0);
		this.dpd.AddEventPrefab(prefabInstance);
		DecoManager.Instance.ClearDecoObjectsInArea(prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxSize);
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x001A946C File Offset: 0x001A766C
	public void Remove(int id, string prefabName, byte rotation, Vector3i position)
	{
		PrefabInstance prefab = this.dpd.GetPrefab(id);
		if (prefab == null)
		{
			Log.Error(string.Format("[{0}] Could not find prefab with id {1} to remove", "EventPrefabsClient", id));
			return;
		}
		if (prefab.prefab.PrefabName != prefabName)
		{
			Log.Error(string.Format("[{0}] trying to remove prefab with id {1} but it has a different name. Looking for: {2}, found: {3}", new object[]
			{
				"EventPrefabsClient",
				id,
				prefabName,
				prefab.prefab.PrefabName
			}));
			return;
		}
		if (prefab.boundingBoxPosition != position)
		{
			Log.Error(string.Format("[{0}] trying to remove prefab with id {1} but it has a different position. Looking for: {2}, found: {3}", new object[]
			{
				"EventPrefabsClient",
				id,
				position,
				prefab.boundingBoxPosition
			}));
			return;
		}
		if (prefab.rotation != rotation)
		{
			Log.Error(string.Format("[{0}] trying to remove prefab with id {1} but it has a different rotation. Looking for: {2}, found: {3}", new object[]
			{
				"EventPrefabsClient",
				id,
				rotation,
				prefab.rotation
			}));
			return;
		}
		Log.Out(string.Format("EventPrefabsClient Remove {0} {1}", id, prefabName));
		this.dpd.RemoveEventPrefab(prefab);
	}

	// Token: 0x040036EA RID: 14058
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabCache prefabCache;

	// Token: 0x040036EB RID: 14059
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicPrefabDecorator dpd;
}
