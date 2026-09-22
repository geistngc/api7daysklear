using System;

namespace XMLData.Parsers
{
	// Token: 0x02001656 RID: 5718
	public static class EnumParser
	{
		// Token: 0x0600B3ED RID: 46061 RVA: 0x003E29FB File Offset: 0x003E0BFB
		public static TEnum Parse<TEnum>(string _value) where TEnum : struct, IConvertible
		{
			return EnumUtils.Parse<TEnum>(_value, false);
		}

		// Token: 0x0600B3EE RID: 46062 RVA: 0x00436FDB File Offset: 0x004351DB
		public static string Unparse<TEnum>(TEnum _value) where TEnum : struct, IConvertible
		{
			return _value.ToStringCached<TEnum>();
		}
	}
}
