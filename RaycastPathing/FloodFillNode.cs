using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018D2 RID: 6354
	[Preserve]
	public class FloodFillNode : RaycastNode
	{
		// Token: 0x0600C40A RID: 50186 RVA: 0x00487987 File Offset: 0x00485B87
		public FloodFillNode(Vector3 pos, float scale = 1f, int depth = 0) : base(pos, scale, depth)
		{
			this.score = new FloodFillNodeScore();
		}

		// Token: 0x0600C40B RID: 50187 RVA: 0x0048799D File Offset: 0x00485B9D
		public FloodFillNode(Vector3 min, Vector3 max, float scale = 1f, int depth = 0) : base(min, max, scale, depth)
		{
			this.score = new FloodFillNodeScore();
		}

		// Token: 0x17001839 RID: 6201
		// (get) Token: 0x0600C40C RID: 50188 RVA: 0x004879B5 File Offset: 0x00485BB5
		// (set) Token: 0x0600C40D RID: 50189 RVA: 0x004879C2 File Offset: 0x00485BC2
		public float G
		{
			get
			{
				return this.score.G;
			}
			set
			{
				this.score.G = value;
			}
		}

		// Token: 0x1700183A RID: 6202
		// (get) Token: 0x0600C40E RID: 50190 RVA: 0x004879D0 File Offset: 0x00485BD0
		// (set) Token: 0x0600C40F RID: 50191 RVA: 0x004879DD File Offset: 0x00485BDD
		public float Heuristic
		{
			get
			{
				return this.score.H;
			}
			set
			{
				this.score.H = value;
			}
		}

		// Token: 0x1700183B RID: 6203
		// (get) Token: 0x0600C410 RID: 50192 RVA: 0x004879EB File Offset: 0x00485BEB
		public float F
		{
			get
			{
				return this.score.F;
			}
		}

		// Token: 0x04009496 RID: 38038
		[PublicizedFrom(EAccessModifier.Private)]
		public FloodFillNodeScore score;
	}
}
