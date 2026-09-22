using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200173A RID: 5946
	public class TownPlanner
	{
		// Token: 0x0600B902 RID: 47362 RVA: 0x0044E69C File Offset: 0x0044C89C
		public TownPlanner(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600B903 RID: 47363 RVA: 0x0044E794 File Offset: 0x0044C994
		public void Plan(DynamicProperties _properties, int worldSeed)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			Dictionary<BiomeType, List<Vector2i>> streetTilesByBiome = this.getStreetTilesByBiome();
			Dictionary<int, TownPlanner.TownshipSpawnInfo> townshipSpawnInfos = new Dictionary<int, TownPlanner.TownshipSpawnInfo>();
			this.getTownshipCounts(_properties, townshipSpawnInfos);
			int num = 0;
			List<int> list = new List<int>();
			townshipSpawnInfos.CopyKeysTo(list);
			list.Sort((int key1, int key2) => townshipSpawnInfos[key2].max.CompareTo(townshipSpawnInfos[key1].max));
			int num2 = 1982;
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom();
			List<int> list2 = new List<int>
			{
				0,
				1,
				2,
				3
			};
			foreach (int key in list)
			{
				TownshipData townshipData;
				if (WorldBuilderStatic.idToTownshipData.TryGetValue(key, out townshipData))
				{
					string name = townshipData.Name;
					this.worldBuilder.SetTaskMessage(string.Format(this.worldBuilder.messageTownPlanning, Application.isEditor ? name : string.Empty));
					gameRandom.SetSeed(worldSeed + num2++);
					bool flag = townshipData.Category == TownshipData.eCategory.Roadside;
					int num3 = this.worldBuilder.StreetTileMapWidth / 10;
					if (flag)
					{
						num3 = this.worldBuilder.StreetTileMapWidth / 8;
					}
					int num4 = this.worldBuilder.StreetTileMapWidth - num3;
					int num5 = this.worldBuilder.StreetTileMapWidth - num3;
					int num6 = 80;
					int num7 = 30;
					bool flag2 = false;
					TownPlanner.TownshipSpawnInfo townshipSpawnInfo = townshipSpawnInfos[key];
					BiomeType biomeType = BiomeType.none;
					int i = 0;
					while (i < townshipSpawnInfo.count)
					{
						int distance = flag2 ? 1 : (flag ? 3 : townshipSpawnInfo.distance);
						if (biomeType == BiomeType.none)
						{
							biomeType = this.getBiomeWithMostAvailableSpace(townshipData);
						}
						List<Vector2i> list3 = null;
						BiomeType biomeType2 = biomeType;
						for (int j = 0; j < 5; j++)
						{
							List<Vector2i> collection;
							if (streetTilesByBiome.TryGetValue(biomeType, out collection))
							{
								list3 = new List<Vector2i>(collection);
								for (int k = list3.Count - 1; k >= 0; k--)
								{
									Vector2i vector2i = list3[k];
									if (vector2i.x <= num3 || vector2i.y <= num3 || vector2i.x >= num4 || vector2i.y >= num5 || this.IsTooClose(vector2i, flag, distance))
									{
										list3.RemoveAt(k);
									}
								}
								if (list3.Count > 0)
								{
									break;
								}
							}
							biomeType = this.nextBiomeType(biomeType, townshipData);
							if (biomeType == biomeType2)
							{
								break;
							}
						}
						Township township = null;
						List<StreetTile> list4 = null;
						List<StreetTile> list5 = null;
						int l;
						if (list3 != null && list3.Count > 0)
						{
							list4 = new List<StreetTile>();
							list5 = new List<StreetTile>();
							township = new Township(this.worldBuilder, townshipData)
							{
								BiomeType = biomeType
							};
							int num8 = townshipSpawnInfo.min;
							if (townshipSpawnInfo.max >= 10)
							{
								num8 = (townshipSpawnInfo.min + townshipSpawnInfo.max) / 2;
							}
							l = gameRandom.RandomRange(num8, townshipSpawnInfo.max + 1);
							num8 = townshipSpawnInfo.min;
							if (flag2 && num8 >= 4)
							{
								num8 = 1;
								l = 4;
							}
							while (l >= num8)
							{
								int num9 = gameRandom.RandomRange(0, list3.Count);
								int num10 = (gameRandom.RandomFloat < 0.5f) ? (list3.Count - 1) : 1;
								for (int m = 0; m < list3.Count; m++)
								{
									Vector2i vector2i2 = list3[num9];
									township.GridCenter = vector2i2;
									num9 = (num9 + num10) % list3.Count;
									for (int n = 0; n < 12; n++)
									{
										this.GetStreetLayout(vector2i2, l, gameRandom, flag, list4);
										if (list4.Count >= l && (townshipData.OutskirtDistrictPercent <= 0f || this.Grow(townshipData, list4, list5, !flag2)))
										{
											m = 999999;
											l = -1;
											break;
										}
									}
								}
								l--;
							}
						}
						else
						{
							l = 0;
							num6 = 0;
						}
						if (l < 0)
						{
							goto IL_422;
						}
						if (num6 > 0)
						{
							num6--;
							i--;
							biomeType = this.nextBiomeType(biomeType, townshipData);
						}
						else
						{
							if (townshipSpawnInfo.min < 4 || num7 <= 0)
							{
								goto IL_422;
							}
							num7--;
							i--;
							flag2 = true;
							biomeType = this.nextBiomeType(biomeType, townshipData);
						}
						IL_768:
						i++;
						continue;
						IL_422:
						num6 = 80;
						num7 = 30;
						flag2 = false;
						if (l >= 0)
						{
							biomeType = BiomeType.none;
							goto IL_768;
						}
						if (townshipData.OutskirtDistrictPercent > 0f)
						{
							string outskirtDistrict = townshipData.OutskirtDistrict;
							foreach (StreetTile streetTile in list5)
							{
								streetTile.SetTownship(township);
								streetTile.District = DistrictPlannerStatic.Districts[outskirtDistrict];
								township.Streets[streetTile.GridPosition] = streetTile;
							}
						}
						foreach (StreetTile streetTile2 in list4)
						{
							streetTile2.SetTownship(township);
							township.Streets[streetTile2.GridPosition] = streetTile2;
							streetTile2.District = null;
						}
						this.worldBuilder.DistrictPlanner.PlanTownship(township);
						if (!township.IsRoadside())
						{
							foreach (StreetTile streetTile3 in township.Streets.Values)
							{
								streetTile3.SetExitsToMyTownship();
							}
							foreach (StreetTile streetTile4 in township.Streets.Values)
							{
								if (streetTile4.District.type != District.Type.Gateway)
								{
									list2.Shuffle(gameRandom);
									foreach (int num11 in list2)
									{
										StreetTile neighbor = streetTile4.GetNeighbor(num11);
										if (neighbor != null && neighbor.District != null && neighbor.District.type != District.Type.Gateway && neighbor.District != streetTile4.District)
										{
											int num12 = streetTile4.CountDistrictExitsBetween(neighbor);
											int num13 = (streetTile4.District.type == District.Type.Downtown || neighbor.District.type == District.Type.Downtown) ? 2 : 1;
											if (num12 > num13)
											{
												streetTile4.SetExitUnUsedAndFromNeighbor(num11);
											}
										}
									}
								}
							}
							int num14 = 0;
							foreach (StreetTile streetTile5 in township.Streets.Values)
							{
								if (streetTile5.District.type != District.Type.Gateway)
								{
									int num15 = 1 << (int)streetTile5.District.type;
									if ((num14 & num15) == 0 && streetTile5.ChangeLConnectionToCap(gameRandom))
									{
										num14 |= num15;
									}
								}
							}
						}
						township.CleanupStreets();
						township.ID = num++;
						this.worldBuilder.Townships.Add(township);
						TownPlanner.BiomeStats biomeStats;
						if (!this.biomeStats.TryGetValue(biomeType, out biomeStats))
						{
							biomeStats = new TownPlanner.BiomeStats();
							this.biomeStats.Add(biomeType, biomeStats);
						}
						biomeStats.townshipCount++;
						int num16;
						if (!biomeStats.counts.TryGetValue(name, out num16))
						{
							biomeStats.counts.Add(name, 1);
						}
						else
						{
							biomeStats.counts[name] = num16 + 1;
						}
						biomeType = this.nextBiomeType(biomeType, townshipData);
						goto IL_768;
					}
				}
			}
			Log.Out("TownPlanner Plan {0} in {1}", new object[]
			{
				this.worldBuilder.Townships.Count,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
			for (int num17 = 0; num17 < 5; num17++)
			{
				BiomeType biomeType3 = (BiomeType)num17;
				TownPlanner.BiomeStats biomeStats2;
				if (this.biomeStats.TryGetValue(biomeType3, out biomeStats2))
				{
					string text = "";
					foreach (KeyValuePair<string, int> keyValuePair in biomeStats2.counts)
					{
						text += string.Format(", {0} {1}", keyValuePair.Key, keyValuePair.Value);
					}
					Log.Out("TownPlanner {0} has {1} townships{2}", new object[]
					{
						biomeType3,
						biomeStats2.townshipCount,
						text
					});
				}
			}
			this.biomeStats.Clear();
			this.worldBuilder.SetTaskMessage(this.worldBuilder.messageTownPlanningFinished);
		}

		// Token: 0x0600B904 RID: 47364 RVA: 0x0044F118 File Offset: 0x0044D318
		public void SpawnPrefabs()
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			foreach (Township township in this.worldBuilder.Townships)
			{
				township.SpawnPrefabs();
			}
			Log.Out(string.Format("TownPlanner SpawnPrefabs in {0}, r={1:x}", (float)microStopwatch.ElapsedMilliseconds * 0.001f, Rand.Instance.PeekSample()));
		}

		// Token: 0x0600B905 RID: 47365 RVA: 0x0044F1A4 File Offset: 0x0044D3A4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsTooClose(Vector2i position, bool isRoadsideTownship, int distance)
		{
			foreach (Township township in this.worldBuilder.Townships)
			{
				bool flag = township.IsRoadside();
				foreach (Vector2i vector2i in township.Streets.Keys)
				{
					if (isRoadsideTownship && flag && Utils.FastAbs((float)(position.x - vector2i.x)) < 5f && Utils.FastAbs((float)(position.y - vector2i.y)) < 5f)
					{
						return true;
					}
					if (Utils.FastAbs((float)(position.x - vector2i.x)) < (float)distance && Utils.FastAbs((float)(position.y - vector2i.y)) < (float)distance)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600B906 RID: 47366 RVA: 0x0044F2B8 File Offset: 0x0044D4B8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool Grow(TownshipData _data, List<StreetTile> baseTiles, List<StreetTile> finalTiles, bool _cancelIfInvalidNeighbor)
		{
			finalTiles.Clear();
			Vector2i[] array = (_data.OutskirtDistrictPercent < 1f) ? this.dir4way : this.dir8way;
			foreach (StreetTile streetTile in baseTiles)
			{
				foreach (Vector2i other in array)
				{
					StreetTile streetTileGrid = this.worldBuilder.GetStreetTileGrid(streetTile.GridPosition + other);
					if (streetTileGrid != null && !baseTiles.Contains(streetTileGrid) && !finalTiles.Contains(streetTileGrid))
					{
						if (!streetTileGrid.IsValidForStreetTile)
						{
							if (_cancelIfInvalidNeighbor)
							{
								return false;
							}
						}
						else
						{
							StreetTile[] neighbors = streetTileGrid.GetNeighbors();
							for (int j = 0; j < neighbors.Length; j++)
							{
								if (neighbors[j].Township != null)
								{
									return false;
								}
							}
							finalTiles.Add(streetTileGrid);
						}
					}
				}
			}
			if (_data.OutskirtDistrictPercent < 1f)
			{
				float num = 1f - _data.OutskirtDistrictPercent;
				float num2 = 0f;
				for (int k = finalTiles.Count - 1; k >= 0; k--)
				{
					num2 += num;
					if (num2 >= 1f)
					{
						num2 -= 1f;
						finalTiles.RemoveAt(k);
					}
				}
			}
			return true;
		}

		// Token: 0x0600B907 RID: 47367 RVA: 0x0044F42C File Offset: 0x0044D62C
		[PublicizedFrom(EAccessModifier.Private)]
		public int getTownshipCounts(DynamicProperties _properties, Dictionary<int, TownPlanner.TownshipSpawnInfo> townshipSpawnInfo)
		{
			int num = 0;
			GameRandom gameRandom = Rand.Instance.gameRandom;
			foreach (KeyValuePair<int, TownshipData> keyValuePair in WorldBuilderStatic.idToTownshipData)
			{
				TownshipData value = keyValuePair.Value;
				if (value.Category != TownshipData.eCategory.Wilderness)
				{
					string text = value.Name.ToLower();
					float num2 = 1f;
					float num3 = 1f;
					_properties.ParseVec(text, "tiles", ref num2, ref num3);
					int count = this.worldBuilder.GetCount(text, this.worldBuilder.Towns, gameRandom);
					int distance = 5;
					_properties.ParseInt(text, "distance", ref distance);
					if (count >= 1 && num2 >= 1f && num3 >= 1f)
					{
						townshipSpawnInfo.Add(value.Id, new TownPlanner.TownshipSpawnInfo((int)num2, (int)num3, count, distance));
					}
					num += count;
				}
			}
			return num;
		}

		// Token: 0x0600B908 RID: 47368 RVA: 0x0044F534 File Offset: 0x0044D734
		[PublicizedFrom(EAccessModifier.Private)]
		public BiomeType nextBiomeType(BiomeType _current, TownshipData _townshipData)
		{
			int num = (int)_current;
			for (int i = 0; i < 4; i++)
			{
				num = (num + 1) % 5;
				if (_townshipData.Biomes.IsEmpty || _townshipData.Biomes.Test_Bit(this.worldBuilder.biomeTagBits[num]))
				{
					return (BiomeType)num;
				}
			}
			return BiomeType.none;
		}

		// Token: 0x0600B909 RID: 47369 RVA: 0x0044F584 File Offset: 0x0044D784
		[PublicizedFrom(EAccessModifier.Private)]
		public BiomeType getBiomeWithMostAvailableSpace(TownshipData _townshipData)
		{
			int num = 0;
			int num2 = 255;
			List<Vector2i> list = new List<Vector2i>();
			for (int i = 0; i < 5; i++)
			{
				if (_townshipData.Biomes.IsEmpty || _townshipData.Biomes.Test_Bit(this.worldBuilder.biomeTagBits[i]))
				{
					this.getStreetTilesForBiome((BiomeType)i, list);
					if (list.Count > num)
					{
						num = list.Count;
						num2 = i;
					}
				}
			}
			return (BiomeType)num2;
		}

		// Token: 0x0600B90A RID: 47370 RVA: 0x0044F5F0 File Offset: 0x0044D7F0
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<BiomeType, List<Vector2i>> getStreetTilesByBiome()
		{
			Dictionary<BiomeType, List<Vector2i>> dictionary = new Dictionary<BiomeType, List<Vector2i>>();
			for (int i = 0; i < 5; i++)
			{
				List<Vector2i> list = new List<Vector2i>();
				this.getStreetTilesForBiome((BiomeType)i, list);
				if (list.Count > 0)
				{
					dictionary.Add((BiomeType)i, list);
				}
			}
			return dictionary;
		}

		// Token: 0x0600B90B RID: 47371 RVA: 0x0044F634 File Offset: 0x0044D834
		[PublicizedFrom(EAccessModifier.Private)]
		public void getStreetTilesForBiome(BiomeType _biomeType, List<Vector2i> _biomeStreetTiles)
		{
			_biomeStreetTiles.Clear();
			foreach (StreetTile streetTile in this.worldBuilder.StreetTileMap)
			{
				if (streetTile.IsValidForStreetTile && streetTile.Township == null && !streetTile.Used && streetTile.BiomeType == _biomeType)
				{
					_biomeStreetTiles.Add(streetTile.GridPosition);
				}
			}
		}

		// Token: 0x0600B90C RID: 47372 RVA: 0x0044F694 File Offset: 0x0044D894
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetStreetLayout(Vector2i startPosition, int townSize, GameRandom rnd, bool isRoadside, List<StreetTile> townTiles)
		{
			townTiles.Clear();
			StreetTile streetTileGrid = this.worldBuilder.GetStreetTileGrid(startPosition);
			if (!isRoadside)
			{
				using (List<Township>.Enumerator enumerator = this.worldBuilder.Townships.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Township township = enumerator.Current;
						if (streetTileGrid.Area.Overlaps(township.BufferArea))
						{
							return;
						}
					}
					goto IL_103;
				}
			}
			if (streetTileGrid.Township != null || streetTileGrid.District != null)
			{
				return;
			}
			foreach (Township township2 in this.worldBuilder.Townships)
			{
				if (streetTileGrid.Area.Overlaps(township2.BufferArea))
				{
					return;
				}
			}
			foreach (StreetTile streetTile in streetTileGrid.GetNeighbors8Way())
			{
				if (streetTile.Township != null || streetTile.District != null)
				{
					return;
				}
			}
			IL_103:
			if (townSize == 1)
			{
				townTiles.Add(streetTileGrid);
				return;
			}
			List<StreetTile> list = new List<StreetTile>();
			if (townSize <= 16)
			{
				townTiles.Add(streetTileGrid);
				list.Add(streetTileGrid);
			}
			else
			{
				int num = (int)Mathf.Sqrt((float)townSize) - 1;
				int num2 = num / -2;
				int num3 = num / 2 - 1;
				for (int j = num2; j <= num3; j++)
				{
					Vector2i pos;
					pos.y = startPosition.y + j;
					bool flag = j == num2 || j == num3;
					for (int k = num2; k <= num3; k++)
					{
						pos.x = startPosition.x + k;
						StreetTile streetTile2 = this.StreetLayoutCheckPos(pos);
						if (streetTile2 == null)
						{
							townTiles.Clear();
							return;
						}
						townTiles.Add(streetTile2);
						if (flag || k == num2 || k == num3)
						{
							list.Add(streetTile2);
						}
					}
				}
				if (townTiles.Count >= townSize)
				{
					return;
				}
			}
			while (list.Count > 0)
			{
				int index = rnd.RandomRange(0, list.Count);
				StreetTile streetTile3 = list[index];
				list.RemoveAt(index);
				int num4 = rnd.RandomRange(4);
				for (int l = 0; l < 4; l++)
				{
					Vector2i pos2 = streetTile3.GridPosition + this.dir4way[l + num4 & 3];
					StreetTile streetTile4 = this.StreetLayoutCheckPos(pos2);
					if (streetTile4 != null && !townTiles.Contains(streetTile4))
					{
						townTiles.Add(streetTile4);
						if (townTiles.Count >= townSize)
						{
							return;
						}
						list.Add(streetTile4);
					}
				}
			}
		}

		// Token: 0x0600B90D RID: 47373 RVA: 0x0044F940 File Offset: 0x0044DB40
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile StreetLayoutCheckPos(Vector2i _pos)
		{
			int num = 2;
			if (_pos.x < num || _pos.x >= this.worldBuilder.StreetTileMapWidth - num || _pos.y < num || _pos.y >= this.worldBuilder.StreetTileMapWidth - num)
			{
				return null;
			}
			StreetTile streetTileGrid = this.worldBuilder.GetStreetTileGrid(_pos);
			if (streetTileGrid != null && streetTileGrid.IsValidForStreetTile)
			{
				foreach (Township township in this.worldBuilder.Townships)
				{
					if (streetTileGrid.Area.Overlaps(township.BufferArea))
					{
						return null;
					}
				}
				return streetTileGrid;
			}
			return null;
		}

		// Token: 0x04008A89 RID: 35465
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTriesPerTownshipSpawnInfo = 80;

		// Token: 0x04008A8A RID: 35466
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTriesSmaller = 30;

		// Token: 0x04008A8B RID: 35467
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A8C RID: 35468
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<BiomeType, TownPlanner.BiomeStats> biomeStats = new Dictionary<BiomeType, TownPlanner.BiomeStats>();

		// Token: 0x04008A8D RID: 35469
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] dir4way = new Vector2i[]
		{
			Vector2i.up,
			Vector2i.right,
			Vector2i.down,
			Vector2i.left
		};

		// Token: 0x04008A8E RID: 35470
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] dir8way = new Vector2i[]
		{
			Vector2i.up,
			Vector2i.up + Vector2i.right,
			Vector2i.right,
			Vector2i.right + Vector2i.down,
			Vector2i.down,
			Vector2i.down + Vector2i.left,
			Vector2i.left,
			Vector2i.left + Vector2i.up
		};

		// Token: 0x0200173B RID: 5947
		[PublicizedFrom(EAccessModifier.Private)]
		public class BiomeStats
		{
			// Token: 0x04008A8F RID: 35471
			public int townshipCount;

			// Token: 0x04008A90 RID: 35472
			public Dictionary<string, int> counts = new Dictionary<string, int>();
		}

		// Token: 0x0200173C RID: 5948
		public class TownshipSpawnInfo
		{
			// Token: 0x0600B90F RID: 47375 RVA: 0x0044FA1F File Offset: 0x0044DC1F
			public TownshipSpawnInfo(int _min, int _max, int _count, int _distance)
			{
				this.min = _min;
				this.max = _max;
				this.count = _count;
				this.distance = _distance;
			}

			// Token: 0x04008A91 RID: 35473
			public int min;

			// Token: 0x04008A92 RID: 35474
			public int max;

			// Token: 0x04008A93 RID: 35475
			public int count;

			// Token: 0x04008A94 RID: 35476
			public int distance;
		}
	}
}
