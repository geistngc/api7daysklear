using System;
using System.Collections.Generic;
using System.Threading;

// Token: 0x020013AF RID: 5039
public class BlockingQueue<T>
{
	// Token: 0x06009EC3 RID: 40643 RVA: 0x003C08F0 File Offset: 0x003BEAF0
	public void Enqueue(T item)
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.queue.Enqueue(item);
			Monitor.PulseAll(this.queue);
		}
	}

	// Token: 0x06009EC4 RID: 40644 RVA: 0x003C0944 File Offset: 0x003BEB44
	public T Dequeue()
	{
		Queue<T> obj = this.queue;
		T result;
		lock (obj)
		{
			while (this.queue.Count == 0)
			{
				if (this.closing)
				{
					result = default(T);
					return result;
				}
				Monitor.Wait(this.queue);
			}
			result = this.queue.Dequeue();
		}
		return result;
	}

	// Token: 0x06009EC5 RID: 40645 RVA: 0x003C09BC File Offset: 0x003BEBBC
	public bool HasData()
	{
		Queue<T> obj = this.queue;
		bool result;
		lock (obj)
		{
			result = (this.queue.Count > 0);
		}
		return result;
	}

	// Token: 0x06009EC6 RID: 40646 RVA: 0x003C0A08 File Offset: 0x003BEC08
	public void Close()
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.closing = true;
			Monitor.PulseAll(this.queue);
		}
	}

	// Token: 0x06009EC7 RID: 40647 RVA: 0x003C0A54 File Offset: 0x003BEC54
	public void Clear()
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.queue.Clear();
		}
	}

	// Token: 0x0400789F RID: 30879
	[PublicizedFrom(EAccessModifier.Private)]
	public bool closing;

	// Token: 0x040078A0 RID: 30880
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Queue<T> queue = new Queue<T>();
}
