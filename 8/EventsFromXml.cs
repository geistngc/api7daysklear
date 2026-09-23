using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x02000D0B RID: 3339
public static class EventsFromXml
{
	// Token: 0x17000AB1 RID: 2737
	// (get) Token: 0x060065F7 RID: 26103 RVA: 0x0027F328 File Offset: 0x0027D528
	public static DateTime Now
	{
		get
		{
			if (!(EventsFromXml.ForceTestDateTime != DateTime.MinValue))
			{
				return DateTime.Now;
			}
			return EventsFromXml.ForceTestDateTime;
		}
	}

	// Token: 0x060065F8 RID: 26104 RVA: 0x0027F346 File Offset: 0x0027D546
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (root == null)
		{
			yield break;
		}
		using (IEnumerator<XElement> enumerator = root.Elements("event").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement element = enumerator.Current;
				EventsFromXml.parseEvent(element);
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060065F9 RID: 26105 RVA: 0x0027F358 File Offset: 0x0027D558
	[PublicizedFrom(EAccessModifier.Private)]
	public static void parseEvent(XElement _element)
	{
		if (!_element.HasAttribute("name"))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'name' missing");
		}
		string attribute = _element.GetAttribute("name");
		DateTime minValue = DateTime.MinValue;
		int num = int.MinValue;
		int minValue2 = int.MinValue;
		DateTime dateTime = DateTime.MinValue;
		int minValue3 = int.MinValue;
		DateTime dateTime2 = DateTime.MinValue;
		string dateString;
		if (_element.TryGetAttribute("base_date", out dateString) && !EventsFromXml.TryParseDate(dateString, out minValue))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'base_date' has invalid format");
		}
		string s;
		if (_element.TryGetAttribute("start_offset", out s) && !int.TryParse(s, out num))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'start_offset' is not a valid integer number");
		}
		string s2;
		if (_element.TryGetAttribute("end_offset", out s2) && !int.TryParse(s2, out minValue2))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'end_offset' is not a valid integer number");
		}
		string dateString2;
		if (_element.TryGetAttribute("start_date", out dateString2) && !EventsFromXml.TryParseDate(dateString2, out dateTime))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'start_date' has invalid format");
		}
		string dateString3;
		if (_element.TryGetAttribute("end_date", out dateString3) && !EventsFromXml.TryParseDate(dateString3, out dateTime2))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'end_date' has invalid format");
		}
		string s3;
		if (_element.TryGetAttribute("duration", out s3) && (!int.TryParse(s3, out minValue3) || minValue3 < 1))
		{
			throw new XmlLoadException("events.xml", _element, "Attribute 'duration' is not a valid integer number or not greater/equal 1");
		}
		if (num > -2147483648 && minValue == DateTime.MinValue)
		{
			throw new XmlLoadException("events.xml", _element, "Event has 'start_offset' but no 'base_date'");
		}
		if (minValue2 > -2147483648 && minValue == DateTime.MinValue)
		{
			throw new XmlLoadException("events.xml", _element, "Event has 'end_offset' but no 'base_date'");
		}
		if (num > -2147483648 && dateTime != DateTime.MinValue)
		{
			throw new XmlLoadException("events.xml", _element, "Event has both 'start_offset' and 'start_date'");
		}
		if (minValue2 > -2147483648 && dateTime2 != DateTime.MinValue)
		{
			throw new XmlLoadException("events.xml", _element, "Event has both 'end_offset' and 'end_date'");
		}
		if (minValue2 > -2147483648 && minValue3 > -2147483648)
		{
			throw new XmlLoadException("events.xml", _element, "Event has both 'end_offset' and 'duration'");
		}
		if (minValue3 > -2147483648 && dateTime2 != DateTime.MinValue)
		{
			throw new XmlLoadException("events.xml", _element, "Event has both 'duration' and 'end_date'");
		}
		if (dateTime == DateTime.MinValue)
		{
			if (minValue == DateTime.MinValue)
			{
				throw new XmlLoadException("events.xml", _element, "Event has neither 'base_date' nor 'start_date'");
			}
			if (num == -2147483648)
			{
				num = 0;
			}
			dateTime = minValue.AddDays((double)num);
		}
		if (dateTime2 == DateTime.MinValue)
		{
			if (minValue3 == -2147483648 && minValue2 == -2147483648)
			{
				throw new XmlLoadException("events.xml", _element, "Event has neither 'end_offset' nor 'duration' nor 'end_date'");
			}
			if (minValue2 > -2147483648)
			{
				dateTime2 = minValue.AddDays((double)minValue2);
			}
			if (minValue3 > -2147483648)
			{
				dateTime2 = dateTime.AddDays((double)minValue3);
			}
		}
		DateTime now = EventsFromXml.Now;
		if (dateTime2 < dateTime)
		{
			dateTime2 = dateTime2.AddYears(1);
		}
		if (dateTime2.Year > now.Year)
		{
			dateTime = dateTime.AddYears(-1);
			dateTime2 = dateTime2.AddYears(-1);
		}
		if (dateTime < now && dateTime2 < now)
		{
			dateTime = dateTime.AddYears(1);
			dateTime2 = dateTime2.AddYears(1);
		}
		EventsFromXml.EventDefinition value = new EventsFromXml.EventDefinition(attribute, dateTime, dateTime2);
		if (!EventsFromXml.Events.TryAdd(attribute, value))
		{
			Log.Error("Event with the same name '" + attribute + "' already defined");
		}
	}

	// Token: 0x060065FA RID: 26106 RVA: 0x0027F6FC File Offset: 0x0027D8FC
	public static bool TryParseDate(string _dateString, out DateTime _date)
	{
		_dateString = _dateString.Trim();
		EventsFromXml.SpecialDateDelegate specialDateDelegate;
		if (EventsFromXml.specialDateHandlers.TryGetValue(_dateString, out specialDateDelegate))
		{
			_date = specialDateDelegate(_dateString);
			return true;
		}
		if (!DateTime.TryParseExact(_dateString, "MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date))
		{
			return false;
		}
		_date = _date.ChangeYear(DateTime.Now.Year);
		return true;
	}

	// Token: 0x060065FB RID: 26107 RVA: 0x0027F764 File Offset: 0x0027D964
	public static DateTime ChangeYear(this DateTime _dt, int _newYear)
	{
		return _dt.AddYears(_newYear - _dt.Year);
	}

	// Token: 0x060065FC RID: 26108 RVA: 0x0027F776 File Offset: 0x0027D976
	public static void Cleanup()
	{
		EventsFromXml.Events.Clear();
	}

	// Token: 0x060065FD RID: 26109 RVA: 0x0027F784 File Offset: 0x0027D984
	[PublicizedFrom(EAccessModifier.Private)]
	static EventsFromXml()
	{
		EventsFromXml.specialDateHandlers["easter"] = new EventsFromXml.SpecialDateDelegate(EventsFromXml.EasterDate);
		EventsFromXml.specialDateHandlers["advent"] = new EventsFromXml.SpecialDateDelegate(EventsFromXml.FirstSundayOfAdvent);
		EventsFromXml.specialDateHandlers["thanksgiving"] = new EventsFromXml.SpecialDateDelegate(EventsFromXml.ThanksgivingDate);
	}

	// Token: 0x060065FE RID: 26110 RVA: 0x0027F800 File Offset: 0x0027DA00
	public static DateTime EasterDate(string _origString)
	{
		return EventsFromXml.EasterSunday(DateTime.Now.Year);
	}

	// Token: 0x060065FF RID: 26111 RVA: 0x0027F820 File Offset: 0x0027DA20
	public static DateTime EasterSunday(int _year)
	{
		int month;
		int day;
		EventsFromXml.EasterSunday(_year, out month, out day);
		return new DateTime(_year, month, day);
	}

	// Token: 0x06006600 RID: 26112 RVA: 0x0027F840 File Offset: 0x0027DA40
	public static void EasterSunday(int _year, out int _month, out int _day)
	{
		int num = _year % 19;
		int num2 = _year / 100;
		int num3 = (num2 - num2 / 4 - (8 * num2 + 13) / 25 + 19 * num + 15) % 30;
		int num4 = num3 - num3 / 28 * (1 - num3 / 28 * (29 / (num3 + 1)) * ((21 - num) / 11));
		_day = num4 - (_year + _year / 4 + num4 + 2 - num2 + num2 / 4) % 7 + 28;
		_month = 3;
		if (_day > 31)
		{
			_month++;
			_day -= 31;
		}
	}

	// Token: 0x06006601 RID: 26113 RVA: 0x0027F8C0 File Offset: 0x0027DAC0
	public static DateTime FirstSundayOfAdvent(string _origString)
	{
		return EventsFromXml.FirstSundayOfAdvent(DateTime.Now.Year);
	}

	// Token: 0x06006602 RID: 26114 RVA: 0x0027F8E0 File Offset: 0x0027DAE0
	public static DateTime FirstSundayOfAdvent(int _year)
	{
		int num = 4;
		int num2 = 0;
		DateTime dateTime = new DateTime(_year, 12, 25);
		if (dateTime.DayOfWeek != DayOfWeek.Sunday)
		{
			num--;
			num2 = (int)dateTime.DayOfWeek;
		}
		return dateTime.AddDays((double)(-1 * (num * 7 + num2)));
	}

	// Token: 0x06006603 RID: 26115 RVA: 0x0027F924 File Offset: 0x0027DB24
	public static DateTime ThanksgivingDate(string _origString)
	{
		DateTime result = default(DateTime);
		for (int i = 22; i <= 28; i++)
		{
			result = new DateTime(DateTime.Now.Year, 11, i);
			if (result.DayOfWeek == DayOfWeek.Thursday)
			{
				break;
			}
		}
		return result;
	}

	// Token: 0x04004F18 RID: 20248
	[PublicizedFrom(EAccessModifier.Private)]
	public const string XMLName = "events.xml";

	// Token: 0x04004F19 RID: 20249
	public static DateTime ForceTestDateTime = DateTime.MinValue;

	// Token: 0x04004F1A RID: 20250
	public static readonly Dictionary<string, EventsFromXml.EventDefinition> Events = new CaseInsensitiveStringDictionary<EventsFromXml.EventDefinition>();

	// Token: 0x04004F1B RID: 20251
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, EventsFromXml.SpecialDateDelegate> specialDateHandlers = new CaseInsensitiveStringDictionary<EventsFromXml.SpecialDateDelegate>();

	// Token: 0x02000D0C RID: 3340
	public readonly struct EventDefinition
	{
		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06006604 RID: 26116 RVA: 0x0027F96C File Offset: 0x0027DB6C
		public bool Active
		{
			get
			{
				DateTime now = EventsFromXml.Now;
				return now >= this.Start && now < this.End;
			}
		}

		// Token: 0x06006605 RID: 26117 RVA: 0x0027F99B File Offset: 0x0027DB9B
		public EventDefinition(string _name, DateTime _start, DateTime _end)
		{
			this.Name = _name;
			this.Start = _start;
			this.End = _end;
		}

		// Token: 0x04004F1C RID: 20252
		public readonly string Name;

		// Token: 0x04004F1D RID: 20253
		public readonly DateTime Start;

		// Token: 0x04004F1E RID: 20254
		public readonly DateTime End;
	}

	// Token: 0x02000D0D RID: 3341
	// (Invoke) Token: 0x06006607 RID: 26119
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate DateTime SpecialDateDelegate(string _origString);
}
