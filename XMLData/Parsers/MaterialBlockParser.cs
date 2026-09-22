using System;

namespace XMLData.Parsers
{
	// Token: 0x02001659 RID: 5721
	public static class MaterialBlockParser
	{
		// Token: 0x0600B3F3 RID: 46067 RVA: 0x00437023 File Offset: 0x00435223
		public static MaterialBlock Parse(string _value)
		{
			return MaterialBlock.materials[_value];
		}

		// Token: 0x0600B3F4 RID: 46068 RVA: 0x00437030 File Offset: 0x00435230
		public static string Unparse(MaterialBlock _value)
		{
			return _value.id;
		}
	}
}
