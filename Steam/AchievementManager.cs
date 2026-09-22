using System;
using System.Collections.Generic;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C7F RID: 7295
	public class AchievementManager : IAchievementManager
	{
		// Token: 0x0600D845 RID: 55365 RVA: 0x004DF210 File Offset: 0x004DD410
		public AchievementManager()
		{
			this.m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.UserStatsReceived_Callback));
			this.m_UserStatsStored = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate(this.UserStatsStored_Callback));
			this.m_UserAchievementStored_t = Callback<UserAchievementStored_t>.Create(new Callback<UserAchievementStored_t>.DispatchDelegate(this.UserAchievementStored_Callback));
		}

		// Token: 0x0600D846 RID: 55366 RVA: 0x004DF27E File Offset: 0x004DD47E
		public void Init(IPlatform _owner)
		{
			_owner.User.UserLoggedIn += delegate(IPlatform _sender)
			{
				SteamUserStats.RequestCurrentStats();
			};
		}

		// Token: 0x0600D847 RID: 55367 RVA: 0x004DF2AC File Offset: 0x004DD4AC
		[PublicizedFrom(EAccessModifier.Private)]
		public void UserStatsReceived_Callback(UserStatsReceived_t _result)
		{
			if (_result.m_nGameID != 251570UL)
			{
				return;
			}
			if (_result.m_eResult != EResult.k_EResultOK)
			{
				Log.Error("AchievementManager: RequestStats failed: {0}", new object[]
				{
					_result.m_eResult.ToStringCached<EResult>()
				});
				return;
			}
			if (this.steamStatsCache.Count > 0)
			{
				return;
			}
			Log.Out("AchievementManager: Received stats and achievements from Steam");
			for (int i = 0; i < 19; i++)
			{
				EnumAchievementDataStat enumAchievementDataStat = (EnumAchievementDataStat)i;
				if (enumAchievementDataStat.IsSupported())
				{
					switch (AchievementData.GetStatType(enumAchievementDataStat))
					{
					case EnumStatType.Int:
					{
						int iValue;
						if (SteamUserStats.GetStat(enumAchievementDataStat.ToStringCached<EnumAchievementDataStat>(), out iValue))
						{
							this.steamStatsCache.Add(enumAchievementDataStat, new AchievementManager.StatCacheEntry(enumAchievementDataStat.ToStringCached<EnumAchievementDataStat>(), iValue, 0f));
						}
						break;
					}
					case EnumStatType.Float:
					{
						float fValue;
						if (SteamUserStats.GetStat(enumAchievementDataStat.ToStringCached<EnumAchievementDataStat>(), out fValue))
						{
							this.steamStatsCache.Add(enumAchievementDataStat, new AchievementManager.StatCacheEntry(enumAchievementDataStat.ToStringCached<EnumAchievementDataStat>(), 0, fValue));
						}
						break;
					}
					}
				}
			}
			for (int j = 0; j < 48; j++)
			{
				EnumAchievementManagerAchievement enumAchievementManagerAchievement = (EnumAchievementManagerAchievement)j;
				bool locked;
				if (enumAchievementManagerAchievement.IsSupported() && SteamUserStats.GetAchievement(enumAchievementManagerAchievement.ToStringCached<EnumAchievementManagerAchievement>(), out locked))
				{
					this.steamAchievementsCache.Add(enumAchievementManagerAchievement, new AchievementManager.AchievementCacheEntry(enumAchievementManagerAchievement.ToStringCached<EnumAchievementManagerAchievement>(), locked));
				}
			}
		}

		// Token: 0x0600D848 RID: 55368 RVA: 0x004DF3E4 File Offset: 0x004DD5E4
		[PublicizedFrom(EAccessModifier.Private)]
		public void UserStatsStored_Callback(UserStatsStored_t _result)
		{
			Log.Out("AchievementManager.UserStatsStored_Callback, result={0}", new object[]
			{
				_result.m_eResult.ToStringCached<EResult>()
			});
		}

		// Token: 0x0600D849 RID: 55369 RVA: 0x004DF404 File Offset: 0x004DD604
		[PublicizedFrom(EAccessModifier.Private)]
		public void UserAchievementStored_Callback(UserAchievementStored_t _result)
		{
			Log.Out("AchievementManager.UserAchievementStored_Callback, name={0}, cur={1}, max={2}", new object[]
			{
				_result.m_rgchAchievementName,
				_result.m_nCurProgress,
				_result.m_nMaxProgress
			});
		}

		// Token: 0x0600D84A RID: 55370 RVA: 0x004DF43C File Offset: 0x004DD63C
		public void ShowAchievementsUi()
		{
			Log.Out("AchievementManager.ShowAchievementsUI");
			SteamFriends.ActivateGameOverlay("Achievements");
		}

		// Token: 0x0600D84B RID: 55371 RVA: 0x004DF454 File Offset: 0x004DD654
		[PublicizedFrom(EAccessModifier.Private)]
		public void SendAchievementEvent(EnumAchievementManagerAchievement _achievement)
		{
			if (AchievementUtils.IsCreativeModeActive())
			{
				return;
			}
			Log.Out("AchievementManager.SendAchievementEvent (" + _achievement.ToStringCached<EnumAchievementManagerAchievement>() + ")");
			AchievementManager.AchievementCacheEntry achievementCacheEntry;
			if (this.steamAchievementsCache.TryGetValue(_achievement, out achievementCacheEntry))
			{
				SteamUserStats.SetAchievement(achievementCacheEntry.name);
				this.steamAchievementsCache[_achievement] = new AchievementManager.AchievementCacheEntry(achievementCacheEntry.name, true);
				SteamUserStats.StoreStats();
			}
		}

		// Token: 0x0600D84C RID: 55372 RVA: 0x004DF4C0 File Offset: 0x004DD6C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetAchievementStatValueFloat(EnumAchievementDataStat _stat, float _value)
		{
			AchievementManager.StatCacheEntry statCacheEntry;
			if (!this.steamStatsCache.TryGetValue(_stat, out statCacheEntry) || AchievementData.GetStatType(_stat) != EnumStatType.Float)
			{
				return;
			}
			this.steamStatsCache[_stat] = new AchievementManager.StatCacheEntry(statCacheEntry.name, 0, _value);
			SteamUserStats.SetStat(statCacheEntry.name, _value);
		}

		// Token: 0x0600D84D RID: 55373 RVA: 0x004DF510 File Offset: 0x004DD710
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetAchievementStatValueInt(EnumAchievementDataStat _stat, int _value)
		{
			AchievementManager.StatCacheEntry statCacheEntry;
			if (!this.steamStatsCache.TryGetValue(_stat, out statCacheEntry) || AchievementData.GetStatType(_stat) != EnumStatType.Int)
			{
				return;
			}
			this.steamStatsCache[_stat] = new AchievementManager.StatCacheEntry(statCacheEntry.name, _value, 0f);
			SteamUserStats.SetStat(statCacheEntry.name, _value);
		}

		// Token: 0x0600D84E RID: 55374 RVA: 0x004DF560 File Offset: 0x004DD760
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetAchievementStatValueFloat(EnumAchievementDataStat _stat)
		{
			AchievementManager.StatCacheEntry statCacheEntry;
			if (this.steamStatsCache.TryGetValue(_stat, out statCacheEntry) && AchievementData.GetStatType(_stat) == EnumStatType.Float)
			{
				return statCacheEntry.fValue;
			}
			return 0f;
		}

		// Token: 0x0600D84F RID: 55375 RVA: 0x004DF594 File Offset: 0x004DD794
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetAchievementStatValueInt(EnumAchievementDataStat _stat)
		{
			AchievementManager.StatCacheEntry statCacheEntry;
			if (this.steamStatsCache.TryGetValue(_stat, out statCacheEntry) && AchievementData.GetStatType(_stat) == EnumStatType.Int)
			{
				return statCacheEntry.iValue;
			}
			return 0;
		}

		// Token: 0x0600D850 RID: 55376 RVA: 0x004DF5C4 File Offset: 0x004DD7C4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsAchievementLocked(EnumAchievementManagerAchievement _achievement)
		{
			AchievementManager.AchievementCacheEntry achievementCacheEntry;
			return this.steamAchievementsCache.TryGetValue(_achievement, out achievementCacheEntry) && achievementCacheEntry.locked;
		}

		// Token: 0x0600D851 RID: 55377 RVA: 0x004DF5EC File Offset: 0x004DD7EC
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateAchievement(EnumAchievementDataStat _stat, float _newValue)
		{
			List<AchievementData.AchievementInfo> achievementInfos = AchievementData.GetAchievementInfos(_stat);
			for (int i = 0; i < achievementInfos.Count; i++)
			{
				EnumAchievementManagerAchievement achievement = achievementInfos[i].achievement;
				if (_newValue >= Convert.ToSingle(achievementInfos[i].triggerPoint) && !this.IsAchievementLocked(achievement))
				{
					this.SendAchievementEvent(achievement);
				}
			}
		}

		// Token: 0x0600D852 RID: 55378 RVA: 0x004DF644 File Offset: 0x004DD844
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateAchievement(EnumAchievementDataStat _stat, int _newValue)
		{
			List<AchievementData.AchievementInfo> achievementInfos = AchievementData.GetAchievementInfos(_stat);
			for (int i = 0; i < achievementInfos.Count; i++)
			{
				EnumAchievementManagerAchievement achievement = achievementInfos[i].achievement;
				if (_newValue >= Convert.ToInt32(achievementInfos[i].triggerPoint) && !this.IsAchievementLocked(achievement))
				{
					this.SendAchievementEvent(achievement);
				}
			}
		}

		// Token: 0x0600D853 RID: 55379 RVA: 0x004DF69C File Offset: 0x004DD89C
		public bool IsAchievementStatSupported(EnumAchievementDataStat _stat)
		{
			return _stat != EnumAchievementDataStat.HighestGamestage;
		}

		// Token: 0x0600D854 RID: 55380 RVA: 0x004DF6B8 File Offset: 0x004DD8B8
		public void SetAchievementStat(EnumAchievementDataStat _stat, int _value)
		{
			if (!_stat.IsSupported())
			{
				return;
			}
			if (AchievementUtils.IsCreativeModeActive())
			{
				return;
			}
			AchievementData.EnumUpdateType updateType = AchievementData.GetUpdateType(_stat);
			EnumStatType statType = AchievementData.GetStatType(_stat);
			if (!this.steamStatsCache.ContainsKey(_stat))
			{
				return;
			}
			if (statType != EnumStatType.Int)
			{
				Log.Warning("AchievementManager.SetAchievementStat, int given for float type stat {0}", new object[]
				{
					_stat.ToStringCached<EnumAchievementDataStat>()
				});
				return;
			}
			int achievementStatValueInt = this.GetAchievementStatValueInt(_stat);
			int num;
			switch (updateType)
			{
			case AchievementData.EnumUpdateType.Sum:
				num = achievementStatValueInt + _value;
				break;
			case AchievementData.EnumUpdateType.Replace:
				num = _value;
				break;
			case AchievementData.EnumUpdateType.Max:
				num = Math.Max(achievementStatValueInt, _value);
				break;
			default:
				num = 0;
				break;
			}
			int num2 = num;
			if (achievementStatValueInt != num2)
			{
				this.SetAchievementStatValueInt(_stat, num2);
				this.UpdateAchievement(_stat, num2);
			}
		}

		// Token: 0x0600D855 RID: 55381 RVA: 0x004DF760 File Offset: 0x004DD960
		public void SetAchievementStat(EnumAchievementDataStat _stat, float _value)
		{
			if (!_stat.IsSupported())
			{
				return;
			}
			if (AchievementUtils.IsCreativeModeActive())
			{
				return;
			}
			AchievementData.EnumUpdateType updateType = AchievementData.GetUpdateType(_stat);
			EnumStatType statType = AchievementData.GetStatType(_stat);
			if (!this.steamStatsCache.ContainsKey(_stat))
			{
				return;
			}
			if (statType != EnumStatType.Float)
			{
				Log.Warning("AchievementManager.SetAchievementStat, float given for int type stat {0}", new object[]
				{
					_stat.ToStringCached<EnumAchievementDataStat>()
				});
				return;
			}
			float achievementStatValueFloat = this.GetAchievementStatValueFloat(_stat);
			float num;
			switch (updateType)
			{
			case AchievementData.EnumUpdateType.Sum:
				num = achievementStatValueFloat + _value;
				break;
			case AchievementData.EnumUpdateType.Replace:
				num = _value;
				break;
			case AchievementData.EnumUpdateType.Max:
				num = Math.Max(achievementStatValueFloat, _value);
				break;
			default:
				num = achievementStatValueFloat;
				break;
			}
			float num2 = num;
			if (achievementStatValueFloat != num2)
			{
				this.SetAchievementStatValueFloat(_stat, num2);
				this.UpdateAchievement(_stat, num2);
			}
		}

		// Token: 0x0600D856 RID: 55382 RVA: 0x004DF806 File Offset: 0x004DDA06
		public void ResetStats(bool _andAchievements)
		{
			SteamUserStats.ResetAllStats(_andAchievements);
		}

		// Token: 0x0600D857 RID: 55383 RVA: 0x004DF810 File Offset: 0x004DDA10
		public void UnlockAllAchievements()
		{
			for (int i = 0; i < 19; i++)
			{
				EnumAchievementDataStat stat = (EnumAchievementDataStat)i;
				if (stat.IsSupported())
				{
					List<AchievementData.AchievementInfo> achievementInfos = AchievementData.GetAchievementInfos(stat);
					AchievementData.AchievementInfo achievementInfo = achievementInfos[achievementInfos.Count - 1];
					switch (AchievementData.GetStatType(stat))
					{
					case EnumStatType.Int:
						this.SetAchievementStat(stat, Convert.ToInt32(achievementInfo.triggerPoint));
						break;
					case EnumStatType.Float:
						this.SetAchievementStat(stat, Convert.ToSingle(achievementInfo.triggerPoint));
						break;
					}
				}
			}
		}

		// Token: 0x0600D858 RID: 55384 RVA: 0x004DF88B File Offset: 0x004DDA8B
		public void Destroy()
		{
			Log.Out("AchievementManager.Cleanup");
		}

		// Token: 0x0400A4A3 RID: 42147
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<UserStatsReceived_t> m_UserStatsReceived;

		// Token: 0x0400A4A4 RID: 42148
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<UserStatsStored_t> m_UserStatsStored;

		// Token: 0x0400A4A5 RID: 42149
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<UserAchievementStored_t> m_UserAchievementStored_t;

		// Token: 0x0400A4A6 RID: 42150
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<EnumAchievementDataStat, AchievementManager.StatCacheEntry> steamStatsCache = new EnumDictionary<EnumAchievementDataStat, AchievementManager.StatCacheEntry>();

		// Token: 0x0400A4A7 RID: 42151
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<EnumAchievementManagerAchievement, AchievementManager.AchievementCacheEntry> steamAchievementsCache = new EnumDictionary<EnumAchievementManagerAchievement, AchievementManager.AchievementCacheEntry>();

		// Token: 0x02001C80 RID: 7296
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct StatCacheEntry
		{
			// Token: 0x0600D859 RID: 55385 RVA: 0x004DF897 File Offset: 0x004DDA97
			public StatCacheEntry(string _name, int _iValue, float _fValue)
			{
				this.name = _name;
				this.iValue = _iValue;
				this.fValue = _fValue;
			}

			// Token: 0x0400A4A8 RID: 42152
			public readonly string name;

			// Token: 0x0400A4A9 RID: 42153
			public readonly int iValue;

			// Token: 0x0400A4AA RID: 42154
			public readonly float fValue;
		}

		// Token: 0x02001C81 RID: 7297
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct AchievementCacheEntry
		{
			// Token: 0x0600D85A RID: 55386 RVA: 0x004DF8AE File Offset: 0x004DDAAE
			public AchievementCacheEntry(string _name, bool _locked)
			{
				this.name = _name;
				this.locked = _locked;
			}

			// Token: 0x0400A4AB RID: 42155
			public readonly string name;

			// Token: 0x0400A4AC RID: 42156
			public readonly bool locked;
		}
	}
}
