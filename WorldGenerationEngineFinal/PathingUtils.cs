using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001708 RID: 5896
	[BurstCompile(CompileSynchronously = true)]
	public class PathingUtils
	{
		// Token: 0x0600B7B7 RID: 47031 RVA: 0x004464CD File Offset: 0x004446CD
		public PathingUtils(WorldBuilder _worldBuilder)
		{
			PathingUtils.worldBuilder = _worldBuilder;
			this.data.nodePool = new PathNodePool(100000);
		}

		// Token: 0x0600B7B8 RID: 47032 RVA: 0x00446508 File Offset: 0x00444708
		public int GetPathCost(Vector2i start, Vector2i end, bool isCountryRoad = false)
		{
			this.InitPathData();
			int num = 0;
			WorldBuilder worldBuilder = PathingUtils.worldBuilder;
			Vector2i vector2i = start / 10;
			Vector2i vector2i2 = end / 10;
			PathNode pathNode;
			for (int i = PathingUtils.FindDetailedPath(ref worldBuilder.data, ref this.data, vector2i, vector2i2, isCountryRoad, false); i >= 0; i = pathNode.pathNext)
			{
				this.data.nodePool.Node(i, out pathNode);
				num++;
			}
			this.data.nodePool.ReturnAll();
			return num;
		}

		// Token: 0x0600B7B9 RID: 47033 RVA: 0x00446584 File Offset: 0x00444784
		public NativeList<Vector2i> GetPath(Vector2i _start, Vector2i _end, bool _isCountryRoad, bool _isRiver = false)
		{
			this.InitPathData();
			WorldBuilder worldBuilder = PathingUtils.worldBuilder;
			Vector2i vector2i = _start / 10;
			Vector2i vector2i2 = _end / 10;
			int i = PathingUtils.FindDetailedPath(ref worldBuilder.data, ref this.data, vector2i, vector2i2, _isCountryRoad, _isRiver);
			this.pathTemp.Clear();
			while (i >= 0)
			{
				PathNode pathNode;
				this.data.nodePool.Node(i, out pathNode);
				Vector2i vector2i3 = pathNode.position * 10 + PathingUtils.stepHalf;
				this.pathTemp.Add(vector2i3);
				i = pathNode.pathNext;
			}
			this.data.nodePool.ReturnAll();
			return this.pathTemp;
		}

		// Token: 0x0600B7BA RID: 47034 RVA: 0x00446630 File Offset: 0x00444830
		public Vector2i GetPathPoint(Vector2i _start, ref NativeList<Vector2> _endPath, bool _isCountryRoad, bool _isRiver, out int _cost)
		{
			this.InitPathData();
			this.ConvertPathToTemp(ref _endPath);
			Vector2i result = Vector2i.min;
			int num = 0;
			WorldBuilder worldBuilder = PathingUtils.worldBuilder;
			Vector2i vector2i = _start / 10;
			int i = PathingUtils.FindDetailedPath(ref worldBuilder.data, ref this.data, vector2i, this.pathTemp, _isCountryRoad, _isRiver);
			if (i >= 0)
			{
				PathNode pathNode;
				this.data.nodePool.Node(i, out pathNode);
				result = pathNode.position * 10 + PathingUtils.stepHalf;
				Vector2 vector = result.AsVector2();
				Vector2 vector2;
				if (PathingUtils.FindClosestPathPoint(_endPath, vector, out vector2, 1) < 400f)
				{
					result = new Vector2i(vector2);
				}
				while (i >= 0)
				{
					this.data.nodePool.Node(i, out pathNode);
					num++;
					i = pathNode.pathNext;
				}
			}
			this.data.nodePool.ReturnAll();
			_cost = num;
			return result;
		}

		// Token: 0x0600B7BB RID: 47035 RVA: 0x0044670A File Offset: 0x0044490A
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitPathData()
		{
			this.data.WorldSize = PathingUtils.worldBuilder.WorldSize;
		}

		// Token: 0x0600B7BC RID: 47036 RVA: 0x00446724 File Offset: 0x00444924
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConvertPathToTemp(ref NativeList<Vector2> _path)
		{
			this.pathTemp.Clear();
			for (int i = 0; i < _path.Length; i++)
			{
				Vector2i vector2i;
				vector2i.x = ((int)_path[i].x + PathingUtils.stepHalf.x) / 10;
				vector2i.y = ((int)_path[i].y + PathingUtils.stepHalf.y) / 10;
				this.pathTemp.Add(vector2i);
			}
		}

		// Token: 0x0600B7BD RID: 47037 RVA: 0x004467A0 File Offset: 0x004449A0
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe static void ClosedListInit(ref PathingUtils.Data data)
		{
			int num = data.WorldSize / 10 + 1;
			if (data.closedList.IsCreated)
			{
				int num2 = data.closedListMinY * data.closedListWidth;
				int num3 = (data.closedListMaxY + 1) * data.closedListWidth;
				void* destination = (void*)((byte*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(data.closedList) + num2);
				UnsafeUtility.MemSet(destination, 0, (long)(num3 - num2));
			}
			if (!data.closedList.IsCreated || data.closedListWidth != num)
			{
				data.closedListWidth = num;
				data.closedList = new NativeArray<byte>(num * num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}
		}

		// Token: 0x0600B7BE RID: 47038 RVA: 0x0044682A File Offset: 0x00444A2A
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Private)]
		public static int FindDetailedPath(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in Vector2i endPos, bool _isCountryRoad, bool _isRiver)
		{
			return PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Invoke(ref wd, ref data, startPos, endPos, _isCountryRoad, _isRiver);
		}

		// Token: 0x0600B7BF RID: 47039 RVA: 0x0044683C File Offset: 0x00444A3C
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DiagonalDist(Vector2i v1, Vector2i v2)
		{
			float num = Utils.FastAbs((float)(v1.x - v2.x));
			float num2 = Utils.FastAbs((float)(v1.y - v2.y));
			return num + num2 + -0.585786f * Utils.FastMin(num, num2);
		}

		// Token: 0x0600B7C0 RID: 47040 RVA: 0x00446882 File Offset: 0x00444A82
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Private)]
		public static int FindDetailedPath(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in NativeList<Vector2i> _endPath, bool _isCountryRoad, bool _isRiver)
		{
			return PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Invoke(ref wd, ref data, startPos, _endPath, _isCountryRoad, _isRiver);
		}

		// Token: 0x0600B7C1 RID: 47041 RVA: 0x00446891 File Offset: 0x00444A91
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CalcPathBounds(in NativeList<Vector2i> _path, out Vector2i _min, out Vector2i _max)
		{
			PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Invoke(_path, out _min, out _max);
		}

		// Token: 0x0600B7C2 RID: 47042 RVA: 0x0044689B File Offset: 0x00444A9B
		[BurstCompile(CompileSynchronously = true)]
		public static float FindClosestPathPoint(in NativeList<Vector2> _path, in Vector2 _startPos, out Vector2 _destPoint, int _step = 1)
		{
			return PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Invoke(_path, _startPos, out _destPoint, _step);
		}

		// Token: 0x0600B7C3 RID: 47043 RVA: 0x004468A6 File Offset: 0x00444AA6
		[BurstCompile(CompileSynchronously = true)]
		public static float FindClosestPathPoint(in NativeList<Vector2i> _path, in Vector2i _startPos, out Vector2i _destPoint)
		{
			return PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Invoke(_path, _startPos, out _destPoint);
		}

		// Token: 0x0600B7C4 RID: 47044 RVA: 0x004468B0 File Offset: 0x00444AB0
		[BurstCompile(CompileSynchronously = true)]
		public static bool IsPointOnPath(in NativeList<Vector2i> _path, in Vector2i _point)
		{
			return PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Invoke(_path, _point);
		}

		// Token: 0x0600B7C5 RID: 47045 RVA: 0x004468BC File Offset: 0x00444ABC
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool IsBlocked(ref WorldBuilder.Data wd, int pathX, int pathY, bool isRiver = false)
		{
			Vector2i vector2i = PathingUtils.pathPositionToWorldCenter(pathX, pathY);
			return !wd.InWorldBounds(vector2i.x, vector2i.y) || wd.GetStreetTileDataWorld(vector2i.x, vector2i.y).IsCity || (!isRiver && PathingUtils.IsWater(ref wd, pathX, pathY));
		}

		// Token: 0x0600B7C6 RID: 47046 RVA: 0x00446912 File Offset: 0x00444B12
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InBounds(ref WorldBuilder.Data wd, Vector2i pos)
		{
			return PathingUtils.InBounds(ref wd, pos.x, pos.y);
		}

		// Token: 0x0600B7C7 RID: 47047 RVA: 0x00446928 File Offset: 0x00444B28
		public static bool InBounds(ref WorldBuilder.Data wd, int pathX, int pathY)
		{
			Vector2i vector2i = PathingUtils.pathPositionToWorldCenter(pathX, pathY);
			return (ulong)vector2i.x < (ulong)((long)wd.WorldSize) && (ulong)vector2i.y < (ulong)((long)wd.WorldSize);
		}

		// Token: 0x0600B7C8 RID: 47048 RVA: 0x00446960 File Offset: 0x00444B60
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool IsWater(ref WorldBuilder.Data wd, int pathX, int pathY)
		{
			Vector2i vector2i = PathingUtils.pathPositionToWorldMin(pathX, pathY);
			if (wd.GetStreetTileDataWorld(vector2i.x, vector2i.y).OverlapsWater)
			{
				for (int i = vector2i.y; i < vector2i.y + 10; i++)
				{
					for (int j = vector2i.x; j < vector2i.x + 10; j++)
					{
						if (wd.GetWater(j, i) > 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600B7C9 RID: 47049 RVA: 0x004469CE File Offset: 0x00444BCE
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetHeight(ref WorldBuilder.Data wd, Vector2i pos)
		{
			return PathingUtils.GetHeight(ref wd, pos.x, pos.y);
		}

		// Token: 0x0600B7CA RID: 47050 RVA: 0x004469E2 File Offset: 0x00444BE2
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetHeight(ref WorldBuilder.Data wd, int pathX, int pathY)
		{
			return wd.GetHeight(PathingUtils.pathPositionToWorldCenter(pathX, pathY));
		}

		// Token: 0x0600B7CB RID: 47051 RVA: 0x004469F1 File Offset: 0x00444BF1
		public BiomeType GetBiome(Vector2i pos)
		{
			return this.GetBiome(pos.x, pos.y);
		}

		// Token: 0x0600B7CC RID: 47052 RVA: 0x00446A05 File Offset: 0x00444C05
		public BiomeType GetBiome(int pathX, int pathY)
		{
			return PathingUtils.worldBuilder.GetBiome(PathingUtils.pathPositionToWorldCenter(pathX, pathY));
		}

		// Token: 0x0600B7CD RID: 47053 RVA: 0x00446A18 File Offset: 0x00444C18
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2i pathPositionToWorldCenter(int pathX, int pathY)
		{
			Vector2i result;
			result.x = pathX * 10 + 5;
			result.y = pathY * 10 + 5;
			return result;
		}

		// Token: 0x0600B7CE RID: 47054 RVA: 0x00446A40 File Offset: 0x00444C40
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2i pathPositionToWorldMin(int pathX, int pathY)
		{
			Vector2i result;
			result.x = pathX * 10;
			result.y = pathY * 10;
			return result;
		}

		// Token: 0x0600B7CF RID: 47055 RVA: 0x00446A64 File Offset: 0x00444C64
		public void AddMoveLimitArea(Rect r)
		{
			int num = (int)r.xMin;
			int num2 = (int)r.yMin;
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					if (j != 7 && i != 7)
					{
						this.SetPathBlocked(num + j, num2 + i, true);
					}
				}
			}
		}

		// Token: 0x0600B7D0 RID: 47056 RVA: 0x00446ABC File Offset: 0x00444CBC
		public void RemoveFullyBlockedArea(Rect r)
		{
			int num = (int)r.xMin;
			int num2 = (int)r.yMin;
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					this.SetPathBlocked(num + j, num2 + i, false);
				}
			}
		}

		// Token: 0x0600B7D1 RID: 47057 RVA: 0x00446B0C File Offset: 0x00444D0C
		public void AddFullyBlockedArea(Rect r)
		{
			int num = (int)(r.xMin + 0.5f);
			int num2 = (int)(r.yMin + 0.5f);
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					this.SetPathBlocked(num + j, num2 + i, true);
				}
			}
		}

		// Token: 0x0600B7D2 RID: 47058 RVA: 0x00446B68 File Offset: 0x00444D68
		public void SetPathBlocked(Vector2i pos, bool isBlocked)
		{
			this.SetPathBlocked(pos.x, pos.y, isBlocked);
		}

		// Token: 0x0600B7D3 RID: 47059 RVA: 0x00446B7D File Offset: 0x00444D7D
		public void SetPathBlocked(int x, int y, bool isBlocked)
		{
			this.SetPathBlocked(x, y, isBlocked ? sbyte.MinValue : 0);
		}

		// Token: 0x0600B7D4 RID: 47060 RVA: 0x00446B90 File Offset: 0x00444D90
		public void SetPathBlocked(int x, int y, sbyte costMult)
		{
			if (!this.data.pathingGrid.IsCreated)
			{
				this.SetupPathingGrid();
			}
			if ((ulong)x >= (ulong)((long)this.data.pathingGridSize) || (ulong)y >= (ulong)((long)this.data.pathingGridSize))
			{
				return;
			}
			this.data.pathingGrid[x + y * this.data.pathingGridSize] = costMult;
		}

		// Token: 0x0600B7D5 RID: 47061 RVA: 0x00446BF6 File Offset: 0x00444DF6
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPathBlocked(ref PathingUtils.Data data, int x, int y)
		{
			return (ulong)x >= (ulong)((long)data.pathingGridSize) || (ulong)y >= (ulong)((long)data.pathingGridSize) || data.pathingGrid[x + y * data.pathingGridSize] == sbyte.MinValue;
		}

		// Token: 0x0600B7D6 RID: 47062 RVA: 0x00446C2C File Offset: 0x00444E2C
		public bool IsPointOnHighwayWorld(int x, int y)
		{
			int index = x / 10 + y / 10 * PathingUtils.worldBuilder.data.PathTileGridWidth;
			return PathingUtils.worldBuilder.data.PathTileGrid[index].TileState == PathTile.PathTileStates.Highway;
		}

		// Token: 0x0600B7D7 RID: 47063 RVA: 0x00446C70 File Offset: 0x00444E70
		public void SetupPathingGrid()
		{
			this.data.pathingGridSize = PathingUtils.worldBuilder.WorldSize / 10;
			this.data.pathingGrid = new NativeArray<sbyte>(this.data.pathingGridSize * this.data.pathingGridSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x0600B7D8 RID: 47064 RVA: 0x00446CC0 File Offset: 0x00444EC0
		public void Cleanup()
		{
			this.data.pathingGrid.Dispose();
			this.data.pathingGridSize = 0;
			this.data.nodePool.Cleanup();
			this.data.minHeapBinned.Cleanup();
			this.data.closedList.Dispose();
			this.pathTemp.Dispose();
		}

		// Token: 0x0600B7DA RID: 47066 RVA: 0x00446DB8 File Offset: 0x00444FB8
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindDetailedPath$BurstManaged(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in Vector2i endPos, bool _isCountryRoad, bool _isRiver)
		{
			if (!PathingUtils.InBounds(ref wd, startPos) || !PathingUtils.InBounds(ref wd, endPos))
			{
				return -1;
			}
			PathingUtils.ClosedListInit(ref data);
			data.closedList[startPos.x + startPos.y * data.closedListWidth] = 1;
			data.closedListMinY = startPos.y;
			data.closedListMaxY = startPos.y;
			data.minHeapBinned.Init();
			int num = data.nodePool.Alloc();
			data.nodePool.Node(num).Set(startPos, 0f, 0f, -1);
			data.minHeapBinned.Add(ref data, num);
			Vector2i vector2i = new Vector2i(Utils.FastMin(startPos.x, endPos.x), Utils.FastMin(startPos.y, endPos.y));
			Vector2i vector2i2 = new Vector2i(Utils.FastMax(startPos.x, endPos.x), Utils.FastMax(startPos.y, endPos.y));
			int num2 = Utils.FastMax(0, vector2i.x - 200);
			int num3 = Utils.FastMax(0, vector2i.y - 200);
			int num4 = Utils.FastMin(vector2i2.x + 200, data.closedListWidth - 1);
			int num5 = Utils.FastMin(vector2i2.y + 200, data.closedListWidth - 1);
			float num6 = _isCountryRoad ? 12f : 11f;
			int num7 = 20000;
			int num8;
			while ((num8 = data.minHeapBinned.ExtractFirst(ref data)) >= 0 && --num7 >= 0)
			{
				ref PathNode ptr = ref data.nodePool.Node(num8);
				Vector2i position = ptr.position;
				if (position == endPos)
				{
					return num8;
				}
				for (int i = 0; i < 8; i++)
				{
					Vector2i vector2i3 = PathingUtils.neighborOffset8Ways[i];
					Vector2i vector2i4 = ptr.position + vector2i3;
					if (vector2i4.x >= num2 && vector2i4.y >= num3 && vector2i4.x < num4 && vector2i4.y < num5)
					{
						int index = vector2i4.x + vector2i4.y * data.closedListWidth;
						if (data.closedList[index] == 0)
						{
							bool flag = vector2i4 == endPos;
							if (!flag)
							{
								bool flag2 = PathingUtils.IsPathBlocked(ref data, vector2i4.x, vector2i4.y);
								if (!flag2)
								{
									flag2 = PathingUtils.IsBlocked(ref wd, vector2i4.x, vector2i4.y, _isRiver);
								}
								if (flag2)
								{
									data.closedList[index] = 1;
									data.closedListMinY = Utils.FastMin(data.closedListMinY, vector2i4.y);
									data.closedListMaxY = Utils.FastMax(data.closedListMaxY, vector2i4.y);
									goto IL_54C;
								}
							}
							float num9 = Utils.FastAbs(PathingUtils.GetHeight(ref wd, position) - PathingUtils.GetHeight(ref wd, vector2i4));
							if (num9 <= num6)
							{
								float num10 = PathingUtils.DiagonalDist(vector2i4, endPos);
								num10 *= 1.4f;
								bool flag3 = vector2i3.x != 0 && vector2i3.y != 0;
								bool flag4 = true;
								if (!_isCountryRoad)
								{
									float num11 = Vector2i.DistanceSqr(vector2i4, startPos);
									float num12 = Vector2i.DistanceSqr(vector2i4, endPos);
									if (num11 <= 8.410001f || num12 <= 8.410001f)
									{
										flag4 = false;
									}
									Vector2i vector2i5 = vector2i4 * 10;
									if (wd.GetStreetTileDataWorld(vector2i5.x, vector2i5.y).ConnectedHighwayCount >= 3)
									{
										goto IL_54C;
									}
									if (!flag)
									{
										bool flag5 = wd.PathTileGrid[vector2i4.x + vector2i4.y * wd.PathTileGridWidth].TileState == PathTile.PathTileStates.Highway;
										if (flag5)
										{
											goto IL_54C;
										}
										if (flag3)
										{
											for (int j = 0; j < 2; j++)
											{
												int num13 = ptr.position.x;
												int num14 = ptr.position.y;
												if (j == 0)
												{
													num13 += vector2i3.x;
												}
												else
												{
													num14 += vector2i3.y;
												}
												flag5 = PathingUtils.IsPathBlocked(ref data, num13, num14);
												if (flag5)
												{
													break;
												}
												flag5 = PathingUtils.IsBlocked(ref wd, num13, num14, false);
												if (flag5)
												{
													break;
												}
												if (wd.PathTileGrid[num13 + num14 * wd.PathTileGridWidth].TileState == PathTile.PathTileStates.Highway)
												{
													flag5 = true;
													break;
												}
											}
											if (flag5)
											{
												goto IL_54C;
											}
										}
									}
									num9 = Utils.FastMax(0f, num9 - 0.5f);
									num9 *= 3f;
								}
								num9 *= 0.2f;
								float num15 = 1f;
								if (flag3)
								{
									num15 += 0.414214f;
								}
								if (flag4 && data.pathingGrid.IsCreated)
								{
									int num16 = (int)data.pathingGrid[vector2i4.x + vector2i4.y * data.pathingGridSize];
									if (num16 > 0)
									{
										num15 *= (float)num16;
									}
								}
								data.closedList[index] = 1;
								data.closedListMinY = Utils.FastMin(data.closedListMinY, vector2i4.y);
								data.closedListMaxY = Utils.FastMax(data.closedListMaxY, vector2i4.y);
								int num17 = data.nodePool.Alloc();
								ref PathNode ptr2 = ref data.nodePool.Node(num17);
								float num18 = ptr.travelledCost + num15 + num9;
								float totalCost = num18 + num10;
								ptr2.Set(vector2i4, num18, totalCost, num8);
								data.minHeapBinned.Add(ref data, num17);
							}
						}
					}
					IL_54C:;
				}
			}
			return -1;
		}

		// Token: 0x0600B7DB RID: 47067 RVA: 0x00447338 File Offset: 0x00445538
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindDetailedPath$BurstManaged(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in NativeList<Vector2i> _endPath, bool _isCountryRoad, bool _isRiver)
		{
			PathingUtils.ClosedListInit(ref data);
			data.closedList[startPos.x + startPos.y * data.closedListWidth] = 1;
			data.closedListMinY = startPos.y;
			data.closedListMaxY = startPos.y;
			data.minHeapBinned.Init();
			int num = data.nodePool.Alloc();
			data.nodePool.Node(num).Set(startPos, 0f, 0f, -1);
			data.minHeapBinned.Add(ref data, num);
			Vector2i vector2i;
			Vector2i vector2i2;
			PathingUtils.CalcPathBounds(_endPath, out vector2i, out vector2i2);
			int num2 = Utils.FastMax(0, vector2i.x - 200);
			int num3 = Utils.FastMax(0, vector2i.y - 200);
			int num4 = Utils.FastMin(vector2i2.x + 200, data.closedListWidth - 1);
			int num5 = Utils.FastMin(vector2i2.y + 200, data.closedListWidth - 1);
			float num6 = _isCountryRoad ? 12f : 11f;
			int num7 = 20000;
			int num8;
			while ((num8 = data.minHeapBinned.ExtractFirst(ref data)) >= 0 && --num7 >= 0)
			{
				ref PathNode ptr = ref data.nodePool.Node(num8);
				Vector2i position = ptr.position;
				if (PathingUtils.IsPointOnPath(_endPath, position))
				{
					return num8;
				}
				for (int i = 0; i < 8; i++)
				{
					Vector2i vector2i3 = PathingUtils.neighborOffset8Ways[i];
					Vector2i vector2i4 = ptr.position + vector2i3;
					if (vector2i4.x >= num2 && vector2i4.y >= num3 && vector2i4.x < num4 && vector2i4.y < num5)
					{
						int index = vector2i4.x + vector2i4.y * data.closedListWidth;
						if (data.closedList[index] == 0)
						{
							Vector2i vector2i5;
							PathingUtils.FindClosestPathPoint(_endPath, vector2i4, out vector2i5);
							if (vector2i4 != vector2i5)
							{
								bool flag = PathingUtils.IsPathBlocked(ref data, vector2i4.x, vector2i4.y);
								if (!flag)
								{
									flag = PathingUtils.IsBlocked(ref wd, vector2i4.x, vector2i4.y, _isRiver);
								}
								if (flag)
								{
									data.closedList[index] = 1;
									data.closedListMinY = Utils.FastMin(data.closedListMinY, vector2i4.y);
									data.closedListMaxY = Utils.FastMax(data.closedListMaxY, vector2i4.y);
									goto IL_37A;
								}
							}
							float num9 = Utils.FastAbs(PathingUtils.GetHeight(ref wd, position) - PathingUtils.GetHeight(ref wd, vector2i4));
							if (num9 <= num6)
							{
								float num10 = Vector2i.Distance(vector2i4, vector2i5);
								num10 *= 1.4f;
								num9 *= 0.2f;
								num9 += 1f;
								if (vector2i3.x != 0 && vector2i3.y != 0)
								{
									num9 += 0.414214f;
								}
								if (data.pathingGrid.IsCreated)
								{
									int num11 = (int)data.pathingGrid[vector2i4.x + vector2i4.y * data.pathingGridSize];
									if (num11 > 0)
									{
										num9 *= (float)num11;
									}
								}
								data.closedList[index] = 1;
								data.closedListMinY = Utils.FastMin(data.closedListMinY, vector2i4.y);
								data.closedListMaxY = Utils.FastMax(data.closedListMaxY, vector2i4.y);
								int num12 = data.nodePool.Alloc();
								ref PathNode ptr2 = ref data.nodePool.Node(num12);
								float num13 = ptr.travelledCost + num9;
								float totalCost = num13 + num10;
								ptr2.Set(vector2i4, num13, totalCost, num8);
								data.minHeapBinned.Add(ref data, num12);
							}
						}
					}
					IL_37A:;
				}
			}
			return -1;
		}

		// Token: 0x0600B7DC RID: 47068 RVA: 0x004476E4 File Offset: 0x004458E4
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CalcPathBounds$BurstManaged(in NativeList<Vector2i> _path, out Vector2i _min, out Vector2i _max)
		{
			_min = Vector2i.max;
			_max = Vector2i.min;
			NativeList<Vector2i> nativeList = _path;
			foreach (Vector2i vector2i in nativeList)
			{
				_min.x = Utils.FastMin(_min.x, vector2i.x);
				_min.y = Utils.FastMin(_min.y, vector2i.y);
				_max.x = Utils.FastMax(_max.x, vector2i.x);
				_max.y = Utils.FastMax(_max.y, vector2i.y);
			}
		}

		// Token: 0x0600B7DD RID: 47069 RVA: 0x004477A8 File Offset: 0x004459A8
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float FindClosestPathPoint$BurstManaged(in NativeList<Vector2> _path, in Vector2 _startPos, out Vector2 _destPoint, int _step = 1)
		{
			_destPoint = Vector2.zero;
			float num = float.MaxValue;
			int length = _path.Length;
			for (int i = 0; i < length; i += _step)
			{
				NativeList<Vector2> nativeList = _path;
				Vector2 vector = nativeList[i];
				float sqrMagnitude = (_startPos - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					_destPoint = vector;
				}
			}
			return num;
		}

		// Token: 0x0600B7DE RID: 47070 RVA: 0x00447814 File Offset: 0x00445A14
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float FindClosestPathPoint$BurstManaged(in NativeList<Vector2i> _path, in Vector2i _startPos, out Vector2i _destPoint)
		{
			_destPoint = Vector2i.zero;
			float num = float.MaxValue;
			NativeList<Vector2i> nativeList = _path;
			foreach (Vector2i vector2i in nativeList)
			{
				float num2 = Vector2i.DistanceSqr(_startPos, vector2i);
				if (num2 < num)
				{
					num = num2;
					_destPoint = vector2i;
				}
			}
			return num;
		}

		// Token: 0x0600B7DF RID: 47071 RVA: 0x00447894 File Offset: 0x00445A94
		[BurstCompile(CompileSynchronously = true)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPointOnPath$BurstManaged(in NativeList<Vector2i> _path, in Vector2i _point)
		{
			NativeList<Vector2i> nativeList = _path;
			foreach (Vector2i vector2i in nativeList)
			{
				if (vector2i.x == _point.x && vector2i.y == _point.y)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040089AC RID: 35244
		public const int PATHING_GRID_TILE_SIZE = 10;

		// Token: 0x040089AD RID: 35245
		public const int stepSize = 10;

		// Token: 0x040089AE RID: 35246
		public static readonly Vector2i stepHalf = new Vector2i(5, 5);

		// Token: 0x040089AF RID: 35247
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cRoadCountryMaxStepH = 12f;

		// Token: 0x040089B0 RID: 35248
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cRoadHighwayMaxStepH = 11f;

		// Token: 0x040089B1 RID: 35249
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightCostScale = 0.2f;

		// Token: 0x040089B2 RID: 35250
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cNeighborsCount = 8;

		// Token: 0x040089B3 RID: 35251
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Vector2i[] neighborOffset8Ways = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 1),
			new Vector2i(1, 0),
			new Vector2i(1, -1),
			new Vector2i(0, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 0),
			new Vector2i(-1, 1)
		};

		// Token: 0x040089B4 RID: 35252
		[PublicizedFrom(EAccessModifier.Private)]
		public static WorldBuilder worldBuilder;

		// Token: 0x040089B5 RID: 35253
		[PublicizedFrom(EAccessModifier.Private)]
		public NativeList<Vector2i> pathTemp = new NativeList<Vector2i>(200, Allocator.Persistent);

		// Token: 0x040089B6 RID: 35254
		[PublicizedFrom(EAccessModifier.Private)]
		public PathingUtils.Data data;

		// Token: 0x02001709 RID: 5897
		public struct Data
		{
			// Token: 0x040089B7 RID: 35255
			public int WorldSize;

			// Token: 0x040089B8 RID: 35256
			public NativeArray<sbyte> pathingGrid;

			// Token: 0x040089B9 RID: 35257
			public int pathingGridSize;

			// Token: 0x040089BA RID: 35258
			public PathNodePool nodePool;

			// Token: 0x040089BB RID: 35259
			public PathingUtils.MinHeapBinned minHeapBinned;

			// Token: 0x040089BC RID: 35260
			public NativeArray<byte> closedList;

			// Token: 0x040089BD RID: 35261
			public int closedListWidth;

			// Token: 0x040089BE RID: 35262
			public int closedListMinY;

			// Token: 0x040089BF RID: 35263
			public int closedListMaxY;
		}

		// Token: 0x0200170A RID: 5898
		public struct MinHeapBinned
		{
			// Token: 0x0600B7E0 RID: 47072 RVA: 0x00447908 File Offset: 0x00445B08
			public void Init()
			{
				if (!this.nodeBins.IsCreated)
				{
					this.nodeBins = new NativeArray<int>(32768, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					this.lowBin = 0;
					this.highBin = 32767;
				}
				this.Reset();
			}

			// Token: 0x0600B7E1 RID: 47073 RVA: 0x00447944 File Offset: 0x00445B44
			public unsafe void Reset()
			{
				if (this.lowBin <= this.highBin)
				{
					int num = UnsafeUtility.SizeOf<int>();
					void* destination = (void*)((byte*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(this.nodeBins) + this.lowBin * num);
					UnsafeUtility.MemSet(destination, byte.MaxValue, (long)((this.highBin - this.lowBin + 1) * num));
				}
				this.lowBin = 32768;
				this.highBin = 0;
			}

			// Token: 0x0600B7E2 RID: 47074 RVA: 0x004479AC File Offset: 0x00445BAC
			public int ExtractFirst(ref PathingUtils.Data data)
			{
				if (this.lowBin <= this.highBin)
				{
					int num = this.nodeBins[this.lowBin];
					PathNode pathNode;
					data.nodePool.Node(num, out pathNode);
					this.nodeBins[this.lowBin] = pathNode.listNext;
					if (pathNode.listNext < 0)
					{
						int num2;
						do
						{
							num2 = this.lowBin + 1;
							this.lowBin = num2;
						}
						while (num2 <= this.highBin && this.nodeBins[this.lowBin] < 0);
						if (this.lowBin > this.highBin)
						{
							this.lowBin = 32768;
							this.highBin = 0;
						}
					}
					return num;
				}
				return -1;
			}

			// Token: 0x0600B7E3 RID: 47075 RVA: 0x00447A5C File Offset: 0x00445C5C
			public void Add(ref PathingUtils.Data data, int _nodeIndex)
			{
				ref PathNode ptr = ref data.nodePool.Node(_nodeIndex);
				int num = (int)(ptr.totalCost * 0.07f);
				if (num >= 32768)
				{
					num = 32767;
				}
				if (num < this.lowBin)
				{
					this.lowBin = num;
				}
				if (num > this.highBin)
				{
					this.highBin = num;
				}
				int num2 = this.nodeBins[num];
				if (num2 < 0)
				{
					ptr.listNext = -1;
					this.nodeBins[num] = _nodeIndex;
					return;
				}
				ref PathNode ptr2 = ref data.nodePool.Node(num2);
				if (ptr.totalCost <= ptr2.totalCost)
				{
					ptr.listNext = num2;
					this.nodeBins[num] = _nodeIndex;
					return;
				}
				ref PathNode ptr3 = ref ptr2;
				int i;
				for (i = ptr3.listNext; i >= 0; i = ptr3.listNext)
				{
					ref PathNode ptr4 = ref data.nodePool.Node(i);
					if (ptr.totalCost <= ptr4.totalCost)
					{
						break;
					}
					ptr3 = ref ptr4;
				}
				ptr.listNext = i;
				ptr3.listNext = _nodeIndex;
			}

			// Token: 0x0600B7E4 RID: 47076 RVA: 0x00447B56 File Offset: 0x00445D56
			public void Cleanup()
			{
				this.nodeBins.Dispose();
			}

			// Token: 0x040089C0 RID: 35264
			[PublicizedFrom(EAccessModifier.Private)]
			public const int cBins = 32768;

			// Token: 0x040089C1 RID: 35265
			[PublicizedFrom(EAccessModifier.Private)]
			public const float cScale = 0.07f;

			// Token: 0x040089C2 RID: 35266
			[PublicizedFrom(EAccessModifier.Private)]
			public NativeArray<int> nodeBins;

			// Token: 0x040089C3 RID: 35267
			[PublicizedFrom(EAccessModifier.Private)]
			public int lowBin;

			// Token: 0x040089C4 RID: 35268
			[PublicizedFrom(EAccessModifier.Private)]
			public int highBin;
		}

		// Token: 0x0200170B RID: 5899
		[PublicizedFrom(EAccessModifier.Private)]
		public enum PathNodeType
		{
			// Token: 0x040089C6 RID: 35270
			Free,
			// Token: 0x040089C7 RID: 35271
			Road,
			// Token: 0x040089C8 RID: 35272
			Prefab,
			// Token: 0x040089C9 RID: 35273
			CityLimits = 4,
			// Token: 0x040089CA RID: 35274
			Blocked = 8
		}

		// Token: 0x0200170C RID: 5900
		// (Invoke) Token: 0x0600B7E6 RID: 47078
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate int FindDetailedPath_0000B7B3$PostfixBurstDelegate(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in Vector2i endPos, bool _isCountryRoad, bool _isRiver);

		// Token: 0x0200170D RID: 5901
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class FindDetailedPath_0000B7B3$BurstDirectCall
		{
			// Token: 0x0600B7E9 RID: 47081 RVA: 0x00447B63 File Offset: 0x00445D63
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.FindDetailedPath$BurstManaged(WorldBuilder.Data*, PathingUtils.Data*, Vector2i*, Vector2i*, bool, bool)).MethodHandle, typeof(PathingUtils.FindDetailedPath_0000B7B3$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B7EA RID: 47082 RVA: 0x00447B90 File Offset: 0x00445D90
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B7EB RID: 47083 RVA: 0x00447BA8 File Offset: 0x00445DA8
			public unsafe static void Constructor()
			{
				PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.FindDetailedPath(WorldBuilder.Data*, PathingUtils.Data*, Vector2i*, Vector2i*, bool, bool)).MethodHandle);
			}

			// Token: 0x0600B7EC RID: 47084 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B7ED RID: 47085 RVA: 0x00447BB9 File Offset: 0x00445DB9
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static FindDetailedPath_0000B7B3$BurstDirectCall()
			{
				PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B7EE RID: 47086 RVA: 0x00447BC0 File Offset: 0x00445DC0
			public static int Invoke(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in Vector2i endPos, bool _isCountryRoad, bool _isRiver)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(WorldGenerationEngineFinal.WorldBuilder/Data&,WorldGenerationEngineFinal.PathingUtils/Data&,Vector2i&,Vector2i&,System.Boolean,System.Boolean), ref wd, ref data, ref startPos, ref endPos, _isCountryRoad, _isRiver, functionPointer);
					}
				}
				return PathingUtils.FindDetailedPath$BurstManaged(ref wd, ref data, startPos, endPos, _isCountryRoad, _isRiver);
			}

			// Token: 0x040089CB RID: 35275
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089CC RID: 35276
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x0200170E RID: 5902
		// (Invoke) Token: 0x0600B7F0 RID: 47088
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate int FindDetailedPath_0000B7B5$PostfixBurstDelegate(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in NativeList<Vector2i> _endPath, bool _isCountryRoad, bool _isRiver);

		// Token: 0x0200170F RID: 5903
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class FindDetailedPath_0000B7B5$BurstDirectCall
		{
			// Token: 0x0600B7F3 RID: 47091 RVA: 0x00447BFF File Offset: 0x00445DFF
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.FindDetailedPath$BurstManaged(WorldBuilder.Data*, PathingUtils.Data*, Vector2i*, NativeList<Vector2i>*, bool, bool)).MethodHandle, typeof(PathingUtils.FindDetailedPath_0000B7B5$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B7F4 RID: 47092 RVA: 0x00447C2C File Offset: 0x00445E2C
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B7F5 RID: 47093 RVA: 0x00447C44 File Offset: 0x00445E44
			public unsafe static void Constructor()
			{
				PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.FindDetailedPath(WorldBuilder.Data*, PathingUtils.Data*, Vector2i*, NativeList<Vector2i>*, bool, bool)).MethodHandle);
			}

			// Token: 0x0600B7F6 RID: 47094 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B7F7 RID: 47095 RVA: 0x00447C55 File Offset: 0x00445E55
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static FindDetailedPath_0000B7B5$BurstDirectCall()
			{
				PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B7F8 RID: 47096 RVA: 0x00447C5C File Offset: 0x00445E5C
			public static int Invoke(ref WorldBuilder.Data wd, ref PathingUtils.Data data, in Vector2i startPos, in NativeList<Vector2i> _endPath, bool _isCountryRoad, bool _isRiver)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(WorldGenerationEngineFinal.WorldBuilder/Data&,WorldGenerationEngineFinal.PathingUtils/Data&,Vector2i&,Unity.Collections.NativeList`1<Vector2i>&,System.Boolean,System.Boolean), ref wd, ref data, ref startPos, ref _endPath, _isCountryRoad, _isRiver, functionPointer);
					}
				}
				return PathingUtils.FindDetailedPath$BurstManaged(ref wd, ref data, startPos, _endPath, _isCountryRoad, _isRiver);
			}

			// Token: 0x040089CD RID: 35277
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089CE RID: 35278
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x02001710 RID: 5904
		// (Invoke) Token: 0x0600B7FA RID: 47098
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate void CalcPathBounds_0000B7B6$PostfixBurstDelegate(in NativeList<Vector2i> _path, out Vector2i _min, out Vector2i _max);

		// Token: 0x02001711 RID: 5905
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class CalcPathBounds_0000B7B6$BurstDirectCall
		{
			// Token: 0x0600B7FD RID: 47101 RVA: 0x00447C9B File Offset: 0x00445E9B
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.CalcPathBounds$BurstManaged(NativeList<Vector2i>*, Vector2i*, Vector2i*)).MethodHandle, typeof(PathingUtils.CalcPathBounds_0000B7B6$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B7FE RID: 47102 RVA: 0x00447CC8 File Offset: 0x00445EC8
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B7FF RID: 47103 RVA: 0x00447CE0 File Offset: 0x00445EE0
			public unsafe static void Constructor()
			{
				PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.CalcPathBounds(NativeList<Vector2i>*, Vector2i*, Vector2i*)).MethodHandle);
			}

			// Token: 0x0600B800 RID: 47104 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B801 RID: 47105 RVA: 0x00447CF1 File Offset: 0x00445EF1
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static CalcPathBounds_0000B7B6$BurstDirectCall()
			{
				PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B802 RID: 47106 RVA: 0x00447CF8 File Offset: 0x00445EF8
			public static void Invoke(in NativeList<Vector2i> _path, out Vector2i _min, out Vector2i _max)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeList`1<Vector2i>&,Vector2i&,Vector2i&), ref _path, ref _min, ref _max, functionPointer);
						return;
					}
				}
				PathingUtils.CalcPathBounds$BurstManaged(_path, out _min, out _max);
			}

			// Token: 0x040089CF RID: 35279
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089D0 RID: 35280
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x02001712 RID: 5906
		// (Invoke) Token: 0x0600B804 RID: 47108
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate float FindClosestPathPoint_0000B7B7$PostfixBurstDelegate(in NativeList<Vector2> _path, in Vector2 _startPos, out Vector2 _destPoint, int _step = 1);

		// Token: 0x02001713 RID: 5907
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class FindClosestPathPoint_0000B7B7$BurstDirectCall
		{
			// Token: 0x0600B807 RID: 47111 RVA: 0x00447D2D File Offset: 0x00445F2D
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.FindClosestPathPoint$BurstManaged(NativeList<Vector2>*, Vector2*, Vector2*, int)).MethodHandle, typeof(PathingUtils.FindClosestPathPoint_0000B7B7$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B808 RID: 47112 RVA: 0x00447D5C File Offset: 0x00445F5C
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B809 RID: 47113 RVA: 0x00447D74 File Offset: 0x00445F74
			public unsafe static void Constructor()
			{
				PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.FindClosestPathPoint(NativeList<Vector2>*, Vector2*, Vector2*, int)).MethodHandle);
			}

			// Token: 0x0600B80A RID: 47114 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B80B RID: 47115 RVA: 0x00447D85 File Offset: 0x00445F85
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static FindClosestPathPoint_0000B7B7$BurstDirectCall()
			{
				PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B80C RID: 47116 RVA: 0x00447D8C File Offset: 0x00445F8C
			public static float Invoke(in NativeList<Vector2> _path, in Vector2 _startPos, out Vector2 _destPoint, int _step = 1)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(Unity.Collections.NativeList`1<UnityEngine.Vector2>&,UnityEngine.Vector2&,UnityEngine.Vector2&,System.Int32), ref _path, ref _startPos, ref _destPoint, _step, functionPointer);
					}
				}
				return PathingUtils.FindClosestPathPoint$BurstManaged(_path, _startPos, out _destPoint, _step);
			}

			// Token: 0x040089D1 RID: 35281
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089D2 RID: 35282
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x02001714 RID: 5908
		// (Invoke) Token: 0x0600B80E RID: 47118
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate float FindClosestPathPoint_0000B7B8$PostfixBurstDelegate(in NativeList<Vector2i> _path, in Vector2i _startPos, out Vector2i _destPoint);

		// Token: 0x02001715 RID: 5909
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class FindClosestPathPoint_0000B7B8$BurstDirectCall
		{
			// Token: 0x0600B811 RID: 47121 RVA: 0x00447DC3 File Offset: 0x00445FC3
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.FindClosestPathPoint$BurstManaged(NativeList<Vector2i>*, Vector2i*, Vector2i*)).MethodHandle, typeof(PathingUtils.FindClosestPathPoint_0000B7B8$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B812 RID: 47122 RVA: 0x00447DF0 File Offset: 0x00445FF0
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B813 RID: 47123 RVA: 0x00447E08 File Offset: 0x00446008
			public unsafe static void Constructor()
			{
				PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.FindClosestPathPoint(NativeList<Vector2i>*, Vector2i*, Vector2i*)).MethodHandle);
			}

			// Token: 0x0600B814 RID: 47124 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B815 RID: 47125 RVA: 0x00447E19 File Offset: 0x00446019
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static FindClosestPathPoint_0000B7B8$BurstDirectCall()
			{
				PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B816 RID: 47126 RVA: 0x00447E20 File Offset: 0x00446020
			public static float Invoke(in NativeList<Vector2i> _path, in Vector2i _startPos, out Vector2i _destPoint)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Single(Unity.Collections.NativeList`1<Vector2i>&,Vector2i&,Vector2i&), ref _path, ref _startPos, ref _destPoint, functionPointer);
					}
				}
				return PathingUtils.FindClosestPathPoint$BurstManaged(_path, _startPos, out _destPoint);
			}

			// Token: 0x040089D3 RID: 35283
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089D4 RID: 35284
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}

		// Token: 0x02001716 RID: 5910
		// (Invoke) Token: 0x0600B818 RID: 47128
		[PublicizedFrom(EAccessModifier.Internal)]
		public delegate bool IsPointOnPath_0000B7B9$PostfixBurstDelegate(in NativeList<Vector2i> _path, in Vector2i _point);

		// Token: 0x02001717 RID: 5911
		[PublicizedFrom(EAccessModifier.Internal)]
		public static class IsPointOnPath_0000B7B9$BurstDirectCall
		{
			// Token: 0x0600B81B RID: 47131 RVA: 0x00447E55 File Offset: 0x00446055
			[BurstDiscard]
			[PublicizedFrom(EAccessModifier.Private)]
			public unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Pointer == 0)
				{
					PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.DeferredCompilation, methodof(PathingUtils.IsPointOnPath$BurstManaged(NativeList<Vector2i>*, Vector2i*)).MethodHandle, typeof(PathingUtils.IsPointOnPath_0000B7B9$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Pointer;
			}

			// Token: 0x0600B81C RID: 47132 RVA: 0x00447E84 File Offset: 0x00446084
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			// Token: 0x0600B81D RID: 47133 RVA: 0x00447E9C File Offset: 0x0044609C
			public unsafe static void Constructor()
			{
				PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(PathingUtils.IsPointOnPath(NativeList<Vector2i>*, Vector2i*)).MethodHandle);
			}

			// Token: 0x0600B81E RID: 47134 RVA: 0x000027FC File Offset: 0x000009FC
			public static void Initialize()
			{
			}

			// Token: 0x0600B81F RID: 47135 RVA: 0x00447EAD File Offset: 0x004460AD
			// Note: this type is marked as 'beforefieldinit'.
			[PublicizedFrom(EAccessModifier.Private)]
			static IsPointOnPath_0000B7B9$BurstDirectCall()
			{
				PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Constructor();
			}

			// Token: 0x0600B820 RID: 47136 RVA: 0x00447EB4 File Offset: 0x004460B4
			public static bool Invoke(in NativeList<Vector2i> _path, in Vector2i _point)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Boolean(Unity.Collections.NativeList`1<Vector2i>&,Vector2i&), ref _path, ref _point, functionPointer);
					}
				}
				return PathingUtils.IsPointOnPath$BurstManaged(_path, _point);
			}

			// Token: 0x040089D5 RID: 35285
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr Pointer;

			// Token: 0x040089D6 RID: 35286
			[PublicizedFrom(EAccessModifier.Private)]
			public static IntPtr DeferredCompilation;
		}
	}
}
