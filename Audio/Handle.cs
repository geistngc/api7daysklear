using System;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001B1F RID: 6943
	public class Handle
	{
		// Token: 0x0600D04C RID: 53324 RVA: 0x004BF8E0 File Offset: 0x004BDAE0
		public Handle(string soundGroupName, AudioSource near, AudioSource far)
		{
			this.name = soundGroupName;
			this.nearSource = near;
			this.farSource = far;
			if (this.nearSource)
			{
				this.basePitch = this.nearSource.pitch;
				this.baseVolume = this.nearSource.volume;
			}
		}

		// Token: 0x0600D04D RID: 53325 RVA: 0x004BF938 File Offset: 0x004BDB38
		public void SetPitch(float pitch)
		{
			if (this.nearSource)
			{
				this.nearSource.pitch = pitch + this.basePitch;
			}
			if (this.farSource)
			{
				this.farSource.pitch = pitch + this.basePitch;
			}
		}

		// Token: 0x0600D04E RID: 53326 RVA: 0x004BF988 File Offset: 0x004BDB88
		public void SetVolume(float volume)
		{
			if (this.nearSource)
			{
				this.nearSource.volume = volume * this.baseVolume;
			}
			if (this.farSource)
			{
				this.farSource.volume = volume * this.baseVolume;
			}
		}

		// Token: 0x0600D04F RID: 53327 RVA: 0x004BF9D5 File Offset: 0x004BDBD5
		public void Stop(int entityId)
		{
			Manager.Stop(entityId, this.name);
		}

		// Token: 0x0600D050 RID: 53328 RVA: 0x004BF9E4 File Offset: 0x004BDBE4
		public float ClipLength()
		{
			if (this.nearSource)
			{
				return this.nearSource.clip.length;
			}
			if (this.farSource)
			{
				return this.farSource.clip.length;
			}
			return 0f;
		}

		// Token: 0x0600D051 RID: 53329 RVA: 0x004BFA34 File Offset: 0x004BDC34
		public bool IsPlaying()
		{
			bool flag = false;
			if (this.nearSource != null)
			{
				flag = this.nearSource.isPlaying;
			}
			if (this.farSource != null)
			{
				flag |= this.farSource.isPlaying;
			}
			return flag;
		}

		// Token: 0x04009EFD RID: 40701
		[PublicizedFrom(EAccessModifier.Private)]
		public string name;

		// Token: 0x04009EFE RID: 40702
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource nearSource;

		// Token: 0x04009EFF RID: 40703
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource farSource;

		// Token: 0x04009F00 RID: 40704
		[PublicizedFrom(EAccessModifier.Private)]
		public float basePitch;

		// Token: 0x04009F01 RID: 40705
		[PublicizedFrom(EAccessModifier.Private)]
		public float baseVolume;
	}
}
