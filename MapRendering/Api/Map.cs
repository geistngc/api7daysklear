using System;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;
using Webserver;
using Webserver.WebAPI;

namespace MapRendering.Api
{
	// Token: 0x02001ABB RID: 6843
	[Preserve]
	public class Map : AbsRestApi
	{
		// Token: 0x0600CE5A RID: 52826 RVA: 0x004B24D0 File Offset: 0x004B06D0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			string requestPath = _context.RequestPath;
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			if (requestPath == "config")
			{
				this.writeConfig(ref jsonWriter);
				AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
				return;
			}
			AbsRestApi.SendEmptyResponse(_context, HttpStatusCode.NotImplemented, null, "INVALID_ID", null);
		}

		// Token: 0x0600CE5B RID: 52827 RVA: 0x004B2524 File Offset: 0x004B0724
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeConfig(ref JsonWriter writer)
		{
			writer.WriteRaw(Map.jsonKeyEnabled);
			writer.WriteBoolean(MapRenderer.Enabled);
			writer.WriteRaw(Map.jsonKeyMapBlockSize);
			writer.WriteInt32(Constants.MapBlockSize);
			writer.WriteRaw(Map.jsonKeyMaxZoom);
			writer.WriteInt32(Constants.Zoomlevels - 1);
			Vector3i other;
			Vector3i one;
			GameManager.Instance.World.GetWorldExtent(out other, out one);
			Vector3i position = one - other;
			writer.WriteRaw(Map.jsonKeyMapSize);
			JsonCommons.WriteVector3I(ref writer, position);
			writer.WriteEndObject();
		}

		// Token: 0x0600CE5C RID: 52828 RVA: 0x004B25A9 File Offset: 0x004B07A9
		public override int[] DefaultMethodPermissionLevels()
		{
			return new int[]
			{
				-2147483647,
				2000,
				int.MinValue,
				int.MinValue,
				int.MinValue
			};
		}

		// Token: 0x0600CE5D RID: 52829 RVA: 0x004B25BC File Offset: 0x004B07BC
		public Map() : base(null)
		{
		}

		// Token: 0x04009C8E RID: 40078
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyEnabled = JsonWriter.GetEncodedPropertyNameWithBeginObject("enabled");

		// Token: 0x04009C8F RID: 40079
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyMapBlockSize = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("mapBlockSize");

		// Token: 0x04009C90 RID: 40080
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyMaxZoom = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("maxZoom");

		// Token: 0x04009C91 RID: 40081
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] jsonKeyMapSize = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("mapSize");
	}
}
