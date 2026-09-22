using System;
using UnityEngine;

// Token: 0x020014E6 RID: 5350
public class AudioGamepadRumbleSource
{
	// Token: 0x0600A7EF RID: 42991 RVA: 0x003E9C8D File Offset: 0x003E7E8D
	public AudioGamepadRumbleSource()
	{
		this.samples = new float[64];
	}

	// Token: 0x0600A7F0 RID: 42992 RVA: 0x003E9CA2 File Offset: 0x003E7EA2
	public void SetAudioSource(AudioSource _audioSource, float _strengthMultiplier, bool _locationBased)
	{
		this.audioSrc = _audioSource;
		this.strengthMultiplier = _strengthMultiplier;
		this.locationBased = _locationBased;
		this.timeAdded = Time.time;
	}

	// Token: 0x0600A7F1 RID: 42993 RVA: 0x003E9CC4 File Offset: 0x003E7EC4
	public float GetSample(int channel)
	{
		this.audioSrc.GetOutputData(this.samples, channel);
		float num = 0f;
		for (int i = 0; i < 64; i++)
		{
			num += this.samples[i];
		}
		return num / 64f;
	}

	// Token: 0x0600A7F2 RID: 42994 RVA: 0x003E9D0A File Offset: 0x003E7F0A
	public void Clear()
	{
		this.audioSrc = null;
	}

	// Token: 0x04007CF6 RID: 31990
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSampleCount = 64;

	// Token: 0x04007CF7 RID: 31991
	public AudioSource audioSrc;

	// Token: 0x04007CF8 RID: 31992
	public float[] samples;

	// Token: 0x04007CF9 RID: 31993
	public float strengthMultiplier;

	// Token: 0x04007CFA RID: 31994
	public bool locationBased;

	// Token: 0x04007CFB RID: 31995
	public float timeAdded;
}
