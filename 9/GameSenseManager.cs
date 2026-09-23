using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SteelSeries.GameSense;
using SteelSeries.GameSense.DeviceZone;
using UnityEngine;

// Token: 0x020011F0 RID: 4592
public class GameSenseManager : IEntityBuffsChanged
{
	// Token: 0x1700117D RID: 4477
	// (get) Token: 0x060092AA RID: 37546 RVA: 0x00376D0F File Offset: 0x00374F0F
	// (set) Token: 0x060092A9 RID: 37545 RVA: 0x00376D07 File Offset: 0x00374F07
	public static GameSenseManager Instance { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060092AB RID: 37547 RVA: 0x00376D16 File Offset: 0x00374F16
	[PublicizedFrom(EAccessModifier.Private)]
	static GameSenseManager()
	{
		if (GameManager.IsDedicatedServer || Application.isEditor)
		{
			return;
		}
		if (GameUtils.GetLaunchArgument("nogamesense") != null)
		{
			return;
		}
		GameSenseManager.Instance = new GameSenseManager();
	}

	// Token: 0x1700117E RID: 4478
	// (get) Token: 0x060092AC RID: 37548 RVA: 0x00376D3E File Offset: 0x00374F3E
	public static bool GameSenseInstalled
	{
		get
		{
			return SdFile.Exists(GSClient._getPropsPath());
		}
	}

	// Token: 0x060092AD RID: 37549 RVA: 0x00376D4C File Offset: 0x00374F4C
	public void Init()
	{
		if (!GameSenseManager.GameSenseInstalled)
		{
			Log.Out("GameSense server not found (no props file), disabling");
			GameSenseManager.Instance = null;
			return;
		}
		new GameObject("GameSense")
		{
			hideFlags = HideFlags.HideAndDontSave
		}.AddComponent<GSClient>();
		GSClient.Instance.RegisterGame("7dtd", "7 Days to Die", "The Fun Pimps");
		GameStats.OnChangedDelegates += this.GameStats_OnChanged;
		ThreadManager.StartCoroutine(this.BindEventsCo());
	}

	// Token: 0x060092AE RID: 37550 RVA: 0x00376DBF File Offset: 0x00374FBF
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator BindEventsCo()
	{
		yield return new WaitForSeconds(0.1f);
		this.BindEventTime();
		this.BindEventAmmo();
		this.BindEventHealth();
		this.BindEventDurability();
		this.BindEventStealth();
		this.BindEventBleeding();
		this.BindEventBloodmoon();
		this.BindEventCompass();
		this.BindEventHit();
		this.BindEventDukes();
		yield break;
	}

	// Token: 0x060092AF RID: 37551 RVA: 0x00376DD0 File Offset: 0x00374FD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventTime()
	{
		ColorRanges color = ColorRanges.Create(new ColorRange[]
		{
			new ColorRange(0U, 16U, ColorStatic.Create(byte.MaxValue, 0, 0)),
			new ColorRange(17U, 32U, ColorStatic.Create(byte.MaxValue, 160, 0)),
			new ColorRange(33U, 49U, ColorStatic.Create(byte.MaxValue, byte.MaxValue, 0)),
			new ColorRange(50U, 66U, ColorStatic.Create(byte.MaxValue, byte.MaxValue, 0)),
			new ColorRange(67U, 74U, ColorStatic.Create(byte.MaxValue, 160, 0)),
			new ColorRange(75U, 82U, ColorStatic.Create(byte.MaxValue, 80, 0)),
			new ColorRange(83U, 91U, ColorStatic.Create(byte.MaxValue, 40, 0)),
			new ColorRange(92U, 100U, ColorStatic.Create(byte.MaxValue, 0, 0))
		});
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneFunctionKeys>(), IlluminationMode.Percent, color);
		this.BindEventWrapper("TIME", 0, 100, EventIconId.Clock, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B0 RID: 37552 RVA: 0x00376EE4 File Offset: 0x003750E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventAmmo()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneNumberKeys>(), IlluminationMode.Percent, ColorGradient.Create(new RGB(160, 0, byte.MaxValue), new RGB(0, 0, byte.MaxValue)));
		this.BindEventWrapper("AMMO", 0, 100, EventIconId.Ammo, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B1 RID: 37553 RVA: 0x00376F38 File Offset: 0x00375138
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventHealth()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneRowQ>(), IlluminationMode.Percent, ColorGradient.Create(new RGB(byte.MaxValue, 0, 0), new RGB(0, byte.MaxValue, 0)), RateRange.Create(new FreqRepeatLimitPair[]
		{
			new FreqRepeatLimitPair(0U, 10U, 2U, 0U),
			new FreqRepeatLimitPair(11U, 20U, 1U, 0U)
		}));
		this.BindEventWrapper("HEALTH", 0, 100, EventIconId.Health, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B2 RID: 37554 RVA: 0x00376FB8 File Offset: 0x003751B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventDurability()
	{
		RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
		rgbperkeyZoneCustom.zone = new byte[]
		{
			83,
			84,
			85,
			86
		};
		AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Percent, ColorGradient.Create(new RGB(130, 130, 0), new RGB(0, byte.MaxValue, 200)));
		this.BindEventWrapper("DURABILITY", 0, 100, EventIconId.Item, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B3 RID: 37555 RVA: 0x0037702C File Offset: 0x0037522C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventStealth()
	{
		RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
		rgbperkeyZoneCustom.zone = new byte[]
		{
			224,
			225,
			57,
			43,
			53
		};
		AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Percent, ColorGradient.Create(new RGB(50, 120, 50), new RGB(0, 250, 250)));
		this.BindEventWrapper("STEALTH", 0, 100, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B4 RID: 37556 RVA: 0x00377098 File Offset: 0x00375298
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventBleeding()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneNavCluster>(), IlluminationMode.Color, ColorStatic.Create(byte.MaxValue, 0, 0), RateStatic.Create(5U, 0U));
		this.BindEventWrapper("BLEEDING", 0, 1, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B5 RID: 37557 RVA: 0x003770E0 File Offset: 0x003752E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventBloodmoon()
	{
		ColorRanges color = ColorRanges.Create(new ColorRange[]
		{
			new ColorRange(0U, 0U, ColorStatic.Create(0, 0, 0)),
			new ColorRange(1U, 10U, ColorStatic.Create(60, 0, 0)),
			new ColorRange(11U, 20U, ColorStatic.Create(70, 0, 0)),
			new ColorRange(21U, 30U, ColorStatic.Create(80, 0, 0)),
			new ColorRange(31U, 40U, ColorStatic.Create(90, 0, 0)),
			new ColorRange(41U, 50U, ColorStatic.Create(105, 0, 0)),
			new ColorRange(51U, 60U, ColorStatic.Create(120, 0, 0)),
			new ColorRange(61U, 70U, ColorStatic.Create(135, 0, 0)),
			new ColorRange(71U, 80U, ColorStatic.Create(150, 0, 0)),
			new ColorRange(81U, 90U, ColorStatic.Create(190, 0, 0)),
			new ColorRange(91U, 100U, ColorStatic.Create(byte.MaxValue, 0, 0))
		});
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneRowA>(), IlluminationMode.Color, color, RateRange.Create(new FreqRepeatLimitPair[]
		{
			new FreqRepeatLimitPair(81U, 90U, 1U, 0U)
		}));
		this.BindEventWrapper("BLOODMOON", 0, 100, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B6 RID: 37558 RVA: 0x00377230 File Offset: 0x00375430
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventCompass()
	{
		foreach (Tuple<string, byte> tuple in new Tuple<string, byte>[]
		{
			new Tuple<string, byte>("NORTH", 96),
			new Tuple<string, byte>("NORTHEAST", 97),
			new Tuple<string, byte>("EAST", 94),
			new Tuple<string, byte>("SOUTHEAST", 91),
			new Tuple<string, byte>("SOUTH", 90),
			new Tuple<string, byte>("SOUTHWEST", 89),
			new Tuple<string, byte>("WEST", 92),
			new Tuple<string, byte>("NORTHWEST", 95)
		})
		{
			RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
			rgbperkeyZoneCustom.zone = new byte[]
			{
				tuple.Item2
			};
			AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Color, ColorStatic.Create(50, byte.MaxValue, 0));
			this.BindEventWrapper(tuple.Item1, 0, 1, EventIconId.Compass, false, new AbstractHandler[]
			{
				abstractHandler
			});
		}
	}

	// Token: 0x060092B7 RID: 37559 RVA: 0x0037731C File Offset: 0x0037551C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventHit()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<MouseZoneAll>(), IlluminationMode.Color, ColorStatic.Create(byte.MaxValue, 0, 0), RateStatic.Create(8U, 0U));
		this.BindEventWrapper("HIT", 0, 1, EventIconId.Headshot, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x060092B8 RID: 37560 RVA: 0x00377364 File Offset: 0x00375564
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventDukes()
	{
		LineData lineData = LineData.Create(LineDataText.Create("Dukes: ", "", true, 0U), LineDataAccessor.ContextFrameKey("dukesstring"));
		FrameDataMultiLine frameDataMultiLine = FrameDataMultiLine.Create(new LineData[]
		{
			lineData
		}, new FrameModifiers(10000U, false), EventIconId.Money);
		AbstractScreenDevice_Zone dz = ScriptableObject.CreateInstance<ScreenedZoneOne>();
		ScreenMode mode = ScreenMode.screen;
		AbstractFrameData[] datas = new FrameDataMultiLine[]
		{
			frameDataMultiLine
		};
		ScreenHandler screenHandler = ScreenHandler.Create(dz, mode, datas);
		this.BindEventWrapper("DUKES", 0, 1, EventIconId.Money, true, new AbstractHandler[]
		{
			screenHandler
		});
	}

	// Token: 0x060092B9 RID: 37561 RVA: 0x003773DF File Offset: 0x003755DF
	public void Cleanup()
	{
		GameStats.OnChangedDelegates -= this.GameStats_OnChanged;
	}

	// Token: 0x060092BA RID: 37562 RVA: 0x003773F2 File Offset: 0x003755F2
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventWrapper(string _eventName, int _minValue, int _maxValue, EventIconId _icon, bool _optionalValue, params AbstractHandler[] _handlers)
	{
		GSClient.Instance.BindEvent(_eventName, _minValue, _maxValue, _icon, _handlers, _optionalValue);
		this.registeredEvents.Add(_eventName);
	}

	// Token: 0x060092BB RID: 37563 RVA: 0x00377414 File Offset: 0x00375614
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetEventValues()
	{
		foreach (string eventName in this.registeredEvents)
		{
			GSClient.Instance.SendEvent(eventName, 0, null);
		}
	}

	// Token: 0x060092BC RID: 37564 RVA: 0x00377470 File Offset: 0x00375670
	public void SessionStarted(EntityPlayerLocal _playerEntity)
	{
		this.ResetEventValues();
		this.entityPlayer = new WeakReference<EntityPlayerLocal>(_playerEntity);
		_playerEntity.PlayerStats.AddBuffChangedDelegate(this);
		this.lastTimePercent = -1;
		this.previousAmmoPercentage = -1;
		this.previousSlotIndex = -1;
		this.previousDurability = -1;
		this.previousStealth = -1;
		this.bloodMoonWarning = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
		this.duskDawnTime = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		this.lastBloodMoonValue = -1;
		this.lastDirection = GameSenseManager.EDirection45.None;
		this.dukesItem = ItemClass.GetItem(TraderInfo.CurrencyItem, false);
	}

	// Token: 0x060092BD RID: 37565 RVA: 0x003774FC File Offset: 0x003756FC
	public void SessionEnded()
	{
		this.ResetEventValues();
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer.TryGetTarget(out entityPlayerLocal))
		{
			entityPlayerLocal.PlayerStats.RemoveBuffChangedDelegate(this);
		}
		this.entityPlayer = null;
		this.dukesItem = null;
	}

	// Token: 0x060092BE RID: 37566 RVA: 0x00377538 File Offset: 0x00375738
	[PublicizedFrom(EAccessModifier.Private)]
	public void GameStats_OnChanged(EnumGameStats _gameStat, object _newValue)
	{
		if (_gameStat == EnumGameStats.BloodMoonDay)
		{
			this.bloodMoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
		}
	}

	// Token: 0x060092BF RID: 37567 RVA: 0x0037754C File Offset: 0x0037574C
	public void Update()
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.UpdateEventAmmo();
		this.UpdateEventDurability();
		this.UpdateEventStealth();
		this.UpdateEventDukes();
	}

	// Token: 0x060092C0 RID: 37568 RVA: 0x00377574 File Offset: 0x00375774
	public void UpdateEventTime(ulong _worldTime)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.UpdateEventBloodmoon(_worldTime);
		int num = (int)(_worldTime % 24000UL) / 240;
		if (this.lastTimePercent == num)
		{
			return;
		}
		this.lastTimePercent = num;
		GSClient.Instance.SendEvent("TIME", num, null);
	}

	// Token: 0x060092C1 RID: 37569 RVA: 0x003775C8 File Offset: 0x003757C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventAmmo()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int focusedItemIdx = entityPlayerLocal.inventory.GetFocusedItemIdx();
		ItemValue itemValue = entityPlayerLocal.inventory.GetItem(focusedItemIdx).itemValue;
		ItemClass itemClass;
		if (itemValue == null || itemValue.type == 0 || (itemClass = itemValue.ItemClass) == null)
		{
			this.previousAmmoPercentage = 0;
			GSClient.Instance.SendEvent("AMMO", 0, null);
			return;
		}
		ItemActionAttack itemActionAttack = itemClass.Actions[0] as ItemActionAttack;
		int num;
		if (itemActionAttack == null || itemActionAttack is ItemActionMelee || (num = (int)EffectManager.GetValue(PassiveEffects.MagazineSize, itemValue, 0f, entityPlayerLocal, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)) <= 0)
		{
			this.previousAmmoPercentage = 0;
			GSClient.Instance.SendEvent("AMMO", 0, null);
			return;
		}
		int meta = itemValue.Meta;
		int num2 = 100 * meta / num;
		if (num2 == this.previousAmmoPercentage)
		{
			return;
		}
		this.previousAmmoPercentage = num2;
		GSClient.Instance.SendEvent("AMMO", num2, null);
	}

	// Token: 0x060092C2 RID: 37570 RVA: 0x003776D6 File Offset: 0x003758D6
	public void UpdateEventHealth(int _value)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		GSClient.Instance.SendEvent("HEALTH", _value, null);
	}

