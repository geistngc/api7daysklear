using System;
using UnityEngine;

namespace XMLData.Parsers
{
	// Token: 0x0200165C RID: 5724
	public static class Vector3Parser
	{
		// Token: 0x0600B3F9 RID: 46073 RVA: 0x00437062 File Offset: 0x00435262
		public static Vector3 Parse(string _value)
		{
			return StringParsers.ParseVector3(_value, 0, -1);
		}

		// Token: 0x0600B3FA RID: 46074 RVA: 0x0043706C File Offset: 0x0043526C
		public static string Unparse(Vector3 _value)
		{
			return string.Format("{0},{1},{2}", _value.x.ToCultureInvariantString(), _value.y.ToCultureInvariantString(), _value.z.ToCultureInvariantString());
		}
	}
}
