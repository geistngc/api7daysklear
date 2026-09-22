using System;

namespace Platform
{
	// Token: 0x02001BF0 RID: 7152
	public static class EUserBlockStateExtensions
	{
		// Token: 0x0600D4AC RID: 54444 RVA: 0x004CE964 File Offset: 0x004CCB64
		public static bool IsBlocked(this EUserBlockState blockState)
		{
			bool result;
			if (blockState != EUserBlockState.NotBlocked)
			{
				if (blockState - EUserBlockState.InGame > 1)
				{
					throw new ArgumentOutOfRangeException("blockState", blockState, string.Format("{0} not implemented for {1}.{2}", "IsBlocked", "EUserBlockState", blockState));
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
