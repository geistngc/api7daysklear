using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C11 RID: 3089
public class DecoChunk
{
	// Token: 0x06005E45 RID: 24133 RVA: 0x0024BDE4 File Offset: 0x00249FE4
	public DecoChunk(int _x, int _z, int _drawX, int _drawZ)
	{
		this.Reset(_x, _z, _drawX, _drawZ);
	}

	// Token: 0x06005E46 RID: 24134 RVA: 0x0024BE30 File Offset: 0x0024A030
	public void Reset(int _x, int _z, int _drawX, int _drawZ)
	{
		this.decoChunkX = _x;
		this.decoChunkZ = _z;
		this.drawX = _drawX;
		this.drawZ = _drawZ;
		this.decosPerSmallChunks.Clear();
		this.isDecorated = false;
		this.isModelsUpdated = false;
		this.isGameObjectUpdated = false;
	}

	// Token: 0x06005E47 RID: 24135 RVA: 0x0024BE70 File Offset: 0x0024A070
	public void RestoreGeneratedDecos(Predicate<DecoObject> decoObjectValidator = null)
	{
		foreach (long smallChunkKey in this.decosPerSmallChunks.Keys)
		{
			this.RestoreGeneratedDecos(smallChunkKey, decoObjectValidator);
		}
	}

	// Token: 0x06005E48 RID: 24136 RVA: 0x0024BECC File Offset: 0x0024A0CC
	public void RestoreGeneratedDecos(long smallChunkKey, Predicate<DecoObject> decoObjectValidator = null)
	{
		List<DecoObject> list;
		if (this.decosPerSmallChunks.TryGetValue(smallChunkKey, out list))
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				DecoObject decoObject = list[i];
				if (decoObjectValidator == null || decoObjectValidator(decoObject))
				{
					switch (decoObject.state)
					{
					case DecoState.GeneratedInactive:
						decoObject.state = DecoState.GeneratedActive;
						this.isModelsUpdated = false;
						break;
					case DecoState.Dynamic:
						this.RemoveDecoObject(decoObject);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06005E49 RID: 24137 RVA: 0x00230B44 File Offset: 0x0022ED44
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int MakeKey16(int _x, int _z)
	{
		return _x << 16 | (_z & 65535);
	}

	// Token: 0x06005E4A RID: 24138 RVA: 0x0024BF42 File Offset: 0x0024A142
	public static int ToDecoChunkPos(float _worldPos)
	{
		return Utils.Fastfloor(_worldPos / 128f);
	}

	// Token: 0x06005E4B RID: 24139 RVA: 0x0024BF50 File Offset: 0x0024A150
	public static int ToDecoChunkPos(int _worldPos)
	{
		if (_worldPos >= 0)
		{
			return _worldPos / 128;
		}
		return (_worldPos - 128 + 1) / 128;
	}

	// Token: 0x06005E4C RID: 24140 RVA: 0x0024BF70 File Offset: 0x0024A170
	public void UpdateGameObject()
	{
		if (!this.rootObj)
		{
			this.rootObj = new GameObject();
		}
		this.SetVisible(true);
		this.rootObj.name = "DecoC_" + this.decoChunkX.ToString() + "_" + this.decoChunkZ.ToString();
		this.rootObj.transform.position = new Vector3((float)(this.drawX * 128), 0f, (float)(this.drawZ * 128)) - Origin.position;
		this.isGameObjectUpdated = true;
	}

	// Token: 0x06005E4D RID: 24141 RVA: 0x0024C011 File Offset: 0x0024A211
	public IEnumerator UpdateModels(World _world, MicroStopwatch ms)
	{
		this.SetVisible(true);
		foreach (KeyValuePair<long, List<DecoObject>> keyValuePair in this.decosPerSmallChunks)
		{
			List<DecoObject> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				DecoObject decoObject = value[i];
				if (decoObject.state != DecoState.GeneratedInactive && !decoObject.go && decoObject.asyncItem == null)
				{
					string modelName = decoObject.GetModelName();
					List<DecoObject> list;
					if (!this.models.TryGetValue(modelName, out list))
					{
						list = new List<DecoObject>();
						this.models.Add(modelName, list);
					}
					list.Add(decoObject);
				}
			}
		}
		foreach (KeyValuePair<string, List<DecoObject>> keyValuePair2 in this.models)
		{
			List<DecoObject> value2 = keyValuePair2.Value;
			GameObjectPool.AsyncItem objectsForTypeAsync = GameObjectPool.Instance.GetObjectsForTypeAsync(keyValuePair2.Key, value2.Count, new GameObjectPool.CreateAsyncCallback(this.CreateGameObjectCallback), value2);
			if (objectsForTypeAsync != null)
			{
				this.asyncItems.Add(objectsForTypeAsync);
				for (int j = 0; j < value2.Count; j++)
				{
					value2[j].asyncItem = objectsForTypeAsync;
				}
			}
			if (ms.ElapsedMicroseconds > 900L)
			{
				yield return null;
				ms.ResetAndRestart();
			}
		}
		Dictionary<string, List<DecoObject>>.Enumerator enumerator2 = default(Dictionary<string, List<DecoObject>>.Enumerator);
		this.models.Clear();
		this.isModelsUpdated = true;
		yield break;
		yield break;
	}

	// Token: 0x06005E4E RID: 24142 RVA: 0x0024C028 File Offset: 0x0024A228
	public void CreateGameObjectCallback(object _userData, UnityEngine.Object[] _objs, int _objsCount, bool _isAsync)
	{
		List<DecoObject> list = (List<DecoObject>)_userData;
		Transform transform = this.rootObj.transform;
		for (int i = 0; i < _objsCount; i++)
		{
			GameObject gameObject = (GameObject)_objs[i];
			list[i].CreateGameObjectCallback(gameObject, transform, _isAsync);
			this.occlusionTs.Add(gameObject.transform);
		}
		if (this.occlusionTs.Count > 0)
		{
			if (OcclusionManager.Instance.cullDecorations)
			{
				OcclusionManager.Instance.AddDeco(this, this.occlusionTs);
			}
			this.occlusionTs.Clear();
		}
	}

	// Token: 0x06005E4F RID: 24143 RVA: 0x0024C0B4 File Offset: 0x0024A2B4
	public void AddDecoObject(DecoObject _decoObject, bool _tryInstantiate = false)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(_decoObject.pos.x), World.toChunkXZ(_decoObject.pos.z));
		List<DecoObject> list;
		if (!this.decosPerSmallChunks.TryGetValue(key, out list))
		{
			list = new List<DecoObject>(64);
			this.decosPerSmallChunks.Add(key, list);
		}
		list.Add(_decoObject);
		if (_tryInstantiate)
		{
			if (ThreadManager.IsMainThread() && this.rootObj)
			{
				_decoObject.CreateGameObject(this, this.rootObj.transform);
				if (OcclusionManager.Instance.cullDecorations && _decoObject.go)
				{
					this.occlusionTs.Add(_decoObject.go.transform);
					OcclusionManager.Instance.AddDeco(this, this.occlusionTs);
					this.occlusionTs.Clear();
					return;
				}
			}
			else
			{
				this.isModelsUpdated = false;
			}
		}
	}

