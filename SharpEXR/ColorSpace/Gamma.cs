using System;

namespace SharpEXR.ColorSpace
{
	// Token: 0x020016DF RID: 5855
	public static class Gamma
	{
		// Token: 0x0600B719 RID: 46873 RVA: 0x00441499 File Offset: 0x0043F699
		public static float Expand(float nonlinear)
		{
			return (float)Math.Pow((double)nonlinear, 2.2);
		}

		// Token: 0x0600B71A RID: 46874 RVA: 0x004414AC File Offset: 0x0043F6AC
		public static float Compress(float linear)
		{
			return (float)Math.Pow((double)linear, 0.45454545454545453);
		}

		// Token: 0x0600B71B RID: 46875 RVA: 0x004414BF File Offset: 0x0043F6BF
		public static void Expand(ref tVec3 pColor)
		{
			pColor.X = Gamma.Expand(pColor.X);
			pColor.Y = Gamma.Expand(pColor.Y);
			pColor.Z = Gamma.Expand(pColor.Z);
		}

		// Token: 0x0600B71C RID: 46876 RVA: 0x004414F4 File Offset: 0x0043F6F4
		public static void Compress(ref tVec3 pColor)
		{
			pColor.X = Gamma.Compress(pColor.X);
			pColor.Y = Gamma.Compress(pColor.Y);
			pColor.Z = Gamma.Compress(pColor.Z);
		}

		// Token: 0x0600B71D RID: 46877 RVA: 0x00441529 File Offset: 0x0043F729
		public static void Expand(ref float r, ref float g, ref float b)
		{
			r = Gamma.Expand(r);
			g = Gamma.Expand(g);
			b = Gamma.Expand(b);
		}

		// Token: 0x0600B71E RID: 46878 RVA: 0x00441546 File Offset: 0x0043F746
		public static void Compress(ref float r, ref float g, ref float b)
		{
			r = Gamma.Compress(r);
			g = Gamma.Compress(g);
			b = Gamma.Compress(b);
		}

		// Token: 0x0600B71F RID: 46879 RVA: 0x00441564 File Offset: 0x0043F764
		public static tVec3 Expand(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Expand(ref result);
			return result;
		}

		// Token: 0x0600B720 RID: 46880 RVA: 0x00441584 File Offset: 0x0043F784
		public static tVec3 Compress(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Compress(ref result);
			return result;
		}

		// Token: 0x0600B721 RID: 46881 RVA: 0x004415A3 File Offset: 0x0043F7A3
		public static float Expand_sRGB(float nonlinear)
		{
			if (nonlinear > 0.04045f)
			{
				return (float)Math.Pow((double)((nonlinear + 0.055f) / 1.055f), 2.4000000953674316);
			}
			return nonlinear / 12.92f;
		}

		// Token: 0x0600B722 RID: 46882 RVA: 0x004415D2 File Offset: 0x0043F7D2
		public static float Compress_sRGB(float linear)
		{
			if (linear > 0.0031308f)
			{
				return 1.055f * (float)Math.Pow((double)linear, 0.4166666567325592) - 0.055f;
			}
			return 12.92f * linear;
		}

		// Token: 0x0600B723 RID: 46883 RVA: 0x00441601 File Offset: 0x0043F801
		public static void Expand_sRGB(ref tVec3 pColor)
		{
			pColor.X = Gamma.Expand_sRGB(pColor.X);
			pColor.Y = Gamma.Expand_sRGB(pColor.Y);
			pColor.Z = Gamma.Expand_sRGB(pColor.Z);
		}

		// Token: 0x0600B724 RID: 46884 RVA: 0x00441636 File Offset: 0x0043F836
		public static void Compress_sRGB(ref tVec3 pColor)
		{
			pColor.X = Gamma.Compress_sRGB(pColor.X);
			pColor.Y = Gamma.Compress_sRGB(pColor.Y);
			pColor.Z = Gamma.Compress_sRGB(pColor.Z);
		}

		// Token: 0x0600B725 RID: 46885 RVA: 0x0044166B File Offset: 0x0043F86B
		public static void Expand_sRGB(ref float r, ref float g, ref float b)
		{
			r = Gamma.Expand_sRGB(r);
			g = Gamma.Expand_sRGB(g);
			b = Gamma.Expand_sRGB(b);
		}

		// Token: 0x0600B726 RID: 46886 RVA: 0x00441688 File Offset: 0x0043F888
		public static void Compress_sRGB(ref float r, ref float g, ref float b)
		{
			r = Gamma.Compress_sRGB(r);
			g = Gamma.Compress_sRGB(g);
			b = Gamma.Compress_sRGB(b);
		}

		// Token: 0x0600B727 RID: 46887 RVA: 0x004416A8 File Offset: 0x0043F8A8
		public static tVec3 Expand_sRGB(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Expand_sRGB(ref result);
			return result;
		}

		// Token: 0x0600B728 RID: 46888 RVA: 0x004416C8 File Offset: 0x0043F8C8
		public static tVec3 Compress_sRGB(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Compress_sRGB(ref result);
			return result;
		}
	}
}
