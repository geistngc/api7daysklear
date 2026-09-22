using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Platform.XBL.Save.MasterFileTable.V05
{
	// Token: 0x02001C69 RID: 7273
	public sealed class BlobRef : IEquatable<BlobRef>, IMigratable
	{
		// Token: 0x17001AAA RID: 6826
		// (get) Token: 0x0600D760 RID: 55136 RVA: 0x004DB148 File Offset: 0x004D9348
		// (set) Token: 0x0600D761 RID: 55137 RVA: 0x004DB150 File Offset: 0x004D9350
		public ushort Version { get; [PublicizedFrom(EAccessModifier.Private)] set; } = 5;

		// Token: 0x17001AAB RID: 6827
		// (get) Token: 0x0600D762 RID: 55138 RVA: 0x004DB159 File Offset: 0x004D9359
		// (set) Token: 0x0600D763 RID: 55139 RVA: 0x004DB161 File Offset: 0x004D9361
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

		// Token: 0x17001AAC RID: 6828
		// (get) Token: 0x0600D764 RID: 55140 RVA: 0x004DB18C File Offset: 0x004D938C
		// (set) Token: 0x0600D765 RID: 55141 RVA: 0x004DB194 File Offset: 0x004D9394
		public byte[] FutureData { get; [PublicizedFrom(EAccessModifier.Private)] set; } = Array.Empty<byte>();

		// Token: 0x0600D766 RID: 55142 RVA: 0x004DB1A0 File Offset: 0x004D93A0
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

		// Token: 0x0600D767 RID: 55143 RVA: 0x004DB2E8 File Offset: 0x004D94E8
		public void Read(PooledBinaryReader reader)
		{
			if (this.Version != 5 || this.Id != 0UL || this.Length != 0U || !this.m_hash.Span.SequenceEqual(BlobRef.s_emptyHash) || this.FutureData.Length != 0)
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

		// Token: 0x0600D768 RID: 55144 RVA: 0x004DB414 File Offset: 0x004D9614
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

		// Token: 0x0600D769 RID: 55145 RVA: 0x004DB477 File Offset: 0x004D9677
		public static HashAlgorithm GetHashAlgorithm()
		{
			return BlobRef.s_md5.Value;
		}

		// Token: 0x0600D76A RID: 55146 RVA: 0x004DB484 File Offset: 0x004D9684
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

		// Token: 0x0600D76B RID: 55147 RVA: 0x004DB4E4 File Offset: 0x004D96E4
		public bool Equals(BlobRef other)
		{
			return other != null && (this == other || (this.Id == other.Id && this.Length == other.Length && (this.Hash.Equals(other.Hash) || this.Hash.Span.SequenceEqual(other.Hash.Span))));
		}

		// Token: 0x0600D76C RID: 55148 RVA: 0x004DB554 File Offset: 0x004D9754
		public override bool Equals(object obj)
		{
			if (this != obj)
			{
				BlobRef blobRef = obj as BlobRef;
				return blobRef != null && this.Equals(blobRef);
			}
			return true;
		}

		// Token: 0x0600D76D RID: 55149 RVA: 0x004DB57C File Offset: 0x004D977C
		public override int GetHashCode()
		{
			return HashCode.Combine<ulong, uint, int>(this.Id, this.Length, SpanUtils.GetHashCode<byte>(this.Hash.Span));
		}

		// Token: 0x0600D76E RID: 55150 RVA: 0x001DC27C File Offset: 0x001DA47C
		public static bool operator ==(BlobRef left, BlobRef right)
		{
			return object.Equals(left, right);
		}

		// Token: 0x0600D76F RID: 55151 RVA: 0x001DC285 File Offset: 0x001DA485
		public static bool operator !=(BlobRef left, BlobRef right)
		{
			return !object.Equals(left, right);
		}

		// Token: 0x0400A435 RID: 42037
		public const ushort VERSION = 5;

		// Token: 0x0400A436 RID: 42038
		public const int HashSizeBytes = 16;

		// Token: 0x0400A437 RID: 42039
		public const int SizeBytes = 28;

		// Token: 0x0400A438 RID: 42040
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ThreadLocal<MD5> s_md5 = new ThreadLocal<MD5>(new Func<MD5>(MD5.Create));

		// Token: 0x0400A439 RID: 42041
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] s_emptyHash = new byte[16];

		// Token: 0x0400A43B RID: 42043
		public ulong Id;

		// Token: 0x0400A43C RID: 42044
		public uint Length;

		// Token: 0x0400A43D RID: 42045
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadOnlyMemory<byte> m_hash = BlobRef.s_emptyHash;
	}
}
