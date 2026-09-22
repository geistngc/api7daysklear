using System;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x02001653 RID: 5715
	public static class boolParser
	{
		// Token: 0x0600B3E6 RID: 46054 RVA: 0x00436DAB File Offset: 0x00434FAB
		public static bool Parse(string _value)
		{
			if (_value == "true")
			{
				return true;
			}
			if (_value == "false")
			{
				return false;
			}
			throw new InvalidValueException("Expected bool value, found \"" + _value + "\"", -1);
		}

		// Token: 0x0600B3E7 RID: 46055 RVA: 0x00436DE1 File Offset: 0x00434FE1
		public static string Unparse(bool _value)
		{
			if (!_value)
			{
				return "false";
			}
			return "true";
		}
	}
}
