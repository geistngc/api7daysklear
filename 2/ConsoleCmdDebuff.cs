using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000205 RID: 517
[Preserve]
public class ConsoleCmdDebuff : ConsoleCmdAbstract
{
	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000FCB RID: 4043 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000FCC RID: 4044 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0006629B File Offset: 0x0006449B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debuff"
		};
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000662AC File Offset: 0x000644AC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on clients, use \"debuffplayer\" instead for other players / remote clients");
			return;
		}
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (!(primaryPlayer != null))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No local player found.");
			return;
		}
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("debuff requires a buff name as the only argument!");
			ConsoleCmdDebuff.PrintActiveBuffNames(primaryPlayer);
			return;
		}
		if (primaryPlayer.Buffs.GetBuff(_params[0]) == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Debuff failed: buff \"" + _params[0] + "\" unknown or not active");
			ConsoleCmdDebuff.PrintActiveBuffNames(primaryPlayer);
			return;
		}
		primaryPlayer.Buffs.RemoveBuff(_params[0], -1, true);
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00066368 File Offset: 0x00064568
	public static void PrintActiveBuffNames(EntityPlayer _player)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Active buffs:");
		foreach (BuffValue buffValue in _player.Buffs.ActiveBuffs)
		{
			if (buffValue != null && buffValue.BuffClass != null)
			{
				BuffClass buffClass = buffValue.BuffClass;
				if (buffClass.Name.Equals(buffClass.LocalizedName))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" - " + buffClass.Name);
				}
				else
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
					{
						" - ",
						buffClass.Name,
						" (",
						buffClass.LocalizedName,
						")"
					}));
				}
			}
		}
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00066450 File Offset: 0x00064650
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Removes a buff from the local player";
	}
}
