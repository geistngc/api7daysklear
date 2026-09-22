using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018C9 RID: 6345
	[Preserve]
	public class RaycastPath
	{
		// Token: 0x17001820 RID: 6176
		// (get) Token: 0x0600C3CC RID: 50124 RVA: 0x0048709A File Offset: 0x0048529A
		// (set) Token: 0x0600C3CD RID: 50125 RVA: 0x004870A2 File Offset: 0x004852A2
		public RaycastPathInfo Info { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001821 RID: 6177
		// (get) Token: 0x0600C3CE RID: 50126 RVA: 0x004870AB File Offset: 0x004852AB
		public Vector3 Start
		{
			get
			{
				return this.Info.Start;
			}
		}

		// Token: 0x17001822 RID: 6178
		// (get) Token: 0x0600C3CF RID: 50127 RVA: 0x004870B8 File Offset: 0x004852B8
		public Vector3 Target
		{
			get
			{
				return this.Info.Target;
			}
		}

		// Token: 0x17001823 RID: 6179
		// (get) Token: 0x0600C3D0 RID: 50128 RVA: 0x004870C5 File Offset: 0x004852C5
		public Vector3i StartBlockPos
		{
			get
			{
				return this.Info.StartBlockPos;
			}
		}

		// Token: 0x17001824 RID: 6180
		// (get) Token: 0x0600C3D1 RID: 50129 RVA: 0x004870D2 File Offset: 0x004852D2
		public Vector3i TargetBlockPos
		{
			get
			{
				return this.Info.TargetBlockPos;
			}
		}

		// Token: 0x17001825 RID: 6181
		// (get) Token: 0x0600C3D2 RID: 50130 RVA: 0x004870DF File Offset: 0x004852DF
		public bool PathStartsIndoors
		{
			get
			{
				return this.Info.PathStartsIndoors;
			}
		}

		// Token: 0x17001826 RID: 6182
		// (get) Token: 0x0600C3D3 RID: 50131 RVA: 0x004870EC File Offset: 0x004852EC
		public bool PathEndsIndoors
		{
			get
			{
				return this.Info.PathEndsIndoors;
			}
		}

		// Token: 0x0600C3D4 RID: 50132 RVA: 0x004870F9 File Offset: 0x004852F9
		public RaycastPath(Vector3 start, Vector3 target)
		{
			this.Info = new RaycastPathInfo(start, target);
			RaycastPathManager.Instance.Add(this);
		}

		// Token: 0x0600C3D5 RID: 50133 RVA: 0x00487130 File Offset: 0x00485330
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~RaycastPath()
		{
			this.Destruct();
		}

		// Token: 0x0600C3D6 RID: 50134 RVA: 0x0048715C File Offset: 0x0048535C
		public void Destruct()
		{
			RaycastPathManager.Instance.Remove(this);
		}

		// Token: 0x0600C3D7 RID: 50135 RVA: 0x00487169 File Offset: 0x00485369
		public void AddNode(RaycastNode node)
		{
			if (!this.Nodes.Contains(node))
			{
				this.Nodes.Add(node);
			}
		}

		// Token: 0x0600C3D8 RID: 50136 RVA: 0x00487185 File Offset: 0x00485385
		public void AddProjectedPoint(Vector3 point)
		{
			if (!this.ProjectedPoints.Contains(point))
			{
				this.ProjectedPoints.Add(point);
			}
		}

		// Token: 0x0600C3D9 RID: 50137 RVA: 0x004871A4 File Offset: 0x004853A4
		public virtual void DebugDraw()
		{
			for (int i = 0; i < this.ProjectedPoints.Count - 1; i++)
			{
				Utils.DrawLine(World.blockToTransformPos(new Vector3i(this.ProjectedPoints[i] - Origin.position)), World.blockToTransformPos(new Vector3i(this.ProjectedPoints[i + 1] - Origin.position)), Color.white, Color.cyan, 2, 0f);
			}
			for (int j = 0; j < this.Nodes.Count; j++)
			{
				RaycastNode raycastNode = this.Nodes[j];
				for (int k = 0; k < raycastNode.Neighbors.Count; k++)
				{
					RaycastNode raycastNode2 = raycastNode.Neighbors[k];
					for (int l = 0; l < raycastNode2.ChildSolidBlocks.Count; l++)
					{
						RaycastPathUtils.DrawNode(raycastNode2.ChildSolidBlocks[l], Color.red, 0f);
					}
					for (int m = 0; m < raycastNode2.ChildAirBlocks.Count; m++)
					{
						RaycastPathUtils.DrawNode(raycastNode2.ChildAirBlocks[m], Color.cyan, 0f);
					}
				}
				if (raycastNode.Children.Count < 1)
				{
					cPathNodeType nodeType = raycastNode.nodeType;
					if (nodeType != cPathNodeType.Air)
					{
						if (nodeType == cPathNodeType.Door)
						{
							RaycastPathUtils.DrawNode(raycastNode, Color.green, 0f);
						}
					}
					else
					{
						RaycastPathUtils.DrawNode(raycastNode, Color.cyan, 0f);
					}
				}
				else
				{
					for (int n = 0; n < raycastNode.ChildSolidBlocks.Count; n++)
					{
						RaycastPathUtils.DrawNode(raycastNode.ChildSolidBlocks[n], Color.red, 0f);
					}
					for (int num = 0; num < raycastNode.ChildAirBlocks.Count; num++)
					{
						RaycastPathUtils.DrawNode(raycastNode.ChildAirBlocks[num], Color.cyan, 0f);
					}
				}
			}
			for (int num2 = 0; num2 < this.Nodes.Count - 1; num2++)
			{
				RaycastNode raycastNode3 = this.Nodes[num2];
				RaycastNode raycastNode4 = this.Nodes[num2 + 1];
				Utils.DrawLine(raycastNode3.Position - Origin.position, raycastNode4.Position - Origin.position, Color.white, Color.green, 2, 0f);
			}
		}

		// Token: 0x04009472 RID: 38002
		public List<RaycastNode> Nodes = new List<RaycastNode>();

		// Token: 0x04009473 RID: 38003
		public List<Vector3> ProjectedPoints = new List<Vector3>();
	}
}
