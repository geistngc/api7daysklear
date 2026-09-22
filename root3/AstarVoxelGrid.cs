using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Pathfinding;
using UnityEngine;

// Token: 0x020008DA RID: 2266
public class AstarVoxelGrid : LayerGridGraph
{
	// Token: 0x060041B2 RID: 16818 RVA: 0x0019A788 File Offset: 0x00198988
	public void Init()
	{
		if (AstarVoxelGrid.connectionsPool == null)
		{
			AstarVoxelGrid.connectionsPool = new List<Connection[]>[16];
			for (int i = 0; i < 16; i++)
			{
				AstarVoxelGrid.connectionsPool[i] = new List<Connection[]>();
			}
		}
		this.gridMover = new AstarVoxelGrid.ProceduralGridMover();
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x0019A7CC File Offset: 0x001989CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitScan()
	{
		base.Scan();
	}

	// Token: 0x060041B4 RID: 16820 RVA: 0x0019A7D4 File Offset: 0x001989D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override IEnumerable<Progress> ScanInternal()
	{
		foreach (Progress progress in this.<>n__0())
		{
			yield return progress;
		}
		yield break;
	}

	// Token: 0x060041B5 RID: 16821 RVA: 0x0019A7E4 File Offset: 0x001989E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateArea(GraphUpdateObject o)
	{
		IntRect intRect;
		IntRect a;
		IntRect a2;
		bool flag;
		int num;
		base.CalculateAffectedRegions(o, out intRect, out a, out a2, out flag, out num);
		IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
		IntRect intRect2 = IntRect.Intersection(a, b);
		this.collision.Initialize(base.transform, this.nodeSize);
		intRect2 = IntRect.Intersection(a2, b);
		for (int i = intRect2.xmin; i <= intRect2.xmax; i++)
		{
			for (int j = intRect2.ymin; j <= intRect2.ymax; j++)
			{
				this.RecalculateCell(i, j, true, false);
			}
		}
		a.Expand(1);
		intRect2 = IntRect.Intersection(a, b);
		for (int k = intRect2.xmin; k <= intRect2.xmax; k++)
		{
			for (int l = intRect2.ymin; l <= intRect2.ymax; l++)
			{
				this.CalculateConnections(k, l);
			}
		}
	}

	// Token: 0x060041B6 RID: 16822 RVA: 0x000027FC File Offset: 0x000009FC
	[Conditional("DEBUG_PATHGRIDVALIDATE")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void Validate()
	{
	}

	// Token: 0x060041B7 RID: 16823 RVA: 0x0019A8E0 File Offset: 0x00198AE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SampleHeights(Vector3 pos)
	{
		this.CheckHeights(pos);
		int num = this.heightsUsed / 2;
		for (int i = 0; i < num; i++)
		{
			AstarVoxelGrid.HitData hitData = this.heights[i];
			this.heights[i] = this.heights[this.heightsUsed - 1 - i];
			this.heights[this.heightsUsed - 1 - i] = hitData;
		}
	}

	// Token: 0x060041B8 RID: 16824 RVA: 0x0019A950 File Offset: 0x00198B50
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckHeights(Vector3 position)
	{
		World world = GameManager.Instance.World;
		ChunkCluster chunkCache = world.ChunkCache;
		Vector3 position2 = Origin.position;
		int type = BlockValue.Air.type;
		this.heightsUsed = 0;
		int num = 0;
		Vector3 vector = position;
		vector.y += 320f;
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 down = Vector3.down;
		float num2 = float.MaxValue;
		do
		{
			AstarVoxelGrid.HitData hitData;
			hitData.hitFlags = AstarVoxelGrid.HitDataFlags.None;
			hitData.blockerFlags = 4096;
			RaycastHit raycastHit;
			if (!defaultPhysicsScene.Raycast(vector, down, out raycastHit, 320.01f, 1073807360, QueryTriggerInteraction.Ignore))
			{
				goto IL_1FD;
			}
			vector.y = raycastHit.point.y - 0.11f;
			if (raycastHit.point.y > num2)
			{
				hitData.hitFlags |= AstarVoxelGrid.HitDataFlags.InsideOversized;
			}
			else if (raycastHit.collider.CompareTag("T_Block"))
			{
				this.localWorldRayHit.Clear();
				Transform transform = raycastHit.collider.transform;
				Vector3 hitPointPos = raycastHit.point + Origin.position;
				if (GameUtils.FindMasterBlockForEntityModelBlock(world, down, string.Empty, hitPointPos, transform, this.localWorldRayHit))
				{
					BlockValue blockValue = this.localWorldRayHit.hit.blockValue;
					if (blockValue.Block.isOversized)
					{
						hitData.hitFlags |= AstarVoxelGrid.HitDataFlags.InsideOversized;
						Quaternion rotation = blockValue.Block.shape.GetRotation(blockValue);
						Vector3i vector3i;
						Vector3i vector3i2;
						OversizedBlockUtils.GetWorldAlignedBoundsExtents(this.localWorldRayHit.hit.blockPos, rotation, blockValue.Block.oversizedBounds, out vector3i, out vector3i2);
						num2 = (float)vector3i.y;
					}
				}
			}
			hitData.point = raycastHit.point;
			hitData.point.y = hitData.point.y + 0.05f;
			if (Vector3.Dot(raycastHit.normal, Vector3.up) < 0.707f)
			{
				hitData.hitFlags |= AstarVoxelGrid.HitDataFlags.InvalidCover;
			}
			AstarVoxelGrid.cellHits[num] = hitData;
		}
		while (++num < 512);
		Log.Warning("AstarVoxelGrid CheckHeights too many hits");
		IL_1FD:
		int num3 = Utils.Fastfloor(position.x + position2.x);
		int num4 = Utils.Fastfloor(position.z + position2.z);
		Vector3i vector3i3 = new Vector3i(num3, 0, num4);
		Vector3i vector3i4 = vector3i3;
		IChunk chunkFromWorldPos = world.GetChunkFromWorldPos(vector3i3);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		int x = World.toBlockXZ(num3);
		int z = World.toBlockXZ(num4);
		int i = 0;
		float num5 = 257f;
		IL_920:
		while (i < num)
		{
			float num6 = num5;
			AstarVoxelGrid.HitData hitData2 = AstarVoxelGrid.cellHits[i++];
			num5 = hitData2.point.y;
			float num7 = num6 - num5;
			vector3i3.y = Utils.Fastfloor(num5 + position2.y);
			BlockValue block = this.GetBlock(chunkFromWorldPos, x, vector3i3.y, z);
			int type2 = block.type;
			Block block2 = block.Block;
			if (block2.shape.IsTerrain())
			{
				AstarVoxelGrid.HitData[] array = this.heights;
				int num8 = this.heightsUsed;
				this.heightsUsed = num8 + 1;
				array[num8] = hitData2;
			}
			else
			{
				if (num7 > 0.95f)
				{
					if (block2.PathType > 0)
					{
						float num9 = (float)Utils.Fastfloor(hitData2.point.y);
						if (hitData2.point.y - num9 > 0.4f)
						{
							vector3i3.y++;
							block = this.GetBlock(chunkFromWorldPos, x, vector3i3.y, z);
							type2 = block.type;
							block2 = block.Block;
							num5 = num9 + 1.01f;
							hitData2.point.y = num5;
						}
					}
					int num8;
					if (block2.PathType > 0)
					{
						hitData2.blockerFlags = 4111;
					}
					else
					{
						if (type2 != type)
						{
							hitData2.blockerFlags |= this.CalcBlockingFlags(hitData2.point, 0.2f);
							Vector2 pathOffset = block2.GetPathOffset((int)block.rotation);
							hitData2.point.x = hitData2.point.x + pathOffset.x;
							hitData2.point.z = hitData2.point.z + pathOffset.y;
						}
						vector3i4.y = vector3i3.y + 1;
						BlockValue block3 = this.GetBlock(chunkFromWorldPos, x, vector3i4.y, z);
						Block block4 = block3.Block;
						bool flag = block2.HasTag(BlockTags.Door) || block2.HasTag(BlockTags.ClosetDoor);
						bool flag2 = false;
						if (flag && block2.MaxDamagePlusDowngrades - block.damage == 1)
						{
							BlockShapeModelEntity blockShapeModelEntity = block2.shape as BlockShapeModelEntity;
							if (blockShapeModelEntity != null && !blockShapeModelEntity.IsObstructionForDamageState(block))
							{
								flag2 = true;
							}
						}
						if (!flag2)
						{
							if (flag)
							{
								if (!block2.isMultiBlock || !block.ischild || block.parenty == 0)
								{
									hitData2.blockerFlags |= 16384;
								}
							}
							else
							{
								flag = block4.HasTag(BlockTags.Door);
								if (flag && (!block4.isMultiBlock || !block3.ischild || block3.parenty == 0))
								{
									hitData2.blockerFlags |= 16384;
								}
							}
						}
						if (num7 > 2.95f && (block2.IsElevator((int)block.rotation) || block4.IsElevator((int)block3.rotation)))
						{
							hitData2.blockerFlags |= 8192;
							Vector3i vector3i5 = vector3i4;
							BlockValue blockValue2 = block3;
							Block block5 = block4;
							int num10 = (int)(num6 - 1f + position2.y);
							int num11 = 0;
							while (vector3i5.y <= num10)
							{
								if (block5.IsElevator((int)blockValue2.rotation))
								{
									num11 = 0;
								}
								else
								{
									if (!blockValue2.isair || num11 >= 1)
									{
										break;
									}
									num11++;
								}
								vector3i5.y++;
								blockValue2 = this.GetBlock(chunkFromWorldPos, x, vector3i5.y, z);
								block5 = blockValue2.Block;
							}
							vector3i5.y -= num11;
							AstarVoxelGrid.HitData hitData3 = default(AstarVoxelGrid.HitData);
							Vector3 vector2 = vector;
							float num12 = num5 + position2.y - -0.2f;
							while ((float)vector3i5.y > num12)
							{
								vector2.y = (float)vector3i5.y - position2.y;
								hitData3.blockerFlags = (8192 | this.CalcBlockingFlags(vector2, 0f));
								hitData3.point.x = vector2.x;
								hitData3.point.z = vector2.z;
								hitData3.point.y = vector2.y + -0.2f;
								hitData3.hitFlags |= AstarVoxelGrid.HitDataFlags.InvalidCover;
								AstarVoxelGrid.HitData[] array2 = this.heights;
								num8 = this.heightsUsed;
								this.heightsUsed = num8 + 1;
								array2[num8] = hitData3;
								vector3i5.y--;
							}
						}
					}
					AstarVoxelGrid.HitData[] array3 = this.heights;
					num8 = this.heightsUsed;
					this.heightsUsed = num8 + 1;
					array3[num8] = hitData2;
				}
				else
				{
					num5 = num6;
				}
				float num13 = float.MinValue;
				if (i < num)
				{
					num13 = AstarVoxelGrid.cellHits[i].point.y;
				}
				for (;;)
				{
					vector3i3.y--;
					vector.y = (float)vector3i3.y - position2.y;
					if (vector.y <= num13)
					{
						goto IL_920;
					}
					if (vector3i3.y < 0)
					{
						break;
					}
					block = this.GetBlock(chunkFromWorldPos, x, vector3i3.y, z);
					type2 = block.type;
					if (type2 == type)
					{
						goto IL_920;
					}
					block2 = block.Block;
					if (block2.shape.IsTerrain() || block2.IsElevator())
					{
						goto IL_920;
					}
					if (!block2.HasTag(BlockTags.Door) && block2.PathType > 0 && block2.IsMovementBlocked(world, vector3i3, block, BlockFace.Top))
					{
						bool flag3 = true;
						for (int j = 0; j < 4; j++)
						{
							Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[j];
							Vector3i vector3i6 = vector3i3;
							vector3i6.x += vector2i.x;
							vector3i6.z += vector2i.y;
							block = chunkCache.GetBlock(vector3i6);
							block2 = block.Block;
							if (block2.PathType <= 0 || !block2.IsMovementBlocked(world, vector3i6, block, BlockFace.Top))
							{
								vector3i6.y--;
								block = chunkCache.GetBlock(vector3i6);
								block2 = block.Block;
								if (block2.PathType > 0)
								{
									flag3 = false;
								}
								else if (block2.IsMovementBlocked(world, vector3i6, block, BlockFace.Top))
								{
									Vector3 origin;
									origin.x = (float)vector3i6.x - position2.x;
									origin.y = vector.y + 0.51f;
									origin.z = (float)vector3i6.z - position2.z;
									RaycastHit raycastHit2;
									if (defaultPhysicsScene.Raycast(origin, down, out raycastHit2, 1.6f, 1073807360, QueryTriggerInteraction.Ignore))
									{
										flag3 = false;
										break;
									}
									break;
								}
							}
						}
						if (!flag3)
						{
							hitData2.point.y = vector.y + 0.03f;
							hitData2.blockerFlags = 4111;
							AstarVoxelGrid.HitData[] array4 = this.heights;
							int num8 = this.heightsUsed;
							this.heightsUsed = num8 + 1;
							array4[num8] = hitData2;
						}
					}
				}
				i = int.MaxValue;
			}
		}
	}

	// Token: 0x060041B9 RID: 16825 RVA: 0x0019B288 File Offset: 0x00199488
	public override void RecalculateCell(int x, int z, bool resetPenalties = true, bool resetTags = true)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		if (world.ChunkCache == null)
		{
			return;
		}
		Vector3 vector = base.transform.Transform(new Vector3((float)x + 0.5f, 0f, (float)z + 0.5f));
		this.SampleHeights(vector);
		if (this.heightsUsed > this.layerCount)
		{
			if (this.heightsUsed > 255)
			{
				UnityEngine.Debug.LogError("Too many layers " + this.heightsUsed.ToString());
				return;
			}
			base.AddLayers(this.heightsUsed - this.layerCount);
		}
		Vector3 position = Origin.position;
		int num = Utils.Fastfloor(vector.x + position.x);
		int num2 = Utils.Fastfloor(vector.z + position.z);
		Vector3i vector3i = new Vector3i(num, 0, num2);
		IChunk chunkFromWorldPos = world.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		int x2 = World.toBlockXZ(num);
		int z2 = World.toBlockXZ(num2);
		int num3 = this.width * this.depth;
		int num4 = x + z * this.width;
		int i = 0;
		this.validHeights.Clear();
		while (i < this.heightsUsed)
		{
			int num5 = num4 + num3 * i;
			AstarVoxelGrid.VoxelNode voxelNode = (AstarVoxelGrid.VoxelNode)this.nodes[num5];
			if (voxelNode == null)
			{
				voxelNode = (this.nodes[num5] = AstarVoxelGrid.levelGridNodePool.Alloc(false));
				voxelNode.Init(this.active);
				voxelNode.NodeInGridIndex = num4;
				voxelNode.LayerCoordinateInGrid = i;
				voxelNode.GraphIndex = this.graphIndex;
			}
			Vector3 point = this.heights[i].point;
			voxelNode.position = (Int3)point;
			vector3i.y = Utils.Fastfloor(point.y + position.y);
			this.validHeights.Add(vector3i.y);
			voxelNode.ClearCustomConnections(true);
			voxelNode.Walkable = true;
			int num6 = 0;
			int num7 = 0;
			voxelNode.PenaltyHigh = 0;
			voxelNode.PenaltyLow = 0;
			voxelNode.BlockerFlags = this.heights[i].blockerFlags;
			if ((this.heights[i].hitFlags & AstarVoxelGrid.HitDataFlags.InsideOversized) != AstarVoxelGrid.HitDataFlags.None)
			{
				voxelNode.Walkable = false;
			}
			if ((voxelNode.BlockerFlags & 16384) > 0)
			{
				voxelNode.Tag = 3U;
				voxelNode.Penalty = (uint)num6;
				BlockValue block = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				int num8 = block.Block.MaxDamagePlusDowngrades - block.damage;
				voxelNode.PenaltyLow = (num8 + 10) * 20 / 3;
			}
			else
			{
				vector3i.y++;
				BlockValue block2;
				Block block3;
				if (point.y - (float)Utils.Fastfloor(point.y) > 0.4f)
				{
					vector3i.y++;
					block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
					block3 = block2.Block;
					if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
					{
						int num9 = block3.MaxDamagePlusDowngrades - block2.damage;
						voxelNode.PenaltyHigh = (num9 + 10) * 20;
						if (block3.PathType > 0)
						{
							num6 += voxelNode.PenaltyHigh;
							AstarVoxelGrid.VoxelNode voxelNode2 = voxelNode;
							voxelNode2.BlockerFlags |= 240;
						}
						else
						{
							int num10 = (int)this.CalcBlockingFlags(point, 1.5f);
							AstarVoxelGrid.VoxelNode voxelNode3 = voxelNode;
							voxelNode3.BlockerFlags |= (ushort)((num10 & 15) << 4);
						}
					}
					vector3i.y--;
				}
				block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				block3 = block2.Block;
				bool flag = false;
				if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
				{
					int num11 = block3.MaxDamagePlusDowngrades - block2.damage;
					voxelNode.PenaltyHigh += (num11 + 10) * 20;
					if (block3.PathType > 0)
					{
						num6 += voxelNode.PenaltyHigh;
						AstarVoxelGrid.VoxelNode voxelNode4 = voxelNode;
						voxelNode4.BlockerFlags |= 240;
					}
					else
					{
						bool flag2 = false;
						int num12 = i + 1;
						if (num12 < this.heightsUsed && Utils.Fastfloor(this.heights[num12].point.y + position.y) == vector3i.y)
						{
							flag2 = true;
							int blockerFlags = (int)this.heights[num12].blockerFlags;
							if ((blockerFlags & 4096) > 0)
							{
								num6 += voxelNode.PenaltyHigh;
								AstarVoxelGrid.VoxelNode voxelNode5 = voxelNode;
								voxelNode5.BlockerFlags |= 240;
							}
							else
							{
								AstarVoxelGrid.VoxelNode voxelNode6 = voxelNode;
								voxelNode6.BlockerFlags |= (ushort)(((blockerFlags >> 8 | blockerFlags) & 15) << 4);
							}
						}
						if (!flag2)
						{
							int num13 = (int)this.CalcBlockingFlags(point, 1f);
							AstarVoxelGrid.VoxelNode voxelNode7 = voxelNode;
							voxelNode7.BlockerFlags |= (ushort)((num13 & 15) << 4);
						}
					}
				}
				vector3i.y--;
				block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				block3 = block2.Block;
				if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
				{
					int num14 = block3.MaxDamagePlusDowngrades - block2.damage;
					voxelNode.PenaltyLow = (num14 + 10) * 20;
					if (block3.PathType > 0)
					{
						num6 += voxelNode.PenaltyLow;
					}
				}
				if (num7 > 0)
				{
					voxelNode.Tag = 1U;
				}
				else if (flag)
				{
					voxelNode.Tag = 2U;
				}
				else
				{
					voxelNode.Tag = 0U;
				}
				if (block3.IsElevator((int)block2.rotation))
				{
					voxelNode.Tag = 4U;
				}
				if (num6 > 268435455)
				{
					Log.Warning("RecalculateCell {0}, id{1} {2}, pen {3}", new object[]
					{
						vector3i,
						block3.blockID,
						block3.GetBlockName(),
						num6
					});
					if (num6 < 0)
					{
						num6 = 0;
					}
					else
					{
						num6 = 268435455;
					}
				}
				voxelNode.Penalty = (uint)num6;
			}
			i++;
		}
		int num15 = num4 + num3 * i;
		while (i < this.layerCount)
		{
			LevelGridNode levelGridNode = this.nodes[num15];
			if (levelGridNode != null)
			{
				levelGridNode.Destroy();
				this.nodes[num15] = null;
				AstarVoxelGrid.levelGridNodePool.Free((AstarVoxelGrid.VoxelNode)levelGridNode);
			}
			num15 += num3;
			i++;
		}
	}

	// Token: 0x060041BA RID: 16826 RVA: 0x0019B8CE File Offset: 0x00199ACE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x060041BB RID: 16827 RVA: 0x0019B8D8 File Offset: 0x00199AD8
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort CalcBlockingFlags(Vector3 pos, float offsetY)
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		int num = 0;
		pos.y += 0.2f + offsetY;
		Vector3 vector;
		vector.y = 0f;
		for (int i = 0; i < 4; i++)
		{
			Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[i];
			vector.x = (float)vector2i.x;
			vector.z = (float)vector2i.y;
			Vector3 origin = pos - vector * 0.2f;
			RaycastHit raycastHit;
			if (defaultPhysicsScene.SphereCast(origin, 0.1f, vector, out raycastHit, 0.59f, 1073807360, QueryTriggerInteraction.Ignore))
			{
				if (offsetY > 0.5f || raycastHit.normal.y < 0.643f)
				{
					num |= 1 << i;
				}
				else if (Vector3.Dot(vector, raycastHit.normal) > -0.35f)
				{
					num |= 1 << i;
				}
				else
				{
					num |= 256 << i;
				}
			}
		}
		return (ushort)num;
	}

