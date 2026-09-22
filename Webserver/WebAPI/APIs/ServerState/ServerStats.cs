using System;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver.LiveData;

namespace Webserver.WebAPI.APIs.ServerState
{
	// Token: 0x02001AE4 RID: 6884
	[Preserve]
	public class ServerStats : AbsRestApi
	{
		// Token: 0x0600CF20 RID: 53024 RVA: 0x004B91D4 File Offset: 0x004B73D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(ServerStats.jsonKeyGameTime);
			ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int item3 = valueTuple.Item3;
			JsonCommons.WriteGameTimeObject(ref jsonWriter, item, item2, item3);
			jsonWriter.WriteRaw(ServerStats.jsonKeyPlayers);
			jsonWriter.WriteInt32(GameManager.Instance.World.Players.Count);
			jsonWriter.WriteRaw(ServerStats.jsonKeyHostiles);
			jsonWriter.WriteInt32(Hostiles.Instance.GetCount());
			jsonWriter.WriteRaw(ServerStats.jsonKeyAnimals);
			jsonWriter.WriteInt32(Animals.Instance.GetCount());
			jsonWriter.WriteEndObject();
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CF21 RID: 53025 RVA: 0x004B7F39 File Offset: 0x004B6139
		public override int DefaultPermissionLevel()
		{
			return 2000;
		}

		// Token: 0x0600CF22 RID: 53026 RVA: 0x004B25BC File Offset: 0x004B07BC
		public ServerStats() : base(null)
		{
		}

		// Token: 0x04009D7D RID: 40317
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyGameTime = JsonWriter.GetEncodedPropertyNameWithBeginObject("gameTime");

		// Token: 0x04009D7E RID: 40318
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyPlayers = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("players");

		// Token: 0x04009D7F RID: 40319
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyHostiles = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hostiles");

		// Token: 0x04009D80 RID: 40320
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyAnimals = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("animals");
	}
}
