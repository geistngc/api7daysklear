using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpEXR.AttributeTypes;
using SharpEXR.ColorSpace;

namespace SharpEXR
{
	// Token: 0x020016CD RID: 5837
	public class EXRPart
	{
		// Token: 0x17001647 RID: 5703
		// (get) Token: 0x0600B65C RID: 46684 RVA: 0x0043EF4C File Offset: 0x0043D14C
		// (set) Token: 0x0600B65D RID: 46685 RVA: 0x0043EF54 File Offset: 0x0043D154
		public Dictionary<string, float[]> FloatChannels
		{
			get
			{
				return this.floatChannels;
			}
			[PublicizedFrom(EAccessModifier.Protected)]
			set
			{
				this.floatChannels = value;
			}
		}

		// Token: 0x17001648 RID: 5704
		// (get) Token: 0x0600B65E RID: 46686 RVA: 0x0043EF5D File Offset: 0x0043D15D
		// (set) Token: 0x0600B65F RID: 46687 RVA: 0x0043EF65 File Offset: 0x0043D165
		public Dictionary<string, Half[]> HalfChannels
		{
			get
			{
				return this.halfChannels;
			}
			[PublicizedFrom(EAccessModifier.Protected)]
			set
			{
				this.halfChannels = value;
			}
		}

		// Token: 0x0600B660 RID: 46688 RVA: 0x0043EF70 File Offset: 0x0043D170
		public EXRPart(EXRVersion version, EXRHeader header, OffsetTable offsets)
		{
			this.Version = version;
			this.Header = header;
			this.Offsets = offsets;
			if (this.Version.IsMultiPart)
			{
				this.Type = header.Type;
			}
			else
			{
				this.Type = (version.IsSinglePartTiled ? PartType.Tiled : PartType.ScanLine);
			}
			this.DataWindow = this.Header.DataWindow;
			this.FloatChannels = new Dictionary<string, float[]>();
			this.HalfChannels = new Dictionary<string, Half[]>();
			foreach (Channel channel in header.Channels)
			{
				if (channel.Type == PixelType.Float)
				{
					this.FloatChannels[channel.Name] = new float[this.DataWindow.Width * this.DataWindow.Height];
				}
				else
				{
					if (channel.Type != PixelType.Half)
					{
						throw new NotImplementedException("Only 16 and 32 bit floating point EXR images are supported.");
					}
					this.HalfChannels[channel.Name] = new Half[this.DataWindow.Width * this.DataWindow.Height];
				}
			}
		}

		// Token: 0x0600B661 RID: 46689 RVA: 0x0043F0A8 File Offset: 0x0043D2A8
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void CheckHasData()
		{
			if (!this.hasData)
			{
				throw new InvalidOperationException("Call EXRPart.Open before performing image operations.");
			}
		}

		// Token: 0x0600B662 RID: 46690 RVA: 0x0043F0BD File Offset: 0x0043D2BD
		public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			return this.GetHalfs(channels, premultiplied, gamma, this.HasAlpha);
		}

