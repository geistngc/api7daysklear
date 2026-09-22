using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018CE RID: 6350
	[Preserve]
	public class RaycastNodeHierarcy
	{
		// Token: 0x0600C3FB RID: 50171 RVA: 0x004876F8 File Offset: 0x004858F8
		public void SetWayPoint(RaycastNode node)
		{
			this.flowToWaypoint = true;
			this.waypoint = node;
		}

		// Token: 0x0600C3FC RID: 50172 RVA: 0x00487708 File Offset: 0x00485908
		public void SetParent(RaycastNode node)
		{
			this.parent = node;
		}

		// Token: 0x0600C3FD RID: 50173 RVA: 0x00487711 File Offset: 0x00485911
		public void AddNeighbor(RaycastNode node)
		{
			if (!this.neighbors.Contains(node))
			{
				this.neighbors.Add(node);
			}
		}

		// Token: 0x0600C3FE RID: 50174 RVA: 0x00487730 File Offset: 0x00485930
		public RaycastNode GetNeighbor(Vector3 pos)
		{
			for (int i = 0; i < this.neighbors.Count; i++)
			{
				if (this.neighbors[i].Center == pos)
				{
					return this.neighbors[i];
				}
			}
			return null;
		}

		// Token: 0x0600C3FF RID: 50175 RVA: 0x0048777C File Offset: 0x0048597C
		public void AddChild(RaycastNode node)
		{
			if (!this.children.Contains(node))
			{
				this.children.Add(node);
				if (node.nodeType == cPathNodeType.Air)
				{
					if (!this.childAirBlocks.Contains(node))
					{
						this.childAirBlocks.Add(node);
						return;
					}
				}
				else if (!this.childSolidBlocks.Contains(node))
				{
					this.childSolidBlocks.Add(node);
				}
			}
		}

		// Token: 0x0400948B RID: 38027
		public RaycastNode parent;

		// Token: 0x0400948C RID: 38028
		public List<RaycastNode> neighbors = new List<RaycastNode>();

		// Token: 0x0400948D RID: 38029
		public List<RaycastNode> children = new List<RaycastNode>();

		// Token: 0x0400948E RID: 38030
		public List<RaycastNode> childAirBlocks = new List<RaycastNode>();

		// Token: 0x0400948F RID: 38031
		public List<RaycastNode> childSolidBlocks = new List<RaycastNode>();

		// Token: 0x04009490 RID: 38032
		public bool flowToWaypoint;

		// Token: 0x04009491 RID: 38033
		public RaycastNode waypoint;
	}
}
