using System;
using System.Globalization;
using System.IO;
using SandboxOptions;

// Token: 0x02001203 RID: 4611
public class GameStats
{
	// Token: 0x140000FA RID: 250
	// (add) Token: 0x0600933A RID: 37690 RVA: 0x00379FFC File Offset: 0x003781FC
	// (remove) Token: 0x0600933B RID: 37691 RVA: 0x0037A030 File Offset: 0x00378230
	public static event GameStats.OnChangedDelegate OnChangedDelegates;

	// Token: 0x0600933C RID: 37692 RVA: 0x0037A064 File Offset: 0x00378264
	[PublicizedFrom(EAccessModifier.Private)]
	public void initPropertyDecl()
	{
		this.propertyList = new GameStats.PropertyDecl[]
		{
			new GameStats.PropertyDecl(EnumGameStats.GameState, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.GameModeId, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.TimeLimitActive, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.TimeLimitThisRound, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.FragLimitActive, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.FragLimitThisRound, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.DayLimitActive, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.DayLimitThisRound, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.ShowWindow, true, GameStats.EnumType.String, string.Empty),
			new GameStats.PropertyDecl(EnumGameStats.LoadScene, true, GameStats.EnumType.String, string.Empty),
			new GameStats.PropertyDecl(EnumGameStats.CurrentRoundIx, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.ShowAllPlayersOnMap, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.ShowFriendPlayerOnMap, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.ShowSpawnWindow, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.IsSpawnNearOtherPlayer, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.TimeOfDayIncPerSec, true, GameStats.EnumType.Int, 20),
			new GameStats.PropertyDecl(EnumGameStats.IsCreativeMenuEnabled, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.IsTeleportEnabled, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.IsFlyingEnabled, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.IsPlayerDamageEnabled, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.IsPlayerCollisionEnabled, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.IsSaveSupplyCrates, false, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.IsResetMapOnRestart, false, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.IsSpawnEnemies, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.PlayerKillingMode, true, GameStats.EnumType.Int, EnumPlayerKillingMode.KillStrangersOnly),
			new GameStats.PropertyDecl(EnumGameStats.ScorePlayerKillMultiplier, true, GameStats.EnumType.Int, 1),
			new GameStats.PropertyDecl(EnumGameStats.ScoreZombieKillMultiplier, true, GameStats.EnumType.Int, 1),
			new GameStats.PropertyDecl(EnumGameStats.ScoreDiedMultiplier, true, GameStats.EnumType.Int, -5),
			new GameStats.PropertyDecl(EnumGameStats.EnemyCount, false, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.AnimalCount, false, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.IsVersionCheckDone, false, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.ZombieHordeMeter, false, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.DropOnDeath, true, GameStats.EnumType.Int, 1),
			new GameStats.PropertyDecl(EnumGameStats.DropOnQuit, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.GameDifficulty, true, GameStats.EnumType.Int, 2),
			new GameStats.PropertyDecl(EnumGameStats.BloodMoonEnemyCount, true, GameStats.EnumType.Int, 8),
			new GameStats.PropertyDecl(EnumGameStats.EnemySpawnMode, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.EnemyDifficulty, true, GameStats.EnumType.Int, EnumEnemyDifficulty.Normal),
			new GameStats.PropertyDecl(EnumGameStats.DayLightLength, true, GameStats.EnumType.Int, 18),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimCount, true, GameStats.EnumType.Int, 5),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimSize, true, GameStats.EnumType.Int, 41),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimDeadZone, true, GameStats.EnumType.Int, 30),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimExpiryTime, true, GameStats.EnumType.Int, 3),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimDecayMode, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimOnlineDurabilityModifier, true, GameStats.EnumType.Int, 32),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimOfflineDurabilityModifier, true, GameStats.EnumType.Int, 32),
			new GameStats.PropertyDecl(EnumGameStats.LandClaimOfflineDelay, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.BedrollExpiryTime, true, GameStats.EnumType.Int, 45),
			new GameStats.PropertyDecl(EnumGameStats.AirDropFrequency, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.GlobalMessageToShow, false, GameStats.EnumType.String, ""),
			new GameStats.PropertyDecl(EnumGameStats.AirDropMarker, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.PartySharedKillRange, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.ChunkStabilityEnabled, false, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.AutoParty, true, GameStats.EnumType.Bool, false),
			new GameStats.PropertyDecl(EnumGameStats.OptionsPOICulling, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.BloodMoonDay, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.BlockDamagePlayer, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.XPMultiplier, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.BloodMoonWarning, true, GameStats.EnumType.Int, 8),
			new GameStats.PropertyDecl(EnumGameStats.AllowedViewDistance, false, GameStats.EnumType.Int, 12),
			new GameStats.PropertyDecl(EnumGameStats.TwitchBloodMoonAllowed, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.DeathPenalty, true, GameStats.EnumType.Int, EnumDeathPenalty.XPOnly),
			new GameStats.PropertyDecl(EnumGameStats.QuestProgressionDailyLimit, true, GameStats.EnumType.Int, 4),
			new GameStats.PropertyDecl(EnumGameStats.BiomeProgression, true, GameStats.EnumType.Bool, true),
			new GameStats.PropertyDecl(EnumGameStats.StormFreq, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.CameraRestrictionMode, true, GameStats.EnumType.Int, 0),
			new GameStats.PropertyDecl(EnumGameStats.JarRefund, true, GameStats.EnumType.Int, 60),
			new GameStats.PropertyDecl(EnumGameStats.SandboxPreset, true, GameStats.EnumType.String, ""),
			new GameStats.PropertyDecl(EnumGameStats.SandboxCode, true, GameStats.EnumType.String, ""),
			new GameStats.PropertyDecl(EnumGameStats.DayNightLength, true, GameStats.EnumType.Int, 60),
			new GameStats.PropertyDecl(EnumGameStats.BlockDamageAI, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.BlockDamageAIBM, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.LootAbundance, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.LootRespawnDays, true, GameStats.EnumType.Int, 7),
			new GameStats.PropertyDecl(EnumGameStats.GlobalGSModifier, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.BiomeGSModifier, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.GlobalLSModifier, true, GameStats.EnumType.Int, 100),
			new GameStats.PropertyDecl(EnumGameStats.BiomeLSModifier, true, GameStats.EnumType.Int, 100)
		};
	}

	// Token: 0x1700118C RID: 4492
	// (get) Token: 0x0600933D RID: 37693 RVA: 0x0037A77E File Offset: 0x0037897E
	public static GameStats Instance
	{
		get
		{
			if (GameStats.m_Instance == null)
			{
				GameStats.m_Instance = new GameStats();
				GameStats.m_Instance.initPropertyDecl();
				GameStats.m_Instance.initDefault();
			}
			return GameStats.m_Instance;
		}
	}

	// Token: 0x0600933E RID: 37694 RVA: 0x0037A7AC File Offset: 0x003789AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void initDefault()
	{
		foreach (GameStats.PropertyDecl propertyDecl in this.propertyList)
		{
			int name = (int)propertyDecl.name;
			this.propertyValues[name] = propertyDecl.defaultValue;
		}
	}

	// Token: 0x0600933F RID: 37695 RVA: 0x0037A7EC File Offset: 0x003789EC
	public void Write(BinaryWriter _write)
	{
		foreach (GameStats.PropertyDecl propertyDecl in this.propertyList)
		{
			if (propertyDecl.bPersistent)
			{
				switch (propertyDecl.type)
				{
				case GameStats.EnumType.Int:
					_write.Write(GameStats.GetInt(propertyDecl.name));
					break;
				case GameStats.EnumType.Float:
					_write.Write(GameStats.GetFloat(propertyDecl.name));
					break;
				case GameStats.EnumType.String:
					_write.Write(GameStats.GetString(propertyDecl.name));
					break;
				case GameStats.EnumType.Bool:
					_write.Write(GameStats.GetBool(propertyDecl.name));
					break;
				case GameStats.EnumType.Binary:
					_write.Write(Utils.ToBase64(GameStats.GetString(propertyDecl.name)));
					break;
				}
			}
		}
	}

	// Token: 0x06009340 RID: 37696 RVA: 0x0037A8B0 File Offset: 0x00378AB0
	public void Read(BinaryReader _reader)
	{
		foreach (GameStats.PropertyDecl propertyDecl in this.propertyList)
		{
			if (propertyDecl.bPersistent)
			{
				int name = (int)propertyDecl.name;
				switch (propertyDecl.type)
				{
				case GameStats.EnumType.Int:
					this.propertyValues[name] = _reader.ReadInt32();
					break;
				case GameStats.EnumType.Float:
					this.propertyValues[name] = _reader.ReadSingle();
					break;
				case GameStats.EnumType.String:
					this.propertyValues[name] = _reader.ReadString();
					break;
				case GameStats.EnumType.Bool:
					this.propertyValues[name] = _reader.ReadBoolean();
					break;
				case GameStats.EnumType.Binary:
					this.propertyValues[name] = Utils.FromBase64(_reader.ReadString());
					break;
				}
			}
		}
	}

	// Token: 0x06009341 RID: 37697 RVA: 0x0037A97C File Offset: 0x00378B7C
	public static object Parse(EnumGameStats _enum, string _val)
	{
		int num = GameStats.find(_enum);
		if (num == -1)
		{
			return null;
		}
		switch (GameStats.Instance.propertyList[num].type)
		{
		case GameStats.EnumType.Int:
			return int.Parse(_val);
		case GameStats.EnumType.Float:
			return StringParsers.ParseFloat(_val, 0, -1, NumberStyles.Any);
		case GameStats.EnumType.String:
			return _val;
		case GameStats.EnumType.Bool:
			return StringParsers.ParseBool(_val, 0, -1, true);
		case GameStats.EnumType.Binary:
			return _val;
		default:
			return null;
		}
	}

	// Token: 0x06009342 RID: 37698 RVA: 0x0037A9FC File Offset: 0x00378BFC
	public static string GetString(EnumGameStats _eProperty)
	{
		string result;
		try
		{
			result = (string)GameStats.Instance.propertyValues[(int)_eProperty];
		}
		catch (InvalidCastException)
		{
			Log.Error("GetString: InvalidCastException " + _eProperty.ToStringCached<EnumGameStats>());
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x06009343 RID: 37699 RVA: 0x0037AA4C File Offset: 0x00378C4C
	public static float GetFloat(EnumGameStats _eProperty)
	{
		float result;
		try
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				BaseSandboxOption baseSandboxOption = GameStats.Instance.sandboxReferences[(int)_eProperty];
				if (baseSandboxOption != null)
				{
					return baseSandboxOption.GetFloatValue();
				}
			}
			result = (float)GameStats.Instance.propertyValues[(int)_eProperty];
		}
		catch (InvalidCastException)
		{
			Log.Error("GetFloat: InvalidCastException " + _eProperty.ToStringCached<EnumGameStats>());
			result = 0f;
		}
		return result;
	}

	// Token: 0x06009344 RID: 37700 RVA: 0x0037AAC4 File Offset: 0x00378CC4
	public static int GetInt(EnumGameStats _eProperty)
	{
		int result;
		try
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				BaseSandboxOption baseSandboxOption = GameStats.Instance.sandboxReferences[(int)_eProperty];
				if (baseSandboxOption != null)
				{
					return baseSandboxOption.GetIntValue();
				}
			}
			result = (int)GameStats.Instance.propertyValues[(int)_eProperty];
		}
		catch (InvalidCastException)
		{
			Log.Error("GetInt: InvalidCastException " + _eProperty.ToStringCached<EnumGameStats>());
			result = 0;
		}
		return result;
	}

	// Token: 0x06009345 RID: 37701 RVA: 0x0037AB38 File Offset: 0x00378D38
	public static bool GetBool(EnumGameStats _eProperty)
	{
		bool result;
		try
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				BaseSandboxOption baseSandboxOption = GameStats.Instance.sandboxReferences[(int)_eProperty];
				if (baseSandboxOption != null)
				{
					return baseSandboxOption.GetBoolValue();
				}
			}
			result = (bool)GameStats.Instance.propertyValues[(int)_eProperty];
		}
		catch (InvalidCastException)
		{
			Log.Error("GetBool: InvalidCastException " + _eProperty.ToStringCached<EnumGameStats>());
			result = false;
		}
		return result;
	}

	// Token: 0x06009346 RID: 37702 RVA: 0x0037ABAC File Offset: 0x00378DAC
	public static void SetupSandboxReferences()
	{
		GameStats.Instance.sandboxReferences = new BaseSandboxOption[81];
		for (int i = 0; i < GameStats.Instance.propertyList.Length; i++)
		{
			GameStats.PropertyDecl propertyDecl = GameStats.Instance.propertyList[i];
			SandboxOptions optionType = SandboxOptions.Max;
			if (Enum.TryParse<SandboxOptions>(propertyDecl.name.ToString(), out optionType))
			{
				GameStats.Instance.sandboxReferences[(int)propertyDecl.name] = SandboxOptionManager.GetOption(optionType);
			}
			else
			{
				EnumGameStats name = propertyDecl.name;
				if (name != EnumGameStats.BlockDamagePlayer)
				{
					if (name == EnumGameStats.LootAbundance)
					{
						GameStats.Instance.sandboxReferences[(int)propertyDecl.name] = SandboxOptionManager.GetOption(SandboxOptions.GlobalLootCount);
					}
				}
				else
				{
					GameStats.Instance.sandboxReferences[(int)propertyDecl.name] = SandboxOptionManager.GetOption(SandboxOptions.BlockDamage);
				}
			}
		}
	}

	// Token: 0x06009347 RID: 37703 RVA: 0x0037AC76 File Offset: 0x00378E76
	public static object GetObject(EnumGameStats _eProperty)
	{
		return GameStats.Instance.propertyValues[(int)_eProperty];
	}

	// Token: 0x06009348 RID: 37704 RVA: 0x0037AC84 File Offset: 0x00378E84
	[PublicizedFrom(EAccessModifier.Private)]
	public static int find(EnumGameStats _eProperty)
	{
		for (int i = 0; i < GameStats.Instance.propertyList.Length; i++)
		{
			if (GameStats.Instance.propertyList[i].name == _eProperty)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06009349 RID: 37705 RVA: 0x0037ACC3 File Offset: 0x00378EC3
	public static void SetObject(EnumGameStats _eProperty, object _value)
	{
		GameStats.Instance.propertyValues[(int)_eProperty] = _value;
		if (GameStats.OnChangedDelegates != null)
		{
			GameStats.OnChangedDelegates(_eProperty, _value);
		}
	}

	// Token: 0x0600934A RID: 37706 RVA: 0x0037ACE5 File Offset: 0x00378EE5
	public static void Set(EnumGameStats _eProperty, int _value)
	{
		GameStats.SetObject(_eProperty, _value);
	}

	// Token: 0x0600934B RID: 37707 RVA: 0x0037ACF3 File Offset: 0x00378EF3
	public static void Set(EnumGameStats _eProperty, float _value)
	{
		GameStats.SetObject(_eProperty, _value);
	}

	// Token: 0x0600934C RID: 37708 RVA: 0x0037AD01 File Offset: 0x00378F01
	public static void Set(EnumGameStats _eProperty, string _value)
	{
		GameStats.SetObject(_eProperty, _value);
	}

	// Token: 0x0600934D RID: 37709 RVA: 0x0037AD0A File Offset: 0x00378F0A
	public static void Set(EnumGameStats _eProperty, bool _value)
	{
		GameStats.SetObject(_eProperty, _value);
	}

	// Token: 0x0600934E RID: 37710 RVA: 0x0037AD18 File Offset: 0x00378F18
	public static bool IsDefault(EnumGameStats _eProperty)
	{
		return GameStats.Instance.propertyValues[(int)_eProperty] != null && GameStats.Instance.propertyValues[(int)_eProperty].Equals(GameStats.Instance.propertyList[(int)_eProperty].defaultValue);
	}

	// Token: 0x0600934F RID: 37711 RVA: 0x0037AD50 File Offset: 0x00378F50
	public static GameStats.EnumType? GetStatType(EnumGameStats _eProperty)
	{
		foreach (GameStats.PropertyDecl propertyDecl in GameStats.Instance.propertyList)
		{
			if (propertyDecl.name == _eProperty)
			{
				return new GameStats.EnumType?(propertyDecl.type);
			}
		}
		return null;
	}

	// Token: 0x04006E66 RID: 28262
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStats.PropertyDecl[] propertyList;

	// Token: 0x04006E67 RID: 28263
	[PublicizedFrom(EAccessModifier.Private)]
	public object[] propertyValues = new object[81];

	// Token: 0x04006E68 RID: 28264
	[PublicizedFrom(EAccessModifier.Private)]
	public BaseSandboxOption[] sandboxReferences;

	// Token: 0x04006E69 RID: 28265
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameStats m_Instance;

	// Token: 0x02001204 RID: 4612
	// (Invoke) Token: 0x06009352 RID: 37714
	public delegate void OnChangedDelegate(EnumGameStats _gameState, object _newValue);

	// Token: 0x02001205 RID: 4613
	public enum EnumType
	{
		// Token: 0x04006E6B RID: 28267
		Int,
		// Token: 0x04006E6C RID: 28268
		Float,
		// Token: 0x04006E6D RID: 28269
		String,
		// Token: 0x04006E6E RID: 28270
		Bool,
		// Token: 0x04006E6F RID: 28271
		Binary
	}

	// Token: 0x02001206 RID: 4614
	[PublicizedFrom(EAccessModifier.Private)]
	public struct PropertyDecl
	{
		// Token: 0x06009355 RID: 37717 RVA: 0x0037ADB1 File Offset: 0x00378FB1
		public PropertyDecl(EnumGameStats _name, bool _bPersistent, GameStats.EnumType _type, object _defaultValue)
		{
			this.name = _name;
			this.type = _type;
			this.defaultValue = _defaultValue;
			this.bPersistent = _bPersistent;
		}

		// Token: 0x04006E70 RID: 28272
		public EnumGameStats name;

		// Token: 0x04006E71 RID: 28273
		public GameStats.EnumType type;

		// Token: 0x04006E72 RID: 28274
		public object defaultValue;

		// Token: 0x04006E73 RID: 28275
		public bool bPersistent;
	}
}
