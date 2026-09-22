using System;
using System.Collections.Generic;
using Unity.XGamingRuntime;
using UnityEngine;

namespace Platform.XBL
{
	// Token: 0x02001C2E RID: 7214
	public class AchievementManager : IAchievementManager
	{
		// Token: 0x0600D61B RID: 54811 RVA: 0x004D4298 File Offset: 0x004D2498
		[PublicizedFrom(EAccessModifier.Private)]
		static AchievementManager()
		{
			string launchArgument = GameUtils.GetLaunchArgument("debugachievements");
			if (launchArgument != null)
			{
				if (launchArgument == "verbose")
				{
					AchievementManager.debug = AchievementManager.EDebugLevel.Verbose;
					return;
				}
				AchievementManager.debug = AchievementManager.EDebugLevel.Normal;
			}
		}

		// Token: 0x0600D61C RID: 54812 RVA: 0x004D42D4 File Offset: 0x004D24D4
		public void Init(IPlatform _owner)
		{
			_owner.User.UserLoggedIn += delegate(IPlatform _sender)
			{
				this.xblUser = (User)_owner.User;
			};
		}

		// Token: 0x0600D61D RID: 54813 RVA: 0x004D4311 File Offset: 0x004D2511
		public void ShowAchievementsUi()
		{
			SDK.XGameUiShowAchievementsAsync(this.xblUser.UserHandle, 1745806870U, delegate(int _hresult)
			{
				XblHelpers.Succeeded(_hresult, "Open achievements UI", true, false);
			});
		}

		// Token: 0x0600D61E RID: 54814 RVA: 0x004D4348 File Offset: 0x004D2548
		public bool IsAchievementStatSupported(EnumAchievementDataStat _stat)
		{
			return _stat != EnumAchievementDataStat.HighestPlayerLevel;
		}

		// Token: 0x0600D61F RID: 54815 RVA: 0x004D4364 File Offset: 0x004D2564
		public void SetAchievementStat(EnumAchievementDataStat _stat, int _value)
		{
			if (!_stat.IsSupported())
			{
				return;
			}
			if (AchievementUtils.IsCreativeModeActive())
			{
				if (AchievementManager.debug != AchievementManager.EDebugLevel.Off && Time.unscaledTime - this.lastAchievementsDisabledWarningTime > 30f)
				{
					this.lastAchievementsDisabledWarningTime = Time.unscaledTime;
					Log.Warning("[XBL] Achievements disabled due to creative mode, creative menu or debug menu enabled");
				}
				return;
			}
			if (AchievementData.GetStatType(_stat) != EnumStatType.Int)
			{
				Log.Warning("AchievementManager.SetAchievementStat, int given for float type stat {0}", new object[]
				{
					_stat.ToStringCached<EnumAchievementDataStat>()
				});
				return;
			}
			AchievementManager.StatCacheEntry statCacheEntry;
			if (AchievementData.GetUpdateType(_stat) != AchievementData.EnumUpdateType.Sum && this.sentStatsCache.TryGetValue(_stat, out statCacheEntry) && statCacheEntry.iValue == _value)
			{
				if (AchievementManager.debug == AchievementManager.EDebugLevel.Verbose && Time.unscaledTime - statCacheEntry.lastSendTime > 30f)
				{
					this.sentStatsCache[_stat] = new AchievementManager.StatCacheEntry(_value, 0f, Time.unscaledTime);
					Log.Warning(string.Format("[XBL] Not sending achievement {0}, already sent with value {1}", _stat.ToStringCached<EnumAchievementDataStat>(), _value));
				}
				return;
			}
			if (XblHelpers.Succeeded(SDK.XBL.XblEventsWriteInGameEvent(this.xblUser.XblContextHandle, _stat.ToStringCached<EnumAchievementDataStat>(), string.Format("{{\"Value\":{0}}}", _value), "{}"), "Send int stat event '" + _stat.ToStringCached<EnumAchievementDataStat>() + "'", true, false))
			{
				this.sentStatsCache[_stat] = new AchievementManager.StatCacheEntry(_value, 0f, Time.unscaledTime);
				if (AchievementManager.debug == AchievementManager.EDebugLevel.Verbose)
				{
					Log.Out(string.Format("[XBL] Sent achievement update: {0} = {1}", _stat.ToStringCached<EnumAchievementDataStat>(), _value));
				}
			}
		}

