using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020002C5 RID: 709
[Preserve]
public class ConsoleCmdListDLC : ConsoleCmdAbstract
{
	// Token: 0x06001479 RID: 5241 RVA: 0x0007C81E File Offset: 0x0007AA1E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"listdlc",
			"dlcs"
		};
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600147A RID: 5242 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x0600147B RID: 5243 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x0007C836 File Offset: 0x0007AA36
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "List the available DLC and their current entitlement status.";
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x0007C83D File Offset: 0x0007AA3D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "List DLCs and their entitlement states.\nUsage:\n   listdlc\n   dlcs\n";
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x0007C844 File Offset: 0x0007AA44
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		ConsoleCmdListDLC.ExecuteList();
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0007C84C File Offset: 0x0007AA4C
	public static void ExecuteList()
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("DLC List:");
		foreach (object obj in Enum.GetValues(typeof(EntitlementSetEnum)))
		{
			EntitlementSetEnum entitlementSetEnum = (EntitlementSetEnum)obj;
			if (entitlementSetEnum != EntitlementSetEnum.None)
			{
				ValueTuple<bool, bool> valueTuple = EntitlementManager.Instance.CheckOverride(entitlementSetEnum);
				bool item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				bool flag = EntitlementManager.Instance.HasEntitlement(entitlementSetEnum);
				bool flag2 = EntitlementManager.Instance.IsAvailableOnPlatform(entitlementSetEnum);
				bool flag3 = EntitlementManager.Instance.IsEntitlementPurchasable(entitlementSetEnum);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0}: {1} => Entitlement State: {2}, Available on Platform: {3}, Purchasable: {4}, Override State: {5}", new object[]
				{
					(int)entitlementSetEnum,
					entitlementSetEnum,
					flag,
					flag2,
					flag3,
					item ? item2.ToString() : "Not Overridden"
				}));
			}
		}
	}
}
