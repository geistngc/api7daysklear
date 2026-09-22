using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A39 RID: 6713
	[Preserve]
	public class ClipPairAdapter : IClipAdapter
	{
		// Token: 0x170018E1 RID: 6369
		// (get) Token: 0x0600CBB2 RID: 52146 RVA: 0x004A9EB4 File Offset: 0x004A80B4
		// (set) Token: 0x0600CBB3 RID: 52147 RVA: 0x004A9EBC File Offset: 0x004A80BC
		public bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600CBB4 RID: 52148 RVA: 0x004A9EC5 File Offset: 0x004A80C5
		public ClipPairAdapter()
		{
			this.clipAdapterLo = new ClipAdapter();
			this.clipAdapterHi = new ClipAdapter();
		}

		// Token: 0x0600CBB5 RID: 52149 RVA: 0x004A9EE3 File Offset: 0x004A80E3
		public float GetSample(int idx, params float[] _params)
		{
			return _params[0] * (this.clipAdapterLo.GetSample(idx, null) * (1f - _params[1]) + _params[1] * this.clipAdapterHi.GetSample(idx, null));
		}

		// Token: 0x0600CBB6 RID: 52150 RVA: 0x004A9F12 File Offset: 0x004A8112
		public IEnumerator Load()
		{
			yield return this.clipAdapterLo.Load();
			yield return this.clipAdapterHi.Load();
			yield break;
		}

		// Token: 0x0600CBB7 RID: 52151 RVA: 0x004A9F21 File Offset: 0x004A8121
		public void LoadImmediate()
		{
			this.clipAdapterLo.LoadImmediate();
			this.clipAdapterHi.LoadImmediate();
		}

		// Token: 0x0600CBB8 RID: 52152 RVA: 0x004A9F39 File Offset: 0x004A8139
		public void Unload()
		{
			this.clipAdapterLo.Unload();
			this.clipAdapterHi.Unload();
			this.IsLoaded = false;
		}

		// Token: 0x0600CBB9 RID: 52153 RVA: 0x000027FC File Offset: 0x000009FC
		public void ParseXml(XElement _xmlNode)
		{
		}

		// Token: 0x0600CBBA RID: 52154 RVA: 0x004A9F58 File Offset: 0x004A8158
		public void SetPaths(int _num, PlacementType _placement, SectionType _section, LayerType _layer, string stress = "")
		{
			this.clipAdapterLo.SetPaths(_num, _placement, _section, _layer, "Lo");
			this.clipAdapterHi.SetPaths(_num, _placement, _section, _layer, "Hi");
		}

		// Token: 0x04009B33 RID: 39731
		[PublicizedFrom(EAccessModifier.Private)]
		public ClipAdapter clipAdapterLo;

		// Token: 0x04009B34 RID: 39732
		[PublicizedFrom(EAccessModifier.Private)]
		public ClipAdapter clipAdapterHi;
	}
}
