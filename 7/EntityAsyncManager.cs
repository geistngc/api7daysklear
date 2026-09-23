using System;
using System.Collections.Generic;
using System.Diagnostics;

// Token: 0x02000487 RID: 1159
public class EntityAsyncManager
{
	// Token: 0x060023F8 RID: 9208 RVA: 0x000DA1D7 File Offset: 0x000D83D7
	public EntityAsyncManager(NetEntityPackageQueue netEntityPackageQueue)
	{
		this.netEntityPackageQueue = netEntityPackageQueue;
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000DA200 File Offset: 0x000D8400
	public EntityAsyncManager.EntityCreateHandle StartCreateEntity(EntityCreationData _ecd, Action<EntityAsyncManager.EntityCreateHandle> _onComplete = null)
	{
		EntityFactory.CreateEntityOperation op = EntityFactory.CreateEntityAsync(_ecd);
		EntityAsyncManager.EntityCreateHandle entityCreateHandle = new EntityAsyncManager.EntityCreateHandle(this, op, _onComplete);
		if (!this.requestIdMap.TryAdd(entityCreateHandle.Id, entityCreateHandle))
		{
			throw new Exception(string.Format("Request already exists for entity id {0}. EntityCreationData: {1}", entityCreateHandle.Id, _ecd));
		}
		this.requestQueue.Enqueue(entityCreateHandle);
		return entityCreateHandle;
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000DA25C File Offset: 0x000D845C
	public void Update()
	{
		EntityAsyncManager.EntityCreateHandle entityCreateHandle;
		while (this.requestQueue.TryPeek(out entityCreateHandle))
		{
			if (entityCreateHandle.IsCompleted)
			{
				this.requestQueue.Dequeue();
			}
			else
			{
				if (!entityCreateHandle.TryComplete())
				{
					break;
				}
				this.requestQueue.Dequeue();
			}
		}
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000DA2A4 File Offset: 0x000D84A4
	public bool IsEntityPending(int _entityId)
	{
		EntityAsyncManager.EntityCreateHandle entityCreateHandle;
		return this.requestIdMap.TryGetValue(_entityId, out entityCreateHandle) && !entityCreateHandle.IsCompleted;
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000DA2CC File Offset: 0x000D84CC
	public void EnsureEntity(int _entityId)
	{
		EntityAsyncManager.EntityCreateHandle entityCreateHandle;
		if (!this.requestIdMap.TryGetValue(_entityId, out entityCreateHandle))
		{
			return;
		}
		EntityAsyncManager.LogWarning("force spawning pending entity " + entityCreateHandle.DebugInfo);
		entityCreateHandle.WaitForComplete();
		this.requestIdMap.Remove(entityCreateHandle.Id);
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000DA318 File Offset: 0x000D8518
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnCreateEntityRequestFinalized(int id)
	{
		this.requestIdMap.Remove(id);
		this.netEntityPackageQueue.ProcessPackagesForEntity(id);
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000DA334 File Offset: 0x000D8534
	public void CompletePendingCreateTasks()
	{
		if (this.requestQueue.Count == 0)
		{
			return;
		}
		EntityAsyncManager.LogInfo(string.Format("completing {0} pending create tasks", this.requestQueue.Count));
		EntityAsyncManager.EntityCreateHandle entityCreateHandle;
		while (this.requestQueue.TryDequeue(out entityCreateHandle))
		{
			entityCreateHandle.WaitForComplete();
		}
		this.requestIdMap.Clear();
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000DA391 File Offset: 0x000D8591
	public void Cleanup()
	{
		this.CompletePendingCreateTasks();
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000DA399 File Offset: 0x000D8599
	[Conditional("DEBUG_ENTITY_ASYNC")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LogDebug(string message)
	{
		Log.Out("[EntityAsync] " + message);
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000DA399 File Offset: 0x000D8599
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LogInfo(string message)
	{
		Log.Out("[EntityAsync] " + message);
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000DA3AB File Offset: 0x000D85AB
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LogWarning(string message)
	{
		Log.Warning("[EntityAsync] " + message);
	}

	// Token: 0x0400198B RID: 6539
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<EntityAsyncManager.EntityCreateHandle> requestQueue = new Queue<EntityAsyncManager.EntityCreateHandle>(64);

	// Token: 0x0400198C RID: 6540
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, EntityAsyncManager.EntityCreateHandle> requestIdMap = new Dictionary<int, EntityAsyncManager.EntityCreateHandle>(64);

	// Token: 0x0400198D RID: 6541
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly NetEntityPackageQueue netEntityPackageQueue;

	// Token: 0x02000488 RID: 1160
	public class EntityCreateHandle
	{
		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06002403 RID: 9219 RVA: 0x000DA3BD File Offset: 0x000D85BD
		// (set) Token: 0x06002404 RID: 9220 RVA: 0x000DA3C5 File Offset: 0x000D85C5
		public bool IsCompleted { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06002405 RID: 9221 RVA: 0x000DA3CE File Offset: 0x000D85CE
		public int Id
		{
			get
			{
				return this.op.EntityId;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06002406 RID: 9222 RVA: 0x000DA3DB File Offset: 0x000D85DB
		public string DebugInfo
		{
			get
			{
				return this.op.DebugEntityInfo;
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06002407 RID: 9223 RVA: 0x000DA3E8 File Offset: 0x000D85E8
		public Entity Entity
		{
			get
			{
				return this.op.entity;
			}
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x000DA3F5 File Offset: 0x000D85F5
		public EntityCreateHandle(EntityAsyncManager manager, EntityFactory.CreateEntityOperation op, Action<EntityAsyncManager.EntityCreateHandle> onComplete)
		{
			this.manager = manager;
			this.op = op;
			this.onComplete = onComplete;
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x000DA414 File Offset: 0x000D8614
		public bool TryComplete()
		{
			if (this.IsCompleted)
			{
				return true;
			}
			if (!this.op.IsLoadingComplete)
			{
				return false;
			}
			try
			{
				this.IsCompleted = true;
				this.op.CompleteEntity();
				Action<EntityAsyncManager.EntityCreateHandle> action = this.onComplete;
				if (action != null)
				{
					action(this);
				}
				this.onComplete = null;
			}
			finally
			{
				this.manager.OnCreateEntityRequestFinalized(this.Id);
			}
			return true;
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x000DA48C File Offset: 0x000D868C
		public Entity WaitForComplete()
		{
			this.op.WaitForLoadingComplete();
			if (!this.TryComplete())
			{
				return null;
			}
			return this.op.entity;
		}

		// Token: 0x0400198E RID: 6542
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EntityAsyncManager manager;

		// Token: 0x0400198F RID: 6543
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EntityFactory.CreateEntityOperation op;

		// Token: 0x04001990 RID: 6544
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<EntityAsyncManager.EntityCreateHandle> onComplete;
	}
}
