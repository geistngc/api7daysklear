using System;
using System.Collections.Generic;

// Token: 0x020011FB RID: 4603
public static class GameSparksCollector
{
	// Token: 0x1700118A RID: 4490
	// (get) Token: 0x0600930A RID: 37642 RVA: 0x00378A26 File Offset: 0x00376C26
	// (set) Token: 0x0600930B RID: 37643 RVA: 0x00378A2D File Offset: 0x00376C2D
	public static bool CollectGamePlayData { get; set; }

	// Token: 0x0600930C RID: 37644 RVA: 0x00378A38 File Offset: 0x00376C38
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData GetObject(string _keyString, GSRequestData _collection)
	{
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = _collection.GetGSData(_keyString);
			if (gsrequestData != null)
			{
				result = gsrequestData;
			}
			else
			{
				gsrequestData = new GSRequestData();
				_collection.Add(_keyString, gsrequestData);
				result = gsrequestData;
			}
		}
		return result;
	}

	// Token: 0x0600930D RID: 37645 RVA: 0x00378A94 File Offset: 0x00376C94
	[PublicizedFrom(EAccessModifier.Private)]
	public static ValueTuple<GSRequestData, string> GetRequestDataAndKey(GameSparksCollector.GSDataCollection _collectionType, GameSparksCollector.GSDataKey _key, string _subKey = null)
	{
		string text = _key.ToStringCached<GameSparksCollector.GSDataKey>();
		GSRequestData gsrequestData = (_collectionType == GameSparksCollector.GSDataCollection.SessionUpdates) ? GameSparksCollector.dataUpdates : GameSparksCollector.dataSessionTotal;
		if (_subKey == null)
		{
			return new ValueTuple<GSRequestData, string>(gsrequestData, text);
		}
		return new ValueTuple<GSRequestData, string>(GameSparksCollector.GetObject(text, gsrequestData), _subKey);
	}

	// Token: 0x0600930E RID: 37646 RVA: 0x00378AD4 File Offset: 0x00376CD4
	public static void SetValue(GameSparksCollector.GSDataKey _key, string _subKey, int _value, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				item.AddNumber(item2, _value);
			}
		}
	}

	// Token: 0x0600930F RID: 37647 RVA: 0x00378B38 File Offset: 0x00376D38
	public static void SetValue(GameSparksCollector.GSDataKey _key, string _subKey, string _value, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				item.AddString(item2, _value);
			}
		}
	}

	// Token: 0x06009310 RID: 37648 RVA: 0x00378B9C File Offset: 0x00376D9C
	public static void IncrementCounter(GameSparksCollector.GSDataKey _key, string _subKey, int _increment, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				int num = item.GetInt(item2).GetValueOrDefault();
				num += _increment;
				item.AddNumber(item2, num);
			}
		}
	}

	// Token: 0x06009311 RID: 37649 RVA: 0x00378C18 File Offset: 0x00376E18
	public static void IncrementCounter(GameSparksCollector.GSDataKey _key, string _subKey, float _increment, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				float num = item.GetFloat(item2).GetValueOrDefault();
				num += _increment;
				item.AddNumber(item2, (double)num);
			}
		}
	}

	// Token: 0x06009312 RID: 37650 RVA: 0x00378C94 File Offset: 0x00376E94
	public static void SetMax(GameSparksCollector.GSDataKey _key, string _subKey, int _currentValue, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				int num = item.GetInt(item2) ?? int.MinValue;
				num = Math.Max(num, _currentValue);
				item.AddNumber(item2, num);
			}
		}
	}

	// Token: 0x06009313 RID: 37651 RVA: 0x00378D24 File Offset: 0x00376F24
	public static GSRequestData GetSessionUpdateDataAndReset()
	{
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = GameSparksCollector.dataUpdates;
			GameSparksCollector.dataUpdates = new GSRequestData();
			result = gsrequestData;
		}
		return result;
	}

	// Token: 0x06009314 RID: 37652 RVA: 0x00378D70 File Offset: 0x00376F70
	public static GSRequestData GetSessionTotalData(bool _reset)
	{
		if (!_reset)
		{
			return GameSparksCollector.dataSessionTotal;
		}
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = GameSparksCollector.dataSessionTotal;
			GameSparksCollector.dataSessionTotal = new GSRequestData();
			result = gsrequestData;
		}
		return result;
	}

	// Token: 0x06009315 RID: 37653 RVA: 0x00378DC4 File Offset: 0x00376FC4
	public static void PlayerLevelUp(EntityPlayerLocal _localPlayer, int _level)
	{
		if (_level == 15)
		{
			GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel15, _localPlayer);
			GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel15, _localPlayer);
			return;
		}
		if (_level == 30)
		{
			GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel30, _localPlayer);
			GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel30, _localPlayer);
			return;
		}
		if (_level != 50)
		{
			return;
		}
		GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel50, _localPlayer);
		GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel50, _localPlayer);
	}

	// Token: 0x06009316 RID: 37654 RVA: 0x00378E04 File Offset: 0x00377004
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SendSkillStats(GameSparksCollector.GSDataKey _key, EntityPlayerLocal _localPlayer)
	{
		foreach (KeyValuePair<int, ProgressionValue> keyValuePair in _localPlayer.Progression.GetDict())
		{
			ProgressionClass progressionClass = keyValuePair.Value.ProgressionClass;
			for (int i = progressionClass.MinLevel + 1; i <= keyValuePair.Value.Level; i++)
			{
				string subKey;
				if (progressionClass.Parent == null || progressionClass.Parent == progressionClass)
				{
					subKey = string.Format("{0}_{1}", progressionClass.Name, i);
				}
				else
				{
					ProgressionClass parent = progressionClass.Parent;
					while (parent.Parent != null && parent.Parent != parent)
					{
						parent = parent.Parent;
					}
					subKey = string.Format("{0}_{1}_{2}", parent.Name, progressionClass.Name, i);
				}
				GameSparksCollector.SetValue(_key, subKey, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
			}
		}
	}

	// Token: 0x06009317 RID: 37655 RVA: 0x00378F08 File Offset: 0x00377108
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SendSaveTimePlayed(GameSparksCollector.GSDataKey _key, EntityPlayerLocal _localPlayer)
	{
		int num = (int)(_localPlayer.totalTimePlayed / 60f);
		if (num > 0)
		{
			GameSparksCollector.SetValue(_key, null, num, true, GameSparksCollector.GSDataCollection.SessionUpdates);
		}
	}

	// Token: 0x04006DDD RID: 28125
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObject = new object();

	// Token: 0x04006DDE RID: 28126
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData dataUpdates = new GSRequestData();

	// Token: 0x04006DDF RID: 28127
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData dataSessionTotal = new GSRequestData();

	// Token: 0x020011FC RID: 4604
	public enum GSDataKey
	{
		// Token: 0x04006DE2 RID: 28130
		HoursPlayedAtLevel15,
		// Token: 0x04006DE3 RID: 28131
		HoursPlayedAtLevel30,
		// Token: 0x04006DE4 RID: 28132
		HoursPlayedAtLevel50,
		// Token: 0x04006DE5 RID: 28133
		SkillsPurchasedAtLevel15,
		// Token: 0x04006DE6 RID: 28134
		SkillsPurchasedAtLevel30,
		// Token: 0x04006DE7 RID: 28135
		SkillsPurchasedAtLevel50,
		// Token: 0x04006DE8 RID: 28136
		PlayerLevelAtHour,
		// Token: 0x04006DE9 RID: 28137
		XpEarnedBy,
		// Token: 0x04006DEA RID: 28138
		PlayerDeathCauses,
		// Token: 0x04006DEB RID: 28139
		ZombiesKilledBy,
		// Token: 0x04006DEC RID: 28140
		CraftedItems,
		// Token: 0x04006DED RID: 28141
		TraderItemsBought,
		// Token: 0x04006DEE RID: 28142
		VendingItemsBought,
		// Token: 0x04006DEF RID: 28143
		TraderMoneySpentOn,
		// Token: 0x04006DF0 RID: 28144
		VendingMoneySpentOn,
		// Token: 0x04006DF1 RID: 28145
		TotalMoneySpentOn,
		// Token: 0x04006DF2 RID: 28146
		PeakConcurrentClients,
		// Token: 0x04006DF3 RID: 28147
		PeakConcurrentPlayers,
		// Token: 0x04006DF4 RID: 28148
		QuestTraderToTraderDistance,
		// Token: 0x04006DF5 RID: 28149
		QuestAcceptedDistance,
		// Token: 0x04006DF6 RID: 28150
		QuestOfferedDistance,
		// Token: 0x04006DF7 RID: 28151
		QuestStarterTraderDistance,
		// Token: 0x04006DF8 RID: 28152
		PlayerProfileIsCustom,
		// Token: 0x04006DF9 RID: 28153
		PlayerArchetypeName,
		// Token: 0x04006DFA RID: 28154
		UsedTwitchIntegration
	}

	// Token: 0x020011FD RID: 4605
	public enum GSDataCollection
	{
		// Token: 0x04006DFC RID: 28156
		SessionTotal,
		// Token: 0x04006DFD RID: 28157
		SessionUpdates
	}
}
