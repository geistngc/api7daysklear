using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001914 RID: 6420
	[Preserve]
	public class ChallengeObjectiveTrader : BaseChallengeObjective
	{
		// Token: 0x1700188D RID: 6285
		// (get) Token: 0x0600C64A RID: 50762 RVA: 0x001B2E28 File Offset: 0x001B1028
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Trader;
			}
		}

		// Token: 0x1700188E RID: 6286
		// (get) Token: 0x0600C64B RID: 50763 RVA: 0x00491154 File Offset: 0x0048F354
		public override string DescriptionText
		{
			get
			{
				if (string.IsNullOrEmpty(this.TraderName))
				{
					if (!this.BuyItems)
					{
						return Localization.Get("challengeObjectiveSellItems", false, null);
					}
					return Localization.Get("challengeObjectiveBuyItems", false, null);
				}
				else
				{
					if (!this.BuyItems)
					{
						return string.Format(Localization.Get("challengeObjectiveSellItemsTo", false, null), Localization.Get(this.TraderName, false, null));
					}
					return string.Format(Localization.Get("challengeObjectiveBuyItemsFrom", false, null), Localization.Get(this.TraderName, false, null));
				}
			}
		}

		// Token: 0x0600C64C RID: 50764 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C64D RID: 50765 RVA: 0x004911D5 File Offset: 0x0048F3D5
		public override void HandleAddHooks()
		{
			if (this.BuyItems)
			{
				QuestEventManager.Current.BuyItems += this.Current_BuyItems;
				return;
			}
			QuestEventManager.Current.SellItems += this.Current_SellItems;
		}

		// Token: 0x0600C64E RID: 50766 RVA: 0x0049120C File Offset: 0x0048F40C
		public override void HandleRemoveHooks()
		{
			if (this.BuyItems)
			{
				QuestEventManager.Current.BuyItems -= this.Current_BuyItems;
				return;
			}
			QuestEventManager.Current.SellItems -= this.Current_SellItems;
		}

		// Token: 0x0600C64F RID: 50767 RVA: 0x00491244 File Offset: 0x0048F444
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BuyItems(string traderName, int itemCounts)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || traderName == this.TraderName)
			{
				base.Current += itemCounts;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C650 RID: 50768 RVA: 0x004912AC File Offset: 0x0048F4AC
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_SellItems(string traderName, int itemCounts)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || traderName == this.TraderName)
			{
				base.Current += itemCounts;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C651 RID: 50769 RVA: 0x00491314 File Offset: 0x0048F514
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("is_buy"))
			{
				this.BuyItems = StringParsers.ParseBool(e.GetAttribute("is_buy"), 0, -1, true);
			}
			if (e.HasAttribute("trader_name"))
			{
				this.TraderName = e.GetAttribute("trader_name");
			}
		}

		// Token: 0x0600C652 RID: 50770 RVA: 0x00491380 File Offset: 0x0048F580
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveTrader
			{
				BuyItems = this.BuyItems,
				TraderName = this.TraderName
			};
		}

		// Token: 0x040095CE RID: 38350
		[PublicizedFrom(EAccessModifier.Private)]
		public bool BuyItems;

		// Token: 0x040095CF RID: 38351
		[PublicizedFrom(EAccessModifier.Private)]
		public string TraderName = "";
	}
}
