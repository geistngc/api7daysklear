using System;
using System.Collections;
using System.Collections.Generic;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A51 RID: 6737
	[Preserve]
	public class BloodmoonLayerMixer : LayerMixer<BloodmoonConfiguration>
	{
		// Token: 0x0600CC36 RID: 52278 RVA: 0x004AB804 File Offset: 0x004A9A04
		public BloodmoonLayerMixer()
		{
			BloodmoonLayerMixer.player = GameManager.Instance.World.GetPrimaryPlayer();
			this.paramsFor = new EnumDictionary<LayerType, LayerParams>();
			Enum.GetValues(typeof(LayerType)).Cast<LayerType>().ToList<LayerType>().ForEach(delegate(LayerType lyr)
			{
				this.paramsFor.Add(lyr, new LayerParams(0f, 1f));
			});
		}

		// Token: 0x170018F6 RID: 6390
		public override float this[int _idx]
		{
			get
			{
				if (BloodmoonLayerMixer.player == null)
				{
					BloodmoonLayerMixer.player = GameManager.Instance.World.GetPrimaryPlayer();
				}
				float arg = (BloodmoonLayerMixer.player == null) ? 0f : BloodmoonLayerMixer.player.ThreatLevel.Numeric;
				float num = 0f;
				foreach (KeyValuePair<LayerType, LayerState> keyValuePair in this.config.Layers)
				{
					LayerParams layerParams = this.paramsFor[keyValuePair.Key];
					layerParams.Volume = Mathf.Clamp01(layerParams.Volume + ((keyValuePair.Value.Get(arg) == LayerStateType.disabled) ? -3.7792895E-06f : 3.7792895E-06f));
					layerParams.Mix = Mathf.Clamp01(layerParams.Mix + ((keyValuePair.Value.Get(arg) != LayerStateType.hi) ? -3.7792895E-06f : 3.7792895E-06f));
					foreach (LayeredContent layeredContent in this.clipSetsFor[keyValuePair.Key])
					{
						num += layeredContent.GetSample(PlacementType.Loop, _idx, new float[]
						{
							layerParams.Volume,
							layerParams.Mix
						});
					}
				}
				return (float)Math.Tanh((double)num);
			}
		}

		// Token: 0x0600CC38 RID: 52280 RVA: 0x004AB9F8 File Offset: 0x004A9BF8
		public override IEnumerator Load()
		{
			yield return this.<>n__0();
			foreach (LayerParams layerParams in this.paramsFor.Values)
			{
				layerParams.Mix = 1f;
				layerParams.Volume = 0f;
			}
			foreach (LayerType layer in this.config.Layers.Keys)
			{
				LayeredContent content = LayeredContent.Get<BloodmoonClipSet>(SectionType.Bloodmoon, layer);
				yield return content.Load();
				this.clipSetsFor.Add(layer, new List<LayeredContent>
				{
					content
				});
				content = null;
			}
			Dictionary<LayerType, LayerState>.KeyCollection.Enumerator enumerator2 = default(Dictionary<LayerType, LayerState>.KeyCollection.Enumerator);
			BloodmoonLayerMixer.player = GameManager.Instance.World.GetPrimaryPlayer();
			yield break;
			yield break;
		}

		// Token: 0x04009B74 RID: 39796
		public static float ThreatLevel = 0.75f;

		// Token: 0x04009B75 RID: 39797
		[PublicizedFrom(EAccessModifier.Private)]
		public static EntityPlayerLocal player;

		// Token: 0x04009B76 RID: 39798
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cIncrement = 3.7792895E-06f;

		// Token: 0x04009B77 RID: 39799
		[PublicizedFrom(EAccessModifier.Private)]
		public EnumDictionary<LayerType, LayerParams> paramsFor;
	}
}
