using System;
using System.Net;
using UnityEngine.Profiling;
using Utf8Json;

namespace Webserver.WebAPI.APIs.ServerState
{
	// Token: 0x02001ADF RID: 6879
	public abstract class KeyValueListAbs : AbsRestApi
	{
		// Token: 0x0600CF07 RID: 52999 RVA: 0x004B8AA8 File Offset: 0x004B6CA8
		[PublicizedFrom(EAccessModifier.Protected)]
		public KeyValueListAbs(string _listName) : base(null)
		{
			this.buildSampler = CustomSampler.Create("JSON_" + _listName + "_BuildSampler", false);
		}

		// Token: 0x0600CF08 RID: 53000 RVA: 0x004B8AD0 File Offset: 0x004B6CD0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRestGet(RequestContext _context)
		{
			JsonWriter jsonWriter;
			AbsRestApi.PrepareEnvelopedResult(out jsonWriter);
			jsonWriter.EnsureCapacity(this.largestBuffer);
			jsonWriter.WriteBeginArray();
			bool flag = true;
			this.iterateList(ref jsonWriter, ref flag);
			jsonWriter.WriteEndArray();
			int num = jsonWriter.CurrentOffset + 128;
			if (num > this.largestBuffer)
			{
				this.largestBuffer = num;
			}
			AbsRestApi.SendEnvelopedResult(_context, ref jsonWriter, HttpStatusCode.OK, null, null, null);
		}

		// Token: 0x0600CF09 RID: 53001 RVA: 0x004B8B39 File Offset: 0x004B6D39
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeKeyType(ref JsonWriter _writer, ref bool _first, string _key, string _type)
		{
			if (!_first)
			{
				_writer.WriteValueSeparator();
			}
			_first = false;
			_writer.WriteRaw(KeyValueListAbs.keyName);
			_writer.WriteString(_key);
			_writer.WriteRaw(KeyValueListAbs.keyType);
			_writer.WriteString(_type);
		}

		// Token: 0x0600CF0A RID: 53002 RVA: 0x004B8B6D File Offset: 0x004B6D6D
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeInt(ref JsonWriter _writer, ref bool _first, string _key, int _value)
		{
			this.writeKeyType(ref _writer, ref _first, _key, "int");
			_writer.WriteRaw(KeyValueListAbs.keyValue);
			_writer.WriteInt32(_value);
		}

		// Token: 0x0600CF0B RID: 53003 RVA: 0x004B8B90 File Offset: 0x004B6D90
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, int _value)
		{
			this.writeInt(ref _writer, ref _first, _key, _value);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF0C RID: 53004 RVA: 0x004B8BA3 File Offset: 0x004B6DA3
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, int _value, int? _default)
		{
			this.writeInt(ref _writer, ref _first, _key, _value);
			_writer.WriteRaw(KeyValueListAbs.keyDefault);
			if (_default != null)
			{
				_writer.WriteInt32(_default.Value);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF0D RID: 53005 RVA: 0x004B8BDF File Offset: 0x004B6DDF
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeFloat(ref JsonWriter _writer, ref bool _first, string _key, float _value)
		{
			this.writeKeyType(ref _writer, ref _first, _key, "float");
			_writer.WriteRaw(KeyValueListAbs.keyValue);
			_writer.WriteSingle(_value);
		}

		// Token: 0x0600CF0E RID: 53006 RVA: 0x004B8C02 File Offset: 0x004B6E02
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, float _value)
		{
			this.writeFloat(ref _writer, ref _first, _key, _value);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF0F RID: 53007 RVA: 0x004B8C15 File Offset: 0x004B6E15
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, float _value, float? _default)
		{
			this.writeFloat(ref _writer, ref _first, _key, _value);
			_writer.WriteRaw(KeyValueListAbs.keyDefault);
			if (_default != null)
			{
				_writer.WriteSingle(_default.Value);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF10 RID: 53008 RVA: 0x004B8C51 File Offset: 0x004B6E51
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeBool(ref JsonWriter _writer, ref bool _first, string _key, bool _value)
		{
			this.writeKeyType(ref _writer, ref _first, _key, "bool");
			_writer.WriteRaw(KeyValueListAbs.keyValue);
			_writer.WriteBoolean(_value);
		}

		// Token: 0x0600CF11 RID: 53009 RVA: 0x004B8C74 File Offset: 0x004B6E74
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, bool _value)
		{
			this.writeBool(ref _writer, ref _first, _key, _value);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF12 RID: 53010 RVA: 0x004B8C87 File Offset: 0x004B6E87
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, bool _value, bool? _default)
		{
			this.writeBool(ref _writer, ref _first, _key, _value);
			_writer.WriteRaw(KeyValueListAbs.keyDefault);
			if (_default != null)
			{
				_writer.WriteBoolean(_default.Value);
			}
			else
			{
				_writer.WriteNull();
			}
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF13 RID: 53011 RVA: 0x004B8CC3 File Offset: 0x004B6EC3
		[PublicizedFrom(EAccessModifier.Private)]
		public void writeString(ref JsonWriter _writer, ref bool _first, string _key, string _value)
		{
			this.writeKeyType(ref _writer, ref _first, _key, "string");
			_writer.WriteRaw(KeyValueListAbs.keyValue);
			_writer.WriteString(_value);
		}

		// Token: 0x0600CF14 RID: 53012 RVA: 0x004B8CE6 File Offset: 0x004B6EE6
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, string _value)
		{
			this.writeString(ref _writer, ref _first, _key, _value);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF15 RID: 53013 RVA: 0x004B8CF9 File Offset: 0x004B6EF9
		[PublicizedFrom(EAccessModifier.Protected)]
		public void addItem(ref JsonWriter _writer, ref bool _first, string _key, string _value, string _default)
		{
			this.writeString(ref _writer, ref _first, _key, _value);
			_writer.WriteRaw(KeyValueListAbs.keyDefault);
			_writer.WriteString(_default);
			_writer.WriteEndObject();
		}

		// Token: 0x0600CF16 RID: 53014
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract void iterateList(ref JsonWriter _writer, ref bool _first);

		// Token: 0x04009D65 RID: 40293
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler buildSampler;

		// Token: 0x04009D66 RID: 40294
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] keyName = JsonWriter.GetEncodedPropertyNameWithBeginObject("name");

		// Token: 0x04009D67 RID: 40295
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] keyType = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("type");

		// Token: 0x04009D68 RID: 40296
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] keyValue = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("value");

		// Token: 0x04009D69 RID: 40297
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] keyDefault = JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("default");

		// Token: 0x04009D6A RID: 40298
		[PublicizedFrom(EAccessModifier.Private)]
		public int largestBuffer;
	}
}
