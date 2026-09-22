using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

// Token: 0x02001390 RID: 5008
public sealed class BackedArraySingleView<[IsUnmanaged] T> : IBackedArrayView<T>, IDisposable where T : struct, ValueType
{
	// Token: 0x06009DE2 RID: 40418 RVA: 0x003BC7B4 File Offset: 0x003BA9B4
	[PublicizedFrom(EAccessModifier.Private)]
	public BackedArraySingleView<T>.View CreateView()
	{
		BackedArraySingleView<T>.View result = new BackedArraySingleView<T>.View(this);
		int num = Interlocked.Increment(ref this.m_viewCount);
		if (num > 16)
		{
			Log.Warning(string.Format("{0}<T> has opened a large amount of array views, this could indicate a memory issue. Count: {1}, View Length: {2}", "BackedArraySingleView", num, this.m_viewLength));
		}
		return result;
	}

	// Token: 0x06009DE3 RID: 40419 RVA: 0x003BC800 File Offset: 0x003BAA00
	public BackedArraySingleView(IBackedArray<T> array, BackedArrayHandleMode mode, int viewLength = 0)
	{
		this.m_array = array;
		this.m_length = array.Length;
		this.m_mode = mode;
		if (!mode.CanRead())
		{
			throw new ArgumentException("Expected a readable mode.", "mode");
		}
		if (viewLength <= 0)
		{
			viewLength = BackedArraySingleView<T>.GetDefaultViewLength(array.Length);
		}
		this.m_viewLength = Math.Min(this.m_length, viewLength);
		this.m_views = new ThreadLocal<BackedArraySingleView<T>.View>(new Func<BackedArraySingleView<T>.View>(this.CreateView), true);
	}

	// Token: 0x06009DE4 RID: 40420 RVA: 0x003BC880 File Offset: 0x003BAA80
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose(bool disposing)
	{
		if (!disposing)
		{
			Log.Error("BackedArraySingleView<T> is being finalized, it should be disposed properly.");
			return;
		}
		if (this.IsDisposed())
		{
			return;
		}
		foreach (BackedArraySingleView<T>.View view in this.m_views.Values)
		{
			view.Dispose();
		}
		this.m_views.Dispose();
		this.m_views = null;
		this.m_array = null;
	}

	// Token: 0x06009DE5 RID: 40421 RVA: 0x003BC900 File Offset: 0x003BAB00
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x06009DE6 RID: 40422 RVA: 0x003BC910 File Offset: 0x003BAB10
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~BackedArraySingleView()
	{
		this.Dispose(false);
	}

	// Token: 0x06009DE7 RID: 40423 RVA: 0x003BC940 File Offset: 0x003BAB40
	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThrowIfDisposed()
	{
		if (this.IsDisposed())
		{
			throw new ObjectDisposedException("BackedArraySingleView has already been disposed.");
		}
	}

	// Token: 0x06009DE8 RID: 40424 RVA: 0x003BC955 File Offset: 0x003BAB55
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsDisposed()
	{
		return this.m_array == null;
	}

	// Token: 0x06009DE9 RID: 40425 RVA: 0x003BC960 File Offset: 0x003BAB60
	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThrowIfCannotWrite()
	{
		if (!this.m_mode.CanWrite())
		{
			throw new NotSupportedException("This BackedArraySingleView is not writable.");
		}
	}

	// Token: 0x170012A9 RID: 4777
	// (get) Token: 0x06009DEA RID: 40426 RVA: 0x003BC97A File Offset: 0x003BAB7A
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x170012AA RID: 4778
	// (get) Token: 0x06009DEB RID: 40427 RVA: 0x003BC982 File Offset: 0x003BAB82
	public BackedArrayHandleMode Mode
	{
		get
		{
			return this.m_mode;
		}
	}

	// Token: 0x170012AB RID: 4779
	public T this[int i]
	{
		get
		{
			return this.m_views.Value[i];
		}
		set
		{
			this.m_views.Value[i] = value;
		}
	}

	// Token: 0x06009DEE RID: 40430 RVA: 0x003BC9B4 File Offset: 0x003BABB4
	public void Flush()
	{
		foreach (BackedArraySingleView<T>.View view in this.m_views.Values)
		{
			view.m_handle.Flush();
		}
	}

	// Token: 0x06009DEF RID: 40431 RVA: 0x003BCA08 File Offset: 0x003BAC08
	public static int GetDefaultViewLength(int length)
	{
		return Mathf.NextPowerOfTwo(4 * (int)Math.Sqrt((double)length));
	}

