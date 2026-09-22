using System;

// Token: 0x020000B0 RID: 176
[PublicizedFrom(EAccessModifier.Internal)]
public class AudioBiome
{
	// Token: 0x06000365 RID: 869 RVA: 0x00018BEC File Offset: 0x00016DEC
	public AudioBiome()
	{
		int num = Enum.GetNames(typeof(AudioObject.Trigger)).Length;
		this.triggers = new AudioTrigger[num];
		for (int i = 0; i < num; i++)
		{
			this.triggers[i] = new AudioTrigger((AudioObject.Trigger)i);
		}
	}

	// Token: 0x06000366 RID: 870 RVA: 0x00018C37 File Offset: 0x00016E37
	public void Add(AudioObject _audioObject)
	{
		this.triggers[(int)_audioObject.trigger].Add(_audioObject);
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00018C4C File Offset: 0x00016E4C
	public void TransitionFrom(float _biomeTransition)
	{
		for (int i = 3; i < this.triggers.Length - 1; i++)
		{
			AudioTrigger audioTrigger = this.triggers[i];
			if (i != 5 && i != 4)
			{
				audioTrigger.TurnOff();
				audioTrigger.SetVolume(1f - _biomeTransition);
			}
			audioTrigger.Update();
		}
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00018C98 File Offset: 0x00016E98
	public void TransitionTo(float _biomeTransition)
	{
		for (int i = 3; i < this.triggers.Length - 1; i++)
		{
			AudioTrigger audioTrigger = this.triggers[i];
			if (i != 5 && i != 4)
			{
				audioTrigger.SetVolume(_biomeTransition);
			}
			audioTrigger.Update();
		}
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00018CD8 File Offset: 0x00016ED8
	public void Pause()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].Pause();
		}
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00018D08 File Offset: 0x00016F08
	public void UnPause()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].UnPause();
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00018D38 File Offset: 0x00016F38
	public void TurnOff()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].TurnOff();
		}
	}

	// Token: 0x040003CD RID: 973
	public AudioTrigger[] triggers;
}
