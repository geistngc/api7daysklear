using System;

namespace Platform.GameCore
{
	// Token: 0x02001C7D RID: 7293
	public class UserDataRoamingGameCore : UserDataRoamingAbs
	{
		// Token: 0x17001AD1 RID: 6865
		// (get) Token: 0x0600D840 RID: 55360 RVA: 0x0002F184 File Offset: 0x0002D384
		public override SaveRoamingMode SaveRoamingMode
		{
			get
			{
				return SaveRoamingMode.Forced;
			}
		}

		// Token: 0x17001AD2 RID: 6866
		// (get) Token: 0x0600D841 RID: 55361 RVA: 0x0002003D File Offset: 0x0001E23D
		public override UserDataStorageType DefaultSaveStorage
		{
			get
			{
				return UserDataStorageType.Roaming;
			}
		}
	}
}