	// Token: 0x060092C3 RID: 37571 RVA: 0x003776F8 File Offset: 0x003758F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventDurability()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int focusedItemIdx = entityPlayerLocal.inventory.GetFocusedItemIdx();
		ItemValue itemValue = entityPlayerLocal.inventory.GetItem(focusedItemIdx).itemValue;
		ItemClass itemClass;
		if (itemValue == null || itemValue.type == 0 || (itemClass = itemValue.ItemClass) == null || !itemClass.ShowQualityBar)
		{
			this.previousDurability = 0;
			GSClient.Instance.SendEvent("DURABILITY", 0, null);
			return;
		}
		int num = (int)(100f * itemValue.PercentUsesLeft);
		if (num == this.previousDurability)
		{
			return;
		}
		this.previousDurability = num;
		GSClient.Instance.SendEvent("DURABILITY", num, null);
	}

	// Token: 0x060092C4 RID: 37572 RVA: 0x003777B0 File Offset: 0x003759B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventStealth()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int num = (int)(100f * entityPlayerLocal.Stealth.ValuePercentUI);
		if (num == this.previousStealth)
		{
			return;
		}
		this.previousStealth = num;
		GSClient.Instance.SendEvent("STEALTH", num, null);
	}

	// Token: 0x060092C5 RID: 37573 RVA: 0x00377813 File Offset: 0x00375A13
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventBleeding(bool _isBleeding)
	{
		GSClient.Instance.SendEvent("BLEEDING", _isBleeding ? 1 : 0, null);
	}

	// Token: 0x060092C6 RID: 37574 RVA: 0x0037782C File Offset: 0x00375A2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventBloodmoon(ulong _worldTime)
	{
		int num = 0;
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		if (GameUtils.IsBloodMoonTime(_worldTime, this.duskDawnTime, this.bloodMoonDay))
		{
			num = 100;
		}
		else if (this.bloodMoonDay == item && this.bloodMoonWarning >= 0 && this.bloodMoonWarning <= item2)
		{
			int num2 = this.duskDawnTime.Item1 - 2;
			if (item2 > num2)
			{
				num = 85;
			}
			else
			{
				float num3 = (float)item2 + (float)item3 / 60f - (float)this.bloodMoonWarning;
				float num4 = (float)(num2 - this.bloodMoonWarning);
				num = (int)(80f * num3 / num4);
			}
		}
		if (num == this.lastBloodMoonValue)
		{
			return;
		}
		this.lastBloodMoonValue = num;
		GSClient.Instance.SendEvent("BLOODMOON", num, null);
	}

	// Token: 0x060092C7 RID: 37575 RVA: 0x003778F4 File Offset: 0x00375AF4
	public void UpdateEventCompass(float _rotation)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		GameSenseManager.EDirection45 edirection;
		if ((double)_rotation > 337.5 || (double)_rotation <= 22.5)
		{
			edirection = GameSenseManager.EDirection45.N;
		}
		else if ((double)_rotation <= 67.5)
		{
			edirection = GameSenseManager.EDirection45.NE;
		}
		else if ((double)_rotation <= 112.5)
		{
			edirection = GameSenseManager.EDirection45.E;
		}
		else if ((double)_rotation <= 157.5)
		{
			edirection = GameSenseManager.EDirection45.SE;
		}
		else if ((double)_rotation <= 202.5)
		{
			edirection = GameSenseManager.EDirection45.S;
		}
		else if ((double)_rotation <= 247.5)
		{
			edirection = GameSenseManager.EDirection45.SW;
		}
		else if ((double)_rotation <= 292.5)
		{
			edirection = GameSenseManager.EDirection45.W;
		}
		else
		{
			edirection = GameSenseManager.EDirection45.NW;
		}
		if (this.lastDirection == edirection)
		{
			return;
		}
		this.lastDirection = edirection;
		GSClient.Instance.SendEvent("NORTH", (edirection == GameSenseManager.EDirection45.N) ? 1 : 0, null);
		GSClient.Instance.SendEvent("NORTHEAST", (edirection == GameSenseManager.EDirection45.NE) ? 1 : 0, null);
		GSClient.Instance.SendEvent("EAST", (edirection == GameSenseManager.EDirection45.E) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTHEAST", (edirection == GameSenseManager.EDirection45.SE) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTH", (edirection == GameSenseManager.EDirection45.S) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTHWEST", (edirection == GameSenseManager.EDirection45.SW) ? 1 : 0, null);
		GSClient.Instance.SendEvent("WEST", (edirection == GameSenseManager.EDirection45.W) ? 1 : 0, null);
		GSClient.Instance.SendEvent("NORTHWEST", (edirection == GameSenseManager.EDirection45.NW) ? 1 : 0, null);
	}

	// Token: 0x060092C8 RID: 37576 RVA: 0x00377A64 File Offset: 0x00375C64
	public void UpdateEventHit()
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.stopHitEventTime = Time.unscaledTime + 0.5f;
		if (this.stopHitEventCoroutine == null)
		{
			GSClient.Instance.SendEvent("HIT", 1, null);
			this.stopHitEventCoroutine = ThreadManager.StartCoroutine(this.StopHitCo());
		}
	}

	// Token: 0x060092C9 RID: 37577 RVA: 0x00377AB9 File Offset: 0x00375CB9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StopHitCo()
	{
		while (Time.unscaledTime < this.stopHitEventTime)
		{
			yield return null;
		}
		GSClient.Instance.SendEvent("HIT", 0, null);
		this.stopHitEventCoroutine = null;
		yield break;
	}

	// Token: 0x060092CA RID: 37578 RVA: 0x00377AC8 File Offset: 0x00375CC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventDukes()
	{
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime < this.dukesNextTime)
		{
			return;
		}
		this.dukesNextTime = unscaledTime + 5f;
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int num = entityPlayerLocal.inventory.GetItemCount(this.dukesItem, false, -1, -1, true);
		num += entityPlayerLocal.bag.GetItemCount(this.dukesItem, -1, -1, true);
		ContextFrameObject contextFrameObject = new ContextFrameObject();
		contextFrameObject["dukesstring"] = ((double)num).FormatNumberWithMetricPrefix(true, 2);
		ContextFrameObject frame = contextFrameObject;
		GSClient.Instance.SendEvent("DUKES", num, frame);
	}

	// Token: 0x060092CB RID: 37579 RVA: 0x00377B70 File Offset: 0x00375D70
	public void EntityBuffAdded(BuffValue _buff)
	{
		if (_buff.BuffClass.Name.EqualsCaseInsensitive("buffInjuryBleeding"))
		{
			this.UpdateEventBleeding(true);
		}
	}

	// Token: 0x060092CC RID: 37580 RVA: 0x00377B90 File Offset: 0x00375D90
	public void EntityBuffRemoved(BuffValue _buff)
	{
		if (_buff.BuffClass.Name.EqualsCaseInsensitive("buffInjuryBleeding"))
		{
			this.UpdateEventBleeding(false);
		}
	}

	// Token: 0x04006D98 RID: 28056
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<string> registeredEvents = new HashSet<string>();

	// Token: 0x04006D99 RID: 28057
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameName = "7dtd";

	// Token: 0x04006D9A RID: 28058
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameNameFull = "7 Days to Die";

	// Token: 0x04006D9B RID: 28059
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameDeveloper = "The Fun Pimps";

	// Token: 0x04006D9C RID: 28060
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventHealth = "HEALTH";

	// Token: 0x04006D9D RID: 28061
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventAmmo = "AMMO";

	// Token: 0x04006D9E RID: 28062
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventTime = "TIME";

	// Token: 0x04006D9F RID: 28063
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventDurability = "DURABILITY";

	// Token: 0x04006DA0 RID: 28064
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventStealth = "STEALTH";

	// Token: 0x04006DA1 RID: 28065
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventBleeding = "BLEEDING";

	// Token: 0x04006DA2 RID: 28066
	[PublicizedFrom(EAccessModifier.Private)]
	public const string BleedingBuffName = "buffInjuryBleeding";

	// Token: 0x04006DA3 RID: 28067
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventBloodmoon = "BLOODMOON";

	// Token: 0x04006DA4 RID: 28068
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BloodmoonBlinkWarningHours = 2;

	// Token: 0x04006DA5 RID: 28069
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventHit = "HIT";

	// Token: 0x04006DA6 RID: 28070
	[PublicizedFrom(EAccessModifier.Private)]
	public const float HitEventDuration = 0.5f;

	// Token: 0x04006DA7 RID: 28071
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventDukes = "DUKES";

	// Token: 0x04006DA8 RID: 28072
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DukesStringValueKey = "dukesstring";

	// Token: 0x04006DA9 RID: 28073
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DukesLabelLocKey = "gamesenseDukesLabel";

	// Token: 0x04006DAA RID: 28074
	[PublicizedFrom(EAccessModifier.Private)]
	public WeakReference<EntityPlayerLocal> entityPlayer;

	// Token: 0x04006DAB RID: 28075
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastTimePercent = -1;

	// Token: 0x04006DAC RID: 28076
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousSlotIndex = -1;

	// Token: 0x04006DAD RID: 28077
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousAmmoPercentage = -1;

	// Token: 0x04006DAE RID: 28078
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousDurability = -1;

	// Token: 0x04006DAF RID: 28079
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousStealth = -1;

	// Token: 0x04006DB0 RID: 28080
	[PublicizedFrom(EAccessModifier.Private)]
	public int bloodMoonDay;

	// Token: 0x04006DB1 RID: 28081
	[PublicizedFrom(EAccessModifier.Private)]
	public int bloodMoonWarning;

	// Token: 0x04006DB2 RID: 28082
	[TupleElementNames(new string[]
	{
		"duskHour",
		"dawnHour"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<int, int> duskDawnTime;

	// Token: 0x04006DB3 RID: 28083
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastBloodMoonValue = -1;

	// Token: 0x04006DB4 RID: 28084
	[PublicizedFrom(EAccessModifier.Private)]
	public GameSenseManager.EDirection45 lastDirection = GameSenseManager.EDirection45.None;

	// Token: 0x04006DB5 RID: 28085
	[PublicizedFrom(EAccessModifier.Private)]
	public float stopHitEventTime = -1f;

	// Token: 0x04006DB6 RID: 28086
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine stopHitEventCoroutine;

	// Token: 0x04006DB7 RID: 28087
	[PublicizedFrom(EAccessModifier.Private)]
	public float dukesNextTime;

	// Token: 0x04006DB8 RID: 28088
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue dukesItem;

	// Token: 0x020011F1 RID: 4593
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EDirection45
	{
		// Token: 0x04006DBA RID: 28090
		N,
		// Token: 0x04006DBB RID: 28091
		NE,
		// Token: 0x04006DBC RID: 28092
		E,
		// Token: 0x04006DBD RID: 28093
		SE,
		// Token: 0x04006DBE RID: 28094
		S,
		// Token: 0x04006DBF RID: 28095
		SW,
		// Token: 0x04006DC0 RID: 28096
		W,
		// Token: 0x04006DC1 RID: 28097
		NW,
		// Token: 0x04006DC2 RID: 28098
		None
	}
}