	// Token: 0x06005E50 RID: 24144 RVA: 0x0024C190 File Offset: 0x0024A390
	public DecoObject GetDecoObjectAt(Vector3i _worldBlockPos)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(_worldBlockPos.x), World.toChunkXZ(_worldBlockPos.z));
		List<DecoObject> list;
		if (!this.decosPerSmallChunks.TryGetValue(key, out list))
		{
			return null;
		}
		foreach (DecoObject decoObject in list)
		{
			if (decoObject.pos.x == _worldBlockPos.x && decoObject.pos.z == _worldBlockPos.z && decoObject.state != DecoState.GeneratedInactive)
			{
				return decoObject;
			}
		}
		return null;
	}

	// Token: 0x06005E51 RID: 24145 RVA: 0x0024C240 File Offset: 0x0024A440
	public bool RemoveDecoObject(Vector3i _worldBlockPos)
	{
		DecoObject decoObjectAt = this.GetDecoObjectAt(_worldBlockPos);
		if (decoObjectAt == null)
		{
			return false;
		}
		this.RemoveDecoObject(decoObjectAt);
		return true;
	}

	// Token: 0x06005E52 RID: 24146 RVA: 0x0024C264 File Offset: 0x0024A464
	public void RemoveDecoObject(DecoObject deco)
	{
		if (deco.state == DecoState.Dynamic)
		{
			long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(deco.pos.x), World.toChunkXZ(deco.pos.z));
			List<DecoObject> list;
			if (this.decosPerSmallChunks.TryGetValue(key, out list))
			{
				list.Remove(deco);
			}
		}
		else
		{
			deco.state = DecoState.GeneratedInactive;
		}
		if (OcclusionManager.Instance.cullDecorations && deco.go)
		{
			OcclusionManager.Instance.RemoveDeco(this, deco.go.transform);
		}
		deco.Destroy();
	}

	// Token: 0x06005E53 RID: 24147 RVA: 0x0024C2F8 File Offset: 0x0024A4F8
	public void ClearDecoObjectsInArea(Vector3i _boundsPosition, Vector3i _boundsSize)
	{
		Vector3i vector3i = _boundsPosition + _boundsSize;
		int num = World.toChunkXZ(_boundsPosition.x);
		int num2 = World.toChunkXZ(_boundsPosition.z);
		int num3 = World.toChunkXZ(vector3i.x);
		int num4 = World.toChunkXZ(vector3i.z);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				long key = WorldChunkCache.MakeChunkKey(i, j);
				List<DecoObject> list;
				if (this.decosPerSmallChunks.TryGetValue(key, out list))
				{
					for (int k = list.Count - 1; k >= 0; k--)
					{
						DecoObject decoObject = list[k];
						Vector3i pos = decoObject.pos;
						if (pos.x >= _boundsPosition.x && pos.x < vector3i.x && pos.y >= _boundsPosition.y && pos.y < vector3i.y && pos.z >= _boundsPosition.z && pos.z < vector3i.z)
						{
							if (OcclusionManager.Instance.cullDecorations && decoObject.go)
							{
								OcclusionManager.Instance.RemoveDeco(this, decoObject.go.transform);
							}
							decoObject.Destroy();
							list.RemoveAt(k);
						}
					}
					if (list.Count == 0)
					{
						this.decosPerSmallChunks.Remove(key);
					}
				}
			}
		}
	}

	// Token: 0x06005E54 RID: 24148 RVA: 0x0024C474 File Offset: 0x0024A674
	public void Destroy()
	{
		if (OcclusionManager.Instance.cullDecorations)
		{
			OcclusionManager.Instance.RemoveDecoChunk(this);
		}
		foreach (KeyValuePair<long, List<DecoObject>> keyValuePair in this.decosPerSmallChunks)
		{
			List<DecoObject> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				value[i].Destroy();
			}
		}
		for (int j = 0; j < this.asyncItems.Count; j++)
		{
			GameObjectPool.Instance.CancelAsync(this.asyncItems[j]);
		}
		this.asyncItems.Clear();
		this.isModelsUpdated = false;
		this.isGameObjectUpdated = false;
		UnityEngine.Object.Destroy(this.rootObj);
	}

	// Token: 0x06005E55 RID: 24149 RVA: 0x0024C554 File Offset: 0x0024A754
	public void SetVisible(bool _bVisible)
	{
		if (this.rootObj && this.rootObj.activeSelf != _bVisible)
		{
			this.rootObj.SetActive(_bVisible);
		}
	}

	// Token: 0x06005E56 RID: 24150 RVA: 0x0024C57D File Offset: 0x0024A77D
	public override string ToString()
	{
		return string.Format("DecoChunk {0},{1}", this.decoChunkX, this.decoChunkZ);
	}

	// Token: 0x04004928 RID: 18728
	public int decoChunkX;

	// Token: 0x04004929 RID: 18729
	public int decoChunkZ;

	// Token: 0x0400492A RID: 18730
	public int drawX;

	// Token: 0x0400492B RID: 18731
	public int drawZ;

	// Token: 0x0400492C RID: 18732
	public bool isDecorated;

	// Token: 0x0400492D RID: 18733
	public bool isModelsUpdated;

	// Token: 0x0400492E RID: 18734
	public bool isGameObjectUpdated;

	// Token: 0x0400492F RID: 18735
	public GameObject rootObj;

	// Token: 0x04004930 RID: 18736
	public Dictionary<long, List<DecoObject>> decosPerSmallChunks = new Dictionary<long, List<DecoObject>>(64);

	// Token: 0x04004931 RID: 18737
	public OcclusionManager.OccludeeZone occludeeZone;

	// Token: 0x04004932 RID: 18738
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<Transform> occlusionTs = new List<Transform>();

	// Token: 0x04004933 RID: 18739
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<GameObjectPool.AsyncItem> asyncItems = new List<GameObjectPool.AsyncItem>();

	// Token: 0x04004934 RID: 18740
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, List<DecoObject>> models = new Dictionary<string, List<DecoObject>>();
}
