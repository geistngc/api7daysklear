using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020018E2 RID: 6370
	public class PathFinderThread
	{
		// Token: 0x0600C488 RID: 50312 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetFinishedCount()
		{
			return 0;
		}

		// Token: 0x0600C489 RID: 50313 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual int GetQueueCount()
		{
			return 0;
		}

		// Token: 0x0600C48A RID: 50314 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void StartWorkerThreads()
		{
		}

		// Token: 0x0600C48B RID: 50315 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Cleanup()
		{
		}

		// Token: 0x0600C48C RID: 50316 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool IsCalculatingPath(int _entityId)
		{
			return false;
		}

		// Token: 0x0600C48D RID: 50317 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void FindPath(EntityAlive _entity, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
		}

		// Token: 0x0600C48E RID: 50318 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void FindPath(EntityAlive _entity, Vector3 _startPos, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
		}

		// Token: 0x0600C48F RID: 50319 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void FindPath(PathInfo _pathInfo)
		{
		}

		// Token: 0x0600C490 RID: 50320 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual PathInfo GetPath(int _entityId)
		{
			return null;
		}

		// Token: 0x0600C491 RID: 50321 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void RemovePathsFor(int _entityId)
		{
		}

		// Token: 0x040094BD RID: 38077
		public static PathFinderThread Instance;
	}
}
