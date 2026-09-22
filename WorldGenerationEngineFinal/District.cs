using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x020016F9 RID: 5881
	public class District
	{
		// Token: 0x0600B772 RID: 46962 RVA: 0x00442F1F File Offset: 0x0044111F
		public District()
		{
		}

		// Token: 0x0600B773 RID: 46963 RVA: 0x00442F40 File Offset: 0x00441140
		public District(District _other)
		{
			this.name = _other.name;
			this.prefabName = _other.prefabName;
			this.tag = _other.tag;
			this.townships = _other.townships;
			this.weight = _other.weight;
			this.preview_color = _other.preview_color;
			this.avoidedNeighborDistricts = _other.avoidedNeighborDistricts;
			this.Init();
		}

		// Token: 0x0600B774 RID: 46964 RVA: 0x00442FC4 File Offset: 0x004411C4
		public void Init()
		{
			this.type = District.Type.None;
			if (this.name.EndsWith("commercial"))
			{
				this.type = District.Type.Commercial;
				this.spawnOrder = 1f;
				return;
			}
			if (this.name.EndsWith("downtown"))
			{
				this.type = District.Type.Downtown;
				this.spawnOrder = 99f;
				return;
			}
			if (this.name.EndsWith("gateway"))
			{
				this.type = District.Type.Gateway;
				return;
			}
			if (this.name.EndsWith("industrial"))
			{
				this.type = District.Type.Industrial;
				return;
			}
			if (this.name.EndsWith("residential"))
			{
				this.type = District.Type.Residential;
				this.spawnOrder = -1f;
				return;
			}
			if (this.name.EndsWith("rural"))
			{
				this.type = District.Type.Rural;
			}
		}

		// Token: 0x04008968 RID: 35176
		public string name;

		// Token: 0x04008969 RID: 35177
		public string prefabName;

		// Token: 0x0400896A RID: 35178
		public District.Type type;

		// Token: 0x0400896B RID: 35179
		public FastTags<TagGroup.Poi> tag;

		// Token: 0x0400896C RID: 35180
		public FastTags<TagGroup.Poi> townships;

		// Token: 0x0400896D RID: 35181
		public float weight = 0.5f;

		// Token: 0x0400896E RID: 35182
		public float spawnOrder;

		// Token: 0x0400896F RID: 35183
		public Color preview_color;

		// Token: 0x04008970 RID: 35184
		public bool spawnCustomSizePrefabs;

		// Token: 0x04008971 RID: 35185
		public List<string> avoidedNeighborDistricts = new List<string>();

		// Token: 0x020016FA RID: 5882
		public enum Type
		{
			// Token: 0x04008973 RID: 35187
			None,
			// Token: 0x04008974 RID: 35188
			Commercial,
			// Token: 0x04008975 RID: 35189
			Downtown,
			// Token: 0x04008976 RID: 35190
			Gateway,
			// Token: 0x04008977 RID: 35191
			Industrial,
			// Token: 0x04008978 RID: 35192
			Residential,
			// Token: 0x04008979 RID: 35193
			Rural
		}
	}
}
