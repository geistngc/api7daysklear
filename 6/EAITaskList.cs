using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200045B RID: 1115
[Preserve]
public class EAITaskList
{
	// Token: 0x060021F2 RID: 8690 RVA: 0x000CD74C File Offset: 0x000CB94C
	public EAITaskList(EAIManager _manager)
	{
		this.allTasks = new List<EAITaskEntry>();
		this.executingTasks = new List<EAITaskEntry>();
		this.executeDelayScale = 0.85f + _manager.random.RandomFloat * 0.25f;
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000CD79D File Offset: 0x000CB99D
	public void AddTask(int _priority, EAIBase _eai)
	{
		this.allTasks.Add(new EAITaskEntry(_priority, _eai));
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000CD7B4 File Offset: 0x000CB9B4
	public void AddTaskList(EAITaskList _taskList)
	{
		for (int i = 0; i < _taskList.Tasks.Count; i++)
		{
			this.allTasks.Add(_taskList.Tasks[i]);
		}
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000CD7EE File Offset: 0x000CB9EE
	public void RemoveTask(EAITaskEntry _entry)
	{
		this.allTasks.Remove(_entry);
		if (this.executingTasks.Contains(_entry))
		{
			this.executingTasks.Remove(_entry);
		}
	}

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x060021F6 RID: 8694 RVA: 0x000CD818 File Offset: 0x000CBA18
	public List<EAITaskEntry> Tasks
	{
		get
		{
			return this.allTasks;
		}
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000CD820 File Offset: 0x000CBA20
	public List<EAITaskEntry> GetExecutingTasks()
	{
		return this.executingTasks;
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000CD828 File Offset: 0x000CBA28
	public T GetTask<T>() where T : class
	{
		for (int i = 0; i < this.allTasks.Count; i++)
		{
			T t = this.allTasks[i].action as T;
			if (t != null)
			{
				return t;
			}
		}
		return default(T);
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000CD87C File Offset: 0x000CBA7C
	public void OnUpdateTasks()
	{
		this.startedTasks.Clear();
		int i = 0;
		while (i < this.allTasks.Count)
		{
			EAITaskEntry eaitaskEntry = this.allTasks[i];
			if (!eaitaskEntry.isExecuting)
			{
				goto IL_7A;
			}
			if (!this.isBestTask(eaitaskEntry) || !eaitaskEntry.action.Continue())
			{
				this.executingTasks.Remove(eaitaskEntry);
				eaitaskEntry.isExecuting = false;
				eaitaskEntry.executeTime = eaitaskEntry.action.executeDelay * this.executeDelayScale;
				eaitaskEntry.action.Reset();
				goto IL_7A;
			}
			IL_10D:
			i++;
			continue;
			IL_7A:
			eaitaskEntry.executeTime -= 0.05f;
			eaitaskEntry.action.executeWaitTime += 0.05f;
			if (eaitaskEntry.executeTime > 0f)
			{
				goto IL_10D;
			}
			eaitaskEntry.executeTime = eaitaskEntry.action.executeDelay * this.executeDelayScale;
			if (this.isBestTask(eaitaskEntry))
			{
				if (eaitaskEntry.action.CanExecute())
				{
					this.startedTasks.Add(eaitaskEntry);
					this.executingTasks.Add(eaitaskEntry);
					eaitaskEntry.isExecuting = true;
				}
				eaitaskEntry.action.executeWaitTime = 0f;
				goto IL_10D;
			}
			goto IL_10D;
		}
		for (int j = 0; j < this.startedTasks.Count; j++)
		{
			this.startedTasks[j].action.Start();
		}
		for (int k = 0; k < this.executingTasks.Count; k++)
		{
			this.executingTasks[k].action.Update();
		}
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000CDA04 File Offset: 0x000CBC04
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBestTask(EAITaskEntry _task)
	{
		int i = 0;
		while (i < this.executingTasks.Count)
		{
			EAITaskEntry eaitaskEntry = this.executingTasks[i++];
			if (eaitaskEntry != _task)
			{
				if (eaitaskEntry.priority > _task.priority)
				{
					if (eaitaskEntry.action.IsContinuous())
					{
						continue;
					}
				}
				else if (this.areTasksCompatible(_task, eaitaskEntry))
				{
					continue;
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000CDA60 File Offset: 0x000CBC60
	[PublicizedFrom(EAccessModifier.Private)]
	public bool areTasksCompatible(EAITaskEntry _task, EAITaskEntry _other)
	{
		return (_task.action.MutexBits & _other.action.MutexBits) == 0;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000CDA7C File Offset: 0x000CBC7C
	public void StopAllTasks()
	{
		this.allTasks.Clear();
		this.executingTasks.Clear();
		this.startedTasks.Clear();
	}

	// Token: 0x04001778 RID: 6008
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> allTasks;

	// Token: 0x04001779 RID: 6009
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> executingTasks;

	// Token: 0x0400177A RID: 6010
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITaskEntry> startedTasks = new List<EAITaskEntry>();

	// Token: 0x0400177B RID: 6011
	[PublicizedFrom(EAccessModifier.Private)]
	public float executeDelayScale;
}
