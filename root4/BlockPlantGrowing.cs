using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000139 RID: 313
[Preserve]
public class BlockPlantGrowing : BlockPlant
{
	// Token: 0x06000878 RID: 2168 RVA: 0x0003BE95 File Offset: 0x0003A095
	public BlockPlantGrowing()
	{
		this.fertileLevel = 5;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0003BEC4 File Offset: 0x0003A0C4
	public override void LateInit()
	{
		base.LateInit();
		if (!base.Properties.Classes.ContainsKey("PlantGrowing"))
		{
			return;
		}
		DynamicProperties dynamicProperties = base.Properties.Classes["PlantGrowing"];
		if (dynamicProperties.Values.ContainsKey("Next"))
		{
			this.nextPlant = ItemClass.GetItem(dynamicProperties.Values["Next"], false).ToBlockValue(false);
			if (this.nextPlant.Equals(BlockValue.Air))
			{
				throw new Exception("Block with name '" + dynamicProperties.Values["Next"] + "' not found!");
			}
		}
		this.growOnTop = BlockValue.Air;
		if (dynamicProperties.Values.ContainsKey("IsGrowOnTopEnabled") && StringParsers.ParseBool(dynamicProperties.Values["IsGrowOnTopEnabled"], 0, -1, true))
		{
			this.bGrowOnTopEnabled = true;
			if (dynamicProperties.Values.ContainsKey("GrowOnTop"))
			{
				this.growOnTop = ItemClass.GetItem(dynamicProperties.Values["GrowOnTop"], false).ToBlockValue(false);
				if (this.growOnTop.Equals(BlockValue.Air))
				{
					throw new Exception("Block with name '" + dynamicProperties.Values["GrowOnTop"] + "' not found!");
				}
			}
		}
		if (dynamicProperties.Values.ContainsKey("GrowthRate"))
		{
			this.growthRate = StringParsers.ParseFloat(dynamicProperties.Values["GrowthRate"], 0, -1, NumberStyles.Any);
		}
		if (dynamicProperties.Values.ContainsKey("GrowthDeviation"))
		{
			this.growthDeviation = StringParsers.ParseFloat(dynamicProperties.Values["GrowthDeviation"], 0, -1, NumberStyles.Any);
		}
		if (dynamicProperties.Values.ContainsKey("FertileLevel"))
		{
			this.fertileLevel = int.Parse(dynamicProperties.Values["FertileLevel"]);
		}
		if (dynamicProperties.Values.ContainsKey("LightLevelStay"))
		{
			this.lightLevelStay = int.Parse(dynamicProperties.Values["LightLevelStay"]);
		}
		if (dynamicProperties.Values.ContainsKey("LightLevelGrow"))
		{
			this.lightLevelGrow = int.Parse(dynamicProperties.Values["LightLevelGrow"]);
		}
		if (dynamicProperties.Values.ContainsKey("GrowIfAnythinOnTop"))
		{
			this.isPlantGrowingIfAnythingOnTop = StringParsers.ParseBool(dynamicProperties.Values["GrowIfAnythinOnTop"], 0, -1, true);
		}
		if (dynamicProperties.Values.ContainsKey("IsRandom"))
		{
			this.isPlantGrowingRandom = StringParsers.ParseBool(dynamicProperties.Values["IsRandom"], 0, -1, true);
		}
		if (this.growthRate > 0f)
		{
			this.BlockTag = BlockTags.GrowablePlant;
			this.IsRandomlyTick = true;
			return;
		}
		this.IsRandomlyTick = false;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0003C190 File Offset: 0x0003A390
	public override bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (!base.CanPlaceBlockAt(_world, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache != null)
		{
			byte light = chunkCache.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN);
			if ((int)light < this.lightLevelStay || (int)light < this.lightLevelGrow)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0003C1F0 File Offset: 0x0003A3F0
	public override bool CanGrowOn(WorldBase _world, Vector3i _blockPos, BlockValue _blockValueOfPlant)
	{
		return this.fertileLevel == 0 || _world.GetBlock(_blockPos).Block.blockMaterial.FertileLevel >= this.fertileLevel;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0003C22B File Offset: 0x0003A42B
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		if (_ea is EntityPlayerLocal)
		{
			_ea.Progression.AddLevelExp((int)_result.blockValue.Block.blockMaterial.Experience, "_xpOther", Progression.XPTypes.Other, true, true, -1, null);
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0003C26C File Offset: 0x0003A46C
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (this.nextPlant.isair)
		{
			return;
		}
		if (_blockValue.ischild)
		{
			Log.Warning("BlockPlantGrowing OnBlockAdded child at {0}, {1}", new object[]
			{
				_blockPos,
				_blockValue
			});
			return;
		}
		if (!_world.IsRemote())
		{
			this.addScheduledTick(_world, _blockPos);
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0003C2D4 File Offset: 0x0003A4D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addScheduledTick(WorldBase _world, Vector3i _blockPos)
	{
		if (!this.isPlantGrowingRandom)
		{
			_world.GetWBT().AddScheduledBlockUpdate(_blockPos, this.blockID, this.GetTickRate());
			return;
		}
		int num = (int)this.GetTickRate();
		int num2 = (int)((float)num * this.growthDeviation);
		int num3 = num / 2;
		int max = num + num3;
		GameRandom gameRandom = _world.GetGameRandom();
		int num4;
		int num5;
		do
		{
			float randomGaussian = gameRandom.RandomGaussian;
			num4 = Mathf.RoundToInt((float)num + (float)num2 * randomGaussian);
			num5 = Utils.FastClamp(num4, num3, max);
		}
		while (num5 != num4);
		_world.GetWBT().AddScheduledBlockUpdate(_blockPos, this.blockID, (ulong)((long)num5));
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0003C363 File Offset: 0x0003A563
	public override ulong GetTickRate()
	{
		return (ulong)(this.growthRate * BlockPlantGrowing.CropGrowthModifier * 20f * 60f);
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0003C380 File Offset: 0x0003A580
	public override bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		if (this.nextPlant.isair)
		{
			return false;
		}
		if (!this.CheckPlantAlive(_world, _blockPos, _blockValue))
		{
			return true;
		}
		if (_bRandomTick)
		{
			this.addScheduledTick(_world, _blockPos);
			return true;
		}
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return true;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		if ((int)chunkCache.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN) < this.lightLevelGrow)
		{
			this.addScheduledTick(_world, _blockPos);
			return true;
		}
		BlockValue block = _world.GetBlock(_blockPos + Vector3i.up);
		if (!this.isPlantGrowingIfAnythingOnTop && !block.isair)
		{
			return true;
		}
		BlockPlant blockPlant = this.nextPlant.Block as BlockPlant;
		if (blockPlant != null && !blockPlant.CanGrowOn(_world, _blockPos + Vector3i.down, this.nextPlant))
		{
			return true;
		}
		_blockValue.type = this.nextPlant.type;
		BiomeDefinition biome = ((World)_world).GetBiome(_blockPos.x, _blockPos.z);
		if (biome != null && biome.Replacements.ContainsKey(_blockValue.type))
		{
			_blockValue.type = biome.Replacements[_blockValue.type];
		}
		BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(_blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
		blockValue.rotation = _blockValue.rotation;
		blockValue.meta = _blockValue.meta;
		blockValue.meta2 = 0;
		_blockValue = blockValue;
		if (this.bGrowOnTopEnabled)
		{
			_blockValue.meta = (_blockValue.meta + 1 & 15);
		}
		if (this.isPlantGrowingRandom || _ticksIfLoaded <= this.GetTickRate() || !_blockValue.Block.UpdateTick(_world, _blockPos, _blockValue, false, _ticksIfLoaded - this.GetTickRate(), _rnd))
		{
			_world.SetBlockRPC(_blockPos, _blockValue);
		}
		if (!this.growOnTop.isair && _blockPos.y + 1 < 255 && block.isair)
		{
			_blockValue.type = this.growOnTop.type;
			_blockValue = _blockValue.Block.OnBlockPlaced(_world, _blockPos, _blockValue, _rnd);
			Block block2 = _blockValue.Block;
			if (_blockValue.damage >= block2.blockMaterial.MaxDamage)
			{
				_blockValue.damage = block2.blockMaterial.MaxDamage - 1;
			}
			if (this.isPlantGrowingRandom || _ticksIfLoaded <= this.GetTickRate() || !block2.UpdateTick(_world, _blockPos + Vector3i.up, _blockValue, false, _ticksIfLoaded - this.GetTickRate(), _rnd))
			{
				_world.SetBlockRPC(_blockPos + Vector3i.up, _blockValue);
			}
		}
		return true;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0003C610 File Offset: 0x0003A810
	public BlockValue ForceNextGrowStage(World _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockValue block = _world.GetBlock(_blockPos + Vector3i.up);
		if (!this.isPlantGrowingIfAnythingOnTop && !block.isair)
		{
			return _blockValue;
		}
		_blockValue.type = this.nextPlant.type;
		BiomeDefinition biome = _world.GetBiome(_blockPos.x, _blockPos.z);
		if (biome != null && biome.Replacements.ContainsKey(_blockValue.type))
		{
			_blockValue.type = biome.Replacements[_blockValue.type];
		}
		BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(_blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
		blockValue.rotation = _blockValue.rotation;
		blockValue.meta = _blockValue.meta;
		blockValue.meta2 = 0;
		_blockValue = blockValue;
		if (this.bGrowOnTopEnabled)
		{
			_blockValue.meta = (_blockValue.meta + 1 & 15);
		}
		if (!this.growOnTop.isair && _blockPos.y + 1 < 255 && block.isair)
		{
			_blockValue.type = this.growOnTop.type;
			_blockValue = _blockValue.Block.OnBlockPlaced(_world, _blockPos, _blockValue, _world.GetGameRandom());
			Block block2 = _blockValue.Block;
			if (_blockValue.damage >= block2.blockMaterial.MaxDamage)
			{
				_blockValue.damage = block2.blockMaterial.MaxDamage - 1;
			}
			_world.SetBlockRPC(_blockPos + Vector3i.up, _blockValue);
		}
		return _blockValue;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0003C791 File Offset: 0x0003A991
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		if (!_world.IsRemote())
		{
			_world.GetWBT().InvalidateScheduledBlockUpdate(_blockPos, this.blockID);
		}
	}

	// Token: 0x04000963 RID: 2403
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropPlantGrowing = "PlantGrowing";

	// Token: 0x04000964 RID: 2404
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropNext = "Next";

	// Token: 0x04000965 RID: 2405
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropGrowthRate = "GrowthRate";

	// Token: 0x04000966 RID: 2406
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropGrowthDeviation = "GrowthDeviation";

	// Token: 0x04000967 RID: 2407
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropFertileLevel = "FertileLevel";

	// Token: 0x04000968 RID: 2408
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropGrowOnTop = "GrowOnTop";

	// Token: 0x04000969 RID: 2409
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropIsGrowOnTopEnabled = "IsGrowOnTopEnabled";

	// Token: 0x0400096A RID: 2410
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropLightLevelStay = "LightLevelStay";

	// Token: 0x0400096B RID: 2411
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropLightLevelGrow = "LightLevelGrow";

	// Token: 0x0400096C RID: 2412
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropIsRandom = "IsRandom";

	// Token: 0x0400096D RID: 2413
	[PublicizedFrom(EAccessModifier.Protected)]
	public const string PropGrowIfAnythinOnTop = "GrowIfAnythinOnTop";

	// Token: 0x0400096E RID: 2414
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue nextPlant;

	// Token: 0x0400096F RID: 2415
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue growOnTop;

	// Token: 0x04000970 RID: 2416
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bGrowOnTopEnabled;

	// Token: 0x04000971 RID: 2417
	[PublicizedFrom(EAccessModifier.Protected)]
	public float growthRate;

	// Token: 0x04000972 RID: 2418
	[PublicizedFrom(EAccessModifier.Protected)]
	public float growthDeviation = 0.25f;

	// Token: 0x04000973 RID: 2419
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lightLevelGrow = 8;

	// Token: 0x04000974 RID: 2420
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPlantGrowingRandom = true;

	// Token: 0x04000975 RID: 2421
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPlantGrowingIfAnythingOnTop = true;

	// Token: 0x04000976 RID: 2422
	public static float CropGrowthModifier = 1f;
}
