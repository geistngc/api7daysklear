using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PrefabVolumes;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001734 RID: 5940
	public class StreetTile
	{
		// Token: 0x0600B8A6 RID: 47270 RVA: 0x0044B1E3 File Offset: 0x004493E3
		public void SetTownship(Township _township)
		{
			this.Township = _township;
			this.GetData().IsCity = (_township != null && !_township.IsWilderness());
		}

		// Token: 0x1700166C RID: 5740
		// (get) Token: 0x0600B8A7 RID: 47271 RVA: 0x0044B206 File Offset: 0x00449406
		public int GroupID
		{
			get
			{
				if (this.Township == null)
				{
					return -1;
				}
				return this.Township.ID;
			}
		}

		// Token: 0x1700166D RID: 5741
		// (get) Token: 0x0600B8A8 RID: 47272 RVA: 0x0044B21D File Offset: 0x0044941D
		public bool IsValidForStreetTile
		{
			get
			{
				return !this.GetData().OverlapsWater && (!this.OverlapsBiomes && !this.OverlapsRadiation && !this.HasSteepSlope) && this.TerrainType != TerrainType.mountains;
			}
		}

		// Token: 0x1700166E RID: 5742
		// (get) Token: 0x0600B8A9 RID: 47273 RVA: 0x0044B254 File Offset: 0x00449454
		public bool IsValidForGateway
		{
			get
			{
				return !this.GetData().OverlapsWater && (!this.OverlapsBiomes && !this.OverlapsRadiation && !this.HasSteepSlope && this.TerrainType != TerrainType.mountains) && !this.HasPrefabs;
			}
		}

		// Token: 0x1700166F RID: 5743
		// (get) Token: 0x0600B8AA RID: 47274 RVA: 0x0044B291 File Offset: 0x00449491
		public bool HasPrefabs
		{
			get
			{
				return this.StreetTilePrefabDatas != null && this.StreetTilePrefabDatas.Count > 0;
			}
		}

		// Token: 0x17001670 RID: 5744
		// (get) Token: 0x0600B8AB RID: 47275 RVA: 0x0044B2AB File Offset: 0x004494AB
		public bool HasStreetTilePrefab
		{
			get
			{
				return this.Township != null && this.District != null && this.HasPrefabs;
			}
		}

		// Token: 0x17001671 RID: 5745
		// (get) Token: 0x0600B8AC RID: 47276 RVA: 0x0044B2C5 File Offset: 0x004494C5
		public Vector2i WorldPositionMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.WorldPosition + new Vector2i(150, 150);
			}
		}

		// Token: 0x17001672 RID: 5746
		// (get) Token: 0x0600B8AD RID: 47277 RVA: 0x0044B2E1 File Offset: 0x004494E1
		public BiomeType BiomeType
		{
			get
			{
				return this.worldBuilder.GetBiome(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001673 RID: 5747
		// (get) Token: 0x0600B8AE RID: 47278 RVA: 0x0044B2F4 File Offset: 0x004494F4
		public float PositionHeight
		{
			get
			{
				return this.worldBuilder.GetHeight(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001674 RID: 5748
		// (get) Token: 0x0600B8AF RID: 47279 RVA: 0x0044B307 File Offset: 0x00449507
		public TerrainType TerrainType
		{
			get
			{
				return this.worldBuilder.GetTerrainType(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001675 RID: 5749
		// (get) Token: 0x0600B8B0 RID: 47280 RVA: 0x0044B31A File Offset: 0x0044951A
		public int RoadExitCount
		{
			get
			{
				if (this.RoadShape < 0)
				{
					return 0;
				}
				return this.worldBuilder.StreetTileShared.RoadShapeExitCounts[this.RoadShape];
			}
		}

		// Token: 0x0600B8B1 RID: 47281 RVA: 0x0044B342 File Offset: 0x00449542
		public bool HasRoadExit(int _dir)
		{
			return this.RoadShape >= 0 && (this.worldBuilder.StreetTileShared.RoadShapeExitsPerRotation[this.RoadShape][this.Rotations] & 1 << _dir) > 0;
		}

		// Token: 0x0600B8B2 RID: 47282 RVA: 0x0044B37B File Offset: 0x0044957B
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetRoadExits(int _shape, int _rotations)
		{
			return this.worldBuilder.StreetTileShared.RoadShapeExitsPerRotation[_shape][_rotations];
		}

		// Token: 0x17001676 RID: 5750
		// (get) Token: 0x0600B8B3 RID: 47283 RVA: 0x0044B395 File Offset: 0x00449595
		public string PrefabName
		{
			get
			{
				if (this.RoadShape < 0)
				{
					return "none";
				}
				return this.worldBuilder.StreetTileShared.RoadShapesDistrict[this.RoadShape];
			}
		}

		// Token: 0x17001677 RID: 5751
		// (get) Token: 0x0600B8B4 RID: 47284 RVA: 0x0044B3BD File Offset: 0x004495BD
		// (set) Token: 0x0600B8B5 RID: 47285 RVA: 0x0044B3C5 File Offset: 0x004495C5
		public int Rotations
		{
			[PublicizedFrom(EAccessModifier.Private)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.rotations;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				if (value != this.rotations)
				{
					this.rotations = value;
				}
			}
		}

		// Token: 0x0600B8B6 RID: 47286 RVA: 0x0044B3D8 File Offset: 0x004495D8
		public StreetTile(WorldBuilder _worldBuilder, Vector2i gridPosition)
		{
			this.worldBuilder = _worldBuilder;
			this.GridPosition = gridPosition;
			this.WorldPosition = this.GridPosition * 150;
			this.WorldPositionCenter = this.WorldPosition + new Vector2i(75, 75);
			this.Area = new Rect(this.WorldPosition.AsVector2(), Vector2.one * 150f);
			this.RoadShape = -1;
			this.Rotations = 0;
			if (this.GridPosition.x < 1 || this.GridPosition.x >= this.worldBuilder.StreetTileMapWidth - 1)
			{
				this.OverlapsRadiation = true;
			}
			if (this.GridPosition.y < 1 || this.GridPosition.y >= this.worldBuilder.StreetTileMapWidth - 1)
			{
				this.OverlapsRadiation = true;
			}
			this.HighwayExitInit();
			this.UpdateValidity();
		}

		// Token: 0x0600B8B7 RID: 47287 RVA: 0x0044B4F4 File Offset: 0x004496F4
		public void UpdateValidity()
		{
			float positionHeight = this.PositionHeight;
			Vector2i worldPositionCenter = this.WorldPositionCenter;
			foreach (Vector2i a in this.worldBuilder.StreetTileShared.dir9way)
			{
				Vector2i vector2i = worldPositionCenter + a * 75;
				if (this.worldBuilder.GetRad(vector2i.x, vector2i.y) > 0)
				{
					this.OverlapsRadiation = true;
				}
				if (Utils.FastAbs(this.worldBuilder.GetHeight(vector2i.x, vector2i.y) - positionHeight) > 20f)
				{
					this.HasSteepSlope = true;
				}
			}
			ref StreetTileData data = ref this.GetData();
			BiomeType biomeType = this.BiomeType;
			int num = 0;
			int num2 = 0;
			Vector2i worldPositionMax = this.WorldPositionMax;
			for (int j = this.WorldPosition.y; j < worldPositionMax.y; j += 3)
			{
				for (int k = this.WorldPosition.x; k < worldPositionMax.x; k += 3)
				{
					num++;
					if (biomeType != this.worldBuilder.GetBiome(k, j))
					{
						this.OverlapsBiomes = true;
					}
					if (this.worldBuilder.data.GetWater(k, j) > 0)
					{
						num2++;
						data.OverlapsWater = true;
					}
				}
			}
			if ((float)num2 / (float)num > 0.9f)
			{
				this.AllIsWater = true;
			}
		}

		// Token: 0x0600B8B8 RID: 47288 RVA: 0x0044B654 File Offset: 0x00449854
		[PublicizedFrom(EAccessModifier.Private)]
		public ref StreetTileData GetData()
		{
			return this.worldBuilder.data.GetStreetTileDataWorld(this.WorldPosition.x, this.WorldPosition.y);
		}

		// Token: 0x0600B8B9 RID: 47289 RVA: 0x0044B67C File Offset: 0x0044987C
		public Stamp GetStamp()
		{
			Color32 customColor = (this.District.type == District.Type.Gateway) ? new Color32(byte.MaxValue, 0, 204, byte.MaxValue) : new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
			int num = this.RoadShape;
			if (num < 0)
			{
				num = 2;
				customColor = Color.black;
			}
			TranslationData transData = new TranslationData(this.WorldPositionCenter.x, this.WorldPositionCenter.y, 1f, this.rotations * -90);
			return new Stamp(this.worldBuilder, this.worldBuilder.StampManager.GetStamp(this.worldBuilder.StreetTileShared.RoadShapes[num], null), transData, customColor, 0.1f, false, "");
		}

		// Token: 0x0600B8BA RID: 47290 RVA: 0x0044B740 File Offset: 0x00449940
		public StreetTile[] GetNeighbors()
		{
			if (this.neighbors4Way == null)
			{
				this.neighbors4Way = new StreetTile[4];
				for (int i = 0; i < 4; i++)
				{
					this.neighbors4Way[i] = this.GetNeighbor(this.worldBuilder.StreetTileShared.dir4way[i]);
				}
			}
			return this.neighbors4Way;
		}

		// Token: 0x0600B8BB RID: 47291 RVA: 0x0044B798 File Offset: 0x00449998
		public int GetNeighborCount()
		{
			if (this.neighbors4Way == null)
			{
				this.GetNeighbors();
			}
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (this.neighbors4Way[i] != null)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600B8BC RID: 47292 RVA: 0x0044B7D4 File Offset: 0x004499D4
		public StreetTile[] GetNeighborsDiagonal()
		{
			if (this.neighborsDiagonal == null)
			{
				this.neighborsDiagonal = new StreetTile[4];
				for (int i = 0; i < 4; i++)
				{
					this.neighborsDiagonal[i] = this.GetNeighbor(this.worldBuilder.StreetTileShared.dirDiagonal[i]);
				}
			}
			return this.neighborsDiagonal;
		}

		// Token: 0x0600B8BD RID: 47293 RVA: 0x0044B82C File Offset: 0x00449A2C
		public StreetTile[] GetNeighbors8Way()
		{
			if (this.neighbors8Way == null)
			{
				this.neighbors8Way = new StreetTile[8];
				for (int i = 0; i < 8; i++)
				{
					this.neighbors8Way[i] = this.GetNeighbor(this.worldBuilder.StreetTileShared.dir8way[i]);
				}
			}
			return this.neighbors8Way;
		}

		// Token: 0x0600B8BE RID: 47294 RVA: 0x0044B883 File Offset: 0x00449A83
		public StreetTile GetNeighbor(Vector2i direction)
		{
			return this.worldBuilder.GetStreetTileGrid(this.GridPosition + direction);
		}

		// Token: 0x0600B8BF RID: 47295 RVA: 0x0044B89C File Offset: 0x00449A9C
		public bool HasNeighbor(StreetTile otherTile)
		{
			StreetTile[] neighbors = this.GetNeighbors();
			for (int i = 0; i < neighbors.Length; i++)
			{
				if (neighbors[i] == otherTile)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B8C0 RID: 47296 RVA: 0x0044B8C7 File Offset: 0x00449AC7
		public StreetTile GetNeighbor(int _dir)
		{
			if (this.neighbors4Way == null)
			{
				this.GetNeighbors();
			}
			return this.neighbors4Way[_dir];
		}

		// Token: 0x0600B8C1 RID: 47297 RVA: 0x0044B8E0 File Offset: 0x00449AE0
		public int GetNeighborDir(StreetTile otherTile)
		{
			if (this.neighbors4Way == null)
			{
				this.GetNeighbors();
			}
			for (int i = 0; i < 4; i++)
			{
				if (this.neighbors4Way[i] == otherTile)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600B8C2 RID: 47298 RVA: 0x0044B918 File Offset: 0x00449B18
		public bool HasExitTo(StreetTile otherTile)
		{
			if (this.Township == null && this.District == null)
			{
				return false;
			}
			if (otherTile.Township == null && otherTile.District == null)
			{
				return false;
			}
			int neighborDir = this.GetNeighborDir(otherTile);
			return neighborDir >= 0 && this.HasRoadExit(neighborDir);
		}

		// Token: 0x0600B8C3 RID: 47299 RVA: 0x0044B960 File Offset: 0x00449B60
		public void SetPathingConstraintsForTile(bool allBlocked = false)
		{
			if (this.Township != null && this.District != null)
			{
				this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
				this.isFullyBlocked = true;
				return;
			}
			if (allBlocked && !this.isFullyBlocked && !this.isPartBlocked)
			{
				this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
				this.isFullyBlocked = true;
				return;
			}
			if (!allBlocked)
			{
				if (this.isFullyBlocked)
				{
					this.worldBuilder.PathingUtils.RemoveFullyBlockedArea(this.Area);
				}
				this.worldBuilder.PathingUtils.AddMoveLimitArea(this.Area);
				this.isPartBlocked = true;
			}
		}

		// Token: 0x0600B8C4 RID: 47300 RVA: 0x0044BA0C File Offset: 0x00449C0C
		[PublicizedFrom(EAccessModifier.Private)]
		public void HighwayExitInit()
		{
			this.highwayExitPositions = new Vector2i[4];
			for (int i = 0; i < 4; i++)
			{
				this.highwayExitPositions[i] = this.HighwayExitFromDir(i);
			}
		}

		// Token: 0x0600B8C5 RID: 47301 RVA: 0x0044BA44 File Offset: 0x00449C44
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i HighwayExitFromDir(int _dir)
		{
			Vector2i result;
			if (_dir == 0)
			{
				result.x = this.WorldPositionCenter.x;
				result.y = this.WorldPositionMax.y - 1;
				return result;
			}
			if (_dir == 1)
			{
				result.x = this.WorldPositionMax.x - 1;
				result.y = this.WorldPositionCenter.y;
				return result;
			}
			if (_dir == 2)
			{
				result.x = this.WorldPositionCenter.x;
				result.y = this.WorldPosition.y;
				return result;
			}
			result.x = this.WorldPosition.x;
			result.y = this.WorldPositionCenter.y;
			return result;
		}

		// Token: 0x0600B8C6 RID: 47302 RVA: 0x0044BAF8 File Offset: 0x00449CF8
		public int GetHighwayExitDir(Vector2i _worldPos)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.GetHighwayExitPos(i) == _worldPos)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B8C7 RID: 47303 RVA: 0x0044BB23 File Offset: 0x00449D23
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2i GetHighwayExitPos(int _dir)
		{
			return this.highwayExitPositions[_dir];
		}

		// Token: 0x0600B8C8 RID: 47304 RVA: 0x0044BB34 File Offset: 0x00449D34
		public List<Vector2i> GetHighwayExits(bool _isGateway = false)
		{
			List<Vector2i> list = new List<Vector2i>();
			if (this.UsedExitList.Count == 1)
			{
				int num = -1;
				for (int i = 0; i < 4; i++)
				{
					if ((this.ConnectedExits & 1 << i) > 0)
					{
						num = (i + 2 & 3);
						break;
					}
				}
				if (num != -1)
				{
					list.Add(this.GetHighwayExitPos(num));
				}
				else
				{
					Log.Error("Could not find opposite highway exit!");
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					if ((this.ConnectedExits & 1 << j) <= 0)
					{
						list.Add(this.GetHighwayExitPos(j));
					}
				}
			}
			return list;
		}

		// Token: 0x0600B8C9 RID: 47305 RVA: 0x0044BBC8 File Offset: 0x00449DC8
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetRoadShape(int _exits)
		{
			int count = this.worldBuilder.StreetTileShared.RoadShapeExitCounts.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (_exits == this.GetRoadExits(i, j))
					{
						this.RoadShape = i;
						this.Rotations = j;
						return;
					}
				}
			}
			this.RoadShape = -1;
			this.Rotations = 0;
		}

		// Token: 0x0600B8CA RID: 47306 RVA: 0x0044BC2C File Offset: 0x00449E2C
		public int CountAllDistrictExits()
		{
			int num = 0;
			this.CalcConnectedDistrictTiles(StreetTile.tempTiles);
			for (int i = 0; i < StreetTile.tempTiles.Count; i++)
			{
				num += StreetTile.tempTiles[i].CountDistrictExits();
			}
			StreetTile.tempTiles.Clear();
			return num;
		}

		// Token: 0x0600B8CB RID: 47307 RVA: 0x0044BC7C File Offset: 0x00449E7C
		public int CountDistrictExitsBetween(StreetTile _neighbor)
		{
			int num = 0;
			this.CalcConnectedDistrictTiles(StreetTile.tempTiles);
			_neighbor.CalcConnectedDistrictTiles(StreetTile.tempTiles2);
			for (int i = 0; i < StreetTile.tempTiles.Count; i++)
			{
				StreetTile streetTile = StreetTile.tempTiles[i];
				for (int j = 0; j < StreetTile.tempTiles2.Count; j++)
				{
					StreetTile otherTile = StreetTile.tempTiles2[j];
					int neighborDir = streetTile.GetNeighborDir(otherTile);
					if (neighborDir >= 0 && (streetTile.ConnectedExits & 1 << neighborDir) > 0)
					{
						num++;
					}
				}
			}
			StreetTile.tempTiles.Clear();
			StreetTile.tempTiles2.Clear();
			return num;
		}

		// Token: 0x0600B8CC RID: 47308 RVA: 0x0044BD1D File Offset: 0x00449F1D
		public void CalcConnectedDistrictTiles(List<StreetTile> _tiles)
		{
			_tiles.Add(this);
			this.CalcConnectedDistrictNeighbors(_tiles);
		}

		// Token: 0x0600B8CD RID: 47309 RVA: 0x0044BD2D File Offset: 0x00449F2D
		public int CalcConnectedDistrictTilesCount(List<StreetTile> _tiles)
		{
			this.CalcConnectedDistrictNeighbors(_tiles);
			int result = 1 + _tiles.Count;
			_tiles.Clear();
			return result;
		}

		// Token: 0x0600B8CE RID: 47310 RVA: 0x0044BD44 File Offset: 0x00449F44
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcConnectedDistrictNeighbors(List<StreetTile> _tiles)
		{
			for (int i = 0; i < 4; i++)
			{
				if ((this.ConnectedExits & 1 << i) > 0)
				{
					StreetTile neighbor = this.GetNeighbor(i);
					if (neighbor != null && neighbor.District == this.District && neighbor.Township == this.Township && !_tiles.Contains(neighbor))
					{
						_tiles.Add(neighbor);
						neighbor.CalcConnectedDistrictNeighbors(_tiles);
					}
				}
			}
		}

		// Token: 0x0600B8CF RID: 47311 RVA: 0x0044BDAC File Offset: 0x00449FAC
		public int CountDistrictExits()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if ((this.ConnectedExits & 1 << i) > 0)
				{
					StreetTile neighbor = this.GetNeighbor(i);
					if (neighbor != null && neighbor.District != this.District && neighbor.Township == this.Township)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600B8D0 RID: 47312 RVA: 0x0044BE04 File Offset: 0x0044A004
		public bool ChangeLConnectionToCap(GameRandom _rand)
		{
			if (this.ConnectedExits != 3 && this.ConnectedExits != 6 && this.ConnectedExits != 12 && this.ConnectedExits != 9)
			{
				return false;
			}
			int num = this.CalcConnectedDistrictTilesCount(StreetTile.tempTiles);
			for (int i = 0; i < 4; i++)
			{
				if ((this.ConnectedExits & 1 << i) > 0)
				{
					StreetTile neighbor = this.GetNeighbor(i);
					if (neighbor != null && neighbor.District == this.District && neighbor.Township == this.Township)
					{
						StreetTile.tempTiles.Add(neighbor);
					}
				}
			}
			bool result = false;
			if (StreetTile.tempTiles.Count == 2)
			{
				int index = _rand.RandomRange(0, StreetTile.tempTiles.Count);
				int neighborDir = this.GetNeighborDir(StreetTile.tempTiles[index]);
				this.ConnectedExits &= ~(1 << neighborDir);
				int num2 = this.CalcConnectedDistrictTilesCount(StreetTile.tempTiles2);
				if (num == num2)
				{
					this.SetExitUnUsedAndFromNeighbor(neighborDir);
					result = true;
				}
				else
				{
					this.ConnectedExits |= 1 << neighborDir;
				}
			}
			StreetTile.tempTiles.Clear();
			return result;
		}

		// Token: 0x0600B8D1 RID: 47313 RVA: 0x0044BF1C File Offset: 0x0044A11C
		public void SetExitsToMyTownship()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				StreetTile neighbor = this.GetNeighbor(i);
				if (neighbor != null && neighbor.Township == this.Township)
				{
					num |= 1 << i;
				}
			}
			this.SetExits(num);
		}

		// Token: 0x0600B8D2 RID: 47314 RVA: 0x0044BF60 File Offset: 0x0044A160
		public int GetExitUsedCount()
		{
			return this.UsedExitList.Count<Vector2i>();
		}

		// Token: 0x0600B8D3 RID: 47315 RVA: 0x0044BF70 File Offset: 0x0044A170
		public void SetExits(int _exits)
		{
			this.ConnectedExits = _exits;
			this.SetRoadShape(_exits);
			this.UsedExitList.Clear();
			for (int i = 0; i < 4; i++)
			{
				if ((_exits & 1 << i) > 0)
				{
					Vector2i highwayExitPos = this.GetHighwayExitPos(i);
					this.UsedExitList.Add(highwayExitPos);
				}
			}
		}

		// Token: 0x0600B8D4 RID: 47316 RVA: 0x0044BFC1 File Offset: 0x0044A1C1
		public void SetExitUsed(int _dir)
		{
			this.ConnectedExits |= 1 << _dir;
			this.SetExits(this.ConnectedExits);
		}

		// Token: 0x0600B8D5 RID: 47317 RVA: 0x0044BFE4 File Offset: 0x0044A1E4
		public bool SetExitUsed(Vector2i exit)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.GetHighwayExitPos(i) == exit)
				{
					this.SetExitUsed(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B8D6 RID: 47318 RVA: 0x0044C016 File Offset: 0x0044A216
		public void SetExitUnUsed(int _dir)
		{
			this.ConnectedExits &= ~(1 << _dir);
			this.SetExits(this.ConnectedExits);
		}

		// Token: 0x0600B8D7 RID: 47319 RVA: 0x0044C038 File Offset: 0x0044A238
		public void SetExitUnUsed(Vector2i _exit)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.GetHighwayExitPos(i) == _exit)
				{
					this.SetExitUnUsed(i);
					return;
				}
			}
		}

		// Token: 0x0600B8D8 RID: 47320 RVA: 0x0044C068 File Offset: 0x0044A268
		public void SetExitUnUsedAndFromNeighbor(int _dir)
		{
			this.SetExitUnUsed(_dir);
			StreetTile neighbor = this.GetNeighbor(_dir);
			if (neighbor == null)
			{
				return;
			}
			neighbor.SetExitUnUsed(_dir + 2 & 3);
		}

		// Token: 0x17001678 RID: 5752
		// (get) Token: 0x0600B8D9 RID: 47321 RVA: 0x0044C087 File Offset: 0x0044A287
		public bool ContainsHighway
		{
			get
			{
				return this.ConnectedHighways.Count > 0;
			}
		}

		// Token: 0x0600B8DA RID: 47322 RVA: 0x0044C097 File Offset: 0x0044A297
		public void AddHighway(Path _path)
		{
			if (!this.ConnectedHighways.Contains(_path))
			{
				this.ConnectedHighways.Add(_path);
				this.GetData().ConnectedHighwayCount = this.ConnectedHighways.Count;
			}
		}

		// Token: 0x0600B8DB RID: 47323 RVA: 0x0044C0C9 File Offset: 0x0044A2C9
		public void RemoveHighway(Path _path)
		{
			if (this.ConnectedHighways.Remove(_path))
			{
				this.GetData().ConnectedHighwayCount = this.ConnectedHighways.Count;
			}
		}

		// Token: 0x0600B8DC RID: 47324 RVA: 0x0044C0F0 File Offset: 0x0044A2F0
		public bool SpawnPrefabs()
		{
			if (this.District == null || this.District.name == "wilderness")
			{
				if (!this.ContainsHighway)
				{
					this.District = DistrictPlannerStatic.Districts["wilderness"];
					if (this.spawnWildernessPrefab())
					{
						return true;
					}
				}
				this.District = null;
				return false;
			}
			string streetPrefabName = string.Format(this.PrefabName, this.District.prefabName);
			this.spawnStreetTile(this.WorldPosition, streetPrefabName, this.Rotations);
			return true;
		}

		// Token: 0x0600B8DD RID: 47325 RVA: 0x0044C178 File Offset: 0x0044A378
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnStreetTile(Vector2i tileMinPositionWorld, string streetPrefabName, int baseRotations)
		{
			bool useExactString = false;
			PrefabData streetTile = this.worldBuilder.PrefabManager.GetStreetTile(streetPrefabName, this.WorldPositionCenter, useExactString);
			if (streetTile == null && string.Format(this.PrefabName, "") != streetPrefabName)
			{
				streetTile = this.worldBuilder.PrefabManager.GetStreetTile(string.Format(this.PrefabName, ""), this.WorldPositionCenter, useExactString);
			}
			if (streetTile == null)
			{
				return false;
			}
			int num = this.Township.Height + streetTile.yOffset;
			if (num < 3)
			{
				return false;
			}
			int num2 = baseRotations + (int)streetTile.rotationToFaceNorth & 3;
			if (num2 == 1)
			{
				num2 = 3;
			}
			else if (num2 == 3)
			{
				num2 = 1;
			}
			Vector3i position = new Vector3i(tileMinPositionWorld.x, num, tileMinPositionWorld.y) + this.worldBuilder.PrefabWorldOffset;
			int num3;
			if (this.worldBuilder.PrefabManager.StreetTilesUsed.TryGetValue(streetTile.Name, out num3))
			{
				this.worldBuilder.PrefabManager.StreetTilesUsed[streetTile.Name] = num3 + 1;
			}
			else
			{
				this.worldBuilder.PrefabManager.StreetTilesUsed.Add(streetTile.Name, 1);
			}
			float totalDensityLeft = 62f;
			float num4;
			if (PrefabManagerStatic.TileMaxDensityScore.TryGetValue(streetTile.Name, out num4))
			{
				totalDensityLeft = num4;
			}
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			int prefabInstanceId = prefabManager.PrefabInstanceId;
			prefabManager.PrefabInstanceId = prefabInstanceId + 1;
			this.AddPrefab(new PrefabDataInstance(prefabInstanceId, position, (byte)num2, streetTile));
			this.SpawnMarkerPartsAndPrefabs(streetTile, new Vector3i(this.WorldPosition.x, num, this.WorldPosition.y), num2, 0, totalDensityLeft);
			return true;
		}

		// Token: 0x0600B8DE RID: 47326 RVA: 0x0044C30F File Offset: 0x0044A50F
		public void SmoothWildernessTerrain()
		{
			this.SmoothTerrainRect(this.WildernessPOIPos, this.WildernessPOISize.x, this.WildernessPOISize.y, this.WildernessPOIHeight, 18);
		}

		// Token: 0x0600B8DF RID: 47327 RVA: 0x0044C33C File Offset: 0x0044A53C
		public void SmoothTownshipTerrain()
		{
			if (this.Township != null && this.District != null)
			{
				this.Township.CalcCenterStreetTile();
				int fadeRange = (this.Township.Streets.Count <= 2) ? 50 : 110;
				this.SmoothTerrainRect(this.WorldPosition, 150, 150, this.Township.Height, fadeRange);
			}
		}

		// Token: 0x0600B8E0 RID: 47328 RVA: 0x0044C3A4 File Offset: 0x0044A5A4
		[PublicizedFrom(EAccessModifier.Private)]
		public void SmoothTerrainRect(Vector2i _startPos, int _sizeX, int _sizeY, int _height, int _fadeRange)
		{
			int num = _fadeRange + 1;
			float num2 = (float)_fadeRange;
			int x = _startPos.x;
			int num3 = _startPos.x + _sizeX;
			int y = _startPos.y;
			int num4 = _startPos.y + _sizeY;
			int num5 = Utils.FastMax(x - num, 1);
			int num6 = Utils.FastMax(y - num, 1);
			int num7 = Utils.FastMin(num3 + num, this.worldBuilder.WorldSize);
			int num8 = Utils.FastMin(num4 + num, this.worldBuilder.WorldSize);
			for (int i = num6; i < num8; i++)
			{
				bool flag = i >= y && i <= num4;
				int y2 = flag ? i : ((i < _startPos.y) ? y : num4);
				for (int j = num5; j < num7; j++)
				{
					bool flag2 = j >= x && j <= num3;
					if (flag2 && flag)
					{
						this.worldBuilder.SetHeightTrusted(j, i, (float)_height);
					}
					else
					{
						int x2 = flag2 ? j : ((j < _startPos.x) ? x : num3);
						float num9 = Mathf.Sqrt((float)this.distanceSqr(j, i, x2, y2)) / num2;
						if (num9 < 1f)
						{
							float height = this.worldBuilder.GetHeight(j, i);
							this.worldBuilder.SetHeightTrusted(j, i, StreetTile.SmoothStep((float)_height, height, (double)num9));
						}
					}
				}
			}
		}

		// Token: 0x0600B8E1 RID: 47329 RVA: 0x0044C50C File Offset: 0x0044A70C
		[PublicizedFrom(EAccessModifier.Private)]
		public void SmoothTerrainCircle(Vector2i _centerPos, int _size, int _height)
		{
			int num = _size / 2;
			float num2 = (float)num * 1.8f;
			int num3 = Mathf.CeilToInt(num2 * num2);
			int num4 = (int)((float)num * 3.2f);
			float num5 = (float)num4 - num2;
			int num6 = Utils.FastMax(_centerPos.x - num4, 1);
			int num7 = Utils.FastMax(_centerPos.y - num4, 1);
			int num8 = Utils.FastMin(_centerPos.x + num4, this.worldBuilder.WorldSize);
			int num9 = Utils.FastMin(_centerPos.y + num4, this.worldBuilder.WorldSize);
			for (int i = num7; i < num9; i++)
			{
				for (int j = num6; j < num8; j++)
				{
					int num10 = this.distanceSqr(j, i, _centerPos.x, _centerPos.y);
					if (num10 <= num3)
					{
						this.worldBuilder.SetHeightTrusted(j, i, (float)_height);
					}
					else
					{
						float num11 = (Mathf.Sqrt((float)num10) - num2) / num5;
						if (num11 < 1f)
						{
							float height = this.worldBuilder.GetHeight(j, i);
							this.worldBuilder.SetHeightTrusted(j, i, StreetTile.SmoothStep((float)_height, height, (double)num11));
						}
					}
				}
			}
		}

		// Token: 0x0600B8E2 RID: 47330 RVA: 0x0044C629 File Offset: 0x0044A829
		[PublicizedFrom(EAccessModifier.Private)]
		public static float SmoothStep(float from, float to, double t)
		{
			t = -2.0 * t * t * t + 3.0 * t * t;
			return (float)((double)to * t + (double)from * (1.0 - t));
		}

		// Token: 0x0600B8E3 RID: 47331 RVA: 0x0044C660 File Offset: 0x0044A860
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnWildernessPrefab()
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + 4096953 + this.GridPosition.x + this.GridPosition.y * 200);
			FastTags<TagGroup.Poi> fastTags = (this.worldBuilder.Towns == WorldBuilder.GenerationSelections.None) ? FastTags<TagGroup.Poi>.none : this.worldBuilder.StreetTileShared.traderTag;
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			FastTags<TagGroup.Poi> withoutTags = fastTags;
			FastTags<TagGroup.Poi> none = FastTags<TagGroup.Poi>.none;
			Vector2i worldPositionCenter = this.WorldPositionCenter;
			PrefabData wildernessPrefab = prefabManager.GetWildernessPrefab(withoutTags, none, default(Vector2i), default(Vector2i), worldPositionCenter, false);
			for (int i = 0; i < 6; i++)
			{
				if (this.spawnWildernessPrefab(wildernessPrefab, gameRandom))
				{
					GameRandomManager.Instance.FreeGameRandom(gameRandom);
					return true;
				}
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return false;
		}

		// Token: 0x0600B8E4 RID: 47332 RVA: 0x0044C734 File Offset: 0x0044A934
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnWildernessPrefab(PrefabData prefab, GameRandom rndm)
		{
			int num = (int)prefab.rotationToFaceNorth + rndm.RandomRange(0, 4) & 3;
			int num2 = prefab.size.x;
			int num3 = prefab.size.z;
			if (num == 1 || num == 3)
			{
				num2 = prefab.size.z;
				num3 = prefab.size.x;
			}
			Vector2i vector2i;
			if (num2 >= 110 || num3 >= 110)
			{
				vector2i = this.WorldPositionCenter - new Vector2i(num2 / 2, num3 / 2);
				if (num2 > 150 || num3 > 150)
				{
					Log.Warning("RWG spawnWildernessPrefab {0}, overflows TileSize {1}", new object[]
					{
						prefab.Name,
						150
					});
				}
			}
			else
			{
				vector2i.x = this.WorldPosition.x + 20 + rndm.RandomRange(110 - num2);
				vector2i.y = this.WorldPosition.y + 20 + rndm.RandomRange(110 - num3);
			}
			if (vector2i.x < 0 || vector2i.x + num2 > this.worldBuilder.WorldSize)
			{
				return false;
			}
			if (vector2i.y < 0 || vector2i.y + num3 > this.worldBuilder.WorldSize)
			{
				return false;
			}
			Vector2i vector2i2;
			vector2i2.x = vector2i.x + num2 / 2;
			vector2i2.y = vector2i.y + num3 / 2;
			Vector2i vector2i3;
			vector2i3.x = vector2i.x + num2 - 1;
			vector2i3.y = vector2i.y + num3 - 1;
			BiomeType biome = this.worldBuilder.GetBiome(vector2i2.x, vector2i2.y);
			int num4 = Mathf.CeilToInt(this.worldBuilder.GetHeight(vector2i2.x, vector2i2.y));
			List<int> list = new List<int>();
			for (int i = vector2i.y; i < vector2i.y + num3; i++)
			{
				for (int j = vector2i.x; j < vector2i.x + num2; j++)
				{
					if (this.worldBuilder.data.GetWater(j, i) > 0)
					{
						return false;
					}
					if (biome != this.worldBuilder.GetBiome(j, i))
					{
						return false;
					}
					int num5 = Mathf.CeilToInt(this.worldBuilder.GetHeight(j, i));
					if (Utils.FastAbsInt(num5 - num4) > 11)
					{
						return false;
					}
					list.Add(num5);
				}
			}
			num4 = this.getMedianHeight(list);
			if (num4 + prefab.yOffset < 2)
			{
				return false;
			}
			int heightCeil = this.getHeightCeil(vector2i2.x, vector2i2.y);
			Vector3i vector3i = new Vector3i(this.subHalfWorld(vector2i.x), heightCeil, this.subHalfWorld(vector2i.y));
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			int prefabInstanceId = prefabManager.PrefabInstanceId;
			prefabManager.PrefabInstanceId = prefabInstanceId + 1;
			int id = prefabInstanceId;
			rndm.SetSeed(vector2i.x + vector2i.x * vector2i.y + vector2i.y);
			if (prefab.POIMarkers != null)
			{
				List<Marker> list2 = prefab.RotatePOIMarkers(true, num);
				for (int k = list2.Count - 1; k >= 0; k--)
				{
					if (list2[k].MarkerType != Marker.MarkerTypes.RoadExit)
					{
						list2.RemoveAt(k);
					}
				}
				if (list2.Count > 0)
				{
					int index = rndm.RandomRange(0, list2.Count);
					float pathRadius = (float)Utils.FastMax(list2[index].size.x, list2[index].size.z) * 0.5f;
					Vector3i startPos = list2[index].startPos;
					Vector2 vector = new Vector2((float)startPos.x + (float)list2[index].size.x / 2f, (float)startPos.z + (float)list2[index].size.z / 2f);
					Vector2 vector2 = new Vector2((float)vector2i.x + vector.x, (float)vector2i.y + vector.y);
					this.worldBuilder.WildernessPlanner.AddPathInfo(new Vector2i(vector2), pathRadius);
				}
			}
			int y = num4 + prefab.yOffset;
			this.SpawnMarkerPartsAndPrefabsWilderness(prefab, new Vector3i(vector2i.x, y, vector2i.y), (int)((byte)num));
			PrefabDataInstance pdi = new PrefabDataInstance(id, new Vector3i(vector3i.x, y, vector3i.z), (byte)num, prefab);
			this.AddPrefab(pdi);
			this.worldBuilder.WildernessPrefabCount++;
			this.WildernessPOIPos = vector2i;
			this.WildernessPOICenter = vector2i2;
			this.WildernessPOISize.x = num2;
			this.WildernessPOISize.y = num3;
			this.WildernessPOIHeight = num4;
			int num6 = vector2i.x / 10;
			int num7 = vector2i3.x / 10;
			int num8 = vector2i.y / 10;
			int num9 = vector2i3.y / 10;
			int num10 = num6 - 2;
			int num11 = num7 + 2;
			int num12 = num8 - 2;
			int num13 = num9 + 2;
			for (int l = num12; l <= num13; l++)
			{
				if (l >= 0 && l < this.worldBuilder.data.PathTileGridWidth)
				{
					for (int m = num10; m <= num11; m++)
					{
						if (m >= 0 && m < this.worldBuilder.data.PathTileGridWidth)
						{
							if (m >= num6 && m <= num7 && l >= num8 && l <= num9)
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, true);
							}
							else if (m == num10 || m == num11 || l == num12 || l == num13)
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, 2);
							}
							else
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, 5);
							}
						}
					}
				}
			}
			num10 = vector2i.x;
			num11 = vector2i3.x;
			num12 = vector2i.y;
			num13 = vector2i3.y;
			for (int n = num12; n <= num13; n += 150)
			{
				for (int num14 = num10; num14 <= num11; num14 += 150)
				{
					StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(num14, n);
					if (streetTileWorld != null)
					{
						streetTileWorld.Used = true;
					}
				}
			}
			return true;
		}

		// Token: 0x0600B8E5 RID: 47333 RVA: 0x0044CD67 File Offset: 0x0044AF67
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddPrefab(PrefabDataInstance pdi)
		{
			this.StreetTilePrefabDatas.Add(pdi);
			if (this.Township != null)
			{
				this.Township.AddPrefab(pdi);
				return;
			}
			this.worldBuilder.PrefabManager.AddUsedPrefabWorld(-1, pdi);
		}

		// Token: 0x17001679 RID: 5753
		// (get) Token: 0x0600B8E6 RID: 47334 RVA: 0x0044CD9C File Offset: 0x0044AF9C
		public bool NeedsWildernessSmoothing
		{
			get
			{
				return this.WildernessPOISize.x > 0;
			}
		}

		// Token: 0x0600B8E7 RID: 47335 RVA: 0x0044CDAC File Offset: 0x0044AFAC
		[PublicizedFrom(EAccessModifier.Private)]
		public int getMedianHeight(List<int> heights)
		{
			heights.Sort();
			int count = heights.Count;
			int num = count / 2;
			if (count % 2 == 0)
			{
				return (heights[num] + heights[num - 1]) / 2;
			}
			return heights[num];
		}

		// Token: 0x0600B8E8 RID: 47336 RVA: 0x0044CDE8 File Offset: 0x0044AFE8
		[PublicizedFrom(EAccessModifier.Private)]
		public int getAverageHeight(List<int> heights)
		{
			int num = 0;
			foreach (int num2 in heights)
			{
				num += num2;
			}
			return num / heights.Count;
		}

		// Token: 0x0600B8E9 RID: 47337 RVA: 0x0044CE40 File Offset: 0x0044B040
		[PublicizedFrom(EAccessModifier.Private)]
		public int getHeightCeil(int x, int y)
		{
			return Mathf.CeilToInt(this.worldBuilder.GetHeight(x, y));
		}

		// Token: 0x0600B8EA RID: 47338 RVA: 0x0044CE54 File Offset: 0x0044B054
		[PublicizedFrom(EAccessModifier.Private)]
		public int subHalfWorld(int pos)
		{
			return pos - this.worldBuilder.WorldSize / 2;
		}

		// Token: 0x0600B8EB RID: 47339 RVA: 0x0044CE68 File Offset: 0x0044B068
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int distanceSqr(Vector2i v1, Vector2i v2)
		{
			int num = v1.x - v2.x;
			int num2 = v1.y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600B8EC RID: 47340 RVA: 0x0044CE98 File Offset: 0x0044B098
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int distanceSqr(int x1, int y1, int x2, int y2)
		{
			int num = x1 - x2;
			int num2 = y1 - y2;
			return num * num + num2 * num2;
		}

		// Token: 0x0600B8ED RID: 47341 RVA: 0x0044CEB4 File Offset: 0x0044B0B4
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float distSqr(Vector2 v1, Vector2 v2)
		{
			float num = v1.x - v2.x;
			float num2 = v1.y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600B8EE RID: 47342 RVA: 0x0044CEE4 File Offset: 0x0044B0E4
		[PublicizedFrom(EAccessModifier.Private)]
		public void SpawnMarkerPartsAndPrefabs(PrefabData _parentPrefab, Vector3i _parentPosition, int _parentRotations, int _depth, float totalDensityLeft)
		{
			List<Marker> list = _parentPrefab.RotatePOIMarkers(true, _parentRotations);
			if (list.Count == 0)
			{
				return;
			}
			FastTags<TagGroup.Poi> fastTags = FastTags<TagGroup.Poi>.Parse(this.District.name);
			this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
			Vector3i size = _parentPrefab.size;
			if (_parentRotations % 2 == 1)
			{
				ref int ptr = ref size.z;
				int x = size.x;
				int num = size.z;
				ptr = x;
				size.x = num;
			}
			List<Marker> list2 = list.FindAll((Marker m) => m.MarkerType == Marker.MarkerTypes.POISpawn);
			if (_depth < 5 && list2.Count > 0)
			{
				list2.Sort((Marker m1, Marker m2) => (m2.size.x + m2.size.y + m2.size.z).CompareTo(m1.size.x + m1.size.y + m1.size.z));
				List<string> list3 = new List<string>();
				for (int i = 0; i < list2.Count; i++)
				{
					if (!list3.Contains(list2[i].GroupName))
					{
						list3.Add(list2[i].GroupName);
					}
				}
				this.Township.rand.SetSeed(this.Township.ID + (_parentPosition.x * _parentPosition.x + _parentPosition.y * _parentPosition.y));
				using (List<string>.Enumerator enumerator = list3.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Marker> list4 = (from m in list2
						where m.GroupName == groupName
						orderby this.Township.rand.RandomFloat descending
						select m).ToList<Marker>();
						int j = 0;
						while (j < list4.Count)
						{
							Marker marker = list4[j];
							Vector2i vector2i = new Vector2i(marker.size.x, marker.size.z);
							Vector2i one = new Vector2i(marker.startPos.x, marker.startPos.z);
							one + vector2i;
							Vector2i vector2i2 = one + vector2i / 2;
							Vector2i vector2i3 = vector2i;
							if (this.District.spawnCustomSizePrefabs)
							{
								int num2;
								if (this.District.name != "gateway" && (num2 = Marker.MarkerSizes.IndexOf(new Vector3i(vector2i.x, 0, vector2i.y))) >= 0)
								{
									if (num2 > 0)
									{
										vector2i3 = new Vector2i(Marker.MarkerSizes[num2 - 1].x + 1, Marker.MarkerSizes[num2 - 1].z + 1);
									}
								}
								else
								{
									vector2i3 = vector2i / 2;
								}
							}
							Vector2i vector2i4 = new Vector2i(_parentPosition.x + vector2i2.x, _parentPosition.z + vector2i2.y);
							if (_depth == 0)
							{
								int halfWorldSize = this.worldBuilder.HalfWorldSize;
								Vector2 a = new Vector2((float)(vector2i4.x - halfWorldSize), (float)(vector2i4.y - halfWorldSize));
								float num3 = 0f;
								List<PrefabDataInstance> prefabs = this.Township.Prefabs;
								for (int k = 0; k < prefabs.Count; k++)
								{
									PrefabDataInstance prefabDataInstance = prefabs[k];
									float densityScore = prefabDataInstance.prefab.DensityScore;
									if (densityScore > 6f)
									{
										Vector2 centerXZV = prefabDataInstance.CenterXZV2;
										if (Vector2.Distance(a, centerXZV) < 190f)
										{
											if (densityScore >= 20f)
											{
												num3 += densityScore * 1.3f;
											}
											else
											{
												num3 += densityScore - 6f;
											}
										}
									}
								}
								if (num3 > 0f)
								{
									totalDensityLeft = Utils.FastMax(6f, totalDensityLeft - num3);
								}
							}
							PrefabData prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, totalDensityLeft, 1f);
							if (prefabWithDistrict != null)
							{
								goto IL_4D8;
							}
							prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, totalDensityLeft + 8f, 0.3f);
							if (prefabWithDistrict != null)
							{
								goto IL_4D8;
							}
							prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, 18f, 0f);
							if (prefabWithDistrict != null)
							{
								Log.Warning("SpawnMarkerPartsAndPrefabs retry2 {0}, tags {1}, size {2} {3}, totalDensityLeft {4}, picked {5}, density {6}", new object[]
								{
									this.District.name,
									marker.Tags,
									vector2i3,
									vector2i,
									totalDensityLeft,
									prefabWithDistrict.Name,
									prefabWithDistrict.DensityScore
								});
								goto IL_4D8;
							}
							Log.Warning("SpawnMarkerPartsAndPrefabs failed {0}, tags {1}, size {2} {3}, totalDensityLeft {4}", new object[]
							{
								this.District.name,
								marker.Tags,
								vector2i3,
								vector2i,
								totalDensityLeft
							});
							IL_8A1:
							j++;
							continue;
							IL_4D8:
							int num4 = _parentPosition.x + marker.startPos.x;
							int num5 = _parentPosition.z + marker.startPos.z;
							if (_parentPosition.y + marker.startPos.y + prefabWithDistrict.yOffset < 3)
							{
								Log.Error("SpawnMarkerPartsAndPrefabs y low! {0}, pos {1} {2}", new object[]
								{
									prefabWithDistrict.Name,
									num4,
									num5
								});
								goto IL_8A1;
							}
							totalDensityLeft -= prefabWithDistrict.DensityScore;
							if (prefabWithDistrict.Tags.Test_AnySet(this.worldBuilder.StreetTileShared.traderTag) || prefabWithDistrict.Name.Contains("trader"))
							{
								Vector2i vector2i5;
								vector2i5.x = num4 + marker.size.x / 2;
								vector2i5.y = num5 + marker.size.z / 2;
								this.worldBuilder.TraderCenterPositions.Add(vector2i5);
								if (this.BiomeType == BiomeType.forest)
								{
									this.worldBuilder.TraderForestCenterPositions.Add(vector2i5);
								}
								this.HasTrader = true;
								Log.Out("Trader {0}, {1}, {2}, at {3}", new object[]
								{
									prefabWithDistrict.Name,
									this.BiomeType,
									this.District.name,
									vector2i5
								});
							}
							int num6 = (int)marker.Rotations;
							byte b = (byte)(_parentRotations + (int)prefabWithDistrict.rotationToFaceNorth + num6 & 3);
							int num7 = prefabWithDistrict.size.x;
							int num8 = prefabWithDistrict.size.z;
							int num;
							if (b == 1 || b == 3)
							{
								int num9 = num7;
								num = num8;
								num8 = num9;
								num7 = num;
							}
							if (num6 == 2)
							{
								num4 += vector2i.x / 2 - num7 / 2;
							}
							else if (num6 == 3)
							{
								num5 += vector2i.y / 2 - num8 / 2;
								num4 += vector2i.x;
								num4 -= num7;
							}
							else if (num6 == 0)
							{
								num4 += vector2i.x / 2 - num7 / 2;
								num5 += vector2i.y;
								num5 -= num8;
							}
							else if (num6 == 1)
							{
								num5 += vector2i.y / 2 - num8 / 2;
							}
							Vector3i position = new Vector3i(num4, _parentPosition.y + marker.startPos.y + prefabWithDistrict.yOffset, num5) + this.worldBuilder.PrefabWorldOffset;
							PrefabManager prefabManager = this.worldBuilder.PrefabManager;
							num = prefabManager.PrefabInstanceId;
							prefabManager.PrefabInstanceId = num + 1;
							PrefabDataInstance prefabDataInstance2 = new PrefabDataInstance(num, position, b, prefabWithDistrict);
							Color preview_color = this.District.preview_color;
							if (prefabDataInstance2.prefab.Name.StartsWith("remnant_") || prefabDataInstance2.prefab.Name.StartsWith("abandoned_"))
							{
								preview_color.r *= 0.75f;
								preview_color.g *= 0.75f;
								preview_color.b *= 0.75f;
							}
							else if (prefabDataInstance2.prefab.DensityScore < 1f)
							{
								preview_color.r *= 0.4f;
								preview_color.g *= 0.4f;
								preview_color.b *= 0.4f;
							}
							else if (prefabDataInstance2.prefab.Name.StartsWith("trader_"))
							{
								preview_color = new Color(0.6f, 0.3f, 0.3f);
							}
							prefabDataInstance2.previewColor = preview_color;
							this.Township.AddPrefab(prefabDataInstance2);
							this.SpawnMarkerPartsAndPrefabs(prefabWithDistrict, new Vector3i(num4, _parentPosition.y + marker.startPos.y + prefabWithDistrict.yOffset, num5), (int)b, _depth + 1, totalDensityLeft);
							break;
						}
					}
				}
			}
			List<Marker> list5 = list.FindAll((Marker m) => m.MarkerType == Marker.MarkerTypes.PartSpawn);
			if (_depth < 20 && list5.Count > 0)
			{
				List<string> list6 = new List<string>();
				for (int l = 0; l < list5.Count; l++)
				{
					if (!list6.Contains(list5[l].GroupName))
					{
						list6.Add(list5[l].GroupName);
					}
				}
				this.Township.rand.SetSeed(this.Township.ID + (_parentPosition.x * _parentPosition.x + _parentPosition.y * _parentPosition.y) + 1);
				using (List<string>.Enumerator enumerator = list6.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Marker> list7 = (from m in list5
						where m.GroupName == groupName
						orderby this.Township.rand.RandomFloat descending
						select m).ToList<Marker>();
						float num10 = 1f;
						if (list7.Count > 1)
						{
							num10 = 0f;
							foreach (Marker marker2 in list7)
							{
								num10 += marker2.PartChanceToSpawn;
							}
						}
						float num11 = 0f;
						using (List<Marker>.Enumerator enumerator2 = list7.GetEnumerator())
						{
							IL_CB4:
							while (enumerator2.MoveNext())
							{
								Marker marker3 = enumerator2.Current;
								num11 += marker3.PartChanceToSpawn / num10;
								if (this.Township.rand.RandomRange(0f, 1f) <= num11)
								{
									if (!marker3.Tags.IsEmpty)
									{
										if (_depth == 0)
										{
											if (!this.District.tag.Test_AnySet(marker3.Tags))
											{
												continue;
											}
										}
										else if (!marker3.Tags.IsEmpty && !fastTags.Test_AnySet(marker3.Tags))
										{
											continue;
										}
									}
									PrefabData prefabByName = this.worldBuilder.PrefabManager.GetPrefabByName(marker3.PartToSpawn);
									if (prefabByName == null)
									{
										Log.Error("Part to spawn {0} not found!", new object[]
										{
											marker3.PartToSpawn
										});
									}
									else
									{
										Vector3i vector3i = new Vector3i(_parentPosition.x + marker3.startPos.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker3.startPos.y, _parentPosition.z + marker3.startPos.z - this.worldBuilder.WorldSize / 2);
										if (vector3i.y > 0)
										{
											byte b2 = marker3.Rotations;
											if (b2 == 1)
											{
												b2 = 3;
											}
											else if (b2 == 3)
											{
												b2 = 1;
											}
											byte b3 = (byte)((_parentRotations + (int)prefabByName.rotationToFaceNorth + (int)b2) % 4);
											Vector3i size2 = prefabByName.size;
											if (b3 == 1 || b3 == 3)
											{
												size2 = new Vector3i(size2.z, size2.y, size2.x);
											}
											Bounds bounds = new Bounds(vector3i + size2 * 0.5f, size2 - Vector3.one);
											foreach (Bounds bounds2 in this.partBounds)
											{
												if (bounds2.Intersects(bounds))
												{
													goto IL_CB4;
												}
											}
											Township township = this.Township;
											PrefabManager prefabManager2 = this.worldBuilder.PrefabManager;
											int num = prefabManager2.PrefabInstanceId;
											prefabManager2.PrefabInstanceId = num + 1;
											township.AddPrefab(new PrefabDataInstance(num, vector3i, b3, prefabByName));
											totalDensityLeft -= prefabByName.DensityScore;
											this.partBounds.Add(bounds);
											this.SpawnMarkerPartsAndPrefabs(prefabByName, _parentPosition + marker3.startPos, (int)b3, _depth + 1, totalDensityLeft);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.District != null && this.District.name == "gateway")
			{
				list5 = list.FindAll((Marker m) => m.PartToSpawn.Contains("highway_transition"));
				if (list5.Count > 0)
				{
					foreach (Marker marker4 in list5)
					{
						Vector2 vector = new Vector2((float)marker4.startPos.x, (float)marker4.startPos.z) - new Vector2((float)(size.x / 2), (float)(size.z / 2));
						if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
						{
							if (vector.x > 0f)
							{
								if (!this.HasExitTo(this.GetNeighbor(Vector2i.right)))
								{
									continue;
								}
								if (this.GetNeighbor(Vector2i.right).Township != this.Township)
								{
									continue;
								}
							}
							else
							{
								if (!this.HasExitTo(this.GetNeighbor(Vector2i.left)))
								{
									continue;
								}
								if (this.GetNeighbor(Vector2i.left).Township != this.Township)
								{
									continue;
								}
							}
						}
						else if (vector.y > 0f)
						{
							if (!this.HasExitTo(this.GetNeighbor(Vector2i.up)))
							{
								continue;
							}
							if (this.GetNeighbor(Vector2i.up).Township != this.Township)
							{
								continue;
							}
						}
						else if (!this.HasExitTo(this.GetNeighbor(Vector2i.down)) || this.GetNeighbor(Vector2i.down).Township != this.Township)
						{
							continue;
						}
						PrefabData prefabByName2 = this.worldBuilder.PrefabManager.GetPrefabByName(marker4.PartToSpawn);
						if (prefabByName2 != null)
						{
							Vector3i vector3i2 = new Vector3i(_parentPosition.x + marker4.startPos.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker4.startPos.y, _parentPosition.z + marker4.startPos.z - this.worldBuilder.WorldSize / 2);
							if (vector3i2.y > 0)
							{
								byte b4 = marker4.Rotations;
								if (b4 == 1)
								{
									b4 = 3;
								}
								else if (b4 == 3)
								{
									b4 = 1;
								}
								byte b5 = (byte)((_parentRotations + (int)prefabByName2.rotationToFaceNorth + (int)b4) % 4);
								Vector3i size3 = prefabByName2.size;
								if (b5 == 1 || b5 == 3)
								{
									size3 = new Vector3i(size3.z, size3.y, size3.x);
								}
								Township township2 = this.Township;
								PrefabManager prefabManager3 = this.worldBuilder.PrefabManager;
								int num = prefabManager3.PrefabInstanceId;
								prefabManager3.PrefabInstanceId = num + 1;
								township2.AddPrefab(new PrefabDataInstance(num, vector3i2, b5, prefabByName2));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B8EF RID: 47343 RVA: 0x0044DF58 File Offset: 0x0044C158
		[PublicizedFrom(EAccessModifier.Private)]
		public void SpawnMarkerPartsAndPrefabsWilderness(PrefabData _parentPrefab, Vector3i _parentPosition, int _parentRotations)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(_parentPosition.ToString().GetHashCode());
			List<Marker> list = _parentPrefab.RotatePOIMarkers(true, _parentRotations);
			List<Marker> list2 = list.FindAll((Marker m) => m.MarkerType == Marker.MarkerTypes.POISpawn);
			if (list2.Count > 0)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					Marker marker = list2[i];
					Vector2i vector2i = new Vector2i(marker.size.x, marker.size.z);
					Vector2i vector2i2 = new Vector2i(marker.startPos.x, marker.startPos.z) + vector2i / 2;
					Vector2i minSize = vector2i;
					PrefabData wildernessPrefab = this.worldBuilder.PrefabManager.GetWildernessPrefab(this.worldBuilder.StreetTileShared.traderTag, marker.Tags, minSize, vector2i, new Vector2i(_parentPosition.x + vector2i2.x, _parentPosition.z + vector2i2.y), false);
					if (wildernessPrefab != null)
					{
						int num = _parentPosition.x + marker.startPos.x;
						int num2 = _parentPosition.z + marker.startPos.z;
						int num3 = (int)marker.Rotations;
						byte b = (byte)(_parentRotations + (int)wildernessPrefab.rotationToFaceNorth + num3 & 3);
						int num4 = wildernessPrefab.size.x;
						int num5 = wildernessPrefab.size.z;
						if (b == 1 || b == 3)
						{
							int num6 = num4;
							num4 = num5;
							num5 = num6;
						}
						if (num3 == 2)
						{
							num += vector2i.x / 2 - num4 / 2;
						}
						else if (num3 == 3)
						{
							num2 += vector2i.y / 2 - num5 / 2;
							num += vector2i.x;
							num -= num4;
						}
						else if (num3 == 0)
						{
							num += vector2i.x / 2 - num4 / 2;
							num2 += vector2i.y;
							num2 -= num5;
						}
						else if (num3 == 1)
						{
							num2 += vector2i.y / 2 - num5 / 2;
						}
						Vector3i position = new Vector3i(num - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker.startPos.y + wildernessPrefab.yOffset, num2 - this.worldBuilder.WorldSize / 2);
						PrefabManager prefabManager = this.worldBuilder.PrefabManager;
						int prefabInstanceId = prefabManager.PrefabInstanceId;
						prefabManager.PrefabInstanceId = prefabInstanceId + 1;
						PrefabDataInstance pdi = new PrefabDataInstance(prefabInstanceId, position, b, wildernessPrefab);
						this.AddPrefab(pdi);
						this.worldBuilder.WildernessPrefabCount++;
						wildernessPrefab.RotatePOIMarkers(true, (int)b);
						this.SpawnMarkerPartsAndPrefabsWilderness(wildernessPrefab, new Vector3i(num, _parentPosition.y + marker.startPos.y + wildernessPrefab.yOffset, num2), (int)b);
					}
				}
			}
			List<Marker> list3 = list.FindAll((Marker m) => m.MarkerType == Marker.MarkerTypes.PartSpawn);
			if (list3.Count > 0)
			{
				List<string> list4 = new List<string>();
				for (int j = 0; j < list3.Count; j++)
				{
					if (!list4.Contains(list3[j].GroupName))
					{
						list4.Add(list3[j].GroupName);
					}
				}
				using (List<string>.Enumerator enumerator = list4.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Marker> list5 = list3.FindAll((Marker m) => m.GroupName == groupName);
						float num7 = 1f;
						if (list5.Count > 1)
						{
							num7 = 0f;
							foreach (Marker marker2 in list5)
							{
								num7 += marker2.PartChanceToSpawn;
							}
						}
						float num8 = 0f;
						foreach (Marker marker3 in list5)
						{
							num8 += marker3.PartChanceToSpawn / num7;
							if (gameRandom.RandomRange(0f, 1f) <= num8 && (marker3.Tags.IsEmpty || this.worldBuilder.StreetTileShared.wildernessTag.Test_AnySet(marker3.Tags)))
							{
								PrefabData prefabByName = this.worldBuilder.PrefabManager.GetPrefabByName(marker3.PartToSpawn);
								if (prefabByName != null)
								{
									Vector3i position2 = new Vector3i(_parentPosition.x + marker3.startPos.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker3.startPos.y, _parentPosition.z + marker3.startPos.z - this.worldBuilder.WorldSize / 2);
									byte b2 = marker3.Rotations;
									if (b2 == 1)
									{
										b2 = 3;
									}
									else if (b2 == 3)
									{
										b2 = 1;
									}
									byte b3 = (byte)((_parentRotations + (int)prefabByName.rotationToFaceNorth + (int)b2) % 4);
									PrefabManager prefabManager2 = this.worldBuilder.PrefabManager;
									int prefabInstanceId = prefabManager2.PrefabInstanceId;
									prefabManager2.PrefabInstanceId = prefabInstanceId + 1;
									PrefabDataInstance pdi2 = new PrefabDataInstance(prefabInstanceId, position2, b3, prefabByName);
									this.AddPrefab(pdi2);
									this.worldBuilder.WildernessPrefabCount++;
									this.SpawnMarkerPartsAndPrefabsWilderness(prefabByName, _parentPosition + marker3.startPos, (int)b3);
									break;
								}
								Log.Error("Part to spawn {0} not found!", new object[]
								{
									marker3.PartToSpawn
								});
							}
						}
					}
				}
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x0600B8F0 RID: 47344 RVA: 0x0044E570 File Offset: 0x0044C770
		public int GetNumTownshipNeighbors()
		{
			int num = 0;
			StreetTile[] neighbors = this.GetNeighbors();
			for (int i = 0; i < neighbors.Length; i++)
			{
				if (neighbors[i].Township == this.Township)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04008A48 RID: 35400
		public const int TileSize = 150;

		// Token: 0x04008A49 RID: 35401
		[PublicizedFrom(EAccessModifier.Private)]
		public const int TileSizeHalf = 75;

		// Token: 0x04008A4A RID: 35402
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityRadius = 190f;

		// Token: 0x04008A4B RID: 35403
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityBase = 6f;

		// Token: 0x04008A4C RID: 35404
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityMid = 20f;

		// Token: 0x04008A4D RID: 35405
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityMidScale = 1.3f;

		// Token: 0x04008A4E RID: 35406
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityBudget = 62f;

		// Token: 0x04008A4F RID: 35407
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityRetry = 18f;

		// Token: 0x04008A50 RID: 35408
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cRadiationEdgeSize = 1;

		// Token: 0x04008A51 RID: 35409
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightDiffMax = 20f;

		// Token: 0x04008A52 RID: 35410
		[PublicizedFrom(EAccessModifier.Private)]
		public const int partDepthLimit = 20;

		// Token: 0x04008A53 RID: 35411
		[PublicizedFrom(EAccessModifier.Private)]
		public const int poiDepthLimit = 5;

		// Token: 0x04008A54 RID: 35412
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTilePadInside = 20;

		// Token: 0x04008A55 RID: 35413
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTileSizeInside = 110;

		// Token: 0x04008A56 RID: 35414
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSmoothFullRadius = 1.8f;

		// Token: 0x04008A57 RID: 35415
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSmoothFadeRadius = 3.2f;

		// Token: 0x04008A58 RID: 35416
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A59 RID: 35417
		public Township Township;

		// Token: 0x04008A5A RID: 35418
		public District District;

		// Token: 0x04008A5B RID: 35419
		public readonly List<PrefabDataInstance> StreetTilePrefabDatas = new List<PrefabDataInstance>();

		// Token: 0x04008A5C RID: 35420
		public readonly Vector2i GridPosition;

		// Token: 0x04008A5D RID: 35421
		public readonly Vector2i WorldPosition;

		// Token: 0x04008A5E RID: 35422
		public readonly Vector2i WorldPositionCenter;

		// Token: 0x04008A5F RID: 35423
		public readonly Rect Area;

		// Token: 0x04008A60 RID: 35424
		public bool OverlapsRadiation;

		// Token: 0x04008A61 RID: 35425
		public bool OverlapsBiomes;

		// Token: 0x04008A62 RID: 35426
		public bool HasSteepSlope;

		// Token: 0x04008A63 RID: 35427
		public bool AllIsWater;

		// Token: 0x04008A64 RID: 35428
		public bool HasTrader;

		// Token: 0x04008A65 RID: 35429
		public bool HasFeature;

		// Token: 0x04008A66 RID: 35430
		public Vector2i WildernessPOIPos;

		// Token: 0x04008A67 RID: 35431
		public Vector2i WildernessPOICenter;

		// Token: 0x04008A68 RID: 35432
		public Vector2i WildernessPOISize;

		// Token: 0x04008A69 RID: 35433
		public int WildernessPOIHeight;

		// Token: 0x04008A6A RID: 35434
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i[] highwayExitPositions;

		// Token: 0x04008A6B RID: 35435
		[PublicizedFrom(EAccessModifier.Private)]
		public int RoadShape;

		// Token: 0x04008A6C RID: 35436
		public int ConnectedExits;

		// Token: 0x04008A6D RID: 35437
		public readonly List<Vector2i> UsedExitList = new List<Vector2i>();

		// Token: 0x04008A6E RID: 35438
		public readonly List<Path> ConnectedHighways = new List<Path>();

		// Token: 0x04008A6F RID: 35439
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Bounds> partBounds = new List<Bounds>();

		// Token: 0x04008A70 RID: 35440
		[PublicizedFrom(EAccessModifier.Private)]
		public int rotations;

		// Token: 0x04008A71 RID: 35441
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile[] neighbors4Way;

		// Token: 0x04008A72 RID: 35442
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile[] neighborsDiagonal;

		// Token: 0x04008A73 RID: 35443
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile[] neighbors8Way;

		// Token: 0x04008A74 RID: 35444
		public bool Used;

		// Token: 0x04008A75 RID: 35445
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isFullyBlocked;

		// Token: 0x04008A76 RID: 35446
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPartBlocked;

		// Token: 0x04008A77 RID: 35447
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<StreetTile> tempTiles = new List<StreetTile>();

		// Token: 0x04008A78 RID: 35448
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<StreetTile> tempTiles2 = new List<StreetTile>();

		// Token: 0x02001735 RID: 5941
		[PublicizedFrom(EAccessModifier.Private)]
		public enum RoadShapeTypes
		{
			// Token: 0x04008A7A RID: 35450
			straight,
			// Token: 0x04008A7B RID: 35451
			t,
			// Token: 0x04008A7C RID: 35452
			intersection,
			// Token: 0x04008A7D RID: 35453
			cap,
			// Token: 0x04008A7E RID: 35454
			corner
		}
	}
}
