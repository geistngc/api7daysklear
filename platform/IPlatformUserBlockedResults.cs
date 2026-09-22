using System;

namespace Platform
{
	// Token: 0x02001BB4 RID: 7092
	public interface IPlatformUserBlockedResults
	{
		// Token: 0x17001A23 RID: 6691
		// (get) Token: 0x0600D37F RID: 54143
		IPlatformUser User { get; }

		// Token: 0x0600D380 RID: 54144
		void Block(EBlockType blockType);

		// Token: 0x0600D381 RID: 54145 RVA: 0x004CB068 File Offset: 0x004C9268
		void BlockAll()
		{
			foreach (EBlockType blockType in EnumUtils.Values<EBlockType>())
			{
				this.Block(blockType);
			}
		}

		// Token: 0x0600D382 RID: 54146
		void Error();
	}
}
