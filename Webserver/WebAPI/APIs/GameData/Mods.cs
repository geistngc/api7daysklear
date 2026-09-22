using System;
using System.Net;
using UnityEngine.Scripting;
using Utf8Json;

namespace Webserver.WebAPI.APIs.GameData
{
	// Token: 0x02001AF0 RID: 6896
	[Preserve]
	public class Mods : AbsRestApi
	{
		// Token: 0x0600CF75 RID: 53109 RVA: 0x004BAE70 File Offset: 0x004B9070
		public Mods(Web _parent) : base(null)
		{
			JsonWriter jsonWriter = default(JsonWriter);
			jsonWriter.WriteBeginArray();
			for (int i = 0; i < _parent.WebMods.Count; i++)
			{
				WebMod webMod = _parent.WebMods[i];
				if (i > 0)
				{
					jsonWriter.WriteValueSeparator();
				}
				Mods.writeModJson(ref jsonWriter, webMod);
			}
			jsonWriter.WriteEndArray();
			this.loadedWebMods = jsonWriter.ToUtf8ByteArray();
		}

		// Token: 0x0600CF76 RID: 53110 RVA: 0x004BAEE0 File Offset: 0x004B90E0
		[PublicizedFrom(EAccessModifier.Private)]
		public static void writeModJson(ref JsonWriter _writer, WebMod _webMod)
		{
			_writer.WriteBeginObject();
			_writer.WritePropertyName("name");
			_writer.WriteString(_webMod.ParentMod.Name);
			_writer.WriteValueSeparator();
			_writer.WritePropertyName("displayName");
			_writer.WriteString(_webMod.ParentMod.DisplayName);
			_writer.WriteValueSeparator();
			_writer.WritePropertyName("description");
			_writer.WriteString(_webMod.ParentMod.Description);
			_writer.WriteValueSeparator();
			_writer.WritePropertyName("author");
			_writer.WriteString(_webMod.ParentMod.Author);
			_writer.WriteValueSeparator();
			_writer.WritePropertyName("version");
			_writer.WriteString(_webMod.ParentMod.VersionString);
			_writer.WriteValueSeparator();
			_writer.WritePropertyName("website");
			_writer.WriteString(_webMod.ParentMod.Website);
			Mods.writeWebModJson(ref _writer, _webMod);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF77 RID: 53111 RVA: 0x004BAFC8 File Offset: 0x004B91C8
		[PublicizedFrom(EAccessModifier.Private)]
		public static void writeWebModJson(ref JsonWriter _writer, WebMod _webMod)
		{
			if (_webMod.ModUrl != null)
			{
				_writer.WriteValueSeparator();
				_writer.WritePropertyName("web");
				_writer.WriteBeginObject();
				_writer.WritePropertyName("baseUrl");
				_writer.WriteString(_webMod.ModUrl);
				string reactBundle = _webMod.ReactBundle;
				if (reactBundle != null)
				{
					_writer.WriteValueSeparator();
					_writer.WritePropertyName("bundle");
					_writer.WriteString(reactBundle);
				}
				string cssPath = _webMod.CssPath;
				if (cssPath != null)
				{
					_writer.WriteValueSeparator();
					_writer.WritePropertyName("css");
					_writer.WriteString(cssPath);
				}
				_writer.WriteEndObject();
			}
		}

		// Token: 0x0600CF78 RID: 53112 RVA: 0x004BB058 File Offset: 0x004B9258
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.WriteRaw(this.loadedWebMods);
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CF79 RID: 53113 RVA: 0x004B7F39 File Offset: 0x004B6139
		public override int DefaultPermissionLevel()
		{
			return 2000;
		}

		// Token: 0x04009DC8 RID: 40392
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly byte[] loadedWebMods;
	}
}
