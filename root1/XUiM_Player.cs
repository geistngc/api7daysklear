using System;
using UnityEngine;

// Token: 0x0200112B RID: 4395
public class XUiM_Player : XUiModel
{
	// Token: 0x06008AD9 RID: 35545 RVA: 0x0034F415 File Offset: 0x0034D615
	public static int GetLevel(EntityPlayer _player)
	{
		return _player.Progression.GetLevel();
	}

	// Token: 0x06008ADA RID: 35546 RVA: 0x0034F422 File Offset: 0x0034D622
	public static float GetLevelPercent(EntityPlayer _player)
	{
		return _player.Progression.GetLevelProgressPercentage();
	}

	// Token: 0x06008ADB RID: 35547 RVA: 0x0034F42F File Offset: 0x0034D62F
	public static int GetXPToNextLevel(EntityPlayer _player)
	{
		return _player.Progression.ExpToNextLevel;
	}

	// Token: 0x06008ADC RID: 35548 RVA: 0x0034F43C File Offset: 0x0034D63C
	public static float GetFood(EntityPlayer _player)
	{
		return _player.Stats.Food.Value;
	}

	// Token: 0x06008ADD RID: 35549 RVA: 0x0034F44E File Offset: 0x0034D64E
	public static float GetModifiedCurrentFood(EntityPlayer _player)
	{
		return _player.Stats.Food.Value + _player.Buffs.GetCustomVar("$foodAmount");
	}

	// Token: 0x06008ADE RID: 35550 RVA: 0x0034F471 File Offset: 0x0034D671
	public static float GetFoodPercent(EntityPlayer _player)
	{
		return 1f - _player.Stats.Food.Value / _player.Stats.Food.ModifiedMax;
	}

	// Token: 0x06008ADF RID: 35551 RVA: 0x0034F49A File Offset: 0x0034D69A
	public static int GetFoodMax(EntityPlayer _player)
	{
		return (int)_player.Stats.Food.Max;
	}

	// Token: 0x06008AE0 RID: 35552 RVA: 0x0034F4AD File Offset: 0x0034D6AD
	public static float GetWater(EntityPlayer _player)
	{
		return _player.Stats.Water.Value;
	}

	// Token: 0x06008AE1 RID: 35553 RVA: 0x0034F4BF File Offset: 0x0034D6BF
	public static float GetModifiedCurrentWater(EntityPlayer _player)
	{
		return _player.Stats.Water.Value + _player.Buffs.GetCustomVar("$waterAmount");
	}

	// Token: 0x06008AE2 RID: 35554 RVA: 0x0034F4E2 File Offset: 0x0034D6E2
	public static float GetWaterPercent(EntityPlayer _player)
	{
		return _player.Stats.Water.ValuePercentUI * 100f;
	}

	// Token: 0x06008AE3 RID: 35555 RVA: 0x0034F4FA File Offset: 0x0034D6FA
	public static int GetWaterMax(EntityPlayer _player)
	{
		return (int)_player.Stats.Water.Max;
	}

	// Token: 0x06008AE4 RID: 35556 RVA: 0x0034F50D File Offset: 0x0034D70D
	public static string GetCoreTemp(EntityPlayer _player)
	{
		return ValueDisplayFormatters.Temperature(_player.Buffs.GetCustomVar("_coretemp"), -1);
	}

	// Token: 0x06008AE5 RID: 35557 RVA: 0x0034F525 File Offset: 0x0034D725
	public static string GetOutsideTemp(EntityPlayer _player)
	{
		return ValueDisplayFormatters.Temperature(_player.Buffs.GetCustomVar("_outsidetemp"), -1);
	}

	// Token: 0x06008AE6 RID: 35558 RVA: 0x0034F53D File Offset: 0x0034D73D
	public static int GetZombieKills(EntityPlayer _player)
	{
		return _player.KilledZombies;
	}

	// Token: 0x06008AE7 RID: 35559 RVA: 0x0034F545 File Offset: 0x0034D745
	public static int GetPlayerKills(EntityPlayer _player)
	{
		return _player.KilledPlayers;
	}

	// Token: 0x06008AE8 RID: 35560 RVA: 0x0034F54D File Offset: 0x0034D74D
	public static int GetDeaths(EntityPlayer _player)
	{
		return _player.Died;
	}

