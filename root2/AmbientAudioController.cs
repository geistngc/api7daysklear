using System;
using MusicUtils;
using UnityEngine.Audio;

// Token: 0x020000AE RID: 174
public class AmbientAudioController : IGamePrefsChangedListener
{
	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000351 RID: 849 RVA: 0x0001877E File Offset: 0x0001697E
	public static AmbientAudioController Instance
	{
		get
		{
			if (AmbientAudioController.instance == null)
			{
				AmbientAudioController.instance = new AmbientAudioController();
			}
			return AmbientAudioController.instance;
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00018796 File Offset: 0x00016996
	[PublicizedFrom(EAccessModifier.Private)]
	public AmbientAudioController()
	{
		GamePrefs.AddChangeListener(this);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x000187F1 File Offset: 0x000169F1
	public void SetAmbientVolume(float _val)
	{
		if (AmbientAudioController.master)
		{
			AmbientAudioController.master.SetFloat("ambVol", AmbientAudioController.volumeCurve.GetMixerValue(_val));
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0001881A File Offset: 0x00016A1A
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsAmbientVolumeLevel)
		{
			this.SetAmbientVolume(GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel));
		}
	}

	// Token: 0x040003C8 RID: 968
	[PublicizedFrom(EAccessModifier.Private)]
	public static AmbientAudioController instance;

	// Token: 0x040003C9 RID: 969
	[PublicizedFrom(EAccessModifier.Private)]
	public static AudioMixer master = DataLoader.LoadAsset<AudioMixer>("@:Sound_Mixers/MasterAudioMixer.mixer", false);

	// Token: 0x040003CA RID: 970
	[PublicizedFrom(EAccessModifier.Private)]
	public static LogarithmicCurve volumeCurve = new LogarithmicCurve(2.0, 6.0, -80f, 0f, 0f, 1f);
}
