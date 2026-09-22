using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Twitch
{
	// Token: 0x020017F8 RID: 6136
	public class TwitchAction
	{
		// Token: 0x0600BE10 RID: 48656 RVA: 0x00467159 File Offset: 0x00465359
		public bool HasModifiedPrice()
		{
			return this.DefaultCost != this.ModifiedCost;
		}

		// Token: 0x0600BE11 RID: 48657 RVA: 0x0046716C File Offset: 0x0046536C
		public bool HasExtraConditions()
		{
			return ((!this.SingleDayUse && !this.RandomDaily) || this.AllowedDay != -1) && !this.OnCooldown && this.OnlyUsableByType == TwitchAction.OnlyUsableTypes.Everyone;
		}

		// Token: 0x0600BE12 RID: 48658 RVA: 0x0046719C File Offset: 0x0046539C
		public static int GetAdjustedBitPriceCeil(int price)
		{
			int num = TwitchAction.bitPrices.Find((int p) => p >= price);
			if (num == 0)
			{
				return TwitchAction.bitPrices[TwitchAction.bitPrices.Count - 1];
			}
			return num;
		}

		// Token: 0x0600BE13 RID: 48659 RVA: 0x004671E8 File Offset: 0x004653E8
		public static int GetAdjustedBitPriceFloor(int price)
		{
			return TwitchAction.bitPrices.FindLast((int p) => p <= price);
		}

		// Token: 0x0600BE14 RID: 48660 RVA: 0x00467218 File Offset: 0x00465418
		public static int GetAdjustedBitPriceFloorNoZero(int price)
		{
			return Mathf.Max(TwitchAction.GetAdjustedBitPriceFloor(price), TwitchAction.bitPrices[0]);
		}

		// Token: 0x0600BE15 RID: 48661 RVA: 0x00467230 File Offset: 0x00465430
		public int GetModifiedDiscountCost()
		{
			return TwitchAction.GetAdjustedBitPriceFloorNoZero((int)((float)this.ModifiedCost * TwitchManager.Current.BitPriceMultiplier));
		}

		// Token: 0x0600BE16 RID: 48662 RVA: 0x0046724C File Offset: 0x0046544C
		public void DecreaseCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.bitPrices[(int)MathUtils.Clamp((float)(TwitchAction.bitPrices.IndexOf(this.ModifiedCost) - 1), 0f, (float)(TwitchAction.bitPrices.Count - 1))];
				return;
			}
			if (this.ModifiedCost > 25)
			{
				this.ModifiedCost -= 25;
			}
		}

		// Token: 0x0600BE17 RID: 48663 RVA: 0x004672B8 File Offset: 0x004654B8
		public void IncreaseCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.bitPrices[(int)MathUtils.Clamp((float)(TwitchAction.bitPrices.IndexOf(this.ModifiedCost) + 1), 0f, (float)(TwitchAction.bitPrices.Count - 1))];
				return;
			}
			if (this.ModifiedCost < 2000)
			{
				this.ModifiedCost += 25;
			}
		}

		// Token: 0x0600BE18 RID: 48664 RVA: 0x00467326 File Offset: 0x00465526
		public void ResetToDefaultCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.GetAdjustedBitPriceFloorNoZero(this.DefaultCost);
				return;
			}
			this.ModifiedCost = this.DefaultCost;
		}

		// Token: 0x17001714 RID: 5908
		// (get) Token: 0x0600BE19 RID: 48665 RVA: 0x0046734F File Offset: 0x0046554F
		public bool CanUse
		{
			get
			{
				return this.Enabled && this.CheckAllowed();
			}
		}

		// Token: 0x17001715 RID: 5909
		// (get) Token: 0x0600BE1A RID: 48666 RVA: 0x00467361 File Offset: 0x00465561
		public bool RandomDaily
		{
			get
			{
				return this.RandomGroup != "";
			}
		}

		// Token: 0x17001716 RID: 5910
		// (get) Token: 0x0600BE1B RID: 48667 RVA: 0x00467373 File Offset: 0x00465573
		public bool SpecialOnly
		{
			get
			{
				return this.PointType == TwitchAction.PointTypes.SP;
			}
		}

		// Token: 0x0600BE1C RID: 48668 RVA: 0x00467380 File Offset: 0x00465580
		public bool CheckUsable(TwitchIRCClient.TwitchChatMessage message)
		{
			switch (this.OnlyUsableByType)
			{
			case TwitchAction.OnlyUsableTypes.Broadcaster:
				return message.isBroadcaster;
			case TwitchAction.OnlyUsableTypes.Mods:
				return message.isMod;
			case TwitchAction.OnlyUsableTypes.VIPs:
				return message.isVIP;
			case TwitchAction.OnlyUsableTypes.Subs:
				return message.isSub;
			case TwitchAction.OnlyUsableTypes.Name:
				return this.OnlyUsableBy.ContainsCaseInsensitive(message.UserName);
			default:
				return true;
			}
		}

		// Token: 0x0600BE1D RID: 48669 RVA: 0x004673E4 File Offset: 0x004655E4
		public void Init()
		{
			if (this.CategoryNames.Count > 0)
			{
				this.MainCategory = TwitchActionManager.Current.GetCategory(this.CategoryNames[this.CategoryNames.Count - 1]);
				if (this.DisplayCategory == null)
				{
					this.DisplayCategory = this.MainCategory;
				}
			}
			this.OnInit();
		}

		// Token: 0x0600BE1E RID: 48670 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600BE1F RID: 48671 RVA: 0x00467441 File Offset: 0x00465641
		public virtual TwitchActionEntry SetupActionEntry()
		{
			return new TwitchActionEntry();
		}

		// Token: 0x0600BE20 RID: 48672 RVA: 0x00467448 File Offset: 0x00465648
		public bool IsInPreset(TwitchActionPreset preset)
		{
			return ((!preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name)) || preset.AddedActions.Contains(this.Name)) && !preset.RemovedActions.Contains(this.Name);
		}

		// Token: 0x0600BE21 RID: 48673 RVA: 0x004674A1 File Offset: 0x004656A1
		public bool IsInPresetForList(TwitchActionPreset preset)
		{
			return (!preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name)) || preset.AddedActions.Contains(this.Name);
		}

		// Token: 0x0600BE22 RID: 48674 RVA: 0x004674D9 File Offset: 0x004656D9
		public bool IsInPresetDefault(TwitchActionPreset preset)
		{
			return !preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name);
		}

		// Token: 0x0600BE23 RID: 48675 RVA: 0x00467500 File Offset: 0x00465700
		public bool CanPerformAction(EntityPlayer target, TwitchActionEntry entry)
		{
			if (entry.Target == null)
			{
				entry.Target = target;
			}
			return this.OnPerformAction(target, entry);
		}

		// Token: 0x0600BE24 RID: 48676 RVA: 0x00467524 File Offset: 0x00465724
		public void SetQueued()
		{
			this.lastUse = Time.time;
			if (this.CooldownAdditions != null)
			{
				float actionCooldownModifier = TwitchManager.Current.ActionCooldownModifier;
				for (int i = 0; i < this.CooldownAdditions.Count; i++)
				{
					TwitchActionCooldownAddition twitchActionCooldownAddition = this.CooldownAdditions[i];
					if (twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchActions.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[twitchActionCooldownAddition.ActionName];
						twitchAction.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchAction.tempCooldownSet = Time.time;
					}
					else if (!twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchVotes.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchVote twitchVote = TwitchActionManager.TwitchVotes[twitchActionCooldownAddition.ActionName];
						twitchVote.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchVote.tempCooldownSet = Time.time;
					}
				}
			}
			if (this.VoteCooldownAddition != 0f)
			{
				TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
				votingManager.VoteStartDelayTimeRemaining = Math.Max(this.VoteCooldownAddition, votingManager.VoteStartDelayTimeRemaining);
			}
			if (this.SingleDayUse)
			{
				this.AllowedDay = -1;
			}
		}

		// Token: 0x0600BE25 RID: 48677 RVA: 0x00467640 File Offset: 0x00465840
		public virtual bool ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			properties.ParseString(TwitchAction.PropTitle, ref this.Title);
			if (properties.Values.ContainsKey(TwitchAction.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[TwitchAction.PropTitleKey], false, null);
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCommand))
			{
				this.BaseCommand = (this.Command = properties.Values[TwitchAction.PropCommand].ToLower());
				if (!Regex.IsMatch(this.Command, "^#[a-zA-Z0-9]+(_[a-zA-Z0-9]+)*$"))
				{
					return false;
				}
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCommandKey))
			{
				this.Command = Localization.Get(properties.Values[TwitchAction.PropCommandKey], false, null).ToLower();
				if (Localization.LocalizationChecks)
				{
					if (this.Command.StartsWith("l_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#l_");
					}
					else if (this.Command.StartsWith("ul_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#ul_");
					}
					else if (this.Command.StartsWith("le_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#le_");
					}
				}
				if (!Regex.IsMatch(this.Command, "^#[\\p{L}\\p{N}]+([-_][\\p{L}\\p{N}]+)*$"))
				{
					return false;
				}
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCategory))
			{
				this.CategoryNames.AddRange(properties.Values[TwitchAction.PropCategory].Split(',', StringSplitOptions.None));
			}
			if (properties.Values.ContainsKey(TwitchAction.PropDisplayCategory))
			{
				this.DisplayCategory = TwitchActionManager.Current.GetCategory(properties.Values[TwitchAction.PropDisplayCategory]);
			}
			properties.ParseString(TwitchAction.PropEventName, ref this.EventName);
			properties.ParseString(TwitchAction.PropDescription, ref this.Description);
			if (properties.Values.ContainsKey(TwitchAction.PropDescriptionKey))
			{
				this.Description = Localization.Get(properties.Values[TwitchAction.PropDescriptionKey], false, null);
			}
			properties.ParseInt(TwitchAction.PropDefaultCost, ref this.DefaultCost);
			properties.ParseInt(TwitchAction.PropStartGameStage, ref this.StartGameStage);
			properties.ParseBool(TwitchAction.PropIsPositive, ref this.IsPositive);
			bool flag = false;
			properties.ParseBool(TwitchAction.PropSpecialOnly, ref flag);
			if (flag)
			{
				this.PointType = TwitchAction.PointTypes.SP;
			}
			properties.ParseBool(TwitchAction.PropAddCooldown, ref this.AddsToCooldown);
			properties.ParseInt(TwitchAction.PropCooldownAddAmount, ref this.CooldownAddAmount);
			properties.ParseBool(TwitchAction.PropCooldownBlocked, ref this.CooldownBlocked);
			properties.ParseBool(TwitchAction.PropWaitingBlocked, ref this.WaitingBlocked);
			if (properties.Values.ContainsKey(TwitchAction.PropCooldown))
			{
				this.OriginalCooldown = (this.Cooldown = StringParsers.ParseFloat(properties.Values[TwitchAction.PropCooldown], 0, -1, NumberStyles.Any));
				this.lastUse = Time.time - this.Cooldown;
				this.ModifiedCooldown = this.Cooldown;
			}
			properties.ParseBool(TwitchAction.PropEnabled, ref this.Enabled);
			this.OriginalEnabled = this.Enabled;
			properties.ParseBool(TwitchAction.PropSingleDayUse, ref this.SingleDayUse);
			properties.ParseString(TwitchAction.PropRandomGroup, ref this.RandomGroup);
			properties.ParseBool(TwitchAction.PropShowInActionList, ref this.ShowInActionList);
			if (properties.Values.ContainsKey(TwitchAction.PropSpecialRequirement))
			{
				string[] array = this.Properties.Values[TwitchAction.PropSpecialRequirement].Split(',', StringSplitOptions.None);
				this.SpecialRequirementList = new List<TwitchAction.SpecialRequirements>();
				foreach (string text in array)
				{
					TwitchAction.SpecialRequirements item = TwitchAction.SpecialRequirements.None;
					if (Enum.TryParse<TwitchAction.SpecialRequirements>(text, true, out item))
					{
						this.SpecialRequirementList.Add(item);
					}
					else
					{
						Log.Error("TwitchAction " + this.Title + " has unknown SpecialRequirement " + text);
					}
				}
			}
			properties.ParseString(TwitchAction.PropReplaces, ref this.Replaces);
			properties.ParseBool(TwitchAction.PropTwitchNotify, ref this.TwitchNotify);
			properties.ParseBool(TwitchAction.PropDelayNotify, ref this.DelayNotify);
			properties.ParseBool(TwitchAction.PropPlayBitSound, ref this.PlayBitSound);
			properties.ParseBool(TwitchAction.PropHideOnDisable, ref this.HideOnDisable);
			properties.ParseBool(TwitchAction.PropIgnoreCooldown, ref this.IgnoreCooldown);
			properties.ParseBool(TwitchAction.PropIgnoreDiscount, ref this.IgnoreDiscount);
			properties.ParseBool(TwitchAction.PropStreamerOnly, ref this.StreamerOnly);
			properties.ParseEnum<TwitchAction.OnlyUsableTypes>(TwitchAction.PropOnlyUsableByType, ref this.OnlyUsableByType);
			string text2 = "";
			properties.ParseString(TwitchAction.PropOnlyUsableBy, ref text2);
			if (text2 != "")
			{
				this.OnlyUsableBy = text2.Split(',', StringSplitOptions.None);
			}
			properties.ParseEnum<TwitchAction.PointTypes>(TwitchAction.PropPointTypes, ref this.PointType);
			this.ResetToDefaultCost();
			this.UpdateCost(1f);
			if (this.CooldownAddAmount == -1)
			{
				this.CooldownAddAmount = this.DefaultCost;
			}
			properties.ParseFloat(TwitchAction.PropVoteCooldownAddition, ref this.VoteCooldownAddition);
			if (properties.Values.ContainsKey(TwitchAction.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchAction.PropPresets].Split(',', StringSplitOptions.None));
			}
			properties.ParseBool(TwitchAction.PropOnlyShowInPreset, ref this.OnlyShowInPreset);
			if (properties.Contains(TwitchAction.PropMinRespawnCount) || properties.Contains(TwitchAction.PropMaxRespawnCount))
			{
				properties.ParseInt(TwitchAction.PropMinRespawnCount, ref this.MinRespawnCount);
				properties.ParseInt(TwitchAction.PropMaxRespawnCount, ref this.MaxRespawnCount);
				this.RespawnCountType = TwitchAction.RespawnCountTypes.Both;
			}
			else
			{
				this.RespawnCountType = TwitchAction.RespawnCountTypes.None;
			}
			properties.ParseEnum<TwitchAction.RespawnCountTypes>(TwitchAction.PropRespawnCountType, ref this.RespawnCountType);
			properties.ParseInt(TwitchAction.PropRespawnThreshold, ref this.RespawnThreshold);
			return true;
		}

		// Token: 0x0600BE26 RID: 48678 RVA: 0x00467C18 File Offset: 0x00465E18
		public bool UpdateCost(float bitPriceModifier = 1f)
		{
			int currentCost = this.CurrentCost;
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				if (!this.IgnoreDiscount && bitPriceModifier != 1f)
				{
					this.CurrentCost = TwitchAction.GetAdjustedBitPriceFloorNoZero((int)((float)this.ModifiedCost * bitPriceModifier));
				}
				else
				{
					this.CurrentCost = TwitchAction.GetAdjustedBitPriceFloorNoZero(this.ModifiedCost);
				}
			}
			else
			{
				this.CurrentCost = this.ModifiedCost;
			}
			return currentCost != this.CurrentCost;
		}

		// Token: 0x0600BE27 RID: 48679 RVA: 0x00467C85 File Offset: 0x00465E85
		public void AddRequirement(BaseTwitchRequirement requirement)
		{
			if (this.TwitchRequirements == null)
			{
				this.TwitchRequirements = new List<BaseTwitchRequirement>();
			}
			requirement.OwnerAction = this;
			requirement.OwnerType = BaseTwitchRequirement.OwnerTypes.Action;
			this.TwitchRequirements.Add(requirement);
		}

		// Token: 0x0600BE28 RID: 48680 RVA: 0x00467CB4 File Offset: 0x00465EB4
		public bool CheckAllowed()
		{
			if (this.TwitchRequirements != null)
			{
				EntityPlayerLocal localPlayer = TwitchManager.Current.LocalPlayer;
				for (int i = 0; i < this.TwitchRequirements.Count; i++)
				{
					BaseTwitchRequirement baseTwitchRequirement = this.TwitchRequirements[i];
					if (baseTwitchRequirement.HideAction && !baseTwitchRequirement.CanPerform(localPlayer))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600BE29 RID: 48681 RVA: 0x00467D0C File Offset: 0x00465F0C
		public virtual bool IsReady(TwitchManager twitchManager)
		{
			if (this.TwitchRequirements != null)
			{
				EntityPlayerLocal localPlayer = twitchManager.LocalPlayer;
				for (int i = 0; i < this.TwitchRequirements.Count; i++)
				{
					BaseTwitchRequirement baseTwitchRequirement = this.TwitchRequirements[i];
					if (!baseTwitchRequirement.HideAction && !baseTwitchRequirement.CanPerform(localPlayer))
					{
						return false;
					}
				}
			}
			if (this.SpecialRequirementList != null)
			{
				for (int j = 0; j < this.SpecialRequirementList.Count; j++)
				{
					switch (this.SpecialRequirementList[j])
					{
					case TwitchAction.SpecialRequirements.HasSpawnedEntities:
						if (twitchManager.actionSpawnLiveList.Count == 0)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NoSpawnedEntities:
						if (twitchManager.actionSpawnLiveList.Count > 0)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Bloodmoon:
						if (!twitchManager.isBMActive)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotBloodmoon:
						if (twitchManager.isBMActive)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotBloodmoonDay:
						if (SkyManager.IsBloodMoonVisible())
						{
							return false;
						}
						if (GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime) == GameStats.GetInt(EnumGameStats.BloodMoonDay))
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.EarlyDay:
					{
						int num = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if ((float)num > SkyManager.GetDuskTime() - 5f || (float)num < SkyManager.GetDawnTime())
						{
							return false;
						}
						break;
					}
					case TwitchAction.SpecialRequirements.Daytime:
					{
						int num2 = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if ((float)num2 > SkyManager.GetDuskTime() || (float)num2 < SkyManager.GetDawnTime())
						{
							return false;
						}
						break;
					}
					case TwitchAction.SpecialRequirements.Night:
						GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if (!SkyManager.IsDark())
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotAllDaySetting:
						if (SkyManager.isAllTimeDay || SkyManager.isAllTimeDay)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.IsCooldown:
						if (!TwitchManager.HasInstance || !twitchManager.IsReady)
						{
							return false;
						}
						if (twitchManager.CurrentCooldownPreset.CooldownType != CooldownPreset.CooldownTypes.Fill)
						{
							return false;
						}
						if (twitchManager.CooldownType != TwitchManager.CooldownTypes.MaxReached)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.InLandClaim:
						if (!twitchManager.LocalPlayerInLandClaim)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotInLandClaim:
						if (twitchManager.LocalPlayerInLandClaim)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Safe:
						if (!twitchManager.LocalPlayer.TwitchSafe)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotSafe:
						if (twitchManager.LocalPlayer.TwitchSafe)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NoFullProgression:
						if (!twitchManager.IsReady)
						{
							return false;
						}
						if (!twitchManager.UseProgression || twitchManager.OverrideProgession)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotOnVehicle:
						if (!twitchManager.IsReady)
						{
							return false;
						}
						if (twitchManager.LocalPlayer.AttachedToEntity != null)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotInTrader:
						if (twitchManager.LocalPlayer.IsInTrader)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Encumbrance:
						if ((int)EffectManager.GetValue(PassiveEffects.CarryCapacity, null, 0f, twitchManager.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) <= 30)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.WeatherGracePeriod:
						if (GameManager.Instance.World.GetWorldTime() <= 30000UL)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotOnQuest:
						if (QuestEventManager.Current.QuestBounds.width != 0f)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.OnQuest:
						if (QuestEventManager.Current.QuestBounds.width == 0f)
						{
							return false;
						}
						break;
					}
				}
			}
			bool flag = (!this.SingleDayUse && !this.RandomDaily) || this.AllowedDay == GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
			if (this.tempCooldown > 0f && twitchManager.CurrentUnityTime - this.tempCooldownSet < this.tempCooldown)
			{
				return false;
			}
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
			return twitchManager.CurrentUnityTime - this.lastUse >= this.ModifiedCooldown && flag;
		}

		// Token: 0x0600BE2A RID: 48682 RVA: 0x004680B3 File Offset: 0x004662B3
		public void UpdateModifiedCooldown(float modifier)
		{
			this.ModifiedCooldown = modifier * this.Cooldown;
		}

		// Token: 0x0600BE2B RID: 48683 RVA: 0x00010E62 File Offset: 0x0000F062
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool OnPerformAction(EntityPlayer Target, TwitchActionEntry entry)
		{
			return false;
		}

		// Token: 0x0600BE2C RID: 48684 RVA: 0x004680C3 File Offset: 0x004662C3
		public void AddCooldownAddition(TwitchActionCooldownAddition newCooldown)
		{
			if (this.CooldownAdditions == null)
			{
				this.CooldownAdditions = new List<TwitchActionCooldownAddition>();
			}
			this.CooldownAdditions.Add(newCooldown);
		}

		// Token: 0x0600BE2D RID: 48685 RVA: 0x004680E4 File Offset: 0x004662E4
		public void ResetCooldown(float currentUnityTime)
		{
			this.lastUse = currentUnityTime - this.ModifiedCooldown;
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
		}

		// Token: 0x0600BE2E RID: 48686 RVA: 0x0046810A File Offset: 0x0046630A
		public void SetCooldown(float currentUnityTime, float newCooldownTime)
		{
			this.lastUse = currentUnityTime - this.ModifiedCooldown;
			this.tempCooldown = newCooldownTime;
			this.tempCooldownSet = currentUnityTime;
		}

		// Token: 0x04008EC9 RID: 36553
		public string Name = "";

		// Token: 0x04008ECA RID: 36554
		public string Title = "";

		// Token: 0x04008ECB RID: 36555
		public string BaseCommand = "";

		// Token: 0x04008ECC RID: 36556
		public string Command = "";

		// Token: 0x04008ECD RID: 36557
		public string EventName = "";

		// Token: 0x04008ECE RID: 36558
		public List<string> CategoryNames = new List<string>();

		// Token: 0x04008ECF RID: 36559
		public int DefaultCost = 5;

		// Token: 0x04008ED0 RID: 36560
		public int ModifiedCost = 5;

		// Token: 0x04008ED1 RID: 36561
		public int CurrentCost = 5;

		// Token: 0x04008ED2 RID: 36562
		public int StartGameStage;

		// Token: 0x04008ED3 RID: 36563
		public string Description;

		// Token: 0x04008ED4 RID: 36564
		public float OriginalCooldown;

		// Token: 0x04008ED5 RID: 36565
		public float Cooldown;

		// Token: 0x04008ED6 RID: 36566
		public bool IsPositive;

		// Token: 0x04008ED7 RID: 36567
		public bool AddsToCooldown;

		// Token: 0x04008ED8 RID: 36568
		public int CooldownAddAmount = -1;

		// Token: 0x04008ED9 RID: 36569
		public bool CooldownBlocked;

		// Token: 0x04008EDA RID: 36570
		public bool WaitingBlocked;

		// Token: 0x04008EDB RID: 36571
		public bool OnCooldown;

		// Token: 0x04008EDC RID: 36572
		public bool Enabled = true;

		// Token: 0x04008EDD RID: 36573
		public bool SingleDayUse;

		// Token: 0x04008EDE RID: 36574
		public bool DelayNotify;

		// Token: 0x04008EDF RID: 36575
		public bool TwitchNotify = true;

		// Token: 0x04008EE0 RID: 36576
		public bool PlayBitSound = true;

		// Token: 0x04008EE1 RID: 36577
		public string RandomGroup = "";

		// Token: 0x04008EE2 RID: 36578
		public string Replaces = "";

		// Token: 0x04008EE3 RID: 36579
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastUse;

		// Token: 0x04008EE4 RID: 36580
		public int AllowedDay = -1;

		// Token: 0x04008EE5 RID: 36581
		public int groupIndex;

		// Token: 0x04008EE6 RID: 36582
		public float tempCooldownSet;

		// Token: 0x04008EE7 RID: 36583
		public float tempCooldown;

		// Token: 0x04008EE8 RID: 36584
		public bool IgnoreCooldown;

		// Token: 0x04008EE9 RID: 36585
		public bool IgnoreDiscount;

		// Token: 0x04008EEA RID: 36586
		public float ModifiedCooldown;

		// Token: 0x04008EEB RID: 36587
		public List<TwitchActionCooldownAddition> CooldownAdditions;

		// Token: 0x04008EEC RID: 36588
		public bool ShowInActionList = true;

		// Token: 0x04008EED RID: 36589
		public List<TwitchAction.SpecialRequirements> SpecialRequirementList;

		// Token: 0x04008EEE RID: 36590
		public List<BaseTwitchRequirement> TwitchRequirements;

		// Token: 0x04008EEF RID: 36591
		public bool HideOnDisable;

		// Token: 0x04008EF0 RID: 36592
		public TwitchAction.OnlyUsableTypes OnlyUsableByType;

		// Token: 0x04008EF1 RID: 36593
		public string[] OnlyUsableBy;

		// Token: 0x04008EF2 RID: 36594
		public bool OriginalEnabled = true;

		// Token: 0x04008EF3 RID: 36595
		public TwitchAction.PointTypes PointType;

		// Token: 0x04008EF4 RID: 36596
		public TwitchActionManager.ActionCategory MainCategory;

		// Token: 0x04008EF5 RID: 36597
		public TwitchActionManager.ActionCategory DisplayCategory;

		// Token: 0x04008EF6 RID: 36598
		public List<string> PresetNames;

		// Token: 0x04008EF7 RID: 36599
		public bool OnlyShowInPreset;

		// Token: 0x04008EF8 RID: 36600
		public float VoteCooldownAddition;

		// Token: 0x04008EF9 RID: 36601
		public bool StreamerOnly;

		// Token: 0x04008EFA RID: 36602
		public TwitchAction.RespawnCountTypes RespawnCountType;

		// Token: 0x04008EFB RID: 36603
		public int MinRespawnCount = -1;

		// Token: 0x04008EFC RID: 36604
		public int MaxRespawnCount = -1;

		// Token: 0x04008EFD RID: 36605
		public int RespawnThreshold;

		// Token: 0x04008EFE RID: 36606
		public DynamicProperties Properties;

		// Token: 0x04008EFF RID: 36607
		public static string PropCommand = "command";

		// Token: 0x04008F00 RID: 36608
		public static string PropCommandKey = "command_key";

		// Token: 0x04008F01 RID: 36609
		public static string PropTitle = "title";

		// Token: 0x04008F02 RID: 36610
		public static string PropTitleKey = "title_key";

		// Token: 0x04008F03 RID: 36611
		public static string PropCategory = "category";

		// Token: 0x04008F04 RID: 36612
		public static string PropDisplayCategory = "display_category";

		// Token: 0x04008F05 RID: 36613
		public static string PropEventName = "event_name";

		// Token: 0x04008F06 RID: 36614
		public static string PropDefaultCost = "default_cost";

		// Token: 0x04008F07 RID: 36615
		public static string PropDescription = "description";

		// Token: 0x04008F08 RID: 36616
		public static string PropDescriptionKey = "description_key";

		// Token: 0x04008F09 RID: 36617
		public static string PropStartGameStage = "start_gamestage";

		// Token: 0x04008F0A RID: 36618
		public static string PropCooldown = "cooldown";

		// Token: 0x04008F0B RID: 36619
		public static string PropIsPositive = "is_positive";

		// Token: 0x04008F0C RID: 36620
		public static string PropSpecialOnly = "special_only";

		// Token: 0x04008F0D RID: 36621
		public static string PropAddCooldown = "add_cooldown";

		// Token: 0x04008F0E RID: 36622
		public static string PropCooldownAddAmount = "cooldown_add_amount";

		// Token: 0x04008F0F RID: 36623
		public static string PropCooldownBlocked = "cooldown_blocked";

		// Token: 0x04008F10 RID: 36624
		public static string PropWaitingBlocked = "waiting_blocked";

		// Token: 0x04008F11 RID: 36625
		public static string PropEnabled = "enabled";

		// Token: 0x04008F12 RID: 36626
		public static string PropSingleDayUse = "single_day";

		// Token: 0x04008F13 RID: 36627
		public static string PropRandomGroup = "random_group";

		// Token: 0x04008F14 RID: 36628
		public static string PropReplaces = "replaces";

		// Token: 0x04008F15 RID: 36629
		public static string PropShowInActionList = "show_in_action_list";

		// Token: 0x04008F16 RID: 36630
		public static string PropPlayBitSound = "play_bit_sound";

		// Token: 0x04008F17 RID: 36631
		public static string PropSpecialRequirement = "special_requirement";

		// Token: 0x04008F18 RID: 36632
		public static string PropTwitchNotify = "twitch_notify";

		// Token: 0x04008F19 RID: 36633
		public static string PropDelayNotify = "delay_notify";

		// Token: 0x04008F1A RID: 36634
		public static string PropHideOnDisable = "hide_on_disable";

		// Token: 0x04008F1B RID: 36635
		public static string PropOnlyUsableByType = "only_usable_type";

		// Token: 0x04008F1C RID: 36636
		public static string PropOnlyUsableBy = "only_usable_by";

		// Token: 0x04008F1D RID: 36637
		public static string PropIgnoreCooldown = "ignore_cooldown";

		// Token: 0x04008F1E RID: 36638
		public static string PropIgnoreDiscount = "ignore_discount";

		// Token: 0x04008F1F RID: 36639
		public static string PropPointTypes = "point_type";

		// Token: 0x04008F20 RID: 36640
		public static string PropPresets = "presets";

		// Token: 0x04008F21 RID: 36641
		public static string PropOnlyShowInPreset = "only_show_in_preset";

		// Token: 0x04008F22 RID: 36642
		public static string PropVoteCooldownAddition = "vote_cooldown_add";

		// Token: 0x04008F23 RID: 36643
		public static string PropStreamerOnly = "streamer_only";

		// Token: 0x04008F24 RID: 36644
		public static string PropMinRespawnCount = "min_respawn_count";

		// Token: 0x04008F25 RID: 36645
		public static string PropMaxRespawnCount = "max_respawn_count";

		// Token: 0x04008F26 RID: 36646
		public static string PropRespawnCountType = "respawn_count_type";

		// Token: 0x04008F27 RID: 36647
		public static string PropRespawnThreshold = "respawn_threshold";

		// Token: 0x04008F28 RID: 36648
		public static HashSet<string> ExtendsExcludes = new HashSet<string>
		{
			TwitchAction.PropShowInActionList,
			TwitchAction.PropCommandKey,
			TwitchAction.PropCommand,
			TwitchAction.PropTitleKey,
			TwitchAction.PropDescriptionKey
		};

		// Token: 0x04008F29 RID: 36649
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<int> bitPrices = new List<int>
		{
			10,
			15,
			20,
			25,
			30,
			35,
			40,
			45,
			50,
			55,
			60,
			65,
			70,
			75,
			80,
			85,
			90,
			95,
			100,
			105,
			110,
			115,
			120,
			125,
			130,
			135,
			140,
			145,
			150,
			155,
			160,
			165,
			170,
			175,
			180,
			185,
			190,
			195,
			200,
			250,
			300,
			350,
			400,
			450,
			500,
			550,
			600,
			650,
			700,
			750,
			800,
			850,
			900,
			950,
			1000,
			1100,
			1200,
			1250,
			1300,
			1400,
			1500,
			1600,
			1700,
			1750,
			1800,
			1900,
			2000,
			2250,
			2500,
			2750,
			3000,
			3500,
			4000,
			4500,
			5000,
			5500,
			6000,
			6500,
			7000,
			7500,
			8000,
			8500,
			9000,
			9500,
			10000
		};

		// Token: 0x020017F9 RID: 6137
		public enum SpecialRequirements
		{
			// Token: 0x04008F2B RID: 36651
			None,
			// Token: 0x04008F2C RID: 36652
			HasSpawnedEntities,
			// Token: 0x04008F2D RID: 36653
			NoSpawnedEntities,
			// Token: 0x04008F2E RID: 36654
			Bloodmoon,
			// Token: 0x04008F2F RID: 36655
			NotBloodmoon,
			// Token: 0x04008F30 RID: 36656
			NotBloodmoonDay,
			// Token: 0x04008F31 RID: 36657
			EarlyDay,
			// Token: 0x04008F32 RID: 36658
			Daytime,
			// Token: 0x04008F33 RID: 36659
			Night,
			// Token: 0x04008F34 RID: 36660
			NotAllDaySetting,
			// Token: 0x04008F35 RID: 36661
			IsCooldown,
			// Token: 0x04008F36 RID: 36662
			InLandClaim,
			// Token: 0x04008F37 RID: 36663
			NotInLandClaim,
			// Token: 0x04008F38 RID: 36664
			Safe,
			// Token: 0x04008F39 RID: 36665
			NotSafe,
			// Token: 0x04008F3A RID: 36666
			NoFullProgression,
			// Token: 0x04008F3B RID: 36667
			NotOnVehicle,
			// Token: 0x04008F3C RID: 36668
			NotInTrader,
			// Token: 0x04008F3D RID: 36669
			Encumbrance,
			// Token: 0x04008F3E RID: 36670
			WeatherGracePeriod,
			// Token: 0x04008F3F RID: 36671
			NotOnQuest,
			// Token: 0x04008F40 RID: 36672
			OnQuest
		}

		// Token: 0x020017FA RID: 6138
		public enum OnlyUsableTypes
		{
			// Token: 0x04008F42 RID: 36674
			Everyone,
			// Token: 0x04008F43 RID: 36675
			Broadcaster,
			// Token: 0x04008F44 RID: 36676
			Mods,
			// Token: 0x04008F45 RID: 36677
			VIPs,
			// Token: 0x04008F46 RID: 36678
			Subs,
			// Token: 0x04008F47 RID: 36679
			Name
		}

		// Token: 0x020017FB RID: 6139
		public enum PointTypes
		{
			// Token: 0x04008F49 RID: 36681
			PP,
			// Token: 0x04008F4A RID: 36682
			SP,
			// Token: 0x04008F4B RID: 36683
			Bits
		}

		// Token: 0x020017FC RID: 6140
		public enum RespawnCountTypes
		{
			// Token: 0x04008F4D RID: 36685
			None,
			// Token: 0x04008F4E RID: 36686
			Both,
			// Token: 0x04008F4F RID: 36687
			BlocksOnly,
			// Token: 0x04008F50 RID: 36688
			SpawnsOnly
		}
	}
}
