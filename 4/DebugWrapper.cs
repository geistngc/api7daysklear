using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020013D1 RID: 5073
public abstract class DebugWrapper
{
	// Token: 0x06009F6E RID: 40814 RVA: 0x003C3D6C File Offset: 0x003C1F6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public DebugWrapper(DebugWrapper parent)
	{
		this.m_readCounter = (((parent != null) ? parent.m_readCounter : null) ?? new AtomicCounter());
		this.m_writeCounter = (((parent != null) ? parent.m_writeCounter : null) ?? new AtomicCounter());
		this.DebugNonMainThreadAccess = (parent != null && parent.DebugNonMainThreadAccess);
		this.DebugConcurrentModifications = (parent != null && parent.DebugConcurrentModifications);
	}

	// Token: 0x170012CF RID: 4815
	// (get) Token: 0x06009F6F RID: 40815 RVA: 0x003C3DD9 File Offset: 0x003C1FD9
	// (set) Token: 0x06009F70 RID: 40816 RVA: 0x003C3DE1 File Offset: 0x003C1FE1
	public bool DebugNonMainThreadAccess { get; set; }

	// Token: 0x170012D0 RID: 4816
	// (get) Token: 0x06009F71 RID: 40817 RVA: 0x003C3DEA File Offset: 0x003C1FEA
	// (set) Token: 0x06009F72 RID: 40818 RVA: 0x003C3DF2 File Offset: 0x003C1FF2
	public bool DebugConcurrentModifications { get; set; }

