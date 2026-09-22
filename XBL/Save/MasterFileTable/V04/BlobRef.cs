using System;
using System.Security.Cryptography;
using System.Threading;

namespace Platform.XBL.Save.MasterFileTable.V04
{
	// Token: 0x02001C6D RID: 7277
	public sealed class BlobRef : IEquatable<BlobRef>
	{
		// Token: 0x0600D796 RID: 55190 RVA: 0x004DC18E File Offset: 0x004DA38E
		public BlobRef(ulong id, uint length) : this(id, length, BlobRef.s_emptyHash.AsMemory<byte>())
		{
		}

		// Token: 0x0600D797 RID: 55191 RVA: 0x004DC1A8 File Offset: 0x004DA3A8
		public BlobRef(ulong id, uint length, ReadOnlySpan<byte> hash)
		{
			this.Id = id;
			this.Length = length;
			if (16 != hash.Length)
			{
				throw new InvalidOperationException("Expected hash to be of length " + 16.ToString());
			}
			this.Hash = hash.ToArray();
		}

		// Token: 0x0600D798 RID: 55192 RVA: 0x004DC200 File Offset: 0x004DA400
		public BlobRef(ulong id, uint length, ReadOnlyMemory<byte> hash)
		{
			this.Id = id;
			this.Length = length;
			if (16 != hash.Length)
			{
				throw new InvalidOperationException("Expected hash to be of length " + 16.ToString());
			}
			this.Hash = hash;
		}

		// Token: 0x0600D799 RID: 55193 RVA: 0x004DC250 File Offset: 0x004DA450
		public BlobRef(ulong id, RefCountedBuffer buffer)
		{
			this.Id = id;
			this.Length = (uint)buffer.Length;
			HashAlgorithm hashAlgorithm = BlobRef.GetHashAlgorithm();
			if (128 != hashAlgorithm.HashSize)
			{
				throw new InvalidOperationException("Unexpected hash algorithm hash size.");
			}
			byte[] array = new byte[16];
			int num;
			if (!hashAlgorithm.TryComputeHash(buffer.Span, array, out num))
			{
				throw new InvalidOperationException("Expected to be able to compute the hash.");
			}
			this.Hash = array;
		}

		// Token: 0x0600D79A RID: 55194 RVA: 0x004DC2D0 File Offset: 0x004DA4D0
		public void Write(PooledBinaryWriter writer)
		{
			writer.Write(this.Id);
			writer.Write(this.Length);
			if (16 == this.Hash.Length)
			{
				writer.Write(this.Hash.Span);
				return;
			}
			Log.Error("[BlobRef] Expected a hash size of exactly " + 16.ToString() + ", padding or truncating as needed.");
			if (this.Hash.Length < 16)
			{
				writer.Write(this.Hash.Span);
				for (int i = this.Hash.Length; i < 16; i++)
				{
					writer.Write(0);
				}
				return;
			}
			writer.Write(this.Hash.Span.Slice(0, 16));
		}

		// Token: 0x0600D79B RID: 55195 RVA: 0x004DC390 File Offset: 0x004DA590
		public static BlobRef Read(PooledBinaryReader reader)
		{
			ulong id = reader.ReadUInt64();
			uint length = reader.ReadUInt32();
			byte[] array = new byte[16];
			int num;
			if (!reader.TryReadAllBytes(array, out num))
			{
				Log.Error(string.Format("[BlobRef] Expected {0} for the hash bytes, but reached the end of stream after reading {1} bytes?", 16, num));
			}
			return new BlobRef(id, length, array.AsMemory<byte>());
		}

		// Token: 0x0600D79C RID: 55196 RVA: 0x004DC3F0 File Offset: 0x004DA5F0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"BlobRef[Id=",
				SaveContainer.IdToString(this.Id),
				", Length=",
				this.Length.FormatSize(false),
				", Hash=",
				this.Hash.ToHexString(),
				"]"
			});
		}

		// Token: 0x0600D79D RID: 55197 RVA: 0x004DC453 File Offset: 0x004DA653
		public static HashAlgorithm GetHashAlgorithm()
		{
			return BlobRef.s_md5.Value;
		}

		// Token: 0x0600D79E RID: 55198 RVA: 0x004DC460 File Offset: 0x004DA660
		public bool Equals(BlobRef other)
		{
			return other != null && (this == other || (this.Id == other.Id && this.Length == other.Length && (this.Hash.Equals(other.Hash) || this.Hash.Span.SequenceEqual(other.Hash.Span))));
		}

		// Token: 0x0600D79F RID: 55199 RVA: 0x004DC4C8 File Offset: 0x004DA6C8
		public override bool Equals(object obj)
		{
			if (this != obj)
			{
				BlobRef blobRef = obj as BlobRef;
				return blobRef != null && this.Equals(blobRef);
			}
			return true;
		}

		// Token: 0x0600D7A0 RID: 55200 RVA: 0x004DC4EE File Offset: 0x004DA6EE
		public override int GetHashCode()
		{
			return HashCode.Combine<ulong, uint, int>(this.Id, this.Length, SpanUtils.GetHashCode<byte>(this.Hash.Span));
		}

		// Token: 0x0600D7A1 RID: 55201 RVA: 0x001DC27C File Offset: 0x001DA47C
		public static bool operator ==(BlobRef left, BlobRef right)
		{
			return object.Equals(left, right);
		}

		// Token: 0x0600D7A2 RID: 55202 RVA: 0x001DC285 File Offset: 0x001DA485
		public static bool operator !=(BlobRef left, BlobRef right)
		{
			return !object.Equals(left, right);
		}

		// Token: 0x0400A452 RID: 42066
		public const int HashSizeBytes = 16;

		// Token: 0x0400A453 RID: 42067
		public const int SizeBytes = 28;

		// Token: 0x0400A454 RID: 42068
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ThreadLocal<MD5> s_md5 = new ThreadLocal<MD5>(new Func<MD5>(MD5.Create));

		// Token: 0x0400A455 RID: 42069
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] s_emptyHash = new byte[16];

		// Token: 0x0400A456 RID: 42070
		public readonly ulong Id;

		// Token: 0x0400A457 RID: 42071
		public readonly uint Length;

		// Token: 0x0400A458 RID: 42072
		public readonly ReadOnlyMemory<byte> Hash;
	}
}
