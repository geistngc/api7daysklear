using System;

namespace XMLData.Formatters
{
	// Token: 0x0200165E RID: 5726
	public abstract class ValueFormatter<TValue>
	{
		// Token: 0x0600B3FD RID: 46077
		public abstract string FormatValue(TValue _value);

		// Token: 0x0600B3FE RID: 46078 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public ValueFormatter()
		{
		}
	}
}
