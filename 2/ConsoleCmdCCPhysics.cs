using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001F1 RID: 497
[Preserve]
public class ConsoleCmdCCPhysics : ConsoleCmdAbstract
{
	// Token: 0x06000F55 RID: 3925 RVA: 0x00063CAC File Offset: 0x00061EAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ccphysics"
		};
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000F56 RID: 3926 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000F57 RID: 3927 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x00063CBC File Offset: 0x00061EBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enables or disables changes to CCPhysics layer interactions. Reloading the game session may be necessary to fully apply if changed.";
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000F5A RID: 3930 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000F5B RID: 3931 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x00063CC4 File Offset: 0x00061EC4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			ConsoleCmdCCPhysics.EnableCCPhysicsChanges = !ConsoleCmdCCPhysics.EnableCCPhysicsChanges;
		}
		else
		{
			string a = _params[0].ToLowerInvariant();
			if (!(a == "on"))
			{
				if (!(a == "off"))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unrecognised subcommand. Use 'on' or 'off' subcommand, or none to toggle.");
					return;
				}
				ConsoleCmdCCPhysics.EnableCCPhysicsChanges = false;
			}
			else
			{
				ConsoleCmdCCPhysics.EnableCCPhysicsChanges = true;
			}
		}
		Physics.IgnoreLayerCollision(20, 15, ConsoleCmdCCPhysics.EnableCCPhysicsChanges);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Physics layer interaction changes " + (ConsoleCmdCCPhysics.EnableCCPhysicsChanges ? "enabled" : "disabled") + ". Reloading the game session may be necessary to fully apply.");
	}

	// Token: 0x04000C90 RID: 3216
	public static bool ClientVerification = true;

	// Token: 0x04000C91 RID: 3217
	public static bool AllowSendToPeer = true;

	// Token: 0x04000C92 RID: 3218
	public static bool AllowReceiveFromPeer = true;

	// Token: 0x04000C93 RID: 3219
	public static bool DisablePeerAction = false;

	// Token: 0x04000C94 RID: 3220
	public static bool EnableCCPhysicsChanges = true;
}
