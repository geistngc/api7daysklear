using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200140B RID: 5131
public class GameObjectPool
{
	// Token: 0x170012E9 RID: 4841
	// (get) Token: 0x0600A13F RID: 41279 RVA: 0x003CA6E8 File Offset: 0x003C88E8
	public static GameObjectPool Instance
	{
		get
		{
			if (GameObjectPool.instance == null)
			{
				GameObjectPool.Instantiate();
			}
			return GameObjectPool.instance;
		}
	}

	// Token: 0x170012EA RID: 4842
	// (get) Token: 0x0600A140 RID: 41280 RVA: 0x003CA6FB File Offset: 0x003C88FB
	// (set) Token: 0x0600A141 RID: 41281 RVA: 0x003CA703 File Offset: 0x003C8903
	public int MaxPooledInstancesPerItem
	{
		get
		{
			return this.maxPooledInstancesPerItem;
		}
		set
		{
			this.maxPooledInstancesPerItem = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "MaxPooledInstancesPerItem", this.maxPooledInstancesPerItem));
		}
	}

	// Token: 0x170012EB RID: 4843
	// (get) Token: 0x0600A142 RID: 41282 RVA: 0x003CA72B File Offset: 0x003C892B
	// (set) Token: 0x0600A143 RID: 41283 RVA: 0x003CA733 File Offset: 0x003C8933
	public int MaxDestroysPerUpdate
	{
		get
		{
			return this.maxDestroysPerUpdate;
		}
		set
		{
			this.maxDestroysPerUpdate = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "MaxDestroysPerUpdate", this.maxDestroysPerUpdate));
		}
	}

	// Token: 0x170012EC RID: 4844
	// (get) Token: 0x0600A144 RID: 41284 RVA: 0x003CA75B File Offset: 0x003C895B
	// (set) Token: 0x0600A145 RID: 41285 RVA: 0x003CA763 File Offset: 0x003C8963
	public GameObjectPool.ShrinkThreshold ShrinkThresholdHigh
	{
		get
		{
			return this.shrinkThresholdHigh;
		}
		set
		{
			this.shrinkThresholdHigh = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdHigh", this.shrinkThresholdHigh));
		}
	}

	// Token: 0x170012ED RID: 4845
	// (get) Token: 0x0600A146 RID: 41286 RVA: 0x003CA78B File Offset: 0x003C898B
	// (set) Token: 0x0600A147 RID: 41287 RVA: 0x003CA793 File Offset: 0x003C8993
	public GameObjectPool.ShrinkThreshold ShrinkThresholdMedium
	{
		get
		{
			return this.shrinkThresholdMedium;
		}
		set
		{
			this.shrinkThresholdMedium = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdMedium", this.shrinkThresholdMedium));
		}
	}

	// Token: 0x170012EE RID: 4846
	// (get) Token: 0x0600A148 RID: 41288 RVA: 0x003CA7BB File Offset: 0x003C89BB
	// (set) Token: 0x0600A149 RID: 41289 RVA: 0x003CA7C3 File Offset: 0x003C89C3
	public GameObjectPool.ShrinkThreshold ShrinkThresholdLow
	{
		get
		{
			return this.shrinkThresholdLow;
		}
		set
		{
			this.shrinkThresholdLow = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdLow", this.shrinkThresholdLow));
		}
	}

	// Token: 0x0600A14A RID: 41290 RVA: 0x003CA7EB File Offset: 0x003C89EB
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Instantiate()
	{
		GameObjectPool.instance = new GameObjectPool();
	}

	// Token: 0x0600A14B RID: 41291 RVA: 0x003CA7F7 File Offset: 0x003C89F7
	public void Init()
	{
		PlatformOptimizations.ConfigureGameObjectPoolForPlatform(this);
		this.tintMaskShader = GlobalAssets.FindShader("Game/Entity Tint Mask");
	}

	// Token: 0x0600A14C RID: 41292 RVA: 0x003CA810 File Offset: 0x003C8A10
	public void Cleanup()
	{
		foreach (KeyValuePair<string, GameObjectPool.PoolItem> keyValuePair in this.pool)
		{
			List<GameObject> objs = keyValuePair.Value.objs;
			for (int i = 0; i < objs.Count; i++)
			{
				this.DestroyObject(objs[i].gameObject);
			}
			objs.Clear();
		}
		this.activePool.Clear();
		for (int j = 0; j < this.asyncItems.Count; j++)
		{
			GameObjectPool.AsyncItem asyncItem = this.asyncItems[j];
			if (!asyncItem.async.isDone)
			{
				asyncItem.async.Cancel();
			}
		}
		this.asyncItems.Clear();
	}

	// Token: 0x0600A14D RID: 41293 RVA: 0x003CA8EC File Offset: 0x003C8AEC
	public void FrameUpdate()
	{
		float time = Time.time;
		int num = 0;
		for (int i = this.activePool.Count - 1; i >= 0; i--)
		{
			GameObjectPool.PoolItem poolItem = this.activePool[i];
			if (poolItem.updateTime - time <= 0f)
			{
				int num2 = poolItem.objs.Count;
				if (num2 <= 0)
				{
					if (poolItem.activeCount <= 0)
					{
						this.activePool.RemoveAt(i);
					}
				}
				else
				{
					GameObjectPool.ShrinkThreshold shrinkThreshold = this.GetShrinkThreshold(num2);
					poolItem.updateTime = time + shrinkThreshold.Delay;
					int num3 = Mathf.Min(num2, shrinkThreshold.DestroyCount);
					int num4 = 0;
					while (num4 < num3 && num < this.maxDestroysPerUpdate)
					{
						num2--;
						GameObject obj = poolItem.objs[num2];
						poolItem.objs.RemoveAt(num2);
						poolItem.activeCount--;
						this.DestroyObject(obj);
						num++;
						num4++;
					}
					if (num >= this.maxDestroysPerUpdate)
					{
						break;
					}
				}
			}
		}
		for (int j = this.asyncItems.Count - 1; j >= 0; j--)
		{
			GameObjectPool.AsyncItem asyncItem = this.asyncItems[j];
			if (asyncItem.async.isDone)
			{
				UnityEngine.Object[] result = asyncItem.async.Result;
				int num5 = result.Length;
				GameObjectPool.PoolItem item = asyncItem.item;
				item.activeCount += num5;
				for (int k = 0; k < num5; k++)
				{
					GameObject gameObject = (GameObject)result[k];
					gameObject.name = item.name;
					if (item.createOnceToAllCallback != null)
					{
						item.createOnceToAllCallback(gameObject);
						item.createOnceToAllCallback = null;
					}
					if (item.createCallback != null)
					{
						item.createCallback(gameObject);
					}
				}
				asyncItem.callback(asyncItem.userData, result, num5, true);
				this.asyncItems.RemoveAt(j);
			}
		}
	}

	// Token: 0x0600A14E RID: 41294 RVA: 0x003CAAEC File Offset: 0x003C8CEC
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold GetShrinkThreshold(int count)
	{
		if (count >= this.shrinkThresholdHigh.Count)
		{
			return this.shrinkThresholdHigh;
		}
		if (count >= this.shrinkThresholdMedium.Count)
		{
			return this.shrinkThresholdMedium;
		}
		if (count >= this.shrinkThresholdLow.Count)
		{
			return this.shrinkThresholdLow;
		}
		return this.shrinkThresholdMin;
	}

	// Token: 0x0600A14F RID: 41295 RVA: 0x003CAB40 File Offset: 0x003C8D40
	public void AddPooledObject(string name, GameObjectPool.LoadCallback _loadCallback, GameObjectPool.CreateCallback _createOnceToAllCallback, GameObjectPool.CreateCallback _createCallback)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(name, out poolItem))
		{
			poolItem = new GameObjectPool.PoolItem();
			poolItem.name = name;
			poolItem.loadCallback = _loadCallback;
			poolItem.createOnceToAllCallback = _createOnceToAllCallback;
			poolItem.createCallback = _createCallback;
			poolItem.objs = new List<GameObject>();
			this.pool.Add(name, poolItem);
		}
		else
		{
			GameObjectPool.PoolItem poolItem2 = poolItem;
			poolItem2.createOnceToAllCallback = (GameObjectPool.CreateCallback)Delegate.Combine(poolItem2.createOnceToAllCallback, _createOnceToAllCallback);
		}
		Transform transform = poolItem.loadCallback();
		if (transform)
		{
			this.setItemPrefab(poolItem, transform.gameObject);
		}
	}

	// Token: 0x0600A150 RID: 41296 RVA: 0x003CABD2 File Offset: 0x003C8DD2
	[PublicizedFrom(EAccessModifier.Private)]
	public void setItemPrefab(GameObjectPool.PoolItem item, GameObject go)
	{
		item.prefab = go;
		this.getOriginalTint(item, go);
	}

	// Token: 0x0600A151 RID: 41297 RVA: 0x003CABE4 File Offset: 0x003C8DE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void getOriginalTint(GameObjectPool.PoolItem item, GameObject go)
	{
		bool flag = false;
		List<Color> list = new List<Color>();
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			foreach (Material material in componentsInChildren[i].sharedMaterials)
			{
				if (!(material != null) || !(material.shader == this.tintMaskShader))
				{
					flag = true;
					break;
				}
				list.Add(material.color);
			}
		}
		item.originalTint = Color.clear;
		if (!flag && list.Count > 0)
		{
			int num = 1;
			while (num < list.Count && !(list[0] != list[num]))
			{
				num++;
			}
			if (num == list.Count)
			{
				item.originalTint = list[0];
			}
		}
	}

	// Token: 0x0600A152 RID: 41298 RVA: 0x003CACBC File Offset: 0x003C8EBC
	public GameObject GetObjectForType(string objectType)
	{
		Color color;
		return this.GetObjectForType(objectType, out color);
	}

	// Token: 0x0600A153 RID: 41299 RVA: 0x003CACD4 File Offset: 0x003C8ED4
	public GameObject GetObjectForType(string objectType, out Color originalTint)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(objectType, out poolItem))
		{
			Log.Error("GameObjectPool GetObjectForType {0} unknown", new object[]
			{
				objectType
			});
			originalTint = Color.white;
			return null;
		}
		GameObject gameObject = poolItem.prefab;
		if (!gameObject)
		{
			Transform transform = poolItem.loadCallback();
			if (transform)
			{
				gameObject = transform.gameObject;
				this.setItemPrefab(poolItem, gameObject);
			}
		}
		originalTint = poolItem.originalTint;
		if (!gameObject)
		{
			return null;
		}
		List<GameObject> objs = poolItem.objs;
		int count = objs.Count;
		if (count > 0)
		{
			poolItem.updateTime = Time.time + 5f;
			GameObject result = objs[count - 1];
			objs.RemoveAt(count - 1);
			return result;
		}
		return poolItem.Instantiate();
	}

	// Token: 0x0600A154 RID: 41300 RVA: 0x003CADA0 File Offset: 0x003C8FA0
	public GameObjectPool.AsyncItem GetObjectsForTypeAsync(string objectType, int _count, GameObjectPool.CreateAsyncCallback _callback, object _userData)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(objectType, out poolItem))
		{
			Log.Error("GameObjectPool GetObjectForType {0} unknown", new object[]
			{
				objectType
			});
			return null;
		}
		GameObject gameObject = poolItem.prefab;
		if (!gameObject)
		{
			Transform transform = poolItem.loadCallback();
			if (transform)
			{
				gameObject = transform.gameObject;
				this.setItemPrefab(poolItem, gameObject);
			}
		}
		if (!gameObject)
		{
			return null;
		}
		List<GameObject> objs = poolItem.objs;
		int count = objs.Count;
		if (count >= _count && count <= 128)
		{
			poolItem.updateTime = Time.time + 5f;
			for (int i = 0; i < _count; i++)
			{
				int index = count - 1 - i;
				GameObject gameObject2 = objs[index];
				objs.RemoveAt(index);
				this.asyncPoolObjs[i] = gameObject2;
			}
			UnityEngine.Object[] objs2 = this.asyncPoolObjs;
			_callback(_userData, objs2, _count, false);
			return null;
		}
		if (_count <= 3)
		{
			for (int j = 0; j < _count; j++)
			{
				GameObject gameObject3 = poolItem.Instantiate();
				this.asyncPoolObjs[j] = gameObject3;
			}
			UnityEngine.Object[] objs2 = this.asyncPoolObjs;
			_callback(_userData, objs2, _count, false);
			return null;
		}
		GameObjectPool.AsyncItem asyncItem = new GameObjectPool.AsyncItem();
		asyncItem.item = poolItem;
		asyncItem.callback = _callback;
		asyncItem.userData = _userData;
		asyncItem.async = UnityEngine.Object.InstantiateAsync<GameObject>(gameObject, _count);
		this.asyncItems.Add(asyncItem);
		return asyncItem;
	}

	// Token: 0x0600A155 RID: 41301 RVA: 0x003CAF08 File Offset: 0x003C9108
	public void CancelAsync(GameObjectPool.AsyncItem _ai)
	{
		if (this.asyncItems.Remove(_ai))
		{
			if (_ai.async.isDone)
			{
				UnityEngine.Object[] result = _ai.async.Result;
				for (int i = 0; i < result.Length; i++)
				{
					UnityEngine.Object.Destroy(result[i]);
				}
				return;
			}
			_ai.async.Cancel();
		}
	}

	// Token: 0x0600A156 RID: 41302 RVA: 0x003CAF5E File Offset: 0x003C915E
	public void PoolObjectAsync(GameObject obj)
	{
		this.PoolObject(obj);
	}

	// Token: 0x0600A157 RID: 41303 RVA: 0x003CAF68 File Offset: 0x003C9168
	public void PoolObject(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		string name = obj.name;
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(name, out poolItem))
		{
			return;
		}
		List<GameObject> objs = poolItem.objs;
		if (objs.Count < this.maxPooledInstancesPerItem)
		{
			obj.SetActive(false);
			obj.transform.SetParent(null, false);
			objs.Add(obj);
			if (objs.Count >= 1 && !this.activePool.Contains(poolItem))
			{
				this.activePool.Add(poolItem);
				return;
			}
		}
		else
		{
			poolItem.activeCount--;
			obj.SetActive(false);
			this.DestroyObject(obj);
		}
	}

	// Token: 0x0600A158 RID: 41304 RVA: 0x003CB006 File Offset: 0x003C9206
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyObject(GameObject obj)
	{
		obj.GetComponentsInChildren<Renderer>(this.tempRenderers);
		Utils.CleanupMaterialsOfRenderers<List<Renderer>>(this.tempRenderers);
		this.tempRenderers.Clear();
		UnityEngine.Object.Destroy(obj);
	}

	// Token: 0x0600A159 RID: 41305 RVA: 0x003CB030 File Offset: 0x003C9230
	public void CmdList(string _mode)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Pool objects:");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		foreach (KeyValuePair<string, GameObjectPool.PoolItem> keyValuePair in this.pool)
		{
			GameObjectPool.PoolItem value = keyValuePair.Value;
			if (value.prefab != null)
			{
				list.Add(value.prefab);
			}
			num += value.activeCount;
			num2 += value.objs.Count;
			if (value.activeCount > 0)
			{
				num3++;
				list2.Add(value.prefab);
			}
			if (_mode == "all" || (_mode == "active" && value.activeCount > 0))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" {0}, prefab {1}, active {2}, count {3}", new object[]
				{
					value.name,
					value.prefab ? "1" : "0",
					value.activeCount,
					value.objs.Count
				});
			}
		}
		string text = string.Format(" types {0}, used {1}, pooled {2}, active {3}", new object[]
		{
			this.pool.Count,
			num3,
			num2,
			num
		});
		if (Application.isEditor)
		{
			long num4;
			long num5;
			ProfilerUtils.CalculateDependentBytes(list.ToArray(), out num4, out num5);
			long num6;
			long num7;
			ProfilerUtils.CalculateDependentBytes(list2.ToArray(), out num6, out num7);
			text += string.Format(", used mesh {0:F2} MB, used texture {1:F2} MB, required mesh {2:F2} MB, required texture {3:F2} MB", new object[]
			{
				(double)num4 * 9.5367431640625E-07,
				(double)num5 * 9.5367431640625E-07,
				(double)num6 * 9.5367431640625E-07,
				(double)num7 * 9.5367431640625E-07
			});
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(text);
	}

	// Token: 0x0600A15A RID: 41306 RVA: 0x003CB268 File Offset: 0x003C9468
	public void CmdShrink()
	{
		bool flag;
		do
		{
			flag = false;
			for (int i = this.activePool.Count - 1; i >= 0; i--)
			{
				GameObjectPool.PoolItem poolItem = this.activePool[i];
				if (poolItem.objs.Count > 0)
				{
					poolItem.updateTime = 0f;
					this.FrameUpdate();
					flag = true;
					break;
				}
			}
		}
		while (flag);
	}

	// Token: 0x0600A15B RID: 41307 RVA: 0x000027FC File Offset: 0x000009FC
	[Conditional("DEBUG_GOPOOL_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerBegin(string _name)
	{
	}

	// Token: 0x0600A15C RID: 41308 RVA: 0x000027FC File Offset: 0x000009FC
	[Conditional("DEBUG_GOPOOL_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerEnd()
	{
	}

	// Token: 0x040079AA RID: 31146
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cActivePoolAddAtCount = 1;

	// Token: 0x040079AB RID: 31147
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cActivePoolMinCount = 0;

	// Token: 0x040079AC RID: 31148
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cActivePoolRemoveDelay = 10f;

	// Token: 0x040079AD RID: 31149
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObjectPool instance;

	// Token: 0x040079AE RID: 31150
	[PublicizedFrom(EAccessModifier.Private)]
	public Shader tintMaskShader;

	// Token: 0x040079AF RID: 31151
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, GameObjectPool.PoolItem> pool = new Dictionary<string, GameObjectPool.PoolItem>();

	// Token: 0x040079B0 RID: 31152
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameObjectPool.PoolItem> activePool = new List<GameObjectPool.PoolItem>();

	// Token: 0x040079B1 RID: 31153
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameObjectPool.AsyncItem> asyncItems = new List<GameObjectPool.AsyncItem>();

	// Token: 0x040079B2 RID: 31154
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cAsyncPoolObjsCount = 128;

	// Token: 0x040079B3 RID: 31155
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject[] asyncPoolObjs = new GameObject[128];

	// Token: 0x040079B4 RID: 31156
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Renderer> tempRenderers = new List<Renderer>();

	// Token: 0x040079B5 RID: 31157
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxPooledInstancesPerItem = 200;

	// Token: 0x040079B6 RID: 31158
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxDestroysPerUpdate = 1;

	// Token: 0x040079B7 RID: 31159
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdHigh = new GameObjectPool.ShrinkThreshold(100, 1, 0.1f);

	// Token: 0x040079B8 RID: 31160
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdMedium = new GameObjectPool.ShrinkThreshold(40, 1, 0.5f);

	// Token: 0x040079B9 RID: 31161
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdLow = new GameObjectPool.ShrinkThreshold(12, 1, 3f);

	// Token: 0x040079BA RID: 31162
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdMin = new GameObjectPool.ShrinkThreshold(0, 1, 10f);

	// Token: 0x0200140C RID: 5132
	// (Invoke) Token: 0x0600A15F RID: 41311
	public delegate Transform LoadCallback();

	// Token: 0x0200140D RID: 5133
	// (Invoke) Token: 0x0600A163 RID: 41315
	public delegate void CreateCallback(GameObject obj);

	// Token: 0x0200140E RID: 5134
	// (Invoke) Token: 0x0600A167 RID: 41319
	public delegate void CreateAsyncCallback(object _userData, UnityEngine.Object[] _objs, int _objsCount, bool _isAsync);

	// Token: 0x0200140F RID: 5135
	public class PoolItem
	{
		// Token: 0x0600A16A RID: 41322 RVA: 0x003CB370 File Offset: 0x003C9570
		public GameObject Instantiate()
		{
			this.activeCount++;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
			gameObject.name = this.name;
			if (this.createOnceToAllCallback != null)
			{
				this.createOnceToAllCallback(gameObject);
				this.createOnceToAllCallback = null;
			}
			if (this.createCallback != null)
			{
				this.createCallback(gameObject);
			}
			return gameObject;
		}

		// Token: 0x040079BB RID: 31163
		public string name;

		// Token: 0x040079BC RID: 31164
		public GameObject prefab;

		// Token: 0x040079BD RID: 31165
		public GameObjectPool.LoadCallback loadCallback;

		// Token: 0x040079BE RID: 31166
		public GameObjectPool.CreateCallback createOnceToAllCallback;

		// Token: 0x040079BF RID: 31167
		public GameObjectPool.CreateCallback createCallback;

		// Token: 0x040079C0 RID: 31168
		public List<GameObject> objs;

		// Token: 0x040079C1 RID: 31169
		public float updateTime;

		// Token: 0x040079C2 RID: 31170
		public int activeCount;

		// Token: 0x040079C3 RID: 31171
		public Color originalTint;
	}

	// Token: 0x02001410 RID: 5136
	public class AsyncItem
	{
		// Token: 0x040079C4 RID: 31172
		public GameObjectPool.PoolItem item;

		// Token: 0x040079C5 RID: 31173
		public GameObjectPool.CreateAsyncCallback callback;

		// Token: 0x040079C6 RID: 31174
		public AsyncInstantiateOperation async;

		// Token: 0x040079C7 RID: 31175
		public object userData;
	}

	// Token: 0x02001411 RID: 5137
	public struct ShrinkThreshold
	{
		// Token: 0x0600A16D RID: 41325 RVA: 0x003CB3D3 File Offset: 0x003C95D3
		public ShrinkThreshold(int count, int destroyCount, float delay)
		{
			this.Count = count;
			this.DestroyCount = destroyCount;
			this.Delay = delay;
		}

		// Token: 0x0600A16E RID: 41326 RVA: 0x003CB3EC File Offset: 0x003C95EC
		public override string ToString()
		{
			return string.Format("({0} = {1}, {2} = {3}, {4} = {5:F2}s)", new object[]
			{
				"Count",
				this.Count,
				"DestroyCount",
				this.DestroyCount,
				"Delay",
				this.Delay
			});
		}

		// Token: 0x040079C8 RID: 31176
		public int Count;

		// Token: 0x040079C9 RID: 31177
		public int DestroyCount;

		// Token: 0x040079CA RID: 31178
		public float Delay;
	}
}
