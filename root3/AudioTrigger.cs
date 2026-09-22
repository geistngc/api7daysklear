using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B7 RID: 183
[PublicizedFrom(EAccessModifier.Internal)]
public class AudioTrigger
{
	// Token: 0x06000387 RID: 903 RVA: 0x0001A056 File Offset: 0x00018256
	public AudioTrigger(AudioObject.Trigger _trigger)
	{
		this.trigger = _trigger;
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0001A070 File Offset: 0x00018270
	public void Add(AudioObject _audioObject)
	{
		this.sound.Add(_audioObject);
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0001A080 File Offset: 0x00018280
	public void Update()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].Update(fixedDeltaTime);
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0001A0C0 File Offset: 0x000182C0
	public void SetVolume(float _vol)
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].SetBiomeVolume(_vol);
		}
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0001A0F8 File Offset: 0x000182F8
	public void Pause()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].Pause();
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0001A130 File Offset: 0x00018330
	public void UnPause()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].UnPause();
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0001A168 File Offset: 0x00018368
	public void TurnOff()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].TurnOff(false);
		}
	}

	// Token: 0x04000413 RID: 1043
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AudioObject.Trigger trigger;

	// Token: 0x04000414 RID: 1044
	public List<AudioObject> sound = new List<AudioObject>();
}
