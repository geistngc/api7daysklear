using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000006 RID: 6
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	[PublicizedFrom(EAccessModifier.Internal)]
	public sealed class NullableAttribute : Attribute
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000021BE File Offset: 0x000003BE
		public NullableAttribute(byte A_1)
		{
			this.NullableFlags = new byte[]
			{
				A_1
			};
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021D6 File Offset: 0x000003D6
		public NullableAttribute(byte[] A_1)
		{
			this.NullableFlags = A_1;
		}

		// Token: 0x04000003 RID: 3
		public readonly byte[] NullableFlags;
	}
}
