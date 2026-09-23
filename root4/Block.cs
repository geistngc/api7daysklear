using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000106 RID: 262
[Preserve]
public class Block
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600064C RID: 1612 RVA: 0x0002CC52 File Offset: 0x0002AE52
	public static bool BlocksLoaded
	{
		get
		{
			return Block.list != null;
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002CC5C File Offset: 0x0002AE5C
	public static Dictionary<string, int> BlockIdsByName()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (KeyValuePair<string, Block> keyValuePair in Block.nameToBlock)
		{
			dictionary[keyValuePair.Key] = keyValuePair.Value.blockID;
		}
		return dictionary;
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600064E RID: 1614 RVA: 0x0002CCC8 File Offset: 0x0002AEC8
	// (set) Token: 0x0600064F RID: 1615 RVA: 0x0002CCE9 File Offset: 0x0002AEE9
	public DynamicProperties Properties
	{
		get
		{
			if (this.dynamicProperties != null)
			{
				return this.dynamicProperties;
			}
			return Block.PropertiesCache.Cache(this.blockID);
		}
		set
		{
			this.dynamicProperties = value;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000650 RID: 1616 RVA: 0x0002CCF4 File Offset: 0x0002AEF4
	public RecipeUnlockData[] UnlockedBy
	{
		get
		{
			if (this.unlockedBy == null)
			{
				if (this.Properties.Values.ContainsKey(Block.PropUnlockedBy))
				{
					string[] array = this.Properties.Values[Block.PropUnlockedBy].Split(',', StringSplitOptions.None);
					if (array.Length != 0)
					{
						this.unlockedBy = new RecipeUnlockData[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							this.unlockedBy[i] = new RecipeUnlockData(array[i]);
						}
					}
				}
				else
				{
					this.unlockedBy = new RecipeUnlockData[0];
				}
			}
			return this.unlockedBy;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000651 RID: 1617 RVA: 0x0002CD82 File Offset: 0x0002AF82
	public bool IsCollideMovement
	{
		get
		{
			return (this.BlockingType & 2) != 0;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000652 RID: 1618 RVA: 0x0002CD8F File Offset: 0x0002AF8F
	public bool IsCollideSight
	{
		get
		{
			return (this.BlockingType & 1) != 0;
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000653 RID: 1619 RVA: 0x0002CD9C File Offset: 0x0002AF9C
	public bool IsCollideBullets
	{
		get
		{
			return (this.BlockingType & 4) != 0;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x0002CDA9 File Offset: 0x0002AFA9
	public bool IsCollideRockets
	{
		get
		{
			return (this.BlockingType & 8) != 0;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000655 RID: 1621 RVA: 0x0002CDB6 File Offset: 0x0002AFB6
	public bool IsCollideMelee
	{
		get
		{
			return (this.BlockingType & 16) != 0;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000656 RID: 1622 RVA: 0x0002CDC4 File Offset: 0x0002AFC4
	public bool IsCollideArrows
	{
		get
		{
			return (this.BlockingType & 32) != 0;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000657 RID: 1623 RVA: 0x0002CDD2 File Offset: 0x0002AFD2
	// (set) Token: 0x06000658 RID: 1624 RVA: 0x0002CDE9 File Offset: 0x0002AFE9
	public bool IsNotifyOnLoadUnload
	{
		get
		{
			return this.bNotifyOnLoadUnload || this.shape.IsNotifyOnLoadUnload;
		}
		set
		{
			this.bNotifyOnLoadUnload = value;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000659 RID: 1625 RVA: 0x0002CDF2 File Offset: 0x0002AFF2
	// (set) Token: 0x0600065A RID: 1626 RVA: 0x0002CDFA File Offset: 0x0002AFFA
	public List<ShapesFromXml.ShapeCategory> ShapeCategories { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x0600065B RID: 1627 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool AllowBlockTriggers
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0002CE04 File Offset: 0x0002B004
	public Block()
	{
		this.shape = new BlockShapeCube();
		this.shape.Init(this);
		this.Properties = new DynamicProperties();
		this.blockMaterial = MaterialBlock.air;
		this.MeshIndex = 0;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0002CFC8 File Offset: 0x0002B1C8
	public static Vector3 StringToVector3(string _input)
	{
		Vector3 zero = Vector3.zero;
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 2, 0, -1);
		int num = 255;
		int num2 = 255;
		int num3 = 255;
		StringParsers.TryParseSInt32(_input, out num, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer);
		if (separatorPositions.TotalFound > 0)
		{
			StringParsers.TryParseSInt32(_input, out num2, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer);
		}
		if (separatorPositions.TotalFound > 1)
		{
			StringParsers.TryParseSInt32(_input, out num3, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Integer);
		}
		zero.x = (float)num / 255f;
		zero.y = (float)num2 / 255f;
		zero.z = (float)num3 / 255f;
		return zero;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0002D084 File Offset: 0x0002B284
	public virtual void Init()
	{
		if (Block.nameToBlockCaseInsensitive.ContainsKey(this.blockName))
		{
			Log.Error("Block " + this.blockName + " is found multiple times, overriding with latest definition!");
		}
		Block.nameToBlock[this.blockName] = this;
		Block.nameToBlockCaseInsensitive[this.blockName] = this;
		if (this.Properties.Values.ContainsKey(Block.PropTag))
		{
			this.Tags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[Block.PropTag]);
		}
		if (this.Properties.Values.ContainsKey(Block.PropLightOpacity))
		{
			int.TryParse(this.Properties.GetValue(Block.PropLightOpacity), out this.lightOpacity);
		}
		else
		{
			this.lightOpacity = Math.Max(this.blockMaterial.LightOpacity, (int)this.shape.LightOpacity);
		}
		this.Properties.ParseColorHex(Block.PropTintColor, ref this.tintColor);
		StringParsers.TryParseBool(this.Properties.GetValue(Block.PropCanPickup), out this.CanPickup);
		if (this.CanPickup && this.Properties.Params1.ContainsKey(Block.PropCanPickup))
		{
			this.PickedUpItemValue = this.Properties.Params1[Block.PropCanPickup];
		}
		if (this.Properties.Values.ContainsKey(Block.PropFuelValue))
		{
			int.TryParse(this.Properties.Values[Block.PropFuelValue], out this.FuelValue);
		}
		if (this.Properties.Values.ContainsKey(Block.PropWeight))
		{
			int startValue;
			int.TryParse(this.Properties.Values[Block.PropWeight], out startValue);
			this.Weight = new DataItem<int>(startValue);
		}
		this.CanMobsSpawnOn = false;
		this.Properties.ParseBool(Block.PropCanMobsSpawnOn, ref this.CanMobsSpawnOn);
		this.CanPlayersSpawnOn = true;
		this.Properties.ParseBool(Block.PropCanPlayersSpawnOn, ref this.CanPlayersSpawnOn);
		if (this.Properties.Values.ContainsKey(Block.PropPickupTarget))
		{
			this.PickupTarget = this.Properties.Values[Block.PropPickupTarget];
		}
		if (this.Properties.Values.ContainsKey(Block.PropPickupSource))
		{
			this.PickupSource = this.Properties.Values[Block.PropPickupSource];
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlaceAltBlockValue))
		{
			this.placeAltBlockNames = this.Properties.Values[Block.PropPlaceAltBlockValue].Split(',', StringSplitOptions.None);
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlaceShapeCategories))
		{
			string[] array = this.Properties.Values[Block.PropPlaceShapeCategories].Split(',', StringSplitOptions.None);
			this.ShapeCategories = new List<ShapesFromXml.ShapeCategory>();
			foreach (string text in array)
			{
				ShapesFromXml.ShapeCategory item;
				if (ShapesFromXml.shapeCategories.TryGetValue(text, out item))
				{
					this.ShapeCategories.Add(item);
				}
				else
				{
					Log.Error("Block " + this.blockName + " has unknown ShapeCategory " + text);
				}
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropIndexName))
		{
			this.IndexName = this.Properties.Values[Block.PropIndexName];
		}
		this.Properties.ParseBool(Block.PropCanBlocksReplace, ref this.CanBlocksReplace);
		this.Properties.ParseBool(Block.PropCanDecorateOnSlopes, ref this.CanDecorateOnSlopes);
		this.SlopeMaxCos = 90f;
		this.Properties.ParseFloat(Block.PropSlopeMax, ref this.SlopeMaxCos);
		this.SlopeMaxCos = Mathf.Cos(this.SlopeMaxCos * 0.017453292f);
		if (this.Properties.Values.ContainsKey(Block.PropIsProp))
		{
			this.IsProp = StringParsers.ParseBool(this.Properties.Values[Block.PropIsProp], 0, -1, true);
		}
		else
		{
			this.IsProp = (this.shape is BlockShapeModelEntity);
		}
		if (this.Properties.Values.ContainsKey(Block.PropIsTerrainDecoration))
		{
			this.IsTerrainDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropIsTerrainDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropIsDecoration))
		{
			this.IsDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropIsDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropDistantDecoration))
		{
			this.IsDistantDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropDistantDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropBigDecorationRadius))
		{
			this.BigDecorationRadius = int.Parse(this.Properties.Values[Block.PropBigDecorationRadius]);
		}
		if (this.Properties.Values.ContainsKey(Block.PropSmallDecorationRadius))
		{
			this.SmallDecorationRadius = int.Parse(this.Properties.Values[Block.PropSmallDecorationRadius]);
		}
		this.Properties.ParseFloat(Block.PropGndAlign, ref this.GroundAlignDistance);
		this.Properties.ParseBool(Block.PropIgnoreKeystoneOverlay, ref this.IgnoreKeystoneOverlay);
		this.LPHardnessScale = 1f;
		if (this.Properties.Values.ContainsKey(Block.PropLPScale))
		{
			this.LPHardnessScale = StringParsers.ParseFloat(this.Properties.Values[Block.PropLPScale], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapColor))
		{
			this.MapColor = StringParsers.ParseColor32(this.Properties.Values[Block.PropMapColor]);
			this.bMapColorSet = true;
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapColor2))
		{
			this.MapColor2 = StringParsers.ParseColor32(this.Properties.Values[Block.PropMapColor2]);
			this.bMapColor2Set = true;
		}
		if (this.Properties.Values.ContainsKey(Block.PropGroupName))
		{
			string[] array3 = this.Properties.Values[Block.PropGroupName].Split(',', StringSplitOptions.None);
			if (!Block.groupNameStringToGroupNames.TryGetValue(this.Properties.Values[Block.PropGroupName], out this.GroupNames))
			{
				if (array3.Length != 0)
				{
					this.GroupNames = new string[array3.Length];
					for (int j = 0; j < array3.Length; j++)
					{
						this.GroupNames[j] = array3[j].Trim();
					}
				}
				Block.groupNameStringToGroupNames.Add(this.Properties.Values[Block.PropGroupName], this.GroupNames);
			}
			if (this.GroupNames.Length != 0)
			{
				for (int k = 0; k < this.GroupNames.Length; k++)
				{
					if (this.GroupNames[k] == "Limited")
					{
						this.IsLimited = true;
						break;
					}
				}
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropCustomIcon))
		{
			this.CustomIcon = this.Properties.Values[Block.PropCustomIcon];
		}
		if (this.Properties.Values.ContainsKey(Block.PropCustomIconTint))
		{
			this.CustomIconTint = StringParsers.ParseHexColor(this.Properties.Values[Block.PropCustomIconTint]);
		}
		else
		{
			this.CustomIconTint = Color.white;
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlacementWireframe))
		{
			this.bHasPlacementWireframe = StringParsers.ParseBool(this.Properties.Values[Block.PropPlacementWireframe], 0, -1, true);
		}
		else
		{
			this.bHasPlacementWireframe = true;
		}
		this.isOversized = this.Properties.Values.ContainsKey(Block.PropOversizedBounds);
		if (this.isOversized)
		{
			this.oversizedBounds = StringParsers.ParseBounds(this.Properties.Values[Block.PropOversizedBounds]);
		}
		else
		{
			this.oversizedBounds = default(Bounds);
		}
		if (this.Properties.Values.ContainsKey(Block.PropMultiBlockDim))
		{
			this.isMultiBlock = true;
			Vector3i vector3i = StringParsers.ParseVector3i(this.Properties.Values[Block.PropMultiBlockDim], 0, -1, false);
			List<Vector3i> list = new List<Vector3i>();
			if (this.Properties.Values.ContainsKey(Block.PropMultiBlockLayer0))
			{
				int num = 0;
				while (this.Properties.Values.ContainsKey(Block.PropMultiBlockLayer + num.ToString()))
				{
					string[] array4 = this.Properties.Values[Block.PropMultiBlockLayer + num.ToString()].Split(',', StringSplitOptions.None);
					for (int l = 0; l < array4.Length; l++)
					{
						array4[l] = array4[l].Trim();
						if (array4[l].Length > vector3i.x)
						{
							throw new Exception("Multi block layer entry " + l.ToString() + " too long for block " + this.blockName);
						}
						for (int m = 0; m < array4[l].Length; m++)
						{
							if (array4[l][m] != ' ')
							{
								list.Add(new Vector3i(m, num, l));
							}
						}
					}
					num++;
				}
			}
			else
			{
				int num2 = vector3i.x / 2;
				int num3 = Mathf.RoundToInt((float)vector3i.x / 2f + 0.1f) - 1;
				int num4 = vector3i.z / 2;
				int num5 = Mathf.RoundToInt((float)vector3i.z / 2f + 0.1f) - 1;
				for (int n = -num2; n <= num3; n++)
				{
					for (int num6 = 0; num6 < vector3i.y; num6++)
					{
						for (int num7 = -num4; num7 <= num5; num7++)
						{
							list.Add(new Vector3i(n, num6, num7));
						}
					}
				}
			}
			this.multiBlockPos = new Block.MultiBlockArray(vector3i, list);
		}
		if (this.Properties.Values.ContainsKey(Block.PropTerrainAlignment))
		{
			this.terrainAlignmentMode = EnumUtils.Parse<TerrainAlignmentMode>(this.Properties.Values[Block.PropTerrainAlignment], false);
			if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
			{
				bool flag = this.shape is BlockShapeModelEntity;
				bool flag2 = this.isOversized || this.isMultiBlock;
				if (!flag || !flag2)
				{
					Debug.LogWarning(string.Format("Failed to apply TerrainAlignmentMode \"{0}\" to {1}. ", this.terrainAlignmentMode, this.blockName) + "Terrain alignment is only supported for oversized- and multi-blocks with shape type \"ModelEntity\".\n" + string.Format("isModelEntity: {0}. isOversized: {1}. isMultiBlock: {2}. ", flag, this.isOversized, this.isMultiBlock));
					this.terrainAlignmentMode = TerrainAlignmentMode.None;
				}
			}
		}
		else
		{
			this.terrainAlignmentMode = TerrainAlignmentMode.None;
		}
		this.Properties.ParseFloat(Block.PropHeatMapStrength, ref this.HeatMapStrength);
		this.FallDamage = 1f;
		if (this.Properties.Values.ContainsKey(Block.PropFallDamage))
		{
			this.FallDamage = StringParsers.ParseFloat(this.Properties.Values[Block.PropFallDamage], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropCount))
		{
			this.Count = int.Parse(this.Properties.Values[Block.PropCount]);
		}
		if (this.Properties.Values.ContainsKey("ImposterExclude"))
		{
			this.bImposterExclude = StringParsers.ParseBool(this.Properties.Values["ImposterExclude"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("ImposterExcludeAndStop"))
		{
			this.bImposterExcludeAndStop = StringParsers.ParseBool(this.Properties.Values["ImposterExcludeAndStop"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("ImposterDontBlock"))
		{
			this.bImposterDontBlock = StringParsers.ParseBool(this.Properties.Values["ImposterDontBlock"], 0, -1, true);
		}
		this.AllowedRotations = EBlockRotationClasses.No45;
		if (this.shape is BlockShapeModelEntity)
		{
			this.AllowedRotations |= EBlockRotationClasses.Basic45;
		}
		if (this.Properties.Values.ContainsKey(Block.PropAllowAllRotations) && StringParsers.ParseBool(this.Properties.Values[Block.PropAllowAllRotations], 0, -1, true))
		{
			this.AllowedRotations |= EBlockRotationClasses.Basic45;
		}
		if (this.Properties.Values.ContainsKey("OnlySimpleRotations") && StringParsers.ParseBool(this.Properties.Values["OnlySimpleRotations"], 0, -1, true))
		{
			this.AllowedRotations &= ~(EBlockRotationClasses.Headfirst | EBlockRotationClasses.Sideways);
		}
		if (this.Properties.Values.ContainsKey("AllowedRotations"))
		{
			this.AllowedRotations = EBlockRotationClasses.None;
			foreach (string text2 in this.Properties.Values["AllowedRotations"].Split(',', StringSplitOptions.None))
			{
				EBlockRotationClasses eblockRotationClasses;
				if (EnumUtils.TryParse<EBlockRotationClasses>(text2, out eblockRotationClasses, true))
				{
					this.AllowedRotations |= eblockRotationClasses;
				}
				else
				{
					Log.Error(string.Concat(new string[]
					{
						"Rotation class '",
						text2,
						"' not found for block '",
						this.blockName,
						"'"
					}));
				}
			}
		}
		if (this.isMultiBlock && this.multiBlockPos != null)
		{
			if (this.multiBlockPos.dim.x > 16)
			{
				Log.Error(string.Format("MultiBlock '{0}' dim.x={1} exceeds XZ encoding range (16). Reauthor with smaller X dimension.", this.blockName, this.multiBlockPos.dim.x));
			}
			if (this.multiBlockPos.dim.z > 16)
			{
				Log.Error(string.Format("MultiBlock '{0}' dim.z={1} exceeds XZ encoding range (16). Reauthor with smaller Z dimension.", this.blockName, this.multiBlockPos.dim.z));
			}
			if (this.multiBlockPos.dim.y > 64)
			{
				Log.Error(string.Format("MultiBlock '{0}' dim.y={1} exceeds Y encoding range (64).", this.blockName, this.multiBlockPos.dim.y));
			}
			if ((this.AllowedRotations & EBlockRotationClasses.Advanced) != EBlockRotationClasses.None && this.multiBlockPos.dim.y > 8)
			{
				Log.Error(string.Format("MultiBlock '{0}' dim.y={1} is > 8 and allows Advanced rotations which would overflow XZ encoding. Reauthor dimensions (e.g. swap Y with X/Z) or restrict to OnlySimpleRotations.", this.blockName, this.multiBlockPos.dim.y));
			}
		}
		if (this.Properties.Values.ContainsKey("PlaceAsRandomRotation"))
		{
			this.PlaceRandomRotation = StringParsers.ParseBool(this.Properties.Values["PlaceAsRandomRotation"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropIsPlant))
		{
			this.bIsPlant = StringParsers.ParseBool(this.Properties.Values[Block.PropIsPlant], 0, -1, true);
		}
		this.Properties.ParseString("CustomPlaceSound", ref this.CustomPlaceSound);
		this.Properties.ParseString("UpgradeSound", ref this.UpgradeSound);
		this.Properties.ParseString("DowngradeFX", ref this.DowngradeFX);
		this.Properties.ParseString("DestroyFX", ref this.DestroyFX);
		if (this.Properties.Values.ContainsKey(Block.PropBuffsWhenWalkedOn))
		{
			this.BuffsWhenWalkedOn = this.Properties.Values[Block.PropBuffsWhenWalkedOn].Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (this.BuffsWhenWalkedOn.Length < 1)
			{
				this.BuffsWhenWalkedOn = null;
			}
		}
		this.Properties.ParseBool(Block.PropIsReplaceRandom, ref this.IsReplaceRandom);
		if (this.Properties.Values.ContainsKey(Block.PropCraftExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropCraftExpValue], out this.CraftComponentExp);
		}
		if (this.Properties.Values.ContainsKey(Block.PropCraftTimeValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropCraftTimeValue], out this.CraftComponentTime);
		}
		if (this.Properties.Values.ContainsKey(Block.PropLootExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropLootExpValue], out this.LootExp);
		}
		if (this.Properties.Values.ContainsKey(Block.PropDestroyExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropDestroyExpValue], out this.DestroyExp);
		}
		this.Properties.ParseString(Block.PropParticleOnDeath, ref this.deathParticleName);
		if (this.Properties.Values.ContainsKey(Block.PropPlaceExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropPlaceExpValue], out this.PlaceExp);
		}
		if (this.Properties.Values.ContainsKey(Block.PropUpgradeExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropUpgradeExpValue], out this.UpgradeExp);
		}
		this.Properties.ParseFloat(Block.PropEconomicValue, ref this.EconomicValue);
		this.Properties.ParseFloat(Block.PropEconomicSellScale, ref this.EconomicSellScale);
		this.Properties.ParseInt(Block.PropEconomicBundleSize, ref this.EconomicBundleSize);
		if (this.Properties.Values.ContainsKey(Block.PropSellableToTrader))
		{
			StringParsers.TryParseBool(this.Properties.Values[Block.PropSellableToTrader], out this.SellableToTrader);
		}
		this.Properties.ParseString(Block.PropTraderStageTemplate, ref this.TraderStageTemplate);
		if (this.Properties.Values.ContainsKey(Block.PropCreativeMode))
		{
			this.CreativeMode = EnumUtils.Parse<EnumCreativeMode>(this.Properties.Values[Block.PropCreativeMode], false);
		}
		if (this.Properties.Values.ContainsKey(Block.PropFilterTag))
		{
			this.FilterTags = this.Properties.Values[Block.PropFilterTag].Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (this.FilterTags.Length < 1)
			{
				this.FilterTags = null;
			}
		}
		this.SortOrder = this.Properties.GetString(Block.PropCreativeSort1);
		this.SortOrder += this.Properties.GetString(Block.PropCreativeSort2);
		if (this.Properties.Values.ContainsKey(Block.PropDisplayType))
		{
			this.DisplayType = this.Properties.Values[Block.PropDisplayType];
		}
		if (this.Properties.Values.ContainsKey(Block.PropItemTypeIcon))
		{
			this.ItemTypeIcon = this.Properties.Values[Block.PropItemTypeIcon];
		}
		if (this.Properties.Values.ContainsKey(Block.PropAutoShape))
		{
			this.AutoShapeType = EnumUtils.Parse<EAutoShapeType>(this.Properties.Values[Block.PropAutoShape], false);
			if (this.AutoShapeType != EAutoShapeType.None)
			{
				string[] array5 = this.blockName.Split(':', StringSplitOptions.None);
				this.autoShapeBaseName = array5[0];
				this.autoShapeShapeName = array5[1];
				Block.autoShapeMaterials.Add(this.autoShapeBaseName);
			}
		}
		this.MaxDamage = this.blockMaterial.MaxDamage;
		this.Properties.ParseInt(Block.PropMaxDamage, ref this.MaxDamage);
		this.Properties.ParseInt(Block.PropStartDamage, ref this.StartDamage);
		this.Properties.ParseInt(Block.PropStage2Health, ref this.Stage2Health);
		this.Properties.ParseFloat(Block.PropDamage, ref this.Damage);
		this.IsExplosionAffected = true;
		this.Properties.ParseBool(Block.PropExplosionAffected, ref this.IsExplosionAffected);
		if (this.Properties.Values.ContainsKey(Block.PropActivationDistance))
		{
			int.TryParse(this.Properties.Values[Block.PropActivationDistance], out this.activationDistance);
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlacementDistance))
		{
			int.TryParse(this.Properties.Values[Block.PropPlacementDistance], out this.placementDistance);
		}
		if (this.Properties.Values.ContainsKey("PassThroughDamage"))
		{
			this.EnablePassThroughDamage = StringParsers.ParseBool(this.Properties.Values["PassThroughDamage"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("CopyPaintOnDowngrade"))
		{
			string[] array6 = this.Properties.Values["CopyPaintOnDowngrade"].Split(',', StringSplitOptions.None);
			HashSet<BlockFace> hashSet = new HashSet<BlockFace>();
			for (int num8 = 0; num8 < array6.Length; num8++)
			{
				char c = array6[num8][0];
				if (c <= 'E')
				{
					if (c != 'B')
					{
						if (c == 'E')
						{
							hashSet.Add(BlockFace.East);
						}
					}
					else
					{
						hashSet.Add(BlockFace.Bottom);
					}
				}
				else if (c != 'N')
				{
					switch (c)
					{
					case 'S':
						hashSet.Add(BlockFace.South);
						break;
					case 'T':
						hashSet.Add(BlockFace.Top);
						break;
					case 'W':
						hashSet.Add(BlockFace.West);
						break;
					}
				}
				else
				{
					hashSet.Add(BlockFace.North);
				}
			}
			this.RemovePaintOnDowngrade = new List<BlockFace>();
			for (int num9 = 0; num9 < 6; num9++)
			{
				if (!hashSet.Contains((BlockFace)num9))
				{
					this.RemovePaintOnDowngrade.Add((BlockFace)num9);
				}
			}
		}
		for (int num10 = 0; num10 < 1; num10++)
		{
			string useGlobalUV = ShapesFromXml.TextureLabelsByChannel[num10].UseGlobalUV;
			string @string = this.Properties.GetString(useGlobalUV);
			if (@string.Length > 0)
			{
				this.UVModesPerSide[num10] = 0U;
				if (!@string.Contains(","))
				{
					char c2 = @string[0];
					Block.UVMode uvmode = (c2 == 'G') ? Block.UVMode.Global : ((c2 == 'L') ? Block.UVMode.Local : Block.UVMode.Default);
					for (int num11 = 0; num11 < this.cUVModeSides; num11++)
					{
						this.UVModesPerSide[num10] |= (uint)((uint)uvmode << (num11 * this.cUVModeBits & 31));
					}
				}
				else
				{
					int num12 = 0;
					foreach (char c3 in @string)
					{
						if (c3 != ',')
						{
							Block.UVMode uvmode2 = (c3 == 'G') ? Block.UVMode.Global : ((c3 == 'L') ? Block.UVMode.Local : Block.UVMode.Default);
							this.UVModesPerSide[num10] |= (uint)((uint)uvmode2 << (num12 & 31));
							num12 += this.cUVModeBits;
						}
					}
				}
			}
		}
		this.RadiusEffects = null;
		string string2 = this.Properties.GetString(Block.PropRadiusEffect);
		if (string2.Length > 0)
		{
			List<BlockRadiusEffect> list2 = new List<BlockRadiusEffect>();
			foreach (string text3 in string2.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries))
			{
				BlockRadiusEffect item2 = default(BlockRadiusEffect);
				item2.radiusSq = 1f;
				item2.variable = text3;
				int num14 = text3.IndexOf(',');
				if (num14 >= 0)
				{
					float num15 = StringParsers.ParseFloat(text3, num14 + 1, -1, NumberStyles.Any);
					item2.radiusSq = num15 * num15;
					item2.variable = text3.Substring(0, num14);
				}
				list2.Add(item2);
			}
			this.RadiusEffects = list2.ToArray();
		}
		if (this.Properties.Values.ContainsKey(Block.PropDescriptionKey))
		{
			this.DescriptionKey = this.Properties.Values[Block.PropDescriptionKey];
		}
		else
		{
			this.DescriptionKey = string.Format("{0}Desc", this.blockName);
			if (!Localization.Exists(this.DescriptionKey, false))
			{
				this.DescriptionKey = Block.defaultBlockDescriptionKey;
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropCraftingSkillGroup))
		{
			this.CraftingSkillGroup = this.Properties.Values[Block.PropCraftingSkillGroup];
		}
		else
		{
			this.CraftingSkillGroup = "";
		}
		if (this.Properties.Values.ContainsKey(Block.PropHarvestOverdamage))
		{
			this.HarvestOverdamage = StringParsers.ParseBool(this.Properties.Values[Block.PropHarvestOverdamage], 0, -1, true);
		}
		this.bShowModelOnFall = (!this.Properties.Values.ContainsKey(Block.PropShowModelOnFall) || StringParsers.ParseBool(this.Properties.Values[Block.PropShowModelOnFall], 0, -1, true));
		if (this.Properties.Values.ContainsKey("HandleFace"))
		{
			this.HandleFace = EnumUtils.Parse<BlockFace>(this.Properties.Values["HandleFace"], false);
		}
		if (this.Properties.Values.ContainsKey("DisplayInfo"))
		{
			this.DisplayInfo = EnumUtils.Parse<Block.EnumDisplayInfo>(this.Properties.Values["DisplayInfo"], false);
		}
		if (this.Properties.Values.ContainsKey("SelectAlternates"))
		{
			this.SelectAlternates = StringParsers.ParseBool(this.Properties.Values["SelectAlternates"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropNoScrapping))
		{
			this.NoScrapping = StringParsers.ParseBool(this.Properties.Values[Block.PropNoScrapping], 0, -1, true);
		}
		this.VehicleHitScale = 1f;
		this.Properties.ParseFloat(Block.PropVehicleHitScale, ref this.VehicleHitScale);
		if (this.Properties.Values.ContainsKey("UiBackgroundTexture") && !StringParsers.TryParseSInt32(this.Properties.Values["UiBackgroundTexture"], out this.uiBackgroundTextureId))
		{
			this.uiBackgroundTextureId = -1;
		}
		this.Properties.ParseString(Block.PropBlockAddedEvent, ref this.blockAddedEvent);
		this.Properties.ParseString(Block.PropBlockDestroyedEvent, ref this.blockDestroyedEvent);
		this.Properties.ParseString(Block.PropBlockDowngradeEvent, ref this.blockDowngradeEvent);
		this.Properties.ParseString(Block.PropBlockDowngradedToEvent, ref this.blockDowngradedToEvent);
		this.Properties.ParseBool(Block.PropIsTemporaryBlock, ref this.IsTemporaryBlock);
		this.Properties.ParseBool(Block.PropRefundOnUnload, ref this.RefundOnUnload);
		this.Properties.ParseString(Block.PropSoundPickup, ref this.SoundPickup);
		this.Properties.ParseString(Block.PropSoundPlace, ref this.SoundPlace);
		this.Properties.ParseString(Block.PropSoundHitAdditional, ref this.SoundHitAdditional);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0002EB44 File Offset: 0x0002CD44
	public virtual void LateInit()
	{
		this.shape.LateInit();
		if (this.AutoShapeType == EAutoShapeType.Shape)
		{
			this.autoShapeHelper = Block.GetBlockByName(this.autoShapeBaseName + ":" + ShapesFromXml.VariantHelperName, false);
		}
		if (this.Properties.Values.ContainsKey(Block.PropSiblingBlock))
		{
			this.SiblingBlock = ItemClass.GetItem(this.Properties.Values[Block.PropSiblingBlock], false).ToBlockValue(false);
			if (this.SiblingBlock.Equals(BlockValue.Air))
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropSiblingBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.SiblingBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey("MirrorSibling"))
		{
			string text = this.Properties.Values["MirrorSibling"];
			this.MirrorSibling = ItemClass.GetItem(text, false).ToBlockValue(false).type;
			if (this.MirrorSibling == 0)
			{
				throw new Exception("Block with name '" + text + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.MirrorSibling = 0;
		}
		string str;
		if (this.Properties.TryGetValue(Block.PropUpgradeBlockClass, Block.PropUpgradeBlockToBlock, out str))
		{
			this.UpgradeBlock = Block.GetBlockValue(str, false);
			if (this.UpgradeBlock.isair)
			{
				throw new Exception("Block with name '" + str + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.UpgradeBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey(Block.PropDowngradeBlock))
		{
			this.DowngradeBlock = Block.GetBlockValue(this.Properties.Values[Block.PropDowngradeBlock], false);
			if (this.DowngradeBlock.isair)
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropDowngradeBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.DowngradeBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey("ImposterExchange"))
		{
			this.ImposterExchange = Block.GetBlockValue(this.Properties.Values["ImposterExchange"], false).type;
			if (this.Properties.Params1.ContainsKey("ImposterExchange"))
			{
				this.ImposterExchangeTexIdx = (byte)int.Parse(this.Properties.Params1["ImposterExchange"]);
			}
		}
		if (this.Properties.Values.ContainsKey("MergeInto"))
		{
			this.MergeIntoId = Block.GetBlockValue(this.Properties.Values["MergeInto"], false).type;
			if (this.MergeIntoId == 0)
			{
				Log.Warning("Warning: MergeInto block with name '{0}' not found!", new object[]
				{
					this.Properties.Values["MergeInto"]
				});
			}
			if (this.Properties.Params1.ContainsKey("MergeInto"))
			{
				string[] array = this.Properties.Params1["MergeInto"].Split(',', StringSplitOptions.None);
				if (array.Length == 6)
				{
					this.MergeIntoTexIds = new int[6];
					for (int i = 0; i < this.MergeIntoTexIds.Length; i++)
					{
						this.MergeIntoTexIds[i] = int.Parse(array[i].Trim());
					}
				}
			}
		}
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			Block.PropertiesCache.Store(this.blockID, this.dynamicProperties);
			this.dynamicProperties = null;
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0002EEE4 File Offset: 0x0002D0E4
	public static void InitStatic()
	{
		Block.nameToBlock = new Dictionary<string, Block>();
		Block.nameToBlockCaseInsensitive = new CaseInsensitiveStringDictionary<Block>();
		Block.list = new Block[Block.MAX_BLOCKS];
		Block.autoShapeMaterials = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		Block.groupNameStringToGroupNames = new Dictionary<string, string[]>();
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			Block.PropertiesCache = new DynamicPropertiesCache();
		}
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0002EF40 File Offset: 0x0002D140
	public static void LateInitAll()
	{
		for (int i = 0; i < Block.MAX_BLOCKS; i++)
		{
			if (Block.list[i] != null)
			{
				Block.list[i].LateInit();
			}
		}
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			GC.Collect();
		}
		int type = BlockValue.Air.type;
		for (int j = 0; j < Block.MAX_BLOCKS; j++)
		{
			Block block = Block.list[j];
			if (block != null)
			{
				int num = block.MaxDamage;
				int num2 = 0;
				int type2 = block.DowngradeBlock.type;
				while (type2 != type)
				{
					Block block2 = Block.list[type2];
					num += block2.MaxDamage;
					type2 = block2.DowngradeBlock.type;
					if (++num2 > 10)
					{
						Log.Warning("Block '{0}' over downgrade limit", new object[]
						{
							block.blockName
						});
						break;
					}
				}
				block.MaxDamagePlusDowngrades = num;
			}
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0002F01B File Offset: 0x0002D21B
	public static void OnWorldUnloaded()
	{
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			DynamicPropertiesCache propertiesCache = Block.PropertiesCache;
			if (propertiesCache != null)
			{
				propertiesCache.Cleanup();
			}
			Block.PropertiesCache = null;
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool FilterIndexType(BlockValue bv)
	{
		return true;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x0002F03A File Offset: 0x0002D23A
	public Vector2 GetPathOffset(int _rotation)
	{
		if (this.PathType != -1)
		{
			return Vector2.zero;
		}
		return this.shape.GetPathOffset(_rotation);
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0002F057 File Offset: 0x0002D257
	public static void Cleanup()
	{
		Block.nameToBlock = null;
		Block.nameToBlockCaseInsensitive = null;
		Block.groupNameStringToGroupNames = null;
		Block.list = null;
		Block.autoShapeMaterials = null;
		Block.fullMappingDataForClients = null;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0002F080 File Offset: 0x0002D280
	public void CopyDroppedFrom(Block _other)
	{
		foreach (KeyValuePair<EnumDropEvent, List<Block.SItemDropProb>> keyValuePair in _other.itemsToDrop)
		{
			EnumDropEvent key = keyValuePair.Key;
			List<Block.SItemDropProb> value = keyValuePair.Value;
			List<Block.SItemDropProb> list = this.itemsToDrop.ContainsKey(key) ? this.itemsToDrop[key] : null;
			if (list == null)
			{
				list = new List<Block.SItemDropProb>();
				this.itemsToDrop[key] = list;
			}
			for (int i = 0; i < value.Count; i++)
			{
				bool flag = true;
				int num = 0;
				while (flag && num < list.Count)
				{
					if (list[num].name == value[i].name)
					{
						flag = false;
					}
					num++;
				}
				if (flag)
				{
					list.Add(value[i]);
				}
			}
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0002F184 File Offset: 0x0002D384
	public virtual BlockFace getInventoryFace()
	{
		return BlockFace.North;
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0002F187 File Offset: 0x0002D387
	public virtual byte GetLightValue(BlockValue _blockValue)
	{
		return this.lightValue;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0002F18F File Offset: 0x0002D38F
	public virtual Block SetLightValue(float _lightValueInPercent)
	{
		this.lightValue = (byte)(15f * _lightValueInPercent);
		return this;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool UseBuffsWhenWalkedOn(World world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002F1A0 File Offset: 0x0002D3A0
	public virtual bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _face);
		}
		else
		{
			if (!this.IsCollideMovement)
			{
				return false;
			}
			if (this.BlocksMovement == 0)
			{
				return this.shape.IsMovementBlocked(_blockValue, _face);
			}
			return this.BlocksMovement == 1;
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0002F248 File Offset: 0x0002D448
	public virtual bool IsSeeThrough(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return !this.IsCollideSight && !_world.IsWater(_blockPos);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			Log.Error("IsSeeThrough {0} at {1} has child parent, {2} at {3} ", new object[]
			{
				this,
				_blockPos,
				block.Block,
				parentPos
			});
			return true;
		}
		return this.IsSeeThrough(_world, parentPos, block);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0002F2D8 File Offset: 0x0002D4D8
	public virtual bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _sides);
		}
		else
		{
			if (_sides == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 0; i <= 5; i++)
			{
				if ((1 << i & (int)_sides) != 0 && !this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0002F390 File Offset: 0x0002D590
	public bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, Vector3 _entityPos)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _entityPos);
		}
		else
		{
			BlockFaceFlag blockFaceFlag = BlockFaceFlags.FrontSidesFromPosition(_blockPos, _entityPos);
			if (blockFaceFlag == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 2; i <= 5; i++)
			{
				if ((1 << i & (int)blockFaceFlag) != 0 && !this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0002F44C File Offset: 0x0002D64C
	public bool IsMovementBlockedAny(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, Vector3 _entityPos)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlockedAny {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlockedAny(_world, parentPos, block, _entityPos);
		}
		else
		{
			BlockFaceFlag blockFaceFlag = BlockFaceFlags.FrontSidesFromPosition(_blockPos, _entityPos);
			if (blockFaceFlag == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 2; i <= 5; i++)
			{
				if ((1 << i & (int)blockFaceFlag) != 0 && this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0002F508 File Offset: 0x0002D708
	public virtual float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue blockDef, BlockFace stepFace)
	{
		if (!this.IsCollideMovement)
		{
			return 0f;
		}
		return this.shape.GetStepHeight(blockDef, stepFace);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0002F528 File Offset: 0x0002D728
	public float MinStepHeight(BlockValue blockDef, BlockFaceFlag stepSides)
	{
		float num = -1f;
		for (int i = 2; i <= 5; i++)
		{
			if ((1 << i & (int)stepSides) != 0)
			{
				if (num < 0f)
				{
					num = this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i);
				}
				else
				{
					num = Math.Min(num, this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i));
				}
			}
		}
		return Math.Max(num, 0f);
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0002F58C File Offset: 0x0002D78C
	public float MaxStepHeight(BlockValue blockDef, BlockFaceFlag stepSides)
	{
		float num = -1f;
		for (int i = 2; i <= 5; i++)
		{
			if ((1 << i & (int)stepSides) != 0)
			{
				if (num < 0f)
				{
					num = this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i);
				}
				else
				{
					num = Math.Max(num, this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i));
				}
			}
		}
		return Math.Max(num, 0f);
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0002F5F0 File Offset: 0x0002D7F0
	public float MinStepHeight(Vector3i blockPos, BlockValue blockDef, Vector3 entityPos)
	{
		BlockFaceFlag stepSides = BlockFaceFlags.FrontSidesFromPosition(blockPos, entityPos);
		return this.MinStepHeight(blockDef, stepSides);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0002F610 File Offset: 0x0002D810
	public float MaxStepHeight(Vector3i blockPos, BlockValue blockDef, Vector3 entityPos)
	{
		BlockFaceFlag stepSides = BlockFaceFlags.FrontSidesFromPosition(blockPos, entityPos);
		return this.MaxStepHeight(blockDef, stepSides);
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0002F62D File Offset: 0x0002D82D
	public virtual float GetHardness()
	{
		return this.blockMaterial.Hardness.Value;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0002F640 File Offset: 0x0002D840
	public virtual int GetWeight()
	{
		int result = 0;
		if (this.Weight != null)
		{
			result = this.Weight.Value;
		}
		return result;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0002F664 File Offset: 0x0002D864
	public Block.UVMode GetUVMode(int side, int channel)
	{
		return (Block.UVMode)((ulong)(this.UVModesPerSide[channel] >> side * this.cUVModeBits) & (ulong)((long)this.cUVModeMask));
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x0002F684 File Offset: 0x0002D884
	public virtual Rect getUVRectFromSideAndMetadata(int _meshIndex, BlockFace _side, Vector3[] _vertices, BlockValue _blockValue)
	{
		return this.getUVRectFromSideAndMetadata(_meshIndex, _side, (_vertices != null) ? _vertices[0] : Vector3.zero, _blockValue);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0002F6A4 File Offset: 0x0002D8A4
	public virtual Rect getUVRectFromSideAndMetadata(int _meshIndex, BlockFace _side, Vector3 _worldPos, BlockValue _blockValue)
	{
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		if (sideTextureId < 0)
		{
			return UVRectTiling.Empty.uv;
		}
		UVRectTiling[] uvMapping = MeshDescription.meshes[_meshIndex].textureAtlas.uvMapping;
		if (sideTextureId >= uvMapping.Length)
		{
			return UVRectTiling.Empty.uv;
		}
		UVRectTiling uvrectTiling = uvMapping[sideTextureId];
		if (uvrectTiling.blockW == 1 && uvrectTiling.blockH == 1)
		{
			return uvrectTiling.uv;
		}
		float x = _worldPos.x;
		float y = _worldPos.y;
		float z = _worldPos.z;
		switch (_side)
		{
		case BlockFace.Top:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.Bottom:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.North:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.West:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.South:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.East:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		default:
			return new Rect(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0002FA38 File Offset: 0x0002DC38
	public virtual void GetCollidingAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, Bounds _aabb, List<Bounds> _aabbList)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedAddY, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			Bounds bounds = Block.staticList_IntersectRayWithBlockList[i];
			if (_aabb.Intersects(bounds))
			{
				_aabbList.Add(bounds);
			}
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0002FA94 File Offset: 0x0002DC94
	public virtual bool HasCollidingAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, Bounds _aabb)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedAddY, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			Bounds bounds = Block.staticList_IntersectRayWithBlockList[i];
			if (_aabb.Intersects(bounds))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0002FAEC File Offset: 0x0002DCEC
	public virtual void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, List<Bounds> _result)
	{
		Vector3 b = new Vector3(0f, _distortedAddY, 0f);
		foreach (Bounds item in this.shape.GetBounds(_blockValue))
		{
			item.center += new Vector3((float)_x, (float)_y, (float)_z);
			item.max += b;
			_result.Add(item);
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0002FB68 File Offset: 0x0002DD68
	public virtual IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		return this.shape.GetBounds(_blockValue);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		return false;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void DoExchangeAction(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, string _action, int _itemCount)
	{
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0002FB76 File Offset: 0x0002DD76
	public virtual void OnBlockLoaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockLoaded(_world, _blockPos, _blockValue);
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0002FB8F File Offset: 0x0002DD8F
	public virtual void OnBlockUnloaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockUnloaded(_world, _blockPos, _blockValue);
		}
		if (this.RefundOnUnload)
		{
			GameEventManager.Current.RefundSpawnedBlock(_blockPos);
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnNeighborBlockChange(WorldBase world, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0002FBBC File Offset: 0x0002DDBC
	public static bool CanFallBelow(WorldBase _world, int _x, int _y, int _z)
	{
		BlockValue block = _world.GetBlock(_x, _y - 1, _z);
		Block block2 = block.Block;
		return block.isair || !block2.StabilitySupport;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0002FBF1 File Offset: 0x0002DDF1
	public virtual ulong GetTickRate()
	{
		return 10UL;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0002FBF8 File Offset: 0x0002DDF8
	public virtual void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		this.shape.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
		MultiBlockManager.TrackedBlockData trackedBlockData;
		if (this.isMultiBlock && !MultiBlockManager.Instance.TryGetPOIMultiBlock(_blockPos, out trackedBlockData))
		{
			this.multiBlockPos.AddChilds(_world, _chunk, _blockPos, _blockValue);
		}
		if (this.IsTemporaryBlock)
		{
			ChunkCustomData chunkCustomData;
			if (!_chunk.ChunkCustomData.dict.TryGetValue("temporaryblocks", out chunkCustomData))
			{
				chunkCustomData = new ChunkBlockClearData("temporaryblocks", 0UL, false, _world as World);
				_chunk.ChunkCustomData.Add("temporaryblocks", chunkCustomData);
			}
			(chunkCustomData as ChunkBlockClearData).BlockList.Add(World.toBlock(_blockPos));
		}
		if (!string.IsNullOrEmpty(this.blockAddedEvent))
		{
			GameEventManager.Current.HandleAction(this.blockAddedEvent, null, null, false, _blockPos, "", "", false, true, "", null, null);
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockReset(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0002FCDC File Offset: 0x0002DEDC
	public virtual void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
			if (this.isMultiBlock)
			{
				this.multiBlockPos.RemoveChilds(_world, _blockPos, _blockValue);
			}
			ChunkCustomData chunkCustomData;
			if (this.IsTemporaryBlock && _chunk.ChunkCustomData.dict.TryGetValue("temporaryblocks", out chunkCustomData))
			{
				(chunkCustomData as ChunkBlockClearData).BlockList.Remove(World.toBlock(_blockPos));
				return;
			}
		}
		else if (this.isMultiBlock)
		{
			this.multiBlockPos.RemoveParentBlock(_world, _blockPos, _blockValue);
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0002FD6C File Offset: 0x0002DF6C
	public virtual void OnBlockValueChanged(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (_oldBlockValue.ischild)
		{
			return;
		}
		this.shape.OnBlockValueChanged(_world, _blockPos, _oldBlockValue, _newBlockValue);
		if (this.isMultiBlock && _oldBlockValue.rotation != _newBlockValue.rotation)
		{
			this.multiBlockPos.RemoveChilds(_world, _blockPos, _oldBlockValue);
			this.multiBlockPos.AddChilds(_world, _chunk, _blockPos, _newBlockValue);
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0002FDCB File Offset: 0x0002DFCB
	public virtual void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0002FDE0 File Offset: 0x0002DFE0
	public virtual void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		_ebcd.UpdateTemperature();
		this.ForceAnimationState(_blockValue, _ebcd);
		if (this.GroundAlignDistance != 0f)
		{
			((World)_world).m_ChunkManager.AddGroundAlignBlock(_ebcd);
		}
		if (_world.TryRetrieveAndRemovePendingDowngradeBlock(_blockPos) && !string.IsNullOrEmpty(this.blockDowngradedToEvent))
		{
			GameEventManager.Current.HandleAction(this.blockDowngradedToEvent, null, null, false, _blockPos, "", "", false, true, "", null, null);
		}
		if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				MultiBlockManager.Instance.TryRegisterTerrainAlignedBlock(_blockPos, _blockValue);
			}
			MultiBlockManager.Instance.SetTerrainAlignmentDirty(_blockPos);
		}
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0002FEA0 File Offset: 0x0002E0A0
	public virtual int DamageBlock(WorldBase _world, BlockValueRef _blockValueRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo = null, bool _bUseHarvestTool = false, bool _bBypassMaxDamage = false)
	{
		return this.OnBlockDamaged(_world, _blockValueRef, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, 0);
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0002FEC4 File Offset: 0x0002E0C4
	public virtual int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return 0;
		}
		if (this.isMultiBlock && _blockValue.ischild && _bvRef.Type == BlockValueRefType.Block)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_bvRef.BlockPosition, _blockValue);
			BlockValue block = chunkCache.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("Block on position {0} with name '{1}' should be a parent but is not! (6)", new object[]
				{
					parentPos,
					block.Block.blockName
				});
				return 0;
			}
			return block.Block.OnBlockDamaged(_world, parentPos, block, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
		}
		else
		{
			Block block2 = _blockValue.Block;
			int damage = _blockValue.damage;
			bool flag = damage >= block2.MaxDamage;
			int num = damage + _damagePoints;
			chunkCache.InvokeOnBlockDamagedDelegates(_bvRef, _blockValue, _damagePoints, _entityIdThatDamaged);
			if (num < 0)
			{
				if (!this.UpgradeBlock.isair)
				{
					BlockValue blockValue = this.UpgradeBlock;
					blockValue = BlockPlaceholderMap.Instance.Replace(_bvRef, blockValue, _world.GetGameRandom(), false);
					blockValue.rotation = this.convertRotation(_blockValue, blockValue);
					blockValue.meta = _blockValue.meta;
					blockValue.damage = 0;
					Block block3 = blockValue.Block;
					if (!block3.shape.IsTerrain())
					{
						_world.SetBlockRPC(_bvRef, blockValue);
						if (chunkCache.GetTextureFull(_bvRef) != 0L)
						{
							GameManager.Instance.SetBlockTextureServer(_bvRef, BlockFace.None, 0, _entityIdThatDamaged, byte.MaxValue);
						}
					}
					else
					{
						_world.SetBlockRPC(_bvRef, blockValue, block3.Density);
					}
					DynamicMeshManager.ChunkChanged(_bvRef, _entityIdThatDamaged, _blockValue.type);
					return blockValue.damage;
				}
				if (_blockValue.damage != 0)
				{
					_blockValue.damage = 0;
					_world.SetBlockRPC(_bvRef, _blockValue);
				}
				return 0;
			}
			else
			{
				if (this.Stage2Health > 0)
				{
					int num2 = block2.MaxDamage - this.Stage2Health;
					if (damage < num2 && num >= num2)
					{
						num = num2;
					}
				}
				if (!flag && num >= block2.MaxDamage)
				{
					int num3 = num - block2.MaxDamage;
					DynamicMeshManager.ChunkChanged(_bvRef, _entityIdThatDamaged, _blockValue.type);
					Block.DestroyedResult destroyedResult = this.OnBlockDestroyedBy(_world, _bvRef, _blockValue, _entityIdThatDamaged, _bUseHarvestTool);
					if (destroyedResult != Block.DestroyedResult.Keep)
					{
						if (!this.DowngradeBlock.isair && destroyedResult == Block.DestroyedResult.Downgrade)
						{
							if (_recDepth == 0)
							{
								this.SpawnDowngradeFX(_world, _blockValue, _bvRef, block2.tintColor, _entityIdThatDamaged);
							}
							BlockValue blockValue2 = this.DowngradeBlock;
							blockValue2 = BlockPlaceholderMap.Instance.Replace(_bvRef, blockValue2, _world.GetGameRandom(), false);
							blockValue2.rotation = _blockValue.rotation;
							blockValue2.meta = _blockValue.meta;
							Block block4 = blockValue2.Block;
							if (!block4.shape.IsTerrain())
							{
								_world.SetBlockRPC(_bvRef, blockValue2);
								if (chunkCache.GetTextureFull(_bvRef) != 0L)
								{
									if (this.RemovePaintOnDowngrade == null)
									{
										GameManager.Instance.SetBlockTextureServer(_bvRef, BlockFace.None, 0, _entityIdThatDamaged, byte.MaxValue);
									}
									else
									{
										for (int i = 0; i < this.RemovePaintOnDowngrade.Count; i++)
										{
											GameManager.Instance.SetBlockTextureServer(_bvRef, this.RemovePaintOnDowngrade[i], 0, _entityIdThatDamaged, byte.MaxValue);
										}
									}
								}
								_world.AddPendingDowngradeBlock(_bvRef);
								if (!string.IsNullOrEmpty(this.blockDowngradeEvent))
								{
									Entity entity = _world.GetEntity(_entityIdThatDamaged);
									EntityVehicle entityVehicle = entity as EntityVehicle;
									if (entityVehicle != null)
									{
										entity = entityVehicle.GetFirstAttached();
									}
									GameEventManager.Current.HandleAction(this.blockDowngradeEvent, null, entity as EntityPlayer, false, _bvRef, "", "", false, true, "", null, null);
								}
							}
							else
							{
								_world.SetBlockRPC(_bvRef, blockValue2, block4.Density);
							}
							if ((num3 > 0 && this.EnablePassThroughDamage) || _bBypassMaxDamage)
							{
								block4.OnBlockDamaged(_world, _bvRef, blockValue2, num3, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
							}
						}
						else
						{
							Entity entity2 = _world.GetEntity(_entityIdThatDamaged);
							QuestEventManager.Current.BlockDestroyed(block2, _bvRef, entity2);
							this.SpawnDestroyFX(_world, _blockValue, _bvRef, this.GetColorForSide(_blockValue, BlockFace.Top), _entityIdThatDamaged);
							_world.SetBlockRPC(_bvRef, BlockValue.Air);
							if (!string.IsNullOrEmpty(this.blockDestroyedEvent))
							{
								Entity entity3 = _world.GetEntity(_entityIdThatDamaged);
								EntityVehicle entityVehicle2 = entity3 as EntityVehicle;
								if (entityVehicle2 != null)
								{
									entity3 = entityVehicle2.GetFirstAttached();
								}
								GameEventManager.Current.HandleAction(this.blockDestroyedEvent, null, entity3 as EntityPlayer, false, _bvRef, "", "", false, true, "", null, null);
							}
						}
					}
					return block2.MaxDamage;
				}
				if (_blockValue.damage != num)
				{
					_blockValue.damage = num;
					if (!block2.shape.IsTerrain())
					{
						_world.SetBlocksRPC(new List<BlockChangeInfo>
						{
							new BlockChangeInfo(_bvRef, _blockValue, false, true)
						});
					}
					else
					{
						sbyte density = _world.GetDensity(_bvRef);
						sbyte b = (sbyte)Utils.FastMin(-1f, (float)MarchingCubes.DensityTerrain * (1f - (float)num / (float)block2.MaxDamage));
						if ((_damagePoints > 0 && b > density) || (_damagePoints < 0 && b < density))
						{
							_world.SetBlockRPC(_bvRef, _blockValue, b);
						}
						else
						{
							_world.SetBlockRPC(_bvRef, _blockValue);
						}
					}
					if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
					{
						MultiBlockManager.Instance.SetTerrainAlignmentDirty(_bvRef);
					}
				}
				return _blockValue.damage;
			}
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x000303D8 File Offset: 0x0002E5D8
	public virtual bool IsHealthShownInUI(HitInfoDetails _hit, BlockValue _bv)
	{
		if (this.isMultiBlock && _bv.ischild)
		{
			Vector3i vector3i = _hit.blockPos + _bv.parent;
			BlockValue block = GameManager.Instance.World.ChunkCache.GetBlock(vector3i);
			if (block.ischild)
			{
				Log.Error("Block on position {0} with name '{1}' should be a parent but is not! (6)", new object[]
				{
					vector3i,
					block.Block.blockName
				});
				return false;
			}
			return block.Block.IsHealthShownInUI(_hit, block);
		}
		else
		{
			if (this.Stage2Health > 0)
			{
				return _bv.Block.MaxDamage - _bv.damage > this.Stage2Health;
			}
			return _bv.damage > 0;
		}
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0003048F File Offset: 0x0002E68F
	[PublicizedFrom(EAccessModifier.Private)]
	public byte convertRotation(BlockValue _oldBV, BlockValue _newBV)
	{
		return _oldBV.rotation;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00030498 File Offset: 0x0002E698
	public void AddDroppedId(EnumDropEvent _eEvent, string _name, int _minCount, int _maxCount, float _prob, float _resourceScale, float _stickChance, string _toolCategory, string _tag)
	{
		List<Block.SItemDropProb> list;
		this.itemsToDrop.TryGetValue(_eEvent, out list);
		if (list == null)
		{
			list = new List<Block.SItemDropProb>();
			this.itemsToDrop[_eEvent] = list;
		}
		list.Add(new Block.SItemDropProb(_name, _minCount, _maxCount, _prob, _resourceScale, _stickChance, _toolCategory, _tag));
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000304E3 File Offset: 0x0002E6E3
	public bool HasItemsToDropForEvent(EnumDropEvent _eEvent)
	{
		return this.itemsToDrop.ContainsKey(_eEvent);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x000304F4 File Offset: 0x0002E6F4
	public void DropItemsOnEvent(WorldBase _world, BlockValue _blockValue, EnumDropEvent _eEvent, float _overallProb, Vector3 _dropPos, Vector3 _randomPosAdd, float _lifetime, int _entityId, bool _bGetSameItemIfNoneFound)
	{
		GameRandom gameRandom = _world.GetGameRandom();
		this.itemsDropped.Clear();
		List<Block.SItemDropProb> list;
		if (!this.itemsToDrop.TryGetValue(_eEvent, out list))
		{
			if (_bGetSameItemIfNoneFound)
			{
				ItemValue itemValue = _blockValue.ToItemValue();
				this.itemsDropped.Add(new ItemStack(itemValue, 1));
			}
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				Block.SItemDropProb sitemDropProb = list[i];
				int num = gameRandom.RandomRange(sitemDropProb.minCount, sitemDropProb.maxCount + 1);
				if (num > 0)
				{
					if (sitemDropProb.stickChance < 0.001f || gameRandom.RandomFloat > sitemDropProb.stickChance)
					{
						if (sitemDropProb.name.Equals("[recipe]"))
						{
							List<Recipe> recipes = CraftingManager.GetRecipes(_blockValue.Block.GetBlockName());
							if (recipes.Count > 0)
							{
								for (int j = 0; j < recipes[0].ingredients.Count; j++)
								{
									if (recipes[0].ingredients[j].count / 2 > 0)
									{
										ItemStack item = new ItemStack(recipes[0].ingredients[j].itemValue, recipes[0].ingredients[j].count / 2);
										this.itemsDropped.Add(item);
									}
								}
							}
						}
						else
						{
							ItemValue itemValue2 = sitemDropProb.name.Equals("*") ? _blockValue.ToItemValue() : new ItemValue(ItemClass.GetItem(sitemDropProb.name, false).type, false);
							if (!itemValue2.IsEmpty() && sitemDropProb.prob > gameRandom.RandomFloat)
							{
								this.itemsDropped.Add(new ItemStack(itemValue2, num));
							}
						}
					}
					else
					{
						Vector3i vector3i = World.worldToBlockPos(_dropPos);
						if ((World.SandboxUseTraderArea != TraderAreaStates.Default || !GameManager.Instance.World.IsWithinTraderArea(vector3i)) && (_overallProb >= 0.999f || gameRandom.RandomFloat < _overallProb))
						{
							BlockValue blockValue = Block.GetBlockValue(sitemDropProb.name, false);
							if (!blockValue.isair && _world.GetBlock(vector3i).isair)
							{
								_world.SetBlockRPC(vector3i, blockValue);
							}
						}
					}
				}
			}
		}
		for (int k = 0; k < this.itemsDropped.Count; k++)
		{
			if (_overallProb >= 0.999f || gameRandom.RandomFloat < _overallProb)
			{
				ItemClass itemClass = this.itemsDropped[k].itemValue.ItemClass;
				_lifetime = ((_lifetime > 0.001f) ? _lifetime : ((itemClass != null) ? itemClass.GetLifetimeOnDrop() : 0f));
				if (_lifetime > 0.001f)
				{
					_world.GetGameManager().ItemDropServer(this.itemsDropped[k], _dropPos, _randomPosAdd, _entityId, _lifetime, false);
				}
			}
		}
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000307DF File Offset: 0x0002E9DF
	public float GetExplosionResistance()
	{
		return this.blockMaterial.ExplosionResistance;
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x000307EC File Offset: 0x0002E9EC
	public bool intersectRayWithBlock(BlockValue _blockValue, int _x, int _y, int _z, Ray _ray, out Vector3 _hitPoint, World _world)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, 0f, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			if (Block.staticList_IntersectRayWithBlockList[i].IntersectRay(_ray))
			{
				_hitPoint = new Vector3((float)_x, (float)_y, (float)_z);
				return true;
			}
		}
		_hitPoint = Vector3.zero;
		return false;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00030868 File Offset: 0x0002EA68
	public virtual Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache != null)
		{
			chunkCache.InvokeOnBlockDamagedDelegates(_bvRef, _blockValue, _blockValue.Block.MaxDamage, _playerThatStartedExpl);
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00030896 File Offset: 0x0002EA96
	public virtual void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		_world.SetBlockRPC(_blockPos, BlockValue.Air);
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000308AC File Offset: 0x0002EAAC
	public virtual bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (_blockPos.y > 253)
		{
			return false;
		}
		Block block = _blockValue.Block;
		if (!GameManager.Instance.IsEditMode())
		{
			if (!block.isMultiBlock)
			{
				if (((World)_world).IsWithinTraderPlacingProtection(_blockPos))
				{
					return false;
				}
			}
			else
			{
				Bounds bounds = block.multiBlockPos.CalcBounds(_blockValue.type, (int)_blockValue.rotation);
				bounds.center += _blockPos.ToVector3();
				if (((World)_world).IsWithinTraderPlacingProtection(bounds))
				{
					return false;
				}
			}
		}
		return (!block.isMultiBlock || _blockPos.y + block.multiBlockPos.dim.y < 254) && (GameManager.Instance.IsEditMode() || !block.bRestrictSubmergedPlacement || !this.IsUnderwater(_world, _blockPos, _blockValue)) && (GameManager.Instance.IsEditMode() || _bOmitCollideCheck || !this.overlapsWithOtherBlock(_world, _blockPos, _blockValue));
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000309A0 File Offset: 0x0002EBA0
	public Vector3i GetFreePlacementPosition(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityPlacing)
	{
		Vector3i vector3i = _blockPos;
		int num = 15;
		while (_blockValue.Block.overlapsWithOtherBlock(_world, vector3i, _blockValue))
		{
			Vector3 direction = _entityPlacing.getHeadPosition() - (vector3i.ToVector3() + Vector3.one * 0.5f);
			Vector3 vector;
			BlockFace blockFace;
			vector3i = Voxel.OneVoxelStep(vector3i, vector3i.ToVector3() + Vector3.one * 0.5f, direction, out vector, out blockFace);
			if (--num <= 0)
			{
				break;
			}
		}
		if (num <= 0)
		{
			vector3i = _blockPos;
		}
		return vector3i;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00030A28 File Offset: 0x0002EC28
	[PublicizedFrom(EAccessModifier.Private)]
	public bool overlapsWithOtherBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock)
		{
			int type = _world.GetBlock(_blockPos).type;
			return type != 0 && !Block.list[type].CanBlocksReplaceOrGroundCover();
		}
		byte rotation = _blockValue.rotation;
		for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
		{
			Vector3i pos = _blockPos + this.multiBlockPos.Get(i, _blockValue.type, (int)rotation);
			int type2 = _world.GetBlock(pos).type;
			if (type2 != 0 && !Block.list[type2].CanBlocksReplaceOrGroundCover())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00030AC6 File Offset: 0x0002ECC6
	public bool CanBlocksReplaceOrGroundCover()
	{
		return this.CanBlocksReplace || this.blockMaterial.IsGroundCover;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00030AE0 File Offset: 0x0002ECE0
	public bool IsUnderwater(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (this.isMultiBlock)
		{
			int num = _blockPos.y + this.multiBlockPos.dim.y - 1;
			for (int i = 0; i < this.multiBlockPos.Length; i++)
			{
				Vector3i vector3i = _blockPos + this.multiBlockPos.Get(i, _blockValue.type, (int)_blockValue.rotation);
				if (vector3i.y == num && _world.IsWater(vector3i))
				{
					return true;
				}
			}
		}
		else if (_world.IsWater(_blockPos))
		{
			return true;
		}
		return false;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00030B68 File Offset: 0x0002ED68
	public virtual BlockValue OnBlockPlaced(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		return _blockValue;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00030B6C File Offset: 0x0002ED6C
	public virtual void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		DynamicMeshManager.ChunkChanged(_bpResult.blockPos, (_ea != null) ? _ea.entityId : -2, _bpResult.blockValue.type);
		if (this.SelectAlternates)
		{
			byte rotation = _bpResult.blockValue.rotation;
			_bpResult.blockValue = _bpResult.blockValue.Block.GetAltBlockValue(_ea.inventory.holdingItemItemValue.Meta);
			_bpResult.blockValue.rotation = rotation;
		}
		else
		{
			string placeAltBlockValue = this.GetPlaceAltBlockValue(_world);
			_bpResult.blockValue = ((placeAltBlockValue.Length == 0) ? _bpResult.blockValue : Block.GetBlockValue(placeAltBlockValue, false));
		}
		Block block = _bpResult.blockValue.Block;
		if (block.PlaceRandomRotation)
		{
			int num;
			bool flag;
			do
			{
				num = _rnd.RandomRange(28);
				if (num < 4)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Basic90) > EBlockRotationClasses.None);
				}
				else if (num < 8)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Headfirst) > EBlockRotationClasses.None);
				}
				else if (num < 24)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Sideways) > EBlockRotationClasses.None);
				}
				else
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Basic45) > EBlockRotationClasses.None);
				}
			}
			while (!flag);
			_bpResult.blockValue.rotation = (byte)num;
		}
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00030C80 File Offset: 0x0002EE80
	public virtual void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		Block block = _result.blockValue.Block;
		int changingEntityId = (_ea == null) ? -1 : _ea.entityId;
		if (block.shape.IsTerrain())
		{
			_world.SetBlockRPC(_result.blockPos, _result.blockValue, this.Density, changingEntityId);
		}
		else if (!block.IsTerrainDecoration)
		{
			_world.SetBlockRPC(_result.blockPos, _result.blockValue, MarchingCubes.DensityAir, changingEntityId);
		}
		else
		{
			_world.SetBlockRPC(_result.blockPos, _result.blockValue, changingEntityId);
		}
		if (this.blockName.Equals("keystoneBlock") && _ea is EntityPlayerLocal)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager == null)
			{
				return;
			}
			achievementManager.SetAchievementStat(EnumAchievementDataStat.LandClaimPlaced, 1);
		}
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00030D4C File Offset: 0x0002EF4C
	public virtual void PlaceProp(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		BlockPlacement.EnumPlacement placement = _result.placement;
		Vector3 vector;
		if (placement != BlockPlacement.EnumPlacement.Voxel)
		{
			if (placement != BlockPlacement.EnumPlacement.Free)
			{
				throw new ArgumentOutOfRangeException();
			}
			vector = _result.pos - _result.blockPos + _result.blockFace.GetNormal() * 0.5f;
		}
		else
		{
			vector = new Vector3(0.5f, 0.5f, 0.5f);
		}
		Vector3 b = vector;
		Vector3 vector2 = _result.blockPos + b + _result.propTransform.position - new Vector3(0.5f, 0.5f, 0.5f);
		Vector3i vector3i = new Vector3i(0, 0, 0);
		vector3i.FloorToInt(vector2);
		int chunkX = World.toChunkXZ(vector3i.x);
		int chunkZ = World.toChunkXZ(vector3i.z);
		_world.SetPropRPC(new PropChangeInfo.Builder(chunkX, chunkZ).SetPosition(vector2 - World.toChunkXyzWorldPos(vector3i)).SetRotation(_result.propTransform.rotation).SetScale(_result.propTransform.scale).SetBlockValue(_result.blockValue).Build());
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0002F184 File Offset: 0x0002D384
	public virtual Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00030E78 File Offset: 0x0002F078
	public virtual ItemStack OnBlockPickedUp(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		ItemValue itemValue;
		if (this.PickupTarget != null)
		{
			itemValue = new ItemValue(ItemClass.GetItem(this.PickupTarget, false).type, false);
		}
		else if (this.PickedUpItemValue != null)
		{
			if (this.PickedUpItemValue.Contains(","))
			{
				string[] array = this.PickedUpItemValue.Split(',', StringSplitOptions.None);
				itemValue = ItemClass.CreateItemValue(array[0], (array.Length >= 2) ? int.Parse(array[1]) : 1, false);
			}
			else
			{
				itemValue = ItemClass.GetItem(this.PickedUpItemValue, false);
			}
		}
		else
		{
			itemValue = _blockValue.ToItemValue();
		}
		return new ItemStack(itemValue, 1);
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00030F0C File Offset: 0x0002F10C
	public virtual bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		bool flag = this.CanPickup;
		Block block = _blockValue.Block;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _player, null, block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
		{
			_player.PlayOneShot("keystone_impact_overlay", false, false, false, null, 1f);
			return false;
		}
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player, Localization.Get("ttRepairBeforePickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return false;
		}
		ItemStack itemStack = block.OnBlockPickedUp(_world, _blockPos, _blockValue, _player.entityId);
		if (!_player.inventory.CanTakeItem(itemStack) && !_player.bag.CanTakeItem(itemStack))
		{
			GameManager.ShowTooltip(_player, Localization.Get("xuiInventoryFullForPickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return false;
		}
		_world.GetGameManager().PickupBlockServer(_blockPos, _blockValue, _player.entityId, null);
		return false;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00031020 File Offset: 0x0002F220
	public void PickupOrDrop(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayer _player, bool forcePickup)
	{
		if (_player == null)
		{
			return;
		}
		bool flag = this.CanPickup;
		Block block = _blockValue.Block;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _player, null, block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag = true;
		}
		if (!flag && !forcePickup)
		{
			return;
		}
		if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
		{
			return;
		}
		_world.GetGameManager().PickupBlockServer(_blockPos, _blockValue, _player.entityId, null);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool OnEntityCollidedWithBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, Entity _entity)
	{
		return false;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CanPlantStay(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000310A3 File Offset: 0x0002F2A3
	public void SetBlockName(string _blockName)
	{
		this.blockName = _blockName;
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000310AC File Offset: 0x0002F2AC
	public string GetBlockName()
	{
		return this.blockName;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000310B4 File Offset: 0x0002F2B4
	public static HashSet<string> GetAutoShapeMaterials()
	{
		return Block.autoShapeMaterials;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000310BB File Offset: 0x0002F2BB
	public EAutoShapeType GetAutoShapeType()
	{
		return this.AutoShapeType;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x000310C3 File Offset: 0x0002F2C3
	public string GetAutoShapeBlockName()
	{
		return this.autoShapeBaseName;
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000310CB File Offset: 0x0002F2CB
	public string GetAutoShapeShapeName()
	{
		return this.autoShapeShapeName;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x000310D3 File Offset: 0x0002F2D3
	public Block GetAutoShapeHelperBlock()
	{
		return this.autoShapeHelper;
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x000310DB File Offset: 0x0002F2DB
	public string GetLocalizedAutoShapeShapeName()
	{
		return Localization.Get("shape" + this.GetAutoShapeShapeName(), false, null);
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x000310F4 File Offset: 0x0002F2F4
	public bool AutoShapeSupportsShapeName(string _shapeName)
	{
		return this.AutoShapeType == EAutoShapeType.Helper && this.ContainsAlternateBlock(this.autoShapeBaseName + ":" + _shapeName);
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00031118 File Offset: 0x0002F318
	public int AutoShapeAlternateShapeNameIndex(string _shapeName)
	{
		if (this.AutoShapeType == EAutoShapeType.Helper)
		{
			return this.GetAlternateBlockIndex(this.autoShapeBaseName + ":" + _shapeName);
		}
		return -1;
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0003113C File Offset: 0x0002F33C
	public virtual string GetLocalizedBlockName()
	{
		if (this.localizedBlockName != null)
		{
			return this.localizedBlockName;
		}
		if (this.AutoShapeType != EAutoShapeType.None)
		{
			return this.localizedBlockName = this.blockMaterial.GetLocalizedMaterialName() + " - " + this.GetLocalizedAutoShapeShapeName();
		}
		return this.localizedBlockName = Localization.Get(this.GetBlockName(), false, null);
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0003119C File Offset: 0x0002F39C
	public virtual string GetLocalizedBlockName(ItemValue _itemValueRef)
	{
		if (this.AutoShapeType != EAutoShapeType.Helper || _itemValueRef.ToBlockValue(false).Equals(BlockValue.Air))
		{
			return this.GetLocalizedBlockName();
		}
		this.GetAltBlocks();
		return this.placeAltBlockClasses[_itemValueRef.Meta].GetLocalizedBlockName();
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x000311E8 File Offset: 0x0002F3E8
	public string GetIconName()
	{
		return this.CustomIcon ?? this.GetBlockName();
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x000311FA File Offset: 0x0002F3FA
	public void SetSideTextureId(int _textureId, int channel)
	{
		this.textureInfos[channel].singleTextureId = _textureId;
		this.textureInfos[channel].bTextureForEachSide = false;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00031220 File Offset: 0x0002F420
	public void SetSideTextureId(string[] _texIds, int channel)
	{
		this.textureInfos[channel].sideTextureIds = new int[_texIds.Length];
		for (int i = 0; i < _texIds.Length; i++)
		{
			this.textureInfos[channel].sideTextureIds[i] = int.Parse(_texIds[i]);
		}
		this.textureInfos[channel].bTextureForEachSide = true;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00031284 File Offset: 0x0002F484
	public int GetSideTextureId(BlockValue _blockValue, BlockFace _side, int channel)
	{
		if (this.textureInfos[channel].bTextureForEachSide)
		{
			int num = this.shape.MapSideAndRotationToTextureIdx(_blockValue, _side);
			if (num >= this.textureInfos[channel].sideTextureIds.Length)
			{
				num = 0;
			}
			return this.textureInfos[channel].sideTextureIds[num];
		}
		return this.textureInfos[channel].singleTextureId;
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x000312F0 File Offset: 0x0002F4F0
	public MaterialBlock GetMaterialForSide(BlockValue _blockValue, BlockFace _side)
	{
		MaterialBlock materialBlock = null;
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		Block block = _blockValue.Block;
		if (sideTextureId != -1 && MeshDescription.meshes[(int)block.MeshIndex].textureAtlas.uvMapping.Length > sideTextureId)
		{
			materialBlock = MeshDescription.meshes[(int)block.MeshIndex].textureAtlas.uvMapping[sideTextureId].material;
		}
		if (materialBlock == null)
		{
			materialBlock = block.blockMaterial;
		}
		return materialBlock;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0003135D File Offset: 0x0002F55D
	public int GetUiBackgroundTextureId(BlockValue _blockValue, BlockFace _side, int channel = 0)
	{
		if (this.uiBackgroundTextureId < 0)
		{
			return this.GetSideTextureId(_blockValue, _side, channel);
		}
		return this.uiBackgroundTextureId;
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x00031378 File Offset: 0x0002F578
	public string GetParticleForSide(BlockValue _blockValue, BlockFace _side)
	{
		MaterialBlock materialForSide = this.GetMaterialForSide(_blockValue, _side);
		if (materialForSide != null && materialForSide.ParticleCategory != null)
		{
			return materialForSide.ParticleCategory;
		}
		if (materialForSide != null && materialForSide.SurfaceCategory != null)
		{
			return materialForSide.SurfaceCategory;
		}
		return null;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x000313B4 File Offset: 0x0002F5B4
	public string GetDestroyParticle(BlockValue _blockValue)
	{
		if (this.blockMaterial.ParticleDestroyCategory != null)
		{
			return this.blockMaterial.ParticleDestroyCategory;
		}
		if (this.blockMaterial.ParticleCategory != null)
		{
			return this.blockMaterial.ParticleCategory;
		}
		if (this.blockMaterial.SurfaceCategory != null)
		{
			return this.blockMaterial.SurfaceCategory;
		}
		return null;
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00031410 File Offset: 0x0002F610
	public Color GetColorForSide(BlockValue _blockValue, BlockFace _side)
	{
		TextureAtlas textureAtlas = MeshDescription.meshes[(int)_blockValue.Block.MeshIndex].textureAtlas;
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		if (sideTextureId != -1 && textureAtlas.uvMapping.Length > sideTextureId)
		{
			return textureAtlas.uvMapping[sideTextureId].color;
		}
		return Color.gray;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00031468 File Offset: 0x0002F668
	public Color GetMapColor(BlockValue _blockValue, Vector3 _normal, int _yPos)
	{
		Color color;
		if (!this.bMapColorSet)
		{
			if (_normal.x > 0.5f || _normal.z > 0.5f || _normal.x < -0.5f || _normal.z < -0.5f)
			{
				color = this.GetColorForSide(_blockValue, BlockFace.South);
			}
			else
			{
				color = this.GetColorForSide(_blockValue, BlockFace.Top);
			}
		}
		else
		{
			color = this.MapColor;
		}
		float num = this.MapSpecular;
		if (this.bMapColor2Set && this.MapElevMinMax.y != this.MapElevMinMax.x)
		{
			float num2 = (float)Utils.FastMax(_yPos - this.MapElevMinMax.x, 0) / (float)(this.MapElevMinMax.y - this.MapElevMinMax.x);
			color = Color.Lerp(this.MapColor, this.MapColor2, num2);
			num = Utils.FastMax(num - num2 * 0.5f, 0f);
		}
		float num3 = (_normal.z + 1f) / 2f * (_normal.x + 1f) / 2f;
		num3 *= 2f;
		color = Utils.Saturate(color * 0.5f + color * num3);
		color.a = num;
		return color;
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x0003159E File Offset: 0x0002F79E
	public static bool CanDrop(BlockValue _blockValue)
	{
		return !_blockValue.Equals(BlockValue.Air);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsElevator()
	{
		return false;
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsElevator(int rotation)
	{
		return false;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x000315AF File Offset: 0x0002F7AF
	public virtual bool IsPlant()
	{
		return this.blockMaterial.IsPlant || this.bIsPlant;
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x000315C6 File Offset: 0x0002F7C6
	public bool HasTag(BlockTags _tag)
	{
		return this.BlockTag == _tag;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x000315D1 File Offset: 0x0002F7D1
	public bool HasAnyFastTags(FastTags<TagGroup.Global> _tags)
	{
		return this.Tags.Test_AnySet(_tags);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x000315DF File Offset: 0x0002F7DF
	public bool HasAllFastTags(FastTags<TagGroup.Global> _tags)
	{
		return this.Tags.Test_AllSet(_tags);
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x000315ED File Offset: 0x0002F7ED
	public virtual bool CanRepair(BlockValue _blockValue)
	{
		return _blockValue.damage > 0;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x000315F8 File Offset: 0x0002F7F8
	public virtual string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		Block block = _blockValue.Block;
		if (!this.CanPickup && EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) <= 0f)
		{
			return null;
		}
		if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
		{
			return null;
		}
		string text = block.GetBlockName();
		if (!string.IsNullOrEmpty(block.PickedUpItemValue))
		{
			text = block.PickedUpItemValue;
			int num = text.IndexOf(",");
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
		}
		else if (!string.IsNullOrEmpty(block.PickupTarget))
		{
			text = block.PickupTarget;
		}
		return string.Format(Localization.Get("pickupPrompt", false, null), Localization.Get(text, false, null));
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x000316C0 File Offset: 0x0002F8C0
	public void SpawnDowngradeFX(WorldBase _world, BlockValue _blockValue, BlockValueRef _bvRef, Color _color, int _entityIdThatCaused)
	{
		Block block = _blockValue.Block;
		if (block.DowngradeFX != null)
		{
			this.SpawnFX(_world, _bvRef, 1f, _color, _entityIdThatCaused, block.DowngradeFX);
			return;
		}
		this.SpawnDestroyParticleEffect(_world, _blockValue, _bvRef, 1f, _color, _entityIdThatCaused);
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00031708 File Offset: 0x0002F908
	public void SpawnDestroyFX(WorldBase _world, BlockValue _blockValue, BlockValueRef _bvRef, Color _color, int _entityIdThatCaused)
	{
		Block block = _blockValue.Block;
		if (block.DestroyFX != null)
		{
			this.SpawnFX(_world, _bvRef, 1f, _color, _entityIdThatCaused, block.DestroyFX);
			return;
		}
		this.SpawnDestroyParticleEffect(_world, _blockValue, _bvRef, 1f, _color, _entityIdThatCaused);
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00031750 File Offset: 0x0002F950
	public virtual void SpawnDestroyParticleEffect(WorldBase _world, BlockValue _blockValue, BlockValueRef _bvRef, float _lightValue, Color _color, int _entityIdThatCaused)
	{
		if (this.deathParticleName != null)
		{
			_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.deathParticleName, World.blockToTransformPos(_world, _bvRef) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, this.blockMaterial.SurfaceCategory + "destroy", null, true, 1f, ""), _entityIdThatCaused, false, true);
			return;
		}
		MaterialBlock materialForSide = this.GetMaterialForSide(_blockValue, BlockFace.Top);
		string destroyParticle = this.GetDestroyParticle(_blockValue);
		if (destroyParticle != null && materialForSide.SurfaceCategory != null)
		{
			_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("blockdestroy_" + destroyParticle, World.blockToTransformPos(_world, _bvRef) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, this.blockMaterial.SurfaceCategory + "destroy", null, true, 1f, ""), _entityIdThatCaused, false, true);
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00031848 File Offset: 0x0002FA48
	public void SpawnFX(WorldBase _world, BlockValueRef _bvRef, float _lightValue, Color _color, int _entityIdThatCaused, string _fxName)
	{
		string[] array = _fxName.Split(',', StringSplitOptions.None);
		_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(array[0], World.blockToTransformPos(_world, _bvRef) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, array[1], null, true, 1f, ""), _entityIdThatCaused, false, true);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x000318AC File Offset: 0x0002FAAC
	public static BlockValue GetBlockValue(string _blockName, bool _caseInsensitive = false)
	{
		Block blockByName = Block.GetBlockByName(_blockName, _caseInsensitive);
		if (blockByName != null)
		{
			return blockByName.ToBlockValue();
		}
		return BlockValue.Air;
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x000318D0 File Offset: 0x0002FAD0
	public static Block GetBlockByName(string _blockname, bool _caseInsensitive = false)
	{
		if (Block.nameToBlock == null)
		{
			return null;
		}
		Block result;
		if (_caseInsensitive)
		{
			Block.nameToBlockCaseInsensitive.TryGetValue(_blockname, out result);
		}
		else
		{
			Block.nameToBlock.TryGetValue(_blockname, out result);
		}
		return result;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00031908 File Offset: 0x0002FB08
	public BlockValue ToBlockValue()
	{
		return new BlockValue
		{
			type = this.blockID
		};
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0003192C File Offset: 0x0002FB2C
	public static BlockValue GetBlockValue(int _blockType)
	{
		if (Block.list[_blockType] == null)
		{
			return BlockValue.Air;
		}
		return new BlockValue
		{
			type = _blockType
		};
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0003195C File Offset: 0x0002FB5C
	public BlockValue GetBlockValueFromProperty(string _propValue)
	{
		BlockValue result = BlockValue.Air;
		if (!this.Properties.Values.ContainsKey(_propValue))
		{
			throw new Exception("You need to specify a property with name '" + _propValue + "' for the block " + this.blockName);
		}
		result = Block.GetBlockValue(this.Properties.Values[_propValue], false);
		if (result.Equals(BlockValue.Air))
		{
			throw new Exception("Block with name '" + this.Properties.Values[_propValue] + "' not found!");
		}
		return result;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000319EB File Offset: 0x0002FBEB
	public virtual bool ShowModelOnFall()
	{
		return this.bShowModelOnFall;
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060006D0 RID: 1744 RVA: 0x000319F4 File Offset: 0x0002FBF4
	public BlockActivationCommand[] CustomCmds
	{
		get
		{
			if (this.customCmds == null)
			{
				int num = 0;
				int num2 = 1;
				while (num2 <= 10 && this.Properties.Values.ContainsKey(string.Format("{0}{1}", Block.PropCustomCommandName, num2)))
				{
					num++;
					num2++;
				}
				this.customCmds = new BlockActivationCommand[num];
				if (num > 0)
				{
					for (int i = 1; i <= num; i++)
					{
						if (this.Properties.Values.ContainsKey(string.Format("{0}{1}", Block.PropCustomCommandName, i)))
						{
							BlockActivationCommand blockActivationCommand = default(BlockActivationCommand);
							blockActivationCommand.text = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandName, i)];
							blockActivationCommand.icon = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandIcon, i)];
							blockActivationCommand.eventName = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandEvent, i)];
							string key = string.Format("{0}{1}", Block.PropCustomCommandIconColor, i);
							if (this.Properties.Values.ContainsKey(key))
							{
								blockActivationCommand.iconColor = StringParsers.ParseHexColor(this.Properties.Values[key]);
							}
							else
							{
								blockActivationCommand.iconColor = Color.white;
							}
							key = string.Format("{0}{1}", Block.PropCustomCommandActivateTime, i);
							if (this.Properties.Values.ContainsKey(key))
							{
								blockActivationCommand.activateTime = StringParsers.ParseFloat(this.Properties.Values[key], 0, -1, NumberStyles.Any);
							}
							else
							{
								blockActivationCommand.activateTime = -1f;
							}
							blockActivationCommand.enabled = true;
							this.customCmds[i - 1] = blockActivationCommand;
						}
					}
				}
			}
			return this.customCmds;
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00031BF4 File Offset: 0x0002FDF4
	public virtual bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = this.CanPickup;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag = true;
		}
		return flag || this.CustomCmds.Length != 0;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00031C48 File Offset: 0x0002FE48
	public virtual BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = false;
		bool flag2 = this.CanPickup;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag2 = true;
		}
		if (flag2)
		{
			this.cmds[0].enabled = true;
			flag = true;
		}
		if (!flag)
		{
			return BlockActivationCommand.Empty;
		}
		return this.cmds;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00031CB4 File Offset: 0x0002FEB4
	public virtual bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == this.cmds[0].text || _commandName == this.cmds[1].text)
		{
			this.OnBlockActivated(_world, _blockPos, _blockValue, _player);
			return true;
		}
		return false;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00031D04 File Offset: 0x0002FF04
	public virtual void RenderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		this.shape.renderDecorations(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, _nBlocks);
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00031D2C File Offset: 0x0002FF2C
	public int GetActivationDistanceSq()
	{
		int num = this.activationDistance;
		if (num == 0)
		{
			return (int)(Constants.cCollectItemDistance * Constants.cCollectItemDistance);
		}
		return num * num;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00031D54 File Offset: 0x0002FF54
	public int GetPlacementDistanceSq()
	{
		int num = this.placementDistance;
		if (num == 0)
		{
			num = this.activationDistance;
		}
		if (num == 0)
		{
			return (int)(Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance);
		}
		return num * num;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00031D88 File Offset: 0x0002FF88
	public virtual void CheckUpdate(BlockValue _oldBV, BlockValue _newBV, out bool bUpdateMesh, out bool bUpdateNotify, out bool bUpdateLight)
	{
		bUpdateMesh = (bUpdateNotify = (bUpdateLight = true));
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool RotateVerticesOnCollisionCheck(BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool ActivateBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		return false;
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool ActivateBlockOnce(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return false;
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTriggerAddedFromPrefab(BlockTrigger _trigger, Vector3i _blockPos, BlockValue _blockValue, FastTags<TagGroup.Global> _questTags)
	{
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTriggerRefresh(BlockTrigger _trigger, BlockValue _bv, FastTags<TagGroup.Global> questTag)
	{
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTriggerChanged(BlockTrigger _trigger, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTriggerChanged(BlockTrigger _trigger, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges)
	{
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnTriggered(EntityPlayer _player, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Refresh(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00031DA4 File Offset: 0x0002FFA4
	public void HandleTrigger(EntityPlayer _player, World _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageBlockTrigger>().Setup(_blockPos, _blockValue), false);
			return;
		}
		BlockTrigger blockTrigger = ((Chunk)_world.ChunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		if (blockTrigger != null && _player != null)
		{
			_world.triggerManager.TriggerBlocks(_player, _player.prefab, blockTrigger);
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00031E2D File Offset: 0x0003002D
	public override string ToString()
	{
		return this.blockName + " " + this.blockID.ToString();
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00031E4C File Offset: 0x0003004C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsLinear()
	{
		bool[] usedIds = new bool[Block.MAX_BLOCKS];
		List<Block> list = new List<Block>(Block.nameToBlock.Count);
		Block.nameToBlock.CopyValuesTo(list);
		Block.assignLeftOverBlocks(usedIds, list);
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00031E84 File Offset: 0x00030084
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignId(Block _b, int _id, bool[] _usedIds)
	{
		Block.list[_id] = _b;
		_b.blockID = _id;
		_usedIds[_id] = true;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00031E9C File Offset: 0x0003009C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignLeftOverBlocks(bool[] _usedIds, List<Block> _unassignedBlocks)
	{
		foreach (KeyValuePair<string, int> keyValuePair in Block.fixedBlockIds)
		{
			if (Block.nameToBlock.ContainsKey(keyValuePair.Key))
			{
				Block block = Block.nameToBlock[keyValuePair.Key];
				if (_unassignedBlocks.Contains(block))
				{
					_unassignedBlocks.Remove(block);
					Block.assignId(block, keyValuePair.Value, _usedIds);
				}
			}
		}
		int num = 0;
		int num2 = 255;
		foreach (Block block2 in _unassignedBlocks)
		{
			if (block2.shape.IsTerrain())
			{
				while (_usedIds[++num])
				{
				}
				Block.assignId(block2, num, _usedIds);
			}
			else
			{
				while (_usedIds[++num2])
				{
				}
				Block.assignId(block2, num2, _usedIds);
			}
		}
		Log.Out("Block IDs total {0}, terr {1}, last {2}", new object[]
		{
			Block.nameToBlock.Count,
			num,
			num2
		});
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00031FD8 File Offset: 0x000301D8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsFromMapping()
	{
		List<Block> list = new List<Block>();
		bool[] usedIds = new bool[Block.MAX_BLOCKS];
		foreach (KeyValuePair<string, Block> keyValuePair in Block.nameToBlock)
		{
			int idForName = Block.nameIdMapping.GetIdForName(keyValuePair.Key);
			if (idForName >= 0)
			{
				Block.assignId(keyValuePair.Value, idForName, usedIds);
			}
			else
			{
				list.Add(keyValuePair.Value);
			}
		}
		Block.assignLeftOverBlocks(usedIds, list);
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00032074 File Offset: 0x00030274
	[PublicizedFrom(EAccessModifier.Private)]
	public static void createFullMappingForClients()
	{
		NameIdMapping nameIdMapping = new NameIdMapping(null, Block.MAX_BLOCKS);
		foreach (KeyValuePair<string, Block> keyValuePair in Block.nameToBlock)
		{
			nameIdMapping.AddMapping(keyValuePair.Value.blockID, keyValuePair.Key, false);
		}
		Block.fullMappingDataForClients = nameIdMapping.SaveToArray();
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x000320F0 File Offset: 0x000302F0
	public static void AssignIds()
	{
		if (Block.nameToBlock.Count > Block.MAX_BLOCKS)
		{
			throw new ArgumentOutOfRangeException(string.Format("Too many blocks defined ({0}, allowed {1}", Block.nameToBlock.Count, Block.MAX_BLOCKS));
		}
		if (Block.nameIdMapping != null)
		{
			Log.Out("Block IDs with mapping");
			Block.assignIdsFromMapping();
		}
		else
		{
			Log.Out("Block IDs withOUT mapping");
			Block.assignIdsLinear();
		}
		Block.createFullMappingForClients();
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsTileEntitySavedInPrefab()
	{
		return false;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00032163 File Offset: 0x00030363
	public virtual string GetCustomDescription(Vector3i _blockPos, BlockValue _bv)
	{
		return "";
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0003216A File Offset: 0x0003036A
	public string GetPlaceAltBlockValue(WorldBase _world)
	{
		if (this.placeAltBlockNames != null && this.placeAltBlockNames.Length != 0)
		{
			return this.placeAltBlockNames[_world.GetGameRandom().RandomRange(0, this.placeAltBlockNames.Length)];
		}
		return string.Empty;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x0003219E File Offset: 0x0003039E
	public Block GetAltBlock(int _typeId)
	{
		this.GetAltBlocks();
		if (this.placeAltBlockClasses != null && this.placeAltBlockClasses.Length != 0)
		{
			return this.placeAltBlockClasses[_typeId];
		}
		return Block.list[0];
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x000321C8 File Offset: 0x000303C8
	public BlockValue GetAltBlockValue(int typeID)
	{
		return this.GetAltBlock(typeID).ToBlockValue();
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x000321D6 File Offset: 0x000303D6
	public string[] GetAltBlockNames()
	{
		return this.placeAltBlockNames;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x000321E0 File Offset: 0x000303E0
	public Block[] GetAltBlocks()
	{
		if (this.placeAltBlockClasses == null && this.placeAltBlockNames != null)
		{
			this.placeAltBlockClasses = new Block[this.placeAltBlockNames.Length];
			for (int i = 0; i < this.placeAltBlockNames.Length; i++)
			{
				this.placeAltBlockClasses[i] = Block.GetBlockByName(this.placeAltBlockNames[i], false);
			}
		}
		return this.placeAltBlockClasses;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0003223F File Offset: 0x0003043F
	public int AlternateBlockCount()
	{
		return this.placeAltBlockNames.Length;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x0003224C File Offset: 0x0003044C
	public bool ContainsAlternateBlock(string block)
	{
		for (int i = 0; i < this.placeAltBlockNames.Length; i++)
		{
			if (this.placeAltBlockNames[i] == block)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00032280 File Offset: 0x00030480
	public int GetAlternateBlockIndex(string block)
	{
		for (int i = 0; i < this.placeAltBlockNames.Length; i++)
		{
			if (this.placeAltBlockNames[i] == block)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x000322B4 File Offset: 0x000304B4
	public static void GetShapeCategories(IEnumerable<Block> _altBlocks, List<ShapesFromXml.ShapeCategory> _targetList)
	{
		_targetList.Clear();
		foreach (Block block in _altBlocks)
		{
			if (block.ShapeCategories != null)
			{
				foreach (ShapesFromXml.ShapeCategory item in block.ShapeCategories)
				{
					if (!_targetList.Contains(item))
					{
						_targetList.Add(item);
					}
				}
			}
		}
		_targetList.Sort();
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00032354 File Offset: 0x00030554
	public int GetShownMaxDamage()
	{
		BlockCompositeTileEntity blockCompositeTileEntity = this as BlockCompositeTileEntity;
		if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<TEFeatureDoor>())
		{
			return this.MaxDamagePlusDowngrades;
		}
		return this.MaxDamage;
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00032385 File Offset: 0x00030585
	public bool SupportsRotation(byte _rotation)
	{
		if (_rotation < 4)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Basic90) > EBlockRotationClasses.None;
		}
		if (_rotation < 8)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Headfirst) > EBlockRotationClasses.None;
		}
		if (_rotation < 24)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Sideways) > EBlockRotationClasses.None;
		}
		return (this.AllowedRotations & EBlockRotationClasses.Basic45) > EBlockRotationClasses.None;
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x000323C4 File Offset: 0x000305C4
	public bool SupportsRotationFromMask(int rotationMask)
	{
		for (int i = 0; i < 28; i++)
		{
			if ((rotationMask & 1 << i) != 0 && this.SupportsRotation((byte)i))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x000323F8 File Offset: 0x000305F8
	public void RotateHoldingBlock(ItemClassBlock.ItemBlockInventoryData _blockInventoryData, bool _increaseRotation, bool _playSoundOnRotation = true)
	{
		if (_blockInventoryData.mode == BlockPlacement.EnumRotationMode.Auto)
		{
			_blockInventoryData.mode = BlockPlacement.EnumRotationMode.Simple;
		}
		BlockValue bv = _blockInventoryData.itemValue.ToBlockValue(true);
		bv.rotation = _blockInventoryData.rotation;
		bv = this.BlockPlacementHelper.OnPlaceBlock(_blockInventoryData.Placement, _blockInventoryData.mode, _blockInventoryData.localRot, _blockInventoryData.world, bv, _blockInventoryData.propTransform, ((EntityPlayerLocal)_blockInventoryData.holdingEntity).HitInfo.hit, _blockInventoryData.holdingEntity.position).blockValue;
		int rotation = (int)_blockInventoryData.rotation;
		_blockInventoryData.rotation = this.BlockPlacementHelper.LimitRotation(_blockInventoryData.mode, ref _blockInventoryData.localRot, ((EntityPlayerLocal)_blockInventoryData.holdingEntity).HitInfo.hit, _increaseRotation, bv, bv.rotation);
		if (_playSoundOnRotation && rotation != (int)_blockInventoryData.rotation)
		{
			_blockInventoryData.holdingEntity.PlayOneShot("rotateblock", false, false, false, null, 1f);
		}
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x000324E8 File Offset: 0x000306E8
	public void GroundAlign(BlockEntityData _data)
	{
		if (!_data.bHasTransform)
		{
			return;
		}
		BlockValue blockValue = _data.blockValue;
		int type = blockValue.type;
		Transform transform = _data.transform;
		GameObject gameObject = transform.gameObject;
		gameObject.SetActive(false);
		Vector3 vector = Vector3.zero;
		int num = 0;
		Ray ray = new Ray(Vector3.zero, Vector3.down);
		Vector3 b = new Vector3(0.5f, 0.75f, 0.5f) - Origin.position;
		float num2 = this.GroundAlignDistance + 0.5f;
		Vector3i pos = _data.pos;
		Vector3 vector2 = transform.position;
		Vector3 vector3;
		if (!this.isMultiBlock)
		{
			vector3 = new Vector3(0f, float.MinValue, 0f);
			ray.origin = pos.ToVector3() + b;
			RaycastHit raycastHit;
			bool flag = Physics.SphereCast(ray, 0.22f, out raycastHit, num2 - 0.22f + 0.25f, 1082195968);
			if (!flag)
			{
				flag = Physics.SphereCast(ray, 0.48f, out raycastHit, num2 - 0.48f + 0.25f, 1082195968);
			}
			if (flag)
			{
				vector3 = raycastHit.point;
				vector = raycastHit.normal;
				num = 1;
			}
		}
		else
		{
			if (blockValue.ischild)
			{
				pos = new Vector3i(blockValue.parentx, blockValue.parenty, blockValue.parentz);
			}
			vector3 = vector2;
			vector3.y = float.MinValue;
			byte rotation = blockValue.rotation;
			for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.multiBlockPos.Get(i, type, (int)rotation);
				if (vector3i.y == 0)
				{
					ray.origin = (pos + vector3i).ToVector3() + b;
					RaycastHit raycastHit;
					if (Physics.SphereCast(ray, 0.22f, out raycastHit, num2 - 0.22f + 0.25f, 1082195968))
					{
						if (vector3.y < raycastHit.point.y)
						{
							vector3.y = raycastHit.point.y;
						}
						vector += raycastHit.normal;
						num++;
					}
				}
			}
			if (num > 0)
			{
				vector *= 1f / (float)num;
				vector.Normalize();
			}
		}
		if (num > 0)
		{
			vector2 = vector3;
			Quaternion quaternion = transform.rotation;
			quaternion = Quaternion.FromToRotation(Vector3.up, vector) * quaternion;
			transform.SetPositionAndRotation(vector2, quaternion);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00032770 File Offset: 0x00030970
	public static void CacheStats()
	{
		DynamicPropertiesCache propertiesCache = Block.PropertiesCache;
		if (propertiesCache == null)
		{
			return;
		}
		propertiesCache.Stats();
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00032784 File Offset: 0x00030984
	public void GetCollisionCollisionHash(IncrementalHash calcHash)
	{
		calcHash.AppendDataNoAlloc(1);
		calcHash.AppendDataNoAlloc((short)((byte)this.AllowedRotations));
		calcHash.AppendDataNoAlloc(this.BlockingType);
		calcHash.AppendDataNoAlloc(this.isMultiBlock);
		calcHash.AppendDataNoAlloc(this.isOversized);
		this.oversizedBounds.CalculatePersistableHash(calcHash);
		this.shape.CalculateCollisionHash(calcHash);
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x000327E1 File Offset: 0x000309E1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void takeItemWithTimer(Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player, float _takeDelay)
	{
		Block.TakeItemWithTimer(_blockPos, _blockValue, _player, _takeDelay, new Func<Vector3i, EntityPlayerLocal, bool>(this.takeItemWithTimerCanTake));
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x0002003D File Offset: 0x0001E23D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool takeItemWithTimerCanTake(Vector3i _blockPos, EntityAlive _player)
	{
		return true;
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x000327FC File Offset: 0x000309FC
	public static void TakeItemWithTimer(Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player, float _takeDelay, Func<Vector3i, EntityPlayerLocal, bool> _canTakeCallback = null)
	{
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player, Localization.Get("ttRepairBeforePickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if (_canTakeCallback != null && !_canTakeCallback(_blockPos, _player))
		{
			return;
		}
		LocalPlayerUI playerUI = _player.PlayerUI;
		TimerEventData timerEventData = new TimerEventData();
		timerEventData.Data = new object[]
		{
			_blockValue,
			_blockPos,
			_player
		};
		timerEventData.FullTimeFinishEvent += Block.TakeItemWithTimerDone;
		XUiC_Timer.OpenTimer(playerUI.xui, _takeDelay, timerEventData, -1f, "", true);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x000328A0 File Offset: 0x00030AA0
	public static void TakeItemWithTimerDone(TimerEventData _timerData)
	{
		World world = GameManager.Instance.World;
		object[] array = (object[])_timerData.Data;
		BlockValue blockValue = (BlockValue)array[0];
		Vector3i vector3i = (Vector3i)array[1];
		BlockValue block = world.GetBlock(vector3i);
		EntityPlayerLocal entityPlayerLocal = array[2] as EntityPlayerLocal;
		if (block.damage > 0)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttRepairBeforePickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if (block.type != blockValue.type)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttBlockMissingPickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		TileEntity tileEntity = world.GetTileEntity(vector3i);
		if (tileEntity != null && tileEntity.IsUserAccessing())
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttCantPickupInUse", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(block.ToItemValue(), 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			uiforPlayer.xui.PlayerInventory.DropItem(itemStack);
		}
		world.SetBlockRPC(vector3i, BlockValue.Air);
	}

	// Token: 0x04000710 RID: 1808
	public const int cAirId = 0;

	// Token: 0x04000711 RID: 1809
	public const int cTerrainStartId = 1;

	// Token: 0x04000712 RID: 1810
	public const int cTerrainEndId = 239;

	// Token: 0x04000713 RID: 1811
	public const int cTerrainIdCount = 239;

	// Token: 0x04000714 RID: 1812
	public const int cWaterId = 240;

	// Token: 0x04000715 RID: 1813
	public const int cWaterPOIId = 241;

	// Token: 0x04000716 RID: 1814
	public const int cWaterDataId = 242;

	// Token: 0x04000717 RID: 1815
	public const int cGeneralStartId = 256;

	// Token: 0x04000718 RID: 1816
	public static int MAX_BLOCKS = 65536;

	// Token: 0x04000719 RID: 1817
	public static int ItemsStartHere = Block.MAX_BLOCKS;

	// Token: 0x0400071A RID: 1818
	public static bool FallInstantly = false;

	// Token: 0x0400071B RID: 1819
	public const int BlockFaceDrawn_Top = 1;

	// Token: 0x0400071C RID: 1820
	public const int BlockFaceDrawn_Bottom = 2;

	// Token: 0x0400071D RID: 1821
	public const int BlockFaceDrawn_North = 4;

	// Token: 0x0400071E RID: 1822
	public const int BlockFaceDrawn_West = 8;

	// Token: 0x0400071F RID: 1823
	public const int BlockFaceDrawn_South = 16;

	// Token: 0x04000720 RID: 1824
	public const int BlockFaceDrawn_East = 32;

	// Token: 0x04000721 RID: 1825
	public const int BlockFaceDrawn_AllORD = 63;

	// Token: 0x04000722 RID: 1826
	public const int BlockFaceDrawn_All = 255;

	// Token: 0x04000723 RID: 1827
	public static float cWaterLevel = 62.88f;

	// Token: 0x04000724 RID: 1828
	public static string PropCanPickup = "CanPickup";

	// Token: 0x04000725 RID: 1829
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPickupTarget = "PickupTarget";

	// Token: 0x04000726 RID: 1830
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPickupSource = "PickupSource";

	// Token: 0x04000727 RID: 1831
	public static string PropPlaceAltBlockValue = "PlaceAltBlockValue";

	// Token: 0x04000728 RID: 1832
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceShapeCategories = "ShapeCategories";

	// Token: 0x04000729 RID: 1833
	public static string PropSiblingBlock = "SiblingBlock";

	// Token: 0x0400072A RID: 1834
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelValue = "FuelValue";

	// Token: 0x0400072B RID: 1835
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropWeight = "Weight";

	// Token: 0x0400072C RID: 1836
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanMobsSpawnOn = "CanMobsSpawnOn";

	// Token: 0x0400072D RID: 1837
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanPlayersSpawnOn = "CanPlayersSpawnOn";

	// Token: 0x0400072E RID: 1838
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIndexName = "IndexName";

	// Token: 0x0400072F RID: 1839
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanBlocksReplace = "CanBlocksReplace";

	// Token: 0x04000730 RID: 1840
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanDecorateOnSlopes = "CanDecorateOnSlopes";

	// Token: 0x04000731 RID: 1841
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSlopeMax = "SlopeMax";

	// Token: 0x04000732 RID: 1842
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsProp = "IsProp";

	// Token: 0x04000733 RID: 1843
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsTerrainDecoration = "IsTerrainDecoration";

	// Token: 0x04000734 RID: 1844
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsDecoration = "IsDecoration";

	// Token: 0x04000735 RID: 1845
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDistantDecoration = "IsDistantDecoration";

	// Token: 0x04000736 RID: 1846
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBigDecorationRadius = "BigDecorationRadius";

	// Token: 0x04000737 RID: 1847
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSmallDecorationRadius = "SmallDecorationRadius";

	// Token: 0x04000738 RID: 1848
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGndAlign = "GndAlign";

	// Token: 0x04000739 RID: 1849
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIgnoreKeystoneOverlay = "IgnoreKeystoneOverlay";

	// Token: 0x0400073A RID: 1850
	public static string PropUpgradeBlockClass = "UpgradeBlock";

	// Token: 0x0400073B RID: 1851
	public static string PropUpgradeBlockToBlock = "ToBlock";

	// Token: 0x0400073C RID: 1852
	public static string PropUpgradeBlockItemCount = "ItemCount";

	// Token: 0x0400073D RID: 1853
	public static string PropDowngradeBlock = "DowngradeBlock";

	// Token: 0x0400073E RID: 1854
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLPScale = "LPHardnessScale";

	// Token: 0x0400073F RID: 1855
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapColor = "MapColor";

	// Token: 0x04000740 RID: 1856
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapColor2 = "MapColor2";

	// Token: 0x04000741 RID: 1857
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGroupName = "Group";

	// Token: 0x04000742 RID: 1858
	public static string PropCustomIcon = "CustomIcon";

	// Token: 0x04000743 RID: 1859
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCustomIconTint = "CustomIconTint";

	// Token: 0x04000744 RID: 1860
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlacementWireframe = "PlacementWireframe";

	// Token: 0x04000745 RID: 1861
	public static string PropMultiBlockDim = "MultiBlockDim";

	// Token: 0x04000746 RID: 1862
	public static string PropOversizedBounds = "OversizedBounds";

	// Token: 0x04000747 RID: 1863
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTerrainAlignment = "TerrainAlignment";

	// Token: 0x04000748 RID: 1864
	public static string PropMultiBlockLayer = "MultiBlockLayer";

	// Token: 0x04000749 RID: 1865
	public static string PropMultiBlockLayer0 = "MultiBlockLayer0";

	// Token: 0x0400074A RID: 1866
	public static string PropDisableCover = "DisableCover";

	// Token: 0x0400074B RID: 1867
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsPlant = "IsPlant";

	// Token: 0x0400074C RID: 1868
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHeatMapStrength = "HeatMapStrength";

	// Token: 0x0400074D RID: 1869
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFallDamage = "FallDamage";

	// Token: 0x0400074E RID: 1870
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBuffsWhenWalkedOn = "BuffsWhenWalkedOn";

	// Token: 0x0400074F RID: 1871
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRadiusEffect = "ActiveRadiusEffects";

	// Token: 0x04000750 RID: 1872
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCount = "Count";

	// Token: 0x04000751 RID: 1873
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropAllowAllRotations = "AllowAllRotations";

	// Token: 0x04000752 RID: 1874
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivationDistance = "ActivationDistance";

	// Token: 0x04000753 RID: 1875
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlacementDistance = "PlacementDistance";

	// Token: 0x04000754 RID: 1876
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsReplaceRandom = "IsReplaceRandom";

	// Token: 0x04000755 RID: 1877
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftExpValue = "CraftComponentExpValue";

	// Token: 0x04000756 RID: 1878
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftTimeValue = "CraftComponentTimeValue";

	// Token: 0x04000757 RID: 1879
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLootExpValue = "LootExpValue";

	// Token: 0x04000758 RID: 1880
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDestroyExpValue = "DestroyExpValue";

	// Token: 0x04000759 RID: 1881
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropParticleOnDeath = "ParticleOnDeath";

	// Token: 0x0400075A RID: 1882
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceExpValue = "PlaceExpValue";

	// Token: 0x0400075B RID: 1883
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUpgradeExpValue = "UpgradeExpValue";

	// Token: 0x0400075C RID: 1884
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicValue = "EconomicValue";

	// Token: 0x0400075D RID: 1885
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicSellScale = "EconomicSellScale";

	// Token: 0x0400075E RID: 1886
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicBundleSize = "EconomicBundleSize";

	// Token: 0x0400075F RID: 1887
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSellableToTrader = "SellableToTrader";

	// Token: 0x04000760 RID: 1888
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTraderStageTemplate = "TraderStageTemplate";

	// Token: 0x04000761 RID: 1889
	public static string PropResourceScale = "ResourceScale";

	// Token: 0x04000762 RID: 1890
	public static string PropMaxDamage = "MaxDamage";

	// Token: 0x04000763 RID: 1891
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStartDamage = "StartDamage";

	// Token: 0x04000764 RID: 1892
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStage2Health = "Stage2Health";

	// Token: 0x04000765 RID: 1893
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamage = "Damage";

	// Token: 0x04000766 RID: 1894
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropExplosionAffected = "ExplosionAffected";

	// Token: 0x04000767 RID: 1895
	public static string PropDescriptionKey = "DescriptionKey";

	// Token: 0x04000768 RID: 1896
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActionSkillGroup = "ActionSkillGroup";

	// Token: 0x04000769 RID: 1897
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftingSkillGroup = "CraftingSkillGroup";

	// Token: 0x0400076A RID: 1898
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropShowModelOnFall = "ShowModelOnFall";

	// Token: 0x0400076B RID: 1899
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLightOpacity = "LightOpacity";

	// Token: 0x0400076C RID: 1900
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHarvestOverdamage = "HarvestOverdamage";

	// Token: 0x0400076D RID: 1901
	public static string PropTintColor = "TintColor";

	// Token: 0x0400076E RID: 1902
	public static string PropCreativeMode = "CreativeMode";

	// Token: 0x0400076F RID: 1903
	public static string PropFilterTag = "FilterTags";

	// Token: 0x04000770 RID: 1904
	public static string PropTag = "Tags";

	// Token: 0x04000771 RID: 1905
	public static string PropCreativeSort1 = "SortOrder1";

	// Token: 0x04000772 RID: 1906
	public static string PropCreativeSort2 = "SortOrder2";

	// Token: 0x04000773 RID: 1907
	public static string PropDisplayType = "DisplayType";

	// Token: 0x04000774 RID: 1908
	public static string PropUnlockedBy = "UnlockedBy";

	// Token: 0x04000775 RID: 1909
	public static string PropNoScrapping = "NoScrapping";

	// Token: 0x04000776 RID: 1910
	public static string PropVehicleHitScale = "VehicleHitScale";

	// Token: 0x04000777 RID: 1911
	public static string PropItemTypeIcon = "ItemTypeIcon";

	// Token: 0x04000778 RID: 1912
	public static string PropAutoShape = "AutoShape";

	// Token: 0x04000779 RID: 1913
	public static string PropBlockAddedEvent = "AddedEvent";

	// Token: 0x0400077A RID: 1914
	public static string PropBlockDestroyedEvent = "DestroyedEvent";

	// Token: 0x0400077B RID: 1915
	public static string PropBlockDowngradeEvent = "DowngradeEvent";

	// Token: 0x0400077C RID: 1916
	public static string PropBlockDowngradedToEvent = "DowngradedToEvent";

	// Token: 0x0400077D RID: 1917
	public static string PropIsTemporaryBlock = "IsTemporaryBlock";

	// Token: 0x0400077E RID: 1918
	public static string PropRefundOnUnload = "RefundOnUnload";

	// Token: 0x0400077F RID: 1919
	public static string PropSoundPickup = "SoundPickup";

	// Token: 0x04000780 RID: 1920
	public static string PropSoundPlace = "SoundPlace";

	// Token: 0x04000781 RID: 1921
	public static string PropSoundHitAdditional = "SoundHitAdditional";

	// Token: 0x04000782 RID: 1922
	public static string PropCustomCommandName = "CustomCommandName";

	// Token: 0x04000783 RID: 1923
	public static string PropCustomCommandIcon = "CustomCommandIcon";

	// Token: 0x04000784 RID: 1924
	public static string PropCustomCommandIconColor = "CustomCommandIconColor";

	// Token: 0x04000785 RID: 1925
	public static string PropCustomCommandEvent = "CustomCommandEvent";

	// Token: 0x04000786 RID: 1926
	public static string PropCustomCommandActivateTime = "CustomCommandActivateTime";

	// Token: 0x04000787 RID: 1927
	public static NameIdMapping nameIdMapping;

	// Token: 0x04000788 RID: 1928
	public static byte[] fullMappingDataForClients;

	// Token: 0x04000789 RID: 1929
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Block> nameToBlock;

	// Token: 0x0400078A RID: 1930
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Block> nameToBlockCaseInsensitive;

	// Token: 0x0400078B RID: 1931
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string[]> groupNameStringToGroupNames;

	// Token: 0x0400078C RID: 1932
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<string> autoShapeMaterials;

	// Token: 0x0400078D RID: 1933
	public static Block[] list;

	// Token: 0x0400078E RID: 1934
	public static DynamicPropertiesCache PropertiesCache;

	// Token: 0x0400078F RID: 1935
	public static string defaultBlockDescriptionKey = "";

	// Token: 0x04000790 RID: 1936
	public int blockID;

	// Token: 0x04000791 RID: 1937
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicProperties dynamicProperties;

	// Token: 0x04000792 RID: 1938
	public BlockShape shape;

	// Token: 0x04000793 RID: 1939
	public int BlockingType;

	// Token: 0x04000794 RID: 1940
	public BlockValue SiblingBlock;

	// Token: 0x04000795 RID: 1941
	public BlockTags BlockTag;

	// Token: 0x04000796 RID: 1942
	public BlockPlacement BlockPlacementHelper;

	// Token: 0x04000797 RID: 1943
	public bool CanBlocksReplace;

	// Token: 0x04000798 RID: 1944
	public float LPHardnessScale;

	// Token: 0x04000799 RID: 1945
	public float MovementFactor;

	// Token: 0x0400079A RID: 1946
	public bool CanPickup;

	// Token: 0x0400079B RID: 1947
	public string PickedUpItemValue;

	// Token: 0x0400079C RID: 1948
	public string PickupTarget;

	// Token: 0x0400079D RID: 1949
	public string PickupSource;

	// Token: 0x0400079E RID: 1950
	public byte BlocksMovement;

	// Token: 0x0400079F RID: 1951
	public int FuelValue;

	// Token: 0x040007A0 RID: 1952
	public DataItem<int> Weight;

	// Token: 0x040007A1 RID: 1953
	public bool CanMobsSpawnOn;

	// Token: 0x040007A2 RID: 1954
	public bool CanPlayersSpawnOn;

	// Token: 0x040007A3 RID: 1955
	public string IndexName;

	// Token: 0x040007A4 RID: 1956
	public bool CanDecorateOnSlopes;

	// Token: 0x040007A5 RID: 1957
	public float SlopeMaxCos;

	// Token: 0x040007A6 RID: 1958
	public bool IsProp;

	// Token: 0x040007A7 RID: 1959
	public bool IsTerrainDecoration;

	// Token: 0x040007A8 RID: 1960
	public bool IsDecoration;

	// Token: 0x040007A9 RID: 1961
	public bool IsDistantDecoration;

	// Token: 0x040007AA RID: 1962
	public int BigDecorationRadius;

	// Token: 0x040007AB RID: 1963
	public int SmallDecorationRadius;

	// Token: 0x040007AC RID: 1964
	public float GroundAlignDistance;

	// Token: 0x040007AD RID: 1965
	public bool IgnoreKeystoneOverlay;

	// Token: 0x040007AE RID: 1966
	public bool IsLimited;

	// Token: 0x040007AF RID: 1967
	public const int cPathScan = -1;

	// Token: 0x040007B0 RID: 1968
	public const int cPathSolid = 1;

	// Token: 0x040007B1 RID: 1969
	public int PathType;

	// Token: 0x040007B2 RID: 1970
	public float PathOffsetX;

	// Token: 0x040007B3 RID: 1971
	public float PathOffsetZ;

	// Token: 0x040007B4 RID: 1972
	public BlockFaceFlag WaterFlowMask = BlockFaceFlag.All;

	// Token: 0x040007B5 RID: 1973
	public bool WaterClipEnabled;

	// Token: 0x040007B6 RID: 1974
	public Plane WaterClipPlane;

	// Token: 0x040007B7 RID: 1975
	public BlockValue DowngradeBlock;

	// Token: 0x040007B8 RID: 1976
	public BlockValue LockpickDowngradeBlock;

	// Token: 0x040007B9 RID: 1977
	public BlockValue UpgradeBlock;

	// Token: 0x040007BA RID: 1978
	public string[] GroupNames = new string[]
	{
		"Decor/Miscellaneous"
	};

	// Token: 0x040007BB RID: 1979
	public string CustomIcon;

	// Token: 0x040007BC RID: 1980
	public Color CustomIconTint;

	// Token: 0x040007BD RID: 1981
	public bool bHasPlacementWireframe;

	// Token: 0x040007BE RID: 1982
	public bool bUserHidden;

	// Token: 0x040007BF RID: 1983
	public float FallDamage;

	// Token: 0x040007C0 RID: 1984
	public float HeatMapStrength;

	// Token: 0x040007C1 RID: 1985
	public string[] BuffsWhenWalkedOn;

	// Token: 0x040007C2 RID: 1986
	public BlockRadiusEffect[] RadiusEffects;

	// Token: 0x040007C3 RID: 1987
	public string DescriptionKey;

	// Token: 0x040007C4 RID: 1988
	public string CraftingSkillGroup = "";

	// Token: 0x040007C5 RID: 1989
	public string ActionSkillGroup = "";

	// Token: 0x040007C6 RID: 1990
	public bool IsReplaceRandom = true;

	// Token: 0x040007C7 RID: 1991
	public float CraftComponentExp = 1f;

	// Token: 0x040007C8 RID: 1992
	public float CraftComponentTime = 1f;

	// Token: 0x040007C9 RID: 1993
	public float LootExp = 1f;

	// Token: 0x040007CA RID: 1994
	public float DestroyExp = 1f;

	// Token: 0x040007CB RID: 1995
	[PublicizedFrom(EAccessModifier.Protected)]
	public string deathParticleName;

	// Token: 0x040007CC RID: 1996
	public float EconomicValue;

	// Token: 0x040007CD RID: 1997
	public float EconomicSellScale = 1f;

	// Token: 0x040007CE RID: 1998
	public int EconomicBundleSize = 1;

	// Token: 0x040007CF RID: 1999
	public bool SellableToTrader = true;

	// Token: 0x040007D0 RID: 2000
	public string TraderStageTemplate;

	// Token: 0x040007D1 RID: 2001
	public float PlaceExp = 1f;

	// Token: 0x040007D2 RID: 2002
	public float UpgradeExp = 1f;

	// Token: 0x040007D3 RID: 2003
	public int Count = 1;

	// Token: 0x040007D4 RID: 2004
	public int Stacknumber = 500;

	// Token: 0x040007D5 RID: 2005
	public bool HarvestOverdamage;

	// Token: 0x040007D6 RID: 2006
	public bool SelectAlternates;

	// Token: 0x040007D7 RID: 2007
	public EnumCreativeMode CreativeMode;

	// Token: 0x040007D8 RID: 2008
	public string[] FilterTags;

	// Token: 0x040007D9 RID: 2009
	public bool NoScrapping;

	// Token: 0x040007DA RID: 2010
	public string SortOrder;

	// Token: 0x040007DB RID: 2011
	public string DisplayType = "defaultBlock";

	// Token: 0x040007DC RID: 2012
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeUnlockData[] unlockedBy;

	// Token: 0x040007DD RID: 2013
	public string ItemTypeIcon = "";

	// Token: 0x040007DE RID: 2014
	[PublicizedFrom(EAccessModifier.Private)]
	public EAutoShapeType AutoShapeType;

	// Token: 0x040007DF RID: 2015
	[PublicizedFrom(EAccessModifier.Private)]
	public string autoShapeBaseName;

	// Token: 0x040007E0 RID: 2016
	[PublicizedFrom(EAccessModifier.Private)]
	public string autoShapeShapeName;

	// Token: 0x040007E1 RID: 2017
	[PublicizedFrom(EAccessModifier.Private)]
	public Block autoShapeHelper;

	// Token: 0x040007E2 RID: 2018
	public float VehicleHitScale;

	// Token: 0x040007E3 RID: 2019
	[PublicizedFrom(EAccessModifier.Private)]
	public Color MapColor;

	// Token: 0x040007E4 RID: 2020
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bMapColorSet;

	// Token: 0x040007E5 RID: 2021
	[PublicizedFrom(EAccessModifier.Private)]
	public Color MapColor2;

	// Token: 0x040007E6 RID: 2022
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bMapColor2Set;

	// Token: 0x040007E7 RID: 2023
	[PublicizedFrom(EAccessModifier.Private)]
	public float MapSpecular;

	// Token: 0x040007E8 RID: 2024
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i MapElevMinMax;

	// Token: 0x040007E9 RID: 2025
	[PublicizedFrom(EAccessModifier.Private)]
	public byte lightValue;

	// Token: 0x040007EA RID: 2026
	public int lightOpacity;

	// Token: 0x040007EB RID: 2027
	public Color tintColor = Color.clear;

	// Token: 0x040007EC RID: 2028
	public Color defaultTintColor = Color.clear;

	// Token: 0x040007ED RID: 2029
	public Vector3 tintColorV = Vector3.one;

	// Token: 0x040007EE RID: 2030
	public byte MeshIndex;

	// Token: 0x040007EF RID: 2031
	public List<Block.SItemNameCount> RepairItems;

	// Token: 0x040007F0 RID: 2032
	public List<Block.SItemNameCount> RepairItemsMeshDamage;

	// Token: 0x040007F1 RID: 2033
	public bool bRestrictSubmergedPlacement;

	// Token: 0x040007F2 RID: 2034
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockAddedEvent;

	// Token: 0x040007F3 RID: 2035
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDestroyedEvent;

	// Token: 0x040007F4 RID: 2036
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDowngradeEvent;

	// Token: 0x040007F5 RID: 2037
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDowngradedToEvent;

	// Token: 0x040007F6 RID: 2038
	public bool IsTemporaryBlock;

	// Token: 0x040007F7 RID: 2039
	public bool RefundOnUnload;

	// Token: 0x040007F8 RID: 2040
	public string SoundPickup = "craft_take_item";

	// Token: 0x040007F9 RID: 2041
	public string SoundPlace = "craft_place_item";

	// Token: 0x040007FA RID: 2042
	public string SoundHitAdditional;

	// Token: 0x040007FB RID: 2043
	public bool isMultiBlock;

	// Token: 0x040007FC RID: 2044
	public Block.MultiBlockArray multiBlockPos;

	// Token: 0x040007FD RID: 2045
	public bool isOversized;

	// Token: 0x040007FE RID: 2046
	public Bounds oversizedBounds;

	// Token: 0x040007FF RID: 2047
	public TerrainAlignmentMode terrainAlignmentMode;

	// Token: 0x04000800 RID: 2048
	public const int BT_All = 255;

	// Token: 0x04000801 RID: 2049
	public const int BT_None = 0;

	// Token: 0x04000802 RID: 2050
	public const int BT_Sight = 1;

	// Token: 0x04000803 RID: 2051
	public const int BT_Movement = 2;

	// Token: 0x04000804 RID: 2052
	public const int BT_Bullets = 4;

	// Token: 0x04000805 RID: 2053
	public const int BT_Rockets = 8;

	// Token: 0x04000806 RID: 2054
	public const int BT_Melee = 16;

	// Token: 0x04000807 RID: 2055
	public const int BT_Arrows = 32;

	// Token: 0x04000808 RID: 2056
	public bool IsCheckCollideWithEntity;

	// Token: 0x04000809 RID: 2057
	public bool DisableCover;

	// Token: 0x0400080A RID: 2058
	[PublicizedFrom(EAccessModifier.Private)]
	public Block.TextureInfo[] textureInfos = new Block.TextureInfo[1];

	// Token: 0x0400080B RID: 2059
	[PublicizedFrom(EAccessModifier.Private)]
	public int uiBackgroundTextureId = -1;

	// Token: 0x0400080C RID: 2060
	public int TerrainTAIndex = 1;

	// Token: 0x0400080D RID: 2061
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bNotifyOnLoadUnload;

	// Token: 0x0400080E RID: 2062
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bIsPlant;

	// Token: 0x0400080F RID: 2063
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShowModelOnFall;

	// Token: 0x04000810 RID: 2064
	public Dictionary<EnumDropEvent, List<Block.SItemDropProb>> itemsToDrop = new EnumDictionary<EnumDropEvent, List<Block.SItemDropProb>>();

	// Token: 0x04000811 RID: 2065
	public bool IsSleeperBlock;

	// Token: 0x04000812 RID: 2066
	public bool IsRandomlyTick;

	// Token: 0x04000813 RID: 2067
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] placeAltBlockNames;

	// Token: 0x04000814 RID: 2068
	[PublicizedFrom(EAccessModifier.Private)]
	public Block[] placeAltBlockClasses;

	// Token: 0x04000816 RID: 2070
	public MaterialBlock blockMaterial;

	// Token: 0x04000817 RID: 2071
	public bool StabilitySupport = true;

	// Token: 0x04000818 RID: 2072
	public bool StabilityIgnore;

	// Token: 0x04000819 RID: 2073
	public bool StabilityFull;

	// Token: 0x0400081A RID: 2074
	public sbyte Density;

	// Token: 0x0400081B RID: 2075
	[PublicizedFrom(EAccessModifier.Private)]
	public string blockName;

	// Token: 0x0400081C RID: 2076
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedBlockName;

	// Token: 0x0400081D RID: 2077
	public float ResourceScale;

	// Token: 0x0400081E RID: 2078
	public int MaxDamage;

	// Token: 0x0400081F RID: 2079
	public int MaxDamagePlusDowngrades;

	// Token: 0x04000820 RID: 2080
	public int StartDamage;

	// Token: 0x04000821 RID: 2081
	[PublicizedFrom(EAccessModifier.Private)]
	public int Stage2Health;

	// Token: 0x04000822 RID: 2082
	public float Damage;

	// Token: 0x04000823 RID: 2083
	public bool IsExplosionAffected;

	// Token: 0x04000824 RID: 2084
	public EBlockRotationClasses AllowedRotations;

	// Token: 0x04000825 RID: 2085
	public bool PlaceRandomRotation;

	// Token: 0x04000826 RID: 2086
	public string CustomPlaceSound;

	// Token: 0x04000827 RID: 2087
	public string UpgradeSound;

	// Token: 0x04000828 RID: 2088
	public string DowngradeFX;

	// Token: 0x04000829 RID: 2089
	public string DestroyFX;

	// Token: 0x0400082A RID: 2090
	[PublicizedFrom(EAccessModifier.Private)]
	public int activationDistance;

	// Token: 0x0400082B RID: 2091
	[PublicizedFrom(EAccessModifier.Private)]
	public int placementDistance;

	// Token: 0x0400082C RID: 2092
	public int cUVModeBits = 2;

	// Token: 0x0400082D RID: 2093
	public int cUVModeMask = 3;

	// Token: 0x0400082E RID: 2094
	public int cUVModeSides = 7;

	// Token: 0x0400082F RID: 2095
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly uint[] UVModesPerSide = new uint[1];

	// Token: 0x04000830 RID: 2096
	public bool bImposterExclude;

	// Token: 0x04000831 RID: 2097
	public bool bImposterExcludeAndStop;

	// Token: 0x04000832 RID: 2098
	public int ImposterExchange;

	// Token: 0x04000833 RID: 2099
	public byte ImposterExchangeTexIdx;

	// Token: 0x04000834 RID: 2100
	public bool bImposterDontBlock;

	// Token: 0x04000835 RID: 2101
	public int MergeIntoId;

	// Token: 0x04000836 RID: 2102
	public int[] MergeIntoTexIds;

	// Token: 0x04000837 RID: 2103
	public int MirrorSibling;

	// Token: 0x04000838 RID: 2104
	[PublicizedFrom(EAccessModifier.Protected)]
	public static List<Bounds> staticList_IntersectRayWithBlockList = new List<Bounds>();

	// Token: 0x04000839 RID: 2105
	public BlockFace HandleFace = BlockFace.None;

	// Token: 0x0400083A RID: 2106
	public bool EnablePassThroughDamage;

	// Token: 0x0400083B RID: 2107
	public List<BlockFace> RemovePaintOnDowngrade;

	// Token: 0x0400083C RID: 2108
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x0400083D RID: 2109
	public bool HasTileEntity;

	// Token: 0x0400083E RID: 2110
	public Block.EnumDisplayInfo DisplayInfo;

	// Token: 0x0400083F RID: 2111
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("take", "hand", false, false, null)
	};

	// Token: 0x04000840 RID: 2112
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemStack> itemsDropped = new List<ItemStack>();

	// Token: 0x04000841 RID: 2113
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockActivationCommand[] customCmds;

	// Token: 0x04000842 RID: 2114
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, int> fixedBlockIds = new Dictionary<string, int>
	{
		{
			"air",
			0
		},
		{
			"water",
			240
		},
		{
			"terrWaterPOI",
			241
		},
		{
			"waterdata",
			242
		}
	};

	// Token: 0x02000107 RID: 263
	public struct SItemDropProb
	{
		// Token: 0x06000700 RID: 1792 RVA: 0x00032E41 File Offset: 0x00031041
		public SItemDropProb(string _name, int _minCount, int _maxCount, float _prob, float _resourceScale, float _stickChance, string _toolCategory, string _tag)
		{
			this.name = _name;
			this.minCount = _minCount;
			this.maxCount = _maxCount;
			this.prob = _prob;
			this.resourceScale = _resourceScale;
			this.stickChance = _stickChance;
			this.toolCategory = _toolCategory;
			this.tag = _tag;
		}

		// Token: 0x04000843 RID: 2115
		public string name;

		// Token: 0x04000844 RID: 2116
		public int minCount;

		// Token: 0x04000845 RID: 2117
		public int maxCount;

		// Token: 0x04000846 RID: 2118
		public float prob;

		// Token: 0x04000847 RID: 2119
		public float resourceScale;

		// Token: 0x04000848 RID: 2120
		public float stickChance;

		// Token: 0x04000849 RID: 2121
		public string toolCategory;

		// Token: 0x0400084A RID: 2122
		public string tag;
	}

	// Token: 0x02000108 RID: 264
	public struct SItemNameCount
	{
		// Token: 0x0400084B RID: 2123
		public string ItemName;

		// Token: 0x0400084C RID: 2124
		public int Count;
	}

	// Token: 0x02000109 RID: 265
	public class MultiBlockArray
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x00032E80 File Offset: 0x00031080
		// (set) Token: 0x06000702 RID: 1794 RVA: 0x00032E88 File Offset: 0x00031088
		public Vector3i[] pos { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06000703 RID: 1795 RVA: 0x00032E91 File Offset: 0x00031091
		public MultiBlockArray(Vector3i _dim, List<Vector3i> _pos)
		{
			this.dim = _dim;
			this.pos = _pos.ToArray();
			this.Length = _pos.Count;
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00032EB8 File Offset: 0x000310B8
		public BoundsInt GetBlockBounds()
		{
			Vector3Int vector3Int = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
			Vector3Int vector3Int2 = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
			foreach (Vector3i v3i in this.pos)
			{
				vector3Int = Vector3Int.Min(vector3Int, v3i);
				vector3Int2 = Vector3Int.Max(vector3Int2, v3i);
			}
			BoundsInt result = default(BoundsInt);
			result.SetMinMax(vector3Int, vector3Int2);
			return result;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00032F44 File Offset: 0x00031144
		public Vector3i Get(int _idx, int _blockId, int _rotation)
		{
			Vector3 vector = Block.list[_blockId].shape.GetRotation(new BlockValue
			{
				type = _blockId,
				rotation = (byte)_rotation
			}) * this.pos[_idx].ToVector3();
			return new Vector3i(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00032FB9 File Offset: 0x000311B9
		public Vector3i GetParentPos(Vector3i _childPos, BlockValue _blockValue)
		{
			return new Vector3i(_childPos.x + _blockValue.parentx, _childPos.y + _blockValue.parenty, _childPos.z + _blockValue.parentz);
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00032FEC File Offset: 0x000311EC
		public void AddChilds(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.Get(i, _blockValue.type, (int)rotation);
				if (!(vector3i == Vector3i.zero))
				{
					Vector3i vector3i2 = _blockPos + vector3i;
					int x = World.toBlockXZ(vector3i2.x);
					int z = World.toBlockXZ(vector3i2.z);
					int y = vector3i2.y;
					if (y >= 0 && y < 254)
					{
						Chunk chunk = (Chunk)chunkCache.GetChunkFromWorldPos(vector3i2);
						if (chunk == null)
						{
							long num = WorldChunkCache.MakeChunkKey(World.toChunkXZ(vector3i2.x), World.toChunkXZ(vector3i2.z));
							if (_chunk.Key == num)
							{
								chunk = _chunk;
							}
						}
						if (chunk != null)
						{
							BlockValue block = chunk.GetBlock(x, y, z);
							if (block.isair || !block.Block.shape.IsTerrain())
							{
								BlockValue blockValue = _blockValue;
								blockValue.ischild = true;
								blockValue.parentx = -vector3i.x;
								blockValue.parenty = -vector3i.y;
								blockValue.parentz = -vector3i.z;
								chunk.SetBlock(_world, x, y, z, blockValue, false, true, false, false, -1);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00033144 File Offset: 0x00031344
		public void RemoveChilds(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.Get(i, _blockValue.type, (int)rotation);
				if ((vector3i.x != 0 || vector3i.y != 0 || vector3i.z != 0) && chunkCache.GetBlock(_blockPos + vector3i).type == _blockValue.type)
				{
					chunkCache.SetBlock(_blockPos + vector3i, true, BlockValue.Air, true, MarchingCubes.DensityAir, false, false, false, true, -1);
				}
			}
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x000331E0 File Offset: 0x000313E0
		public void RemoveParentBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			Vector3i parentPos = this.GetParentPos(_blockPos, _blockValue);
			BlockValue block = chunkCache.GetBlock(parentPos);
			if (!block.ischild && block.type == _blockValue.type)
			{
				chunkCache.SetBlock(parentPos, BlockValue.Air, true, true);
			}
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x00033238 File Offset: 0x00031438
		public bool ContainsPos(WorldBase _world, Vector3i _parentPos, BlockValue _blockValue, Vector3i _posToCheck)
		{
			if (_world.ChunkCache == null)
			{
				return false;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				if (_parentPos + this.Get(i, _blockValue.type, (int)rotation) == _posToCheck)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0003328C File Offset: 0x0003148C
		public bool ToParentLocalPos(out Vector3i localOffset, WorldBase _world, Vector3i _parentPos, BlockValue _blockValue, Vector3i _posToCheck)
		{
			localOffset = Vector3i.zero;
			if (_world.ChunkCache == null)
			{
				return false;
			}
			byte rotation = _blockValue.rotation;
			for (int i = 0; i < this.Length; i++)
			{
				if (_parentPos + this.Get(i, _blockValue.type, (int)rotation) == _posToCheck)
				{
					localOffset = this.pos[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x000332FC File Offset: 0x000314FC
		public Bounds CalcBounds(int _blockId, int _rotation)
		{
			Quaternion rotation = Block.list[_blockId].shape.GetRotation(new BlockValue
			{
				type = _blockId,
				rotation = (byte)_rotation
			});
			Vector3 vector = Vector3.positiveInfinity;
			Vector3 vector2 = Vector3.negativeInfinity;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3 rhs = rotation * this.pos[i].ToVector3();
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
			}
			Bounds result = default(Bounds);
			result.SetMinMax(vector, vector2);
			return result;
		}

		// Token: 0x0400084D RID: 2125
		public int Length;

		// Token: 0x0400084E RID: 2126
		public Vector3i dim;
	}

	// Token: 0x0200010A RID: 266
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TextureInfo
	{
		// Token: 0x04000850 RID: 2128
		public bool bTextureForEachSide;

		// Token: 0x04000851 RID: 2129
		public int singleTextureId;

		// Token: 0x04000852 RID: 2130
		public int[] sideTextureIds;
	}

	// Token: 0x0200010B RID: 267
	public enum UVMode : byte
	{
		// Token: 0x04000854 RID: 2132
		Default,
		// Token: 0x04000855 RID: 2133
		Global,
		// Token: 0x04000856 RID: 2134
		Local
	}

	// Token: 0x0200010C RID: 268
	public enum EnumDisplayInfo
	{
		// Token: 0x04000858 RID: 2136
		None,
		// Token: 0x04000859 RID: 2137
		Name,
		// Token: 0x0400085A RID: 2138
		Description,
		// Token: 0x0400085B RID: 2139
		Custom
	}

	// Token: 0x0200010D RID: 269
	public enum DestroyedResult
	{
		// Token: 0x0400085D RID: 2141
		None,
		// Token: 0x0400085E RID: 2142
		Keep,
		// Token: 0x0400085F RID: 2143
		Downgrade,
		// Token: 0x04000860 RID: 2144
		Remove
	}
}
