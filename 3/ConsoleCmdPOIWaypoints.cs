using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000264 RID: 612
[Preserve]
public class ConsoleCmdPOIWaypoints : ConsoleCmdAbstract
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x0600122A RID: 4650 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x0600122B RID: 4651 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x0600122C RID: 4652 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x00071BC6 File Offset: 0x0006FDC6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"poiwaypoints",
			"pwp"
		};
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x00071BDE File Offset: 0x0006FDDE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Adds waypoints for specified POIs.";
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00071BE5 File Offset: 0x0006FDE5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return this.getDescription() + "\n\npwp * - adds waypoints to all POIs in the world.\npwp <name> - adds waypoints to all POIs that starts with the name.\npwp <distance> - adds waypoints to all POIs with the specified distance.\npwp * <distance> - adds waypoints to all POIs within the specified distance.\npwp <name> <distance> - adds waypoints to all POIs within the specified distance that start with the name.\npwp -clear - removes all POI waypoints.";
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00071BF8 File Offset: 0x0006FDF8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		if (_params.Count != 1)
		{
			if (_params.Count == 2)
			{
				float distance;
				if (float.TryParse(_params[1], out distance))
				{
					if (_params[0] == "*")
					{
						this.CreateWaypoints("", distance);
						return;
					}
					this.CreateWaypoints(_params[0], distance);
					return;
				}
				else
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid distance.");
				}
			}
			return;
		}
		if (_params[0] == "-clear")
		{
			POIWaypoint.ClearAll(GameManager.Instance.World.GetPrimaryPlayer());
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("POI Waypoints have been cleared.");
			return;
		}
		if (_params[0] == "*")
		{
			this.CreateWaypoints("", 0f);
			return;
		}
		float distance2;
		if (float.TryParse(_params[0], out distance2))
		{
			this.CreateWaypoints("", distance2);
			return;
		}
		this.CreateWaypoints(_params[0], 0f);
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x00071D20 File Offset: 0x0006FF20
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateWaypoints(string filterName, float distance)
	{
		GameManager instance = GameManager.Instance;
		int num = 0;
		if (instance != null && instance.GetDynamicPrefabDecorator() != null)
		{
			List<PrefabInstance> list = new List<PrefabInstance>();
			instance.GetDynamicPrefabDecorator().GetWorldPrefabs(list);
			if (list != null)
			{
				float num2 = distance * distance;
				EntityPlayer primaryPlayer = instance.World.GetPrimaryPlayer();
				foreach (PrefabInstance prefabInstance in list)
				{
					if ((distance == 0f || (primaryPlayer.position - prefabInstance.boundingBoxPosition).sqrMagnitude < num2) && prefabInstance.boundingBoxSize.Volume() >= 100 && prefabInstance.name.StartsWith(filterName) && POIWaypoint.TrySet(primaryPlayer, prefabInstance.id, true))
					{
						num++;
					}
				}
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Added {0} POI waypoints.", new object[]
		{
			num
		});
	}

	// Token: 0x04000D23 RID: 3363
	[PublicizedFrom(EAccessModifier.Private)]
	public const int SmallPoiVolumeLimit = 100;
}
