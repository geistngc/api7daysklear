using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018CC RID: 6348
	[Preserve]
	public class RaycastNode
	{
		// Token: 0x0600C3E2 RID: 50146 RVA: 0x004874AB File Offset: 0x004856AB
		public RaycastNode(Vector3 pos, float scale = 1f, int depth = 0)
		{
			this.info = new RaycastNodeInfo(pos, scale, depth);
			this.hierarchy = new RaycastNodeHierarcy();
		}

		// Token: 0x0600C3E3 RID: 50147 RVA: 0x004874CC File Offset: 0x004856CC
		public RaycastNode(Vector3 min, Vector3 max, float scale = 1f, int depth = 0)
		{
			this.info = new RaycastNodeInfo(min, max, scale, depth);
			this.hierarchy = new RaycastNodeHierarcy();
		}

		// Token: 0x1700182B RID: 6187
		// (get) Token: 0x0600C3E4 RID: 50148 RVA: 0x004874EF File Offset: 0x004856EF
		public Vector3 Position
		{
			get
			{
				return this.info.Position;
			}
		}

		// Token: 0x1700182C RID: 6188
		// (get) Token: 0x0600C3E5 RID: 50149 RVA: 0x004874FC File Offset: 0x004856FC
		public Vector3 Center
		{
			get
			{
				return this.info.Center;
			}
		}

		// Token: 0x1700182D RID: 6189
		// (get) Token: 0x0600C3E6 RID: 50150 RVA: 0x00487509 File Offset: 0x00485709
		public Vector3i BlockPos
		{
			get
			{
				return this.info.BlockPos;
			}
		}

		// Token: 0x1700182E RID: 6190
		// (get) Token: 0x0600C3E7 RID: 50151 RVA: 0x00487516 File Offset: 0x00485716
		public float Scale
		{
			get
			{
				return this.info.Scale;
			}
		}

		// Token: 0x1700182F RID: 6191
		// (get) Token: 0x0600C3E8 RID: 50152 RVA: 0x00487523 File Offset: 0x00485723
		public int Depth
		{
			get
			{
				return this.info.Depth;
			}
		}

		// Token: 0x17001830 RID: 6192
		// (get) Token: 0x0600C3E9 RID: 50153 RVA: 0x00487530 File Offset: 0x00485730
		public Vector3 Min
		{
			get
			{
				return this.info.Min;
			}
		}

		// Token: 0x17001831 RID: 6193
		// (get) Token: 0x0600C3EA RID: 50154 RVA: 0x0048753D File Offset: 0x0048573D
		public Vector3 Max
		{
			get
			{
				return this.info.Max;
			}
		}

		// Token: 0x17001832 RID: 6194
		// (get) Token: 0x0600C3EB RID: 50155 RVA: 0x0048754A File Offset: 0x0048574A
		public RaycastNode Parent
		{
			get
			{
				return this.hierarchy.parent;
			}
		}

		// Token: 0x17001833 RID: 6195
		// (get) Token: 0x0600C3EC RID: 50156 RVA: 0x00487557 File Offset: 0x00485757
		public List<RaycastNode> Neighbors
		{
			get
			{
				return this.hierarchy.neighbors;
			}
		}

		// Token: 0x0600C3ED RID: 50157 RVA: 0x00487564 File Offset: 0x00485764
		public void SetParent(RaycastNode node)
		{
			this.hierarchy.parent = node;
		}

		// Token: 0x0600C3EE RID: 50158 RVA: 0x00487572 File Offset: 0x00485772
		public void AddNeighbor(RaycastNode node)
		{
			this.hierarchy.neighbors.Add(node);
		}

		// Token: 0x0600C3EF RID: 50159 RVA: 0x00487585 File Offset: 0x00485785
		public RaycastNode GetNeighbor(Vector3 pos)
		{
			return this.hierarchy.GetNeighbor(pos);
		}

		// Token: 0x0600C3F0 RID: 50160 RVA: 0x00487593 File Offset: 0x00485793
		public void AddChild(RaycastNode node)
		{
			this.hierarchy.AddChild(node);
		}

		// Token: 0x17001834 RID: 6196
		// (get) Token: 0x0600C3F1 RID: 50161 RVA: 0x004875A1 File Offset: 0x004857A1
		public List<RaycastNode> Children
		{
			get
			{
				return this.hierarchy.children;
			}
		}

		// Token: 0x17001835 RID: 6197
		// (get) Token: 0x0600C3F2 RID: 50162 RVA: 0x004875AE File Offset: 0x004857AE
		public List<RaycastNode> ChildAirBlocks
		{
			get
			{
				return this.hierarchy.childAirBlocks;
			}
		}

		// Token: 0x17001836 RID: 6198
		// (get) Token: 0x0600C3F3 RID: 50163 RVA: 0x004875BB File Offset: 0x004857BB
		public List<RaycastNode> ChildSolidBlocks
		{
			get
			{
				return this.hierarchy.childSolidBlocks;
			}
		}

		// Token: 0x17001837 RID: 6199
		// (get) Token: 0x0600C3F4 RID: 50164 RVA: 0x004875C8 File Offset: 0x004857C8
		public RaycastNode Waypoint
		{
			get
			{
				return this.hierarchy.waypoint;
			}
		}

		// Token: 0x0600C3F5 RID: 50165 RVA: 0x004875D5 File Offset: 0x004857D5
		public void SetWaypoint(RaycastNode node)
		{
			this.hierarchy.SetWayPoint(node);
		}

		// Token: 0x17001838 RID: 6200
		// (get) Token: 0x0600C3F6 RID: 50166 RVA: 0x004875E3 File Offset: 0x004857E3
		public bool FlowToWaypoint
		{
			get
			{
				return this.hierarchy.flowToWaypoint;
			}
		}

		// Token: 0x0600C3F7 RID: 50167 RVA: 0x004875F0 File Offset: 0x004857F0
		public void SetType(cPathNodeType _nodeType)
		{
			this.nodeType = _nodeType;
		}

		// Token: 0x0600C3F8 RID: 50168 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void DebugDraw()
		{
		}

		// Token: 0x04009481 RID: 38017
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNodeInfo info;

		// Token: 0x04009482 RID: 38018
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNodeHierarcy hierarchy;

		// Token: 0x04009483 RID: 38019
		public cPathNodeType nodeType;
	}
}
