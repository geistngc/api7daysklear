using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

// Token: 0x02000115 RID: 277
[Preserve]
public class BlockCollector : Block
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000742 RID: 1858 RVA: 0x00034264 File Offset: 0x00032464
	public string[] Outputs
	{
		get
		{
			return this.outputs;
		}
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0003426C File Offset: 0x0003246C
	public BlockCollector.FuelType GetFuelType(string name)
	{
		BlockCollector.FuelType result;
		if (!this.fuelTypes.TryGetValue(name, out result))
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0003428C File Offset: 0x0003248C
	public BlockCollector.OutputType GetOutputType(string name)
	{
		BlockCollector.OutputType result;
		if (!this.outputTypes.TryGetValue(name, out result))
		{
			result = null;
		}
		return result;
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000745 RID: 1861 RVA: 0x000342AC File Offset: 0x000324AC
	public string[] ItemIconBackdrop
	{
		get
		{
			return this.itemIconBackdrop;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000746 RID: 1862 RVA: 0x000342B4 File Offset: 0x000324B4
	public string ItemBackgroundColor
	{
		get
		{
			return this.itemBackgroundColor;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000747 RID: 1863 RVA: 0x000342BC File Offset: 0x000324BC
	public string LootLabelKey
	{
		get
		{
			return this.lootLabelKey;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000748 RID: 1864 RVA: 0x000342C4 File Offset: 0x000324C4
	public string ActivationEvent
	{
		get
		{
			return this.activationEvent;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000749 RID: 1865 RVA: 0x000342CC File Offset: 0x000324CC
	public string CloseEvent
	{
		get
		{
			return this.closeEvent;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x0600074A RID: 1866 RVA: 0x000342D4 File Offset: 0x000324D4
	public string[] RequiredMods
	{
		get
		{
			return this.requiredMods;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600074B RID: 1867 RVA: 0x000342DC File Offset: 0x000324DC
	public bool RequiredModsOnly
	{
		get
		{
			return this.requiredModsOnly;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600074C RID: 1868 RVA: 0x000342E4 File Offset: 0x000324E4
	public string[] ModTransformEnableNames
	{
		get
		{
			return this.modTransformEnableNames;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x0600074D RID: 1869 RVA: 0x000342EC File Offset: 0x000324EC
	public string[] ModTransformDisableNames
	{
		get
		{
			return this.modTransformDisableNames;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600074E RID: 1870 RVA: 0x000342F4 File Offset: 0x000324F4
	public int FuelGridInitialHeight
	{
		get
		{
			return this.fuelGridHeight;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x0600074F RID: 1871 RVA: 0x000342FC File Offset: 0x000324FC
	public int CatalystGridInitialHeight
	{
		get
		{
			return this.catalystGridHeight;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000750 RID: 1872 RVA: 0x00034304 File Offset: 0x00032504
	public int OutputGridInitialHeight
	{
		get
		{
			return this.outputGridInitialHeight;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000751 RID: 1873 RVA: 0x0003430C File Offset: 0x0003250C
	public string FuelTitleText
	{
		get
		{
			return this.fuelTitleText;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000752 RID: 1874 RVA: 0x00034314 File Offset: 0x00032514
	public string FuelTitleSprite
	{
		get
		{
			return this.fuelTitleSprite;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000753 RID: 1875 RVA: 0x0003431C File Offset: 0x0003251C
	public string FuelTitleSpriteAtlas
	{
		get
		{
			return this.fuelTitleSpriteAtlas;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000754 RID: 1876 RVA: 0x00034324 File Offset: 0x00032524
	public string CatalystTitleText
	{
		get
		{
			return this.catalystTitleText;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000755 RID: 1877 RVA: 0x0003432C File Offset: 0x0003252C
	public string CatalystTitleSprite
	{
		get
		{
			return this.catalystTitleSprite;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000756 RID: 1878 RVA: 0x00034334 File Offset: 0x00032534
	public string CatalystTitleSpriteAtlas
	{
		get
		{
			return this.catalystTitleSpriteAtlas;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000757 RID: 1879 RVA: 0x0003433C File Offset: 0x0003253C
	public string[] FuelTypesSprites
	{
		get
		{
			return this.fuelTypesSprites;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000758 RID: 1880 RVA: 0x00034344 File Offset: 0x00032544
	public string[] FuelTypesSpriteAtlases
	{
		get
		{
			return this.fuelTypesSpriteAtlases;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000759 RID: 1881 RVA: 0x0003434C File Offset: 0x0003254C
	public string[] CatalystTypesSprites
	{
		get
		{
			return this.catalystTypesSprites;
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x0600075A RID: 1882 RVA: 0x00034354 File Offset: 0x00032554
	public string[] CatalystTypesSpriteAtlases
	{
		get
		{
			return this.catalystTypesSpriteAtlases;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600075B RID: 1883 RVA: 0x0003435C File Offset: 0x0003255C
	public string[] CatalystTypes
	{
		get
		{
			return this.catalystTypes;
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00034364 File Offset: 0x00032564
	public BlockCollector()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000344B4 File Offset: 0x000326B4
	public override void Init()
	{
		base.Init();
		string text = "";
		base.Properties.ParseString(BlockCollector.PropCollectorType, ref text);
		if (text != "")
		{
			Enum.TryParse<BlockCollector.CollectorTypes>(text, out this.CollectorType);
		}
		string text2 = null;
		base.Properties.ParseString(BlockCollector.PropFuelTypes, ref text2);
		string[] array = text2.TrimEnd('}').Split('{', 125, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			BlockCollector.FuelType fuelType = new BlockCollector.FuelType(array[i]);
			this.fuelTypes[fuelType.Name] = fuelType;
		}
		string text3 = null;
		base.Properties.ParseString(BlockCollector.PropOutputTypes, ref text3);
		string[] array2 = text3.Split('{', 125, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = array2[j].TrimEnd('}');
			BlockCollector.OutputType outputType = new BlockCollector.OutputType(array2[j]);
			this.outputTypes.Add(outputType.Name, outputType);
		}
		string text4 = null;
		base.Properties.ParseString(BlockCollector.PropOutputs, ref text4);
		this.outputs = text4.Split(',', StringSplitOptions.None);
		for (int k = 0; k < this.outputs.Length; k++)
		{
			BlockCollector.OutputType outputType2 = this.GetOutputType(this.outputs[k]);
			if (outputType2 != null)
			{
				List<int> list;
				if (!this.OrderedSlotOutputs.TryGetValue(outputType2, out list))
				{
					list = new List<int>();
					this.OrderedSlotOutputs[outputType2] = list;
				}
				list.Add(k);
			}
		}
		base.Properties.ParseString(BlockCollector.PropOpenSound, ref this.OpenSound);
		base.Properties.ParseString(BlockCollector.PropRunningSound, ref this.RunningSound);
		base.Properties.ParseString(BlockCollector.PropActivateSound, ref this.ActivateSound);
		base.Properties.ParseString(BlockCollector.PropCloseSound, ref this.CloseSound);
		base.Properties.ParseFloat(BlockCollector.PropTakeDelay, ref this.TakeDelay);
		string text5 = null;
		base.Properties.ParseString(BlockCollector.PropItemIconBackdrop, ref text5);
		this.itemIconBackdrop = text5.Split(',', StringSplitOptions.None);
		base.Properties.ParseString(BlockCollector.PropLootLabelKey, ref this.lootLabelKey);
		base.Properties.ParseString(BlockCollector.PropActivationEvent, ref this.activationEvent);
		base.Properties.ParseString(BlockCollector.PropCloseEvent, ref this.closeEvent);
		base.Properties.ParseString(BlockCollector.PropItemBackgroundColor, ref this.itemBackgroundColor);
		string text6 = "";
		this.requiredMods = null;
		base.Properties.ParseString(BlockCollector.PropRequiredMods, ref text6);
		if (!string.IsNullOrEmpty(text6))
		{
			this.requiredMods = text6.Split(',', StringSplitOptions.None);
		}
		base.Properties.ParseBool(BlockCollector.PropRequiredModsOnly, ref this.requiredModsOnly);
		string text7 = ",,";
		base.Properties.ParseString(BlockCollector.PropModTransformEnableNames, ref text7);
		this.modTransformEnableNames = text7.Split(',', StringSplitOptions.None);
		text7 = ",,";
		base.Properties.ParseString(BlockCollector.PropModTransformDisableNames, ref text7);
		this.modTransformDisableNames = text7.Split(',', StringSplitOptions.None);
		text7 = "Count,Speed,Type";
		base.Properties.ParseString(BlockCollector.PropModTypes, ref text7);
		string[] array3 = text7.Split(',', StringSplitOptions.None);
		this.ModTypes = new BlockCollector.ModEffectTypes[array3.Length];
		int num = 0;
		while (num < array3.Length && num < this.ModTypes.Length)
		{
			this.ModTypes[num] = Enum.Parse<BlockCollector.ModEffectTypes>(array3[num]);
			num++;
		}
		base.Properties.ParseString(BlockCollector.PropEmptyText, ref this.emptyText);
		base.Properties.ParseString(BlockCollector.PropHasItem1Text, ref this.hasItem1Text);
		base.Properties.ParseString(BlockCollector.PropHasItem2Text, ref this.hasItem2Text);
		base.Properties.ParseInt(BlockCollector.PropOutputGridHeight, ref this.outputGridInitialHeight);
		base.Properties.ParseInt(BlockCollector.PropFuelGridHeight, ref this.fuelGridHeight);
		base.Properties.ParseString(BlockCollector.PropFuelTitleText, ref this.fuelTitleText);
		base.Properties.ParseString(BlockCollector.PropFuelTitleSprite, ref this.fuelTitleSprite);
		base.Properties.ParseString(BlockCollector.PropFuelTitleSpriteAtlas, ref this.fuelTitleSpriteAtlas);
		string text8 = null;
		base.Properties.ParseString(BlockCollector.PropFuelTypesSprites, ref text8);
		if (text8 != null)
		{
			string[] array4 = text8.Split(',', StringSplitOptions.None);
			int num2 = array4.Length / 2;
			this.fuelTypesSprites = new string[num2];
			this.fuelTypesSpriteAtlases = new string[num2];
			for (int l = 0; l < num2; l++)
			{
				int num3 = l * 2;
				this.fuelTypesSprites[l] = array4[num3];
				this.FuelTypesSpriteAtlases[l] = array4[num3 + 1];
			}
		}
		base.Properties.ParseInt(BlockCollector.PropCatalystGridHeight, ref this.catalystGridHeight);
		text8 = null;
		base.Properties.ParseString(BlockCollector.PropCatalystTypes, ref text8);
		if (text8 != null)
		{
			this.catalystTypes = text8.Split(',', StringSplitOptions.None);
		}
		base.Properties.ParseString(BlockCollector.PropCatalystTypes, ref text8);
		if (text8 != null)
		{
			string[] array5 = text8.Split(',', StringSplitOptions.None);
			int num4 = array5.Length / 2;
			this.CatalystConverts = new BlockCollector.CatalystConvert[num4];
			for (int m = 0; m < num4; m++)
			{
				this.CatalystConverts[m] = new BlockCollector.CatalystConvert(array5[m * 2], array5[m * 2 + 1]);
			}
		}
		base.Properties.ParseString(BlockCollector.PropCatalystMultiplier, ref text8);
		if (text8 != null)
		{
			array = text8.Split(",", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array6 = array[i].Split("=", StringSplitOptions.None);
				if (array6.Length == 2)
				{
					int value;
					if (!int.TryParse(array6[1], out value))
					{
						value = 1;
					}
					this.CatalystMultipliers.Add(array6[0], value);
				}
			}
		}
		base.Properties.ParseString(BlockCollector.PropCatalystRequirements, ref text8);
		if (text8 != null)
		{
			array = text8.Split(",", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array7 = array[i].Split("=", StringSplitOptions.None);
				if (array7.Length == 2)
				{
					int value2;
					if (!int.TryParse(array7[1], out value2))
					{
						value2 = 1;
					}
					this.CatalystRequirements.Add(array7[0], value2);
				}
			}
		}
		base.Properties.ParseString(BlockCollector.PropCatalystTitleText, ref this.catalystTitleText);
		base.Properties.ParseString(BlockCollector.PropCatalystTitleSprite, ref this.catalystTitleSprite);
		base.Properties.ParseString(BlockCollector.PropCatalystTitleSpriteAtlas, ref this.catalystTitleSpriteAtlas);
		base.Properties.ParseString(BlockCollector.PropCatalystTypesSprites, ref text8);
		if (text8 != null)
		{
			string[] array8 = text8.Split(',', StringSplitOptions.None);
			int num5 = array8.Length / 2;
			this.catalystTypesSprites = new string[num5];
			this.catalystTypesSpriteAtlases = new string[num5];
			for (int n = 0; n < num5; n++)
			{
				int num6 = n * 2;
				this.catalystTypesSprites[n] = array8[num6];
				this.catalystTypesSpriteAtlases[n] = array8[num6 + 1];
			}
		}
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00034B97 File Offset: 0x00032D97
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		this.addTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00034BBB File Offset: 0x00032DBB
	public override bool OnBlockActivated(string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "Search")
		{
			return this.OnBlockActivated(_world, _blockPos, _blockValue, _player);
		}
		if (!(_commandName == "take"))
		{
			return false;
		}
		base.takeItemWithTimer(_blockPos, _blockValue, _player, this.TakeDelay);
		return true;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00034BFC File Offset: 0x00032DFC
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.cmds[1].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x00034C48 File Offset: 0x00032E48
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityCollector tileEntityCollector = _world.GetTileEntity(_result.blockPos) as TileEntityCollector;
		if (tileEntityCollector == null)
		{
			return;
		}
		if (_ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityCollector.worldTimeTouched = _world.GetWorldTime();
			tileEntityCollector.SetEmpty();
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00034C98 File Offset: 0x00032E98
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityCollector tileEntityCollector = _world.GetTileEntity(_blockPos) as TileEntityCollector;
		if (tileEntityCollector == null)
		{
			return string.Empty;
		}
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if (tileEntityCollector.IsWaterEmpty())
		{
			return string.Format(Localization.Get(this.emptyText, false, null), arg, localizedBlockName);
		}
		if (tileEntityCollector.HasModConvert)
		{
			return string.Format(Localization.Get(this.hasItem2Text, false, null), arg, localizedBlockName);
		}
		return string.Format(Localization.Get(this.hasItem1Text, false, null), arg, localizedBlockName);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00034D4C File Offset: 0x00032F4C
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		TileEntityCollector tileEntityCollector = world.GetTileEntity(_blockPos) as TileEntityCollector;
		if (tileEntityCollector != null)
		{
			tileEntityCollector.OnDestroy();
		}
		this.removeTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00034D88 File Offset: 0x00032F88
	[PublicizedFrom(EAccessModifier.Protected)]
	public void addTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		TileEntityCollector tileEntityCollector = new TileEntityCollector(_chunk);
		tileEntityCollector.localChunkPos = World.toBlock(_blockPos);
		tileEntityCollector.SetWorldTime();
		_chunk.AddTileEntity(tileEntityCollector);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00034DB5 File Offset: 0x00032FB5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void removeTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		_chunk.RemoveTileEntityAt<TileEntityCollector>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00034DCC File Offset: 0x00032FCC
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityCollector tileEntityCollector = _world.GetTileEntity(_bvRef) as TileEntityCollector;
		if (tileEntityCollector != null)
		{
			tileEntityCollector.OnDestroy();
		}
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_DewCollectorWindowGroup.CloseIfOpenAtPos(_bvRef, null);
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00034E03 File Offset: 0x00033003
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
		this.UpdateVisible(_world, _blockPos);
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00034E18 File Offset: 0x00033018
	public void UpdateVisible(WorldBase _world, Vector3i _blockPos)
	{
		TileEntityCollector tileEntityCollector = _world.GetTileEntity(_blockPos) as TileEntityCollector;
		if (tileEntityCollector != null)
		{
			tileEntityCollector.UpdateVisible();
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00034E3C File Offset: 0x0003303C
	public override bool OnBlockActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_player.inventory.IsHoldingItemActionRunning())
		{
			return false;
		}
		TileEntityCollector tileEntityCollector = _world.GetTileEntity(_blockPos) as TileEntityCollector;
		if (tileEntityCollector == null)
		{
			return false;
		}
		_player.AimingGun = false;
		LockManager.Instance.LockRequestLocal(tileEntityCollector, null, 0);
		return true;
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00034E84 File Offset: 0x00033084
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool takeItemWithTimerCanTake(Vector3i _blockPos, EntityAlive _player)
	{
		if ((GameManager.Instance.World.GetTileEntity(_blockPos) as TileEntityCollector).IsEmpty())
		{
			return true;
		}
		GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttWorkstationNotEmpty", false, null), string.Empty, "ui_denied", null, false, false, 0f);
		return false;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00034EDC File Offset: 0x000330DC
	public bool UsesFuel()
	{
		bool flag = false;
		switch (this.CollectorType)
		{
		case BlockCollector.CollectorTypes.DewCollector:
			flag = (XUiM_Recipes.DewCollectorInput == 0f);
			break;
		case BlockCollector.CollectorTypes.Apiary:
			flag = (XUiM_Recipes.ApiaryInput == 0f);
			break;
		case BlockCollector.CollectorTypes.ChickenCoop:
			flag = (XUiM_Recipes.ChickenCoopInput == 0f);
			break;
		}
		return !flag && this.fuelTypes.Values.Count > 0;
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00034F49 File Offset: 0x00033149
	public bool UsesCatalyst()
	{
		return this.CatalystTypes.Length != 0;
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00034F55 File Offset: 0x00033155
	[PublicizedFrom(EAccessModifier.Private)]
	public int modifyTime(int time, float modifier)
	{
		if (modifier == 0f)
		{
			time = -1;
		}
		else
		{
			time = (int)(1f / modifier * (float)time);
		}
		return time;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00034F74 File Offset: 0x00033174
	public int GetSandboxModifiedTime(int time)
	{
		if (time != 0)
		{
			switch (this.CollectorType)
			{
			case BlockCollector.CollectorTypes.DewCollector:
				time = this.modifyTime(time, XUiM_Recipes.DewCollectorTimeModifier);
				break;
			case BlockCollector.CollectorTypes.Apiary:
				time = this.modifyTime(time, XUiM_Recipes.ApiaryTimeModifier);
				break;
			case BlockCollector.CollectorTypes.ChickenCoop:
				time = this.modifyTime(time, XUiM_Recipes.ChickenCoopTimeModifier);
				break;
			}
		}
		return time;
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x00034FD0 File Offset: 0x000331D0
	public int GetSandboxModifiedFuelNeeded(int cost)
	{
		switch (this.CollectorType)
		{
		case BlockCollector.CollectorTypes.DewCollector:
			cost = (int)(XUiM_Recipes.DewCollectorInput * (float)cost);
			break;
		case BlockCollector.CollectorTypes.Apiary:
			cost = (int)(XUiM_Recipes.ApiaryInput * (float)cost);
			break;
		case BlockCollector.CollectorTypes.ChickenCoop:
			cost = (int)(XUiM_Recipes.ChickenCoopInput * (float)cost);
			break;
		}
		return cost;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00035020 File Offset: 0x00033220
	public int GetSandboxModifiedOutput(int cost)
	{
		switch (this.CollectorType)
		{
		case BlockCollector.CollectorTypes.DewCollector:
			cost = (int)(XUiM_Recipes.DewCollectorOutput * (float)cost);
			break;
		case BlockCollector.CollectorTypes.Apiary:
			cost = (int)(XUiM_Recipes.ApiaryOutput * (float)cost);
			break;
		case BlockCollector.CollectorTypes.ChickenCoop:
			cost = (int)(XUiM_Recipes.ChickenCoopOutput * (float)cost);
			break;
		}
		return cost;
	}

	// Token: 0x0400087A RID: 2170
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCollectorType = "CollectorType";

	// Token: 0x0400087B RID: 2171
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelTypes = "FuelTypes";

	// Token: 0x0400087C RID: 2172
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOutputTypes = "OutputTypes";

	// Token: 0x0400087D RID: 2173
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOutputs = "Outputs";

	// Token: 0x0400087E RID: 2174
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOpenSound = "OpenSound";

	// Token: 0x0400087F RID: 2175
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRunningSound = "RunningSound";

	// Token: 0x04000880 RID: 2176
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivateSound = "ActivateSound";

	// Token: 0x04000881 RID: 2177
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCloseSound = "CloseSound";

	// Token: 0x04000882 RID: 2178
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTakeDelay = "TakeDelay";

	// Token: 0x04000883 RID: 2179
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropItemIconBackdrop = "ItemIconBackdrop";

	// Token: 0x04000884 RID: 2180
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLootLabelKey = "LootLabelKey";

	// Token: 0x04000885 RID: 2181
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivationEvent = "ActivationEvent";

	// Token: 0x04000886 RID: 2182
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCloseEvent = "CloseEvent";

	// Token: 0x04000887 RID: 2183
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRequiredMods = "RequiredMods";

	// Token: 0x04000888 RID: 2184
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRequiredModsOnly = "RequiredModsOnly";

	// Token: 0x04000889 RID: 2185
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModTransformEnableNames = "ModTransformEnableNames";

	// Token: 0x0400088A RID: 2186
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModTransformDisableNames = "ModTransformDisableNames";

	// Token: 0x0400088B RID: 2187
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModTypes = "ModTypes";

	// Token: 0x0400088C RID: 2188
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropItemBackgroundColor = "ItemBackgroundColor";

	// Token: 0x0400088D RID: 2189
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEmptyText = "EmptyText";

	// Token: 0x0400088E RID: 2190
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHasItem1Text = "HasItem1Text";

	// Token: 0x0400088F RID: 2191
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHasItem2Text = "HasItem2Text";

	// Token: 0x04000890 RID: 2192
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOutputGridHeight = "OutputGridHeight";

	// Token: 0x04000891 RID: 2193
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelGridHeight = "FuelGridHeight";

	// Token: 0x04000892 RID: 2194
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelTitleText = "FuelTitleText";

	// Token: 0x04000893 RID: 2195
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelTitleSprite = "FuelTitleSprite";

	// Token: 0x04000894 RID: 2196
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelTitleSpriteAtlas = "FuelTitleSpriteAtlas";

	// Token: 0x04000895 RID: 2197
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelTypesSprites = "FuelTypesSprites";

	// Token: 0x04000896 RID: 2198
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystGridHeight = "CatalystGridHeight";

	// Token: 0x04000897 RID: 2199
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystTypes = "CatalystTypes";

	// Token: 0x04000898 RID: 2200
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystMultiplier = "CatalystMultiplier";

	// Token: 0x04000899 RID: 2201
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystRequirements = "CatalystRequirements";

	// Token: 0x0400089A RID: 2202
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystConvert = "CatalystConvert";

	// Token: 0x0400089B RID: 2203
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystTitleText = "CatalystTitleText";

	// Token: 0x0400089C RID: 2204
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystTitleSprite = "CatalystTitleSprite";

	// Token: 0x0400089D RID: 2205
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystTitleSpriteAtlas = "CatalystTitleSpriteAtlas";

	// Token: 0x0400089E RID: 2206
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCatalystTypesSprites = "CatalystTypesSprites";

	// Token: 0x0400089F RID: 2207
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] outputs;

	// Token: 0x040008A0 RID: 2208
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockCollector.CollectorTypes CollectorType;

	// Token: 0x040008A1 RID: 2209
	public Dictionary<BlockCollector.OutputType, List<int>> OrderedSlotOutputs = new Dictionary<BlockCollector.OutputType, List<int>>();

	// Token: 0x040008A2 RID: 2210
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, BlockCollector.FuelType> fuelTypes = new Dictionary<string, BlockCollector.FuelType>();

	// Token: 0x040008A3 RID: 2211
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, BlockCollector.OutputType> outputTypes = new Dictionary<string, BlockCollector.OutputType>();

	// Token: 0x040008A4 RID: 2212
	public string OpenSound;

	// Token: 0x040008A5 RID: 2213
	public string CloseSound;

	// Token: 0x040008A6 RID: 2214
	public string RunningSound;

	// Token: 0x040008A7 RID: 2215
	public string ActivateSound;

	// Token: 0x040008A8 RID: 2216
	[PublicizedFrom(EAccessModifier.Protected)]
	public float TakeDelay = 2f;

	// Token: 0x040008A9 RID: 2217
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] itemIconBackdrop;

	// Token: 0x040008AA RID: 2218
	[PublicizedFrom(EAccessModifier.Private)]
	public string itemBackgroundColor = "255,255,255,255";

	// Token: 0x040008AB RID: 2219
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootLabelKey;

	// Token: 0x040008AC RID: 2220
	[PublicizedFrom(EAccessModifier.Private)]
	public string activationEvent;

	// Token: 0x040008AD RID: 2221
	[PublicizedFrom(EAccessModifier.Private)]
	public string closeEvent;

	// Token: 0x040008AE RID: 2222
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] requiredMods;

	// Token: 0x040008AF RID: 2223
	[PublicizedFrom(EAccessModifier.Private)]
	public bool requiredModsOnly;

	// Token: 0x040008B0 RID: 2224
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] modTransformEnableNames;

	// Token: 0x040008B1 RID: 2225
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] modTransformDisableNames;

	// Token: 0x040008B2 RID: 2226
	[PublicizedFrom(EAccessModifier.Protected)]
	public string emptyText = "CollectorEmptyText";

	// Token: 0x040008B3 RID: 2227
	[PublicizedFrom(EAccessModifier.Protected)]
	public string hasItem1Text = "CollectorHasItem1Text";

	// Token: 0x040008B4 RID: 2228
	[PublicizedFrom(EAccessModifier.Protected)]
	public string hasItem2Text = "CollectorHasItem2Text";

	// Token: 0x040008B5 RID: 2229
	[PublicizedFrom(EAccessModifier.Protected)]
	public int fuelGridHeight = 1;

	// Token: 0x040008B6 RID: 2230
	[PublicizedFrom(EAccessModifier.Protected)]
	public int catalystGridHeight = 1;

	// Token: 0x040008B7 RID: 2231
	[PublicizedFrom(EAccessModifier.Protected)]
	public int outputGridInitialHeight = 1;

	// Token: 0x040008B8 RID: 2232
	[PublicizedFrom(EAccessModifier.Protected)]
	public string fuelTitleText;

	// Token: 0x040008B9 RID: 2233
	[PublicizedFrom(EAccessModifier.Protected)]
	public string fuelTitleSprite = string.Empty;

	// Token: 0x040008BA RID: 2234
	[PublicizedFrom(EAccessModifier.Protected)]
	public string fuelTitleSpriteAtlas = string.Empty;

	// Token: 0x040008BB RID: 2235
	[PublicizedFrom(EAccessModifier.Protected)]
	public string catalystTitleText;

	// Token: 0x040008BC RID: 2236
	[PublicizedFrom(EAccessModifier.Protected)]
	public string catalystTitleSprite = string.Empty;

	// Token: 0x040008BD RID: 2237
	[PublicizedFrom(EAccessModifier.Protected)]
	public string catalystTitleSpriteAtlas = string.Empty;

	// Token: 0x040008BE RID: 2238
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] fuelTypesSprites = new string[0];

	// Token: 0x040008BF RID: 2239
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] fuelTypesSpriteAtlases = new string[0];

	// Token: 0x040008C0 RID: 2240
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] catalystTypesSprites = new string[0];

	// Token: 0x040008C1 RID: 2241
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] catalystTypesSpriteAtlases = new string[0];

	// Token: 0x040008C2 RID: 2242
	public BlockCollector.ModEffectTypes[] ModTypes;

	// Token: 0x040008C3 RID: 2243
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] catalystTypes = new string[0];

	// Token: 0x040008C4 RID: 2244
	public Dictionary<string, int> CatalystMultipliers = new Dictionary<string, int>();

	// Token: 0x040008C5 RID: 2245
	public Dictionary<string, int> CatalystRequirements = new Dictionary<string, int>();

	// Token: 0x040008C6 RID: 2246
	public BlockCollector.CatalystConvert[] CatalystConverts = new BlockCollector.CatalystConvert[0];

	// Token: 0x040008C7 RID: 2247
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("Search", "search", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};

	// Token: 0x02000116 RID: 278
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum CollectorTypes
	{
		// Token: 0x040008C9 RID: 2249
		DewCollector,
		// Token: 0x040008CA RID: 2250
		Apiary,
		// Token: 0x040008CB RID: 2251
		ChickenCoop
	}

	// Token: 0x02000117 RID: 279
	public class CatalystConvert
	{
		// Token: 0x06000773 RID: 1907 RVA: 0x000351EF File Offset: 0x000333EF
		public CatalystConvert(string _convertFrom, string _convertTo)
		{
			this.convertFrom = _convertFrom;
			this.convertTo = _convertTo;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00035208 File Offset: 0x00033408
		public ItemStack Convert(ItemStack _inStack)
		{
			ItemStack result = null;
			if (!_inStack.IsEmpty() && _inStack.itemValue.ItemClass.Name == this.convertFrom)
			{
				result = new ItemStack(ItemClass.GetItem(this.convertTo, false), _inStack.count);
			}
			return result;
		}

		// Token: 0x040008CC RID: 2252
		[PublicizedFrom(EAccessModifier.Private)]
		public string convertFrom;

		// Token: 0x040008CD RID: 2253
		[PublicizedFrom(EAccessModifier.Private)]
		public string convertTo;
	}

	// Token: 0x02000118 RID: 280
	public class FuelType
	{
		// Token: 0x06000775 RID: 1909 RVA: 0x00035258 File Offset: 0x00033458
		public FuelType(string ftDef)
		{
			string[] array = ftDef.Split(',', StringSplitOptions.None);
			this.Name = array[0];
			this.Items = array.Skip(1).ToArray<string>();
			for (int i = 0; i < this.Items.Length; i++)
			{
				this.Items[i] = this.Items[i].Trim();
			}
		}

		// Token: 0x040008CE RID: 2254
		public string Name;

		// Token: 0x040008CF RID: 2255
		public string[] Items;
	}

	// Token: 0x02000119 RID: 281
	public class OutputType
	{
		// Token: 0x06000776 RID: 1910 RVA: 0x000352B8 File Offset: 0x000334B8
		public OutputType(string otDef)
		{
			string[] array = otDef.Split(",", StringSplitOptions.None);
			int num = 0;
			this.Name = array[num++];
			this.Fuel = array[num++];
			this.FuelCost = this.safeParseInt(array[num++]);
			this.AdditionalFuelCost = this.safeParseInt(array[num++]);
			this.DiscountedFuelDivisor = this.safeParseInt(array[num++]);
			this.OutputItem = array[num++];
			this.OutputItemModded = array[num++];
			this.ModdedConvertSpeedMultiplier = this.safeParseInt(array[num++]);
			this.ModdedConvertCountMultiplier = this.safeParseInt(array[num++]);
			this.MinConvertTime = this.safeParseInt(array[num++]);
			this.MaxConvertTime = this.safeParseInt(array[num++]);
			this.ConvertSound = array[num++];
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x000353A0 File Offset: 0x000335A0
		[PublicizedFrom(EAccessModifier.Private)]
		public int safeParseInt(string intString)
		{
			int result;
			if (!int.TryParse(intString, out result))
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x040008D0 RID: 2256
		public string Name;

		// Token: 0x040008D1 RID: 2257
		public string Fuel;

		// Token: 0x040008D2 RID: 2258
		public int FuelCost;

		// Token: 0x040008D3 RID: 2259
		public int AdditionalFuelCost;

		// Token: 0x040008D4 RID: 2260
		public int DiscountedFuelDivisor;

		// Token: 0x040008D5 RID: 2261
		public string OutputItem;

		// Token: 0x040008D6 RID: 2262
		public string OutputItemModded;

		// Token: 0x040008D7 RID: 2263
		public int ModdedConvertSpeedMultiplier;

		// Token: 0x040008D8 RID: 2264
		public int ModdedConvertCountMultiplier;

		// Token: 0x040008D9 RID: 2265
		public int MinConvertTime;

		// Token: 0x040008DA RID: 2266
		public int MaxConvertTime;

		// Token: 0x040008DB RID: 2267
		public string ConvertSound;
	}

	// Token: 0x0200011A RID: 282
	public enum ModEffectTypes
	{
		// Token: 0x040008DD RID: 2269
		Type,
		// Token: 0x040008DE RID: 2270
		Speed,
		// Token: 0x040008DF RID: 2271
		Count,
		// Token: 0x040008E0 RID: 2272
		Modify,
		// Token: 0x040008E1 RID: 2273
		Expand,
		// Token: 0x040008E2 RID: 2274
		Cost
	}
}
