using System;
using Newtonsoft.Json;

namespace Services.Analytics
{
	// Token: 0x02001686 RID: 5766
	public class TruncateStringSerializerConverter : JsonConverter<string>
	{
		// Token: 0x0600B4A1 RID: 46241 RVA: 0x0043A484 File Offset: 0x00438684
		public TruncateStringSerializerConverter(int maxLength)
		{
			this._maxLength = maxLength;
		}

		// Token: 0x0600B4A2 RID: 46242 RVA: 0x0043A493 File Offset: 0x00438693
		public TruncateStringSerializerConverter()
		{
			this._maxLength = 50;
		}

		// Token: 0x0600B4A3 RID: 46243 RVA: 0x0043A4A3 File Offset: 0x004386A3
		public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
		{
			if (string.IsNullOrEmpty(value))
			{
				writer.WriteNull();
				return;
			}
			writer.WriteValue((value.Length > this._maxLength) ? value.Substring(0, this._maxLength) : value);
		}

		// Token: 0x0600B4A4 RID: 46244 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return null;
		}

		// Token: 0x040087C1 RID: 34753
		[PublicizedFrom(EAccessModifier.Private)]
		public const int DefaultMaxLength = 50;

		// Token: 0x040087C2 RID: 34754
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int _maxLength;
	}
}
