using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000BF7 RID: 3063
public class BiomeDefinition
{
	// Token: 0x06005D88 RID: 23944 RVA: 0x00245AB6 File Offset: 0x00243CB6
	public static string LocalizedBiomeName(BiomeDefinition.BiomeType _biomeType)
	{
		return Localization.Get("biome_" + _biomeType.ToStringCached<BiomeDefinition.BiomeType>(), false, null);
	}

	// Token: 0x06005D89 RID: 23945 RVA: 0x00245AD0 File Offset: 0x00243CD0
	public BiomeDefinition(byte _id, byte _subId, string _name, uint _color, int _radiationLevel, string _buff)
	{
		this.m_Id = _id;
		this.m_BiomeType = (BiomeDefinition.BiomeType)(Enum.IsDefined(typeof(BiomeDefinition.BiomeType), (int)this.m_Id) ? this.m_Id : 0);
		this.subId = _subId;
		this.m_sBiomeName = _name;
		this.LocalizedName = Localization.Get("biome_" + _name, false, null);
		this.m_SpectrumName = this.m_sBiomeName;
		this.m_uiColor = _color;
		this.m_RadiationLevel = _radiationLevel;
		this.m_Layers = new List<BiomeLayer>();
		this.m_DecoBlocks = new List<BiomeBlockDecoration>();
		this.m_DistantDecoBlocks = new List<BiomeBlockDecoration>();
		this.m_DecoPrefabs = new List<BiomePrefabDecoration>();
		this.m_DecoBluffs = new List<BiomeBluffDecoration>();
		this.Buff = _buff;
		this.InitWeather();
	}

	// Token: 0x06005D8A RID: 23946 RVA: 0x00245BFF File Offset: 0x00243DFF
	public void AddLayer(BiomeLayer _layer)
	{
		this.m_Layers.Add(_layer);
		this.TotalLayerDepth += _layer.m_Depth;
	}

	// Token: 0x06005D8B RID: 23947 RVA: 0x00245C20 File Offset: 0x00243E20
	public void AddDecoBlock(BiomeBlockDecoration _deco)
	{
		this.m_DecoBlocks.Add(_deco);
		if (Block.BlocksLoaded)
		{
			Block block = _deco.blockValues[0].Block;
			if (block != null && block.IsDistantDecoration)
			{
				this.m_DistantDecoBlocks.Add(_deco);
			}
		}
	}

	// Token: 0x06005D8C RID: 23948 RVA: 0x00245C69 File Offset: 0x00243E69
	public void AddDecoPrefab(BiomePrefabDecoration _deco)
	{
		this.m_DecoPrefabs.Add(_deco);
	}

	// Token: 0x06005D8D RID: 23949 RVA: 0x00245C77 File Offset: 0x00243E77
	public void AddBluff(BiomeBluffDecoration _deco)
	{
		this.m_DecoBluffs.Add(_deco);
	}

	// Token: 0x06005D8E RID: 23950 RVA: 0x00245C85 File Offset: 0x00243E85
	public void AddReplacement(int _sourceId, int _targetId)
	{
		this.Replacements[_sourceId] = _targetId;
	}

	// Token: 0x06005D8F RID: 23951 RVA: 0x00245C94 File Offset: 0x00243E94
	public void addSubBiome(BiomeDefinition _subbiome)
	{
		this.subbiomes.Add(_subbiome);
	}

	// Token: 0x06005D90 RID: 23952 RVA: 0x00245CA2 File Offset: 0x00243EA2
	public override bool Equals(object obj)
	{
		return obj is BiomeDefinition && ((BiomeDefinition)obj).m_Id == this.m_Id;
	}

	// Token: 0x06005D91 RID: 23953 RVA: 0x00245CC1 File Offset: 0x00243EC1
	public override int GetHashCode()
	{
		return (int)this.m_Id;
	}

	// Token: 0x06005D92 RID: 23954 RVA: 0x00245CC9 File Offset: 0x00243EC9
	public override string ToString()
	{
		return this.m_sBiomeName;
	}

	// Token: 0x06005D93 RID: 23955 RVA: 0x00245CD1 File Offset: 0x00243ED1
	public static uint GetBiomeColor(BiomeDefinition.BiomeType _type)
	{
		return BiomeDefinition.BiomeColors[(int)_type];
	}

	// Token: 0x06005D94 RID: 23956 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitWeather()
	{
	}

