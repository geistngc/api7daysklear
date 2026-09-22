using System;
using Pathfinding;

namespace GamePath
{
	// Token: 0x020018DC RID: 6364
	[PublicizedFrom(EAccessModifier.Internal)]
	public class TraversalProviderNoBreak : ITraversalProvider
	{
		// Token: 0x0600C456 RID: 50262 RVA: 0x00489F5C File Offset: 0x0048815C
		public bool CanTraverseConnection(Path path, Connection conn)
		{
			if (!conn.node.Walkable)
			{
				return false;
			}
			if (conn.tag == 3U)
			{
				TEFeatureDoor tefeatureDoor = conn.payload as TEFeatureDoor;
				if (tefeatureDoor != null)
				{
					if (tefeatureDoor.IsOpen())
					{
						return true;
					}
					if ((path.enabledTags >> 3 & 1) == 0)
					{
						return false;
					}
					bool flag;
					if (!tefeatureDoor.CanOpen(out flag))
					{
						return false;
					}
				}
				return true;
			}
			return (path.enabledTags >> (int)conn.node.Tag & 1) != 0;
		}

		// Token: 0x0600C457 RID: 50263 RVA: 0x00489FD0 File Offset: 0x004881D0
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
			return (path.enabledTags >> (int)node.Tag & 1) != 0 && node.Penalty < 1000U;
		}

		// Token: 0x0600C458 RID: 50264 RVA: 0x0048A033 File Offset: 0x00488233
		public uint GetTraversalCost(Path path, GraphNode node)
		{
			return node.Penalty;
		}
	}
}
