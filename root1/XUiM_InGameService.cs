using System;
using System.Text;

// Token: 0x02001126 RID: 4390
public class XUiM_InGameService : XUiModel
{
	// Token: 0x06008AC0 RID: 35520 RVA: 0x0034D764 File Offset: 0x0034B964
	public static string GetServiceStats(XUi _xui, InGameService _service)
	{
		if (_service == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (_service.ServiceType == InGameService.InGameServiceTypes.VendingRent)
		{
			TraderInfo traderInfo = (_xui.Trader.Trader as TileEntityVendingMachine).TraderData.TraderInfo;
			stringBuilder.Append(XUiM_InGameService.StringFormatHandler(Localization.Get("xuiCost", false, null), traderInfo.RentCost));
			stringBuilder.Append(XUiM_InGameService.StringFormatHandler(Localization.Get("xuiGameTime", false, null), traderInfo.RentTimeInDays, Localization.Get("xuiGameDays", false, null)));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06008AC1 RID: 35521 RVA: 0x001287DE File Offset: 0x001269DE
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string _title, object _value)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1}[-]\n", _title, _value);
	}

	// Token: 0x06008AC2 RID: 35522 RVA: 0x0034D7FB File Offset: 0x0034B9FB
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string _title, object _value, string _units)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1} {2}[-]\n", _title, _value, _units);
	}
}
