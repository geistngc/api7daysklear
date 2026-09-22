using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Platform.XBL.Save.MasterFileTable.Latest
{
	// Token: 0x02001C78 RID: 7288
	public sealed class BlobRef : IEquatable<BlobRef>, IMigratable
	{
		// Token: 0x17001AC5 RID: 6853
		// (get) Token: 0x0600D805 RID: 55301 RVA: 0x004DE127 File Offset: 0x004DC327
		// (set) Token: 0x0600D806 RID: 55302 RVA: 0x004DE12F File Offset: 0x004DC32F
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 6;

		// Token: 0x17001AC6 RID: 6854
		// (get) Token: 0x0600D807 RID: 55303 RVA: 0x004DE138 File Offset: 0x004DC338
		// (set) Token: 0x0600D808 RID: 55304 RVA: 0x004DE140 File Offset: 0x004DC340
		public ReadOnlyMemory<byte> Hash
		{
			get
			{
				return this.m_hash;
			}
			set
			{
				if (16 != value.Length)
				{
					throw new InvalidOperationException(string.Format("Expected hash to be of length {0}", 16));
				}
				this.m_hash = value;
			}
		}

		// Token: 0x17001AC7 RID: 6855
		// (get) Token: 0x0600D809 RID: 55305 RVA: 0x004DE16B File Offset: 0x004DC36B
		// (set) Token: 0x0600D80A RID: 55306 RVA: 0x004DE173 File Offset: 0x004DC373
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x0600D80B RID: 55307 RVA: 0x004DE17C File Offset: 0x004DC37C
		public void Write(PooledBinaryWriter writer)
		{
			writer.Write(this.Version);
			int value = 0;
			long position = writer.BaseStream.Position;
			writer.Write(value);
			long position2 = writer.BaseStream.Position;
			writer.Write(this.Id);
			writer.Write(this.Length);
			if (16 != this.Hash.Length)
			{
				Log.Error(string.Format("[{0}] Expected a hash size of exactly {1}, padding or truncating as needed.", "BlobRef", 16));
				if (this.Hash.Length < 16)
				{
					writer.Write(this.Hash.Span);
					for (int i = this.Hash.Length; i < 16; i++)
					{
						writer.Write(0);
					}
				}
				else
				{
					writer.Write(this.Hash.Span.Slice(0, 16));
				}
			}
			else
			{
				writer.Write(this.Hash.Span);
			}
			writer.Write(this.FutureData);
			long position3 = writer.BaseStream.Position;
			value = (int)(position3 - position2);
			writer.BaseStream.Position = position;
			writer.Write(value);
			writer.BaseStream.Position = position3;
		}

		// Token: 0x0600D80C RID: 55308 RVA: 0x004DE2C4 File Offset: 0x004DC4C4
		public void Read(PooledBinaryReader reader)
		{
			if (this.Version != 6 || this.Id != 0UL || this.Length != 0U || !this.m_hash.Span.SequenceEqual(BlobRef.s_emptyHash) || this.FutureData.Length != 0)
			{
				throw new InvalidOperationException("Read should only be called on a new BlobRef.");
			}
			this.Version = reader.ReadUInt16();
			int num = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			this.Id = reader.ReadUInt64();
			this.Length = reader.ReadUInt32();
			byte[] array = new byte[16];
			int num2;
			if (!reader.TryReadAllBytes(array, out num2))
			{
				Log.Error(string.Format("[{0}] Expected {1} for the hash bytes, but reached the end of stream after reading {2} bytes?", "BlobRef", 16, num2));
			}
			this.Hash = array;
			long position2 = reader.BaseStream.Position;
			this.FutureData = new byte[(int)((long)num - (position2 - position))];
			int num3;
			if (!reader.TryReadAllBytes(this.FutureData, out num3))
			{
				throw new IOException(string.Format("Expected {0} bytes to be read for future data but only got {1} bytes.", this.FutureData.Length, num3));
			}
		}

		// Token: 0x0600D80D RID: 55309 RVA: 0x004DE3F0 File Offset: 0x004DC5F0
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

		// Token: 0x0600D80E RID: 55310 RVA: 0x004DE453 File Offset: 0x004DC653
		public static HashAlgorithm GetHashAlgorithm()
		{
			return BlobRef.s_md5.Value;
		}

		// Token: 0x0600D80F RID: 55311 RVA: 0x004DE460 File Offset: 0x004DC660
		public static ReadOnlyMemory<byte> CalculateHash(RefCountedBuffer buffer)
		{
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
			return array;
		}

		// Token: 0x0600D810 RID: 55312 RVA: 0x004DE4C0 File Offset: 0x004DC6C0
		public bool Equals(BlobRef other)
		{
			return other != null && (this == other || (this.Id == other.Id && this.Length == other.Length && (this.Hash.Equals(other.Hash) || this.Hash.Span.SequenceEqual(other.Hash.Span))));
		}

		// Token: 0x0600D811 RID: 55313 RVA: 0x004DE530 File Offset: 0x004DC730
		public override bool Equals(object obj)
		{
			if (this != obj)
			{
				BlobRef blobRef = obj as BlobRef;
				return blobRef != null && this.Equals(blobRef);
			}
			return true;
		}

		// Token: 0x0600D812 RID: 55314 RVA: 0x004DE558 File Offset: 0x004DC758
		public override int GetHashCode()
		{
			return HashCode.Combine<ulong, uint, int>(this.Id, this.Length, SpanUtils.GetHashCode<byte>(this.Hash.Span));
		}

		// Token: 0x0600D813 RID: 55315 RVA: 0x001DC27C File Offset: 0x001DA47C
		public static bool operator ==(BlobRef left, BlobRef right)
		{
			return object.Equals(left, right);
		}

		// Token: 0x0600D814 RID: 55316 RVA: 0x001DC285 File Offset: 0x001DA485
		public static bool operator !=(BlobRef left, BlobRef right)
		{
			return !object.Equals(left, right);
		}

		// Token: 0x0400A484 RID: 42116
		public const ushort VERSION = 6;

		// Token: 0x0400A485 RID: 42117
		public const int HashSizeBytes = 16;

		// Token: 0x0400A486 RID: 42118
		public const int SizeBytes = 28;

		// Token: 0x0400A487 RID: 42119
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ThreadLocal<MD5> s_md5 = new ThreadLocal<MD5>(new Func<MD5>(MD5.Create));

		// Token: 0x0400A488 RID: 42120
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] s_emptyHash = new byte[16];

		// Token: 0x0400A48A RID: 42122
		public ulong Id;

		// Token: 0x0400A48B RID: 42123
		public uint Length;

		// Token: 0x0400A48C RID: 42124
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadOnlyMemory<byte> m_hash = BlobRef.s_emptyHash;
	}
}
