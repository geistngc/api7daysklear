using System;
using System.Collections.Generic;
using Audio;
using MusicUtils.Enums;
using SandboxOptions;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000B3 RID: 179
[Serializable]
public class AudioObject
{
	// Token: 0x06000370 RID: 880 RVA: 0x00018EB0 File Offset: 0x000170B0
	public void Init()
	{
		foreach (AudioClip audioClip in this.audioClips)
		{
			if (audioClip != null)
			{
				AudioSource audioSource = UnityEngine.Object.Instantiate<AudioSource>(this.masterAudioSource);
				if (this.playOrder == AudioObject.PlayOrder.ByValue)
				{
					audioSource.transform.parent = EnvironmentAudioManager.Instance.transform;
					audioSource.loop = true;
					audioSource.gameObject.SetActive(false);
				}
				else
				{
					audioSource.transform.parent = EnvironmentAudioManager.sourceSounds.transform;
				}
				audioSource.clip = audioClip;
				audioSource.name = audioClip.name;
				audioSource.volume = 0f;
				audioSource.outputAudioMixerGroup = this.audioMixerGroup;
				this.runtimeAudioSrcs.Add(audioSource);
			}
		}
		this.audioClips = null;
		if (this.trigger == AudioObject.Trigger.Random)
		{
			this.repeatTime = Time.time + Manager.random.RandomRange(this.repeatFreqRange.x, this.repeatFreqRange.y);
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00018FB0 File Offset: 0x000171B0
	public void SetValue(float _value)
	{
		this.value = _value;
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			float num = this.transitionCurve.Evaluate(this.value) * (float)this.runtimeAudioSrcs.Count;
			int num2 = 0;
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				float num3 = Mathf.Clamp01(num - (float)num2);
				if (num > (float)(num2 + 1))
				{
					num3 = 1f - Mathf.Clamp01(num - (float)(num2 + 1));
				}
				audioSource.volume = num3 * (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
				if (GameManager.Instance != null && GameManager.Instance.World != null && GameManager.Instance.World.GetPrimaryPlayer() != null && GameManager.Instance.World.GetPrimaryPlayer().Stats != null)
				{
					if (this.outdoorOnly)
					{
						audioSource.volume *= EnvironmentAudioManager.Instance.invAmountEnclosedPow;
					}
					else if (this.indoorOnly)
					{
						audioSource.volume *= 1f - EnvironmentAudioManager.Instance.invAmountEnclosedPow;
					}
				}
				audioSource.gameObject.SetActive(audioSource.volume > 0f);
				if (audioSource.volume > 0f && !audioSource.isPlaying)
				{
					if (!audioSource.isActiveAndEnabled)
					{
						audioSource.gameObject.SetActive(true);
					}
					audioSource.Play();
				}
				num2++;
			}
		}
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00019168 File Offset: 0x00017368
	public void SetBiomeVolume(float _volume)
	{
		this.biomeVolume = _volume;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00019174 File Offset: 0x00017374
	public void Pause()
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.Pause();
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Pause();
				}
			}
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x000191F0 File Offset: 0x000173F0
	public void UnPause()
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.UnPause();
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource.volume > 0f)
				{
					audioSource.UnPause();
				}
			}
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00019274 File Offset: 0x00017474
	public void TurnOff(bool immediate = false)
	{
		this.fadingOut = true;
		if (immediate)
		{
			if (this.currentAudioSrc != null)
			{
				this.currentAudioSrc.Stop();
			}
			if (this.playOrder == AudioObject.PlayOrder.ByValue)
			{
				foreach (AudioSource audioSource in this.runtimeAudioSrcs)
				{
					audioSource.Stop();
				}
			}
			this.DestroySources();
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x000192F8 File Offset: 0x000174F8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool PlayConditionPasses()
	{
		World world = GameManager.Instance.World;
		EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
		if (primaryPlayer == null || primaryPlayer.Stats == null || primaryPlayer.IsDead())
		{
			return false;
		}
		if (WeatherManager.currentWeather == null)
		{
			return false;
		}
		if (WeatherManager.currentWeather.Wind() < this.minWind)
		{
			return false;
		}
		float num = world.GetWorldTime() / 24000f;
		float num2 = (num - (float)((int)num)) * 24f;
		float num3 = SkyManager.GetDawnTime() + this.dawnOffset;
		float num4 = SkyManager.GetDuskTime() + this.duskOffset;
		bool flag = SkyManager.IsBloodMoonVisible();
		bool flag2 = SkyManager.isAllTimeDay;
		bool flag3 = SkyManager.isAllTimeNight;
		if (flag2 || flag3)
		{
			num4 = 22f;
			num3 = 4f;
			if (SkyManager.WasBloodMoon)
			{
				flag2 = false;
				flag3 = false;
			}
		}
		if (this.outdoorOnly && primaryPlayer.Stats.AmountEnclosed >= 1f)
		{
			return false;
		}
		if (this.indoorOnly && primaryPlayer.Stats.AmountEnclosed <= 0f)
		{
			return false;
		}
		if (this.dayOnly && !flag2)
		{
			if (num2 < 12f && num2 < num3 - 0.02f)
			{
				return false;
			}
			if (num2 > 12f && num2 > num4 + 0.02f)
			{
				return false;
			}
		}
		if (this.nightOnly && !flag3)
		{
			if (num2 < 12f && num2 > num3 + 0.02f)
			{
				return false;
			}
			if (num2 > 12f && num2 < num4 - 0.02f)
			{
				return false;
			}
		}
		bool flag4 = false;
		ThreatLevelType category = primaryPlayer.ThreatLevel.Category;
		for (int i = 0; i < this.validThreatLevels.Length; i++)
		{
			if (this.validThreatLevels[i] == category)
			{
				flag4 = true;
				break;
			}
		}
		if (!flag4)
		{
			return false;
		}
		switch (this.trigger)
		{
		case AudioObject.Trigger.Dusk:
		{
			bool flag5 = EffectManager.GetValue(PassiveEffects.NoTimeDisplay, null, 0f, primaryPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f || !SandboxOptionManager.GetBool(SandboxOptions.ShowDayTime) || (flag2 || flag3);
			if (num4 > num2 + 0.01f || num4 < num2 - 0.01f || flag || flag5)
			{
				return false;
			}
			break;
		}
		case AudioObject.Trigger.Dawn:
		{
			bool flag6 = EffectManager.GetValue(PassiveEffects.NoTimeDisplay, null, 0f, primaryPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f || !SandboxOptionManager.GetBool(SandboxOptions.ShowDayTime) || (flag2 || flag3);
			if (num3 > num2 + 0.01f || num3 < num2 - 0.01f || flag6)
			{
				return false;
			}
			break;
		}
		case AudioObject.Trigger.Day7Dusk:
			if (!flag)
			{
				return false;
			}
			if (num4 > num2 + 0.01f || num4 < num2 - 0.01f)
			{
				return false;
			}
			break;
		case AudioObject.Trigger.Day8Dawn:
			if (SkyManager.dayCount - (float)(8 * ((int)SkyManager.dayCount / 8)) >= 1f)
			{
				return false;
			}
			if (num3 > num2 + 0.01f || num3 < num2 - 0.01f)
			{
				return false;
			}
			break;
		case AudioObject.Trigger.Random:
			if ((num2 > num4 - 0.25f && num2 < num4 + 0.25f) || (num2 > num3 - 0.25f && num2 < num3 + 0.25f))
			{
				return false;
			}
			if (world.dmsConductor != null && world.dmsConductor.IsMusicPlaying)
			{
				return false;
			}
			if (Time.time < this.repeatTime)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00019634 File Offset: 0x00017834
	public void DestroySources()
	{
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource != null)
				{
					UnityEngine.Object.DestroyImmediate(audioSource.gameObject);
				}
			}
			this.runtimeAudioSrcs.Clear();
		}
		if (this.currentAudioSrc != null)
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
			this.currentAudioSrc = null;
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x000196D0 File Offset: 0x000178D0
	public void SetVolume(float volume)
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.volume = volume;
		}
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000196EC File Offset: 0x000178EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayAtPoint()
	{
		if (this.currentAudioSrc != null)
		{
			if (this.playAtPosition != Vector3.zero)
			{
				this.currentAudioSrc.gameObject.transform.position = this.playAtPosition - Origin.position;
			}
			this.currentAudioSrc.Play();
		}
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0001974C File Offset: 0x0001794C
	public void Play()
	{
		int index = 0;
		AudioObject.PlayOrder playOrder = this.playOrder;
		if (playOrder != AudioObject.PlayOrder.Random)
		{
			if (playOrder == AudioObject.PlayOrder.FirstToLast)
			{
				this.currentPlayNum = 1;
			}
		}
		else
		{
			index = Manager.random.RandomRange(this.runtimeAudioSrcs.Count);
		}
		if (this.currentAudioSrc)
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
		}
		AudioSource original = this.runtimeAudioSrcs[index];
		this.currentAudioSrc = UnityEngine.Object.Instantiate<AudioSource>(original);
		GameObject gameObject = this.currentAudioSrc.gameObject;
		gameObject.name = this.name;
		gameObject.transform.SetParent(EnvironmentAudioManager.Instance.transform, false);
		gameObject.SetActive(true);
		this.currentAudioSrc.volume = (float)((this.trigger == AudioObject.Trigger.Thunder) ? 1 : 0);
		if (!this.currentAudioSrc.isPlaying)
		{
			this.PlayAtPoint();
		}
		if (this.trigger == AudioObject.Trigger.Continual)
		{
			this.currentAudioSrc.loop = true;
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00019835 File Offset: 0x00017A35
	public bool IsPlaying()
	{
		return !(this.currentAudioSrc == null) && this.currentAudioSrc.isPlaying;
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00019852 File Offset: 0x00017A52
	public void SetPosition(Vector3 _position)
	{
		this.playAtPosition = _position;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0001985C File Offset: 0x00017A5C
	public void Update(float deltaTime)
	{
		if (this.runtimeAudioSrcs.Count == 0)
		{
			return;
		}
		if (!(this.currentAudioSrc != null))
		{
			if (this.PlayConditionPasses())
			{
				this.DestroySources();
				this.value = 0f;
				this.fadingOut = false;
				this.playTime = 0f;
				this.loopTime = Time.time;
				this.Play();
			}
			return;
		}
		if (this.playOrder != AudioObject.PlayOrder.ByValue)
		{
			if (this.playOrder != AudioObject.PlayOrder.FirstToLast || this.currentPlayNum == 1 || this.fadingOut)
			{
				this.playTime += (this.fadingOut ? (-deltaTime * 0.02f) : deltaTime);
				float num = this.fadeInSec.Evaluate(this.playTime);
				float time = this.fadeInSec[this.fadeInSec.length - 1].time;
				if (this.playTime >= time)
				{
					this.playTime = time;
				}
				float num2 = num * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
				if (!this.name.Contains("Stinger"))
				{
					num2 *= (this.music ? EnvironmentAudioManager.musicVolume : 1f);
				}
				if (!this.fadingOut)
				{
					if (num2 == 0f)
					{
						num2 = 0.001f;
					}
				}
				else if (num2 < 0.01f)
				{
					num2 = 0f;
				}
				else
				{
					EnvironmentAudioManager.Instance.fadingBiomes = true;
				}
				this.currentAudioSrc.volume = num2;
			}
			else
			{
				this.currentAudioSrc.volume = (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
			}
			this.currentAudioSrc.gameObject.SetActive(this.currentAudioSrc.volume > 0f && this.currentAudioSrc.isPlaying);
			if (this.playOrder == AudioObject.PlayOrder.FirstToLast && this.currentPlayNum < this.runtimeAudioSrcs.Count && (this.currentAudioSrc.volume == 0f || !this.currentAudioSrc.isPlaying))
			{
				UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
				this.currentAudioSrc = null;
				List<AudioSource> list = this.runtimeAudioSrcs;
				int num3 = this.currentPlayNum;
				this.currentPlayNum = num3 + 1;
				this.currentAudioSrc = UnityEngine.Object.Instantiate<AudioSource>(list[num3]);
				this.currentAudioSrc.transform.parent = EnvironmentAudioManager.Instance.transform;
				this.currentAudioSrc.gameObject.name = this.name;
				this.currentAudioSrc.name = this.name;
				this.currentAudioSrc.gameObject.SetActive(true);
				this.currentAudioSrc.volume = EnvironmentAudioManager.GlobalEnvironmentVolumeScale * (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume;
				if (!this.currentAudioSrc.isPlaying)
				{
					this.PlayAtPoint();
				}
			}
		}
		if (this.outdoorOnly)
		{
			this.currentAudioSrc.volume = this.currentAudioSrc.volume * EnvironmentAudioManager.Instance.invAmountEnclosedPow;
		}
		else if (this.indoorOnly)
		{
			this.currentAudioSrc.volume = this.currentAudioSrc.volume * (1f - EnvironmentAudioManager.Instance.invAmountEnclosedPow);
		}
		if (this.loopDuration > 0f && Time.time > this.loopTime + this.loopDuration)
		{
			this.TurnOff(false);
		}
		float num4 = GameManager.Instance.World.GetWorldTime() / 24000f;
		float num5 = (num4 - (float)((int)num4)) * 24f;
		float num6 = SkyManager.GetDawnTime() + this.dawnOffset;
		float num7 = SkyManager.GetDuskTime() + this.duskOffset;
		if (SkyManager.isAllTimeDay)
		{
			if (this.nightOnly)
			{
				this.TurnOff(false);
			}
		}
		else if (SkyManager.isAllTimeNight)
		{
			if (this.dayOnly)
			{
				this.TurnOff(false);
			}
		}
		else
		{
			if (this.dayOnly && num5 < 12f && num5 < num6 - 0.02f)
			{
				this.TurnOff(false);
			}
			if (this.dayOnly && num5 > 12f && num5 > num7 + 0.02f)
			{
				this.TurnOff(false);
			}
			if (this.nightOnly && num5 < 12f && num5 > num6 + 0.02f)
			{
				this.TurnOff(false);
			}
			if (this.nightOnly && num5 > 12f && num5 < num7 - 0.02f)
			{
				this.TurnOff(false);
			}
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			return;
		}
		if (this.trigger == AudioObject.Trigger.Continual && this.currentAudioSrc != null && !this.currentAudioSrc.isPlaying && this.currentAudioSrc.volume > 0f)
		{
			if (!this.currentAudioSrc.isActiveAndEnabled)
			{
				this.currentAudioSrc.gameObject.SetActive(true);
			}
			if (this.currentAudioSrc.isActiveAndEnabled)
			{
				this.PlayAtPoint();
			}
		}
		if (!(this.currentAudioSrc != null) || !this.currentAudioSrc.isPlaying || (this.fadingOut && this.currentAudioSrc.volume <= 0f))
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
			this.currentAudioSrc = null;
			if (this.trigger == AudioObject.Trigger.Random)
			{
				this.repeatTime = Time.time + Manager.random.RandomRange(this.repeatFreqRange.x, this.repeatFreqRange.y);
			}
		}
	}

	// Token: 0x040003D7 RID: 983
	public static Dictionary<byte, BiomeDefinition.BiomeType> biomeIdMap;

	// Token: 0x040003D8 RID: 984
	public string name;

	// Token: 0x040003D9 RID: 985
	public AudioMixerGroup audioMixerGroup;

	// Token: 0x040003DA RID: 986
	public AudioSource masterAudioSource;

	// Token: 0x040003DB RID: 987
	public AudioClip[] audioClips;

	// Token: 0x040003DC RID: 988
	public List<AudioSource> runtimeAudioSrcs = new List<AudioSource>();

	// Token: 0x040003DD RID: 989
	public AudioObject.Trigger trigger;

	// Token: 0x040003DE RID: 990
	public bool indoorOnly;

	// Token: 0x040003DF RID: 991
	public bool outdoorOnly;

	// Token: 0x040003E0 RID: 992
	public bool dayOnly;

	// Token: 0x040003E1 RID: 993
	public bool nightOnly;

	// Token: 0x040003E2 RID: 994
	public float duskOffset;

	// Token: 0x040003E3 RID: 995
	public float dawnOffset;

	// Token: 0x040003E4 RID: 996
	public float minWind;

	// Token: 0x040003E5 RID: 997
	public bool affectedByEnv;

	// Token: 0x040003E6 RID: 998
	public AudioObject.PlayOrder playOrder;

	// Token: 0x040003E7 RID: 999
	public BiomeDefinition.BiomeType[] validBiomes;

	// Token: 0x040003E8 RID: 1000
	public AnimationCurve fadeInSec = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040003E9 RID: 1001
	public AnimationCurve transitionCurve;

	// Token: 0x040003EA RID: 1002
	public Vector2 repeatFreqRange;

	// Token: 0x040003EB RID: 1003
	public float loopDuration;

	// Token: 0x040003EC RID: 1004
	public bool music;

	// Token: 0x040003ED RID: 1005
	public ThreatLevelType[] validThreatLevels;

	// Token: 0x040003EE RID: 1006
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource currentAudioSrc;

	// Token: 0x040003EF RID: 1007
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float loopTime;

	// Token: 0x040003F0 RID: 1008
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float repeatTime;

	// Token: 0x040003F1 RID: 1009
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool fadingOut;

	// Token: 0x040003F2 RID: 1010
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float value;

	// Token: 0x040003F3 RID: 1011
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float playTime;

	// Token: 0x040003F4 RID: 1012
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float biomeVolume = 1f;

	// Token: 0x040003F5 RID: 1013
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentPlayNum;

	// Token: 0x040003F6 RID: 1014
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 playAtPosition;

	// Token: 0x040003F7 RID: 1015
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float timeEpsilon = 0.01f;

	// Token: 0x020000B4 RID: 180
	public enum Trigger
	{
		// Token: 0x040003F9 RID: 1017
		Rain,
		// Token: 0x040003FA RID: 1018
		Snow,
		// Token: 0x040003FB RID: 1019
		Thunder,
		// Token: 0x040003FC RID: 1020
		TimeOfDay,
		// Token: 0x040003FD RID: 1021
		Dusk,
		// Token: 0x040003FE RID: 1022
		Dawn,
		// Token: 0x040003FF RID: 1023
		Day7Times,
		// Token: 0x04000400 RID: 1024
		Day7Dusk,
		// Token: 0x04000401 RID: 1025
		Day8Dawn,
		// Token: 0x04000402 RID: 1026
		Random,
		// Token: 0x04000403 RID: 1027
		Continual,
		// Token: 0x04000404 RID: 1028
		Wind
	}

	// Token: 0x020000B5 RID: 181
	public enum PlayOrder
	{
		// Token: 0x04000406 RID: 1030
		Random,
		// Token: 0x04000407 RID: 1031
		FirstToLast,
		// Token: 0x04000408 RID: 1032
		ByValue
	}
}
