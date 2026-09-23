using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Platform;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02001392 RID: 5010
public sealed class FileBackedArray<[IsUnmanaged] T> : IBackedArray<T>, IDisposable where T : struct, ValueType
{
	// Token: 0x06009DF7 RID: 40439 RVA: 0x003BCC44 File Offset: 0x003BAE44
	public FileBackedArray(int length)
	{
		if (length <= 0)
		{
			throw new ArgumentOutOfRangeException("length", length, "Length should be positive.");
		}
		this.m_length = length;
		this.m_valueSize = UnsafeUtility.SizeOf(typeof(T));
		this.m_filePath = PlatformManager.NativePlatform.Utils.GetTempFileName("fba", ".fba");
		this.m_fileStream = new FileStream(this.m_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
		this.m_fileStream.Seek((long)(length * this.m_valueSize - 1), SeekOrigin.Begin);
		this.m_fileStream.WriteByte(0);
		this.m_fileStream.Flush();
		this.m_fileStreams = new ThreadLocal<FileStream>(new Func<FileStream>(this.CreateFileStream), true);
	}

	// Token: 0x06009DF8 RID: 40440 RVA: 0x003BCD1C File Offset: 0x003BAF1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose(bool disposing)
	{
		if (!disposing)
		{
			Log.Error("FileBackedArray<T> is being finalized, it should be disposed properly.");
			return;
		}
		object fileStreamsLock = this.m_fileStreamsLock;
		lock (fileStreamsLock)
		{
			if (this.m_fileStreams != null)
			{
				foreach (FileStream fileStream in this.m_fileStreams.Values)
				{
					fileStream.Dispose();
				}
				this.m_fileStreams.Dispose();
				this.m_fileStreams = null;
			}
			if (this.m_fileStream != null)
			{
				this.m_fileStream.Dispose();
				this.m_fileStream = null;
			}
		}
		try
		{
			File.Delete(this.m_filePath);
		}
		catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
		{
			Log.Warning("FileBackedArray<T> Failed to delete: " + this.m_filePath);
		}
	}

	// Token: 0x06009DF9 RID: 40441 RVA: 0x003BCE30 File Offset: 0x003BB030
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x06009DFA RID: 40442 RVA: 0x003BCE40 File Offset: 0x003BB040
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~FileBackedArray()
	{
		this.Dispose(false);
	}

	// Token: 0x06009DFB RID: 40443 RVA: 0x003BCE70 File Offset: 0x003BB070
	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThrowIfDisposed()
	{
		if (this.m_fileStream == null)
		{
			throw new ObjectDisposedException("FileBackedArray has already been disposed.");
		}
	}

	// Token: 0x06009DFC RID: 40444 RVA: 0x003BCE88 File Offset: 0x003BB088
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckBounds(int start, int length)
	{
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("Expected length to be non-negative but was {0}.", length));
		}
		if (start < 0 || start + length > this.m_length)
		{
			throw new ArgumentOutOfRangeException(string.Format("Expected requested range [{0}, {1}) to be a subset of [0, {2}).", start, start + length, this.m_length));
		}
	}

	// Token: 0x06009DFD RID: 40445 RVA: 0x003BCEE8 File Offset: 0x003BB0E8
	[PublicizedFrom(EAccessModifier.Private)]
	public FileStream CreateFileStream()
	{
		object fileStreamsLock = this.m_fileStreamsLock;
		FileStream result;
		lock (fileStreamsLock)
		{
			result = new FileStream(this.m_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read | FileShare.Write | FileShare.Delete, 4096);
		}
		return result;
	}

	// Token: 0x06009DFE RID: 40446 RVA: 0x003BCF38 File Offset: 0x003BB138
	[PublicizedFrom(EAccessModifier.Private)]
	public FileStream GetFileStream()
	{
		object fileStreamsLock = this.m_fileStreamsLock;
		FileStream value;
		lock (fileStreamsLock)
		{
			value = this.m_fileStreams.Value;
		}
		return value;
	}

	// Token: 0x170012AD RID: 4781
	// (get) Token: 0x06009DFF RID: 40447 RVA: 0x003BCF80 File Offset: 0x003BB180
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x06009E00 RID: 40448 RVA: 0x003BCF88 File Offset: 0x003BB188
	[PublicizedFrom(EAccessModifier.Private)]
	public FileBackedArray<T>.FileBackedArrayHandle GetHandle(int start, int length, BackedArrayHandleMode mode)
	{
		this.CheckBounds(start, length);
		return new FileBackedArray<T>.FileBackedArrayHandle(this, this.m_valueSize, start, length, mode);
	}

	// Token: 0x06009E01 RID: 40449 RVA: 0x003BCFA4 File Offset: 0x003BB1A4
	public IBackedArrayHandle GetMemory(int start, int length, out Memory<T> memory)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadWrite);
		memory = handle.GetMemory();
		return handle;
	}

	// Token: 0x06009E02 RID: 40450 RVA: 0x003BCFC8 File Offset: 0x003BB1C8
	public IBackedArrayHandle GetReadOnlyMemory(int start, int length, out ReadOnlyMemory<T> memory)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadOnly);
		memory = handle.GetReadOnlyMemory();
		return handle;
	}

	// Token: 0x06009E03 RID: 40451 RVA: 0x003BCFEC File Offset: 0x003BB1EC
	public unsafe IBackedArrayHandle GetMemoryUnsafe(int start, int length, out T* arrayPtr)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadWrite);
		arrayPtr = handle.GetPtr();
		return handle;
	}

	// Token: 0x06009E04 RID: 40452 RVA: 0x003BD00C File Offset: 0x003BB20C
	public unsafe IBackedArrayHandle GetReadOnlyMemoryUnsafe(int start, int length, out T* arrayPtr)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadOnly);
		arrayPtr = handle.GetPtr();
		return handle;
	}

	// Token: 0x06009E05 RID: 40453 RVA: 0x003BD02C File Offset: 0x003BB22C
	public IBackedArrayHandle GetSpan(int start, int length, out Span<T> span)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadWrite);
		span = handle.GetSpan();
		return handle;
	}

	// Token: 0x06009E06 RID: 40454 RVA: 0x003BD050 File Offset: 0x003BB250
	public IBackedArrayHandle GetReadOnlySpan(int start, int length, out ReadOnlySpan<T> span)
	{
		FileBackedArray<T>.FileBackedArrayHandle handle = this.GetHandle(start, length, BackedArrayHandleMode.ReadOnly);
		span = handle.GetReadOnlySpan();
		return handle;
	}

	// Token: 0x170012AE RID: 4782
	// (get) Token: 0x06009E07 RID: 40455 RVA: 0x003BD074 File Offset: 0x003BB274
	// (set) Token: 0x06009E08 RID: 40456 RVA: 0x003BD07C File Offset: 0x003BB27C
	public FileBackedArray<T>.OnWrittenHandler OnWritten { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x04007833 RID: 30771
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FILE_STREAM_BUFFER_SIZE = 4096;

	// Token: 0x04007834 RID: 30772
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_length;

	// Token: 0x04007835 RID: 30773
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_valueSize;

	// Token: 0x04007836 RID: 30774
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string m_filePath;

	// Token: 0x04007837 RID: 30775
	[PublicizedFrom(EAccessModifier.Private)]
	public FileStream m_fileStream;

	// Token: 0x04007838 RID: 30776
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object m_fileStreamsLock = new object();

	// Token: 0x04007839 RID: 30777
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadLocal<FileStream> m_fileStreams;

	// Token: 0x02001393 RID: 5011
	// (Invoke) Token: 0x06009E0A RID: 40458
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void OnWrittenHandler(int start, ReadOnlySpan<T> span);

	// Token: 0x02001394 RID: 5012
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class FileBackedArrayMemoryManager : MemoryManager<T>
	{
		// Token: 0x06009E0D RID: 40461 RVA: 0x003BD085 File Offset: 0x003BB285
		public unsafe FileBackedArrayMemoryManager(T* ptr, int length, int valueSize)
		{
			this.m_ptr = ptr;
			this.m_length = length;
			this.m_valueSize = valueSize;
		}

		// Token: 0x06009E0E RID: 40462 RVA: 0x003BD0A2 File Offset: 0x003BB2A2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void Dispose(bool disposing)
		{
			this.m_ptr = null;
			this.m_length = 0;
			this.m_valueSize = 0;
		}

		// Token: 0x06009E0F RID: 40463 RVA: 0x003BD0BA File Offset: 0x003BB2BA
		public unsafe override Span<T> GetSpan()
		{
			return new Span<T>((void*)this.m_ptr, this.m_length);
		}

		// Token: 0x06009E10 RID: 40464 RVA: 0x003BD0D0 File Offset: 0x003BB2D0
		public unsafe override MemoryHandle Pin(int elementIndex = 0)
		{
			return new MemoryHandle((void*)(this.m_ptr + (IntPtr)(elementIndex * this.m_valueSize) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), default(GCHandle), null);
		}

		// Token: 0x06009E11 RID: 40465 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Unpin()
		{
		}

		// Token: 0x0400783B RID: 30779
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe T* m_ptr;

		// Token: 0x0400783C RID: 30780
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_length;

		// Token: 0x0400783D RID: 30781
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_valueSize;
	}

	// Token: 0x02001395 RID: 5013
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class FileBackedArrayHandle : IBackedArrayHandle, IDisposable
	{
		// Token: 0x06009E12 RID: 40466 RVA: 0x003BD104 File Offset: 0x003BB304
		public unsafe FileBackedArrayHandle(FileBackedArray<T> array, int valueSize, int start, int length, BackedArrayHandleMode mode)
		{
			this.m_array = array;
			this.m_valueSize = valueSize;
			this.m_start = start;
			this.m_length = length;
			this.m_mode = mode;
			this.m_buffer = new NativeArray<T>(this.m_length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			BackedArrayHandleMode mode2 = this.m_mode;
			void* ptr;
			if (mode2 != BackedArrayHandleMode.ReadOnly)
			{
				if (mode2 != BackedArrayHandleMode.ReadWrite)
				{
					throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
				}
				ptr = this.m_buffer.GetUnsafePtr<T>();
			}
			else
			{
				ptr = this.m_buffer.GetUnsafeReadOnlyPtr<T>();
			}
			this.m_ptr = (T*)ptr;
			byte* ptr2 = (byte*)this.m_ptr;
			int num = this.m_length * this.m_valueSize;
			int i = 0;
			int num2 = this.m_start * this.m_valueSize;
			FileStream fileStream = this.m_array.GetFileStream();
			fileStream.Seek((long)num2, SeekOrigin.Begin);
			while (i < num)
			{
				int num3 = fileStream.Read(new Span<byte>((void*)(ptr2 + i), num - i));
				if (num3 <= 0)
				{
					this.m_buffer.Dispose();
					this.m_buffer = default(NativeArray<T>);
					throw new IOException(string.Format("Unexpected end of file (read {0} but expected {1} after offset {2}).", i, num, num2));
				}
				i += num3;
			}
			FileBackedArray<T> array2 = this.m_array;
			array2.OnWritten = (FileBackedArray<T>.OnWrittenHandler)Delegate.Combine(array2.OnWritten, new FileBackedArray<T>.OnWrittenHandler(this.OnWritten));
		}

		// Token: 0x06009E13 RID: 40467 RVA: 0x003BD26C File Offset: 0x003BB46C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (!disposing)
			{
				Log.Error("FileBackedArrayHandle is being finalized, it should be disposed properly.");
				return;
			}
			if (this.m_array != null)
			{
				FileBackedArray<T> array = this.m_array;
				array.OnWritten = (FileBackedArray<T>.OnWrittenHandler)Delegate.Remove(array.OnWritten, new FileBackedArray<T>.OnWrittenHandler(this.OnWritten));
				if (this.m_mode.CanWrite())
				{
					try
					{
						this.FlushInternal();
					}
					catch (Exception e)
					{
						Log.Error("Failed to write potential changes back to the FileBackedArray file stream.");
						Log.Exception(e);
					}
				}
			}
			if (this.m_memoryOwner != null)
			{
				this.m_memoryOwner.Dispose();
				this.m_memoryOwner = null;
			}
			if (this.m_buffer != default(NativeArray<T>))
			{
				this.m_buffer.Dispose();
				this.m_buffer = default(NativeArray<T>);
			}
			this.m_start = 0;
			this.m_length = 0;
			this.m_ptr = null;
			this.m_array = null;
		}

		// Token: 0x06009E14 RID: 40468 RVA: 0x003BD350 File Offset: 0x003BB550
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009E15 RID: 40469 RVA: 0x003BD360 File Offset: 0x003BB560
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~FileBackedArrayHandle()
		{
			this.Dispose(false);
		}

		// Token: 0x06009E16 RID: 40470 RVA: 0x003BD390 File Offset: 0x003BB590
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfDisposed()
		{
			if (this.IsDisposed())
			{
				throw new ObjectDisposedException("FileBackedArrayHandle has already been disposed.");
			}
		}

		// Token: 0x06009E17 RID: 40471 RVA: 0x003BD3A5 File Offset: 0x003BB5A5
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsDisposed()
		{
			return this.m_array == null;
		}

		// Token: 0x06009E18 RID: 40472 RVA: 0x003BD3B0 File Offset: 0x003BB5B0
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfCannotWrite()
		{
			if (!this.m_mode.CanWrite())
			{
				throw new NotSupportedException("This FileBackedArrayHandle is not writable.");
			}
		}

		// Token: 0x06009E19 RID: 40473 RVA: 0x003BD3CC File Offset: 0x003BB5CC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnWritten(int start, ReadOnlySpan<T> span)
		{
			int num = Math.Max(start, this.m_start);
			int num2 = Math.Min(start + span.Length, this.m_start + this.m_length) - num;
			if (num2 <= 0)
			{
				return;
			}
			ReadOnlySpan<T> readOnlySpan = span.Slice(num - start, num2);
			Span<T> destination = this.GetSpan().Slice(num - this.m_start, num2);
			readOnlySpan.CopyTo(destination);
		}

		// Token: 0x170012AF RID: 4783
		// (get) Token: 0x06009E1A RID: 40474 RVA: 0x003BD436 File Offset: 0x003BB636
		public BackedArrayHandleMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		// Token: 0x06009E1B RID: 40475 RVA: 0x003BD440 File Offset: 0x003BB640
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe void FlushInternal()
		{
			FileStream fileStream = this.m_array.GetFileStream();
			ReadOnlySpan<T> span = new ReadOnlySpan<T>((void*)this.m_ptr, this.m_length);
			fileStream.Seek((long)(this.m_start * this.m_valueSize), SeekOrigin.Begin);
			fileStream.Write(MemoryMarshal.Cast<T, byte>(span));
			fileStream.Flush();
			FileBackedArray<T>.OnWrittenHandler onWritten = this.m_array.OnWritten;
			if (onWritten == null)
			{
				return;
			}
			onWritten(this.m_start, span);
		}

		// Token: 0x06009E1C RID: 40476 RVA: 0x003BD4AE File Offset: 0x003BB6AE
		public void Flush()
		{
			this.FlushInternal();
		}

		// Token: 0x06009E1D RID: 40477 RVA: 0x003BD4B6 File Offset: 0x003BB6B6
		public Memory<T> GetMemory()
		{
			if (this.m_memoryOwner == null)
			{
				this.m_memoryOwner = new FileBackedArray<T>.FileBackedArrayMemoryManager(this.m_ptr, this.m_length, this.m_valueSize);
			}
			return this.m_memoryOwner.Memory;
		}

		// Token: 0x06009E1E RID: 40478 RVA: 0x003BD4E8 File Offset: 0x003BB6E8
		public ReadOnlyMemory<T> GetReadOnlyMemory()
		{
			if (this.m_memoryOwner == null)
			{
				this.m_memoryOwner = new FileBackedArray<T>.FileBackedArrayMemoryManager(this.m_ptr, this.m_length, this.m_valueSize);
			}
			return this.m_memoryOwner.Memory;
		}

		// Token: 0x06009E1F RID: 40479 RVA: 0x003BD51F File Offset: 0x003BB71F
		public unsafe T* GetPtr()
		{
			return this.m_ptr;
		}

		// Token: 0x06009E20 RID: 40480 RVA: 0x003BD527 File Offset: 0x003BB727
		public unsafe Span<T> GetSpan()
		{
			return new Span<T>((void*)this.m_ptr, this.m_length);
		}

		// Token: 0x06009E21 RID: 40481 RVA: 0x003BD53A File Offset: 0x003BB73A
		public unsafe ReadOnlySpan<T> GetReadOnlySpan()
		{
			return new ReadOnlySpan<T>((void*)this.m_ptr, this.m_length);
		}

		// Token: 0x0400783E RID: 30782
		[PublicizedFrom(EAccessModifier.Private)]
		public FileBackedArray<T> m_array;

		// Token: 0x0400783F RID: 30783
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int m_valueSize;

		// Token: 0x04007840 RID: 30784
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_start;

		// Token: 0x04007841 RID: 30785
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_length;

		// Token: 0x04007842 RID: 30786
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BackedArrayHandleMode m_mode;

		// Token: 0x04007843 RID: 30787
		[PublicizedFrom(EAccessModifier.Private)]
		public NativeArray<T> m_buffer;

		// Token: 0x04007844 RID: 30788
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe T* m_ptr;

		// Token: 0x04007845 RID: 30789
		[PublicizedFrom(EAccessModifier.Private)]
		public IMemoryOwner<T> m_memoryOwner;
	}
}
