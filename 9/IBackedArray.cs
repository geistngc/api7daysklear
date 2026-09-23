using System;
using System.Runtime.CompilerServices;

// Token: 0x02001396 RID: 5014
public interface IBackedArray<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x170012B0 RID: 4784
	// (get) Token: 0x06009E22 RID: 40482
	int Length { get; }

	// Token: 0x06009E23 RID: 40483
	IBackedArrayHandle GetMemory(int start, int length, out Memory<T> memory);

	// Token: 0x06009E24 RID: 40484
	IBackedArrayHandle GetReadOnlyMemory(int start, int length, out ReadOnlyMemory<T> memory);

	// Token: 0x06009E25 RID: 40485
	unsafe IBackedArrayHandle GetMemoryUnsafe(int start, int length, out T* arrayPtr);

	// Token: 0x06009E26 RID: 40486
	unsafe IBackedArrayHandle GetReadOnlyMemoryUnsafe(int start, int length, out T* arrayPtr);

	// Token: 0x06009E27 RID: 40487
	IBackedArrayHandle GetSpan(int start, int length, out Span<T> span);

	// Token: 0x06009E28 RID: 40488
	IBackedArrayHandle GetReadOnlySpan(int start, int length, out ReadOnlySpan<T> span);
}
