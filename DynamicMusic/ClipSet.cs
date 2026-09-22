using System;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A45 RID: 6725
	[Preserve]
	public class ClipSet : LayeredContent
	{
		// Token: 0x0600CBF4 RID: 52212 RVA: 0x004AAD5C File Offset: 0x004A8F5C
		public override float GetSample(PlacementType _placement, int _idx, params float[] _params)
		{
			IClipAdapter clipAdapter;
			if (!this.clips.TryGetValue(_placement, out clipAdapter))
			{
				clipAdapter = this.clips[PlacementType.Loop];
			}
			return clipAdapter.GetSample(_idx, _params);
		}

		// Token: 0x0600CBF5 RID: 52213 RVA: 0x004AAD90 File Offset: 0x004A8F90
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement xelement in _xmlNode.Elements("clip"))
			{
				PlacementType key = EnumUtils.Parse<PlacementType>(xelement.GetAttribute("key"), false);
				IClipAdapter clipAdapter = LayeredContent.CreateClipAdapter(xelement.GetAttribute("type"));
				clipAdapter.ParseXml(xelement);
				this.clips.Add(key, clipAdapter);
			}
		}
	}
}
