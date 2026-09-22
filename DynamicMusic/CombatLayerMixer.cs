using System;
using System.Collections;
using System.Collections.Generic;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A62 RID: 6754
	[Preserve]
	public class CombatLayerMixer : FixedLayerMixer
	{
		// Token: 0x0600CC9F RID: 52383 RVA: 0x004ACFDC File Offset: 0x004AB1DC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void updateHyperbar(int _idx)
		{
			this.hyperbar = _idx / (Content.SamplesFor[base.Sect] * 2) % this.maxHyperbar;
		}

		// Token: 0x0600CCA0 RID: 52384 RVA: 0x004ACFFF File Offset: 0x004AB1FF
		public override IEnumerator Load()
		{
			yield return this.<>n__0();
			this.maxHyperbar = this.config.Layers.Values.First<FixedConfigurationLayerData>().LayerInstances.First<List<PlacementType>>().Count;
			yield break;
		}

		// Token: 0x04009BA8 RID: 39848
		[PublicizedFrom(EAccessModifier.Private)]
		public int maxHyperbar;
	}
}
