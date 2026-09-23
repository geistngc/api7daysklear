using System;

// Token: 0x02001207 RID: 4615
public static class GameStatsBridge
{
	// Token: 0x06009356 RID: 37718 RVA: 0x0037ADD0 File Offset: 0x00378FD0
	public static void Init()
	{
		GameStats.OnChangedDelegates -= GameStatsBridge.GameStats_OnChangedDelegates;
		GameStats.OnChangedDelegates += GameStatsBridge.GameStats_OnChangedDelegates;
		GameStatsBridge.UpdateStaticFields(EnumGameStats.XPMultiplier, GameStats.GetInt(EnumGameStats.XPMultiplier));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.GlobalGSModifier, GameStats.GetInt(EnumGameStats.GlobalGSModifier));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.BiomeGSModifier, GameStats.GetInt(EnumGameStats.BiomeGSModifier));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.GlobalLSModifier, GameStats.GetInt(EnumGameStats.GlobalLSModifier));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.BiomeLSModifier, GameStats.GetInt(EnumGameStats.BiomeLSModifier));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.BlockDamageAI, GameStats.GetInt(EnumGameStats.BlockDamageAI));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.BlockDamageAIBM, GameStats.GetInt(EnumGameStats.BlockDamageAIBM));
		GameStatsBridge.UpdateStaticFields(EnumGameStats.LootAbundance, GameStats.GetInt(EnumGameStats.LootAbundance));
	}

	// Token: 0x06009357 RID: 37719 RVA: 0x0037AE97 File Offset: 0x00379097
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GameStats_OnChangedDelegates(EnumGameStats _gameState, object _newValue)
	{
		GameStatsBridge.UpdateStaticFields(_gameState, _newValue);
	}

	// Token: 0x06009358 RID: 37720 RVA: 0x0037AEA0 File Offset: 0x003790A0
	[PublicizedFrom(EAccessModifier.Private)]
	public static float ToFloatPercent(object _value)
	{
		if (_value == null)
		{
			return 0f;
		}
		float result;
		try
		{
			result = Convert.ToSingle(_value) / 100f;
		}
		catch (Exception ex)
		{
			Log.Warning(string.Format("GameStatsBridge: unable to convert {0} ({1}) to float: {2}", _value.GetType(), _value, ex.Message));
			result = 0f;
		}
		return result;
	}

	// Token: 0x06009359 RID: 37721 RVA: 0x0037AEFC File Offset: 0x003790FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateStaticFields(EnumGameStats _gameState, object _newValue)
	{
		if (_gameState != EnumGameStats.XPMultiplier)
		{
			switch (_gameState)
			{
			case EnumGameStats.BlockDamageAI:
				ItemActionAttack.EntityBlockDamagePercent = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			case EnumGameStats.BlockDamageAIBM:
				ItemActionAttack.BMBlockDamagePercent = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			case EnumGameStats.LootAbundance:
				LootContainer.GlobalCountModifier = GameStatsBridge.ToFloatPercent(_newValue);
				break;
			case EnumGameStats.LootRespawnDays:
				break;
			case EnumGameStats.GlobalGSModifier:
				EntityPlayer.GlobalGameStageModifier = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			case EnumGameStats.BiomeGSModifier:
				EntityPlayer.BiomeGameStageModifier = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			case EnumGameStats.GlobalLSModifier:
				EntityPlayer.GlobalLootStageModifier = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			case EnumGameStats.BiomeLSModifier:
				EntityPlayer.BiomeLootStageModifier = GameStatsBridge.ToFloatPercent(_newValue);
				return;
			default:
				return;
			}
			return;
		}
		Progression.XPGain = GameStatsBridge.ToFloatPercent(_newValue);
	}
}
