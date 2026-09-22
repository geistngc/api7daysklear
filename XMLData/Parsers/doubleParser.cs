using System;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x02001655 RID: 5717
	public static class doubleParser
	{
		// Token: 0x0600B3EB RID: 46059 RVA: 0x00436FA4 File Offset: 0x004351A4
		public static double Parse(string _value)
		{
			double result;
			if (StringParsers.TryParseDouble(_value, out result))
			{
				return result;
			}
			throw new InvalidValueException("Expected double value, found \"" + _value + "\"", -1);
		}

		// Token: 0x0600B3EC RID: 46060 RVA: 0x00436FD3 File Offset: 0x004351D3
		public static string Unparse(double _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
