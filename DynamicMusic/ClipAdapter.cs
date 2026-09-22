using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils;
using MusicUtils.Enums;
using Unity.Profiling;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A37 RID: 6711
	[Preserve]
	public class ClipAdapter : IClipAdapter
	{
		// Token: 0x0600CBA2 RID: 52130 RVA: 0x004A9C54 File Offset: 0x004A7E54
		public float GetSample(int idx, params float[] _params)
		{
			if (idx % 4096 == 0)
			{
				this.reader.Position = 2 * idx;
				this.reader.Read(this.sampleData, 4096);
			}
			return this.sampleData[idx % 4096];
		}

		// Token: 0x170018DE RID: 6366
		// (get) Token: 0x0600CBA3 RID: 52131 RVA: 0x004A9C91 File Offset: 0x004A7E91
		// (set) Token: 0x0600CBA4 RID: 52132 RVA: 0x004A9C99 File Offset: 0x004A7E99
		public bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600CBA5 RID: 52133 RVA: 0x004A9CA2 File Offset: 0x004A7EA2
		public IEnumerator Load()
		{
			using (ClipAdapter.s_LoadMarker.Auto())
			{
				this.reader = new WaveReader(this.path);
				this.sampleData = MemoryPools.poolFloat.Alloc(4096);
				this.IsLoaded = true;
			}
			yield return null;
			yield break;
		}

		// Token: 0x0600CBA6 RID: 52134 RVA: 0x004A9CB4 File Offset: 0x004A7EB4
		public void LoadImmediate()
		{
			using (ClipAdapter.s_LoadImmediateMarker.Auto())
			{
				this.reader = new WaveReader(this.path);
				this.sampleData = MemoryPools.poolFloat.Alloc(4096);
				this.IsLoaded = true;
			}
		}

		// Token: 0x0600CBA7 RID: 52135 RVA: 0x004A9D1C File Offset: 0x004A7F1C
		public void Unload()
		{
			MemoryPools.poolFloat.Free(this.sampleData);
			this.sampleData = null;
			this.reader.Cleanup();
			this.reader = null;
			this.IsLoaded = false;
		}

		// Token: 0x0600CBA8 RID: 52136 RVA: 0x004A9D4E File Offset: 0x004A7F4E
		public void ParseXml(XElement _xmlNode)
		{
			this.path = _xmlNode.GetAttribute("value");
		}

		// Token: 0x0600CBA9 RID: 52137 RVA: 0x004A9D68 File Offset: 0x004A7F68
		public void SetPaths(int _num, PlacementType _placement, SectionType _section, LayerType _layer, string stress = "")
		{
			this.path = string.Concat(new string[]
			{
				GameIO.GetApplicationPath(),
				"/Data/Music/",
				_num.ToString("000"),
				DMSConstants.PlacementAbbrv[_placement],
				DMSConstants.SectionAbbrvs[_section],
				DMSConstants.LayerAbbrvs[_layer],
				stress,
				".wav"
			});
		}

		// Token: 0x04009B29 RID: 39721
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_LoadMarker = new ProfilerMarker("DynamicMusic.ClipAdapter.Load");

		// Token: 0x04009B2A RID: 39722
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_LoadImmediateMarker = new ProfilerMarker("DynamicMusic.ClipAdapter.LoadImmediate");

		// Token: 0x04009B2B RID: 39723
		[PublicizedFrom(EAccessModifier.Private)]
		public const int bufferSize = 4096;

		// Token: 0x04009B2C RID: 39724
		[PublicizedFrom(EAccessModifier.Private)]
		public string path;

		// Token: 0x04009B2D RID: 39725
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] sampleData;

		// Token: 0x04009B2E RID: 39726
		[PublicizedFrom(EAccessModifier.Private)]
		public WaveReader reader;
	}
}
