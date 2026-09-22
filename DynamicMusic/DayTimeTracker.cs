using System;
using System.Collections.Generic;
using DynamicMusic.Factories;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001A4F RID: 6735
	public class DayTimeTracker : AbstractDayTimeTracker, INotifiableFilter<MusicActionType, SectionType>, INotifiable<MusicActionType>, IFilter<SectionType>
	{
		// Token: 0x0600CC26 RID: 52262 RVA: 0x004AB38C File Offset: 0x004A958C
		public DayTimeTracker()
		{
			this.world = GameManager.Instance.World;
			this.conductor = this.world.dmsConductor;
			ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int @int = GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
			this.duskTime = (float)item / 24f * (float)@int;
			this.dawnTime = (float)item2 / 24f * (float)@int;
			this.currentDay = this.GetCurrentDay();
			this.MusicTimeTracker = Factory.CreateMusicTimeTracker();
		}

		// Token: 0x0600CC27 RID: 52263 RVA: 0x004AB41A File Offset: 0x004A961A
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (this.currentDay != this.GetCurrentDay())
			{
				this.UpdateDay();
			}
			this.currentTime = this.GetCurrentTime();
			this.UpdateDayPeriod();
		}

		// Token: 0x0600CC28 RID: 52264 RVA: 0x004AB442 File Offset: 0x004A9642
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateDay()
		{
			this.currentDay = this.GetCurrentDay();
			this.MusicTimeTracker.Notify();
		}

		// Token: 0x0600CC29 RID: 52265 RVA: 0x004AB45B File Offset: 0x004A965B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override int GetCurrentDay()
		{
			return GameUtils.WorldTimeToDays(this.world.worldTime);
		}

		// Token: 0x0600CC2A RID: 52266 RVA: 0x004AB46D File Offset: 0x004A966D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float GetCurrentTime()
		{
			return SkyManager.GetTimeOfDayAsMinutes();
		}

		// Token: 0x0600CC2B RID: 52267 RVA: 0x004AB474 File Offset: 0x004A9674
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateDayPeriod()
		{
			if (this.currentTime < this.dawnTime - 0.33333334f)
			{
				this.dayPeriod = AbstractDayTimeTracker.DayPeriodType.Morning;
				return;
			}
			if (this.currentTime <= this.dawnTime + 0.33333334f)
			{
				this.dayPeriod = AbstractDayTimeTracker.DayPeriodType.Dusk;
				return;
			}
			if (this.currentTime < this.duskTime - 0.33333334f)
			{
				this.dayPeriod = AbstractDayTimeTracker.DayPeriodType.Day;
				return;
			}
			if (this.currentTime <= this.duskTime + 0.33333334f)
			{
				this.dayPeriod = AbstractDayTimeTracker.DayPeriodType.Dusk;
				return;
			}
			this.dayPeriod = AbstractDayTimeTracker.DayPeriodType.Night;
		}

		// Token: 0x0600CC2C RID: 52268 RVA: 0x004AB4F8 File Offset: 0x004A96F8
		public override string ToString()
		{
			return string.Format("Current Day: {0}\nCurrent part of the day: {1}\nCurrent Time: {2}\nDawn time: {3}\nDusk Time: {4}\n", new object[]
			{
				this.currentDay,
				this.dayPeriod.ToStringCached<AbstractDayTimeTracker.DayPeriodType>(),
				this.currentTime,
				this.dawnTime,
				this.duskTime
			});
		}

		// Token: 0x0600CC2D RID: 52269 RVA: 0x004AB55C File Offset: 0x004A975C
		public override List<SectionType> Filter(List<SectionType> _sectionTypes)
		{
			this.Update();
			GameStats.GetInt(EnumGameStats.BloodMoonDay);
			GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
			if (GameUtils.IsBloodMoonTime(this.world.worldTime, GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength)), GameStats.GetInt(EnumGameStats.BloodMoonDay)))
			{
				_sectionTypes.Clear();
				_sectionTypes.Add(this.conductor.IsBloodmoonMusicEligible ? SectionType.Bloodmoon : SectionType.None);
				return _sectionTypes;
			}
			if (this.dayPeriod.Equals(AbstractDayTimeTracker.DayPeriodType.Dawn) || this.dayPeriod.Equals(AbstractDayTimeTracker.DayPeriodType.Dusk))
			{
				_sectionTypes.Remove(SectionType.Exploration);
				_sectionTypes.Remove(SectionType.HomeDay);
				_sectionTypes.Remove(SectionType.HomeNight);
				_sectionTypes.Remove(SectionType.Suspense);
			}
			else if (!this.dayPeriod.Equals(AbstractDayTimeTracker.DayPeriodType.Day))
			{
				_sectionTypes.Remove(SectionType.Exploration);
				_sectionTypes.Remove(SectionType.HomeDay);
			}
			else
			{
				_sectionTypes.Remove(SectionType.HomeNight);
			}
			return this.MusicTimeTracker.Filter(_sectionTypes);
		}

		// Token: 0x0600CC2E RID: 52270 RVA: 0x004AB65C File Offset: 0x004A985C
		public void Notify(MusicActionType _state)
		{
			this.MusicTimeTracker.Notify(_state);
		}

		// Token: 0x04009B66 RID: 39782
		[PublicizedFrom(EAccessModifier.Private)]
		public const float duskDawnWindowRadius = 0.33333334f;

		// Token: 0x04009B67 RID: 39783
		[PublicizedFrom(EAccessModifier.Private)]
		public World world;

		// Token: 0x04009B68 RID: 39784
		[PublicizedFrom(EAccessModifier.Private)]
		public Conductor conductor;

		// Token: 0x04009B69 RID: 39785
		[PublicizedFrom(EAccessModifier.Private)]
		public IMultiNotifiableFilter MusicTimeTracker;
	}
}
