using System;
using SandboxOptions;
using UnityEngine;

// Token: 0x02001141 RID: 4417
public class XUiM_Trader : XUiModel
{
	// Token: 0x1700101E RID: 4126
	// (get) Token: 0x06008BA8 RID: 35752 RVA: 0x00352E5F File Offset: 0x0035105F
	public TraderData TraderData
	{
		get
		{
			if (this.Trader == null)
			{
				return null;
			}
			return this.Trader.TraderData;
		}
	}

	// Token: 0x06008BA9 RID: 35753 RVA: 0x00352E78 File Offset: 0x00351078
	public static int GetBuyPrice(XUi _xui, ItemValue itemValue, int count, ItemClass itemClass = null, int index = -1)
	{
		bool flag = false;
		TraderData traderData = _xui.Trader.TraderData;
		TraderInfo traderInfo = (traderData != null) ? traderData.TraderInfo : null;
		if (itemClass == null)
		{
			itemClass = itemValue.ItemClass;
		}
		float num;
		int economicBundleSize;
		if (itemClass.IsBlock())
		{
			num = Block.list[itemValue.type].EconomicValue;
			economicBundleSize = Block.list[itemValue.type].EconomicBundleSize;
		}
		else
		{
			num = EffectManager.GetValue(PassiveEffects.EconomicValue, itemValue, itemClass.EconomicValue, _xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			economicBundleSize = itemClass.EconomicBundleSize;
		}
		if (num == 0f)
		{
			return 0;
		}
		float num2 = 0f;
		float num3 = 0f;
		if (traderData == null)
		{
			num3 = TraderInfo.BuyMarkup;
		}
		else if (traderInfo.Rentable || traderInfo.PlayerOwned)
		{
			if (index != -1)
			{
				num3 = 1f + (float)traderData.PrimaryInventory[index].Markup * 0.2f;
				flag = true;
			}
		}
		else
		{
			flag = (traderInfo.OverrideBuyMarkup != -1f);
			num3 = (flag ? traderInfo.OverrideBuyMarkup : TraderInfo.BuyMarkup);
		}
		if (itemValue.HasQuality)
		{
			num2 = num * num3;
			if (itemClass.TraderQualityMinMod > 0f || itemClass.TraderQualityMaxMod > 0f)
			{
				num2 *= Mathf.Lerp(itemClass.TraderQualityMinMod, itemClass.TraderQualityMaxMod, ((float)itemValue.Quality - 1f) / 5f);
			}
			else
			{
				num2 *= Mathf.Lerp(TraderInfo.QualityMinMod, TraderInfo.QualityMaxMod, ((float)itemValue.Quality - 1f) / 5f);
			}
			float percentUsesLeft = itemValue.PercentUsesLeft;
			num2 *= percentUsesLeft;
		}
		else if (itemClass.HasSubItems)
		{
			for (int i = 0; i < itemValue.Modifications.Length; i++)
			{
				ItemValue itemValue2 = itemValue.Modifications[i];
				if (!itemValue2.IsEmpty())
				{
					num2 += (float)XUiM_Trader.GetBuyPrice(_xui, itemValue2, 1, null, -1);
				}
			}
		}
		else
		{
			num2 = num * num3;
		}
		if (!flag)
		{
			num2 -= num2 * EffectManager.GetValue(PassiveEffects.BarteringBuying, null, 0f, XUiM_Player.GetPlayer(), null, itemClass.ItemTags, true, true, true, true, true, 1, true, false);
		}
		return Mathf.CeilToInt((float)((int)(num2 * (float)(count / economicBundleSize))) * SandboxOptionManager.GetFloat(SandboxOptions.TraderBuyPrices));
	}

	// Token: 0x06008BAA RID: 35754 RVA: 0x003530C0 File Offset: 0x003512C0
	public static int GetSellPrice(XUi _xui, ItemValue itemValue, int count, ItemClass itemClass = null)
	{
		bool flag = false;
		TraderData traderData = _xui.Trader.TraderData;
		if (itemClass == null)
		{
			itemClass = itemValue.ItemClass;
		}
		float num;
		int economicBundleSize;
		if (itemClass.IsBlock())
		{
			Block block = Block.list[itemValue.type];
			num = block.EconomicValue;
			num *= block.EconomicSellScale;
			economicBundleSize = block.EconomicBundleSize;
		}
		else
		{
			num = itemClass.EconomicValue;
			num *= itemClass.EconomicSellScale;
			num = EffectManager.GetValue(PassiveEffects.EconomicValue, itemValue, num, _xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			economicBundleSize = itemClass.EconomicBundleSize;
		}
		if (num == 0f)
		{
			return 0;
		}
		float num2 = 0f;
		float num3;
		if (traderData == null)
		{
			num3 = TraderInfo.SellMarkdown;
		}
		else
		{
			TraderInfo traderInfo = traderData.TraderInfo;
			flag = (traderInfo.OverrideSellMarkdown != -1f);
			num3 = (flag ? traderInfo.OverrideSellMarkdown : TraderInfo.SellMarkdown);
		}
		if (itemValue.HasQuality)
		{
			num2 = num * num3;
			if (itemClass.TraderQualityMinMod > 0f || itemClass.TraderQualityMaxMod > 0f)
			{
				num2 *= Mathf.Lerp(itemClass.TraderQualityMinMod, itemClass.TraderQualityMaxMod, ((float)itemValue.Quality - 1f) / 5f);
			}
			else
			{
				num2 *= Mathf.Lerp(TraderInfo.QualityMinMod, TraderInfo.QualityMaxMod, ((float)itemValue.Quality - 1f) / 5f);
			}
			float percentUsesLeft = itemValue.PercentUsesLeft;
			num2 *= percentUsesLeft;
		}
		else if (itemClass.HasSubItems)
		{
			for (int i = 0; i < itemValue.Modifications.Length; i++)
			{
				ItemValue itemValue2 = itemValue.Modifications[i];
				if (!itemValue2.IsEmpty())
				{
					num2 += (float)XUiM_Trader.GetSellPrice(_xui, itemValue2, 1, null);
				}
			}
		}
		else
		{
			num2 = num * num3;
		}
		if (!flag)
		{
			num2 += num2 * EffectManager.GetValue(PassiveEffects.BarteringSelling, null, 0f, XUiM_Player.GetPlayer(), null, itemClass.ItemTags, true, true, true, true, true, 1, true, false);
		}
		return Mathf.CeilToInt((float)((int)(num2 * (float)(count / economicBundleSize))) * SandboxOptionManager.GetFloat(SandboxOptions.TraderSellPrices));
	}

	// Token: 0x0400674A RID: 26442
	public ITrader Trader;

	// Token: 0x0400674B RID: 26443
	public XUiC_TraderWindowGroup TraderWindowGroup;
}
