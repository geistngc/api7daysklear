using System;

namespace Webserver.Permissions
{
	// Token: 0x02001B08 RID: 6920
	public static class PermissionUtils
	{
		// Token: 0x0600D001 RID: 53249 RVA: 0x004BE4D4 File Offset: 0x004BC6D4
		public static bool CanViewAllPlayers(int _permissionLevel)
		{
			return AdminWebModules.Instance.ModuleAllowedWithLevel("webapi.viewallplayers", _permissionLevel);
		}

		// Token: 0x0600D002 RID: 53250 RVA: 0x004BE4E6 File Offset: 0x004BC6E6
		public static bool CanViewAllClaims(int _permissionLevel)
		{
			return AdminWebModules.Instance.ModuleAllowedWithLevel("webapi.viewallclaims", _permissionLevel);
		}
	}
}
