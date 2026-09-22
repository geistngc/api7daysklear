using System;
using UnityEngine;

namespace XMLData.Parsers
{
	// Token: 0x0200165B RID: 5723
	public static class Vector2Parser
	{
		// Token: 0x0600B3F7 RID: 46071 RVA: 0x00437038 File Offset: 0x00435238
		public static Vector2 Parse(string _value)
		{
			return StringParsers.ParseVector2(_value);
		}

		// Token: 0x0600B3F8 RID: 46072 RVA: 0x00437040 File Offset: 0x00435240
		public static string Unparse(Vector2 _value)
		{
			return string.Format("{0},{1}", _value.x.ToCultureInvariantString(), _value.y.ToCultureInvariantString());
		}
	}
}
