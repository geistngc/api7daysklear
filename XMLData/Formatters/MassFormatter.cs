using System;

namespace XMLData.Formatters
{
	// Token: 0x0200165D RID: 5725
	public class MassFormatter : ValueFormatter<float>
	{
		// Token: 0x0600B3FB RID: 46075 RVA: 0x00112609 File Offset: 0x00110809
		public override string FormatValue(float _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
