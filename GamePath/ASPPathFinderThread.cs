using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018DD RID: 6365
	public class ASPPathFinderThread : PathFinderThread
	{
		// Token: 0x0600C45A RID: 50266 RVA: 0x0048A03B File Offset: 0x0048823B
		public ASPPathFinderThread()
		{
			PathFinderThread.Instance = this;
		}

		// Token: 0x0600C45B RID: 50267 RVA: 0x0048A05F File Offset: 0x0048825F
		public override int GetFinishedCount()
		{
			return this.finishedPaths.Count;
		}

		// Token: 0x0600C45C RID: 50268 RVA: 0x0048A06C File Offset: 0x0048826C
		public override int GetQueueCount()
		{
			return this.entityWaitQueue.list.Count;
		}

		// Token: 0x0600C45D RID: 50269 RVA: 0x0048A07E File Offset: 0x0048827E
		public override void StartWorkerThreads()
		{
			this.coroutine = GameManager.Instance.StartCoroutine(this.FindPaths());
		}

		// Token: 0x0600C45E RID: 50270 RVA: 0x0048A096 File Offset: 0x00488296
		public override void Cleanup()
		{
			GameManager.Instance.StopCoroutine(this.coroutine);
			this.entityWaitQueue.Clear();
			this.finishedPaths.Clear();
		}

		// Token: 0x0600C45F RID: 50271 RVA: 0x0048A0BE File Offset: 0x004882BE
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator FindPaths()
		{
			for (;;)
			{
				int num = 0;
				while (num < 8 && this.entityWaitQueue.list.Count != 0)
				{
					int num2 = this.entityWaitQueue.list[0];
					this.entityWaitQueue.Remove(num2);
					PathInfo pathInfo;
					if (!this.finishedPaths.TryGetValue(num2, out pathInfo))
					{
						Log.Warning("{0} path dup id {1}", new object[]
						{
							GameManager.frameCount,
							num2
						});
					}
					else
					{
						pathInfo.entity.navigator.GetPathTo(pathInfo);
						if (pathInfo.state == PathInfo.State.Queued)
						{
							this.finishedPaths.Remove(num2);
						}
					}
					num++;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600C460 RID: 50272 RVA: 0x0048A0CD File Offset: 0x004882CD
		public override void FindPath(EntityAlive _entity, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			this.entityWaitQueue.Add(_entity.entityId);
			this.finishedPaths[_entity.entityId] = new PathInfoSingleTarget(_entity, _targetPos, _canBreak, _speed, _aiTask);
		}

		// Token: 0x0600C461 RID: 50273 RVA: 0x0048A100 File Offset: 0x00488300
		public override void FindPath(EntityAlive _entity, Vector3 _startPos, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			this.entityWaitQueue.Add(_entity.entityId);
			PathInfoSingleTarget pathInfoSingleTarget = new PathInfoSingleTarget(_entity, _targetPos, _canBreak, _speed, _aiTask);
			pathInfoSingleTarget.SetStartPos(_startPos);
			this.finishedPaths[_entity.entityId] = pathInfoSingleTarget;
		}

		// Token: 0x0600C462 RID: 50274 RVA: 0x0048A145 File Offset: 0x00488345
		public override void FindPath(PathInfo _pathInfo)
		{
			this.entityWaitQueue.Add(_pathInfo.entity.entityId);
			this.finishedPaths[_pathInfo.entity.entityId] = _pathInfo;
		}

		// Token: 0x0600C463 RID: 50275 RVA: 0x0048A174 File Offset: 0x00488374
		public override PathInfo GetPath(int _entityId)
		{
			PathInfo pathInfo;
			if (this.finishedPaths.TryGetValue(_entityId, out pathInfo) && pathInfo.state == PathInfo.State.Done)
			{
				this.finishedPaths.Remove(_entityId);
				return pathInfo;
			}
			return null;
		}

		// Token: 0x0600C464 RID: 50276 RVA: 0x0048A1AA File Offset: 0x004883AA
		public override bool IsCalculatingPath(int _entityId)
		{
			return this.finishedPaths.ContainsKey(_entityId);
		}

		// Token: 0x0600C465 RID: 50277 RVA: 0x0048A1B8 File Offset: 0x004883B8
		public override void RemovePathsFor(int _entityId)
		{
			this.finishedPaths.Remove(_entityId);
			this.entityWaitQueue.Remove(_entityId);
		}

		// Token: 0x040094AD RID: 38061
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine coroutine;

		// Token: 0x040094AE RID: 38062
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetList<int> entityWaitQueue = new HashSetList<int>();

		// Token: 0x040094AF RID: 38063
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, PathInfo> finishedPaths = new Dictionary<int, PathInfo>();
	}
}
