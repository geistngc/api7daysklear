using System;

// Token: 0x02000AF8 RID: 2808
public class AtmosphereEffect
{
	// Token: 0x060053AF RID: 21423 RVA: 0x00200F58 File Offset: 0x001FF158
	public static AtmosphereEffect Load(string _folder, AtmosphereEffect _default)
	{
		AtmosphereEffect atmosphereEffect = new AtmosphereEffect();
		string str = "@:Textures/Environment/Spectrums/" + ((_folder != null) ? (_folder + "/") : "");
		atmosphereEffect.spectrums[0] = ColorSpectrum.FromTexture(str + "sky.tga");
		atmosphereEffect.spectrums[1] = ColorSpectrum.FromTexture(str + "ambient.tga");
		atmosphereEffect.spectrums[2] = ColorSpectrum.FromTexture(str + "sun.tga");
		atmosphereEffect.spectrums[3] = ColorSpectrum.FromTexture(str + "moon.tga");
		atmosphereEffect.spectrums[4] = ColorSpectrum.FromTexture(str + "fog.tga");
		atmosphereEffect.spectrums[5] = ColorSpectrum.FromTexture(str + "fogfade.tga");
		if (_default != null)
		{
			for (int i = 0; i < atmosphereEffect.spectrums.Length; i++)
			{
				if (atmosphereEffect.spectrums[i] == null)
				{
					atmosphereEffect.spectrums[i] = _default.spectrums[i];
				}
			}
		}
		return atmosphereEffect;
	}

	// Token: 0x0400416D RID: 16749
	public ColorSpectrum[] spectrums = new ColorSpectrum[6];

	// Token: 0x02000AF9 RID: 2809
	public enum ESpecIdx
	{
		// Token: 0x0400416F RID: 16751
		Sky,
		// Token: 0x04004170 RID: 16752
		Ambient,
		// Token: 0x04004171 RID: 16753
		Sun,
		// Token: 0x04004172 RID: 16754
		Moon,
		// Token: 0x04004173 RID: 16755
		Fog,
		// Token: 0x04004174 RID: 16756
		FogFade,
		// Token: 0x04004175 RID: 16757
		Count
	}
}
