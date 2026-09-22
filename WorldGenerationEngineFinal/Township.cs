using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200173F RID: 5951
	public class Township
	{
		// Token: 0x0600B913 RID: 47379 RVA: 0x0044FAD0 File Offset: 0x0044DCD0
		public Township(WorldBuilder _worldBuilder, TownshipData _townshipData)
		{
			this.worldBuilder = _worldBuilder;
			this.townshipData = _townshipData;
		}

		// Token: 0x0600B914 RID: 47380 RVA: 0x0044FB28 File Offset: 0x0044DD28
		public void Cleanup()
		{
			GameRandomManager.Instance.FreeGameRandom(this.rand);
		}

		// Token: 0x1700167A RID: 5754
		// (get) Token: 0x0600B915 RID: 47381 RVA: 0x0044FB3A File Offset: 0x0044DD3A
		public TownshipData Data
		{
			get
			{
				return this.townshipData;
			}
		}

		// Token: 0x0600B916 RID: 47382 RVA: 0x0044FB42 File Offset: 0x0044DD42
		public string GetTypeName()
		{
			return this.townshipData.Name;
		}

		// Token: 0x0600B917 RID: 47383 RVA: 0x0044FB4F File Offset: 0x0044DD4F
		public bool IsBig()
		{
			return this.townshipData.Name == "citybig";
		}

		// Token: 0x0600B918 RID: 47384 RVA: 0x0044FB66 File Offset: 0x0044DD66
		public bool IsRoadside()
		{
			return this.townshipData.Category == TownshipData.eCategory.Roadside;
		}

		// Token: 0x0600B919 RID: 47385 RVA: 0x0044FB76 File Offset: 0x0044DD76
		public bool IsRural()
		{
			return this.townshipData.Category == TownshipData.eCategory.Rural;
		}

		// Token: 0x0600B91A RID: 47386 RVA: 0x0044FB86 File Offset: 0x0044DD86
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsWilderness()
		{
			return this.townshipData.Category == TownshipData.eCategory.Wilderness;
		}

		// Token: 0x0600B91B RID: 47387 RVA: 0x0044FB96 File Offset: 0x0044DD96
		public void SortGatewaysClockwise()
		{
			this.Gateways.Sort(delegate(StreetTile _t1, StreetTile _t2)
			{
				float num = Mathf.Atan2((float)(_t1.GridPosition.y - this.GridCenter.y), (float)(_t1.GridPosition.x - this.GridCenter.x));
				float value = Mathf.Atan2((float)(_t2.GridPosition.y - this.GridCenter.y), (float)(_t2.GridPosition.x - this.GridCenter.x));
				return num.CompareTo(value);
			});
		}

		// Token: 0x0600B91C RID: 47388 RVA: 0x0044FBB0 File Offset: 0x0044DDB0
		public void CleanupStreets()
		{
			if (this.Streets == null || this.Streets.Count == 0)
			{
				Log.Error("CleanupStreets none!");
				return;
			}
			this.rand = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + this.ID + this.Streets.Count);
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			int num4 = int.MinValue;
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				num = Utils.FastMin(num, streetTile.WorldPosition.x);
				num2 = Utils.FastMin(num2, streetTile.WorldPosition.y);
				num3 = Utils.FastMax(num3, streetTile.WorldPositionMax.x);
				num4 = Utils.FastMax(num4, streetTile.WorldPositionMax.y);
			}
			this.Area = new Rect((float)num, (float)num2, (float)(num3 - num), (float)(num4 - num2));
			this.BufferArea = new Rect(this.Area.xMin - 150f, this.Area.yMin - 150f, this.Area.width + 300f, this.Area.height + 300f);
		}

		// Token: 0x0600B91D RID: 47389 RVA: 0x0044FD1C File Offset: 0x0044DF1C
		public void SpawnPrefabs()
		{
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				if (streetTile == null)
				{
					Log.Error("WorldTileData is null, this shouldn't happen!");
				}
				else
				{
					streetTile.SpawnPrefabs();
				}
			}
			this.Prefabs.Clear();
		}

		// Token: 0x0600B91E RID: 47390 RVA: 0x0044FD90 File Offset: 0x0044DF90
		public StreetTile CalcCenterStreetTile()
		{
			if (this.centerMostTile != null)
			{
				return this.centerMostTile;
			}
			Vector2i vector2i = Vector2i.zero;
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				vector2i += streetTile.GridPosition;
			}
			vector2i /= this.Streets.Count;
			int num = int.MaxValue;
			StreetTile result = null;
			foreach (StreetTile streetTile2 in this.Streets.Values)
			{
				int num2 = Vector2i.DistanceSqrInt(vector2i, streetTile2.GridPosition);
				if (num2 < num)
				{
					num = num2;
					result = streetTile2;
				}
			}
			this.centerMostTile = result;
			float num3 = 0f;
			Vector2i worldPositionCenter = this.centerMostTile.WorldPositionCenter;
			for (int i = -50; i <= 50; i += 50)
			{
				Vector2i pos;
				pos.y = worldPositionCenter.y + i;
				for (int j = -50; j <= 50; j += 50)
				{
					pos.x = worldPositionCenter.x + j;
					num3 += this.worldBuilder.GetHeight(pos);
				}
			}
			this.Height = Mathf.CeilToInt(num3 / 9f);
			this.Height += 3;
			if (this.Streets.Count > 2 && this.Height < 130)
			{
				if (this.BiomeType == BiomeType.snow)
				{
					this.Height += 25;
				}
				else if (this.BiomeType == BiomeType.wasteland)
				{
					this.Height += 12;
				}
			}
			return result;
		}

		// Token: 0x0600B91F RID: 47391 RVA: 0x0044FF5C File Offset: 0x0044E15C
		public void AddToUsedPOIList(string name)
		{
			this.worldBuilder.PrefabManager.AddUsedPrefab(name);
		}

		// Token: 0x0600B920 RID: 47392 RVA: 0x0044FF70 File Offset: 0x0044E170
		public List<Vector2i> GetUnusedTownExits(int _gatewayUnusedMax = 4)
		{
			this.list.Clear();
			if (this.townshipData.SpawnGateway)
			{
				using (List<StreetTile>.Enumerator enumerator = this.Gateways.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StreetTile streetTile = enumerator.Current;
						if (streetTile.UsedExitList.Count <= _gatewayUnusedMax)
						{
							foreach (Vector2i item in streetTile.GetHighwayExits(true))
							{
								this.list.Add(item);
							}
						}
					}
					goto IL_102;
				}
			}
			foreach (StreetTile streetTile2 in this.Streets.Values)
			{
				foreach (Vector2i item2 in streetTile2.GetHighwayExits(false))
				{
					this.list.Add(item2);
				}
			}
			IL_102:
			return this.list;
		}

		// Token: 0x0600B921 RID: 47393 RVA: 0x004500BC File Offset: 0x0044E2BC
		public void AddPrefab(PrefabDataInstance pdi)
		{
			this.Prefabs.Add(pdi);
			this.worldBuilder.PrefabManager.AddUsedPrefabWorld(this.ID, pdi);
		}

		// Token: 0x0600B922 RID: 47394 RVA: 0x004500E4 File Offset: 0x0044E2E4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool[] GetNeighborExits(StreetTile current)
		{
			bool[] array = new bool[4];
			int num = -1;
			foreach (StreetTile streetTile in current.GetNeighbors())
			{
				num++;
				if (streetTile != null && streetTile.District != null && streetTile.Township != null)
				{
					bool flag = current.District.name == "highway";
					bool flag2 = current.District.name == "gateway";
					bool flag3 = streetTile.District.name == "highway";
					bool flag4 = streetTile.District.name == "gateway";
					if ((streetTile.Township == current.Township || ((flag || flag4 || flag2 || flag3) && (!flag2 || flag3) && (!flag || flag4))) && (streetTile.HasExitTo(current) || (flag && flag3)))
					{
						array[num] = true;
					}
				}
			}
			return array;
		}

		// Token: 0x0600B923 RID: 47395 RVA: 0x004501E0 File Offset: 0x0044E3E0
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetNeighborCount(Vector2i current)
		{
			int num = 0;
			for (int i = 0; i < this.worldBuilder.TownshipShared.dir4way.Length; i++)
			{
				Vector2i key = current + this.worldBuilder.TownshipShared.dir4way[i];
				if (this.Streets.ContainsKey(key))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600B924 RID: 47396 RVA: 0x0045023C File Offset: 0x0044E43C
		public override string ToString()
		{
			return string.Format("Township {0}, {1}, {2}", this.townshipData.Name, this.townshipData.Category, this.ID);
		}

		// Token: 0x04008A99 RID: 35481
		[PublicizedFrom(EAccessModifier.Private)]
		public const int BUFFER_DISTANCE = 300;

		// Token: 0x04008A9A RID: 35482
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A9B RID: 35483
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly TownshipData townshipData;

		// Token: 0x04008A9C RID: 35484
		public int ID;

		// Token: 0x04008A9D RID: 35485
		public BiomeType BiomeType;

		// Token: 0x04008A9E RID: 35486
		public Rect Area;

		// Token: 0x04008A9F RID: 35487
		public Rect BufferArea;

		// Token: 0x04008AA0 RID: 35488
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile commercialCap;

		// Token: 0x04008AA1 RID: 35489
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile ruralCap;

		// Token: 0x04008AA2 RID: 35490
		public Vector2i GridCenter;

		// Token: 0x04008AA3 RID: 35491
		public int Height;

		// Token: 0x04008AA4 RID: 35492
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile centerMostTile;

		// Token: 0x04008AA5 RID: 35493
		public Dictionary<Vector2i, StreetTile> Streets = new Dictionary<Vector2i, StreetTile>();

		// Token: 0x04008AA6 RID: 35494
		public List<PrefabDataInstance> Prefabs = new List<PrefabDataInstance>();

		// Token: 0x04008AA7 RID: 35495
		public List<StreetTile> Gateways = new List<StreetTile>();

		// Token: 0x04008AA8 RID: 35496
		public Dictionary<Township, int> TownshipConnectionCounts = new Dictionary<Township, int>();

		// Token: 0x04008AA9 RID: 35497
		public GameRandom rand;

		// Token: 0x04008AAA RID: 35498
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector2i> list = new List<Vector2i>();
	}
}
