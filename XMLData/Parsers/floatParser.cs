using System;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x02001657 RID: 5719
	public static class floatParser
	{
		// Token: 0x0600B3EF RID: 46063 RVA: 0x00436FE4 File Offset: 0x004351E4
		public static float Parse(string _value)
		{
			float result;
			if (StringParsers.TryParseFloat(_value, out result))
			{
				return result;
			}
			throw new InvalidValueException("Expected float value, found \"" + _value + "\"", -1);
		}

		// Token: 0x0600B3F0 RID: 46064 RVA: 0x00437013 File Offset: 0x00435213
		public static string Unparse(float _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
