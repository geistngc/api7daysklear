using System;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.WorldState
{
	// Token: 0x02001ADA RID: 6874
	[Preserve]
	public class Bloodmoon : AbsRestApi
	{
		// Token: 0x0600CEF8 RID: 52984 RVA: 0x004B80AC File Offset: 0x004B62AC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			ulong worldTime = GameManager.Instance.World.worldTime;
			ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(worldTime);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int item3 = valueTuple.Item3;
			int @int = GameStats.GetInt(EnumUtils.Parse<EnumGameStats>("BloodMoonDay", false));
			ValueTuple<int, int> valueTuple2 = GameUtils.CalcDuskDawnHours(GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("DayLightLength", false)));
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(Bloodmoon.jsonKeyGameTime);
			JsonCommons.WriteGameTimeObject(ref jsonWriter, item, item2, item3);
			jsonWriter.WriteRaw(Bloodmoon.jsonKeyBloodmoonActive);
			jsonWriter.WriteBoolean(GameUtils.IsBloodMoonTime(worldTime, valueTuple2, @int));
			jsonWriter.WriteRaw(Bloodmoon.jsonKeyNextBloodmoon);
			JsonCommons.WriteGameTimeObject(ref jsonWriter, @int, valueTuple2.Item1, 0);
			jsonWriter.WriteRaw(Bloodmoon.jsonKeyNextBloodmoonEnd);
			JsonCommons.WriteGameTimeObject(ref jsonWriter, @int + 1, valueTuple2.Item2, 0);
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CEF9 RID: 52985 RVA: 0x004B25BC File Offset: 0x004B07BC
		public Bloodmoon() : base(null)
		{
		}

		// Token: 0x04009D46 RID: 40262
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyGameTime = JsonWriter.GetEncodedPropertyNameWithBeginObject("gameTime");

		// Token: 0x04009D47 RID: 40263
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyBloodmoonActive = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("bloodmoonActive");

		// Token: 0x04009D48 RID: 40264
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyNextBloodmoon = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nextBloodmoon");

		// Token: 0x04009D49 RID: 40265
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyNextBloodmoonEnd = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nextBloodmoonEnd");
	}
}
