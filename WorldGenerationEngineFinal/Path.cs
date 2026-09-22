using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001707 RID: 5895
	public class Path
	{
		// Token: 0x0600B7A0 RID: 47008 RVA: 0x00445104 File Offset: 0x00443304
		public Path(WorldBuilder _worldBuilder, Vector2i _startPosition, Vector2i _endPosition, int _lanes, bool _isCountryRoad)
		{
			this.worldBuilder = _worldBuilder;
			this.StartPosition = _startPosition;
			this.EndPosition = _endPosition;
			this.lanes = _lanes;
			this.radius = (float)this.lanes * 0.5f * 4.5f;
			this.isCountryRoad = _isCountryRoad;
			this.CreatePath();
		}

		// Token: 0x0600B7A1 RID: 47009 RVA: 0x00445178 File Offset: 0x00443378
		public Path(WorldBuilder _worldBuilder, Vector2i _startPosition, Vector2i _endPosition, float _radius, bool _isCountryRoad)
		{
			this.worldBuilder = _worldBuilder;
			this.StartPosition = _startPosition;
			this.EndPosition = _endPosition;
			this.radius = _radius;
			this.lanes = Mathf.CeilToInt(this.radius / 4.5f * 2f);
			this.isCountryRoad = _isCountryRoad;
			this.CreatePath();
		}

		// Token: 0x0600B7A2 RID: 47010 RVA: 0x004451F0 File Offset: 0x004433F0
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~Path()
		{
			this.Cleanup();
		}

		// Token: 0x0600B7A3 RID: 47011 RVA: 0x0044521C File Offset: 0x0044341C
		public void Cleanup()
		{
			if (this.FinalPathPoints.IsCreated)
			{
				this.FinalPathPoints.Dispose();
			}
			if (this.pathPoints3d.IsCreated)
			{
				this.pathPoints3d.Dispose();
			}
		}

		// Token: 0x0600B7A4 RID: 47012 RVA: 0x00445250 File Offset: 0x00443450
		public void RemoveFromStreetTiles()
		{
			if (this.isCountryRoad)
			{
				return;
			}
			if (!this.FinalPathPoints.IsCreated)
			{
				return;
			}
			StreetTile streetTile = null;
			for (int i = 0; i < this.FinalPathPoints.Length; i++)
			{
				Vector2i pos;
				pos.x = (int)this.FinalPathPoints[i].x;
				pos.y = (int)this.FinalPathPoints[i].y;
				StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(pos);
				if (streetTileWorld != streetTile && streetTileWorld != null)
				{
					streetTileWorld.RemoveHighway(this);
				}
				streetTile = streetTileWorld;
			}
		}

		// Token: 0x0600B7A5 RID: 47013 RVA: 0x004452DC File Offset: 0x004434DC
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreatePath()
		{
			if (this.StartPosition.x < 8 || this.StartPosition.y < 8 || this.EndPosition.x < 8 || this.EndPosition.y < 8)
			{
				Log.Error("CreatePath position oob {0} to {1}", new object[]
				{
					this.StartPosition,
					this.EndPosition
				});
				return;
			}
			NativeList<Vector2i> path = this.worldBuilder.PathingUtils.GetPath(this.StartPosition, this.EndPosition, this.isCountryRoad, this.isRiver);
			if (path.Length <= 1)
			{
				return;
			}
			this.IsValid = true;
			this.FinalPathPoints = new NativeList<Vector2>(16, Allocator.Persistent);
			Vector2 vector = new Vector2((float)this.EndPosition.x, (float)this.EndPosition.y);
			this.FinalPathPoints.Add(vector);
			for (int i = 1; i < path.Length; i++)
			{
				if (Path.DistanceSqr(path[i], this.StartPosition) >= 16f && Path.DistanceSqr(path[i], this.EndPosition) >= 16f)
				{
					vector.x = (float)path[i].x;
					vector.y = (float)path[i].y;
					this.FinalPathPoints.Add(vector);
				}
			}
			Vector2 vector2 = new Vector2((float)this.StartPosition.x, (float)this.StartPosition.y);
			this.FinalPathPoints.Add(vector2);
			this.ProcessPath();
		}

		// Token: 0x0600B7A6 RID: 47014 RVA: 0x00445478 File Offset: 0x00443678
		[PublicizedFrom(EAccessModifier.Private)]
		public void ProcessPath()
		{
			float weight = this.isCountryRoad ? 0.3f : 0.4f;
			int skipIndex = this.isCountryRoad ? 0 : 2;
			for (int i = 0; i < 4; i++)
			{
				this.RoundOffCorners(weight, skipIndex);
			}
			float num = (float)this.worldBuilder.WorldSize;
			float num2 = 0f;
			for (int j = 0; j < this.FinalPathPoints.Length; j++)
			{
				float x = this.FinalPathPoints[j].x;
				if (x < 0f || x >= num)
				{
					this.IsValid = false;
					return;
				}
				float y = this.FinalPathPoints[j].y;
				if (y < 0f || y >= num)
				{
					this.IsValid = false;
					return;
				}
				if (j > 0)
				{
					num2 += Vector2.Distance(this.FinalPathPoints[j - 1], this.FinalPathPoints[j]);
				}
				Vector3 vector = new Vector3(x, this.worldBuilder.GetHeight((int)x, (int)y), y);
				this.pathPoints3d.Add(vector);
			}
			this.Cost = Mathf.RoundToInt(num2);
			float[] heights = new float[this.pathPoints3d.Length];
			if (this.isCountryRoad)
			{
				for (int k = 0; k < 4; k++)
				{
					this.SmoothHeights(heights);
				}
			}
			else
			{
				float num3 = 0f;
				for (int l = 0; l < this.pathPoints3d.Length; l++)
				{
					num3 += this.pathPoints3d[l].y;
				}
				num3 /= (float)this.pathPoints3d.Length;
				num3 += 8f;
				for (int m = 0; m < this.pathPoints3d.Length; m++)
				{
					Vector3 vector2 = this.pathPoints3d[m];
					if (vector2.y > num3)
					{
						int index = (int)vector2.x + (int)vector2.z * this.worldBuilder.WorldSize;
						if (this.worldBuilder.data.poiHeightMask[index] == 0)
						{
							vector2.y = num3 * 0.3f + vector2.y * 0.7f;
							this.pathPoints3d[m] = vector2;
						}
					}
				}
				for (int n = 0; n < 50; n++)
				{
					this.SmoothHeights(heights);
				}
			}
			this.FinalPathPoints.Clear();
			Vector2 zero = Vector2.zero;
			for (int num4 = 0; num4 < this.pathPoints3d.Length; num4++)
			{
				zero.x = (float)((int)(this.pathPoints3d[num4].x + 0.5f));
				zero.y = (float)((int)(this.pathPoints3d[num4].z + 0.5f));
				this.FinalPathPoints.Add(zero);
			}
		}

		// Token: 0x0600B7A7 RID: 47015 RVA: 0x00445760 File Offset: 0x00443960
		[PublicizedFrom(EAccessModifier.Private)]
		public void RoundOffCorners(float _weight, int _skipIndex)
		{
			float d = _weight * 0.5f;
			float d2 = 1f - _weight;
			_skipIndex++;
			int num = this.FinalPathPoints.Length - _skipIndex;
			for (int i = _skipIndex; i < num; i++)
			{
				this.FinalPathPoints[i] = this.FinalPathPoints[i] * d2 + (this.FinalPathPoints[i - 1] + this.FinalPathPoints[i + 1]) * d;
			}
		}

		// Token: 0x0600B7A8 RID: 47016 RVA: 0x004457E8 File Offset: 0x004439E8
		public void DrawPathToRoadIds(byte[] ids)
		{
			float num = this.radius;
			if (this.isCountryRoad)
			{
				num += 6f;
			}
			else
			{
				num += 10f;
			}
			object obj = (this.lanes >= 2) ? (this.radius - 1f) : this.radius;
			float num2 = this.radius * this.radius;
			float num3 = num * num;
			object obj2 = obj;
			float num4 = obj2 * obj2;
			for (int i = 0; i < this.FinalPathPoints.Length - 1; i++)
			{
				float x = this.FinalPathPoints[i].x;
				float y = this.FinalPathPoints[i].y;
				float x2 = this.FinalPathPoints[i + 1].x;
				float y2 = this.FinalPathPoints[i + 1].y;
				int num5 = (int)(Utils.FastMin(x, x2) - num - 1.5f);
				num5 = Utils.FastMax(0, num5);
				int num6 = (int)(Utils.FastMax(x, x2) + num + 1.5f);
				num6 = Utils.FastMin(num6, this.worldBuilder.WorldSize - 1);
				int num7 = (int)(Utils.FastMin(y, y2) - num - 1.5f);
				num7 = Utils.FastMax(0, num7);
				int num8 = (int)(Utils.FastMax(y, y2) + num + 1.5f);
				num8 = Utils.FastMin(num8, this.worldBuilder.WorldSize - 1);
				for (int j = num7; j < num8; j++)
				{
					Vector2 point;
					point.y = (float)j;
					int k = num5;
					while (k < num6)
					{
						point.x = (float)k;
						Vector2 b;
						float num9 = this.GetPointToLineDistanceSq(point, this.FinalPathPoints[i], this.FinalPathPoints[i + 1], out b);
						float num10;
						if (num9 < num3)
						{
							num10 = Utils.FastClamp01(Vector2.Distance(this.FinalPathPoints[i], b) / Vector2.Distance(this.FinalPathPoints[i], this.FinalPathPoints[i + 1]));
							goto IL_276;
						}
						num9 = this.distanceSqr((float)k, (float)j, this.FinalPathPoints[i]);
						if (num9 < num3)
						{
							float num11 = this.distanceSqr((float)k, (float)j, this.FinalPathPoints[i + 1]);
							if (num9 <= num11)
							{
								if (i > 0)
								{
									float num12 = this.distanceSqr((float)k, (float)j, this.FinalPathPoints[i - 1]);
									Vector2 vector;
									if (num9 >= num12 || this.GetPointToLineDistanceSq(point, this.FinalPathPoints[i - 1], this.FinalPathPoints[i], out vector) < num3)
									{
										goto IL_42A;
									}
								}
								num10 = -1f;
								goto IL_276;
							}
						}
						IL_42A:
						k++;
						continue;
						IL_276:
						int num13 = k + j * this.worldBuilder.WorldSize;
						if (this.isRiver)
						{
							if (num9 <= num2)
							{
								ids[num13] = 4;
								if (this.worldBuilder.GetHeight(k, j) < (float)this.worldBuilder.WaterHeight)
								{
									this.worldBuilder.SetWater(k, j, (byte)this.worldBuilder.WaterHeight);
									goto IL_42A;
								}
								this.worldBuilder.SetWater(k, j, (byte)this.worldBuilder.WaterHeight);
							}
						}
						else
						{
							int num14 = (int)ids[num13];
							if (num14 == 2 || (num9 > num2 && (num14 & 128) > 0))
							{
								goto IL_42A;
							}
							if (!this.isCountryRoad)
							{
								if (num9 > num2)
								{
									int num15 = num13;
									ids[num15] |= 128;
								}
								else if (num9 > num4)
								{
									ids[num13] = 3;
								}
								else
								{
									ids[num13] = 2;
								}
							}
							else if (num9 <= num2)
							{
								ids[num13] = 1;
							}
						}
						float height = this.worldBuilder.GetHeight(k, j);
						float v = 3f;
						if (!this.isRiver)
						{
							v = (float)(Utils.FastMax((int)this.worldBuilder.data.GetWater(k, j), this.worldBuilder.WaterHeight) + 1);
						}
						float num16 = this.pathPoints3d[i].y;
						if (num10 > 0f)
						{
							num16 = Utils.FastLerpUnclamped(num16, this.pathPoints3d[i + 1].y, num10);
						}
						num16 = Utils.FastMax(v, num16);
						float num17 = Utils.FastClamp01((Mathf.Sqrt(num9) - this.radius) / (num - this.radius));
						num17 *= num17;
						num16 = Utils.FastLerpUnclamped(num16, height, num17);
						this.worldBuilder.SetHeightTrusted(k, j, num16);
						goto IL_42A;
					}
				}
			}
		}

		// Token: 0x0600B7A9 RID: 47017 RVA: 0x00445C58 File Offset: 0x00443E58
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetPointToLineDistanceSq(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 pointOnLine)
		{
			Vector2 vector;
			vector.x = lineEnd.x - lineStart.x;
			vector.y = lineEnd.y - lineStart.y;
			float num = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
			vector.x /= num;
			vector.y /= num;
			float num2 = Vector2.Dot(point - lineStart, vector);
			if (num2 < 0f || num2 > num)
			{
				pointOnLine = new Vector2(100000f, 100000f);
				return float.MaxValue;
			}
			pointOnLine = lineStart + num2 * vector;
			return this.distanceSqr(point, pointOnLine);
		}

		// Token: 0x0600B7AA RID: 47018 RVA: 0x00445D24 File Offset: 0x00443F24
		public bool Crosses(Path path)
		{
			foreach (Vector2 v in this.FinalPathPoints)
			{
				foreach (Vector2 v2 in path.FinalPathPoints)
				{
					if (this.distanceSqr(v, v2) < 100f)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600B7AB RID: 47019 RVA: 0x00445DC4 File Offset: 0x00443FC4
		public bool IsConnectedTo(Path path)
		{
			if (this.Crosses(path))
			{
				return true;
			}
			foreach (Vector2 v in path.FinalPathPoints)
			{
				if (this.distanceSqr(this.StartPosition.AsVector2(), v) < 100f)
				{
					return true;
				}
				if (this.distanceSqr(this.EndPosition.AsVector2(), v) < 100f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B7AC RID: 47020 RVA: 0x00445E60 File Offset: 0x00444060
		public bool IsConnectedToHighway()
		{
			if (this.worldBuilder.PathingUtils.IsPointOnHighwayWorld(this.StartPosition.x, this.StartPosition.y))
			{
				return true;
			}
			if (this.worldBuilder.PathingUtils.IsPointOnHighwayWorld(this.EndPosition.x, this.EndPosition.y))
			{
				return true;
			}
			foreach (Vector2 vector in this.FinalPathPoints)
			{
				if (this.worldBuilder.PathingUtils.IsPointOnHighwayWorld((int)vector.x, (int)vector.y))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B7AD RID: 47021 RVA: 0x00445F28 File Offset: 0x00444128
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceSqr(Vector2i v1, Vector2i v2)
		{
			int num = v1.x - v2.x;
			int num2 = v1.y - v2.y;
			return (float)(num * num + num2 * num2);
		}

		// Token: 0x0600B7AE RID: 47022 RVA: 0x00445F58 File Offset: 0x00444158
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float distanceSqr(Vector2 v1, Vector2 v2)
		{
			float num = v1.x - v2.x;
			float num2 = v1.y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600B7AF RID: 47023 RVA: 0x00445F88 File Offset: 0x00444188
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float distanceSqr(float x, float y, Vector2 v2)
		{
			float num = x - v2.x;
			float num2 = y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600B7B0 RID: 47024 RVA: 0x00445FAC File Offset: 0x004441AC
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<Vector3> romChain(List<Vector3> points, int numberOfPointsOnCurve = 5, float parametricSplineVal = 0.2f)
		{
			List<Vector3> list = new List<Vector3>(points.Count * numberOfPointsOnCurve + 1);
			for (int i = 0; i < points.Count - 3; i++)
			{
				list.AddRange(this.catmulRom(points[i], points[i + 1], points[i + 2], points[i + 3], numberOfPointsOnCurve, parametricSplineVal));
			}
			return list;
		}

		// Token: 0x0600B7B1 RID: 47025 RVA: 0x00446010 File Offset: 0x00444210
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector3> catmulRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int numberOfPointsOnCurve, float parametricSplineVal)
		{
			List<Vector3> list = new List<Vector3>(numberOfPointsOnCurve + 1);
			float t = this.getT(0f, p0, p1, parametricSplineVal);
			float t2 = this.getT(t, p1, p2, parametricSplineVal);
			float t3 = this.getT(t2, p2, p3, parametricSplineVal);
			for (float num = t; num < t2; num += (t2 - t) / (float)numberOfPointsOnCurve)
			{
				Vector3 a = (t - num) / (t - 0f) * p0 + (num - 0f) / (t - 0f) * p1;
				Vector3 a2 = (t2 - num) / (t2 - t) * p1 + (num - t) / (t2 - t) * p2;
				Vector3 a3 = (t3 - num) / (t3 - t2) * p2 + (num - t2) / (t3 - t2) * p3;
				Vector3 a4 = (t2 - num) / (t2 - 0f) * a + (num - 0f) / (t2 - 0f) * a2;
				Vector3 a5 = (t3 - num) / (t3 - t) * a2 + (num - t) / (t3 - t) * a3;
				Vector3 item = (t2 - num) / (t2 - t) * a4 + (num - t) / (t2 - t) * a5;
				list.Add(item);
			}
			return list;
		}

		// Token: 0x0600B7B2 RID: 47026 RVA: 0x00446168 File Offset: 0x00444368
		[PublicizedFrom(EAccessModifier.Protected)]
		public float getT(float t, Vector3 p0, Vector3 p1, float alpha)
		{
			if (p0 == p1)
			{
				return t;
			}
			return Mathf.Pow((p1 - p0).sqrMagnitude, 0.5f * alpha) + t;
		}

		// Token: 0x0600B7B3 RID: 47027 RVA: 0x004461A0 File Offset: 0x004443A0
		public void CommitPathingMapData()
		{
			PathTile tile;
			tile.TileState = (this.isCountryRoad ? PathTile.PathTileStates.Country : PathTile.PathTileStates.Highway);
			for (int i = 0; i < this.FinalPathPoints.Length - 1; i++)
			{
				Vector2i vector2i;
				vector2i.x = (int)this.FinalPathPoints[i].x;
				vector2i.y = (int)this.FinalPathPoints[i].y;
				if (!this.isCountryRoad)
				{
					this.SetPathTileAnd4WayWorld(vector2i.x, vector2i.y, tile);
					StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i.x, vector2i.y);
					if (streetTileWorld != null)
					{
						streetTileWorld.AddHighway(this);
					}
				}
				else
				{
					this.SetPathTileWorld(vector2i.x, vector2i.y, tile);
				}
			}
		}

		// Token: 0x0600B7B4 RID: 47028 RVA: 0x00446264 File Offset: 0x00444464
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetPathTileWorld(int x, int y, PathTile _tile)
		{
			x /= 10;
			if ((ulong)x >= (ulong)((long)this.worldBuilder.data.PathTileGridWidth))
			{
				return;
			}
			y /= 10;
			if ((ulong)y >= (ulong)((long)this.worldBuilder.data.PathTileGridWidth))
			{
				return;
			}
			int index = x + y * this.worldBuilder.data.PathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[index] = _tile;
		}

		// Token: 0x0600B7B5 RID: 47029 RVA: 0x004462D8 File Offset: 0x004444D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetPathTileAnd4WayWorld(int x, int y, PathTile _tile)
		{
			int pathTileGridWidth = this.worldBuilder.data.PathTileGridWidth;
			x /= 10;
			x -= 2;
			if ((ulong)x >= (ulong)((long)(pathTileGridWidth - 4)))
			{
				return;
			}
			y /= 10;
			y -= 2;
			if ((ulong)y >= (ulong)((long)(pathTileGridWidth - 4)))
			{
				return;
			}
			int num = x + y * pathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[num + 2] = _tile;
			num += pathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[num + 2] = _tile;
			num += pathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[num] = _tile;
			this.worldBuilder.data.PathTileGrid[num + 1] = _tile;
			this.worldBuilder.data.PathTileGrid[num + 2] = _tile;
			this.worldBuilder.data.PathTileGrid[num + 3] = _tile;
			this.worldBuilder.data.PathTileGrid[num + 4] = _tile;
			num += pathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[num + 2] = _tile;
			num += pathTileGridWidth;
			this.worldBuilder.data.PathTileGrid[num + 2] = _tile;
		}

		// Token: 0x0600B7B6 RID: 47030 RVA: 0x00446414 File Offset: 0x00444614
		[PublicizedFrom(EAccessModifier.Private)]
		public void SmoothHeights(float[] heights)
		{
			for (int i = 0; i < this.pathPoints3d.Length; i++)
			{
				heights[i] = this.pathPoints3d[i].y;
			}
			for (int j = 1; j < this.pathPoints3d.Length - 1; j++)
			{
				Vector3 vector = this.pathPoints3d[j];
				int index = (int)vector.x + (int)vector.z * this.worldBuilder.WorldSize;
				if (this.worldBuilder.data.poiHeightMask[index] == 0)
				{
					vector.y = (heights[j - 1] + heights[j] + heights[j + 1]) * 0.33333334f;
					this.pathPoints3d[j] = vector;
				}
			}
		}

		// Token: 0x04008994 RID: 35220
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSingleLaneRadius = 4.5f;

		// Token: 0x04008995 RID: 35221
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cShoulderWidth = 1;

		// Token: 0x04008996 RID: 35222
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cBlendDistCountry = 6f;

		// Token: 0x04008997 RID: 35223
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cBlendDistHighway = 10f;

		// Token: 0x04008998 RID: 35224
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightSmoothAverageBias = 8f;

		// Token: 0x04008999 RID: 35225
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightSmoothDecreasePer = 0.3f;

		// Token: 0x0400899A RID: 35226
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x0400899B RID: 35227
		public readonly Vector2i StartPosition;

		// Token: 0x0400899C RID: 35228
		public readonly Vector2i EndPosition;

		// Token: 0x0400899D RID: 35229
		[PublicizedFrom(EAccessModifier.Private)]
		public int lanes;

		// Token: 0x0400899E RID: 35230
		[PublicizedFrom(EAccessModifier.Private)]
		public float radius = 8f;

		// Token: 0x0400899F RID: 35231
		public bool isCountryRoad;

		// Token: 0x040089A0 RID: 35232
		public bool isRiver;

		// Token: 0x040089A1 RID: 35233
		public bool connectsToHighway;

		// Token: 0x040089A2 RID: 35234
		public int Cost;

		// Token: 0x040089A3 RID: 35235
		public bool IsValid;

		// Token: 0x040089A4 RID: 35236
		public NativeList<Vector2> FinalPathPoints;

		// Token: 0x040089A5 RID: 35237
		[PublicizedFrom(EAccessModifier.Private)]
		public NativeList<Vector3> pathPoints3d = new NativeList<Vector3>(Allocator.Persistent);

		// Token: 0x040089A6 RID: 35238
		[PublicizedFrom(EAccessModifier.Private)]
		public const int FreeId = 0;

		// Token: 0x040089A7 RID: 35239
		[PublicizedFrom(EAccessModifier.Private)]
		public const int CountryId = 1;

		// Token: 0x040089A8 RID: 35240
		[PublicizedFrom(EAccessModifier.Private)]
		public const int HighwayId = 2;

		// Token: 0x040089A9 RID: 35241
		[PublicizedFrom(EAccessModifier.Private)]
		public const int HighwayDirtId = 3;

		// Token: 0x040089AA RID: 35242
		[PublicizedFrom(EAccessModifier.Private)]
		public const int WaterId = 4;

		// Token: 0x040089AB RID: 35243
		[PublicizedFrom(EAccessModifier.Private)]
		public const int HighwayBlendIdMask = 128;
	}
}
