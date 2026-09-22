using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001329 RID: 4905
public class BiomeParticleManager
{
	// Token: 0x06009B0C RID: 39692 RVA: 0x003A8F48 File Offset: 0x003A7148
	public static void RegisterEffect(string biomeName, string prefabName, float chunkMargin)
	{
		if (GameManager.IsDedicatedServer || BiomeParticleManager.RegistrationCompleted)
		{
			return;
		}
		if (BiomeParticleManager.effects == null)
		{
			BiomeParticleManager.effects = new Dictionary<string, DictionaryList<string, BiomeParticleManager.ParticleEffectData>>();
		}
		BiomeParticleManager.ParticleEffectData value;
		value.biomeName = biomeName;
		value.prefabName = prefabName;
		value.chunkMargin = chunkMargin + 1f;
		DataLoader.PreloadBundle(prefabName);
		DictionaryList<string, BiomeParticleManager.ParticleEffectData> dictionaryList;
		if (!BiomeParticleManager.effects.TryGetValue(biomeName, out dictionaryList))
		{
			dictionaryList = new DictionaryList<string, BiomeParticleManager.ParticleEffectData>();
			BiomeParticleManager.effects.Add(biomeName, dictionaryList);
		}
		dictionaryList.Add(prefabName, value);
	}

	// Token: 0x06009B0D RID: 39693 RVA: 0x003A8FC4 File Offset: 0x003A71C4
	public static List<GameObject> SpawnParticles(Chunk chunk, Transform _parent)
	{
		if (GameManager.IsDedicatedServer)
		{
			return null;
		}
		Vector3i worldPos = chunk.GetWorldPos();
		BiomeDefinition biome = GameManager.Instance.World.GetBiome(worldPos.x, worldPos.z);
		if (biome == null)
		{
			return null;
		}
		string sBiomeName = biome.m_sBiomeName;
		DictionaryList<string, BiomeParticleManager.ParticleEffectData> dictionaryList;
		if (!BiomeParticleManager.effects.TryGetValue(sBiomeName, out dictionaryList))
		{
			return null;
		}
		if (dictionaryList.list.Count == 0)
		{
			return null;
		}
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
		if (@float <= 0.04f)
		{
			return null;
		}
		int height = (int)GameManager.Instance.World.GetHeight(worldPos.x, worldPos.z);
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < dictionaryList.list.Count; i++)
		{
			BiomeParticleManager.ParticleEffectData particleEffectData = dictionaryList.list[i];
			if ((float)chunk.X % particleEffectData.chunkMargin == 0f && (float)chunk.Z % particleEffectData.chunkMargin == 0f)
			{
				GameObject gameObject = DataLoader.LoadAsset<GameObject>(particleEffectData.prefabName, false);
				if (gameObject)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					Vector3 vector;
					vector.x = (float)worldPos.x;
					vector.y = (float)height;
					vector.z = (float)worldPos.z;
					gameObject2.name = gameObject2.name + "_ (" + vector.ToCultureInvariantString() + ")";
					Transform transform = gameObject2.transform;
					transform.SetParent(_parent, false);
					transform.position = vector - Origin.position;
					ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
					if (component)
					{
						ParticleSystem.MainModule main = component.main;
						main.maxParticles = (int)((float)main.maxParticles * @float);
					}
					list.Add(gameObject2);
				}
			}
		}
		return list;
	}

	// Token: 0x040074C9 RID: 29897
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, DictionaryList<string, BiomeParticleManager.ParticleEffectData>> effects;

	// Token: 0x040074CA RID: 29898
	public static bool RegistrationCompleted;

	// Token: 0x0200132A RID: 4906
	public struct ParticleEffectData
	{
		// Token: 0x040074CB RID: 29899
		public string biomeName;

		// Token: 0x040074CC RID: 29900
		public string prefabName;

		// Token: 0x040074CD RID: 29901
		public float chunkMargin;
	}
}
