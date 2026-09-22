using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018CF RID: 6351
	[Preserve]
	public class FloodFillPath : RaycastPath
	{
		// Token: 0x0600C401 RID: 50177 RVA: 0x00487815 File Offset: 0x00485A15
		public FloodFillPath(Vector3 start, Vector3 target) : base(start, target)
		{
		}

		// Token: 0x0600C402 RID: 50178 RVA: 0x00487838 File Offset: 0x00485A38
		public bool IsPosOpen(Vector3 pos)
		{
			return this.open.Find((FloodFillNode n) => n.Position == pos) != null;
		}

		// Token: 0x0600C403 RID: 50179 RVA: 0x0048786C File Offset: 0x00485A6C
		public bool IsPosClosed(Vector3 pos)
		{
			return this.closed.Find((FloodFillNode n) => n.Position == pos) != null;
		}

		// Token: 0x0600C404 RID: 50180 RVA: 0x004878A0 File Offset: 0x00485AA0
		public FloodFillNode getLowestScore()
		{
			FloodFillNode result = null;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.open.Count; i++)
			{
				FloodFillNode floodFillNode = this.open[i];
				if (floodFillNode.F <= num && floodFillNode.Heuristic < num2)
				{
					result = floodFillNode;
					num = floodFillNode.F;
					num2 = floodFillNode.Heuristic;
				}
			}
			return result;
		}

		// Token: 0x0600C405 RID: 50181 RVA: 0x00487908 File Offset: 0x00485B08
		public override void DebugDraw()
		{
			for (int i = 0; i < this.closed.Count; i++)
			{
				FloodFillNode floodFillNode = this.closed[i];
				if (floodFillNode.nodeType == cPathNodeType.Air)
				{
					RaycastPathUtils.DrawBounds(floodFillNode.BlockPos, Color.yellow, 0f, 1f);
				}
			}
			base.DebugDraw();
		}

		// Token: 0x04009492 RID: 38034
		public List<FloodFillNode> open = new List<FloodFillNode>();

		// Token: 0x04009493 RID: 38035
		public List<FloodFillNode> closed = new List<FloodFillNode>();
	}
}
