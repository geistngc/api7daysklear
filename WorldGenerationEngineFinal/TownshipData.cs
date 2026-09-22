using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001770 RID: 6000
	public class TownshipData
	{
		// Token: 0x0600BA47 RID: 47687 RVA: 0x00457E38 File Offset: 0x00456038
		public TownshipData(string _name, int _id)
		{
			this.Name = _name;
			this.Id = _id;
			if (_name.EndsWith("roadside"))
			{
				this.Category = TownshipData.eCategory.Roadside;
			}
			else if (_name.EndsWith("rural"))
			{
				this.Category = TownshipData.eCategory.Rural;
			}
			else if (_name.EndsWith("wilderness"))
			{
				this.Category = TownshipData.eCategory.Wilderness;
			}
			WorldBuilderStatic.idToTownshipData[this.Id] = this;
		}

		// Token: 0x04008BD9 RID: 35801
		public string Name;

		// Token: 0x04008BDA RID: 35802
		public int Id;

		// Token: 0x04008BDB RID: 35803
		public List<string> SpawnableTerrain = new List<string>();

		// Token: 0x04008BDC RID: 35804
		public bool SpawnCustomSizes;

		// Token: 0x04008BDD RID: 35805
		public bool SpawnTrader = true;

		// Token: 0x04008BDE RID: 35806
		public bool SpawnGateway = true;

		// Token: 0x04008BDF RID: 35807
		public string OutskirtDistrict;

		// Token: 0x04008BE0 RID: 35808
		public float OutskirtDistrictPercent;

		// Token: 0x04008BE1 RID: 35809
		public FastTags<TagGroup.Poi> Biomes;

		// Token: 0x04008BE2 RID: 35810
		public readonly TownshipData.eCategory Category;

		// Token: 0x02001771 RID: 6001
		public enum eCategory
		{
			// Token: 0x04008BE4 RID: 35812
			Normal,
			// Token: 0x04008BE5 RID: 35813
			Roadside,
			// Token: 0x04008BE6 RID: 35814
			Rural,
			// Token: 0x04008BE7 RID: 35815
			Wilderness
		}
	}
}