	// Token: 0x04007826 RID: 30758
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArray<T> m_array;

	// Token: 0x04007827 RID: 30759
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_length;

	// Token: 0x04007828 RID: 30760
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly BackedArrayHandleMode m_mode;

	// Token: 0x04007829 RID: 30761
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_viewLength;

	// Token: 0x0400782A RID: 30762
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadLocal<BackedArraySingleView<T>.View> m_views;

	// Token: 0x0400782B RID: 30763
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_viewCount;

	// Token: 0x0400782C RID: 30764
	[PublicizedFrom(EAccessModifier.Private)]
	public const int VIEW_COUNT_WARNING_THRESHOLD = 16;

	// Token: 0x02001391 RID: 5009
	[PublicizedFrom(EAccessModifier.Private)]
	public class View : IDisposable
	{
		// Token: 0x06009DF0 RID: 40432 RVA: 0x003BCA19 File Offset: 0x003BAC19
		public View(BackedArraySingleView<T> backedArraySingleView)
		{
			this.m_backedArraySingleView = backedArraySingleView;
		}

		// Token: 0x06009DF1 RID: 40433 RVA: 0x003BCA34 File Offset: 0x003BAC34
		public void Cache(int offset)
		{
			if (this.m_start <= offset && offset < this.m_end)
			{
				return;
			}
			IBackedArray<T> array = this.m_backedArraySingleView.m_array;
			int length = this.m_backedArraySingleView.m_length;
			int viewLength = this.m_backedArraySingleView.m_viewLength;
			BackedArrayHandleMode mode = this.m_backedArraySingleView.m_mode;
			if (offset < 0 || offset >= length)
			{
				throw new IndexOutOfRangeException(string.Format("Expected index {0} to be in the range [0, {1}).", offset, length));
			}
			if (offset + viewLength > length)
			{
				offset = length - viewLength;
			}
			IBackedArrayHandle handle = this.m_handle;
			if (handle != null)
			{
				handle.Dispose();
			}
			IBackedArrayHandle handle2;
			if (mode != BackedArrayHandleMode.ReadOnly)
			{
				if (mode != BackedArrayHandleMode.ReadWrite)
				{
					throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
				}
				handle2 = array.GetMemoryUnsafe(offset, viewLength, out this.m_ptr);
			}
			else
			{
				handle2 = array.GetReadOnlyMemoryUnsafe(offset, viewLength, out this.m_ptr);
			}
			this.m_handle = handle2;
			this.m_start = offset;
			this.m_end = offset + viewLength;
		}

		// Token: 0x170012AC RID: 4780
		public unsafe T this[int i]
		{
			get
			{
				this.Cache(i);
				return this.m_ptr[(IntPtr)(i - this.m_start) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
			set
			{
				this.Cache(i);
				this.m_ptr[(IntPtr)(i - this.m_start) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			}
		}

		// Token: 0x06009DF4 RID: 40436 RVA: 0x003BCB78 File Offset: 0x003BAD78
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~View()
		{
			this.Dispose(false);
		}

		// Token: 0x06009DF5 RID: 40437 RVA: 0x003BCBA8 File Offset: 0x003BADA8
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009DF6 RID: 40438 RVA: 0x003BCBB8 File Offset: 0x003BADB8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool isDisposing)
		{
			object disposeLock = this.m_disposeLock;
			lock (disposeLock)
			{
				if (!isDisposing)
				{
					Log.Warning("View<T> is being finalized, it should be disposed properly.");
				}
				if (this.m_handle != null)
				{
					this.m_handle.Dispose();
					this.m_handle = null;
					this.m_ptr = null;
					this.m_start = 0;
					this.m_end = 0;
					Interlocked.Decrement(ref this.m_backedArraySingleView.m_viewCount);
				}
			}
		}

		// Token: 0x0400782D RID: 30765
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BackedArraySingleView<T> m_backedArraySingleView;

		// Token: 0x0400782E RID: 30766
		public IBackedArrayHandle m_handle;

		// Token: 0x0400782F RID: 30767
		public unsafe T* m_ptr;

		// Token: 0x04007830 RID: 30768
		public int m_start;

		// Token: 0x04007831 RID: 30769
		public int m_end;

		// Token: 0x04007832 RID: 30770
		[PublicizedFrom(EAccessModifier.Private)]
		public object m_disposeLock = new object();
	}
}
