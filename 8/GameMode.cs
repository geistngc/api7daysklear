using System;
using System.Collections.Generic;
using System.Reflection;
using Platform;

// Token: 0x020011CA RID: 4554
public abstract class GameMode
{
	// Token: 0x060091B1 RID: 37297
	public abstract GameMode.ModeGamePref[] GetSupportedGamePrefsInfo();

	// Token: 0x060091B2 RID: 37298 RVA: 0x0036F38C File Offset: 0x0036D58C
	public Dictionary<EnumGamePrefs, GameMode.ModeGamePref> GetGamePrefs()
	{
		if (this.gamePrefs == null)
		{
			this.gamePrefs = new EnumDictionary<EnumGamePrefs, GameMode.ModeGamePref>();
			GameMode.ModeGamePref[] supportedGamePrefsInfo = this.GetSupportedGamePrefsInfo();
			int length = supportedGamePrefsInfo.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				EnumGamePrefs gamePref = supportedGamePrefsInfo[i].GamePref;
				GamePrefs.EnumType valueType = supportedGamePrefsInfo[i].ValueType;
				object defaultValue = supportedGamePrefsInfo[i].DefaultValue;
				this.gamePrefs.Add(gamePref, new GameMode.ModeGamePref(gamePref, valueType, defaultValue, null));
			}
		}
		return this.gamePrefs;
	}

	// Token: 0x060091B3 RID: 37299
	public abstract void ResetGamePrefs();

	// Token: 0x060091B4 RID: 37300
	public abstract string GetDescription();

	// Token: 0x060091B5 RID: 37301
	public abstract int GetID();

	// Token: 0x060091B6 RID: 37302
	public abstract void Init();

	// Token: 0x060091B7 RID: 37303
	public abstract string GetName();

	// Token: 0x060091B8 RID: 37304 RVA: 0x0036F40D File Offset: 0x0036D60D
	public string GetTypeName()
	{
		if (this.cachedTypeName == null)
		{
			this.cachedTypeName = base.GetType().Name;
		}
		return this.cachedTypeName;
	}

	// Token: 0x060091B9 RID: 37305
	public abstract int GetRoundCount();

	// Token: 0x060091BA RID: 37306
	public abstract void StartRound(int _idx);

	// Token: 0x060091BB RID: 37307
	public abstract void EndRound(int _idx);

	// Token: 0x060091BC RID: 37308 RVA: 0x0004E558 File Offset: 0x0004C758
	public virtual string GetAdditionalGameInfo(World _world)
	{
		return string.Empty;
	}

	// Token: 0x060091BD RID: 37309 RVA: 0x0036F430 File Offset: 0x0036D630
	[PublicizedFrom(EAccessModifier.Private)]
	public static void InitGameModeDict()
	{
		if (GameMode.gameModes == null)
		{
			GameMode.gameModes = new Dictionary<int, GameMode>();
			Type typeFromHandle = typeof(GameMode);
			foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
			{
				if (!type.IsAbstract && typeFromHandle.IsAssignableFrom(type))
				{
					GameMode gameMode = Activator.CreateInstance(type) as GameMode;
					GameMode.gameModes.Add(gameMode.GetID(), gameMode);
				}
			}
		}
	}

	// Token: 0x060091BE RID: 37310 RVA: 0x0036F4A7 File Offset: 0x0036D6A7
	public static GameMode GetGameModeForId(int _id)
	{
		GameMode.InitGameModeDict();
		if (GameMode.gameModes.ContainsKey(_id))
		{
			return GameMode.gameModes[_id];
		}
		return null;
	}

	// Token: 0x060091BF RID: 37311 RVA: 0x0036F4C8 File Offset: 0x0036D6C8
	public static GameMode GetGameModeForName(string _name)
	{
		GameMode.InitGameModeDict();
		foreach (KeyValuePair<int, GameMode> keyValuePair in GameMode.gameModes)
		{
			if (keyValuePair.Value.GetTypeName().EqualsCaseInsensitive(_name))
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	// Token: 0x060091C0 RID: 37312 RVA: 0x0036F53C File Offset: 0x0036D73C
	public override string ToString()
	{
		if (this.localizedName == null)
		{
			this.localizedName = Localization.Get(this.GetName(), false, null);
		}
		return this.localizedName;
	}

	// Token: 0x060091C1 RID: 37313 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameMode()
	{
	}

	// Token: 0x04006BE7 RID: 27623
	public static GameMode[] AvailGameModes = new GameMode[]
	{
		new GameModeSurvival()
	};

	// Token: 0x04006BE8 RID: 27624
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<EnumGamePrefs, GameMode.ModeGamePref> gamePrefs;

	// Token: 0x04006BE9 RID: 27625
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedTypeName;

	// Token: 0x04006BEA RID: 27626
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName;

	// Token: 0x04006BEB RID: 27627
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, GameMode> gameModes;

	// Token: 0x020011CB RID: 4555
	public struct ModeGamePref
	{
		// Token: 0x060091C3 RID: 37315 RVA: 0x0036F574 File Offset: 0x0036D774
		public ModeGamePref(EnumGamePrefs _gamePref, GamePrefs.EnumType _valueType, object _defaultValue, Dictionary<DeviceFlag, object> _deviceDefaults = null)
		{
			this.GamePref = _gamePref;
			this.ValueType = _valueType;
			if (_deviceDefaults != null && _deviceDefaults.ContainsKey(DeviceFlag.StandaloneWindows))
			{
				this.DefaultValue = _deviceDefaults[DeviceFlag.StandaloneWindows];
				return;
			}
			this.DefaultValue = _defaultValue;
		}

		// Token: 0x04006BEC RID: 27628
		public EnumGamePrefs GamePref;

		// Token: 0x04006BED RID: 27629
		public GamePrefs.EnumType ValueType;

		// Token: 0x04006BEE RID: 27630
		public object DefaultValue;
	}
}
