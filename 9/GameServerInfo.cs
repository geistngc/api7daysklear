using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Platform;
using SandboxOptions;
using UnityEngine;

// Token: 0x02000778 RID: 1912
public class GameServerInfo
{
	// Token: 0x14000044 RID: 68
	// (add) Token: 0x060038EC RID: 14572 RVA: 0x001769A4 File Offset: 0x00174BA4
	// (remove) Token: 0x060038ED RID: 14573 RVA: 0x001769DC File Offset: 0x00174BDC
	public event Action<GameServerInfo> OnChangedAny;

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x060038EE RID: 14574 RVA: 0x00176A14 File Offset: 0x00174C14
	// (remove) Token: 0x060038EF RID: 14575 RVA: 0x00176A4C File Offset: 0x00174C4C
	public event Action<GameServerInfo, GameInfoString> OnChangedString;

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x060038F0 RID: 14576 RVA: 0x00176A84 File Offset: 0x00174C84
	// (remove) Token: 0x060038F1 RID: 14577 RVA: 0x00176ABC File Offset: 0x00174CBC
	public event Action<GameServerInfo, GameInfoInt> OnChangedInt;

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x060038F2 RID: 14578 RVA: 0x00176AF4 File Offset: 0x00174CF4
	// (remove) Token: 0x060038F3 RID: 14579 RVA: 0x00176B2C File Offset: 0x00174D2C
	public event Action<GameServerInfo, GameInfoBool> OnChangedBool;

	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x060038F4 RID: 14580 RVA: 0x00176B61 File Offset: 0x00174D61
	public bool IsValid
	{
		get
		{
			return !this.isBroken;
		}
	}

	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x060038F5 RID: 14581 RVA: 0x00176B6C File Offset: 0x00174D6C
	public bool IsDedicated
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated);
		}
	}

	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x060038F6 RID: 14582 RVA: 0x00176B75 File Offset: 0x00174D75
	public bool IsDedicatedStock
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated) && this.GetValue(GameInfoBool.StockSettings) && !this.GetValue(GameInfoBool.ModdedConfig);
		}
	}

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x060038F7 RID: 14583 RVA: 0x00176B96 File Offset: 0x00174D96
	public bool IsDedicatedModded
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated) && this.GetValue(GameInfoBool.ModdedConfig);
		}
	}

	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x060038F8 RID: 14584 RVA: 0x00176BAB File Offset: 0x00174DAB
	public bool IsPeerToPeer
	{
		get
		{
			return !this.GetValue(GameInfoBool.IsDedicated) && !this.isNoResponse;
		}
	}

	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x060038F9 RID: 14585 RVA: 0x00176BC1 File Offset: 0x00174DC1
	public bool AllowsCrossplay
	{
		get
		{
			return this.GetValue(GameInfoBool.AllowCrossplay);
		}
	}

	// Token: 0x170005A6 RID: 1446
	// (get) Token: 0x060038FA RID: 14586 RVA: 0x00176BCB File Offset: 0x00174DCB
	public bool EACEnabled
	{
		get
		{
			return this.GetValue(GameInfoBool.EACEnabled);
		}
	}

	// Token: 0x170005A7 RID: 1447
	// (get) Token: 0x060038FB RID: 14587 RVA: 0x00176BD4 File Offset: 0x00174DD4
	public bool IgnoresSanctions
	{
		get
		{
			return this.GetValue(GameInfoBool.SanctionsIgnored);
		}
	}

	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x060038FC RID: 14588 RVA: 0x00176BE0 File Offset: 0x00174DE0
	public EPlayGroup PlayGroup
	{
		get
		{
			EPlayGroup result;
			if (!EnumUtils.TryParse<EPlayGroup>(this.GetValue(GameInfoString.PlayGroup), out result, false))
			{
				return EPlayGroup.Unknown;
			}
			return result;
		}
	}

	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x060038FD RID: 14589 RVA: 0x00176C02 File Offset: 0x00174E02
	// (set) Token: 0x060038FE RID: 14590 RVA: 0x00176C0A File Offset: 0x00174E0A
	public bool IsFriends
	{
		get
		{
			return this.isFriends;
		}
		set
		{
			if (value != this.isFriends)
			{
				this.isFriends = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x060038FF RID: 14591 RVA: 0x00176C2D File Offset: 0x00174E2D
	public bool IsFavoriteHistory
	{
		get
		{
			return this.IsFavorite || this.IsHistory;
		}
	}

	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x06003900 RID: 14592 RVA: 0x00176C3F File Offset: 0x00174E3F
	// (set) Token: 0x06003901 RID: 14593 RVA: 0x00176C47 File Offset: 0x00174E47
	public bool IsFavorite
	{
		get
		{
			return this.isFavorite;
		}
		set
		{
			if (value != this.isFavorite)
			{
				this.isFavorite = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x06003902 RID: 14594 RVA: 0x00176C6A File Offset: 0x00174E6A
	public bool IsHistory
	{
		get
		{
			return this.lastPlayed > 0;
		}
	}

	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x06003903 RID: 14595 RVA: 0x00176C75 File Offset: 0x00174E75
	// (set) Token: 0x06003904 RID: 14596 RVA: 0x00176C7D File Offset: 0x00174E7D
	public int LastPlayedLinux
	{
		get
		{
			return this.lastPlayed;
		}
		set
		{
			if (value != this.lastPlayed)
			{
				this.lastPlayed = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005AE RID: 1454
	// (get) Token: 0x06003905 RID: 14597 RVA: 0x00176CA0 File Offset: 0x00174EA0
	// (set) Token: 0x06003906 RID: 14598 RVA: 0x00176CA8 File Offset: 0x00174EA8
	public bool IsLAN
	{
		get
		{
			return this.isLan;
		}
		set
		{
			if (value != this.isLan)
			{
				this.isLan = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x06003907 RID: 14599 RVA: 0x00176CCB File Offset: 0x00174ECB
	// (set) Token: 0x06003908 RID: 14600 RVA: 0x00176CD3 File Offset: 0x00174ED3
	public bool IsLobby
	{
		get
		{
			return this.isLobby;
		}
		set
		{
			if (value != this.isLobby)
			{
				this.isLobby = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x06003909 RID: 14601 RVA: 0x00176CF6 File Offset: 0x00174EF6
	// (set) Token: 0x0600390A RID: 14602 RVA: 0x00176CFE File Offset: 0x00174EFE
	public bool IsNoResponse
	{
		get
		{
			return this.isNoResponse;
		}
		set
		{
			if (value != this.isNoResponse)
			{
				this.isNoResponse = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x0600390B RID: 14603 RVA: 0x00176D21 File Offset: 0x00174F21
	public VersionInformation Version
	{
		get
		{
			return this.version;
		}
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x0600390C RID: 14604 RVA: 0x00176D29 File Offset: 0x00174F29
	public bool IsCompatibleVersion
	{
		get
		{
			return this.version.Major < 0 || this.version.EqualsMinor(Constants.cVersionInformation);
		}
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x00176D4C File Offset: 0x00174F4C
	public GameServerInfo()
	{
		this.OnChangedAny += this.RefreshServerDisplayTexts;
		this.Strings = new ReadOnlyDictionary<GameInfoString, string>(this.tableStrings);
		this.Ints = new ReadOnlyDictionary<GameInfoInt, int>(this.tableInts);
		this.Bools = new ReadOnlyDictionary<GameInfoBool, bool>(this.tableBools);
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x00176E1C File Offset: 0x0017501C
	public GameServerInfo(GameServerInfo _gsi)
	{
		foreach (KeyValuePair<GameInfoString, string> keyValuePair in _gsi.tableStrings)
		{
			this.SetValue(keyValuePair.Key, keyValuePair.Value);
		}
		foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in _gsi.tableInts)
		{
			this.SetValue(keyValuePair2.Key, keyValuePair2.Value);
		}
		foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in _gsi.tableBools)
		{
			this.SetValue(keyValuePair3.Key, keyValuePair3.Value);
		}
		this.isBroken = _gsi.isBroken;
		this.isFriends = _gsi.isFriends;
		this.isFavorite = _gsi.isFavorite;
		this.lastPlayed = _gsi.lastPlayed;
		this.isLan = _gsi.isLan;
		this.isLobby = _gsi.isLobby;
		this.isNoResponse = _gsi.isNoResponse;
		this.RefreshServerDisplayTexts(this);
		this.OnChangedAny += this.RefreshServerDisplayTexts;
	}

	// Token: 0x0600390F RID: 14607 RVA: 0x00177008 File Offset: 0x00175208
	public GameServerInfo(string _serverInfoString)
	{
		if (_serverInfoString.Length == 0)
		{
			this.isBroken = true;
			return;
		}
		this.BuildInfoFromString(_serverInfoString);
		this.OnChangedAny += this.RefreshServerDisplayTexts;
	}

	// Token: 0x06003910 RID: 14608 RVA: 0x001770BC File Offset: 0x001752BC
	public void Merge(GameServerInfo _gameServerInfo, EServerRelationType _source)
	{
		this.isFriends |= _gameServerInfo.IsFriends;
		this.isFavorite |= _gameServerInfo.IsFavorite;
		this.isLan |= _gameServerInfo.IsLAN;
		if (_source == EServerRelationType.History)
		{
			this.lastPlayed = _gameServerInfo.LastPlayedLinux;
		}
		foreach (KeyValuePair<GameInfoBool, bool> keyValuePair in _gameServerInfo.tableBools)
		{
			if (keyValuePair.Value)
			{
				this.SetValue(keyValuePair.Key, keyValuePair.Value);
			}
		}
		foreach (KeyValuePair<GameInfoString, string> keyValuePair2 in _gameServerInfo.tableStrings)
		{
			if (_source == EServerRelationType.LAN || keyValuePair2.Key != GameInfoString.IP)
			{
				this.SetValue(keyValuePair2.Key, keyValuePair2.Value);
			}
		}
		foreach (KeyValuePair<GameInfoInt, int> keyValuePair3 in _gameServerInfo.tableInts)
		{
			this.SetValue(keyValuePair3.Key, keyValuePair3.Value);
		}
	}

	// Token: 0x06003911 RID: 14609 RVA: 0x0017721C File Offset: 0x0017541C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildInfoFromString(string _serverInfoString)
	{
		if (_serverInfoString.Length == 0)
		{
			this.isBroken = true;
			return;
		}
		foreach (string text in _serverInfoString.Substring(0, _serverInfoString.Length - 1).Split(';', StringSplitOptions.None))
		{
			if (text.Length >= 2)
			{
				string[] array2 = text.Split(':', StringSplitOptions.None);
				if (array2.Length >= 2)
				{
					string key = array2[0].Trim(GameServerInfo.whiteSpaceChars);
					string value = array2[1];
					this.ParseAny(key, value);
				}
			}
		}
		this.RefreshServerDisplayTexts(this);
	}

	// Token: 0x06003912 RID: 14610 RVA: 0x001772A4 File Offset: 0x001754A4
	public bool ParseAny(string _key, string _value)
	{
		bool result;
		try
		{
			GameInfoString key;
			GameInfoInt key2;
			GameInfoBool key3;
			if (EnumUtils.TryParse<GameInfoString>(_key, out key, true))
			{
				this.SetValue(key, _value);
			}
			else if (EnumUtils.TryParse<GameInfoInt>(_key, out key2, true))
			{
				this.SetValue(key2, Convert.ToInt32(_value));
			}
			else if (EnumUtils.TryParse<GameInfoBool>(_key, out key3, true))
			{
				this.SetValue(key3, StringParsers.ParseBool(_value, 0, -1, true));
			}
			result = true;
		}
		catch (Exception)
		{
			string text = this.GetValue(GameInfoString.IP);
			if (string.IsNullOrEmpty(text))
			{
				text = "<unknown>";
			}
			int value = this.GetValue(GameInfoInt.Port);
			Log.Warning("GameServer {0}:{1} replied with invalid setting: {2}={3}", new object[]
			{
				text,
				value,
				_key,
				_value
			});
			this.isBroken = true;
			result = false;
		}
		return result;
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x00177368 File Offset: 0x00175568
	public bool Parse(string _key, string _value)
	{
		GameInfoString key;
		if (EnumUtils.TryParse<GameInfoString>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x00177390 File Offset: 0x00175590
	public bool Parse(string _key, int _value)
	{
		GameInfoInt key;
		if (EnumUtils.TryParse<GameInfoInt>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x001773B8 File Offset: 0x001755B8
	public bool Parse(string _key, bool _value)
	{
		GameInfoBool key;
		if (EnumUtils.TryParse<GameInfoBool>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x001773E0 File Offset: 0x001755E0
	public void SetValue(GameInfoString _key, string _value)
	{
		if (_value == null)
		{
			_value = "";
		}
		this.tableStrings[_key] = _value.Replace(':', '^').Replace(';', '*');
		if (_key == GameInfoString.IP || _key == GameInfoString.SteamID || _key == GameInfoString.UniqueId)
		{
			this.hashcode = long.MinValue;
		}
		if (_key == GameInfoString.ServerVersion)
		{
			VersionInformation versionInformation;
			if (VersionInformation.TryParseSerializedString(_value, out versionInformation))
			{
				this.version = versionInformation;
			}
			else
			{
				Log.Warning("Server browser: Could not parse version from received data (from entry: " + this.GetValue(GameInfoString.IP) + "): " + _value);
			}
		}
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoString> onChangedString = this.OnChangedString;
		if (onChangedString != null)
		{
			onChangedString(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00177498 File Offset: 0x00175698
	public string GetValue(GameInfoString _key)
	{
		string text;
		if (!this.tableStrings.TryGetValue(_key, out text))
		{
			return "";
		}
		return text.Replace('^', ':').Replace('*', ';');
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x001774D0 File Offset: 0x001756D0
	public void SetValue(GameInfoInt _key, int _value)
	{
		if (_key != GameInfoInt.Ping || !this.tableInts.ContainsKey(GameInfoInt.Ping) || _value >= 0)
		{
			this.tableInts[_key] = _value;
		}
		if (_key == GameInfoInt.Port)
		{
			this.hashcode = long.MinValue;
		}
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoInt> onChangedInt = this.OnChangedInt;
		if (onChangedInt != null)
		{
			onChangedInt(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x00177548 File Offset: 0x00175748
	public int GetValue(GameInfoInt _key)
	{
		int result;
		if (!this.tableInts.TryGetValue(_key, out result))
		{
			return -1;
		}
		return result;
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00177568 File Offset: 0x00175768
	public void SetValue(GameInfoBool _key, bool _value)
	{
		this.tableBools[_key] = _value;
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoBool> onChangedBool = this.OnChangedBool;
		if (onChangedBool != null)
		{
			onChangedBool(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x001775B4 File Offset: 0x001757B4
	public bool GetValue(GameInfoBool _key)
	{
		bool flag;
		return this.tableBools.TryGetValue(_key, out flag) && flag;
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x001775D1 File Offset: 0x001757D1
	public override string ToString()
	{
		return this.ToString(false);
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x001775DC File Offset: 0x001757DC
	public string ToString(bool _lineBreaks)
	{
		if ((!_lineBreaks && this.cachedToString == null) || (_lineBreaks && this.cachedToStringLineBreaks == null))
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<GameInfoString, string> keyValuePair in this.tableStrings)
			{
				stringBuilder.Append(keyValuePair.Key.ToStringCached<GameInfoString>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in this.tableInts)
			{
				stringBuilder.Append(keyValuePair2.Key.ToStringCached<GameInfoInt>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair2.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in this.tableBools)
			{
				stringBuilder.Append(keyValuePair3.Key.ToStringCached<GameInfoBool>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair3.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			stringBuilder.Append('\r');
			stringBuilder.Append('\n');
			if (_lineBreaks)
			{
				this.cachedToStringLineBreaks = stringBuilder.ToString();
			}
			else
			{
				this.cachedToString = stringBuilder.ToString();
			}
		}
		if (!_lineBreaks)
		{
			return this.cachedToString;
		}
		return this.cachedToStringLineBreaks;
	}

	// Token: 0x0600391E RID: 14622 RVA: 0x001777DC File Offset: 0x001759DC
	public override int GetHashCode()
	{
		if (this.hashcode == -9223372036854775808L)
		{
			string text = this.GetValue(GameInfoString.IP) + this.GetValue(GameInfoInt.Port).ToString();
			this.hashcode = (long)text.GetHashCode();
		}
		return (int)this.hashcode;
	}

	// Token: 0x0600391F RID: 14623 RVA: 0x0017782C File Offset: 0x00175A2C
	public override bool Equals(object _obj)
	{
		if (_obj == null)
		{
			return false;
		}
		GameServerInfo p = _obj as GameServerInfo;
		return this.Equals(p);
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x0017784C File Offset: 0x00175A4C
	public bool Equals(GameServerInfo _p)
	{
		return _p != null && this.GetValue(GameInfoString.IP) == _p.GetValue(GameInfoString.IP) && this.GetValue(GameInfoInt.Port) == _p.GetValue(GameInfoInt.Port);
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x0017787C File Offset: 0x00175A7C
	public void UpdateGameTimePlayers(ulong _time, int _players)
	{
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime - this.timeLastWorldTimeUpdate > 15f || this.GetValue(GameInfoInt.CurrentPlayers) != _players)
		{
			this.timeLastWorldTimeUpdate = unscaledTime;
			if (PrefabEditModeManager.Instance.IsActive())
			{
				this.SetValue(GameInfoString.LevelName, PrefabEditModeManager.Instance.LoadedPrefab.Name);
			}
			this.SetValue(GameInfoInt.CurrentServerTime, (int)_time);
			this.SetValue(GameInfoInt.CurrentPlayers, _players);
			this.SetValue(GameInfoInt.FreePlayerSlots, this.GetValue(GameInfoInt.MaxPlayers) - _players);
			Action<GameServerInfo> onChangedAny = this.OnChangedAny;
			if (onChangedAny == null)
			{
				return;
			}
			onChangedAny(this);
		}
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x00177904 File Offset: 0x00175B04
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshServerDisplayTexts(GameServerInfo gsi)
	{
		string value = gsi.GetValue(GameInfoString.CombinedPrimaryId);
		PlatformUserIdentifierAbs author = null;
		if (!string.IsNullOrEmpty(value))
		{
			author = PlatformUserIdentifierAbs.FromCombinedString(value, false);
		}
		string value2 = gsi.GetValue(GameInfoString.GameHost);
		string value3 = gsi.GetValue(GameInfoString.LevelName);
		string value4 = gsi.GetValue(GameInfoString.ServerDescription);
		string value5 = gsi.GetValue(GameInfoString.ServerWebsiteURL);
		string value6 = gsi.GetValue(GameInfoString.ServerLoginConfirmationText);
		if (!string.IsNullOrEmpty(value2) && string.IsNullOrEmpty(this.ServerDisplayName.Text))
		{
			this.ServerDisplayName.Update(value2, author);
		}
		if (!string.IsNullOrEmpty(value3) && (string.IsNullOrEmpty(this.ServerWorldName.Text) || this.ServerWorldName.Text != value3))
		{
			this.ServerWorldName.Update(value3, author);
		}
		if (!string.IsNullOrEmpty(value4) && string.IsNullOrEmpty(this.ServerDescription.Text))
		{
			this.ServerDescription.Update(value4.Replace("\\n", "\n"), author);
		}
		if (!string.IsNullOrEmpty(value5) && string.IsNullOrEmpty(this.ServerURL.Text))
		{
			this.ServerURL.Update(value5, author);
		}
		if (!string.IsNullOrEmpty(value6) && string.IsNullOrEmpty(this.ServerLoginConfirmationText.Text))
		{
			this.ServerLoginConfirmationText.Update(value6, author);
		}
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x00177A43 File Offset: 0x00175C43
	public void ClearOnChanged()
	{
		this.OnChangedAny = null;
		this.OnChangedString = null;
		this.OnChangedInt = null;
		this.OnChangedBool = null;
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x00177A61 File Offset: 0x00175C61
	public static bool IsSearchable(GameInfoString _gameInfoKey)
	{
		return GameServerInfo.SearchableStringInfosSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x00177A6E File Offset: 0x00175C6E
	public static bool IsSearchable(GameInfoInt _gameInfoKey)
	{
		return GameServerInfo.IntInfosInGameTagsSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x00177A7B File Offset: 0x00175C7B
	public static bool IsSearchable(GameInfoBool _gameInfoKey)
	{
		return GameServerInfo.BoolInfosInGameTagsSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003927 RID: 14631 RVA: 0x00177A88 File Offset: 0x00175C88
	public static GameServerInfo BuildGameServerInfo()
	{
		GameServerInfo gameServerInfo = new GameServerInfo();
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.ServerEnabled);
		SandboxOptionManager sandboxOptionManager = SandboxOptionManager.Current;
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay))
		{
			bool flag = true;
			if (8 < GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount))
			{
				Log.Warning(string.Format("CROSSPLAY INCOMPATIBLE VALUE: PLAYER COUNT GREATER THAN MAX OF {0}", 8));
				flag = false;
			}
			if (GamePrefs.GetBool(EnumGamePrefs.IgnoreEOSSanctions))
			{
				Log.Warning("CROSSPLAY INCOMPATIBLE VALUE: EOS SANCTIONS IGNORED");
				flag = false;
			}
			if (!flag)
			{
				Log.Warning("CROSSPLAY DISABLED FOR SESSION, CORRECT VALUES TO BE CROSSPLAY COMPATIBLE");
				GamePrefs.Set(EnumGamePrefs.ServerAllowCrossplay, false);
			}
		}
		gameServerInfo.SetValue(GameInfoString.GameType, "7DTD");
		gameServerInfo.SetValue(GameInfoString.GameName, GamePrefs.GetString(EnumGamePrefs.GameName));
		gameServerInfo.SetValue(GameInfoString.GameMode, GamePrefs.GetString(EnumGamePrefs.GameMode).Replace("GameMode", ""));
		gameServerInfo.SetValue(GameInfoString.GameHost, GameManager.IsDedicatedServer ? GamePrefs.GetString(EnumGamePrefs.ServerName) : GamePrefs.GetString(EnumGamePrefs.PlayerName));
		GameInfoString key = GameInfoString.LevelName;
		PrefabEditModeManager instance = PrefabEditModeManager.Instance;
		gameServerInfo.SetValue(key, (instance != null && instance.IsActive()) ? PrefabEditModeManager.Instance.LoadedPrefab.Name : GamePrefs.GetString(EnumGamePrefs.GameWorld));
		gameServerInfo.SetValue(GameInfoString.ServerDescription, GamePrefs.GetString(EnumGamePrefs.ServerDescription));
		gameServerInfo.SetValue(GameInfoString.ServerWebsiteURL, GamePrefs.GetString(EnumGamePrefs.ServerWebsiteURL));
		gameServerInfo.SetValue(GameInfoString.ServerLoginConfirmationText, GamePrefs.GetString(EnumGamePrefs.ServerLoginConfirmationText));
		gameServerInfo.SetValue(GameInfoBool.IsDedicated, GameManager.IsDedicatedServer);
		gameServerInfo.SetValue(GameInfoBool.IsPasswordProtected, !string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerPassword)));
		bool value = GameManager.IsDedicatedServer ? GamePrefs.GetBool(EnumGamePrefs.EACEnabled) : GamePrefs.GetBool(EnumGamePrefs.ServerEACPeerToPeer);
		gameServerInfo.SetValue(GameInfoBool.EACEnabled, value);
		gameServerInfo.SetValue(GameInfoBool.SanctionsIgnored, GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.IgnoreEOSSanctions));
		gameServerInfo.SetValue(GameInfoBool.AllowCrossplay, @bool && GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay) && PermissionsManager.IsCrossplayAllowed());
		gameServerInfo.SetValue(GameInfoString.PlayGroup, EPlayGroupExtensions.Current.ToStringCached<EPlayGroup>());
		gameServerInfo.SetValue(GameInfoInt.MaxPlayers, GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount));
		gameServerInfo.SetValue(GameInfoInt.FreePlayerSlots, GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount) - (GameManager.IsDedicatedServer ? 0 : 1));
		gameServerInfo.SetValue(GameInfoInt.CurrentPlayers, GameManager.IsDedicatedServer ? 0 : 1);
		gameServerInfo.SetValue(GameInfoInt.Port, GamePrefs.GetInt(EnumGamePrefs.ServerPort));
		gameServerInfo.SetValue(GameInfoString.ServerVersion, Constants.cVersionInformation.SerializableString);
		gameServerInfo.SetValue(GameInfoBool.Architecture64, !Constants.Is32BitOs);
		gameServerInfo.SetValue(GameInfoString.Platform, Application.platform.ToString());
		gameServerInfo.SetValue(GameInfoBool.IsPublic, GamePrefs.GetBool(EnumGamePrefs.ServerIsPublic));
		gameServerInfo.SetValue(GameInfoInt.ServerVisibility, PermissionsManager.IsMultiplayerAllowed() ? GamePrefs.GetInt(EnumGamePrefs.ServerVisibility) : 0);
		SandboxOptionPreset preset = SandboxOptionManager.Current.GetPreset(GamePrefs.GetString(EnumGamePrefs.SandboxPreset));
		gameServerInfo.SetValue(GameInfoBool.StockSettings, preset != null && !preset.IsCustomPreset && !preset.IsModded && !preset.IsUserPreset);
		bool flag2 = StockFileHashes.HasStockXMLs();
		gameServerInfo.SetValue(GameInfoBool.StockFiles, flag2);
		gameServerInfo.SetValue(GameInfoBool.ModdedConfig, !flag2 || ModManager.AnyConfigModActive());
		gameServerInfo.SetValue(GameInfoString.Region, GamePrefs.GetString(EnumGamePrefs.Region));
		gameServerInfo.SetValue(GameInfoString.Language, GameManager.IsDedicatedServer ? GamePrefs.GetString(EnumGamePrefs.Language) : Localization.ActiveLanguage);
		gameServerInfo.SetValue(GameInfoString.SandboxPreset, GamePrefs.GetString(EnumGamePrefs.SandboxPreset));
		gameServerInfo.SetValue(GameInfoString.SandboxCode, GamePrefs.GetString(EnumGamePrefs.SandboxCode));
		gameServerInfo.SetValue(GameInfoInt.BlockDamagePlayer, GamePrefs.GetInt(EnumGamePrefs.BlockDamagePlayer));
		gameServerInfo.SetValue(GameInfoInt.BlockDamageAI, GamePrefs.GetInt(EnumGamePrefs.BlockDamageAI));
		gameServerInfo.SetValue(GameInfoInt.BlockDamageAIBM, GamePrefs.GetInt(EnumGamePrefs.BlockDamageAIBM));
		gameServerInfo.SetValue(GameInfoInt.XPMultiplier, GamePrefs.GetInt(EnumGamePrefs.XPMultiplier));
		gameServerInfo.SetValue(GameInfoBool.BuildCreate, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		gameServerInfo.SetValue(GameInfoInt.DayNightLength, GamePrefs.GetInt(EnumGamePrefs.DayNightLength));
		gameServerInfo.SetValue(GameInfoInt.DayLightLength, GamePrefs.GetInt(EnumGamePrefs.DayLightLength));
		gameServerInfo.SetValue(GameInfoInt.DeathPenalty, GamePrefs.GetInt(EnumGamePrefs.DeathPenalty));
		gameServerInfo.SetValue(GameInfoInt.DropOnDeath, GamePrefs.GetInt(EnumGamePrefs.DropOnDeath));
		gameServerInfo.SetValue(GameInfoInt.DropOnQuit, GamePrefs.GetInt(EnumGamePrefs.DropOnQuit));
		gameServerInfo.SetValue(GameInfoInt.BedrollDeadZoneSize, GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize));
		gameServerInfo.SetValue(GameInfoInt.BedrollExpiryTime, GamePrefs.GetInt(EnumGamePrefs.BedrollExpiryTime));
		gameServerInfo.SetValue(GameInfoInt.MaxSpawnedZombies, GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies));
		gameServerInfo.SetValue(GameInfoInt.MaxSpawnedAnimals, GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedAnimals));
		gameServerInfo.SetValue(GameInfoBool.EnemySpawnMode, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		gameServerInfo.SetValue(GameInfoInt.EnemyDifficulty, GamePrefs.GetInt(EnumGamePrefs.EnemyDifficulty));
		gameServerInfo.SetValue(GameInfoInt.ZombieFeralSense, GamePrefs.GetInt(EnumGamePrefs.ZombieFeralSense));
		gameServerInfo.SetValue(GameInfoInt.ZombieMove, GamePrefs.GetInt(EnumGamePrefs.ZombieMove));
		gameServerInfo.SetValue(GameInfoInt.ZombieMoveNight, GamePrefs.GetInt(EnumGamePrefs.ZombieMoveNight));
		gameServerInfo.SetValue(GameInfoInt.ZombieFeralMove, GamePrefs.GetInt(EnumGamePrefs.ZombieFeralMove));
		gameServerInfo.SetValue(GameInfoInt.ZombieBMMove, GamePrefs.GetInt(EnumGamePrefs.ZombieBMMove));
		gameServerInfo.SetValue(GameInfoInt.AISmellMode, GamePrefs.GetInt(EnumGamePrefs.AISmellMode));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonFrequency, GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonRange, GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonWarning, GamePrefs.GetInt(EnumGamePrefs.BloodMoonWarning));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonEnemyCount, GamePrefs.GetInt(EnumGamePrefs.BloodMoonEnemyCount));
		gameServerInfo.SetValue(GameInfoInt.LootAbundance, GamePrefs.GetInt(EnumGamePrefs.LootAbundance));
		int num = GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays);
		if (num == 0)
		{
			num = -1;
		}
		gameServerInfo.SetValue(GameInfoInt.LootRespawnDays, num);
		gameServerInfo.SetValue(GameInfoInt.AirDropFrequency, GamePrefs.GetInt(EnumGamePrefs.AirDropFrequency));
		gameServerInfo.SetValue(GameInfoBool.AirDropMarker, GamePrefs.GetBool(EnumGamePrefs.AirDropMarker));
		gameServerInfo.SetValue(GameInfoInt.PartySharedKillRange, GamePrefs.GetInt(EnumGamePrefs.PartySharedKillRange));
		gameServerInfo.SetValue(GameInfoInt.PlayerKillingMode, GamePrefs.GetInt(EnumGamePrefs.PlayerKillingMode));
		gameServerInfo.SetValue(GameInfoInt.LandClaimCount, GamePrefs.GetInt(EnumGamePrefs.LandClaimCount));
		gameServerInfo.SetValue(GameInfoInt.LandClaimSize, GamePrefs.GetInt(EnumGamePrefs.LandClaimSize));
		gameServerInfo.SetValue(GameInfoInt.LandClaimDeadZone, GamePrefs.GetInt(EnumGamePrefs.LandClaimDeadZone));
		gameServerInfo.SetValue(GameInfoInt.LandClaimExpiryTime, GamePrefs.GetInt(EnumGamePrefs.LandClaimExpiryTime));
		gameServerInfo.SetValue(GameInfoInt.LandClaimDecayMode, GamePrefs.GetInt(EnumGamePrefs.LandClaimDecayMode));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOnlineDurabilityModifier, GamePrefs.GetInt(EnumGamePrefs.LandClaimOnlineDurabilityModifier));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOfflineDurabilityModifier, GamePrefs.GetInt(EnumGamePrefs.LandClaimOfflineDurabilityModifier));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOfflineDelay, GamePrefs.GetInt(EnumGamePrefs.LandClaimOfflineDelay));
		gameServerInfo.SetValue(GameInfoInt.MaxChunkAge, GamePrefs.GetInt(EnumGamePrefs.MaxChunkAge));
		gameServerInfo.SetValue(GameInfoBool.ShowFriendPlayerOnMap, GamePrefs.GetBool(EnumGamePrefs.ShowFriendPlayerOnMap));
		gameServerInfo.SetValue(GameInfoInt.DayCount, GamePrefs.GetInt(EnumGamePrefs.DayCount));
		gameServerInfo.SetValue(GameInfoBool.AllowSpawnNearBackpack, GamePrefs.GetBool(EnumGamePrefs.AllowSpawnNearBackpack));
		gameServerInfo.SetValue(GameInfoInt.AllowSpawnNearFriend, GamePrefs.GetInt(EnumGamePrefs.AllowSpawnNearFriend));
		gameServerInfo.SetValue(GameInfoInt.QuestProgressionDailyLimit, GamePrefs.GetInt(EnumGamePrefs.QuestProgressionDailyLimit));
		gameServerInfo.SetValue(GameInfoBool.BiomeProgression, GamePrefs.GetBool(EnumGamePrefs.BiomeProgression));
		gameServerInfo.SetValue(GameInfoInt.StormFreq, GamePrefs.GetInt(EnumGamePrefs.StormFreq));
		gameServerInfo.SetValue(GameInfoInt.JarRefund, GamePrefs.GetInt(EnumGamePrefs.JarRefund));
		return gameServerInfo;
	}

	// Token: 0x06003928 RID: 14632 RVA: 0x001780B9 File Offset: 0x001762B9
	public static void PrepareLocalServerInfo()
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo = GameServerInfo.BuildGameServerInfo();
	}

	// Token: 0x06003929 RID: 14633 RVA: 0x001780CC File Offset: 0x001762CC
	public static void SetLocalServerWorldInfo()
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoInt.CurrentServerTime, (int)GameManager.Instance.World.worldTime);
		Vector3i other;
		Vector3i one;
		GameManager.Instance.World.GetWorldExtent(out other, out one);
		Vector3i vector3i = one - other;
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoInt.WorldSize, vector3i.x);
	}

	// Token: 0x04002ED6 RID: 11990
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	// Token: 0x04002ED7 RID: 11991
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoString, string> tableStrings = new EnumDictionary<GameInfoString, string>();

	// Token: 0x04002ED8 RID: 11992
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoInt, int> tableInts = new EnumDictionary<GameInfoInt, int>();

	// Token: 0x04002ED9 RID: 11993
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoBool, bool> tableBools = new EnumDictionary<GameInfoBool, bool>();

	// Token: 0x04002EDA RID: 11994
	public readonly ReadOnlyDictionary<GameInfoString, string> Strings;

	// Token: 0x04002EDB RID: 11995
	public readonly ReadOnlyDictionary<GameInfoInt, int> Ints;

	// Token: 0x04002EDC RID: 11996
	public readonly ReadOnlyDictionary<GameInfoBool, bool> Bools;

	// Token: 0x04002EDD RID: 11997
	public AuthoredText ServerDisplayName = new AuthoredText();

	// Token: 0x04002EDE RID: 11998
	public AuthoredText ServerWorldName = new AuthoredText();

	// Token: 0x04002EDF RID: 11999
	public AuthoredText ServerDescription = new AuthoredText();

	// Token: 0x04002EE0 RID: 12000
	public AuthoredText ServerURL = new AuthoredText();

	// Token: 0x04002EE1 RID: 12001
	public AuthoredText ServerLoginConfirmationText = new AuthoredText();

	// Token: 0x04002EE2 RID: 12002
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBroken;

	// Token: 0x04002EE3 RID: 12003
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isFriends;

	// Token: 0x04002EE4 RID: 12004
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isFavorite;

	// Token: 0x04002EE5 RID: 12005
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastPlayed;

	// Token: 0x04002EE6 RID: 12006
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLan;

	// Token: 0x04002EE7 RID: 12007
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLobby;

	// Token: 0x04002EE8 RID: 12008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isNoResponse;

	// Token: 0x04002EE9 RID: 12009
	[PublicizedFrom(EAccessModifier.Private)]
	public long hashcode = long.MinValue;

	// Token: 0x04002EEA RID: 12010
	[PublicizedFrom(EAccessModifier.Private)]
	public VersionInformation version = new VersionInformation(VersionInformation.EGameReleaseType.Alpha, -1, -1, -1);

	// Token: 0x04002EEF RID: 12015
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeLastWorldTimeUpdate;

	// Token: 0x04002EF0 RID: 12016
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] whiteSpaceChars = new char[]
	{
		' ',
		'\r',
		'\n',
		'\t'
	};

	// Token: 0x04002EF1 RID: 12017
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedToString;

	// Token: 0x04002EF2 RID: 12018
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedToStringLineBreaks;

	// Token: 0x04002EF3 RID: 12019
	public static readonly GameInfoString[] SearchableStringInfos = new GameInfoString[]
	{
		GameInfoString.LevelName,
		GameInfoString.GameHost,
		GameInfoString.SteamID,
		GameInfoString.Region,
		GameInfoString.Language,
		GameInfoString.UniqueId,
		GameInfoString.CombinedNativeId,
		GameInfoString.ServerVersion,
		GameInfoString.PlayGroup,
		GameInfoString.SandboxCode
	};

	// Token: 0x04002EF4 RID: 12020
	public static readonly GameInfoInt[] IntInfosInGameTags = new GameInfoInt[]
	{
		GameInfoInt.DayNightLength,
		GameInfoInt.DeathPenalty,
		GameInfoInt.DropOnDeath,
		GameInfoInt.DropOnQuit,
		GameInfoInt.BloodMoonEnemyCount,
		GameInfoInt.EnemyDifficulty,
		GameInfoInt.PlayerKillingMode,
		GameInfoInt.CurrentServerTime,
		GameInfoInt.DayLightLength,
		GameInfoInt.AirDropFrequency,
		GameInfoInt.LootAbundance,
		GameInfoInt.LootRespawnDays,
		GameInfoInt.MaxSpawnedZombies,
		GameInfoInt.LandClaimCount,
		GameInfoInt.LandClaimSize,
		GameInfoInt.LandClaimExpiryTime,
		GameInfoInt.LandClaimDecayMode,
		GameInfoInt.LandClaimOnlineDurabilityModifier,
		GameInfoInt.LandClaimOfflineDurabilityModifier,
		GameInfoInt.MaxSpawnedAnimals,
		GameInfoInt.PartySharedKillRange,
		GameInfoInt.ZombieFeralSense,
		GameInfoInt.ZombieMove,
		GameInfoInt.ZombieMoveNight,
		GameInfoInt.ZombieFeralMove,
		GameInfoInt.ZombieBMMove,
		GameInfoInt.AISmellMode,
		GameInfoInt.XPMultiplier,
		GameInfoInt.BlockDamagePlayer,
		GameInfoInt.BlockDamageAI,
		GameInfoInt.BlockDamageAIBM,
		GameInfoInt.BloodMoonFrequency,
		GameInfoInt.BloodMoonRange,
		GameInfoInt.BloodMoonWarning,
		GameInfoInt.BedrollExpiryTime,
		GameInfoInt.LandClaimOfflineDelay,
		GameInfoInt.Port,
		GameInfoInt.FreePlayerSlots,
		GameInfoInt.CurrentPlayers,
		GameInfoInt.MaxPlayers,
		GameInfoInt.WorldSize,
		GameInfoInt.MaxChunkAge,
		GameInfoInt.QuestProgressionDailyLimit,
		GameInfoInt.StormFreq,
		GameInfoInt.JarRefund
	};

	// Token: 0x04002EF5 RID: 12021
	public static readonly GameInfoBool[] BoolInfosInGameTags = new GameInfoBool[]
	{
		GameInfoBool.IsDedicated,
		GameInfoBool.ShowFriendPlayerOnMap,
		GameInfoBool.BuildCreate,
		GameInfoBool.StockSettings,
		GameInfoBool.ModdedConfig,
		GameInfoBool.RequiresMod,
		GameInfoBool.AirDropMarker,
		GameInfoBool.EnemySpawnMode,
		GameInfoBool.IsPasswordProtected,
		GameInfoBool.AllowCrossplay,
		GameInfoBool.EACEnabled,
		GameInfoBool.SanctionsIgnored,
		GameInfoBool.BiomeProgression
	};

	// Token: 0x04002EF6 RID: 12022
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoString> SearchableStringInfosSet = new HashSet<GameInfoString>(GameServerInfo.SearchableStringInfos);

	// Token: 0x04002EF7 RID: 12023
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoInt> IntInfosInGameTagsSet = new HashSet<GameInfoInt>(GameServerInfo.IntInfosInGameTags);

	// Token: 0x04002EF8 RID: 12024
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoBool> BoolInfosInGameTagsSet = new HashSet<GameInfoBool>(GameServerInfo.BoolInfosInGameTags);

	// Token: 0x02000779 RID: 1913
	public class UniqueIdEqualityComparer : IEqualityComparer<GameServerInfo>
	{
		// Token: 0x0600392B RID: 14635 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Private)]
		public UniqueIdEqualityComparer()
		{
		}

		// Token: 0x0600392C RID: 14636 RVA: 0x001781DB File Offset: 0x001763DB
		public bool Equals(GameServerInfo _x, GameServerInfo _y)
		{
			return _x == _y || (_x != null && _y != null && _x.GetValue(GameInfoString.UniqueId) == _y.GetValue(GameInfoString.UniqueId));
		}

		// Token: 0x0600392D RID: 14637 RVA: 0x00178200 File Offset: 0x00176400
		public int GetHashCode(GameServerInfo _obj)
		{
			return _obj.GetValue(GameInfoString.UniqueId).GetHashCode();
		}

		// Token: 0x04002EF9 RID: 12025
		public static readonly GameServerInfo.UniqueIdEqualityComparer Instance = new GameServerInfo.UniqueIdEqualityComparer();
	}
}
