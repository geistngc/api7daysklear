using System;
using UnityEngine.Scripting;

// Token: 0x020011CF RID: 4559
[Preserve]
public class GameModeEditWorld : GameModeAbstract
{
	// Token: 0x060091DC RID: 37340 RVA: 0x0036FD95 File Offset: 0x0036DF95
	public override string GetName()
	{
		return "gmEditWorld";
	}

	// Token: 0x060091DD RID: 37341 RVA: 0x0036FD9C File Offset: 0x0036DF9C
	public override string GetDescription()
	{
		return "gmEditWorldDesc";
	}

	// Token: 0x060091DE RID: 37342 RVA: 0x00081531 File Offset: 0x0007F731
	public override int GetID()
	{
		return 8;
	}

	// Token: 0x060091DF RID: 37343 RVA: 0x0036FDA4 File Offset: 0x0036DFA4
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerIsPublic, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerVisibility, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerMaxPlayerCount, GamePrefs.EnumType.Int, 8, null)
		};
	}

	// Token: 0x060091E0 RID: 37344 RVA: 0x0036FE14 File Offset: 0x0036E014
	public override void Init()
	{
		base.Init();
		GameStats.Set(EnumGameStats.ShowSpawnWindow, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.IsSpawnNearOtherPlayer, true);
		GameStats.Set(EnumGameStats.ShowWindow, "");
		GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, 0);
		GameStats.Set(EnumGameStats.IsSpawnEnemies, false);
		GameStats.Set(EnumGameStats.IsTeleportEnabled, true);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
		GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);
		GameStats.Set(EnumGameStats.IsPlayerDamageEnabled, false);
		GameStats.Set(EnumGameStats.AirDropFrequency, 0);
		GameStats.Set(EnumGameStats.AutoParty, true);
		GamePrefs.Set(EnumGamePrefs.DynamicSpawner, "");
	}

	// Token: 0x060091E1 RID: 37345 RVA: 0x0002003D File Offset: 0x0001E23D
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x060091E2 RID: 37346 RVA: 0x0036FA24 File Offset: 0x0036DC24
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x060091E3 RID: 37347 RVA: 0x000027FC File Offset: 0x000009FC
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x060091E4 RID: 37348 RVA: 0x0036FE9C File Offset: 0x0036E09C
	public override string GetAdditionalGameInfo(World _world)
	{
		switch (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode))
		{
		case 0:
			return "Player move";
		case 1:
			return "Selection move " + ((GamePrefs.GetInt(EnumGamePrefs.SelectionContextMode) == 0) ? "absolute" : "relative");
		case 2:
			return "Selection size";
		default:
			return "";
		}
	}

	// Token: 0x04006BF1 RID: 27633
	public static readonly string TypeName = typeof(GameModeEditWorld).Name;
}
