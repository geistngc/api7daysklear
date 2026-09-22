using System;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A44 RID: 6724
	[Preserve]
	public class BloodmoonClipSet : LayeredContent
	{
		// Token: 0x0600CBF1 RID: 52209 RVA: 0x004AACA4 File Offset: 0x004A8EA4
		public override float GetSample(PlacementType _placement, int _idx, params float[] _params)
		{
			return this.clips[_placement].GetSample(_idx, _params);
		}

		// Token: 0x0600CBF2 RID: 52210 RVA: 0x004AACBC File Offset: 0x004A8EBC
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
