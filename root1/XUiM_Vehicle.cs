using System;
using Audio;

// Token: 0x02001142 RID: 4418
public class XUiM_Vehicle : XUiModel
{
	// Token: 0x1700101F RID: 4127
	// (get) Token: 0x06008BAC RID: 35756 RVA: 0x003532C2 File Offset: 0x003514C2
	// (set) Token: 0x06008BAD RID: 35757 RVA: 0x003532CA File Offset: 0x003514CA
	public EntityVehicle CurrentVehicle { get; set; }

	// Token: 0x06008BAE RID: 35758 RVA: 0x003532D3 File Offset: 0x003514D3
	public static string GetEntityName(XUi _xui)
	{
		if (!(_xui.Vehicle.CurrentVehicle != null))
		{
			return "";
		}
		return _xui.Vehicle.CurrentVehicle.EntityName;
	}

	// Token: 0x06008BAF RID: 35759 RVA: 0x003532FE File Offset: 0x003514FE
	public static float GetSpeed(XUi _xui)
	{
		if (!(_xui.Vehicle.CurrentVehicle != null))
		{
			return 0f;
		}
		return _xui.Vehicle.CurrentVehicle.GetVehicle().MaxPossibleSpeed;
	}

	// Token: 0x06008BB0 RID: 35760 RVA: 0x00353330 File Offset: 0x00351530
	public static string GetNoise(XUi _xui)
	{
		EntityVehicle currentVehicle = _xui.Vehicle.CurrentVehicle;
		if (currentVehicle == null)
		{
			return "";
		}
		float noise = currentVehicle.GetVehicle().GetNoise();
		string result;
		if (noise > 0.33f)
		{
			if (noise > 0.66f)
			{
				result = Localization.Get("xuiVehicleNoiseLoud", false, null);
			}
			else
			{
				result = Localization.Get("xuiVehicleNoiseModerate", false, null);
			}
		}
		else
		{
			result = Localization.Get("xuiVehicleNoiseSoft", false, null);
		}
		return result;
	}

	// Token: 0x06008BB1 RID: 35761 RVA: 0x003533A4 File Offset: 0x003515A4
	public static float GetProtection(XUi _xui)
	{
		EntityVehicle currentVehicle = _xui.Vehicle.CurrentVehicle;
		if (currentVehicle == null)
		{
			return 0f;
		}
		return (1f - currentVehicle.GetVehicle().GetPlayerDamagePercent()) * 100f;
	}

	// Token: 0x06008BB2 RID: 35762 RVA: 0x003533E3 File Offset: 0x003515E3
	public static float GetFuelLevel(XUi _xui)
	{
		if (!(_xui.Vehicle.CurrentVehicle != null))
		{
			return 0f;
		}
		return _xui.Vehicle.CurrentVehicle.GetVehicle().GetFuelPercent() * 100f;
	}

	// Token: 0x06008BB3 RID: 35763 RVA: 0x00353419 File Offset: 0x00351619
	public static float GetFuelFill(XUi _xui)
	{
		if (!(_xui.Vehicle.CurrentVehicle != null))
		{
			return 0f;
		}
		return _xui.Vehicle.CurrentVehicle.GetVehicle().GetFuelPercent();
	}

	// Token: 0x06008BB4 RID: 35764 RVA: 0x00353449 File Offset: 0x00351649
	public static int GetPassengers(XUi _xui)
	{
		if (!(_xui.Vehicle.CurrentVehicle != null))
		{
			return 1;
		}
		return _xui.Vehicle.CurrentVehicle.GetAttachMaxCount();
	}

	// Token: 0x06008BB5 RID: 35765 RVA: 0x00353470 File Offset: 0x00351670
	public static string GetSpeedText(XUi _xui)
	{
		float num = (_xui.Vehicle.CurrentVehicle != null) ? _xui.Vehicle.CurrentVehicle.GetVehicle().MaxPossibleSpeed : 0f;
		string result;
		if (num <= 9f)
		{
			if (num > 0f)
			{
				result = Localization.Get("xuiVehicleSpeedSlow", false, null);
			}
			else
			{
				result = Localization.Get("xuiVehicleSpeedNone", false, null);
			}
		}
		else if (num > 12f)
		{
			result = Localization.Get("xuiVehicleSpeedFast", false, null);
		}
		else
		{
			result = Localization.Get("xuiVehicleSpeedNormal", false, null);
		}
		return result;
	}

	// Token: 0x06008BB6 RID: 35766 RVA: 0x00353504 File Offset: 0x00351704
	public bool SetPart(XUi _xui, string _vehicleSlotName, ItemStack _stack, out ItemStack _resultStack)
	{
		Log.Warning("XUiM_Vehicle SetPart {0}", new object[]
		{
			_vehicleSlotName
		});
		this.CurrentVehicle == null;
		_resultStack = _stack;
		return false;
	}

	// Token: 0x06008BB7 RID: 35767 RVA: 0x000027FC File Offset: 0x000009FC
	public void RefreshVehicle()
	{
	}

	// Token: 0x06008BB8 RID: 35768 RVA: 0x0035352C File Offset: 0x0035172C
	public static bool RepairVehicle(XUi _xui, Vehicle _vehicle = null)
	{
		if (_vehicle == null)
		{
			_vehicle = _xui.Vehicle.CurrentVehicle.GetVehicle();
		}
		ItemValue item = ItemClass.GetItem("resourceRepairKit", false);
		if (item.ItemClass == null)
		{
			return false;
		}
		EntityPlayerLocal entityPlayer = _xui.playerUI.entityPlayer;
		LocalPlayerUI playerUI = _xui.playerUI;
		int itemCount = entityPlayer.bag.GetItemCount(item, -1, -1, true);
		int itemCount2 = entityPlayer.inventory.GetItemCount(item, false, -1, -1, true);
		int repairAmountNeeded = _vehicle.GetRepairAmountNeeded();
		if (itemCount + itemCount2 <= 0 || repairAmountNeeded <= 0)
		{
			if (repairAmountNeeded > itemCount + itemCount2)
			{
				Manager.PlayInsidePlayerHead("misc/missingitemtorepair", -1, 0f, false, false);
			}
			return false;
		}
		float num = 0f;
		ProgressionValue progressionValue = entityPlayer.Progression.GetProgressionValue("perkGreaseMonkey");
		if (progressionValue != null)
		{
			num += (float)progressionValue.Level * 0.1f;
		}
		_vehicle.RepairParts(1000, num);
		if (itemCount2 > 0)
		{
			entityPlayer.inventory.DecItem(item, 1, false, null);
		}
		else
		{
			entityPlayer.bag.DecItem(item, 1, false, null);
		}
		playerUI.xui.CollectedItemList.RemoveItemStack(new ItemStack(item, 1));
		Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
		return true;
	}

	// Token: 0x0400674C RID: 26444
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRepairBase = 1000;

	// Token: 0x0400674D RID: 26445
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRepairPercent = 0f;

	// Token: 0x0400674E RID: 26446
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRepairPerkPercent = 0.1f;
}
