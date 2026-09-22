using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018CD RID: 6349
	[Preserve]
	public class RaycastNodeInfo
	{
		// Token: 0x0600C3F9 RID: 50169 RVA: 0x004875FC File Offset: 0x004857FC
		public RaycastNodeInfo(Vector3 pos, float scale = 1f, int depth = 0)
		{
			this.Position = pos;
			this.BlockPos = World.worldToBlockPos(pos);
			this.Scale = scale;
			this.Depth = depth;
			this.Min = pos - Vector3.one * scale * 0.5f;
			this.Max = pos + Vector3.one * scale * 0.5f;
			this.Center = (this.Min + this.Max) * 0.5f;
		}

		// Token: 0x0600C3FA RID: 50170 RVA: 0x00487694 File Offset: 0x00485894
		public RaycastNodeInfo(Vector3 min, Vector3 max, float scale = 1f, int depth = 0)
		{
			this.Position = (min + max) * 0.5f;
			this.BlockPos = World.worldToBlockPos(this.Position);
			this.Scale = scale;
			this.Depth = depth;
			this.Min = min;
			this.Max = max;
			this.Center = this.Position;
		}

		// Token: 0x04009484 RID: 38020
		public readonly Vector3 Position;

		// Token: 0x04009485 RID: 38021
		public readonly Vector3i BlockPos;

		// Token: 0x04009486 RID: 38022
		public readonly float Scale;

		// Token: 0x04009487 RID: 38023
		public readonly int Depth;

		// Token: 0x04009488 RID: 38024
		public readonly Vector3 Min;

		// Token: 0x04009489 RID: 38025
		public readonly Vector3 Max;

		// Token: 0x0400948A RID: 38026
		public readonly Vector3 Center;
	}
}