	// Token: 0x170012D1 RID: 4817
	// (get) Token: 0x06009F73 RID: 40819 RVA: 0x003C3DFB File Offset: 0x003C1FFB
	public bool NeedsScope
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.DebugConcurrentModifications;
		}
	}

	// Token: 0x06009F74 RID: 40820 RVA: 0x003C3E03 File Offset: 0x003C2003
	[PublicizedFrom(EAccessModifier.Protected)]
	public IDisposable DebugReadScope()
	{
		if (this.DebugNonMainThreadAccess && !ThreadManager.IsMainThread())
		{
			Log.Exception(new DebugWrapperException("A read is being issued outside of the main thread."));
		}
		if (!this.NeedsScope)
		{
			return DebugWrapper.DummyScope.Instance;
		}
		return new DebugWrapper.ReadScope(this);
	}

	// Token: 0x06009F75 RID: 40821 RVA: 0x003C3E37 File Offset: 0x003C2037
	[PublicizedFrom(EAccessModifier.Protected)]
	public IDisposable DebugReadWriteScope()
	{
		if (this.DebugNonMainThreadAccess && !ThreadManager.IsMainThread())
		{
			Log.Exception(new DebugWrapperException("A read/write is being issued outside of the main thread."));
		}
		if (!this.NeedsScope)
		{
			return DebugWrapper.DummyScope.Instance;
		}
		return new DebugWrapper.ReadWriteScope(this);
	}

	// Token: 0x06009F76 RID: 40822 RVA: 0x003C3E6B File Offset: 0x003C206B
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator<T> DebugEnumerator<T>(IEnumerator<T> enumerator)
	{
		return new DebugWrapper.Enumerator<T>(this.DebugReadScope(), enumerator);
	}

	// Token: 0x06009F77 RID: 40823 RVA: 0x003C3E79 File Offset: 0x003C2079
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator DebugEnumerator(IEnumerator enumerator)
	{
		return new DebugWrapper.Enumerator(this.DebugReadScope(), enumerator);
	}

	// Token: 0x0400791F RID: 31007
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AtomicCounter m_readCounter;

	// Token: 0x04007920 RID: 31008
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AtomicCounter m_writeCounter;

	// Token: 0x020013D2 RID: 5074
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DummyScope : IDisposable
	{
		// Token: 0x06009F78 RID: 40824 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Private)]
		public DummyScope()
		{
		}

		// Token: 0x06009F79 RID: 40825 RVA: 0x000027FC File Offset: 0x000009FC
		public void Dispose()
		{
		}

		// Token: 0x04007923 RID: 31011
		public static readonly DebugWrapper.DummyScope Instance = new DebugWrapper.DummyScope();
	}

	// Token: 0x020013D3 RID: 5075
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class ReadScope : IDisposable
	{
		// Token: 0x06009F7B RID: 40827 RVA: 0x003C3E94 File Offset: 0x003C2094
		public ReadScope(DebugWrapper wrapper)
		{
			this.m_wrapper = wrapper;
			if (this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				Log.Exception(new DebugWrapperException("A read is being issued while there are active writers."));
				this.m_concurrentModificationNotified = true;
			}
			this.m_wrapper.m_readCounter.Increment();
		}

		// Token: 0x06009F7C RID: 40828 RVA: 0x003C3EF8 File Offset: 0x003C20F8
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~ReadScope()
		{
			this.Dispose(false);
		}

		// Token: 0x06009F7D RID: 40829 RVA: 0x003C3F28 File Offset: 0x003C2128
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009F7E RID: 40830 RVA: 0x003C3F38 File Offset: 0x003C2138
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (this.m_disposed)
			{
				return;
			}
			this.m_disposed = true;
			if (!disposing)
			{
				Log.Error("DebugWrapper.ReadScope was not disposed of correctly.");
			}
			this.m_wrapper.m_readCounter.Decrement();
			if (!this.m_concurrentModificationNotified && this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				Log.Exception(new DebugWrapperException("A read was issued while there were active writers."));
			}
		}

		// Token: 0x04007924 RID: 31012
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DebugWrapper m_wrapper;

		// Token: 0x04007925 RID: 31013
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_concurrentModificationNotified;

		// Token: 0x04007926 RID: 31014
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;
	}

	// Token: 0x020013D4 RID: 5076
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class ReadWriteScope : IDisposable
	{
		// Token: 0x06009F7F RID: 40831 RVA: 0x003C3FAC File Offset: 0x003C21AC
		public ReadWriteScope(DebugWrapper wrapper)
		{
			this.m_wrapper = wrapper;
			if (this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				if (this.m_wrapper.m_writeCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write is being issued while there are active writers."));
				}
				else if (this.m_wrapper.m_readCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write is being issued while there are active readers."));
				}
				this.m_concurrentModificationNotified = true;
			}
			this.m_wrapper.m_readCounter.Increment();
			this.m_wrapper.m_writeCounter.Increment();
		}

		// Token: 0x06009F80 RID: 40832 RVA: 0x003C4058 File Offset: 0x003C2258
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~ReadWriteScope()
		{
			this.Dispose(false);
		}

		// Token: 0x06009F81 RID: 40833 RVA: 0x003C4088 File Offset: 0x003C2288
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009F82 RID: 40834 RVA: 0x003C4098 File Offset: 0x003C2298
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (this.m_disposed)
			{
				return;
			}
			this.m_disposed = true;
			if (!disposing)
			{
				Log.Error("DebugWrapper.ReadWriteScope was not disposed of correctly.");
			}
			this.m_wrapper.m_writeCounter.Decrement();
			this.m_wrapper.m_readCounter.Decrement();
			if (!this.m_concurrentModificationNotified && this.m_wrapper.DebugConcurrentModifications)
			{
				if (this.m_wrapper.m_writeCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write was issued while there were active writers."));
					return;
				}
				if (this.m_wrapper.m_readCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write was issued while there are active readers."));
				}
			}
		}

		// Token: 0x04007927 RID: 31015
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DebugWrapper m_wrapper;

		// Token: 0x04007928 RID: 31016
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_concurrentModificationNotified;

		// Token: 0x04007929 RID: 31017
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;
	}

	// Token: 0x020013D5 RID: 5077
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class Enumerator : IEnumerator, IDisposable
	{
		// Token: 0x06009F83 RID: 40835 RVA: 0x003C413E File Offset: 0x003C233E
		public Enumerator(IDisposable scope, IEnumerator enumerator)
		{
			this.m_scope = scope;
			this.m_enumerator = enumerator;
		}

		// Token: 0x06009F84 RID: 40836 RVA: 0x003C4154 File Offset: 0x003C2354
		public void Dispose()
		{
			this.m_scope.Dispose();
			IDisposable disposable = this.m_enumerator as IDisposable;
			if (disposable == null)
			{
				return;
			}
			disposable.Dispose();
		}

		// Token: 0x06009F85 RID: 40837 RVA: 0x003C4176 File Offset: 0x003C2376
		public bool MoveNext()
		{
			return this.m_enumerator.MoveNext();
		}

		// Token: 0x06009F86 RID: 40838 RVA: 0x003C4183 File Offset: 0x003C2383
		public void Reset()
		{
			this.m_enumerator.Reset();
		}

		// Token: 0x170012D2 RID: 4818
		// (get) Token: 0x06009F87 RID: 40839 RVA: 0x003C4190 File Offset: 0x003C2390
		public object Current
		{
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x0400792A RID: 31018
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IDisposable m_scope;

		// Token: 0x0400792B RID: 31019
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEnumerator m_enumerator;
	}

	// Token: 0x020013D6 RID: 5078
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class Enumerator<T> : IEnumerator<!0>, IEnumerator, IDisposable
	{
		// Token: 0x06009F88 RID: 40840 RVA: 0x003C419D File Offset: 0x003C239D
		public Enumerator(IDisposable scope, IEnumerator<T> enumerator)
		{
			this.m_scope = scope;
			this.m_enumerator = enumerator;
		}

		// Token: 0x06009F89 RID: 40841 RVA: 0x003C41B3 File Offset: 0x003C23B3
		public void Dispose()
		{
			this.m_scope.Dispose();
			this.m_enumerator.Dispose();
		}

		// Token: 0x06009F8A RID: 40842 RVA: 0x003C41CB File Offset: 0x003C23CB
		public bool MoveNext()
		{
			return this.m_enumerator.MoveNext();
		}

		// Token: 0x06009F8B RID: 40843 RVA: 0x003C41D8 File Offset: 0x003C23D8
		public void Reset()
		{
			this.m_enumerator.Reset();
		}

		// Token: 0x170012D3 RID: 4819
		// (get) Token: 0x06009F8C RID: 40844 RVA: 0x003C41E5 File Offset: 0x003C23E5
		public T Current
		{
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x170012D4 RID: 4820
		// (get) Token: 0x06009F8D RID: 40845 RVA: 0x003C41F2 File Offset: 0x003C23F2
		public object Current
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x0400792C RID: 31020
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IDisposable m_scope;

		// Token: 0x0400792D RID: 31021
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEnumerator<T> m_enumerator;
	}
}
