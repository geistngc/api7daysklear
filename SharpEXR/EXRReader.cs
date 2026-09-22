using System;
using System.IO;
using System.Text;

namespace SharpEXR
{
	// Token: 0x020016D0 RID: 5840
	public class EXRReader : IDisposable, IEXRReader
	{
		// Token: 0x0600B687 RID: 46727 RVA: 0x0044051D File Offset: 0x0043E71D
		public EXRReader(Stream stream, bool leaveOpen = false) : this(new BinaryReader(stream, Encoding.ASCII, leaveOpen))
		{
		}

		// Token: 0x0600B688 RID: 46728 RVA: 0x00440531 File Offset: 0x0043E731
		public EXRReader(BinaryReader reader)
		{
			this.reader = reader;
		}

		// Token: 0x0600B689 RID: 46729 RVA: 0x00440540 File Offset: 0x0043E740
		public byte ReadByte()
		{
			return this.reader.ReadByte();
		}

		// Token: 0x0600B68A RID: 46730 RVA: 0x0044054D File Offset: 0x0043E74D
		public int ReadInt32()
		{
			return this.reader.ReadInt32();
		}

		// Token: 0x0600B68B RID: 46731 RVA: 0x0044055A File Offset: 0x0043E75A
		public uint ReadUInt32()
		{
			return this.reader.ReadUInt32();
		}

		// Token: 0x0600B68C RID: 46732 RVA: 0x00440567 File Offset: 0x0043E767
		public Half ReadHalf()
		{
			return Half.ToHalf(this.reader.ReadUInt16());
		}

		// Token: 0x0600B68D RID: 46733 RVA: 0x00440579 File Offset: 0x0043E779
		public float ReadSingle()
		{
			return this.reader.ReadSingle();
		}

		// Token: 0x0600B68E RID: 46734 RVA: 0x00440586 File Offset: 0x0043E786
		public double ReadDouble()
		{
			return this.reader.ReadDouble();
		}

		// Token: 0x0600B68F RID: 46735 RVA: 0x00440594 File Offset: 0x0043E794
		public string ReadNullTerminatedString(int maxLength)
		{
			long position = this.reader.BaseStream.Position;
			StringBuilder stringBuilder = new StringBuilder();
			byte value;
			while ((value = this.reader.ReadByte()) != 0)
			{
				if (this.reader.BaseStream.Position - position > (long)maxLength)
				{
					throw new EXRFormatException("Null terminated string exceeded maximum length of " + maxLength.ToString() + " bytes.");
				}
				stringBuilder.Append((char)value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600B690 RID: 46736 RVA: 0x0044060C File Offset: 0x0043E80C
		public string ReadString()
		{
			int length = this.ReadInt32();
			return this.ReadString(length);
		}

		// Token: 0x0600B691 RID: 46737 RVA: 0x00440628 File Offset: 0x0043E828
		public string ReadString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append((char)this.reader.ReadByte());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600B692 RID: 46738 RVA: 0x0044065F File Offset: 0x0043E85F
		public byte[] ReadBytes(int count)
		{
			return this.reader.ReadBytes(count);
		}

		// Token: 0x0600B693 RID: 46739 RVA: 0x0044066D File Offset: 0x0043E86D
		public void CopyBytes(byte[] dest, int offset, int count)
		{
			if (this.reader.BaseStream.Read(dest, offset, count) != count)
			{
				throw new Exception("Less bytes read than expected");
			}
		}

		// Token: 0x0600B694 RID: 46740 RVA: 0x00440690 File Offset: 0x0043E890
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600B695 RID: 46741 RVA: 0x004406A0 File Offset: 0x0043E8A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				try
				{
					this.reader.Dispose();
				}
				catch
				{
				}
			}
			this.disposed = true;
		}

		// Token: 0x1700164C RID: 5708
		// (get) Token: 0x0600B696 RID: 46742 RVA: 0x004406E0 File Offset: 0x0043E8E0
		// (set) Token: 0x0600B697 RID: 46743 RVA: 0x004406F3 File Offset: 0x0043E8F3
		public int Position
		{
			get
			{
				return (int)this.reader.BaseStream.Position;
			}
			set
			{
				this.reader.BaseStream.Seek((long)value, SeekOrigin.Begin);
			}
		}

		// Token: 0x040088CC RID: 35020
		[PublicizedFrom(EAccessModifier.Private)]
		public BinaryReader reader;

		// Token: 0x040088CD RID: 35021
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;
	}
}
