using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AFA RID: 2810
public class BiomeAtmosphereEffects
{
	// Token: 0x060053B1 RID: 21425 RVA: 0x00201060 File Offset: 0x001FF260
	public void Init(World _world)
	{
		this.world = _world;
		this.worldColorSpectrums = new AtmosphereEffect[255];
		this.worldColorSpectrums[0] = AtmosphereEffect.Load("default", null);
		foreach (KeyValuePair<uint, BiomeDefinition> keyValuePair in _world.Biomes.GetBiomeMap())
		{
			this.worldColorSpectrums[(int)keyValuePair.Value.m_Id] = AtmosphereEffect.Load(keyValuePair.Value.m_SpectrumName, this.worldColorSpectrums[0]);
		}
		this.ForceDefault = false;
	}

	// Token: 0x060053B2 RID: 21426 RVA: 0x00201110 File Offset: 0x001FF310
	public void Reload()
	{
		this.Init(this.world);
		this.Update();
	}

	// Token: 0x060053B3 RID: 21427 RVA: 0x00201124 File Offset: 0x001FF324
	public virtual void Update()
	{
		EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		if (this.ForceDefault)
		{
			this.currentBiomeIntensity = BiomeIntensity.Default;
			return;
		}
		Vector3i blockPosition = primaryPlayer.GetBlockPosition();
		BiomeIntensity biomeIntensity;
		if (!blockPosition.Equals(this.playerPosition) && this.world.GetBiomeIntensity(blockPosition, out biomeIntensity))
		{
			this.playerPosition = blockPosition;
			if (!this.currentBiomeIntensity.Equals(biomeIntensity))
			{
				WorldBiomes biomes = GameManager.Instance.World.Biomes;
				BiomeDefinition biome = biomes.GetBiome(biomeIntensity.biomeId0);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity0;
				}
				this.nearBiomes[0] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId1);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity1;
				}
				this.nearBiomes[1] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId2);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity1;
				}
				this.nearBiomes[2] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId3);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity2;
				}
				this.nearBiomes[3] = biome;
			}
			this.currentBiomeIntensity = biomeIntensity;
		}
	}

	// Token: 0x060053B4 RID: 21428 RVA: 0x0020124F File Offset: 0x001FF44F
	public virtual Color GetSkyColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Sky);
	}

	// Token: 0x060053B5 RID: 21429 RVA: 0x0020125F File Offset: 0x001FF45F
	public virtual Color GetAmbientColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Ambient);
	}

	// Token: 0x060053B6 RID: 21430 RVA: 0x0020126F File Offset: 0x001FF46F
	public virtual Color GetSunColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Sun);
	}

	// Token: 0x060053B7 RID: 21431 RVA: 0x0020127F File Offset: 0x001FF47F
	public virtual Color GetMoonColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Moon);
	}

	// Token: 0x060053B8 RID: 21432 RVA: 0x0020128F File Offset: 0x001FF48F
	public virtual Color GetFogColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Fog);
	}

	// Token: 0x060053B9 RID: 21433 RVA: 0x0020129F File Offset: 0x001FF49F
	public virtual Color GetFogFadeColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.FogFade);
	}

	// Token: 0x060053BA RID: 21434 RVA: 0x000BD92B File Offset: 0x000BBB2B
	public virtual Color GetCloudsColor(float _v)
	{
		return Color.white;
	}

	// Token: 0x060053BB RID: 21435 RVA: 0x002012B0 File Offset: 0x001FF4B0
	[PublicizedFrom(EAccessModifier.Private)]
	public Color getColorFromSpectrum(BiomeIntensity _bi, float _v, AtmosphereEffect.ESpecIdx _spectrumIdx)
	{
		float intensity = _bi.intensity0;
		float intensity2 = _bi.intensity1;
		float intensity3 = _bi.intensity2;
		float num = _bi.intensity3;
		num = 0f;
		return (this.worldColorSpectrums[(int)_bi.biomeId0].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity + this.worldColorSpectrums[(int)_bi.biomeId1].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity2 + this.worldColorSpectrums[(int)_bi.biomeId2].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity3 + this.worldColorSpectrums[(int)_bi.biomeId3].spectrums[(int)_spectrumIdx].GetValue(_v) * num) / (intensity + intensity2 + intensity3 + num);
	}

	// Token: 0x04004176 RID: 16758
	public BiomeDefinition[] nearBiomes = new BiomeDefinition[4];

	// Token: 0x04004177 RID: 16759
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeIntensity currentBiomeIntensity = BiomeIntensity.Default;

	// Token: 0x04004178 RID: 16760
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04004179 RID: 16761
	[PublicizedFrom(EAccessModifier.Private)]
	public AtmosphereEffect[] worldColorSpectrums;

	// Token: 0x0400417A RID: 16762
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i playerPosition;

	// Token: 0x0400417B RID: 16763
	public bool ForceDefault;
}
