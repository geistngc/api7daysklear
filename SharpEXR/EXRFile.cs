using System;
using System.Collections.Generic;
using System.IO;
using SharpEXR.AttributeTypes;

namespace SharpEXR
{
	// Token: 0x020016C9 RID: 5833
	public class EXRFile
	{
		// Token: 0x1700163B RID: 5691
		// (get) Token: 0x0600B638 RID: 46648 RVA: 0x0043E924 File Offset: 0x0043CB24
		// (set) Token: 0x0600B639 RID: 46649 RVA: 0x0043E92C File Offset: 0x0043CB2C
		public EXRVersion Version { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700163C RID: 5692
		// (get) Token: 0x0600B63A RID: 46650 RVA: 0x0043E935 File Offset: 0x0043CB35
		// (set) Token: 0x0600B63B RID: 46651 RVA: 0x0043E93D File Offset: 0x0043CB3D
		public List<EXRHeader> Headers { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700163D RID: 5693
		// (get) Token: 0x0600B63C RID: 46652 RVA: 0x0043E946 File Offset: 0x0043CB46
		// (set) Token: 0x0600B63D RID: 46653 RVA: 0x0043E94E File Offset: 0x0043CB4E
		public List<OffsetTable> OffsetTables { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700163E RID: 5694
		// (get) Token: 0x0600B63E RID: 46654 RVA: 0x0043E957 File Offset: 0x0043CB57
		// (set) Token: 0x0600B63F RID: 46655 RVA: 0x0043E95F File Offset: 0x0043CB5F
		public List<EXRPart> Parts { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600B641 RID: 46657 RVA: 0x0043E968 File Offset: 0x0043CB68
		public void Read(IEXRReader reader)
		{
			if (reader.ReadInt32() != 20000630)
			{
				throw new EXRFormatException("Invalid or corrupt EXR layout: First four bytes were not 20000630.");
			}
			int value = reader.ReadInt32();
			this.Version = new EXRVersion(value);
			this.Headers = new List<EXRHeader>();
			if (this.Version.IsMultiPart)
			{
				for (;;)
				{
					EXRHeader exrheader = new EXRHeader();
					exrheader.Read(this, reader);
					if (exrheader.IsEmpty)
					{
						break;
					}
					this.Headers.Add(exrheader);
				}
				throw new NotImplementedException("Multi part EXR files are not currently supported");
			}
			if (this.Version.IsSinglePartTiled)
			{
				throw new NotImplementedException("Tiled EXR files are not currently supported");
			}
			EXRHeader exrheader2 = new EXRHeader();
			exrheader2.Read(this, reader);
			this.Headers.Add(exrheader2);
			this.OffsetTables = new List<OffsetTable>();
			foreach (EXRHeader exrheader3 in this.Headers)
			{
				int num;
				if (this.Version.IsMultiPart)
				{
					num = exrheader3.ChunkCount;
				}
				else if (this.Version.IsSinglePartTiled)
				{
					num = 0;
				}
				else
				{
					EXRCompression compression = exrheader3.Compression;
					Box2I dataWindow = exrheader3.DataWindow;
					int scanLinesPerBlock = EXRFile.GetScanLinesPerBlock(compression);
					num = (int)Math.Ceiling((double)dataWindow.Height / (double)scanLinesPerBlock);
				}
				OffsetTable offsetTable = new OffsetTable(num);
				offsetTable.Read(reader, num);
				this.OffsetTables.Add(offsetTable);
			}
		}

		// Token: 0x0600B642 RID: 46658 RVA: 0x0043EAF0 File Offset: 0x0043CCF0
		public static int GetScanLinesPerBlock(EXRCompression compression)
		{
			switch (compression)
			{
			case EXRCompression.ZIP:
			case EXRCompression.PXR24:
				return 16;
			case EXRCompression.PIZ:
			case EXRCompression.B44:
			case EXRCompression.B44A:
				return 32;
			default:
				return 1;
			}
		}

		// Token: 0x0600B643 RID: 46659 RVA: 0x0043EB18 File Offset: 0x0043CD18
		public static int GetBytesPerPixel(ImageDestFormat format)
		{
			switch (format)
			{
			case ImageDestFormat.RGB8:
			case ImageDestFormat.BGR8:
				return 3;
			case ImageDestFormat.RGBA8:
			case ImageDestFormat.PremultipliedRGBA8:
			case ImageDestFormat.BGRA8:
			case ImageDestFormat.PremultipliedBGRA8:
				return 4;
			case ImageDestFormat.RGB16:
			case ImageDestFormat.BGR16:
				return 6;
			case ImageDestFormat.RGBA16:
			case ImageDestFormat.PremultipliedRGBA16:
			case ImageDestFormat.BGRA16:
			case ImageDestFormat.PremultipliedBGRA16:
				return 8;
			case ImageDestFormat.RGB32:
			case ImageDestFormat.BGR32:
				return 12;
			case ImageDestFormat.RGBA32:
			case ImageDestFormat.PremultipliedRGBA32:
			case ImageDestFormat.BGRA32:
			case ImageDestFormat.PremultipliedBGRA32:
				return 16;
			default:
				throw new ArgumentException("Unrecognized destination format", "format");
			}
		}

		// Token: 0x0600B644 RID: 46660 RVA: 0x0043EB94 File Offset: 0x0043CD94
		public static int GetBitsPerPixel(ImageDestFormat format)
		{
			switch (format)
			{
			case ImageDestFormat.RGB8:
			case ImageDestFormat.RGBA8:
			case ImageDestFormat.PremultipliedRGBA8:
			case ImageDestFormat.BGR8:
			case ImageDestFormat.BGRA8:
			case ImageDestFormat.PremultipliedBGRA8:
				return 8;
			case ImageDestFormat.RGB16:
			case ImageDestFormat.RGBA16:
			case ImageDestFormat.PremultipliedRGBA16:
			case ImageDestFormat.BGR16:
			case ImageDestFormat.BGRA16:
			case ImageDestFormat.PremultipliedBGRA16:
				return 16;
			case ImageDestFormat.RGB32:
			case ImageDestFormat.RGBA32:
			case ImageDestFormat.PremultipliedRGBA32:
			case ImageDestFormat.BGR32:
			case ImageDestFormat.BGRA32:
			case ImageDestFormat.PremultipliedBGRA32:
				return 32;
			default:
				throw new ArgumentException("Unrecognized destination format", "format");
			}
		}

		// Token: 0x0600B645 RID: 46661 RVA: 0x0043EC08 File Offset: 0x0043CE08
		public static EXRFile FromFile(string file)
		{
			EXRReader exrreader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read), false);
			EXRFile result = EXRFile.FromReader(exrreader);
			exrreader.Dispose();
			return result;
		}

		// Token: 0x0600B646 RID: 46662 RVA: 0x0043EC30 File Offset: 0x0043CE30
		public static EXRFile FromStream(Stream stream)
		{
			EXRReader exrreader = new EXRReader(new BinaryReader(stream));
			EXRFile result = EXRFile.FromReader(exrreader);
			exrreader.Dispose();
			return result;
		}

		// Token: 0x0600B647 RID: 46663 RVA: 0x0043EC58 File Offset: 0x0043CE58
		public static EXRFile FromReader(IEXRReader reader)
		{
			EXRFile exrfile = new EXRFile();
			exrfile.Read(reader);
			exrfile.Parts = new List<EXRPart>();
			for (int i = 0; i < exrfile.Headers.Count; i++)
			{
				EXRPart item = new EXRPart(exrfile.Version, exrfile.Headers[i], exrfile.OffsetTables[i]);
				exrfile.Parts.Add(item);
			}
			return exrfile;
		}
	}
}