		// Token: 0x0600B663 RID: 46691 RVA: 0x0043F0D0 File Offset: 0x0043D2D0
		public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetHalfs(srcFormat, channels, premultiplied, gamma);
		}

		// Token: 0x0600B664 RID: 46692 RVA: 0x0043F174 File Offset: 0x0043D374
		public Half[] GetHalfs(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			ImageDestFormat imageDestFormat;
			if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA)
			{
				if (premultiplied)
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.PremultipliedBGRA16 : ImageDestFormat.PremultipliedRGBA16);
				}
				else
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGRA16 : ImageDestFormat.RGBA16);
				}
			}
			else
			{
				imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGR16 : ImageDestFormat.RGB16);
			}
			int bytesPerPixel = EXRFile.GetBytesPerPixel(imageDestFormat);
			if (srcFormat != ImageSourceFormat.SingleRGB)
			{
			}
			byte[] bytes = this.GetBytes(srcFormat, imageDestFormat, gamma, this.DataWindow.Width * bytesPerPixel);
			Half[] array = new Half[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return array;
		}

		// Token: 0x0600B665 RID: 46693 RVA: 0x0043F1ED File Offset: 0x0043D3ED
		public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			return this.GetFloats(channels, premultiplied, gamma, this.HasAlpha);
		}

		// Token: 0x0600B666 RID: 46694 RVA: 0x0043F200 File Offset: 0x0043D400
		public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetFloats(srcFormat, channels, premultiplied, gamma);
		}

		// Token: 0x0600B667 RID: 46695 RVA: 0x0043F2A4 File Offset: 0x0043D4A4
		public float[] GetFloats(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			ImageDestFormat imageDestFormat;
			if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA)
			{
				if (premultiplied)
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.PremultipliedBGRA32 : ImageDestFormat.PremultipliedRGBA32);
				}
				else
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGRA32 : ImageDestFormat.RGBA32);
				}
			}
			else
			{
				imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGR32 : ImageDestFormat.RGB32);
			}
			int bytesPerPixel = EXRFile.GetBytesPerPixel(imageDestFormat);
			if (srcFormat != ImageSourceFormat.SingleRGB)
			{
			}
			byte[] bytes = this.GetBytes(srcFormat, imageDestFormat, gamma, this.DataWindow.Width * bytesPerPixel);
			float[] array = new float[bytes.Length / 4];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return array;
		}

		// Token: 0x0600B668 RID: 46696 RVA: 0x0043F320 File Offset: 0x0043D520
		public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma)
		{
			return this.GetBytes(destFormat, gamma, this.DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
		}

		// Token: 0x0600B669 RID: 46697 RVA: 0x0043F34C File Offset: 0x0043D54C
		public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma, int stride)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (this.HalfChannels.ContainsKey("A") ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (this.FloatChannels.ContainsKey("A") ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetBytes(srcFormat, destFormat, gamma, stride);
		}

		// Token: 0x0600B66A RID: 46698 RVA: 0x0043F40C File Offset: 0x0043D60C
		public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma)
		{
			return this.GetBytes(srcFormat, destFormat, gamma, this.DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
		}

		// Token: 0x0600B66B RID: 46699 RVA: 0x0043F438 File Offset: 0x0043D638
		public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma, int stride)
		{
			this.CheckHasData();
			int bytesPerPixel = EXRFile.GetBytesPerPixel(destFormat);
			int bitsPerPixel = EXRFile.GetBitsPerPixel(destFormat);
			if (stride < bytesPerPixel * this.DataWindow.Width)
			{
				throw new ArgumentException("Stride was lower than minimum", "stride");
			}
			byte[] array = new byte[stride * this.DataWindow.Height];
			int num = stride - bytesPerPixel * this.DataWindow.Width;
			bool flag = srcFormat == ImageSourceFormat.HalfRGB || srcFormat == ImageSourceFormat.HalfRGBA;
			bool sourceAlpha = false;
			bool destinationAlpha = destFormat == ImageDestFormat.BGRA16 || destFormat == ImageDestFormat.BGRA32 || destFormat == ImageDestFormat.BGRA8 || destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8 || destFormat == ImageDestFormat.PremultipliedRGBA16 || destFormat == ImageDestFormat.PremultipliedRGBA32 || destFormat == ImageDestFormat.PremultipliedRGBA8 || destFormat == ImageDestFormat.RGBA16 || destFormat == ImageDestFormat.RGBA32 || destFormat == ImageDestFormat.RGBA8;
			bool premultiplied = destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8 || destFormat == ImageDestFormat.PremultipliedRGBA16 || destFormat == ImageDestFormat.PremultipliedRGBA32 || destFormat == ImageDestFormat.PremultipliedRGBA8;
			bool bgra = destFormat == ImageDestFormat.BGR16 || destFormat == ImageDestFormat.BGR32 || destFormat == ImageDestFormat.BGR8 || destFormat == ImageDestFormat.BGRA16 || destFormat == ImageDestFormat.BGRA32 || destFormat == ImageDestFormat.BGRA8 || destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8;
			Half[] ha;
			Half[] hb;
			Half[] hr;
			Half[] hg = hr = (hb = (ha = null));
			float[] fa;
			float[] fb;
			float[] fr;
			float[] fg = fr = (fb = (fa = null));
			if (flag)
			{
				if (!this.HalfChannels.ContainsKey("R"))
				{
					throw new ArgumentException("Half type channel R not found", "srcFormat");
				}
				if (!this.HalfChannels.ContainsKey("G"))
				{
					throw new ArgumentException("Half type channel G not found", "srcFormat");
				}
				if (!this.HalfChannels.ContainsKey("B"))
				{
					throw new ArgumentException("Half type channel B not found", "srcFormat");
				}
				hr = this.HalfChannels["R"];
				hg = this.HalfChannels["G"];
				hb = this.HalfChannels["B"];
				if (srcFormat == ImageSourceFormat.HalfRGBA)
				{
					if (!this.HalfChannels.ContainsKey("A"))
					{
						throw new ArgumentException("Half type channel A not found", "srcFormat");
					}
					ha = this.HalfChannels["A"];
					sourceAlpha = true;
				}
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R"))
				{
					throw new ArgumentException("Single type channel R not found", "srcFormat");
				}
				if (!this.FloatChannels.ContainsKey("G"))
				{
					throw new ArgumentException("Single type channel G not found", "srcFormat");
				}
				if (!this.FloatChannels.ContainsKey("B"))
				{
					throw new ArgumentException("Single type channel B not found", "srcFormat");
				}
				fr = this.FloatChannels["R"];
				fg = this.FloatChannels["G"];
				fb = this.FloatChannels["B"];
				if (srcFormat == ImageSourceFormat.HalfRGBA)
				{
					if (!this.FloatChannels.ContainsKey("A"))
					{
						throw new ArgumentException("Single type channel A not found", "srcFormat");
					}
					fa = this.FloatChannels["A"];
					sourceAlpha = true;
				}
			}
			int num2 = 0;
			int num3 = 0;
			BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(array));
			int i = 0;
			while (i < this.DataWindow.Height)
			{
				this.GetScanlineBytes(bytesPerPixel, num3, num2, flag, destinationAlpha, sourceAlpha, hr, hg, hb, ha, fr, fg, fb, fa, bitsPerPixel, gamma, premultiplied, bgra, array, binaryWriter);
				num3 += this.DataWindow.Width * bytesPerPixel;
				num2 += this.DataWindow.Width;
				i++;
				num3 += num;
			}
			binaryWriter.Dispose();
			binaryWriter.BaseStream.Dispose();
			return array;
		}

		// Token: 0x0600B66C RID: 46700 RVA: 0x0043F7BC File Offset: 0x0043D9BC
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetScanlineBytes(int bytesPerPixel, int destIndex, int srcIndex, bool isHalf, bool destinationAlpha, bool sourceAlpha, Half[] hr, Half[] hg, Half[] hb, Half[] ha, float[] fr, float[] fg, float[] fb, float[] fa, int bitsPerPixel, GammaEncoding gamma, bool premultiplied, bool bgra, byte[] buffer, BinaryWriter writer)
		{
			writer.Seek(destIndex, SeekOrigin.Begin);
			int i = 0;
			while (i < this.DataWindow.Width)
			{
				float num;
				float num2;
				float num3;
				float num4;
				if (isHalf)
				{
					num = hr[srcIndex];
					num2 = hg[srcIndex];
					num3 = hb[srcIndex];
					if (destinationAlpha)
					{
						num4 = (sourceAlpha ? ha[srcIndex] : 1f);
					}
					else
					{
						num4 = 1f;
					}
				}
				else
				{
					num = fr[srcIndex];
					num2 = fg[srcIndex];
					num3 = fb[srcIndex];
					if (destinationAlpha)
					{
						num4 = (sourceAlpha ? fa[srcIndex] : 1f);
					}
					else
					{
						num4 = 1f;
					}
				}
				if (bitsPerPixel == 8)
				{
					byte b = byte.MaxValue;
					byte b2;
					byte b3;
					byte b4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num * num4 * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num2 * num4 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num3 * num4 * 255f) + 0.5)));
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
						else
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num2 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num3 * 255f) + 0.5)));
							if (destinationAlpha)
							{
								b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num) * num4 * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num2) * num4 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num3) * num4 * 255f) + 0.5)));
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
						else
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num) * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num2) * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num3) * 255f) + 0.5)));
							if (destinationAlpha)
							{
								b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
							}
						}
					}
					else if (premultiplied)
					{
						b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num) * num4 * 255f) + 0.5)));
						b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num2) * num4 * 255f) + 0.5)));
						b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num3) * num4 * 255f) + 0.5)));
						b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
					}
					else
					{
						b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num) * 255f) + 0.5)));
						b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num2) * 255f) + 0.5)));
						b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num3) * 255f) + 0.5)));
						if (destinationAlpha)
						{
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
					}
					if (bgra)
					{
						buffer[destIndex] = b4;
						buffer[destIndex + 1] = b3;
						buffer[destIndex + 2] = b2;
					}
					else
					{
						buffer[destIndex] = b2;
						buffer[destIndex + 1] = b3;
						buffer[destIndex + 2] = b4;
					}
					if (destinationAlpha)
					{
						buffer[destIndex + 3] = b;
					}
				}
				else if (bitsPerPixel == 32)
				{
					float value = 1f;
					float value2;
					float value3;
					float value4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							value2 = num * num4;
							value3 = num2 * num4;
							value4 = num3 * num4;
							value = num4;
						}
						else
						{
							value2 = num;
							value3 = num2;
							value4 = num3;
							if (destinationAlpha)
							{
								value = num4;
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							value2 = Gamma.Compress(num) * num4;
							value3 = Gamma.Compress(num2) * num4;
							value4 = Gamma.Compress(num3) * num4;
							value = num4;
						}
						else
						{
							value2 = Gamma.Compress(num);
							value3 = Gamma.Compress(num2);
							value4 = Gamma.Compress(num3);
							if (destinationAlpha)
							{
								value = num4;
							}
						}
					}
					else if (premultiplied)
					{
						value2 = Gamma.Compress_sRGB(num) * num4;
						value3 = Gamma.Compress_sRGB(num2) * num4;
						value4 = Gamma.Compress_sRGB(num3) * num4;
						value = num4;
					}
					else
					{
						value2 = Gamma.Compress_sRGB(num);
						value3 = Gamma.Compress_sRGB(num2);
						value4 = Gamma.Compress_sRGB(num3);
						if (destinationAlpha)
						{
							value = num4;
						}
					}
					if (bgra)
					{
						writer.Write(value4);
						writer.Write(value3);
						writer.Write(value2);
					}
					else
					{
						writer.Write(value2);
						writer.Write(value3);
						writer.Write(value4);
					}
					if (destinationAlpha)
					{
						writer.Write(value);
					}
				}
				else
				{
					Half half = new Half(1f);
					Half half2;
					Half half3;
					Half half4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							half2 = (Half)(num * num4);
							half3 = (Half)(num2 * num4);
							half4 = (Half)(num3 * num4);
							half = (Half)num4;
						}
						else
						{
							half2 = (Half)num;
							half3 = (Half)num2;
							half4 = (Half)num3;
							if (destinationAlpha)
							{
								half = (Half)num4;
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							half2 = (Half)(Gamma.Compress(num) * num4);
							half3 = (Half)(Gamma.Compress(num2) * num4);
							half4 = (Half)(Gamma.Compress(num3) * num4);
							half = (Half)num4;
						}
						else
						{
							half2 = (Half)Gamma.Compress(num);
							half3 = (Half)Gamma.Compress(num2);
							half4 = (Half)Gamma.Compress(num3);
							if (destinationAlpha)
							{
								half = (Half)num4;
							}
						}
					}
					else if (premultiplied)
					{
						half2 = (Half)(Gamma.Compress_sRGB(num) * num4);
						half3 = (Half)(Gamma.Compress_sRGB(num2) * num4);
						half4 = (Half)(Gamma.Compress_sRGB(num3) * num4);
						half = (Half)num4;
					}
					else
					{
						half2 = (Half)Gamma.Compress_sRGB(num);
						half3 = (Half)Gamma.Compress_sRGB(num2);
						half4 = (Half)Gamma.Compress_sRGB(num3);
						if (destinationAlpha)
						{
							half = (Half)num4;
						}
					}
					if (bgra)
					{
						writer.Write(half4.value);
						writer.Write(half3.value);
						writer.Write(half2.value);
					}
					else
					{
						writer.Write(half2.value);
						writer.Write(half3.value);
						writer.Write(half4.value);
					}
					if (destinationAlpha)
					{
						writer.Write(half.value);
					}
				}
				i++;
				destIndex += bytesPerPixel;
				srcIndex++;
			}
		}

		// Token: 0x0600B66D RID: 46701 RVA: 0x0044019C File Offset: 0x0043E39C
		public void Open(string file)
		{
			EXRReader exrreader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read), false);
			this.Open(exrreader);
			exrreader.Dispose();
		}

		// Token: 0x0600B66E RID: 46702 RVA: 0x004401C8 File Offset: 0x0043E3C8
		public void Open(Stream stream)
		{
			EXRReader exrreader = new EXRReader(new BinaryReader(stream));
			this.Open(exrreader);
			exrreader.Dispose();
		}

		// Token: 0x0600B66F RID: 46703 RVA: 0x004401EE File Offset: 0x0043E3EE
		public void Close()
		{
			this.hasData = false;
			this.HalfChannels.Clear();
			this.FloatChannels.Clear();
		}

		// Token: 0x0600B670 RID: 46704 RVA: 0x0044020D File Offset: 0x0043E40D
		public void Open(IEXRReader reader)
		{
			this.hasData = true;
			this.ReadPixelData(reader);
		}

		// Token: 0x0600B671 RID: 46705 RVA: 0x00440220 File Offset: 0x0043E420
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReadPixelBlock(IEXRReader reader, uint offset, int linesPerBlock, List<Channel> sortedChannels)
		{
			reader.Position = (int)offset;
			if (this.Version.IsMultiPart)
			{
				reader.ReadUInt32();
				reader.ReadUInt32();
			}
			int num = reader.ReadInt32();
			int num2 = Math.Min(this.DataWindow.Height, num + linesPerBlock);
			int num3 = num * this.DataWindow.Width;
			reader.ReadInt32();
			if (this.Header.Compression != EXRCompression.None)
			{
				throw new NotImplementedException("Compressed images are currently not supported");
			}
			foreach (Channel channel in sortedChannels)
			{
				float[] array = null;
				Half[] array2 = null;
				if (channel.Type == PixelType.Float)
				{
					array = this.FloatChannels[channel.Name];
				}
				else
				{
					if (channel.Type != PixelType.Half)
					{
						throw new NotImplementedException();
					}
					array2 = this.HalfChannels[channel.Name];
				}
				int num4 = num3;
				for (int i = num; i < num2; i++)
				{
					int j = 0;
					while (j < this.DataWindow.Width)
					{
						if (channel.Type == PixelType.Float)
						{
							array[num4] = reader.ReadSingle();
						}
						else
						{
							if (channel.Type != PixelType.Half)
							{
								throw new NotImplementedException();
							}
							array2[num4] = reader.ReadHalf();
						}
						j++;
						num4++;
					}
				}
			}
		}

		// Token: 0x0600B672 RID: 46706 RVA: 0x004403A0 File Offset: 0x0043E5A0
		public void OpenParallel(string file)
		{
			this.Open(file);
		}

		// Token: 0x0600B673 RID: 46707 RVA: 0x004403AC File Offset: 0x0043E5AC
		public void OpenParallel(ParallelReaderCreationDelegate createReader)
		{
			IEXRReader iexrreader = createReader();
			this.Open(iexrreader);
			iexrreader.Dispose();
		}

		// Token: 0x0600B674 RID: 46708 RVA: 0x004403D0 File Offset: 0x0043E5D0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void ReadPixelData(IEXRReader reader)
		{
			int scanLinesPerBlock = EXRFile.GetScanLinesPerBlock(this.Header.Compression);
			List<Channel> sortedChannels = (from c in this.Header.Channels
			orderby c.Name
			select c).ToList<Channel>();
			foreach (uint offset in this.Offsets)
			{
				this.ReadPixelBlock(reader, offset, scanLinesPerBlock, sortedChannels);
			}
		}

		// Token: 0x17001649 RID: 5705
		// (get) Token: 0x0600B675 RID: 46709 RVA: 0x00440468 File Offset: 0x0043E668
		public bool IsRGB
		{
			get
			{
				return (this.HalfChannels.ContainsKey("R") || this.FloatChannels.ContainsKey("R")) && (this.HalfChannels.ContainsKey("G") || this.FloatChannels.ContainsKey("G")) && (this.HalfChannels.ContainsKey("B") || this.FloatChannels.ContainsKey("B"));
			}
		}

		// Token: 0x1700164A RID: 5706
		// (get) Token: 0x0600B676 RID: 46710 RVA: 0x004404E3 File Offset: 0x0043E6E3
		public bool HasAlpha
		{
			get
			{
				return this.HalfChannels.ContainsKey("A") || this.FloatChannels.ContainsKey("A");
			}
		}

		// Token: 0x040088C2 RID: 35010
		public readonly EXRVersion Version;

		// Token: 0x040088C3 RID: 35011
		public readonly EXRHeader Header;

		// Token: 0x040088C4 RID: 35012
		public readonly OffsetTable Offsets;

		// Token: 0x040088C5 RID: 35013
		public readonly PartType Type;

		// Token: 0x040088C6 RID: 35014
		public readonly Box2I DataWindow;

		// Token: 0x040088C7 RID: 35015
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasData;

		// Token: 0x040088C8 RID: 35016
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, float[]> floatChannels;

		// Token: 0x040088C9 RID: 35017
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, Half[]> halfChannels;
	}
}
