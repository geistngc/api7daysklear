using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000007 RID: 7
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[PublicizedFrom(EAccessModifier.Internal)]
	public sealed class NullableContextAttribute : Attribute
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000021E5 File Offset: 0x000003E5
		public NullableContextAttribute(byte A_1)
		{
			this.Flag = A_1;
		}

		// Token: 0x04000004 RID: 4
		public readonly byte Flag;
	}
}
