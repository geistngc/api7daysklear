using System;
using System.Collections;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A5E RID: 6750
	[Preserve]
	public class Bloodmoon : LayeredSection<BloodmoonLayerMixer>
	{
		// Token: 0x0600CC8A RID: 52362 RVA: 0x004ACDC8 File Offset: 0x004AAFC8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator PlayCoroutine()
		{
			yield return this.<>n__0();
			yield return new WaitUntil(() => !this.src.isPlaying && !this.IsPaused);
			this.Reset();
			this.Mixer.Unload();
			this.coroutines.Remove(MusicActionType.Play);
			yield break;
		}

		// Token: 0x0600CC8B RID: 52363 RVA: 0x004ACDD8 File Offset: 0x004AAFD8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void FillStream(float[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				int num = i;
				LayerMixer<BloodmoonConfiguration> mixer = this.Mixer;
				int cursor = this.cursor;
				this.cursor = cursor + 1;
				data[num] = mixer[cursor];
				this.cursor %= Content.SamplesFor[base.Sect] * 2;
			}
		}
	}
}