	// Token: 0x06005D95 RID: 23957 RVA: 0x00245CDC File Offset: 0x00243EDC
	public BiomeDefinition.WeatherGroup AddWeatherGroup(string _name, float _prob, float _duration, Vector2 _delay, string _buffName)
	{
		BiomeDefinition.WeatherGroup weatherGroup = new BiomeDefinition.WeatherGroup();
		weatherGroup.name = _name;
		weatherGroup.stormLevel = ((!_name.StartsWith("storm")) ? 0 : (_name.Contains("build") ? 1 : 2));
		weatherGroup.prob = _prob;
		weatherGroup.duration = (int)(_duration * 1000f);
		weatherGroup.delay.x = (int)(_delay.x * 1000f);
		weatherGroup.delay.y = (int)(_delay.y * 1000f);
		if (!string.IsNullOrEmpty(_buffName))
		{
			weatherGroup.buffName = _buffName;
		}
		this.weatherGroups.Add(weatherGroup);
		return weatherGroup;
	}

	// Token: 0x06005D96 RID: 23958 RVA: 0x00245D84 File Offset: 0x00243F84
	public void SetupWeather()
	{
		float num = 0f;
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			num += weatherGroup.prob;
			weatherGroup.probabilities.Normalize();
		}
		num += 1E-06f;
		for (int j = 0; j < this.weatherGroups.Count; j++)
		{
			this.weatherGroups[j].prob /= num;
		}
	}

	// Token: 0x06005D97 RID: 23959 RVA: 0x00245E05 File Offset: 0x00244005
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float WeatherGetValue(BiomeDefinition.Probabilities.ProbType _type)
	{
		return this.weatherValues[(int)_type];
	}

	// Token: 0x06005D98 RID: 23960 RVA: 0x00245E0F File Offset: 0x0024400F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WeatherSetValue(BiomeDefinition.Probabilities.ProbType _type, float _value)
	{
		this.weatherValues[(int)_type] = _value;
	}

	// Token: 0x06005D99 RID: 23961 RVA: 0x00245E1C File Offset: 0x0024401C
	public void WeatherRandomize(float _rand)
	{
		float num = 0f;
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			num += weatherGroup.prob;
			if (_rand < num)
			{
				this.SelectWeatherGroup(i);
				return;
			}
		}
	}

	// Token: 0x06005D9A RID: 23962 RVA: 0x00245E68 File Offset: 0x00244068
	public bool WeatherRandomize(string _weatherGroup)
	{
		int num = this.FindWeatherGroupIndex(_weatherGroup);
		if (num >= 0)
		{
			this.SelectWeatherGroup(num);
			return true;
		}
		return false;
	}

	// Token: 0x06005D9B RID: 23963 RVA: 0x00245E8C File Offset: 0x0024408C
	public int WeatherGetDuration(string _weatherGroup)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.FindWeatherGroup(_weatherGroup);
		if (weatherGroup != null)
		{
			return weatherGroup.duration;
		}
		return 0;
	}

	// Token: 0x06005D9C RID: 23964 RVA: 0x00245EAC File Offset: 0x002440AC
	public int WeatherGetDuration(string _weatherGroup, out Vector2i _delay)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.FindWeatherGroup(_weatherGroup);
		if (weatherGroup != null)
		{
			_delay = weatherGroup.delay;
			return weatherGroup.duration;
		}
		_delay = Vector2i.zero;
		return 0;
	}

	// Token: 0x06005D9D RID: 23965 RVA: 0x00245EE4 File Offset: 0x002440E4
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition.WeatherGroup FindWeatherGroup(string _name)
	{
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			if (weatherGroup.name == _name)
			{
				return weatherGroup;
			}
		}
		return null;
	}

	// Token: 0x06005D9E RID: 23966 RVA: 0x00245F28 File Offset: 0x00244128
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindWeatherGroupIndex(string _name)
	{
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			if (this.weatherGroups[i].name == _name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06005D9F RID: 23967 RVA: 0x00245F68 File Offset: 0x00244168
	[PublicizedFrom(EAccessModifier.Private)]
	public void SelectWeatherGroup(int _index)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[_index];
		this.weatherName = weatherGroup.name;
		this.weatherSpectrum = weatherGroup.spectrum;
		this.currentWeatherGroupIndex = _index;
		this.currentWeatherGroup = weatherGroup;
		for (int i = 0; i < 5; i++)
		{
			float randomValue = weatherGroup.probabilities.GetRandomValue((BiomeDefinition.Probabilities.ProbType)i);
			this.weatherValues[i] = randomValue;
		}
	}

	// Token: 0x06005DA0 RID: 23968 RVA: 0x00245FCC File Offset: 0x002441CC
	public void SetWeatherGroup(int _index)
	{
		this.currentWeatherGroupIndex = _index;
		BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[_index];
		this.currentWeatherGroup = weatherGroup;
		this.weatherSpectrum = weatherGroup.spectrum;
	}

	// Token: 0x0400482E RID: 18478
	public float currentPlayerIntensity;

	// Token: 0x0400482F RID: 18479
	public const string BiomeNameLocalizationPrefix = "biome_";

	// Token: 0x04004830 RID: 18480
	public static string[] BiomeNames = new string[]
	{
		"any",
		"snow",
		"forest",
		"pine_forest",
		"plains",
		"desert",
		"water",
		"radiated",
		"wasteland",
		"burnt_forest",
		"city",
		"city_wasteland",
		"wasteland_hub",
		"caveFloor",
		"caveCeiling"
	};

	// Token: 0x04004831 RID: 18481
	public static uint[] BiomeColors = new uint[]
	{
		0U,
		16777215U,
		0U,
		16384U,
		0U,
		16770167U,
		25599U,
		0U,
		16754688U,
		12189951U,
		8421504U,
		12632256U,
		10526880U,
		0U,
		0U
	};

	// Token: 0x04004832 RID: 18482
	public readonly byte m_Id;

	// Token: 0x04004833 RID: 18483
	public readonly BiomeDefinition.BiomeType m_BiomeType;

	// Token: 0x04004834 RID: 18484
	public byte subId;

	// Token: 0x04004835 RID: 18485
	public readonly string m_sBiomeName;

	// Token: 0x04004836 RID: 18486
	public readonly string LocalizedName;

	// Token: 0x04004837 RID: 18487
	public uint m_uiColor;

	// Token: 0x04004838 RID: 18488
	public string m_SpectrumName;

	// Token: 0x04004839 RID: 18489
	public static Dictionary<string, byte> nameToId;

	// Token: 0x0400483A RID: 18490
	public int m_RadiationLevel;

	// Token: 0x0400483B RID: 18491
	public int Difficulty = 1;

	// Token: 0x0400483C RID: 18492
	public List<BiomeLayer> m_Layers;

	// Token: 0x0400483D RID: 18493
	public List<BiomeBlockDecoration> m_DecoBlocks;

	// Token: 0x0400483E RID: 18494
	public List<BiomeBlockDecoration> m_DistantDecoBlocks;

	// Token: 0x0400483F RID: 18495
	public List<BiomePrefabDecoration> m_DecoPrefabs;

	// Token: 0x04004840 RID: 18496
	public List<BiomeBluffDecoration> m_DecoBluffs;

	// Token: 0x04004841 RID: 18497
	public List<BiomeDefinition.WeatherGroup> weatherGroups = new List<BiomeDefinition.WeatherGroup>();

	// Token: 0x04004842 RID: 18498
	public string weatherName;

	// Token: 0x04004843 RID: 18499
	public int currentWeatherGroupIndex;

	// Token: 0x04004844 RID: 18500
	public BiomeDefinition.WeatherGroup currentWeatherGroup;

	// Token: 0x04004845 RID: 18501
	public SpectrumWeatherType weatherSpectrum;

	// Token: 0x04004846 RID: 18502
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] weatherValues = new float[5];

	// Token: 0x04004847 RID: 18503
	public int TotalLayerDepth;

	// Token: 0x04004848 RID: 18504
	public List<BiomeDefinition> subbiomes = new List<BiomeDefinition>();

	// Token: 0x04004849 RID: 18505
	public float noiseFreq = 0.03f;

	// Token: 0x0400484A RID: 18506
	public float noiseMin = 0.2f;

	// Token: 0x0400484B RID: 18507
	public float noiseMax = 1f;

	// Token: 0x0400484C RID: 18508
	public Vector2 noiseOffset;

	// Token: 0x0400484D RID: 18509
	public Dictionary<int, int> Replacements = new Dictionary<int, int>();

	// Token: 0x0400484E RID: 18510
	public float GameStageMod;

	// Token: 0x0400484F RID: 18511
	public float GameStageBonus;

	// Token: 0x04004850 RID: 18512
	public float LootStageMod;

	// Token: 0x04004851 RID: 18513
	public float LootStageBonus;

	// Token: 0x04004852 RID: 18514
	public int LootStageMin = -1;

	// Token: 0x04004853 RID: 18515
	public int LootStageMax = -1;

	// Token: 0x04004854 RID: 18516
	public string Buff;

	// Token: 0x02000BF8 RID: 3064
	public enum BiomeType
	{
		// Token: 0x04004856 RID: 18518
		Any,
		// Token: 0x04004857 RID: 18519
		Snow,
		// Token: 0x04004858 RID: 18520
		Forest,
		// Token: 0x04004859 RID: 18521
		PineForest,
		// Token: 0x0400485A RID: 18522
		Plains,
		// Token: 0x0400485B RID: 18523
		Desert,
		// Token: 0x0400485C RID: 18524
		Water,
		// Token: 0x0400485D RID: 18525
		Radiated,
		// Token: 0x0400485E RID: 18526
		Wasteland,
		// Token: 0x0400485F RID: 18527
		burnt_forest,
		// Token: 0x04004860 RID: 18528
		city,
		// Token: 0x04004861 RID: 18529
		city_wasteland,
		// Token: 0x04004862 RID: 18530
		wasteland_hub,
		// Token: 0x04004863 RID: 18531
		caveFloor,
		// Token: 0x04004864 RID: 18532
		caveCeiling
	}

	// Token: 0x02000BF9 RID: 3065
	public class Probabilities
	{
		// Token: 0x06005DA2 RID: 23970 RVA: 0x002460B0 File Offset: 0x002442B0
		public Probabilities()
		{
			this.probabilities = new List<Vector3>[5];
			for (int i = 0; i < 5; i++)
			{
				this.probabilities[i] = new List<Vector3>();
			}
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x002460E8 File Offset: 0x002442E8
		public void AddProbability(BiomeDefinition.Probabilities.ProbType _type, Vector2 _range, float _probability)
		{
			this.probabilities[(int)_type].Add(new Vector3(_range.x, _range.y, _probability));
		}

		// Token: 0x06005DA4 RID: 23972 RVA: 0x00246118 File Offset: 0x00244318
		public Vector2 CalcMinMaxPossibleValue(BiomeDefinition.Probabilities.ProbType type)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MinValue);
			List<Vector3> list = this.probabilities[(int)type];
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector2 = list[i];
				if (vector2.x < vector.x)
				{
					vector.x = vector2.x;
				}
				if (vector2.y > vector.y)
				{
					vector.y = vector2.y;
				}
			}
			return vector;
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x00246190 File Offset: 0x00244390
		public float GetRandomValue(BiomeDefinition.Probabilities.ProbType _type)
		{
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			float randomFloat = gameRandom.RandomFloat;
			float num = 0f;
			List<Vector3> list = this.probabilities[(int)_type];
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector = list[i];
				num += vector.z;
				if (randomFloat < num)
				{
					float randomFloat2 = gameRandom.RandomFloat;
					return vector.x * randomFloat2 + vector.y * (1f - randomFloat2);
				}
			}
			return 0f;
		}

		// Token: 0x06005DA6 RID: 23974 RVA: 0x0024621C File Offset: 0x0024441C
		public void Normalize()
		{
			for (int i = 0; i < 5; i++)
			{
				List<Vector3> list = this.probabilities[i];
				float num = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					num += list[j].z;
				}
				for (int k = 0; k < list.Count; k++)
				{
					Vector3 value = list[k];
					value.z /= num;
					list[k] = value;
				}
			}
		}

		// Token: 0x04004865 RID: 18533
		public const int ProbTypeCount = 5;

		// Token: 0x04004866 RID: 18534
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector3>[] probabilities;

		// Token: 0x02000BFA RID: 3066
		public enum ProbType
		{
			// Token: 0x04004868 RID: 18536
			Temperature,
			// Token: 0x04004869 RID: 18537
			Precipitation,
			// Token: 0x0400486A RID: 18538
			CloudThickness,
			// Token: 0x0400486B RID: 18539
			Wind,
			// Token: 0x0400486C RID: 18540
			Fog,
			// Token: 0x0400486D RID: 18541
			Count
		}
	}

	// Token: 0x02000BFB RID: 3067
	public class WeatherGroup
	{
		// Token: 0x06005DA7 RID: 23975 RVA: 0x00246299 File Offset: 0x00244499
		public void AddProbability(BiomeDefinition.Probabilities.ProbType _type, Vector2 _range, float _probability)
		{
			this.probabilities.AddProbability(_type, _range, _probability);
		}

		// Token: 0x0400486E RID: 18542
		public string name;

		// Token: 0x0400486F RID: 18543
		public int stormLevel;

		// Token: 0x04004870 RID: 18544
		public float prob;

		// Token: 0x04004871 RID: 18545
		public int duration;

		// Token: 0x04004872 RID: 18546
		public Vector2i delay;

		// Token: 0x04004873 RID: 18547
		public string buffName;

		// Token: 0x04004874 RID: 18548
		public SpectrumWeatherType spectrum;

		// Token: 0x04004875 RID: 18549
		public BiomeDefinition.Probabilities probabilities = new BiomeDefinition.Probabilities();
	}
}
