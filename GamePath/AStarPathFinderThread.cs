using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018D9 RID: 6361
	public class AStarPathFinderThread : PathFinderThread
	{
		// Token: 0x0600C443 RID: 50243 RVA: 0x00489056 File Offset: 0x00487256
		public AStarPathFinderThread()
		{
			PathFinderThread.Instance = this;
		}

		// Token: 0x0600C444 RID: 50244 RVA: 0x00489086 File Offset: 0x00487286
		public override int GetFinishedCount()
		{
			return this.finishedPaths.Count;
		}

		// Token: 0x0600C445 RID: 50245 RVA: 0x00489093 File Offset: 0x00487293
		public override int GetQueueCount()
		{
			return this.entityWaitQueue.list.Count;
		}

		// Token: 0x0600C446 RID: 50246 RVA: 0x004890A8 File Offset: 0x004872A8
		public override void StartWorkerThreads()
		{
			this.threadInfo = ThreadManager.StartThread("Pathfinder", null, new ThreadManager.ThreadFunctionLoopDelegate(this.thread_Pathfinder), null, null, null, false, false);
		}

		// Token: 0x0600C447 RID: 50247 RVA: 0x004890D8 File Offset: 0x004872D8
		public override void Cleanup()
		{
			this.threadInfo.RequestTermination();
			this.writerThreadWaitHandle.Set();
			this.threadInfo.WaitForEnd(30);
			this.threadInfo = null;
			this.entityWaitQueue.Clear();
			this.finishedPaths.Clear();
			this.writerThreadWaitHandle = null;
		}

		// Token: 0x0600C448 RID: 50248 RVA: 0x00489130 File Offset: 0x00487330
		[PublicizedFrom(EAccessModifier.Protected)]
		public int thread_Pathfinder(ThreadManager.ThreadInfo _threadInfo)
		{
			while (!_threadInfo.TerminationRequested())
			{
				try
				{
					if (this.entityWaitQueue.list.Count == 0)
					{
						this.writerThreadWaitHandle.WaitOne();
					}
					PathInfo pathInfo = null;
					Dictionary<int, PathInfo> obj = this.finishedPaths;
					lock (obj)
					{
						if (this.entityWaitQueue.list.Count <= 0)
						{
							continue;
						}
						int num = this.entityWaitQueue.list[0];
						this.entityWaitQueue.Remove(num);
						if (!this.finishedPaths.ContainsKey(num))
						{
							continue;
						}
						pathInfo = this.finishedPaths[num];
					}
					pathInfo.entity.navigator.GetPathTo(pathInfo);
					obj = this.finishedPaths;
					lock (obj)
					{
						if (pathInfo.path == null)
						{
							this.finishedPaths.Remove(pathInfo.entity.entityId);
						}
						else
						{
							this.finishedPaths[pathInfo.entity.entityId] = pathInfo;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception in PathFinder thread: " + ex.Message);
					Log.Error(ex.StackTrace);
				}
			}
			return -1;
		}

		// Token: 0x0600C449 RID: 50249 RVA: 0x0048929C File Offset: 0x0048749C
		public override bool IsCalculatingPath(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			bool result;
			lock (obj)
			{
				result = this.finishedPaths.ContainsKey(_entityId);
			}
			return result;
		}

		// Token: 0x0600C44A RID: 50250 RVA: 0x004892E4 File Offset: 0x004874E4
		public override void FindPath(EntityAlive _entity, Vector3 _target, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				if (!this.entityWaitQueue.hashSet.Contains(_entity.entityId))
				{
					this.entityWaitQueue.Add(_entity.entityId);
				}
				this.finishedPaths[_entity.entityId] = new PathInfoSingleTarget(_entity, _target, _canBreak, _speed, _aiTask);
			}
			this.writerThreadWaitHandle.Set();
		}

		// Token: 0x0600C44B RID: 50251 RVA: 0x00489370 File Offset: 0x00487570
		public override PathInfo GetPath(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				PathInfo pathInfo;
				if (this.finishedPaths.TryGetValue(_entityId, out pathInfo) && pathInfo.path != null)
				{
					this.finishedPaths.Remove(_entityId);
					return pathInfo;
				}
			}
			return null;
		}

		// Token: 0x0600C44C RID: 50252 RVA: 0x004893D8 File Offset: 0x004875D8
		public override void RemovePathsFor(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				this.finishedPaths.Remove(_entityId);
				if (this.entityWaitQueue.hashSet.Contains(_entityId))
				{
					this.entityWaitQueue.Remove(_entityId);
				}
			}
		}

		// Token: 0x040094A5 RID: 38053
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.ThreadInfo threadInfo;

		// Token: 0x040094A6 RID: 38054
		[PublicizedFrom(EAccessModifier.Private)]
		public AutoResetEvent writerThreadWaitHandle = new AutoResetEvent(false);

		// Token: 0x040094A7 RID: 38055
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetList<int> entityWaitQueue = new HashSetList<int>();

		// Token: 0x040094A8 RID: 38056
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, PathInfo> finishedPaths = new Dictionary<int, PathInfo>();
	}
}
