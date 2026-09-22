using System;

namespace XMLData.Parsers
{
	// Token: 0x02001658 RID: 5720
	public static class intParser
	{
		// Token: 0x0600B3F1 RID: 46065 RVA: 0x0043701B File Offset: 0x0043521B
		public static int Parse(string _value)
		{
			return int.Parse(_value);
		}

		// Token: 0x0600B3F2 RID: 46066 RVA: 0x003C1B9F File Offset: 0x003BFD9F
		public static string Unparse(int _value)
		{
			return _value.ToString();
		}
	}
}
