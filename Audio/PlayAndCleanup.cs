using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001B1B RID: 6939
	public class PlayAndCleanup
	{
		// Token: 0x0600D03B RID: 53307 RVA: 0x004BF3D8 File Offset: 0x004BD5D8
		public PlayAndCleanup(LoopingPair _lp, string _soundGroupName)
		{
			this.lp = _lp;
			double num;
			this.lp.sgoBegin.src.PlayScheduled(num = AudioSettings.dspTime + 0.05);
			this.lp.sgoLoop.src.PlayScheduled(num + (double)this.lp.sgoBegin.src.clip.samples / 44100.0);
			Manager.AddPlayingAudioSource(this.lp.sgoBegin.src);
			Manager.AddPlayingAudioSource(this.lp.sgoLoop.src);
			if (!string.IsNullOrEmpty(_soundGroupName))
			{
				Manager.AddPlayAndCleanupAudioSource(_soundGroupName, this.lp.sgoBegin.src);
				Manager.AddPlayAndCleanupAudioSource(_soundGroupName, this.lp.sgoLoop.src);
			}
			GameManager.Instance.StartCoroutine(this.StopBeginWhenDone(this.lp.sgoBegin.src.clip.length, _soundGroupName));
		}

		// Token: 0x0600D03C RID: 53308 RVA: 0x004BF4DE File Offset: 0x004BD6DE
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopBeginWhenDone(float waitTime, string soundGroupName)
		{
			yield return new WaitForSeconds(waitTime + 0.1f);
			if (GameManager.Instance.IsPaused())
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.lp.sgoBegin.src == null)
			{
				yield break;
			}
			if (this.lp.sgoBegin.src.isPlaying)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.lp.sgoBegin.src == null)
			{
				yield break;
			}
			Manager.RemovePlayingAudioSource(this.lp.sgoBegin.src);
			if (!string.IsNullOrEmpty(soundGroupName))
			{
				Manager.RemovePlayAndCleanupAudioSource(soundGroupName, this.lp.sgoBegin.src);
			}
			if (this.lp.sgoBegin.go)
			{
				UnityEngine.Object.Destroy(this.lp.sgoBegin.go);
			}
			yield break;
		}

		// Token: 0x0600D03D RID: 53309 RVA: 0x004BF4FC File Offset: 0x004BD6FC
		public PlayAndCleanup(GameObject _go, AudioSource _source, float _occlusion = 0f, float delay = 0f, bool isLooping = false, bool hasLoopingAnalog = false, string soundGroupName = "")
		{
			this.go = _go;
			this.src = _source;
			float num = 1f - _occlusion;
			float num2 = Utils.FastAbs(Manager.currentListenerPosition.y - this.go.transform.position.y);
			num2 = Utils.FastClamp01(num2 / 30f);
			this.src.volume *= (1f - num2) * num;
			if (num < 0.95f)
			{
				this.go.AddComponent<AudioLowPassFilter>().cutoffFrequency = Utils.FastLerp(10f, 5000f, Mathf.Pow(num, 2f));
			}
			if (delay > 0f)
			{
				this.src.PlayDelayed(delay);
			}
			else
			{
				Manager.PlaySource(this.src);
			}
			Manager.AddPlayingAudioSource(this.src);
			if (!string.IsNullOrEmpty(soundGroupName))
			{
				Manager.AddPlayAndCleanupAudioSource(soundGroupName, this.src);
			}
			if (!isLooping)
			{
				float waitTime = _source.clip.length * (1f + Utils.FastClamp01(1f - _source.pitch)) + delay;
				GameManager.Instance.StartCoroutine(this.StopWhenDone(waitTime, soundGroupName));
			}
		}

		// Token: 0x0600D03E RID: 53310 RVA: 0x004BF628 File Offset: 0x004BD828
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopWhenDone(float waitTime, string soundGroupName)
		{
			yield return new WaitForSeconds(waitTime + 0.001f);
			if (GameManager.Instance.IsPaused())
			{
				yield return new WaitForSeconds(0.1f);
			}
			Manager.RemovePlayingAudioSource(this.src);
			if (!string.IsNullOrEmpty(soundGroupName))
			{
				Manager.RemovePlayAndCleanupAudioSource(soundGroupName, this.src);
			}
			if (this.go)
			{
				UnityEngine.Object.Destroy(this.go);
			}
			yield break;
		}

		// Token: 0x04009EEB RID: 40683
		[PublicizedFrom(EAccessModifier.Private)]
		public GameObject go;

		// Token: 0x04009EEC RID: 40684
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource src;

		// Token: 0x04009EED RID: 40685
		[PublicizedFrom(EAccessModifier.Private)]
		public LoopingPair lp;
	}
}