		// Token: 0x0600D620 RID: 54816 RVA: 0x004D44D0 File Offset: 0x004D26D0
		public void SetAchievementStat(EnumAchievementDataStat _stat, float _value)
		{
			if (!_stat.IsSupported())
			{
				return;
			}
			if (AchievementUtils.IsCreativeModeActive())
			{
				if (AchievementManager.debug != AchievementManager.EDebugLevel.Off && Time.unscaledTime - this.lastAchievementsDisabledWarningTime > 30f)
				{
					this.lastAchievementsDisabledWarningTime = Time.unscaledTime;
					Log.Warning("[XBL] Achievements disabled due to creative mode, creative menu or debug menu enabled");
				}
				return;
			}
			if (AchievementData.GetStatType(_stat) != EnumStatType.Float)
			{
				Log.Warning("AchievementManager.SetAchievementStat, float given for int type stat {0}", new object[]
				{
					_stat.ToStringCached<EnumAchievementDataStat>()
				});
				return;
			}
			AchievementManager.StatCacheEntry statCacheEntry;
			if (AchievementData.GetUpdateType(_stat) != AchievementData.EnumUpdateType.Sum && this.sentStatsCache.TryGetValue(_stat, out statCacheEntry) && statCacheEntry.fValue == _value)
			{
				if (AchievementManager.debug == AchievementManager.EDebugLevel.Verbose && Time.unscaledTime - statCacheEntry.lastSendTime > 30f)
				{
					this.sentStatsCache[_stat] = new AchievementManager.StatCacheEntry(0, _value, Time.unscaledTime);
					Log.Warning("[XBL] Not sending achievement " + _stat.ToStringCached<EnumAchievementDataStat>() + ", already sent with value " + _value.ToCultureInvariantString());
				}
				return;
			}
			if (XblHelpers.Succeeded(SDK.XBL.XblEventsWriteInGameEvent(this.xblUser.XblContextHandle, _stat.ToStringCached<EnumAchievementDataStat>(), "{\"Value\":" + _value.ToCultureInvariantString() + "}", "{}"), "Send float stat event '" + _stat.ToStringCached<EnumAchievementDataStat>() + "'", true, false))
			{
				this.sentStatsCache[_stat] = new AchievementManager.StatCacheEntry(0, _value, Time.unscaledTime);
				if (AchievementManager.debug == AchievementManager.EDebugLevel.Verbose)
				{
					Log.Out(string.Format("[XBL] Sent achievement update: {0} = {1}", _stat.ToStringCached<EnumAchievementDataStat>(), _value));
				}
			}
		}

		// Token: 0x0600D621 RID: 54817 RVA: 0x000027FC File Offset: 0x000009FC
		public void ResetStats(bool _andAchievements)
		{
		}

		// Token: 0x0600D622 RID: 54818 RVA: 0x004D4640 File Offset: 0x004D2840
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

		// Token: 0x0600D623 RID: 54819 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0400A364 RID: 41828
		[PublicizedFrom(EAccessModifier.Private)]
		public const int suppressRepeatedNotSentWarningsTime = 30;

		// Token: 0x0400A365 RID: 41829
		[PublicizedFrom(EAccessModifier.Private)]
		public User xblUser;

		// Token: 0x0400A366 RID: 41830
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly AchievementManager.EDebugLevel debug = AchievementManager.EDebugLevel.Off;

		// Token: 0x0400A367 RID: 41831
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<EnumAchievementDataStat, AchievementManager.StatCacheEntry> sentStatsCache = new EnumDictionary<EnumAchievementDataStat, AchievementManager.StatCacheEntry>();

		// Token: 0x0400A368 RID: 41832
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastAchievementsDisabledWarningTime;

		// Token: 0x02001C2F RID: 7215
		[PublicizedFrom(EAccessModifier.Private)]
		public enum EDebugLevel
		{
			// Token: 0x0400A36A RID: 41834
			Off,
			// Token: 0x0400A36B RID: 41835
			Normal,
			// Token: 0x0400A36C RID: 41836
			Verbose
		}

		// Token: 0x02001C30 RID: 7216
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct StatCacheEntry
		{
			// Token: 0x0600D625 RID: 54821 RVA: 0x004D46CE File Offset: 0x004D28CE
			public StatCacheEntry(int _iValue, float _fValue, float _lastSendTime)
			{
				this.iValue = _iValue;
				this.fValue = _fValue;
				this.lastSendTime = _lastSendTime;
			}

			// Token: 0x0400A36D RID: 41837
			public readonly int iValue;

			// Token: 0x0400A36E RID: 41838
			public readonly float fValue;

			// Token: 0x0400A36F RID: 41839
			public readonly float lastSendTime;
		}
	}
}