	// Token: 0x060041BC RID: 16828 RVA: 0x0019B9D0 File Offset: 0x00199BD0
	public override void CalculateConnections(int x, int z, int layerIndex)
	{
		int num = this.width * this.depth;
		int num2 = z * this.width + x + num * layerIndex;
		AstarVoxelGrid.VoxelNode voxelNode = (AstarVoxelGrid.VoxelNode)this.nodes[num2];
		if (voxelNode == null)
		{
			return;
		}
		voxelNode.ResetAllGridConnections();
		if (!voxelNode.Walkable)
		{
			return;
		}
		Vector3 vector = (Vector3)voxelNode.position;
		World.worldToBlockPos(vector + Origin.position);
		float y = vector.y;
		float num3 = y + this.characterHeight;
		vector.y += 0.5f;
		Vector3i pos = World.worldToBlockPos(vector + Origin.position);
		World world = GameManager.Instance.World;
		world.GetBlock(pos);
		if ((voxelNode.BlockerFlags & 8192) > 0 && layerIndex + 1 < this.layerCount)
		{
			LevelGridNode levelGridNode = this.nodes[num2 + num];
			if (levelGridNode != null && (float)levelGridNode.position.y * 0.001f - y < 2.1f)
			{
				this.AddConnection(voxelNode, levelGridNode, 500U, 4U, null);
				this.AddConnection(levelGridNode, voxelNode, 250U, 4U, null);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[i];
			int num4 = x + vector2i.x;
			if ((ulong)num4 < (ulong)((long)this.width))
			{
				int num5 = z + vector2i.y;
				if ((ulong)num5 < (ulong)((long)this.depth))
				{
					int num6 = num5 * this.width + num4;
					int num7 = 255;
					float num8 = 0f;
					for (int j = 0; j < this.layerCount; j++)
					{
						int num9 = num6 + j * num;
						AstarVoxelGrid.VoxelNode voxelNode2 = (AstarVoxelGrid.VoxelNode)this.nodes[num9];
						if (voxelNode2 != null && voxelNode2.Walkable)
						{
							float num10 = (float)voxelNode2.position.y * 0.001f;
							float num11;
							if (j == this.layerCount - 1 || this.nodes[num9 + num] == null)
							{
								num11 = float.PositiveInfinity;
							}
							else
							{
								num11 = (float)this.nodes[num9 + num].position.y * 0.001f - num10;
								if (num11 <= -0.001f)
								{
									LevelGridNode levelGridNode2 = this.nodes[num9 + num];
									Utils.DrawLine((Vector3)voxelNode.position, (Vector3)voxelNode2.position, new Color(1f, 0f, 1f), new Color(1f, 0.5f, 0f), 3, 5f);
									Utils.DrawLine((Vector3)voxelNode2.position, (Vector3)levelGridNode2.position, new Color(1f, 0f, 0f), new Color(1f, 1f, 0f), 2, 5f);
									Log.Warning("Path node otherHeight bad {0}, {1}, {2}", new object[]
									{
										num11,
										levelGridNode2.position,
										voxelNode2.position
									});
								}
							}
							float num12 = num10 - y;
							if (num12 < -0.1f)
							{
								if (num12 >= -9.4f && num3 < num10 + num11)
								{
									num7 = j;
									num8 = num12;
								}
							}
							else
							{
								if (num12 >= 1.51f)
								{
									break;
								}
								if (num11 >= 0.7f)
								{
									if (num12 >= 0.6f)
									{
										if ((voxelNode.BlockerFlags & 15) != 15 && ((int)voxelNode.BlockerFlags & 16 << i) == 0 && ((int)voxelNode2.BlockerFlags & 17 << (i ^ 2)) == 0)
										{
											if (num12 >= 1.05f || ((int)voxelNode2.BlockerFlags & 256 << i) == 0)
											{
												if (((int)voxelNode2.BlockerFlags & 256 << (i ^ 2)) == 0)
												{
													this.AddConnection(voxelNode, voxelNode2, (uint)(num12 * 8000f), 0U, null);
													this.AddDummyConnection(voxelNode2, voxelNode);
													if ((voxelNode.BlockerFlags & 8192) == 0)
													{
														num7 = 255;
														break;
													}
													break;
												}
											}
											else
											{
												num7 = j;
												num8 = 0f;
											}
										}
									}
									else
									{
										if (voxelNode2.Tag == 3U)
										{
											Vector3i vector3i = World.worldToBlockPos((Vector3)voxelNode2.position + Origin.position);
											Block block = world.GetBlock(vector3i).Block;
											TEFeatureDoor payload = null;
											if (block.HasTag(BlockTags.Door) && block is BlockCompositeTileEntity)
											{
												TileEntityComposite tileEntityComposite = world.GetTileEntity(vector3i) as TileEntityComposite;
												if (tileEntityComposite != null)
												{
													tileEntityComposite.TryGetSelfOrFeature(out payload);
												}
											}
											this.AddConnection(voxelNode, voxelNode2, 2000U, 3U, payload);
											this.AddDummyConnection(voxelNode2, voxelNode);
										}
										if ((voxelNode2.BlockerFlags & 12288) > 0)
										{
											num7 = j;
											num8 = 0f;
										}
									}
								}
							}
						}
					}
					if (num7 != 255)
					{
						int num13 = num6 + num7 * num;
						AstarVoxelGrid.VoxelNode voxelNode3 = (AstarVoxelGrid.VoxelNode)this.nodes[num13];
						bool flag = false;
						int num14 = (int)(voxelNode.BlockerFlags & 15);
						if (num14 == 0 || num14 == 15)
						{
							num14 = (int)(voxelNode.BlockerFlags & 240);
							if (num14 == 0 || num14 == 240)
							{
								num14 = (int)(voxelNode3.BlockerFlags & 15);
								if (num8 <= -0.95f || num14 == 0 || num14 == 15)
								{
									num14 = (int)(voxelNode3.BlockerFlags & 240);
									if (num14 == 0 || num14 == 240)
									{
										flag = true;
									}
								}
							}
						}
						if (!flag)
						{
							int num15 = 0;
							if (((int)voxelNode.BlockerFlags & 16 << i) > 0)
							{
								num15 += voxelNode.PenaltyHigh;
							}
							if (((int)voxelNode3.BlockerFlags & 16 << (i ^ 2)) > 0)
							{
								num15 += voxelNode3.PenaltyHigh;
							}
							if (((int)voxelNode.BlockerFlags & 1 << i) > 0)
							{
								num15 += voxelNode.PenaltyLow;
							}
							if (((int)voxelNode3.BlockerFlags & 1 << (i ^ 2)) > 0 && num8 > -0.95f)
							{
								num15 += voxelNode3.PenaltyLow;
							}
							if (num15 > 0)
							{
								this.AddConnection(voxelNode, voxelNode3, (uint)num15, 1U, null);
								this.AddDummyConnection(voxelNode3, voxelNode);
								num7 = 255;
							}
						}
						voxelNode.SetConnectionValue(i, num7);
					}
				}
			}
		}
	}

	// Token: 0x060041BD RID: 16829 RVA: 0x0019BFF0 File Offset: 0x0019A1F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddConnection(GridNodeBase node, GridNodeBase other, uint cost, uint tag, object payload)
	{
		Connection[] connections = node.connections;
		int num = 0;
		if (connections != null)
		{
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].node == other)
				{
					connections[i].cost = cost;
					connections[i].tag = tag;
					connections[i].payload = payload;
					return;
				}
			}
			num = connections.Length;
		}
		num++;
		Connection[] array = AstarVoxelGrid.AllocConnection(num);
		for (int j = 0; j < num - 1; j++)
		{
			array[j] = connections[j];
		}
		array[num - 1] = new Connection(other, cost, tag, payload, byte.MaxValue);
		node.connections = array;
	}

	// Token: 0x060041BE RID: 16830 RVA: 0x0019C0A4 File Offset: 0x0019A2A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddDummyConnection(GridNodeBase node, GridNodeBase other)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].node == other)
				{
					return;
				}
			}
		}
		this.AddConnection(node, other, uint.MaxValue, 0U, null);
	}

	// Token: 0x060041BF RID: 16831 RVA: 0x0019C0E4 File Offset: 0x0019A2E4
	public static void ClearConnections(GridNodeBase node)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			node.connections = null;
			int num = connections.Length;
			for (int i = 0; i < num; i++)
			{
				AstarVoxelGrid.RemoveConnection((GridNodeBase)connections[i].node, node);
			}
			if (num < 16)
			{
				AstarVoxelGrid.connectionsPool[num].Add(connections);
			}
		}
	}

	// Token: 0x060041C0 RID: 16832 RVA: 0x0019C13C File Offset: 0x0019A33C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RemoveConnection(GridNodeBase node, GridNodeBase other)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			int num = connections.Length;
			int i = 0;
			while (i < num)
			{
				if (connections[i].node == other)
				{
					if (num <= 1)
					{
						node.connections = null;
					}
					else
					{
						Connection[] array = AstarVoxelGrid.AllocConnection(num - 1);
						int j;
						for (j = 0; j < i; j++)
						{
							array[j] = connections[j];
						}
						while (j < array.Length)
						{
							array[j] = connections[j + 1];
							j++;
						}
						node.connections = array;
					}
					if (num < 16)
					{
						AstarVoxelGrid.connectionsPool[num].Add(connections);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060041C1 RID: 16833 RVA: 0x0019C1EC File Offset: 0x0019A3EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static Connection[] AllocConnection(int count)
	{
		Connection[] array = null;
		if (count < 16)
		{
			List<Connection[]> list = AstarVoxelGrid.connectionsPool[count];
			int count2 = list.Count;
			if (count2 > 0)
			{
				array = list[count2 - 1];
				list.RemoveAt(count2 - 1);
			}
		}
		if (array == null)
		{
			array = new Connection[count];
		}
		return array;
	}

	// Token: 0x060041C2 RID: 16834 RVA: 0x0019C231 File Offset: 0x0019A431
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue GetBlock(IChunk chunk, int _x, int _y, int _z)
	{
		if (_y >= 256)
		{
			return BlockValue.Air;
		}
		return chunk.GetBlock(_x, _y, _z);
	}

	// Token: 0x060041C3 RID: 16835 RVA: 0x0019C24B File Offset: 0x0019A44B
	public void SetPos(Vector3 pos)
	{
		this.center = pos;
		this.IsFullUpdateNeeded = false;
		this.InitScan();
	}

	// Token: 0x060041C4 RID: 16836 RVA: 0x0019C261 File Offset: 0x0019A461
	public void Move(Vector3 targetPos)
	{
		this.gridMover.graph = this;
		this.gridMover.targetPosition = targetPos;
		this.gridMover.UpdateGraph();
	}

	// Token: 0x060041C5 RID: 16837 RVA: 0x0019C286 File Offset: 0x0019A486
	public bool IsMoving()
	{
		return this.gridMover.updatingGraph;
	}

	// Token: 0x04003532 RID: 13618
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cGridHeight = 320f;

	// Token: 0x04003533 RID: 13619
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cGridHeightPadded = 320.01f;

	// Token: 0x04003534 RID: 13620
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCollisionMask = 1073807360;

	// Token: 0x04003535 RID: 13621
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLayerMinHeight = 0.7f;

	// Token: 0x04003536 RID: 13622
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cClimbMinHeight = 0.6f;

	// Token: 0x04003537 RID: 13623
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cClimbMaxHeight = 1.51f;

	// Token: 0x04003538 RID: 13624
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDropOnTopHeight = 0.95f;

	// Token: 0x04003539 RID: 13625
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDropMaxHeight = 9.4f;

	// Token: 0x0400353A RID: 13626
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cJumpPenalty = 8;

	// Token: 0x0400353B RID: 13627
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDoorPenalty = 2;

	// Token: 0x0400353C RID: 13628
	public const int cPenaltyPerMeter = 1000;

	// Token: 0x0400353D RID: 13629
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPenaltyHealthBase = 10;

	// Token: 0x0400353E RID: 13630
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPenaltyHealthScale = 20;

	// Token: 0x0400353F RID: 13631
	public const uint cDummyPenalty = 4294967295U;

	// Token: 0x04003540 RID: 13632
	public const int cTagOpen = 0;

	// Token: 0x04003541 RID: 13633
	public const int cTagBreak = 1;

	// Token: 0x04003542 RID: 13634
	public const int cTagLowHeight = 2;

	// Token: 0x04003543 RID: 13635
	public const int cTagDoor = 3;

	// Token: 0x04003544 RID: 13636
	public const int cTagLadder = 4;

	// Token: 0x04003545 RID: 13637
	public const int cTagTest = 8;

	// Token: 0x04003546 RID: 13638
	public const ushort cBlockerFlagLow0 = 1;

	// Token: 0x04003547 RID: 13639
	public const ushort cBlockerFlagLow = 15;

	// Token: 0x04003548 RID: 13640
	public const ushort cBlockerFlagHigh0 = 16;

	// Token: 0x04003549 RID: 13641
	public const ushort cBlockerFlagHigh = 240;

	// Token: 0x0400354A RID: 13642
	public const ushort cBlockerFlagHighLow0 = 17;

	// Token: 0x0400354B RID: 13643
	public const ushort cBlockerFlagHighLow = 255;

	// Token: 0x0400354C RID: 13644
	public const ushort cBlockerFlagSlopeDir0 = 256;

	// Token: 0x0400354D RID: 13645
	public const ushort cBlockerFlagFloor = 4096;

	// Token: 0x0400354E RID: 13646
	public const ushort cBlockerFlagLadder = 8192;

	// Token: 0x0400354F RID: 13647
	public const ushort cBlockerFlagDoor = 16384;

	// Token: 0x04003550 RID: 13648
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector2i[] neighboursOffsetV2 = new Vector2i[]
	{
		new Vector2i(0, -1),
		new Vector2i(1, 0),
		new Vector2i(0, 1),
		new Vector2i(-1, 0)
	};

	// Token: 0x04003551 RID: 13649
	public bool IsUsed;

	// Token: 0x04003552 RID: 13650
	public bool IsFullUpdateNeeded;

	// Token: 0x04003553 RID: 13651
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid.ProceduralGridMover gridMover;

	// Token: 0x04003554 RID: 13652
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRayHitsMax = 512;

	// Token: 0x04003555 RID: 13653
	[PublicizedFrom(EAccessModifier.Private)]
	public static AstarVoxelGrid.HitData[] cellHits = new AstarVoxelGrid.HitData[512];

	// Token: 0x04003556 RID: 13654
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid.HitData[] heights = new AstarVoxelGrid.HitData[512];

	// Token: 0x04003557 RID: 13655
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightsUsed;

	// Token: 0x04003558 RID: 13656
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryPooledObject<AstarVoxelGrid.VoxelNode> levelGridNodePool = new MemoryPooledObject<AstarVoxelGrid.VoxelNode>(100000);

	// Token: 0x04003559 RID: 13657
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cConnectionPoolMax = 16;

	// Token: 0x0400355A RID: 13658
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Connection[]>[] connectionsPool;

	// Token: 0x0400355B RID: 13659
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly WorldRayHitInfo localWorldRayHit = new WorldRayHitInfo();

	// Token: 0x0400355C RID: 13660
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<int> validHeights = new HashSet<int>();

	// Token: 0x0400355D RID: 13661
	public Vector2 GridMovePendingPos;

	// Token: 0x020008DB RID: 2267
	[Flags]
	public enum HitDataFlags : ushort
	{
		// Token: 0x0400355F RID: 13663
		None = 0,
		// Token: 0x04003560 RID: 13664
		InvalidCover = 1,
		// Token: 0x04003561 RID: 13665
		InsideOversized = 2
	}

	// Token: 0x020008DC RID: 2268
	public struct HitData
	{
		// Token: 0x04003562 RID: 13666
		public Vector3 point;

		// Token: 0x04003563 RID: 13667
		public ushort blockerFlags;

		// Token: 0x04003564 RID: 13668
		public AstarVoxelGrid.HitDataFlags hitFlags;
	}

	// Token: 0x020008DD RID: 2269
	public class VoxelNode : LevelGridNode, IMemoryPoolableObject
	{
		// Token: 0x060041C9 RID: 16841 RVA: 0x000027FC File Offset: 0x000009FC
		public void Reset()
		{
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x000027FC File Offset: 0x000009FC
		public void Cleanup()
		{
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x0019C33A File Offset: 0x0019A53A
		public override void ClearCustomConnections(bool alsoReverse)
		{
			AstarVoxelGrid.ClearConnections(this);
		}

		// Token: 0x060041CC RID: 16844 RVA: 0x0019C344 File Offset: 0x0019A544
		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			handler.heap.Add(pathNode);
			pathNode.UpdateG(path);
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = base.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					LevelGridNode levelGridNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					PathNode pathNode2 = handler.GetPathNode(levelGridNode);
					if (pathNode2 != null && pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
					{
						levelGridNode.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
			if (this.connections != null)
			{
				ushort pathID = handler.PathID;
				for (int j = 0; j < this.connections.Length; j++)
				{
					if (this.connections[j].cost != 4294967295U)
					{
						GraphNode node = this.connections[j].node;
						PathNode pathNode3 = handler.GetPathNode(node);
						if (pathNode3.parent == pathNode && pathNode3.pathID == pathID)
						{
							node.UpdateRecursiveG(path, pathNode3, handler);
						}
					}
				}
			}
		}

		// Token: 0x04003565 RID: 13669
		public int PenaltyHigh;

		// Token: 0x04003566 RID: 13670
		public int PenaltyLow;

		// Token: 0x04003567 RID: 13671
		public ushort BlockerFlags;
	}

	// Token: 0x020008DE RID: 2270
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_procedural_grid_mover.php")]
	public class ProceduralGridMover
	{
		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x060041CE RID: 16846 RVA: 0x0019C477 File Offset: 0x0019A677
		// (set) Token: 0x060041CF RID: 16847 RVA: 0x0019C47F File Offset: 0x0019A67F
		public bool updatingGraph { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x060041D0 RID: 16848 RVA: 0x0019C488 File Offset: 0x0019A688
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 PointToGraphSpace(Vector3 p)
		{
			return this.graph.transform.InverseTransform(p);
		}

		// Token: 0x060041D1 RID: 16849 RVA: 0x0019C49C File Offset: 0x0019A69C
		public void UpdateGraph()
		{
			if (this.updatingGraph)
			{
				return;
			}
			this.updatingGraph = true;
			IEnumerator ie = this.UpdateGraphCoroutine();
			AstarPath.active.AddWorkItem(new AstarWorkItem(delegate(IWorkItemContext context, bool force)
			{
				if (force)
				{
					while (ie.MoveNext())
					{
					}
				}
				bool flag;
				try
				{
					flag = !ie.MoveNext();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
					flag = true;
				}
				if (flag)
				{
					this.updatingGraph = false;
				}
				return flag;
			}));
		}

		// Token: 0x060041D2 RID: 16850 RVA: 0x0019C4ED File Offset: 0x0019A6ED
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator UpdateGraphCoroutine()
		{
			Vector3 vector = this.PointToGraphSpace(this.targetPosition) - this.PointToGraphSpace(this.graph.center);
			vector.x = Mathf.Round(vector.x);
			vector.z = Mathf.Round(vector.z);
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				yield break;
			}
			Int2 offset = new Int2(-Mathf.RoundToInt(vector.x), -Mathf.RoundToInt(vector.z));
			this.graph.center = this.targetPosition;
			this.graph.UpdateTransform();
			int width = this.graph.width;
			int depth = this.graph.depth;
			int layers = this.graph.LayerCount;
			LayerGridGraph layerGridGraph = this.graph as LayerGridGraph;
			GridNodeBase[] nodes;
			if (layerGridGraph != null)
			{
				GridNodeBase[] nodes2 = layerGridGraph.nodes;
				nodes = nodes2;
			}
			else
			{
				GridNodeBase[] nodes2 = this.graph.nodes;
				nodes = nodes2;
			}
			if (this.buffer == null || this.buffer.Length != width * depth)
			{
				this.buffer = new GridNodeBase[width * depth];
			}
			if (Mathf.Abs(offset.x) <= width && Mathf.Abs(offset.y) <= depth)
			{
				IntRect recalculateRect = new IntRect(0, 0, offset.x, offset.y);
				if (recalculateRect.xmin > recalculateRect.xmax)
				{
					int xmax = recalculateRect.xmax;
					recalculateRect.xmax = width + recalculateRect.xmin;
					recalculateRect.xmin = width + xmax;
				}
				if (recalculateRect.ymin > recalculateRect.ymax)
				{
					int ymax = recalculateRect.ymax;
					recalculateRect.ymax = depth + recalculateRect.ymin;
					recalculateRect.ymin = depth + ymax;
				}
				IntRect connectionRect = recalculateRect.Expand(1);
				connectionRect = IntRect.Intersection(connectionRect, new IntRect(0, 0, width, depth));
				int widthStart = width - offset.x;
				int destOffset = offset.x;
				if (offset.x < 0)
				{
					widthStart = -offset.x;
					destOffset += width;
				}
				int num9;
				for (int i = 0; i < layers; i = num9 + 1)
				{
					int num = i * width * depth;
					for (int j = 0; j < depth; j++)
					{
						int num2 = j * width;
						int num3 = (j + offset.y + depth) % depth * width;
						int num4 = num + num2;
						Array.Copy(nodes, num4, this.buffer, num3 + destOffset, widthStart);
						Array.Copy(nodes, num4 + widthStart, this.buffer, num3, width - widthStart);
					}
					for (int k = 0; k < depth; k++)
					{
						int num5 = k * width;
						for (int l = 0; l < width; l++)
						{
							int num6 = num5 + l;
							GridNodeBase gridNodeBase = this.buffer[num6];
							if (gridNodeBase != null)
							{
								gridNodeBase.NodeInGridIndex = num6;
							}
							nodes[num + num6] = gridNodeBase;
						}
						int num7;
						int num8;
						if (k >= recalculateRect.ymin && k < recalculateRect.ymax)
						{
							num7 = 0;
							num8 = depth;
						}
						else
						{
							num7 = recalculateRect.xmin;
							num8 = recalculateRect.xmax;
						}
						for (int m = num7; m < num8; m++)
						{
							GridNodeBase gridNodeBase2 = this.buffer[num5 + m];
							if (gridNodeBase2 != null)
							{
								gridNodeBase2.ClearConnections(false);
							}
						}
					}
					if ((i & 7) == 7)
					{
						yield return null;
					}
					num9 = i;
				}
				int yieldEvery = 160;
				int num10 = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) * Mathf.Max(width, depth);
				yieldEvery = Mathf.Max(yieldEvery, num10 / 10);
				int counter = 0;
				for (int i = 0; i < depth; i = num9 + 1)
				{
					int num11;
					int num12;
					if (i >= recalculateRect.ymin && i < recalculateRect.ymax)
					{
						num11 = 0;
						num12 = width;
					}
					else
					{
						num11 = recalculateRect.xmin;
						num12 = recalculateRect.xmax;
					}
					for (int n = num11; n < num12; n++)
					{
						this.graph.RecalculateCell(n, i, false, false);
					}
					counter += num12 - num11;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num9 = i;
				}
				yieldEvery *= 48;
				for (int i = 0; i < depth; i = num9 + 1)
				{
					int num13;
					int num14;
					if (i >= connectionRect.ymin && i < connectionRect.ymax)
					{
						num13 = 0;
						num14 = width;
					}
					else
					{
						num13 = connectionRect.xmin;
						num14 = connectionRect.xmax;
					}
					for (int num15 = num13; num15 < num14; num15++)
					{
						this.graph.CalculateConnections(num15, i);
					}
					counter += (num14 - num13) * layers;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num9 = i;
				}
				yield return null;
				for (int num16 = 0; num16 < depth; num16++)
				{
					for (int num17 = 0; num17 < width; num17++)
					{
						if (num17 == 0 || num16 == 0 || num17 == width - 1 || num16 == depth - 1)
						{
							this.graph.CalculateConnections(num17, num16);
						}
					}
				}
				recalculateRect = default(IntRect);
				connectionRect = default(IntRect);
			}
			else
			{
				int counter = Mathf.Max(depth * width / 20, 1000);
				int yieldEvery = 0;
				int num9;
				for (int destOffset = 0; destOffset < depth; destOffset = num9 + 1)
				{
					for (int num18 = 0; num18 < width; num18++)
					{
						this.graph.RecalculateCell(num18, destOffset, true, true);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num9 = destOffset;
				}
				for (int destOffset = 0; destOffset < depth; destOffset = num9 + 1)
				{
					for (int num19 = 0; num19 < width; num19++)
					{
						this.graph.CalculateConnections(num19, destOffset);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num9 = destOffset;
				}
			}
			yield break;
		}

		// Token: 0x04003568 RID: 13672
		public float updateDistance = 10f;

		// Token: 0x04003569 RID: 13673
		public Vector3 targetPosition;

		// Token: 0x0400356A RID: 13674
		public GridGraph graph;

		// Token: 0x0400356B RID: 13675
		[PublicizedFrom(EAccessModifier.Private)]
		public GridNodeBase[] buffer;
	}
}
