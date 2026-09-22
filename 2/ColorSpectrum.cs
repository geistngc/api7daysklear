using System;
using Unity.Collections;
using UnityEngine;

// Token: 0x020013C2 RID: 5058
public class ColorSpectrum
{
	// Token: 0x06009F16 RID: 40726 RVA: 0x003C1FE0 File Offset: 0x003C01E0
	public static ColorSpectrum FromTexture(string _filename)
	{
		Texture2D texture2D = DataLoader.LoadAsset<Texture2D>(_filename, false);
		if (texture2D == null)
		{
			return null;
		}
		ColorSpectrum result = new ColorSpectrum(_filename, texture2D);
		DataLoader.UnloadAsset(_filename, texture2D);
		return result;
	}

	// Token: 0x06009F17 RID: 40727 RVA: 0x003C200E File Offset: 0x003C020E
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsSupportedRawTextureFormat(TextureFormat _format)
	{
		return _format - TextureFormat.RGB24 <= 2 || _format == TextureFormat.BGRA32;
	}

	// Token: 0x06009F18 RID: 40728 RVA: 0x003C2020 File Offset: 0x003C0220
	public ColorSpectrum(string name, Texture2D _tex)
	{
		int width = _tex.width;
		this.values = new Color[width];
		if (!this.IsSupportedRawTextureFormat(_tex.format))
		{
			Log.Warning("Color Spectrum texture " + name + " is not in a format supported for non-allocating GetRawTextureData access. Falling back to GetPixels.");
			Color[] pixels = _tex.GetPixels();
			for (int i = 0; i < width; i++)
			{
				this.values[i] = pixels[i].linear;
			}
			return;
		}
		TextureFormat format = _tex.format;
		switch (format)
		{
		case TextureFormat.RGB24:
		{
			NativeArray<TextureUtils.ColorRGB24> rawTextureData = _tex.GetRawTextureData<TextureUtils.ColorRGB24>();
			for (int j = 0; j < width; j++)
			{
				this.values[j] = TextureUtils.GetLinearColor(rawTextureData[j]);
			}
			return;
		}
		case TextureFormat.RGBA32:
		{
			NativeArray<Color32> rawTextureData2 = _tex.GetRawTextureData<Color32>();
			for (int k = 0; k < width; k++)
			{
				this.values[k] = TextureUtils.GetLinearColor(rawTextureData2[k]);
			}
			return;
		}
		case TextureFormat.ARGB32:
		{
			NativeArray<TextureUtils.ColorARGB32> rawTextureData3 = _tex.GetRawTextureData<TextureUtils.ColorARGB32>();
			for (int l = 0; l < width; l++)
			{
				this.values[l] = TextureUtils.GetLinearColor(rawTextureData3[l]);
			}
			return;
		}
		default:
		{
			if (format != TextureFormat.BGRA32)
			{
				return;
			}
			NativeArray<TextureUtils.ColorBGRA32> rawTextureData4 = _tex.GetRawTextureData<TextureUtils.ColorBGRA32>();
			for (int m = 0; m < width; m++)
			{
				this.values[m] = TextureUtils.GetLinearColor(rawTextureData4[m]);
			}
			return;
		}
		}
	}

	// Token: 0x06009F19 RID: 40729 RVA: 0x003C2190 File Offset: 0x003C0390
	public Color GetValue(float _v)
	{
		int num = this.values.Length;
		float num2 = (float)num * _v;
		int num3 = (int)num2;
		Color a = this.values[num3];
		Color b = this.values[(num3 + 1) % num];
		return Color.LerpUnclamped(a, b, num2 - (float)num3);
	}

	// Token: 0x06009F1A RID: 40730 RVA: 0x003C21D5 File Offset: 0x003C03D5
	public static bool Exists(string _filename)
	{
		return Resources.Load(_filename) != null;
	}

	// Token: 0x040078D3 RID: 30931
	[PublicizedFrom(EAccessModifier.Private)]
	public Color[] values;
}
