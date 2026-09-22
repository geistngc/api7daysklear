using System;

namespace Platform.PS5
{
	// Token: 0x02001CBF RID: 7359
	public class UserDataRoamingPS5 : UserDataRoamingAbs
	{
		// Token: 0x17001B23 RID: 6947
		// (get) Token: 0x0600DA55 RID: 55893 RVA: 0x0002F184 File Offset: 0x0002D384
		public override SaveRoamingMode SaveRoamingMode
		{
			get
			{
				return SaveRoamingMode.Forced;
			}
		}

		// Token: 0x17001B24 RID: 6948
		// (get) Token: 0x0600DA56 RID: 55894 RVA: 0x0002003D File Offset: 0x0001E23D
		public override UserDataStorageType DefaultSaveStorage
		{
			get
			{
				return UserDataStorageType.Roaming;
			}
		}
	}
}
