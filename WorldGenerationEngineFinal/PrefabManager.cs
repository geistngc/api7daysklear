using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001720 RID: 5920
	public class PrefabManager
	{
		// Token: 0x0600B835 RID: 47157 RVA: 0x00448440 File Offset: 0x00446640
		public PrefabManager(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600B836 RID: 47158 RVA: 0x0044847B File Offset: 0x0044667B
		public void LoadPrefabs()
		{
			this.prefabManagerData.LoadPrefabs();
		}

		// Token: 0x0600B837 RID: 47159 RVA: 0x00448488 File Offset: 0x00446688
		public void ShufflePrefabData(int _seed)
		{
			this.prefabManagerData.ShufflePrefabData(_seed);
		}

		// Token: 0x0600B838 RID: 47160 RVA: 0x00448496 File Offset: 0x00446696
		public void Clear()
		{
			this.StreetTilesUsed.Clear();
		}

		// Token: 0x0600B839 RID: 47161 RVA: 0x004484A3 File Offset: 0x004466A3
		public void ClearDisplayed()
		{
			this.UsedPrefabsWorld.Clear();
			this.WorldUsedPrefabNames.Clear();
		}

		// Token: 0x0600B83A RID: 47162 RVA: 0x004484BB File Offset: 0x004466BB
		public void Cleanup()
		{
			this.prefabManagerData.Cleanup();
			this.ClearDisplayed();
		}

		// Token: 0x0600B83B RID: 47163 RVA: 0x004484D0 File Offset: 0x004466D0
		public static bool isSizeValid(PrefabData prefab, Vector2i minSize, Vector2i maxSize)
		{
			return (maxSize == default(Vector2i) || (prefab.size.x <= maxSize.x && prefab.size.z <= maxSize.y) || (prefab.size.z <= maxSize.x && prefab.size.x <= maxSize.y)) && (minSize == default(Vector2i) || (prefab.size.x >= minSize.x && prefab.size.z >= minSize.y) || (prefab.size.z >= minSize.x && prefab.size.x >= minSize.y));
		}

		// Token: 0x0600B83C RID: 47164 RVA: 0x004485A0 File Offset: 0x004467A0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isThemeValid(PrefabData prefab, Vector2i prefabPos, List<PrefabDataInstance> prefabInstances, int distance)
		{
			if (prefab.ThemeTags.IsEmpty)
			{
				return true;
			}
			prefabPos.x -= this.worldBuilder.WorldSize / 2;
			prefabPos.y -= this.worldBuilder.WorldSize / 2;
			int num = distance * distance;
			foreach (PrefabDataInstance prefabDataInstance in prefabInstances)
			{
				if (!prefabDataInstance.prefab.ThemeTags.IsEmpty && prefabDataInstance.prefab.ThemeTags.Test_AnySet(prefab.ThemeTags) && Vector2i.DistanceSqr(prefabDataInstance.CenterXZ, prefabPos) < (float)num)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B83D RID: 47165 RVA: 0x00448670 File Offset: 0x00446870
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isNameValid(PrefabData prefab, Vector2i prefabPos, List<PrefabDataInstance> prefabInstances, int distance)
		{
			prefabPos.x -= this.worldBuilder.WorldSize / 2;
			prefabPos.y -= this.worldBuilder.WorldSize / 2;
			int num = distance * distance;
			foreach (PrefabDataInstance prefabDataInstance in prefabInstances)
			{
				if (!(prefabDataInstance.prefab.Name != prefab.Name) && Vector2i.DistanceSqr(prefabDataInstance.CenterXZ, prefabPos) < (float)num)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B83E RID: 47166 RVA: 0x0044871C File Offset: 0x0044691C
		public PrefabData GetPrefabWithDistrict(District _district, FastTags<TagGroup.Poi> _markerTags, Vector2i minSize, Vector2i maxSize, Vector2i center, float densityPointsLeft, float _distanceScale)
		{
			bool flag = !_district.tag.IsEmpty;
			bool flag2 = !_markerTags.IsEmpty;
			PrefabData result = null;
			float num = float.MinValue;
			int worldSizeDistDiv = this.worldBuilder.WorldSizeDistDiv;
			for (int i = 0; i < this.prefabManagerData.prefabDataList.Count; i++)
			{
				PrefabData prefabData = this.prefabManagerData.prefabDataList[i];
				if (prefabData.DensityScore <= densityPointsLeft && !prefabData.Tags.Test_AnySet(this.prefabManagerData.PartsAndTilesTags) && (!flag || prefabData.Tags.Test_AllSet(_district.tag)))
				{
					if (flag2)
					{
						if (!prefabData.Tags.Test_AnySet(_markerTags))
						{
							goto IL_149;
						}
					}
					else if (prefabData.Tags.Test_AnySet(this.prefabManagerData.HasTags))
					{
						goto IL_149;
					}
					if (PrefabManager.isSizeValid(prefabData, minSize, maxSize))
					{
						int num2 = prefabData.ThemeRepeatDistance;
						if (prefabData.ThemeTags.Test_AnySet(this.prefabManagerData.TraderTags))
						{
							num2 /= worldSizeDistDiv;
						}
						if (this.isThemeValid(prefabData, center, this.UsedPrefabsWorld, num2) && (_distanceScale <= 0f || this.isNameValid(prefabData, center, this.UsedPrefabsWorld, (int)((float)prefabData.DuplicateRepeatDistance * _distanceScale))))
						{
							float scoreForPrefab = this.getScoreForPrefab(prefabData, center);
							if (scoreForPrefab > num)
							{
								num = scoreForPrefab;
								result = prefabData;
							}
						}
					}
				}
				IL_149:;
			}
			return result;
		}

		// Token: 0x0600B83F RID: 47167 RVA: 0x00448890 File Offset: 0x00446A90
		public PrefabData GetWildernessPrefab(FastTags<TagGroup.Poi> _withoutTags, FastTags<TagGroup.Poi> _markerTags, Vector2i minSize = default(Vector2i), Vector2i maxSize = default(Vector2i), Vector2i center = default(Vector2i), bool _isRetry = false)
		{
			PrefabData prefabData = null;
			float num = float.MinValue;
			for (int i = 0; i < this.prefabManagerData.prefabDataList.Count; i++)
			{
				PrefabData prefabData2 = this.prefabManagerData.prefabDataList[i];
				if (!prefabData2.Tags.Test_AnySet(this.prefabManagerData.PartsAndTilesTags) && (prefabData2.Tags.Test_AnySet(this.prefabManagerData.WildernessTags) || prefabData2.Tags.Test_AnySet(this.prefabManagerData.TraderTags)) && (_markerTags.IsEmpty || prefabData2.Tags.Test_AnySet(_markerTags) || prefabData2.ThemeTags.Test_AnySet(_markerTags)) && PrefabManager.isSizeValid(prefabData2, minSize, maxSize) && this.isThemeValid(prefabData2, center, this.UsedPrefabsWorld, prefabData2.ThemeRepeatDistance) && (_isRetry || this.isNameValid(prefabData2, center, this.UsedPrefabsWorld, prefabData2.DuplicateRepeatDistance)))
				{
					float scoreForPrefab = this.getScoreForPrefab(prefabData2, center);
					if (scoreForPrefab > num)
					{
						num = scoreForPrefab;
						prefabData = prefabData2;
					}
				}
			}
			if (prefabData == null && !_isRetry)
			{
				return this.GetWildernessPrefab(_withoutTags, _markerTags, minSize, maxSize, center, true);
			}
			return prefabData;
		}

		// Token: 0x0600B840 RID: 47168 RVA: 0x004489B4 File Offset: 0x00446BB4
		[PublicizedFrom(EAccessModifier.Private)]
		public static int getRandomVal(int min, int maxExclusive, int seed)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			int result = gameRandom.RandomRange(min, maxExclusive);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return result;
		}

		// Token: 0x0600B841 RID: 47169 RVA: 0x004489E0 File Offset: 0x00446BE0
		[PublicizedFrom(EAccessModifier.Private)]
		public static float getRandomVal(float min, float max, int seed)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			float result = gameRandom.RandomRange(min, max);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return result;
		}

		// Token: 0x0600B842 RID: 47170 RVA: 0x00448A0C File Offset: 0x00446C0C
		public PrefabData GetPrefabByName(string _lowerCaseName)
		{
			PrefabData result;
			if (!this.prefabManagerData.AllPrefabDatas.TryGetValue(_lowerCaseName.ToLower(), out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600B843 RID: 47171 RVA: 0x00448A38 File Offset: 0x00446C38
		public PrefabData GetStreetTile(string _lowerCaseName, Vector2i centerPoint, bool useExactString = false)
		{
			GameRandom rnd = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + (centerPoint.x + centerPoint.x * centerPoint.y * centerPoint.y));
			string text = this.prefabManagerData.AllPrefabDatas.Keys.Where(delegate(string c)
			{
				Vector2i vector2i;
				int num;
				return ((useExactString && c.Equals(_lowerCaseName)) || (!useExactString && c.StartsWith(_lowerCaseName))) && (!PrefabManagerStatic.TileMinMaxCounts.TryGetValue(c, out vector2i) || !this.StreetTilesUsed.TryGetValue(c, out num) || num < vector2i.y);
			}).OrderByDescending(delegate(string c)
			{
				Vector2i vector2i;
				return (float)(PrefabManagerStatic.TileMinMaxCounts.TryGetValue(c, out vector2i) ? vector2i.x : 0) + rnd.RandomRange(0f, 1f);
			}).FirstOrDefault<string>();
			GameRandomManager.Instance.FreeGameRandom(rnd);
			if (text == null)
			{
				Log.Warning("Tile starting with " + _lowerCaseName + " not found!");
				return null;
			}
			return this.prefabManagerData.AllPrefabDatas[text];
		}

		// Token: 0x0600B844 RID: 47172 RVA: 0x00448B10 File Offset: 0x00446D10
		public bool SavePrefabData(Stream _stream)
		{
			bool result;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.CreateXmlDeclaration();
				XmlElement node = xmlDocument.AddXmlElement("prefabs");
				for (int i = 0; i < this.UsedPrefabsWorld.Count; i++)
				{
					PrefabDataInstance prefabDataInstance = this.UsedPrefabsWorld[i];
					if (prefabDataInstance != null)
					{
						string value = "";
						if (prefabDataInstance.prefab != null && prefabDataInstance.prefab.location.Type != PathAbstractions.EAbstractedLocationType.None)
						{
							value = prefabDataInstance.prefab.location.Name;
						}
						else if (prefabDataInstance.location.Type != PathAbstractions.EAbstractedLocationType.None)
						{
							value = prefabDataInstance.location.Name;
						}
						node.AddXmlElement("decoration").SetAttrib("type", "model").SetAttrib("name", value).SetAttrib("position", prefabDataInstance.boundingBoxPosition.ToStringNoBlanks()).SetAttrib("rotation", prefabDataInstance.rotation.ToString());
					}
				}
				xmlDocument.Save(_stream);
				result = true;
			}
			catch (Exception e)
			{
				Log.Exception(e);
				result = false;
			}
			return result;
		}

		// Token: 0x0600B845 RID: 47173 RVA: 0x00448C30 File Offset: 0x00446E30
		public void GetPrefabsAround(Vector3 _position, float _distance, Dictionary<int, PrefabDataInstance> _prefabs)
		{
			for (int i = 0; i < this.UsedPrefabsWorld.Count; i++)
			{
				PrefabDataInstance prefabDataInstance = this.UsedPrefabsWorld[i];
				_prefabs[this.UsedPrefabsWorld[i].id] = this.UsedPrefabsWorld[i];
			}
		}

		// Token: 0x0600B846 RID: 47174 RVA: 0x00448C84 File Offset: 0x00446E84
		[PublicizedFrom(EAccessModifier.Private)]
		public float getScoreForPrefab(PrefabData prefab, Vector2i center)
		{
			float num = 1f;
			float num2 = 1f;
			FastTags<TagGroup.Poi> other = FastTags<TagGroup.Poi>.Parse(this.worldBuilder.GetBiome(center).ToString());
			PrefabManager.POIWeightData poiweightData = null;
			for (int i = 0; i < PrefabManagerStatic.prefabWeightData.Count; i++)
			{
				PrefabManager.POIWeightData poiweightData2 = PrefabManagerStatic.prefabWeightData[i];
				bool flag = poiweightData2.PartialPOIName.Length > 0 && prefab.Name.Contains(poiweightData2.PartialPOIName, StringComparison.OrdinalIgnoreCase);
				if (flag && !poiweightData2.BiomeTags.IsEmpty && !poiweightData2.BiomeTags.Test_AnySet(other))
				{
					return float.MinValue;
				}
				if (flag || (!poiweightData2.Tags.IsEmpty && ((!prefab.Tags.IsEmpty && prefab.Tags.Test_AnySet(poiweightData2.Tags)) || (!prefab.ThemeTags.IsEmpty && prefab.ThemeTags.Test_AnySet(poiweightData2.Tags)))))
				{
					poiweightData = poiweightData2;
					break;
				}
			}
			if (poiweightData != null)
			{
				num2 = poiweightData.Weight;
				num += poiweightData.Bias;
				int num4;
				int num3 = this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num4) ? num4 : 0;
				if (num3 < poiweightData.MinCount)
				{
					num += (float)(poiweightData.MinCount - num3);
				}
				int num5;
				if (this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num5) && num5 >= poiweightData.MaxCount)
				{
					num2 = 0f;
				}
			}
			num += (float)prefab.DifficultyTier / 5f;
			int num6;
			if (this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num6))
			{
				num /= (float)num6 + 1f;
			}
			return num * num2;
		}

		// Token: 0x0600B847 RID: 47175 RVA: 0x00448E3C File Offset: 0x0044703C
		public void AddUsedPrefab(string prefabName)
		{
			int num;
			if (this.WorldUsedPrefabNames.TryGetValue(prefabName, out num))
			{
				this.WorldUsedPrefabNames[prefabName] = num + 1;
				return;
			}
			this.WorldUsedPrefabNames.Add(prefabName, 1);
		}

		// Token: 0x0600B848 RID: 47176 RVA: 0x00448E76 File Offset: 0x00447076
		public void AddUsedPrefabWorld(int townshipID, PrefabDataInstance pdi)
		{
			this.UsedPrefabsWorld.Add(pdi);
			this.AddUsedPrefab(pdi.prefab.Name);
		}

		// Token: 0x040089F3 RID: 35315
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x040089F4 RID: 35316
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly PrefabManagerData prefabManagerData = new PrefabManagerData();

		// Token: 0x040089F5 RID: 35317
		public readonly Dictionary<string, int> StreetTilesUsed = new Dictionary<string, int>();

		// Token: 0x040089F6 RID: 35318
		public readonly List<PrefabDataInstance> UsedPrefabsWorld = new List<PrefabDataInstance>();

		// Token: 0x040089F7 RID: 35319
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, int> WorldUsedPrefabNames = new Dictionary<string, int>();

		// Token: 0x040089F8 RID: 35320
		public int PrefabInstanceId;

		// Token: 0x02001721 RID: 5921
		public class POIWeightData
		{
			// Token: 0x0600B849 RID: 47177 RVA: 0x00448E98 File Offset: 0x00447098
			public POIWeightData(string _partialPOIName, FastTags<TagGroup.Poi> _tags, FastTags<TagGroup.Poi> _biomeTags, float _weight, float _bias, int minCount, int maxCount)
			{
				this.PartialPOIName = _partialPOIName.ToLower();
				this.Tags = _tags;
				this.BiomeTags = _biomeTags;
				this.Weight = _weight;
				this.Bias = _bias;
				this.MinCount = minCount;
				this.MaxCount = maxCount;
			}

			// Token: 0x040089F9 RID: 35321
			public string PartialPOIName;

			// Token: 0x040089FA RID: 35322
			public FastTags<TagGroup.Poi> Tags;

			// Token: 0x040089FB RID: 35323
			public FastTags<TagGroup.Poi> BiomeTags;

			// Token: 0x040089FC RID: 35324
			public float Weight;

			// Token: 0x040089FD RID: 35325
			public float Bias;

			// Token: 0x040089FE RID: 35326
			public int MinCount;

			// Token: 0x040089FF RID: 35327
			public int MaxCount;
		}
	}
}