	// Token: 0x06008AE9 RID: 35561 RVA: 0x0034F555 File Offset: 0x0034D755
	public static string GetKMTraveled(EntityPlayer _player)
	{
		return (_player.distanceWalked / 1000f).ToCultureInvariantString("0.00") + " KM";
	}

	// Token: 0x06008AEA RID: 35562 RVA: 0x0034F577 File Offset: 0x0034D777
	public static int GetItemsCrafted(EntityPlayer _player)
	{
		return (int)_player.totalItemsCrafted;
	}

	// Token: 0x06008AEB RID: 35563 RVA: 0x0034F57F File Offset: 0x0034D77F
	public static string GetLongestLife(EntityPlayer _player)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)((int)_player.longestLife) * 60f);
	}

	// Token: 0x06008AEC RID: 35564 RVA: 0x0034F594 File Offset: 0x0034D794
	public static string GetCurrentLife(EntityPlayer _player)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)((int)_player.currentLife) * 60f);
	}

	// Token: 0x06008AED RID: 35565 RVA: 0x0034F5A9 File Offset: 0x0034D7A9
	public static float GetHealth(EntityPlayer _player)
	{
		return _player.Stats.Health.Value;
	}

	// Token: 0x06008AEE RID: 35566 RVA: 0x0034F5BB File Offset: 0x0034D7BB
	public static float GetStamina(EntityPlayer _player)
	{
		return _player.Stats.Stamina.Value;
	}

	// Token: 0x06008AEF RID: 35567 RVA: 0x0034F5CD File Offset: 0x0034D7CD
	public static float GetMaxHealth(EntityPlayer _player)
	{
		return _player.Stats.Health.Max;
	}

	// Token: 0x06008AF0 RID: 35568 RVA: 0x0034F5DF File Offset: 0x0034D7DF
	public static float GetMaxStamina(EntityPlayer _player)
	{
		return _player.Stats.Stamina.Max;
	}

	// Token: 0x06008AF1 RID: 35569 RVA: 0x0034F5F1 File Offset: 0x0034D7F1
	public static bool GetHasFullHealth(EntityPlayer _player)
	{
		return Mathf.Approximately(_player.Stats.Health.Max, _player.Stats.Health.Value);
	}

	// Token: 0x06008AF2 RID: 35570 RVA: 0x0034F618 File Offset: 0x0034D818
	public static EntityPlayer GetPlayer()
	{
		return GameManager.Instance.World.GetPrimaryPlayer();
	}

	// Token: 0x06008AF3 RID: 35571 RVA: 0x0034F629 File Offset: 0x0034D829
	public static EntityPlayer GetPlayer(int _id)
	{
		if (GameManager.Instance != null && GameManager.Instance.World != null)
		{
			return GameManager.Instance.World.GetEntity(_id) as EntityPlayer;
		}
		return null;
	}

	// Token: 0x06008AF4 RID: 35572 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CalcDisplayProtectionValues()
	{
	}

	// Token: 0x06008AF5 RID: 35573 RVA: 0x0034F65C File Offset: 0x0034D85C
	public static string GetStatValue(PassiveEffects _effect, EntityPlayer _player, DisplayInfoEntry _entry, FastTags<TagGroup.Global> _overrideMovementTag)
	{
		FastTags<TagGroup.Global> fastTags = _player.generalTags;
		if (_entry.TagsSet)
		{
			fastTags = _entry.Tags;
		}
		if (_overrideMovementTag.IsEmpty)
		{
			fastTags |= EntityAlive.MovementTagRunning;
		}
		else
		{
			fastTags |= _overrideMovementTag;
		}
		float num = EffectManager.GetValue(_effect, null, 0f, _player, null, fastTags, true, true, true, true, true, 1, true, true);
		if (_entry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
		{
			num *= 100f;
			num = Mathf.Floor(num);
			if (_entry.ShowInverted)
			{
				num -= 100f;
			}
			return num.ToString("0") + "%";
		}
		if (_entry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
		{
			return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num);
		}
		if (_entry.DisplayType == DisplayInfoEntry.DisplayTypes.Integer)
		{
			num = Mathf.Floor(num);
		}
		else
		{
			num *= 100f;
			num = Mathf.Floor(num);
			num /= 100f;
		}
		if (_entry.ShowInverted)
		{
			num -= 1f;
		}
		return num.ToString("0.##");
	}
}
