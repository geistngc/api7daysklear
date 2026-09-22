using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018CA RID: 6346
	[Preserve]
	public class RaycastPathInfo
	{
		// Token: 0x0600C3DA RID: 50138 RVA: 0x00487408 File Offset: 0x00485608
		public RaycastPathInfo(Vector3 start, Vector3 target)
		{
			this.Start = start;
			this.Target = target;
			this.StartNode = new RaycastNode(this.Start, 1f, 0);
			this.TargetNode = new RaycastNode(this.Target, 1f, 0);
			this.PathStartsIndoors = RaycastPathWorldUtils.IsUnderground(start);
			this.PathEndsIndoors = RaycastPathWorldUtils.IsUnderground(target);
		}

		// Token: 0x17001827 RID: 6183
		// (get) Token: 0x0600C3DB RID: 50139 RVA: 0x0048746F File Offset: 0x0048566F
		public Vector3i StartBlockPos
		{
			get
			{
				return this.StartNode.BlockPos;
			}
		}

		// Token: 0x17001828 RID: 6184
		// (get) Token: 0x0600C3DC RID: 50140 RVA: 0x0048747C File Offset: 0x0048567C
		public Vector3i TargetBlockPos
		{
			get
			{
				return this.TargetNode.BlockPos;
			}
		}

		// Token: 0x17001829 RID: 6185
		// (get) Token: 0x0600C3DD RID: 50141 RVA: 0x00487489 File Offset: 0x00485689
		// (set) Token: 0x0600C3DE RID: 50142 RVA: 0x00487491 File Offset: 0x00485691
		public bool PathStartsIndoors { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700182A RID: 6186
		// (get) Token: 0x0600C3DF RID: 50143 RVA: 0x0048749A File Offset: 0x0048569A
		// (set) Token: 0x0600C3E0 RID: 50144 RVA: 0x004874A2 File Offset: 0x004856A2
		public bool PathEndsIndoors { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600C3E1 RID: 50145 RVA: 0x0019D014 File Offset: 0x0019B214
		public static implicit operator bool(RaycastPathInfo exists)
		{
			return exists != null;
		}

		// Token: 0x04009475 RID: 38005
		public readonly Vector3 Start;

		// Token: 0x04009476 RID: 38006
		public readonly Vector3 Target;

		// Token: 0x04009477 RID: 38007
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNode StartNode;

		// Token: 0x04009478 RID: 38008
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNode TargetNode;
	}
}
