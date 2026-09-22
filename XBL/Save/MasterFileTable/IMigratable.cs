using System;

namespace Platform.XBL.Save.MasterFileTable
{
	// Token: 0x02001C67 RID: 7271
	public interface IMigratable
	{
		// Token: 0x17001AA9 RID: 6825
		// (get) Token: 0x0600D744 RID: 55108
		ushort Version { get; }

		// Token: 0x0600D745 RID: 55109
		void Write(PooledBinaryWriter writer);

		// Token: 0x0600D746 RID: 55110
		void Read(PooledBinaryReader reader);
	}
}
