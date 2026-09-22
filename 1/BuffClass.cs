using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class BuffClass
{
	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x0600345C RID: 13404 RVA: 0x0015C94B File Offset: 0x0015AB4B
	// (set) Token: 0x0600345D RID: 13405 RVA: 0x0015C953 File Offset: 0x0015AB53
	public float DurationMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.durationMax;
		}
		set
		{
			if (this.initialDurationMax == 0f && value > 0f)
			{
				this.initialDurationMax = value;
			}
			this.durationMax = value;
		}
	}

	// Token: 0x1700054B RID: 1355
	// (get) Token: 0x0600345E RID: 13406 RVA: 0x0015C978 File Offset: 0x0015AB78
	public float InitialDurationMax
	{
		get
		{
			return this.initialDurationMax;
		}
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x0015C980 File Offset: 0x0015AB80
	public BuffClass(string _name = "")
	{
		this.Name = _name.ToLower();
		this.LocalizedName = string.Empty;
		this.DescriptionKey = string.Empty;
		this.TooltipKey = string.Empty;
		this.Icon = string.Empty;
		this.IconBlink = false;
		this.Requirements = null;
		this.Hidden = false;
		this.DamageType = EnumDamageTypes.None;
		this.StackType = BuffEffectStackTypes.Replace;
		this.durationMax = 0f;
		this.initialDurationMax = 0f;
	}

	// Token: 0x06003460 RID: 13408 RVA: 0x0015CA2D File Offset: 0x0015AC2D
	public void Tick(BuffValue _ev)
	{
		_ev.DurationTick();
		if (this.DurationMax > 0f && _ev.DurationInSeconds >= this.DurationMax)
		{
			_ev.Finished = true;
		}
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x0015CA58 File Offset: 0x0015AC58
	public void ModifyValue(EntityAlive _self, PassiveEffects _effect, BuffValue _bv, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags)
	{
		if (_bv.Remove)
		{
			return;
		}
		_self.MinEventContext.Tags |= _tags;
		if (!this.canRun(_self.MinEventContext))
		{
			return;
		}
		if (this.Effects != null)
		{
			this.Effects.ModifyValue(_self, _effect, ref _base_value, ref _perc_value, _bv.DurationInSeconds, _tags, (this.StackType == BuffEffectStackTypes.Effect) ? _bv.StackEffectMultiplier : 1);
		}
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x0015CAC8 File Offset: 0x0015ACC8
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, EntityAlive _self, PassiveEffects _effect, BuffValue _bv, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags)
	{
		if (_bv.Remove)
		{
			return;
		}
		_self.MinEventContext.Tags |= _tags;
		if (!this.canRun(_self.MinEventContext))
		{
			return;
		}
		if (this.Effects != null)
		{
			this.Effects.GetModifiedValueData(_modValueSources, _sourceType, _self, _effect, ref _base_value, ref _perc_value, _bv.DurationInSeconds, _tags, (this.StackType == BuffEffectStackTypes.Effect) ? _bv.StackEffectMultiplier : 1);
		}
	}

	// Token: 0x06003463 RID: 13411 RVA: 0x0015CB3E File Offset: 0x0015AD3E
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.Effects == null)
		{
			return;
		}
		if (!this.canRun(_params))
		{
			return;
		}
		this.Effects.FireEvent(_eventType, _params);
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x0015CB60 File Offset: 0x0015AD60
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canRun(MinEventParams _params)
	{
		return this.Requirements == null || this.Requirements.IsValid(_params);
	}

	// Token: 0x040029A4 RID: 10660
	public string Name;

	// Token: 0x040029A5 RID: 10661
	public string LocalizedName;

	// Token: 0x040029A6 RID: 10662
	public string Description;

	// Token: 0x040029A7 RID: 10663
	public string DescriptionKey;

	// Token: 0x040029A8 RID: 10664
	public string Tooltip;

	// Token: 0x040029A9 RID: 10665
	public string TooltipKey;

	// Token: 0x040029AA RID: 10666
	public string Icon;

	// Token: 0x040029AB RID: 10667
	public string DisplayValueCVar;

	// Token: 0x040029AC RID: 10668
	public string DisplayValueKey;

	// Token: 0x040029AD RID: 10669
	public BuffClass.CVarDisplayFormat DisplayValueFormat;

	// Token: 0x040029AE RID: 10670
	public Color IconColor;

	// Token: 0x040029AF RID: 10671
	public bool IconBlink;

	// Token: 0x040029B0 RID: 10672
	public EnumEntityUINotificationDisplayMode DisplayType;

	// Token: 0x040029B1 RID: 10673
	public RequirementGroup Requirements;

	// Token: 0x040029B2 RID: 10674
	public MinEffectController Effects;

	// Token: 0x040029B3 RID: 10675
	public bool Hidden;

	// Token: 0x040029B4 RID: 10676
	public bool ShowOnHUD = true;

	// Token: 0x040029B5 RID: 10677
	public bool AllowInEditor;

	// Token: 0x040029B6 RID: 10678
	public EnumGameStats RequiredGameStat = EnumGameStats.Last;

	// Token: 0x040029B7 RID: 10679
	[PublicizedFrom(EAccessModifier.Private)]
	public float durationMax;

	// Token: 0x040029B8 RID: 10680
	[PublicizedFrom(EAccessModifier.Private)]
	public float initialDurationMax;

	// Token: 0x040029B9 RID: 10681
	public int UpdateRateTicks = 20;

	// Token: 0x040029BA RID: 10682
	public EnumDamageTypes DamageType;

	// Token: 0x040029BB RID: 10683
	public EnumDamageSource DamageSource;

	// Token: 0x040029BC RID: 10684
	public BuffEffectStackTypes StackType;

	// Token: 0x040029BD RID: 10685
	public bool RemoveOnDeath = true;

	// Token: 0x040029BE RID: 10686
	public FastTags<TagGroup.Global> NameTag;

	// Token: 0x040029BF RID: 10687
	public FastTags<TagGroup.Global> Tags = FastTags<TagGroup.Global>.none;

	// Token: 0x02000652 RID: 1618
	public enum CVarDisplayFormat
	{
		// Token: 0x040029C1 RID: 10689
		None,
		// Token: 0x040029C2 RID: 10690
		Float,
		// Token: 0x040029C3 RID: 10691
		FlooredToInt,
		// Token: 0x040029C4 RID: 10692
		RoundedToInt,
		// Token: 0x040029C5 RID: 10693
		CeiledToInt,
		// Token: 0x040029C6 RID: 10694
		Time,
		// Token: 0x040029C7 RID: 10695
		Percentage,
		// Token: 0x040029C8 RID: 10696
		Degrees
	}
}
