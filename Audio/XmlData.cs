using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001B14 RID: 6932
	public class XmlData
	{
		// Token: 0x0600D033 RID: 53299 RVA: 0x004BF12C File Offset: 0x004BD32C
		public XmlData()
		{
			this.soundGroupName = "Invalid";
			this.maxVoices = 1;
			this.maxVoicesPerEntity = 5;
			this.audioClipMap = new List<ClipSourceMap>();
			this.noiseData = new NoiseData();
			this.localCrouchVolumeScale = 0.5f;
			this.crouchNoiseScale = 0.5f;
			this.noiseScale = 1f;
			this.maxRepeatRate = 0.1f;
			this.prioritizeNewNodes = false;
			this.stopOldestNode = false;
			this.voicesPlaying = 0;
			this.lastRecordedPlayTime = Time.time;
			this.lastClipLenght = 0f;
			this.maxVolume = 1f;
			this.sequence = false;
			this.runningVolumeScale = 1f;
			this.lowestPitch = 1f;
			this.highestPitch = 1f;
			this.distantFadeStart = -1f;
			this.distantFadeEnd = -1f;
			this.channel = XmlData.Channel.Environment;
			this.priority = 99;
		}

		// Token: 0x0600D034 RID: 53300 RVA: 0x004BF230 File Offset: 0x004BD430
		public bool CanAddANewVoice()
		{
			if (this.maxRepeatRate > 0f)
			{
				float time = Time.time;
				float num = time - this.lastRecordedPlayTime;
				if (num < this.maxRepeatRate)
				{
					return false;
				}
				float num2 = this.maxRepeatRate;
				if (this.prioritizeNewNodes && this.lastClipLenght > 0f)
				{
					num2 = this.lastClipLenght;
				}
				this.voicesPlaying = Utils.FastClamp(this.voicesPlaying - (int)(num / num2), 0, 999);
				if (this.voicesPlaying >= this.maxVoices)
				{
					if (this.prioritizeNewNodes)
					{
						this.stopOldestNode = true;
						return true;
					}
					return false;
				}
				else
				{
					this.voicesPlaying++;
					this.lastRecordedPlayTime = time;
				}
			}
			return true;
		}

		// Token: 0x0600D035 RID: 53301 RVA: 0x004BF2DD File Offset: 0x004BD4DD
		public List<ClipSourceMap> GetClipList()
		{
			if (Manager.Instance.bUseAltSounds && this.altAudioClipMap != null)
			{
				return this.altAudioClipMap;
			}
			if (this.hasProfanity && GamePrefs.GetBool(EnumGamePrefs.OptionsFilterProfanity))
			{
				return this.cleanClipMap;
			}
			return this.audioClipMap;
		}

		// Token: 0x0600D036 RID: 53302 RVA: 0x004BF31C File Offset: 0x004BD51C
		public ClipSourceMap GetRandomClip()
		{
			List<ClipSourceMap> clipList = this.GetClipList();
			int num = 0;
			int count = clipList.Count;
			if (count > 1)
			{
				if (count == 2)
				{
					num = (this.randomLastIndex ^ 1);
				}
				else
				{
					num = Manager.random.RandomRange(count - 1);
					if (num >= this.randomLastIndex)
					{
						num++;
					}
				}
				this.randomLastIndex = num;
			}
			else if (count == 0)
			{
				Log.Warning("No Clips in Audio ClipSourceMap " + this.soundGroupName + ", " + ((this.hasProfanity && GamePrefs.GetBool(EnumGamePrefs.OptionsFilterProfanity)) ? "using 'no profanity' map:" : ""));
				return null;
			}
			return clipList[num];
		}

		// Token: 0x0600D037 RID: 53303 RVA: 0x004BF3B6 File Offset: 0x004BD5B6
		public void AddAltClipSourceMap(ClipSourceMap csm)
		{
			if (this.altAudioClipMap == null)
			{
				this.altAudioClipMap = new List<ClipSourceMap>();
			}
			this.altAudioClipMap.Add(csm);
		}

		// Token: 0x04009EB8 RID: 40632
		public string soundGroupName;

		// Token: 0x04009EB9 RID: 40633
		public int maxVoices;

		// Token: 0x04009EBA RID: 40634
		public List<ClipSourceMap> audioClipMap;

		// Token: 0x04009EBB RID: 40635
		public List<ClipSourceMap> altAudioClipMap;

		// Token: 0x04009EBC RID: 40636
		public List<ClipSourceMap> cleanClipMap;

		// Token: 0x04009EBD RID: 40637
		public NoiseData noiseData;

		// Token: 0x04009EBE RID: 40638
		public float localCrouchVolumeScale;

		// Token: 0x04009EBF RID: 40639
		public float runningVolumeScale;

		// Token: 0x04009EC0 RID: 40640
		public float crouchNoiseScale;

		// Token: 0x04009EC1 RID: 40641
		public float noiseScale;

		// Token: 0x04009EC2 RID: 40642
		public float maxRepeatRate;

		// Token: 0x04009EC3 RID: 40643
		public bool prioritizeNewNodes;

		// Token: 0x04009EC4 RID: 40644
		public bool stopOldestNode;

		// Token: 0x04009EC5 RID: 40645
		public int voicesPlaying;

		// Token: 0x04009EC6 RID: 40646
		public float lastRecordedPlayTime;

		// Token: 0x04009EC7 RID: 40647
		public float lastClipLenght;

		// Token: 0x04009EC8 RID: 40648
		public bool playImmediate;

		// Token: 0x04009EC9 RID: 40649
		public bool sequence;

		// Token: 0x04009ECA RID: 40650
		public float maxVolume;

		// Token: 0x04009ECB RID: 40651
		public float lowestPitch;

		// Token: 0x04009ECC RID: 40652
		public float highestPitch;

		// Token: 0x04009ECD RID: 40653
		public float distantFadeStart;

		// Token: 0x04009ECE RID: 40654
		public float distantFadeEnd;

		// Token: 0x04009ECF RID: 40655
		public int maxVoicesPerEntity;

		// Token: 0x04009ED0 RID: 40656
		public bool hasProfanity;

		// Token: 0x04009ED1 RID: 40657
		public XmlData.Channel channel;

		// Token: 0x04009ED2 RID: 40658
		public int priority;

		// Token: 0x04009ED3 RID: 40659
		public bool vibratesController = true;

		// Token: 0x04009ED4 RID: 40660
		public float vibrationStrengthMultiplier = 1f;

		// Token: 0x04009ED5 RID: 40661
		[PublicizedFrom(EAccessModifier.Private)]
		public int randomLastIndex;

		// Token: 0x02001B15 RID: 6933
		public enum Channel
		{
			// Token: 0x04009ED7 RID: 40663
			Mouth,
			// Token: 0x04009ED8 RID: 40664
			Environment
		}
	}
}
