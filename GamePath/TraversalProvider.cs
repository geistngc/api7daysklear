using System;
using Pathfinding;

namespace GamePath
{
	// Token: 0x020018DB RID: 6363
	[PublicizedFrom(EAccessModifier.Internal)]
	public class TraversalProvider : ITraversalProvider
	{
		// Token: 0x0600C452 RID: 50258 RVA: 0x00489EC8 File Offset: 0x004880C8
		public bool CanTraverseConnection(Path path, Connection conn)
		{
			return conn.node.Walkable && (path.enabledTags >> (int)conn.node.Tag & 1) != 0;
		}

		// Token: 0x0600C453 RID: 50259 RVA: 0x00489EF4 File Offset: 0x004880F4
		public bool CanTraverse(Path path, GraphNode node, int gridDirection)
		{
			if (!node.Walkable)
			{
				return false;
			}
			if (!path.CanBreakBlocks && gridDirection >= 0)
			{
				AstarVoxelGrid.VoxelNode voxelNode = node as AstarVoxelGrid.VoxelNode;
				if (voxelNode != null && ((int)voxelNode.BlockerFlags & 17 << gridDirection) > 0)
				{
					return false;
				}
			}
			return (path.enabledTags >> (int)node.Tag & 1) != 0;
		}

		// Token: 0x0600C454 RID: 50260 RVA: 0x00489F49 File Offset: 0x00488149
		public uint GetTraversalCost(Path path, GraphNode node)
		{
			return (uint)(node.Penalty * path.CostScale);
		}
	}
}
