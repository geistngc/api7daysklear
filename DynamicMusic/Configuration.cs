using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A41 RID: 6721
	[Preserve]
	public class Configuration : AbstractConfiguration, IFiniteConfiguration, IConfiguration<IList<PlacementType>>, IConfiguration
	{
		// Token: 0x170018EA RID: 6378
		// (get) Token: 0x0600CBE2 RID: 52194 RVA: 0x004AAA2E File Offset: 0x004A8C2E
		// (set) Token: 0x0600CBE3 RID: 52195 RVA: 0x004AAA36 File Offset: 0x004A8C36
		public Dictionary<LayerType, IList<PlacementType>> Layers { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600CBE4 RID: 52196 RVA: 0x004AAA3F File Offset: 0x004A8C3F
		public Configuration()
		{
			this.Layers = new Dictionary<LayerType, IList<PlacementType>>();
		}

		// Token: 0x0600CBE5 RID: 52197 RVA: 0x004AAA52 File Offset: 0x004A8C52
		public override int CountFor(LayerType _layer)
		{
			if (!this.Layers.ContainsKey(_layer))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x0600CBE6 RID: 52198 RVA: 0x004AAA68 File Offset: 0x004A8C68
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement e in _xmlNode.Elements("layer"))
			{
				this.ParseLayers(e);
			}
		}

		// Token: 0x0600CBE7 RID: 52199 RVA: 0x004AAAC8 File Offset: 0x004A8CC8
		[PublicizedFrom(EAccessModifier.Private)]
		public void ParseLayers(XElement e)
		{
			List<PlacementType> list = new List<PlacementType>();
			foreach (string s in e.GetAttribute("value").Split(',', StringSplitOptions.None))
			{
				list.Add((PlacementType)byte.Parse(s));
			}
			LayerType key = EnumUtils.Parse<LayerType>(e.GetAttribute("key"), false);
			this.Layers.Add(key, list);
		}
	}
}
