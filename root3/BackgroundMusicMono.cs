using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class BackgroundMusicMono : SingletonMonoBehaviour<BackgroundMusicMono>
{
	// Token: 0x06002CE6 RID: 11494 RVA: 0x0011F534 File Offset: 0x0011D734
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		AudioListener[] array = UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].enabled)
			{
				base.transform.position = array[i].transform.position;
				break;
			}
		}
		AudioListener.volume = Mathf.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
		this.AddMusicTrack(BackgroundMusicMono.MusicTrack.None, null);
		this.AddMusicTrack(BackgroundMusicMono.MusicTrack.BackgroundMusic, GameManager.Instance.BackgroundMusicClip);
		this.AddMusicTrack(BackgroundMusicMono.MusicTrack.CreditsSong, GameManager.Instance.CreditsSongClip);
		this.Play(BackgroundMusicMono.MusicTrack.BackgroundMusic);
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x0011F5C0 File Offset: 0x0011D7C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (GameStats.GetInt(EnumGameStats.GameState) == 1 && !GameManager.Instance.IsPaused())
		{
			this.Play(BackgroundMusicMono.MusicTrack.None);
		}
		else if (LocalPlayerUI.primaryUI.windowManager.IsWindowOpen(XUiC_Credits.ID))
		{
			this.Play(BackgroundMusicMono.MusicTrack.CreditsSong);
		}
		else
		{
			this.Play(BackgroundMusicMono.MusicTrack.BackgroundMusic);
		}
		this.activeTracks.RemoveWhere((BackgroundMusicMono.MusicTrack activeTrack) => !this.UpdateTrack(activeTrack));
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x0011F62C File Offset: 0x0011D82C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddMusicTrack(BackgroundMusicMono.MusicTrack musicTrack, AudioClip audioClip)
	{
		if (!audioClip)
		{
			this.musicTrackStates.Add(musicTrack, new BackgroundMusicMono.MusicTrackState(null));
			return;
		}
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.volume = 0f;
		audioSource.clip = audioClip;
		audioSource.loop = true;
		this.musicTrackStates.Add(musicTrack, new BackgroundMusicMono.MusicTrackState(audioSource));
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x0011F68B File Offset: 0x0011D88B
	[PublicizedFrom(EAccessModifier.Private)]
	public void Play(BackgroundMusicMono.MusicTrack musicTrack)
	{
		if (this.currentlyPlaying == musicTrack)
		{
			return;
		}
		this.currentlyPlaying = musicTrack;
		this.activeTracks.Add(musicTrack);
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x0011F6AC File Offset: 0x0011D8AC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateTrack(BackgroundMusicMono.MusicTrack activeTrack)
	{
		BackgroundMusicMono.MusicTrackState musicTrackState = this.musicTrackStates[activeTrack];
		AudioSource audioSource = musicTrackState.AudioSource;
		if (!audioSource)
		{
			return false;
		}
		float num = musicTrackState.CurrentVolume;
		if (activeTrack == this.currentlyPlaying)
		{
			num += Time.deltaTime / 3f;
		}
		else
		{
			num -= Time.deltaTime / 3f;
		}
		num = Mathf.Clamp01(num);
		musicTrackState.CurrentVolume = num;
		bool flag = activeTrack == this.currentlyPlaying || num > 0f;
		audioSource.volume = Mathf.Clamp01(GamePrefs.GetFloat(EnumGamePrefs.OptionsMenuMusicVolumeLevel) * num);
		if (audioSource.isPlaying == flag)
		{
			return flag;
		}
		if (flag)
		{
			audioSource.Play();
		}
		else
		{
			audioSource.Stop();
		}
		return flag;
	}

	// Token: 0x0400233B RID: 9019
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float secondsToFadeOut = 3f;

	// Token: 0x0400233C RID: 9020
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float secondsToFadeIn = 3f;

	// Token: 0x0400233D RID: 9021
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly EnumDictionary<BackgroundMusicMono.MusicTrack, BackgroundMusicMono.MusicTrackState> musicTrackStates = new EnumDictionary<BackgroundMusicMono.MusicTrack, BackgroundMusicMono.MusicTrackState>();

	// Token: 0x0400233E RID: 9022
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly HashSet<BackgroundMusicMono.MusicTrack> activeTracks = new HashSet<BackgroundMusicMono.MusicTrack>();

	// Token: 0x0400233F RID: 9023
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BackgroundMusicMono.MusicTrack currentlyPlaying;

	// Token: 0x02000567 RID: 1383
	[PublicizedFrom(EAccessModifier.Private)]
	public enum MusicTrack
	{
		// Token: 0x04002341 RID: 9025
		None,
		// Token: 0x04002342 RID: 9026
		BackgroundMusic,
		// Token: 0x04002343 RID: 9027
		CreditsSong
	}

	// Token: 0x02000568 RID: 1384
	[PublicizedFrom(EAccessModifier.Private)]
	public class MusicTrackState
	{
		// Token: 0x06002CED RID: 11501 RVA: 0x0011F782 File Offset: 0x0011D982
		public MusicTrackState(AudioSource audioSource)
		{
			this.AudioSource = audioSource;
		}

		// Token: 0x04002344 RID: 9028
		public readonly AudioSource AudioSource;

		// Token: 0x04002345 RID: 9029
		public float CurrentVolume;
	}
}
