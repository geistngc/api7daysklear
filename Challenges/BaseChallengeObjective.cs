using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018FB RID: 6395
	[Preserve]
	public class BaseChallengeObjective
	{
		// Token: 0x1700184C RID: 6220
		// (get) Token: 0x0600C511 RID: 50449 RVA: 0x0048CB4A File Offset: 0x0048AD4A
		// (set) Token: 0x0600C512 RID: 50450 RVA: 0x0048CB52 File Offset: 0x0048AD52
		public byte CurrentFileVersion { get; set; }

		// Token: 0x1700184D RID: 6221
		// (get) Token: 0x0600C513 RID: 50451 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Invalid;
			}
		}

		// Token: 0x1700184E RID: 6222
		// (get) Token: 0x0600C514 RID: 50452 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual ChallengeClass.UINavTypes NavType
		{
			get
			{
				return ChallengeClass.UINavTypes.None;
			}
		}

		// Token: 0x1700184F RID: 6223
		// (get) Token: 0x0600C515 RID: 50453 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool NeedsConstantUIUpdate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001850 RID: 6224
		// (get) Token: 0x0600C516 RID: 50454 RVA: 0x0048CB5B File Offset: 0x0048AD5B
		// (set) Token: 0x0600C517 RID: 50455 RVA: 0x0048CB63 File Offset: 0x0048AD63
		public bool Complete
		{
			get
			{
				return this.complete;
			}
			set
			{
				if (this.complete != value)
				{
					this.complete = value;
					this.HandleValueChanged();
				}
			}
		}

		// Token: 0x17001851 RID: 6225
		// (get) Token: 0x0600C518 RID: 50456 RVA: 0x0048CB7B File Offset: 0x0048AD7B
		// (set) Token: 0x0600C519 RID: 50457 RVA: 0x0048CB83 File Offset: 0x0048AD83
		public int Current
		{
			get
			{
				return this.current;
			}
			set
			{
				if (this.current != value)
				{
					this.current = value;
					this.HandleValueChanged();
				}
			}
		}

		// Token: 0x0600C51A RID: 50458 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void BaseInit()
		{
		}

		// Token: 0x17001852 RID: 6226
		// (get) Token: 0x0600C51B RID: 50459 RVA: 0x0048CB9B File Offset: 0x0048AD9B
		public EntityPlayerLocal Player
		{
			get
			{
				return this.Owner.Owner.Player;
			}
		}

		// Token: 0x0600C51C RID: 50460 RVA: 0x0048CBAD File Offset: 0x0048ADAD
		public void ResetComplete()
		{
			this.Complete = false;
			this.current = 0;
		}

		// Token: 0x14000118 RID: 280
		// (add) Token: 0x0600C51D RID: 50461 RVA: 0x0048CBC0 File Offset: 0x0048ADC0
		// (remove) Token: 0x0600C51E RID: 50462 RVA: 0x0048CBF8 File Offset: 0x0048ADF8
		public event ObjectiveValueChanged ValueChanged;

		// Token: 0x17001853 RID: 6227
		// (get) Token: 0x0600C51F RID: 50463 RVA: 0x0048CC2D File Offset: 0x0048AE2D
		public string ObjectiveText
		{
			get
			{
				return string.Format("{0} {1}", this.DescriptionText, this.StatusText);
			}
		}

		// Token: 0x17001854 RID: 6228
		// (get) Token: 0x0600C520 RID: 50464 RVA: 0x00032163 File Offset: 0x00030363
		public virtual string DescriptionText
		{
			get
			{
				return "";
			}
		}

		// Token: 0x17001855 RID: 6229
		// (get) Token: 0x0600C521 RID: 50465 RVA: 0x0048CC45 File Offset: 0x0048AE45
		public virtual string StatusText
		{
			get
			{
				if (this.Owner != null && this.Owner.AutoCompleted)
				{
					return "--";
				}
				return string.Format("{0}/{1}", this.current, this.MaxCount);
			}
		}

		// Token: 0x17001856 RID: 6230
		// (get) Token: 0x0600C522 RID: 50466 RVA: 0x0048CC82 File Offset: 0x0048AE82
		public virtual float FillAmount
		{
			get
			{
				return (float)this.current / (float)this.MaxCount;
			}
		}

		// Token: 0x0600C523 RID: 50467 RVA: 0x0048CC93 File Offset: 0x0048AE93
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged();
			}
		}

		// Token: 0x0600C524 RID: 50468 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Init()
		{
		}

		// Token: 0x0600C525 RID: 50469 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void HandleOnCreated()
		{
		}

		// Token: 0x0600C526 RID: 50470 RVA: 0x0048CCA8 File Offset: 0x0048AEA8
		public virtual bool HandleCheckStatus()
		{
			this.Complete = this.CheckObjectiveComplete(false);
			return this.Complete;
		}

		// Token: 0x0600C527 RID: 50471 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void UpdateStatus()
		{
		}

		// Token: 0x0600C528 RID: 50472 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void HandleAddHooks()
		{
		}

		// Token: 0x0600C529 RID: 50473 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void HandleRemoveHooks()
		{
		}

		// Token: 0x0600C52A RID: 50474 RVA: 0x0048CCBD File Offset: 0x0048AEBD
		public virtual void HandleTrackingStarted()
		{
			this.IsTracking = true;
		}

		// Token: 0x0600C52B RID: 50475 RVA: 0x0048CCC8 File Offset: 0x0048AEC8
		public virtual void CopyValues(BaseChallengeObjective obj, BaseChallengeObjective objFromClass)
		{
			this.current = obj.current;
			this.MaxCount = objFromClass.MaxCount;
			this.ShowRequirements = objFromClass.ShowRequirements;
			this.Biome = objFromClass.Biome;
			this.complete = (this.Current >= this.MaxCount);
		}

		// Token: 0x0600C52C RID: 50476 RVA: 0x0048CD1C File Offset: 0x0048AF1C
		public virtual void HandleTrackingEnded()
		{
			this.IsTracking = false;
		}

		// Token: 0x0600C52D RID: 50477 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleUpdatingCurrent()
		{
		}

		// Token: 0x0600C52E RID: 50478 RVA: 0x0048CD25 File Offset: 0x0048AF25
		public void HandleAutoComplete()
		{
			this.current = this.MaxCount;
			this.Complete = true;
		}

		// Token: 0x0600C52F RID: 50479 RVA: 0x0048CD3C File Offset: 0x0048AF3C
		public static BaseChallengeObjective ReadObjective(byte _currentVersion, ChallengeObjectiveType _type, BinaryReader _br)
		{
			BaseChallengeObjective baseChallengeObjective = null;
			switch (_type)
			{
			case ChallengeObjectiveType.BlockPlace:
				baseChallengeObjective = new ChallengeObjectiveBlockPlace();
				break;
			case ChallengeObjectiveType.BlockUpgrade:
				baseChallengeObjective = new ChallengeObjectiveBlockUpgrade();
				break;
			case ChallengeObjectiveType.Bloodmoon:
				baseChallengeObjective = new ChallengeObjectiveBloodmoon();
				break;
			case ChallengeObjectiveType.Craft:
				baseChallengeObjective = new ChallengeObjectiveCraft();
				break;
			case ChallengeObjectiveType.CureDebuff:
				baseChallengeObjective = new ChallengeObjectiveCureDebuff();
				break;
			case ChallengeObjectiveType.EnterBiome:
				baseChallengeObjective = new ChallengeObjectiveEnterBiome();
				break;
			case ChallengeObjectiveType.Gather:
				baseChallengeObjective = new ChallengeObjectiveGather();
				break;
			case ChallengeObjectiveType.GatherIngredient:
				baseChallengeObjective = new ChallengeObjectiveGatherIngredient();
				break;
			case ChallengeObjectiveType.Harvest:
				baseChallengeObjective = new ChallengeObjectiveHarvest();
				break;
			case ChallengeObjectiveType.Hold:
				baseChallengeObjective = new ChallengeObjectiveHold();
				break;
			case ChallengeObjectiveType.Kill:
				baseChallengeObjective = new ChallengeObjectiveKill();
				break;
			case ChallengeObjectiveType.QuestComplete:
				baseChallengeObjective = new ChallengeObjectiveQuestComplete();
				break;
			case ChallengeObjectiveType.Scrap:
				baseChallengeObjective = new ChallengeObjectiveScrap();
				break;
			case ChallengeObjectiveType.Survive:
				baseChallengeObjective = new ChallengeObjectiveSurvive();
				break;
			case ChallengeObjectiveType.Trader:
				baseChallengeObjective = new ChallengeObjectiveTrader();
				break;
			case ChallengeObjectiveType.Wear:
				baseChallengeObjective = new ChallengeObjectiveWear();
				break;
			case ChallengeObjectiveType.Use:
				baseChallengeObjective = new ChallengeObjectiveUseItem();
				break;
			case ChallengeObjectiveType.ChallengeComplete:
				baseChallengeObjective = new ChallengeObjectiveChallengeComplete();
				break;
			case ChallengeObjectiveType.MeetTrader:
				baseChallengeObjective = new ChallengeObjectiveMeetTrader();
				break;
			case ChallengeObjectiveType.KillByTag:
				baseChallengeObjective = new ChallengeObjectiveKillByTag();
				break;
			case ChallengeObjectiveType.ChallengeStatAwarded:
				baseChallengeObjective = new ChallengeObjectiveChallengeStatAwarded();
				break;
			case ChallengeObjectiveType.SpendSkillPoint:
				baseChallengeObjective = new ChallengeObjectiveSpendSkillPoint();
				break;
			case ChallengeObjectiveType.Twitch:
				baseChallengeObjective = new ChallengeObjectiveTwitch();
				break;
			case ChallengeObjectiveType.Time:
				baseChallengeObjective = new ChallengeObjectiveTime();
				break;
			case ChallengeObjectiveType.GatherByTag:
				baseChallengeObjective = new ChallengeObjectiveGatherByTag();
				break;
			case ChallengeObjectiveType.LootContainer:
				baseChallengeObjective = new ChallengeObjectiveLootContainer();
				break;
			}
			if (baseChallengeObjective != null)
			{
				baseChallengeObjective.Read(_currentVersion, _br);
			}
			return baseChallengeObjective;
		}

		// Token: 0x0600C530 RID: 50480 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual Recipe GetRecipeItem()
		{
			return null;
		}

		// Token: 0x0600C531 RID: 50481 RVA: 0x0048CEBC File Offset: 0x0048B0BC
		public virtual Recipe[] GetRecipeItems()
		{
			if (this.Owner.NeedsPreRequisites)
			{
				Recipe recipeFromRequirements = this.Owner.GetRecipeFromRequirements();
				if (recipeFromRequirements != null)
				{
					return new Recipe[]
					{
						recipeFromRequirements
					};
				}
			}
			return null;
		}

		// Token: 0x0600C532 RID: 50482 RVA: 0x0048CEF1 File Offset: 0x0048B0F1
		public virtual void Read(byte _currentVersion, BinaryReader _br)
		{
			this.current = _br.ReadInt32();
		}

		// Token: 0x0600C533 RID: 50483 RVA: 0x0048CEFF File Offset: 0x0048B0FF
		public void WriteObjective(BinaryWriter _bw)
		{
			_bw.Write((byte)this.ObjectiveType);
			this.Write(_bw);
		}

		// Token: 0x0600C534 RID: 50484 RVA: 0x0048CF14 File Offset: 0x0048B114
		public virtual void Write(BinaryWriter _bw)
		{
			_bw.Write(this.current);
		}

		// Token: 0x0600C535 RID: 50485 RVA: 0x0048CF24 File Offset: 0x0048B124
		public virtual bool CheckObjectiveComplete(bool handleComplete = true)
		{
			this.HandleUpdatingCurrent();
			if (this.Current >= this.MaxCount)
			{
				this.Current = this.MaxCount;
				this.Complete = true;
				if (handleComplete)
				{
					this.Owner.HandleComplete(true, false);
				}
				return true;
			}
			if (handleComplete)
			{
				this.Owner.HandleComplete(true, false);
			}
			this.Complete = false;
			return false;
		}

		// Token: 0x0600C536 RID: 50486 RVA: 0x0048CF84 File Offset: 0x0048B184
		public virtual bool CheckBaseRequirements()
		{
			return this.Owner.ChallengeGroup.IsActive && (this.Biome != "" && (this.Owner.Owner.Player.biomeStandingOn == null || this.Owner.Owner.Player.biomeStandingOn.m_sBiomeName != this.Biome));
		}

		// Token: 0x0600C537 RID: 50487 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual BaseChallengeObjective Clone()
		{
			return null;
		}

		// Token: 0x0600C538 RID: 50488 RVA: 0x0048CFF8 File Offset: 0x0048B1F8
		public virtual void ParseElement(XElement e)
		{
			if (e.HasAttribute("count"))
			{
				this.MaxCount = StringParsers.ParseSInt32(e.GetAttribute("count"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("show_requirements"))
			{
				this.ShowRequirements = StringParsers.ParseBool(e.GetAttribute("show_requirements"), 0, -1, true);
			}
			if (e.HasAttribute("biome"))
			{
				this.Biome = e.GetAttribute("biome");
			}
		}

		// Token: 0x0600C539 RID: 50489 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Update(float deltaTime)
		{
		}

		// Token: 0x0600C53A RID: 50490 RVA: 0x0048D08D File Offset: 0x0048B28D
		public void HandleUpdate(float deltaTime)
		{
			if (this.Owner.IsActive)
			{
				this.Update(deltaTime);
			}
		}

		// Token: 0x0600C53B RID: 50491 RVA: 0x0048D0A3 File Offset: 0x0048B2A3
		public virtual void CompleteObjective(bool handleComplete = true)
		{
			this.Current = this.MaxCount;
			this.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0400956C RID: 38252
		public static byte FileVersion = 1;

		// Token: 0x0400956E RID: 38254
		public int MaxCount = 1;

		// Token: 0x0400956F RID: 38255
		public bool IsRequirement;

		// Token: 0x04009570 RID: 38256
		public Challenge Owner;

		// Token: 0x04009571 RID: 38257
		public ChallengeClass OwnerClass;

		// Token: 0x04009572 RID: 38258
		public string Biome = "";

		// Token: 0x04009573 RID: 38259
		public bool ShowRequirements = true;

		// Token: 0x04009574 RID: 38260
		[PublicizedFrom(EAccessModifier.Private)]
		public bool complete;

		// Token: 0x04009575 RID: 38261
		public bool IsTracking;

		// Token: 0x04009576 RID: 38262
		[PublicizedFrom(EAccessModifier.Protected)]
		public int current;
	}
}
