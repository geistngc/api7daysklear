using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A3E RID: 6718
	[Preserve]
	public class BloodmoonConfiguration : AbstractConfiguration, IConfiguration<LayerState>, IConfiguration
	{
		// Token: 0x170018E8 RID: 6376
		// (get) Token: 0x0600CBD8 RID: 52184 RVA: 0x004AA8FA File Offset: 0x004A8AFA
		// (set) Token: 0x0600CBD9 RID: 52185 RVA: 0x004AA902 File Offset: 0x004A8B02
		public Dictionary<LayerType, LayerState> Layers { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600CBDA RID: 52186 RVA: 0x004AA90B File Offset: 0x004A8B0B
		public BloodmoonConfiguration()
		{
			this.Layers = new Dictionary<LayerType, LayerState>();
		}

		// Token: 0x0600CBDB RID: 52187 RVA: 0x004AA91E File Offset: 0x004A8B1E
		public override int CountFor(LayerType _layer)
		{
			if (!this.Layers.ContainsKey(_layer))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x0600CBDC RID: 52188 RVA: 0x004AA934 File Offset: 0x004A8B34
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement element in _xmlNode.Elements("layer"))
			{
				LayerType key = EnumUtils.Parse<LayerType>(element.GetAttribute("key"), false);
				float lo = float.Parse(element.GetAttribute("lo"));
				float hi = float.Parse(element.GetAttribute("hi"));
				this.Layers.Add(key, new LayerState((float tl) => BloodmoonConfiguration.getState(tl, lo, hi)));
			}
		}

		// Token: 0x0600CBDD RID: 52189 RVA: 0x004AA9FC File Offset: 0x004A8BFC
		[PublicizedFrom(EAccessModifier.Private)]
		public static LayerStateType getState(float _threatLevel, float _enabledThreshold, float _hiThreshold)
		{
			if (_threatLevel < _enabledThreshold)
			{
				return LayerStateType.disabled;
			}
			if (_threatLevel >= _hiThreshold)
			{
				return LayerStateType.hi;
			}
			return LayerStateType.lo;
		}
	}
}
